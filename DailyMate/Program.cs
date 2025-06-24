using ApplicationCore.Abstraction;
using ApplicationCore.Concrete;
using Domain;
using Domain.Entites;
using Domain.Repository;
using Infrastructure;
using Infrastructure.Persistence;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ------------- Genel Log Ayar� -------------
Console.WriteLine("?? JWT KEY: " + builder.Configuration.GetValue<string>("JwtKey"));
IdentityModelEventSource.ShowPII = true; // imza hatas�nda stacktrace g�rebilmek i�in

// ------------- Servisler -------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerCollection(builder.Configuration);   // JWT�li Swagger

builder.Services.AddPersistence(builder.Configuration);

// **** 1) Identity�yi yaln�zca Store + Manager i�in ekle  ****
// Cookie auth �emas� y�klenmesin diye SignInManager kullanm�yorsak Cookie�yi iptal ediyoruz.
builder.Services
    .AddIdentityCore<ApplicationUser>(options => { })          // Cookie eklemez
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// **** 2) JWT�i a��k�a default �ema yap ****
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var key = builder.Configuration.GetValue<string>("JwtKey")!;
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };

    // Ayr�nt�l� event loglar�
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            Console.WriteLine($"?? Header Token: {ctx.Request.Headers["Authorization"]}");
            return Task.CompletedTask;
        },
        OnTokenValidated = ctx =>
        {
            Console.WriteLine("? TOKEN GE�ERL�");
            foreach (var c in ctx.Principal!.Claims)
                Console.WriteLine($"   � {c.Type}: {c.Value}");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = ctx =>
        {
            Console.WriteLine("? TOKEN DO�RULANAMADI:");
            Console.WriteLine(ctx.Exception.ToString());
            return Task.CompletedTask;
        },
        OnChallenge = ctx =>
        {
            Console.WriteLine($"?? Challenge ? error: {ctx.Error} | desc: {ctx.ErrorDescription}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// --- Uygulama Katman� ---
builder.Services.AddScoped<ApiResponse>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IHabitService, HabitService>();
builder.Services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));
builder.Services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFlutterApp",
        builder => builder
            .AllowAnyOrigin() // Geli�tirme s�recinde. Prod'da sadece Flutter URL'sine izin ver!
            .AllowAnyMethod()
            .AllowAnyHeader());
});

//builder.WebHost.UseUrls("http://0.0.0.0:5148");
// ------------- Pipeline -------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// �stek ba��nda header�� konsola d�ken k���k middleware
app.Use(async (ctx, next) =>
{
    Console.WriteLine($"?? {ctx.Request.Method} {ctx.Request.Path}");
    await next();
});
app.UseCors("AllowFlutterApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ------------- �al��t�r -------------
try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("?? Uygulama ��kt�: " + ex);
    throw;
}
