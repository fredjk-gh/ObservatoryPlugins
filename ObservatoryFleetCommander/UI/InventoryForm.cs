using com.github.fredjk_gh.ObservatoryFleetCommander.Data;
using com.github.fredjk_gh.PluginCommon;
using com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs;
using com.github.fredjk_gh.PluginCommon.UI;
using com.github.fredjk_gh.PluginCommon.UI.Shared;
using System.Diagnostics;

namespace com.github.fredjk_gh.ObservatoryFleetCommander.UI
{
    public partial class InventoryForm : Form
    {
        private readonly CommanderContext _c;
        private readonly CarrierData _carrierData;
        private DateTime _lastUpdate = DateTime.UtcNow;
        private readonly List<CommodityResource> _allCommodities = [];
        private readonly List<InventoryItem> _dgvItems = [];

        private static readonly ImageSpec MISMATCH_WARNING_ICON = new(Images.WarningSignImage)
        {
            Size = new(24, 24),
            Visible = false,
            Tag = "warning",
            ToolTip = "Tracked inventory and carrier cargo numbers from statistics disagree.",
            Color = Color.Yellow,
        };

        private static readonly ImageSpec NEED_CARRIER_STATS_INFO_ICON = new(Images.InfoBubbleImage)
        {
            Size = new(24, 24),
            Visible = false,
            Tag = "info",
            ToolTip = "Please open the Carrier Admin panel for fresh data.",
        };

        internal InventoryForm(CommanderContext ctx, CarrierData carrierData)
        {
            InitializeComponent();
            Icon = Icon.FromHandle(Images.CommoditiesImage.GetHicon());
            _c = ctx;
            _carrierData = carrierData;

            dgvInventory.BackgroundColorChanged += DataGridView_AColorChanged;
            dgvInventory.ForeColorChanged += DataGridView_AColorChanged;
            dgvTradeOrders.BackgroundColorChanged += DataGridView_AColorChanged;
            dgvTradeOrders.ForeColorChanged += DataGridView_AColorChanged;

            _allCommodities.AddRange([.. FDevIDs.AllCommoditiesBySymbol.Values.OrderBy(c => c.Name)]);
            
            _dgvItems = [.. _carrierData.Inventory.Values.OrderByDescending(i => i.Quantity)];

            Text = $"{_carrierData.CarrierName}: Inventory (Beta)";

            tlblCargo.ToolTipManager = tTips;
            tlblCargo.AddImage(MISMATCH_WARNING_ICON);
            tlblCargo.AddImage(NEED_CARRIER_STATS_INFO_ICON);

            dgvInventory.DataError += DgvInventory_DataError;
            dgvInventory.RowsAdded += DgvInventory_RowsAdded;
            dgvInventory.UserDeletedRow += DgvInventory_UserDeletedRow;
            dgvInventory.CellValueChanged += DgvInventory_CellValueChanged;
            dgvInventory.RowValidating += DgvInventory_RowValidating;
            dgvInventory.CellBeginEdit += DgvInventory_CellBeginEdit;
            dgvInventory.CellPainting += DgvInventory_CellPainting;

            dgvInventory.AutoGenerateColumns = false;
            var itemSource = new BindingSource
            {
                DataSource = _allCommodities
            };
            colItem.DataSource = itemSource;
            colItem.DisplayMember = "Name"; // From CommodityResource
            colItem.ValueMember = "Symbol"; // From CommodityResource
            colItem.ValueType = typeof(string);
            colItem.FlatStyle = FlatStyle.Flat;

            var dataSource = new BindingSource
            {
                DataSource = _dgvItems
            };
            dgvInventory.DataSource = dataSource;
            colItem.DataPropertyName = "ItemId"; // From InventoryItem
            colQty.DataPropertyName = "Quantity"; // From InventoryItem

            Draw();
        }

        private void DgvInventory_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0
                || e.ColumnIndex < 0
                || e.RowIndex == dgvInventory.NewRowIndex
                || e.ColumnIndex == colQty.Index) return; // Do defalt things.

