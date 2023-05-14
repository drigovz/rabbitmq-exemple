namespace Producer.Application.Core.Persons.Handlers;

public class AddPersonHandler : IRequestHandler<AddPersonCommand, BaseResponse>
{
    private readonly IPersonRepository _repository;
    private readonly NotificationContext _notification;

    public AddPersonHandler(IPersonRepository repository, NotificationContext notification)
    {
        _repository = repository;
        _notification = notification;
    }

    public async Task<BaseResponse> Handle(AddPersonCommand request, CancellationToken cancellationToken)
    {
        var person = new Person(request.FirstName, request.LastName, request.Email);
        if (!person.Valid)
        {
            _notification.AddNotifications(person.ValidationResult);

            return new BaseResponse
            {
                Notifications = _notification.Notifications,
            };
        }

        var result = await _repository.AddAsync(person);
        if (result is null)
        {
            _notification.AddNotification("Error", "Error When try to add new client!");

            return new BaseResponse { Notifications = _notification.Notifications, };
        }

        return new BaseResponse { Result = result, };
    }
}