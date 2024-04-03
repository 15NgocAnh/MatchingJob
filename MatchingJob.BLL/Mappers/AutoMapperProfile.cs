using AutoMapper;
using MatchingJob.Entities;
using MatchingJob.DAL.DTOs.User;

namespace MatchingJob.BLL.Mappers
{
    public class AutoMapperProfile : Profile
    {
        private readonly IPasswordHasher _passwordHasher;

        public AutoMapperProfile(IPasswordHasher passwordHasher) 
        {
            _passwordHasher = passwordHasher;

            CreateMap<UsersDTO, User>().ForMember(dest => dest.Password, opt => opt.MapFrom(src => _passwordHasher.Hash(src.Password))).ReverseMap();
            CreateMap<LoginModel, User>().ForMember(dest => dest.Password, opt => opt.MapFrom(src => _passwordHasher.Hash(src.Password)));
        }
    }
}
