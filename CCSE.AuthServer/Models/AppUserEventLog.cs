using System;

namespace AuthServer.Models
{
    /// <summary>
    /// Event log for AppUser
    /// </summary>
    public class AppUserEventLog
    {
        public Guid Id { get; set; }
        public string EventType { get; set; }
        public string EventBody { get; set; }
        public State State { get; set; }
    }

    /// <summary>
    /// Defined states
    /// </summary>
    public enum State
    {
        InProgress = 0,
        Completed = 1
    }
}
