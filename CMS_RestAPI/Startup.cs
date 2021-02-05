using CMS_RestAPI.Models;
using CMS_RestAPI.Models.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CMS_RestAPI
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
            services.AddControllers();
            services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("CMS API", new OpenApiInfo()
                {
                    Title = "CMS API",
                    Version = "V.1",
                    Description = "CMS API",
                    Contact = new OpenApiContact()
                    {
                        Email = "nihatcanertug@gmail.com",
                        Name = "NÝHATCAN ERTUÐ",
                        Url = new Uri("https://github.com/nihatcanertug")
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "MIT Licance",
                        Url = new Uri("https://github.com/nihatcanertug")
                    }
                });
                options.AddSecurityDefinition("Jwt", new OpenApiSecurityScheme
                {
                    Description = "Jwt Authentication header using scheme",
                    Name = "Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Jwt",
                });
                //options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference
                //            {
                //                Type = ReferenceType.Schema,
                //                Id = "Jwt",
                //            },
                //            Scheme = "oauth 3.0",
                //            Name = "Jwt",
                //            In = ParameterLocation.Header,
                //        },
                //        new List<string>()
                //    }

                // API'ýn sahib olduðu yetenekler yani Controller içerisindeki Action Metodlarýmýza yazdýðýmýz summary yani özet bilgilerin Swagger UI aracýnda gözükmesi için yapýlan bir konfigurasyon.
                //var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlCommnetFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
                //options.IncludeXmlComments(xmlCommnetFullPath);

            });

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.SecretKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateAudience = false,
                    ValidateIssuer = false
                };
            });



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            //CMS Rest API farklý bir domainde, API'ye request atan web siteside haliyle farklý bir domainde olacaktýr. Farklý origin'lerde kaynaklarda bulunan bu web platformlarýn saðlýklý bir þekilde iletiþim kurmasý için API'ye request atan web sitelerini tanýtmamýz gerekmektedir. Bunun için hiyerarþik olarak bulunan "UseCors" middleware kullanýlacaktýr. Özellikle veritabanýna varlýk insert etmek istediðimizde yada var olan bir valýk üzerinde deðiþiklik yapmak istediðimizde atacaðýmýz requestlerin baþarýlý olmasý için aþaðýda iþlemin yapýlmasý gerekmektedir.

            app.UseCors(options => options
                .AllowAnyOrigin() // Request atan web projesinin bulunduðu domain bilgisi, bu method içerisinde herhangi bir domain bilgisi verilmezse world wide web içerisinde herhangi bir alan adýna sahip web sitesi bize request atabilir.
                .AllowAnyMethod() // Hangi methodlara izin verildiði
                .AllowAnyHeader() // Hangi request header'larýna izin verildiði adým adým burdaki middleware'da titiz bir þekilde belirlenir.
                                  //Standart .Net içerisinde de ayný mantýk bulunmaktadýr. Standart .Net içerisinde Web Config içerisinde genel ayarlar yapýlabilindiði gibi Controller içerisine girerek action methodlara attribute olarakta bu origin bilgileri verile bilir. Method bu ayarlar verilebilir.

            //Asp .Net Core için [EnableCors] attribute vasýtasýyla yapýlýr.

            //Bu global ayarlar controller bazýnda ve action method bazýnda yapýlmaktadýr.
            );

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/CMS API/swagger.json", "CMS API");
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
