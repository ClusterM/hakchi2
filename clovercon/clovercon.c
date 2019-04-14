/* --------------------------------------------------------------------------
 * @license_begin
 *
 *  Controllers I2C driver
 *  Copyright (C) 2016  Nintendo Co. Ltd
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License along
 *  with this program; if not, write to the Free Software Foundation, Inc.,
 *  51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 * @license_end
 * ----------------------------------------------------------------------- */

#include <linux/module.h>
#include <linux/kernel.h>
#include <linux/init.h>
#include <linux/errno.h>
#include <linux/i2c.h>
#include <linux/input.h>
#include <linux/input-polldev.h>
#include <linux/delay.h>
#include <linux/gpio.h>
#include <linux/interrupt.h>
#include <linux/irq.h>
#include <linux/workqueue.h>
#include <linux/mutex.h>
#include <linux/bug.h>
#include <linux/miscdevice.h>

static unsigned short home_combination = 0xffff;
static unsigned short autofire = 0;
static unsigned short autofire_xy = 0;
static unsigned short autofire_interval = 8;
static unsigned short fc_start = 0;

MODULE_AUTHOR("Christophe Aguettaz <christophe.aguettaz@nerd.nintendo.com>, mod by Cluster <clusterrr@clusterrr.com");
MODULE_DESCRIPTION("Nintendo Clover/Wii Classic/Wii Pro controllers on I2C");

MODULE_LICENSE("GPL");

#define DRV_NAME "clovercon"

#define CLASSIC_ID             0
#define CONTROLLER_I2C_ADDRESS 0x52
//Rounded up to multiple of 1/HZ
#define POLL_INTERVAL          5

/* HOME button is to be reported only after these many successful polling
 * positives.
 * Keep it at ~0.1s, adapt whenever POLL_INTERVAL is changed */
#define HOME_BUTTON_THRESHOLD 15
#define RESET_COMBINATION_THRESHOLD 50
#define AUTOFIRE_COMBINATION_THRESHOLD 150
#define START_COMBINATION_THRESHOLD 150

//Delay expressed in polling intervals
#define RETRY_BASE_DELAY 512

#define D_BTN_R      1
#define D_BTN_START  2
#define D_BTN_HOME   3
#define D_BTN_SELECT 4
#define D_BTN_L      5
#define D_BTN_DOWN   6
#define D_BTN_RIGHT  7

#define D_BTN_UP     0
#define D_BTN_LEFT   1
#define D_BTN_ZR     2
#define D_BTN_X      3
#define D_BTN_A      4
#define D_BTN_Y      5
#define D_BTN_B      6
#define D_BTN_ZL     7

#define DEAD_ZONE      20
#define STICK_MAX      72
#define STICK_FUZZ     4

#define ACC_DEAD_ZONE      15
#define ACC_TRIGGER_ZONE   40

#define TRIGGER_MIN    0
#define TRIGGER_MAX    0xff
#define TRIGGER_FUZZ   4

#define MAX_CON_COUNT  4

#define MIN(X, Y) ((X) < (Y) ? (X) : (Y))
#define MAX(X, Y) ((X) > (Y) ? (X) : (Y))

#define CLOVERCON_DETECT_USE_IRQ 0

#if CLOVERCON_DETECT_USE_IRQ
#define INVAL_IRQ            -1
#define DETECT_DELAY         (HZ / 10)
#else
#define DETECT_DELAY         (HZ / 5)
#endif

#define DEBOUNCE_VALUE       0x71

static struct delayed_work detect_work;

static DEFINE_MUTEX(con_state_lock);
static DEFINE_MUTEX(detect_task_lock);

#define VERBOSITY        1
#define STATE_DEVICES    1

#if VERBOSITY > 0
    #define ERR(m, ...) printk(KERN_ERR m, ##__VA_ARGS__)
    #define INF(m, ...) printk(KERN_INFO m, ##__VA_ARGS__)
#else
    #define ERR(m, ...)
    #define INF(m, ...)
#endif

#if VERBOSITY > 1
    #define DBG(m, ...) printk(KERN_DEBUG m, ##__VA_ARGS__)
    #define FAST_ERR(m, ...) ERR(m, ##__VA_ARGS__)
    #define FAST_DBG(m, ...) DBG(m, ##__VA_ARGS__)
#else
    #define DBG(m, ...)
    #define FAST_ERR(m, ...)
    #define FAST_DBG(m, ...)
#endif

#if VERBOSITY > 2
    #define HEXDUMP(prefix, data, len) print_hex_dump(KERN_DEBUG, prefix, DUMP_PREFIX_NONE, 16, 256, data, len, true)
#else
    #define HEXDUMP(prefix, data, len)
#endif

#if (VERBOSITY > 0) || STATE_DEVICES
static ssize_t device_dumb_write(struct file *fp, const char __user *buf,
    size_t count, loff_t *pos)
{
    return count; // a-la /dev/null
}

static long device_dumb_ioctl(struct file *fp, unsigned code, unsigned long value)
{
    long ret = 0;
    switch (code) {
        default:
        ret = -EINVAL;
        break;
    }

    return ret;
}

static int device_dumb_open(struct inode *ip, struct file *fp)
{
    return 0;
}

static int device_dumb_release(struct inode *ip, struct file *fp)
{
    return 0;
}
#endif

#if STATE_DEVICES
static ssize_t clovercon_state_read(struct file *fp, char __user *buf,
    size_t count, loff_t *pos);

