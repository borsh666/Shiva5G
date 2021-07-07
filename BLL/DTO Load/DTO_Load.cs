using BLL.DTO;
using OfficeOpenXml.FormulaParsing.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BLL
{
    public abstract class DTO_Load
    {
        protected List<ViewModelTechnology> lstTechnology;
        protected string siteStructureType;
        protected string siteInstallType;
        protected string siteOwner;
        protected string siteCoLocation;
        protected decimal latitude;
        protected decimal longitude;
        protected string siteID;
        protected bool isSiteSRAN;
        protected Queries query;

        public Dictionary<string, string> Info { get; set; }
        public bool IsRrusFromOss { get; set; }
        public List<ModelRRU> LstLoadPowerTRX_CM;
        public List<ModelRRU> LstLoadRRU_CM;


        public DTO_Load(string siteID, bool isSiteSRAN)
        {
            Info = new Dictionary<string, string>();

            this.siteID = siteID;

            CheckForMoreThanOneCandidate();

            this.isSiteSRAN = isSiteSRAN;

            QueryMaterialize();
        }

        private void CheckForMoreThanOneCandidate()
        {
            query = new Queries(this.siteID);
            var gsmCheck = query.CheckForMoreThanOneCandidateGSM();
            var umtsCheck = query.CheckForMoreThanOneCandidateUMTS();
            var lteCheck = query.CheckForMoreThanOneCandidateLTE();

            if (gsmCheck > 1)
                throw new ArgumentException("Имате повече от едно Property - GSM кандидат със статус 'Candidate' = 'Accepted'");
            if (umtsCheck > 1)
                throw new ArgumentException("Имате повече от едно Property - UMTS кандидат със статус 'Candidate' = 'Accepted'");
            if (lteCheck > 1)
                throw new ArgumentException("Имате повече от едно Property - LTE кандидат със статус 'Candidate' = 'Accepted'");

        }

        public List<ModelRRU> LoadPowerTRX_Oss()
        {
            var watch = new Stopwatch();
            watch.Start();
            List<ModelRRU> lstLoadPowerTRX_CM = query.Get2GSiteCM()
                .Union(query.Get3GSiteCM()).ToList()
                .Union(query.Get4GSiteCM()).ToList()
                .Union(query.Get5GSiteCM()).ToList();
            Info.Add("lstLoadPowerTRX_CM()", watch.Elapsed.TotalSeconds.ToString());

            for (int i = 0; i < lstLoadPowerTRX_CM.Where(n => n.Technology == "G").Count(); i++)
            {
                var intBand = int.Parse(lstLoadPowerTRX_CM[i].Band);
                if (intBand < 660 || intBand > 770)
                    lstLoadPowerTRX_CM[i].Band = "900";
                else
                    lstLoadPowerTRX_CM[i].Band = "1800";
            }

            watch.Stop();

            return lstLoadPowerTRX_CM;
        }

        public List<ModelRRU> LoadRRU_Oss()
        {
            var watch = new Stopwatch();
            watch.Start();
            List<ModelRRU> lstLoadRRU_CM = query.GetCMallTechRRU_ver2().ToList();
            watch.Stop();

            return lstLoadRRU_CM;
        }

        public virtual void QueryMaterialize()
        {
            var watch = new Stopwatch();
            watch.Start();

            query = new Queries(siteID);

            this.lstTechnology = query.GetAllTechAsset();
            Info.Add("GetAllTechAsset()", watch.Elapsed.TotalSeconds.ToString());
            watch.Restart();

            this.latitude = query.GetSiteLat();
            this.longitude = query.GetSiteLong();
            Info.Add("GetSiteLat/Long()", watch.Elapsed.TotalSeconds.ToString());
            watch.Restart();


            query.GetSiteStructInstallType(out siteStructureType, out siteInstallType);
            Info.Add("GetSiteStructInstallType()", watch.Elapsed.TotalSeconds.ToString());
            watch.Restart();

            query.GetSiteCoLocationOwnership(out siteOwner, out siteCoLocation);
            Info.Add("GetSiteCoLocationOwnership()", watch.Elapsed.TotalSeconds.ToString());
            watch.Restart();


        }

        public Site Site()
        {

            var site = new Site();

            site.Latitude = this.latitude;
            site.Longitude = this.longitude;
            site.StructureType = this.siteStructureType;
            site.InstallationType = this.siteInstallType;
            site.SiteOwner = this.siteOwner;
            site.ColocationType = this.siteCoLocation;
            site.SiteID = siteID;

            var address = lstTechnology.Select(n => new { Address = n.SiteAddress + " " + n.SiteAddress1 + " " + n.SiteAddress2 })
                 .FirstOrDefault();
            if (address != null)
                site.Address = address.ToString();
            else
                site.Address = String.Empty;

            if (site.Address.Contains("NOVAL"))
                site.Address = site.Address.Replace("NOVAL", "").Replace("{", "").Replace("}", "").Replace("Address =", "").Trim();

            site.Candidate = lstTechnology.Select(n => n.Candidate).FirstOrDefault();
            site.SiteName = lstTechnology.Select(n => n.SiteName).FirstOrDefault();
            site.StructureHeight = lstTechnology.Select(n => n.AGL).FirstOrDefault().ToString();
            site.BSC = lstTechnology.Where(n => n.Controler.Contains("BSC")).Select(n => n.Controler).FirstOrDefault();
            site.RNC = lstTechnology.Where(n => n.Controler.Contains("RNC")).Select(n => n.Controler).FirstOrDefault();
            site.Sectors = Sector();
            site.OffsetExcel = 1;

            return site;
        }

        //New
        public virtual List<Sector> Sector()
        {
            var sectors = new List<Sector>();

            var checkTech = lstTechnology.Max(n => n.Sector);
            if (checkTech == null)
                throw new Exception("Please check for technology/layer definition in Asset.");

            int totalSectors = 0;
            if (!int.TryParse(checkTech, out totalSectors))
                throw new FormatException($"Please check idname {lstTechnology.Max(n => n.CellName)}!");

            for (int i = 1; i <= totalSectors; i++)
            {
                var sector = new Sector();
                var currentSector = lstTechnology.Where(n => int.Parse(n.Sector) == i);

                sector.SectorNumb = i.ToString();
                sector.Azimuth = currentSector.Select(n => n.Azimuth).FirstOrDefault().ToString();

                sector.Antennas = Antenna().Where(n => n.SectorNumber == i.ToString())
                    .OrderBy(n => n.PhyIndex).ToList();


                //New
                if (sector.Antennas.Count > Report.AntennaOffsetSteps.Count)
                    sector.Antennas = sector.Antennas.Take(Report.AntennaOffsetSteps.Count).ToList();

                //Set Antenna/ Port Offsets
                for (int j = 0; j < sector.Antennas.Count(); j++)
                {
                    sector.Antennas[j].OffsetExcel = Report.AntennaOffsetSteps[j];

                    if (sector.Antennas[j].Ports.Count() > Report.MaxAllowedPorts[j])
                        sector.Antennas[j].Ports = sector.Antennas[j].Ports.Take(Report.MaxAllowedPorts[j]).ToList();

                    foreach (var port in sector.Antennas[j].Ports)
                        port.OffsetExcel = Report.PortOffsetSteps[j] + port.BandPosition;
                }

                sector.OffsetExcel = i;

                sectors.Add(sector);
            }


            return sectors;
        }

        public List<Antenna> Antenna()
        {
            var antennaGroup = lstTechnology.GroupBy(n => new
            {
                SectorNumber = n.Sector,
                Azimuth = n.Azimuth,
                AntenaType = n.AntennaType,
                PhyIndex = (decimal)n.PHYINDEX,

            })
            .Select(n => new Antenna
            {
                SectorNumber = n.Key.SectorNumber,
                Azimuth = n.Key.Azimuth,
                AntennaType = n.Key.AntenaType,
                PhyIndex = n.Key.PhyIndex,
                AntennaMount = n.FirstOrDefault().ANTENNA_MOUNT,
                AGL = Math.Round((decimal)n.FirstOrDefault().AGL, 0),
                ARTL = (decimal)n.FirstOrDefault().ARTL,
                MechanicalTilt = (decimal)n.FirstOrDefault().MECHANICAL_TILT,
                Ports = PortLoad(n.Key.SectorNumber, n.Key.AntenaType, n.Key.PhyIndex)

            }).ToList();



            return antennaGroup;

        }

        public abstract List<Port> PortLoad(string sector, string antennaType, decimal phyIndex);

        public abstract List<Port> TechLoad(string sector, string antennaType, decimal phyIndex);

        public virtual List<Port> ViewModelTechnologyToPort(List<ViewModelTechnology> viewModelTech)
        {
            var ports = new List<Port>();

            foreach (var tech in viewModelTech)
            {
                var objTech = new Port();
                objTech.SectorNumber = tech.Sector;
                objTech.AntennaType = tech.AntennaType;
                objTech.PhyIndex = (decimal)tech.PHYINDEX;
                objTech.Band = tech.Band;
                objTech.Technology = tech.LAYER_TECHNOLOGY;
                objTech.CellName = tech.CellName;
                objTech.Etilt = tech.Etilt != null ? tech.Etilt.ToString() : "";
                objTech.Feeder_Length = tech.FEEDERLENGTH.ToString();
                objTech.RET = "Yes";
                objTech.RRU_Type = tech.RRU_Type;

                objTech.ModelRRUs = new List<ModelRRU>();

                if (this.IsRrusFromOss)
                {
                    var filterLoadPowerTRX_CM = this.LstLoadPowerTRX_CM
                    .Where(n => n.Sector == objTech.SectorNumber && n.Technology == objTech.Technology
                        && objTech.Band == n.Band && n.CellName == objTech.CellName);


                    var filterLoadRRU_CM = this.LstLoadRRU_CM
                        .Where(n => n.Technology == objTech.Technology && objTech.Band == n.Band.ToString() && n.CellName.ToString() == objTech.CellName);

                    foreach (var rru in filterLoadRRU_CM)
                    {
                        if (string.IsNullOrEmpty(rru.RRU_Type))
                            objTech.Request_Remarks += $"For tech {objTech.Technology} , band {objTech.Band} there is no RRU_Type in table RRUsPerNodes";

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
                }
                else
                {
                    objTech.ModelRRUs.Add(

                  new ModelRRU
                  {
                      RRU_Type = tech.RRU_Type,
                      Technology = tech.LAYER_TECHNOLOGY,
                      Band = tech.Band,
                      CellName = tech.CellName,
                      GSM_TRX = tech.GSM_TRX != null ? (int)tech.GSM_TRX : 0,
                      UMTS_TRX = tech.UMTS_TRX != null ? (int)tech.UMTS_TRX : 0,
                      LTE_TRX = tech.LTE_TRX != null ? (int)tech.LTE_TRX : 0,
                      NR_TRX = tech.NR_TRX != null ? (int)tech.NR_TRX : 0,
                      GSM_Pwr_per_TRX = tech.GSM_Pwr_per_TRX != null ? tech.GSM_Pwr_per_TRX.ToString() : "",
                      UMTS_Pwr_per_TRX = tech.UMTS_Pwr_per_TRX != null ? tech.UMTS_Pwr_per_TRX.ToString() : "",
                      LTE_Pwr_per_TRX = tech.LTE_Pwr_per_TRX != null ? tech.LTE_Pwr_per_TRX.ToString() : "",
                      NR_Pwr_per_TRX = tech.NR_Pwr_per_TRX != null ? tech.NR_Pwr_per_TRX.ToString() : "",
                  }
                  );
                    
                }


                if (!string.IsNullOrEmpty(tech.TMA))
                    objTech.TMA = tech.TMA;
                objTech.Collocation = tech.CoLocation;

                objTech.Feeder_Type = SupportFunc.FeederTypeMap(tech.FEEDERTYPE);

                objTech.Combiner_Splitter = tech.COMBINER_SPLITTER;
                objTech.Sec_Combiner_Splitter = tech.SEC_COMBINER_SPLITTER;

          
                if (decimal.TryParse(objTech.Feeder_Length, out decimal feederLength))
                    objTech.Feeder_Length = Decimal.ToInt16(feederLength).ToString();

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
                var addedPorts = SplitIfNeedTechWhenBelongToMoreThanOnePort(tech, objTech);
                ports.AddRange(addedPorts);

            };

            return ports;
        }

        public List<Port> SplitIfNeedTechWhenBelongToMoreThanOnePort(ViewModelTechnology tech, Port port)
        {
            //One Tech  for more than one port!!!!

            if (port.ModelRRUs.Count == 0)
                throw new ArgumentNullException($"There is  no cell:{tech.CellName} with band {tech.Band}  in CM! Check tables: " +
                    $"CMSR.RRUsPerNodes, CM4G.LCELL, CM.UCELLSETUP_SRAN and CM2G.GBTS_power");

            var addedPorts = new List<Port>();

            var isTechsForMoreThanOnePort = tech.PortNumber.Contains(",");
            if (isTechsForMoreThanOnePort)
            {

                var rruCombination = new Dictionary<string, string[]>
                {
                    { "900", new string[]  { "CD", "AB", } }   ,
                    { "1800", new string[] { "AC", "DB", } }   ,
                    { "2100", new string[] { "AC", "DB", } }   ,
                    { "2600", new string[] { "AC", "DB", } }   ,
                };



                int currentInput = 0;
                var orderedPorts = tech.PortNumber.Split(',').OrderBy(n => n.Trim());


                foreach (var portNumber in orderedPorts)
                {
                    var clonePort = port.DeepCopy();
                    clonePort.PortNumber = int.Parse(portNumber);

                    if (clonePort.RRU_Type != "REUSE")
                    {
                        foreach (var rru in clonePort.ModelRRUs)
                        {
                            var newRruName = rru.RRU_Type + rruCombination[clonePort.Band][currentInput];
                            rru.RRU_Type = newRruName;
                        }
                        clonePort.RRU_Type = string.Join(",", clonePort.ModelRRUs.Distinct());
                    }

                    currentInput++;

                    addedPorts.Add(clonePort);
                }


            }
            else
            {
                if (tech.PortNumber != "NOVAL")
                    port.PortNumber = int.Parse(tech.PortNumber);
                addedPorts.Add(port);
            }

            return addedPorts;
        }

    }
}
