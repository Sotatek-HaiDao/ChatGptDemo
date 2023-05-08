using Azure.AI.OpenAI;
using Azure;

namespace ChatGptDemo.Services
{
    public interface IChatService
    {
        Task<Response<ChatCompletions>> SendMessage(string payload, string[] chatHistory);
    }
}
