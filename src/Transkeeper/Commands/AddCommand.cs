using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Spectre.Console;
using Spectre.Console.Cli;
using Transkeeper.Abstractions;
using Transkeeper.Infrastructure;
using Transkeeper.Models;

namespace Transkeeper.Commands;

public sealed class AddCommand : Command<CmdSettings>
{
    private readonly IRepository<Transaction> _repository;

    public AddCommand(IRepository<Transaction> repository)
    {
        _repository = repository;
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] CmdSettings settings)
    {
        if (!string.IsNullOrEmpty(settings.Id))
        {
            AnsiConsole.MarkupLine("[red]You cannot specify [white]-i|--id[/] with [white]add[/] command.[/]");

            return (int)ExitCode.InvalidInput;
        }

        Transaction tr;

        if (string.IsNullOrEmpty(settings.Json))
        {
            AnsiConsole.MarkupLine("To store a new transaction provide the following:");

            var id = AnsiConsole.Prompt(
                new TextPrompt<int>("Transaction Id:")
                    .ValidationErrorMessage("[red]Transaction Id must be an integer.[/]"));

            var amount = AnsiConsole.Prompt(
                new TextPrompt<decimal>("Amount:")
                    .ValidationErrorMessage("[red]Amount must be a floating point number.[/]"));

            var date = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("Transaction date:")
                    .ValidationErrorMessage("[red]Transaction date must be a formatted datetime string.[/]"));

            tr = new Transaction
            {
                Id = id,
                Amount = amount,
                TransactionDate = date
            };
        }
        else
        {
            try
            {
                tr = JsonSerializer.Deserialize<Transaction>(settings.Json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                })!;
            }
            catch (Exception e)
            {
                if (e is JsonException or NotSupportedException)
                {
                    AnsiConsole.MarkupLine("[red]Invalid json, failed to deserialize.[/]");

                    return (int)ExitCode.InvalidInput;
                }

                throw;
            }
        }

        if (_repository.TrySave(tr))
        {
            AnsiConsole.MarkupLine("[yellow]Successfully stored![/]");

            return (int)ExitCode.Success;
        }

        AnsiConsole.MarkupLine($"[red]Transaction with [white]Id: {tr.Id}[/] already exists.[/]");

        return (int)ExitCode.DuplicateId;
    }
}