/* file operations for /dev/clovercon* */
static const struct file_operations clovercon_state_fops = {
    .owner = THIS_MODULE,
    .read = clovercon_state_read,
    .write = device_dumb_write,
    .unlocked_ioctl = device_dumb_ioctl,
    .open = device_dumb_open,
    .release = device_dumb_release,
};
#endif

enum ControllerState {
    CS_OK,
    CS_RETRY_1,
    CS_RETRY_2,
    CS_ERR
};

static bool get_bit(u8 data, int bitnum) {
    return (data & (1 << bitnum)) >> bitnum;
}

static const struct i2c_device_id clovercon_idtable[] = {
    { "classic", CLASSIC_ID },
    {}
};

MODULE_DEVICE_TABLE(i2c, clovercon_idtable);

struct clovercon_info {
    struct input_polled_dev *dev;
    struct i2c_client *client;
    struct i2c_adapter *adapter;
#if CLOVERCON_DETECT_USE_IRQ
    int irq;
#endif
    int detection_active;
    int gpio;
    int id;
    enum ControllerState state;
    u16 retry_counter;
    int home_counter;
    int reset_counter;
    int autofire_timer;
    int autofire_counter_a;
    int autofire_counter_b;
    int autofire_counter_x;
    int autofire_counter_y;
    bool autofire_a;
    bool autofire_b;
    bool autofire_x;
    bool autofire_y;
    int start_counter;
    u8 data_format;
    u16 buttons_state;
#if STATE_DEVICES
    struct miscdevice state_device;
    char state_device_name[32];
#endif
};

static struct clovercon_info con_info_list[MAX_CON_COUNT];
static int module_params[2 * MAX_CON_COUNT];
static int arr_argc = 0;

#define CON_NAME_PREFIX "Nintendo Clovercon - controller"
const char *controller_names[] = {CON_NAME_PREFIX"1", CON_NAME_PREFIX"2",
                                  CON_NAME_PREFIX"3", CON_NAME_PREFIX"4"};

module_param_array(module_params, int, &arr_argc, S_IRUSR | S_IRGRP | S_IROTH);
MODULE_PARM_DESC(module_params, "Input info in the form con0_i2c_bus, con0_detect_gpio, "
                                "form con1_i2c_bus, con1_detect_gpio, ... gpio < 0 means no detection");
module_param(home_combination, short, S_IRUSR | S_IWUSR | S_IRGRP | S_IROTH);
MODULE_PARM_DESC(home_combination, "Button combination to open menu "
                "(0x001=A,0x002=B,0x004=Select,0x008=Start,0x010=Up,0x020=Down,0x040=Left,0x080=Right,0x100=X,0x200=Y,0x400=L,0x800=R");
module_param(autofire, short, S_IRUSR | S_IWUSR | S_IRGRP | S_IROTH);
MODULE_PARM_DESC(autofire, "Enable autofire (hold select+a / select+b for a second)");
module_param(autofire_xy, short, S_IRUSR | S_IWUSR | S_IRGRP | S_IROTH);
MODULE_PARM_DESC(autofire_xy, "Use X/Y on classic controller as autofire A/B");
module_param(autofire_interval, short, S_IRUSR | S_IWUSR | S_IRGRP | S_IROTH);
MODULE_PARM_DESC(autofire_interval, "Autofire interval (default is 8)");
module_param(fc_start, short, S_IRUSR | S_IWUSR | S_IRGRP | S_IROTH);
MODULE_PARM_DESC(fc_start, "Enable start button emulation for second controller");

#if CLOVERCON_DETECT_USE_IRQ
struct clovercon_info * clovercon_info_from_irq(int irq) {
    int i;
    for (i = 0; i < MAX_CON_COUNT; i++) {
        if (con_info_list[i].irq == irq) {
            return &con_info_list[i];
        }
    }
    return NULL;
}
#endif

#if STATE_DEVICES
static ssize_t clovercon_state_read(struct file *fp, char __user *buf,
    size_t count, loff_t *pos)
{
    // which contoller? we need device file name
    char path_buffer[64];
    char state_buffer[16];
    int id;
    size_t l;
    u16 state;

    char* path = dentry_path_raw(fp->f_path.dentry,path_buffer,sizeof(path_buffer));
    while (*path && *(path+1)) path++; // moving to last character which is number
    if (!*path) return 0;
    id = *path - '0';

    state = con_info_list[id-1].buttons_state;
    snprintf(state_buffer, sizeof(state_buffer), "%02X%02X", state & 0xFF, (state >> 8) & 0xFF);
    l = MAX(MIN(count, strlen(state_buffer) - *pos), 0);
    memcpy(buf, state_buffer + *pos, l);
    if (l > 0) *pos += l;
    return l;
}
#endif

struct clovercon_info * clovercon_info_from_adapter(struct i2c_adapter *adapter) {
    int i;
    for (i = 0; i < MAX_CON_COUNT; i++) {
        if (con_info_list[i].adapter == adapter) {
            return &con_info_list[i];
        }
    }
    return NULL;
}

static int clovercon_write(struct i2c_client *client, u8 *data, size_t count) {
    struct i2c_msg msg = {
        .addr = client->addr,
        .len = count,
        .buf = data
    };
    int ret;

    ret = i2c_transfer(client->adapter, &msg, 1);
    if (ret < 0) {
        //read and write errors are DBG only to account for normal
        //errors coming from disconnects
        DBG("write failed with error %i", ret);
        return ret;
    } else if (ret != 1) {
        DBG("incomplete write, return value: %i", ret);
        return -EIO;
    }

    return 0;
}

static int clovercon_prepare_read(struct i2c_client *client, u8 address) {
    int ret;

    ret = clovercon_write(client, &address, 1);
    if (ret) {
        DBG("prepare_read failed");
        return ret;
    }

    return 0;
}

