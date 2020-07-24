using AutoMapper;
using PeopleS.API.Dtos;
using PeopleS.API.Models;

namespace PeopleS.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserDetailedDto>().ReverseMap();
            CreateMap<UserMainInfoDto, User>().ReverseMap();
            CreateMap<User, UserDateDto>().ReverseMap();
            CreateMap<User, UserForPostDto>();
            CreateMap<Post, PostDto>();
        }
    }
}