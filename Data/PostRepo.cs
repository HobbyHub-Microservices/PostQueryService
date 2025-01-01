using Microsoft.EntityFrameworkCore;
using PostQueryService.Models;

namespace PostQueryService.Data;

public class PostRepo : IPostRepo
{
    private readonly AppDbContext _context;

    public PostRepo(AppDbContext context)
    {
        _context = context;
    }
    public bool SaveChanges()
    {
        return (_context.SaveChanges() >= 0);
    }

    public async Task<IEnumerable<ViewPost>> GetAllPostsAsync()
    {
        return await _context.ViewPosts.ToListAsync();
    }

    public async Task<ViewPost> GetPostByIdAsync(int postId)
    {
        return await _context.ViewPosts.FirstOrDefaultAsync(p => p.PostId == postId);
    }

    public async Task<IEnumerable<ViewPost>> GetPostByUserName(string userName)
    {
        return await _context.ViewPosts.Where(p => p.UserName == userName).ToListAsync();
    }

    public async Task DeletedHobby(string hobbyName)
    {
        var postsToDelete = _context.ViewPosts.Where(p => p.HobbyName == hobbyName);

        if (postsToDelete.Any())
        {
            _context.ViewPosts.RemoveRange(postsToDelete);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateHobbyName(string oldName, string newName)
    {
        if (string.IsNullOrEmpty(oldName))
        {
            throw new ArgumentException("Old hobby name cannot be null or empty", nameof(oldName));
        }

        if (string.IsNullOrEmpty(newName))
        {
            throw new ArgumentException("New hobby name cannot be null or empty", nameof(newName));
        }

        try
        {

            var postsToUpdate = _context.ViewPosts.Where(p => p.HobbyName == oldName);

            if (postsToUpdate.Any())
            {
                await postsToUpdate.ForEachAsync(p => p.HobbyName = newName);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating the hobby name", ex);
        }
    }

    public async Task DeletedUserPosts(string userName)
    {
        if (string.IsNullOrEmpty(userName))
        {
            throw new ArgumentException("User name cannot be null or empty", nameof(userName));
        }
        
        try
        {
            var postsToUpdate = _context.ViewPosts.Where(p => p.UserName == userName);

            if (postsToUpdate.Any())
            {
                // await postsToUpdate.ForEachAsync(p => p.UserName = "deleted_user");
                _context.ViewPosts.RemoveRange(postsToUpdate);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating user posts", ex);
        }
    }

    public async Task UpdateUserName(string oldName, string newName)
    {
        throw new NotImplementedException();
    }

    public async Task CreatePost(ViewPost post)
    {
        if (post == null)
        {
            throw new ArgumentNullException(nameof(post));
        }
        
        await  _context.ViewPosts.AddAsync(post);
        
        // await _messageBus.PublishAsync(new PostCreatedEvent(post));
        
        await _context.SaveChangesAsync();
    }
}