using com.github.fredjk_gh.PluginCommon.Data;
using com.github.fredjk_gh.PluginCommon.Data.Journals;
using com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs;
using static System.Windows.Forms.AxHost;

namespace com.github.fredjk_gh.PluginCommon.UI
{
    public static class UIFormatter
    {
        public static string Credits(long valueCr)
        {
            return $"{valueCr:n0} Cr";
        }

        public static string DistanceLy(double valueLy, int precision = 2)
        {
            switch (precision)
            {
                case 0: return $"{valueLy:n0} Ly";
                case 1: return $"{valueLy:n1} Ly";
                case 2: return $"{valueLy:n2} Ly";
                case 3: return $"{valueLy:n3} Ly";
                case 4: return $"{valueLy:n4} Ly";
                default:
                    if (precision < 0) return $"{valueLy:n0} Ly";
                    else return $"{valueLy:n5} Ly";
            }
        }

        public static string DistanceLs(double valueLs, int precision = 0)
        {
            switch (precision)
            {
                case 0: return $"{valueLs:n0} Ls";
                case 1: return $"{valueLs:n1} Ls";
                case 2: return $"{valueLs:n2} Ls";
                case 3: return $"{valueLs:n3} Ls";
                case 4: return $"{valueLs:n4} Ls";
                default:
                    if (precision <0) return $"{valueLs:n0} Ls";
                    else return $"{valueLs:n5} Ls";
            }
        }

        public static string DistanceBelowLs(float valueMeters)
        {
            if (valueMeters > Conversions.CONV_M_TO_MM_DIVISOR)
            {
                return $"{Conversions.MetersToMm(valueMeters):n2} Mm";
            }
            else if (valueMeters > Conversions.CONV_M_TO_KM_DIVISOR)
            {
                return $"{Conversions.MetersToKm(valueMeters):n2} km";
            }
            else
            {
                return $"{valueMeters:n1} m";
            }
        }

        public static string GravityG(float valueMps2)
        {
            return $"{Conversions.Mpers2ToG(valueMps2):n2}g";
        }

        public static string TemperatureK(float valueKelvin)
        {
            return $"{valueKelvin:n0} K";
        }

        public static string PressureAtm(float valuePa)
        {
            return $"{Conversions.PaToAtm(valuePa):n2} atm";
        }

        public static string BodyLabelDisplay(string bodyShortName, string typePrefix = UIConstants.BODY)
        {
            if (bodyShortName.Equals(UIConstants.PRIMARY_STAR)) return bodyShortName;
            if (bodyShortName.Equals(UIConstants.SYSTEM_BARYCENTRE)) return bodyShortName;

            return $"{typePrefix} {bodyShortName}".Trim();
        }

        public static string StarLabelAbbreviated(string journalStarType)
        {
            return JournalConstants.JournalTypeAbbreviation[journalStarType];
        }

        public static string PlanetLabelAbbreviated(string journalPlanetClass, string terraformState = "")
        {
            return $"{(!string.IsNullOrWhiteSpace(terraformState) ? "T-" : string.Empty)}{JournalConstants.JournalTypeAbbreviation[journalPlanetClass]}";
        }

        public static string PlanetLabelWithTerraformState(string journalPlanetClass, string terraformState = "")
        {
            return $"{terraformState ?? string.Empty} {journalPlanetClass}".Trim();
        }

        public static string RingTypeLabel(string journalRingClass)
        {
            return $"{FDevIDs.RingsById[journalRingClass].Name} ring";
        }

        public static string Coordinates(double x, double y, double z)
        {
            return $"{x:n2}, {y:n2}, {z:n2}";
        }

        public static string SurfaceCoordinates(double lat, double lng)
        {
            return $"{lat:#,##0.0###}, {lng:#,##0.0###}";
        }

        public static string DateTime(DateTime dateTime, bool useUTCGameTime = false)
        {
            if (useUTCGameTime)
                return dateTime.ToString("yyyy-MM-dd HH:mm:ss");

            return dateTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string DateTimeAsShortTime(DateTime dateTime, bool useUTCGameTime = false)
        {
            if (useUTCGameTime)
                return dateTime.ToUniversalTime().ToShortTimeString();

            return dateTime.ToShortTimeString();
        }

        public static string DateMMMyyyy(DateTime date)
        {
            return $"{date:MMM yyyy}";
        }

        public static string Timehmmss(TimeSpan valueTime)
        {
            return $"{valueTime:h\\:mm\\:ss}";
        }

        public static string Minutes(double valueMins)
        {
            return $"{valueMins:n1} minutes";
        }

        public static string Tonnage(long valueTonnage)
        {
            return $"{valueTonnage:n0} T";
        }

        public static string Density(double valueDensity)
        {
            return $"{valueDensity:n4} MT/km^3";
        }
    }
}
