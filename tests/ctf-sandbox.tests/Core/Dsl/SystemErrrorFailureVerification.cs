using Shouldly;


namespace ctf_sandbox.tests.Core.Dsl;

public class SystemErrorFailureVerification : ErrorVerification<SystemError>
{
    public SystemErrorFailureVerification(SystemError error, UseCaseContext context)
        : base(error, context, e => e.Message)
    {
    }

    public new SystemErrorFailureVerification ErrorMessage(string expectedMessage)
    {
        base.ErrorMessage(expectedMessage);
        return this;
    }

    public SystemErrorFailureVerification FieldErrorMessageContaining(string expectedField, string expectedMessagePart)
    {
        var expandedExpectedField = Context.ExpandAliases(expectedField);
        var expandedExpectedPart = Context.ExpandAliases(expectedMessagePart);
        var fields = Response.Fields;

        fields.ShouldNotBeNull("Expected field errors but none were found");
        var messages = fields.Where(f => expandedExpectedField.Equals(f.Field)).Select(f => f.Message).ToList();

        messages.ShouldNotBeEmpty(
            $"Expected field error for field '{expandedExpectedField}', but field was not found in errors: {string.Join(", ", fields.Select(f => f.Field))}");
        messages.ShouldContain(m => m != null && m.Contains(expandedExpectedPart),
            $"Expected a field error message for '{expandedExpectedField}' containing '{expandedExpectedPart}', but got: {string.Join(" | ", messages)}");

        return this;
    }

    public SystemErrorFailureVerification FieldErrorMessage(string expectedField, string expectedMessage)
    {
        var expandedExpectedField = Context.ExpandAliases(expectedField);
        var expandedExpectedMessage = Context.ExpandAliases(expectedMessage);
        var error = Response;
        var fields = error.Fields;

        fields.ShouldNotBeNull("Expected field errors but none were found");
        fields.ShouldNotBeEmpty("Expected field errors but none were found");

        var matchingFieldError = fields.FirstOrDefault(f => expandedExpectedField.Equals(f.Field));

        matchingFieldError.ShouldNotBeNull(
            $"Expected field error for field '{expandedExpectedField}', but field was not found in errors: {string.Join(", ", fields.Select(f => f.Field))}");

        var actualMessage = matchingFieldError.Message;
        actualMessage.ShouldBe(expandedExpectedMessage,
            $"Expected field error message for field '{expandedExpectedField}': '{expandedExpectedMessage}', but got: '{actualMessage}'");

        return this;
    }
}
