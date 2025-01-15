﻿using MediatR;
using MedVisit.AccessManagement.Mediatr.User.Handlers;
using MedVisit.AccessManagement.Mediatr.User.Queries;
using MedVisit.AccessManagement.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedVisit.AccessManagement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("account")]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получить профиль пользователя
        /// </summary>
        /// <response code="200">Успех</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpGet("profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var profile = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return profile == null ? NotFound("Profile not found") : Ok(await _mediator.Send(new GetUserByIdQuery(profile)));
        }

        /// <summary>
        /// Получить профиль пользователя
        /// </summary>
        /// <response code="200">Успех</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpPut("profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateProfile([FromBody] UserDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim);
            return Ok(await _mediator.Send(new UpdateUserCommand(userId, request)));
        }
    }
}
