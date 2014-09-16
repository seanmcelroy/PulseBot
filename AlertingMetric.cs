namespace PulseBot
{
	using System;
	
	public class AlertingMetric
    {
        public string IPAddress { get; set; }
        public int? PingInterval { get; set; }
        public int? PingTimeout { get; set; }
		public int? PingHipChatFailCount { get; set; }
		public int? PingPagerDutyFailCount { get; set; }
        public string PagerDutyServiceAPIKey { get; set; }
        public string PagerDutyMessage { get; set; }
        public DateTime? LastPagerDutyAlert { get; set; }
	    public string HipChatApiKey { get; set; }
	    public string HipChatRoomName { get; set; }
    }
}
