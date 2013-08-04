using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.Models;

namespace WebApiBook.IssueTrackerApi.Controllers
{
    public class IssueController : ApiController
    {
        private readonly IIssueStore _store;
        private readonly IStateFactory<Issue, IssueState> _stateFactory;

        public IssueController(IIssueStore store, IStateFactory<Issue, IssueState> stateFactory )
        {
            _store = store;
            _stateFactory = stateFactory;
        }

        public async Task<IEnumerable<IssueState>> Get()
        {
            var result = await _store.FindAsync();
            return result.Select(i => _stateFactory.Create(i));
        } 

        public async Task<IssueState> Get(string id)
        {
            var result = await _store.FindAsync(id);
            return _stateFactory.Create(result);
        }
    }
}