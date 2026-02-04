using com.github.fredjk_gh.ObservatoryFleetCommander.Data;
using com.github.fredjk_gh.ObservatoryFleetCommander.UI;
using com.github.fredjk_gh.PluginCommon.Data.Spansh;
using com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric;
using com.github.fredjk_gh.PluginCommon.Utilities;

namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    internal partial class SpanshCarrierRouterForm : Form, ICarrierRouteCreator
    {
        private readonly CommanderContext _c;
        private FleetCarrierRouteJobResult.RouteResult _carrierRoute;

        internal SpanshCarrierRouterForm(CommanderContext ctx, string currentCommander)
        {
            InitializeComponent();

            _c = ctx;

            btnClearRoute.Visible = false;
            btnClearRoute.Enabled = false;

            if (_c.Manager.CarrierCount == 0)
            {
                SetOutput("No known carriers. Please close this window, and perform a Read-All and try again.");
                btnSaveRoute.Enabled = false;
                btnGenerateRoute.Enabled = false;
            }
            else
            {
                foreach (var carrierData in _c.Manager.Carriers)
                {
                    int carrierIndex = cbCarriers.Items.Add(carrierData);
                    if (carrierData.Owner == currentCommander)
                    {
                        cbCarriers.SelectedIndex = carrierIndex;
                    }
                }
            }

            DialogResult = DialogResult.Cancel;
        }

        public ulong SelectedCarrierId { get => ((CarrierData)cbCarriers.SelectedItem).CarrierId; }

        public CarrierData CarrierDataForSelectedId
        {
            get => (CarrierData)cbCarriers.SelectedItem;
        }

        private void SetOutput(string msg = "")
        {
            _c.Core.ExecuteOnUIThread(() => { txtOutput.Text = msg; });
        }

        private void ValidateInputs()
        {
            if (cbCarriers.SelectedIndex < 0)
            {
                btnGenerateRoute.Enabled = false;
                btnSaveRoute.Enabled = false;
                return;
            }

            CarrierData carrierData = (CarrierData)cbCarriers.SelectedItem;

            if (carrierData == null)
            {
                btnGenerateRoute.Enabled = false;
                btnSaveRoute.Enabled = false;
                return;
            }

            if (!string.IsNullOrWhiteSpace(txtStartSystem.Text)
                && !string.IsNullOrWhiteSpace(txtDestSystem.Text)
                && nudUsedCapacity.Value > 0)
            {
                // TODO: Can I / should I validate Spansh knows these systems?
                btnGenerateRoute.Enabled = true;
            }
            else
            {
                btnGenerateRoute.Enabled = false;
                btnSaveRoute.Enabled = false;
                return;
            }

            if (_carrierRoute != null)
            {
                btnSaveRoute.Enabled = true;
            }
        }

        private void PrefillCarrierInfo(CarrierData carrierData)
        {
            if (carrierData == null)
            {
                lblCarrierTypeValue.Text = string.Empty;
                txtStartSystem.Text = string.Empty;
                nudUsedCapacity.Value = 0;
                return;
            }

            lblCarrierTypeValue.Text = $"{carrierData.CarrierType}";
            nudUsedCapacity.Maximum = carrierData.CarrierCapacity;

            FleetCarrierRouteJobResult.Jump nextJump = null;
            if (carrierData.IsPositionKnown)
            {
                txtStartSystem.Text = carrierData.Position.SystemName;
                if (carrierData.HasRoute)
                {
                    txtStartSystem.Text = carrierData.Route.Jumps[0].SystemName;
                    nextJump = carrierData.Route.GetNextJump(carrierData.Position.SystemName);
                }
            }
            else
            {
                txtStartSystem.Text = "";
            }
            nudUsedCapacity.Value = carrierData.LastCarrierStats is null 
                ? carrierData.CarrierCapacity 
                : carrierData.CarrierCapacity - carrierData.LastCarrierStats.SpaceUsage.FreeSpace;

            if (carrierData.HasRoute)
            {
                txtDestSystem.Text = carrierData.Route.Destinations.LastOrDefault();
                btnClearRoute.Visible = true;
                btnClearRoute.Enabled = true;

                if (nextJump != null)
                {
                    var nextSystem = nextJump.SystemName;
                    Misc.SetTextToClipboard(nextSystem);
                    SetOutput($"Route to {carrierData.Route.Destinations.LastOrDefault()} detected; next jump is {nextSystem} and has been placed on the clipboard.");
                }
                // Otherwise, route is presumably completed or we're not on-course... Either way, enable clearing it.
            }
            else
            {
                btnClearRoute.Visible = false;
                btnClearRoute.Enabled = false;
                SetOutput();
                txtDestSystem.Text = "";
            }
        }

        private void ToggleInputs(bool isEnabled)
        {
            cbCarriers.Enabled = isEnabled;
            txtStartSystem.Enabled = isEnabled;
            txtDestSystem.Enabled = isEnabled;
            nudUsedCapacity.Enabled = isEnabled;
            btnClearRoute.Enabled = isEnabled;
        }

        private void CbCarriers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCarriers.SelectedIndex >= 0)
            {
                PrefillCarrierInfo(CarrierDataForSelectedId);
            }
            ValidateInputs();
        }

        private void TxtStartSystem_TextChanged(object sender, EventArgs e)
        {
            ValidateInputs();
        }

        private void TxtDestSystem_TextChanged(object sender, EventArgs e)
        {
            ValidateInputs();
        }

        private void NudUsedCapacity_ValueChanged(object sender, EventArgs e)
        {
            ValidateInputs();
        }

        private void BtnGenerateRoute_Click(object sender, EventArgs e)
        {
            // Disable UI elements to prevent fiddling.
            btnGenerateRoute.Enabled = false;
            ToggleInputs(false);

            // Maybe make this a member variable and use it to enable btnGenerateRoute?
            CarrierData data = (CarrierData)cbCarriers.SelectedItem;

            // Create a request
            FleetCarrierRouteJobResult.Parameters req = new()
            {
                SourceSystem = txtStartSystem.Text.Trim(),
                DestinationSystems = [.. txtDestSystem.Text.Trim().Split(',').Where(d => !string.IsNullOrWhiteSpace(d))],
                CurrentFuel = data.CarrierFuel,
                CapacityUsed = (int)nudUsedCapacity.Value,
                Capacity = data.CarrierCapacity,
                Mass = data.IsSquadronCarrier ? 15000 : data.CarrierCapacity,
            };
            if (!req.IsValid())
            {
                SetOutput($"Request is not valid!");
                ToggleInputs(true);
                ValidateInputs();
                return;
            }

            Task.Run(() =>
            {
#if DEBUG
                using CancellationTokenSource cts = new();

#else
                using CancellationTokenSource cts = new(10000); // This polls, give extra time.
#endif
                try
                {
                    var fetchTask = SpanshHelper.FetchFleetCarrierRoute(_c, req, cts.Token);
                    var result = fetchTask.Result;

                    if (result is not null && result.Result is not null)
                    {
                        _carrierRoute = result.Result;
                        SetOutput($"Route ready! {_carrierRoute.Jumps.Count - 1} jumps.");
                    }
                    else
                    {
                        SetOutput("Unexpected: No result AND no error from Spansh fetch!?");
                    }
                }
                catch (Exception ex)
                {
                    // These are likely to be SpanshException wrapped and should be half decently descriptive.
                    // The helper logs the errors at source for pure call stacks.
                    SetOutput(ex.Message);
                }
                finally
                {
                    // We're explicitly not on the UI thread here, don't touch the UI directly.
                    _c.Core.ExecuteOnUIThread(() =>
                    {
                        ToggleInputs(true);
                        ValidateInputs(); // will now enable the Save-and-Start button if successful.
                    });
                }
            });
        }

        private void BtnAccept_Click(object sender, EventArgs e)
        {
            // If a route has been fetched, set the route into the CarrierData object for the selected carrier.
            var data = CarrierDataForSelectedId;
            if (data == null) // This shouldn't happen.
            {
                SetOutput($"Nowhere to save route!? Data");
                btnSaveRoute.Enabled = false;
                ValidateInputs();
                return;
            }

            data.Route = _carrierRoute;
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnClearRoute_Click(object sender, EventArgs e)
        {
            txtDestSystem.Text = string.Empty;
            btnClearRoute.Enabled = false;
            btnClearRoute.Visible = false;
            DialogResult = DialogResult.Abort;
        }
    }
}
