using BLL_Atoll.Enums;
using BLL_Atoll.Interfaces;
using static BLL_Atoll.SupportFunc;

namespace BLL_Atoll.Models.Site
{
    public abstract record SectorAntStatus : ISector, IAntenna, IStatus
    {
        public int AntennaId { get; set; }
        public int SectorId { get; init; }
        public Status Status { get; set; }
        public AntennaLevel AntennaLevel { get; set; }

        public abstract void Errors();

        public void AddAntennaIdForSecRemoteEmptyAntenna(List<Antenna> antennas)
        {
            var antLastIdBySec = AntLastIdBySec(antennas);

            if (AntennaLevel is AntennaLevel.Secondary or
                AntennaLevel.Empty)
            {
                AntennaId = antLastIdBySec.Count > 0 ?
                            antLastIdBySec[SectorId] + 1 :
                            SectorId * 10 + 1;
            }

            var isThisAntennaIdExistInAntennas =
                antennas
                .Select(n => n.AntennaId)
                .Contains(AntennaId);

            if (AntennaLevel == AntennaLevel.Remote &&
                isThisAntennaIdExistInAntennas)
            {
                Status = Status.NotOk;
            }
        }
    }
}