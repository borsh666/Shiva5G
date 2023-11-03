//using BLL_Atoll.Enums;
//using BLL_Atoll.Models.Site;
//using static BLL_Atoll.SupportFunc;

//namespace BLL_Atoll
//{
//    internal class SecondRemoteAntenna
//    {
//        private List<SecondaryRemoteAntennaModel> secRemoteEmptyAnntenas;
//        private string siteID;


//        public SecondRemoteAntenna(string siteID)
//        {
//            this.siteID = siteID;
//            //secondaryRemoteAnntenas = LoadSecondaryEmptyAnntenas();
//            secRemoteEmptyAnntenas = LoadSecRemoteEmptyAnntenas();
//        }

//        private List<SecondaryRemoteAntennaModel> LoadAnntenasFromDb(string sql)
//        {
//            var query = File.ReadAllText(sql);
//            query = query.Replace(Global.PATTERN_SITEID, this.siteID);

//            var antennas = MapElement<SecondaryRemoteAntennaModel>(GetFromDb(query, Global.CONN_STR_ATOLL));
//            antennas ??= new List<SecondaryRemoteAntennaModel>();
//            antennas = antennas.Where(n => !string.IsNullOrEmpty(n.AntennaType)).ToList();

//            return antennas;
//        }

//        //private List<SecondaryRemoteAntennaModel> LoadSecondaryEmptyAnntenas()
//        //{
//        //    var secAnt = LoadAnntenasFromDb(Global.QUERY_SEC_REMOTE_ANTENNA);
//        //    secAnt.ForEach(n => n.AntennaLevel = AntennaLevel.Secondary);

//        //    var emptyAnt = LoadAnntenasFromDb(Global.QUERY_EMPTY_ANTENNA);
//        //    emptyAnt.ForEach(n => n.AntennaLevel = AntennaLevel.Empty);

//        //    secAnt.AddRange(emptyAnt);
//        //    return secAnt;
//        //}

//        private List<SecondaryRemoteAntennaModel> LoadSecRemoteEmptyAnntenas()
//        {
//            return LoadAnntenasFromDb(Global.QUERY_SEC_REMOTE_EMPTY_ANTENNA);
//        }

//        public void SectorTrxUpdate(ref List<Sector> sectors)
//        {
//            foreach (var sec in sectors)
//            {
//                int UpdateTrx(int sectorTrx, Band band, string tech)
//                {
//                    return secRemoteEmptyAnntenas
//                           .Any(n => 
//                                n.Band == band &&
//                                n.Tech.Contains(tech) &&
//                                n.SectorId == sec.SectorId) ? sectorTrx + 1 : sectorTrx;
//                }

//                sec.GSM_900 = UpdateTrx( sec.GSM_900, Band.B9, "G");
//                sec.GSM_1800 = UpdateTrx( sec.GSM_1800, Band.B18, "G");
//                sec.UMTS_900 = UpdateTrx( sec.UMTS_900, Band.B9, "U");
//                sec.UMTS_2100 = UpdateTrx( sec.UMTS_2100, Band.B21, "U");
//                sec.LTE_900 = UpdateTrx( sec.LTE_900, Band.B9, "L");
//                sec.LTE_1800 = UpdateTrx( sec.LTE_1800, Band.B18, "L");
//                sec.LTE_2100 = UpdateTrx( sec.LTE_2100, Band.B21, "L");
//                sec.LTE_2600 = UpdateTrx( sec.LTE_2600, Band.B26, "L");
//                sec.DSS_1800 = UpdateTrx( sec.DSS_1800, Band.B18, "DSS");
//                sec.DSS_2100 = UpdateTrx( sec.DSS_2100, Band.B21, "DSS");
//                sec.NR_3500 = UpdateTrx( sec.NR_3500, Band.B35, "NR");
//            }
//        }

//        //public void PortUpdate(List<Antenna> antennas, ref List<Port> ports)
//        //{
//        //    if (this.secondaryRemoteAnntenas.Any())
//        //    {
//        //        Dictionary<int, int> antLastIdBySec = AntLastIdBySec(antennas);

