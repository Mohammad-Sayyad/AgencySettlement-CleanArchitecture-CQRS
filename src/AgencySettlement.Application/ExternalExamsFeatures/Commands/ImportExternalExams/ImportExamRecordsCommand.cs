using AgencySettlement.Application.DTOs;
using MediatR;

namespace AgencySettlement.Application.ExternalExams.Commands.ImportExternalExams;

public sealed record ImportExamRecordsCommand(
    ImportExamRequest Record
) : IRequest<ImportExamResult>;