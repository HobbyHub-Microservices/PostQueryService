using System.ComponentModel.DataAnnotations;

namespace PostQueryService.Models;

public class ViewPost
{
    [Key]
    [Required]
    public int PostId { get; set; }
    
    [Required]
    public string Title { get; set; }
    
    [Required]
    public string Content { get; set; }
    
    [Required]
    public string HobbyName { get; set; }
    
    [Required]
    public string UserName { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; } 
    
    public List<string> ImageUrls { get; set; }
}