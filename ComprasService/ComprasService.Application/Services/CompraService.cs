using AutoMapper;
using ComprasService.Application.Dto;
using ComprasService.Application.Interfaces;
using ComprasService.Domain.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComprasService.Application.Services
{
    public class CompraService : ServiceBase, IComprasService
    {
        public CompraService(IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ICurrentUserService currentUserService) 
            : base(unitOfWork, mapper, currentUserService)
        {
        }

        public Task<CompraDto> RegistrarCompra(RegistraCompraDto compra)
        {
            throw new NotImplementedException();
        }
    }
}
