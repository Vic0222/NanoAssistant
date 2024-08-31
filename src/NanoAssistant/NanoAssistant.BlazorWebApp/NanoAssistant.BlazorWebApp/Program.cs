using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using MudBlazor.Services;
using NanoAssistant.BlazorWebApp.Client.Pages;
using NanoAssistant.BlazorWebApp.Components;
using NanoAssistant.Core.SemanticPlugins;
using NanoAssistant.Core.Serializers;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Serialization;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

var isDevelopment = builder.Environment.IsDevelopment();
if (!isDevelopment)
{
    builder.Logging.ClearProviders();
    builder.Logging.AddJsonConsole();
}
// Add orleans
builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.AddAdoNetGrainStorageAsDefault(options =>
    {
        options.Invariant = "Npgsql";
        options.ConnectionString = siloBuilder.Configuration.GetConnectionString("GrainStorage");
        options.GrainStorageSerializer = new SystemTextJsonSerializer();
    });
    string? flyPrivateIP = siloBuilder.Configuration["FLY_PRIVATE_IP"];
    if (!string.IsNullOrEmpty(flyPrivateIP)) //this means we are running in fly.io
    {
        siloBuilder.Configure<EndpointOptions>(options =>
        {
            options.AdvertisedIPAddress = IPAddress.Parse(flyPrivateIP);
        });
    }

    if (isDevelopment)
    {
        siloBuilder.UseLocalhostClustering();
    }
    else
    {
        siloBuilder.UseLocalhostClustering();
    }

    siloBuilder.Services.AddSerializer(serializerBuilder =>
    {
        serializerBuilder.AddJsonSerializer(isSupported: type => true);
    });

});


var kernelBuilder = builder.Services.AddKernel();

kernelBuilder.AddOpenAIChatCompletion(builder.Configuration["OpenAI:Model"], builder.Configuration["OpenAI:ApiKey"]);
kernelBuilder.Plugins.AddFromType<FinanceTrackerPlugin>("FinanceTracker");

builder.Services.AddSingleton<PromptExecutionSettings>(new OpenAIPromptExecutionSettings()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
});

//builder.Services.AddHttpClient("default");
//builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("default"));
// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(NanoAssistant.BlazorWebApp.Client._Imports).Assembly);

app.Run();
