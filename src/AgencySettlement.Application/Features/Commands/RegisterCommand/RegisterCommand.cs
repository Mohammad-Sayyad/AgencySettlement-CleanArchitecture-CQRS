using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Commands.RegisterCommand
{

    public sealed record RegisterCommand(
    string PhoneNumber,
    string Password,
    string FirstName,
    string LastName) : IRequest<RegisterResponse>;
}
