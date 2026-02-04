using Observatory.Framework;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.PluginCommon.PluginInterop.Messages
{
    public class ArchivistJournalsMessage : PluginMessageWrapper
    {
        public const string MESSAGE_TYPE = "ArchivistJournalsMessage";

        public const int MESSAGE_VERSION = 1;
        public const string FIELD_INT_MSG_VER = "messageVersion";
        public const string FIELD_STRING_SYSTEM_NAME = "systemName";
        public const string FIELD_ULONG_ID64 = "id64";
        public const string FIELD_LISTJOURNALBASE_PREAMBLE_ENTRIES = "preambleJournalEntries";
        public const string FIELD_LISTJOURNALBASE_SYSTEM_ENTRIES = "systemJournalEntries";
        public const string FIELD_BOOL_IS_GENERATED_FROM_SPANSH = "isGeneratedFromSpansh";
        public const string FIELD_STRING_SOURCE_COMMANDER_NAME = "sourceCommanderName";
        public const string FIELD_INT_COMMANDER_VISIT_COUNT = "commanderVisitCount";

        public static ArchivistJournalsMessage New(
            string systemName,
            ulong id64,
            List<JournalBase> preambleJournalEntries,
            List<JournalBase> systemJournalEntries,
            bool isGeneratedFromSpansh = false,
            string sourceCommanderName = "",
            int commanderVisitCount = 0)
        {
            return new()
            {
                SystemName = systemName,
                Id64 = id64,
                PreambleJournalEntries = preambleJournalEntries,
                SystemJournalEntries = systemJournalEntries,
                IsGeneratedFromSpansh = isGeneratedFromSpansh,
                SourceCommanderName = sourceCommanderName,
                CommanderVisitCount = commanderVisitCount
            };
        }

        public static bool TryUnmarshal(PluginMessageWrapper message, out PluginMessageWrapper result)
        {
            result = null;
            try
            {
                result = new ArchivistJournalsMessage(message);
            }
            catch (Exception) { }
            return result != null;
        }

        public ArchivistJournalsMessage(MessageSender sender, PluginMessage msg) : base(sender, msg)
        { }

        protected ArchivistJournalsMessage(string type, Guid? inReplyTo = null) : base(type, inReplyTo)
        { }

        protected ArchivistJournalsMessage(Guid? inReplyTo = null) : base(MESSAGE_TYPE, inReplyTo)
        { }

        private ArchivistJournalsMessage(PluginMessageWrapper raw) : base(raw.Sender, raw.MessageFromInit)
        {
            Unmarshal();
        }

        protected override Dictionary<string, object> Marshal()
        {
            return new()
            {
                { FIELD_STRING_SYSTEM_NAME, SystemName },
                { FIELD_ULONG_ID64, Id64 },
                { FIELD_LISTJOURNALBASE_PREAMBLE_ENTRIES, PreambleJournalEntries },
                { FIELD_LISTJOURNALBASE_SYSTEM_ENTRIES, SystemJournalEntries },
                { FIELD_BOOL_IS_GENERATED_FROM_SPANSH, IsGeneratedFromSpansh },
                { FIELD_STRING_SOURCE_COMMANDER_NAME, SourceCommanderName },
                { FIELD_INT_COMMANDER_VISIT_COUNT, CommanderVisitCount },
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
            PreambleJournalEntries = (List<JournalBase>)MessageFromInit.MessagePayload[FIELD_LISTJOURNALBASE_PREAMBLE_ENTRIES];
            SystemJournalEntries = (List<JournalBase>)MessageFromInit.MessagePayload[FIELD_LISTJOURNALBASE_SYSTEM_ENTRIES];
            MessageVersion = Convert.ToInt32(MessageFromInit.MessagePayload[FIELD_INT_MSG_VER]);

            IsGeneratedFromSpansh = false;
            if (MessageFromInit.MessagePayload.TryGetValue(FIELD_BOOL_IS_GENERATED_FROM_SPANSH, out object isGeneratedFromSpansh))
                IsGeneratedFromSpansh = Convert.ToBoolean(isGeneratedFromSpansh);
            SourceCommanderName = string.Empty;
            if (MessageFromInit.MessagePayload.TryGetValue(FIELD_STRING_SOURCE_COMMANDER_NAME, out object sourceCmdr))
                SourceCommanderName = sourceCmdr.ToString();
            CommanderVisitCount = 0;
            if (MessageFromInit.MessagePayload.TryGetValue(FIELD_INT_COMMANDER_VISIT_COUNT, out object visitCount))
                CommanderVisitCount = Convert.ToInt32(visitCount);
        }

        public int MessageVersion { get; set; }

        public string SystemName { get; set; }

        public ulong Id64 { get; set; }

        public List<JournalBase> PreambleJournalEntries { get; set; }

        public List<JournalBase> SystemJournalEntries { get; set; }

        public bool IsGeneratedFromSpansh { get; set; }

        public string SourceCommanderName { get; set; }

        public int CommanderVisitCount { get; set; }
    }
}
