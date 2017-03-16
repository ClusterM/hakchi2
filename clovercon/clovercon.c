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

static unsigned short home_combination = 0xffff;
static char autofire = 0;
static char autofire_xy = 0;
static unsigned char autofire_interval = 8;
static char fc_start = 0;

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

#define DATA_FORMAT    1
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

#define VERBOSITY        0

#if VERBOSITY > 0
	#define ERR(m, ...) printk(KERN_ERR  "Clovercon error: " m "\n", ##__VA_ARGS__)
	#define INF(m, ...) printk(KERN_INFO  "Clovercon info: " m "\n", ##__VA_ARGS__)
#else
	#define ERR(m, ...)
	#define INF(m, ...)
#endif

#if VERBOSITY > 1
	#define DBG(m, ...) printk(KERN_DEBUG  "Clovercon: " m "\n", ##__VA_ARGS__)
	#if VERBOSITY > 2
		#define FAST_ERR(m, ...) ERR(m, ##__VA_ARGS__)
		#define FAST_DBG(m, ...) DBG(m, ##__VA_ARGS__)
	#else
		#define FAST_ERR(m, ...) trace_printk(KERN_ERR  "Clovercon error: " m "\n", ##__VA_ARGS__)
		#define FAST_DBG(m, ...) trace_printk(KERN_DEBUG  "Clovercon: " m "\n", ##__VA_ARGS__)
	#endif
#else
	#define DBG(m, ...)
	#define FAST_ERR(m, ...)
	#define FAST_DBG(m, ...)
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
};

static struct clovercon_info con_info_list[MAX_CON_COUNT];
static int module_params[2 * MAX_CON_COUNT];
static int arr_argc = 0;

#define CON_NAME_PREFIX "Nintendo Clovercon - controller"
const char *controller_names[] = {CON_NAME_PREFIX"1", CON_NAME_PREFIX"2",
                                  CON_NAME_PREFIX"3", CON_NAME_PREFIX"4"};

module_param_array(module_params, int, &arr_argc, 0000);
MODULE_PARM_DESC(module_params, "Input info in the form con0_i2c_bus, con0_detect_gpio, "
	                            "form con1_i2c_bus, con1_detect_gpio, ... gpio < 0 means no detection");
module_param(home_combination, short, 0000);
MODULE_PARM_DESC(home_combination, "Button combination to open menu "
				"(0x001=A,0x002=B,0x004=Select,0x008=Start,0x010=Up,0x020=Down,0x040=Left,0x080=Right,0x100=X,0x200=Y,0x400=L,0x800=R");
module_param(autofire, byte, 0000);
MODULE_PARM_DESC(autofire, "Enable autofire (hold select+a / select+b for a second)");
module_param(autofire_xy, byte, 0000);
MODULE_PARM_DESC(autofire_xy, "Use X/Y on classic controller as autofire A/B");
module_param(autofire_interval, byte, 0000);
MODULE_PARM_DESC(autofire_interval, "Autofire interval (default is 8)");
module_param(fc_start, byte, 0000);
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
	// print_hex_dump(KERN_DEBUG, "Controller info data: " , DUMP_PREFIX_NONE, 16, 256, data, len, false);
}

