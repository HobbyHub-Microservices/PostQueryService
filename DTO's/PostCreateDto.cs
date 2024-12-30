using System.ComponentModel.DataAnnotations;

namespace PostQueryService.DTO_s;

public class PostCreateDto
{
    [Required, MaxLength(255)]
    public string Title { get; set; }
    [Required]
    public string Content { get; set; }
    public string UserName { get; set; }
    public string HobbyName { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public List<string> ImageUrls { get; set; } = new List<string>();
}