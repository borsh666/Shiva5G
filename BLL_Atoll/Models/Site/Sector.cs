using BLL_Atoll.Attributes;
using BLL_Atoll.Enums;
using BLL_Atoll.Interfaces;
using System.Reflection;

namespace BLL_Atoll.Models.Site
{
    public partial record Sector : IExcel, ISector
    {

        [ForPrint("GSM_900")]
        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        [VerticalOffset]
        public int GSM_900 { get; set; }

        [ForPrint("GSM_1800")]
        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        [VerticalOffset]
        public int GSM_1800 { get; set; }

        [ForPrint("UMTS_2100")]
        [VerticalOffset]
        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        public int UMTS_2100 { get; set; }

        [ForPrint("UMTS_900")]
        [VerticalOffset]
        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        public int UMTS_900 { get; set; }

        [ForPrint("LTE_900")]
        [VerticalOffset]
        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        public int LTE_900 { get; set; }

        [ForPrint("LTE_1800")]
        [VerticalOffset]
        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        public int LTE_1800 { get; set; }

        [ForPrint("LTE_2100")]
        [VerticalOffset]
        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        public int LTE_2100 { get; set; }

        [ForPrint("LTE_2600")]
        [VerticalOffset]
        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        public int LTE_2600 { get; set; }

        [ForPrint("DSS_1800")]
        [VerticalOffset]
        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        public int DSS_1800 { get; set; }

        [ForPrint("DSS_2100")]
        [VerticalOffset]
        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        public int DSS_2100 { get; set; }

        [ForPrint("NR-TDD 3600")]
        [VerticalOffset]
        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA })]
        public int NR_3500 { get; set; }

        public int SectorId { get; init; }

        [ForPrint("Azimuth")]
        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA, ReportType.PSK })]
        public int Azimuth { get; init; }

        [Report(new[] { ReportType.IRFC, ReportType.SRF, ReportType.SA, ReportType.PSK })]
        [ForPrint("Request_Remarks")]
        public string Request_Remarks { get; set; }


    }

    public partial record Sector
    {

        public void PopulateTrxValues()
        {
            var filterCells = Engine.Cells
                              .Where(n => n.SectorId == SectorId);

            var group = filterCells.GroupBy(n => new { n.SectorId, n.CellName })
                .Select(n => n.First())
                .ToList();

            int SumTrx(Technology tech, Band band)
            {
                var trx = group
                   .Where(n => n.Technology == tech && n.Band == band)
                   .Select(n => n.Trx());

                int totalTrx = 0;

                if (trx.Any())
                {
                    if (tech == Technology.DSS)
                        totalTrx = 1;
                    else
                        totalTrx = trx.Sum();
                }
                return totalTrx;
            }

            this.GSM_900 = SumTrx(Technology.G, Band.B9);
            this.GSM_1800 = SumTrx(Technology.G, Band.B18);

            this.UMTS_900 = SumTrx(Technology.U, Band.B9);
            this.UMTS_2100 = SumTrx(Technology.U, Band.B21);

            this.LTE_900 = SumTrx(Technology.L, Band.B9);
            this.LTE_1800 = SumTrx(Technology.L, Band.B18);
            this.LTE_2100 = SumTrx(Technology.L, Band.B21);
            this.LTE_2600 = SumTrx(Technology.L, Band.B26);

            this.DSS_1800 = SumTrx(Technology.DSS, Band.B18);
            this.DSS_2100 = SumTrx(Technology.DSS, Band.B21);
            this.NR_3500 = SumTrx(Technology.NR, Band.B35);
        }

        public void PopulateErrors()
        {
            var uniqueErrors = Engine.Errs[SectorId].Distinct().ToList();

            this.Request_Remarks = String.Join(", ", uniqueErrors);
        }

        public SheetNames SheetName(PropertyInfo propInfo)
        {
            var verticalAtt = propInfo.GetCustomAttribute(typeof(VerticalOffsetAttribute));
            if (verticalAtt != null)
                return SheetNames.Common;

            if (Engine.Report.Document == ReportType.PSK)
                return SheetNames.Sectors;

            return Engine.Report.WorkSheetsName
                    .Where(n => n.ToString().Contains(SectorId.ToString()))
                    .FirstOrDefault();
        }

        public int OffsetExcel(PropertyInfo propInfo)
        {
            var verticalAtt = propInfo.GetCustomAttribute(typeof(VerticalOffsetAttribute));
            if (verticalAtt != null)
                return SectorId;
            else
                return 1;
        }
    }
}
