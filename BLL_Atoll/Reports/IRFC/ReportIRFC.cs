using BLL_Atoll.Enums;

namespace BLL_Atoll
{
    public class ReportIRFC:Report
    {
        public ReportIRFC(string reportPath) : base(reportPath)
        {
            base.TemplateFilePath = SetTemplateFilePath(TEMPLATE_FILE_NAME);
        }

        public override ReportType Document => ReportType.IRFC;

        private readonly string TEMPLATE_FILE_NAME = "IRFC_Template.xlsx";

        public override string RangeTo => "Y80";
        public override string RangeFrom => "A1";

    }
}
