using BLL_Atoll.Enums;

namespace BLL_Atoll.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]

    public class ReportAttribute : Attribute
    {
        public ReportAttribute(ReportType[] repotTypes)
        {
            this.RepotTypes = repotTypes;
        }
        public ReportType[] RepotTypes { get; init; }
    }
}


