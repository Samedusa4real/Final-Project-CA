using Backend___Putka.DAL;
using Backend___Putka.Models;
using Backend___Putka.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<PutkaDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 8;
    opt.Password.RequireUppercase = false;
}).AddDefaultTokenProviders().AddEntityFrameworkStores<PutkaDbContext>();


builder.Services.AddScoped<LayoutService>();

builder.Services.AddHttpContextAccessor();

//builder.Services.ConfigureApplicationCookie(options =>
//{
//    bool alreadyRedirected = false;

//    options.Events.OnRedirectToLogin = options.Events.OnRedirectToAccessDenied = context =>
//    {
//        if (!alreadyRedirected && context.HttpContext.Request.Path.Value.StartsWith("/manage"))
//        {
//            var redirectUri = new Uri(context.RedirectUri);
//            context.Response.Redirect("/manage/account/login" + redirectUri.Query);

//            alreadyRedirected = true;
//        }

//        return Task.CompletedTask;
//    };

//});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
