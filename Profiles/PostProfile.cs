using AutoMapper;
using PostQueryService.DTO_s;
using PostQueryService.Models;

namespace PostQueryService.Profiles;

public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<PostCreateDto, ViewPost>();
        CreateMap<ViewPost, PostViewDto>();
    }
    
}