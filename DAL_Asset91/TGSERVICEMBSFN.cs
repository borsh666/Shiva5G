//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class TGSERVICEMBSFN
    {
        public decimal PROJECTNO { get; set; }
        public decimal SERVICEMBSFNPK { get; set; }
        public decimal SERVICEFK { get; set; }
        public decimal LTECARRIERMBSFNFK { get; set; }
        public decimal LTECARRIERFK { get; set; }
        public decimal PRIORITY { get; set; }
        public decimal MODIFYUSER { get; set; }
    
        public virtual LTECARRIERMBSFN LTECARRIERMBSFN { get; set; }
        public virtual TGSERVICE TGSERVICE { get; set; }
    }
}