using System.Text.Json;
using Chmury.Infrastructure;
using Chmury.Services;
using Microsoft.Extensions.FileProviders;
using Neo4j.Driver;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation(opts =>
    {
        opts.FileProviders.Add(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Views")));
    });
}

builder.Services.AddMvc();
builder.Services.AddControllers();
builder.Services.Configure<Neo4jSettings>(builder.Configuration.GetSection("Neo4j"));

var settings = new Neo4jSettings();
builder.Configuration.GetSection("Neo4j").Bind(settings);

builder.Services.AddSingleton(GraphDatabase.Driver(settings.Neo4jConnection,
    AuthTokens.Basic(settings.Neo4jUser, settings.Neo4jPassword)));

builder.Services.AddScoped<INeo4jService, Neo4jService>();
builder.Services.AddScoped<IUniversityService, UniversityService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();
app.MapControllers();
app.Run();