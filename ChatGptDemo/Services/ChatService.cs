using Azure;
using Azure.AI.OpenAI;
using System.Text;

namespace ChatGptDemo.Services
{
    public class ChatService : IChatService
    {
        private readonly OpenAIClient _client;
        private readonly IConfiguration _configuration;
        public ChatService(IConfiguration configuration)
        {
            _configuration = configuration;

            string endpoint = _configuration["OPENAI:BASE_URL"];
            string key = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
            _client = new(new Uri(endpoint), new AzureKeyCredential(key));
        }

        public async Task<Response<ChatCompletions>> SendMessage(string message, string[] history)
        {          
            var chatCompletionsOptions = new ChatCompletionsOptions();
            chatCompletionsOptions.Temperature = (float)0.1;
            chatCompletionsOptions.MaxTokens = 1000;
            // Append conversation history for continuous chat dialog
            for (int i = 0; i < history.Length; i++)
            {
                if (i % 2 == 0)
                {
                    chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.User, history[i]));
                }
                else
                { 
                    chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.Assistant, history[i]));
                }
            }
            // Append new user input (question)
            chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.User, message));
            Response<ChatCompletions> responseWithoutStream = await _client.GetChatCompletionsAsync(_configuration["OPENAI:DEPLOYMENT_ID"], chatCompletionsOptions);
            return responseWithoutStream;
        }
    }
}
