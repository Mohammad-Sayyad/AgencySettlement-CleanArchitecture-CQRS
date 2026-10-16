namespace AgencySettlement.Application.ExternalExamsFeatures.Commands.ImportSingleLessons.Rules;

public sealed class HekmatSingleLessonPlanRule : ISingleLessonPlanRule
{
    public int RegistrationPlanId => 2;
    public (decimal DebitAmount, decimal CreditAmount) Calculate(decimal agencyAmount) => (0m, agencyAmount);
}
