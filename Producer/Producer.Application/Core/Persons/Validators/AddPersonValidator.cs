namespace Producer.Application.Core.Persons.Validators;

public class AddPersonValidator : AbstractValidator<AddPersonCommand>
{
    public AddPersonValidator()
    {
        RuleFor(_ => _.FirstName).FirstName();
        RuleFor(_ => _.LastName).LastName();
        RuleFor(_ => _.Email).Email();
    }
}
