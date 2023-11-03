using BLL_Atoll.Attributes;
using BLL_Atoll.Interfaces;
using BLL_Atoll.Enums;
using System.Reflection;
using static BLL_Atoll.SupportFunc;

namespace BLL_Atoll.Models.Site
{
    public partial class BandGrouping : ISector, IAntenna, IExcel, IStatus
    {
        [Report(new[] { ReportType.PSK })]
        [ForPrint("Technology")]
        public string Tech { get; set; }

        [Report(new[] { ReportType.PSK })]
        [ForPrint("Etilt")]
        public decimal Etilt { get; set; }

        [Report(new[] { ReportType.PSK })]
        [ForPrint("Antenna_IN_Total_Power,W")]
        public double Antenna_IN_Total_Power { get; set; }
    }
    public partial class BandGrouping
    {
        public int PortId { get; init; }
        public int AntennaId { get; set; }
        public int SectorId { get; init; }
        public Band Band { get; set; }
        public Status Status { get; set; }
        public decimal Feeder_Length { get; set; }
        public string? Feeder_Name { get; set; }
        public string Combiner_Splitter { get; init; }
        public string Sec_Combiner_Splitter { get; init; }

        public void CalcAntPowerBandAndTech(List<Cell> cells, List<Rru> rrus)
        {
            var filterCellsByBand = cells
               .Where(n => n.SectorId == SectorId
                           && n.AntennaId == AntennaId
                           && n.Band == Band)
               .OrderBy(n => n.CellName); 


            var filterCellsByBandGropByPortCellName = filterCellsByBand
                .GroupBy(n => new { n.SectorId, n.AntennaId, n.PortId, n.CellName })
                .Select(n => n.First()).ToList();
            

            var filterRrusByBandAndByCombination = rrus
               .Where(n => n.SectorId == SectorId
               && n.AntennaId == AntennaId
               && n.Band == Band
               && n.IsRruInComb()).ToList();

            foreach (var cell in filterCellsByBandGropByPortCellName)
            {
                foreach (var rru in filterRrusByBandAndByCombination)
                {
                    if (rru.CellName == cell.CellName)
                    {
                        cell.TrxPower = ConvertW_dBm(ConvertdBm_W(cell.TrxPower) / 2);
                        break;
                    }
                }
            }

            var powerWithFeederLossWatt = filterCellsByBandGropByPortCellName
                .Where(n => n.TrxPower > 0)
                .Select(n => n.TrxPower - n.Feeder_Att_dB)
                .Select(n =>  ConvertdBm_W(n)).ToList();

            var antennaInTotalPowerDbm = ConvertW_dBm(powerWithFeederLossWatt.Sum()) -
                                         CombinerSplitterLossDb(this.Combiner_Splitter) -
                                         CombinerSplitterLossDb(this.Sec_Combiner_Splitter);

            //Watts
            Antenna_IN_Total_Power = ConvertdBm_W(antennaInTotalPowerDbm);

            var techs = filterCellsByBand.Select(n => n.Technology.ToString()).Distinct().ToList();
            Tech = string.Join(" ", techs);

            Tech = Tech.Replace("NR", "TDD NR");

            Band = filterCellsByBand.Select(n => (Band)n.Band!).Distinct().First();

        }

        public void SetZeroEtiltForAntAAU5639W_PSK(List<Antenna> antennas)
        {
            var antenna = antennas.First(n => n.AntennaId == AntennaId);

            if (antenna.AntennaType == "AAU5639W")
                this.Etilt = 0;
        }

        public int OffsetExcel(PropertyInfo propInfo)
        {
            var offset = Engine.Report.AntennaOffsetSteps()[AntennaId % 10 - 1] + Engine.Report.BandOffsetSteps()[Band];
            return offset;
        }

        public SheetNames SheetName(PropertyInfo propInfo)
        {
            return SheetNames.Sectors;
        }

        public void Errors()
        {
            var errs = new List<string>();

            if (AntennaId % 10 > Report.MaxAllowedAntennas)
                errs.Add($"Max antenna is {Report.MaxAllowedAntennas}. Antenna {AntennaId} can not be seen!");

            if (errs.Any())
                Status = Status.NotOk;
            else
                Status = Status.Ok;

            Engine.Errs[SectorId].AddRange(errs);
        }
    }
}
