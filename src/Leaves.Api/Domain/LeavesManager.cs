using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leaves.Api.Models;
using Leaves.Api.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Leaves.Api.Domain
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

        public async Task<GetLeavesContract> GetByUserId(string userId)
        {
            return new GetLeavesContract {
                Items = await leavesRepository
                    .GetByUserId(userId)
                    .Select(leave => mapper.Map<Leave, GetLeavesItemContract>(leave))
                    .ToList()
            };
        }

        public async Task<GetLeavesContract> GetAll()
        {
            return new GetLeavesContract {
                Items = await leavesRepository
                    .GetAll()
                    .Select(leave => mapper.Map<Leave, GetLeavesItemContract>(leave))
                    .ToList()
            };
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
                return LeaveResult.ReturnNotFound();
            }
            if (leave.Status == LeaveStatus.Approved)
            {
                return LeaveResult.Fail(
                    $"Leave id={leaveId} has already been approved"
                );
            }
            try
            {
                var publishContract = mapper.Map<Leave, PublishUserEventContract>(leave);
                var eventUrl = await googleCalendarManager.PublishUserEventAsync(
                    publishContract
                );

                leave.Status = LeaveStatus.Approved;
                leave.GoogleCalendarLink = eventUrl;

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

        public async Task<LeaveResult> DeclineAsync(int leaveId)
        {
            var leave = leavesRepository.GetById(leaveId);
            if (leave == null)
            {
                return LeaveResult.ReturnNotFound();
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
