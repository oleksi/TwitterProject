using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using TweetSharp;
using TwitterProjectData;
using TwitterProjectModel;
using WorkerRole1.Properties;

namespace WorkerRole1
{
	public class WorkerRole : RoleEntryPoint
	{
		private int m_MinutesToWaitMin = 0;
		private int m_MinutesToWaitMax = 0;
		private int m_MaxFolloewersCount = 0;
		private double m_FollowersFriendsProportion = 0.0;
		FriendFinderRepository m_FriendFinderRepository = null;
		private TwitterService m_TwitterService = null;

		public override void Run()
		{
			// This is a sample worker implementation. Replace with your logic.
			Trace.WriteLine("WorkerRole1 entry point called", "Information");

			while (true)
			{
				MonitorUser nextMontiroUser = m_FriendFinderRepository.GetNextMonitorUser();
				if (nextMontiroUser != null)
				{
					//searchin for prospects for the current monitor user
					SearchOptions so = new SearchOptions() { Q = String.Format("@{0}", nextMontiroUser.UserName), Count = 100, Resulttype = TwitterSearchResultType.Recent };
					TwitterSearchResult sr = m_TwitterService.Search(so);

					//retrieving a list of friend prospects
					List<TwitterStatus> twitterStatuses = sr.Statuses.Where(st => st.Text.Contains("RT") == false && 
																			st.User != null &&
																			st.User.ScreenName.ToLower() != nextMontiroUser.UserName.ToLower() &&
																			st.User.Language == "en" && st.User.FollowersCount < m_MaxFolloewersCount &&
																			(Convert.ToDouble(st.User.FollowersCount) / st.User.FriendsCount) <= m_FollowersFriendsProportion).ToList<TwitterStatus>();
					List<FriendProspect> friendProspects = new List<FriendProspect>();
					twitterStatuses.ForEach(ts => friendProspects.Add(new FriendProspect() { UserName = ts.User.ScreenName }));

					//adding friends prospects
					m_FriendFinderRepository.AddFriendProspects(friendProspects, nextMontiroUser);

					//updating monitor's user last update date
					nextMontiroUser.LastMonitorDate = DateTime.Now;
					m_FriendFinderRepository.SaveMonitorUser(nextMontiroUser);
				}

				int nextSleepValue = getNextSleepValue();
				Thread.Sleep(nextSleepValue);
				Trace.WriteLine("Working", "Information");
			}
		}

		public override bool OnStart()
		{
			m_MinutesToWaitMin = Convert.ToInt32(RoleEnvironment.GetConfigurationSettingValue("MinutesToWaitMin"));
			m_MinutesToWaitMax = Convert.ToInt32(RoleEnvironment.GetConfigurationSettingValue("MinutesToWaitMax"));
			m_MaxFolloewersCount = Convert.ToInt32(RoleEnvironment.GetConfigurationSettingValue("MaxFolloewersCount"));
			m_FollowersFriendsProportion = Convert.ToDouble(RoleEnvironment.GetConfigurationSettingValue("FollowersFriendsProportion")); 

			m_FriendFinderRepository = new FriendFinderRepository();

			m_TwitterService = new TwitterService(RoleEnvironment.GetConfigurationSettingValue("TwitterConsumerKey"), RoleEnvironment.GetConfigurationSettingValue("TwitterConsumerSecret"));
			m_TwitterService.AuthenticateWith(RoleEnvironment.GetConfigurationSettingValue("TwitterAccessToken"), RoleEnvironment.GetConfigurationSettingValue("TwitterAccessTokenSecret"));

			// Set the maximum number of concurrent connections 
			ServicePointManager.DefaultConnectionLimit = 12;

			// For information on handling configuration changes
			// see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

			return base.OnStart();
		}

		private int getNextSleepValue()
		{
			int nextSleepValue = 0;
			Random rnd = new Random(DateTime.Now.Millisecond);
			nextSleepValue = rnd.Next(m_MinutesToWaitMin, m_MinutesToWaitMax);
			return nextSleepValue * 1000 * 60; //number of minutes to wait
		}
	}
}