static int clovercon_read(struct i2c_client *client, u8 address, u8 *values, size_t count) {
    struct i2c_msg data_msg = {
        .addr = client->addr,
        .flags = I2C_M_RD,
        .len = count,
        .buf = values
    };
    int ret;

    ret = clovercon_prepare_read(client, address);
    if (ret)
        return ret;

    // Wait > 150µs before prepare_read and actual read
    usleep_range(150, 200);

    ret = i2c_transfer(client->adapter, &data_msg, 1);
    if (ret < 0) {
        DBG("read failed with error %i", ret);
        return ret;
    } else if (ret != 1) {
        DBG("incomplete read, return value: %i", ret);
        return -EIO;
    }

    return 0;
}

static int clovercon_read_controller_info(struct i2c_client *client, u8 *data, size_t len) {
    int ret;

    len = MIN(len, 0xff -0xfa + 1);
    ret = clovercon_read(client, 0xfa, data, len);
    if (ret)
        return ret;

    return len;
}

static int clovercon_setup(struct clovercon_info *info) {
    struct i2c_client *client = info->client;
    u8 init_data[] = { 0xf0, 0x55, 0xfb, 0x00, 0xfe, 3 };
    static const int CON_INFO_LEN = 6;
    u8 con_info_data[CON_INFO_LEN];
    int ret;
#if VERBOSITY > 2
    u8 debug_data[32];
    s16 debug_pos;
#endif
    DBG("Clovercon setup");

#if VERBOSITY > 3
    ret = clovercon_read_controller_info(client, con_info_data, CON_INFO_LEN);
    if (ret < 0) {
        ERR("error reading controller info");
        goto err;
    }
    HEXDUMP("con_info_data before setup: ", con_info_data, ret);
#endif

    ret = clovercon_write(client, &init_data[0], 2);
    if (ret)
        goto err;
    ret = clovercon_write(client, &init_data[2], 2);
    if (ret)
        goto err;
    // trying to set data format to 3
    ret = clovercon_write(client, &init_data[4], 2);

    ret = clovercon_read_controller_info(client, con_info_data, CON_INFO_LEN);
    if (ret < 0) {
        ERR("error reading controller info");
        goto err;
    } else if (ret != CON_INFO_LEN) {
        ERR("wrong read length %i reading controller info", ret);
        ret = -EIO;
        goto err;
    }
    HEXDUMP("con_info_data after setup: ", con_info_data, sizeof(con_info_data));

#if VERBOSITY > 2
    for (debug_pos = 0; debug_pos < 256; debug_pos += sizeof(debug_data))
    {
        ret = clovercon_read(client, debug_pos, debug_data, sizeof(debug_data));
        if (ret)
        {
            ERR("read failed for active controller - possible controller disconnect");
            ret = -EIO;
            goto err;
        }
        HEXDUMP("clvcon dump: ", debug_data, sizeof(debug_data));
    }
#endif

    // autodetecting data format
    // it should be 0x03 for original classic controllers and clovercons
    // but seems like not every 3rd party controller supports data format selection
    info->data_format = con_info_data[4];

    // no, i want to support everything! /Cluster
    /*
    if (con_info_data[5] != 1) {
        ERR("unsupported controller id %i", (int)con_info_data[5]);
        ret = -EIO;
        goto err;
    }
    */
    return 0;

err:
    ERR("controller setup failed with error %i", -ret);
    return ret;
}

static void clamp_stick(int *px, int *py) {
    int x_sign = 1;
    int y_sign = 1;
    int x = *px;
    int y = *py;
    //int norm;

    if (x < 0) {
        x_sign = -1;
        x = -x;
    }
    if (y < 0) {
        y_sign = -1;
        y = -y;
    }

    x = MAX(0, x - DEAD_ZONE);
    y = MAX(0, y - DEAD_ZONE);

/*
    if (x == 0 && y == 0) {
        goto clamp_end;
    }

    norm = (y <= x) ? (DIAG_MAX * x + (STICK_MAX - DIAG_MAX) * y) :
                      (DIAG_MAX * y + (STICK_MAX - DIAG_MAX) * x);

    if (DIAG_MAX * STICK_MAX < norm) {
        x = DIAG_MAX * STICK_MAX * x / norm;
        y = DIAG_MAX * STICK_MAX * y / norm;
    }
clamp_end:
*/
    *px = x * x_sign;
    *py = y * y_sign;
}

