using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using API.Data;
using API.IRepository;
using API.IService;
using API.Models;
using API.Repository;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using API.DTOs.Auth;
using API.Validator;
using System.Web.WebPages;
using FluentValidation;
using API.DTOs.Employee;
using System.Text.Json.Serialization;
using TutaSpa.API.IService;
using TutaSpa.Data;
using DinkToPdf.Contracts;
using DinkToPdf;
using System.IdentityModel.Tokens.Jwt;
using ChatSupport.Hubs;
using API.ChatHub;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddLogging();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddResponseCaching();


//cấu hình jwt 
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                 Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireAssertion(context =>
        {
            var tokenTypeClaim = context.User.FindFirst("Permission");
            return tokenTypeClaim == null || tokenTypeClaim.Value != "ChangePassword";
        })
        .Build();

    // ✅ Policy riêng cho token đổi mật khẩu
    options.AddPolicy("ChangePasswordOnly", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("Permission", "ChangePassword"));
});



// Cấu hình db
builder.Services.AddDbContext<ApplicationDbContext>(options => options
                .UseSqlServer(builder.Configuration
                .GetConnectionString("DefaultConnection"))
    );

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
}
).AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


builder.Services.AddSignalR();



//builder.Services.AddSingleton<OtpService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IOTPService, OtpService>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductBatchService, ProductBatchService>();

builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGmailService, GmailService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
//Validators
builder.Services.AddScoped<IValidator<RegisterDTO>, RegisterValidator>();
builder.Services.AddScoped<IValidator<CreateEmployeeDTO>, CreateEmployValidator>();
builder.Services.AddScoped<IValidator<ResetPassDTO>, ChangePasswordValidator>();

// MongoDB configuration
builder.Services.AddScoped<MongoDBInitialCreate>();


//config
builder.Services.Configure<GmailSettings>(builder.Configuration.GetSection("GmailSettings"));
builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection("SendGridSettings"));
builder.Services.Configure<SmsSettings>(builder.Configuration.GetSection("SmsSettings"));
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });



builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<IOTPService, OtpService>();
var userFrontendUrl = builder.Configuration.GetSection("FrontendUrl")["UserFrontend"] ?? "http://localhost:5173";
var adminFrontendUrl = builder.Configuration.GetSection("FrontendUrl")["AdminFrontend"] ?? "http://localhost:5174";

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp",
        builder =>
        {
            builder
                .WithOrigins(userFrontendUrl, adminFrontendUrl,"https://admin-theta-one-30.vercel.app")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();// Quan trọng nếu dùng SignalR
        });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
    });

}

app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    var token = context.Request.Query["access_token"].ToString();


    Console.WriteLine(token);

    if (path.StartsWithSegments("/chat") && !string.IsNullOrEmpty(token))
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            context.User = principal;
        }
        catch
        {


            context.Response.StatusCode = 401;
            return;
        }
    }

    await next();
});

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowVueApp");

app.UseStaticFiles();


app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCaching();
app.UseEndpoints(enpoints =>
{
    enpoints.MapControllers();

    enpoints.MapHub<ChatHub>("/chat").RequireCors("AllowVueApp");
    enpoints.MapHub<BookingHub>("/bookingHub").RequireCors("AllowVueApp");
}
);

app.Run();
