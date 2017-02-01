using System;
using AutoMapper;
using ABC.Leaves.Api.Enums;
using ABC.Leaves.Api.Models;
using ABC.Leaves.Api.Repositories;
using ABC.Leaves.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ABC.Leaves.Api.Services
{
    public class EmployeeLeavesService : IEmployeeLeavesService
    {
        private readonly IEmployeeLeavesRepository repository;
        private readonly IMapper mapper;

        public EmployeeLeavesService(IEmployeeLeavesRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public IActionResult Apply(EmployeeLeaveViewModel viewModel)
        {
            if (viewModel == null)
            {
                return new BadRequestResult();
            }
            var model = mapper.Map<EmployeeLeaveViewModel, EmployeeLeave>(viewModel);
            model.Id = Guid.NewGuid().ToString();
            model.Status = EmployeeLeaveStatus.Applied;
            repository.Insert(model);
            return new OkResult();
        }

        public IActionResult Approve(string id)
        {
            return ChangeStatus(id, EmployeeLeaveStatus.Approved);
        }

        public IActionResult Decline(string id)
        {
            return ChangeStatus(id, EmployeeLeaveStatus.Declined);
        }

        private IActionResult ChangeStatus(string id, EmployeeLeaveStatus status)
        {
            if (id == null)
            {
                return new BadRequestObjectResult("ID can not be null");
            }
            var model = repository.Get(id);
            if (model == null)
            {
                return new NotFoundResult();
            }
            model.Status = status;
            repository.Update(model);
            return new OkResult();
        }
    }
}
