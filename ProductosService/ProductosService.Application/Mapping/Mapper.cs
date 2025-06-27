using AutoMapper;
using ProductosService.Application.Dto;
using ProductosService.Domain.Entity;

namespace ProductosService.Transversal.Mapping
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Producto, ProductoDto>().ReverseMap();
            CreateMap<Producto, ProductoNuevoDto>().ReverseMap();
        }
    }
}
