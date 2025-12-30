using LiteDB;
using Observatory.Framework.Files.ParameterTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryArchivist.DB
{
    internal class SystemInfo
    {
        public SystemInfo() { }

        public SystemInfo(ulong id64, string systemName, StarPosition position) : this()
        {
            Id64 = id64;
            ProcGenName = systemName; // Will be moved and overridden if not proc gen.
            CommonName = systemName; // Arguably the name this was first seen as.
            x = position.x;
            y = position.y;
            z = position.z;
        }

        [BsonId]
        public UInt64 Id64 { get; set; }
        public string ProcGenName { get; set; }
        public string CommonName { get; set; }

        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
    }
}
