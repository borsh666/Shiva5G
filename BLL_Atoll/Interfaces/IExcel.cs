using BLL_Atoll.Enums;
using System.Reflection;

namespace BLL_Atoll.Interfaces
{
    public interface IExcel : IOffset
    {
        public SheetNames SheetName(PropertyInfo propInfo);
    }
}
