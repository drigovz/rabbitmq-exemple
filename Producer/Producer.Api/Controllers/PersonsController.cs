namespace Producer.Api.Controllers;

[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class PersonsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PersonsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AddPersonCommand command) =>
        Ok(await _mediator.Send(command));

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([BindRequired] Guid id) =>
        Ok(await _mediator.Send(new GetPersonQuery { Id = id }));
    
    [HttpPatch]
    public async Task<IActionResult> Post([FromBody] UpdatePersonCommand command) =>
        Ok(await _mediator.Send(command));
}