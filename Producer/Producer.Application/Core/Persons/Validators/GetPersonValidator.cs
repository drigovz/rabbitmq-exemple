namespace Producer.Application.Core.Persons.Validators;

public class GetPersonValidator : AbstractValidator<GetPersonQuery>
{
    public GetPersonValidator()
    {
        RuleFor(_ => _.Id.ToString()).IsGuid();
    }
}
