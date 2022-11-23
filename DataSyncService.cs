using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;

using Newtonsoft.Json;

namespace PlantApp
{
    public class DataSyncService : IDataSyncService
    {
        private readonly string C_URL = "https://github.com/Keev-in/PlantApp/blob/main/config.json";
        private readonly HttpClient _httpClient;
        private string _repositoryContent = null;
        private string _localContent = null;
        private string _configFile = "config.json";
        private bool _needUpdate;

        public bool NeedUpdate { get => _needUpdate; set => _needUpdate = value; }

        public DataSyncService()
        {
            _httpClient = new HttpClient();
            GetContentFromRepository();
            GetLocalContent();
            _needUpdate = IsSomethingNewToUpdate();
        }

        public void GetContentFromRepository()
        {
            _repositoryContent = _httpClient.GetStringAsync(C_URL).Result;
        }

        public void GetLocalContent()
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("Configurations\\config.json").Result;
            using var reader = new StreamReader(stream);

            var content = reader.ReadToEnd();
            _localContent = JsonConvert.SerializeObject(content);
            var temp = JsonConvert.DeserializeObject<Plant>(_localContent);
        }

        public string ContentParsing(string content)
        {
            var tableContent = content.Substring(content.IndexOf("\"config.json\">") + 23, content.IndexOf("</table>") - content.IndexOf("\"config.json\">"));
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(tableContent);

            StringBuilder sb = new StringBuilder();
            foreach (var node in doc.DocumentNode.SelectNodes("/tr"))
            {
                sb.Append(node.InnerText.Trim().Replace("&quot;", "\""));
            }
            
            return sb.ToString();
        }

        public bool IsSomethingNewToUpdate()
        {
            var repositoryContent = ContentParsing(_repositoryContent);
            var temp = JsonConvert.DeserializeObject<Plant>(repositoryContent);
            var localContent = JsonConvert.DeserializeObject<Plant>(_localContent);

            if (repositoryContent.Equals(_localContent))
                return true;

            return false;
        }

        public void UpdateLocalContent()
        {

        }
    }
}
