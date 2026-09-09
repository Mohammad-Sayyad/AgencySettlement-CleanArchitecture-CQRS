using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Commands.RegisterCommand
{
    public sealed record RegisterResponse(
     long UserId,
     string PhoneNumber,
     string FirstName,
     string LastName);
}
