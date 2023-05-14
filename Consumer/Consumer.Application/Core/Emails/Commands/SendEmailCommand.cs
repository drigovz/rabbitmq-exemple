
namespace Consumer.Application.Core.Emails.Commands;

public class SendEmailCommand  : IRequest<BaseResponse>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Body { get; set; }
}