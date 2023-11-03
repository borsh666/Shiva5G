using BLL_Atoll.Attributes;
using BLL_Atoll.Enums;
using BLL_Atoll.Interfaces;
using BLL_Atoll.Models.Site;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Reflection;
using System.Text;


namespace BLL_Atoll
{
    public class ExcelOutput
    {
        private class ExcelField
        {
            public PropertyInfo ModelPropInfo { get; internal set; }
            public int[] ExcelFieldCoord { get; init; }
            public SheetNames ExcelSheetName { get; internal set; }
            public bool NeedValidation { get; internal set; }
            public List<string> DropDown { get; internal set; }
            public string Name { get; internal set; }
            public string ExcelFullAddress { get; internal set; }
        }

        //Get data from elements
        private Dictionary<string, PropertyInfo> GetAllAttrPropInfo()
        {
            List<PropertyInfo> GetPropForPrint<T>()
            {
                return typeof(T)
                .GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                .Where(n => SupportFunc.HasAttribute(typeof(ForPrintAttribute), n)).ToList();
            }
            List<PropertyInfo> reportProp = GetPropForPrint<Report>();
            List<PropertyInfo> siteProp = GetPropForPrint<Site>();
            List<PropertyInfo> sectorProp = GetPropForPrint<Sector>();
            List<PropertyInfo> antennaProp = GetPropForPrint<Antenna>();
            List<PropertyInfo> portProp = GetPropForPrint<Port>();
            List<PropertyInfo> bandProp = GetPropForPrint<BandGrouping>();
            List<PropertyInfo> rruProp = GetPropForPrint<Rru>();

            var propsInfo = reportProp.Concat(siteProp).Concat(sectorProp)
                    .Concat(antennaProp);

            if (Engine.Report.Document == ReportType.PSK)
                propsInfo = propsInfo.Concat(bandProp);
            else
                propsInfo = propsInfo.Concat(portProp).Concat(rruProp);


            var attrPropInfo = new Dictionary<string, PropertyInfo>();

            foreach (var propInfo in propsInfo)
            {
                var forPrintAttr = (ForPrintAttribute)propInfo.GetCustomAttribute(typeof(ForPrintAttribute));

                if (forPrintAttr != null)
                {
                    attrPropInfo.Add($"{forPrintAttr.Name}", propInfo);

                    foreach (string str in Enum.GetNames(typeof(NewCurrentFound)))
                        attrPropInfo.Add($"{forPrintAttr.Name} {str}", propInfo);
                }
            }
            return attrPropInfo;
        }

        //private Dictionary<string, List<string>> GetDropDowns()
        //{
        //    var dropDowns = Properties.Resource.DropDowns;
        //    string jsonStr = Encoding.UTF8.GetString(dropDowns);

        //    return JsonConvert.DeserializeObject<Dictionary<String, List<string>>>(jsonStr)!;
        //}

        private static Dictionary<string, List<string>> GetDropDowns()
        {
            byte[] dropDowns = File.ReadAllBytes(Global.DROP_DOWNS);

            string jsonStr = Encoding.UTF8.GetString(dropDowns);

            return JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonStr)!;
        }

        private List<ExcelField> CreateExcelFieldsFromElements(ExcelPackage excelPackage)
        {
            var excelFieldsDic = new Dictionary<string, ExcelField>();

            var allAttrPropInfo = GetAllAttrPropInfo();

            foreach (var sheetEnum in Engine.Report.WorkSheetsName)
            {

                var sheetStr = SupportFunc.GetEnumDisplayName(sheetEnum);

                ExcelWorksheet sheet = excelPackage.Workbook
                    .Worksheets[sheetStr];

                foreach (var excelCellName in allAttrPropInfo.Keys.ToArray())
                {
                    var excelFields = GetExcelCoordFromString(sheet, excelCellName, Engine.Report.RangeFrom, Engine.Report.RangeTo);
                    foreach (var excelField in excelFields)
                    {
                        excelField.ExcelSheetName = sheetEnum;
                        excelField.ModelPropInfo = allAttrPropInfo[excelCellName];
                        excelField.Name = excelCellName;
                        excelFieldsDic.Add($"{sheet} {excelCellName} {excelField.ExcelFullAddress} {excelField}", excelField);

                        if (Engine.Report.Document == ReportType.SRF || Engine.Report.Document == ReportType.SA)
                        {
                            if (excelField.Name.Contains(NewCurrentFound.New.ToString()) ||
                                excelField.Name.Contains(NewCurrentFound.Found.ToString()))
                            {
                                excelField.NeedValidation = true;
                            }
                        }
                    }
                }
            }
            return excelFieldsDic.Values.ToList(); ;
        }

