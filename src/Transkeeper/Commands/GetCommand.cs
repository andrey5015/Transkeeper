using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;
using System.Text.Json;
using Spectre.Console;
using Spectre.Console.Cli;
using Transkeeper.Abstractions;
using Transkeeper.Infrastructure;
using Transkeeper.Models;

namespace Transkeeper.Commands;

public class GetCommand : Command<CmdSettings>
{
    private readonly IRepository<Transaction> _repository;

    public GetCommand(IRepository<Transaction> repository)
    {
        _repository = repository;
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] CmdSettings settings)
    {
        if (!string.IsNullOrEmpty(settings.Json))
        {
            AnsiConsole.MarkupLine("[red]You cannot specify [white]-j|--json[/] with [white]get[/] command.[/]");

            return (int)ExitCode.InvalidInput;
        }

        int id;

        if (string.IsNullOrEmpty(settings.Id))
        {
            id = AnsiConsole.Prompt(
                new TextPrompt<int>("Transaction Id:")
                    .ValidationErrorMessage("[red]Transaction Id must be an integer.[/]"));
        }
        else
        {
            if (!int.TryParse(settings.Id, out id))
            {
                AnsiConsole.MarkupLine("[red]Transaction Id must be an integer.[/]");

                return (int) ExitCode.InvalidInput;
            }
        }

        var result = _repository.GetById(id);

        if (result is null)
        {
            AnsiConsole.MarkupLine($"[red]Transaction with [white]Id {id}[/] was not found.[/]");

            return (int)ExitCode.NotFound;
        }

        AnsiConsole.Write(
            new Panel(JsonSerializer.Serialize(result))
                .Header("Retrieved transaction:")
                .Collapse()
                .RoundedBorder()
                .BorderColor(Color.Yellow));

        return (int)ExitCode.Success;
    }
}
