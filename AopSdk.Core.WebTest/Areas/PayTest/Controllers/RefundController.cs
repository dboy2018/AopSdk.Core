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
    public class RefundController : Controller
    {
        private readonly IOptions<AliPayConfig> _settings;
        // GET: /<controller>/
        public RefundController(IOptions<AliPayConfig> _settings)
        {
            this._settings = _settings;
        }
        // GET
        public IActionResult Index()
        {
            return
            View();
        }

        public IActionResult Refund(string WIDout_trade_no,string WIDtrade_no,string WIDrefund_amount,string WIDrefund_reason,string WIDout_request_no)
        {
            DefaultAopClient client = new DefaultAopClient(_settings.Value.gateway_url, _settings.Value.app_id, _settings.Value.private_key, _settings.Value.format, _settings.Value.version, _settings.Value.sign_type, _settings.Value.alipay_public_key, _settings.Value.charset, false);

            // 商户订单号，和交易号不能同时为空
            string out_trade_no = WIDout_trade_no;

            // 支付宝交易号，和商户订单号不能同时为空
            string trade_no = WIDtrade_no;

            // 退款金额，不能大于订单总金额
            string refund_amount = WIDrefund_amount;

            // 退款原因
            string refund_reason = WIDrefund_reason;

            // 退款单号，同一笔多次退款需要保证唯一，部分退款该参数必填。
            string out_request_no = WIDout_request_no;

            AlipayTradeRefundModel model = new AlipayTradeRefundModel
            {
                OutTradeNo = out_trade_no,
                TradeNo = trade_no,
                RefundAmount = refund_amount,
                RefundReason = refund_reason,
                OutRequestNo = out_request_no
            };

            AlipayTradeRefundRequest request = new AlipayTradeRefundRequest();
            request.SetBizModel(model);

            AlipayTradeRefundResponse response = null;
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