//        //        foreach (var port in ports)
//        //        {
//        //            var tech = new List<string>();
//        //            var eTilts = new List<decimal>();

//        //            var filterAnt = secondaryRemoteAnntenas.Where(
//        //                    n => n.SectorId == port.SectorId &&
//        //                    port.AntennaId == antLastIdBySec[port.SectorId] &&
//        //                    n.PortId == port.PortId &&
//        //                    n.AntennaLevel != AntennaLevel.Empty);

//        //            if (filterAnt.Any())
//        //            {
//        //                port.Tech = String.Join(" ", filterAnt
//        //                                                    .Select(n => n.Tech)
//        //                                                    .Distinct());
//        //                port.Etilt = filterAnt.Max(n => n.Etilt);
//        //                port.Bands = filterAnt.Select(n => n.Band).Distinct().ToList();
//        //                //port.GsmTrx = port.Tech.Contains("G") ? 1 : 0;
//        //                //port.UmtsTrx = port.Tech.Contains("U") ? 1 : 0;
//        //                //port.LteTrx = port.Tech.Contains("L") ? 1 : 0;
//        //                //port.NrTrx = port.Tech.Contains("NR") ? 1 : 0;
//        //                port.Status = Status.Occupied;
//        //            }
//        //        }
//        //    }

//        //}
//        public void PortUpdate(List<Antenna> antennas, ref List<Port> ports)
//        {
//            if (this.secRemoteEmptyAnntenas.Any())
//            {
//                var antLastIdBySec = AntLastIdBySec(antennas).Values.First();
                
//                var filterAnt = antennas.Where(
//                        n => n.AntennaId == antLastIdBySec).First();

//                var port = new Port()
//                {
//                    SectorId = filterAnt.SectorId,
//                    AntennaId = filterAnt.AntennaId,
//                    PortId = 1,
//                    Status = Status.Occupied
//                }

//                foreach (var port in ports)
//                {


//                    if (filterAnt.Any())
//                    {
//                        port.Tech = String
//                            .Join(" ", 
//                                  filterAnt
//                                        .Select(n => n.Tech)
//                                        .Distinct());
//                        port.Etilt = filterAnt.Max(n => n.Etilt);
//                        port.Bands = filterAnt.Select(n => n.Band).Distinct().ToList();
//                        port.Status = Status.Occupied;
//                    }
//                }
//            }

//        }
//        public void AntennaUpdate(ref List<Antenna> antennas)
//        {
//            if (this.secRemoteEmptyAnntenas.Any())
//            {
//                Dictionary<int, int> antLastIdBySec = AntLastIdBySec(antennas);

//                var secRemEmptyAntDistinctBySecIdAntId = secRemoteEmptyAnntenas
//                    .DistinctBy(n => new { n.SectorId, n.AntennaType });

//                var newAntennas = new List<Antenna>();

//                foreach (var secRemEmptyAntenna in secRemEmptyAntDistinctBySecIdAntId)
//                {
//                    var newAnt = new Antenna()
//                    {
//                        AntennaLevel = secRemEmptyAntenna.AntennaLevel,
//                        SectorId = secRemEmptyAntenna.SectorId,
//                        AntennaId = antLastIdBySec[secRemEmptyAntenna.SectorId] + 1,
//                        AntennaType = secRemEmptyAntenna.AntennaType,
//                        Height = secRemEmptyAntenna.Height,
//                        Mtilt = secRemEmptyAntenna.Mtilt
//                    };
//                    newAntennas.Add(newAnt);

//                    Engine.Errs[secRemEmptyAntenna.SectorId]
//                        .Add($"Second/Remote/Empty antenna {secRemEmptyAntenna.AntennaType} with azimuth {secRemEmptyAntenna.Azimuth}");
//                }
//                antennas.AddRange(newAntennas);
//            }
//        }
//    }
//}
