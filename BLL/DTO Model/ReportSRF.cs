using BLL.Enums;
using System.Collections.Generic;
using System.IO;

namespace BLL.DTO
{
    public class ReportSRF : Report
    {
        public ReportSRF(string reportPath) : base(reportPath)
        {
            SetInitalParam();
        }

        public ReportSRF(string reportPath, string userinfo) : base(reportPath, userinfo)
        {
            SetInitalParam();
        }

        private void SetInitalParam()
        {
            Report.MaxAllowedAntennas = 4;
            //Port distribution by antenna.Exapmple First Antenna - 6 port max
            Report.MaxAllowedPorts = new List<int>() { 6, 4, 3, 1 };
            Report.AntennaOffsetSteps = new List<int>() { 1, 7, 11, 14 };
            Report.PortOffsetSteps = new List<int>() { 0, 6, 10, 13 };
    }

        public override ReportType Document
        {
            get
            {
                return ReportType.SRF;
            }
        }
        public override string TemplateFilePath
        {
            get
            {
                string tempPath = Path.GetTempFileName();
                File.WriteAllBytes(tempPath, Properties.Resources.SRF_Template_v1_4);
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
                    "GSM_900",
                    "GSM_900N",
                    "UMTS_900",
                    "UMTS_900N",
                    "LTE_900",
                    "LTE_900N",
                    "GSM_1800",
                    "GSM_1800N",
                    "LTE_1800",
                    "LTE_1800N",
                    "UMTS_2100",
                    "UMTS_2100N",
                    "LTE_2100",
                    "LTE_2100N",
                    "LTE_2600",
                    "LTE_2600N",
                    "NR_1800",
                    "NR_2100",
                    "NR_3500",
                    "NR_1800N",
                    "NR_2100N",
                    "NR_3500N",
                };
            }
        }

        public override string[] ForPrintSectorSheet
        {
            get
            {
                return new string[]
                {
                    "Antenna #",
                    "Pole Current",
                    "Azimuth Current",
                    "AntennaType Current ",
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
                    "RRU_Total # Current",
                    "GSM_TRX Current",
                    "GSM_Pwr_per_TRX Current,dBm",
                    "UMTS_TRX Current",
                    "UMTS_Pwr_per_TRX Current,dBm",
                    "LTE_TRX Current",
                    "LTE_Pwr_per_TRX Current,dBm",
                    "TMA Type Current",
                    "Combiner_Splitter Current",
                    "Sec_Combiner_Splitter Current",
                    "Collocation Current",
                    "Antenna_IN_Total_Power,dBm",
                    "Request_Remarks",
                    "PortName Current",

                };
            }
        }

    
    }
}
