var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("filestoring", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("FileStoring:BaseUrl") ?? "http://filestoring:80");
});
builder.Services.AddHttpClient("analysis", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("Analysis:BaseUrl") ?? "http://analysis:80");
});

builder.Services.AddScoped<Gateway.Services.IGatewayService, Gateway.Services.GatewayService>();

var app = builder.Build();
app.UseMiddleware<Gateway.Middleware.ErrorHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();