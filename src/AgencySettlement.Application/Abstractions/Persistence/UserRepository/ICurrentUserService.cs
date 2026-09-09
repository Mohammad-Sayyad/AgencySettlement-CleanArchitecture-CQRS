using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Abstractions.Persistence.UserRepository
{
    public interface ICurrentUserService
    {
        long? UserId { get; }

        string? PhoneNumber { get; }
    }
}