        private List<ExcelField> CreateExcelFieldsFromDropDown(ExcelPackage excelPackage)
        {
            var excelFieldsDic = new Dictionary<string, ExcelField>();

            var dropDownsMap = GetDropDowns();

            foreach (var sheetEnum in Engine.Report.WorkSheetsName)
            {
                var sheetStr = SupportFunc.GetEnumDisplayName(sheetEnum);

                ExcelWorksheet sheet = excelPackage.Workbook
                    .Worksheets[sheetStr];

                foreach (var item in dropDownsMap)
                {
                    var excelFields = GetExcelCoordFromString(sheet, item.Key, Engine.Report.RangeFrom, Engine.Report.RangeTo);

                    foreach (var excelField in excelFields)
                    {
                        excelField.ExcelSheetName = sheetEnum;
                        excelField.NeedValidation = true;
                        excelField.DropDown = item.Value;
                        excelFieldsDic.Add($"{sheet} {item.Key} {excelField.ExcelFullAddress} {excelField}", excelField);
                    }
                }
            }
            return excelFieldsDic.Values.ToList();
        }

        private List<ExcelField> GetExcelCoordFromString(ExcelWorksheet sheet, string excelCell, string searchFrom, string searchTo)
        {
            var excelFields = new List<ExcelField>();

            var excelCellsName =
                         from cell in sheet.Cells[$"{searchFrom}:{searchTo}"]
                         where cell.Value?.ToString()?.Trim() == excelCell
                         select cell;

            if (excelCellsName.Any())
                foreach (var excelCellName in excelCellsName)
                {
                    var excelField = new ExcelField
                    {
                        ExcelFieldCoord = new[] { excelCellName!.Start.Row,
                                              excelCellName!.Start.Column },
                        Name = excelCell,
                        ExcelFullAddress = excelCellName.FullAddress
                    };
                    excelFields.Add(excelField);
                }
            return excelFields;
        }

        private void PopulateValues<T>(ExcelWorksheets sheets, List<ExcelField> excelFields, T obj)
        where T : IExcel
        {
            foreach (var excelField in excelFields)
            {

                if (excelField.ModelPropInfo.DeclaringType!.FullName != typeof(T).ToString())
                    continue;
                if (excelField.ExcelSheetName != obj.SheetName(excelField.ModelPropInfo))
                    continue;
                if (excelField.NeedValidation)
                    continue;

                var sheet = sheets.Where(n => n.Name == SupportFunc.GetEnumDisplayName(excelField.ExcelSheetName))
                    .FirstOrDefault()!;

                sheet.Hidden = eWorkSheetHidden.Visible;

                var x = excelField.ExcelFieldCoord[0];
                var y = excelField.ExcelFieldCoord[1];

                var isVerticalOffset = excelField.ModelPropInfo
                    .GetCustomAttribute(typeof(VerticalOffsetAttribute));

                var propValue = excelField.ModelPropInfo.GetValue(obj);

                var offset = obj.OffsetExcel(excelField.ModelPropInfo);


                if (isVerticalOffset != null)
                {
                    sheet.Cells[x + offset, y].Value = propValue;
                    sheet.Cells[x + offset, y].Style.Locked = true;
                }
                else
                {
                    sheet.Cells[x, y + offset].Value = propValue;
                    sheet.Cells[x, y + offset].Style.Locked = true;
                }

            }
        }

