using com.github.fredjk_gh.PluginCommon.UI.Shared;

namespace com.github.fredjk_gh.PluginCommon.UI
{
    public class DGVImageLabelColumn : DataGridViewColumn
    {
        public DGVImageLabelColumn()
            : base(new DGVImageLabelCell())
        {
            DefaultCellStyle = new DataGridViewCellStyle()
            {
                NullValue = null,
            };
            ValueType = typeof(ImageLabelSpec);
        }

        public override DataGridViewCell CellTemplate
        {
            get => base.CellTemplate;
            set
            {
                if (value is not null and not DGVImageLabelCell)
                    throw new InvalidCastException("A cell template of type DGVImageLabelCell expected");
                base.CellTemplate = value;
            }
        }
    }
}
