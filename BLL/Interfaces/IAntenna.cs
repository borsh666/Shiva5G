namespace BLL.Interfaces
{
    interface IAntenna
    {
        string SectorNumber { get; set; }
        string AntennaType { get; set; }
        decimal PhyIndex { get; set; }
    }
}