using System;
using System.Threading.Tasks;
using ABC.Leaves.Api.Services;
using ABC.Leaves.Api.Models;
using ABC.Leaves.Api.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ABC.Leaves.Api.Domain
{
    public class LeavesManager : ILeavesManager
    {
        private readonly ILeavesRepository leavesRepository;
        private readonly IMapper mapper;
        private readonly IUserManager userManager;
        private readonly IGoogleOAuthService googleAuthService;
        private readonly IGoogleCalendarService googleCalendarService;

        public LeavesManager(
            ILeavesRepository leavesRepository,
            IGoogleOAuthService googleAuthService,
            IGoogleCalendarService googleCalendarService,
            IUserManager userManager,
            IMapper mapper)
        {
            this.leavesRepository = leavesRepository;
            this.googleAuthService = googleAuthService;
            this.googleCalendarService = googleCalendarService;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task<LeaveApplyResult> ApplyAsync(LeaveApplyDto leaveDto)
        {
            var leave = mapper.Map<LeaveApplyDto, Leave>(leaveDto);
            await leavesRepository.InsertAsync(leave);
            return LeaveApplyResult.Success(leave);
        }

        public async Task<LeaveApproveResult> ApproveAsync(int leaveId)
        {
            var leave = leavesRepository.GetById(leaveId);
            if (leave == null)
            {
                return LeaveApproveResult.FailNotFound(leaveId);
            }
            if (leave.Status == LeaveStatus.Approved)
            {
                return LeaveApproveResult.Fail($"Leave id={leaveId} has already been approved");
            }
            try
            {
                leave.Status = LeaveStatus.Approved;
                await leavesRepository.UpdateAsync(leave);
            }
            catch (DbUpdateConcurrencyException)
            {
                return LeaveApproveResult.Fail($"Leave id={leaveId} is being updated by another user");
            }

            var shareResult = await ShareInGoogleCalendar(leave);
            return LeaveApproveResult.Success(shareResult);
        }

        public async Task<LeaveDeclineResult> DeclineAsync(int leaveId)
        {
            var leave = leavesRepository.GetById(leaveId);
            if (leave == null)
            {
                return LeaveDeclineResult.FailNotFound(leaveId);
            }
            if (leave.Status == LeaveStatus.Approved)
            {
                return LeaveDeclineResult.Fail(
                    $"Cannot decline leave id={leaveId} " +
                    "that has already been approved"
                );
            }
            if (leave.Status == LeaveStatus.Declined)
            {
                return LeaveDeclineResult.Fail($"Leave id={leaveId} has already been declined");
            }
            try
            {
                leave.Status = LeaveStatus.Declined;
                await leavesRepository.UpdateAsync(leave);
            }
            catch (DbUpdateConcurrencyException)
            {
                return LeaveDeclineResult.Fail($"Leave id={leaveId} is being updated by another user");
            }
            return LeaveDeclineResult.Success();
        }

        private async Task<OperationResult> ShareInGoogleCalendar(Leave leave)
        {
            if (leave == null)
            {
                throw new ArgumentNullException(nameof(leave));
            }

            var userResult = GetLeaveApplicant(leave.Id);
            if (!userResult.Succeeded)
            {
                return OperationResult.FailFrom(userResult);
            }
            var user = userResult.User;

            var tokenResult = await userManager.GetUserRefreshToken(user);
            if (!tokenResult.Succeeded)
            {
                return OperationResult.FailFrom(tokenResult);
            }
            var refreshToken = tokenResult.GetValue<string>();

            var exchangeResult = await googleAuthService.ExchangeRefreshToken(refreshToken);
            if (!exchangeResult.Succeeded)
            {
                return OperationResult.FailFrom(exchangeResult);
            }
            var accessToken = exchangeResult.AccessToken;
            if (String.IsNullOrEmpty(accessToken))
            {
                return OperationResult.Fail("Failed to refresh user's access token");
            }

            var eventResult = await googleCalendarService.AddEventAsync(
                accessToken, leave.Start, leave.End);
            if (!eventResult.Succeeded)
            {
                return OperationResult.FailFrom(eventResult);
            }
            var eventUri = eventResult.EventUri;
            return OperationResult.Success(eventUri);
        }

        private AppUserResult GetLeaveApplicant(int leaveId)
        {
            var leave = leavesRepository.GetById(leaveId);
            if (leave == null)
            {
                return AppUserResult.Fail($"Leave id={leaveId} is not found");
            }
            if (leave.User == null)
            {
                return AppUserResult.Fail(
                    $"Leave id={leaveId} applicant is undefined"
                );
            }
            return AppUserResult.Success(leave.User);
        }
    }
}
