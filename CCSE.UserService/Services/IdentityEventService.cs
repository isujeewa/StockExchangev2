using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace UserService.API.Services
{
    /// <summary>
    /// Extension for IEventService in IdentityServer
    /// </summary>
    public class IdentityEventService : IEventService
    {
        private readonly ILogger<IdentityEventService> _log;
        public IdentityEventService(ILogger<IdentityEventService> log)
        {
            _log = log;
        }
        public bool CanRaiseEventType(EventTypes evtType)
        {
            throw new System.NotImplementedException();
        }

        public Task RaiseAsync(Event evt)
        {
            if (evt.EventType == EventTypes.Success ||
            evt.EventType == EventTypes.Information)
            {
                _log.LogInformation("{Name} ({Id}), Details: {@details}",
                    evt.Name,
                    evt.Id,
                    evt);
            }
            else
            {
                _log.LogError("{Name} ({Id}), Details: {@details}",
                    evt.Name,
                    evt.Id,
                    evt);
            }

            return Task.CompletedTask;
        }
    }
}
