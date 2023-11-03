using BLL_Atoll.Enums;

namespace BLL_Atoll
{
    public class ReportSRF : Report
    {
        public ReportSRF(string reportPath) : base(reportPath)
        {
            base.TemplateFilePath = SetTemplateFilePath(TEMPLATE_FILE_NAME);
        }

        public override ReportType Document => ReportType.SRF;

        public const string TEMPLATE_FILE_NAME = "SRF_Template.xlsm";


        public override string RangeTo => "AD80";
        public override string RangeFrom => "A1";

        public override int[] PortOffsetSteps()
        {
            return new int[MaxAllowedAntennas] { 6, 4, 3, 1 };
        }
    }
}
