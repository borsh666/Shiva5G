using BLL.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static BLL.SupportFunc;


namespace BLL
{
    public class DTO_Load_PSK : DTO_Load
    {
        public DTO_Load_PSK(string siteID) : base(siteID)
        {
            this.IsGroupByBand = true;
        }


        public override List<Port> TechLoad(string sector, string antennaType, decimal phyIndex)
        {
            var technologies = new List<Port>();

            var filterTechnology = this.lstTechnology.Where(n => n.Sector == sector
                && n.AntennaType == antennaType && n.PHYINDEX == phyIndex).ToList();

            return base.ViewModelTechnologyToPort(filterTechnology);

        }
        //Override only for set Antenna Offset
        public override List<Sector> Sector()
        {
            var sectors = base.Sector();

            foreach (var sector in sectors)
            {
                int step = 1;

                foreach (var antenna in sector.Antennas)
                {
                    antenna.OffsetExcel = step;

                    foreach (var port in antenna.Ports)
                        port.OffsetExcel = antenna.OffsetExcel + port.BandPosition - 1;

                    step = step + ReportPSK.NumberOfBandsPerAntenna;


                }
            }

            return sectors;
        }

        public override List<Port> PortLoad(string sector, string antennaType, decimal phyIndex)
        {
            return BandLoad(sector, antennaType, phyIndex);
        }

        public List<Port> BandLoad(string sector, string antennaType, decimal phyIndex)
        {

            var allTech = this.TechLoad(sector, antennaType, phyIndex);

            var portGrouping = new PortGrouping(allTech);

            var groupByBand = portGrouping.GropingBySecAntPhBand();

            foreach (var port in groupByBand)
            {
                double getCalcPower = 0;

                port.ModelRRUs.RemoveAll(n => n.Band == "3500");

                //Here only for 2G sum TRXs. For 3G,4G,5G TRX=1
                var groupByCellName = port.ModelRRUs.GroupBy(n => n.CellName)
                    .Select(n => new ModelRRU
                    {
                        GSM_TRX = n.Select(k => k.GSM_TRX).Sum(),
                        UMTS_TRX = n.Select(k => k.UMTS_TRX).First(),
                        LTE_TRX = n.Select(k => k.LTE_TRX).First(),
                        NR_TRX = n.Select(k => k.NR_TRX).First(),
                        GSM_Pwr_per_TRX = n.Select(k => decimal.Parse(k.GSM_Pwr_per_TRX)).Sum().ToString(),
                        UMTS_Pwr_per_TRX = n.Select(k => k.UMTS_Pwr_per_TRX).First(),
                        LTE_Pwr_per_TRX = n.Select(k => k.LTE_Pwr_per_TRX).First(),
                        NR_Pwr_per_TRX = n.Select(k => k.NR_Pwr_per_TRX).First(),
                    });

                foreach (var rru in groupByCellName)
                    getCalcPower += GetPortCalcPower(rru);

                //SF1862
                //if (sector == "1" && antennaType == "AQU4518R25v18" && item.Band == "900")
                //    Console.WriteLine();

                port.Antenna_IN_Total_Power = CalcAntennaInTotalPower(port, getCalcPower);
            }

            return groupByBand;
        }

        private double GetPortCalcPower(ModelRRU rru)
        {
            if (rru == null)
                return 0;

            return (rru.GSM_TRX * ConvertdBm_W(double.Parse(rru.GSM_Pwr_per_TRX))) +
                    (rru.UMTS_TRX * ConvertdBm_W(double.Parse(rru.UMTS_Pwr_per_TRX))) +
                    (rru.LTE_TRX * ConvertdBm_W(double.Parse(rru.LTE_Pwr_per_TRX))) +
                    (rru.NR_TRX * ConvertdBm_W(double.Parse(rru.NR_Pwr_per_TRX)));

        }

        private double CalcAntennaInTotalPower(Port port, double portPower)
        {
            var antenna_IN_Total_Power = ConvertW_dBm(portPower)
                        - port.Combiner_Splitter_Loss
                        - port.Second_Combiner_Splitter_Loss
                        - port.Feeder_Att_dB;

            antenna_IN_Total_Power = ConvertdBm_W(antenna_IN_Total_Power);

            return antenna_IN_Total_Power;
        }
    }
}
