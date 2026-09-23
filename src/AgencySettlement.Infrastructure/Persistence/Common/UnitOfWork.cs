using AgencySettlement.Application.Abstractions.Persistence.Common;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Infrastructure.Persistence.Common
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly AgencySettlementDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(AgencySettlementDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync(
            CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
                return;

            _transaction = await _context.Database.BeginTransactionAsync(
                cancellationToken);
        }

        public async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(
            CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
                return;

            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);

            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task RollbackTransactionAsync(
            CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
                return;

            await _transaction.RollbackAsync(cancellationToken);

            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}
