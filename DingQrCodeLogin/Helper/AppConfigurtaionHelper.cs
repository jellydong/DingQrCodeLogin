using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace DingQrCodeLogin.Helper
{
    /// <summary>
    /// 读取配置文件
    /// </summary>
    public class AppConfigurtaionHelper
    {
        public static IConfiguration Configuration { get; set; }
        static AppConfigurtaionHelper()
        {
            //ReloadOnChange = true 当appsettings.json被修改时重新加载            
            Configuration = new ConfigurationBuilder()
                .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
                .Build();
        }
    }
}
