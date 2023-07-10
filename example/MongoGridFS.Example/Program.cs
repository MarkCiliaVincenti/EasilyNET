using EasilyNET.AutoDependencyInjection.Extensions;
using MongoGridFS.Example;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// 自动注入服务模块
builder.Services.AddApplication<AppWebModule>();

//添加Serilog配置
_ = builder.Host.UseSerilog((hbc, lc) =>
{
    const LogEventLevel logLevel = LogEventLevel.Information;
    _ = lc.ReadFrom.Configuration(hbc.Configuration)
          .MinimumLevel.Override("Microsoft", logLevel)
          .MinimumLevel.Override("System", logLevel)
          .Enrich.FromLogContext()
          .WriteTo.Async(wt =>
          {
              wt.Console();
              wt.Debug();
          });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) _ = app.UseDeveloperExceptionPage();

// 添加自动化注入的一些中间件.
app.InitializeApplication();
app.MapControllers();
app.Run();