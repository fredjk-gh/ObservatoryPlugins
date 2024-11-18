using com.github.fredjk_gh.ObservatoryFleetCommander.Data;
using Observatory.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryFleetCommander.UI
{
    public interface ICarrierRouteCreator
    {
        string SelectedCommander { get; }

        CarrierData CarrierDataForSelectedCommander { get; }
    }
}
