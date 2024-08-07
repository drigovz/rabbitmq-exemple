namespace Producer.Core.Validations;

public static class BaseValidations
{
    public static IRuleBuilderOptions<T, string> FirstName<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.NotNull().NotEmpty().WithMessage("FirstName is required!");

    public static IRuleBuilderOptions<T, string> LastName<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.NotNull().NotEmpty().WithMessage("LastName is required!");

    public static IRuleBuilderOptions<T, string> Email<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.NotNull().NotEmpty().EmailAddress();

    public static IRuleBuilderOptions<T, DateTime> DateTimeValidate<T>(this IRuleBuilder<T, DateTime> ruleBuilder) =>
        ruleBuilder.NotNull()
            .NotEmpty()
            .Must(BeAValidDate)
            .WithMessage("Date must be validate date format!");

    public static IRuleBuilderOptions<T, string> IsGuid<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.NotNull()
            .NotEmpty()
            .Must(BeAValidGuid)
            .WithMessage("Id must be validate guid format!");

    private static bool BeAValidDate(DateTime date) =>
        !date.Equals(default(DateTime));

    private static bool BeAValidGuid(string value) =>
        Guid.TryParse(value, out _);
}
