using AutoMapper;
using ABC.Leaves.Api.Models;

namespace ABC.Leaves.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LeavePostDto, LeaveApplyDto>();
            CreateMap<LeaveApplyDto, Leave>();
        }
    }
}
