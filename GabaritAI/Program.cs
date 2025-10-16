using GabaritAI.Services;

var builder = WebApplication.CreateBuilder(args);

// Configura sessões
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Adiciona Razor Pages
builder.Services.AddRazorPages();

// Registra o serviço de memoria da conversa
builder.Services.AddSingleton<IChatMemoryService, ChatMemoryService>();

// Registra o serviço de RAG
builder.Services.AddSingleton<IRagService, RagService>();

// ✅ Corrigido: Registra o serviço OpenAI com HttpClient (sem alterar mais nada)
builder.Services.AddHttpClient<IOpenAIService, OpenAIService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.MapRazorPages();

app.Run();
