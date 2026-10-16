namespace AgencySettlement.Application.ExternalExamsFeatures.Commands.ImportSingleLessons.Rules;

public interface ISingleLessonPlanRule
{
    int RegistrationPlanId { get; }
    (decimal DebitAmount, decimal CreditAmount) Calculate(decimal agencyAmount);
}
