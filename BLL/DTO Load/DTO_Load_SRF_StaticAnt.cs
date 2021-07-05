using BLL.DTO;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BLL
{
    public class DTO_Load_SRF_StaticAnt : DTO_Load_SRF
    {
        public DTO_Load_SRF_StaticAnt(string siteID, bool isSiteSRAN) : base(siteID, isSiteSRAN)
        {
        }

        private static void SetPortEtiltAndFeederLen(Site newSite, int sectorNumber)
        {
            var ports1800Index = ReportSRF_StaticAnt.portOccupancy.ToList().IndexOf("1800");

            var port1800 = newSite.Sectors[sectorNumber].Antennas.First().Ports[ports1800Index];

            var ports2600Index = ReportSRF_StaticAnt.portOccupancy.AllIndexesOf("2600");

            foreach (var ports2600 in ports2600Index)
            {
                newSite.Sectors[sectorNumber].Antennas.First().Ports[ports2600].Etilt = port1800.Etilt;
                newSite.Sectors[sectorNumber].Antennas.First().Ports[ports2600].Feeder_Length = port1800.Feeder_Length;
            }

        }

        public Site LoadNewSiteAntenna()
        {

            var oldSite = base.Site();
            var newSite = base.Site();


            for (int sectorNumber = 0; sectorNumber < newSite.Sectors.Count; sectorNumber++)
            {
                var oldAntennas = oldSite.Sectors[sectorNumber].Antennas;

                if (oldAntennas.Count == 0)
                    continue;

                bool AnyPortWith_1800_2100_3G_4G = CheckForPortWith_1800_2100_3G_4G(oldAntennas.First());


                var newAntenna = ReportSRF_StaticAnt.CreateStaticAnt(AnyPortWith_1800_2100_3G_4G);
                newSite.Sectors[sectorNumber].Antennas = new List<Antenna> { newAntenna };
                newSite.Sectors[sectorNumber].Antennas.First().MechanicalTilt = oldAntennas.First().MechanicalTilt;

                var oldPorts = oldAntennas.SelectMany(n => n.Ports.Where(k => k.ModelRRUs != null));

                foreach (var newPort in newSite.Sectors[sectorNumber].Antennas.First().Ports)
                {
                    foreach (var oldPort in oldPorts)
                    {
                        var oldPortBand = oldPort.ModelRRUs.Select(n => n.Band).Where(n => !string.IsNullOrEmpty(n)).FirstOrDefault();
                        if (newPort.Band == oldPortBand && newPort.Status == "Free")
                        {
                            newPort.Status = "Used";
                            newPort.Etilt = oldPort.Etilt;
                            newPort.RRU_Type = oldPort.RRU_Type;
                            newPort.Feeder_Length = oldPort.Feeder_Length;
                            newPort.Technology = oldPort.Technology;

                            //if (oldPort.Combiner_Splitter != "None")
                            //    newPort.Combiner_Splitter = "None";

                            break;
                        }
                    }
                }

                SetPortEtiltAndFeederLen(newSite, sectorNumber);

            }

            return newSite;
        }

        private bool CheckForPortWith_1800_2100_3G_4G(Antenna antenna)
        {
            var portWithCombiner = antenna.Ports.Where(n => n.Combiner_Splitter != "").FirstOrDefault();
            if (portWithCombiner != null)
            {
                var isBand_1800 = portWithCombiner.ModelRRUs.Any(n => n.Band == "1800" && (n.Technology == "U" || n.Technology == "L"));
                var isBand_2100 = portWithCombiner.ModelRRUs.Any(n => n.Band == "2100" && (n.Technology == "U" || n.Technology == "L"));

                if (isBand_1800 && isBand_2100)
                    return true;
            }

            return false;

        }
    }
}

