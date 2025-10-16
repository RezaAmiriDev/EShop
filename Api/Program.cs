using ClassLibrary.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebFrameWork.Configuration;
using WebFrameWork.Mapper;
using ServiceLayer.Services;
using ClassLibrary.Repository;
using ModelLayer.Reposetotry;


var builder = WebApplication.CreateBuilder(args);


// DbContext & Identity
builder.Services.AddDbContext<MobiContext>(options =>
 options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser , IdentityRole>()
    .AddEntityFrameworkStores<MobiContext>().AddDefaultTokenProviders();

// 3. register IHttpContextAccessor اگر نیاز داری
builder.Services.AddHttpContextAccessor();

// 4. حالا repositoryها را register کن (extension یا مستقیم)
builder.Services.RegisterServicesEntities(); // یا builder.Services.RegisterServiesEntities() طبق نام متد تو


// JWT Auth
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = jwtSection.GetValue<string>("Key");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection.GetValue<string>("Issuer"),
        ValidAudience = jwtSection.GetValue<string>("Audience"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!))
    };
});

// CORS — اجازه به frontend (تغییر آدرس به آدرس وب شما)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", policy =>
        {
            policy.WithOrigins("https://localhost:7164") // آدرس frontend (web)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // اگر کوکی یا احراز هویت با credentials دارید
        });
});

// Generic repo
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repos<>));

builder.Services.AddAutoMapper(cfg => { }, typeof(MapperConfig).Assembly);

// ثبت سرویس‌ها
builder.Services.AddScoped<ICustomerRepository, CustomerRepo>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowWebApp");

// pipeline
app.UseAuthentication(); // <- مهم: قبل از UseAuthorization // اگر JWT یا auth داری
app.UseAuthorization();

app.MapControllers();

app.Run();
