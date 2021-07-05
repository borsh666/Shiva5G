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
    
    public partial class PTPLINKEND
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PTPLINKEND()
        {
            this.LINKAMCTHRESHOLDs = new HashSet<LINKAMCTHRESHOLD>();
            this.LINKENDANTENNAs = new HashSet<LINKENDANTENNA>();
            this.LINKENDFEEDs = new HashSet<LINKENDFEED>();
        }
    
        public decimal PROJECTNO { get; set; }
        public decimal PTPLINKENDKEY { get; set; }
        public decimal LINKKEY { get; set; }
        public decimal SITEADDRKEY { get; set; }
        public Nullable<decimal> ISENDA { get; set; }
        public Nullable<decimal> LINKTERMEQUIPKEY { get; set; }
        public Nullable<decimal> AVGREFRACTIVITY { get; set; }
        public Nullable<decimal> MINANTHEIGHT { get; set; }
        public Nullable<decimal> INCLINATION { get; set; }
        public Nullable<decimal> BEARING { get; set; }
        public Nullable<decimal> FREQDESIG { get; set; }
        public Nullable<decimal> FREQCENTRE { get; set; }
        public Nullable<decimal> FREQBW { get; set; }
        public Nullable<decimal> RADIOEQUIPKEY { get; set; }
        public Nullable<decimal> RADMOU { get; set; }
        public Nullable<decimal> RADTHRESH { get; set; }
        public Nullable<decimal> TXATTENUATOR { get; set; }
        public Nullable<decimal> RXATTENUATOR { get; set; }
        public Nullable<decimal> TXBRANCHLOSS { get; set; }
        public Nullable<decimal> RXBRANCHLOSS { get; set; }
        public Nullable<decimal> TXMISCLOSS { get; set; }
        public Nullable<decimal> RXMISCLOSS { get; set; }
        public Nullable<decimal> DIVFREQDESIG { get; set; }
        public Nullable<decimal> DIVFREQCENTRE { get; set; }
        public Nullable<decimal> DIVFREQBW { get; set; }
        public Nullable<decimal> DIVRADIOEQUIP { get; set; }
        public Nullable<decimal> DIVRADMOU { get; set; }
        public Nullable<decimal> DIVRADTHRESH { get; set; }
        public Nullable<decimal> DIVTXATTENUATOR { get; set; }
        public Nullable<decimal> DIVRXATTENUATOR { get; set; }
        public Nullable<decimal> DIVTXBRANCHLOSS { get; set; }
        public Nullable<decimal> DIVRXBRANCHLOSS { get; set; }
        public Nullable<decimal> DIVTXMISCLOSS { get; set; }
        public Nullable<decimal> DIVRXMISCLOSS { get; set; }
        public Nullable<decimal> PERMISSION { get; set; }
        public Nullable<decimal> CREATEUSER { get; set; }
        public Nullable<decimal> USERGROUP { get; set; }
        public Nullable<System.DateTime> CREATEDATE { get; set; }
        public Nullable<System.DateTime> MODIFYDATE { get; set; }
        public decimal MODIFYUSER { get; set; }
        public Nullable<decimal> ENABLEATPC { get; set; }
        public Nullable<decimal> OVERRIDEATPCRANGE { get; set; }
        public Nullable<decimal> ENABLENOMPOWER { get; set; }
        public Nullable<decimal> DIVENABLEATPC { get; set; }
        public Nullable<decimal> DIVOVERRIDEATPCRANGE { get; set; }
        public Nullable<decimal> DIVENABLENOMPOWER { get; set; }
        public Nullable<decimal> OBJECTTYPE { get; set; }
        public Nullable<decimal> DIVCMPFADEMARGIN { get; set; }
        public Nullable<decimal> DIVOVERRIDEAMC { get; set; }
        public Nullable<decimal> ANNUALAVAIL { get; set; }
        public Nullable<decimal> OVERRIDEDIVSPACING { get; set; }
        public Nullable<decimal> DIVENABLEAMC { get; set; }
        public Nullable<decimal> DIVOVERRIDEMODULATIONTYPE { get; set; }
        public Nullable<decimal> DIVREQUIREDTHROUGHPUT { get; set; }
        public Nullable<decimal> DIVSUGGESTEDMODULATIONTYPE { get; set; }
        public Nullable<decimal> RXLEVEL { get; set; }
        public Nullable<decimal> ENABLEAMC { get; set; }
        public Nullable<decimal> ANNUALRELIAB { get; set; }
        public Nullable<decimal> OVERRIDEAMC { get; set; }
        public Nullable<decimal> REQUIREDTHROUGHPUT { get; set; }
        public Nullable<decimal> OVERRIDEMODULATIONTYPE { get; set; }
        public Nullable<decimal> CMPFADEMARGIN { get; set; }
        public Nullable<decimal> SUGGESTEDMODULATIONTYPE { get; set; }
        public Nullable<decimal> DIVRXLEVEL { get; set; }
        public Nullable<decimal> DIVHIGHPRIORITYTHROUGHPUT { get; set; }
        public Nullable<decimal> DIVREQUIREDAVAILABILITY { get; set; }
        public Nullable<decimal> HIGHPRIORITYTHROUGHPUT { get; set; }
        public Nullable<decimal> REQUIREDAVAILABILITY { get; set; }
        public Nullable<decimal> ADDITIONALLINKTRAFFIC { get; set; }
        public Nullable<decimal> DIVHIGHPRIORITYAVAILABLITY { get; set; }
        public Nullable<decimal> HIGHPRIORITYAVAILABLITY { get; set; }
        public Nullable<decimal> CHANNELCAPACITYENUM { get; set; }
        public Nullable<decimal> ETHERNETIP { get; set; }
        public Nullable<decimal> NOOFCHANNELS { get; set; }
        public Nullable<decimal> USEPDHSDH { get; set; }
        public string CHANNELLIST { get; set; }
        public Nullable<decimal> LINKOCCUPANCY { get; set; }
        public Nullable<decimal> USEDAVAILABILITY { get; set; }
        public Nullable<decimal> USEDCAPACITY { get; set; }
        public Nullable<decimal> DIVENABLEXPIC { get; set; }
        public Nullable<decimal> DIVOVERRIDEXPIFVALUE { get; set; }
        public Nullable<decimal> DIVSPACINGREF { get; set; }
        public Nullable<decimal> ENABLEXPIC { get; set; }
        public Nullable<decimal> OVERRIDEDIVSPACINGVAL { get; set; }
        public Nullable<decimal> OVERRIDEXPIFVALUE { get; set; }
        public Nullable<decimal> ANNUALAVAILP { get; set; }
        public Nullable<decimal> ANNUALAVAILSYR { get; set; }
        public Nullable<decimal> ANNUALUNAVAILP { get; set; }
        public Nullable<decimal> ANNUALUNAVAILSYR { get; set; }
        public Nullable<decimal> ATMOSABSORTION { get; set; }
        public Nullable<decimal> AVAILABILITYP { get; set; }
        public Nullable<decimal> AVAILABLECAPACITY { get; set; }
        public Nullable<decimal> CALFM { get; set; }
        public string CAPACITYSTATUS { get; set; }
        public Nullable<decimal> CONTROLOVERHEAD { get; set; }
        public Nullable<decimal> CROSSPOLOUTAGE { get; set; }
        public Nullable<decimal> DISPERSIVEFM { get; set; }
        public Nullable<decimal> DIVANNUALAVAILP { get; set; }
        public Nullable<decimal> DIVANNUALAVAILSYR { get; set; }
        public Nullable<decimal> DIVANNUALUNAVAILP { get; set; }
        public Nullable<decimal> DIVANNUALUNAVAILSYR { get; set; }
        public Nullable<decimal> DIVATMOSABSORTION { get; set; }
        public Nullable<decimal> DIVAVAILABILITYP { get; set; }
        public Nullable<decimal> DIVCALFM { get; set; }
        public Nullable<decimal> DIVCROSSPOLOUTAGE { get; set; }
        public Nullable<decimal> DIVDISPERSIVEFM { get; set; }
        public Nullable<decimal> DIVEQUIPMENTPE { get; set; }
        public Nullable<decimal> DIVFLATFM { get; set; }
        public Nullable<decimal> DIVFLATFMAINTERF { get; set; }
        public Nullable<decimal> DIVFLATOUTAGE { get; set; }
        public Nullable<decimal> DIVFSLOSS { get; set; }
        public Nullable<decimal> DIVIMPROVEMENTFACTOR { get; set; }
        public Nullable<decimal> DIVINTERFMARGIN { get; set; }
        public Nullable<decimal> DIVLINKBBER { get; set; }
        public Nullable<decimal> DIVLINKENDANTGAIN { get; set; }
        public Nullable<decimal> DIVLINKESR { get; set; }
        public Nullable<decimal> DIVLINKPER { get; set; }
        public Nullable<decimal> DIVLINKSESR { get; set; }
        public Nullable<decimal> DIVMAXACHIEVABLETPUT { get; set; }
        public Nullable<decimal> DIVOBSTRUCTIONLOSS { get; set; }
        public Nullable<decimal> DIVOPTIMUMAVAILCAP { get; set; }
        public Nullable<decimal> DIVOPTIMUMPACKSIZE { get; set; }
        public Nullable<decimal> DIVOUTAGEAFTDIVERSITY { get; set; }
        public Nullable<decimal> DIVPERCENTAGEOFTIMEMODE { get; set; }
        public Nullable<decimal> DIVRAINPR { get; set; }
        public Nullable<decimal> DIVSELECTIVEOUTAGE { get; set; }
        public Nullable<decimal> DIVTHRESHDEGRAD { get; set; }
        public Nullable<decimal> DIVTHRESHOLDVALUE { get; set; }
        public Nullable<decimal> DIVTHROUGHPUTMBPS { get; set; }
        public Nullable<decimal> DIVTOTALLOSS { get; set; }
        public Nullable<decimal> DIVTOTANNUALOUTAGEP { get; set; }
        public Nullable<decimal> DIVTOTANNUALOUTAGEPTP { get; set; }
        public Nullable<decimal> DIVTOTANNUALOUTAGEPTSYR { get; set; }
        public Nullable<decimal> DIVTOTANNUALOUTAGESYR { get; set; }
        public Nullable<decimal> DIVTOTANNUALRELP { get; set; }
        public Nullable<decimal> DIVTOTANNUALRELSYR { get; set; }
        public Nullable<decimal> DIVTOTANNUALUNRELP { get; set; }
        public Nullable<decimal> DIVTOTANNUALUNRELSYR { get; set; }
        public Nullable<decimal> DIVTOTANTTENLOSS { get; set; }
        public Nullable<decimal> DIVTOTBRANCHLOSS { get; set; }
        public Nullable<decimal> DIVTOTMISCLOSS { get; set; }
        public Nullable<decimal> DIVTOTWMOUTAGEP { get; set; }
        public Nullable<decimal> DIVTOTWMOUTAGEPTP { get; set; }
        public Nullable<decimal> DIVTOTWMOUTAGEPTSYR { get; set; }
        public Nullable<decimal> DIVTOTWMOUTAGESYR { get; set; }
        public Nullable<decimal> DIVTOTWMRELP { get; set; }
        public Nullable<decimal> DIVTOTWMRELSYR { get; set; }
        public Nullable<decimal> DIVTOTWMUNRELP { get; set; }
        public Nullable<decimal> DIVTOTWMUNRELSYR { get; set; }
        public Nullable<decimal> DIVWMAVAILP { get; set; }
        public Nullable<decimal> DIVWMAVAILSYR { get; set; }
        public Nullable<decimal> DIVWMUNAVAILP { get; set; }
        public Nullable<decimal> DIVWMUNAVAILSYR { get; set; }
        public Nullable<decimal> EQUIPMENTPE { get; set; }
        public Nullable<decimal> ETHERNETCAPACITY { get; set; }
        public Nullable<decimal> FLATFM { get; set; }
        public Nullable<decimal> FLATFMAINTERF { get; set; }
        public Nullable<decimal> FLATOUTAGE { get; set; }
        public Nullable<decimal> FSLOSS { get; set; }
        public Nullable<decimal> IMPROVEMENTFACTOR { get; set; }
        public Nullable<decimal> INTERFMARGIN { get; set; }
        public Nullable<decimal> LINKBBER { get; set; }
        public Nullable<decimal> LINKENDANTGAIN { get; set; }
        public Nullable<decimal> LINKESR { get; set; }
        public Nullable<decimal> LINKPER { get; set; }
        public Nullable<decimal> LINKSESR { get; set; }
        public Nullable<decimal> MAXACHIEVABLETPUT { get; set; }
        public Nullable<decimal> OBSTRUCTIONLOSS { get; set; }
        public Nullable<decimal> OPTIMUMAVAILCAP { get; set; }
        public Nullable<decimal> OPTIMUMPACKSIZE { get; set; }
        public Nullable<decimal> OUTAGEAFTDIVERSITY { get; set; }
        public Nullable<decimal> PERCENTAGEOFTIMEMODE { get; set; }
        public Nullable<decimal> RAINPR { get; set; }
        public Nullable<decimal> SELECTIVEOUTAGE { get; set; }
        public Nullable<decimal> TDMCAPACITY { get; set; }
        public Nullable<decimal> THRESHDEGRAD { get; set; }
        public Nullable<decimal> THRESHOLDVALUE { get; set; }
        public Nullable<decimal> THROUGHPUTMBPS { get; set; }
        public Nullable<decimal> TOTALCAP { get; set; }
        public Nullable<decimal> TOTALCSTRAFFIC { get; set; }
        public Nullable<decimal> TOTALLOSS { get; set; }
        public Nullable<decimal> TOTALPSTRAFFIC { get; set; }
        public Nullable<decimal> TOTANNUALOUTAGEP { get; set; }
        public Nullable<decimal> TOTANNUALOUTAGEPTP { get; set; }
        public Nullable<decimal> TOTANNUALOUTAGEPTSYR { get; set; }
        public Nullable<decimal> TOTANNUALOUTAGESYR { get; set; }
        public Nullable<decimal> TOTANNUALRELP { get; set; }
        public Nullable<decimal> TOTANNUALRELSYR { get; set; }
        public Nullable<decimal> TOTANNUALUNRELP { get; set; }
        public Nullable<decimal> TOTANNUALUNRELSYR { get; set; }
        public Nullable<decimal> TOTANTTENLOSS { get; set; }
        public Nullable<decimal> TOTBRANCHLOSS { get; set; }
        public Nullable<decimal> TOTMISCLOSS { get; set; }
        public Nullable<decimal> TOTWMOUTAGEP { get; set; }
        public Nullable<decimal> TOTWMOUTAGEPTP { get; set; }
        public Nullable<decimal> TOTWMOUTAGEPTSYR { get; set; }
        public Nullable<decimal> TOTWMOUTAGESYR { get; set; }
        public Nullable<decimal> TOTWMRELP { get; set; }
        public Nullable<decimal> TOTWMRELSYR { get; set; }
        public Nullable<decimal> TOTWMUNRELP { get; set; }
        public Nullable<decimal> TOTWMUNRELSYR { get; set; }
        public Nullable<decimal> WMAVAILP { get; set; }
        public Nullable<decimal> WMAVAILSYR { get; set; }
        public Nullable<decimal> WMUNAVAILP { get; set; }
        public Nullable<decimal> WMUNAVAILSYR { get; set; }
    
        public virtual LINK LINK { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LINKAMCTHRESHOLD> LINKAMCTHRESHOLDs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LINKENDANTENNA> LINKENDANTENNAs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LINKENDFEED> LINKENDFEEDs { get; set; }
        public virtual SITEADDRESS SITEADDRESS { get; set; }
    }
}