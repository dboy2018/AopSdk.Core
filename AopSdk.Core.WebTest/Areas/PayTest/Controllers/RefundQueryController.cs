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
    public class RefundQueryController : Controller
    {
        private readonly IOptions<AliPayConfig> _settings;

        public RefundQueryController(IOptions<AliPayConfig> _settings)
        {
            this._settings = _settings;
        }

        // GET
        public IActionResult Index()
        {
            return
                View();
        }

        public IActionResult RefundQuery(string WIDout_trade_no, string WIDtrade_no, string WIDout_request_no)
        {
            var client = new DefaultAopClient(_settings.Value.gateway_url, _settings.Value.app_id,
                _settings.Value.private_key, _settings.Value.format, _settings.Value.version, _settings.Value.sign_type,
                _settings.Value.alipay_public_key, _settings.Value.charset, false);

            // 商户订单号，和交易号不能同时为空
            var out_trade_no = WIDout_trade_no;

            // 支付宝交易号，和商户订单号不能同时为空
            var trade_no = WIDtrade_no;

            // 请求退款接口时，传入的退款号，如果在退款时未传入该值，则该值为创建交易时的商户订单号，必填。
            var out_request_no = WIDout_request_no;

            var model = new AlipayTradeFastpayRefundQueryModel
            {
                OutTradeNo = out_trade_no,
                TradeNo = trade_no,
                OutRequestNo = out_request_no
            };

            var request = new AlipayTradeFastpayRefundQueryRequest();
            request.SetBizModel(model);

            AlipayTradeFastpayRefundQueryResponse response = null;
            try
            {
                response = client.Execute(request);
            }
            catch (Exception exp)
            {
                throw exp;
            }

            return Content(response.Body);
        }
    }
}