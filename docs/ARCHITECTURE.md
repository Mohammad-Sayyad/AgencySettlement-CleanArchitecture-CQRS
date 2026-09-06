# Architecture decisions

## 1. No Service Layer

The use case is implemented by the CQRS Handler. Domain invariants live in domain entities/value objects. Persistence and HTTP details stay in Infrastructure.

## 2. Repository pattern

A small base repository exists for common operations, while every entity gets a dedicated repository contract and class. This keeps entity-specific query intent visible and avoids an all-purpose generic repository.

## 3. Financial correctness

Amounts use `decimal(18,2)` and `MidpointRounding.AwayFromZero`. The Gaj amount is calculated as `GrossAmount - AgencyAmount`, which guarantees the two final shares sum exactly to the Gross amount after rounding.

## 4. Percentage correctness

There are two rows in `Percents`: Agency and Gaj. A valid pair must have the same validity window and priority and must satisfy `Gaj = 100 - Agency`. A settlement refuses to proceed if a valid pair cannot be selected.

## 5. History

`SettlementHistory` is an immutable snapshot. It captures the selected price, selected percent rows, validity dates, priority, agency identity snapshot, input snapshot, CandidateExamId/amount snapshot and final totals. This lets an auditor reconstruct what was used at calculation time even after configuration changes.

## 6. External API persistence

The external API is only a source. Its returned CandidateExamId/Amount data is stored in `ExternalExamAmounts` under the created Settlement so the exact API-derived base amount is persisted.

## 7. Idempotency

`Settlements.IdempotencyKey` is unique. A request hash makes reusing the same key for a different business request an explicit 409 conflict.
