using Consumer.Application.Core.Emails.Commands;

namespace Consumer.Application.Core.Emails.Handlers;

public class SendEmailHandler : IRequestHandler<SendEmailCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {
        var result = new { request.Name, request.Email, request.Body };
        
        return new BaseResponse
        {
            Result = result,
        };
    }
}