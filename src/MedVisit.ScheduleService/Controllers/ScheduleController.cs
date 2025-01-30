using MedVisit.ScheduleService.Models;
using MedVisit.ScheduleService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedVisit.ScheduleService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly IManagementAvailabilitySlotsService _scheduleService;

        public ScheduleController(IManagementAvailabilitySlotsService scheduleService)
        {
            _scheduleService = scheduleService;
        }
        /// <summary>
        /// Создает окошки для записи на услугу мед работника.
        /// </summary>
        /// <param name="medicalWorkerId">ID мед работника</param
        /// <param name="startDate">С Даты</param>
        /// <param name="endDate">По Дату</param>
        /// <param name="timeSlotInMinutes">Время для слота</param>
        /// <response code="201">Окошки для записи созданы</response>
        [HttpPost]
        [Route("Create")]
        [Authorize(Roles = "ServiceOwner")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateAvailabilitySlots([FromBody] AvailabilitySlotRequest request)
        {
            await _scheduleService.CreateAvailabilitySlots(request);

            return StatusCode(StatusCodes.Status201Created, "Окошки для записи созданы");
        }

        /// <summary>
        /// Получить доступные окошки для записи на услугу мед работника на определенную дату.
        /// </summary>
        /// <param name="medicalWorkerId">ID мед работника</param>
        /// <param name="date">Дата</param>
        /// <returns>Список доступных окошек</returns>
        [HttpGet]
        [Route("GetAllSlots")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllSlots(int medicalWorkerId, DateTime date)
        {
            var allSlots = await _scheduleService.GetAllSlotsAsync(medicalWorkerId, date);
            return Ok(allSlots);
        }

        /// <summary>
        /// Освободить слот, который был забронирован.
        /// </summary>
        /// <param name="availabilityId">ID окошка</param>
        /// <response code="200">Слот освобожден</response>
        /// <response code="400">Окошко не найдено или уже свободно</response>
        [HttpPut]
        [Route("FreeSlot")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> FreeSlot(int availabilityId)
        {
            var result = await _scheduleService.FreeSlotAsync(availabilityId);

            if (result)
            {
                return Ok("Слот освобожден");
            }

            return BadRequest("Не удалось освободить слот (возможно, он уже свободен)");
        }

        /// <summary>
        /// Забронировать слот для записи на услугу мед работника.
        /// </summary>
        /// <param name="availabilityId">ID окошка</param>
        /// <response code="200">Слот забронирован</response>
        /// <response code="400">Окошко не найдено или уже занято</response>
        [HttpPut]
        [Authorize(Roles = "User")]
        [Route("BookSlot")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BookSlot(int availabilityId)
        {
            var result = await _scheduleService.BookSlotAsync(availabilityId);

            if (result)
            {
                return Ok("Слот забронирован");
            }

            return BadRequest("Не удалось забронировать слот (возможно, он уже занят)");
        }
    }
}
