﻿using com.github.fredjk_gh.PluginCommon.Data.Spansh.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh
{
    public class BodySignals : UpdateTimeJsonBase

    {
        [JsonPropertyName("genuses")]
        public List<string> Genuses { get; init; }

        [JsonPropertyName("signals")]
        [JsonConverter(typeof(NameIntItemConverter))]
        public List<NameIntItem> Signals { get; init; }
    }
}
