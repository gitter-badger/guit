﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Guit.Events;
using Guit.Plugin.Releaseator.Properties;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using Merq;

namespace Guit.Plugin.Releaseator
{
    [Shared]
    [MenuCommand("Releaseator.Fetch", 'f', nameof(Releaseator), typeof(Resources))]
    class FetchCommand : IMenuCommand, IAfterExecuteCallback
    {
        readonly IEnumerable<ReleaseConfig> repositories;
        readonly IEventStream eventStream;
        readonly CredentialsHandler credentials;
        readonly ReleaseatorView view;
        readonly MainThread mainThread;

        [ImportingConstructor]
        public FetchCommand(
            IEnumerable<ReleaseConfig> repositories,
            IEventStream eventStream,
            CredentialsHandler credentials,
            ReleaseatorView view,
            MainThread mainThread)
        {
            this.repositories = repositories;
            this.eventStream = eventStream;
            this.credentials = credentials;
            this.view = view;
            this.mainThread = mainThread;
        }

        public Task AfterExecuteAsync(CancellationToken cancellation)
        {
            mainThread.Invoke(() => view.Refresh());

            return Task.CompletedTask;
        }

        public Task ExecuteAsync(object? parameter = null, CancellationToken cancellation = default)
        {
            foreach (var (config, index) in repositories.Select((config, index) => (config, index)))
            {
                eventStream.Push(Status.Create((index + 1) / repositories.Count(), "Fetching {0}...", config.Repository.GetName()));

                config.Repository.Fetch(credentials, prune: true);
            };

            eventStream.Push(Status.Succeeded());

            return Task.CompletedTask;
        }
    }
}