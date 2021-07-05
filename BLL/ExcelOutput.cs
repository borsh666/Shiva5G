using BLL.Attributes;
using BLL.DTO;
using BLL.Enums;
using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace BLL
{
    public class ExcelOutput
    {

        private Report report;
        private Site site;

        public ExcelOutput(Site site, Report report)
        {
            this.report = report;
            this.site = site;
        }

        private class DataValid
        {
            public string FieldName { get; set; }
            public string[] FieldDataValidation { get; set; }
            public int HowManyDataValid { get; set; }
        }

        //Намира кординатите на дадено поле в ексел Dictionary<string, int[]>  Key:Име на полето,  Value: кординати
        private Dictionary<string, int[]> FindTextExcel(int worksheetNumber, string[] findWhat, string rangeFrom, string rangeTo)
        {
            //----We get the IDs of all Excel processes existing before creating our new one-----
            List<int> oldExcelIDs = new List<int>();
            Process[] excelProcesses = Process.GetProcessesByName("Excel");
            foreach (Process pro in excelProcesses) { oldExcelIDs.Add(pro.Id); }
            //-----------------------------------------------------------------------------------


            object False = false;
            object True = true;
            var output = new Dictionary<string, int[]>();

            var objApp = new Application();
            var workbooks = objApp.Workbooks;
            var workbook = workbooks.Open(report.TemplateFilePath);


            _Worksheet ws = (_Worksheet)workbook.Worksheets[worksheetNumber];

            Range rngCommon = ws.get_Range(rangeFrom, rangeTo);



            foreach (var find in findWhat)
            {
                Range findRngCommon = rngCommon.Find(find, Missing.Value, XlFindLookIn.xlValues, Missing.Value, Missing.Value, XlSearchDirection.xlNext, False, False, Missing.Value);
                if (findRngCommon != null)
                {
                    var row = findRngCommon.Row;
                    var col = findRngCommon.Column;
                    output.Add(find, new int[] { row, col });
                }
            }

            workbook.Close();
            workbooks.Close();
            objApp.Quit();

            Marshal.ReleaseComObject(workbook);
            Marshal.ReleaseComObject(workbooks);
            Marshal.ReleaseComObject(objApp);

            //--------Take the list of excel processes again and compare the IDs, if the Id is not in the old list is the one we just created, let's kill it!------
            excelProcesses = Process.GetProcessesByName("Excel");
            foreach (Process proc in excelProcesses)
            {
                if (!oldExcelIDs.Contains(proc.Id))
                {
                    try
                    {
                        proc.Kill();
                    }
                    catch { }
                }
            }

            return output;

        }

        private void PopulateValues(ExcelWorksheet ws, Dictionary<string, int[]> lstCoords, IEnumerable<PropertyInfo> properties, object obj, bool isVerticalOffset, int offset)
        {

            foreach (var prop in properties)
            {

                var propIsInDic = lstCoords.Keys.Where(n => n.Contains(prop.Name)).FirstOrDefault();


                if (propIsInDic != null)
                {
                    var x = lstCoords[propIsInDic][0];
                    var y = lstCoords[propIsInDic][1];

                    if (isVerticalOffset)
                    {

                        ws.Cells[x + offset, y].Value = prop.GetValue(obj);
                        ws.Cells[x + offset, y].Style.Locked = true;
                    }
                    else
                    {
                        ws.Cells[x, y + offset].Value = prop.GetValue(obj);
                        ws.Cells[x, y + offset].Style.Locked = true;
                    }

                }
            }
        }

        private void PopulateValuesStaticAnt(ExcelWorksheet ws, Dictionary<string, int[]> lstCoords, IEnumerable<PropertyInfo> properties, object obj, bool isVerticalOffset, int offset)
        {

            foreach (var prop in properties)
            {

                var propIsInDic = lstCoords.Keys.Where(n => n.Contains(prop.Name) && (n.Contains("New") || n.Contains("new"))).FirstOrDefault();

                if (propIsInDic != null)
                {
                    var x = lstCoords[propIsInDic][0];
                    var y = lstCoords[propIsInDic][1];

                    ws.Cells[x, y + offset].Value = prop.GetValue(obj);
                    ws.Cells[x, y + offset].Style.Locked = true;


                }
            }

            if (obj.GetType() == typeof(Port))
            {
                var port = obj as Port;

                ws.Cells[lstCoords["Antenna port #"][0], lstCoords["Antenna port #"][1] + offset].Value = port.PortName;
                ws.Cells[lstCoords["Antenna port #"][0], lstCoords["Antenna port #"][1] + offset].Style.Locked = true;
                ws.Cells[lstCoords["Port_occupancy"][0], lstCoords["Port_occupancy"][1] + offset].Value = port.Band;
                ws.Cells[lstCoords["Port_occupancy"][0], lstCoords["Port_occupancy"][1] + offset].Style.Locked = true;
            }

        }
        
        private static void SetRetExcelFormula(ExcelWorksheet ws, Dictionary<string, int[]> fieldsCoords, string fieldName, string antennaName)
        {
            var retCoordsX = fieldsCoords[fieldName][0];
            var retCoordsY = fieldsCoords[fieldName][1];
            var antennaCoordsX = fieldsCoords[antennaName][0];
            var antennaCoordsY = fieldsCoords[antennaName][1];

            //NEW
            for (int j = 0; j < Report.MaxAllowedAntennas; j++)
            {
                for (int k = 0; k < Report.MaxAllowedPorts[j]; k++)
                {
                    var getCellAdressRet = ws.Cells[retCoordsX, retCoordsY + Report.AntennaOffsetSteps[j] + k].Address;
                    var getCellAdressAntenna = ws.Cells[antennaCoordsX, antennaCoordsY + Report.AntennaOffsetSteps[j]].Address;

                    ws.Cells[getCellAdressRet].Formula = $"IF((OR({getCellAdressAntenna}=\"80010992\", {getCellAdressAntenna}=\"80020872\",{getCellAdressAntenna}=\"5961300\" )), \"FlexRET\", \"\")";
                }
            }
        }
      
        private static void SetBandExcelFormula(ExcelWorksheet ws, Dictionary<string, int[]> fieldsCoords, string fieldName, string antennaName)
        {
            var bandCoordsX = fieldsCoords[fieldName][0];
            var bandCoordsY = fieldsCoords[fieldName][1];
            var antennaCoordsX = fieldsCoords[antennaName][0];
            var antennaCoordsY = fieldsCoords[antennaName][1];

            //NEW
            for (int j = 0; j < Report.MaxAllowedAntennas; j++)
            {
                for (int k = 0; k < Report.MaxAllowedPorts[j]; k++)
                {
                    var getCellAdressBand = ws.Cells[bandCoordsX, bandCoordsY + Report.AntennaOffsetSteps[j] + k].Address;
                    var getCellAdressAntenna = ws.Cells[antennaCoordsX, antennaCoordsY + Report.AntennaOffsetSteps[j]].Address;

                    ws.Cells[getCellAdressBand].Formula = $"VLOOKUP({getCellAdressAntenna},Antennas!A:H,{k + 2},FALSE)";
                }
            }
        }

        private static void SetPortNameExcelFormula(ExcelWorksheet ws, Dictionary<string, int[]> fieldsCoords, string fieldName, string antennaName)
        {
            var bandCoordsX = fieldsCoords[fieldName][0];
            var bandCoordsY = fieldsCoords[fieldName][1];
            var antennaCoordsX = fieldsCoords[antennaName][0];
            var antennaCoordsY = fieldsCoords[antennaName][1];

            //NEW
            for (int j = 0; j < Report.MaxAllowedAntennas; j++)
            {
                for (int k = 0; k < Report.MaxAllowedPorts[j]; k++)
                {
                    var getCellAdressBand = ws.Cells[bandCoordsX, bandCoordsY + Report.AntennaOffsetSteps[j] + k].Address;
                    var getCellAdressAntenna = ws.Cells[antennaCoordsX, antennaCoordsY + Report.AntennaOffsetSteps[j]].Address;

                    ws.Cells[getCellAdressBand].Formula = $"VLOOKUP({getCellAdressAntenna},AntennasPortName!A:H,{k + 2},FALSE)";
                }
            }
        }

        public void CopyAntennasToExcelSheet(string resultFilePath)
        {

            var allAntennas = Queries.GetAllAntennasPortsBands().OrderBy(n => n.AntennaType);

            //Old
            //var pathAntennasFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AntennaPortMapping.txt");
            //var inputFile = File.ReadAllText(pathAntennasFile).Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);


            //Old
            //inputFile = inputFile.OrderBy(n => n).ToArray();

            FileInfo templateFile = new FileInfo(resultFilePath);

            using (ExcelPackage package = new ExcelPackage(templateFile))
            {
                var worksheets = package.Workbook.Worksheets;
                worksheets.Add("Antennas");

                var ws = worksheets.Where(n => n.Name == "Antennas").FirstOrDefault();
                ws.Hidden = eWorkSheetHidden.Hidden;

                int x = 1;
                int y = 1;

                foreach (var item in allAntennas)
                {
                    ws.Cells[y, x].Value = item.AntennaType;
                    x++;

                    var bands = item.BandRange.Split(' ').Select(n => n.Trim()).ToList();

                    foreach (var band in bands)
                    {
                        ws.Cells[y, x].Value = band;
                        x++;
                    }
                    y++;
                    x = 1;
                }

                //Old
                //foreach (var item in inputFile)
                //{
                //    var bands = item.Split('|').Select(n => n.Trim()).ToList();

                //    foreach (var band in bands)
                //    {
                //        ws.Cells[y, x].Value = band;
                //        x++;
                //    }
                //    y++;
                //    x = 1;
                //}

                package.Save();
            }

        }

        public void CopyAntennasPortNameToExcelSheet(string resultFilePath)
        {

            var allAntennas = Queries.GetAllAntennasPortsBands().OrderBy(n => n.AntennaType);


            FileInfo templateFile = new FileInfo(resultFilePath);

            using (ExcelPackage package = new ExcelPackage(templateFile))
            {
                var worksheets = package.Workbook.Worksheets;
                worksheets.Add("AntennasPortName");

                var ws = worksheets.Where(n => n.Name == "AntennasPortName").FirstOrDefault();
                ws.Hidden = eWorkSheetHidden.Hidden;

                int x = 1;
                int y = 1;

                foreach (var item in allAntennas)
                {
                    ws.Cells[y, x].Value = item.AntennaType;
                    x++;

                    var portNames = item.PortName.Split(' ').Select(n => n.Trim()).ToList();

                    foreach (var portName in portNames)
                    {
                        ws.Cells[y, x].Value = portName;
                        x++;
                    }
                    y++;
                    x = 1;
                }
                package.Save();
            }

        }

        public void CopyCombinersToExcelSheet(string resultFilePath)
        {

            var combiners = Properties.Resources.CombinersList.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            FileInfo templateFile = new FileInfo(resultFilePath);

            using (ExcelPackage package = new ExcelPackage(templateFile))
            {
                var worksheets = package.Workbook.Worksheets;
                worksheets.Add("Combiners");

                var ws = worksheets.Where(n => n.Name == "Combiners").FirstOrDefault();
                ws.Hidden = eWorkSheetHidden.Hidden;

                int x = 1;
                int y = 1;

                foreach (var combiner in combiners)
                {
                    ws.Cells[y, x].Value = combiner;
                    y++;
                }
                package.Save();
            }

        }

        public void CopyRRUsToExcelSheet(string resultFilePath)
        {

            var rrus = Properties.Resources.RruList
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            FileInfo templateFile = new FileInfo(resultFilePath);

            using (ExcelPackage package = new ExcelPackage(templateFile))
            {
                var worksheets = package.Workbook.Worksheets;
                worksheets.Add("RRUs");

                var ws = worksheets.Where(n => n.Name == "RRUs").FirstOrDefault();
                ws.Hidden = eWorkSheetHidden.Hidden;

                int x = 1;
                int y = 1;

                foreach (var combiner in rrus)
                {
                    ws.Cells[y, x].Value = combiner;
                    y++;
                }
                package.Save();
            }

        }

        public void PopulateExcel()
        {
            FileInfo templateFile = new FileInfo(report.TemplateFilePath);
            FileInfo newFile = new FileInfo(report.ReportPath);

            try
            {
                using (ExcelPackage package = new ExcelPackage(templateFile))
                {
                    var worksheets = package.Workbook.Worksheets;

                    //Common WorkSheet
                    var wsCommon = worksheets.Where(n => n.Name == report.WorkSheetsName[0]).FirstOrDefault();
                    wsCommon.Cells.Style.Locked = false;

                    //New  Hide all sector's sheets
                    worksheets.Where(n => report.WorkSheetsName.Any(x => x == n.Name && x.Contains("Sector"))).ToList()
                        .ForEach(n => n.Hidden = eWorkSheetHidden.Hidden);

                    //Sector1 WorkSheet
                    var wsSector = worksheets.Where(n => n.Name == report.WorkSheetsName[1]).FirstOrDefault();


                    //Get Coords from Common and Sector1 Sheets
                    var lstCoordsCommon = FindTextExcel(wsCommon.Index, report.ForPrintCommonSheet, report.RangeFrom, report.RangeTo);
                    var lstCoordsSector1 = FindTextExcel(wsSector.Index, report.ForPrintSectorSheet, report.RangeFrom, report.RangeTo);

                    //Reflection for Properties marked with attribute ForPrint
                    var propReport = typeof(Report).GetProperties().Where(n => SupportFunc.HasAttribute(typeof(ForPrintAttribute), n));
                    var propSite = typeof(Site).GetProperties().Where(n => SupportFunc.HasAttribute(typeof(ForPrintAttribute), n));
                    var propSector = typeof(Sector).GetProperties().Where(n => SupportFunc.HasAttribute(typeof(ForPrintAttribute), n));
                    var propAntenna = typeof(Antenna).GetProperties().Where(n => SupportFunc.HasAttribute(typeof(ForPrintAttribute), n));
                    var propPort = typeof(Port).GetProperties().Where(n => SupportFunc.HasAttribute(typeof(ForPrintAttribute), n));



                    //Populate  Common Sheet
                    PopulateValues(wsCommon, lstCoordsCommon, propReport, report, false, report.OffsetExcel);
                    PopulateValues(wsCommon, lstCoordsCommon, propSite, site, false, site.OffsetExcel);
                    foreach (var sector in site.Sectors)
                        PopulateValues(wsCommon, lstCoordsCommon, propSector, sector, true, sector.OffsetExcel);

                    var sectors = site.Sectors.Count();

                    if (site.Sectors.Count() > Report.MaxAllowedSectors)
                        sectors = Report.MaxAllowedSectors;


                    //Populate  Sector's Sheets
                    for (int i = 0; i < sectors; i++)
                    {
                        wsSector = worksheets.Where(n => n.Name == report.WorkSheetsName[i + 1]).FirstOrDefault();

                        //New
                        wsSector.Hidden = eWorkSheetHidden.Visible;

                        wsSector.Cells.Style.Locked = false;

                        //Горе в дясно слагам името на сайта за всеки шиит.
                        wsSector.Cells["A1"].Value = site.SiteID;
                        wsSector.Cells["A1"].Style.Locked = true;

                        foreach (var antenna in site.Sectors[i].Antennas)
                        {
                            PopulateValues(wsSector, lstCoordsSector1, propAntenna, antenna, false, antenna.OffsetExcel);

                            foreach (var port in antenna.Ports)
                            {
                                PopulateValues(wsSector, lstCoordsSector1, propPort, port, false, port.OffsetExcel);
                            }
                        }
                    }

                    package.SaveAs(newFile);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message} {Environment.NewLine}Моля проверете за отворен файл със същото име.");
            }
        }

        public void PopulateExcelReportPSK()
        {
            FileInfo templateFile = new FileInfo(report.TemplateFilePath);
            FileInfo newFile = new FileInfo(report.ReportPath);

            try
            {

                using (ExcelPackage package = new ExcelPackage(templateFile))
                {
                    var worksheets = package.Workbook.Worksheets;

                    //Common WorkSheet
                    var wsCommon = worksheets.Where(n => n.Name == report.WorkSheetsName[0]).FirstOrDefault();
                    wsCommon.Cells.Style.Locked = false;


                    //Get Coords from Common  Sheet
                    var lstCoordsCommon = FindTextExcel(wsCommon.Index, report.ForPrintCommonSheet, report.RangeFrom, report.RangeTo);


                    //Reflection for Properties marked with attribute ForPrint
                    var propReport = typeof(Report).GetProperties().Where(n => SupportFunc.HasAttribute(typeof(ForPrintAttribute), n));
                    var propSite = typeof(Site).GetProperties().Where(n => SupportFunc.HasAttribute(typeof(ForPrintAttribute), n));
                    // var propSector = typeof(Sector).GetProperties().Where(n => SupportFunc.HasAttribute(typeof(ForPrintAttribute), n));
                    var propAntenna = typeof(Antenna).GetProperties().Where(n => SupportFunc.HasAttribute(typeof(ForPrintAttribute), n));
                    var propPort = typeof(Port).GetProperties().Where(n => SupportFunc.HasAttribute(typeof(ForPrintAttribute), n));



                    //Populate  Common Sheet
                    PopulateValues(wsCommon, lstCoordsCommon, propReport, report, false, report.OffsetExcel);
                    PopulateValues(wsCommon, lstCoordsCommon, propSite, site, false, site.OffsetExcel);
                    //foreach (var sector in site.Sectors)
                    //    PopulateValues(wsCommon, lstCoordsCommon, propSector, sector, true, sector.OffsetExcel);

                    var sectors = site.Sectors.Count();

                    if (site.Sectors.Count() > Report.MaxAllowedSectors)
                        sectors = Report.MaxAllowedSectors;


                    //Populate Sectors WorkSheet
                    var wsSector = worksheets.Where(n => n.Name == report.WorkSheetsName[1]).FirstOrDefault();

                    wsSector.Cells.Style.Locked = false;

                    for (int i = 0; i < site.Sectors.Count(); i++)
                    {
                        foreach (var antenna in site.Sectors[i].Antennas)
                        {
                            //Get Coords from  Sectors Sheet
                            var lstCoordsSector = FindTextExcel(wsSector.Index, report.ForPrintSectorSheet, report.Ranges[i + 1].First(), report.Ranges[i + 1].Last());

                            PopulateValues(wsSector, lstCoordsSector, propAntenna, antenna, false, antenna.OffsetExcel);

                            foreach (var port in antenna.Ports)
                            {
                                PopulateValues(wsSector, lstCoordsSector, propPort, port, false, port.OffsetExcel);
                            }
                        }
                    }

                    package.SaveAs(newFile);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message} {Environment.NewLine}Моля проверете за отворен файл със същото име.");
            }
        }

        public void InsertDataValidationsAndFormulasSRF_StaticAnt(string resultFilePath)
        {
            FileInfo templateFile = new FileInfo(resultFilePath);

            var fieldsDataInput = Properties.Resources.ExcelFieldsForValidationSRF_StaticAnt_txt.Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var dataValidLst = new List<DataValid>();


            foreach (var fieldData in fieldsDataInput)
            {
                var split = fieldData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                var dataValid = new DataValid()
                {
                    FieldName = split.First(),
                    FieldDataValidation = split.Skip(1).Take(split.Length - 2).ToArray(),
                    HowManyDataValid = int.Parse(split.Last())
                };
                dataValidLst.Add(dataValid);
            }

            using (ExcelPackage package = new ExcelPackage(templateFile))
            {
                var startSheet = 1;  //First sheet - Common
                var ws = package.Workbook.Worksheets[startSheet];
                var fieldsCoords = FindTextExcel(startSheet, dataValidLst.Select(n => n.FieldName).ToArray(), report.RangeFrom, report.RangeTo);
                //За всяко едно поле
                foreach (var fieldCoords in fieldsCoords)
                {


                    var howManyValid = dataValidLst.Where(n => n.FieldName == fieldCoords.Key)
                    .Select(n => n.HowManyDataValid)
                    .FirstOrDefault();

                    //За всяка една клетка от това поле, на която трябва да се сложи DataValidation
                    for (int j = 1; j <= howManyValid; j++)
                    {
                        var getCellAdress = ws.Cells[fieldCoords.Value[0], fieldCoords.Value[1] + j].Address;
                        var dataValidation = ws.DataValidations.AddListValidation(getCellAdress);

                        dataValidation.ShowErrorMessage = true;
                        dataValidation.ErrorTitle = "An invalid feedback was entered";
                        dataValidation.Error = "Please choose feedback from drop down only.";

                        var currentValidLst = dataValidLst.Where(n => n.FieldName == fieldCoords.Key)
                       .SelectMany(n => n.FieldDataValidation);

                        foreach (var valiData in currentValidLst)
                            dataValidation.Formula.Values.Add(valiData);
                    }
                }


                ws.Protection.AllowSelectLockedCells = true;
                ws.Protection.AllowSelectUnlockedCells = true;

                package.Save();

            }

        }

        public void InsertDataValidationsAndFormulas(string resultFilePath)
        {
            FileInfo templateFile = new FileInfo(resultFilePath);
            var startSheet = 1;  //First sheet - Common
            //NEW
            var endSheet = report.WorkSheetsName.Length;  //Last Sector (9)

            var fieldsDataInput = new List<string>();
            if (report.Document == ReportType.IRFC)
                fieldsDataInput = Properties.Resources.ExcelFieldsForValidationIRFC.Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            else
                fieldsDataInput = Properties.Resources.ExcelFieldsForValidation.Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries).ToList();


            var dataValidLst = new List<DataValid>();


            foreach (var fieldData in fieldsDataInput)
            {
                var split = fieldData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                var dataValid = new DataValid()
                {
                    FieldName = split.First(),
                    FieldDataValidation = split.Skip(1).Take(split.Length - 2).ToArray(),
                    HowManyDataValid = Report.MaxAllowedPorts.Sum()
                };
                dataValidLst.Add(dataValid);
            }


            using (ExcelPackage package = new ExcelPackage(templateFile))
            {
                //За всеки един шиит (от секторите)
                for (int i = startSheet; i <= endSheet; i++)
                {
                    var ws = package.Workbook.Worksheets[i];
                    var fieldsCoords = FindTextExcel(i, dataValidLst.Select(n => n.FieldName).ToArray(), report.RangeFrom, report.RangeTo);
                    //За всяко едно поле
                    foreach (var fieldCoords in fieldsCoords)
                    {

                        if (fieldCoords.Key == "BandRange New" || fieldCoords.Key == "PortName New")
                            continue;

                        var howManyValid = dataValidLst.Where(n => n.FieldName == fieldCoords.Key)
                        .Select(n => n.HowManyDataValid)
                        .FirstOrDefault();


                        //За всяка една клетка от това поле, на която трябва да се сложи DataValidation
                        for (int j = 1; j <= howManyValid; j++)
                        {
                            var getCellAdress = ws.Cells[fieldCoords.Value[0], fieldCoords.Value[1] + j].Address;
                            var dataValidation = ws.DataValidations.AddListValidation(getCellAdress);

                            if (fieldCoords.Key.Contains("RRU_Type"))
                                dataValidation.ShowErrorMessage = false;
                            else
                            {
                                dataValidation.ShowErrorMessage = true;
                                dataValidation.ErrorTitle = "An invalid feedback was entered";
                                dataValidation.Error = "Please choose feedback from drop down only.";
                            }

                            //Добавя DataValidation
                            if (fieldCoords.Key == "AntennaType New" || fieldCoords.Key == "AntennaType found")
                            {
                                dataValidation.Formula.ExcelFormula = "Antennas!A:A";
                            }
                            else if (fieldCoords.Key.Contains("Combiner_Splitter"))
                            {
                                dataValidation.Formula.ExcelFormula = "Combiners!A:A";
                            }
                            else if (fieldCoords.Key.Contains("RRU_Type"))
                            {
                                dataValidation.Formula.ExcelFormula = "RRUs!A:A";
                            }
                            else
                            {
                                var currentValidLst = dataValidLst.Where(n => n.FieldName == fieldCoords.Key)
                               .SelectMany(n => n.FieldDataValidation);

                                foreach (var valiData in currentValidLst)
                                    dataValidation.Formula.Values.Add(valiData);
                            }
                        }
                    }


                    //NEW 
                    if (fieldsCoords.ContainsKey("BandRange New") && fieldsCoords.ContainsKey("AntennaType New"))
                        SetBandExcelFormula(ws, fieldsCoords, "BandRange New", "AntennaType New");
                    if (fieldsCoords.ContainsKey("PortName New") && fieldsCoords.ContainsKey("AntennaType New"))
                        SetPortNameExcelFormula(ws, fieldsCoords, "PortName New", "AntennaType New");

                    if (fieldsCoords.ContainsKey("RET Current"))
                        SetRetExcelFormula(ws, fieldsCoords, "RET Current", "AntennaType current");

                    if (fieldsCoords.ContainsKey("RET New"))
                        SetRetExcelFormula(ws, fieldsCoords, "RET New", "AntennaType New");

                    if (fieldsCoords.ContainsKey("RET Found"))
                        SetRetExcelFormula(ws, fieldsCoords, "RET Found", "AntennaType found");

                    ws.Protection.AllowSelectLockedCells = true;
                    ws.Protection.AllowSelectUnlockedCells = true;
                    //  ws.Protection.IsProtected = true;  //--------Protect whole sheet
                }

                package.Save();

            }

        }

        public void PopulateExcelWithStaticAnt(Site siteStaticAnt, ReportSRF_StaticAnt reportStaticAnt)
        {
            FileInfo templateFile = new FileInfo(report.TemplateFilePath);
            FileInfo newFile = new FileInfo(report.ReportPath);

            try
            {
                using (ExcelPackage package = new ExcelPackage(templateFile))
                {
                    var worksheets = package.Workbook.Worksheets;

                    //Common WorkSheet
                    var wsCommon = worksheets.Where(n => n.Name == report.WorkSheetsName[0]).FirstOrDefault();
                    wsCommon.Cells.Style.Locked = false;

                    //New  Hide all sector's sheets
                    worksheets.Where(n => report.WorkSheetsName.Any(x => x == n.Name && x.Contains("Sector"))).ToList()
                        .ForEach(n => n.Hidden = eWorkSheetHidden.Hidden);

                    //Sector1 WorkSheet
                    var wsSector = worksheets.Where(n => n.Name == report.WorkSheetsName[1]).FirstOrDefault();


                    //Get Coords from Common and Sector1 Sheets
                    var lstCoordsCommon = FindTextExcel(wsCommon.Index, report.ForPrintCommonSheet, report.RangeFrom, report.RangeTo);
                    var lstCoordsSector1 = FindTextExcel(wsSector.Index, report.ForPrintSectorSheet, report.RangeFrom, report.RangeTo);
                    var lstCoordsSectorStaticAnt1 = FindTextExcel(wsSector.Index, reportStaticAnt.ForPrintSectorSheet, reportStaticAnt.RangeFrom, reportStaticAnt.RangeTo);

                    //Reflection for Properties marked with attribute ForPrint
                    var propReport = typeof(Report).GetProperties().Where(n => SupportFunc.HasAttribute(typeof(ForPrintAttribute), n));
                    var propSite = typeof(Site).GetProperties().Where(n => SupportFunc.HasAttribute(typeof(ForPrintAttribute), n));
                    var propSector = typeof(Sector).GetProperties().Where(n => SupportFunc.HasAttribute(typeof(ForPrintAttribute), n));
                    var propAntenna = typeof(Antenna).GetProperties().Where(n => SupportFunc.HasAttribute(typeof(ForPrintAttribute), n));
                    var propPort = typeof(Port).GetProperties().Where(n => SupportFunc.HasAttribute(typeof(ForPrintAttribute), n));



                    //Populate  Common Sheet
                    PopulateValues(wsCommon, lstCoordsCommon, propReport, report, false, report.OffsetExcel);
                    PopulateValues(wsCommon, lstCoordsCommon, propSite, site, false, site.OffsetExcel);

                    foreach (var sector in site.Sectors)
                        PopulateValues(wsCommon, lstCoordsCommon, propSector, sector, true, sector.OffsetExcel);

                    var sectors = site.Sectors.Count();

                    if (site.Sectors.Count() > Report.MaxAllowedSectors)
                        sectors = Report.MaxAllowedSectors;


                    //Populate  Sector's Sheets
                    for (int i = 0; i < sectors; i++)
                    {
                        wsSector = worksheets.Where(n => n.Name == report.WorkSheetsName[i + 1]).FirstOrDefault();

                        //New
                        wsSector.Hidden = eWorkSheetHidden.Visible;

                        wsSector.Cells.Style.Locked = false;

                        //Горе в дясно слагам името на сайта за всеки шиит.
                        wsSector.Cells["A1"].Value = site.SiteID;
                        wsSector.Cells["A1"].Style.Locked = true;

                        if (site.Sectors[i].Antennas.Count == 0)
                            continue;

                        var firstAntPhyIndex = site.Sectors[i].Antennas.First().PhyIndex;

                        foreach (var antenna in site.Sectors[i].Antennas)
                        {
                            PopulateValues(wsSector, lstCoordsSector1, propAntenna, antenna, false, antenna.OffsetExcel);

                            foreach (var port in antenna.Ports)
                            {
                                int offset = port.OffsetExcel;

                                if (firstAntPhyIndex == antenna.PhyIndex)
                                    offset = ReportSRF_StaticAnt.mapOldNewPortNames[port.OffsetExcel];

                                PopulateValues(wsSector, lstCoordsSector1, propPort, port, false, offset);
                            }


                        }

                        //New Site with static antenna
                        foreach (var antenna in siteStaticAnt.Sectors[i].Antennas)
                        {
                            PopulateValuesStaticAnt(wsSector, lstCoordsSectorStaticAnt1, propAntenna, antenna, false, antenna.OffsetExcel);

                            foreach (var port in antenna.Ports)
                                PopulateValuesStaticAnt(wsSector, lstCoordsSectorStaticAnt1, propPort, port, false, port.OffsetExcel);
                        }

                        //Merge Ret row
                        wsSector.Cells["C27:H27"].Merge = true;
                        //Set antennas
                        wsSector.Cells["C8"].Value = siteStaticAnt.Sectors[i].Antennas.First().AntennaType;
                        wsSector.Cells["I8:M8"].Value = "none";
                        //Merge and set RRUs row
                        wsSector.Cells["I33:O33"].Value = "none";
                        wsSector.Cells["I33:O33"].Merge = true;
                    }

                    package.SaveAs(newFile);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message} {Environment.NewLine}Моля проверете за отворен файл със същото име.");
            }
        }
    }
}
