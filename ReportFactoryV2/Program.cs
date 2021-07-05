using System;
using System.Windows.Forms;

namespace ReportFactoryV2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //string reportPath = @"D:\\test.xlsx";
            //try
            //{

            //    var isSiteRAN = true;
            //    var dto = new DTO_Load_SA_SRF("SF1001", isSiteRAN);
            //    var site = dto.Site();

            //    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //    var report = new ReportSRF(reportPath);

            //    var excelOutput = new ExcelOutput(site, report);

            //    excelOutput.PopulateExcel();
            //    excelOutput.CopyAntennasToExcelSheet(reportPath);
            //    excelOutput.InsertDataValidationsAndFormulas(reportPath);
            //}
            //catch (Exception ex)
            //{

            //    var s = new StackTrace(ex);
            //    var frame = s.GetFrames();

            //    var lstAssemblies = new List<Assembly>()
            //    {
            //        AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == "BLL"),
            //        AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == "DAL_Client"),
            //        AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == "DAL_GSM"),

            //    };
            //    foreach (var thisasm in lstAssemblies)
            //    {
            //        var methodnames = frame.Select(f => f.GetMethod());

            //        foreach (var methodname in methodnames)
            //        {
            //            if (methodname.Module.Assembly == thisasm)
            //            {
            //                Console.WriteLine($"Грешка в метод {methodname.Name} :");
            //                Console.WriteLine(ex.Message);
            //            }

            //        }

            //    }

            //}



            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ReportFactoryV2());
        }
    }
}
