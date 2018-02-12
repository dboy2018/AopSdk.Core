using System;
using System.Collections.Generic;
using System.IO;
using Aop.Api.Util;
using AopSdk.Core.WebTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AopSdk.Core.WebTest.Areas.NoticeTest.Controllers
{
    public class NotifyController : Controller
    {
        private readonly IOptions<AliPayConfig> _settings;

        public NotifyController(IOptions<AliPayConfig> _settings)
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
            var sArray = GetRequestPost();
            var msg = string.Empty;
            if (sArray.Count != 0)
            {
                var flag = AlipaySignature.RSACheckV1(sArray, _settings.Value.alipay_public_key,
                    _settings.Value.charset, _settings.Value.sign_type, false);
                if (flag)
                {
                    //交易状态
                    //判断该笔订单是否在商户网站中已经做过处理
                    //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                    //请务必判断请求时的total_amount与通知时获取的total_fee为一致的
                    //如果有做过处理，不执行商户的业务程序

                    //注意：
                    //退款日期超过可退款期限后（如三个月可退款），支付宝系统发送该交易状态通知
                    string trade_status = Request.Form["trade_status"];
                   

                    msg = "success";
                }
                else
                {
                    msg = "fail";
                }
            }
            System.IO.File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "result.txt"), msg);
            return Content(msg);
        }

        public Dictionary<string, string> GetRequestPost()
        {
            var sArray = new Dictionary<string, string>();

            var coll = Request.Form;
            foreach (var item in coll) sArray.Add(item.Key, item.Value);

            return sArray;
        }
    }
}