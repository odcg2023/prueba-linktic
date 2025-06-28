using AutoMapper;
using Microsoft.AspNetCore.Http;
using ProductosService.Application.Interfaces;
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
        protected readonly ICurrentUserService CurrentUserService;
        public ServiceBase(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            UnidadTrabajo = unitOfWork;
            Mapper = mapper;
            CurrentUserService = currentUserService;
        }
    }
}
