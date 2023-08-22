using MyWebApiApp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();
builder.Services.AddLogging();

builder.Services.AddSwaggerGen();

//builder.Services.AddCors((options) =>
//{
//    options.AddPolicy("FeedClientApp",
//        new CorsPolicyBuilder()
//        .WithOrigins("http://localhost:4200")
//        .AllowAnyHeader()
//        .AllowAnyMethod()
//        .AllowCredentials()
//        .Build());
//});

// other code and build the app



var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services); // calling ConfigureServices method
var app = builder.Build();

app.UseCors("AllowAngularOrigins");

startup.Configure(app, builder.Environment); // calling Configure method

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
