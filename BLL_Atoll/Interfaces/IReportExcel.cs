using BLL_Atoll.Enums;

namespace BLL_Atoll.Interfaces
{
    interface IReportExcel:IExcel
    {
        string RangeFrom { get; set; }
        string RangeTo { get; set; }
        string TemplateFilePath { get; set; }
        SheetNames[] WorkSheetsName { get; }
        IDictionary<int,string[]> Ranges { get; set; }
    
    }
}
