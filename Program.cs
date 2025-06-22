using copilot_api.Services;
using copilot_api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add in-memory user service
builder.Services.AddSingleton<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// 1. Error handling middleware (first to catch all exceptions)
app.UseMiddleware<ErrorHandlingMiddleware>();

// 2. Authentication middleware (API key validation)
app.UseMiddleware<ApiKeyAuthenticationMiddleware>();

// 3. Request logging middleware
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
