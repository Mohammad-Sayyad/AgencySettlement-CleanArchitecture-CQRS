using AgencySettlement.Application.Abstractions.Persistence.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Commands.SettlementsCommand.Calculation
{
    public sealed class CalculateSettlementCommandHandler
      : IRequestHandler<CalculateSettlementCommand, SettlementResultDto>
    {
        private readonly ISettlementCalculationCoordinator _coordinator;
        private readonly IUnitOfWork _unitOfWork;

        public CalculateSettlementCommandHandler(
            ISettlementCalculationCoordinator coordinator,
            IUnitOfWork unitOfWork)
        {
            _coordinator = coordinator;
            _unitOfWork = unitOfWork;
        }

        public async Task<SettlementResultDto> Handle(
            CalculateSettlementCommand command,
            CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(
                cancellationToken);

            try
            {
                var result =
                    await _coordinator.CalculateAsync(
                        command.Request,
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
