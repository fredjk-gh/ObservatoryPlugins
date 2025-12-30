using com.github.fredjk_gh.PluginCommon.Data.Id64;
using com.github.fredjk_gh.PluginCommon.Data.Journals;
using com.github.fredjk_gh.PluginCommon.Data.Spansh.System;
using com.github.fredjk_gh.PluginCommon.PluginInterop.Messages;
using com.github.fredjk_gh.PluginCommon.UI.Shared;
using Observatory.Framework.Files.Journal;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using static com.github.fredjk_gh.PluginCommon.UI.ThemeableImageLabel;

namespace com.github.fredjk_gh.ObservatoryArchivist.UI
{
    public partial class ArchivistUI : UserControl
    {
        private JsonSerializerOptions PRETTY_PRINT_OPTIONS = new()
        {
            WriteIndented = true,
            AllowTrailingCommas = true,
        };

        private ArchivistContext _context;
        private System.Windows.Forms.Timer _filterTimer = new();
        private JsonViewer _viewer;

        internal ArchivistUI(ArchivistContext context)
        {
            _context = context;

            InitializeComponent();

            // TODO: convert to Image buttons and update resources.
            btnOpenInViewer.SetIcon(Properties.Resources.OpenInNewIcon.ToBitmap(), new(32, 32));
            btnOpenInSearch.SetIcon(Properties.Resources.SearchIcon.ToBitmap(), new(32, 32));
            btnSearchDB.SetIcon(Properties.Resources.DBSearchIcon.ToBitmap(), new(32, 32));
            btnLoadFromSpansh.SetIcon(Properties.Resources.SpanshIcon.ToBitmap(), new(32, 32));
            btnResendAll.SetIcon(Properties.Resources.ReplayAllIcon.ToBitmap(), new(32, 32));
            btnCopy.SetIcon(Properties.Resources.CopyIcon.ToBitmap(), new(32, 32));
            btnView.SetIcon(Properties.Resources.OpenInNewIcon.ToBitmap(), new(32, 32));

            btnSendViaMsg.OriginalImage = PluginCommon.Images.ReplayViaMessageImage;
            btnSendViaMsg.ImageSize = new(32, 32);

            btnMessagesClear.OriginalImage = PluginCommon.Images.RouteClearImage;
            btnMessagesClear.ImageSize = new(32, 32);
            btnFindMessagesClear.OriginalImage = PluginCommon.Images.RouteClearImage;
            btnFindMessagesClear.ImageSize = new(32, 32);

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
            PopulateRecentSystems();
            txtFindMessages.Text = string.Empty;
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
                txtId64Details.Text = string.Empty;

                if (cmdrData == null)
                    SetMessage("Current commander is unknown.");
                else
                    SetMessage("Current system is unknown.");
                return;
            }

            txtSystemName.Text = cmdrData.CurrentSystem.SystemName;
            lblRecordCommanderValue.Text = cmdrData.CommanderName;
            PopulateLatestEntry(cmdrData);
            PopulateId64Details(cmdrData);
        }

        public void PopulateRecentSystems()
        {
            if (_context.IsReadAll) return;

            if (_context.Data.RecentSystems.Count == 0)
            {
                _context.Data.RecentSystems = _context.Archive.GetRecentVisitedsystems();
            }

            lbRecentSystems.Items.Clear();
            foreach (var item in _context.Data.RecentSystems)
            {
                lbRecentSystems.Items.Add(item);
            }
        }

        public void SetMessage(string message, TextBox txtBox = null)
        {
            if (_context.IsReadAll) return;

            if (txtBox == null) txtBox = txtMessages;

            if (!string.IsNullOrWhiteSpace(message))
                txtBox.Text = message
                    + (string.IsNullOrEmpty(txtBox.Text) ? "" : Environment.NewLine + txtBox.Text);
        }

        public void ClearMessage()
        {
            if (_context.IsReadAll) return;

            txtMessages.Text = string.Empty;
            txtFindMessages.Text = string.Empty;
        }


        internal void PopulateLatestEntry(ArchivistCommanderData data = null)
        {
            if (_context.IsReadAll) return;

            var cmdrData = data ?? _context.Data.ForCommander();
            if (cmdrData == null) return;

            string rawJson = cmdrData.CurrentSystem.SystemJournalEntries.Last();
            string prettyPrintJson = PrettyPrintJson(rawJson);
            txtLastEntry.Text = prettyPrintJson;
        }

