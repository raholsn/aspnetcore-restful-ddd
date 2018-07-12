using System;
using System.Collections.Generic;

using API.Contracts;
using API.Contracts.Dtos;

using AutoMapper;

using Domain;

using Shared;

namespace Application.Helpers
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Computer, ComputerDto>();
            CreateMap<GraphicsCard, GraphicsCardDto>();
            CreateMap<Ram, RamDto>();
            CreateMap<CreateRam, Ram>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));
            CreateMap<CreateGraphicsCard, GraphicsCard>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));
            CreateMap<UpdateComputer, Computer>();
            CreateMap<UpdateGraphicsCard, GraphicsCard>();
            CreateMap<UpdateRam, Ram>();
        }
    }
}
