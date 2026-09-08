using AgencySettlement.Application.Abstractions.Persistence.ComboBoxRepository;
using AgencySettlement.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Infrastructure.Persistence.Repositories
{
    public sealed class SettlementLookupRepository
    : ISettlementLookupRepository
    {
        private readonly AgencySettlementDbContext _context;

        public SettlementLookupRepository(
            AgencySettlementDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<AgencyLookupDto>> GetAgenciesAsync(
            CancellationToken cancellationToken)
        {
            return await _context.Agencies
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new AgencyLookupDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    DetailCode = x.DetailCode,
                    StateId = x.StateId,
                    RegionId = x.RegionId
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<YearLookupDto>> GetYearsAsync(
            CancellationToken cancellationToken)
        {
            return await _context.YearTypes
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Select(x => new YearLookupDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<ExamDateLookupDto>> GetExamDatesAsync(
            CancellationToken cancellationToken)
        {
            return await _context.ExamDates
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Select(x => new ExamDateLookupDto
                {
                    Id = x.Id,
                    PersianDate = x.PersianDate
                })
                .ToListAsync(cancellationToken);
        }
    }
}
