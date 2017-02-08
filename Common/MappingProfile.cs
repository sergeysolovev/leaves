using AutoMapper;
using ABC.Leaves.Api.Models;
using ABC.Leaves.Api.Dto;

namespace ABC.Leaves.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EmployeeLeaveDto, EmployeeLeave>();
            CreateMap<EmployeeLeave, EmployeeLeaveDto>();
        }
    }
}
