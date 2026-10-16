namespace AgencySettlement.Application.ExternalExamsFeatures.Commands.ImportSingleLessons.Rules;

public interface ISingleLessonPlanRuleFactory
{
    ISingleLessonPlanRule Get(int registrationPlanId);
}

public sealed class SingleLessonPlanRuleFactory : ISingleLessonPlanRuleFactory
{
    private readonly IReadOnlyDictionary<int, ISingleLessonPlanRule> _rules;
    public SingleLessonPlanRuleFactory(IEnumerable<ISingleLessonPlanRule> rules) => _rules = rules.ToDictionary(x => x.RegistrationPlanId);
    public ISingleLessonPlanRule Get(int registrationPlanId) => _rules.TryGetValue(registrationPlanId, out var rule)
        ? rule
        : throw new InvalidOperationException($"RegistrationPlanId={registrationPlanId} برای تسویه تک‌درس پشتیبانی نمی‌شود.");
}
