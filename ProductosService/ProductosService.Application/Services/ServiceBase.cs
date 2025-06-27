using AutoMapper;
using Microsoft.AspNetCore.Http;
using ProductosService.Domain.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductosService.Application.Services
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
