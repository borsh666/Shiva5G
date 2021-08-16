using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;
using System.Diagnostics;
using BLL.Enums;

namespace BLL
{
    public class DTO_Load_SA : DTO_Load
    {
        public DTO_Load_SA(string siteID) : base(siteID)
        {

        }

        public override void QueryMaterialize()
        {
            base.QueryMaterialize();

            var watch = new Stopwatch();
            watch.Start();

            this.LstLoadRRU_CM = base.LoadRRU_Oss();

            var mapTechnoly = SupportFunc.MapAssetTechWithCmTech(this.lstTechnology, this.LstLoadRRU_CM.Select(n => n.CellName));

            if (mapTechnoly.Count != 0)
            {
                this.LstLoadPowerTRX_CM = base.LoadPowerTRX_Oss();

                this.lstTechnology = mapTechnoly;
                this.IsRrusFromOss = true;
            }

        }

        //New
        public override List<Sector> Sector()
        {
            var sectors = base.Sector();

            foreach (var sector in sectors)
            {
                if (this.IsRrusFromOss)
                {
                    sector.GSM_900 = this.LstLoadPowerTRX_CM
                       .Where(n => n.Sector == sector.SectorNumb && n.Technology == Technology.G.ToString() && n.Band == ((int)Band.B9).ToString())
                       .Sum(n => (n.GSM_TRX));
                    sector.GSM_1800 = this.LstLoadPowerTRX_CM
                        .Where(n => n.Sector == sector.SectorNumb && n.Technology == Technology.G.ToString() && n.Band == ((int)Band.B18).ToString())
                        .Sum(n => (n.GSM_TRX));
                    sector.UMTS_900 = this.LstLoadPowerTRX_CM
                        .Where(n => n.Sector == sector.SectorNumb && n.Technology == Technology.U.ToString() && n.Band == ((int)Band.B9).ToString())
                        .Select(n => n.CellName).Distinct().Count();
                    sector.UMTS_2100 = this.LstLoadPowerTRX_CM
                        .Where(n => n.Sector == sector.SectorNumb && n.Technology == Technology.U.ToString() && n.Band == ((int)Band.B21).ToString())
                        .Select(n => n.CellName).Distinct().Count();
                    sector.LTE_900 = this.LstLoadPowerTRX_CM
                        .Where(n => n.Sector == sector.SectorNumb && n.Technology == Technology.L.ToString() && n.Band == ((int)Band.B9).ToString())
                        .Select(n => n.CellName).Distinct().Count();
                    sector.LTE_1800 = this.LstLoadPowerTRX_CM
                        .Where(n => n.Sector == sector.SectorNumb && n.Technology == Technology.L.ToString() && n.Band == ((int)Band.B18).ToString())
                        .Select(n => n.CellName).Distinct().Count();
                    sector.LTE_2100 = this.LstLoadPowerTRX_CM
                        .Where(n => n.Sector == sector.SectorNumb && n.Technology == Technology.L.ToString() && n.Band == ((int)Band.B21).ToString())
                        .Select(n => n.CellName).Distinct().Count();
                    sector.LTE_2600 = this.LstLoadPowerTRX_CM
                        .Where(n => n.Sector == sector.SectorNumb && n.Technology == Technology.L.ToString() && n.Band == ((int)Band.B26).ToString())
                        .Select(n => n.CellName).Distinct().Count();
                    sector.NR_1800 = this.LstLoadPowerTRX_CM
                       .Where(n => n.Sector == sector.SectorNumb && n.Technology == Technology.NR.ToString() && n.Band == ((int)Band.B18).ToString())
                       .Select(n => n.CellName).Distinct().Count();
                    sector.NR_2100 = this.LstLoadPowerTRX_CM
                       .Where(n => n.Sector == sector.SectorNumb && n.Technology == Technology.NR.ToString() && n.Band == ((int)Band.B21).ToString())
                       .Select(n => n.CellName).Distinct().Count();
                    sector.NR_3500 = this.LstLoadPowerTRX_CM
                       .Where(n => n.Sector == sector.SectorNumb && n.Technology == Technology.NR.ToString() && n.Band == ((int)Band.B35).ToString())
                       .Select(n => n.CellName).Distinct().Count();
                }
                else
                {
                    var filterLstTech = lstTechnology.Where(n => n.Sector == sector.SectorNumb);

                    sector.GSM_900 = (int)filterLstTech
                  .Where(n => n.LAYER_TECHNOLOGY == Technology.G.ToString() && n.Band == ((int)Band.B9).ToString()).Sum(n => n.GSM_TRX);

                    sector.GSM_900 = (int)filterLstTech
                      .Where(n => n.LAYER_TECHNOLOGY == Technology.G.ToString() && n.Band == ((int)Band.B9).ToString()).Sum(n => n.GSM_TRX);

                    sector.GSM_1800 = (int)filterLstTech
                      .Where(n => n.LAYER_TECHNOLOGY == Technology.G.ToString() && n.Band == ((int)Band.B18).ToString()).Sum(n => n.GSM_TRX);

                    sector.UMTS_900 = (int)filterLstTech
                      .Where(n => n.LAYER_TECHNOLOGY == Technology.U.ToString() && n.Band == ((int)Band.B9).ToString()).Select(n => n.CellName).Distinct().Count();

                    sector.UMTS_2100 = (int)filterLstTech
                      .Where(n => n.LAYER_TECHNOLOGY == Technology.U.ToString() && n.Band == ((int)Band.B21).ToString()).Select(n => n.CellName).Distinct().Count();

                    sector.LTE_900 = (int)filterLstTech
                      .Where(n => n.LAYER_TECHNOLOGY == Technology.L.ToString() && n.Band == ((int)Band.B9).ToString()).Select(n => n.CellName).Distinct().Count();

                    sector.LTE_1800 = (int)filterLstTech
                      .Where(n => n.LAYER_TECHNOLOGY == Technology.L.ToString() && n.Band == ((int)Band.B18).ToString()).Select(n => n.CellName).Distinct().Count();

                    sector.LTE_2100 = (int)filterLstTech
                      .Where(n => n.LAYER_TECHNOLOGY == Technology.L.ToString() && n.Band == ((int)Band.B21).ToString()).Select(n => n.CellName).Distinct().Count();

                    sector.LTE_2600 = (int)filterLstTech
                      .Where(n => n.LAYER_TECHNOLOGY == Technology.L.ToString() && n.Band == ((int)Band.B26).ToString()).Select(n => n.CellName).Distinct().Count();

                    sector.NR_1800 = (int)filterLstTech
                      .Where(n => n.LAYER_TECHNOLOGY == Technology.NR.ToString() && n.Band == ((int)Band.B18).ToString()).Select(n => n.CellName).Distinct().Count();

                    sector.NR_2100 = (int)filterLstTech
                      .Where(n => n.LAYER_TECHNOLOGY == Technology.NR.ToString() && n.Band == ((int)Band.B21).ToString()).Select(n => n.CellName).Distinct().Count();

                    sector.NR_3500 = (int)filterLstTech
                     .Where(n => n.LAYER_TECHNOLOGY == Technology.NR.ToString() && n.Band == ((int)Band.B35).ToString()).Select(n => n.CellName).Distinct().Count();

                }
            }

            return sectors;
        }

