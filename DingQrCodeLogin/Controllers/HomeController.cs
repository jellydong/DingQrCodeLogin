using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DingQrCodeLogin.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DingQrCodeLogin.Models;
using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;

namespace DingQrCodeLogin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }


        public string DingLogin(string code, string state)
        {
            //state 是前端传入的，钉钉并不会修改，比如有多种登录方式的时候，一个登录方法判断登录方式可以进行不同的处理。

            OapiSnsGetuserinfoBycodeResponse response = new OapiSnsGetuserinfoBycodeResponse();
            try
            {
                string qrAppId= AppConfigurtaionHelper.Configuration["DingDing:QrAppId"];
                string qrAppSecret = AppConfigurtaionHelper.Configuration["DingDing:QrAppSecret"];
                if (string.IsNullOrWhiteSpace(qrAppId)||string.IsNullOrWhiteSpace(qrAppSecret))
                {
                    throw new Exception("请先配置钉钉扫码登录信息！");
                }

                DefaultDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/sns/getuserinfo_bycode");
                OapiSnsGetuserinfoBycodeRequest req = new OapiSnsGetuserinfoBycodeRequest();
                req.TmpAuthCode = code;
                response = client.Execute(req, qrAppId, qrAppSecret);




                //获取到response后就可以进行自己的登录业务处理了

                //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                //此处省略一万行代码


            }
            catch (Exception e)
            {
                response.Errmsg = e.Message;
            }

            return response.Body;
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
