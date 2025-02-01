using MediatR;
using MedVisit.CatalogService.Entities;
using MedVisit.CatalogService.Mediatr.MedicalWorker.Commands;
using MedVisit.CatalogService.Mediatr.MedicalWorker.Queries;
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
    [Route("MedicalWorkers")]
    public class MedicalWorkerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MedicalWorkerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получить всех мед работников
        /// </summary>
        [HttpGet("organization/{organizationId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<MedicalWorkerDto>> GetAllInOrganization(int organizationId)
            => await _mediator.Send(new GetMedicalWorkersInOrganizationQuery(organizationId));

        /// <summary>
        /// Получить мед работника по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор мед работника</param>
        /// <response code="200">Возвращает найденного мед работника</response>
        /// <response code="404">Мед работник не найден</response>
        /// <returns>Пользователь</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([Required] int id)
            => Ok(await _mediator.Send(new GetMedicalWorkerByIdQuery(id)));

        /// <summary>
        /// Создать мед работника
        /// </summary>
        /// <param name="request">Тело запроса</param>
        /// <response code="201">Возвращает созданного мед работника</response>
        /// <response code="400">Входные данные некорректны</response>
        /// <response code="409">Такой мед работник уже существует</response>
        /// <returns>Созданный мед работник</returns>
        [HttpPost]
        [Authorize(Roles = "ServiceOwner")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] MedicalWorkerDto request)
        {
            var result = await _mediator.Send(new CreateMedicalWorkerCommand(request));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Обновить мед работника
        /// </summary>
        /// <param name="id">Идентификатор мед работника/param>
        /// <param name="request">Тело запроса</param>
        /// <response code="200">Возвращает обновленного мед работника</response>
        /// <response code="400">Входные данные некорректны</response>
        /// <response code="404">мед работник не найден</response>
        /// <returns>мед работник</returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "ServiceOwner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([Required] int id, [FromBody] MedicalWorkerDto request)
            => Ok(await _mediator.Send(new UpdateMedicalWorkerCommand(id, request)));

        /// <summary>
        /// Удалить мед работника
        /// </summary>
        /// <param name="id">Идентификатор мед работника</param>
        /// <response code="200">Успех</response>
        /// <response code="404">мед работник не найден</response>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ServiceOwner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([Required] int id)
            => Ok(await _mediator.Send(new DeleteMedicalWorkerCommand(id)));
    }
}
