using BLL.Enums;
using System.IO;

namespace BLL.DTO
{
    public class ReportSA : Report
    {
        public ReportSA(string reportPath) : base(reportPath)
        {
        }

        public ReportSA(string reportPath, string userinfo) : base(reportPath, userinfo)
        {
        }

        public override ReportType Document
        {
            get
            {
                return ReportType.SA;
            }
        }
        public override string TemplateFilePath
        {
            get
            {
                string tempPath = Path.GetTempFileName();
                File.WriteAllBytes(tempPath, Properties.Resources.SA_Template_v1_4);
                base.TemplateFilePath = tempPath;
                return base.TemplateFilePath;
            }


        }
        public override string RangeTo
        {
            get
            {
                return "Y80";
            }
        }
        public override string RangeFrom
        {
            get
            {
                return "A1";
            }
        }
       
        public override string[] ForPrintCommonSheet
        {
            get
            {
                return new string[]
                {
                     "SiteID",
                    "Candidate",
                    "Document_Type",
                    "DateTime",
                    "SiteName",
                    "SiteAddress",
                    "Latitude",
                    "Longitude",
                    "StructureType",
                    "StructureHeight",
                    "SiteOwner",
                    "ColocationType",
                    "InstallationType",
                    "BSC",
                    "RNC",
                    "RF_Engineer",
                    "Mobile",
                    "Email",
                    "LTE_900",
                    "GSM_900",
                    "UMTS_900",
                    "GSM_1800",
                    "LTE_1800",
                    "UMTS_2100",
                    "LTE_2100",
                    "LTE_2600",
                    "NR_1800",
                    "NR_2100",
                    "NR_3500",
                };
            }
        }

        public override string[] ForPrintSectorSheet
        {
            get
            {
                return new string[]
                {
                    "Antenna Current#",
                    "Pole Current",
                    "Azimuth Current",
                    "AntennaType current",
                    "AntennaMount Current",
                    "AGL Current [m]",
                    "ARTL Current",
                    "MechanicalTilt Current",
                    "BandRange Current",
                    "Band Current",
                    "Technology Current",
                    "Etilt Current",
                    "RET Current",
                    "Feeder_Type Current",
                    "Feeder_Length Current",
                    "RRU_Type Current",
                    "RRU_Total #  Current",
                    "TMA Type Current",
                    "Combiner_Splitter Current",
                    "Sec_Combiner_Splitter Current",
                    "Collocation Current",
                    "Audit Remarks:",
                    "Request_Remarks",
                    "PortName",
                  
                };
            }
        }

       

    }
}
