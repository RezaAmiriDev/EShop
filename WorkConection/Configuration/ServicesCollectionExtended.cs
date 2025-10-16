using ClassLibrary.Repository;
using ClassLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using ModelLayer.Interface;
using ModelLayer.Reposetory;
using ServiceLayer.Services;
using WebFrameWork.Mapper;


namespace WebFrameWork.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterServicesEntities(this IServiceCollection services)
        {
            services.AddScoped<IAdressRepository, AddressRepo>();
            services.AddScoped<ICustomerRepository, CustomerRepo>();
            services.AddScoped<IProductRepository, ProductRepo>();
            services.AddScoped<IOrderReposetory, OrderRepo>();
            services.AddScoped<ISellerRepository , SellerRepo>();


            services.AddScoped<CustomerService>();
            services.AddScoped<ProductService>();
            services.AddScoped<SellerService>();

        }
        //public static void RegisterJwtService(this IServiceCollection services, SecuritySetting _siteSetting)
        //{
        //    services.AddAuthentication(options =>
        //    {
        //        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        //    }).AddJwtBearer(options =>
        //    {
        //        var secretKey = Encoding.UTF8.GetBytes(_siteSetting.SecretKey);
        //        ///Encrypting
        //        var keyEncrypting = Encoding.UTF8.GetBytes(_siteSetting.Encryptkey);
        //        var securityKeyEncryp = new SymmetricSecurityKey(keyEncrypting);
        //        var validationParameters = new TokenValidationParameters
        //        {
        //            ClockSkew = TimeSpan.Zero,
        //            RequireSignedTokens = true,
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        //            TokenDecryptionKey = securityKeyEncryp,
        //            RequireExpirationTime = true,
        //            ValidateLifetime = true,
        //            ValidateAudience = true,
        //            ValidAudience = _siteSetting.Audience,
        //            ValidateIssuer = true,
        //            ValidIssuer = _siteSetting.Issuer,

        //        };
        //        options.RequireHttpsMetadata = false;
        //        options.SaveToken = true;
        //        options.TokenValidationParameters = validationParameters;
        //    });

        //    services.AddCors(options =>
        //    {
        //        options.AddPolicy("EnableCors", builder =>
        //        {
        //            builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
        //           .Build();

        //        });
        //    });

        //}


    }
}
