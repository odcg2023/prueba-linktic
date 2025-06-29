using AutoMapper;
using Microsoft.AspNetCore.Http;
using InventariosService.Application.Interfaces;
using InventariosService.Domain.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventariosService.Application.Services
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
