using BLL.Enums;
using System.IO;
using System.Collections.Generic;

namespace BLL.DTO
{
    public class ReportPSK : Report
    {


        public ReportPSK(string reportPath) : base(reportPath)
        {
        }

        public ReportPSK(string reportPath, string userinfo) : base(reportPath, userinfo)
        {
        }

        public override ReportType Document
        {
            get
            {
                return ReportType.PSK;
            }
        }
        public override string TemplateFilePath
        {
            get
            {
                string tempPath = Path.GetTempFileName();
                File.WriteAllBytes(tempPath, Properties.Resources.PSK__Template_v1_4);
                base.TemplateFilePath = tempPath;
                return base.TemplateFilePath;
            }


        }
        public override string RangeTo
        {
            get
            {
                return "Y150";
            }
        }

        public override string RangeFrom
        {
            get
            {
                return "A1";
            }
        }

        //Sector Ranges in Excel
        public override IDictionary<int, string[]> Ranges
        {
            get
            {
                var step = 0;
                var ranges = new Dictionary<int, string[]>();

                for (int i = 1; i <= Report.MaxAllowedSectors; i++)
                {
                    ranges.Add(i, new[] { $"A{step+1}", $"N{step + 14}" });
                    step += 16;
                }

                return ranges;
            }

        }

        public override string[] WorkSheetsName
        {
            get
            {
                return new string[]
                { "Common", "Sectors" };
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
                 };
            }
        }

        public override string[] ForPrintSectorSheet
        {
            get
            {
                return new string[]
                {
                    "Azimuth",
                    "AntennaType",
                    "AGL[m]",
                    "MechanicalTilt",
           //         "Band",
                    "Technology",
                    "Etilt",
                    "GSM_Pwr_ToC,dBm",
                    "UMTS_Pwr_ToC,dBm",
                    "LTE_Pwr_ToC,dBm",
                    "Antenna_IN_Total_Power,W",
                    "Remarks",
                };
            }
        }

        //900,1800,2100,2600,3500
        public const int NumberOfBandsPerAntenna = 5;


    }
}