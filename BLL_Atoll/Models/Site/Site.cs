using BLL_Atoll.Attributes;
using BLL_Atoll.Enums;
using BLL_Atoll.Interfaces;
using System.Reflection;

namespace BLL_Atoll.Models.Site
{
    public record Site:IExcel
    {

        [ForPrint("SiteID")]
        public string SiteID { get; set; }
        
        [ForPrint("SiteAddress")]
        public string SiteAddress { get; init; }
        
        [ForPrint("Candidate")]
        public string Candidate { get; init; }
        
        [ForPrint("SiteName")]
        public string SiteName { get; init; }
        
        [ForPrint("Latitude")]
        public decimal Latitude { get; init; }
        
        [ForPrint("Longitude")]
        public decimal Longitude { get; init; }
        
        [ForPrint("StructureType")]
        public string StructureType { get; init; }
        
        [ForPrint("StructureHeight")]
        public decimal Height { get; init; }
        
        [ForPrint("InstallationType")]
        public string InstallationType { get; init; }
        
        [ForPrint("BSC")]
        public string BSC { get; init; }
        
        [ForPrint("RNC")]
        public string RNC { get; init; }
        
        [ForPrint("SiteOwner")]
        public string SiteOwner { get; init; }
        
        [ForPrint("ColocationType")]
        public string ColocationType { get; init; }

        public int OffsetExcel(PropertyInfo propInfo)
        {
            return 1;
        }

        public SheetNames SheetName(PropertyInfo propInfo)
        {
            return SheetNames.Common;
        }
    }

}
