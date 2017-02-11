using AutoMapper;
using ABC.Leaves.Api.Enums;
using ABC.Leaves.Api.Models;
using ABC.Leaves.Api.Repositories;
using ABC.Leaves.Api.Leaves.Dto;
using ABC.Leaves.Api.GoogleAuth;
using ABC.Leaves.Api.Services.Dto;
using System.Threading.Tasks;
using System;

namespace ABC.Leaves.Api.Leaves
{
    public class EmployeeLeavesService : IEmployeeLeavesService
    {
        private readonly IGoogleAuthService googleAuthService;
        private readonly IGoogleCalendarService googleCalendarService;
        private readonly IEmployeeLeavesRepository leavesRepository;
        private readonly IMapper mapper;

        public EmployeeLeavesService(
            IGoogleAuthService googleAuthService,
            IGoogleCalendarService googleCalendarService,
            IEmployeeLeavesRepository leavesRepository,
            IMapper mapper)
        {
            this.googleAuthService = googleAuthService;
            this.googleCalendarService = googleCalendarService;
            this.leavesRepository = leavesRepository;
            this.mapper = mapper;
        }

        public async Task<ApplyLeaveResult> ApplyAsync(EmployeeLeaveDto leaveDto)
        {
            if (leaveDto.Start.Kind != DateTimeKind.Utc || leaveDto.End.Kind != DateTimeKind.Utc)
            {
                return new ApplyLeaveResult
                {
                    Error = new ErrorDto("Start and End datetimes have to be UTC datetimes")
                };
            }
            var accessToken = leaveDto.GoogleAuthAccessToken;
            var tokenValidation = await googleAuthService.ValidateAccessTokenAsync(accessToken);
            if (!tokenValidation.IsValid)
            {
                return new ApplyLeaveResult { Error = tokenValidation.Error };
            }
            var leave = mapper.Map<EmployeeLeaveDto, EmployeeLeave>(leaveDto);
            await leavesRepository.InsertAsync(leave);
            return new ApplyLeaveResult { IsApplied = true, LeaveId = leave.Id };
        }

        public async Task<ApproveLeaveResult> ApproveAsync(int id)
        {
            var leave = leavesRepository.GetById(id);
            if (leave == null)
            {
                return new ApproveLeaveResult
                {
                    LeaveNotFound = true,
                    Error = new ErrorDto($"Employee leave id={id} is not found")
                };
            }
            if (leave.Status == EmployeeLeaveStatus.Approved)
            {
                return new ApproveLeaveResult
                {
                    Error = new ErrorDto($"Employee leave id={id} has already been approved")
                };
            }
            var accessToken = leave.GoogleAuthAccessToken;
            var tokenValidation = await googleAuthService.ValidateAccessTokenAsync(accessToken);
            if (!tokenValidation.IsValid)
            {
                return new ApproveLeaveResult
                {
                    Error = new ErrorDto($"Access token for leave id={id} is not valid")
                };
            }
            var addEventResult = await googleCalendarService.AddEventAsync(accessToken, leave.Start, leave.End);
            if (addEventResult.EventAdded)
            {
                leave.Status = EmployeeLeaveStatus.Approved;
                await leavesRepository.UpdateAsync(leave);
            }
            return new ApproveLeaveResult
            {
                Error = addEventResult.Error,
                GoogleCalendarEventAdded = addEventResult.EventAdded,
                GoogleCalendarEventUri = addEventResult.EventUri
            };
        }

        public async Task<DeclineLeaveResult> DeclineAsync(int id)
        {
            var leave = leavesRepository.GetById(id);
            if (leave == null)
            {
                return new DeclineLeaveResult
                {
                    LeaveNotFound = true,
                    Error = new ErrorDto($"Employee leave id={id} is not found")
                };
            }
            if (leave.Status == EmployeeLeaveStatus.Approved)
            {
                return new DeclineLeaveResult
                {
                    Error = new ErrorDto($"Employee leave id={id} has already been approved")
                };
            }
            leave.Status = EmployeeLeaveStatus.Declined;
            await leavesRepository.UpdateAsync(leave);
            return new DeclineLeaveResult { Declined = true };
        }
    }
}
