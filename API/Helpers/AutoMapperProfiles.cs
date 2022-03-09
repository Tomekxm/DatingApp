using System;
using System.Linq;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{

    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
            .ForMember(dest => dest.photoUrl, opt => opt.MapFrom(src =>
           src.Photos.FirstOrDefault(x => x.isMain).url))
            .ForMember(dest => dest.age, opt => opt.MapFrom(src => src.dateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<RegisterDto, AppUser>();
            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.senderPhotoUrl, opt => opt.MapFrom(src => 
                src.sender.Photos.FirstOrDefault(x => x.isMain).url))
                .ForMember(dest => dest.recipientPhotoUrl, opt => opt.MapFrom(src => 
                src.recipient.Photos.FirstOrDefault(x => x.isMain).url));
            CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        }
    }
}