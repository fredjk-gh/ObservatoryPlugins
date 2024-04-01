using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryProspectorBasic
{
    internal class RingDetails
    {
        public string ShortName;
        public string RingType;
        public string Commodities;
        public string Density;

        public override string ToString()
        {
            return $"{ShortName}: {RingType} {Commodities}, {Density}";
        }
    }
}
