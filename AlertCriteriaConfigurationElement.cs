namespace PulseBot
{
	using System.Configuration;

	public class AlertCriteriaConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the name for the alert criteria
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

		/// <summary>
		/// Gets or sets the configuration information about the host to monitor
		/// </summary>
		[ConfigurationProperty("host", IsRequired = true)]
		public HostConfigurationElement Host
		{
			get { return (HostConfigurationElement)this["host"]; }
			set { this["host"] = value; }
		}

		/// <summary>
		/// Gets or sets the configuration information for HipChat
		/// </summary>
		[ConfigurationProperty("hipChat", IsRequired = false)]
		public HipChatConfigurationElement HipChat
		{
			get { return (HipChatConfigurationElement)this["hipChat"]; }
			set { this["hipChat"] = value; }
		}

        /// <summary>
        /// Gets or sets the configuration information for PagerDuty
        /// </summary>
        [ConfigurationProperty("pagerDuty", IsRequired = true)]
        public PagerDutyConfigurationElement PagerDuty
        {
            get { return (PagerDutyConfigurationElement)this["pagerDuty"]; }
            set { this["pagerDuty"] = value; }
        }
    }
}
