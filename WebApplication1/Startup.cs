using Azure.Storage.Blobs;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SignalR.Hub;
using System.Text;
using TourishApi.Repository.Interface.Restaurant;
using TourishApi.Repository.Interface.Resthouse;
using TourishApi.Repository.Interface.Transport;
using TourishApi.Task;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Model;
using WebApplication1.Repository.InheritanceRepo;
using WebApplication1.Repository.InheritanceRepo.Receipt;
using WebApplication1.Repository.InheritanceRepo.RestaurantPlace;
using WebApplication1.Repository.InheritanceRepo.RestHoouse;
using WebApplication1.Repository.InheritanceRepo.Transport;
using WebApplication1.Repository.Interface;
using WebApplication1.Repository.Interface.Receipt;
using WebApplication1.Service;
using WebApplication1.Trigger;

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
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularOrigins",
                builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed((host) => true)
                   .AllowCredentials();
                });
            });

            services.AddControllers();

            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.AddDbContext<MyDbContext>(option =>
            {
                option.UseSqlServer(Configuration.GetConnectionString("AzureDb"));

                option.UseTriggers(triggerOptions =>
                {
                    triggerOptions.AddTrigger<NotificationConTrigger>();
                    triggerOptions.AddTrigger<MessageConTrigger>();
                });
            });

            services.AddScoped<IPassengerCarRepository, PassengerCarRepository>();
            services.AddScoped<IAirPlaneRepository, AirPlaneRepository>();

            services.AddScoped<IHotelRepository, HotelRepository>();
            services.AddScoped<IHomeStayRepository, HomeStayRepository>();

            services.AddScoped<IRestaurantRepository, RestaurantRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IReceiptRepository, ReceiptRepository>();

            services.AddScoped<ITourishPlanRepository, TourishPlanRepository>();

            services.AddScoped(x => new BlobServiceClient(Configuration.GetValue<string>("AzureBlobStorage")));
            services.AddScoped<IBlobService, BlobService>();

            services.AddSingleton<IUserIdProvider, IdBasedUserIdProvider>();

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
                options.AddPolicy("CreateAirPlaneAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.CREATE_AIRPLANE));
                options.AddPolicy("UpdateAirPlaneAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.UPDATE_AIRPLANE));
                options.AddPolicy("DeleteAirPlaneAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.DELETE_AIRPLANE));

                options.AddPolicy("CreatePassengerCarAccess", policy =>
                                 policy.RequireClaim("Permissions", PolicyTerm.CREATE_PASSENGER_CAR));
                options.AddPolicy("UpdatePassengerCarAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.UPDATE_PASSENGER_CAR));
                options.AddPolicy("DeletePassengerCarAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.DELETE_PASSENGER_CAR));

                options.AddPolicy("CreateHotelAccess", policy =>
                              policy.RequireClaim("Permissions", PolicyTerm.CREATE_HOTEL));
                options.AddPolicy("UpdateHotelAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.UPDATE_HOTEL));
                options.AddPolicy("DeleteHotelAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.DELETE_HOTEL));

                options.AddPolicy("CreateHomeStayAccess", policy =>
                          policy.RequireClaim("Permissions", PolicyTerm.CREATE_HOME_STAY));
                options.AddPolicy("UpdateHomeStayAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.UPDATE_HOME_STAY));
                options.AddPolicy("DeleteHomeStayAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.DELETE_HOME_STAY));

                options.AddPolicy("CreateRestaurantAccess", policy =>
                       policy.RequireClaim("Permissions", PolicyTerm.CREATE_RESTAURANT));
                options.AddPolicy("UpdateRestaurantAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.UPDATE_RESTAURANT));
                options.AddPolicy("DeleteRestaurantAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.DELETE_RESTAURANT));

                options.AddPolicy("CreateTourishPlanAccess", policy =>
                       policy.RequireClaim("Permissions", PolicyTerm.CREATE_TOURISH_PLAN));
                options.AddPolicy("UpdateTourishPlanAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.UPDATE_TOURISH_PLAN));
                options.AddPolicy("DeleteTourishPlanAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.DELETE_TOURISH_PLAN));

                options.AddPolicy("CreateReceiptAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.CREATE_RECEIPT));
                options.AddPolicy("UpdateReceiptAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.UPDATE_RECEIPT));
                options.AddPolicy("DeleteReceiptAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.DELETE_RECEIPT));


                options.AddPolicy("CreateMessageAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.CREATE_MESSAGE));
                options.AddPolicy("UpdateMessageAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.UPDATE_MESSAGE));
                options.AddPolicy("DeleteMessageAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.DELETE_MESSAGE));

                options.AddPolicy("UpdateUserPasswordAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.UPDATE_PASSWORD_USER));
                options.AddPolicy("UpdateUserInfoAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.UPDATE_INFO_USER));
                options.AddPolicy("GetUserListAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.GET_USER_LIST));
                options.AddPolicy("GetUserAccess", policy =>
                                 policy.RequireClaim("Permissions", PolicyTerm.GET_USER));
                options.AddPolicy("SelfGetUserAccess", policy =>
                                 policy.RequireClaim("Permissions", PolicyTerm.SELF_GET_USER));

            });

            services.AddHangfire(config =>
                config.UseSqlServerStorage(Configuration.GetConnectionString("AzureDb")));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyWebApiApp", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [Obsolete]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseDeveloperExceptionPage();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyWebApiApp v1"));
            }

            if (!env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseDeveloperExceptionPage();

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "MyWebApiApp v1");
                    options.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseHangfireServer();
            app.UseHangfireDashboard();

            RecurringJob.AddOrUpdate<ScheduleDateChangeTask>("MyScheduledTask", x => x.ScheduleDateDueTask(), Cron.MonthInterval(1));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationHub>("api/user/notify");
                endpoints.MapHub<UserMessageHub>("api/user/message");

                endpoints.MapControllers();
            });
        }
    }
}