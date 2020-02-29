#if !NET461
using System.ComponentModel.DataAnnotations;
#endif

namespace TeamShinkansen.Scrapers.Enums
{
    public enum GameRegion
    {
        #if !NET461
        [Display(Name = "Region Free")]
        #endif
        RegionFree,

        #if !NET461
        [Display(Name = "NTSC - Japan")]
        #endif
        NTSC_J,

        #if !NET461
        [Display(Name = "NTSC - North America")]
        #endif
        NTSC_U,

        #if !NET461
        [Display(Name = "PAL")]
        #endif
        PAL,

        #if !NET461
        [Display(Name = "NTSC - China")]
        #endif
        NTSC_C
    }
}
