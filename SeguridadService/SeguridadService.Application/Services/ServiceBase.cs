using AutoMapper;
using SeguridadService.Domain.Interfaces.Repository;

namespace SeguridadService.Application.Services
{
    public class ServiceBase
    {
        protected readonly IUnitOfWork UnidadTrabajo;
        protected readonly IMapper Mapper;
        public ServiceBase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnidadTrabajo = unitOfWork;
            Mapper = mapper;
        }
    }
}
