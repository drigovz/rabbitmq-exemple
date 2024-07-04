namespace Producer.Application.Core.Persons.Handlers;

public class GetPersonHandler : IRequestHandler<GetPersonQuery, BaseResponse>
{
    private readonly IPersonRepository _repository;
    private readonly NotificationContext _notification;

    public GetPersonHandler(IPersonRepository repository, NotificationContext notification)
    {
        _repository = repository;
        _notification = notification;
    }

    public async Task<BaseResponse> Handle(GetPersonQuery request, CancellationToken cancellationToken)
    {
        var person = await _repository.GetByIdAsync(request.Id);
        if (person is null)
            return new BaseResponse
            {
                Notifications = _notification.AddNotification("Error", $"Person with id {request.Id} not found!"),
            };

        return new BaseResponse
        {
            Result = person,
        };
    }
}
