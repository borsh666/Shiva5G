using BLL.Attributes;
using BLL.Interfaces;
using System.Collections.Generic;

namespace BLL.DTO
{
    public class Site:IOffset
    {
        [ForPrint]
        public string SiteID { get; set; }
        [ForPrint]
        public string Address { get; set; }
        [ForPrint]
        public string Candidate { get; set; }
        [ForPrint]
        public string Revision { get; set; }
        [ForPrint]
        public string SiteName { get; set; }
        [ForPrint]
        public decimal Latitude { get; set; }
        [ForPrint]
        public decimal Longitude { get; set; }
        [ForPrint]
        public string StructureType { get; set; }
        [ForPrint]
        public string StructureHeight { get; set; }
        [ForPrint]
        public string InstallationType { get; set; }
        [ForPrint]
        public string BSC { get; set; }
        [ForPrint]
        public string RNC { get; set; }
        [ForPrint]
        public string SiteOwner { get; set; }
        [ForPrint]
        public string ColocationType { get; set; }

        public List<Sector> Sectors { get; set; }

        public int OffsetExcel { get; set; }
    }
}
