using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using ChatGptDemo.Commons.Helpers;
using ChatGptDemo.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ChatGptDemo.Services
{
    public class SearchService : ISeachService
    {
        private static SearchClient _searchClient;
        private static SearchIndexClient _indexClient;
        private readonly IConfiguration _configuration;
        public SearchService(IConfiguration configuration)
        {
            _configuration = configuration;
            string searchServiceUri = _configuration["SEARCH_SERVICE:API_URL"];
            string queryApiKey = _configuration["SEARCH_SERVICE:API_KEY"];

            // Create a service and index client.
            _indexClient = new SearchIndexClient(new Uri(searchServiceUri), new AzureKeyCredential(queryApiKey));
            _searchClient = _indexClient.GetSearchClient("azureblob-index");
            

        }
        public async Task<string> Search(string searchText)
        {
            var options = new SearchOptions()
            {
                QueryType = Azure.Search.Documents.Models.SearchQueryType.Full,
                SearchMode = Azure.Search.Documents.Models.SearchMode.Any
            };
            try
            {
                string pattern = @"[+\-&|!(){}\[\]^""~*?:\\/]"; // Regular expression pattern to match special characters
                string outputString = Regex.Replace(searchText, pattern, "");
                var res = await _searchClient.SearchAsync<object>(searchText: outputString, options).ConfigureAwait(false);
                var receive = res.GetRawResponse().Content.ToString();
                JsonDocument jsonDoc = JsonDocument.Parse(receive);

                // Get the root element of the JsonDocument
                JsonElement root = jsonDoc.RootElement;

                // Navigate to the "value" array
                JsonElement valueArray = root.GetProperty("value");
                string combinedContent = "";
                // Get the first element of the "value" array
                foreach (JsonElement element in valueArray.EnumerateArray())
                {
                    string content = element.GetProperty("content").GetString() ?? "";
                    combinedContent += Utils.Nonewlines(content);
                }
                return combinedContent;
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
