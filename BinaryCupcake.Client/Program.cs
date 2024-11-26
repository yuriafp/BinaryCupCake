using BinaryCupcake.Client;
using BinaryCupcake.Client.Autenticacao;
using BinaryCupcake.Client.Pages.Outros;
using BinaryCupcake.Client.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Syncfusion.Blazor;
using Syncfusion.Licensing;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NHaF5cXmVCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWH5fcnVRQ2VdUUB+WUQ=");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IProdutoService, ClientServices>();
builder.Services.AddScoped<IUsuarioService, ClientServices>();
builder.Services.AddScoped<AutenticacaoService>();
//builder.Services.AddScoped<MessageDialog>();
builder.Services.AddScoped<ICarrinho, ClientServices>();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

builder.Services.AddAuthorizationCore();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
