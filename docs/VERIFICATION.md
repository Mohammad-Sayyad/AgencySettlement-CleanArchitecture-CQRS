# Verification checklist

Static checks completed in the generated workspace:

- Four application layers are present: Domain, Application, Infrastructure, API.
- No Service layer is present.
- CQRS handlers are registered through MediatR assembly scanning.
- Controllers depend only on `IMediator`.
- Dedicated repository interface/implementation exists for every domain entity.
- External API contracts are in Application; HTTP implementation is in Infrastructure.
- Input is persisted in `SettlementInputs`.
- External API amounts are persisted in `ExternalExamAmounts`.
- CandidateExamId is unique inside a Settlement.
- `AgencyPercent + GajPercent = 100` is enforced by Domain and SQL Settlement checks.
- `AgencyAmount + GajAmount = GrossAmount` is enforced by Domain and SQL Settlement checks.
- History contains price/percent validity snapshots and the CandidateExam amount snapshot.
- History UPDATE/DELETE is blocked by SQL trigger.
- Idempotency key is unique and request hash protects key reuse.
- EF Core design-time factory is included for migrations.
- SQL Server schema is included in `scripts/schema.sql`.
- Sample Price/Percent configuration is included in `scripts/seed-sample.sql`.

The current execution environment does not contain the .NET SDK, so an actual `dotnet build`/`dotnet test` run was not possible here. The project files and source were checked statically, including delimiter balance and repository/configuration references.
