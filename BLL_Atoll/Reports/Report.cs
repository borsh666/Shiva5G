using BLL_Atoll.Interfaces;
using BLL_Atoll.Attributes;
using BLL_Atoll.Enums;
using System.Reflection;

namespace BLL_Atoll
{
    public partial class Report : IReportExcel
    {
        [ForPrint("Document_Type")]
        public virtual ReportType Document { get; set; }

        [ForPrint("DateTime")]
        public string Time => DateTime.Now.ToString("yyyy-MM-dd");

        [ForPrint("RF_Engineer")]
        public string RF_Engineer { get; set; }

        [ForPrint("Mobile")]
        public string Mobile { get; set; }

        [ForPrint("Email")]
        public string Email { get; set; }

    }

    public partial class Report
    {
        public Report(string reportPath)
        {
            this.ReportPath = reportPath;
        }

        public const string TEMPLATE_DIRECTORY =  @$"{Global.WORKING_DIRECTORY}Templates\";
        
        public string ReportPath { get; init; }
        public virtual string RangeFrom { get; set; }

        public virtual string RangeTo { get; set; }

        public virtual string TemplateFilePath { get; set; }

        public virtual SheetNames[] WorkSheetsName => new SheetNames[]
            {SheetNames.Common,
                SheetNames.Sector1,
                SheetNames.Sector2,
                SheetNames.Sector3,
                SheetNames.Sector4,
                SheetNames.Sector5,
                SheetNames.Sector6,
                SheetNames.Sector7,
                SheetNames.Sector8,
                SheetNames.Sector9};

        public virtual IDictionary<int, string[]> Ranges { get; set; }

        public const int MaxAllowedSectors = 9;

        public const int MaxAllowedAntennas = 4;

        public virtual int[] AntennaOffsetSteps()
        {
            return new int[MaxAllowedAntennas] { 1, 7, 11, 14 };
        }

        //Port distribution by antenna.Exapmple First Antenna - 6 port max
        public virtual int[] PortOffsetSteps()
        {
            return new int[MaxAllowedAntennas] { 6, 4, 3, 3 };
        }

        public virtual Dictionary<Band, int> BandOffsetSteps()
        {
            return new()
            {
                { Band.B9, 0 },
                { Band.B18, 1 },
                { Band.B21, 2 },
                { Band.B26, 3 },
                { Band.B35, 4 },
            };
        }

        public int OffsetExcel(PropertyInfo propInfo)
        {
            return 1;
        }

        public SheetNames SheetName(PropertyInfo propInfo)
        {
            return SheetNames.Common;
        }

        public static string SetTemplateFilePath(string templateFileName)
        {
                var reportAsBytes = ExcelOutput.ReadExcelFileAsByteArray(
                    Path.Combine(TEMPLATE_DIRECTORY, templateFileName));

                string tempPath = Path.GetTempFileName();
                File.WriteAllBytes(tempPath, reportAsBytes);
                return tempPath;
        }

    }
}
