using AutoMapper;
using StartupWebAPIs.DTOs;
using StartupWebAPIs.Models;

namespace StartupWebAPIs.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Entity -> DTO
            CreateMap<Product, ProductDto>();

            // DTO -> Entity
            CreateMap<CreateProductDto, Product>();
            CreateMap<Product, ProductV2Dto>();
        }
    }
}