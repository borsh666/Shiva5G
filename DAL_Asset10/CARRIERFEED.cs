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
    
    public partial class CARRIERFEED
    {
        public decimal PROJECTNO { get; set; }
        public decimal CARRIERFEEDERSETTINGSKEY { get; set; }
        public decimal PMPCARRIERKEY { get; set; }
        public decimal SECTORANTENNAKEY { get; set; }
        public Nullable<decimal> FEEDERKEY { get; set; }
        public Nullable<decimal> FEEDERLENGTH { get; set; }
        public Nullable<decimal> OVERRIDEFEEDERLOSS { get; set; }
        public Nullable<decimal> USERFEEDERLOSS { get; set; }
        public Nullable<decimal> RADIOFLAG { get; set; }
        public decimal MODIFYUSER { get; set; }
    
        public virtual PMPCARRIER PMPCARRIER { get; set; }
    }
}