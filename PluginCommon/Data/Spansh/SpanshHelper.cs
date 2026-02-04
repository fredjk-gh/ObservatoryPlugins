using com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric;
using com.github.fredjk_gh.PluginCommon.Data.Spansh.System;
using com.github.fredjk_gh.PluginCommon.Utilities;
using Observatory.Framework.Interfaces;
using System.Diagnostics;
using System.Text.Json;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh
{
    public static class SpanshHelper
    {
        public static async Task<SpanshSystem> FetchSystemDump(ICoreContext<IObservatoryWorker> ctx, ulong systemId64, CancellationToken ctoken)
        {
            var url = $"https://spansh.co.uk/api/dump/{systemId64}";
            var task = ctx.Core.HttpClient.GetStringAsync(url, ctoken);
            SpanshSystem result = null;
            string status = "fetching data";
            try
            {
                await task;
                string spanshJson = task.Result;

                status = "parsing response";
                result = JsonHelper.ParseSpanshSystemDump(spanshJson);
            }
            catch (Exception ex)
            {
                var msg = $"Spansh system dump failed while {status}: {ex.Message}{Environment.NewLine}If failure was during fetch, Spansh may not know this system.";
                ctx.Core.GetPluginErrorLogger(ctx.Worker)(ex, msg);
                throw new SpanshException(msg, ex);
            }
            return result;
        }

        public static async Task<FleetCarrierRouteJobResult> FetchFleetCarrierRoute(
            ICoreContext<IObservatoryWorker> ctx,
            FleetCarrierRouteJobResult.Parameters req,
            CancellationToken ctoken)
        {
            var requestUri = req.ToUrl();
            Debug.WriteLine($"Requesting fleet carrier route from spansh via url: {requestUri}");
            string jobJson = "";
            string status = "requesting route";

            var searchStartTask = ctx.Core.HttpClient.GetStringAsync(requestUri, ctoken);
            try
            {
                await searchStartTask;
                jobJson = searchStartTask.Result;
            }
            catch (Exception ex)
            {
                var msg = $"Spansh Fleet Carrier Route plotting failed while {status}: {ex.Message}";
                ctx.Core.GetPluginErrorLogger(ctx.Worker)(ex, msg);
                throw new SpanshException(msg, ex);
            }

            NewJobResponse newJobResponse = JsonSerializer.Deserialize<NewJobResponse>(jobJson);
            var jobId = newJobResponse.Job;
            var jobResultUrl = $"https://spansh.co.uk/api/results/{jobId}";
            Debug.WriteLine($"Job result polling url: {jobResultUrl}");

            // Poll until it's ready. Unfortunate, but the only way we can do it.
            status = "polling for result";
            var routeJson = string.Empty;
            FleetCarrierRouteJobResult jobResult = null;
            do
            {
                Task pause = Task.Delay(1000, ctoken); // Non-blocking wait.
                await pause;
                var fetchResultTask = ctx.Core.HttpClient.GetStringAsync(jobResultUrl, ctoken);
                try
                {
                    await fetchResultTask;
                    routeJson = fetchResultTask.Result;
                    status = "parsing result";
                    jobResult = JsonSerializer.Deserialize<FleetCarrierRouteJobResult>(routeJson);
                }
                catch (Exception ex)
                {
                    var msg = $"Spansh Fleet Carrier Route plotting failed while {status}: {ex.Message}";
                    ctx.Core.GetPluginErrorLogger(ctx.Worker)(ex, msg);
                    throw new SpanshException(msg, ex);
                }

                if (jobResult is not null)
                {
                    if (jobResult.Result is not null)
                    {
                        // We have a result! (Always check this first, in case both "job" and "result" are present to avoid endless looping.)
                        return jobResult;
                    }
                    else if (!string.IsNullOrEmpty(jobResult.Error) || jobResult.Status == "failed")
                    {
                        var msg = $"Spansh Fleet Carrier Route plotting reported error: {jobResult.Error}";
                        throw new SpanshException(msg);
                    }
                    else
                    {
                        routeJson = ""; // Presumably, still working on it...
                        continue;
                    }
                }
                else
                {
                    var msg = $"Spansh Fleet Carrier Route plotting returned unrecognized result: {routeJson}";
                    throw new SpanshException(msg);
                }
            } while (string.IsNullOrWhiteSpace(routeJson));

            var lastMsg = $"Spansh Fleet Carrier Route plotting exited abnormally!?";
            throw new SpanshException(lastMsg);
        }
    }
}
