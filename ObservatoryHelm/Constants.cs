using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryHelm
{
    internal class Constants
    {
        public const int COALESCING_ID_SYSTEM = -1;
        public const double FuelWarningLevelFactor = 1.5;
        public const double CONV_MperS2_TO_G_DIVISOR = 9.81;

        public static readonly Dictionary<string, double> MaxFuelPerJumpByFSDSizeClass = new()
        {
            { "int_hyperdrive_size2_class1", 0.6 },
            { "int_hyperdrive_size2_class2", 0.6 },
            { "int_hyperdrive_size2_class3", 0.6 },
            { "int_hyperdrive_size2_class4", 0.8 },
            { "int_hyperdrive_size2_class5", 0.9 },
            { "int_hyperdrive_size3_class1", 1.2 },
            { "int_hyperdrive_size3_class2", 1.2 },
            { "int_hyperdrive_size3_class3", 1.2 },
            { "int_hyperdrive_size3_class4", 1.5 },
            { "int_hyperdrive_size3_class5", 1.8 },
            { "int_hyperdrive_size4_class1", 2 },
            { "int_hyperdrive_size4_class2", 2 },
            { "int_hyperdrive_size4_class3", 2 },
            { "int_hyperdrive_size4_class4", 2.5 },
            { "int_hyperdrive_size4_class5", 3 },
            { "int_hyperdrive_size5_class1", 3.3 },
            { "int_hyperdrive_size5_class2", 3.3 },
            { "int_hyperdrive_size5_class3", 3.3 },
            { "int_hyperdrive_size5_class4", 4.1 },
            { "int_hyperdrive_size5_class5", 5 },
            { "int_hyperdrive_size6_class1", 5.3 },
            { "int_hyperdrive_size6_class2", 5.3 },
            { "int_hyperdrive_size6_class3", 5.3 },
            { "int_hyperdrive_size6_class4", 6.6 },
            { "int_hyperdrive_size6_class5", 8 },
            { "int_hyperdrive_size7_class1", 8.5 },
            { "int_hyperdrive_size7_class2", 8.5 },
            { "int_hyperdrive_size7_class3", 8.5 },
            { "int_hyperdrive_size7_class4", 10.6 },
            { "int_hyperdrive_size7_class5", 12.8 },
        };
        public static readonly HashSet<string> Scoopables = new() {
            "O", "B", "A", "F", "G", "K", "M",
            "A_BlueWhiteSuperGiant", "B_BlueWhiteSuperGiant", "F_WhiteSuperGiant",
            "G_WhiteSuperGiant", "K_OrangeGiant", "M_RedSuperGiant", "M_RedGiant" };
    }
}
