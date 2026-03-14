using SerializerBenchmark.Core.Interfaces;
using SerializerBenchmark.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = false;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddScoped<ISerializerService, TextJsonService>();
builder.Services.AddScoped<ISerializerService, TextJsonUtf8Service>();
builder.Services.AddScoped<ISerializerService, ProtobufService>();
builder.Services.AddScoped<ISerializerService, MessagePackService>();
builder.Services.AddScoped<ISerializerService, MemoryPackService>();

builder.Services.AddScoped<BenchmarkService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Serializer Benchmark API", Version = "v1" });
});

// CORS — permette richieste dal client React (localhost:5173)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Serializer Benchmark API v1"));
}

app.UseRouting();
app.MapControllers();

// Root redirect to swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

await app.RunAsync();
