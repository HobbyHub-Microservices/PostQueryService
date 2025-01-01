using PostQueryService.Models;

namespace PostQueryService.Data;

public interface IPostRepo
{
    bool SaveChanges();  // Saves changes to the database (for Create, Update, Delete operations)

    // Read side (Query)
    Task<IEnumerable<ViewPost>> GetAllPostsAsync();  // Retrieves all posts (query operation)

    Task<ViewPost> GetPostByIdAsync(int postId);  

    Task<IEnumerable<ViewPost>> GetPostByUserName(string userName);
    
    Task DeletedHobby(string hobbyName);
    
    Task UpdateHobbyName(string oldName, string newName);
    
    Task DeletedUserPosts(string userName);
    
    Task UpdateUserName(string oldName, string newName);
    
    Task CreatePost(ViewPost post);
}