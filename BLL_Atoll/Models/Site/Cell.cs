using BLL_Atoll.Enums;
using BLL_Atoll.Interfaces;

namespace BLL_Atoll.Models.Site
{
    public record Cell : SectorAntStatus, IPort
    {
        public int PortId { get; init; }
        public string? CellName { get; set; }
        public Technology? Technology { get; init; }
        public double BandWidth { get; init; }
        public string? RruType { get; init; }
        public int? TrxAtoll { get; init; }
        public int? TrxCm { get; init; }
        public double TrxPower { get; set; }
        public double? PwrPerTrxAtoll { get; set; }
        public double? PwrPerTrxCm { get; init; }
        public Band? Band { get; init; }
        public string? BandRange { get; init; }
        public decimal? Etilt { get; init; }
        public decimal? Feeder_Length { get; init; }
        public string? Feeder_Name { get; set; }
        public double Feeder_Att_dB { get; set; }
        public string Feeder_Type { get; private set; }

        public int Trx()
        {
            if (Engine.Report.Document == ReportType.IRFC || Engine.Report.Document == ReportType.PSK)
                return TrxAtoll ?? 0;
            else
                return TrxCm ?? 0;
        }

        public void CalcFeederLoss(List<dynamic> feederLossFromDb)
        {
            var feederLoss = feederLossFromDb.Where(n =>
            n.Feeder_Length == Feeder_Length &&
            n.Feeder_Name == Feeder_Name &&
            n.Tech == (int)Band)
                .FirstOrDefault();

            if (feederLoss is not null)
            {
                Feeder_Att_dB = (double)feederLoss.Feeder_Att_dB;
                Feeder_Type = feederLoss.Feeder_Type;
            }
        }

        public void CalcPwrPerTrx()
        {
            //if (SectorId == 1 && AntennaId == 11 && PortId == 1 && Technology == Enums.Technology.L)
            //    Console.WriteLine();

            if (Technology == Enums.Technology.NR)
            {
                TrxPower = SupportFunc.ConvertW_dBm(PwrPerTrxAtoll ?? 0);
                return;
            }


            if (Engine.Report.Document == ReportType.IRFC || Engine.Report.Document == ReportType.PSK)
            {
                if (Technology == Enums.Technology.L)
                    this.PwrPerTrxAtoll = SupportFunc.ConvertW_dBm(
                            (SupportFunc.ConvertdBm_W(this.PwrPerTrxAtoll ?? 0) * 5 * BandWidth * 12));

                TrxPower = this.PwrPerTrxAtoll ?? 0;
            }
            else
                TrxPower = PwrPerTrxCm ?? 0;


        }

        public override void Errors()
        {
            var errs = new List<string>();

            if (CellName is null)
            {
                errs.Add($"Sec {SectorId} Ant {AntennaId} Port {PortId} doesn't have cellname!");
                CellName = $"Sec {SectorId} Ant {AntennaId} Port {PortId} ";
            }
            if (PwrPerTrxAtoll == 0 && Technology != Enums.Technology.DSS)
                errs.Add($"{CellName} doesn't have power in Atoll!");
            if (Technology != Enums.Technology.DSS &&
                (Engine.Report.Document == ReportType.SA ||
                 Engine.Report.Document == ReportType.SA))
            {
                if (PwrPerTrxCm == 0)
                    errs.Add($"{CellName} doesn't have power in Configuration!");
                if (TrxCm == 0)
                    errs.Add($"{CellName} doesn't have trx in Configuration!");
            }

            if (errs.Any())
                Status = Status.NotOk;
            else
                Status = Status.Ok;

            Engine.Errs[SectorId].AddRange(errs);
        }
    }
}

