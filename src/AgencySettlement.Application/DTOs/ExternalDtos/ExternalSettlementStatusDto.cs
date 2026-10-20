using AgencySettlement.Application.DTOs;
using AgencySettlement.Application.DTOs.SingleLessonDto;
using AgencySettlement.Domain.Enums;

namespace AgencySettlement.Application.DTOs.ExternalDtos;

public sealed class ExternalSettlementStatusDto
{
    public int AgencyId { get; set; }

    public int YearId { get; set; }

    public long SettlmentId { get; set; }

    public long? SingleLessonSettlementId { get; set; }

    public string PersianExecutionDate { get; set; } = string.Empty;

    public string PaymentDeadline { get; set; } = string.Empty;

    public string StatusTitle { get; set; } = string.Empty;

    public SettlementPaymentStatus PaymentStatus { get; set; }

    public decimal Balance { get; set; }

    public List<SettlementDetailItemDto> Items { get; set; } = [];

    public int TotalInPersonCount { get; set; }

    public int TotalOnlineCount { get; set; }

    public List<SingleLessonSettlementDetailItemDto> SingleLessonItems { get; set; } = [];

    public decimal ExamTotalDebit { get; set; }

    public decimal ExamTotalCredit { get; set; }

    public decimal ExamBalance { get; set; }

    public decimal SingleLessonTotalDebit { get; set; }

    public decimal SingleLessonTotalCredit { get; set; }

    public decimal SingleLessonBalance { get; set; }

    public decimal TotalDebit { get; set; }

    public decimal TotalCredit { get; set; }

    public decimal TotalBalance { get; set; }

    public int SingleLessonTotalCourseCount { get; set; }
}