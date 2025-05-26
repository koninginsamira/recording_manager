// <copyright file="RecordingMonitor.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Jellyfin.Plugin.RecordingManager
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Jellyfin.Data.Events;
    using MediaBrowser.Controller.Dto;
    using MediaBrowser.Controller.LiveTv;
    using MediaBrowser.Model.LiveTv;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Monitor for Live TV.
    /// </summary>
    public class RecordingMonitor : IHostedService
    {
        private readonly ILiveTvManager liveTvManager;
        private readonly ILogger<RecordingMonitor> logger;
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordingMonitor"/> class.
        /// </summary>
        /// <param name="liveTvManager">Live TV Manager.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="httpClientFactory">HTTP Client Factory.</param>
        public RecordingMonitor(
            ILiveTvManager liveTvManager,
            ILogger<RecordingMonitor> logger,
            IHttpClientFactory httpClientFactory)
        {
            this.liveTvManager = liveTvManager;
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
        }

        /// <inheritdoc/>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.liveTvManager.TimerCreated += this.OnRecordingStarted;
            this.logger.LogInformation("WebhookRecorder plugin started.");
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.liveTvManager.TimerCancelled -= this.OnRecordingStarted;
            this.logger.LogInformation("WebhookRecorder plugin stopped.");
            return Task.CompletedTask;
        }

        private async void OnRecordingStarted(object? sender, GenericEventArgs<TimerEventInfo> e)
        {
            try
            {
                this.logger.LogInformation("Recording started: {E}", e.ToString());
                var query = new RecordingQuery();
                var recordings = await liveTvManager.GetRecordingsAsync(query, new DtoOptions()).ConfigureAwait(false);
                this.logger.LogInformation("Recording found: {E}", recordings.ToString());

                var payload = new
                {
                    eventType = "recordingStarted",
                    e,
                    recordings,
                };

                var json = JsonSerializer.Serialize(payload);

                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    using (var client = this.httpClientFactory.CreateClient())
                    {
                        await client.PostAsync(new Uri("https://webhook.site/08f86cc8-f3c5-4652-b882-873e400fe483"), content).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to send webhook for recording start.");
                throw;
            }
        }
    }
}
