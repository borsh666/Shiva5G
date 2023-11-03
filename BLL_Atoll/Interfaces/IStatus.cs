using BLL_Atoll.Enums;

namespace BLL_Atoll.Interfaces
{
    public interface IStatus
    {
        Status Status { get; set; }
        void Errors();
    }
}