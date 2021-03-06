﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyCodeCamp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCodeCamp.Models
{
    public class CampMappingProfile : Profile
    {
        public CampMappingProfile()
        {
            CreateMap<Camp, CampModel>()
                .ForMember(c => c.StartDate,
                    opt => opt.MapFrom(camp => camp.EventDate))
                .ForMember(c => c.EndDate,
                    opt => opt.ResolveUsing(camp => camp.EventDate.AddDays(camp.Length - 1)))
                .ForMember(c => c.Url,
                    opt => opt.ResolveUsing<CampUlrResolver>())
                .ReverseMap()
                .ForMember(m => m.EventDate,
                    opt => opt.MapFrom(model => model.StartDate))
                .ForMember(m => m.Length,
                    opt => opt.ResolveUsing(model => (model.EndDate - model.StartDate).Days + 1))
                .ForMember(m => m.Location,
                    opt => opt.ResolveUsing(c => new Location()
                    {
                        Address1 = c.LocaltionAddress1,
                        Address2 = c.LocaltionAddress2,
                        Address3 = c.LocaltionAddress3,
                        CityTown = c.LocaltionCityTown,
                        StateProvince = c.LocaltionStateProvince,
                        PostalCode = c.LocaltionPostalCode,
                    }));

            CreateMap<Speaker, SpeakerModel>()
                .ForMember(s => s.Url,
                    opt => opt.ResolveUsing<SpeakerUrlResolver>())
                .ReverseMap();

            CreateMap<Speaker, Speaker2Model>()
                .IncludeBase<Speaker, SpeakerModel>()
                .ForMember(s => s.BadgeName, opt => opt.ResolveUsing(s => $"{s.Name} (@{s.TwitterName})"));
   

            CreateMap<Talk, TalkModel>()
                .ForMember(s => s.Url,
                    opt => opt.ResolveUsing<TalkUrlResolver>())
                .ForMember(s=>s.Links, opt=>opt.ResolveUsing<TalkLinksResolver>())
                .ReverseMap();
        }
    }
}
