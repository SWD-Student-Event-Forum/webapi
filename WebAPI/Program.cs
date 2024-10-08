﻿using EventZone.Domain.DTOs.EmailModels;
using EventZone.Domain.Entities;
using EventZone.Repositories;
using EventZone.Services.Hubs;
using EventZone.Services.Interface;
using EventZone.Services.Services;
using EventZone.WebAPI.Extensions;
using EventZone.WebAPI.Injection;
using EventZone.WebAPI.MiddleWares;
using EventZone.WebAPI.ModelBinder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container, CONFIG FOR IGNORE NULL OBJECT IN JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        // Camel case
        options.SerializerSettings.ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy() // Use CamelCaseNamingStrategy to apply camelCase
        };
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; // Ignore reference loops
    });
builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new CustomEventProductDetailDTOBinderProvider());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//CONFIG FOR SIGNALR
builder.Services.AddSignalR();

//CONFIG FOR JWT AUTHENTICATION ON SWAGGER
builder.Services.AddSwaggerGen(config =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,//Cũ là apikey
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put Bearer + your token in the box below",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    config.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "EventZone API",
        Description = "EventZone API. This is the description",
        Contact = new OpenApiContact
        {
            Name = "Fanpage",
            Url = new Uri("https://www.facebook.com/profile.php?id=61560347204793")
        },
        License = new OpenApiLicense
        {
            Name = "Front-end URL",
            Url = new Uri("https://eventzone.id.vn/")
        }
    });

    config.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    config.AddSecurityRequirement(new OpenApiSecurityRequirement
{
        {
            jwtSecurityScheme, Array.Empty<string>()
        }
});
    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    config.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    // Cấu hình Swagger để sử dụng Newtonsoft.Json
    config.UseAllOfForInheritance();
});
builder.Services.AddSwaggerGenNewtonsoftSupport(); //chuyen sang pascal nhung mat Ifromfile

//SETUP INJECTION SERVICE
builder.Services.ServicesInjection(builder.Configuration);

//SETUP SERVICE IDENTITY: Allow non alphanumeric
builder.Services.AddIdentityCore<User>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.User.RequireUniqueEmail = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireLowercase = false;
})
    .AddRoles<Role>()
    .AddEntityFrameworkStores<StudentEventForumDbContext>();
//ADD AUTHENTICATION - CONFIG FOR JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
    };
});

builder.Services.AddAuthorization();

//CORS - Set Policy
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicyDevelopement", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000", "http://localhost:3001", "http://127.0.0.1:5500", "https://student-event-forum.netlify.app", "https://webapp-git-main-srudent-event-forum.vercel.app/", "https://eventzone.id.vn");
    });
    //opt.AddPolicy("CorsPolicyProduction", policy =>
    //{
    //    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://student-event-forum.netlify.app");
    //});
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("app-cors",
        builder =>
        {
            builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .WithExposedHeaders("X-Pagination")
            .AllowAnyMethod();
        });
});

//ADD EMAIL CONFIG
var emailConfig = builder.Configuration.GetSection("EmailConfiguration")
                   .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// SCOPE FOR MIGRATION
// explain: The CreateScope method creates a new scope. The scope is a way to manage the lifetime of objects in the container.
var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<StudentEventForumDbContext>();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

try
{
    await DBInitializer.Initialize(context, userManager);
}
catch (Exception ex)
{
    logger.LogError(ex, "An problem occurred during migration!");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(config =>
    {
        // Always keep token after reload or refresh browser
        config.SwaggerEndpoint("/swagger/v1/swagger.json", "EventZone API v.01");
        config.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
        config.InjectJavascript("/custom-swagger.js");
    });
    app.ApplyMigrations(logger);
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<PerformanceTimeMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
// USE AUTHENTICA TION, AUTHORIZATION
app.UseAuthorization();

app.UseAuthentication();

// USE MIDDLEWARE
app.UseMiddleware<UserStatusMiddleware>();

app.MapControllers();
app.MapHub<NotificationHub>("/notification-hub");
app.MapHub<ChatHub>("/chat-hub");
app.MapHub<DataHub>("/data-hub");

//USE CORS
app.UseCors("CorsPolicyDevelopement");
app.UseCors("app-cors");

app.Run();