### 钉钉扫码登录网站(两种方式实现)

#### 效果：
![效果]( /lib/DingQrCodeLogin.gif "效果")

> > 源代码地址：https://github.com/jellydong/DingQrCodeLogin
#### 动手敲代码！

##### 第一步，钉钉后台配置 
参考链接：[获取appId及appSecret](https://ding-doc.dingtalk.com/doc#/serverapi2/kymkv6 "获取appId及appSecret").
> 点击进入钉钉开发者平台 的页面，点击左侧菜单的【移动接入应用-登录】，然后点击右上角的【创建扫码登录应用授权】，创建用于免登过程中验证身份的appId及appSecret，创建后即可看到appId和appSecret。

> 这里因为我是本地开发，所以回调地址直接写：http://localhost:5000/Home/DingLogin
> 注意哦，回调地址后面是有使用的~

![钉钉后台配置]( /lib/钉钉配置.png "钉钉后台配置")

##### 第二部 我们创建一个 ASP.NET Core Web项目

###### 修改appsettings.json

修改appsettings.json，增加钉钉的配置信息：

``` json
  "DingDing": {
     "QrAppId": "QrAppId", //你的钉钉扫码登录AppId
    "QrAppSecret": "QrAppSecret" //你的钉钉扫码登录AppSecret
  }
```

###### 创建完成修改Home控制器的Index页面`其实就是钉钉官网文档的代码啦~`：

``` csharp
@{
    ViewData["Title"] = "Home Page";
}

<div class="text-center">
    <h1 class="display-4">Welcome</h1>
    <div id="login_container"></div>
    <button type="button" class="btn btn-primary" id="JumpToLogin">跳转登录</button>
</div>


@section Scripts
{
    <script src="https://g.alicdn.com/dingding/dinglogin/0.0.5/ddLogin.js"></script>
    <script type="text/javascript">
        /*
        * 解释一下goto参数，参考以下例子：
        * var url = encodeURIComponent('http://localhost.me/index.php?test=1&aa=2');
        * var goto = encodeURIComponent('https://oapi.dingtalk.com/connect/oauth2/sns_authorize?appid=appid&response_type=code&scope=snsapi_login&state=STATE&redirect_uri='+url)
        */
        var url = "http://localhost:5000/Home/DingLogin";
        var obj = DDLogin({
            id: "login_container",//这里需要你在自己的页面定义一个HTML标签并设置id，例如<div id="login_container"></div>或<span id="login_container"></span>
            goto: encodeURIComponent('https://oapi.dingtalk.com/connect/oauth2/sns_authorize?appid=appid&response_type=code&scope=snsapi_login&state=STATE&redirect_uri=' + url), //请参考注释里的方式
            style: "border:none;background-color:#FFFFFF;",
            width: "365",
            height: "400"
        });

        var handleMessage = function (event) {
            var origin = event.origin;
            console.log("origin", event.origin);
            if (origin == "https://login.dingtalk.com") { //判断是否来自ddLogin扫码事件。
                var loginTmpCode = event.data; //拿到loginTmpCode后就可以在这里构造跳转链接进行跳转了
                console.log("loginTmpCode", loginTmpCode);

                window.location.href =
                    "https://oapi.dingtalk.com/connect/oauth2/sns_authorize?appid=appid&response_type=code&scope=snsapi_login&state=STATE&redirect_uri=REDIRECT_URI&loginTmpCode=" +
                    loginTmpCode;
            }
        };
        if (typeof window.addEventListener != 'undefined') {
            window.addEventListener('message', handleMessage, false);
        } else if (typeof window.attachEvent != 'undefined') {
            window.attachEvent('onmessage', handleMessage);
        }

        $("#JumpToLogin").click(function(){
            window.location.href =
                "https://oapi.dingtalk.com/connect/qrconnect?appid=appid&response_type=code&scope=snsapi_login&state=LoginDing&redirect_uri=http://localhost:5000/Home/DingLogin";
        });
    </script>
}

```
官网介绍了两种方式，Demo把两种方式都放到一个页面了。登录页面效果：
![登录页面效果]( /lib/登录页.png "登录页面效果")

##### 第三步 回调方法：
第一步的时候我们说回调地址是需要使用的，那么首先我们要有这个地址啊。
因为是Demo，就直接写在HomeController中了
``` csharp
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
```
##### 登录结果
完成上述步骤后，我们就可以运行项目测试了，钉钉会给我们返回用户的`nick`、`openid`及`unionid`，那么，我们可以用这些信息，为所欲为了？
![登录结果]( /lib/登录结果.png "登录结果")


##### 总结

之前过于钉钉扫码，总觉得是很高大上的东西(`原谅我是个菜鸡`),也没有去尝试。
今天看完文档后，用在项目上，然后写了这个Demo,因为我Github没找到合适的，可能是大家觉得简单都不用写了。

## 1024 节日快乐！