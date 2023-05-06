using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System.Text;

namespace ChatGptDemo.Services
{
    public class ChatService : IChatService
    {
        private readonly OpenAIAPI _api;
        private readonly IConfiguration _configuration;
        public ChatService(IConfiguration configuration)
        {
            _configuration = configuration;
            // Put API Key here
            _api = OpenAIAPI.ForAzure(_configuration["OPENAI:RESOURCE_NAME"], _configuration["OPENAI:DEPLOYMENT_ID"], Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY",EnvironmentVariableTarget.Machine));
            _api.ApiVersion = _configuration["OPENAI:API_VERSION"];

        }
        public async Task<ChatResult> SendMessage(string payload)
        {
            try
            {             
                var messages = Enumerable.Empty<ChatMessage>();             
                // Append new user input (question)
                messages = messages.Append(new ChatMessage(ChatMessageRole.User, payload));
                var result = await _api.Chat.CreateChatCompletionAsync(new ChatRequest()
                {
                    Model = Model.ChatGPTTurbo,
                    Temperature = 0.1,
                    MaxTokens = 1000,
                    Messages = messages.ToList()
                });

                // Save usage in DB for reference
                //TODO
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
