using AgencySettlement.Application.Abstractions.Persistence.UserRepository;
using AgencySettlement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Infrastructure.Persistence.Repositories.UserRepositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly AgencySettlementDbContext _context;

        public UserRepository(
            AgencySettlementDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByPhoneNumberAsync(
         string phoneNumber,
         CancellationToken cancellationToken)
        {
            return await _context.Users
                .FirstOrDefaultAsync(
                    x => x.PhoneNumber == phoneNumber,
                    cancellationToken);
        }

        public async Task AddAsync(
            User user,
            CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(
                user,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public Task<User?> GetByIdAsync(
            long id,
            CancellationToken cancellationToken = default)
        {
            return _context.Users
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        public async Task UpdateAsync(
            User user,
            CancellationToken cancellationToken = default)
        {
            _context.Users.Update(user);

            await _context.SaveChangesAsync(
                cancellationToken);
        }
    }
}
