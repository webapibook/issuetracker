namespace WebApiBook.IssueTrackerApi.Infrastructure
{
    public interface IStateFactory<TModel, TState>
    {
        TState Create(TModel model);
    }
}