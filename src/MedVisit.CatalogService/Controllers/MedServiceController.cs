using MediatR;
using MedVisit.CatalogService.Entities;
using MedVisit.CatalogService.Mediatr.MedService.Commands;
using MedVisit.CatalogService.Mediatr.MedService.Queries;
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
    [Route("MedServices")]
    public class MedServiceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MedServiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получить всех мед услуги
        /// </summary>
        [HttpGet("organization/{organizationId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<MedServiceDetailsDto>> GetAllInOrganization(int organizationId)
            => await _mediator.Send(new GetMedServicesInOrganizationQuery(organizationId));

        /// <summary>
        /// Получить мед услугу по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор мед услуги</param>
        /// <response code="200">Возвращает найденную мед услугу</response>
        /// <response code="404">мед услуга не найдена</response>
        /// <returns>мед услуга</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([Required] int id)
            => Ok(await _mediator.Send(new GetMedServiceByIdQuery(id)));

        /// <summary>
        /// Получить мед работников выполняющих мед услугу 
        /// </summary>
        /// <param name="id">Идентификатор мед услуги</param>
        /// <response code="200">Возвращает найденных мед работников</response>
        /// <response code="404">мед услуга не найдена</response>
        /// <returns>мед работники</returns>
        [HttpGet("serviceMedWorkers{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMedicalWorkersForMedService([Required] int id)
            => Ok(await _mediator.Send(new GetMedicalWorkersForMedServiceQuery(id)));

        /// <summary>
        /// Создать мед услугу
        /// </summary>
        /// <param name="request">Тело запроса</param>
        /// <response code="201">Возвращает созданную мед услугу</response>
        /// <response code="400">Входные данные некорректны</response>
        /// <response code="409">Такая мед услуга уже существует</response>
        /// <returns>Созданная мед услуга</returns>
        [HttpPost]
        [Authorize(Roles = "ServiceOwner")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] MedServiceDto request)
        {
            var result = await _mediator.Send(new CreateMedServiceCommand(request));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
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
        public async Task<IActionResult> Update([Required] int id, [FromBody] MedServiceDto request)
            => Ok(await _mediator.Send(new UpdateMedServiceCommand(id, request)));

        /// <summary>
        /// Удалить мед услугу
        /// </summary>
        /// <param name="id">Идентификатор мед услуги</param>
        /// <response code="200">Успех</response>
        /// <response code="404">мед услуга не найдена</response>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ServiceOwner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([Required] int id)
            => Ok(await _mediator.Send(new DeleteMedServiceCommand(id)));
    }
}
