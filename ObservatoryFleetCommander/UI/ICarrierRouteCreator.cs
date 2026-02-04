using com.github.fredjk_gh.ObservatoryFleetCommander.Data;

namespace com.github.fredjk_gh.ObservatoryFleetCommander.UI
{
    internal interface ICarrierRouteCreator
    {
        ulong SelectedCarrierId { get; }

        CarrierData CarrierDataForSelectedId { get; }
    }
}
