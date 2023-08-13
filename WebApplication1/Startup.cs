using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Model;
using WebApplication1.Repository.InheritanceRepo;
using WebApplication1.Repository.Interface;
using WebApplication1.Repository.Interface.Relation;
using WebApplication1.Repository.Relation.InheritanceRepo;

namespace MyWebApiApp
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

            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.AddDbContext<MyDbContext>(option =>
            {
                option.UseSqlServer(Configuration.GetConnectionString("MyDB"));
            });

            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IPublisherRepository, PublisherRepository>();

            // Relation
            services.AddScoped<IBookCategoryRepository, BookCategoryRepository>();
            services.AddScoped<IBookVoucherRepository, BookVoucherRepository>();
            services.AddScoped<IBookAuthorRepository, BookAuthorRepository>();
            services.AddScoped<IBookPublisherRepository, BookPublisherRepository>();

            services.Configure<AppSetting>(Configuration.GetSection("AppSettings"));

            var secretKey = Configuration["AppSettings:SecretKey"];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        //tự cấp token
                        ValidateIssuer = false,
                        ValidateAudience = false,

                        //ký vào token
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

                        ClockSkew = TimeSpan.Zero
                    };
                });

            // Book permission
            services.AddAuthorization(options =>
            {
                options.AddPolicy("CreateBookAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.CREATE_BOOK));
                options.AddPolicy("UpdateBookAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.UPDATE_BOOK));
                options.AddPolicy("DeleteBookAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.DELETE_BOOK));

                options.AddPolicy("CreateCategoryAccess", policy =>
                                 policy.RequireClaim("Permissions", PolicyTerm.CREATE_CATEGORY));
                options.AddPolicy("UpdateCategoryAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.UPDATE_CATEGORY));
                options.AddPolicy("DeleteCategoryAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.DELETE_CATEGORY));

                options.AddPolicy("CreateVoucherAccess", policy =>
                                policy.RequireClaim("Permissions", PolicyTerm.CREATE_VOUCHER));
                options.AddPolicy("UpdateVoucherAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.UPDATE_VOUCHER));
                options.AddPolicy("DeleteVoucherAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.DELETE_VOUCHER));

                options.AddPolicy("CreatePublisherAccess", policy =>
                               policy.RequireClaim("Permissions", PolicyTerm.CREATE_PUBLISHER));
                options.AddPolicy("UpdatePublisherAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.UPDATE_PUBLISHER));
                options.AddPolicy("DeletePublisherAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.DELETE_PUBLISHER));

                options.AddPolicy("CreateAuthorAccess", policy =>
                             policy.RequireClaim("Permissions", PolicyTerm.CREATE_AUTHOR));
                options.AddPolicy("UpdateAuthorAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.UPDATE_AUTHOR));
                options.AddPolicy("DeleteAuthorAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.DELETE_AUTHOR));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyWebApiApp", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyWebApiApp v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}