static void clovercon_poll(struct input_polled_dev *polled_dev) {
    struct clovercon_info *info = polled_dev->private;
    static const int READ_LEN = 21;
    u8 data[READ_LEN];
    int jx, jy, rx, ry, tl, tr, ax, ay, az;
    bool left, right, up, down, a, b, x, y, select, start, home, l, r, zl, zr, reset;
    u16 retry_delay = RETRY_BASE_DELAY;
    u16 buttons_state;
    int ret;
    bool turbo;

    switch (info->state) {
    case CS_OK:
        ret = clovercon_read(info->client, 0, data, READ_LEN);
        if (ret) {
            DBG("read failed for active controller - possible controller disconnect");
            INF("moving controller %i to error state", info->id);
            info->state = CS_RETRY_1;
            break;
        }

        #if VERBOSITY > 4
            HEXDUMP("clvcon polling: ", data, READ_LEN);
        #endif

        /* The Wii could trust the payload 100% because the Wii Remote
         * controller is powered with AA batteries. In the case of
         * CLOVER high voltage on DC power supply has shown very noisy
         * payloads coming out of the I²C bus. But we don't have any
         * checksum or parity bits to discard such corrupted payloads.
         *
         * However the payload zero padding should be impacted by the
         * DC noise and some high bits should pop up there too.
         *
         * Use that as last resort discarding criteria */
        jy = 0;
        data[18] = 0;   // for ultra shitty pro controller clones
                // which will work only after hardware
                // modification (2.2K pull-up resistor on SCL and SDA)
        for (jx=10; jx<21; jx++) {
            if (data[jx] == 0xFF) data[jx] = 0; // for 3rd party controllers
            jy += data[jx];
        }
        if (jy) {
            /* noise, discard payload */
            break;
        }

        jx=0, jy=0, rx=0, ry=0, tl=0, tr=0,
            left=0, right=0, up=0, down=0, a=0, b=0, x=0, y=0, select=0, 
            start=0, home=0, l=0, r=0, zl=0, zr=0;
        switch (info->data_format)
        {
            case 0: // Nunchuck! So one-handed people can play too. Seriosly.
                jx = data[0] - 0x80;
                jy = 0x7fl - data[1];
                ax = data[2] - 0x80;
                ay = data[3] - 0x80;
                az = data[4] - 0x80;
                a = !get_bit(data[5], 0);
                b = !get_bit(data[5], 1);
		        up = jy < -DEAD_ZONE;
		        down = jy > DEAD_ZONE;
		        left = jx < -DEAD_ZONE;
		        right = jx > DEAD_ZONE;
                // Tilt nunchuck left = select
                if (!get_bit(info->buttons_state, 2)) // Select not pressed yet
                {
                    select = (ax <= -ACC_TRIGGER_ZONE) && (ay < ACC_DEAD_ZONE) && (ay > -ACC_DEAD_ZONE) && (az < ACC_DEAD_ZONE) && (az > -ACC_DEAD_ZONE);
                } else { // Already pressed
                    select = ax <= -ACC_DEAD_ZONE;
                }
                // Tilt nunchuck right = start
                start = (ax >= ACC_TRIGGER_ZONE) && (ay < ACC_DEAD_ZONE) && (ay > -ACC_DEAD_ZONE) && (az < ACC_DEAD_ZONE) && (az > -ACC_DEAD_ZONE);
                if (!get_bit(info->buttons_state, 3)) // Start not pressed yet
                {
                    start = (ax >= ACC_TRIGGER_ZONE) && (ay < ACC_DEAD_ZONE) && (ay > -ACC_DEAD_ZONE) && (az < ACC_DEAD_ZONE) && (az > -ACC_DEAD_ZONE);
                } else { // Already pressed
                    start = ax >= ACC_DEAD_ZONE;
                }
                // Tilt nunchuck upside down = home
                home = (az <= -ACC_TRIGGER_ZONE);
                break;
            case 1:
                jx = ((data[0] & 0x3f) - 0x20) * 4;
                rx = (((data[2] >> 7) | ((data[1] & 0xC0) >> 5) | ((data[0] & 0xC0) >> 3)) - 0x10) * 8;
                jy = ((data[1] & 0x3f) - 0x20) * -4;
                ry = ((data[2] & 0x1f) - 0x10) * -8;
                tl = ((data[3] >> 5) | ((data[2] & 0x60) >> 2)) * 8 - 0x80;
                tr = (data[3] & 0x1f) * 8 - 0x80;
                r      = !get_bit(data[4], D_BTN_R);
                start  = !get_bit(data[4], D_BTN_START);
                home   = !get_bit(data[4], D_BTN_HOME);
                select = !get_bit(data[4], D_BTN_SELECT);
                l      = !get_bit(data[4], D_BTN_L);
                down   = !get_bit(data[4], D_BTN_DOWN);
                right  = !get_bit(data[4], D_BTN_RIGHT);
                up   = !get_bit(data[5], D_BTN_UP);
                left = !get_bit(data[5], D_BTN_LEFT);
                zr   = !get_bit(data[5], D_BTN_ZR);
                x    = !get_bit(data[5], D_BTN_X);
                y    = !get_bit(data[5], D_BTN_Y);
                a    = !get_bit(data[5], D_BTN_A);
                b    = !get_bit(data[5], D_BTN_B);
                zl   = !get_bit(data[5], D_BTN_ZL);
                break;
            case 2:
                jx = data[0] - 0x80;
                rx = data[1] - 0x80;
                jy = 0x7fl - data[2];
                ry = 0x7fl - data[3];
                //wtf = data[4]; // What is it?
                tl = data[5] - 0x80;
                tr = data[6] - 0x80;
                r      = !get_bit(data[7], D_BTN_R);
                start  = !get_bit(data[7], D_BTN_START);
                home   = !get_bit(data[7], D_BTN_HOME);
                select = !get_bit(data[7], D_BTN_SELECT);
                l      = !get_bit(data[7], D_BTN_L);
                down   = !get_bit(data[7], D_BTN_DOWN);
                right  = !get_bit(data[7], D_BTN_RIGHT);
                up   = !get_bit(data[8], D_BTN_UP);
                left = !get_bit(data[8], D_BTN_LEFT);
                zr   = !get_bit(data[8], D_BTN_ZR);
                x    = !get_bit(data[8], D_BTN_X);
                y    = !get_bit(data[8], D_BTN_Y);
                a    = !get_bit(data[8], D_BTN_A);
                b    = !get_bit(data[8], D_BTN_B);
                zl   = !get_bit(data[8], D_BTN_ZL);
                break;
            case 3:
                jx = data[0] - 0x80;
                rx = data[1] - 0x80;
                jy = 0x7fl - data[2];
                ry = 0x7fl - data[3];
                tl = data[4];
                tr = data[5];
                r      = !get_bit(data[6], D_BTN_R);
                start  = !get_bit(data[6], D_BTN_START);
                home   = !get_bit(data[6], D_BTN_HOME);
                select = !get_bit(data[6], D_BTN_SELECT);
                l      = !get_bit(data[6], D_BTN_L);
                down   = !get_bit(data[6], D_BTN_DOWN);
                right  = !get_bit(data[6], D_BTN_RIGHT);
                up   = !get_bit(data[7], D_BTN_UP);
                left = !get_bit(data[7], D_BTN_LEFT);
                zr   = !get_bit(data[7], D_BTN_ZR);
                x    = !get_bit(data[7], D_BTN_X);
                y    = !get_bit(data[7], D_BTN_Y);
                a    = !get_bit(data[7], D_BTN_A);
                b    = !get_bit(data[7], D_BTN_B);
                zl   = !get_bit(data[7], D_BTN_ZL);
        }

        // Bitmask for current controller state
        buttons_state = 0;
        if (a) buttons_state |= (1 << 0);
        if (b) buttons_state |= (1 << 1);
        if (select) buttons_state |= (1 << 2);
        if (start) buttons_state |= (1 << 3);
        if (up || (jy < -DEAD_ZONE)) buttons_state |= (1 << 4);
        if (down || (jy > DEAD_ZONE)) buttons_state |= (1 << 5);
        if (left || (jx < -DEAD_ZONE)) buttons_state |= (1 << 6);
        if (right || (jx > DEAD_ZONE)) buttons_state |= (1 << 7);
        if (x) buttons_state |= (1 << 8);
        if (y) buttons_state |= (1 << 9);
        if (l) buttons_state |= (1 << 10);
        if (r) buttons_state |= (1 << 11);
        if (zl) buttons_state |= (1 << 12);
        if (zr) buttons_state |= (1 << 13);
        info->buttons_state = buttons_state;

        // Reset combination
        reset = home_combination == buttons_state;

        // Start button workaroud for second controller on Famicom
        if (fc_start && info->id == 2)
        {
            if (a && !select && b && !start && up && !down && !left && !right)
            info->start_counter++;
            else
            info->start_counter = 0;
            if (info->start_counter >= START_COMBINATION_THRESHOLD)
            start = 1;
        }

        // Autofire
        info->autofire_timer++;
        if (info->autofire_timer >= autofire_interval*2)
        info->autofire_timer = 0;
        turbo = info->autofire_timer / autofire_interval;
        if (autofire)
        {
            if (select && a && !b && !x && !y && !start && !up && !down && !left && !right)
            info->autofire_counter_a++;
            else
            info->autofire_counter_a = 0;
            if (select && !a && b && !x && !y && !start && !up && !down && !left && !right)
            info->autofire_counter_b++;
            else
            info->autofire_counter_b = 0;
            if (select && !a && !b && x && !y && !start && !up && !down && !left && !right)
            info->autofire_counter_x++;
            else
            info->autofire_counter_x = 0;
            if (select && !a && !b && !x && y && !start && !up && !down && !left && !right)
            info->autofire_counter_y++;
            else
            info->autofire_counter_y = 0;

            if (info->autofire_counter_a == AUTOFIRE_COMBINATION_THRESHOLD)
            info->autofire_a = !info->autofire_a;
            if (info->autofire_counter_b == AUTOFIRE_COMBINATION_THRESHOLD)
            info->autofire_b = !info->autofire_b;
            if (info->autofire_counter_x == AUTOFIRE_COMBINATION_THRESHOLD)
            info->autofire_x = !info->autofire_x;
            if (info->autofire_counter_y == AUTOFIRE_COMBINATION_THRESHOLD)
            info->autofire_y = !info->autofire_y;

            if (info->autofire_a && !turbo) a = 0;
            if (info->autofire_b && !turbo) b = 0;
            if (info->autofire_x && !turbo) x = 0;
            if (info->autofire_y && !turbo) y = 0;
        }
        if (autofire_xy)
        {
            // X and Y on classic controller now are autofire A and B
            if (x && turbo) a = 1;
            if (y && turbo) b = 1;
            x = y = 0;
        }

        /* When DC noise is kicking in, we want to avoid the emulator
         * switching to the menu supriously because of noisy home button
         * events.
         *
         * Note that even the Mini NES controller can report the event
         * even though no physical button is present. The MCU is the
         * same as the classic/pro controller. */
        if (home) {
            info->home_counter++;
            if (info->home_counter>HOME_BUTTON_THRESHOLD) {
                info->home_counter = HOME_BUTTON_THRESHOLD;
            }
        } else {
            info->home_counter = 0;
        }
        if (reset) {
            info->reset_counter++;
            if (info->reset_counter>RESET_COMBINATION_THRESHOLD) {
                info->reset_counter = RESET_COMBINATION_THRESHOLD;
            }
        } else {
            info->reset_counter = 0;
        }

        clamp_stick(&jx, &jy);
        clamp_stick(&rx, &ry);

        input_report_abs(polled_dev->input, ABS_X, jx);
        input_report_abs(polled_dev->input, ABS_Y, jy);
        input_report_abs(polled_dev->input, ABS_RX, rx);
        input_report_abs(polled_dev->input, ABS_RY, ry);
        input_report_abs(polled_dev->input, ABS_Z, tl);
        input_report_abs(polled_dev->input, ABS_RZ, tr);
        input_report_key(polled_dev->input, BTN_TR,     r);
        input_report_key(polled_dev->input, BTN_START,  start);
        input_report_key(polled_dev->input, BTN_MODE,   (info->home_counter>=HOME_BUTTON_THRESHOLD) || (info->reset_counter>=RESET_COMBINATION_THRESHOLD));
        input_report_key(polled_dev->input, BTN_SELECT, select);
        input_report_key(polled_dev->input, BTN_TL,     l);
        input_report_key(polled_dev->input, BTN_TRIGGER_HAPPY4,  down);
        input_report_key(polled_dev->input, BTN_TRIGGER_HAPPY2,  right);
        input_report_key(polled_dev->input, BTN_TRIGGER_HAPPY3,  up);
        input_report_key(polled_dev->input, BTN_TRIGGER_HAPPY1,  left);
        input_report_key(polled_dev->input, BTN_TR2,    zr);
        input_report_key(polled_dev->input, BTN_X,      x);
        input_report_key(polled_dev->input, BTN_A,      a);
        input_report_key(polled_dev->input, BTN_Y,      y);
        input_report_key(polled_dev->input, BTN_B,      b);
        input_report_key(polled_dev->input, BTN_TL2,    zl);

        input_sync(polled_dev->input);

        break;
    case CS_RETRY_1:
        retry_delay /= 2;
        //fall-through
    case CS_RETRY_2:
        retry_delay /= 2;
        //fall-through
    case CS_ERR:
        info->retry_counter++;
        if (info->retry_counter == retry_delay) {
            DBG("retrying controller setup");
            ret = clovercon_setup(info);
            if (ret) {
                info->state = MIN(CS_ERR, info->state + 1);
            } else {
                info->state = CS_OK;
                INF("setup succeeded for controller %i, moving to OK state", info->id);
            }
            info->retry_counter = 0;
        }
        break;
    default:
        info->state = CS_ERR;
    }
}

