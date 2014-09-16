// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostConfigurationElement.cs" company="Sean McElroy">
//   Released into the Public Domain by Sean McElroy, 2014
// </copyright>
// <summary>
//   A configuration element that indicates a host should be monitored
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PulseBot
{
	using System.Configuration;

	/// <summary>
	/// A configuration element that indicates a host should be monitored
	/// </summary>
    public class HostConfigurationElement : ConfigurationElement
    {
		/// <summary>
		/// Gets or sets the IP address of the host to monitor
		/// </summary>
        [ConfigurationProperty("ipAddress", IsRequired = true)]
        public string IPAddress
        {
			get { return (string)this["ipAddress"]; }
			set { this["ipAddress"] = value; }
        }

		/// <summary>
		/// Gets or sets the interval for how long to wait between ping checks of the host
		/// </summary>
		[ConfigurationProperty("pingInterval", IsRequired = false)]
		public int? PingInterval
		{
			get { return (int?)this["pingInterval"]; }
			set { this["pingInterval"] = value; }
		}

		/// <summary>
		/// Gets or sets the amount of time to wait until the failure to receive a ping response is determined to be a failure
		/// </summary>
		[ConfigurationProperty("pingTimeout", IsRequired = false)]
        public int? PingTimeout
        {
			get { return (int?)this["pingTimeout"]; }
			set { this["pingTimeout"] = value; }
        }

		[ConfigurationProperty("hipChatFailCount", IsRequired = false)]
		public int? PingHipChatFailCount
		{
			get { return (int?)this["hipChatFailCount"]; }
			set { this["hipChatFailCount"] = value; }
		}

		[ConfigurationProperty("pagerDutyFailCount", IsRequired = false)]
		public int? PingPagerDutyFailCount
		{
			get { return (int?)this["pagerDutyFailCount"]; }
			set { this["pagerDutyFailCount"] = value; }
		}
    }
}
