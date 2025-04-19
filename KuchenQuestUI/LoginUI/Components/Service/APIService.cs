using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginUI.Components.Service
{
    class APIService
    {
        public static string API_IP = "192.168.178.111";

        public string GetAPI_IP() => APIService.API_IP;
    }
}
