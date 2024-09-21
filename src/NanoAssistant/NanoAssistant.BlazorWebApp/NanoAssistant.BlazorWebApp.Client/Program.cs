using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using NanoAssistant.BlazorWebApp.Client.IdentityComponents;
using NanoAssistant.BlazorWebApp.Client.MessageParsers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);




builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

builder.Services.AddSingleton<IMessageParser, ChartMessageParser>();


builder.Services.AddHttpClient("default", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("default"));


builder.Services.AddMudServices();

await builder.Build().RunAsync();
