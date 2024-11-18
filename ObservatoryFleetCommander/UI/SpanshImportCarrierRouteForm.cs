using com.github.fredjk_gh.ObservatoryFleetCommander.Data;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.github.fredjk_gh.ObservatoryFleetCommander.UI
{
    public partial class SpanshImportCarrierRouteForm : Form, ICarrierRouteCreator
    {
        private const string SPANSH_DOMAIN = "spansh.co.uk";
        private IObservatoryCore _core;
        private IObservatoryWorker _worker;
        private string _currentCommander;
        private CarrierManager _carrierManager;
        private CarrierRoute _carrierRoute;
        private string _jobId = null;

        public SpanshImportCarrierRouteForm(IObservatoryCore core, IObservatoryWorker worker, string currentCommander, CarrierManager carrierManager)
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
                txtOutput.Text = "No known carriers. Please close this window, and perform a Read-All and try again.";
                btnSaveRoute.Enabled = false;
                btnImportRoute.Enabled = false;
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
                btnImportRoute.Enabled = false;
                btnSaveRoute.Enabled = false;
                return;
            }

            string selectedCmdr = cbCommanders.Items[cbCommanders.SelectedIndex]?.ToString() ?? "";
            CarrierData carrierData = _carrierManager.GetByCommander(selectedCmdr);

            if (carrierData == null)
            {
                btnImportRoute.Enabled = false;
                btnSaveRoute.Enabled = false;
                return;
            }

            Uri? resultsUri = null;
            Guid jobGuid = Guid.Empty;
            UriCreationOptions options = new();
            // Results URLs can come in one of two flavors:
            // UI URL: https://spansh.co.uk/fleet-carrier/results/<jobId>?<parameters>
            // API result: https://spansh.co.uk/api/results/<jobId>
            if (!string.IsNullOrWhiteSpace(txtSpanshResultsUrl.Text)
                && Uri.TryCreate(txtSpanshResultsUrl.Text?.Trim(), options, out resultsUri)
                && resultsUri.Host == SPANSH_DOMAIN
                && resultsUri.Segments.Length >= 4
                && (resultsUri.Segments[1] == "fleet-carrier/" || resultsUri.Segments[1] == "api/")
                && resultsUri.Segments[2] == "results/"
                && Guid.TryParse(resultsUri.Segments[3], out jobGuid))
            {
                // Ok this looks like a spansh result URL..
                _jobId = resultsUri.Segments[3];
                btnImportRoute.Enabled = true;
            }
            else
            {
                btnImportRoute.Enabled = false;
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
            };

            lblCarrierNameAndId.Text = $"{carrierData.CarrierName} ({carrierData.CarrierCallsign})";

            if (carrierData.HasRoute)
            {
                btnClearRoute.Visible = true;
                btnClearRoute.Enabled = true;

                txtOutput.Text = $"Route to {carrierData.Route.DestinationSystem} detected; importing a new route will overwrite it.";
                // Otherwise, route is presumably completed or we're not on-course... Either way, enable clearing it.
            }
            else
            {
                btnClearRoute.Visible = false;
                btnClearRoute.Enabled = false;
                txtOutput.Text = string.Empty;
            }
        }

        private void ToggleInputs(bool isEnabled)
        {
            cbCommanders.Enabled = isEnabled;
            txtSpanshResultsUrl.Enabled = isEnabled;
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

        private void txtSpanshResultsUrl_TextChanged(object sender, EventArgs e)
        {
            ValidateInputs();
        }

        private async void btnImportRoute_Click(object sender, EventArgs e)
        {
            // Disable UI elements to prevent fiddling.
            btnImportRoute.Enabled = false;
            ToggleInputs(false);

            txtOutput.Text = $"Got Job ID {_jobId}; polling for result...";

            // Poll until it's ready. Unfortunate, but the only way we can do it.
            // That said, I wonder if this should be a separate task at some point.
            var routeJson = string.Empty;
            do
            {
                var fetchResultTask = _core.HttpClient.GetStringAsync($"https://spansh.co.uk/api/results/{_jobId}");
                try
                {
                    await fetchResultTask;
                }
                catch (Exception ex)
                {
                    txtOutput.Text = $"Spansh result fetch failed: {ex.Message}"
                        + $"{Environment.NewLine}This is likely because the result has expired; please use the plotter.";
                    break;
                }

                routeJson = fetchResultTask.Result;
                using var resultDoc = JsonDocument.Parse(routeJson);
                var resultRoot = resultDoc.RootElement;
                JsonElement property;
                if (resultRoot.TryGetProperty("result", out property))
                {
                    // We have a result! Always check this first, in case both "job" and "result" are present to avoid loops.
                    _carrierRoute = CarrierRoute.FromSpanshResultJson(property, _jobId);
                    txtOutput.Text = $"Route ready! {_carrierRoute.Jumps.Count - 1} jumps.";
                    break;
                }
                else if (resultRoot.TryGetProperty("job", out property))
                {
                    routeJson = ""; // Still working on it...
                    Task pause = Task.Delay(1500); // Non-blocking wait.
                    await pause;
                    continue;
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
            btnClearRoute.Enabled = false;
            btnClearRoute.Visible = false;
            DialogResult = DialogResult.Abort;
        }
    }
}
