using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using MudBlazor.Services;
using NanoAssistant.BlazorWebApp.Client.Pages;
using NanoAssistant.BlazorWebApp.Components;
using NanoAssistant.BlazorWebApp.IdentityComponents;
using NanoAssistant.Core.SemanticPlugins;
using NanoAssistant.Core.Serializers;
using NanoAssistant.Core.Services;
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
        // Use ADO.NET for clustering
        Console.WriteLine("Using npsql");
        siloBuilder.UseAdoNetClustering(options =>
        {
            options.Invariant = "Npgsql";
            options.ConnectionString = siloBuilder.Configuration.GetConnectionString("Clustering");
        });
    }

    siloBuilder.Services.AddSerializer(serializerBuilder =>
    {
        serializerBuilder.AddJsonSerializer(isSupported: type => true);
    });

});


var kernelBuilder = builder.Services.AddKernel();

kernelBuilder.AddOpenAIChatCompletion(builder.Configuration["OpenAI:Model"], builder.Configuration["OpenAI:ApiKey"]);
//kernelBuilder.Plugins.AddFromType<FinanceTrackerPlugin>();

builder.Services.AddSingleton<PromptExecutionSettings>(new OpenAIPromptExecutionSettings()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
});

builder.Services.AddHttpClient<INanoFinanceTrackerService, NanoFinanceTrackerService>(client => {
    client.BaseAddress = new Uri(builder.Configuration["FinanceTracker:BaseUrl"]!);
});

builder.Services.AddTransient<FinanceTrackerPlugin>();

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"]!;
    options.ClientId = builder.Configuration["Auth0:ClientId"]!;
    options.ClientSecret = builder.Configuration["Auth0:ClientSecret"]!;
}).WithAccessToken(options =>
{
    options.Audience = "nano-finance-tracker";
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingServerAuthenticationStateProvider>();

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

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(NanoAssistant.BlazorWebApp.Client._Imports).Assembly);

app.MapGet("/Account/Login", async (HttpContext httpContext, string returnUrl = "/") =>
{
    var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            .WithRedirectUri(returnUrl)
            .Build();

    await httpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
});

app.MapGet("/Account/Logout", async (HttpContext httpContext) =>
{
    var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
            .WithRedirectUri("/")
            .Build();

    await httpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
});

app.Run();
