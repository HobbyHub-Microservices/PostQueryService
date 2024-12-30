using Microsoft.EntityFrameworkCore;

namespace PostQueryService.Data;

public class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            // var grpcClient = serviceScope.ServiceProvider.GetService<IUserDataClient>();
            // var users = grpcClient.ReturnAllUsers();
                  
            SeedData(serviceScope.ServiceProvider.GetRequiredService<AppDbContext>());
        }

    }
    
    private static void SeedData(AppDbContext context)
    {
        try
        {
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not run migrations: {ex.Message}");
        }
    }
}