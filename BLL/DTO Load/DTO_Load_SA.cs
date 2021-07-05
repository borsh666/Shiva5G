using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;
using System.Diagnostics;

namespace BLL
{
    public class DTO_Load_SA : DTO_Load
    {
        private List<ModelRRU> lstLoadPowerTRX_CM;
        private List<ModelRRU> lstLoadRRU_CM;


        public DTO_Load_SA(string siteID, bool isSiteSRAN) : base(siteID, isSiteSRAN)
        {
            this.IsRrusFromOss = false;
        }

        public override void QueryMaterialize()
        {
            base.QueryMaterialize();

            var watch = new Stopwatch();
            watch.Start();

            this.lstLoadPowerTRX_CM = query.Get2GSiteCM().Union(query.Get3GSiteCM()).ToList().Union(query.Get4GSiteCM()).ToList();
            Info.Add("lstLoadPowerTRX_CM()", watch.Elapsed.TotalSeconds.ToString());
            watch.Restart();

            this.lstLoadRRU_CM = query.GetCMallTechRRU_ver2().ToList();
            Info.Add("lstLoadRRU_CM()", watch.Elapsed.TotalSeconds.ToString());
            watch.Restart();

            for (int i = 0; i < lstLoadPowerTRX_CM.Where(n => n.Technology == "G").Count(); i++)
            {
                var intBand = int.Parse(lstLoadPowerTRX_CM[i].Band);
                if (intBand < 660 || intBand > 770)
                    lstLoadPowerTRX_CM[i].Band = "900";
                else
                    lstLoadPowerTRX_CM[i].Band = "1800";
            }

            this.lstTechnology = SupportFunc.MapAssetTechWithCmTech(this.lstTechnology, lstLoadRRU_CM.Select(n => n.CellName));
        }

        //New
        public override List<Sector> Sector()
        {
            var sectors = base.Sector();

            foreach (var sector in sectors)
            {
                sector.GSM_900 = lstLoadPowerTRX_CM
                       .Where(n => n.Sector == sector.SectorNumb && n.Technology == "G" && n.Band == "900")
                       .Sum(n => (n.GSM_TRX));
                sector.GSM_1800 = lstLoadPowerTRX_CM
                    .Where(n => n.Sector == sector.SectorNumb && n.Technology == "G" && n.Band == "1800")
                    .Sum(n => (n.GSM_TRX));
                sector.UMTS_900 = lstLoadPowerTRX_CM
                    .Where(n => n.Sector == sector.SectorNumb && n.Technology == "U" && n.Band == "900")
                    .Select(n => n.CellName).Distinct().Count();
                sector.UMTS_2100 = lstLoadPowerTRX_CM
                    .Where(n => n.Sector == sector.SectorNumb && n.Technology == "U" && n.Band == "2100")
                    .Select(n => n.CellName).Distinct().Count();
                sector.LTE_900 = lstLoadPowerTRX_CM
                    .Where(n => n.Sector == sector.SectorNumb && n.Technology == "L" && n.Band == "900")
                    .Select(n => n.CellName).Distinct().Count();
                sector.LTE_1800 = lstLoadPowerTRX_CM
                    .Where(n => n.Sector == sector.SectorNumb && n.Technology == "L" && n.Band == "1800")
                    .Select(n => n.CellName).Distinct().Count();
                sector.LTE_2100 = lstLoadPowerTRX_CM
                    .Where(n => n.Sector == sector.SectorNumb && n.Technology == "L" && n.Band == "2100")
                    .Select(n => n.CellName).Distinct().Count();
                sector.LTE_2600 = lstLoadPowerTRX_CM
                    .Where(n => n.Sector == sector.SectorNumb && n.Technology == "L" && n.Band == "2600")
                    .Select(n => n.CellName).Distinct().Count();
                sector.NR_1800 = lstLoadPowerTRX_CM
                   .Where(n => n.Sector == sector.SectorNumb && n.Technology == "NR" && n.Band == "1800")
                   .Select(n => n.CellName).Distinct().Count();
                sector.NR_2100 = lstLoadPowerTRX_CM
                   .Where(n => n.Sector == sector.SectorNumb && n.Technology == "NR" && n.Band == "2100")
                   .Select(n => n.CellName).Distinct().Count();
                sector.NR_3500 = lstLoadPowerTRX_CM
                   .Where(n => n.Sector == sector.SectorNumb && n.Technology == "NR" && n.Band == "3500")
                   .Select(n => n.CellName).Distinct().Count();
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

                if (port.ModelRRUs.Count() < 1)
                {
                    port.Request_Remarks += $"Не са намерени в СМ данни за {port.Technology}.";
                    continue;
                }

                foreach (var rru in port.ModelRRUs)
                {
                    if (rru.GSM_Pwr_per_TRX == null && rru.UMTS_Pwr_per_TRX == null && rru.LTE_Pwr_per_TRX == null)
                        port.Request_Remarks += $"Band {rru.Band} Cell {rru.CellName} RRU {rru.RRU_Type} is deactivated{Environment.NewLine}";
                }


                port.RRU_Total = port.ModelRRUs.Select(n => n.RRU_SN).Distinct().Count().ToString();

                port.RRU_Type = SupportFunc.MapCmRruWithDropDownRru(port.ModelRRUs).Trim();

                if (port.RRU_Type == string.Empty)
                    port.Request_Remarks += $"In CM no data for RRU Type or RRU SN.Check table HWI.cmsr.RRUsPerNodes";

                //var groupByRRU_SN = port.ModelRRUs.GroupBy(n => n.RRU_SN)
                //    .Select(n => new ModelRRU
                //    {
                //        GSM_TRX = n.Sum(k => k.GSM_TRX),
                //        UMTS_TRX = n.Sum(k => k.UMTS_TRX),
                //        LTE_TRX = n.Sum(k => k.LTE_TRX),
                //        RRU_Type = n.Select(k => k.RRU_Type).FirstOrDefault()
                //    }).ToList();

                ////When we have only one RRU
                //if (groupByRRU_SN.Count() == 1)
                //    port.RRU_Type = groupByRRU_SN.Select(n => n.RRU_Type).FirstOrDefault();
                //else
                //{
                //    var uniqueRRU = groupByRRU_SN.Select(n => n.RRU_Type).Distinct().Count();
                //    if (uniqueRRU == 1)
                //        port.RRU_Type = "2x" + groupByRRU_SN.Select(n => n.RRU_Type).FirstOrDefault();
                //    else
                //        port.RRU_Type = String.Join(" ", groupByRRU_SN.Select(n => n.RRU_Type));
                //}
            }

            return arrangedPorts;

        }

