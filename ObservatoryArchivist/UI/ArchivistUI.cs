using com.github.fredjk_gh.ObservatoryArchivist.DB;
using com.github.fredjk_gh.PluginCommon;
using com.github.fredjk_gh.PluginCommon.Data.EdGIS;
using com.github.fredjk_gh.PluginCommon.Data.Id64;
using com.github.fredjk_gh.PluginCommon.PluginInterop.Messages;
using com.github.fredjk_gh.PluginCommon.UI;
using com.github.fredjk_gh.PluginCommon.UI.Forms;
using com.github.fredjk_gh.PluginCommon.UI.Shared;
using com.github.fredjk_gh.PluginCommon.Utilities;
using Observatory.Framework.Files.Journal;
using System.Diagnostics;
using System.Text;
using static com.github.fredjk_gh.PluginCommon.PluginInterop.PluginTracker;

namespace com.github.fredjk_gh.ObservatoryArchivist.UI
{
    public partial class ArchivistUI : UserControl
    {
        private readonly ArchivistContext _c;
        private readonly System.Windows.Forms.Timer _journalFilterTimer = new();
        private readonly System.Windows.Forms.Timer _notifFilterTimer = new();
        private JsonViewer _viewer;
        private Id64Viewer _id64ViewerForm;
        private SizeF? _lastScaleFactor = null;

        private readonly ThemeableImageButton btnOpenInSearch;
        private readonly ThemeableImageButton btnMessagesClear;
        private readonly ThemeableImageButton btnSearchDB;
        private readonly ThemeableImageButton btnLoadFromSpansh;
        private readonly ThemeableImageButton btnRefreshStatusTable;
        private readonly ThemeableImageButton btnSearchAll;
        private readonly ThemeableImageButton btnClearAll;
        private readonly ThemeableImageButton btnShareNotifs;
        private readonly ThemeableImageButton btnOpenId64;
        private readonly ThemeableImageButton btnLookup;
        private readonly ThemeableImageButton btnCacheValue;
        private readonly ThemeableImageButton btnCopyCoordinates;
        private readonly ThemeableImageLabel tlblLookupCoords;
        private readonly ThemeableImageLabel tlblCoordinates;

        private readonly ImageSpec IMG_POSITION = new(Images.PositionImage)
        {
            Size = new(24, 24),
            Tag = "position",
            ToolTip = "Coordinates",
            Visible = true,
        };

