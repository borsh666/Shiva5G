using BLL;
using BLL.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace WebClient
{
    public partial class WebFormClient : System.Web.UI.Page
    {
        public string siteID;
        public const bool isSiteSRAN = true;
        private string resultFilePath;
        private string resultFileName;
        private string valueFromCookie;

        private void GenerateExcel(FileInfo resultFile)
        {

            //Response.ClearHeaders - clears the current header collection
            //Response.Clear - resets the output stream
            //Response.ClearContent call Clear and is just a better name

            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.AddHeader("Content-Disposition", "attachment; filename=" + resultFile.Name);
            Response.AddHeader("Content-Length", resultFile.Length.ToString());
            Response.ContentType = "text/plain";
            //Forces all currently buffered output to be sent to the client.The Flush method can be called multiple times during request processing.
            Response.Flush();
            Response.TransmitFile(resultFile.FullName);

            //Sends all currently buffered output to the client, stops execution of the page, and raises the EndRequest event.
            //Izkarva te ot stranicata 
            Response.End();

        }

        protected void Page_Load(object sender, EventArgs e)
        {

            //check cookie exists or not
            var cook = Request.Cookies["shiva"];
            if (cook != null)
            {
                valueFromCookie = cook.Value;
                TextBoxMailFrom.Text = cook.Value.Split('|').Skip(1).FirstOrDefault();
            }
            else
            {
                Response.Redirect("WebFormLogin.aspx");
            }

            TextBoxNewsBody.Text += Properties.Resources.News;

        }

        protected void ButtonIRFC_Click(object sender, EventArgs e)
        {

            try
            {
                SupportFunc.CheckForCorrectSiteName(ViewState["SiteID"].ToString());

                resultFileName = $"IRFC_SITE_{ViewState["SiteID"].ToString()}.xlsx";
                resultFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ResultFiles", resultFileName);

                var directory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ResultFiles"));
                if (directory.GetFiles().Count() > 100)
                    foreach (FileInfo file in directory.GetFiles())
                        file.Delete();

                var resultFile = new FileInfo(resultFilePath);

                var dto = new DTO_Load_IRFC(ViewState["SiteID"].ToString(), isSiteSRAN);

                var site = dto.Site();

                var report = new ReportIRFC(resultFilePath, valueFromCookie);

                var excelOutput = new ExcelOutput(site, report);

                excelOutput.PopulateExcel();

                excelOutput.InsertDataValidationsAndFormulas(resultFilePath);

                GenerateExcel(resultFile);

            }
            catch (Exception ex)
            {
                ErrorHandle(ex, "IRFC");
            }
        }

        protected void ButtonSA_Click(object sender, EventArgs e)
        {
            try
            {
                SupportFunc.CheckForCorrectSiteName(ViewState["SiteID"].ToString());

                resultFileName = $"SA_{ViewState["SiteID"].ToString()}_{DateTime.Now.Day}_{DateTime.Now.Month}_{DateTime.Now.Year}.xlsx";
                resultFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ResultFiles", resultFileName);

                var directory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ResultFiles"));
                if (directory.GetFiles().Count() > 100)
                    foreach (FileInfo file in directory.GetFiles())
                        file.Delete();

                var resultFile = new FileInfo(resultFilePath);

                var dto = new DTO_Load_SA(ViewState["SiteID"].ToString(), isSiteSRAN);

                var site = dto.Site();

                var report = new ReportSA(resultFilePath, valueFromCookie);

                var excelOutput = new ExcelOutput(site, report);

                excelOutput.PopulateExcel();

                excelOutput.CopyCombinersToExcelSheet(resultFilePath);

                excelOutput.CopyRRUsToExcelSheet(resultFilePath);

                excelOutput.CopyAntennasToExcelSheet(resultFilePath);

                excelOutput.CopyAntennasPortNameToExcelSheet(resultFilePath);

                excelOutput.InsertDataValidationsAndFormulas(resultFilePath);

                GenerateExcel(resultFile);



            }
            catch (Exception ex)
            {
                ErrorHandle(ex, "SA");
            }
        }

        protected void ButtonSRF_Click(object sender, EventArgs e)
        {
           

            try
            {
                SupportFunc.CheckForCorrectSiteName(ViewState["SiteID"].ToString());

                resultFileName = $"SRF_SITE_{ViewState["SiteID"].ToString()}.xlsx";
                resultFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ResultFiles", resultFileName);

                var directory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ResultFiles"));
                if (directory.GetFiles().Count() > 100)
                    foreach (FileInfo file in directory.GetFiles())
                        file.Delete();

                var resultFile = new FileInfo(resultFilePath);

                var dto = new DTO_Load_SRF(ViewState["SiteID"].ToString(), isSiteSRAN);

                var site = dto.Site();

                var report = new ReportSRF(resultFilePath, valueFromCookie);

                var excelOutput = new ExcelOutput(site, report);

                excelOutput.PopulateExcel();

                excelOutput.CopyCombinersToExcelSheet(resultFilePath);

                excelOutput.CopyRRUsToExcelSheet(resultFilePath);

                excelOutput.CopyAntennasToExcelSheet(resultFilePath);

                excelOutput.CopyAntennasPortNameToExcelSheet(resultFilePath);

                excelOutput.InsertDataValidationsAndFormulas(resultFilePath);

                GenerateExcel(resultFile);
            }
            catch (Exception ex)
            {
                ErrorHandle(ex, "SRF");
            }
        }

        protected void ButtonPSK_Click(object sender, EventArgs e)
        {
            try
            {
                SupportFunc.CheckForCorrectSiteName(ViewState["SiteID"].ToString());

                resultFileName = $"PSK_SITE_{ViewState["SiteID"].ToString()}.xlsx";
                resultFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ResultFiles", resultFileName);

                var directory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ResultFiles"));
                if (directory.GetFiles().Count() > 100)
                    foreach (FileInfo file in directory.GetFiles())
                        file.Delete();

                var resultFile = new FileInfo(resultFilePath);

                var dto = new DTO_Load_PSK(ViewState["SiteID"].ToString(), isSiteSRAN);

                var site = dto.Site();

                var report = new ReportPSK(resultFilePath, valueFromCookie);

                var excelOutput = new ExcelOutput(site, report);

                excelOutput.PopulateExcelReportPSK();
                          
                GenerateExcel(resultFile);
            }
            catch (Exception ex)
            {
                ErrorHandle(ex, "PSK");
            }
        }

        private void ErrorHandle(Exception ex, string formType)
        {
            var lstAssembly = new List<string> { "BLL", "WebClient" };

            var errorMsg = SupportFunc.ShowErrorMetodAndMsg(ex, lstAssembly);

            string siteId = string.Empty;

            if (ViewState["SiteID"] == null)
                siteId = " unknown. Please enter site name!";
            else
                siteId = ViewState["SiteID"].ToString();

            TextBoxMailBody.Text = ($"Здравейте,  Има проблем  forma {formType} за сайт {siteId}!{Environment.NewLine}Грешката е : {errorMsg}{Environment.NewLine}{Environment.NewLine}Моля, изпратете този email до New Delhi Team за отстраняване на проблема.{Environment.NewLine}Лека Работа.");
        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {
            this.siteID = TextBox1.Text.Trim();
            ViewState["SiteID"] = this.siteID;
        }

        protected void ButtonSendMail_Click(object sender, EventArgs e)
        {
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.UseDefaultCredentials = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;



            var mail = new MailMessage();
            mail.To.Add("yuliyan.petrov@vivacom.bg");
            mail.To.Add("stefan.velinov@vivacom.bg");

            var from = TextBoxMailFrom.Text;
            if (!Regex.IsMatch(from, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
                mail.From = new MailAddress("SHIVA@anonymous.com");
            else
                mail.From = new MailAddress(from);


            if (TextBoxMailSubject.Text == String.Empty)
                mail.Subject = $"Shiva Problem with site {ViewState["SiteID"].ToString()}";
            else
                mail.Subject = TextBoxMailSubject.Text;


            mail.Body = TextBoxMailBody.Text;

            smtpClient.Send(mail);
            TextBoxMailBody.Text = "The email has been sent!";
            //   Response.Write("The email has been sent");
            //   Response.AddHeader("REFRESH", "2;URL=WebFormClient.aspx");
        }

        protected void ButtonSRF_2600_Click(object sender, EventArgs e)
        {

            try
            {
                SupportFunc.CheckForCorrectSiteName(ViewState["SiteID"].ToString());

                resultFileName = $"SRF_SITE_{ViewState["SiteID"].ToString()}.xlsx";
                resultFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ResultFiles", resultFileName);

                var directory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ResultFiles"));
                if (directory.GetFiles().Count() > 100)
                    foreach (FileInfo file in directory.GetFiles())
                        file.Delete();

                var resultFile = new FileInfo(resultFilePath);

                var dto = new DTO_Load_SRF(ViewState["SiteID"].ToString(), isSiteSRAN);
              
                
                var report = new ReportSRF(resultFilePath, valueFromCookie);
                var reportStaticAnt = new ReportSRF_StaticAnt(resultFilePath);

                var oldSite = dto.Site();
                var excelOutput = new ExcelOutput(oldSite, report);

                var dto_static_ant = new DTO_Load_SRF_StaticAnt(siteID, isSiteSRAN);
                var site_static_ant = dto_static_ant.LoadNewSiteAntenna();

                excelOutput.PopulateExcelWithStaticAnt(site_static_ant, reportStaticAnt);
                excelOutput.InsertDataValidationsAndFormulasSRF_StaticAnt(resultFilePath);

                GenerateExcel(resultFile);
            }
            catch (Exception ex)
            {
                ErrorHandle(ex, "SRF");
            }
        }
    }
}