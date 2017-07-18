using System.Threading.Tasks;
using AbcLeaves.Api.Models;
using AbcLeaves.Api.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AbcLeaves.Api.Domain
{
    public class LeavesManager
    {
        private readonly IMapper mapper;
        private readonly LeavesRepository leavesRepository;
        private readonly GoogleCalendarManager googleCalendarManager;

        public LeavesManager(
            IMapper mapper,
            LeavesRepository leavesRepository,
            GoogleCalendarManager googleCalendarManager)
        {
            this.mapper = mapper;
            this.leavesRepository = leavesRepository;
            this.googleCalendarManager = googleCalendarManager;
        }

        public async Task<LeaveResult> ApplyAsync(ApplyLeaveContract leaveContract)
        {
            var leave = mapper.Map<ApplyLeaveContract, Leave>(leaveContract);
            await leavesRepository.InsertAsync(leave);
            return LeaveResult.Succeed(leave);
        }

        public async Task<LeaveResult> ApproveAsync(int leaveId)
        {
            var leave = leavesRepository.GetById(leaveId);
            if (leave == null)
            {
                return LeaveResult.FailNotFound(leaveId);
            }
            if (leave.Status == LeaveStatus.Approved)
            {
                return LeaveResult.Fail(
                    $"Leave id={leaveId} has already been approved"
                );
            }
            try
            {
                leave.Status = LeaveStatus.Approved;
                await leavesRepository.UpdateAsync(leave);
            }
            catch (DbUpdateConcurrencyException)
            {
                return LeaveResult.Fail(
                    $"Leave id={leaveId} is being updated by another user"
                );
            }

            var publishContract = mapper.Map<Leave, PublishUserEventContract>(leave);
            var shareResult = await googleCalendarManager.PublishUserEventAsync(publishContract);
            return LeaveResult.Succeed();
        }

        public async Task<LeaveResult> DeclineAsync(int leaveId)
        {
            var leave = leavesRepository.GetById(leaveId);
            if (leave == null)
            {
                return LeaveResult.FailNotFound(leaveId);
            }
            if (leave.Status == LeaveStatus.Approved)
            {
                return LeaveResult.Fail(
                    $"Cannot decline leave id={leaveId} " +
                    "that has already been approved"
                );
            }
            if (leave.Status == LeaveStatus.Declined)
            {
                return LeaveResult.Fail(
                    $"Leave id={leaveId} has already been declined"
                );
            }
            try
            {
                leave.Status = LeaveStatus.Declined;
                await leavesRepository.UpdateAsync(leave);
            }
            catch (DbUpdateConcurrencyException)
            {
                return LeaveResult.Fail(
                    $"Leave id={leaveId} is being updated by another user"
                );
            }
            return LeaveResult.Succeed();
        }
    }
}
