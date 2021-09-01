using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TSManager.Modules
{
    /// <summary>
    /// 主管理部分
    /// </summary>
    internal sealed partial class Downloader
    {
        public static void Init()
        {

        }
        public static void Refresh()
        {

        }
        public static void SelectTSFile(Data.DownloadInfo.TShockInfo info)
        {
            TSMMain.GUI.Download_TShock_Drawer.IsOpen = true;
        }
    }
    /// <summary>
    /// ts下载信息获取
    /// </summary>
    internal sealed partial class Downloader
    {
        public static readonly string TSReleaseAddress = "https://api.github.com/repos/Pryaxis/TShock/releases";
        internal static readonly string GitHubToken = "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"; //这token没给任何权限
        public static async Task<List<Data.DownloadInfo.TShockInfo>> GetTShockReleaseInfoAsync()
        {
            return await Task.Run(async () =>
            {
                var list = new List<Data.DownloadInfo.TShockInfo>();
                using (HttpClient client = new())
                {
                    try
                    {
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");//Set the User Agent to "request"
                        using (HttpResponseMessage response = client.GetAsync(TSReleaseAddress).Result)
                        {
                            response.EnsureSuccessStatusCode();
                            response.Headers.Add("token", GitHubToken);
                            var result = await response.Content.ReadAsStringAsync();
                            var json = JsonConvert.DeserializeObject<JArray>(result);
                            json.Where(j => j["assets"]
                            .Any(a => a["content_type"]?.Value<string>() == "application/zip"))
                            .ForEach(j => list.Add(new(j)));
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.ShowError();
                    }
                }
                return list;
            });
        }
    }
    /// <summary>
    /// 下载相关
    /// </summary>
    internal sealed partial class Downloader
    {
    }
}