static void clovercon_open(struct input_polled_dev *polled_dev) {
    struct clovercon_info *info = polled_dev->private;
    if (clovercon_setup(info)) {
        info->retry_counter = 0;
        info->state = CS_RETRY_1;
        INF("opened controller %i, controller in error state after failed setup", info->id);
    } else {
        info->state = CS_OK;
        info->home_counter = 0;
        info->reset_counter = 0;
        INF("opened controller %i, controller in OK state", info->id);
    }
}

static int clovercon_probe(struct i2c_client *client, const struct i2c_device_id *id) {
    struct clovercon_info *info;
    struct input_polled_dev *polled_dev;
    struct input_dev *input_dev;
    int ret = 0;

    switch (id->driver_data) {
    case CLASSIC_ID:
        DBG("probing classic controller");
        break;
    default:
        ERR("unknown id: %lu\n", id->driver_data);
        return -EINVAL;
    }

    mutex_lock(&con_state_lock);
    info = clovercon_info_from_adapter(client->adapter);
    if (!info) {
        ERR("unkonwn client passed to probe");
        mutex_unlock(&con_state_lock);
        return -EINVAL;
    }
    info->client = client;
    i2c_set_clientdata(client, info);
    mutex_unlock(&con_state_lock);

    polled_dev = input_allocate_polled_device();
    if (!polled_dev) {
        ERR("error allocating polled device");
        return -ENOMEM;
    }

    info->dev = polled_dev;

    polled_dev->poll_interval = POLL_INTERVAL;
    polled_dev->poll = clovercon_poll;
    polled_dev->open = clovercon_open;
    polled_dev->private = info;

    input_dev = polled_dev->input;

    //change controller_names initializer when changing MAX_CON_COUNT
    BUILD_BUG_ON(MAX_CON_COUNT != ARRAY_SIZE(controller_names)); 
    input_dev->name = controller_names[info->id - 1];
    input_dev->phys = DRV_NAME"/clovercon";
    input_dev->id.bustype = BUS_I2C;
    input_dev->dev.parent = &client->dev;

    set_bit(EV_ABS, input_dev->evbit);
    set_bit(ABS_X,  input_dev->absbit);
    set_bit(ABS_Y,  input_dev->absbit);
    set_bit(ABS_RX, input_dev->absbit);
    set_bit(ABS_RY, input_dev->absbit);
    /*
    L/R are analog on the classic controller, digital
    on the pro with values 0 - 0xf8
    */
    set_bit(ABS_Z,  input_dev->absbit);
    set_bit(ABS_RZ, input_dev->absbit);

    set_bit(EV_KEY,     input_dev->evbit);
    set_bit(BTN_X,      input_dev->keybit);
    set_bit(BTN_B,      input_dev->keybit);
    set_bit(BTN_A,      input_dev->keybit);
    set_bit(BTN_Y,      input_dev->keybit);
    set_bit(BTN_TRIGGER_HAPPY3, input_dev->keybit); // up
    set_bit(BTN_TRIGGER_HAPPY4, input_dev->keybit); // down
    set_bit(BTN_TRIGGER_HAPPY2, input_dev->keybit); // right
    set_bit(BTN_TRIGGER_HAPPY1, input_dev->keybit); // left
    set_bit(BTN_TR,     input_dev->keybit);
    set_bit(BTN_TL,     input_dev->keybit);
    set_bit(BTN_TR2,    input_dev->keybit);
    set_bit(BTN_TL2,    input_dev->keybit);
    set_bit(BTN_SELECT, input_dev->keybit);
    set_bit(BTN_START,  input_dev->keybit);
    set_bit(BTN_MODE,   input_dev->keybit);

    input_set_abs_params(input_dev, ABS_X,  -STICK_MAX, STICK_MAX, STICK_FUZZ, 0);
    input_set_abs_params(input_dev, ABS_Y,  -STICK_MAX, STICK_MAX, STICK_FUZZ, 0);
    input_set_abs_params(input_dev, ABS_RX, -STICK_MAX, STICK_MAX, STICK_FUZZ, 0);
    input_set_abs_params(input_dev, ABS_RY, -STICK_MAX, STICK_MAX, STICK_FUZZ, 0);
    input_set_abs_params(input_dev, ABS_Z,  TRIGGER_MIN, TRIGGER_MAX, TRIGGER_FUZZ, 0);
    input_set_abs_params(input_dev, ABS_RZ, TRIGGER_MIN, TRIGGER_MAX, TRIGGER_FUZZ, 0);

    ret = input_register_polled_device(polled_dev);
    if (ret) {
        ERR("registering polled device failed");
        goto err_register_polled_dev;
    }

    INF("probed controller %i", info->id);

    return 0;

err_register_polled_dev:
    input_free_polled_device(polled_dev);

    return ret;
}

