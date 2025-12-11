var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("filestoring", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("FileStoring:BaseUrl") ?? "http://filestoring:80");
});

builder.Services.AddSingleton<FileAnalysisService.Repositories.IReportRepository, FileAnalysisService.Repositories.SqliteReportRepository>();
builder.Services.AddSingleton<FileAnalysisService.Services.IAnalysisService, FileAnalysisService.Services.AnalysisService>();

var app = builder.Build();
app.UseMiddleware<FileAnalysisService.Middleware.ErrorHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();