using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using BLL;
using BLL.DTO;
using System.Collections.Generic;
using System.Linq;

namespace ReportFactoryV2
{


    public partial class ReportFactoryV2 : Form
    {

        private string resultFilePath;

        private string siteID;

        
        public ReportFactoryV2()
        {
            this.BackgroundImage = Properties.Resources.shiva391x480;
            InitializeComponent();

            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;

            progressBar1.Visible = true;
            progressBar1.Value = 0;
        }


        private void IRFC_Click(object sender, EventArgs e)
        {
            textBoxPerformance.Text = "Start" + Environment.NewLine;

            try
            {
                SupportFunc.CheckForCorrectSiteName(this.siteID);

                Cursor.Current = Cursors.WaitCursor;

                saveFileDialog1.FileName = $"IRFC_SITE_" + siteID;
                saveFileDialog1.ShowDialog();
                resultFilePath = saveFileDialog1.FileName;

                labelCurrentLoading.Visible = true;

                labelCurrentLoading.Text = "DTO_Load";
                var dto = new DTO_Load_IRFC(siteID);
                progressBar1.Value = 30;

                foreach (var item in dto.Info)
                    textBoxPerformance.Text += $"Querie: {item.Key} Time: {item.Value}{Environment.NewLine}";

                labelCurrentLoading.Text = "Site_Load";
                var site = dto.Site();
                progressBar1.Value = 60;

                labelCurrentLoading.Text = "Report_Load";
                var report = new ReportIRFC(resultFilePath);
                progressBar1.Value = 70;

                labelCurrentLoading.Text = "ExcelOutput";
                var excelOutput = new ExcelOutput(site, report);
                progressBar1.Value = 80;



                labelCurrentLoading.Text = "PopulateExcel";
                excelOutput.PopulateExcel();
                progressBar1.Value = 90;

                labelCurrentLoading.Text = "DataValidations";
                excelOutput.InsertDataValidationsAndFormulas(resultFilePath);
                progressBar1.Value = 100;

            }
            catch (Exception ex)
            {
                var lstAssembly = new List<string> { "BLL", "ReportFactoryV2", "DAL_GSM" };

                var errorMsg = SupportFunc.ShowErrorMetodAndMsg(ex, lstAssembly);
                MessageBox.Show(errorMsg);
                //  MessageBox.Show(ex.InnerException.Message);
                return;
            }


            var myProcess = Process.Start(resultFilePath);
            progressBar1.Visible = false;
            labelCurrentLoading.Visible = false;




        }

        private void SA_Click(object sender, EventArgs e)
        {
            //this.siteID = "VT5488";

            try
            {
                SupportFunc.CheckForCorrectSiteName(this.siteID);

                Cursor.Current = Cursors.WaitCursor;

                saveFileDialog1.FileName = $"SA_SITE_" + siteID;
                saveFileDialog1.ShowDialog();
                resultFilePath = saveFileDialog1.FileName;

                labelCurrentLoading.Visible = true;

                labelCurrentLoading.Text = "DTO_Load";
                var dto = new DTO_Load_SA(siteID);
                progressBar1.Value = 30;

                labelCurrentLoading.Text = "Site_Load";
                var site = dto.Site();
                progressBar1.Value = 50;

                foreach (var item in dto.Info)
                    textBoxPerformance.Text += $"Querie: {item.Key} Time: {item.Value}{Environment.NewLine}";

                labelCurrentLoading.Text = "Report_Load";
                var report = new ReportSA(resultFilePath);
                progressBar1.Value = 55;

                labelCurrentLoading.Text = "ExcelOutput";
                var excelOutput = new ExcelOutput(site, report);
                progressBar1.Value = 60;

                labelCurrentLoading.Text = "PopulateExcel";
                excelOutput.PopulateExcel();
                progressBar1.Value = 75;

                labelCurrentLoading.Text = "CopyCombinersToExcelSheet";
                excelOutput.CopyCombinersToExcelSheet(resultFilePath);
                progressBar1.Value = 80;

                labelCurrentLoading.Text = "CopyRRUsToExcelSheet";
                excelOutput.CopyRRUsToExcelSheet(resultFilePath);
                progressBar1.Value = 85;

                labelCurrentLoading.Text = "CopyAntennasToExcelSheet";
                excelOutput.CopyAntennasToExcelSheet(resultFilePath);
                progressBar1.Value = 90;

                labelCurrentLoading.Text = "CopyAntennasPortNameToExcelSheet";
                excelOutput.CopyAntennasPortNameToExcelSheet(resultFilePath);
                progressBar1.Value = 95;

                labelCurrentLoading.Text = "DataValidations";
                excelOutput.InsertDataValidationsAndFormulas(resultFilePath);
                progressBar1.Value = 100;



            }
            catch (Exception ex)
            {
                var lstAssembly = new List<string> { "BLL", "ReportFactoryV2", "DAL_GSM" };

                var errorMsg = SupportFunc.ShowErrorMetodAndMsg(ex, lstAssembly);
                MessageBox.Show(errorMsg);
                return;
            }


            var myProcess = Process.Start(resultFilePath);
            progressBar1.Visible = false;
            labelCurrentLoading.Visible = false;

        }

