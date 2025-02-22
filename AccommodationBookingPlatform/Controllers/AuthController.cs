using AccommodationBookingPlatform.Domain;
using Application.Users.Login;
using Application.Users.Register;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace AccommodationBookingPlatform.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AuthController(ISender mediator, IMapper mapper) : ControllerBase
    {

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponse>> Login(
          LoginRequest loginRequest,
          CancellationToken cancellationToken)
        {
            var loginCommand = mapper.Map<LoginCommand>(loginRequest);

            return Ok(await mediator.Send(loginCommand, cancellationToken));
        }

        [HttpPost("register-guest")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterUser(
          RegisterRequest registerRequest,
          CancellationToken cancellationToken)
        {
            var registerCommand = new RegisterCommand { Role = UserRoles.Guest };
            mapper.Map(registerRequest, registerCommand);

            await mediator.Send(registerCommand, cancellationToken);

            return NoContent();
        }
    }
}
