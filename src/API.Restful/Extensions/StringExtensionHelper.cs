using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Restful.Helpers
{
    public static class StringExtensionHelper
    {
        public static bool IsApplicationHal(this string s)
        {
            return s.Equals("application/hal+json");
        }
    }
}
