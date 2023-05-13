using Azure;
using Azure.AI.OpenAI;
using ChatGptDemo.Models;
using System.Text;

namespace ChatGptDemo.Services
{
    public class ChatService : IChatService
    {
        string template = @"You are an intelligent assistant helping Contoso Inc employees with their healthcare plan questions and employee handbook questions. 
Use 'you' to refer to the individual asking the questions even if they ask with 'I'. 
Answer the following question using only the data provided in the sources below. 
For tabular information return it as an html table. Do not return markdown format. 
Each source has a name followed by colon and the actual information, always include the source name for each fact you use in the response and put it in format '[source name that you use]'. 
If you cannot answer using the sources below, say you don't know. 

###

Question: '{q}'?

Sources:
{s}

Answer:
";
        private readonly OpenAIClient _client;
        private readonly IConfiguration _configuration;
        private readonly ISeachService _seachService;
        private readonly string followingUpQuestion = "Generate three very brief follow-up questions that the user would likely ask next.Separate them with '|'";
        public ChatService(IConfiguration configuration, ISeachService seachService)
        {
            _configuration = configuration;

            string endpoint = _configuration["OPENAI:BASE_URL"];
            string key = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
            _client = new(new Uri(endpoint), new AzureKeyCredential(key));
            _seachService = seachService;
        }

        public async Task<ReceiveGPTData> SendMessage(string message, string[] history)
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
            var source = await _seachService.Search(message);
            string modifiedTemplate = template.Replace("{q}", message).Replace("{s}", source);
            chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.User, modifiedTemplate));
            Response<ChatCompletions> responseWithoutStream = await _client.GetChatCompletionsAsync(_configuration["OPENAI:DEPLOYMENT_ID"], chatCompletionsOptions);
            var resModel = responseWithoutStream.Value.Choices[0].Message;
            ReceiveGPTData res = new ReceiveGPTData();
            Role role = new Role();
            role.Label = resModel.Role.Label;
            res.Role = role;
            res.Content = resModel.Content;
            res.FollowingUpQuestions = await GetUpCommingQuestion(chatCompletionsOptions.Messages);
            return res;
        }

        private async Task<List<string>> GetUpCommingQuestion(IList<ChatMessage> chatHistory)
        {
            try
            {

                chatHistory.Add(new ChatMessage(ChatRole.System, followingUpQuestion));
                var chatCompletionsOptions = new ChatCompletionsOptions();
                chatCompletionsOptions.Temperature = (float)0.1;
                chatCompletionsOptions.MaxTokens = 1000;
                foreach (ChatMessage chatMessage in chatHistory)
                {
                    chatCompletionsOptions.Messages.Add(chatMessage);
                }
                Response<ChatCompletions> responseWithoutStream = await _client.GetChatCompletionsAsync(_configuration["OPENAI:DEPLOYMENT_ID"], chatCompletionsOptions);
                return responseWithoutStream.Value.Choices[0].Message.Content.Split("|").ToList();
            }
            catch (Exception ex) 
            {
                return null;
            }
        }
    }
}
