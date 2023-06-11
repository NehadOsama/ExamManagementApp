using CemexExamApp.ContextData;
using CemexExamApp.Models;
using CemexExamApp.Models.Repositories;
using CemexExamApp.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net;
using UoN.ExpressiveAnnotations.NetCore.DependencyInjection;
using Wkhtmltopdf.NetCore;
//using static CemexExamApp.ContextData.ExamManagmentAppContext;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDataProtection();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);//You can set Time 
                                                   //  options.Cookie.Name = ".AspNetCore.Session";
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
});

//builder.Services.AddMvc(options =>
//{
//    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
//    options.Filters.Add(new AuthorizeFilter(policy));
//});

//builder.Services.ConfigureApplicationCookie(Options =>
//{

//    Options.AccessDeniedPath = "/Account/Login";
//    Options.LoginPath = "/Account/Login";
//    Options.ReturnUrlParameter = "/Account/Login";
//});
builder.Services.AddScoped<ICemexManagExam<Topic>, TopicRepository>();
builder.Services.AddScoped<ICemexManagExam<Question>, QuestionRepository>();
builder.Services.AddScoped<ICemexManagExam<SecUser>, UserDbRepository>();
builder.Services.AddScoped<IViewRenderService, ViewRenderService>();
builder.Services.AddWkhtmltopdf("WKhtmltopdf");
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSession();
builder.Services.AddAuthorization();


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ExamManagmentAppContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("ExamDB")));
builder.Services.AddExpressiveAnnotations();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.SlidingExpiration = true;
    options.AccessDeniedPath = "/Account/Login";
    options.LoginPath = "/Account/Login";
    options.ForwardForbid = "/Account/Login";
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.UseRequestLocalization();
//app.UseMiddleware<ExceptionHandlingMiddleware>();
//app.MapDefaultControllerRoute();
//app.UseMvc(
//    configureRoutes => {
//        configureRoutes.MapAreaRoute(name: "Exam", areaName: "Exam", template: "{Exam}/{controller=ExamTaker}/{action=ConfirmEmail}/{id?}");
//        });
//app.UserCookiAuthentication();
app.MapControllerRoute(
    
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
