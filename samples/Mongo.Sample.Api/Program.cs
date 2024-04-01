using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using MongoDb.Sample.Api;
using MongoDb.Sample.Api.Domain;
using MongoDb.Sample.Api.Domain.Repositories.Interfaces;
using MongoDb.Sample.Api.Services;
using System;
using System.IO;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
//mongodb://sa:1$1!11aS@localhost:27017
builder.Services.AddDomain("mongodb://sa:s123!$aS@localhost:27017", "Sample");
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

var categoryRepository = app.Services.GetRequiredService<ICategoryRepository>();

await categoryRepository.SeedWithSomeDataAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.Run();