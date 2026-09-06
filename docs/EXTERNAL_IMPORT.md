# External API Import - Simple

This module has one responsibility:

External API -> receive records -> ignore duplicate CandidateExamId -> insert new records into ExternalExamRecords.

It does NOT calculate settlement, price, percentage, agency share, Gaj share, or create Settlement records.

Endpoint:
POST /api/external-exams/import

Duplicate handling:
1. Duplicates inside the API response are grouped by CandidateExamId.
2. CandidateExamIds already stored in ExternalExamRecords are skipped.
3. The database has a UNIQUE index on CandidateExamId as the final protection.

Configuration:
ExternalExamApi:BaseUrl
ExternalExamApi:ExamsPath
ExternalExamApi:TimeoutSeconds

The existing financial calculation code is intentionally not modified by this patch.
