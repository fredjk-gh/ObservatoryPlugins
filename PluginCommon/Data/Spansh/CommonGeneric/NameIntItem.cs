using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric
{
    public class NameIntItem : GenericJsonBase
    {
        public string Name { get; init; }
        public virtual int Value { get; init; }
    }
}
