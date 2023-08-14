using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;
using Transkeeper.Infrastructure;

namespace Transkeeper.Commands;

public class DefaultCommand : Command<CmdSettings>
{
    private readonly IServiceProvider _serviceProvider;

    public DefaultCommand(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] CmdSettings settings)
    {
        if (!string.IsNullOrEmpty(settings.Id))
        {
            AnsiConsole.MarkupLine("[red]You cannot specify [white][-i|--id][/] option without specifying [white][add][/] or [white][get][/] action.[/]");

            return (int)ExitCode.InvalidInput;
        }

        if (!string.IsNullOrEmpty(settings.Json))
        {
            AnsiConsole.MarkupLine("[red]You cannot specify [white][-j|--json][/] option without specifying [white][add][/] or [white][get][/] action.[/]");

            return (int)ExitCode.InvalidInput;
        }

        const string add = nameof(add);
        const string get = nameof(get);
        const string exit = nameof(exit);

        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new TextPrompt<string>("Choose one of the following actions [green]add/get/exit[/]:")
                    .ValidationErrorMessage("[red]That's not a valid action name[/]")
                    .Validate(a => new[] { "add", "get", "exit" }.Contains(a.ToLower())));

            switch (choice)
            {
                case add:
                    var addCmd = _serviceProvider.GetService<AddCommand>()!;
                    addCmd.Execute(context, settings);
                    break;
                case get:
                    var getCmd = _serviceProvider.GetService<GetCommand>()!;
                    getCmd.Execute(context, settings);
                    break;
                case exit:
                    return (int)ExitCode.Success;
            }
        }
    }
}