using com.github.fredjk_gh.ObservatoryArchivist.DB;
using com.github.fredjk_gh.PluginCommon.Data;
using com.github.fredjk_gh.PluginCommon.Data.Journals;
using com.github.fredjk_gh.PluginCommon.Data.Spansh;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryArchivist
{
    internal static class SpanshAugmenter
    {
        // This should probably be run on a non-UI thread.
        // TODO: improve debuggability / logging by passing in something from the caller.
        internal static async Task<VisitedSystem> FetchAndCacheFromSpansh(
            ArchivistContext context, VisitedSystem systemInfo, CancellationToken ctoken)
        {
            if (systemInfo is null) return null;

            string status = "Fetch complete; parsing response...";
            context.UI.SetMessage(status);

            try
            {
                var task = SpanshHelper.FetchSystemDump(context, systemInfo.SystemId64, ctoken);
                await task;

                var result = task.Result;
                status = "Spansh response parsed, converting...";
                context.UI.SetMessage(status);

                // Convert to Framework objects and serialize to Journal events.
                List<JournalBase> augmentedJournals = JournalGenerator.FromSpansh(result, systemInfo.FirstVisitDateTime);
                VisitedSystem augmented = AugmentJournals(
                    systemInfo, JsonHelper.SerializeJournals(augmentedJournals));

                if (ArchivistData.IsSystemScanComplete(augmentedJournals))
                {
                    status = "Storing result";
                    context.UI.SetMessage(status);
                    context.Journals.UpsertAugmentedSystem(augmented);
                }
                else
                {
                    context.UI.SetMessage("Spansh data appears incomplete. Not caching.");
                }

                return augmented;
            }
            catch (Exception ex)
            {
                context.UI.SetMessage($"Failed to process Spansh data during step {status}: {ex.Message}");
                return null;
            }
        }

        private static VisitedSystem AugmentJournals(VisitedSystem original, List<string> exportedJson)
        {
            VisitedSystem augmented = new()
            {
                Commander = original.Commander,
                SystemName = original.SystemName,
                SystemId64 = original.SystemId64,
                FirstVisitDateTime = original.FirstVisitDateTime,
                VisitCount = original.VisitCount,
                LastVisitDateTime = original.LastVisitDateTime,
                PreambleJournalEntries = original.PreambleJournalEntries,
                SystemJournalEntries = exportedJson
            };

            return augmented;
        }
    }
}
