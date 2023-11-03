using BLL_Atoll.Enums;
using BLL_Atoll.Interfaces;

namespace BLL_Atoll.Models.Site
{
    public record Rru : SectorAntStatus, IPort
    {
        public string RruType()
        {
            string AddRruSuffix(string rruType)
            {
                if (IsRruInComb() && PortIndexInComb() is not null)
                    return $"{rruType}{rruCombination[(Band)Band!][(int)PortIndexInComb()]}";
                //else if (rruType == "5519et")
                //    return $"{rruType}{rruCombination[(Band)Band!][(int)PortIndexInComb()]}";
                else
                    return ConcatRruWithBand(rruType, (Band)Band!);
            }

            if(SectorId ==1 && PortId ==4)
                Console.WriteLine();

            if (Engine.Report.Document == ReportType.IRFC)
            {
                if (RruTypeAtoll.Contains("REUSE"))
                    return RruTypeAtoll;
                else
                    return AddRruSuffix(RruTypeAtoll);
            }
            else
                return AddRruSuffix(RruTypeCm);

        }

        public readonly Dictionary<Band, string[]> rruCombination =
            new()
            {
                { Enums.Band.B9,  new string[] { "CD", "AB", } },
                { Enums.Band.B18, new string[] { "AC", "DB", } },
                { Enums.Band.B21, new string[] { "AC", "DB", } },
                { Enums.Band.B26, new string[] { "AC", "DB", } },
            };
        public double combiner_Splitter_Loss;
        public double second_Combiner_Splitter_Loss;

        public string? RruInComb { get; set; }
        public string? RruSN { get; set; }
        public string? RruTypeAtoll { get; set; }
        public string? RruTypeCm { get; set; }
        public int PortId { get; init; }
        public int RruId { get; init; }
        public Band? Band { get; set; }
        public string? CellName { get; set; }
        public bool IsRruInComb()
        {
            if (!string.IsNullOrEmpty(RruInComb) && RruInComb.Contains(';'))
                return true;
            return false;
        }
        private int? PortIndexInComb()
        {
            var portIndex = new[] { 0, 1 };

            if (int.TryParse(RruInComb.Split(';').First().Trim(), out int port1) &&
                int.TryParse(RruInComb.Split(';').Last().Trim(), out int port2))
            {
                if (PortId == port1)
                    return portIndex.First();
                else if (PortId == port2)
                    return portIndex.Last();
                else
                    return null;
            }
            else
                return null;
        }
        private static string ConcatRruWithBand(string rruType, Band band)
        {
            if (band == Enums.Band.B9)
                return rruType + "-9";
            else if (band == Enums.Band.B18)
                return rruType + "-18";
            else if (band == Enums.Band.B21)
                return rruType + "-21";
            else if (band == Enums.Band.B26)
                return rruType + "-26";
            else if (band == Enums.Band.B35)
                return rruType + "-35";

            return rruType;
        }

        public override void Errors()
        {
            var errs = new List<string>();

            if (CellName is null)
            {
                errs.Add($"Sec {SectorId} Ant {AntennaId} Port {PortId} doesn't have cellname!");
                CellName = $"Sec {SectorId} Ant {AntennaId} Port {PortId} ";
            }
            if (!Band.HasValue)
                errs.Add($"{CellName} doesn't have band!");
            if (PortId == 0)
                errs.Add($"{CellName} doesn't have portId!");
            if (RruTypeAtoll is null)
            {
                RruTypeAtoll = String.Empty;
                errs.Add($"{CellName} doesn't have RruTypeAtoll in Atoll!");
            }
            if (RruInComb is null)
            {
                RruTypeCm = String.Empty;
                errs.Add($"{CellName} doesn't have ports in Atoll!");
            }
            if (Engine.Report.Document == ReportType.SRF || Engine.Report.Document == ReportType.SA)
            {
                if (RruTypeCm is null || RruSN is null)
                {
                    RruTypeCm = String.Empty;
                    RruSN = String.Empty;
                    errs.Add($"{CellName} doesn't have RruType and RruSN in CM!");
                }
            }

            if (errs.Any())
                Status = Status.NotOk;
            else
                Status = Status.Ok;

            if (Engine.Errs.ContainsKey(SectorId))
                Engine.Errs[SectorId].AddRange(errs);
            else
                throw new Exception($"Not such sector id: {SectorId}");
        }

    }
}
