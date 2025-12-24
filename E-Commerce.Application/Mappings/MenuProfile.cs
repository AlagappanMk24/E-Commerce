using AutoMapper;
using E_Commerce.Application.Contracts.DTOs;
using E_Commerce.Domain.Entities;

namespace E_Commerce.Application.Mappings
{
    public class MenuProfile : Profile
    {
        public MenuProfile()
        {
            CreateMap<MenuItem, MenuItemDto>()
                .ForMember(d => d.Children, opt => opt.Ignore())
                .ForMember(d => d.Title,
                    opt => opt.MapFrom(src => src.Title ?? "Untitled"))
                .ForMember(d => d.Url,
                    opt => opt.MapFrom(src => src.Url ?? "#"));
        }
    }
}