static int clovercon_remove(struct i2c_client *client) {
    struct clovercon_info *info;
    struct input_polled_dev *polled_dev;
    
    mutex_lock(&con_state_lock);
    info = i2c_get_clientdata(client);
    polled_dev = info->dev;
    info->dev = NULL;
    mutex_unlock(&con_state_lock);

    input_unregister_polled_device(polled_dev);
    input_free_polled_device(polled_dev);

    INF("removed controller %i", info->id);

    return 0;
}

static struct i2c_driver clovercon_driver = {
    .driver = {
        .name   = "clovercon",
        .owner = THIS_MODULE,
    },

    .id_table   = clovercon_idtable,
    .probe      = clovercon_probe,
    .remove     = clovercon_remove,
};

static struct i2c_board_info clovercon_i2c_board_info = {
    I2C_BOARD_INFO("classic", CONTROLLER_I2C_ADDRESS),
};

/* Must be holding con_state_lock */
int clovercon_add_controller(struct clovercon_info *info) {
    struct i2c_client *client;

    mutex_unlock(&con_state_lock);
    client = i2c_new_device(info->adapter, &clovercon_i2c_board_info);
    mutex_lock(&con_state_lock);
    if (!client) {
        ERR("could not create i2c device");
        return -ENOMEM;
    }

#if STATE_DEVICES
    info->state_device.minor = MISC_DYNAMIC_MINOR,
    sprintf(info->state_device_name, "clovercon%d", info->id);
    info->state_device.name = info->state_device_name;
    info->state_device.fops = &clovercon_state_fops,
    misc_register(&info->state_device);
#endif

    INF("added device for controller %i", info->id);
    return 0;
}

