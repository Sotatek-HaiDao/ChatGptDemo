namespace ChatGptDemo.Models
{
    public class ReceiveGPTData
    {
        public Role Role { get; set; }
        public string Content { get; set; }

        public List<string>? FollowingUpQuestions { get; set; }

    }
    public class Role
    {
        public string Label { get; set; }
    }
}
