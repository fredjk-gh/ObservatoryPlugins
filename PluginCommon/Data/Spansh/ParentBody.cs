using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh
{
    public class ParentBody : GenericJsonBase
    {
        public string ParentType { get; init; }
        public int ParentBodyId { get; init; }
    }
}