        private void PopulateValuesSheetSectorsPSK<T>(ExcelWorksheets sheets, List<ExcelField> excelFields, T obj)
            where T : IExcel, ISector
        {

            foreach (var excelField in excelFields)
            {
                if (excelField.ModelPropInfo.DeclaringType!.FullName != typeof(T).ToString())
                    continue;
                if (excelField.ExcelSheetName != SheetNames.Sectors)
                    continue;

                var sheet = sheets.Where(n => n.Name == SheetNames.Sectors.ToString())
                    .FirstOrDefault()!;

                //if (excelField.Name == "Etilt")
                //    Console.WriteLine();

                var x = excelField.ExcelFieldCoord[0];
                var y = excelField.ExcelFieldCoord[1];

                var sectorId = sheet.Cells[x, y - 1].Value;

                if (sectorId is null || int.Parse(sectorId.ToString()) != obj.SectorId)
                    continue;

                var isVerticalOffset = excelField.ModelPropInfo
                    .GetCustomAttribute(typeof(VerticalOffsetAttribute));

                var propValue = excelField.ModelPropInfo.GetValue(obj);

                var offset = obj.OffsetExcel(excelField.ModelPropInfo);

                sheet.Cells[x, y + offset].Value = propValue;
                sheet.Cells[x, y + offset].Style.Locked = true;
            }
        }

        private static void SetBandAndPortRangeExcelFormula(ExcelWorksheet ws, ExcelField excelField, List<string> antennaCellAdress)
        {
            var bandCoordsX = excelField.ExcelFieldCoord[0];
            var bandCoordsY = excelField.ExcelFieldCoord[1];

            for (int j = 0; j < Report.MaxAllowedAntennas; j++)
            {
                var cellAdressAntenna = antennaCellAdress[Engine.Report.AntennaOffsetSteps()[j] - 1];
                for (int k = 0; k < Engine.Report.PortOffsetSteps()[j]; k++)
                {
                    var offcet = Engine.Report.AntennaOffsetSteps()[j];
                    var getCellAdressBand = ws.Cells[bandCoordsX, bandCoordsY + offcet + k].Address;

                    if (excelField.Name == "BandRange New")
                        ws.Cells[getCellAdressBand].Formula = $"VLOOKUP({cellAdressAntenna},Antennas!A:H,{k + 2},FALSE)";
                    if (excelField.Name == "PortName New")
                        ws.Cells[getCellAdressBand].Formula = $"VLOOKUP({cellAdressAntenna},AntennasPortName!A:H,{k + 2},FALSE)";
                }
            }
        }

        // New sheets creations
        private void CopyAntennasBandRangeToExcelSheet(ref ExcelWorksheets worksheets, List<dynamic> antennaPortMap)
        {
            worksheets.Add("Antennas");
            var ws = worksheets.Where(n => n.Name == "Antennas").First();
            ws.Hidden = eWorkSheetHidden.Hidden;

            var antennaPortMapDic = antennaPortMap.ToList()
                .OrderBy(n => n.PORTID)
                .GroupBy(n => (string)n.ANTENNATYPE)
                .ToDictionary(g => g.Key.ToString(), g => g.Select(k => k.BANDRANGE));

            ws.Hidden = eWorkSheetHidden.Hidden;

            antennaPortMapDic.Add("FOR DISMANTLING", new List<dynamic>() { "NOVAL" });


            int x = 1;
            int y = 1;

            foreach (var map in antennaPortMapDic)
            {
                ws.Cells[y, x].Value = map.Key;
                x++;

                foreach (var band in map.Value)
                {
                    ws.Cells[y, x].Value = band;
                    x++;
                }
                y++;
                x = 1;
            }
        }

