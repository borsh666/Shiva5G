using BLL.DTO;
using MoreLinq;
using OfficeOpenXml.FormulaParsing.Excel.Functions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using static BLL.SupportFunc;
using BLL.Enums;
using System.Runtime.InteropServices;

namespace BLL
{
    public class DTO_Load_IRFC : DTO_Load
    {
        private const string rruTypeReuse = "REUSE";
        //RRU Combination AB,CD.....
        private new readonly IEnumerable<string> rruCombination;

        public DTO_Load_IRFC(string siteID) : base(siteID)
        {
            this.IsRrusFromOss = false;
            rruCombination = base.rruCombination.Values.SelectMany(l => l).Distinct();
        }

        //New
        public override List<Sector> Sector()
        {
            var sectors = base.Sector();

            foreach (var sector in sectors)
            {
                var filterLstTech = lstTechnology.Where(n => n.Sector == sector.SectorNumb);

                sector.GSM_900 = (int)filterLstTech
                  .Where(n => n.LAYER_TECHNOLOGY == Technology.G.ToString() && n.Band == ((int)Band.B9).ToString())
                  .Sum(n => n.GSM_TRX);

                sector.GSM_900 = (int)filterLstTech
                  .Where(n => n.LAYER_TECHNOLOGY == Technology.G.ToString() && n.Band == ((int)Band.B9).ToString())
                  .Sum(n => n.GSM_TRX);

                sector.GSM_1800 = (int)filterLstTech
                  .Where(n => n.LAYER_TECHNOLOGY == Technology.G.ToString() && n.Band == ((int)Band.B18).ToString()).Sum(n => n.GSM_TRX);

                sector.UMTS_900 = (int)filterLstTech
                  .Where(n => n.LAYER_TECHNOLOGY == Technology.U.ToString() && n.Band == ((int)Band.B9).ToString())
                  .Select(n => n.CellName).Distinct().Count();

                sector.UMTS_2100 = (int)filterLstTech
                  .Where(n => n.LAYER_TECHNOLOGY == Technology.U.ToString() && n.Band == ((int)Band.B21).ToString()).Select(n => n.CellName).Distinct().Count();

                sector.LTE_900 = (int)filterLstTech
                  .Where(n => n.LAYER_TECHNOLOGY == Technology.L.ToString() && n.Band == ((int)Band.B9).ToString())
                  .Select(n => n.CellName).Distinct().Count();

                sector.LTE_1800 = (int)filterLstTech
                  .Where(n => n.LAYER_TECHNOLOGY == Technology.L.ToString() && n.Band == ((int)Band.B18).ToString())
                  .Select(n => n.CellName).Distinct().Count();

                sector.LTE_2100 = (int)filterLstTech
                  .Where(n => n.LAYER_TECHNOLOGY == Technology.L.ToString() && n.Band == ((int)Band.B21).ToString())
                  .Select(n => n.CellName).Distinct().Count();

                sector.LTE_2600 = (int)filterLstTech
                  .Where(n => n.LAYER_TECHNOLOGY == Technology.L.ToString() && n.Band == ((int)Band.B26).ToString())
                  .Select(n => n.CellName).Distinct().Count();

                sector.NR_1800 = (int)filterLstTech
                  .Where(n => n.LAYER_TECHNOLOGY == Technology.NR.ToString() && n.Band == ((int)Band.B18).ToString())
                  .Select(n => n.CellName).Distinct().Count();

                sector.NR_2100 = (int)filterLstTech
                  .Where(n => n.LAYER_TECHNOLOGY == Technology.NR.ToString() && n.Band == ((int)Band.B21).ToString())
                  .Select(n => n.CellName).Distinct().Count();

                sector.NR_3500 = (int)filterLstTech
                 .Where(n => n.LAYER_TECHNOLOGY == Technology.NR.ToString() && n.Band == ((int)Band.B35).ToString())
                 .Select(n => n.CellName).Distinct().Count();

            }

            return sectors;
        }

