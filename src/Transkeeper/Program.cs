using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;
using Transkeeper.Abstractions;
using Transkeeper.Infrastructure;
using Transkeeper.Commands;
using Transkeeper.Models;
using Transkeeper.Persistence;

var cb = new ConfigurationBuilder();
cb.AddJsonFile("appsettings.json");
var conf = cb.Build();

var sc = new ServiceCollection();
sc.AddSingleton<IConfiguration>(conf)
    .AddTransient<IRepository<Transaction>, TransactionRepository>();

var reg = new TypeRegistrar(sc);
var app = new CommandApp<DefaultCommand>(reg);
app.Configure(cf =>
{
    cf.AddCommand<AddCommand>("add")
        .WithDescription("Add new transaction.");
    cf.AddCommand<GetCommand>("get")
        .WithDescription("Get transaction by Id.");
});

app.Configure(cr =>
{
    cr.Settings.PropagateExceptions = false;
    cr.Settings.ExceptionHandler = ex =>
    {
        AnsiConsole.MarkupLine("[red]Program crashed, reason:[/]");
        AnsiConsole.WriteException(ex, new ExceptionSettings
        {
            Format = ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks,
            Style = new ExceptionStyle
            {
                Exception = new Style().Foreground(Color.Grey),
                Message = new Style().Foreground(Color.White),
                NonEmphasized = new Style().Foreground(Color.Cornsilk1),
                Parenthesis = new Style().Foreground(Color.Cornsilk1),
                Method = new Style().Foreground(Color.Red),
                ParameterName = new Style().Foreground(Color.Cornsilk1),
                ParameterType = new Style().Foreground(Color.Red),
                Path = new Style().Foreground(Color.Red),
                LineNumber = new Style().Foreground(Color.Cornsilk1),
            }
        });

        return (int)ExitCode.SystemError;
    };
});

return app.Run(args);