        private void CopyAntennasPortNameToExcelSheet(ref ExcelWorksheets worksheets, List<dynamic> antennaPortMap)
        {
            worksheets.Add("AntennasPortName");
            var ws = worksheets.Where(n => n.Name == "AntennasPortName").FirstOrDefault();
            ws.Hidden = eWorkSheetHidden.Hidden;

            var antennaPortMapDic = antennaPortMap.ToList()
             .GroupBy(n => (string)n.ANTENNATYPE)
             .ToDictionary(g => g.Key.ToString(), g => g.Select(k => k.PORTTYPE));

            antennaPortMapDic.Add("FOR DISMANTLING", new List<dynamic>() { "NOVAL" });

            int x = 1;
            int y = 1;

            foreach (var map in antennaPortMapDic)
            {
                ws.Cells[y, x].Value = map.Key;
                x++;

                foreach (var portName in map.Value)
                {
                    ws.Cells[y, x].Value = portName;
                    x++;
                }
                y++;
                x = 1;
            }
        }

        private void CopyRRUsAndCombinersToExcelSheet(ref ExcelWorksheets worksheets)
        {
            var sheetNameDropDownName = new Dictionary<string, string>
            {
                { "RRUs", "RRU_Type New" },
                { "Combiners", "Combiner_Splitter New" }
            };

            var dropDownsMap = GetDropDowns();

            foreach (var item in sheetNameDropDownName)
            {
                worksheets.Add(item.Key);

                var ws = worksheets.Where(n => n.Name == item.Key).First();
                ws.Hidden = eWorkSheetHidden.Hidden;

                int x = 1;
                int y = 1;

                if (dropDownsMap.ContainsKey(item.Value))
                {
                    foreach (var combiner in dropDownsMap[item.Value])
                    {
                        ws.Cells[y, x].Value = combiner;
                        y++;
                    }
                }
            }
        }

        private void InsertDataValidationsAndFormulas(ref ExcelWorksheets worksheets, List<ExcelField> excelFields)
        {

            var howManyDataValid = Engine.Report.AntennaOffsetSteps().Last() +
                                   Engine.Report.PortOffsetSteps().Last();

            var excelFieldsForValidation = excelFields
                .Where(n => n.NeedValidation)
                .OrderBy(n => n.ExcelSheetName)
                .ThenBy(n => n.Name)
                .ToList();

            var antennaCellAdress = new List<string>();

            foreach (var excelField in excelFieldsForValidation)
            {
                var ws = worksheets.Where(n => n.Name == SupportFunc.GetEnumDisplayName(excelField.ExcelSheetName))
                    .FirstOrDefault();

                if (ws is not null)
                {

                    //За всяка една клетка от това поле, на която трябва да се сложи DataValidation
                    for (int offset = 1; offset <= howManyDataValid; offset++)
                    {
                        if (excelField.Name == "BandRange New" || excelField.Name == "PortName New")
                        {
                            SetBandAndPortRangeExcelFormula(ws, excelField, antennaCellAdress);
                            continue;
                        }

                        var x = excelField.ExcelFieldCoord[0];
                        var y = excelField.ExcelFieldCoord[1];
                        var getCell = ws.Cells[x, y + offset];
                        var getCellAdress = getCell.Address;

                        var dataValidation = ws.DataValidations.AddListValidation(getCellAdress);

                        dataValidation.ShowErrorMessage = true;
                        dataValidation.ErrorTitle = "An invalid feedback was entered";
                        dataValidation.Error = "Please choose feedback from drop down only.";

                        if (excelField.Name == "AntennaType New" || excelField.Name == "AntennaType Found")
                        {
                            dataValidation.Formula.ExcelFormula = "Antennas!A:A";

                            //Copy Antenna from AntennaType Current to Antenna New 03.04.2023
                            var getUpperCell = ws.Cells[x - 1, y + offset]; //AntennaType Current 
                            if (getUpperCell.Value != null)
                                getCell.Value = getUpperCell.Value.ToString();

                            antennaCellAdress.Add(getCellAdress);
                        }
                        else if (excelField.Name.Contains("Combiner_Splitter"))
                        {
                            dataValidation.Formula.ExcelFormula = "Combiners!A:A";
                        }
                        else if (excelField.Name.Contains("RRU_Type"))
                        {
                            dataValidation.ShowErrorMessage = false;
                            dataValidation.Formula.ExcelFormula = "RRUs!A:A";
                        }
                        else
                        {
                            if (excelField.DropDown is not null)
                                foreach (var valiData in excelField.DropDown)
                                    dataValidation.Formula.Values.Add(valiData);
                        }
                    }
                }
                else
                    continue;
            }
        }

