using BLL.Attributes;
using BLL.Interfaces;
using System;
using System.Collections.Generic;

namespace BLL.DTO
{
    public class Antenna:IAntenna,IOffset
    {
        [ForPrint]
        public string AntennaMount { get; set; }
        [ForPrint]
        public decimal AGL { get; set; }
        [ForPrint]
        public decimal ARTL { get; set; }
        [ForPrint]
        public decimal MechanicalTilt { get; set; }
        public List<Port> Ports { get; set; }
        
        public string SectorNumber { get; set; }
        [ForPrint]
        public decimal? Azimuth { get; internal set; }
        [ForPrint]
        public string AntennaType { get; set; }

        public decimal PhyIndex { get; set; }

        public int OffsetExcel { get; set; }
       
    }
}