        #region Initialization
        internal ArchivistUI(ArchivistContext context)
        {
            InitializeComponent();
            _c = context;

            SuspendLayout();

            // These are all manually created/positioned because the designer kept... "forgetting" them.
            // Maybe a bug in the designer integration? Ah well, here for now.
            // Status area
            btnOpenInSearch = ThemeableImageButton.New(Images.MagnifyingGlassImage, BtnOpenInSearch_Click);
            ttipArchivistUI.SetToolTip(btnOpenInSearch, "Open this system in the Archivist search area");
            tlpStatus.Controls.Add(btnOpenInSearch, 2, 0);

            btnRefreshStatusTable = ThemeableImageButton.New(Images.RefreshImage, BtnRefreshStatusTable_Click);
            ttipArchivistUI.SetToolTip(btnRefreshStatusTable, "Refresh the system status data");
            tlpStatus.Controls.Add(btnRefreshStatusTable, 2, 1);

            btnMessagesClear = ThemeableImageButton.New(Images.RouteClearImage, BtnMessagesClear_Click);
            ttipArchivistUI.SetToolTip(btnMessagesClear, "Clear messages");
            tlpStatus.Controls.Add(btnMessagesClear, 2, 3);

            // Search area -- common
            btnSearchAll = ThemeableImageButton.New(Images.DatabaseSearchImage, BtnSearchAll_Click);
            ttipArchivistUI.SetToolTip(btnSearchAll, "Search all caches for data related to this system");
            flpSearch.Controls.Add(btnSearchAll);

            btnClearAll = ThemeableImageButton.New(Images.RouteClearImage, BtnClearAll_Click);
            ttipArchivistUI.SetToolTip(btnClearAll, "Clear all search results");
            flpSearch.Controls.Add(btnClearAll);

            // Search area -- Journals
            btnSearchDB = ThemeableImageButton.New(Images.DatabaseSearchImage, BtnSearchDB_Click);
            ttipArchivistUI.SetToolTip(btnSearchDB, "Re-search collected and augmented data for this system");
            flpJournalTools.Controls.Add(btnSearchDB);

            btnLoadFromSpansh = ThemeableImageButton.New(Images.SpanshImage, BtnLoadFromSpansh_Click);
            ttipArchivistUI.SetToolTip(btnLoadFromSpansh, "Fetch and cache data from Spansh and synthesize journals for this system");
            flpJournalTools.Controls.Add(btnLoadFromSpansh);

            Color ctxMenuForeColor = copyToolStripMenuItem.ForeColor;
            copyToolStripMenuItem.Image = ImageCommon.RecolorAndSizeImage(Images.CopyImage, ctxMenuForeColor);
            viewToolStripMenuItem.Image = ImageCommon.RecolorAndSizeImage(Images.OpenInNewImage, ctxMenuForeColor);
            resendAllEventsToolStripMenuItem.Image = ImageCommon.RecolorAndSizeImage(Images.ReplayImage, ctxMenuForeColor);
            viaMessageBusToolStripMenuItem.Image = ImageCommon.RecolorAndSizeImage(Images.ReplayViaMessageImage, ctxMenuForeColor);
            viaCoreJournalEventsToolStripMenuItem.Image = ImageCommon.RecolorAndSizeImage(Images.WarningSignImage, ctxMenuForeColor);

            // Search area -- notifications
            btnShareNotifs = ThemeableImageButton.New(Images.ReplayViaMessageImage, BtnShareNotifs_Click);
            ttipArchivistUI.SetToolTip(btnShareNotifs, "Share cached notifications via plugin message");
            flpNotifSearchTools.Controls.Add(btnShareNotifs);

            // Search area -- Pos Cache
            btnOpenId64 = ThemeableImageButton.New(Images.Id64Image, BtnOpenId64_Click);
            ttipArchivistUI.SetToolTip(btnOpenId64, "View information derived from the System Id64");
            flpAddrCacheTools.Controls.Add(btnOpenId64);

            btnLookup = ThemeableImageButton.New(Images.CloudSearchImage, BtnLookup_Click);
            ttipArchivistUI.SetToolTip(btnLookup, "Lookup coordinates via EdGIS");
            flpAddrCacheTools.Controls.Add(btnLookup);
            
            btnCacheValue = ThemeableImageButton.New(Images.SaveImage, BtnCacheValue_Click);
            ttipArchivistUI.SetToolTip(btnCacheValue, "Save the lookup results to the cache");
            flpAddrCacheTools.Controls.Add(btnCacheValue);


            btnCopyCoordinates = ThemeableImageButton.New(Images.CopyImage, BtnCopyCoordinates_Click);
            btnCopyCoordinates.Location = new Point(
                Convert.ToInt32(245 * _lastScaleFactor?.Width ?? 1),
                Convert.ToInt32(56 * _lastScaleFactor?.Height ?? 1));
            ttipArchivistUI.SetToolTip(btnCopyCoordinates, "Copy coordinates to the system clipboard");
            pAddressCache.Controls.Add(btnCopyCoordinates);

            tlblCoordinates = new()
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ImageColor = Color.Blue, // Transparent
                LabelPosition = LabelPositionType.Right,
                ToolTipManager = ttipArchivistUI,
                Location = new Point(
                    Convert.ToInt32(8 * _lastScaleFactor?.Width ?? 1),
                    Convert.ToInt32(56 * _lastScaleFactor?.Height ?? 1)),
                Text = "x, y, z",
            };
            tlblCoordinates.AddImage(IMG_POSITION);
            pAddressCache.Controls.Add(tlblCoordinates);

            tlblLookupCoords = new()
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ImageColor = Color.Transparent,
                LabelPosition = LabelPositionType.Right,
                ToolTipManager = ttipArchivistUI,
                Location = new Point(
                    Convert.ToInt32(8 * _lastScaleFactor?.Width ?? 1),
                    Convert.ToInt32(56 * _lastScaleFactor?.Height ?? 1)),
                Text = "x, y, z",
            };
            tlblLookupCoords.AddImage(IMG_POSITION);
            pLookupResult.Controls.Add(tlblLookupCoords);

            ResumeLayout();
            Draw();

            dgvNotifSearchResults.BackgroundColorChanged += DataGridView_AColourChanged;
            dgvNotifSearchResults.ForeColorChanged += DataGridView_AColourChanged;

