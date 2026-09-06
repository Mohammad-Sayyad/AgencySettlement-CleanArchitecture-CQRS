using AgencySettlement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Abstractions.Persistence.Repositories
{
    public interface IPercentRuleRepository
    {
        Task<Percent?> GetAsync(
      int agencyId,
      int examModeId,
      CancellationToken cancellationToken);
    }
}
