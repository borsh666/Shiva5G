using BLL_Atoll.Attributes;
using BLL_Atoll.Interfaces;
using BLL_Atoll.Enums;
using System.Reflection;
using static BLL_Atoll.SupportFunc;


namespace BLL_Atoll.Models.Site
{
    public partial record Port : SectorAntStatus, IPort, IExcel
    {
        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        [ForPrint("Technology")]
        public string Tech { get; set; }

        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        [ForPrint("PortName")]
        public string PortType { get; init; }

        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        [ForPrint("BandRange")]
        public string BandRange { get; init; }

        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        [ForPrint("RRU_Type")]
        public string RruType { get; set; }

        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        [ForPrint("RRU_Total #")]
        public int RruCount { get; set; }

        [Report(new[] { ReportType.IRFC })]
        [ForPrint("GSM_TRX")]
        public int GsmTrx { get; set; }

        [Report(new[] { ReportType.IRFC })]
        [ForPrint("UMTS_TRX")]
        public int UmtsTrx { get; set; }

        [Report(new[] { ReportType.IRFC })]
        [ForPrint("LTE_TRX")]
        public int LteTrx { get; set; }

        [Report(new[] { ReportType.IRFC })]
        [ForPrint("NR_TRX")]
        public int NrTrx { get; set; }

        [Report(new[] { ReportType.IRFC })]
        [ForPrint("GSM_Pwr_per_TRX, dBm New")]
        public string GsmPowerTrx { get; set; }

        [Report(new[] { ReportType.IRFC })]
        [ForPrint("UMTS_Pwr_per_TRX, dBm New")]
        public string UmtsPowerTrx { get; set; }

        [Report(new[] { ReportType.IRFC })]
        [ForPrint("LTE_Pwr_per_TRX, dBm New")]
        public string LtePowerTrx { get; set; }

        [Report(new[] { ReportType.IRFC })]
        [ForPrint("NR_Pwr_per_TRX, dBm New")]
        public string NrPowerTrx { get; set; }

        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        [ForPrint("Etilt")]
        public decimal? Etilt { get; set; }

        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        [ForPrint("RET")]
        public string RET { get; init; }

        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        [ForPrint("Feeder_Type")]
        public string Feeder_Type { get; set; }

        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        [ForPrint("Feeder_Length")]
        public decimal Feeder_Length { get; init; }

        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        [ForPrint("TMA Type")]
        public string TMA { get; init; }

        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        [ForPrint("Combiner_Splitter")]
        public string Combiner_Splitter { get; init; }

        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        [ForPrint("Sec_Combiner_Splitter")]
        public string Sec_Combiner_Splitter { get; init; }

        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        [ForPrint("Collocation party")]
        public string Collocation { get; init; }

        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        [ForPrint("Antenna_IN_Total_Power, dBm")]
        public double Antenna_IN_Total_Power { get; set; }
    }

    public partial record Port
    {

        private List<double>? powerWithFeederLossWatt;
        private List<double>? powerWithFeederLossDbm;
        public int PortId { get; init; }
        public List<Band> Bands { get; set; }

        private List<Cell> FilterCellsBySecAntPort()
        {
            var filterCells = Engine.Cells
                .Where(n => n.SectorId == SectorId
                            && n.AntennaId == AntennaId
                            && n.PortId == PortId)
                .OrderBy(n => n.CellName);

            return filterCells.ToList();
        }

        private List<Cell> FilterCellsGroupByPortCellName()
        {
            // CellName Level
            var filterCellGropByPortCellName = FilterCellsBySecAntPort()
                .GroupBy(n => new { n.SectorId, n.AntennaId, n.PortId, n.CellName })
                .Select(n => n.First()).ToList();

            return filterCellGropByPortCellName;
        }

        public void GetTech()
        {
            var filterCellGropByPortCellName = FilterCellsGroupByPortCellName();
            var techs = filterCellGropByPortCellName.Select(n => n.Technology.ToString()).Distinct().ToList();
            Tech = string.Join(" ", techs);
        }

        public void GetBand()
        {
            var filterCellGropByPortCellName = FilterCellsGroupByPortCellName();
            Bands = filterCellGropByPortCellName.Select(n => (Band)n.Band!).Distinct().ToList();
        }

        public void GetFeederType()
        {
            Feeder_Type = String.Join(";",
        FilterCellsBySecAntPort()
              .Select(n => n.Feeder_Type)
              .Distinct().ToList());
        }

