using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AopSdk.Core.WebTest.Models
{
    public class AliPayConfig
    {
        public string app_id { get; set; }
        public string gateway_url { get; set; }
        public string private_key { get; set; }
        public string alipay_public_key { get; set; }
        public string sign_type { get; set; }
        public string charset { get; set; }

        public string format { get; set; }
        public string version { get; set; }
        public string return_url { get; set; }
        public string notify_url { get; set; }


    }
}
