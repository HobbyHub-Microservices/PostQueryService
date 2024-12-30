using System.Text.Json;
using AutoMapper;
using PostQueryService.Data;
using PostQueryService.DTO_s;
using PostQueryService.Models;

namespace PostQueryService.EventProcessor;

public class EventProcessor : IEventProcessor
{
     private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _mapper = mapper;
    }
    public void ProcessEvent(string message)
    {
        var eventType = DetermineEventType(message);
        switch (eventType)
        {
            case EventType.HobbyEdited:
                Console.WriteLine(message);
                EditHobby(message);
                break;
            case EventType.Undetermined:
                Console.WriteLine(message);
                break;
            case EventType.HobbyDeleted:
                Console.WriteLine(message);
                DeleteHobby(message);
                break;
       
        }
    }
    
    private EventType DetermineEventType(string notificationMessage)
    {
        Console.WriteLine("--> DetermineEventType");
     
        try
        {
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
        
            if (eventType?.Event == "Hobby_Edited")
            {
                Console.WriteLine("--> Hobby_Edited");
                return EventType.HobbyEdited;
            }
            
            if (eventType?.Event == "Hobby_Deleted")
            {
                Console.WriteLine("--> Hobby_Deleted");
                return EventType.HobbyDeleted; 
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not determine event type: {ex.Message}");
        }

        Console.WriteLine("--> Unknown event type detected");
        return EventType.Undetermined;
    }

    private async Task EditHobby(string hobbyPublishedMessage)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IPostRepo>();
            var hobbyPublishedEventDto = JsonSerializer.Deserialize<HobbyEditPublishedDto>(hobbyPublishedMessage);

            if (hobbyPublishedEventDto == null || string.IsNullOrEmpty(hobbyPublishedEventDto.Name) || string.IsNullOrEmpty(hobbyPublishedEventDto.Name_old))
            {
                Console.WriteLine("Invalid data in the published event.");
                return;
            }
            
            try
            {
                // Perform the update using the repository method
                await repo.UpdateHobbyName(hobbyPublishedEventDto.Name_old, hobbyPublishedEventDto.Name);
                Console.WriteLine($"Successfully updated hobby names");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not update user names in the database: {ex.Message}");
            }
        }
    }
    
    private async Task DeleteHobby(string hobbyPublishedMessage)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IPostRepo>();
            var hobbyPublishedEventDto = JsonSerializer.Deserialize<HobbyDeletePublishedDto>(hobbyPublishedMessage);
    
            try
            {
                // Perform the update using the repository method
                    await repo.DeletedHobby(hobbyPublishedEventDto.Name);
                    Console.WriteLine($"Successfully deleted hobbies");
     
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not add User to DB {ex.Message}");
            }
        }
    }

}
enum EventType
{
    HobbyEdited,
    HobbyDeleted,
    Undetermined
}
