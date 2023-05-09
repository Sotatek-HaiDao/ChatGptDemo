using Azure.AI.OpenAI;
using Azure;
using ChatGptDemo.Models;

namespace ChatGptDemo.Services
{
    public interface IChatService
    {
        Task<ReceiveGPTData> SendMessage(string payload, string[] chatHistory);
    }
}
