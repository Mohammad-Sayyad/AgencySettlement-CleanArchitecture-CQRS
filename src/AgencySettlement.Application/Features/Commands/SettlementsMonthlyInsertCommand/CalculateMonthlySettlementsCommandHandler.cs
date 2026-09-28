using AgencySettlement.Application.Abstractions.Persistence.Common;
using AgencySettlement.Application.Features.Commands.SettlementsMonthlyInsertCommand.Models;
using AgencySettlement.Application.Features.Commands.SettlementsMonthlyInsertCommand.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Commands.SettlementsMonthlyInsertCommand
{
    public sealed class CalculateMonthlySettlementsCommandHandler
    : IRequestHandler<
        CalculateMonthlySettlementsCommand,
        MonthlySettlementBatchResult>
    {
        private readonly MonthlySettlementCoordinator _coordinator;
        private readonly MonthlySettlementPersistence _persistence;
        private readonly IUnitOfWork _unitOfWork;

        public CalculateMonthlySettlementsCommandHandler(
            MonthlySettlementCoordinator coordinator,
            MonthlySettlementPersistence persistence,
            IUnitOfWork unitOfWork)
        {
            _coordinator = coordinator;
            _persistence = persistence;
            _unitOfWork = unitOfWork;
        }

        public async Task<MonthlySettlementBatchResult> Handle(
            CalculateMonthlySettlementsCommand request,
            CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(
                cancellationToken);

            try
            {
                var result =
                    await _coordinator.CalculateAndPersistAsync(
                        request.YearId,
                        request.Month,
                        _persistence,
                        cancellationToken);

                await _unitOfWork.CommitTransactionAsync(
                    cancellationToken);

                return result;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(
                    cancellationToken);

                throw;
            }
        }
    }
}
