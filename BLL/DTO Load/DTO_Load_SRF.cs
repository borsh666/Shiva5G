using BLL.DTO;
using BLL.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BLL
{
    public class DTO_Load_SRF : DTO_Load
    {
        public DTO_Load_SRF(string siteID) : base(siteID)
        {
            this.IsRrusFromOss = true;
        }

        public override void QueryMaterialize()
        {
            base.QueryMaterialize();

            var watch = new Stopwatch();
            watch.Start();
            
            this.LstLoadRRU_CM = base.LoadRRU_Oss();

            var mapTechnoly = SupportFunc.MapAssetTechWithCmTech(this.lstTechnology, this.LstLoadRRU_CM.Select(n => n.CellName));
            
            if (mapTechnoly.Count == 0)
                throw new Exception("No data in CM for this site!");
          
            this.LstLoadPowerTRX_CM = base.LoadPowerTRX_Oss();

            this.lstTechnology = mapTechnoly;

        }

        //New
        public override List<Sector> Sector()
        {
            var sectors = base.Sector();

            foreach (var sector in sectors)
            {
                sector.GSM_900 = LstLoadPowerTRX_CM
                      .Where(n => n.Sector == sector.SectorNumb && n.Technology == Technology.G.ToString() && n.Band == ((int)Band.B9).ToString())
                      .Sum(n => (n.GSM_TRX));
                sector.GSM_1800 = LstLoadPowerTRX_CM
                    .Where(n => n.Sector == sector.SectorNumb && n.Technology == Technology.G.ToString() && n.Band == ((int)Band.B18).ToString())
                    .Sum(n => (n.GSM_TRX));
                sector.UMTS_900 = LstLoadPowerTRX_CM
                    .Where(n => n.Sector == sector.SectorNumb && n.Technology == Technology.U.ToString() && n.Band == ((int)Band.B9).ToString())
                    .Select(n => n.CellName).Distinct().Count();
                sector.UMTS_2100 = LstLoadPowerTRX_CM
                    .Where(n => n.Sector == sector.SectorNumb && n.Technology == Technology.U.ToString() && n.Band == ((int)Band.B21).ToString())
                    .Select(n => n.CellName).Distinct().Count();
                sector.LTE_900 = LstLoadPowerTRX_CM
                    .Where(n => n.Sector == sector.SectorNumb && n.Technology == Technology.L.ToString() && n.Band == ((int)Band.B9).ToString())
                    .Select(n => n.CellName).Distinct().Count();
                sector.LTE_1800 = LstLoadPowerTRX_CM
                    .Where(n => n.Sector == sector.SectorNumb && n.Technology == Technology.L.ToString() && n.Band == ((int)Band.B18).ToString())
                    .Select(n => n.CellName).Distinct().Count();
                sector.LTE_2100 = LstLoadPowerTRX_CM
                  .Where(n => n.Sector == sector.SectorNumb && n.Technology == Technology.L.ToString() && n.Band == ((int)Band.B21).ToString())
                  .Select(n => n.CellName).Distinct().Count();
                sector.LTE_2600 = LstLoadPowerTRX_CM
                 .Where(n => n.Sector == sector.SectorNumb && n.Technology == Technology.L.ToString() && n.Band == ((int)Band.B26).ToString())
                 .Select(n => n.CellName).Distinct().Count();
                sector.NR_1800 = LstLoadPowerTRX_CM
                   .Where(n => n.Sector == sector.SectorNumb && n.Technology == Technology.NR.ToString() && n.Band == ((int)Band.B18).ToString())
                   .Select(n => n.CellName).Distinct().Count();
                sector.NR_2100 = LstLoadPowerTRX_CM
                  .Where(n => n.Sector == sector.SectorNumb && n.Technology == Technology.NR.ToString() && n.Band == ((int)Band.B21).ToString())
                  .Select(n => n.CellName).Distinct().Count();
                sector.NR_3500 = LstLoadPowerTRX_CM
                 .Where(n => n.Sector == sector.SectorNumb && n.Technology == Technology.NR.ToString() && n.Band == ((int)Band.B35).ToString())
                 .Select(n => n.CellName).Distinct().Count();

            }

            return sectors;
        }

        public override List<Port> PortLoad(string sector, string antennaType, decimal phyIndex)
        {

            var allTech = this.TechLoad(sector, antennaType, phyIndex);

            var portGrouping = new PortGrouping(allTech);

            var arrangedPorts = portGrouping.PortArrange(siteID);


            foreach (var port in arrangedPorts)
            {
                if (port.Status == "Free")
                    continue;

                if (port.ModelRRUs.Count() < 1)
                {
                    port.Request_Remarks += $"Не са намерени в СМ данни за {port.Technology}.";
                    continue;
                }

                port.ModelRRUs = port.ModelRRUs.OrderBy(n => n.Band).ToList();

                port.GSM_TRX = port.ModelRRUs.GroupBy(n => n.CellName).SelectMany(n => n).Max(n => n.GSM_TRX);
                port.UMTS_TRX = port.ModelRRUs.GroupBy(n => n.CellName).SelectMany(n => n).Max(n => n.UMTS_TRX);
                port.LTE_TRX = port.ModelRRUs.GroupBy(n => n.CellName).SelectMany(n => n).Max(n => n.LTE_TRX);
                              
                port.RRU_Total = port.ModelRRUs.Select(n => n.RRU_SN).Distinct().Count().ToString();

                port.RRU_Type = SupportFunc.MapCmRruWithDropDownRru(port.ModelRRUs).Trim();

                if (port.RRU_Type == string.Empty)
                    port.Request_Remarks += $"In CM no data for RRU Type or RRU SN.Check table HWI.cmsr.RRUsPerNodes";

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
