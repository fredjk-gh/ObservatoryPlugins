using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryAggregator
{
    class ShipData
    {
        private ulong _shipId;
        private string _name;

        public ShipData(ulong shipId, string name = "")
        { 
            _shipId = shipId;
            _name = name;
        }

        public ulong ShipId { get => _shipId; }

        public string Name
        {
            get => string.IsNullOrWhiteSpace(_name) ? "(unknown ship)" : _name;
            set => _name = value;
        }
    }
}
