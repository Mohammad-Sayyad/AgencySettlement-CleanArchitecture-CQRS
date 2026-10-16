namespace AgencySettlement.Application.ExternalExamsFeatures.Commands.ImportSingleLessons.Rules;

public sealed class RegularSingleLessonPlanRule : ISingleLessonPlanRule
{
    public int RegistrationPlanId => 1;
    public (decimal DebitAmount, decimal CreditAmount) Calculate(decimal agencyAmount) => (agencyAmount, 0m);
}
