namespace Producer.Application.Core.Persons.Commands;

public class UpdatePersonCommand : IRequest<BaseResponse>
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
}
