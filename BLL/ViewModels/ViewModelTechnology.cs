namespace BLL
{
    public class ViewModelTechnology
    {
        public decimal? AGL { get; set; }
        public string AntennaType { get; set; }
        public string ANTENNA_MOUNT { get; set; }
        public decimal? ARTL { get; set; }
        public decimal? Azimuth { get; set; }
        public string Band { get;  set; }
        public string Candidate { get; set; }
        public decimal? CellID { get; set; }
        public string CellName { get;  set; }
        public string CoLocation { get; set; }
        public string COMBINER_SPLITTER { get; set; }
        public string Controler { get; set; }
        public decimal? Etilt { get; set; }
        public decimal? FEEDERLENGTH { get; set; }
        public string FEEDERTYPE { get; set; }
        public decimal? GSM_Pwr_per_TRX { get;  set; }
        public decimal? GSM_TRX { get;  set; }
        public string LAYER_TECHNOLOGY { get; set; }
        public decimal? LTE_Pwr_per_TRX { get;  set; }
        public decimal? LTE_TRX { get;  set; }
        public decimal? MECHANICAL_TILT { get; set; }
        public decimal? PHYINDEX { get; set; }
        public decimal? LOGINDEX { get; set; }
        public string PortNumber { get; set; }
        public string RRU_Type { get; set; }
        public string Sector { get;  set; }
        public string SEC_COMBINER_SPLITTER { get; set; }
        public string SiteAddress { get; set; }
        public string SiteAddress1 { get; set; }
        public string SiteAddress2 { get; set; }
        public string SiteID { get; set; }
        public string SiteName { get; set; }
        public string TMA { get; set; }
        public decimal? UMTS_Pwr_per_TRX { get;  set; }
        public decimal? UMTS_TRX { get;  set; }
        public decimal? NR_Pwr_per_TRX { get; set; }
        public decimal? NR_TRX { get; set; }
        public decimal? Bandwidth { get; internal set; }
    }
}