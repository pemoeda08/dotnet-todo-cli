using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoCli.Commands
{
    public sealed class AddCommand : Command<AddCommand.Settings>
    {
        private readonly TodoApp todoApp;
        private readonly TodoRenderer renderer;

        public AddCommand()
        {
            todoApp = TodoApp.Default();
            renderer = new TodoRenderer(todoApp);
        }

        public sealed class Settings : CommandSettings
        {
            [CommandArgument(0, "[tasks]")]
            public string[] Tasks { get; init; }
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            var validationResult =  todoApp.Add(settings.Tasks);
            switch (validationResult)
            {
                case TodoValidationResult.NullOrEmptyText:
                    Console.WriteLine("todo add: please input text.");
                    break;

                case TodoValidationResult.Success:
                    renderer.RenderTaskList();
                    break;
            }

            return 0;
        }

    }
}
