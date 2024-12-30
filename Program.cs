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

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();
if (builder.Environment.IsProduction())
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgressConn")));
}
else
{
    Console.WriteLine("--> Using PostgreSQL Server");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgressConn")));
    
    // builder.Services.AddDbContext<AppDbContext>(opt => 
    //     opt.UseInMemoryDatabase("InMem")); 
}
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers(); 
app.MapMetrics();
app.UseHttpsRedirection();
PrepDb.PrepPopulation(app);
app.Run();