            _journalFilterTimer.Tick += JournalFilterTimer_Tick;
            _journalFilterTimer.Interval = 150;
            _notifFilterTimer.Tick += NotifFilterTimer_Tick;
            _notifFilterTimer.Interval = 150;
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            // Capture the scale factor and save it so when we add things at runtime after initial layout has
            // occurred, we can correctly position/size them. (Designer sizes != run-time sizes if you have an HDPI display).
            // 
            // For such a simple thing, I spent far too long figuring it out. And all because of the bugs in the designer
            // that resulted in it constantly wiping my custom controls.
            _lastScaleFactor = factor;
            base.ScaleControl(factor, specified);
        }

        internal void Draw(string msg = "")
        {
            if (_c.IsReadAll) return;

            // Status group
            PopulateCurrentSystemAndCommander(_c.Data.CurrentCommander);

            // Search group
            PopulateCommandersList();
            PopulateRecentSystems();

            PopulateSearchResults();

            PopulatePositionCacheDetails();
            PopulateOnlineLookupResult();

            SetMessage(msg);
        }
        #endregion

        #region Status Group - Public
        internal void PopulateCommandersList()
        {
            if (_c.IsReadAll) return;

            cboCommanderFilter.Items.Clear();

            cboCommanderFilter.SelectedIndex = cboCommanderFilter.Items.Add("(All)");
            foreach (var c in _c.Data.KnownCommanders.Keys)
            {
                cboCommanderFilter.Items.Add(c);
            }
        }

        public void PopulateCurrentSystemAndCommander(string commander = "")
        {
            if (_c.IsReadAll) return;

            var cmdrData = _c.Data.ForCommander(commander);
            if (string.IsNullOrWhiteSpace(_c.Data.CurrentCommander) || cmdrData == null || cmdrData.CurrentSystem == null)
            {
                // Status group
                txtSystemName.Text = string.Empty;
                lblCurrentCommanderValue.Text = string.Empty;

                if (cmdrData == null)
                    SetMessage("Current commander is unknown.");
                else
                    SetMessage("Current system is unknown.");
                return;
            }

            txtSystemName.Text = cmdrData.CurrentSystem.SystemName;
            lblCurrentCommanderValue.Text = cmdrData.CommanderName;

            PopulateSystemDBStatus();
        }

        private void PopulateSystemDBStatus()
        {
            try
            {
                lvSystemDbSummary.Items.Clear();
                if (_c.Data.ForCommander()?.CurrentSystem is null)
                {
                    return;
                }

                var sysData = _c.Data.ForCommander().CurrentSystem;

                // Collected Journals (for this cmdr)
                var collectedJournals = _c.Journals.GetVisitedSystem(sysData.SystemId64, _c.Data.CurrentCommander);
                ListViewItem itemCollectedJournals = lvSystemDbSummary.Items.Add("Collected Journals");
                if (collectedJournals is not null)
                {
                    itemCollectedJournals.SubItems.Add($"{collectedJournals.SystemJournalEntries.Count:#,###} entries");
                    bool completeScan = ArchivistData.IsSystemScanComplete(ArchivistData.ToJournalObj(_c.Core, collectedJournals.SystemJournalEntries));

                    ListViewItem itemCollectedJournalsComplete = lvSystemDbSummary.Items.Add("");
                    itemCollectedJournalsComplete.SubItems.Add($"Scan is {(completeScan ? "" : "in")}complete");
                }
                else
                    itemCollectedJournals.SubItems.Add($"no entries");

                // Augmented Journals
                var augmentedJournals = _c.Journals.GetExactMatchAugmentedSystem(sysData.SystemId64);
                ListViewItem itemAugmentedJournals = lvSystemDbSummary.Items.Add("Augmented Journals");
                if (augmentedJournals is not null)
                {
                    itemAugmentedJournals.SubItems.Add($"{augmentedJournals.SystemJournalEntries.Count:#,###} entries");
                    bool completeScan = ArchivistData.IsSystemScanComplete(ArchivistData.ToJournalObj(_c.Core, augmentedJournals.SystemJournalEntries));

                    ListViewItem itemAugmentedJournalsComplete = lvSystemDbSummary.Items.Add("");
                    itemAugmentedJournalsComplete.SubItems.Add($"Scan is {(completeScan ? "" : "in")}complete");
                }
                else
                    itemAugmentedJournals.SubItems.Add($"no entries");

                // Notifications
                var notifications = _c.Notifications.GetNotifications(sysData.SystemId64, _c.Data.CurrentCommander);
                ListViewItem itemNotifications = lvSystemDbSummary.Items.Add("Notifications");
                if (notifications is not null && notifications.Count > 0)
                    itemNotifications.SubItems.Add($"{notifications.Count:#,##0} entries");
                else
                    itemNotifications.SubItems.Add($"no entries");

                // Address Cache
                var posCache = _c.PositionCache.GetSystem(sysData.SystemId64);
                ListViewItem itemPosCache = lvSystemDbSummary.Items.Add("Position Cache");
                if (posCache is not null)
                    itemPosCache.SubItems.Add($"cached");
                else
                    itemPosCache.SubItems.Add($"not cached!");

                // Future: bookmarks
            }
            catch (Exception ex)
            {
                SetMessage($"Error while updating status: {ex.Message}");
                _c.ErrorLogger(ex, "Archivist: Updating status display");
            }
        }

        public void SetMessage(string message)
        {
            if (_c.IsReadAll) return;

            _c.Core.ExecuteOnUIThread(() =>
            {
                if (!string.IsNullOrWhiteSpace(message))
                    txtMessages.Text = message
                        + (string.IsNullOrEmpty(txtMessages.Text) ? "" : Environment.NewLine + txtMessages.Text);
            });
        }

        public void ClearMessage()
        {
            if (_c.IsReadAll) return;

            txtMessages.Text = string.Empty;
        }
        #endregion

        #region Status Group - event handlers
        private void BtnOpenInSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSystemName.Text)) return;

            cboFindSystem.Text = txtSystemName.Text;
            cboCommanderFilter.SelectedItem = _c.Data.CurrentCommander;

            _c.Search.SetSearchContext(txtSystemName.Text, _c.Data.CurrentCommander);
            _c.Search.SearchAll();
            PopulateSearchResults();
        }

        private void BtnRefreshStatusTable_Click(object sender, EventArgs e)
        {
            PopulateSystemDBStatus();
        }

        private void BtnMessagesClear_Click(object sender, EventArgs e)
        {
            ClearMessage();
        }
        #endregion

        #region Search Group
        public void PopulateRecentSystems()
        {
            if (_c.IsReadAll) return;

            if (_c.Data.RecentSystems.Count == 0)
            {
                _c.Data.RecentSystems = _c.Journals.GetRecentVisitedsystems();
            }

            lbRecentSystems.Items.Clear();
            foreach (var item in _c.Data.RecentSystems)
            {
                lbRecentSystems.Items.Add(item);
            }
        }

        private void PopulateSearchResults()
        {
            // Journals tab
            PopulateJournalSearchResults();

            // Notifications tab
            PopulateNotificationResults();

            // Address Cache
            PopulatePositionCacheDetails();
            PopulateOnlineLookupResult();

            btnSearchAll.Enabled = _c.Search.MaySearch;
            btnClearAll.Enabled = _c.Search.HasResult;
        }

        public void ClearSearch()
        {
            _c.UI.ClearMessage();
            _c.Search.Clear();

            btnSearchAll.Enabled = _c.Search.MaySearch;
            btnClearAll.Enabled = _c.Search.HasResult;

            btnSearchDB.Enabled = false;
            btnLoadFromSpansh.Enabled = false;
            btnShareNotifs.Enabled = false;

            PopulateSearchResults();
        }

        private void OpenJsonViewer(string contents)
        {
            if (_viewer == null || _viewer.IsDisposed)
            {
                _viewer = new JsonViewer(_c);
                _c.Core.RegisterControl(_viewer);
                _viewer.FormClosed += JsonViewer_FormClosed;

                _viewer.ViewJson(contents);

                _viewer.Show();
            }
            else
            {
                _viewer.ViewJson(contents);
            }
        }

        private void BtnSearchAll_Click(object sender, EventArgs e)
        {
            ClearSearch();
            _c.Search.SearchAll();
            PopulateSearchResults();
        }


        private void JsonViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            _c.Core.UnregisterControl(_viewer);
            _viewer = null;
        }

        private void CboCommanderFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            _c.Search.SetSearchContext(cboFindSystem.Text, GetCommanderFilterValue());

            btnSearchAll.Enabled = _c.Search.MaySearch;
            btnClearAll.Enabled = _c.Search.HasResult;
        }

        private string GetCommanderFilterValue()
        {
            string filterValue = string.Empty;

            if (cboCommanderFilter.SelectedIndex >= 1)
            {
                filterValue = cboCommanderFilter.SelectedItem.ToString();
            }
            return filterValue;
        }

        private void LbRecentSystems_DoubleClick(object sender, EventArgs e)
        {
            if (lbRecentSystems.SelectedIndex < 0 || string.IsNullOrWhiteSpace(lbRecentSystems.SelectedItem?.ToString())) return;

            ClearSearch();

            cboFindSystem.Text = lbRecentSystems.SelectedItem?.ToString();
            _c.Search.SetSearchContext(cboFindSystem.Text, GetCommanderFilterValue());
            _c.Search.SearchAll();

            PopulateSearchResults();
        }
        #endregion

        #region Search group - shared - system search / auto complete

        private void CboFindSystem_TextUpdate(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cboFindSystem.Text) || cboFindSystem.Text.Length < 3)
            {
                // Don't trigger for empty/short values.
                autoCompleteFetchTimer.Stop();
                cboFindSystem.DroppedDown = false;

                _c.Search.SetSearchContext(cboFindSystem.Text, GetCommanderFilterValue());
                btnSearchAll.Enabled = _c.Search.MaySearch;
                btnClearAll.Enabled = _c.Search.HasResult;
                return;
            }

            // User typed something non-trivial, reset timer.
            if (autoCompleteFetchTimer.Enabled)
                autoCompleteFetchTimer.Stop();
            autoCompleteFetchTimer.Start();

            _c.Search.SetSearchContext(cboFindSystem.Text, GetCommanderFilterValue());
            btnSearchAll.Enabled = _c.Search.MaySearch;
            btnClearAll.Enabled = _c.Search.HasResult;
        }

        private void AutoCompleteFetchTimer_Tick(object sender, EventArgs e)
        {
            autoCompleteFetchTimer.Stop();

            try
            {
                var commanderName = GetCommanderFilterValue();

                var search = cboFindSystem.Text;
                var autoCompleteItems = _c.Journals.FindVisitedSystemNames(search, commanderName);

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
                Debug.WriteLine($"Autocomplete item update failed: {ex.Message}");
            }
        }

        private void CboFindSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            autoCompleteFetchTimer.Stop();
            _c.Search.SetSearchContext(cboFindSystem.Text, GetCommanderFilterValue());

            btnSearchAll.Enabled = _c.Search.MaySearch;
            btnClearAll.Enabled = _c.Search.HasResult;
        }

        private void CboFindSystem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ClearSearch();
                _c.Search.SetSearchContext(cboFindSystem.Text, GetCommanderFilterValue());
                _c.Search.SearchAll();
                PopulateSearchResults();
            }
        }

        private void BtnClearAll_Click(object sender, EventArgs e)
        {
            ClearSearch();
        }
        #endregion

        #region Search group - Journal

        private void PopulateJournalSearchResults()
        {
            PopulateJournalSearchResult(lbJournals, _c.Search.CollectedJournals, txtJournalFilter.Text);
            PopulateJournalSearchResult(lbAugmented, _c.Search.AugmentedJournals, txtJournalFilter.Text);

            btnSearchDB.Enabled = _c.Search.MaySearch;
            btnLoadFromSpansh.Enabled = (_c.Search.MaySearch && _c.Search.CollectedJournals != null);
        }

        private void PopulateJournalSearchResult(ListBox lb, VisitedSystem result, string filter = "")
        {
            lb.Items.Clear();

            if (result is null) return;

            var filterText = filter?.ToLowerInvariant().Trim() ?? "";
            foreach (var journal in result.SystemJournalEntries)
            {
                if (string.IsNullOrEmpty(filterText) || journal.Contains(filterText, StringComparison.OrdinalIgnoreCase))
                    lb.Items.Add(journal);
            }
        }

        private void BtnLoadFromSpansh_Click(object sender, EventArgs e)
        {
            if (_c.Search.CollectedJournals is null) return;

            CancellationTokenSource cts = new(10000);
            btnLoadFromSpansh.Enabled = false;

            Task.Run(() =>
            {
                try
                {
                    SetMessage($"Fetching data for '{_c.Search.CollectedJournals.SystemName}' from Spansh...");
                    var result = SpanshAugmenter.FetchAndCacheFromSpansh(_c, _c.Search.CollectedJournals, cts.Token);
                    if (result is not null && result.Result is not null)
                    {
                        // It's OK to block here.
                        _c.Search.AugmentedJournals = result.Result;
                        _c.UI.SetMessage($"Conversion complete; {_c.Search.AugmentedJournals.SystemJournalEntries.Count} journal entries displayed");
                        _c.Core.ExecuteOnUIThread(() =>
                        {
                            PopulateJournalSearchResults();
                        });
                    }
                    else
                    {
                        _c.UI.SetMessage("No result returned from Spansh");
                    }
                }
                catch (Exception ex)
                {
                    _c.UI.SetMessage($"Failed to load data from Spansh: {ex.Message}");
                    _c.Core.GetPluginErrorLogger(_c.Worker)(ex, "> LoadFromSpansh");
                }
                finally
                {
                    _c.Core.ExecuteOnUIThread(() =>
                    {
                        btnLoadFromSpansh.Enabled = true;
                    });
                }
            }, cts.Token);
        }

        private void DoCopy(ListBox lb)
        {
            StringBuilder journals = new();

            if (lb.SelectedItems.Count == 0)
            {
                foreach (var journal in lb.Items)
                {
                    journals.AppendLine(journal.ToString());
                }
            }
            else
            {
                foreach (var journal in lb.SelectedItems)
                {
                    journals.AppendLine(journal.ToString());
                }
            }

            Misc.SetTextToClipboard(journals.ToString());
        }

        private void DoOpenViewer(ListBox lb)
        {
            if (lb.SelectedItems.Count != 1) return;

            foreach (var journal in lb.SelectedItems)
            {
                string journalEntry = journal.ToString();

                OpenJsonViewer(journalEntry);
                break;
            }
        }

        private void BtnSearchDB_Click(object sender, EventArgs e)
        {
            if (!_c.Search.MaySearch) return;

            _c.Search.SearchJournals();
            PopulateJournalSearchResults();

            btnSearchAll.Enabled = _c.Search.MaySearch;
            btnClearAll.Enabled = _c.Search.HasResult;
        }

        private void TxtJournalFilter_TextChanged(object sender, EventArgs e)
        {
            _journalFilterTimer.Start();
        }

        private void JournalFilterTimer_Tick(object sender, EventArgs e)
        {
            _journalFilterTimer.Stop();
            PopulateJournalSearchResults();
        }

        #region Search Group - Journal - Listbox handlers
        private void ListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is not ListBox lb) return;

            switch (e.Button)
            {
                case MouseButtons.Right:
                    var index = lb.IndexFromPoint(e.Location);
                    if (index != ListBox.NoMatches)
                    {
                        lb.SelectedIndices.Add(index);
                    }

                    viewToolStripMenuItem.Enabled = (lb.SelectedItems.Count == 1);

                    ctxResultsMenu.Show(Cursor.Position);
                    ctxResultsMenu.Visible = !ctxResultsMenu.Visible;
                    break;
                default:
                    if (ctxResultsMenu.Visible) ctxResultsMenu.Visible = false;
                    break;
            }
        }

        private void ListBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (sender is not ListBox lb) return;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (ctxResultsMenu.Visible) ctxResultsMenu.Visible = false;

                    viewToolStripMenuItem.Enabled = lb.SelectedItems.Count == 1;
                    break;
            }
        }

        private void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not ListBox lb) return;

            switch (e.KeyCode)
            {
                case Keys.A:
                    if (e.Modifiers == Keys.Control)
                    {
                        lb.SelectedItems.Clear();
                        for (int i = lb.Items.Count - 1; i >= 0; i--)
                        {
                            lb.SelectedIndices.Add(i);
                        }
                        e.Handled = true;
                    }
                    break;
                case Keys.C:
                    if (e.Modifiers == Keys.Control)
                    {
                        DoCopy(lb);
                        e.Handled = true;
                    }

                    break;
            }
        }

        private void ListView_DoubleClick(object sender, EventArgs e)
        {
            if (sender is not ListBox lb) return;

            if (lb.SelectedIndex >= 0)
            {
                DoOpenViewer(lb);
            }
        }
        #endregion
        #endregion

        #region Search group - Notifications
        private void PopulateNotificationResults()
        {
            if (_c.IsReadAll) return;

            dgvNotifSearchResults.Rows.Clear();

            btnShareNotifs.Enabled = (_c.Search.Notifications is not null);
            if (_c.Search.Notifications is null) return;

            string filter = txtNotifFilter.Text.Trim();
            bool hasFilter = !string.IsNullOrEmpty(filter);

            dgvNotifSearchResults.Rows.Clear();
            foreach (var n in _c.Search.Notifications)
            {
                if (hasFilter && !IsNotificationIncluded(n, filter))
                    continue;

                DataGridViewRow row = new()
                {
                    DefaultCellStyle = new()
                    {
                        BackColor = dgvNotifSearchResults.BackgroundColor,
                        ForeColor = dgvNotifSearchResults.ForeColor,
                    }
                };

                row.Cells.Add(new DataGridViewTextBoxCell() { Value = n.Title });
                row.Cells.Add(new DataGridViewTextBoxCell() { Value = n.Detail });
                row.Cells.Add(new DataGridViewTextBoxCell() { Value = n.ExtendedDetails });
                row.Cells.Add(new DataGridViewTextBoxCell() { Value = n.Sender });

                dgvNotifSearchResults.Rows.Add(row);
            }
        }

        private bool IsNotificationIncluded(NotificationInfo n, string filter)
        {
            return ((!string.IsNullOrWhiteSpace(n.Title) && n.Title.Contains(filter, StringComparison.OrdinalIgnoreCase))
                || (!string.IsNullOrWhiteSpace(n.Detail) && n.Detail.Contains(filter, StringComparison.OrdinalIgnoreCase))
                || (!string.IsNullOrWhiteSpace(n.ExtendedDetails) && n.ExtendedDetails.Contains(filter, StringComparison.OrdinalIgnoreCase))
                || (!string.IsNullOrWhiteSpace(n.Sender) && n.Sender.Contains(filter, StringComparison.OrdinalIgnoreCase)));
        }

        private void DataGridView_AColourChanged(object sender, EventArgs e)
        {
            if (sender is not DataGridView dgv) return;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = dgv.BackgroundColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = dgv.ForeColor;
            dgv.RowHeadersDefaultCellStyle.BackColor = dgv.BackgroundColor;
            dgv.RowHeadersDefaultCellStyle.ForeColor = dgv.ForeColor;

            if (dgv == dgvNotifSearchResults) PopulateNotificationResults();
        }

        private void NotifFilterTimer_Tick(object sender, EventArgs e)
        {
            _notifFilterTimer.Stop();
            PopulateNotificationResults();
        }

        private void TxtNotifFilter_TextChanged(object sender, EventArgs e)
        {
            _notifFilterTimer.Start();
        }

        private void BtnShareNotifs_Click(object sender, EventArgs e)
        {
            if (_c.Search.Notifications is null || _c.Search.Notifications.Count == 0) return;

            var toShare = _c.Search.Notifications;
            var first = _c.Search.Notifications[0];

            var msg = ArchivistNotificationsMessage.New(
                first.SystemName,
                first.SystemId64,
                first.Commander,
                [.. toShare.Select(n => n.ToNotificationArgs())]);

            _c.Dispatcher.SendMessage(msg, PluginType.fredjk_Aggregator);
            _c.Core.ExecuteOnUIThread(() =>
            {
                _c.UI.SetMessage($"Shared {toShare.Count} notifications for {first.SystemName} from search result");
            });

        }
        #endregion

        #region Search group - Addr Cache
        internal void PopulatePositionCacheDetails()
        {
            if (_c.IsReadAll) return;

            lblCacheStatus.Text = "No result";
            pAddressCache.Visible = false;
            lblPosCacheSystemNameValue.Text = string.Empty;
            lblPosCacheId64Value.Text = string.Empty;
            tlblCoordinates.Text = string.Empty;

            btnOpenId64.Enabled = (_c.Search.PositionCache is not null);
            btnCopyCoordinates.Enabled = (_c.Search.PositionCache is not null);
            btnLookup.Enabled = (_c.Search.PositionCache is null
                && (_c.Search.MaySearch || _c.Search.CollectedJournals is not null));
            btnCacheValue.Enabled = (_c.Search.PositionCacheLookup is not null);

            if (_c.Search.PositionCache is null) return;

            lblCacheStatus.Text = "Cached result:";
            lblPosCacheSystemNameValue.Text = _c.Search.PositionCache.CommonName;
            lblPosCacheId64Value.Text = $"{_c.Search.PositionCache.Id64}";
            tlblCoordinates.Text = UIFormatter.Coordinates(_c.Search.PositionCache.x, _c.Search.PositionCache.y, _c.Search.PositionCache.z);
            pAddressCache.Visible = true;
        }

        private void PopulateOnlineLookupResult()
        {
            if (_c.IsReadAll) return;

            lblOnlineLookupResult.Visible = false;
            pLookupResult.Visible = false;

            btnCacheValue.Enabled = (_c.Search.PositionCacheLookup is not null);

            if (_c.Search.PositionCacheLookup is null) return;

            lblOnlineLookupResult.Text = "Online lookup result:";
            lblLookupSystemName.Text = _c.Search.PositionCacheLookup.SystemName;
            lblLookupId64.Text = $"{_c.Search.PositionCacheLookup.SystemId64}";
            tlblLookupCoords.Text = $"{_c.Search.PositionCacheLookup.Coords}";

            lblOnlineLookupResult.Visible = true;
            pLookupResult.Visible = true;
        }

        private void BtnOpenId64_Click(object sender, EventArgs e)
        {
            if (_id64ViewerForm is not null && !_id64ViewerForm.IsDisposed)
            {
                _id64ViewerForm.Activate();
            }
            else
            {
                _id64ViewerForm = new(Id64Details.FromId64(_c.Search.PositionCache.Id64), _c.Search.PositionCache.CommonName);
                _c.Core.RegisterControl(_id64ViewerForm);
                _id64ViewerForm.FormClosed += Id64Viewer_FormClosed;
                _id64ViewerForm.Show();
            }
        }

        private void Id64Viewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                _c.Core.UnregisterControl(_id64ViewerForm);
                _id64ViewerForm.Dispose();
            }
            catch { }
            finally
            {
                _id64ViewerForm = null;
            }
        }

        private void BtnLookup_Click(object sender, EventArgs e)
        {
            CancellationTokenSource cts = new(5000);

            string sysName = _c.Search.PositionCache?.CommonName ?? _c.Search.SearchSystemName ?? _c.Search.CollectedJournals?.SystemName;
            ulong? id64 = _c.Search.PositionCache?.Id64 ?? _c.Search.SearchId64 ?? _c.Search.CollectedJournals?.SystemId64;
            if (!id64.HasValue && string.IsNullOrWhiteSpace(sysName)) return;

            Task.Run(() =>
            {
                try
                {
                    _c.Search.PositionCacheLookup = EdGISHelper.LookupCoords(_c.Core.HttpClient, cts.Token, sysName, id64);

                    _c.Core.ExecuteOnUIThread(() =>
                    {
                        PopulateOnlineLookupResult();
                        btnCacheValue.Enabled = (_c.Search.PositionCacheLookup is not null);
                    });
                }
                catch (Exception ex)
                {
                    _c.UI.SetMessage($"Failed to lookup coordinates: {ex.Message}");
                }
            });
        }

        private void BtnCacheValue_Click(object sender, EventArgs e)
        {
            if (_c.Search.PositionCacheLookup is null) return;

            var toCache = _c.Search.PositionCacheLookup;
            SystemInfo toSave = new(toCache.SystemId64, toCache.SystemName, new()
            {
                x = toCache.Coords.X,
                y = toCache.Coords.Y,
                z = toCache.Coords.Z
            });
            _c.PositionCache.UpsertSystem(toSave);

            _c.Search.PositionCache = toSave;
            _c.Search.PositionCacheLookup = null;

            PopulatePositionCacheDetails();
            PopulateOnlineLookupResult();
        }

        private void BtnCopyCoordinates_Click(object sender, EventArgs e)
        {
            Misc.SetTextToClipboard(tlblCoordinates.Text);
        }
        #endregion

        #region Search group - ContextMenuStrip handlers
        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ctxResultsMenu.SourceControl is not ListBox lb) return;

            DoCopy(lb);
        }

        private void ViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ctxResultsMenu.SourceControl is not ListBox lb) return;

            DoOpenViewer(lb);
        }

        private void ViaMessageBusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ctxResultsMenu.SourceControl is not ListBox lb) return;

            VisitedSystem searchResult = null;

            if (lb == lbJournals)
                searchResult = _c.Search.CollectedJournals;
            else if (lb == lbAugmented)
                searchResult = _c.Search.AugmentedJournals;

            if (searchResult == null) return;

            List<JournalBase> preamble = ArchivistData.ToJournalObj(_c.Core, searchResult.PreambleJournalEntries);
            List<JournalBase> systemJournals = ArchivistData.ToJournalObj(_c.Core, searchResult.SystemJournalEntries);

            if (!ArchivistData.IsSystemScanComplete(systemJournals))
            {
                _c.UI.SetMessage($"WARNING: Search result data does not represent a incomplete system scan!");
            }

            ArchivistJournalsMessage msg = ArchivistJournalsMessage.New(
                searchResult.SystemName,
                searchResult.SystemId64,
                preamble,
                systemJournals,
                /*isGeneratedFromSpansh=*/ lb == lbAugmented,
                searchResult.Commander,
                searchResult.VisitCount);

            _c.Dispatcher.SendMessage(msg);
        }

        private void ViaCoreJournalEventsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ctxResultsMenu.SourceControl is not ListBox lb) return;

            _c.SetResendAll(true);
            foreach (var item in lb.Items)
            {
                string json = item.ToString();
                _c.Core.DeserializeEvent(json, true);
            }
            _c.SetResendAll(false);
        }
        #endregion


    }
}