        public override List<Port> PortLoad(string sector, string antennaType, decimal phyIndex)
        {
            var allTech = TechLoad(sector, antennaType, phyIndex);

            var portGrouping = new PortGrouping(allTech);

            var arrangedPorts = portGrouping.PortArrange(siteID);

            foreach (var port in arrangedPorts)
            {
                if (port.Status == "Free")
                    continue;

                port.ModelRRUs = port.ModelRRUs.OrderBy(n => n.Band).ToList();

                port.GSM_TRX = port.ModelRRUs.GroupBy(n => n.CellName).SelectMany(n => n).Max(n => n.GSM_TRX);
                port.UMTS_TRX = port.ModelRRUs.GroupBy(n => n.CellName).SelectMany(n => n).Max(n => n.UMTS_TRX);
                port.LTE_TRX = port.ModelRRUs.GroupBy(n => n.CellName).SelectMany(n => n).Max(n => n.LTE_TRX);

                port.RRU_Total = port.ModelRRUs.Select(n => n.RRU_SN).Distinct().Count().ToString();

                if (this.IsRrusFromOss)
                    port.RRU_Type = SupportFunc.MapCmRruWithDropDownRru(port.ModelRRUs).Trim();
                else
                {
                    var rrusWithoutReuse = port.ModelRRUs.Where(n => !n.RRU_Type.Contains("REUSE"));

                    foreach (var rru in rrusWithoutReuse)
                        rru.RRU_Type = SupportFunc.ConcatRruWithBand(rru.RRU_Type, rru.Band);

                    port.RRU_Type = String.Join(" ", rrusWithoutReuse.Select(n => n.RRU_Type));

                }

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
