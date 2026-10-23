namespace com.github.fredjk_gh.PluginCommon.UI
{
    public static class ListViewExtensions
    {
        // Adapted from: https://stackoverflow.com/questions/10795134/c-sharp-listview-column-text-size-is-not-the-same-on-each-computer/36974138#36974138https://stackoverflow.com/questions/10795134/c-sharp-listview-column-text-size-is-not-the-same-on-each-computer/36974138#36974138
        public static void ScaleColumns(this ListView listview, SizeF factor)
        {
            foreach (ColumnHeader column in listview.Columns)
            {
                column.Width = (int)Math.Round(column.Width * factor.Width);
            }
        }
    }
}
