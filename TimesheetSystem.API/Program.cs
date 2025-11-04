using TimesheetSystem.Common.Interfaces;
using TimesheetSystem.Common.Services;

string allowedOrigin = "_timeSheetOrigin";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ITimesheetService, TimesheetService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowedOrigin,
        policy =>
        {
            policy.WithOrigins("https://localhost:7131")
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(allowedOrigin);

app.MapControllers();

app.Run();
