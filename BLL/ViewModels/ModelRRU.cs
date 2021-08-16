namespace BLL
{
    public class ModelRRU
    {
        public string Sector { get; set; }
        public string SiteID { get; set; }
        public string Band { get; internal set; }
        public string Technology { get; set; }
        public string CellName { get; internal set; }
        public string RRU_SN { get; internal set; }
        public string RRU_Type { get; set; }
        public string GSM_Pwr_per_TRX { get; internal set; }
        public int GSM_TRX { get; internal set; }
        public string UMTS_Pwr_per_TRX { get; internal set; }
        public int UMTS_TRX { get; internal set; }
        public string LTE_Pwr_per_TRX { get; internal set; }
        public int LTE_TRX { get; internal set; }
        public string NR_Pwr_per_TRX { get; internal set; }
        public int NR_TRX { get; internal set; }
        public double Feeder_Att_dB { get; internal set; }

        public ModelRRU DeepCopy()
        {
            ModelRRU other = (ModelRRU)this.MemberwiseClone();

            return other;
        }

    }
}