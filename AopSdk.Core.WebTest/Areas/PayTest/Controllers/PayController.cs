using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;
using AopSdk.Core.WebTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AopSdk.Core.WebTest.Areas.PayTest.Controllers
{
    [Area("PayTest")]
    public class PayController : Controller
    {
        private readonly IOptions<AliPayConfig> _settings;
        // GET: /<controller>/
        public PayController(IOptions<AliPayConfig> _settings)
        {
            this._settings = _settings;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Pay(string WIDout_trade_no,string WIDsubject,string WIDtotal_amount,string WIDbody)
        {
           
            DefaultAopClient client = new DefaultAopClient(_settings.Value.gateway_url, _settings.Value.app_id, _settings.Value.private_key, _settings.Value.format, _settings.Value.version, _settings.Value.sign_type, _settings.Value.alipay_public_key, _settings.Value.charset, false);

            // 外部订单号，商户网站订单系统中唯一的订单号
            string out_trade_no = WIDout_trade_no;

            // 订单名称
            string subject = WIDsubject;

            // 付款金额
            string total_amout = WIDtotal_amount;

            // 商品描述
            string body = WIDbody;

            // 组装业务参数model
            AlipayTradePagePayModel model = new AlipayTradePagePayModel
            {
                Body = body,
                Subject = subject,
                TotalAmount = total_amout,
                OutTradeNo = out_trade_no,
                ProductCode = "FAST_INSTANT_TRADE_PAY"
            };

            AlipayTradePagePayRequest request = new AlipayTradePagePayRequest();
            
            // 设置同步回调地址
            request.SetReturnUrl(_settings.Value.return_url);
            // 设置异步通知接收地址
            request.SetNotifyUrl(_settings.Value.notify_url);
            // 将业务model载入到request
            request.SetBizModel(model);

            AlipayTradePagePayResponse response = null;
            try
            {
                response = client.pageExecute(request, null, "post");
               
                ViewBag.Body = response.Body;
               
            }
            catch (Exception exp)
            {
                throw exp;
            }

            return View();
        }
    }
}
