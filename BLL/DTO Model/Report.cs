using BLL.Attributes;
using BLL.Enums;
using BLL.Interfaces;
using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Collections.Generic;

namespace BLL.DTO
{
    public class Report : IReportExcel, IOffset
    {
        private string reportPath;
        private string userinfo;
        public string UserName => userinfo != String.Empty ? userinfo.Split('|').First() : String.Empty;
        public string UserMail => userinfo != String.Empty ? userinfo.Split('|').Skip(1).First() : String.Empty;
        public string UserPhone => userinfo != String.Empty ? userinfo.Split('|').Last() : String.Empty;

        public Report(string reportPath) : this(reportPath, String.Empty)
        {
            this.OffsetExcel = 1;
        }

        public Report(string reportPath, string userinfo)
        {
            this.ReportPath = reportPath;
            this.userinfo = userinfo;
            this.OffsetExcel = 1;
        }

        public string ReportPath
        {
            get
            {
                return reportPath;
            }

            set
            {
                reportPath = value;
            }
        }

        [ForPrint]
        public virtual ReportType Document { get; set; }
        [ForPrint]
        public string Time
        {
            get
            {
                return DateTime.Now.ToString("yyyy-MM-dd");
            }
        }
        [ForPrint]
        public string RF_Engineer
        {
            get
            {
                if (userinfo == String.Empty)
                    return GetDomainInfo()[0];
                else
                    return UserName;
            }
        }
        [ForPrint]
        public string Mobile
        {
            get
            {
                if (userinfo == String.Empty)
                    return GetDomainInfo()[1];
                else
                    return UserPhone;
            }
        }
        [ForPrint]
        public string Email
        {
            get
            {
                if (userinfo == String.Empty)
                    return GetDomainInfo()[2];
                else
                    return UserMail;
            }
        }


        public virtual string RangeFrom { get; set; }

        public virtual string RangeTo { get; set; }

        public virtual string TemplateFilePath { get; set; }
        public virtual string[] ForPrintCommonSheet { get; set; }
        public virtual string[] ForPrintSectorSheet { get; set; }
        public virtual string[] ForDataValidation { get; set; }

        //NEW
        public virtual string[] WorkSheetsName
        {
            get
            {
                return new string[]
                { "Common", "Sector 1", "Sector 2", "Sector 3", "Sector 4", "Sector 5", "Sector 6", "Sector 7", "Sector 8", "Sector 9" };
            }
           
        }


        public virtual int OffsetExcel { get; set; }

        public virtual IDictionary<int, string[]> Ranges { get; set; }

        public static int MaxAllowedSectors = 9;

        public static int MaxAllowedAntennas = 3;

        //Port distribution by antenna.Exapmple First Antenna - 6 port max
        public static List<int> MaxAllowedPorts = new List<int>() { 6, 4, 3 };

        public static List<int> AntennaOffsetSteps = new List<int>() { 1, 7 ,11 };

        public static List<int> PortOffsetSteps = new List<int>() { 0, 6 ,10 };


        private string[] GetDomainInfo()
        {
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString().Replace("AD\\", "");
            string[] result = { "", "", "" };
            PrincipalContext context = new PrincipalContext(ContextType.Domain);
            try
            {
                using (UserPrincipal user = UserPrincipal.FindByIdentity(context, userName))
                {
                    DirectoryEntry directoryEntry = user.GetUnderlyingObject() as DirectoryEntry;
                    object directoryPropertyValueCN = directoryEntry.Properties["cn"].Value;
                    object directoryPropertyValueMail = directoryEntry.Properties["mail"].Value;
                    object directoryPropertyValueMobile = directoryEntry.Properties["mobile"].Value;
                    result[0] = directoryPropertyValueCN.ToString();
                    result[1] = directoryPropertyValueMail.ToString();
                    result[2] = directoryPropertyValueMobile.ToString();

                    return result;
                }
            }
            catch (Exception)
            {
                return result;
            }
        }

    }
}
