using BLL.Attributes;
using BLL.Interfaces;
using System.Collections.Generic;

namespace BLL.DTO
{
    public class Port:IAntenna,IOffset
    {
        public string CellName { get; set; }
        [ForPrint]
        public string Band { get; set; }
        [ForPrint]
        public string BandRange { get; set; }

        public int BandPosition { get; set; }

        public int PortNumber { get; set; }
        [ForPrint]
        public string Technology { get; set; }
        [ForPrint]
        public string Etilt { get; set; }
        [ForPrint]
        public string RET { get; set; }
        [ForPrint]
        public string Feeder_Type { get; set; }
        [ForPrint]
        public string Feeder_Length { get; set; }
        [ForPrint]
        public string TMA { get; set; }
        [ForPrint]
        public string Combiner_Splitter { get; set; }
        [ForPrint]
        public string Sec_Combiner_Splitter { get; set; }
        [ForPrint]
        public string Collocation { get; set; }

        public List<ModelRRU> ModelRRUs { get; set; }
        [ForPrint]
        public string RRU_Type { get; set; }
        [ForPrint]
        public string RRU_Total { get; set; }
        [ForPrint]
        public decimal GSM_TRX { get; set; }
        [ForPrint]
        public string GSM_Pwr_per_TRX { get; set; }
        [ForPrint]
        public decimal UMTS_TRX { get; set; }
        [ForPrint]
        public string UMTS_Pwr_per_TRX { get; set; }
        [ForPrint]
        public decimal LTE_TRX { get; set; }
        [ForPrint]
        public string LTE_Pwr_per_TRX { get; set; }
      //  [ForPrint]
        public decimal NR_TRX { get; set; }
       // [ForPrint]
        public string NR_Pwr_per_TRX { get; set; }
        [ForPrint]
        public string Request_Remarks { get; set; }

        public string SectorNumber { get; set; }
        [ForPrint]
        public string AntennaType { get; set; }

        public decimal PhyIndex { get; set; }

        public int OffsetExcel { get; set; }

        public string Status { get; set; }

        public double Feeder_Att_dB { get; set; }

        public double Combiner_Splitter_Loss { get; set; }

        public double Second_Combiner_Splitter_Loss { get; set; }
        [ForPrint]
        public double Antenna_IN_Total_Power { get; set; }

        //PSK
        [ForPrint]
        public string GSM_Pwr_ToC { get; internal set; }
        [ForPrint]
        public string UMTS_Pwr_ToC { get; internal set; }
        [ForPrint]
        public string LTE_Pwr_ToC { get; internal set; }
        //[ForPrint]
        public string NR_Pwr_ToC { get; internal set; }
        [ForPrint]
        public string PortName { get; internal set; }

        public Port DeepCopy()
        {
            return this.DeepCopyByExpressionTree();

            //Port other = (Port)this.MemberwiseClone();

            //return other;
        }
    }
}