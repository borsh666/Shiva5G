// See https://aka.ms/new-console-template for more information
using BLL_Atoll;

//var allSites = File.ReadAllLines(@"D:\Projects\SV\Shiva_5G_Atoll\Shiva_5G\TestConsoleNet6\Resources\AtollAllSites.txt")
//    .Distinct();

//var allSites = TestConsoleNet6.Properties.Resources.sites_txt.Split(Environment.NewLine);


//foreach (var site in allSites)
//{
//    var engine = new Engine(site.Trim(), new ReportSRF($"SRF_{site}_{Global.CurrentTime}.xlsx"));
//    Engine.Cells.Clear();
//    Engine.Errs.Clear();
//    engine.Start();

//    if (Engine.Errs.Any())
//        Console.WriteLine(site);
//}

//MA4094 SO1904
var siteID = "VT5074";
//var engine = new Engine(siteID, new ReportSRF($"ReportSRF{siteID}.xlsm"));
var engine = new Engine(siteID, new ReportIRFC($"ReportIRFC{siteID}.xlsx"));

engine.Start(out string critError);
Console.WriteLine(critError);



