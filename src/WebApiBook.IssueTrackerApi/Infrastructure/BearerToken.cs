namespace WebApiBook.IssueTrackerApi.Infrastructure
{
    public class BearerToken
    {
        public string Value { get; private set; }

        public BearerToken(string value)
        {
            Value = value;
        }
    }
}