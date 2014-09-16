namespace PulseBot
{
	using System;
	using System.Configuration;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Net.NetworkInformation;
	using System.Text;
	using System.Threading.Tasks;
	using HipChat;
	using Newtonsoft.Json;

	public class Program
	{
		private const string PingData = "deadbeefdeadbeefdeadbeefdeadbeef";

		public static void Main()
		{
			var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			var pulseBotConfigurationSection = (PulseConfigurationSection)config.GetSection("pulseBot");

			var alertingMetrics = pulseBotConfigurationSection.Alerts
			.Select(x => new AlertingMetric
				{
					IPAddress = x.Host.IPAddress,
					PingInterval = x.Host.PingInterval,
					PingTimeout = x.Host.PingTimeout,
					PingHipChatFailCount = x.Host.PingHipChatFailCount,
					PingPagerDutyFailCount = x.Host.PingPagerDutyFailCount,
					HipChatApiKey = x.HipChat.HipChatApiKey,
					HipChatRoomName = x.HipChat.HipChatRoomName,
					LastPagerDutyAlert = null,
					PagerDutyServiceAPIKey = x.PagerDuty.GenericServiceApiKey,
					PagerDutyMessage = x.PagerDuty.MessageOnAlert
				})
				.ToList();

		foreach (var hipChatSink in alertingMetrics.Select(x => new { x.HipChatApiKey, x.HipChatRoomName }).Distinct())
			HipChatClient.SendMessage(hipChatSink.HipChatApiKey, hipChatSink.HipChatRoomName, "PulseBot", "PulseBot has started", true, HipChatClient.BackgroundColor.purple);

			Parallel.ForEach(alertingMetrics, alertingMetric =>
			{
				var consecutiveFailCount = 0;

				while (true)
				{
					try
					{
						if (alertingMetric.PingInterval.HasValue)
						{
							using (var pingSender = new Ping())
							{
								var options = new PingOptions
								{
									DontFragment = true
								};

								// Create a buffer of 32 bytes of data to be transmitted. 
								var buffer = Encoding.ASCII.GetBytes(PingData);
								try
								{
									var reply = pingSender.Send(IPAddress.Parse(alertingMetric.IPAddress), alertingMetric.PingTimeout ?? 10000, buffer, options);
									if (reply != null && reply.Status != IPStatus.Success)
									{
										Console.WriteLine("\r\n{0:yyyy-MM-dd-HH:mm:ss} - {1}: PING FAILURE", DateTime.Now, alertingMetric.PagerDutyMessage);
										consecutiveFailCount++;

										if (alertingMetric.PingHipChatFailCount.HasValue &&
										    consecutiveFailCount == alertingMetric.PingHipChatFailCount.Value)
											HipChatClient.SendMessage(alertingMetric.HipChatApiKey, alertingMetric.HipChatRoomName, "PulseBot", alertingMetric.PagerDutyMessage, true, HipChatClient.BackgroundColor.red);

										if (alertingMetric.PingPagerDutyFailCount.HasValue &&
										    (consecutiveFailCount == alertingMetric.PingPagerDutyFailCount.Value || (alertingMetric.LastPagerDutyAlert.HasValue && (DateTime.Now - alertingMetric.LastPagerDutyAlert.Value).TotalMinutes > 60)))
										{
											Console.WriteLine("\r\nAlerting PagerDuty for {0} after {1} failures!", alertingMetric.PagerDutyMessage,
											                  consecutiveFailCount);
											//PostPagerDutyAlert(alertingMetric.PagerDutyServiceAPIKey, alertingMetric.PagerDutyMessage, consecutiveFailCount);
											alertingMetric.LastPagerDutyAlert = DateTime.Now;
										}
									}
									else if (reply != null && reply.Status == IPStatus.Success)
									{
										consecutiveFailCount = 0;
										Console.Write(".");
									}
								}
								catch (PingException)
								{
									Console.WriteLine("\r\n{0:yyyy-MM-dd-HH:mm:ss} - {1}: PING FAILURE", DateTime.Now, alertingMetric.PagerDutyMessage);
									consecutiveFailCount++;
								}
							}
						}

						System.Threading.Thread.Sleep(alertingMetric.PingInterval.Value * 1000);
					}
					catch (Exception ex)
					{
						Console.WriteLine("\r\n{0}", ex.ToString());
					}
				}
			});
		}

		private static void PostPagerDutyAlert(string pagerDutyServiceKey, string description, float cur)
		{
			Console.WriteLine("[{0} {1}] Posting alert to PagerDuty for {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), description);

			var json = new
			{
				service_key = pagerDutyServiceKey,
				event_type = "trigger",
				description,
				details = new
				{
					current_average = cur
				}
			};

			var http = (HttpWebRequest)WebRequest.Create(new Uri("https://events.pagerduty.com/generic/2010-04-15/create_event.json"));
			http.Accept = "application/json";
			http.ContentType = "application/json";
			http.Method = "POST";

			var parsedContent = JsonConvert.SerializeObject(json);
			var encoding = new ASCIIEncoding();
			var bytes = encoding.GetBytes(parsedContent);

			var newStream = http.GetRequestStream();
			newStream.Write(bytes, 0, bytes.Length);
			newStream.Close();

			var response = http.GetResponse();
			using (var stream = response.GetResponseStream())
			{
				if (stream != null)
				{
					using (var sr = new StreamReader(stream))
					{
						var content = sr.ReadToEnd();
						Console.WriteLine("Response from PagerDuty: {0}", content);
					}
				}
			}
		}
	}
}
