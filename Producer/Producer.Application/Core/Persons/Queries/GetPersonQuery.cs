namespace Producer.Application.Core.Persons.Queries;

public class GetPersonQuery : IRequest<BaseResponse>
{
    public Guid Id { get; set; }
}