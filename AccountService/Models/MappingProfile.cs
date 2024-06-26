﻿using AutoMapper;
using SharingModels.ModelsVM;

namespace AccountService.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserVM>();
        }
    }
}
