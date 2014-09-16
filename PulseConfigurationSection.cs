using System.Configuration;

namespace PulseBot
{
    public class PulseConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// Configuration information for alerting criteria
        /// </summary>
        [ConfigurationProperty("alerts", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof (AlertCriteriaCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public AlertCriteriaCollection Alerts
        {
            get
            {
                return (AlertCriteriaCollection)base["alerts"];
            }
        }
    }
}
