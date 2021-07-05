using BLL.Enums;
using System.IO;

namespace BLL.DTO
{
    public class ReportIRFC:Report
    {
        public ReportIRFC(string reportPath) : base(reportPath)
        {
        }

        public ReportIRFC(string reportPath, string userinfo) : base(reportPath, userinfo)
        {
        }

        public override ReportType Document
        {
            get
            {
                return  ReportType.IRFC;
            }
        }
        public override string TemplateFilePath
        {
            get
            {
                string tempPath = Path.GetTempFileName();
                File.WriteAllBytes(tempPath, Properties.Resources.IRFC_Template_v1_2);
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
                return  "A1";
            }
        }
       
        public override string[] ForPrintCommonSheet
        {
            get
            {
                return  new string[]
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
                return  new string[]
                {
                    "Antenna #",
                    "Pole New",
                    "Azimuth New",
                    "AntennaType New ",
                    "AntennaMount New",
                    "AGL new [m]",
                    "ARTL new",
                    "MechanicalTilt new",
                    "PortName",
                    "BandRange New",
                    "Technology",
                    "Etilt New",
                    "RET New",
                    "Feeder_Type New",
                    "Feeder_Length New",
                    "RRU_Type New",
                    "RRU_Total # New",
                    "GSM_TRX New",
                    "GSM_Pwr_per_TRX New, dBm",
                    "UMTS_TRX New",
                    "UMTS_Pwr_per_TRX New, dBm",
                    "LTE_TRX New",
                    "LTE_Pwr_per_TRX New, dBm",
                    "TMA Type New",
                    "Combiner_Splitter New",
                    "Sec_Combiner_Splitter New",
                    //"Collocation  New",
                    "Antenna_IN_Total_Power, dBm",
                    "Request_Remarks",

                };
            }
        }

        
    }
}