        public void CalcTrxPowerAndFeederLost()
        {
            //if (SectorId == 1 && AntennaId == 11 && PortId == 1)
            //    Console.WriteLine();
            Status = Status.Occupied;

            // CellName Level
            var filterCellGropByPortCellName = FilterCellsGroupByPortCellName();

            int SumTrx(Technology tech)
            {
                var trx = filterCellGropByPortCellName
                   .Where(n => n.Technology == tech)
                   .Select(n => n.Trx());

                return trx.Sum();
            }


            List<double> FilterPowersPerTech(Technology tech)
            {
                var powers = filterCellGropByPortCellName.Where(n => n.Technology == tech)
                                  .Select(n => Math.Round(n.TrxPower, 2));

                if (powers.Any())
                    return powers.ToList();
                else
                    return new List<double>();
            }

            GsmTrx = SumTrx(Technology.G);
            UmtsTrx = SumTrx(Technology.U);
            LteTrx = SumTrx(Technology.L);
            NrTrx = SumTrx(Technology.NR);

            var gsmPowerTrxDbm = FilterPowersPerTech(Technology.G);
            var umtsPowerTrxDbm = FilterPowersPerTech(Technology.U);
            var ltePowerTrxDbm = FilterPowersPerTech(Technology.L);
            var nrPowerTrxDbm = FilterPowersPerTech(Technology.NR);

            GsmPowerTrx = string.Join(";", gsmPowerTrxDbm);
            UmtsPowerTrx = string.Join(";", umtsPowerTrxDbm);
            LtePowerTrx = string.Join(";", ltePowerTrxDbm);
            NrPowerTrx = string.Join(";", nrPowerTrxDbm);


            powerWithFeederLossDbm = filterCellGropByPortCellName
                .Where(n => n.TrxPower > 0)
                .Select(n => n.TrxPower - n.Feeder_Att_dB).ToList();


            powerWithFeederLossWatt = filterCellGropByPortCellName
                .Where(n => n.TrxPower > 0)
                .Select(n => n.TrxPower - n.Feeder_Att_dB)
                .Select(n => ConvertdBm_W(n)).ToList();

        }

        public int OffsetExcel(PropertyInfo propInfo)
        {
            var portOffset = Engine.Report.AntennaOffsetSteps()[(AntennaId % 10) - 1] + PortId - 1;
            return portOffset;
        }

        public SheetNames SheetName(PropertyInfo propInfo)
        {
            if (Engine.Report.Document != ReportType.PSK)
            {
                return Engine.Report.WorkSheetsName
                    .Where(n => n.ToString().Contains(SectorId.ToString()))
                    .FirstOrDefault();
            }
            else
                return SheetNames.Sectors;
        }

        public void CalcAntennaInTotalPower(List<Rru> rrus)
        {
            var isPortHasRruInComb = rrus
               .Where(n => n.SectorId == SectorId
                           && n.AntennaId == AntennaId
                           && n.PortId == PortId)
               .Any(n => n.IsRruInComb());

            var powerWatt = powerWithFeederLossWatt.Sum();

            if ((int)Bands.Min() > 900 && isPortHasRruInComb)
                powerWatt /= 2;

            Antenna_IN_Total_Power = ConvertW_dBm(powerWatt) -
                                                     CombinerSplitterLossDb(this.Combiner_Splitter) -
                                                     CombinerSplitterLossDb(this.Sec_Combiner_Splitter);


        }

        public void AddRru5519etException(List<Rru> rrus)
        {
            
            if (this.Status == Status.Free ||
                Engine.Report.Document == ReportType.PSK)
                return;

            var group = rrus
                    .Where(n => n.SectorId == SectorId &&
                                        n.AntennaId == AntennaId)
                    .GroupBy(n => n.CellName)
                    .Select(n => new
                    {
                        CellNamesUnique = n.Select(n => n.CellName).Distinct().Count(),
                        CellNamesCount = n.Select(n => n.CellName).Count(),
                        Ports = n.Select(n => n.PortId).ToList(),
                    })
                    .Where(n => n.CellNamesUnique < n.CellNamesCount &&
                        RruType.Contains("5519et"));


            if (group is not null && group.Any())
            {
                var firstRruInGroup = group.First();
                if (firstRruInGroup.Ports.Count == 2)
                {
                    if (this.PortId == firstRruInGroup.Ports.First())
                        this.RruType = "5519etAB";
                    if (this.PortId == firstRruInGroup.Ports.Last())
                    {
                        this.GsmTrx = 0; this.GsmPowerTrx = String.Empty;
                        this.UmtsTrx = 0; this.UmtsPowerTrx = String.Empty;
                        this.LteTrx = 0; this.LtePowerTrx = String.Empty;
                        this.NrTrx = 0; this.UmtsPowerTrx = String.Empty;
                        this.RruType = "5519etCD";
                        this.RruCount = 0;
                        this.Antenna_IN_Total_Power = 0;
                    }
                }
            }

        }

