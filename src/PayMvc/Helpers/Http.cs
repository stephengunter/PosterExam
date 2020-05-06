using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;

namespace PayMvc.Helpers
{
    public static class HttpHelpers
    {
		public static string GetOrigin(this HttpRequestMessage httpRequest)
		{
            if (httpRequest.Headers.Contains("Origin"))
            {
                var values = httpRequest.Headers.GetValues("Origin");
                return values.IsNullOrEmpty() ? "" : values.FirstOrDefault();
            }
            return "";
        }
    }
}