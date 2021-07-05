using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BLL.DTO
{
    public class ReportSRF_StaticAnt : ReportSRF
    {


        public ReportSRF_StaticAnt(string reportPath) : base(reportPath)
        {

        }


        internal static readonly string[] technology = { "L", "L", "", "", "L", "U" };
        internal static readonly string[] technology_v1 = { "L", "L", "", "", "", "L U" };
        internal static readonly string ret = "InternalRET";
        internal static readonly string[] feederType = { "RRU TOP JUMPER", "RRU TOP JUMPER", "RRU TOP JUMPER", "", "RRU TOP JUMPER", "RRU TOP JUMPER" };
        internal static readonly string[] feederType_v1 = { "RRU TOP JUMPER", "RRU TOP JUMPER", "RRU TOP JUMPER", "", "", "RRU TOP JUMPER" };
        internal static readonly int portNumber = 6;
        internal static readonly string[] portsBandRange = { "1695–2690", "1695–2690", "694–960", "694–960", "1695–2690", "1695–2690" };
        internal static readonly string[] portIsFree = { "Free", "Free", "Free", "Used", "Free", "Free" };
        internal static readonly string[] portIsFree_v1 = { "Free", "Free", "Free", "Used", "Used", "Free" };
        internal static readonly string[] portOccupancy = { "2600", "2600", "900", "", "1800", "2100" };
        internal static readonly string[] portOccupancy_v1 = { "2600", "2600", "900", "", "", "1800" };
        internal static readonly string[] portName = { "7-8(Y2)", "11-12(Y4)", "3-4(R2)", "1-2(R1)", "5-6(Y1)", "9-10(Y3)" };
        internal static readonly Dictionary<int, int> mapOldNewPortNames = new Dictionary<int, int>
        { { 1, 4 }, { 2, 3 }, { 3, 5 }, { 4, 1 }, { 5, 6 }, { 6, 2 } };
        internal static readonly string[] rru_Type = { "5304-26", "Reuse 5304-26", "", "none", "3953-18", "" };
        internal static readonly string[] rru_Type_v1 = { "5304-26", "Reuse 5304-26", "", "none", "", "" };
        internal static readonly string[] combiner = { "none", "none", "none", "none", "none", "none" };
        internal static readonly string[] combiner_v1 = { "none", "none", "none", "none", "none", " DCB-2-DU-64F-01" };
        internal static readonly int excellOffset = 1;

        public static Antenna CreateStaticAnt(bool portWith_18_21_3G_4G)
        {
            return new Antenna
            {
                AntennaMount = String.Empty,
                AntennaType = "RRV4-65D-R6",
                Ports = LoadPorts(portWith_18_21_3G_4G),
                OffsetExcel = excellOffset
            };
        }

        private static List<Port> LoadPorts(bool portWith_18_21_3G_4G)
        {
            var ports = new Port[ReportSRF_StaticAnt.portNumber].ToList();
            for (int i = 0; i < ReportSRF_StaticAnt.portNumber; i++)
            {
                ports[i] = new Port();
                ports[i].BandPosition = i;
                ports[i].OffsetExcel = excellOffset + i;
                ports[i].BandRange = portsBandRange[i];
                ports[i].PortName = portName[i];
                ports[i].RET = ret;

                if(!portWith_18_21_3G_4G)
                {
                    ports[i].Technology = technology[i];
                    ports[i].Feeder_Type = feederType[i];
                    ports[i].Status = portIsFree[i];
                    ports[i].Band = portOccupancy[i];
                    ports[i].RRU_Type = rru_Type[i];
                }
                else
                {
                    ports[i].Technology = technology_v1[i];
                    ports[i].Feeder_Type = feederType_v1[i];
                    ports[i].Status = portIsFree_v1[i];
                    ports[i].Band = portOccupancy_v1[i];
                    ports[i].RRU_Type = rru_Type_v1[i];
                    ports[i].Combiner_Splitter = combiner_v1[i];
                }
            }
            return ports;
        }

        public override string[] ForPrintSectorSheet
        {
            get
            {
                return new string[]
                {
                    "AntennaType new ",
                    "AntennaMount Current",
                    "MechanicalTilt new",
                    "BandRange New",
                    "Band Current",
                    "Port_occupancy",
                    "Technology New",
                    "Etilt New",
                    "RET New",
                    "Feeder_Type New",
                    "Feeder_Length New",
                    "RRU_Type New",
                    "Antenna port #",
                    "Combiner_Splitter New"
                };
            }
        }

               
       

        //public override string TemplateFilePath
        //{
        //    get
        //    {
        //        string tempPath = Path.GetTempFileName();
        //        File.WriteAllBytes(tempPath, Properties.Resources.SRF_Template_StaticAnt);
        //        base.TemplateFilePath = tempPath;
        //        return base.TemplateFilePath;
        //    }


        //}
    }
}