static int clovercon_setup(struct i2c_client *client) {
	u8 init_data[] = { 0xf0, 0x55, 0xfb, 0x00, 0xfe, DATA_FORMAT };
	static const int CON_INFO_LEN = 6;
	u8 con_info_data[CON_INFO_LEN];
	int ret;

	DBG("Clovercon setup");

	ret = clovercon_write(client, &init_data[0], 2);
	if (ret)
		goto err;
	ret = clovercon_write(client, &init_data[2], 2);
	if (ret)
		goto err;
	ret = clovercon_write(client, &init_data[4], 2);
	if (ret)
		goto err;

	ret = clovercon_read_controller_info(client, con_info_data, CON_INFO_LEN);
	if (ret < 0) {
		ERR("error reading controller info");
		goto err;
	} else if (ret != CON_INFO_LEN) {
		ERR("wrong read length %i reading controller info", ret);
		ret = -EIO;
		goto err;
	}
	if (con_info_data[4] != DATA_FORMAT) {
		ERR("failed to set data format, value is %i", (int)con_info_data[4]);
		ret = -EIO;
		goto err;
	}
	if (con_info_data[5] != 1) {
		ERR("unsupported controller id %i", (int)con_info_data[5]);
		ret = -EIO;
		goto err;
	}

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
	int jx, jy, rx, ry, tl, tr;
	bool left, right, up, down, a, b, x, y, select, start, home, l, r, zl, zr, reset;
	u16 retry_delay = RETRY_BASE_DELAY;
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

		//print_hex_dump(KERN_DEBUG, "", DUMP_PREFIX_OFFSET, 16, 256, data, READ_LEN, false);

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
		for (jx=8; jx<21; jx++) {
			jy += data[jx];
		}
		if (jy) {
			/* noise, discard payload */
			break;
		}

		jx = ((data[0] & 0x3f) - 0x20) * 4;
		rx = (((data[2] >> 7) | ((data[1] & 0xC0) >> 4) | ((data[0] & 0xC0) >> 2)) - 0x10) * 8;
		jy = ((data[1] & 0x3f) - 0x20) * -4;
		ry = ((data[2] & 0x1f) - 0x10) * -8;
		tl = ((data[3] >> 5) | ((data[2] & 0x60) >> 3)) * 8;
		tr = (data[3] & 0x1f) * 8;

		r      = !get_bit(data[4], D_BTN_R);
		start  = !get_bit(data[4], D_BTN_START);
		home   = !get_bit(data[4], D_BTN_HOME);
		select = !get_bit(data[4], D_BTN_SELECT);
		l      = !get_bit(data[4], D_BTN_L);
		down   = !get_bit(data[4], D_BTN_DOWN);
		right  = !get_bit(data[4], D_BTN_RIGHT);

		up   = !get_bit(data[5], DF1_BTN_UP);
		left = !get_bit(data[5], DF1_BTN_LEFT);
		zr   = !get_bit(data[5], DF1_BTN_ZR);
		x    = !get_bit(data[5], DF1_BTN_X);
		y    = !get_bit(data[5], DF1_BTN_Y);
		a    = !get_bit(data[5], DF1_BTN_A);
		b    = !get_bit(data[5], DF1_BTN_B);
		zl   = !get_bit(data[5], DF1_BTN_ZL);

		// Reset combination
		reset =
		    (((home_combination >> 0) & 1) ^ !a) &&
		    (((home_combination >> 1) & 1) ^ !b) &&
		    (((home_combination >> 2) & 1) ^ !select) &&
		    (((home_combination >> 3) & 1) ^ !start) &&
		    (((home_combination >> 4) & 1) ^ !up) &&
		    (((home_combination >> 5) & 1) ^ !down) &&
		    (((home_combination >> 6) & 1) ^ !left) &&
		    (((home_combination >> 7) & 1) ^ !right) &&
		    (((home_combination >> 8) & 1) ^ !x) &&
		    (((home_combination >> 9) & 1) ^ !y) &&
		    (((home_combination >> 10) & 1) ^ !l) &&
		    (((home_combination >> 11) & 1) ^ !r);

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
			ret = clovercon_setup(info->client);
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
	if (clovercon_setup(info->client)) {
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
		.name	= "clovercon",
		.owner = THIS_MODULE,
	},

	.id_table	= clovercon_idtable,
	.probe		= clovercon_probe,
	.remove		= clovercon_remove,
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
	DBG("detect task running");
	mutex_lock(&con_state_lock);
	for (i = 0; i < MAX_CON_COUNT; i++) {
		info = &con_info_list[i];
		if (!info->detection_active) {
			continue;
		}
		val = gpio_get_value(info->gpio);
		DBG("detect pin value: %i", val);
		if (val && !info->client) {
			DBG("detect task adding controller %i", i);
			clovercon_add_controller(info);
		} else if (!val && info->client) {
			DBG("detect task removing controller %i", i);
			clovercon_remove_controller(info);
		}
	}
	mutex_unlock(&con_state_lock);
	mutex_unlock(&detect_task_lock);
	DBG("detect task done");
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
