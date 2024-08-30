using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryAggregator.UI
{
    public class EmojiSpec
    {
        internal EmojiSpec(string emoji)
        {
            Text = emoji;
            Description = "";
            if (Constants.EmojiToolTips.ContainsKey(emoji))
            {
                Description = Constants.EmojiToolTips[emoji];
            }
            else
            {
                // Some emoji have additional suffixes which mean they're not exact matches. Check prefixes.
                foreach (var entry in Constants.EmojiToolTips)
                {
                    if (emoji.StartsWith(entry.Key))
                    {
                        Description = entry.Value;
                        break;
                    }
                }
            }
        }

        public string Text { get; init; }
        public string Description { get; init; }
        public string ToolTipText { get => $"{Text}:  {Description}"; }
    }
}
