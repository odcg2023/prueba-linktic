using AutoMapper;
using Microsoft.AspNetCore.Http;
using ComprasService.Application.Interfaces;
using ComprasService.Domain.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComprasService.Application.Services
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
