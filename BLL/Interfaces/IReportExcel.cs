using System.Collections.Generic;

namespace BLL.Interfaces
{
    interface IReportExcel
    {
        string RangeFrom { get; set; }
        string RangeTo { get; set; }
        string TemplateFilePath { get; set; }
        string[] ForPrintCommonSheet { get; set; }
        string[] ForPrintSectorSheet { get; set; }
        string[] ForDataValidation { get; set; }
        //NEW
        string[] WorkSheetsName { get; }

        IDictionary<int,string[]> Ranges { get; set; }
    
    }
}
