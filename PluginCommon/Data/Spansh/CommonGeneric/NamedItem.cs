﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric
{
    public class NamedItem : GenericJsonBase
    {
        [JsonPropertyName("name")]
        public string Name { get; init; }
    }
}