        public MemoryStream PopulateExcel(List<Site> site, List<Sector> sectors, List<Antenna> antennas,
                                 List<Port> ports, List<BandGrouping> bands, List<dynamic> antennaPortMap)
        {
            var templateFile = new FileInfo(Engine.Report.TemplateFilePath);
            //var newFile = new FileInfo(Engine.report.ReportPath);

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using ExcelPackage package = new(templateFile);
                var worksheets = package.Workbook.Worksheets;

                var excelFields = CreateExcelFieldsFromElements(package);

                void PopElement<T>(List<T> elements) where T : IExcel
                {
                    if (elements.Any())
                        foreach (var element in elements)
                            PopulateValues(worksheets, excelFields, element);
                }

                void PopElementPSK<T>(List<T> elements) where T : IExcel, ISector
                {
                    if (elements.Any())
                        foreach (var element in elements)
                            PopulateValuesSheetSectorsPSK(worksheets, excelFields, element);
                }

                PopElement(new List<Report> { Engine.Report });
                PopElement(site);

                if (Engine.Report.Document == ReportType.PSK)
                {
                    PopElementPSK(sectors);
                    PopElementPSK(antennas);
                    PopElementPSK(bands);
                }
                else
                {
                    PopElement(sectors);
                    PopElement(antennas);
                    PopElement(ports);
                }

                if (Engine.Report.Document == ReportType.SRF || Engine.Report.Document == ReportType.SA)
                {
                    CopyAntennasBandRangeToExcelSheet(ref worksheets, antennaPortMap);
                    CopyAntennasPortNameToExcelSheet(ref worksheets, antennaPortMap);
                    CopyRRUsAndCombinersToExcelSheet(ref worksheets);
                    excelFields = CreateExcelFieldsFromDropDown(package);
                    InsertDataValidationsAndFormulas(ref worksheets, excelFields);
                }

                var fileStream = new MemoryStream();
                package.SaveAs(fileStream);

                package.SaveAs(
                    Path.Combine(Global.EXPORT_EXCEL_FILE_DIR, Engine.Report.ReportPath));

                return fileStream;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public static void CopySheetToAnotherExcel(string sourceSheetName, string destSheetName,Stream inputFileStream, string destinationFilePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Clear the content of "Student" sheet in the destination file
            using (ExcelPackage destinationPackage = new(new FileInfo(destinationFilePath)))
            {
                ExcelWorksheet reportSheet = destinationPackage.Workbook.Worksheets[destSheetName];

                if (reportSheet != null)
                    reportSheet.Cells.Clear();
                    //destinationPackage.Workbook.Worksheets.Delete(destSheetName);

                destinationPackage.Save();
            }

            // Copy the first sheet from the input Excel file to the destination file
            using (ExcelPackage inputPackage = new(inputFileStream))
            using (ExcelPackage destinationPackage = new(new FileInfo(destinationFilePath)))
            {
                ExcelWorksheet sourceSheet = inputPackage.Workbook.Worksheets[sourceSheetName];
                //ExcelWorksheet destinationSheet = destinationPackage.Workbook.Worksheets.Add(destSheetName);
                ExcelWorksheet destinationSheet = destinationPackage.Workbook.Worksheets[destSheetName];

                for (int rowNum = 1; rowNum <= sourceSheet.Dimension.Rows; rowNum++)
                {
                    for (int colNum = 1; colNum <= sourceSheet.Dimension.Columns; colNum++)
                    {
                        destinationSheet.Cells[rowNum, colNum].Value = sourceSheet.Cells[rowNum, colNum].Value;
                    }
                }

                destinationPackage.Save();
            }
        }

        public static byte[] ReadExcelFileAsByteArray(string filePath)
        {
            using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read);
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, (int)fileStream.Length);
            return bytes;
        }

    }
}
