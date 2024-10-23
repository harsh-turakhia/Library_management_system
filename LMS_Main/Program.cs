using AspNetCoreHero.ToastNotification;
using LMS_Repository.Interface;
using LMS_Repository.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

// Cookie Authenctication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.LoginPath = "/Home/Login";
        options.LogoutPath = "/Home/Logout";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.Name = "LMS_Cookie";
    });

builder.Services.AddHttpContextAccessor();


// Session Authenctication
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15); // Set timeout as needed
    options.Cookie.HttpOnly = true; // Prevent JavaScript access to cookies
    options.Cookie.IsEssential = true; // Make the session cookie essential
});


// Toast Notification
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 2;
    config.IsDismissable = true;
    config.Position = NotyfPosition.BottomRight;
}
);

// Dependency Injection
builder.Services.AddScoped<IAuth, Auth>();
builder.Services.AddScoped<IHome, Home>();
builder.Services.AddScoped<IUser, User>();
builder.Services.AddScoped<IAdmin, Admin>();
builder.Services.AddScoped<ILib, Lib>();


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Middlewares
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
