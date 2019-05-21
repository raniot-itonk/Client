using System;

namespace Client.Models
{
    public class HistoryResponse
    {
        public string Event { get; set; }
        public string EventMessage { get; set; }
        public DateTime Timestamp { get; set; }
    }
}