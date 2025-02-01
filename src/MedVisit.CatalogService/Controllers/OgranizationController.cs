using MediatR;
using MedVisit.CatalogService.Entities;
using MedVisit.CatalogService.Mediatr.Organization.Commands;
using MedVisit.CatalogService.Mediatr.Organization.Queries;
using MedVisit.CatalogService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MedVisit.Core.Enums;
using Microsoft.OpenApi.Extensions;

namespace MedVisit.AccessManagement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Organizations")]
    public class OrganizationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrganizationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получить все организации
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<OrganizationDto>> GetOrganizations()
            => await _mediator.Send(new GetOrganizationsQuery());

        /// <summary>
        /// Получить организацию
        /// </summary>
        /// <param name="id">Идентификатор организации</param>
        /// <response code="200">Возвращает найденную организацию</response>
        /// <response code="404">организация не найдена</response>
        /// <returns>организация</returns>
        [HttpGet("ownerOrganization")]
        [Authorize(Roles = "ServiceOwner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOwnerOrganization()
        {
            var ownerIdClaim = User.FindFirst("user_id")?.Value;
            var ownerId = int.Parse(ownerIdClaim);
            return Ok(await _mediator.Send(new GetOwnerOrganizationQuery(ownerId)));
        }

        /// <summary>
        /// Создать организацию
        /// </summary>
        /// <param name="request">Тело запроса</param>
        /// <response code="201">Возвращает созданную организацию</response>
        /// <response code="400">Входные данные некорректны</response>
        /// <returns>Созданная организация</returns>
        [HttpPost]
        [Authorize(Roles = "ServiceOwner")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] OrganizationDto request)
        {
            var ownerIdClaim = User.FindFirst("user_id")?.Value;
            var ownerId = int.Parse(ownerIdClaim);

            var result = await _mediator.Send(new CreateOrganizationCommand(ownerId, request));
            return CreatedAtAction(nameof(GetOwnerOrganization), new { id = result.Id }, result);
        }

        /// <summary>
        /// Обновить мед услугу
        /// </summary>
        /// <param name="id">Идентификатор мед услуги/param>
        /// <param name="request">Тело запроса</param>
        /// <response code="200">Возвращает обновленную мед услугу</response>
        /// <response code="400">Входные данные некорректны</response>
        /// <response code="404">мед услуга не найдена</response>
        /// <returns>мед услуга</returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "ServiceOwner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([Required] int id, [FromBody] OrganizationDto request)
        {

            var ownerIdClaim = User.FindFirst("user_id")?.Value;
            var ownerId = int.Parse(ownerIdClaim);
            return Ok(await _mediator.Send(new UpdateOrganizationCommand(ownerId, request)));
        }

    }
}
