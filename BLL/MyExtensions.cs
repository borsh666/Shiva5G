using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public static class MyExtensions
    {
        public static IEnumerable<int> AllIndexesOf(this string[] str, string searchstring)
        {
            var result = new List<int>();
             
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == searchstring)
                    result.Add(i);
            }

            return result;
        }
    }
}