        private void SRF_Click(object sender, EventArgs e)
        {
            try
            {
                SupportFunc.CheckForCorrectSiteName(this.siteID);

                Cursor.Current = Cursors.WaitCursor;

                saveFileDialog1.FileName = $"SRF_SITE_" + siteID;
                saveFileDialog1.ShowDialog();
                resultFilePath = saveFileDialog1.FileName;

                labelCurrentLoading.Visible = true;

                labelCurrentLoading.Text = "DTO_Load";
                var dto = new DTO_Load_SRF(siteID);
                progressBar1.Value = 30;

                labelCurrentLoading.Text = "Site_Load";
                var site = dto.Site();
                progressBar1.Value = 50;

                foreach (var item in dto.Info)
                    textBoxPerformance.Text += $"Querie: {item.Key} Time: {item.Value}{Environment.NewLine}";

                labelCurrentLoading.Text = "Report_Load";
                var report = new ReportSRF(resultFilePath);
                progressBar1.Value = 55;

                labelCurrentLoading.Text = "ExcelOutput";
                var excelOutput = new ExcelOutput(site, report);
                progressBar1.Value = 60;

                labelCurrentLoading.Text = "PopulateExcel";
                excelOutput.PopulateExcel();
                progressBar1.Value = 75;

                labelCurrentLoading.Text = "CopyCombinersToExcelSheet";
                excelOutput.CopyCombinersToExcelSheet(resultFilePath);
                progressBar1.Value = 80;

                labelCurrentLoading.Text = "CopyRRUsToExcelSheet";
                excelOutput.CopyRRUsToExcelSheet(resultFilePath);
                progressBar1.Value = 85;

                labelCurrentLoading.Text = "CopyAntennasToExcelSheet";
                excelOutput.CopyAntennasToExcelSheet(resultFilePath);
                progressBar1.Value = 90;

                labelCurrentLoading.Text = "CopyAntennasPortNameToExcelSheet";
                excelOutput.CopyAntennasPortNameToExcelSheet(resultFilePath);
                progressBar1.Value = 95;

                labelCurrentLoading.Text = "DataValidations";
                excelOutput.InsertDataValidationsAndFormulas(resultFilePath);
                progressBar1.Value = 100;

            }
            catch (Exception ex)
            {
                var lstAssembly = new List<string> { "BLL", "ReportFactoryV2", "DAL_GSM" };

                var errorMsg = SupportFunc.ShowErrorMetodAndMsg(ex, lstAssembly);
                MessageBox.Show(errorMsg);
                return;
            }


            var myProcess = Process.Start(resultFilePath);
            progressBar1.Visible = false;
            labelCurrentLoading.Visible = false;



        }

