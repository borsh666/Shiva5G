using System;


namespace BLL.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ForPrintAttribute : Attribute
    {
    }
}