        internal void PopulateId64Details(ArchivistCommanderData data = null)
        {
            if (_context.IsReadAll) return;

            var cmdrData = data ?? _context.Data.ForCommander();
            if (cmdrData == null) return;

            txtId64Details.Text = Id64Details.FromId64(cmdrData.CurrentSystem.SystemId64).ToString();
        }

        private string PrettyPrintJson(string rawJson)
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(rawJson);

            return JsonSerializer.Serialize(jsonElement, PRETTY_PRINT_OPTIONS);
        }

        private void DoSearch()
        {
            ClearMessage();

            string searchText = cboFindSystem.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(searchText) || searchText.Length < 3)
            {
                SetMessage("Search canceled: search text is empty or less than 3 characters.", txtFindMessages);
                return;
            }

            ClearSearchResult();

            ulong id64 = 0;
            string cmdrName = "";
            if (cboCommanderFilter.SelectedIndex > 0)
            {
                cmdrName = cboCommanderFilter.SelectedItem.ToString();
            }

            List<VisitedSystem> results = null;
            VisitedSystem result = null;
            string resultMessage = "";
            if (UInt64.TryParse(searchText, out id64))
            {
                // Search by system ID (and/or commander)
                results = _context.Archive.GetVisitedSystem(id64, cmdrName);
            }
            else
            {
                results = _context.Archive.GetVisitedSystem(searchText, cmdrName);
            }

            if (results.Count > 0)
            {
                result = results[0];
            }
            else
            {
                SetMessage("Nothing found", txtFindMessages);
                return;
            }

            if (results.Count > 1)
                resultMessage = $"Results for multiple commanders found; showing result with {result.SystemJournalEntries.Count} records from {result.Commander}";
            else
                resultMessage = $"Match found with {result.SystemJournalEntries.Count} records";

            SetMessage(resultMessage, txtFindMessages);
            _context.Data.LastSearchResult = result;
        }

        public void ClearSearchResult()
        {
            _context.Data.LastSearchResult = null;
            lbJournals.Items.Clear();
            btnLoadFromSpansh.Enabled = false;
            btnView.Enabled = false;
            ClearMessage();
        }

        private void PopulateSearchResult()
        {
            if (_context.Data.LastSearchResult == null) return;

            lbJournals.Items.Clear();
            var filterText = txtFilter.Text?.ToLowerInvariant().Trim() ?? "";
            foreach (var journal in _context.Data.LastSearchResult.SystemJournalEntries)
            {
                if (string.IsNullOrEmpty(filterText) || journal.Contains(filterText, StringComparison.OrdinalIgnoreCase))
                    lbJournals.Items.Add(journal);
            }
            btnLoadFromSpansh.Enabled = true;
        }

        private void OpenJsonViewer(string contents)
        {
            if (_viewer == null || _viewer.IsDisposed)
            {
                _viewer = new JsonViewer(_context);
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

        private async void AugmentFromSpansh(UInt64 systemId)
        {
            var url = $"https://spansh.co.uk/api/dump/{systemId}";

            var task = _context.Core.HttpClient.GetStringAsync(url);
            string spanshJson = string.Empty;

            try
            {
                await task;
                spanshJson = task.Result;
            }
            catch (Exception ex)
            {
                SetMessage($"Spansh data fetch failed: {ex.Message}{Environment.NewLine}Spansh may not know this system.",
                    txtFindMessages);
            }

            string status = "Fetch complete; parsing response...";
            SetMessage(status, txtFindMessages);

            try
            {
                var processTask = Task.Run(() =>
                {
                    return ParseSpanshSystemDump(spanshJson);
                });
                await processTask;

                SpanshSystem result = processTask.Result;
                status = "Spansh response parsed, converting...";
                SetMessage(status, txtFindMessages);

                // Convert to Framework objects and serialize to Journal events.
                VisitedSystem lastSearch = _context.Data.LastSearchResult;
                var convertTask = Task.Run(() =>
                {
                    return JournalGenerator.FromSpansh(result, lastSearch.FirstVisitDateTime);
                });
                await convertTask;

                List<JournalBase> augmentedJournals = convertTask.Result;
                VisitedSystem augmented = JournalUtilities.AugmentJournals(lastSearch, augmentedJournals);
                _context.Archive.UpsertAugmentedSystem(augmented);

                ClearSearchResult();
                _context.Data.LastSearchResult = augmented;
                PopulateSearchResult();
                status = $"Conversion complete; {augmentedJournals.Count} journal entries displayed; data cached";
                SetMessage(status, txtFindMessages);
            }
            catch (Exception ex)
            {
                SetMessage($"Failed to parse Spansh data: {ex.Message}", txtFindMessages);
            }
        }


        private SpanshSystem ParseSpanshSystemDump(string spanshJson)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
            };
            var deserialized = JsonSerializer.Deserialize<SpanshSystemDump>(spanshJson, options);

            return deserialized.System;
        }

        private void DoCopy()
        {
            StringBuilder journals = new();

            if (lbJournals.SelectedItems.Count == 0)
            {
                foreach (var journal in lbJournals.Items)
                {
                    journals.AppendLine(journal.ToString());
                }
            }
            else
            {
                foreach (var journal in lbJournals.SelectedItems)
                {
                    journals.AppendLine(journal.ToString());
                }
            }

            try
            {
                Clipboard.SetText(journals.ToString());
            }
            catch (Exception ex) { } // Ignore.
        }

        private void DoOpenViewer()
        {
            if (lbJournals.SelectedItems.Count != 1) return;

            foreach (var journal in lbJournals.SelectedItems)
            {
                string journalEntry = journal.ToString();

                OpenJsonViewer(journalEntry);
                break;
            }
        }

        private void btnSearchDB_Click(object sender, EventArgs e)
        {
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
            DoCopy();
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoOpenViewer();
        }

        private void _viewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            _context.Core.UnregisterControl(_viewer);
            _viewer = null;
        }

        private void btnResendAll_Click(object sender, EventArgs e)
        {
            // Implement this on Context?
            _context.SetResendAll(true);
            foreach (var item in lbJournals.Items)
            {
                string json = item.ToString();
                _context.Core.DeserializeEvent(json, true);
            }
            _context.SetResendAll(false);
        }

        private void btnSendViaMsg_Click(object sender, EventArgs e)
        {
            if (_context.Data.LastSearchResult == null) return;

            CurrentSystemInfo systemInfo = new(_context.Data.LastSearchResult);
            List<JournalBase> preamble = ArchivistData.ToJournalObj(_context.Core, systemInfo.PreambleJournalEntries);
            List<JournalBase> systemJournals = ArchivistData.ToJournalObj(_context.Core, systemInfo.SystemJournalEntries);

            if (!ArchivistData.IsSystemScanComplete(preamble, systemJournals))
            {
                _context.UI.SetMessage($"WARNING: Search result data does not represent a incomplete system scan!", txtFindMessages);
            }

            ArchivistScansMessage msg = ArchivistScansMessage.New(
                systemInfo.SystemName,
                systemInfo.SystemId64,
                preamble,
                systemJournals,
                false,
                systemInfo.Commander,
                systemInfo.VisitCount);

            _context.Core.SendPluginMessage(_context.PluginWorker, msg.ToPluginMessage());
        }

        private void lbJournals_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    var index = lbJournals.IndexFromPoint(e.Location);
                    if (index != ListBox.NoMatches)
                    {
                        lbJournals.SelectedIndices.Add(index);
                    }

                    viewToolStripMenuItem.Enabled = (lbJournals.SelectedItems.Count == 1);
                    btnView.Enabled = (lbJournals.SelectedItems.Count == 1);

                    ctxResultsMenu.Show(Cursor.Position);
                    ctxResultsMenu.Visible = !ctxResultsMenu.Visible;
                    break;
                default:
                    if (ctxResultsMenu.Visible) ctxResultsMenu.Visible = false;
                    break;
            }
        }

        private void btnOpenInSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSystemName.Text)) return;

            cboFindSystem.Text = txtSystemName.Text;
            tabPanels.SelectedTab = tabSearch;

            DoSearch();
            PopulateSearchResult();
        }

        private void btnOpenInViewer_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLastEntry.Text)) return;

            OpenJsonViewer(txtLastEntry.Text);
        }

        private void lbRecentSystems_DoubleClick(object sender, EventArgs e)
        {
            if (lbRecentSystems.SelectedIndex < 0 || string.IsNullOrWhiteSpace(lbRecentSystems.SelectedItem?.ToString())) return;

            cboFindSystem.Text = lbRecentSystems.SelectedItem?.ToString();

            DoSearch();
            PopulateSearchResult();
        }

        private void btnLoadFromSpansh_Click(object sender, EventArgs e)
        {
            if (_context.Data.LastSearchResult == null) return;

            btnLoadFromSpansh.Enabled = false;
            VisitedSystem? augmentedSystem = _context.Archive.GetExactMatchAugmentedSystem(_context.Data.LastSearchResult.SystemId64);
            if (augmentedSystem == null)
            {
                SetMessage($"Fetching data for '{_context.Data.LastSearchResult.SystemName}' from Spansh...", txtFindMessages);
                AugmentFromSpansh(_context.Data.LastSearchResult.SystemId64);
            }
            else
            {
                ClearSearchResult();
                _context.Data.LastSearchResult = augmentedSystem;
                PopulateSearchResult();
                SetMessage($"Data previously fetched from Spansh loaded; {augmentedSystem.SystemJournalEntries.Count} journal entries displayed", txtFindMessages);
            }
            btnLoadFromSpansh.Enabled = true;
        }

        private void cboFindSystem_TextUpdate(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cboFindSystem.Text) || cboFindSystem.Text.Length < 3)
            {
                // Don't trigger for empty/short values.
                autoCompleteFetchTimer.Stop();
                cboFindSystem.DroppedDown = false;
                return;
            }

            // User typed something non-trivial, reset timer.
            if (autoCompleteFetchTimer.Enabled)
                autoCompleteFetchTimer.Stop();
            autoCompleteFetchTimer.Start();
        }

        private void autoCompleteFetchTimer_Tick(object sender, EventArgs e)
        {
            autoCompleteFetchTimer.Stop();

            try
            {
                var commanderName = "";
                if (cboCommanderFilter.SelectedIndex > 0)
                {
                    commanderName = cboCommanderFilter.SelectedItem.ToString();
                }

                var search = cboFindSystem.Text;
                var autoCompleteItems = _context.Archive.FindVisitedSystemNames(search, commanderName);

                if (autoCompleteItems.Count > 0)
                {
                    cboFindSystem.DataSource = autoCompleteItems;
                    cboFindSystem.SelectedItem = null;
                    cboFindSystem.DroppedDown = true;

                    cboFindSystem.Text = search;
                    cboFindSystem.SelectionStart = search.Length;

                    Cursor.Current = Cursors.Default;
                }
                else
                {
                    cboFindSystem.DroppedDown = false;
                    cboFindSystem.SelectionStart = search.Length;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Autocomplete item update failed: {ex.ToString()}");
            }
        }

        private void cboFindSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            autoCompleteFetchTimer.Stop();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            DoCopy();
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            DoOpenViewer();
        }

        private void lbJournals_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (ctxResultsMenu.Visible) ctxResultsMenu.Visible = false;

                    viewToolStripMenuItem.Enabled = lbJournals.SelectedItems.Count == 1;
                    btnView.Enabled = lbJournals.SelectedItems.Count == 1;

                    break;
            }
        }

        private void lbJournals_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    if (e.Modifiers == Keys.Control)
                    {
                        lbJournals.SelectedItems.Clear();
                        for (int i = lbJournals.Items.Count - 1; i >= 0; i--)
                        {
                            lbJournals.SelectedIndices.Add(i);
                        }
                        btnView.Enabled = false;
                        e.Handled = true;
                    }
                    break;
                case Keys.C:
                    if (e.Modifiers == Keys.Control)
                    {
                        DoCopy();
                        e.Handled = true;
                    }

                    break;
            }
        }

        private void btnMessagesClear_Click(object sender, EventArgs e)
        {
            txtMessages.Text = string.Empty;
        }

        private void btnFindMessagesClear_Click(object sender, EventArgs e)
        {
            txtFindMessages.Text = string.Empty;
        }
    }
}
