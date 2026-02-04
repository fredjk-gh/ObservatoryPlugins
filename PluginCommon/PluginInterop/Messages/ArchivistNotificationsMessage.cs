using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.PluginCommon.PluginInterop.Messages
{
    public class ArchivistNotificationsMessage : PluginMessageWrapper
    {
        public const string MESSAGE_TYPE = "ArchivistNotificationsMessage";
        public const int MESSAGE_VERSION = 1;
        public const string FIELD_INT_MSG_VER = "messageVersion";
        public const string FIELD_STRING_SYSTEM_NAME = "systemName";
        public const string FIELD_ULONG_ID64 = "id64";
        public const string FIELD_STRING_SOURCE_COMMANDER_NAME = "sourceCommanderName";
        public const string FIELD_LISTNOTIFICATIONARGS = "notificationArgs";

        public static ArchivistNotificationsMessage New(
            string systemName,
            ulong id64,
            string sourceCommanderName,
            List<NotificationArgs> notifications)
        {
            return new()
            {
                SystemName = systemName,
                Id64 = id64,
                SourceCommanderName = sourceCommanderName,
                Notifications = notifications
            };
        }

        public static bool TryUnmarshal(PluginMessageWrapper message, out PluginMessageWrapper result)
        {
            result = null;
            try
            {
                result = new ArchivistNotificationsMessage(message);
            }
            catch (Exception) { }
            return result != null;
        }

        public ArchivistNotificationsMessage(MessageSender sender, PluginMessage msg) : base(sender, msg)
        { }

        protected ArchivistNotificationsMessage(string type, Guid? inReplyTo = null) : base(type, inReplyTo)
        { }

        protected ArchivistNotificationsMessage(Guid? inReplyTo = null) : base(MESSAGE_TYPE, inReplyTo)
        { }

        private ArchivistNotificationsMessage(PluginMessageWrapper raw) : base(raw.Sender, raw.MessageFromInit)
        {
            Unmarshal();
        }


        protected override Dictionary<string, object> Marshal()
        {
            return new()
            {
                { FIELD_STRING_SYSTEM_NAME, SystemName },
                { FIELD_ULONG_ID64, Id64 },
                { FIELD_STRING_SOURCE_COMMANDER_NAME, SourceCommanderName },
                { FIELD_LISTNOTIFICATIONARGS, Notifications},
                { FIELD_INT_MSG_VER, 1 },
            };
        }

        protected override void Unmarshal()
        {
            if (MessageFromInit.MessageType != MESSAGE_TYPE)
            {
                throw new ArgumentException($"Provided raw message is not unmarshalable by {this.GetType().Name}.");
            }

            SystemName = MessageFromInit.MessagePayload[FIELD_STRING_SYSTEM_NAME].ToString();
            Id64 = Convert.ToUInt64(MessageFromInit.MessagePayload[FIELD_ULONG_ID64]);
            Notifications = (List<NotificationArgs>)MessageFromInit.MessagePayload[FIELD_LISTNOTIFICATIONARGS];
            MessageVersion = Convert.ToInt32(MessageFromInit.MessagePayload[FIELD_INT_MSG_VER]);

            SourceCommanderName = string.Empty;
            if (MessageFromInit.MessagePayload.TryGetValue(FIELD_STRING_SOURCE_COMMANDER_NAME, out object sourceCmdrName))
                SourceCommanderName = sourceCmdrName.ToString();
        }

        public int MessageVersion { get; set; }

        public string SystemName { get; set; }

        public ulong Id64 { get; set; }

        public string SourceCommanderName { get; set; }

        public List<NotificationArgs> Notifications { get; set; }
    }
}
