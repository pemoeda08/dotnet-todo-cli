using Spectre.Console;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Collections.Generic;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;
using TodoCli.Commands;

namespace TodoCli
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandApp();
            app.Configure(app =>
            {
                app.AddCommand<AddCommand>("add");
                app.AddCommand<ClearCommand>("clear");
                app.AddCommand<RemoveCommand>("remove")
                    .WithAlias("delete")
                    .WithAlias("del");
                app.AddCommand<ListCommand>("list");
            });
            int appStatus = app.Run(args);
            TodoApp.Default().Dispose();
            return appStatus;
        }
    }

}
