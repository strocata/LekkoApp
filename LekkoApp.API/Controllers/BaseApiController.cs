using Microsoft.AspNetCore.Mvc;

namespace LekkoApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
}
