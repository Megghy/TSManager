using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using TSManager;
using TSManager.Web.Data;

TSManager.Core.Init.Start(Array.Empty<string>());

Logger.Text("初始化网页模块");
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

Logger.Success("网页管理模块启动");
app.Run();