        private void PSK_Click(object sender, EventArgs e)
        {
            textBoxPerformance.Text = "Start" + Environment.NewLine;

            // this.siteID = "SF1001";

            try
            {
                SupportFunc.CheckForCorrectSiteName(this.siteID);

                Cursor.Current = Cursors.WaitCursor;

                saveFileDialog1.FileName = $"PSK_SITE_" + siteID;
                saveFileDialog1.ShowDialog();
                resultFilePath = saveFileDialog1.FileName;

                labelCurrentLoading.Visible = true;

                labelCurrentLoading.Text = "DTO_Load";
                var dto = new DTO_Load_PSK(siteID);
                progressBar1.Value = 30;

                foreach (var item in dto.Info)
                    textBoxPerformance.Text += $"Querie: {item.Key} Time: {item.Value}{Environment.NewLine}";


                labelCurrentLoading.Text = "Site_Load";
                var site = dto.Site();
                progressBar1.Value = 60;

                labelCurrentLoading.Text = "Report_Load";
                var report = new ReportPSK(resultFilePath);
                progressBar1.Value = 70;

                labelCurrentLoading.Text = "ExcelOutput";
                var excelOutput = new ExcelOutput(site, report);
                progressBar1.Value = 80;

                labelCurrentLoading.Text = "PopulateExcel";
                excelOutput.PopulateExcelReportPSK();
                progressBar1.Value = 100;

            }
            catch (Exception ex)
            {
                var lstAssembly = new List<string> { "BLL", "ReportFactoryV2", "DAL_GSM" };

                var errorMsg = SupportFunc.ShowErrorMetodAndMsg(ex, lstAssembly);
                MessageBox.Show(errorMsg);
                //  MessageBox.Show(ex.InnerException.Message);
                return;
            }


            var myProcess = Process.Start(resultFilePath);
            progressBar1.Visible = false;
            labelCurrentLoading.Visible = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.siteID = textBoxSiteID.Text.Trim();

        }

        private void buttonAntiInterferin_Click(object sender, EventArgs e)
        {
            string tempAI_Path = Path.GetTempFileName();
            string tempAI_DirPath = Path.GetDirectoryName(tempAI_Path);
            try
            {
                foreach (string f in Directory.EnumerateFiles(tempAI_DirPath, "tmp*.exe"))
                    File.Delete(f);
            }
            catch (Exception )
            {
                //MessageBox.Show("Имате вече отворен Anti Interferin или WO Creator!");
                //return;
            }
            File.Delete(tempAI_DirPath + "\\Oracle.ManagedDataAccess.dll");
            File.WriteAllBytes(tempAI_Path, Properties.Resources.Anti_Interferin_SV);
            File.Move(tempAI_Path, tempAI_Path.Replace(".tmp", ".exe"));

            File.Copy(Directory.GetCurrentDirectory() + "\\Oracle.ManagedDataAccess.dll", tempAI_DirPath + "\\Oracle.ManagedDataAccess.dll");
            var myProcess = Process.Start(tempAI_Path.Replace(".tmp", ".exe"));
        }

