using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ThinSdk.NET
{
    public class HttpHelper
    {
        public static async Task<string> RpcPost(string url, string method, params JValue[] _params)
        {
            var json = new JObject();
            json["id"] = 1;
            json["jsonrpc"] = "2.0";
            json["method"] = method;
            StringBuilder sb = new StringBuilder();
            var array = new JArray();
            for (var i = 0; i < _params.Length; i++)
            {

                array.Add(_params[i]);
            }
            json["params"] = array;
            var data = System.Text.Encoding.UTF8.GetBytes(json.ToString());

            return await HttpPost(url,data);
        }

        public static async Task<string> RpcGet(string url,string method,params JValue[] _params)
        {
            StringBuilder sb = new StringBuilder();
            if (url.Last() != '/')
                url = url + "/";

            sb.Append(url + "?jsonrpc=2.0&id=1&method=" + method + "&params=[");
            for (var i = 0; i < _params.Length; i++)
            {
                sb.Append(_params[i].ToString());
                if (i != _params.Length - 1)
                    sb.Append(",");
            }
            sb.Append("]");
            return await HttpGet(sb.ToString());
        }

        public static async Task<string> HttpGet(string url)
        {
            WebClient wc = new WebClient();
            return await wc.DownloadStringTaskAsync(url);
        }

        public static async Task<string> HttpPost(string url, byte[] data)
        {
            WebClient wc = new WebClient();
            wc.Headers["content-type"] = "text/plain;charset=UTF-8";
            byte[] retdata = await wc.UploadDataTaskAsync(url, "POST", data);
            return System.Text.Encoding.UTF8.GetString(retdata);
        }
    }
}
