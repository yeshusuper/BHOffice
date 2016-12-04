using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Kuaidi100
{
    public class Kuaidi100HistoryProvider
    {
        /// <summary>
        /// 获取内地运单历史纪录
        /// </summary>
        /// <param name="com">快递公司名称</param>
        /// <param name="no">快递单号</param>
        /// <returns></returns>
        /// <exception cref="Exception">任何错误都会抛出异常</exception>
        public static Kuaidi100Content GetHistory(string com, string no)
        {
            var code = ExpressCodeConfig.GetCode(com);
            ExceptionHelper.ThrowIfNullOrEmpty(code, "code 为空");
            var request = HttpWebRequest.CreateHttp(String.Format("http://api.kuaidi100.com/api?id=43d747f46402d75b&com={0}&nu={1}&valicode=&show=0&muti=1&order=asc", 
                code, no));

            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var result = JsonConvert.DeserializeObject<Kuaidi100Result>(reader.ReadToEnd());
                        if (result == null || result.Status == Kuaidi100ResultStatus.Error)
                            throw new Exception(String.Format("获取失败:{0},{1}", com, no));
                        return result;
                    }
                }
            }
        }
    }
}
