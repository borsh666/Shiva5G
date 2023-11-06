using BLL_Atoll.Enums;
using BLL_Atoll.Models.Site;
using Dapper;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace BLL_Atoll
{
    public static class SupportFunc
    {
        public static string ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string result = appSettings[key] ?? 
                    throw new ConfigurationErrorsException($"Not Found such {key} key");
               return result;
            }
            catch (ConfigurationErrorsException)
            {
                throw new ConfigurationErrorsException("Error reading app settings");
            }
        }

        public static List<string> EnumDisplayNameAttToList<T>()
        {

            return Enum.GetValues(typeof(T))
             .Cast<T>()
             .Select(n => n.GetType()
                           .GetMember(n.ToString()!)
                           .First()
                           .GetCustomAttribute<DisplayAttribute>()!
                           .GetName()!)
              .ToList();
        }

        public static string GetEnumDisplayName<T>(T enumName)
        {
            MemberInfo property = typeof(T).GetMember(enumName.ToString()).First();
            var dd = property.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
            if (dd != null)
                return dd.Name;
            else
                return enumName.ToString();
        }


        public static List<dynamic> GetFromDb(string query, string connStr)
        {
            using var con = new SqlConnection(connStr);
            var result = new List<dynamic>();

            con.Open();
            result = con.Query<dynamic>(query, commandTimeout: 600).ToList();
            con.Close();

            return result;

        }

        public static double CombinerSplitterLossDb(string combiner_Splitters)
        {
            var combiner_Splitter_Loss = File.ReadAllLines(Global.COMBINER_SPLITER_LOSS);

            //Combiner_Splitter|Loss
            //var combiner_Splitter_Loss = Properties.Resource.Combiner_Splitter_Loss
            //    .Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            for (int i = 1; i < combiner_Splitter_Loss.Length; i++)
            {
                var fileDelimeter = '|';
                var parts = combiner_Splitter_Loss[i].Split(fileDelimeter).ToList();
                var combiner_SplitterPart = parts.First();
                var lossPart = parts.Last();


                if (combiner_Splitters == combiner_SplitterPart
                    && double.TryParse(lossPart, out double loss))
                    return loss;
            }
            return 0;
        }

        public static double ConvertW_dBm(double w)
        {
            return 10 * Math.Log10(1000 * w);
        }

        public static double ConvertdBm_W(double dBm)
        {
            return Math.Pow(10, (dBm - 30) / 10);
        }


        public static List<T> MapElement<T>(List<dynamic> siteInfo)
        {
            string eleJson = Newtonsoft.Json.JsonConvert.SerializeObject(siteInfo);
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(eleJson);
            return obj;
        }

        //Проверява дали дадено пропърти има атрибут
        public static bool HasAttribute(Type attributeType, PropertyInfo prop)
        {

            var custAttribType = prop.CustomAttributes.ToList();
            if (custAttribType.Count > 0)
            {
                if (custAttribType.Any(n => n.AttributeType.Name == attributeType.Name))
                {
                    return true;
                }
                else
                    return false;
            }
            return false;
        }

        public static void CheckForCorrectSiteName(string siteID)
        {
            if (string.IsNullOrEmpty(siteID) || !Regex.IsMatch(siteID, "^[A-Z]{2}[\\d]{4}$"))
            {
                throw new ArgumentException("Моля въведете правилно SiteID. Пример: SF1001");
            }
        }

        public static int SumTrx(Technology tech, List<Cell> cells)
        {
            var trx = cells
               .Where(n => n.Technology == tech)
               .Select(n => n.Trx());
            return trx.Sum();
        }

        public static List<double> FilterPowersPerTech(Technology tech, List<Cell> cells)
        {
            var powers = cells.Where(n => n.Technology == tech)
                              .Select(n => Math.Round(n.TrxPower, 2));

            if (powers.Any())
                return powers.ToList();
            else
                return new List<double>();
        }

        public static double SumPowers_W(List<double> powers_dbm)
        {
            powers_dbm.ForEach(n => ConvertdBm_W(n));
            return powers_dbm.Sum();
        }

        public static T CopyObject<T>(object objSource)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, objSource);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }


        public static Dictionary<int, int> AntLastIdBySec(List<Antenna> antennasGroup)
        {
            return antennasGroup.GroupBy(n => n.SectorId)
                .Select(x => new
                {
                    SectorId = x.Key,
                    MaxAntennaId = x.Select(n => n.AntennaId).Max()
                })
                .ToDictionary(n => n.SectorId, n => n.MaxAntennaId);
        }

        public static List<Port> PortsGroupByAntPort(List<Port> ports) =>
            ports
            .GroupBy(o => new { o.AntennaId, o.PortId })
            .Select(o => o.First())
            .OrderBy(n => n.SectorId)
            .ThenBy(n => n.AntennaId)
            .ThenBy(n => n.PortId)
            .ToList();
    }
}
