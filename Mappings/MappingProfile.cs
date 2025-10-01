using AutoMapper;
using RandomUserProject.DTOs;
using RandomUserProject.Models;

namespace RandomUserProject.Mappings
{
    /// <summary>
    /// Autor: Samuel Sabino - 30/09/2025
    /// Descrição: class responsavel pelo mapeamento do AutoMapper.
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();

            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}
