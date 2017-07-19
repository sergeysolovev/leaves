﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AbcLeaves.Api.Domain;
using AutoMapper;
using AbcLeaves.Api.Helpers;
using AbcLeaves.Core;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AbcLeaves.Api.Controllers
{
    [Route("api/leaves")]
    public class LeavesController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager userManager;
        private readonly LeavesManager leavesManager;
        private readonly ModelStateHelper modelHelper;

        public LeavesController(
            IMapper mapper,
            UserManager userManager,
            LeavesManager leavesManager,
            ModelStateHelper modelStateHelper)
        {
            this.userManager = userManager;
            this.leavesManager = leavesManager;
            this.modelHelper = modelStateHelper;
            this.mapper = mapper;
        }

        // GET api/leaves
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = await userManager.GetOrCreateUserAsync(HttpContext.User);
            if (user == null)
            {
                return BadRequest();
            }

            return Json(
                await leavesManager.GetByUserId(user.Id)
            );
        }

        // GET api/leaves/all
        [HttpGet("all")]
        [Authorize(Policy = "CanManageAllLeaves")]
        public async Task<IActionResult> GetAll()
        {
            return Json(
                await leavesManager.GetAll()
            );
        }

        // POST api/leaves
        [HttpPost]
        [Authorize(Policy = "CanApplyLeaves")]
        public async Task<IActionResult> Post([FromBody]PostLeaveContract leaveContract)
        {
            if (!ModelState.IsValid)
            {
                return ModelValidationResult
                    .Fail(modelHelper.GetValidationErrors(ModelState))
                    .ToMvcActionResult();
            }

            var user = await userManager.GetOrCreateUserAsync(HttpContext.User);
            if (user == null)
            {
                return BadRequest();
            }

            var applyLeaveContract = mapper.Map<PostLeaveContract, ApplyLeaveContract>(
                leaveContract, opts => opts.AfterMap((src, dst) => dst.UserId = user.Id)
            );

            return await leavesManager
                .ApplyAsync(applyLeaveContract)
                .ToMvcActionResultAsync();
        }

        // PATCH api/leaves/{id}/approve
        [HttpPatch("{id}/approve")]
        [Authorize(Policy = "CanApproveLeaves")]
        public async Task<IActionResult> Approve([FromRoute]int id)
        {
            return await leavesManager
                .ApproveAsync(id)
                .ToMvcActionResultAsync();
        }

        // PATCH api/leaves/{id}/decline
        [HttpPatch("{id}/decline")]
        [Authorize(Policy = "CanDeclineLeaves")]
        public async Task<IActionResult> Decline([FromRoute]int id)
        {
            return await leavesManager
                .DeclineAsync(id)
                .ToMvcActionResultAsync();
        }
    }
}
