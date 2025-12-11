var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<FileStoring.Repositories.ISubmissionRepository, FileStoring.Repositories.SqliteSubmissionRepository>();
builder.Services.AddSingleton<FileStoring.Services.IFileService, FileStoring.Services.FileService>();

var app = builder.Build();

app.UseMiddleware<FileStoring.Middleware.ErrorHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();