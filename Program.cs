using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using PostQueryService.AsyncDataServices;
using PostQueryService.Data;
using PostQueryService.EventProcessor;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddHostedService<MessagebusSubscriber>();
builder.Services.AddScoped<IPostRepo, PostRepo>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();
if (builder.Environment.IsProduction())
{
    var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER");
    var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST");
    var dbPort = Environment.GetEnvironmentVariable("POSTGRES_PORT");
    var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

    if (string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbHost) || string.IsNullOrEmpty(dbPort) ||
        string.IsNullOrEmpty(dbPassword))
    {
        
        Console.WriteLine("One of the string values for Postgres are empty");
        Console.WriteLine($"Host={dbHost};Port={dbPort};Database=Users;Username={dbUser};Password={dbPassword};Trust Server Certificate=true;");
        
    }
    
    builder.Configuration["ConnectionStrings:PostgressConn"] = $"Host={dbHost};Port={dbPort};Database=Posts;Username={dbUser};Password={dbPassword};Trust Server Certificate=true;";
}
else
{
    Console.WriteLine("---> Using InMemory database");
    builder.Services.AddDbContext<AppDbContext>(opt => 
        opt.UseInMemoryDatabase("InMem")); 
    
    // builder.Services.AddDbContext<AppDbContext>(opt => 
    //     opt.UseInMemoryDatabase("InMem")); 
}
var integrationMode = builder.Configuration.GetValue<bool>("IntegrationMode");
if (!integrationMode)
{
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        Console.WriteLine("---> Using Keycloak stuff");
        Console.WriteLine(builder.Configuration["Keycloak:Authority"]);
        Console.WriteLine(builder.Configuration["Keycloak:Audience"]);
    
        options.Authority = builder.Configuration["Keycloak:Authority"]; // Keycloak realm URL
        options.Audience = builder.Configuration["Keycloak:Audience"];   // Client ID
        options.RequireHttpsMetadata = false;            // Disable for development
    
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Keycloak:Authority"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Keycloak:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        }; 
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated successfully");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine($"Token challenge triggered: {context.Error}, {context.ErrorDescription}");
                return Task.CompletedTask;
            }
        };
    
    }); 
}
builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();
app.MapControllers(); 
app.MapMetrics();
app.UseHttpsRedirection();
PrepDb.PrepPopulation(app);
app.Run();


