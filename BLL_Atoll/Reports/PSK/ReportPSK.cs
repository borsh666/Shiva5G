using BLL_Atoll.Enums;

namespace BLL_Atoll
{
    public class ReportPSK : Report
    {
        public ReportPSK(string reportPath) : base(reportPath)
        {
            base.TemplateFilePath = SetTemplateFilePath(TEMPLATE_FILE_NAME);
        }

        public override ReportType Document => ReportType.PSK;

        private readonly string TEMPLATE_FILE_NAME = "PSK_Template.xlsx";

        public override string RangeTo => "Y150";


        public override string RangeFrom => "A1";

        //Sector Ranges in Excel
        public override IDictionary<int, string[]> Ranges
        {
            get
            {
                var step = 0;
                var ranges = new Dictionary<int, string[]>();

                for (int i = 1; i <= Report.MaxAllowedSectors; i++)
                {
                    ranges.Add(i, new[] { $"A{step + 1}", $"N{step + 14}" });
                    step += 16;
                }

                return ranges;
            }

        }

        public override SheetNames[] WorkSheetsName => new SheetNames[]
            {SheetNames.Common,
             SheetNames.Sectors };

        ////900,1800,2100,2600,3500
        //public const int NumberOfBandsPerAntenna = 5;

        public override int[] AntennaOffsetSteps()
        {
            return new int[Report.MaxAllowedAntennas] { 1, 6, 11, 16 };
        }

    }
}