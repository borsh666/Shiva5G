namespace BLL_Atoll.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class ForPrintAttribute : Attribute
    {
        public ForPrintAttribute(string name)
        {
            this.Name = name;
        }
        public string Name { get; init; }
    }
}