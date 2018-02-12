using System;
using System.Collections.Generic;
using System.IO;
using Aop.Api;
using Aop.Api.Request;
using Aop.Api.Util;

namespace AopSdk.Core.ConsoleTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            MenuGet();
            Console.WriteLine("------------------------------------------------------");
            CheckSign();
            Console.WriteLine("------------------------------------------------------");
            CheckSignAndDecrypt();
            Console.WriteLine("------------------------------------------------------");
            EncryptAndSign();
            Console.WriteLine("------------------------------------------------------");
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine(baseDirectory);
            var path = Path.Combine(baseDirectory, "result.txt");
            File.WriteAllText(path, "success");
            Console.Read();
        }


        private static void CheckSign()
        {
            IDictionary<string, string> paramsMap = new Dictionary<string, string>();
            paramsMap.Add("appId", "2013092500031084");
            var privateKeyPem = Path.Combine(GetCurrentPath(), "aop-sandbox-RSA-private-c#.pem");
            if (!File.Exists(privateKeyPem)) throw new FileNotFoundException();
            var sign = AlipaySignature.RSASign(paramsMap, privateKeyPem, null, "RSA");
            paramsMap.Add("sign", sign);
            var publicKey = Path.Combine(GetCurrentPath(), "public-key.pem");
            if (!File.Exists(publicKey)) throw new FileNotFoundException();
            var checkSign = AlipaySignature.RSACheckV2(paramsMap, publicKey);
            Console.Write("---------公众号通知消息签名验证--------" + "\n\r");
            Console.Write("checkSign:" + checkSign + "\n\r");
        }

        private static IAopClient GetAlipayClient()
        {
            //支付宝网关地址
            // -----沙箱地址-----
            var serverUrl = "https://openapi.alipaydev.com/gateway.do";
            // -----线上地址-----
            // string serverUrl = "https://openapi.alipay.com/gateway.do";
            //应用ID
            var appId = "2013092500031084";
//            var appId = "2016091100489306";
            //商户私钥
            var privateKeyPem = Path.Combine(GetCurrentPath(), "aop-sandbox-RSA-private-c#.pem");

            if (!File.Exists(privateKeyPem)) throw new FileNotFoundException();

            IAopClient client = new DefaultAopClient(serverUrl, appId, privateKeyPem);

            return client;
        }

        private static string GetCurrentPath()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(basePath + "Key");
        }

        private static void CheckSignAndDecrypt()
        {
            // 参数构建
            var charset = "UTF-8";
            var bizContent =
                "<XML><AppId><![CDATA[2013082200024893]]></AppId><FromUserId><![CDATA[2088102122485786]]></FromUserId><CreateTime>1377228401913</CreateTime><MsgType><![CDATA[click]]></MsgType><EventType><![CDATA[event]]></EventType><ActionParam><![CDATA[authentication]]></ActionParam><AgreementId><![CDATA[201308220000000994]]></AgreementId><AccountNo><![CDATA[null]]></AccountNo><UserInfo><![CDATA[{\"logon_id\":\"15858179811\",\"user_name\":\"许旦辉\"}]]></UserInfo></XML>";
            var publicKeyPem = Path.Combine(GetCurrentPath(), "public-key.pem");
            var privateKeyPem = Path.Combine(GetCurrentPath(), "aop-sandbox-RSA-private-c#.pem");
            if (!File.Exists(publicKeyPem)) throw new FileNotFoundException();
            if (!File.Exists(privateKeyPem)) throw new FileNotFoundException();
            IDictionary<string, string> paramsMap = new Dictionary<string, string>();
            paramsMap.Add("biz_content", AlipaySignature.RSAEncrypt(bizContent, publicKeyPem, charset));
            paramsMap.Add("charset", charset);
            paramsMap.Add("service", "alipay.mobile.public.message.notify");
            paramsMap.Add("sign_type", "RSA");
            paramsMap.Add("sign", AlipaySignature.RSASign(paramsMap, privateKeyPem, null, "RSA"));

            // 验签&解密
            var resultContent = AlipaySignature.CheckSignAndDecrypt(paramsMap, publicKeyPem, privateKeyPem, true, true);
            Console.Write("resultContent=" + resultContent + "\n\r");
        }

        private static void EncryptAndSign()
        {
            // 参数构建
            var bizContent =
                "<XML><ToUserId><![CDATA[2088102122494786]]></ToUserId><AppId><![CDATA[2013111100036093]]></AppId><AgreementId><![CDATA[20131111000001895078]]></AgreementId>"
                + "<CreateTime>12334349884</CreateTime>"
                + "<MsgType><![CDATA[image-text]]></MsgType>"
                + "<ArticleCount>1</ArticleCount>"
                + "<Articles>"
                + "<Item>"
                + "<Title><![CDATA[[回复测试加密解密]]></Title>"
                + "<Desc><![CDATA[测试加密解密]]></Desc>"
                + "<Url><![CDATA[http://m.taobao.com]]></Url>"
                + "<ActionName><![CDATA[立即前往]]></ActionName>"
                + "</Item>"
                + "</Articles>" + "<Push><![CDATA[false]]></Push>" + "</XML>";
            var publicKeyPem = Path.Combine(GetCurrentPath(), "public-key.pem");
            var privateKeyPem = Path.Combine(GetCurrentPath(), "aop-sandbox-RSA-private-c#.pem");
            if (!File.Exists(publicKeyPem)) throw new FileNotFoundException();
            if (!File.Exists(privateKeyPem)) throw new FileNotFoundException();
            var responseContent =
                AlipaySignature.encryptAndSign(bizContent, publicKeyPem, privateKeyPem, "UTF-8", true, true);
            Console.Write("resultContent=" + responseContent + "\n\r");
        }


        private static void MenuGet()
        {
            var client = GetAlipayClient();
            var req = new AlipayMobilePublicMenuGetRequest();
            var res = client.Execute(req);
            Console.Write("-------------公众号菜单查询-------------" + "\n\r");
            Console.Write("Body:" + res.Body + "\n\r");
        }
    }
}