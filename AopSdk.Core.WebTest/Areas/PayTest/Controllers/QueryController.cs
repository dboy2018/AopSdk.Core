using System;
using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;
using AopSdk.Core.WebTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AopSdk.Core.WebTest.Areas.PayTest.Controllers
{
    [Area("PayTest")]
    public class QueryController : Controller
    {
        private readonly IOptions<AliPayConfig> _settings;

        public QueryController(IOptions<AliPayConfig> _settings)
        {
            this._settings = _settings;
        }
        // GET
        public IActionResult Index()
        {
            return
            View();
        }

        public IActionResult Query(string WIDout_trade_no,string WIDtrade_no)
        {
            DefaultAopClient client = new DefaultAopClient(_settings.Value.gateway_url, _settings.Value.app_id, _settings.Value.private_key, _settings.Value.format, _settings.Value.version, _settings.Value.sign_type, _settings.Value.alipay_public_key, _settings.Value.charset, false);

            // 商户订单号，和交易号不能同时为空
            string out_trade_no = WIDout_trade_no;

            // 支付宝交易号，和商户订单号不能同时为空
            string trade_no = WIDtrade_no;

            AlipayTradeQueryModel model = new AlipayTradeQueryModel
            {
                OutTradeNo = out_trade_no,
                TradeNo = trade_no
            };

            AlipayTradeQueryRequest request = new AlipayTradeQueryRequest();
            request.SetBizModel(model);

            AlipayTradeQueryResponse response = null;
            try
            {
                response = client.Execute(request);
                // response.Body;

            }
            catch (Exception exp)
            {
                throw exp;
            }

            return Content(response.Body);
        }
    }
}