        public void CalcRruTypeAndCount(List<Rru> rrus)
        {
            //if (AntennaId == 11 && PortId == 6)
            //Console.WriteLine($"{SectorId}{AntennaId}{PortId}");
            var rruReuse = "REUSE";

            if (Engine.Report.Document == ReportType.PSK)
                return;
            else if (Engine.Report.Document == ReportType.IRFC)
            {
                var group = rrus
                    .Where(n => n.SectorId == SectorId
                                      && n.AntennaId == AntennaId
                                      && n.PortId == PortId)
                    .GroupBy(n => new { n.SectorId, n.AntennaId, n.PortId, n.CellName })
                    .Select(n => string.Join(" ", n.Select(n => n.RruType()).Distinct()));

                RruCount = group.Where(n => !n.Contains(rruReuse)).Count();

                RruType = String.Join(" ", group.Distinct())
                          .Replace(rruReuse, string.Empty);
            }
            else
            {
                var filterRRUs = rrus
                    .Where(n => n.SectorId == SectorId
                    && n.AntennaId == AntennaId
                    && n.PortId == PortId
                    && n.RruType() != rruReuse);

                if (!filterRRUs.Any())
                {
                    Engine.Errs[SectorId].Add("There is no RruType in CM for AntennaId:{AntennaId} and PortId:{PortId}");
                    Status = Status.NotOk;
                    return;
                }

                var group = filterRRUs
                .GroupBy(n => new { n.SectorId, n.AntennaId, n.PortId })
                .First().ToList();

                RruCount = group.Select(n => n.RruSN).Distinct().Count();

                if (RruCount == 1) //Catch 3926 or 3936-9
                    RruType = group.Select(n => n.RruType()).First();
                else if (group.Any(n => n.IsRruInComb())) //Catch 5304AC 5304DB
                    RruType = string.Join(" ", group.Select(n => n.RruType()).Distinct());
                else
                {
                    var uniqueRruType = group.Select(n => n.RruType()).Distinct().Count();
                    var uniqueRruSn = group.Select(n => n.RruSN).Distinct().Count();
                    var groupByRruType = group
                        .GroupBy(n => n.RruType())
                        .Select(n => new
                        {
                            RruType = n.Key,
                            RruCount = n.Select(k => k.RruSN).Distinct().Count()
                        });

                    if (uniqueRruSn == uniqueRruType)  //Catch 3953-18 3953_21
                        RruType = string.Join(" ", group.Select(n => n.RruType()).Distinct());
                    else     //Catch 3953-18-2х3826 
                    {
                        var rruLst = new List<string>();

                        foreach (var rru in groupByRruType)
                            if (rru.RruCount > 1)
                                rruLst.Add($"{rru.RruCount}x{rru.RruType}");
                            else
                                rruLst.Add(rru.RruType);

                        RruType = string.Join(" ", rruLst);
                    }
                }
            }
        }

        public override void Errors()
        {
            var errs = new List<string>();

            if (Status == Status.Occupied)
            {
                if (Feeder_Type is null)
                {
                    Feeder_Type = String.Empty;
                    errs.Add($"Sec {SectorId} Ant {AntennaId} Port {PortId} doesn't have Feeder_Name!");
                }

            }

            var antNumber = AntennaId % 10;
            var maxAllowedPort = Engine.Report.PortOffsetSteps()[antNumber - 1];

            if (antNumber <= Report.MaxAllowedAntennas && PortId > maxAllowedPort)
            {
                Status = Status.NotOk;
                //errs.Add($"PortId {PortId} is not visiable cos max allowed ports for antenna {AntennaId} is {maxAllowedPort}");
            }
            else
                Status = Status.Ok;

            Engine.Errs[SectorId].AddRange(errs);
        }
    }
}