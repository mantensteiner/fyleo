using System.Collections.Generic;
using System.Threading.Tasks;

namespace fyleo.EventLog
{
    public interface IEventLog
    {
        Task Write(string user, string action, string message);
        Task<Activity[]> GetLatest();
    }
}