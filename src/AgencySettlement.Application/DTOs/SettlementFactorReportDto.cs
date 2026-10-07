using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.DTOs
{
    public sealed class SettlementFactorReportDto
    {
        public string DetailCode { get; set; } = string.Empty;
        public string AgencyName { get; set; } = string.Empty;

        public int EducationalLevelId { get; set; }
        public string EducationalLevelName { get; set; } = string.Empty;

        public int StudyFieldId { get; set; }
        public string StudyFieldName { get; set; } = string.Empty;

        public string PackageName { get; set; } = string.Empty;

        public int CandidateCount { get; set; }

        public int FreeCount { get; set; }
        public int HekmatCount { get; set; }
        public int SiteCount { get; set; }
        public int SchoolScholarshipCount { get; set; }
        public int FreeVolunteerScholarshipCount { get; set; }

        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal Balance { get; set; }

        public decimal AgencyPercent { get; set; }
    }
}
