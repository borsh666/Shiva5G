using BLL.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BLL
{
    public class DTO_Load_PSK : DTO_Load
    {
        public DTO_Load_PSK(string siteID, bool isSiteSRAN) : base(siteID, isSiteSRAN)
        {
        }

        //public override void QueryMaterialize()
        //{
        //    base.QueryMaterialize();

        //    this.LstLoadRRU_CM = base.LoadRRU_Oss();

        //    var mapTechnoly = SupportFunc.MapAssetTechWithCmTech(this.lstTechnology, this.LstLoadRRU_CM.Select(n => n.CellName));

        //    if (mapTechnoly.Count != 0)
        //    {
        //        this.IsRrusFromOss = true;

        //        this.LstLoadPowerTRX_CM = base.LoadPowerTRX_Oss();

        //        this.lstTechnology = mapTechnoly;
        //    }
        //}


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

            foreach (var item in groupByBand)
            {
                double getCalcPower = 0;

                item.ModelRRUs.RemoveAll(n => n.Band == "3500");

                //Here only for 2G sum TRXs. For 3G,4G,5G TRX=1
                var groupByCellName = item.ModelRRUs.GroupBy(n => n.CellName)
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

             
                //getCalcPower = 0;
                foreach (var rru in groupByCellName)
                    getCalcPower += SupportFunc.GetPortCalcPowerIRFC(rru);


                //Convert All Power in W and do mapping.
                //Calculate Antenna_IN_Total_Power in W
                if (item.Feeder_Att_dB > 0)
                {
                    item.Antenna_IN_Total_Power = SupportFunc.ConvertW_dBm(getCalcPower)
                       - item.Combiner_Splitter_Loss
                       - item.Second_Combiner_Splitter_Loss
                       - item.Feeder_Att_dB;

                    item.Antenna_IN_Total_Power = SupportFunc.ConvertdBm_W(item.Antenna_IN_Total_Power);

                }
                else
                    item.Request_Remarks += $"Antenna_IN_Total_Power=0. Моля проверете дали са въведени стойности за JumperType и дължина на фидера{Environment.NewLine}";
            }

            return groupByBand;
        }


    }
}
