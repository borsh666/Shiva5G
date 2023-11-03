using BLL_Atoll.Enums;
using BLL_Atoll.Interfaces;
using BLL_Atoll.Models.Site;
using static BLL_Atoll.SupportFunc;


namespace BLL_Atoll
{
    public class Engine
    {
        private readonly string siteID;
        private readonly List<dynamic> feederLossFromDb = new();
        private readonly List<dynamic> elementsFromDb = new();
        private readonly List<dynamic> antennaPortMap = new();
        private readonly List<dynamic> elementsSecRemoteEmptyAntFromDb = new();

        public static List<Cell> Cells = new();
        public static Dictionary<int, List<string>> Errs = new();


        public static Report Report { get; set; }


        public Engine(string siteID, Report report)
        {
            this.siteID = siteID.Trim();
            Engine.Report = report;
            feederLossFromDb = LoadFeederLost();

            elementsFromDb = LoadElements();

            antennaPortMap = LoadAntennaPortMap();

            elementsSecRemoteEmptyAntFromDb = LoadSecRemoteEmptyAntennas();

            Errs = new();

            for (int sector = 1; sector <= Report.MaxAllowedSectors; sector++)
                Errs.Add(sector, new());

        }

        public List<dynamic> LoadElements()
        {
            CheckForCorrectSiteName(this.siteID);

            var query = File.ReadAllText(Global.QUERY_PATH_ALLELEMENTS);
            query = query.Replace(Global.PATTERN_SITEID, this.siteID);
            query = query.Replace(Global.PATTERN_MAX_SECTORS, Report.MaxAllowedSectors.ToString());
            query = query.Replace(Global.PATTERN_MAX_ANTENNAS, Report.MaxAllowedAntennas.ToString());

            query = query.Replace(Global.PATTERN_JOIN_LEFT_RIGHT,
                Global.REPORT_TYPE_VS_RRU_JOIN[Engine.Report.Document]);

            List<dynamic>? result;


            if (query is not null)
                result = GetFromDb(query, Global.CONN_STR_ATOLL);
            else
                throw new Exception($"Not such site {this.siteID}");

            //result = result.OrderBy(n => n.SectorId)
            //               .ThenBy(n => n.Azimuth)
            //               .ToList();

            //result.ForEach(n => Console.WriteLine($"{n.CellName} {n.SectorId} {n.Azimuth}"));

            ApplyRuleForSmallSectorsDegree30(ref result);
            //Console.WriteLine("-----------------------");
            //result.ForEach(n => Console.WriteLine($"{n.CellName} {n.SectorId} {n.Azimuth}"));

            return result;

        }

        public List<dynamic> LoadSecRemoteEmptyAntennas()
        {
            var query = File.ReadAllText(Global.QUERY_SEC_REMOTE_EMPTY_ANTENNA);
            query = query.Replace(Global.PATTERN_SITEID, this.siteID);

            var result = GetFromDb(query, Global.CONN_STR_SHIVAL);
            return result;

        }

        private static void ApplyRuleForSmallSectorsDegree30(ref List<dynamic> elements)
        {
            var azimuths = elements.Select(n => (int)n.Azimuth)
                .OrderBy(n => n)
                .ToList();


            int SmallAzimuth(int azimuth, int currentAzumith)
            {
                if ((currentAzumith - azimuth <= 30)
                   && (currentAzumith - azimuth > 0))
                    return azimuth;

                return -1;
            }

            foreach (var element in elements)
            {
                foreach (dynamic azimuth in azimuths)
                {
                    dynamic resultAzimuth = SmallAzimuth((int)azimuth, (int)element.Azimuth);
                    if (resultAzimuth > -1)
                    {
                        element.Azimuth = resultAzimuth;
                        element.SectorId = elements.Where(n => (int)n.Azimuth == resultAzimuth)
                                                   .First().SectorId;
                        break;
                    }

                }
            }
        }

        private List<dynamic> LoadFeederLost()
        {
            var query = File.ReadAllText(Global.QUERY_PATH_FEEDERLOSS);

            if (query is not null)
                return GetFromDb(query, Global.CONN_STR_SHIVAL);
            else
                throw new Exception($"Not such site {this.siteID}");
        }

        private static List<dynamic> LoadAntennaPortMap()
        {
            var query = File.ReadAllText(Global.QUERY_ANTENNA_PORT_MAP);

            if (query is not null)
                return GetFromDb(query, Global.CONN_STR_SHIVAL);
            else
                throw new Exception($"Error in antenna port query");
        }

