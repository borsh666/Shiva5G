using BLL.Attributes;
using BLL.Interfaces;
using System.Collections.Generic;

namespace BLL.DTO
{
    public class Sector:IOffset
    {
        public string SectorNumb { get; set; }
        public string Pole { get; set; }
        public string Azimuth { get; set; }
        public List<Antenna> Antennas { get; set; }
        [ForPrint]
        public int GSM_900 { get; set; }
        [ForPrint]
        public int GSM_1800 { get; set; }
        [ForPrint]
        public int UMTS_2100 { get; set; }
        [ForPrint]
        public int UMTS_900 { get; set; }
        [ForPrint]
        public int LTE_900 { get; set; }
        [ForPrint]
        public int LTE_1800 { get; set; }
        [ForPrint]
        public int LTE_2100 { get; set; }
        [ForPrint]
        public int LTE_2600 { get; set; }
        [ForPrint]
        public int NR_1800 { get; set; }
        [ForPrint]
        public int NR_2100 { get; set; }
        [ForPrint]
        public int NR_3500 { get; set; }

        //New Vales
        [ForPrint]
        public int GSM_900N { get { return GSM_900; }  }
        [ForPrint]
        public int GSM_1800N { get { return GSM_1800; } }
        [ForPrint]
        public int UMTS_2100N { get { return UMTS_2100; } }
        [ForPrint]
        public int UMTS_900N { get { return UMTS_900; } }
        [ForPrint]
        public int LTE_900N { get { return LTE_900; } }
        [ForPrint]
        public int LTE_1800N { get { return LTE_1800; } }
        [ForPrint]
        public int LTE_2100N { get { return LTE_2100; } }
        [ForPrint]
        public int LTE_2600N { get { return LTE_2600; } }
        [ForPrint]
        public int NR_1800N { get { return NR_1800; } }
        [ForPrint]
        public int NR_2100N { get { return NR_2100; } }
        [ForPrint]
        public int NR_3500N { get { return NR_3500; } }


        public int OffsetExcel { get; set; }


    }
}
