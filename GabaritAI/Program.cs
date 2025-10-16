using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GabaritAI.Services;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao container
builder.Services.AddRazorPages();
builder.Services.AddSession();

// Registra o serviço da OpenAI
builder.Services.AddSingleton<IOpenAIService>(sp =>
{
    var httpClient = new HttpClient();
    var apiKey = Environment.GetEnvironmentVariable("API_KEY") ?? "SUA_CHAVE_AQUI";
    return new OpenAIService(httpClient, apiKey);
});

var app = builder.Build();

// Configuração do pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();

app.Run();
