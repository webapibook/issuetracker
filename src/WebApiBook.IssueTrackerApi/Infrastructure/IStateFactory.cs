using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiBook.IssueTrackerApi.Infrastructure
{
    public interface IStateFactory<TModel, TState>
    {
        TState Create(TModel model);
    }
}