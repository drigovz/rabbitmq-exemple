namespace Producer.Application.Core.Persons.Handlers;

public class UpdatePersonHandler : IRequestHandler<UpdatePersonCommand, BaseResponse>
{
	private readonly IPersonRepository _repository;
	private readonly NotificationContext _notification;

	public UpdatePersonHandler(IPersonRepository repository, NotificationContext notification)
	{
		_repository = repository;
		_notification = notification;
	}

	public async Task<BaseResponse> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
	{
		var person = await _repository.GetByIdAsync(request.Id);
		if (person is null)
			return new BaseResponse
			{
				Notifications = _notification.AddNotification("Error", $"Person with id {request.Id} not found!"),
			};

		person.Update(
			request.FirstName ?? person.FirstName,
			request.LastName ?? person.LastName,
			request.Email ?? person.Email
		);

		await _repository.UpdateAsync(person);

		return new BaseResponse
		{
			Result = person,
			Notifications = _notification.AddNotification("Success", $"Person with id {request.Id} update succesfull!"),
		};
	}
}
