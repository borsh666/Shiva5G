//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL_Asset10
{
    using System;
    using System.Collections.Generic;
    
    public partial class RADIOMASK
    {
        public decimal PROJECTNO { get; set; }
        public decimal RADIOEQUIPPK { get; set; }
        public decimal MASKTYPE { get; set; }
        public decimal OFFSETVALUE { get; set; }
        public Nullable<decimal> ATTENVALUE { get; set; }
        public Nullable<decimal> MODIFYUSER { get; set; }
    
        public virtual RADIOEQUIP RADIOEQUIP { get; set; }
    }
}