
using Business.Extensions;
using DataAccess;
using DataTransferObject;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using ElmahCore.Mvc;
using ElmahCore.Sql;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Globalization;
using Website.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'AplicationDBContextConnection' not found.");
    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor().AddCircuitOptions(o =>
        o.DetailedErrors = true
    );


    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        // define the list of cultures your app will support
        var supportedCultures = new List<CultureInfo>()
        {
            new CultureInfo("es-AR")
        };
        // set the default culture
        options.DefaultRequestCulture = new RequestCulture("es-AR");
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;
        options.RequestCultureProviders = new List<IRequestCultureProvider>() {


            new QueryStringRequestCultureProvider() // Here, You can also use other localization provider
        };
    });
    var culture = new CultureInfo("es-AR");
    CultureInfo.DefaultThreadCurrentCulture = culture;
    CultureInfo.DefaultThreadCurrentUICulture = culture;

//Configuracion de las variables desde el json de appsettings a clases
IConfiguration mailConfiguration = builder.Configuration.GetSection("MailSettings");
    builder.Services.Configure<MailSettings>(mailConfiguration);
    

    IConfiguration appConfiguration = builder.Configuration.GetSection("AppSettings");
    builder.Services.Configure<AppSettings>(appConfiguration);
    

// Componentes Radzen
builder.Services.AddScoped<DialogService>();
    builder.Services.AddScoped<NotificationService>();
    builder.Services.AddScoped<TooltipService>();
    builder.Services.AddScoped<ContextMenuService>();


builder.Services.AddDbContext<AplicationDBContext>();

builder.Services.AddDefaultIdentity<UserProfile>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<AplicationDBContext>()
                    .AddDefaultTokenProviders()
                    .AddErrorDescriber<CustomIdentityErrorDescriber>()
                    ;


builder.Services.Configure<IdentityOptions>(options =>
{

    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;

    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "GLP.Cookie";
    options.Cookie.Path = "/";
            
    //options.ExpireTimeSpan = TimeSpan.FromMinutes(15);    //No debe llevar este item, muere con la session
    options.SlidingExpiration = true;
    options.LoginPath = new PathString("/Account/Login");
    options.LogoutPath = new PathString("/Account/Logout");
    options.AccessDeniedPath = new PathString("/Account/AccessDenied");
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<UserManager<UserProfile>>();
builder.Services.AddTransient<SignInManager<UserProfile>>();
builder.Services.AddSingleton<AppState>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticateStateProvider>();
builder.Services.AddScoped<CustomAuthenticateStateProvider>();
builder.Services.AddScoped<Business.Interfaces.IStorageService, ProtectedStorageService>();




builder.Services.AddElmah<SqlErrorLog>(options =>
{
    options.ConnectionString = connectionString;
    //options.SqlServerDatabaseSchemaName = "Errors"; //Defaults to dbo if not set
    options.SqlServerDatabaseTableName = "ElmahErrors"; //Defaults to ELMAH_Error if not set
});

//Agrega extensiones de las demás capas donde estan las inyecciones de dependencias de cada capa.
builder.Services.AddBusinessLayer(builder.Configuration);



var app = builder.Build();

// Middleware que intercepta requests a archivos .map y responde con 204 No Content
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? string.Empty;
if (path.EndsWith(".map", StringComparison.OrdinalIgnoreCase))
{
    // Respuesta vacía exitosa: evita 404 y que Elmah lo registre
    context.Response.StatusCode = 204; // No Content
    await context.Response.WriteAsync(""); // Inicializa el body y evita nulls
    return;
}

await next();
});


if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseCookiePolicy(new CookiePolicyOptions
{
    Secure = CookieSecurePolicy.Always
});



//Se agregar la autenticacion y autorizaci�n
app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();
app.UseElmah();

app.UseHttpsRedirection();
app.UseStaticFiles();


app.MapBlazorHub(option =>
{
    option.CloseOnAuthenticationExpiration = true; //This option is used to enable authentication expiration tracking which will close connections when a token expires  
    option.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
});

app.MapFallbackToPage("/_Host");
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

