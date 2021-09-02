using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using TxSpareParts.Core.Entities;
using TxSpareParts.Core.Interfaces;
using TxSpareParts.Infastructure.Data;
using TxSpareParts.Infastructure.Filters;
using TxSpareParts.Infastructure.Interfaces;
using TxSpareParts.Infastructure.Repository;
using TxSpareParts.Infastructure.services;
using TxSpareParts.Utility;
using TxSpareParts.Utility.interfaces;
using TxSpareParts.Utility.Interfaces;
using TxSpareParts.Utility.Options;

namespace TxSpareParts
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.Configure<PasswordOptions>(Configuration.GetSection("PasswordOptions"));
            //services.AddTransient<IPasswordHasher, PasswordHashHandler>();
            services.Configure<EmailOptions>(options =>
            {
                options.Host_Address = Configuration["SMTP:Host_address"];
                options.Host_Port = Convert.ToInt32(Configuration["SMTP:Host_port"]);
                options.Host_Username = Configuration["SMTP:Host_username"];
                options.Host_Password = Configuration["SMTP:Host_password"];
                options.Sender_Email = Configuration["SMTP:Sender_email"];
                options.Sender_Name = Configuration["SMTP:Sender_name"];  
            });
            services.Configure<PhoneNumberOptions>(Configuration.GetSection("Twilio"));
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
             {
                 options.Password.RequireDigit = true;
                 options.Password.RequireLowercase = true;
                 options.Password.RequireUppercase = true;   
                 options.Password.RequiredLength = 10;
                 options.User.RequireUniqueEmail = true;        
             }).AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();
        
            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;   
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["JWT:Providers:validAudience"],
                    ValidIssuer = Configuration["JWT:Providers:validIssuer"],
                    RequireExpirationTime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Providers:Key"])),
                    ValidateIssuerSigningKey = true
                };
            }).AddGoogle(google => {
                google.ClientId = Configuration["Google:ClientId"];
                google.ClientSecret = Configuration["Google:ClientSecret"];
                google.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddFacebook(facebook => {
                facebook.AppId = Configuration["Facebook:AppId"];
                facebook.AppSecret = Configuration["Facebook:AppSecret"];  
                facebook.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });
            services.AddAuthorization(option =>
            {
                option.AddPolicy("ChiefAuthorization", policy => policy.RequireClaim("AdminStatus"));
                option.AddPolicy("SupervisorAuthorization", policy => policy.RequireClaim("SupervisorStatus"));
                option.AddPolicy("StaffAuthorization", policy => policy.RequireClaim("StaffStatus"));   
            });
            services.AddControllers(option =>
            {
                option.Filters.Add<GlobalExceptionFilter>();
            }).AddNewtonsoftJson(option=> {
                option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                option.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            }).ConfigureApiBehaviorOptions(option=>
            {
                option.SuppressModelStateInvalidFilter = true;
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); 
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TxSpareParts", Version = "v1" });
            });

            services.AddMvc(option => 
                {
                option.Filters.Add<ValidationFilter>();     
                })
                    .AddFluentValidation(option => option.RegisterValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

            services.AddRazorPages();
         
            services.AddSingleton<IEmailSender, EmailHandler>();
            services.AddTransient<IJwtTokenHandler, JwtTokenHandler>();
            services.AddTransient<IPhoneNumberVerification, PhoneVerificationHandler>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IProductService, ProductService>();                           
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IPaymentProcessingHandler, PaymentProcessingHandler>();
            services.AddTransient<IEmailConfirmationHandler, EmailConfirmationHandler>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IImageHandler, ImageHandler>();
            services.AddTransient<IInvoiceHandler, InvoiceHandler>();
            services.AddSingleton<IUriService>( provider =>
            {
                var accessor = provider.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var absolute_uri = string.Concat(request.Scheme, "://",request.Host.ToUriComponent());
                return new UriService(absolute_uri);   
            });

            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TxSpareParts v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "areas",  
                    pattern: "/api/{area:exists}/{controller=Home}/{action=Index}/{id?}"                
                );
            });
           
        }
    }
}