/* Must be holding con_state_lock */
void clovercon_remove_controller(struct clovercon_info *info) {
    struct i2c_client *client = info->client;

    mutex_unlock(&con_state_lock);
    i2c_unregister_device(client);
    mutex_lock(&con_state_lock);
    info->client = NULL;
#if STATE_DEVICES
    misc_deregister(&info->state_device);
#endif

    INF("removed device for controller %i", info->id);
}

static void clovercon_remove_controllers(void) {
    int i;

    mutex_lock(&con_state_lock);
    for (i = 0; i < arr_argc / 2; i++) {
        if (!con_info_list[i].client) {
            continue;
        }
        clovercon_remove_controller(&con_info_list[i]);
    }
    mutex_unlock(&con_state_lock);
}

static void clovercon_detect_task(struct work_struct *dummy) {
    struct clovercon_info *info;
    int i;
    int val;

    mutex_lock(&detect_task_lock);
    //DBG("detect task running");
    mutex_lock(&con_state_lock);
    for (i = 0; i < MAX_CON_COUNT; i++) {
        info = &con_info_list[i];
        if (!info->detection_active) {
            continue;
        }
        val = gpio_get_value(info->gpio);
        //DBG("detect pin value: %i", val);
        if (val && !info->client) {
            //DBG("detect task adding controller %i", i);
            clovercon_add_controller(info);
        } else if (!val && info->client) {
            //DBG("detect task removing controller %i", i);
            clovercon_remove_controller(info);
        }
    }
    mutex_unlock(&con_state_lock);
    mutex_unlock(&detect_task_lock);
    //DBG("detect task done");
}

#if CLOVERCON_DETECT_USE_IRQ

static irqreturn_t clovercon_detect_interrupt(int irq, void* dummy) {
    struct clovercon_info *info = clovercon_info_from_irq(irq);
    static int initialized = 0;

    if (info == NULL) {
        FAST_ERR("could not find controller info associated with irq %i", irq);
        return IRQ_HANDLED;
    }

    if (initialized == 0) {
        INIT_DELAYED_WORK(&detect_work, clovercon_detect_task);
        initialized = 1;
    } else {
        PREPARE_DELAYED_WORK(&detect_work, clovercon_detect_task);
    }

    schedule_delayed_work(&detect_work, DETECT_DELAY);

    FAST_DBG("interrupt handler on int %i", irq);
    return IRQ_HANDLED;
}

static int clovercon_setup_irq_detect(struct clovercon_info *info) {
    int irq;
    int ret;

    ret = gpio_to_irq(info->gpio);
    if (ret < 0) {
        ERR("gpio to irq failed");
        return ret;
    } else {
        irq = ret;
        DBG("irq for gpio %i: %i", info->gpio, irq);
    }

    mutex_lock(&con_state_lock);
    info->irq = irq;
    info->detection_active = 1;
    mutex_unlock(&con_state_lock);

    ret = request_irq(ret, clovercon_detect_interrupt, IRQ_TYPE_EDGE_BOTH, "clovercon", NULL);
    if (ret) {
        ERR("failed to request irq");
        return ret;
    }

    return 0;
}

