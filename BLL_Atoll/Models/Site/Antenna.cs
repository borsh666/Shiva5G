using BLL_Atoll.Attributes;
using BLL_Atoll.Enums;
using BLL_Atoll.Interfaces;
using System.Reflection;


namespace BLL_Atoll.Models.Site
{
    public partial record Antenna : SectorAntStatus, IExcel
    {
        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA, ReportType.PSK })]
        [ForPrint("AntennaMount")]
        public string AntennaMount { get; init; }

        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA, ReportType.PSK })]
        [ForPrint("AGL")]
        public decimal Height { get; set; }

        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA, ReportType.PSK })]
        [ForPrint("MechanicalTilt")]
        public decimal? Mtilt { get; set; }

        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA, ReportType.PSK })]
        [ForPrint("AntennaType")]
        public string AntennaType { get; set; }
    }


    public partial record Antenna
    {
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

        public int OffsetExcel(PropertyInfo propInfo)
        {
            return Engine.Report.AntennaOffsetSteps()[(AntennaId % 10) - 1];
        }
        
        public override void Errors()
        {
            var errs = new List<string>();

            if (AntennaId % 10 > Report.MaxAllowedAntennas)
                errs.Add($"Max antenna is {Report.MaxAllowedAntennas}. Antenna {AntennaType} can't be seen!");

            if (errs.Any())
            {
                Status = Status.NotOk;
                Engine.Errs[SectorId].AddRange(errs);
            }
            else
                Status = Status.Ok;
            

            if (AntennaLevel is AntennaLevel.Secondary or
                AntennaLevel.Empty or
                AntennaLevel.Remote)
            {
                Engine.Errs[SectorId].Add($"Antenna {AntennaType} is " +
                    $"{AntennaLevel}");
            }
        }

        public List<Port> GetEmptyPorts(List<Port> occupiedPorts, List<dynamic> antennaPortMap)
        {
            var filterOccupiedPorts = occupiedPorts.Where(n => n.AntennaId == AntennaId);

            List<dynamic> filterAntennaPorts = antennaPortMap
                .Where(p => p.ANTENNATYPE.ToString() == AntennaType).ToList();

            var emptyPorts = new Dictionary<string, Port>();


            foreach (var port in filterAntennaPorts)
            {
                if (filterOccupiedPorts.Where(n => n.PortId == (int)port.PORTID).Any())
                    continue;
                else
                {
                    var key = $"Sec:{SectorId} Ant:{AntennaId} Port:{port.PORTID}";

                    if (emptyPorts.ContainsKey(key))
                        continue;

                    emptyPorts.Add(key, new Port
                    {
                        Status = Status.Free,
                        BandRange = port.BANDRANGE.ToString(),
                        Tech = "Free",
                        PortType = port.PORTTYPE,
                        SectorId = SectorId,
                        AntennaId = AntennaId,
                        PortId = port.PORTID

                    });
                }
            }

            return emptyPorts.Values.ToList();
        }
    }
}
