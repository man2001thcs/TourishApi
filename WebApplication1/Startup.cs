using Azure.Storage.Blobs;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SignalR.Hub;
using StackExchange.Redis;
using System.Text;
using TourishApi.Service.InheritanceService;
using TourishApi.Service.InheritanceService.Schedule;
using TourishApi.Service.Payment;
using TourishApi.Task;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Model;
using WebApplication1.Model.Payment;
using WebApplication1.Repository.InheritanceRepo;
using WebApplication1.Repository.InheritanceRepo.Connect;
using WebApplication1.Repository.InheritanceRepo.Receipt;
using WebApplication1.Repository.InheritanceRepo.RestaurantPlace;
using WebApplication1.Repository.InheritanceRepo.RestHoouse;
using WebApplication1.Repository.InheritanceRepo.Transport;
using WebApplication1.Repository.Interface;
using WebApplication1.Service;
using WebApplication1.Service.InheritanceService;
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

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(Configuration.GetConnectionString("Redis")));

            services.AddScoped(x => new BlobServiceClient(Configuration.GetValue<string>("AzureBlobStorage")));

            // Repo
            services.AddScoped<RestHouseContactRepository>();
            services.AddScoped<MovingContactRepository>();
            services.AddScoped<RestaurantRepository>();
            services.AddScoped<TourishPlanRepository>();
            services.AddScoped<TourishCommentRepository>();
            services.AddScoped<TourishRatingRepository>();
            services.AddScoped<TourishCategoryRepository>();
            services.AddScoped<NotificationRepository>();
            services.AddScoped<GuestMessageConHistoryRepository>();
            services.AddScoped<TourishOuterScheduleRepository>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ReceiptRepository>();
            services.AddScoped<ITourishPlanRepository, TourishPlanRepository>();
            services.AddScoped<IFileRepository, FileRepository>();

            // Service
            services.AddScoped<RestHouseContactService>();
            services.AddScoped<MovingContactService>();
            services.AddScoped<RestaurantService>();
            services.AddScoped<TourishPlanService>();
            services.AddScoped<TourishCommentService>();
            services.AddScoped<TourishRatingService>();
            services.AddScoped<TourishCategoryService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<GuestMessageConHistoryService>();
            services.AddScoped<ReceiptService>();
            services.AddScoped<UserService>();

            services.AddScoped<PaymentService>();
            services.AddHttpClient<PaymentService>();

            services.AddTransient<ISendMailService, SendMailService>();

            services.AddScoped<MovingScheduleService>();
            services.AddScoped<EatScheduleService>();
            services.AddScoped<StayingScheduleService>();

            services.AddScoped<IBlobService, BlobService>();
            services.AddSingleton<IUserIdProvider, IdBasedUserIdProvider>();

            services.Configure<AppSetting>(Configuration.GetSection("AppSettings"));
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.Configure<PayOsSetting>(Configuration.GetSection("PayOsSetting"));

            var secretKey = Configuration["AppSettings:SecretKey"];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        //tự cấp token
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidIssuer = Configuration["AppSettings:Issuer"],

                        //ký vào token
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

                        ClockSkew = TimeSpan.Zero
                    };
                });

            // Book permission
            services.AddAuthorization(options =>
            {

                options.AddPolicy("CreateMovingContactAccess", policy =>
                                 policy.RequireClaim("Permissions", PolicyTerm.CREATE_MOVING_CONTACT));
                options.AddPolicy("UpdateMovingContactAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.UPDATE_MOVING_CONTACT));
                options.AddPolicy("DeleteMovingContactAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.DELETE_MOVING_CONTACT));

                options.AddPolicy("CreateRestHouseContactAccess", policy =>
                          policy.RequireClaim("Permissions", PolicyTerm.CREATE_RESTHOUSE_CONTACT));
                options.AddPolicy("UpdateRestHouseContactAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.UPDATE_RESTHOUSE_CONTACT));
                options.AddPolicy("DeleteRestHouseContactAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.DELETE_RESTHOUSE_CONTACT));

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

                options.AddPolicy("CreateTourishCategoryAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.CREATE_TOURISH_CATEGORY));
                options.AddPolicy("UpdateTourishCategoryAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.UPDATE_TOURISH_CATEGORY));
                options.AddPolicy("DeleteTourishCategoryAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.DELETE_TOURISH_CATEGORY));

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

                options.AddPolicy("CreateNotificationAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.CREATE_NOTIFICATION));
                options.AddPolicy("UpdateNotificationAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.UPDATE_NOTIFICATION));
                options.AddPolicy("DeleteNotificationAccess", policy =>
                                  policy.RequireClaim("Permissions", PolicyTerm.DELETE_NOTIFICATION));

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

            RecurringJob.AddOrUpdate<ScheduleDateChangeTask>("MyScheduledTask", x => x.ScheduleDateDueTask(), Cron.DayInterval(2));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationHub>("api/user/notify");

                endpoints.MapHub<UserMessageHub>("api/user/message");
                endpoints.MapHub<GuestMessageHub>("api/guest/message");

                endpoints.MapControllers();
            });
        }
    }
}