static void clovercon_teardown_irq_detect(struct clovercon_info *info) {
    free_irq(info->irq, NULL);
    mutex_lock(&con_state_lock);
    info->detection_active = 0;
    info->irq = INVAL_IRQ;
    mutex_unlock(&con_state_lock);
}

#else //CLOVERCON_DETECT_USE_IRQ

static void clovercon_detect_timer_task(struct work_struct *dummy) {
    static int initialized = 0;

    if (initialized == 0) {
        INIT_DELAYED_WORK(&detect_work, clovercon_detect_timer_task);
        initialized = 1;
    } else {
        PREPARE_DELAYED_WORK(&detect_work, clovercon_detect_timer_task);
    }

    clovercon_detect_task(NULL);
    schedule_delayed_work(&detect_work, DETECT_DELAY);
}

static int clovercon_setup_timer_detect(struct clovercon_info *info) {
    static int task_running = 0;

    mutex_lock(&con_state_lock);
    info->detection_active = 1;
    mutex_unlock(&con_state_lock);
    
    if (task_running)
        return 0;

    task_running = 1;
    clovercon_detect_timer_task(NULL);

    return 0;
}

static void clovercon_teardown_timer_detect(struct clovercon_info *info) {
    mutex_lock(&con_state_lock);
    info->detection_active = 0;
    mutex_unlock(&con_state_lock);
}

#endif //CLOVERCON_DETECT_USE_IRQ

static int clovercon_setup_i2c(struct clovercon_info *info, int i2c_bus) {
    struct i2c_adapter *adapter;

    adapter = i2c_get_adapter(i2c_bus);
    if (!adapter) {
        ERR("could not access i2c bus %i", i2c_bus);
        return -EINVAL;
    }

    info->adapter = adapter;

    return 0;
}

static int clovercon_setup_detection(struct clovercon_info *info, int gpio_pin) {
    int ret;

    ret = gpio_request(gpio_pin, "clovercon_detect");
    if (ret) {
        ERR("gpio request failed for pin %i", gpio_pin);
        return ret;
    }

    ret = gpio_direction_input(gpio_pin);
    if (ret) {
        ERR("gpio input direction failed");
        goto err_gpio_cleanup;
    }

    info->gpio = gpio_pin;

    ret = gpio_set_debounce(gpio_pin, DEBOUNCE_VALUE);
    if (ret) {
        ERR("failed to debounce gpio %i", gpio_pin);
        goto err_gpio_cleanup;
    }

#if CLOVERCON_DETECT_USE_IRQ
    info->irq = INVAL_IRQ;
    ret = clovercon_setup_irq_detect(info);
#else
    ret = clovercon_setup_timer_detect(info);
#endif

    if (ret) {
        ERR("controller detection setup failed");
        goto err_detect_cleanup;
    }

    return 0;

err_detect_cleanup:
#if CLOVERCON_DETECT_USE_IRQ
    clovercon_teardown_irq_detect(info);
#else
    clovercon_teardown_timer_detect(info);
#endif

err_gpio_cleanup:
    gpio_free(gpio_pin);
    return ret;
}

static void clovercon_teardown_detection(void) {
    int i;
    int gpio;

    cancel_delayed_work_sync(&detect_work);

    for (i = 0; i < MAX_CON_COUNT; i++) {
        if (!con_info_list[i].detection_active) {
            continue;
        }
#if CLOVERCON_DETECT_USE_IRQ
        clovercon_teardown_irq_detect(&con_info_list[i]);
#else
        clovercon_teardown_timer_detect(&con_info_list[i]);
#endif
        mutex_lock(&con_state_lock);
        con_info_list[i].adapter = NULL;
        gpio = con_info_list[i].gpio;
        mutex_unlock(&con_state_lock);

        DBG("Freeing gpio %i", gpio);
        gpio_free(gpio);
    }
}


static int __init clovercon_init(void) {
    int i2c_bus;
    int gpio_pin;
    int ret;
    int i;

    for (i = 0; i < MAX_CON_COUNT; i++) {
        con_info_list[i].detection_active = 0;
        con_info_list[i].id = i + 1;
    }

    for (i = 0; i < arr_argc / 2; i++) {
        i2c_bus = module_params[2 * i];
        gpio_pin = module_params[2 * i + 1];

        DBG("initializing controller %i on bus %i, gpio %i", i, i2c_bus, gpio_pin);

        ret = clovercon_setup_i2c(&con_info_list[i], i2c_bus);
        if (ret) {
            ERR("failed to init controller %i", i);
            goto err_controller_cleanup;
        }

        if (gpio_pin < 0) {
            mutex_lock(&con_state_lock);
            ret = clovercon_add_controller(&con_info_list[i]);
            mutex_unlock(&con_state_lock);
        } else {
            ret = clovercon_setup_detection(&con_info_list[i], gpio_pin);
            if (ret) {
                ERR("failed to init controller %i", i);
                goto err_controller_cleanup;
            }
        }
    }

    ret = i2c_add_driver(&clovercon_driver);
    if (ret) {
        ERR("failed to add driver");
        goto err_controller_cleanup;
    }

    return 0;

err_controller_cleanup:
    clovercon_teardown_detection();
    clovercon_remove_controllers();
    return ret;
}

module_init(clovercon_init);

static void __exit clovercon_exit(void) {
    DBG("exit");

    clovercon_teardown_detection();
    clovercon_remove_controllers();
    i2c_del_driver(&clovercon_driver);
}

module_exit(clovercon_exit);
