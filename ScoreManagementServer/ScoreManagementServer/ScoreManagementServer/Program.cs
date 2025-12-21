using ScoreManagementServer.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ⚠️ 明确指定监听地址
builder.WebHost.UseUrls("http://localhost:5000");

// 添加控制器支持
builder.Services.AddControllers();

// 添加 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 👇 【关键】注册数据库上下文
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseSqlite("Data Source=StudentsData.db"));

// CORS 配置
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUnity", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


var app = builder.Build();

// 开发环境启用 Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowUnity");

// 映射控制器路由
app.MapControllers();

app.Run();