            DataGridViewRow r = dgvInventory.Rows[e.RowIndex];
            DataGridViewComboBoxCell itemCell = (DataGridViewComboBoxCell)r.Cells[e.ColumnIndex];
            string itemId = itemCell.Value?.ToString();
            if (!string.IsNullOrWhiteSpace(itemId) && _carrierData.Inventory.ContainsKey(itemId))
            {
                itemCell.ReadOnly = true; // Mark all the pre-existing items types as non-editable.
                itemCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            }
            // Continue with default things.
        }

        private void DgvInventory_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex == dgvInventory.NewRowIndex
                || e.ColumnIndex == colQty.Index) return; // These are always editable.

            DataGridViewRow r = dgvInventory.Rows[e.RowIndex];
            DataGridViewComboBoxCell itemCell = (DataGridViewComboBoxCell)r.Cells[colItem.Index];

            string itemId = itemCell.Value?.ToString();
            // Disable editing for existing items.
            if (!string.IsNullOrWhiteSpace(itemId) && _carrierData.Inventory.ContainsKey(itemId))
            {
                e.Cancel = true;
                itemCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            try
            {
                var updatedItems = _dgvItems
                    .Where(i => i.ItemId is not null && i.Quantity > 0)
                    .ToDictionary(i => i.ItemId, i => i);

                foreach (string k in updatedItems.Keys)
                {
                    var ii = updatedItems[k];
                    if (!_carrierData.Inventory.TryAdd(k, ii))
                    {
                        // Update
                        if (ii.Quantity == 0)
                        {
                            // Shouldn't happen, but we treat this as a remove (since we know this key exists in carrierData's list.
                            _carrierData.Inventory.Remove(k);
                        }
                        else
                        {
                            _carrierData.Inventory[k].Quantity = ii.Quantity;
                        }
                    }
                }

                foreach (string k in _carrierData.Inventory.Keys.ToList()) // allow modifying the list while we iterate it
                {
                    if (!updatedItems.ContainsKey(k))
                    {
                        // Remove
                        _carrierData.Inventory.Remove(k);
                    }
                }

                _c.SerializeDataCacheV2();
                e.Cancel = false;
            }
            catch (ArgumentException)
            {
                // Most likely failure is duplicate items in the list. Should be hard to make happen now.
                Debug.WriteLine($"There are duplicate items in the list.");
                e.Cancel = true;
            }
        }

        private bool ValidateInventoryRow(int rowIndex)
        {
            bool isValid = true;
            DataGridViewRow r = dgvInventory.Rows[rowIndex];

            // Is Qty an int > 0?
            DataGridViewCell qtyCell = r.Cells[colQty.Index];
            qtyCell.ErrorText = string.Empty;
            int qty = -1;
            if (!Int32.TryParse(qtyCell.Value?.ToString(), out qty))
            {
                qtyCell.ErrorText = $"Value is not a number.";
                isValid = false;
            }
            else if (qty <= 0)
            {
                qtyCell.ErrorText = $"Value must be at least 1. To delete a row, click on the row header to select the row and press Delete.";
                isValid = false;
            }
            else if (qty >= _carrierData.CarrierCapacity)
            {
                qtyCell.ErrorText = $"Value exceeds to total possible capacity of this carrier.";
                isValid = false;
            }

            // Is Item non-empty and a known commodity and not already assigned to another item in the list.
            DataGridViewComboBoxCell itemCell = (DataGridViewComboBoxCell)r.Cells[colItem.Index];
            itemCell.ErrorText = string.Empty;
            string itemId = itemCell.Value?.ToString();
            if (string.IsNullOrWhiteSpace(itemId))
            {
                itemCell.ErrorText = "Please select an item type. It must not be already in the list.";
                isValid = false;
            }
            else
            {
                var c = GetCommodity(itemId);
                if (c is null)
                {
                    itemCell.ErrorText = $"Item with identifier {itemId} is an unknown commodity! Please inform the developer if this is unexpected.";
                    isValid = false;
                }
                else
                {
                    if (_dgvItems.Where(ii => ii.ItemId == itemId).Count() > 1)
                    {
                        itemCell.ErrorText = $"Item {c.Name} already exists in the list. Please edit that row instead or choose a different item type.";
                        isValid = false;
                    }
                }
            }

            return isValid;
        }
        private void DgvInventory_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            e.Cancel = !ValidateInventoryRow(e.RowIndex);
        }

        private void DgvInventory_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            Debug.WriteLine($"Deleted row with index {e.Row.Index}");
            if (e.Row.DataBoundItem is InventoryItem item)
            {
                Debug.WriteLine($"Removed inventory for {item.ItemId}...");
                _carrierData.Inventory.Remove(item.ItemId);
            }
            DrawBasicInfo();
        }

        private void DgvInventory_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            ValidateInventoryRow(e.RowIndex);

            if (e.ColumnIndex == colQty.Index)
                DrawBasicInfo();
        }

        private void DgvInventory_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            ValidateInventoryRow(e.RowIndex);
        }

        private void DgvInventory_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Debug.WriteLine($"DataGrid error: {e.Exception?.Message}");
            e.Cancel = true;
        }

        private void DataGridView_AColorChanged(object sender, EventArgs e)
        {
            if (sender is not DataGridView dgv) return;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = dgv.BackgroundColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = dgv.ForeColor;
            dgv.RowHeadersDefaultCellStyle.BackColor = dgv.BackgroundColor;
            dgv.RowHeadersDefaultCellStyle.ForeColor = dgv.ForeColor;
            dgv.RowsDefaultCellStyle.BackColor = dgv.BackgroundColor;
            dgv.RowsDefaultCellStyle.ForeColor = dgv.ForeColor;

            dgv.Invalidate();
        }

        public void OnInventoryChange(InventoryItem changed, bool alsoUpdateUi = true)
        {
            if (_c.IsReadAll) return;

            // Inventory is databound to a separate datastructure that is synchronized when closing the form.
            // Update the underlying datastructure to avoid clobbering the change we've already in the SoT.
            var existing = _dgvItems.Where(i => i.ItemId == changed.ItemId).FirstOrDefault();
            if (existing is not null)
            {
                if(changed.Quantity == 0)
                {
                    _dgvItems.Remove(existing);
                }
                else
                {
                    existing.Quantity = changed.Quantity;
                }
            }
            else
            {
                _dgvItems.Add(changed);
            }

            if (alsoUpdateUi)
            {
                _c.Core.ExecuteOnUIThread(() =>
                {
                    _lastUpdate = DateTime.UtcNow;
                    DrawLastUpdate();
                    DrawBasicInfo();
                });
            }
        }

        public void OnTradeOrderChange()
        {
            if (_c.IsReadAll) return;

            // Trade Orders are not databound; just re-draw to update from latest underlying data.
            _c.Core.ExecuteOnUIThread(() =>
            {
                _lastUpdate = DateTime.UtcNow;
                DrawLastUpdate();
                DrawTradeOrders();
            });
        }

        public void OnBasicInfoChange()
        {
            if (_c.IsReadAll) return;
            _c.Core.ExecuteOnUIThread(() =>
            {
                _lastUpdate = DateTime.UtcNow;
                DrawLastUpdate();
                DrawBasicInfo();
            });
        }

        void Draw()
        {
            if (_c.IsReadAll) return;
            DrawLastUpdate();
            DrawBasicInfo();
            DrawTradeOrders();
        }

        private void DrawTradeOrders()
        {
            if (_c.IsReadAll) return;
            dgvTradeOrders.Rows.Clear();

            foreach (var k in _carrierData.TradeOrders.Keys)
            {
                var t = _carrierData.TradeOrders[k];
                var r = new DataGridViewRow();

                var typeCell = new DataGridViewTextBoxCell() { Value = t.TradeType };
                var qtyCell = new DataGridViewTextBoxCell() { Value = t.Quantity };
                var nameCell = new DataGridViewTextBoxCell() { Value = t.ItemName };
                var blkMarketCell = new DataGridViewCheckBoxCell() { Value = t.IsBlackMarket };

                r.Cells.Add(typeCell);
                r.Cells.Add(qtyCell);
                r.Cells.Add(nameCell);
                r.Cells.Add(blkMarketCell);
                r.Tag = k;

                dgvTradeOrders.Rows.Add(r);
            }
        }

        private CommodityResource GetCommodity(string symbol)
        {
            return FDevIDs.AllCommoditiesBySymbol.GetValueOrDefault(symbol, null);
        }

        private void DrawBasicInfo()
        {
            if (_c.IsReadAll) return;
            if (_carrierData.LastCarrierStats is null)
            {
                tlblCargo.Text = "(no data)";
                tlblCargo.SetVisibility(NEED_CARRIER_STATS_INFO_ICON.Guid, true);
                lblKnownCargoUsageValue.Text = string.Empty;
                lblCapacityFreeValue.Text = string.Empty;
            }
            else
            {
                int inventoryCount = _dgvItems.Sum(i => i.Quantity);
                tlblCargo.Text = $"{UIFormatter.Tonnage(_carrierData.LastCarrierStats.SpaceUsage.Cargo)} / {UIFormatter.Tonnage(_carrierData.LastCarrierStats.SpaceUsage.CargoSpaceReserved)}";
                tlblCargo.SetVisibility(NEED_CARRIER_STATS_INFO_ICON.Guid, false);
                tlblCargo.SetVisibility(MISMATCH_WARNING_ICON.Guid, _carrierData.LastCarrierStats is not null && inventoryCount != _carrierData.LastCarrierStats.SpaceUsage.Cargo);
                lblKnownCargoUsageValue.Text = UIFormatter.Tonnage(inventoryCount);
                lblCapacityFreeValue.Text = $"{UIFormatter.Tonnage(_carrierData.LastCarrierStats.SpaceUsage.TotalCapacity)} / {UIFormatter.Tonnage(_carrierData.LastCarrierStats.SpaceUsage.FreeSpace)} free";
            }
            lblDockingAccessValue.Text = _carrierData.DockingAccess.ToString();
        }

        private void DrawLastUpdate()
        {
            if (_c.IsReadAll) return;
            lblLastUpdatedValue.Text = UIFormatter.DateTime(_lastUpdate, _c.Settings.UIDateTimesUseInGameTime);
        }
    }
}
