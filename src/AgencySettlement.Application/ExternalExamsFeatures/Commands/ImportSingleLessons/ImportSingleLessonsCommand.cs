using AgencySettlement.Application.DTOs;
using AgencySettlement.Application.DTOs.SingleLessonDto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.ExternalExamsFeatures.Commands.ImportSingleLessons
{
    public sealed record ImportSingleLessonsCommand(
     ImportSingleLessonRequest Record
 ) : IRequest<ImportExamResult>;
}
