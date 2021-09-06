using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HandyControl.Controls;
using Newtonsoft.Json;

namespace TSManager.Modules
{
    /// <summary>
    /// 主管理部分
    /// </summary>
    internal sealed partial class DownloadManager
    {
        public static List<Data.DownloadInfo.TShockInfo> TSInfo => TSMMain.GUI.Download_TShock_List.ItemsSource as List<Data.DownloadInfo.TShockInfo>;
        public static bool IsRefreshing { get; internal set; } = false;
        public static void Init()
        {

        }
        public static void RefreshAsync()
        {
            var d = Dialog.Show<LoadingLine>();
            IsRefreshing = true;
            TSMMain.GUIInvoke(async () => TSMMain.GUI.Download_TShock_List.ItemsSource = await GetTShockReleaseInfoAsync());
            IsRefreshing = false;
            d.Close();
        }
        public static void SelectTSFile(Data.DownloadInfo.TShockInfo info)
        {
            TSMMain.GUI.Download_TShock_Drawer.IsOpen = true;
        }
    }
    /// <summary>
    /// ts下载信息获取
    /// </summary>
    internal sealed partial class DownloadManager
    {
        public static readonly string TSReleaseAddress = "http://api.suki.club/v1/tsmanager/info/tshock-release";
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
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/json"));
                        client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");//Set the User Agent to "request"
                        using (HttpResponseMessage response = client.GetAsync(TSReleaseAddress).Result)
                        {
                            response.EnsureSuccessStatusCode();
                            var result = await response.Content.ReadAsStringAsync();
                            if (Utils.TryParseJson(result, out var jobj))
                                list = jobj["Data"].ToObject<List<Data.DownloadInfo.TShockInfo>>();
                            /*var json = JsonConvert.DeserializeObject<JArray>(result);
                            json.Where(j => j["assets"]
                            .Any(a => a["content_type"]?.Value<string>() == "application/zip"))
                            .ForEach(j => list.Add(new(j)));*/
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
    internal sealed partial class DownloadManager
    {
        public class MultiDownload
        {
            #region 变量
            private long _fileSize;    //文件大小
            private string _fileUrl;   //文件地址
            private short _threadCompleteNum; //线程完成数量
            private volatile int _downloadSize; //当前下载大小(实时的)
            private Thread[] _thread;   //线程数组
            private List<string> _tempFiles = new();
            private readonly object locker = new();
            #endregion
            #region 属性
            /// <summary>
            /// 文件名
            /// </summary>
            public string FileName { get; set; }
            /// <summary>
            /// 文件大小
            /// </summary>
            public long FileSize => _fileSize;
            /// <summary>
            /// 当前下载大小(实时的)
            /// </summary>
            public int DownloadSize => _downloadSize;
            /// <summary>
            /// 是否完成
            /// </summary>
            public bool IsComplete { get; private set; }
            /// <summary>
            /// 线程数量
            /// </summary>
            public int ThreadNum { get; }
            /// <summary>
            /// 保存路径
            /// </summary>
            public string SavePath { get; set; }
            #endregion
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="threahNum">线程数量</param>
            /// <param name="fileUrl">文件Url路径</param>
            /// <param name="savePath">本地保存路径</param>
            public MultiDownload(int threahNum, string fileUrl, string savePath)
            {
                this.ThreadNum = threahNum;
                this._thread = new Thread[threahNum];
                this._fileUrl = fileUrl;
                this.SavePath = savePath;
            }
            public async Task Start()
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_fileUrl);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                _fileSize = response.ContentLength;
                int singelNum = (int)(_fileSize / ThreadNum);  //平均分配
                int remainder = (int)(_fileSize % ThreadNum);  //获取剩余的
                request.Abort();
                response.Close();
                for (int i = 0; i < ThreadNum; i++)
                {
                    List<int> range = new List<int>();
                    range.Add(i * singelNum);
                    if (remainder != 0 && (ThreadNum - 1) == i) //剩余的交给最后一个线程
                        range.Add(i * singelNum + singelNum + remainder - 1);
                    else
                        range.Add(i * singelNum + singelNum - 1);
                    //下载指定位置的数据
                    int[] ran = new int[] { range[0], range[1] };
                    _thread[i] = new(new ParameterizedThreadStart(Download));
                    _thread[i].Name = Path.GetFileNameWithoutExtension(_fileUrl) + "_{0}".Replace("{0}", Convert.ToString(i + 1));
                    _thread[i].Start(ran);
                }
                while (!IsComplete)
                    await Task.Delay(1);
            }
            private void Download(object obj)
            {
                Stream httpFileStream = null, localFileStram = null;
                try
                {
                    int[] ran = obj as int[];
                    string tmpFileBlock = Path.GetTempPath() + Thread.CurrentThread.Name + ".tmp";
                    _tempFiles.Add(tmpFileBlock);
                    HttpWebRequest httprequest = (HttpWebRequest)WebRequest.Create(_fileUrl);
                    httprequest.AddRange(ran[0], ran[1]);
                    HttpWebResponse httpresponse = (HttpWebResponse)httprequest.GetResponse();
                    httpFileStream = httpresponse.GetResponseStream();
                    localFileStram = new FileStream(tmpFileBlock, FileMode.Create);
                    byte[] by = new byte[5000];
                    int getByteSize = httpFileStream.Read(by, 0, (int)by.Length); //Read方法将返回读入by变量中的总字节数
                    while (getByteSize > 0)
                    {
                        Thread.Sleep(20);
                        lock (locker) _downloadSize += getByteSize;
                        localFileStram.Write(by, 0, getByteSize);
                        getByteSize = httpFileStream.Read(by, 0, (int)by.Length);
                    }
                    lock (locker) _threadCompleteNum++;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message.ToString());
                }
                finally
                {
                    if (httpFileStream != null)
                        httpFileStream.Dispose();
                    if (localFileStram != null)
                        localFileStram.Dispose();
                }
                if (_threadCompleteNum == ThreadNum)
                {
                    Complete();
                    IsComplete = true;
                }
            }
            /// <summary>
            /// 下载完成后合并文件块
            /// </summary>
            private void Complete()
            {
                Stream mergeFile = new FileStream(SavePath, FileMode.Create);
                BinaryWriter AddWriter = new(mergeFile);
                foreach (string file in _tempFiles)
                {
                    using (FileStream fs = new(file, FileMode.Open))
                    {
                        BinaryReader TempReader = new(fs);
                        AddWriter.Write(TempReader.ReadBytes((int)fs.Length));
                        TempReader.Close();
                    }
                    File.Delete(file);
                }
                AddWriter.Close();
            }
        }
    }
}
