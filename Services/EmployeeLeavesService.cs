using System;
using AutoMapper;
using ABC.Leaves.Api.Enums;
using ABC.Leaves.Api.Helpers;
using ABC.Leaves.Api.Models;
using ABC.Leaves.Api.Repositories;
using ABC.Leaves.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ABC.Leaves.Api.Services
{
    public class EmployeeLeavesService : IEmployeeLeavesService
    {
        private readonly IEmployeeLeavesRepository repository;
        private readonly IEmployeeRepository employeeRepository;
        private readonly IMapper mapper;

        public EmployeeLeavesService(IEmployeeLeavesRepository repository, IEmployeeRepository employeeRepository, IMapper mapper)
        {
            this.repository = repository;
            this.employeeRepository = employeeRepository;
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

        public IActionResult Approve(string id, string accessToken)
        {
            return ChangeStatus(id, EmployeeLeaveStatus.Approved, accessToken);
        }

        public IActionResult Decline(string id, string accessToken)
        {
            return ChangeStatus(id, EmployeeLeaveStatus.Declined, accessToken);
        }

        private IActionResult ChangeStatus(string id, EmployeeLeaveStatus status, string accessToken)
        {
            if (accessToken == null)
            {
                return new BadRequestObjectResult("Access token can not be null");
            }
            var gmailLogin = UserInfoHelper.GetUserGmailAddress(accessToken);
            var user = employeeRepository.Find(gmailLogin);
            if (user == null && !user.IsAdmin)
            {
                return new ForbidResult("Only administrator can change status of user request.");
            }

            if (id == null)
            {
                return new BadRequestObjectResult("ID can not be null");
            }
            var model = repository.Find(id);
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
