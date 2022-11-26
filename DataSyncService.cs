using System.Text;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace PlantApp
{
    public class DataSyncService
    {
        private readonly string C_URL = "https://github.com/Keev-in/PlantApp/blob/main/Resources/Raw/Configurations/config.json";
        private string _repositoryContent = null;
        private string _localContent = null;
        private string _configFile = "config.json";
        private bool _needUpdate;

        public bool NeedUpdate { get => _needUpdate; set => _needUpdate = value; }

        public DataSyncService(bool synchronize)
        {
            if (synchronize)
                DataSynchronization();
        }

        public void DataSynchronization()
        {
            HttpClient httpClient = new HttpClient();
            GetContentFromRepository(httpClient);
            GetLocalContent();
            if (IsSomethingNewToUpdate())
                UpdateLocalContent();
        }

        public void GetContentFromRepository(HttpClient client)
        {
            _repositoryContent = client.GetStringAsync(C_URL).Result;
        }

        public void GetLocalContent()
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("Configurations\\config.json").Result;
            using var streamReader = new StreamReader(stream);

            _localContent = streamReader.ReadToEnd().Replace("\r\n", "");
        }

        public string RepositoryContentParsing(string content)
        {
            var tableContent = content.Substring(
                content.IndexOf("<table data-hpc class=\"highlight tab-size js-file-line-container js-code-nav-container js-tagsearch-file\" data-tab-size=\"8\" data-paste-markdown-skip data-tagsearch-lang=\"JSON\" data-tagsearch-path=\"Resources/Raw/Configurations/config.json\">") +
                "<table data-hpc class=\"highlight tab-size js-file-line-container js-code-nav-container js-tagsearch-file\" data-tab-size=\"8\" data-paste-markdown-skip data-tagsearch-lang=\"JSON\" data-tagsearch-path=\"Resources/Raw/Configurations/config.json\">".Length, 
                content.IndexOf("</table>") - 
                content.IndexOf("<table data-hpc class=\"highlight tab-size js-file-line-container js-code-nav-container js-tagsearch-file\" data-tab-size=\"8\" data-paste-markdown-skip data-tagsearch-lang=\"JSON\" data-tagsearch-path=\"Resources/Raw/Configurations/config.json\">"));
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
            var parsedContent = RepositoryContentParsing(_repositoryContent);

            var repositoryContent = JsonConvert.DeserializeObject<Plant>(parsedContent);
            var localContent = JsonConvert.DeserializeObject<Plant>(_localContent);

            // Maybe something like contain to prevent situacion when user add to local content plant and want to keep it
            if (repositoryContent.Equals(localContent))
                return true;

            return false;
        }

        private void UpdateLocalContent()
        {

        }
    }
}
