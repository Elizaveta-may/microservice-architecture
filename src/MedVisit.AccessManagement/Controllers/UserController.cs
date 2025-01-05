using MediatR;
using MedVisit.AccessManagement.Mediatr.User.Handlers;
using MedVisit.AccessManagement.Mediatr.User.Queries;
using MedVisit.AccessManagement.Models.User;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MedVisit.AccessManagement.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("Healthy")]
        public IActionResult Check()
        {
            return Ok(new { status = "Healthy" });
        }
        /// <summary>
        /// Получить всех пользователей
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<UserDto>> GetAll()
            => await _mediator.Send(new GetUsersQuery());

        /// <summary>
        /// Получить пользователя по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <response code="200">Возвращает найденного пользователя</response>
        /// <response code="404">Пользователь не найден</response>
        /// <returns>Пользователь</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([Required] int id)
            => Ok(await _mediator.Send(new GetUserByIdQuery(id)));

        /// <summary>
        /// Создать пользователя
        /// </summary>
        /// <param name="request">Тело запроса</param>
        /// <response code="201">Возвращает созданного пользователя</response>
        /// <response code="400">Входные данные некорректны</response>
        /// <response code="409">Такой пользователь уже существует</response>
        /// <returns>Созданный пользователь</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] UserDto request)
        {
            var result = await _mediator.Send(new CreateUserCommand(request));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Обновить пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <param name="request">Тело запроса</param>
        /// <response code="200">Возвращает обновленного пользователя</response>
        /// <response code="400">Входные данные некорректны</response>
        /// <response code="404">Пользователь не найден</response>
        /// <returns>Пользователь</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([Required] int id, [FromBody] UserDto request)
            => Ok(await _mediator.Send(new UpdateUserCommand(id, request)));

        /// <summary>
        /// Удалить пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <response code="200">Успех</response>
        /// <response code="404">Пользователь не найден</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([Required] int id)
            => Ok(await _mediator.Send(new DeleteUserCommand(id)));
    }
}
