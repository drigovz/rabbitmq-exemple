namespace Producer.Application.Core.Persons;

public abstract class PersonCommand : IRequest<BaseResponse>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}
