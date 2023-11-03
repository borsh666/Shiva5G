using BLL_Atoll.Enums;
using System.ComponentModel.DataAnnotations;

namespace BlazorClient.Models
{
    public class ReportInfo
    {
        [Required]
        //[RegularExpression(@"([A-Z]{2}[\d]{4})+", ErrorMessage = "SiteID name contains invalid characters. Example: SF1001")]
        public string SiteId { get; set; }

        [Required]
        public ReportType ReportType { get; set; }

    }
}
