using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Transkeeper.Commands;

public class CmdSettings : CommandSettings
{
    [Description("Id of the transaction to retrieve.")]
    [CommandOption("-i|--id")]
    public string? Id { get; set; }

    [Description("Sterilized transaction in json format.")]
    [CommandOption("-j|--json")]
    public string? Json { get; set; }

    public override ValidationResult Validate()
    {
        if (!string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(Json))
        {
            return ValidationResult.Error("You cannot specify both options [-i|--id] and [-j|--json] simultaneously.");
        }

        return ValidationResult.Success();
    }
}