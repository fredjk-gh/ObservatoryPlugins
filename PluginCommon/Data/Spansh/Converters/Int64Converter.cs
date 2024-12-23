﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.Converters
{
    internal class Int64Converter : JsonConverter<Int64>
    {
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Convert.ToInt64(reader.GetDouble());
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}
