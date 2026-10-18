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

    // فیلد قبلی برای حفظ سازگاری خروجی
    public decimal Balance { get; set; }

    // جزئیات آزمون؛ ساختار فعلی حفظ می‌شود
    public List<SettlementDetailItemDto> Items { get; set; } = [];

    public int TotalInPersonCount { get; set; }

    public int TotalOnlineCount { get; set; }

    // جزئیات تک‌درس
    public List<SingleLessonSettlementDetailItemDto>
        SingleLessonItems
    { get; set; } = [];

    // خلاصه بدهکاری‌ها
    public decimal ExamTotalDebit { get; set; }

    public decimal SingleLessonTotalDebit { get; set; }

    public decimal TotalDebit { get; set; }

    // تعداد کل تک‌درس
    public int SingleLessonTotalCourseCount { get; set; }
}