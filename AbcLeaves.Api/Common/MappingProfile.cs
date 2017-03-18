using AutoMapper;
using AbcLeaves.Api.Models;
using AbcLeaves.Api.Services;
using System;

namespace AbcLeaves.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LeavePostDto, LeaveApplyDto>();
            CreateMap<LeaveApplyDto, Leave>();
            CreateMap<UserEventPublishDto, CalendarEventAddDto>();
            CreateMap<Leave, UserEventPublishDto>().AfterMap((src, dst) => {
                dst.Title = LeaveEventDefaults.Title;
                dst.Description = LeaveEventDefaults.Description;
            });

            CreateMap<CalendarEventAddDto, CalendarEvent>().AfterMap((src, dst) =>
                dst.Summary = src.Title
            );

            CreateMap<DateTime, CalendarEventDateTime>()
                .ConstructUsing(dateTime => new CalendarEventDateTime(dateTime));

            //CreateMap<CalendarEventAddDto, CalendarEvent>();
        }
    }
}
