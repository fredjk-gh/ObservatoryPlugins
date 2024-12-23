using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.PluginCommon.Data
{
    public class Conversions
    {
        public const float CONV_MperS2_TO_G_DIVISOR = 9.81f;
        public const float CONV_M_TO_SOLAR_RAD_DIVISOR = 695500000.0f;
        public const float CONV_M_TO_KM_DIVISOR = 1000.0f;
        public const float CONV_M_TO_LS_DIVISOR = 299792458.0f;
        public const float CONV_M_TO_AU_DIVISOR = 149597870691;
        public const float CONV_S_TO_DAYS_DIVISOR = 86400.0f;
        public const float CONV_S_TO_HOURS_DIVISOR = 3600.0f;
        public const float CONV_PA_TO_ATM_DIVISOR = 101325.0f;
        
        // Distances / lengths:
        // - m <-> Ls
        // - m <-> km
        public static float MetersToKm(float meters)
        {
            return meters / CONV_M_TO_KM_DIVISOR;
        }

        public static float KmToMeters(float km)
        {
            return km * CONV_M_TO_KM_DIVISOR;
        }

        // - m <-> AU
        // - m <-> Sol Radius (SR)
        public static float MetersToSr(float meters)
        {
            return meters / CONV_M_TO_SOLAR_RAD_DIVISOR;
        }

        public static float SrToMeters(float sr)
        {
            return sr * CONV_M_TO_SOLAR_RAD_DIVISOR;
        }

        // - AU <-> Ls
        public static float MetersToAu(float meters)
        {
            return meters / CONV_M_TO_AU_DIVISOR;
        }

        public static float AuToMeters(float au)
        {
            return au * CONV_M_TO_AU_DIVISOR;
        }

        // Gravity:
        // - mps^2 <-> g
        public static float Mpers2ToG(float mps2)
        {
            return mps2 / CONV_MperS2_TO_G_DIVISOR;
        }

        public static float GToMperS2(float g)
        {
            return g * CONV_MperS2_TO_G_DIVISOR;
        }

        // Pressure:
        // - pa <-> atm
        public static float PaToAtm(float pascals)
        {
            return pascals / CONV_PA_TO_ATM_DIVISOR;
        }

        public static float AtmsToPa(float atms)
        {
            return atms * CONV_PA_TO_ATM_DIVISOR;
        }

        // Time period:
        // - s <-> hr
        // - s <-> d
        public static float SecondsToDays(float seconds)
        {
            return seconds / CONV_S_TO_DAYS_DIVISOR;
        }
        public static float DaysToSeconds(float days)
        {
            return days * CONV_S_TO_DAYS_DIVISOR;
        }

        // Angles:
        // - rad <-> deg

    }
}
