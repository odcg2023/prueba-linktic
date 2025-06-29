using AutoMapper;
using InventariosService.Application.Dto;
using InventariosService.Domain.Entity;

namespace InventariosService.Transversal.Mapping
{
    public class Mapper : Profile
    {
        public Mapper()
        {
          CreateMap<Inventario, InventarioProductoDto>().ReverseMap();
        }
    }
}