        private void buttonWOCreator_Click(object sender, EventArgs e)
        {

            string tempWO_Path = Path.GetTempFileName();
            string tempWO_DirPath = Path.GetDirectoryName(tempWO_Path);
            try
            {
                foreach (string f in Directory.EnumerateFiles(tempWO_DirPath, "tmp*.exe"))
                    File.Delete(f);
            }
            catch (Exception )
            {
                //MessageBox.Show("Имате вече отворен WO Creator или AntiInterferin!");
                //return;
            }

            File.WriteAllBytes(tempWO_Path, Properties.Resources.WO_Creator);
            File.Move(tempWO_Path, tempWO_Path.Replace(".tmp", ".exe"));
            var myProcess = Process.Start(tempWO_Path.Replace(".tmp", ".exe"));
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                this.BackgroundImage = null;
                this.buttonAntennaTest.Show();
                this.buttonManySA.Show();
                this.labelManySA.Show();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void buttonAntennaTest_Click(object sender, EventArgs e)
        {

            //  var testForm = new TestForm();
            //testForm.Show();
        }


        private void textBoxPerformance_TextChanged(object sender, EventArgs e)
        {

        }


        private void buttonManyIRFC_Click(object sender, EventArgs e)
        {
            openFileDialogManyIRFC.ShowDialog();
            var filePath = openFileDialogManyIRFC.FileName;
            var sites = File.ReadAllLines(filePath);

            foreach (var siteID in sites)
            {
                try
                {
                    SupportFunc.CheckForCorrectSiteName(siteID);

                    Cursor.Current = Cursors.WaitCursor;

                    resultFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ManyIRFC", $"IRFC_SITE_" + siteID + ".xlsx");

                    labelCurrentLoading.Visible = true;

                    labelCurrentLoading.Text = "DTO_Load";
                    var dto = new DTO_Load_IRFC(siteID);
                    progressBar1.Value = 30;

                    labelCurrentLoading.Text = "Site_Load";
                    var site = dto.Site();
                    progressBar1.Value = 50;

                    labelCurrentLoading.Text = "Report_Load";
                    var report = new ReportIRFC(resultFilePath);
                    progressBar1.Value = 55;

                    labelCurrentLoading.Text = "ExcelOutput";
                    var excelOutput = new ExcelOutput(site, report);
                    progressBar1.Value = 60;

                    labelCurrentLoading.Text = "PopulateExcel";
                    excelOutput.PopulateExcel();
                    progressBar1.Value = 75;

                    labelCurrentLoading.Text = "CopyAntennasToExcelSheet";
                    excelOutput.CopyAntennasToExcelSheet(resultFilePath);
                    progressBar1.Value = 90;

                    labelCurrentLoading.Text = "DataValidations";
                    excelOutput.InsertDataValidationsAndFormulas(resultFilePath);
                    progressBar1.Value = 100;



                }
                catch (Exception ex)
                {
                    var lstAssembly = new List<string> { "BLL", "ReportFactoryV2", "DAL_GSM" };

                    var errorMsg = SupportFunc.ShowErrorMetodAndMsg(ex, lstAssembly);
                    MessageBox.Show(errorMsg);
                    return;
                }


                progressBar1.Visible = false;
                labelCurrentLoading.Visible = false;
            }
        }

        private void buttonManySA_Click(object sender, EventArgs e)
        {
            openFileDialogManySA.ShowDialog();
            var filePath = openFileDialogManySA.FileName;
            var sites = File.ReadAllLines(filePath);

            foreach (var siteID in sites)
            {
                try
                {
                    SupportFunc.CheckForCorrectSiteName(siteID);

                    Cursor.Current = Cursors.WaitCursor;

                    resultFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ManySA", $"SA_SITE_" + siteID + ".xlsx");

                    labelCurrentLoading.Visible = true;

                    labelCurrentLoading.Text = "DTO_Load";
                    var dto = new DTO_Load_SA(siteID);
                    progressBar1.Value = 30;

                    labelCurrentLoading.Text = "Site_Load";
                    var site = dto.Site();
                    progressBar1.Value = 50;

                    labelCurrentLoading.Text = "Report_Load";
                    var report = new ReportSA(resultFilePath);
                    progressBar1.Value = 55;

                    labelCurrentLoading.Text = "ExcelOutput";
                    var excelOutput = new ExcelOutput(site, report);
                    progressBar1.Value = 60;

                    labelCurrentLoading.Text = "PopulateExcel";
                    excelOutput.PopulateExcel();
                    progressBar1.Value = 75;

                    labelCurrentLoading.Text = "CopyAntennasToExcelSheet";
                    excelOutput.CopyAntennasToExcelSheet(resultFilePath);
                    progressBar1.Value = 80;

                    labelCurrentLoading.Text = "DataValidations";
                    excelOutput.InsertDataValidationsAndFormulas(resultFilePath);
                    progressBar1.Value = 100;



                }
                catch (Exception ex)
                {
                    var lstAssembly = new List<string> { "BLL", "ReportFactoryV2", "DAL_GSM" };

                    var errorMsg = SupportFunc.ShowErrorMetodAndMsg(ex, lstAssembly);
                    MessageBox.Show(errorMsg);
                    return;
                }


                progressBar1.Visible = false;
                labelCurrentLoading.Visible = false;
            }

        }

        private void buttonManyPSK_Click(object sender, EventArgs e)
        {
            openFileDialogManyPSK.ShowDialog();
            var filePath = openFileDialogManyPSK.FileName;
            var sites = File.ReadAllLines(filePath);

            foreach (var siteID in sites)
            {
                try
                {
                    SupportFunc.CheckForCorrectSiteName(siteID);

                    Cursor.Current = Cursors.WaitCursor;

                    resultFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ManyPSK", $"PSK_SITE_" + siteID + ".xlsx");

                    labelCurrentLoading.Visible = true;

                    labelCurrentLoading.Text = "DTO_Load";
                    var dto = new DTO_Load_PSK(siteID);
                    progressBar1.Value = 30;

                    foreach (var item in dto.Info)
                        textBoxPerformance.Text += $"Querie: {item.Key} Time: {item.Value}{Environment.NewLine}";


                    labelCurrentLoading.Text = "Site_Load";
                    var site = dto.Site();
                    progressBar1.Value = 60;

                    labelCurrentLoading.Text = "Report_Load";
                    var report = new ReportPSK(resultFilePath);
                    progressBar1.Value = 70;

                    labelCurrentLoading.Text = "ExcelOutput";
                    var excelOutput = new ExcelOutput(site, report);
                    progressBar1.Value = 80;

                    labelCurrentLoading.Text = "PopulateExcel";
                    excelOutput.PopulateExcelReportPSK();
                    progressBar1.Value = 100;



                }
                catch (Exception ex)
                {
                    var lstAssembly = new List<string> { "BLL", "ReportFactoryV2", "DAL_GSM" };

                    var errorMsg = SupportFunc.ShowErrorMetodAndMsg(ex, lstAssembly);
                    MessageBox.Show(errorMsg);
                    return;
                }


                progressBar1.Visible = false;
                labelCurrentLoading.Visible = false;
            }
        }

        private void buttonManySRF_Click(object sender, EventArgs e)
        {
            openFileDialogManySA.ShowDialog();
            var filePath = openFileDialogManySA.FileName;
            var sites = File.ReadAllLines(filePath);

            foreach (var siteID in sites)
            {
                try
                {
                    SupportFunc.CheckForCorrectSiteName(siteID);

                    Cursor.Current = Cursors.WaitCursor;

                    resultFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ManySRF", $"SRF_SITE_" + siteID + ".xlsx");

                    labelCurrentLoading.Visible = true;

                    labelCurrentLoading.Text = "DTO_Load";
                    var dto = new DTO_Load_SRF(siteID);
                    progressBar1.Value = 30;

                    labelCurrentLoading.Text = "Site_Load";
                    var site = dto.Site();
                    progressBar1.Value = 50;

                    labelCurrentLoading.Text = "Report_Load";
                    var report = new ReportSRF(resultFilePath);
                    progressBar1.Value = 55;

                    labelCurrentLoading.Text = "ExcelOutput";
                    var excelOutput = new ExcelOutput(site, report);
                    progressBar1.Value = 60;

                    labelCurrentLoading.Text = "PopulateExcel";
                    excelOutput.PopulateExcel();
                    progressBar1.Value = 75;

                    labelCurrentLoading.Text = "CopyCombinersToExcelSheet";
                    excelOutput.CopyCombinersToExcelSheet(resultFilePath);
                    progressBar1.Value = 80;

                    labelCurrentLoading.Text = "CopyAntennasToExcelSheet";
                    excelOutput.CopyAntennasToExcelSheet(resultFilePath);
                    progressBar1.Value = 90;

                    labelCurrentLoading.Text = "DataValidations";
                    excelOutput.InsertDataValidationsAndFormulas(resultFilePath);
                    progressBar1.Value = 100;

                }
                catch (Exception ex)
                {
                    var lstAssembly = new List<string> { "BLL", "ReportFactoryV2", "DAL_GSM" };

                    var errorMsg = SupportFunc.ShowErrorMetodAndMsg(ex, lstAssembly);
                    MessageBox.Show(errorMsg);
                    return;
                }


                progressBar1.Visible = false;
                labelCurrentLoading.Visible = false;
            }
        }
             

        private void SRF_StaticAnt_Click_1(object sender, EventArgs e)
        {
            try
            {


                SupportFunc.CheckForCorrectSiteName(this.siteID);

                //this.siteID = "SF1072";
                //this.siteID = "PD2054";


                Cursor.Current = Cursors.WaitCursor;

                saveFileDialog1.FileName = $"SRF_SITE_" + siteID;
                saveFileDialog1.ShowDialog();
                resultFilePath = saveFileDialog1.FileName;

                labelCurrentLoading.Visible = true;

                labelCurrentLoading.Text = "DTO_Load";
                var dto = new DTO_Load_SRF(siteID);
                progressBar1.Value = 30;

                labelCurrentLoading.Text = "Site_Load";
                var oldSite = dto.Site();
                progressBar1.Value = 50;
                                            

                var dto_static_ant = new DTO_Load_SRF_StaticAnt(siteID);
                var site_static_ant = dto_static_ant.LoadNewSiteAntenna();
                progressBar1.Value = 70;

                foreach (var item in dto_static_ant.Info)
                    textBoxPerformance.Text += $"Querie: {item.Key} Time: {item.Value}{Environment.NewLine}";

                labelCurrentLoading.Text = "Report_Load";
                var report = new ReportSRF(resultFilePath);
                var reportStaticAnt = new ReportSRF_StaticAnt(resultFilePath);
                progressBar1.Value = 75;

                labelCurrentLoading.Text = "ExcelOutput";
                var excelOutput = new ExcelOutput(oldSite, report);
                progressBar1.Value = 80;

                labelCurrentLoading.Text = "PopulateExcel";
                excelOutput.PopulateExcelWithStaticAnt(site_static_ant, reportStaticAnt);
                progressBar1.Value = 90;

                labelCurrentLoading.Text = "DataValidations";
                excelOutput.InsertDataValidationsAndFormulasSRF_StaticAnt(resultFilePath);
                progressBar1.Value = 100;


            }
            catch (Exception ex)
            {
                var lstAssembly = new List<string> { "BLL", "ReportFactoryV2", "DAL_GSM" };

                var errorMsg = SupportFunc.ShowErrorMetodAndMsg(ex, lstAssembly);
                MessageBox.Show(errorMsg);
                return;
            }


            var myProcess = Process.Start(resultFilePath);
            progressBar1.Visible = false;
            labelCurrentLoading.Visible = false;
        }

        private void buttonManySRF_2600_Click(object sender, EventArgs e)
        {
            openFileDialogManySA.ShowDialog();
            var filePath = openFileDialogManySA.FileName;
            var sites = File.ReadAllLines(filePath);

            foreach (var siteID in sites)
            {
                try
                {
                    SupportFunc.CheckForCorrectSiteName(siteID);

                    Cursor.Current = Cursors.WaitCursor;

                    resultFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ManySRF", $"SRF_SITE_" + siteID + ".xlsx");

                    labelCurrentLoading.Visible = true;

                    labelCurrentLoading.Text = "DTO_Load";
                    var dto = new DTO_Load_SRF(siteID);
                    progressBar1.Value = 30;

                    labelCurrentLoading.Text = "Site_Load";
                    var oldSite = dto.Site();
                    progressBar1.Value = 50;


                    var dto_static_ant = new DTO_Load_SRF_StaticAnt(siteID);
                    var site_static_ant = dto_static_ant.LoadNewSiteAntenna();
                    progressBar1.Value = 70;

                    foreach (var item in dto_static_ant.Info)
                        textBoxPerformance.Text += $"Querie: {item.Key} Time: {item.Value}{Environment.NewLine}";

                    labelCurrentLoading.Text = "Report_Load";
                    var report = new ReportSRF(resultFilePath);
                    var reportStaticAnt = new ReportSRF_StaticAnt(resultFilePath);
                    progressBar1.Value = 75;

                    labelCurrentLoading.Text = "ExcelOutput";
                    var excelOutput = new ExcelOutput(oldSite, report);
                    progressBar1.Value = 80;

                    labelCurrentLoading.Text = "PopulateExcel";
                    excelOutput.PopulateExcelWithStaticAnt(site_static_ant, reportStaticAnt);
                    progressBar1.Value = 85;

                    labelCurrentLoading.Text = "DataValidations";
                    excelOutput.InsertDataValidationsAndFormulasSRF_StaticAnt(resultFilePath);
                    progressBar1.Value = 100;

                }
                catch (Exception ex)
                {
                    var lstAssembly = new List<string> { "BLL", "ReportFactoryV2", "DAL_GSM" };

                    var errorMsg = SupportFunc.ShowErrorMetodAndMsg(ex, lstAssembly);
                    MessageBox.Show(errorMsg);
                    return;
                }


                progressBar1.Visible = false;
                labelCurrentLoading.Visible = false;
            }
        }
    }
}
