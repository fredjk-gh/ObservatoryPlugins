using com.github.fredjk_gh.ObservatoryHelm.Data;
using com.github.fredjk_gh.PluginCommon.PluginInterop.Messages;
using static com.github.fredjk_gh.PluginCommon.PluginInterop.PluginTracker;

namespace com.github.fredjk_gh.ObservatoryHelm.UI.Cards
{
    public partial class GetReferenceSystemForm : Form
    {
        private readonly HelmContext _c;

        internal GetReferenceSystemForm(HelmContext context)
        {
            InitializeComponent();

            _c = context;

            btnLookup.Enabled = false;
            btnSet.Enabled = false;
            lblArchivistRequired.Enabled = !_c.PluginTracker.IsActive(PluginType.fredjk_Archivist);
        }

        private void ValidateInputs()
        {
            btnLookup.Enabled = !string.IsNullOrWhiteSpace(txtSystemName.Text)
                || (!string.IsNullOrWhiteSpace(txtId64.Text) && UInt64.TryParse(txtId64.Text, out _));

            btnSet.Enabled = !string.IsNullOrWhiteSpace(txtSystemName.Text)
                && !string.IsNullOrWhiteSpace(txtX.Text) && Double.TryParse(txtX.Text, out _)
                && !string.IsNullOrWhiteSpace(txtY.Text) && Double.TryParse(txtY.Text, out _)
                && !string.IsNullOrWhiteSpace(txtZ.Text) && Double.TryParse(txtZ.Text, out _);
        }

        internal SystemBasicData ReferenceSystem { get; private set; }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateInputs();
        }

        private void BtnLookup_Click(object sender, EventArgs e)
        {
            UInt64.TryParse(txtId64.Text, out ulong id64);

            var posCacheLookup = ArchivistPositionCacheSingleLookup.New(txtSystemName.Text, id64, /*externalFallback=*/ true);
            _c.Dispatcher.SendMessageAndAwaitResponse<ArchivistPositionCacheSingle>(posCacheLookup, HandleArchivistPositionCacheResponse, PluginType.fredjk_Archivist);
        }

        private void BtnSet_Click(object sender, EventArgs e)
        {
            try
            {
                ReferenceSystem = new(Convert.ToUInt64(txtId64.Text), txtSystemName.Text, new()
                {
                    x = Convert.ToDouble(txtX.Text),
                    y = Convert.ToDouble(txtY.Text),
                    z = Convert.ToDouble(txtZ.Text),
                });
            }
            catch { } // This shouldn't fail if the Set button is enabled.
            Close();
        }

        private void HandleArchivistPositionCacheResponse(ArchivistPositionCacheSingle posCacheResult)
        {
            _c.Core.ExecuteOnUIThread(() =>
            {
                txtSystemName.Text = posCacheResult.Position.SystemName;
                txtId64.Text = posCacheResult.Position.SystemId64.ToString();
                txtX.Text = posCacheResult.Position.X.ToString();
                txtY.Text = posCacheResult.Position.Y.ToString();
                txtZ.Text = posCacheResult.Position.Z.ToString();

                ValidateInputs();
            });
        }
    }
}
