using MediatR;
using MedVisit.AccessManagement.Mediatr.User.Handlers;
using MedVisit.AccessManagement.Mediatr.User.Queries;
using MedVisit.AccessManagement.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prometheus;
using System.ComponentModel.DataAnnotations;

namespace MedVisit.AccessManagement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        private static readonly Counter RequestCounter = Metrics.CreateCounter("user_requests_total", "Total number of requests to user endpoints");
        private static readonly Histogram LatencyHistogram = Metrics.CreateHistogram("user_request_latency_seconds", "Histogram of user request latencies in seconds", new HistogramConfiguration
        {
            Buckets = Histogram.LinearBuckets(0.01, 0.05, 10)
        });
        private static readonly Counter ErrorCounter = Metrics.CreateCounter("user_errors_total", "Total number of errors (500 responses)");

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("Healthy")]
        public IActionResult Check()
        {
            RequestCounter.Inc();
            return Ok(new { status = "Healthy" });
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<UserDto>> GetAll()
        {
            RequestCounter.Inc();
            using (LatencyHistogram.NewTimer())
            {
                try
                {
                    return await _mediator.Send(new GetUsersQuery());
                }
                catch
                {
                    ErrorCounter.Inc();
                    throw;
                }
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([Required] int id)
        {
            RequestCounter.Inc();
            using (LatencyHistogram.NewTimer())
            {
                try
                {
                    var user = await _mediator.Send(new GetUserByIdQuery(id));
                    if (user == null)
                        return NotFound();

                    return Ok(user);
                }
                catch
                {
                    ErrorCounter.Inc();
                    throw;
                }
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] UserDto request)
        {
            RequestCounter.Inc();
            using (LatencyHistogram.NewTimer())
            {
                try
                {
                    var result = await _mediator.Send(new CreateUserCommand(request));
                    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
                }
                catch
                {
                    ErrorCounter.Inc();
                    throw;
                }
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([Required] int id, [FromBody] UserDto request)
        {
            RequestCounter.Inc();
            using (LatencyHistogram.NewTimer())
            {
                try
                {
                    var updatedUser = await _mediator.Send(new UpdateUserCommand(id, request));
                    return Ok(updatedUser);
                }
                catch
                {
                    ErrorCounter.Inc();
                    throw;
                }
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([Required] int id)
        {
            RequestCounter.Inc();
            using (LatencyHistogram.NewTimer())
            {
                try
                {
                    await _mediator.Send(new DeleteUserCommand(id));
                    return Ok();
                }
                catch
                {
                    ErrorCounter.Inc();
                    throw;
                }
            }
        }
    }
}
