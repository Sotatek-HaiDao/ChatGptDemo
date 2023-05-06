using OpenAI_API.Chat;

namespace ChatGptDemo.Services
{
    public interface IChatService
    {
        Task<ChatResult> SendMessage(string payload);
    }
}