        private List<Rru> LoadRrus()
        {
            var rrus = MapElement<Rru>(elementsFromDb);
            rrus.ForEach(o => o.Errors());
            return rrus.Where(n => n.Status != Status.NotOk).ToList();
        }

        private List<Cell> LoadCells()
        {
            Cells = MapElement<Cell>(elementsFromDb);
            Cells.ForEach(o => o.CalcPwrPerTrx());
            Cells.ForEach(o => o.CalcFeederLoss(feederLossFromDb));
            Cells.ForEach(o => o.Errors());
            return Cells.Where(n => n.Status != Status.NotOk).ToList();
        }

        private List<Antenna> LoadAntennas()
        {
            var antennas = MapElement<Antenna>(elementsFromDb);

            antennas.ForEach(o => o.Errors());
            antennas = antennas.Where(n => n.Status == Status.Ok).ToList();

            var antennasGroup = antennas
                .GroupBy(o => new { o.AntennaId })
                .Select(o => o.FirstOrDefault()).ToList();

            return antennasGroup!;
        }

        private List<Antenna> AddSecRemoteEmptyAntennas(List<Antenna> antennas)
        {
            var antennasGroup = new List<Antenna>();

            var antennasSecRemoteEmpty = MapElement<Antenna>(elementsSecRemoteEmptyAntFromDb);

            if (antennasSecRemoteEmpty.Any())
            {
                antennasSecRemoteEmpty
                    .ForEach(o => o.AddAntennaIdForSecRemoteEmptyAntenna(antennas));
                antennasGroup = antennasSecRemoteEmpty
                    .Where(n => n.Status == Status.Ok)
                    .GroupBy(o => new { o.AntennaId })
                    .Select(o => o.First()).ToList();
            }
            antennasGroup.ForEach(o => o.Errors());
            return antennasGroup;
        }

        private List<Port> LoadPorts(List<Rru> rrus, List<Antenna> antennas)
        {
            static List<Port> portsGroupByAntPort(List<Port> ports) =>
                ports
                .GroupBy(o => new { o.AntennaId, o.PortId })
                .Select(o => o.First())
                .OrderBy(n => n.SectorId)
                .ThenBy(n => n.AntennaId)
                .ThenBy(n => n.PortId)
                .ToList();

            var portsFromElements = MapElement<Port>(elementsFromDb);

            var portsGroup = portsGroupByAntPort(portsFromElements);
            portsGroup.ForEach(n => n.GetTech());
            portsGroup.ForEach(n => n.GetBand());
            portsGroup.ForEach(n => n.GetFeederType());
            portsGroup.ForEach(n => n.CalcTrxPowerAndFeederLost());
            portsGroup.ForEach(n => n.CalcAntennaInTotalPower(rrus));
            portsGroup.ForEach(n => n.CalcRruTypeAndCount(rrus));

            //Empty Ports
            var emptyPorts = antennas
                .SelectMany(o => o.GetEmptyPorts(portsGroup, antennaPortMap));
            portsGroup.AddRange(emptyPorts);

            //5519et Exception
            portsGroup.ForEach(n => n.AddRru5519etException(rrus));

            portsGroup.ForEach(o => o.Errors());

            var portsForPrint = portsGroup
                .Where(n => n.Status != Status.NotOk)
                .ToList();

            return portsForPrint;
        }

        private List<Port> AddSecRemoteEmptyPorts(List<Antenna> antennas)
        {
            var ports = new List<Port>();

            var cellsSecRemoteEmptyAnt = MapElement<Cell>(elementsSecRemoteEmptyAntFromDb);

            if (cellsSecRemoteEmptyAnt.Any())
            {
                cellsSecRemoteEmptyAnt.ForEach(n => n.AddAntennaIdForSecRemoteEmptyAntenna(antennas));

                ports = cellsSecRemoteEmptyAnt
                        .Where(o => o.Status == Status.Ok)
                        .GroupBy(o => new { o.AntennaId, o.PortId })
                        .Select(o => new Port
                        {
                            SectorId = o.Select(p => p.SectorId).First(),
                            AntennaId = o.Select(p => p.AntennaId).First(),
                            PortId = o.Select(p => p.PortId).Min(),
                            BandRange = o.Select(p => p.BandRange).FirstOrDefault()!,
                            Tech = string.Join(" ",
                                   o.Select(p => p.Technology)
                                   .Distinct()),
                            Etilt = o.Select(p => p.Etilt == null ? 0 : p.Etilt).Max(),
                            RruType = string.Join(" ",
                                      o.Select(p => p.RruType)
                                      .Where(p => p != "REUSE")
                                      .Distinct()),
                        })
                        .OrderBy(n => n.SectorId)
                        .ThenBy(n => n.AntennaId)
                        .ThenBy(n => n.PortId)
                        .ToList();
            }
            return ports;
        }

