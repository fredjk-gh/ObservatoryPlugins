using com.github.fredjk_gh.ObservatoryFleetCommander.Data;
using Observatory.Framework.Files.ParameterTypes;
using Observatory.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    public partial class SpanshCarrierRouterForm : Form
    {
        private IObservatoryCore _core;
        private IObservatoryWorker _worker;
        private string _currentCommander;
        private CarrierManager _carrierManager;
        private CarrierRoute _carrierRoute;

        public SpanshCarrierRouterForm(IObservatoryCore core, IObservatoryWorker worker, string currentCommander, CarrierManager carrierManager)
        {
            InitializeComponent();

            _core = core;
            _worker = worker;
            _currentCommander = currentCommander;
            _carrierManager = carrierManager;

            btnClearRoute.Visible = false;
            btnClearRoute.Enabled = false;

            if (_carrierManager.Count == 0)
            {
                txtOutput.Text = "No known carriers. Open your Carrier Administration Panel or perform a Read-All, close this window and try again.";
                btnSaveRoute.Enabled = false;
                btnGenerateRoute.Enabled = false;
            }
            else
            {
                foreach (var carrierData in _carrierManager.Carriers)
                {
                    int cmdrIndex = cbCommanders.Items.Add(carrierData.OwningCommander);

                    if (carrierData.OwningCommander == _currentCommander)
                    {
                        cbCommanders.SelectedIndex = cmdrIndex;
                    }
                }
            }

            DialogResult = DialogResult.Cancel;
        }

        public string SelectedCommander { get => _currentCommander; }

        public CarrierData CarrierDataForSelectedCommander
        {
            get => _carrierManager.GetByCommander(SelectedCommander);
        }

        private void ValidateInputs()
        {
            if (cbCommanders.SelectedIndex < 0)
            {
                btnGenerateRoute.Enabled = false;
                btnSaveRoute.Enabled = false;
                return;
            }

            string selectedCmdr = cbCommanders.Items[cbCommanders.SelectedIndex]?.ToString() ?? "";
            CarrierData carrierData = _carrierManager.GetByCommander(selectedCmdr);

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
                lblCarrierNameAndId.Text = string.Empty;
                txtStartSystem.Text = string.Empty;
                nudUsedCapacity.Value = 0;
            };

            lblCarrierNameAndId.Text = $"{carrierData.CarrierName} ({carrierData.CarrierCallsign})";
            JumpInfo nextJump = null;
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
            nudUsedCapacity.Value = carrierData.CapacityUsed;

            if (carrierData.HasRoute)
            {
                txtDestSystem.Text = carrierData.Route.DestinationSystem;
                btnClearRoute.Visible = true;
                btnClearRoute.Enabled = true;

                if (nextJump != null)
                {
                    var nextSystem = nextJump.SystemName;
                    Clipboard.SetText(nextSystem);
                    txtOutput.Text = $"Route to {carrierData.Route.DestinationSystem} detected; next jump is {nextSystem} and has been placed on the clipboard.";
                }
                // Otherwise, route is presumably completed or we're not on-course... Either way, enable clearing it.
            }
            else
            {
                btnClearRoute.Visible = false;
                btnClearRoute.Enabled = false;
                txtOutput.Text = string.Empty;
                txtDestSystem.Text = "";
            }
        }

        private void ToggleInputs(bool isEnabled)
        {
            cbCommanders.Enabled = isEnabled;
            txtStartSystem.Enabled = isEnabled;
            txtDestSystem.Enabled = isEnabled;
            nudUsedCapacity.Enabled = isEnabled;
            btnClearRoute.Enabled = isEnabled;
        }

        private void cbCommanders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCommanders.SelectedIndex >= 0)
            {
                string selectedCmdr = cbCommanders.Items[cbCommanders.SelectedIndex]?.ToString() ?? "";

                if (!string.IsNullOrWhiteSpace(selectedCmdr))
                {
                    _currentCommander = selectedCmdr;
                    PrefillCarrierInfo(CarrierDataForSelectedCommander);
                }
            }
            ValidateInputs();
        }

        private void txtStartSystem_TextChanged(object sender, EventArgs e)
        {
            ValidateInputs();
        }

        private void txtDestSystem_TextChanged(object sender, EventArgs e)
        {
            ValidateInputs();
        }

        private void nudUsedCapacity_ValueChanged(object sender, EventArgs e)
        {
            ValidateInputs();
        }

        private void btnGenerateRoute_Click(object sender, EventArgs e)
        {
            // Disable UI elements to prevent fiddling.
            btnGenerateRoute.Enabled = false;
            ToggleInputs(false);

            // Create a request
            var requestUri = $"https://spansh.co.uk/api/fleetcarrier/route?source={Uri.EscapeDataString(txtStartSystem.Text)}&destinations={Uri.EscapeDataString(txtDestSystem.Text)}&capacity_used={nudUsedCapacity.Value}&calculate_starting_fuel=1";
            string jobJson = "";

            // Send it, get result ID.
            var searchStartTask = _core.HttpClient.GetStringAsync(requestUri);
            try
            {
                jobJson = searchStartTask.Result;
            }
            catch (Exception ex)
            {
                txtOutput.Text = $"Failed to start search on Spansh: {ex.Message}{Environment.NewLine}See Error log.";
                _core.GetPluginErrorLogger(_worker)(ex, "Initiating Spansh route search");
                ToggleInputs(true);
                return;
            }

            using var jobDoc = JsonDocument.Parse(jobJson);
            var jobRoot = jobDoc.RootElement;
            var jobId = jobRoot.GetProperty("job").GetString();
            txtOutput.Text = $"Got Job ID {jobId}; polling for result...";

            // Poll until it's ready.
            var routeJson = string.Empty;
            do
            {
                Thread.Sleep(1500);
                var fetchResultTask = _core.HttpClient.GetStringAsync($"https://spansh.co.uk/api/results/{jobId}");
                try
                {
                    routeJson = fetchResultTask.Result;
                }
                catch (Exception ex)
                {
                    txtOutput.Text = $"Spansh result fetch failed: {ex.Message}{Environment.NewLine}See Error log.";
                    _core.GetPluginErrorLogger(_worker)(ex, "Fetching Spansh route result");
                    ToggleInputs(true);
                    return;
                }

                using var resultDoc = JsonDocument.Parse(routeJson);
                var resultRoot = resultDoc.RootElement;
                JsonElement property;
                if (resultRoot.TryGetProperty("job", out property))
                {
                    routeJson = ""; // Still working on it...
                }
                else if (resultRoot.TryGetProperty("error", out property))
                {
                    txtOutput.Text = $"Spansh returned error: {property.GetString()}{Environment.NewLine}Perhaps your systems are unknown?";
                    ToggleInputs(true);
                    return;
                }
                else if (resultRoot.TryGetProperty("result", out property))
                {
                    // We have a result!
                    _carrierRoute = CarrierRoute.FromSpanshResultJson(property, jobId);
                    txtOutput.Text = $"Route ready! {_carrierRoute.Jumps.Count - 1} jumps.";
                }
                else
                {
                    txtOutput.Text = $"Spansh returned unknown response: {routeJson}";
                    ToggleInputs(true);
                    return;
                }
            } while (string.IsNullOrWhiteSpace(routeJson));

            ToggleInputs(true);
            ValidateInputs(); // will now enable the Save-and-Start button.
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            // If a route has been fetched, set the route into the CarrierData object for the SelectedCommander.
            var data = CarrierDataForSelectedCommander;
            if (data == null) // This shouldn't happen.
            {
                txtOutput.Text = $"Nowhere to save route!? Data";
                btnSaveRoute.Enabled = false;
                ValidateInputs();
                return;
            }

            data.Route = _carrierRoute;
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnClearRoute_Click(object sender, EventArgs e)
        {
            txtDestSystem.Text = string.Empty;
            btnClearRoute.Enabled = false;
            btnClearRoute.Visible = false;
            DialogResult = DialogResult.Abort;
        }
    }
}
