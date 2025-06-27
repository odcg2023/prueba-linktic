using AutoMapper;
using ProductosService.Application.Dto;
using ProductosService.Application.Interfaces;
using ProductosService.Domain.Entity;
using ProductosService.Domain.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductosService.Application.Services
{
    public class ProductoService : ServiceBase, IProductoService
    {
        public ProductoService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            
        }

        public Task<int> CrearProducto(ProductoDto producto)
        {
            Validaciones(producto);
            var nuevoProducto = Mapper.Map<Producto>(producto);
            UnidadTrabajo.Crud<Producto>().Add(nuevoProducto);
            UnidadTrabajo.SaveChanges();
            return Task.FromResult(nuevoProducto.IdProducto);
        }

        public Task<ProductoDto> ObtenerProductoPorId(int idProducto)
        {
            var producto = UnidadTrabajo.Crud<Producto>().Find(x=> x.IdProducto == idProducto).FirstOrDefault();  
            return Task.FromResult(Mapper.Map<ProductoDto>(producto));
        }

        public Task<List<ProductoDto>> ObtenerProductos()
        {
            var productos = UnidadTrabajo.Crud<Producto>().GetAll().ToList();
            return Task.FromResult(Mapper.Map<List<ProductoDto>>(productos));
        }

        private void Validaciones(ProductoDto producto)
        {
            //Validar si existe un producto con el mismo nombre
            ValidarNombreProducto(producto);
        }

        private void ValidarNombreProducto(ProductoDto producto)
        {
            var productoExistente = UnidadTrabajo.Crud<Producto>()
                            .Find(x => x.NombreProducto.Trim().ToLower() ==
                            producto.NombreProducto.Trim().ToLower()).FirstOrDefault();

            if (productoExistente != null)
            {
                throw new ApplicationException($"Ya existe un producto con el nombre: {producto.NombreProducto}");
            }
        }
    }
}
