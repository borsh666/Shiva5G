using BLL_Atoll.Enums;

namespace BLL_Atoll
{
    public class ReportSA : Report
    {
        public ReportSA(string reportPath) : base(reportPath)
        {
            base.TemplateFilePath = SetTemplateFilePath(TEMPLATE_FILE_NAME);
        }


        public override ReportType Document => ReportType.SA;

        private readonly string TEMPLATE_FILE_NAME = "SA_Template.xlsx";

        public override string RangeTo => "Y80";
        public override string RangeFrom => "A1";

        public override int[] PortOffsetSteps()
        {
            return new int[MaxAllowedAntennas] { 6, 4, 3, 1 };
        }
    }
}
