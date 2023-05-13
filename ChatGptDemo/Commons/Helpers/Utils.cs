namespace ChatGptDemo.Commons.Helpers
{
    public static class Utils
    {
        public static string Nonewlines(string s)
        {
            return s.Replace("\n", " ").Replace("\r", " ");
        }
    }
}
