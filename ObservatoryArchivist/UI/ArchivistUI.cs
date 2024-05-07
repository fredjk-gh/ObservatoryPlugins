using Observatory.Framework;
using Observatory.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.github.fredjk_gh.ObservatoryArchivist.UI
{
    public partial class ArchivistUI : UserControl
    {
        private ArchivistContext _context;
        private int _selectedIndex = ListBox.NoMatches;
        private System.Windows.Forms.Timer _filterTimer = new();
        private JsonViewer _viewer;
        private VisitedSystem _lastResult = null;

        internal ArchivistUI(ArchivistContext context)
        {
            _context = context;

            InitializeComponent();

            Draw();

            _filterTimer.Tick += _filterTimer_Tick;
            _filterTimer.Interval = 150;
        }


        public void Draw(string msg = "")
        {
            if (_context.IsReadAll) return;

            // Current System tab
            PopulateCurrentSystem(_context.Data.CurrentCommander);

            // Search tab
            // TODO: Show Commander as well in result?
            PopulateCommandersList();
            lblFindMessages.Text = string.Empty;
            lbJournals.Items.Clear();

            SetMessage(msg);
        }

        public void PopulateCommandersList()
        {
            if (_context.IsReadAll) return;

            cboCommanderFilter.Items.Clear();

            cboCommanderFilter.SelectedIndex = cboCommanderFilter.Items.Add("(All)");
            foreach (var c in _context.Data.KnownCommanders.Keys)
            {
                cboCommanderFilter.Items.Add(c);
            }
        }

        public void PopulateCurrentSystem(string commander = "")
        {
            if (_context.IsReadAll) return;

            var cmdrData = _context.Data.ForCommander(commander);
            if (string.IsNullOrWhiteSpace(_context.Data.CurrentCommander) || cmdrData == null || cmdrData.CurrentSystem == null)
            {
                // Current System tab
                txtSystemName.Text = string.Empty;
                lblRecordCommanderValue.Text = string.Empty;
                txtLastEntry.Text = string.Empty;

                if (cmdrData == null)
                    SetMessage("Current commander is unknown.");
                else
                    SetMessage("Current system is unknown.");
                return;
            }

            txtSystemName.Text = cmdrData.CurrentSystem.SystemName;
            lblRecordCommanderValue.Text = cmdrData.CommanderName;
            PopulateLatestRecord(cmdrData);
        }

        public void SetMessage(string message = "")
        {
            if (_context.IsReadAll) return;

            if (!string.IsNullOrWhiteSpace(message))
                txtMessages.Text = message;
        }

        public void SetFindMessage(string message = "")
        {
            if (_context.IsReadAll) return;

            if (!string.IsNullOrWhiteSpace(message))
                lblFindMessages.Text = message;
        }

        public void ClearMessage()
        {
            if (_context.IsReadAll) return;

            txtMessages.Text = string.Empty;
            lblFindMessages.Text = string.Empty;
        }


        internal void PopulateLatestRecord(ArchivistCommanderData data = null)
        {
            if (_context.IsReadAll) return;

            var cmdrData = data ?? _context.Data.ForCommander();
            if (cmdrData == null) return;

            string rawJson = cmdrData.CurrentSystem.SystemJournalEntries.Last();
            string prettyPrintJson = PrettyPrintJson(rawJson);
            txtLastEntry.Text = prettyPrintJson;
        }

        private string PrettyPrintJson(string rawJson)
        {
            JsonSerializerOptions opts = new()
            {
                WriteIndented = true,
                AllowTrailingCommas = true,
            };
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(rawJson);

            return JsonSerializer.Serialize(jsonElement, opts);
        }

        private void DoSearch()
        {
            ClearMessage();
            lbJournals.Items.Clear();

            VisitedSystem result = null;
            if (cboCommanderFilter.SelectedIndex > 0)
            {
                result = _context.Manager.Get(txtFindSystem.Text, cboCommanderFilter.SelectedItem.ToString());
                if (result != null) SetFindMessage($"Match found with {result.SystemJournalEntries.Count} records");
            }
            else
            {
                var results = _context.Manager.Get(txtFindSystem.Text);
                if (results.Count > 0)
                {
                    result = results[0];
                    if (results.Count > 1)
                    {
                        SetFindMessage($"Results for multiple commanders found; showing result with {result.SystemJournalEntries.Count} from {result.Commander}");
                    }
                    else
                    {
                        SetFindMessage($"Match found with {result.SystemJournalEntries.Count} records");
                    }
                }
            }

            if (result == null)
            {
                ClearSearchResult();
                SetFindMessage("Nothing found");
                return;
            }

            _lastResult = result;
        }

        public void ClearSearchResult()
        {
            _lastResult = null;
            lbJournals.Items.Clear();
            ClearMessage();
        }

        private void PopulateSearchResult()
        {
            if (_lastResult == null) return;

            lbJournals.Items.Clear();
            foreach (var journal in _lastResult.SystemJournalEntries)
            {
                if (string.IsNullOrEmpty(txtFilter.Text) || journal.Contains(txtFilter.Text.Trim()))
                    lbJournals.Items.Add(journal);
            }
        }

        private void OpenJsonViewer(string contents)
        {
            if (_viewer == null || _viewer.IsDisposed)
            {
                _viewer = new JsonViewer();
                _context.Core.RegisterControl(_viewer);
                _viewer.FormClosed += _viewer_FormClosed;

                _viewer.ViewJson(contents);

                _viewer.Show();
            }
            else
            {
                _viewer.ViewJson(contents);
            }
        }

        private void txtFindSystem_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != Convert.ToChar(Keys.Enter))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFindSystem.Text))
            {
                ClearSearchResult();
                return;
            }

            DoSearch();
            PopulateSearchResult();
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            _filterTimer.Start();
        }

        private void _filterTimer_Tick(object sender, EventArgs e)
        {
            _filterTimer.Stop();
            PopulateSearchResult();
        }

        private void cboCommanderFilter_SelectedIndexChanged(object sender, EventArgs e)
        {

            DoSearch();
            PopulateSearchResult();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_selectedIndex == ListBox.NoMatches || _selectedIndex > lbJournals.Items.Count) return;
            string journalEntry = lbJournals.Items[_selectedIndex].ToString();

            Clipboard.SetText(journalEntry);
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_selectedIndex == ListBox.NoMatches || _selectedIndex > lbJournals.Items.Count) return;
            string journalEntry = lbJournals.Items[_selectedIndex].ToString();

            OpenJsonViewer(journalEntry);
        }

        private void _viewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            _context.Core.UnregisterControl(_viewer);
            _viewer = null;
        }

        private void btnResendAll_Click(object sender, EventArgs e)
        {
            // Implement this on Context?
            foreach (var item in lbJournals.Items)
            {
                string json = item.ToString();
                _context.Core.DeserializeEvent(json, true);
            }
        }

        private void lbJournals_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                if (ctxResultsMenu.Visible) ctxResultsMenu.Visible = false;
                return;
            }
            var index = lbJournals.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                _selectedIndex = index;
                lbJournals.SelectedIndex = index;
                ctxResultsMenu.Show(Cursor.Position);
                ctxResultsMenu.Visible = true;
            }
            else
            {
                ctxResultsMenu.Visible = false;
            }
        }

        private void btnOpenInSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSystemName.Text)) return;

            txtFindSystem.Text = txtSystemName.Text;
            tabPanels.SelectedTab = tabSearch;

            DoSearch();
            PopulateSearchResult();
        }

        private void btnOpenInViewer_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLastEntry.Text)) return;

            OpenJsonViewer(txtLastEntry.Text);
        }
    }
}
