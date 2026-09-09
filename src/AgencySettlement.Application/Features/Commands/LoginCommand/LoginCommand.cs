using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Commands.LoginCommand
{
    public sealed record LoginCommand(
    string PhoneNumber,
    string Password
) : IRequest<LoginResponse>;

    public sealed record LoginResponse(
        string AccessToken,
        DateTime ExpiresAt,
        long UserId,
        string PhoneNumber,
        string? FirstName,
        string? LastName
    );
}
