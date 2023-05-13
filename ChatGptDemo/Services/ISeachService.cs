using ChatGptDemo.Models;

namespace ChatGptDemo.Services
{
    public interface ISeachService
    {
        Task<string> Search(string searchText);
    }
}