        public override List<Port> TechLoad(string sector, string antennaType, decimal phyIndex)
        {

            var technologies = new List<Port>();

            var filterTechnology = this.lstTechnology.Where(n => n.Sector == sector && n.AntennaType == antennaType && n.PHYINDEX == phyIndex);


            foreach (var tech in filterTechnology)
            {
                var objTech = new Port();
                objTech.SectorNumber = tech.Sector;
                objTech.AntennaType = tech.AntennaType;
                objTech.PhyIndex = (decimal)tech.PHYINDEX;
                objTech.Band = tech.Band;
                objTech.Technology = tech.LAYER_TECHNOLOGY;
                objTech.CellName = tech.CellName;
                objTech.Etilt = tech.Etilt.ToString();
                objTech.Feeder_Length = tech.FEEDERLENGTH.ToString();
                //objTech.RET = "Yes";


                var filterLoadPowerTRX_CM = lstLoadPowerTRX_CM
                    .Where(n => n.Sector == objTech.SectorNumber && n.Technology == objTech.Technology && objTech.Band == n.Band && n.CellName == objTech.CellName);


                var filterLoadRRU_CM = lstLoadRRU_CM
                    .Where(n => n.Technology == objTech.Technology && objTech.Band == n.Band.ToString() && n.CellName.ToString() == objTech.CellName);

                objTech.ModelRRUs = new List<ModelRRU>();

                foreach (var rru in filterLoadRRU_CM)
                {
                    var rruObj = new ModelRRU()
                    {
                        RRU_Type = rru.RRU_Type,
                        RRU_SN = rru.RRU_SN,
                        Technology = rru.Technology,
                        Band = tech.Band,
                        CellName = rru.CellName,
                        GSM_TRX = filterLoadPowerTRX_CM.Select(n => n.GSM_TRX).Sum(),
                        UMTS_TRX = filterLoadPowerTRX_CM.Select(n => n.UMTS_TRX).FirstOrDefault(),
                        LTE_TRX = filterLoadPowerTRX_CM.Select(n => n.LTE_TRX).FirstOrDefault(),
                        NR_TRX = filterLoadPowerTRX_CM.Select(n => n.NR_TRX).FirstOrDefault(),
                        GSM_Pwr_per_TRX = filterLoadPowerTRX_CM.Select(n => n.GSM_Pwr_per_TRX).FirstOrDefault(),
                        UMTS_Pwr_per_TRX = filterLoadPowerTRX_CM.Select(n => n.UMTS_Pwr_per_TRX).FirstOrDefault(),
                        LTE_Pwr_per_TRX = filterLoadPowerTRX_CM.Select(n => n.LTE_Pwr_per_TRX).FirstOrDefault(),
                        NR_Pwr_per_TRX = filterLoadPowerTRX_CM.Select(n => n.NR_Pwr_per_TRX).FirstOrDefault(),
                    };
                    objTech.ModelRRUs.Add(rruObj);
                }

                objTech.TMA = tech.TMA;
                objTech.Collocation = tech.CoLocation;

                objTech.Feeder_Type = SupportFunc.FeederTypeMap(tech.FEEDERTYPE);

                objTech.Combiner_Splitter = tech.COMBINER_SPLITTER;
                objTech.Sec_Combiner_Splitter = tech.SEC_COMBINER_SPLITTER;


                var forComp = objTech.Technology + objTech.Band + " " + objTech.Feeder_Length + " " + objTech.Feeder_Type;
                var reference = SupportFunc.InitialPowerCalc(Properties.Resources.JumperAttdB);

                //Jumper_Att_dB
                if (reference.ContainsKey(forComp))
                    objTech.Feeder_Att_dB = reference[forComp];

                //Combiner_Splitter_Loss
                reference = SupportFunc.InitialPowerCalc(Properties.Resources.Combiner_Splitter_Loss);
                if (reference.ContainsKey(objTech.Combiner_Splitter))
                    objTech.Combiner_Splitter_Loss = reference[objTech.Combiner_Splitter];

                //Second_Combiner_Splitter_Loss
                if (reference.ContainsKey(objTech.Sec_Combiner_Splitter))
                    objTech.Second_Combiner_Splitter_Loss = reference[objTech.Sec_Combiner_Splitter];


                //One Tech  for more than one port!!!!
                var addedPorts = base.SplitIfNeedTechWhenBelongToMoreThanOnePort(tech, objTech);
                technologies.AddRange(addedPorts);

            };

            return technologies;
        }
    }
}