        public override List<Port> PortLoad(string sector, string antennaType, decimal phyIndex)
        {

            var allTech = TechLoad(sector, antennaType, phyIndex);
            //BS3831
            var portGrouping = new PortGrouping(allTech);

            var arrangedPorts = portGrouping.PortArrange(siteID);

            foreach (var port in arrangedPorts)
            {
                if (port.Status == "Free")
                    continue;

                port.GSM_TRX = port.ModelRRUs.Sum(n => n.GSM_TRX);
                port.UMTS_TRX = port.ModelRRUs.Sum(n => n.UMTS_TRX);
                port.LTE_TRX = port.ModelRRUs.Sum(n => n.LTE_TRX);
                port.NR_TRX = port.ModelRRUs.Sum(n => n.NR_TRX);

                var powers = port.ModelRRUs.Select(n => n.GSM_Pwr_per_TRX).ToList();
                var trx = port.ModelRRUs.Sum(n => n.GSM_TRX);
                port.GSM_Pwr_per_TRX = CalcPowerPerTrx(powers, trx);

                powers = port.ModelRRUs.Select(n => n.UMTS_Pwr_per_TRX).ToList();
                trx = port.ModelRRUs.Sum(n => n.UMTS_TRX);
                port.UMTS_Pwr_per_TRX = CalcPowerPerTrx(powers, trx);

                powers = port.ModelRRUs.Select(n => n.LTE_Pwr_per_TRX).ToList();
                trx = port.ModelRRUs.Sum(n => n.LTE_TRX);
                port.LTE_Pwr_per_TRX = CalcPowerPerTrx(powers, trx);

                powers = port.ModelRRUs.Select(n => n.NR_Pwr_per_TRX).ToList();
                trx = port.ModelRRUs.Sum(n => n.NR_TRX);
                port.NR_Pwr_per_TRX = CalcPowerPerTrx(powers, trx);

                var rrusWithoutReuse = port.ModelRRUs.Where(n => !IsRruReuse(n.RRU_Type));

                foreach (var rru in rrusWithoutReuse)
                {
                    if (!IsRruInComb(rru.RRU_Type, rruCombination))
                        rru.RRU_Type = ConcatRruWithBand(rru.RRU_Type, rru.Band);
                }

                port.RRU_Type = String.Join(" ", rrusWithoutReuse.Select(n => n.RRU_Type));
                port.RRU_Total = rrusWithoutReuse.Count().ToString();

                //SF1862
                //if (sector == "1" && antennaType == "RRV4-65D-R6" && port.Technology == "ULNR")
                //    Console.WriteLine();
                port.Antenna_IN_Total_Power = CalcAntennaInTotalPower(port);
 
            }

            ApplyAllExceptions(ref arrangedPorts);

            return arrangedPorts;
        }

        private void ApplyAllExceptions(ref List<Port> ports)
        {
            //RRU 5519etCD Exception
            var port = ports.Where(n => n.RRU_Type == "5519etCD").FirstOrDefault();
            if(port!=null)
            {
                port.GSM_TRX = 0;
                port.GSM_Pwr_per_TRX = null;
                port.UMTS_TRX = 0;
                port.UMTS_Pwr_per_TRX = null;
                port.LTE_TRX = 0;
                port.LTE_Pwr_per_TRX = null;
                port.Antenna_IN_Total_Power = 0;

            }
        }

        private double CalcAntennaInTotalPower(Port port)
        {
            var isSomeRruInComb = port.ModelRRUs.Any(n => IsRruInComb(n.RRU_Type, rruCombination));
            var isSomeRruIsREUSE = port.ModelRRUs.Any(n => IsRruReuse(n.RRU_Type));

            if (isSomeRruInComb && isSomeRruIsREUSE && IsBandHigh(port.Band))
                port.ModelRRUs.Where(n => IsRruReuse(n.RRU_Type))
                              .ForEach(n => n.RRU_Type = rruCombination.First());

            var groupRruByBand = port.ModelRRUs.GroupBy(n => new { n.Band })
                .Select(n => new
                {
                    n.Key.Band,
                    Power = ConvertW_dBm(
                            n.Sum(k => ConvertdBm_W(double.Parse(k.GSM_Pwr_per_TRX) * k.GSM_TRX)) +
                            n.Sum(k => ConvertdBm_W(double.Parse(k.UMTS_Pwr_per_TRX) * k.UMTS_TRX)) +
                            n.Sum(k =>
                            {
                                var power = ConvertdBm_W(double.Parse(k.LTE_Pwr_per_TRX) * k.LTE_TRX);
                                if (IsRruInComb(k.RRU_Type, rruCombination) && IsBandHigh(port.Band))
                                    power /= 2;
                                return power;
                            }) +
                            n.Sum(k => ConvertdBm_W(double.Parse(k.NR_Pwr_per_TRX) * k.NR_TRX))),

                    n.FirstOrDefault().Feeder_Att_dB
                });

            double calcPowerInW = 0;

            foreach (var rru in groupRruByBand)
                calcPowerInW += ConvertdBm_W(rru.Power - rru.Feeder_Att_dB);


            port.Antenna_IN_Total_Power = SupportFunc.ConvertW_dBm(calcPowerInW)
                - port.Combiner_Splitter_Loss
                - port.Second_Combiner_Splitter_Loss;

            return port.Antenna_IN_Total_Power;
        }

        private bool IsRruInComb(string rruType, IEnumerable<string> rruCombination)
        {
            return rruCombination.Any(l => rruType.Contains(l));
        }

        private bool IsRruReuse(string rruType)
        {
            return rruType.Contains(rruTypeReuse);
        }

        public override List<Port> TechLoad(string sector, string antennaType, decimal phyIndex)
        {
            var technologies = new List<Port>();

            var filterTechnology = this.lstTechnology.Where(n => n.Sector == sector
                && n.AntennaType == antennaType && n.PHYINDEX == phyIndex).ToList();

            return base.ViewModelTechnologyToPort(filterTechnology);

        }
    }
}
