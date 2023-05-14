namespace Consumer.Api.Controllers;

[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class EmailsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmailsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SendEmailCommand command) =>
        Ok(await _mediator.Send(command));
}