using com.github.fredjk_gh.PluginCommon.Data;
using Observatory.Framework;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.ObservatoryArchivist.DB
{
    internal class NotificationInfo
    {
        private string _title = string.Empty;
        private string _detail = string.Empty;
        private string _extDetails = string.Empty;
        private string _sender = string.Empty;

        internal static NotificationInfo FromNotificationArgs(
            ulong systemId64, string systemName, string commanderName, NotificationArgs na)
        {
            return new()
            {
                SystemId64 = systemId64,
                SystemName = systemName,
                Commander = commanderName,
                CoalescingId = na.CoalescingId,
                Title = na.Title,
                Detail = na.Detail,
                ExtendedDetails = na.ExtendedDetails,
                Sender = na.Sender,
            };
        }

        internal NotificationArgs ToNotificationArgs()
        {
            return new NotificationArgs
            {
                CoalescingId = CoalescingId,
                Title = Title,
                Detail = Detail,
                ExtendedDetails = ExtendedDetails,
                Sender = Sender,
            };
        }

        [JsonIgnore]
        public long _id { get; set; }
        public ulong SystemId64 { get; set; }
        public string SystemName { get; set; }
        public string Commander { get; set; }
        public int? CoalescingId { get; set; }
        public string Title { get => _title; set => _title = value ?? string.Empty; }
        public string Detail { get => _detail; set => _detail = value ?? string.Empty; }
        public string ExtendedDetails { get => _extDetails; set => _extDetails = value ?? string.Empty; }
        public string Sender { get => _sender; set => _sender = value ?? string.Empty; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, JsonHelper.JSON_SERIALIZER_OPTIONS);
        }
    }
}
