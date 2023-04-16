using steamAuth.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 디비 컨텍스트 추가
builder.Services.AddDbContext<DBContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("UserInfoDb"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("UserInfoDb"))));


// Add services to the container.
string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
Environment.SetEnvironmentVariable("LD_LIBRARY_PATH", currentDirectory);

builder.Services.AddControllers();
builder.Services.AddHttpClient();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
