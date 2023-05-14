namespace Producer.Application.Core.Persons.Validators;

public class UpdatePersonValidator : AbstractValidator<UpdatePersonCommand>
{
    public UpdatePersonValidator()
    {
        RuleFor(_ => _.Id.ToString()).IsGuid();
    }
}