using BLL.DTO;
using OfficeOpenXml.FormulaParsing.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class PortGrouping
    {
        private List<Port> ports;

        public PortGrouping(List<Port> ports)
        {
            this.ports = ports;

            foreach (var item in this.ports)
            {
                item.Etilt = item.Etilt.Replace(".0", "");
                item.Feeder_Length = item.Feeder_Length.Replace(".0", "");
            }
        }

        private List<Port> PortArrangeWithInvalidPortsFromAsset(string siteID)
        {
            //Dictionary<int,string[]>   Key: 1 Band position in Excel Value[]:Band[0] "790-960" [1] "Free"

            var currentAnt = this.ports.Select(n => n.AntennaType).FirstOrDefault();

            var antennaPortMapping = Queries.GetAntennaPorts(currentAnt);

            var lstInitialPorts = new List<Port>();

            //Init All ports for antenna
            foreach (var item in antennaPortMapping)
            {
                var port = new Port();
                port.SectorNumber = this.ports.Select(n => n.SectorNumber).FirstOrDefault();
                port.AntennaType = currentAnt;
                port.PhyIndex = this.ports.Select(n => n.PhyIndex).FirstOrDefault();
                port.BandRange = item.Value[0];
                port.BandPosition = item.Key;
                port.Status = item.Value[1];
                port.OffsetExcel = item.Key;
                port.Technology = "Free";

                lstInitialPorts.Add(port);
            }

            var groupByAntenna = GropingBySecAntPhBand().OrderBy(n => n.Combiner_Splitter).ToList();

            foreach (var port in groupByAntenna)
            {
                foreach (var initPort in lstInitialPorts)
                {
                    if (initPort.Status == "Used")
                        continue;

                    if (port.AntennaType == initPort.AntennaType && port.PhyIndex == initPort.PhyIndex && SupportFunc.IsEqualBand(initPort.BandRange, port.Band))
                    {
                        port.BandRange = initPort.BandRange;
                        //Тук сетвам офсета на всяка една технология
                        port.BandPosition = initPort.BandPosition;

                        //New 14.11.2017 :  If we have nano , we can combine tech w/o combiner
                        if (port.Combiner_Splitter == "None" && !siteID.Contains("NC"))
                            initPort.Status = "Used";
                        else
                            initPort.Status = "Combiner";

                        break;
                    }
                }


            }


            var groupByBand = GropingBySecAntPhBandPosition(groupByAntenna).ToList();


            //Добавям празните портове за които не съм намерил технология , която да сложа
            groupByBand.AddRange(lstInitialPorts.Where(n => n.Status != "Used" && n.Status != "Combiner"));


            //Проверявам за технологии , които не са получили офсет (port.BandPosition=0) и ги слагам в Remark
            groupByBand.Where(n => n.BandPosition == 0).ToList()
                    .ForEach(n => n.Request_Remarks += $"Технология {n.Technology} за антена {n.AntennaType} за банд {n.Band}" +
                    $" не е намерен spliter/combiner.{Environment.NewLine}");

            //Групирам всички Remarks от всички технологии и ги записвам в една - първата
            var remarks = String.Join("", groupByBand.Select(n => n.Request_Remarks));

            var antennasWithoutTech = Queries.GetAntennasWithoutTech(siteID);

            if (!string.IsNullOrEmpty(antennasWithoutTech))
                remarks += $"{Environment.NewLine}{antennasWithoutTech}";

            groupByBand.ForEach(n => n.Request_Remarks = String.Empty);
            groupByBand.First(n => n.BandPosition != 0).Request_Remarks = remarks;

            //Да отсея тези дето са с port.BandPosition=0
            return groupByBand.Where(n => n.BandPosition != 0).ToList();


        }

        private List<Port> PortArrangeWithValidPortsFromAsset(string siteID)
        {
            //Dictionary<int,string[]>   Key: 1 Band position in Excel Value[]:Band[0] "790-960" [1] "Free" , Y3

            var currentAnt = this.ports.Select(n => n.AntennaType).FirstOrDefault();

            var antennaPortMapping = Queries.GetAntennaPorts(currentAnt);

            var groupByPort = this.GroupByPort();

            foreach (var port in groupByPort)
            {
                foreach (var antPort in antennaPortMapping)
                {
                    if (port.PortNumber == antPort.Key)
                    {
                        port.BandRange = antPort.Value[0];
                        port.PortName = GetPortName(antPort.Value[2]);
                        port.Status = "Used";
                        port.BandPosition = antPort.Key;
                        port.AntennaType = currentAnt;
                        antPort.Value[1] = "Used";
                    }
                }
            }

            var freePorts = antennaPortMapping.Where(n => n.Value[1] == "Free");

            foreach (var freePort in freePorts)
            {
                groupByPort.Add(new Port
                {
                    BandRange = freePort.Value[0],
                    Status = freePort.Value[1],
                    Technology = freePort.Value[1],
                    BandPosition = freePort.Key,
                    PortName = GetPortName(freePort.Value[2]),
                    AntennaType = currentAnt
                });
            }

            return groupByPort;
        }

        private string GetPortName(string portName)
        {
            if (portName == "NOVAL")
                return String.Empty;
            else
                return portName;
        }

        private List<Port> GropingBySecAntPhBandPosition(List<Port> lstPorts)
        {
            var group = lstPorts.GroupBy(n => new
            {
                SectorNumber = n.SectorNumber,
                AntennaType = n.AntennaType,
                PhyIndex = n.PhyIndex,
                BandPosition = n.BandPosition

            })
           .Select(n => new Port
           {
               SectorNumber = n.Key.SectorNumber,
               AntennaType = n.Key.AntennaType,
               PhyIndex = n.Key.PhyIndex,
               BandPosition = n.Key.BandPosition,
               Band = String.Join(" ", n.Select(k => k.Band).Distinct()),
               BandRange = String.Join(" ", n.Select(k => k.BandRange).Distinct()),
               Technology = String.Join("", n.Select(k => k.Technology).Distinct()),
               Etilt = String.Join(" ", n.Select(k => k.Etilt).Distinct()),
               ModelRRUs = n.SelectMany(k => k.ModelRRUs).ToList(),
               TMA = String.Join(" ", n.Select(k => k.TMA).Distinct()),
               Feeder_Type = String.Join(" ", n.Select(k => k.Feeder_Type).Distinct()),
               Feeder_Length = String.Join(" ", n.Select(k => k.Feeder_Length).Distinct()),
               Feeder_Att_dB = n.Max(k => k.Feeder_Att_dB),
               Combiner_Splitter = String.Join(" ", n.Select(k => k.Combiner_Splitter).Distinct()),
               Sec_Combiner_Splitter = String.Join(" ", n.Select(k => k.Sec_Combiner_Splitter).Distinct()),
               Combiner_Splitter_Loss = n.Max(k => k.Combiner_Splitter_Loss),
               Second_Combiner_Splitter_Loss = n.Max(k => k.Second_Combiner_Splitter_Loss),
               Collocation = String.Join(" ", n.Select(k => k.Collocation).Distinct()),
           }).ToList();

            foreach (var item in group)
                item.Technology = string.Join("", item.Technology.ToCharArray().Distinct());

            return group;
        }

        public List<Port> GropingBySecAntPhBand()
        {
            
            var groupPorts = ports.GroupBy(n => new
            {
                SectorNumber = n.SectorNumber,
                AntennaType = n.AntennaType,
                PhyIndex = n.PhyIndex,
                Band = n.Band
            })
            .Select(n => new Port
            {
                SectorNumber = n.Key.SectorNumber,
                AntennaType = n.Key.AntennaType,
                PhyIndex = n.Key.PhyIndex,
                Band = n.Key.Band,
                Technology = String.Join("", n.Select(k => k.Technology).Distinct()),
                Etilt = String.Join(" ", n.Select(k => k.Etilt).Distinct()),
                ModelRRUs = n.SelectMany(k => k.ModelRRUs).ToList(),
                TMA = String.Join(" ", n.Select(k => k.TMA).Distinct()),
                Feeder_Type = String.Join(" ", n.Select(k => k.Feeder_Type).Distinct()),
                Feeder_Length = String.Join(" ", n.Select(k => k.Feeder_Length).Distinct()),
                Feeder_Att_dB = n.Max(k => k.Feeder_Att_dB),
                Combiner_Splitter = String.Join(" ", n.Select(k => k.Combiner_Splitter).Distinct()),
                Sec_Combiner_Splitter = String.Join(" ", n.Select(k => k.Sec_Combiner_Splitter).Distinct()),
                Combiner_Splitter_Loss = n.Max(k => k.Combiner_Splitter_Loss),
                Second_Combiner_Splitter_Loss = n.Max(k => k.Second_Combiner_Splitter_Loss),
                Collocation = String.Join(" ", n.Select(k => k.Collocation).Distinct()),

            }).ToList();

            foreach (var port in groupPorts)
            {
                if (port.Band == "900")
                    port.BandPosition = 1;
                else if (port.Band == "1800")
                    port.BandPosition = 2;
                else if (port.Band == "2100")
                    port.BandPosition = 3;
                else if (port.Band == "2600")
                    port.BandPosition = 4;
                else if (port.Band == "3500")
                    port.BandPosition = 5;
            }

            return groupPorts;
        }

        private List<Port> GroupByPort()
        {
            var group = this.ports.GroupBy(n => new
            {
                PortNumber = n.PortNumber
            })
              .Select(n => new Port
              {
                  PortNumber = n.Key.PortNumber,
                  Technology = String.Join("", n.Select(k => k.Technology.Trim()).Distinct()),
                  Etilt = String.Join(" ", n.Select(k => k.Etilt).Distinct()),
                  ModelRRUs = n.SelectMany(k => k.ModelRRUs).ToList(),
                  TMA = String.Join(" ", n.Select(k => k.TMA).Distinct()),
                  Feeder_Type = String.Join(" ", n.Select(k => k.Feeder_Type).Distinct()),
                  Feeder_Length = String.Join(" ", n.Select(k => k.Feeder_Length).Distinct()),
                  Feeder_Att_dB = n.Max(k => k.Feeder_Att_dB),
                  Combiner_Splitter = String.Join(" ", n.Select(k => k.Combiner_Splitter).Distinct()),
                  Sec_Combiner_Splitter = String.Join(" ", n.Select(k => k.Sec_Combiner_Splitter).Distinct()),
                  Combiner_Splitter_Loss = n.Max(k => k.Combiner_Splitter_Loss),
                  Second_Combiner_Splitter_Loss = n.Max(k => k.Second_Combiner_Splitter_Loss),
                  Collocation = String.Join(" ", n.Select(k => k.Collocation).Distinct()),
                  AntennaType = n.Select(k => k.AntennaType).FirstOrDefault(),
                  Band = n.Select(k => k.Band).FirstOrDefault(),


              }).OrderBy(n => n.PortNumber).ToList();

            return group;
        }

        public List<Port> PortArrange(string siteID)
        {
            var isInvalidPorts = this.ports.Any(n => n.PortNumber == 0);

            if (isInvalidPorts)
                return PortArrangeWithInvalidPortsFromAsset(siteID);
            else
                return PortArrangeWithValidPortsFromAsset(siteID);

        }

    }
}
