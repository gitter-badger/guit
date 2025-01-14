﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Guit
{
    public class MenuCommand : IMenuCommand
    {
        readonly Action? action;
        readonly Func<Task>? function;

        public MenuCommand(Action action) => this.action = action;

        public MenuCommand(Func<Task> function) => this.function = function;

        public async Task ExecuteAsync(object? parameter = null, CancellationToken cancellation = default)
        {
            action?.Invoke();

            if (function != null)
                await function();
        }
    }
}