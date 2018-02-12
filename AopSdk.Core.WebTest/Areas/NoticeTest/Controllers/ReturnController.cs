using System.Collections.Generic;
using Aop.Api.Util;
using AopSdk.Core.WebTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AopSdk.Core.WebTest.Areas.NoticeTest.Controllers
{
    [Area("NoticeTest")]
    public class ReturnController : Controller
    {
        private readonly IOptions<AliPayConfig> _settings;

        public ReturnController(IOptions<AliPayConfig> _settings)
        {
            this._settings = _settings;
        }

        // GET
        public IActionResult Index()
        {
            /* 实际验证过程建议商户添加以下校验。
             1、商户需要验证该通知数据中的out_trade_no是否为商户系统中创建的订单号，
             2、判断total_amount是否确实为该订单的实际金额（即商户订单创建时的金额），
             3、校验通知中的seller_id（或者seller_email) 是否为out_trade_no这笔单据的对应的操作方（有的时候，一个商户可能有多个seller_id/seller_email）
             4、验证app_id是否为该商户本身。
             */
            var sArray = GetRequestGet();
            var msg = string.Empty;
            if (sArray.Count != 0)
            {
                var flag = AlipaySignature.RSACheckV1(sArray, _settings.Value.alipay_public_key,
                    _settings.Value.charset, _settings.Value.sign_type, false);
                if (flag)
                    msg = "同步验证通过";
                else
                    msg = "同步验证失败";
            }

            return Content(msg);
        }

        public Dictionary<string, string> GetRequestGet()
        {
            var sArray = new Dictionary<string, string>();

            var coll = Request.Query;

            foreach (var item in coll) sArray.Add(item.Key, item.Value);

            return sArray;
        }
    }
}