        private void AddSecRemoteEmptyAntAndPorts(ref List<Antenna> antennas, ref List<Port> ports)
        {
            ports.AddRange(AddSecRemoteEmptyPorts(antennas));
            antennas.AddRange(AddSecRemoteEmptyAntennas(antennas));
        }

        private List<BandGrouping> LoadBands(List<Rru> rrus, List<Antenna> antennas)
        {
            var portBandFromElements = MapElement<BandGrouping>(elementsFromDb);

            var portBandGroup = portBandFromElements
                .GroupBy(o => new { o.AntennaId, o.Band })
                .Select(o => o.First())
                .ToList();

            portBandGroup.ForEach(n => n.CalcAntPowerBandAndTech(Cells, rrus));

            if (Report.Document == ReportType.PSK)
                portBandGroup.ForEach(n => n.SetZeroEtiltForAntAAU5639W_PSK(antennas));


            return portBandGroup.OrderBy(n => n.SectorId)
                .ThenBy(n => n.AntennaId)
                .ThenBy(n => n.Band).ToList();
        }

        public List<Sector> LoadSectors()
        {
            var sectors = MapElement<Sector>(elementsFromDb);
            var sectorsGroup = sectors.GroupBy(o => new { o.SectorId })
                  .Select(o => o.First()).ToList();
            sectorsGroup.ForEach(n => n.PopulateTrxValues());

            return sectorsGroup;
        }

        public List<Site> LoadSite()
        {
            var site = MapElement<Site>(elementsFromDb);
            var siteGroup = site.GroupBy(o => new { o.SiteID, o.Candidate })
                                .Select(o => o.First()).ToList();
            if (!siteGroup.Any())
                throw new Exception($"Error main query : no data for site {siteID} !");

            return siteGroup;
        }

        public MemoryStream Start(out string critErrors)
        {
            var streamForExcelExport = new MemoryStream();
            var remarks = new List<string>();
            var remarksStr = String.Empty;
            critErrors = string.Empty;


            try
            {
                var rrus = LoadRrus();

                var showMeRru = rrus
                    .Where(n => n.AntennaId == 11 && n.Band == Band.B18)
                    .ToList();

                Cells = LoadCells();

                var showMeCell = Cells
                    .Where(n => n.AntennaId == 11 && n.Band == Band.B18)
                    .ToList();

                var antennas = LoadAntennas();

                var ports = LoadPorts(rrus, antennas);

                var bands = LoadBands(rrus, antennas);


                AddSecRemoteEmptyAntAndPorts(ref antennas, ref ports);

                var sectors = LoadSectors();

                foreach (var sector in sectors)
                {
                    sector.PopulateErrors();
                    if (!String.IsNullOrEmpty(sector.Request_Remarks))
                        remarks.Add(sector.Request_Remarks);
                }


                if (remarks.Any())
                {
                    remarksStr = String.Join("; ", remarks);
                    File.WriteAllLines(
                        Path.Combine(Global.ERRORS_DIR, $"Remarks_{siteID}_{Report.Document}_{Global.CurrentTime}.txt"), remarks);
                    Console.WriteLine($"Site{siteID} {remarksStr}");
                }



                var site = LoadSite();

                var excelExport = new ExcelOutput();

                streamForExcelExport = excelExport.PopulateExcel(
                    site,
                    sectors,
                    antennas,
                    ports,
                    bands,
                    antennaPortMap);
            }
            catch (Exception ex)
            {
                critErrors = $"{remarksStr}{Environment.NewLine}{ex.Message}";

                Console.WriteLine(critErrors);
                File.WriteAllText(Path.Combine(Global.ERRORS_DIR, $"Errors_{siteID}_{Report.Document}_{Global.CurrentTime}.txt"), critErrors);
            }

            return streamForExcelExport;
        }

    }
}
