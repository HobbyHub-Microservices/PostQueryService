using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PostQueryService.Data;
using PostQueryService.DTO_s;
using PostQueryService.Models;

namespace PostQueryService.Controllers;


[Route("api/query/[controller]")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly IPostRepo _repo;
    private readonly IMapper _mapper;

    public PostController(
        IPostRepo repo,
        IMapper mapper
        )
    {
        _repo = repo;
        _mapper = mapper;
    }
    
    [HttpGet]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<ActionResult<IEnumerable<PostViewDto>>> GetAllPosts()
    {
        Console.WriteLine("--> Getting posts from DB...");

        // Await the asynchronous method
        var postItems = await _repo.GetAllPostsAsync();

        // Map the result to PostViewDto
        return Ok(_mapper.Map<IEnumerable<PostViewDto>>(postItems));
    }
    
    [HttpGet("{id}", Name = "GetPostById")]
    public async Task<ActionResult<PostViewDto>> GetPostById(int id)
    {
        var postItem = await _repo.GetPostByIdAsync(id); // Await the async method

        if (postItem != null)
        {
            return Ok(_mapper.Map<PostViewDto>(postItem));
        }

        return NotFound(); // Return 404 if no post is found
    }
    
    [HttpPost]
    public async Task<ActionResult<PostViewDto>> CreatePost(PostCreateDto postCreateDto)
    {
        var post = _mapper.Map<ViewPost>(postCreateDto);
        await _repo.CreatePost(post);
        _repo.SaveChanges();
        var postViewDto = _mapper.Map<PostViewDto>(post);
        return CreatedAtRoute(nameof(GetPostById), new { Id = postViewDto.PostId }, postViewDto);
    }
}