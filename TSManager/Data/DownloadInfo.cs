using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSManager.Data
{
    public class DownloadInfo
    {
        public struct TShockInfo
        {
            public TShockInfo(JToken json)
            {
                Version = json["tag_name"].Value<string>();//Version.TryParse(json["tag_name"].Value<string>().Replace("v", ""), out var v) ? v : new();
                CreateDate = DateTime.TryParse(json["created_at"].Value<string>(), out var d) ? d : new();
                Name = json["name"].Value<string>();
                Tag = json["tag_name"].Value<string>();
                TargetCommitish = json["target_commitish"].Value<string>();
                Author = json["author"]["login"].Value<string>();
                AuthorAvatar = json["author"]["avatar_url"].Value<string>();
                Description = json["body"].Value<string>();
                ReleaseURL = json["html_url"].Value<string>();
                var assets = json["assets"].FirstOrDefault(j => j["content_type"].Value<string>() == "application/zip");
                DownloadURL = assets["browser_download_url"].Value<string>().Replace("https://github.com/", "https://download.fastgit.org/");
                DownloadCount = assets["download_count"].Value<int>();
                Size = assets["size"].Value<long>();
            }
            public string Version { get; set; }
            public DateTime CreateDate { get; set; }
            public string Name { get; set; }
            public string Tag { get; set; }
            public string TargetCommitish { get; set; }
            public string Author { get; set; }
            public string AuthorAvatar { get; set; }
            public string Description { get; set; }
            public string ReleaseURL { get; set; }
            public string DownloadURL { get; set; }
            public int DownloadCount { get; set; }
            public long Size { get; set; }
        }
    }
}
