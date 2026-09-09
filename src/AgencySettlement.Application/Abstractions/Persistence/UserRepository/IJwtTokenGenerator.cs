using AgencySettlement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Abstractions.Persistence.UserRepository
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
