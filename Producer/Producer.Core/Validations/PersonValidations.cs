namespace Producer.Core.Validations;

public class PersonValidations : AbstractValidator<Person>
{
	public PersonValidations()
	{
		RuleFor(_ => _.FirstName).FirstName();
		RuleFor(_ => _.LastName).LastName();
		RuleFor(_ => _.Email).Email();
	}
}
