using BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class DTO_Load_IRFC : DTO_Load
    {

        public DTO_Load_IRFC(string siteID, bool isSiteSRAN) : base(siteID, isSiteSRAN)
        {
            this.IsRrusFromOss = false;
        }

        //New
        public override List<Sector> Sector()
        {
            var sectors = base.Sector();

            foreach (var sector in sectors)
            {
                var filterLstTech = lstTechnology.Where(n => n.Sector == sector.SectorNumb);

                sector.GSM_900 = (int)filterLstTech
                  .Where(n => n.LAYER_TECHNOLOGY == "G" && n.Band == "900").Sum(n => n.GSM_TRX);

                sector.GSM_900 = (int)filterLstTech
                  .Where(n => n.LAYER_TECHNOLOGY == "G" && n.Band == "900").Sum(n => n.GSM_TRX);

                sector.GSM_1800 = (int)filterLstTech
                  .Where(n => n.LAYER_TECHNOLOGY == "G" && n.Band == "1800").Sum(n => n.GSM_TRX);

                sector.UMTS_900 = (int)filterLstTech
                  .Where(n => n.LAYER_TECHNOLOGY == "U" && n.Band == "900").Select(n => n.CellName).Distinct().Count();

                sector.UMTS_2100 = (int)filterLstTech
                  .Where(n => n.LAYER_TECHNOLOGY == "U" && n.Band == "2100").Select(n => n.CellName).Distinct().Count();

                sector.LTE_900 = (int)filterLstTech
                  .Where(n => n.LAYER_TECHNOLOGY == "L" && n.Band == "900").Select(n => n.CellName).Distinct().Count();

                sector.LTE_1800 = (int)filterLstTech
                  .Where(n => n.LAYER_TECHNOLOGY == "L" && n.Band == "1800").Select(n => n.CellName).Distinct().Count();

                sector.LTE_2100 = (int)filterLstTech
                  .Where(n => n.LAYER_TECHNOLOGY == "L" && n.Band == "2100").Select(n => n.CellName).Distinct().Count();

                sector.LTE_2600 = (int)filterLstTech
                  .Where(n => n.LAYER_TECHNOLOGY == "L" && n.Band == "2600").Select(n => n.CellName).Distinct().Count();

                sector.NR_1800 = (int)filterLstTech
                  .Where(n => n.LAYER_TECHNOLOGY == "NR" && n.Band == "1800").Select(n => n.CellName).Distinct().Count();

                sector.NR_2100 = (int)filterLstTech
                  .Where(n => n.LAYER_TECHNOLOGY == "NR" && n.Band == "2100").Select(n => n.CellName).Distinct().Count();

                sector.NR_3500 = (int)filterLstTech
                 .Where(n => n.LAYER_TECHNOLOGY == "NR" && n.Band == "3500").Select(n => n.CellName).Distinct().Count();

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

                double getCalcPower = 0;

                port.GSM_TRX = port.ModelRRUs.Sum(n => n.GSM_TRX);
                port.UMTS_TRX = port.ModelRRUs.Sum(n => n.UMTS_TRX);
                port.LTE_TRX = port.ModelRRUs.Sum(n => n.LTE_TRX);
                port.NR_TRX = port.ModelRRUs.Sum(n => n.NR_TRX);

                //Това е мощноста взета директно от Асет без мапинг, защото е мощност сетната на един TRX
                port.GSM_Pwr_per_TRX = Math.Round(decimal.Parse(port.ModelRRUs.Max(n => n.GSM_Pwr_per_TRX)), 2).ToString();
                port.UMTS_Pwr_per_TRX = Math.Round(decimal.Parse(port.ModelRRUs.Max(n => n.UMTS_Pwr_per_TRX)), 2).ToString();
                port.LTE_Pwr_per_TRX = Math.Round(decimal.Parse(port.ModelRRUs.Max(n => n.LTE_Pwr_per_TRX)), 2).ToString();
                port.NR_Pwr_per_TRX = Math.Round(decimal.Parse(port.ModelRRUs.Max(n => n.NR_Pwr_per_TRX)), 2).ToString();

                if (port.ModelRRUs.Any(n => n.RRU_Type.Contains("REUSE")))
                {
                    var rrus = port.ModelRRUs.Where(n => n.RRU_Type != "REUSE").Select(n => n.RRU_Type);
                    port.RRU_Type = String.Join(" ", rrus);
                    port.RRU_Total = port.ModelRRUs.Where(n => n.RRU_Type != "REUSE").Select(n => n.RRU_Type).Count().ToString();
                }
                else
                {
                    var rrus = port.ModelRRUs.Select(n => n.RRU_Type);
                    port.RRU_Type = string.Join(" ", rrus);
                    port.RRU_Total = port.ModelRRUs.Select(n => n.RRU_Type).Count().ToString();
                }

                foreach (var rru in port.ModelRRUs)
                {
                    //Temporary solution 12.07.2121
                    if (rru.UMTS_TRX > 1)
                        rru.UMTS_TRX = 1;

                    getCalcPower += SupportFunc.GetPortCalcPowerIRFC(rru);

                }



                //Convert All Power in W and do mapping.
                //Calculate Antenna_IN_Total_Power in W

                if (port.Feeder_Att_dB > 0)
                {
                    //if (port.Band != "2600")
                    port.Antenna_IN_Total_Power = SupportFunc.ConvertW_dBm(getCalcPower)
                        - port.Combiner_Splitter_Loss
                        - port.Second_Combiner_Splitter_Loss
                        - port.Feeder_Att_dB; ;
                }
                else
                    port.Request_Remarks = $"Antenna_IN_Total_Power=0. Моля проверете дали са въведени стойности за JumperType и дължина на фидера{Environment.NewLine}";

            }

            return arrangedPorts;




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
