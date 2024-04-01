using Observatory.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    public partial class FixRouteForm : Form
    {
        private IObservatoryCore _core;
        private IObservatoryWorker _worker;
        private string _currentCommander;
        private CarrierManager _carrierManager;

        public FixRouteForm(IObservatoryCore core, IObservatoryWorker worker, string currentCommander, CarrierManager carrierManager)
        {
            InitializeComponent();

            cbCurrentSystem.Items.Clear();
            cbCurrentSystem.Enabled = false;
            btnApply.Enabled = false;

            _core = core;
            _worker = worker;
            _currentCommander = currentCommander;
            _carrierManager = carrierManager;

            if (_carrierManager.Count == 0)
            {
                cbCommander.Enabled = false;
                cbCommander.Items.Add("(No commanders)");
            }
            else
            {
                foreach (var carrierData in _carrierManager.Carriers)
                {
                    int cmdrIndex = cbCommander.Items.Add(carrierData.OwningCommander);

                    if (carrierData.OwningCommander == _currentCommander)
                    {
                        cbCommander.SelectedIndex = cmdrIndex;
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
            if (cbCommander.SelectedIndex < 0)
            {
                cbCurrentSystem.Items.Clear();
                cbCurrentSystem.Enabled = false;
                txtNextJumpSystem.Text = "(Select Commander.)";
                btnApply.Enabled = false;
                return;
            }

            var carrierData = CarrierDataForSelectedCommander;

            // Should never happen.
            if (carrierData == null || !carrierData.HasRoute)
            {
                cbCurrentSystem.Enabled = false;
            }
            else
            {
                cbCurrentSystem.Enabled = true;
 
                // Ensure selected current system is not the destination.
                btnApply.Enabled = (cbCurrentSystem.SelectedIndex > 0 && cbCurrentSystem.SelectedIndex < carrierData.Route.Jumps.Count - 1);
            }
        }

        #region Events

        private void cbCommander_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCommander.SelectedIndex < 0) return;

            cbCurrentSystem.Items.Clear();
            txtNextJumpSystem.Text = string.Empty;

            string selectedCmdr = cbCommander.Items[cbCommander.SelectedIndex]?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(selectedCmdr))
                return;

            _currentCommander = selectedCmdr;

            CarrierData carrierData = CarrierDataForSelectedCommander;

            if (carrierData == null) {
                cbCurrentSystem.Enabled = false;
                txtNextJumpSystem.Text = "(No known carrier.)";
                return;
            }

            if (!carrierData.HasRoute) {
                cbCurrentSystem.Enabled = false;
                txtNextJumpSystem.Text = "(No route.)";
            }
            else
            {
                // Don't include the destination in the "current system" list of options. Because there's no next
                // jump for that system.
                foreach (var jump in carrierData.Route.Jumps.Take(carrierData.Route.Jumps.Count - 1))
                {
                    int index = cbCurrentSystem.Items.Add(jump.SystemName);

                    if (carrierData.IsPositionKnown && jump.SystemName == carrierData.Position.SystemName)
                    {
                        cbCurrentSystem.SelectedIndex = index;
                    }
                }
            }

            ValidateInputs();
        }

        private void cbCurrentSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            var carrierData = CarrierDataForSelectedCommander;
            int currentSystemIndex = cbCurrentSystem.SelectedIndex;

            if (carrierData == null || currentSystemIndex == -1 || currentSystemIndex >= carrierData.Route.Jumps.Count - 1)
            {
                // No data, no selection or destination is selected.
                return;
            }

            JumpInfo jumpInfo = carrierData.Route.Jumps[currentSystemIndex + 1];
            txtNextJumpSystem.Text = jumpInfo.SystemName;
            Clipboard.SetText(txtNextJumpSystem.Text);

            ValidateInputs();
        }

        private void txtNextJumpSystem_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtNextJumpSystem.Text))
            {
                Clipboard.SetText(txtNextJumpSystem.Text);
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            var carrierData = CarrierDataForSelectedCommander;
            int currentSystemIndex = cbCurrentSystem.SelectedIndex;

            if (carrierData == null || currentSystemIndex == -1 || currentSystemIndex >= carrierData.Route.Jumps.Count - 1)
            {
                // No data, no selection or destination is selected.
                ValidateInputs();
                return;
            }

            // Update current system in data cache.
            JumpInfo jumpInfo = carrierData.Route.Jumps[currentSystemIndex];

            carrierData.MaybeUpdateLocation(new CarrierPositionData(jumpInfo.SystemName, jumpInfo.SystemAddress));

            DialogResult = DialogResult.OK;
            Close();
        }
        #endregion
    }
}
