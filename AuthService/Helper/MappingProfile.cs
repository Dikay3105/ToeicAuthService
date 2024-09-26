using AuthService.Dto;
using AuthService.Models;
using AutoMapper;

namespace AuthService.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
        }
    }
}
