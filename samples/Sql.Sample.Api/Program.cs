using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Sql.Sample.Api;
using Sql.Sample.Api.Domain;
using Sql.Sample.Api.Services;
using System;
using System.IO;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDomain("Data source=Sql_Sample.Db");
builder.Services.AddServices();

builder.Services.AddLogging(cfg => cfg.AddSimpleConsole(opts =>
{
    opts.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffffffZ-";
    opts.ColorBehavior = LoggerColorBehavior.Enabled;
}));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    setup.IncludeXmlComments(xmlPath);
});

builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

var app = builder.Build();

await using var scope = app.Services.CreateAsyncScope();

var context = scope.ServiceProvider.GetRequiredService<Context>();

await context.Database.MigrateAsync();
await context.SeedWithSomeDataAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.Run();