using AutoMapper;
using ABC.Leaves.Api.Models;
using ABC.Leaves.Api.ViewModels;

namespace ABC.Leaves.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EmployeeLeaveViewModel, EmployeeLeave>();
            CreateMap<EmployeeLeave, EmployeeLeaveViewModel>();
        }
    }
}
