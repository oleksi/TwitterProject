using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetSharp;
using TwitterProjectData;
using TwitterProjectModel;

namespace TwitterProjectBL.Tasks
{
	public class FriendFinderTask : BaseTask
	{
		private FriendFinderRepository m_DataRepository = null;
		private int m_MinutesToWaitMin = 0;
		private int m_MinutesToWaitMax = 0;
		private int m_MaxFolloewersCount = 0;
		private double m_FollowersFriendsProportion = 0.0;

		public FriendFinderTask(FriendFinderRepository dataRepository, TwitterService twitterService, string noShowStartTime, string noShowEndTime, int minutesToWaitMin, int minutesToWaitMax, int maxFolloewersCount, double followersFriendsProportion) : base(twitterService, null)
		{
			m_DataRepository = dataRepository;
			m_MinutesToWaitMin = minutesToWaitMin;
			m_MinutesToWaitMax = minutesToWaitMax;
			m_MaxFolloewersCount = maxFolloewersCount;
			m_FollowersFriendsProportion = followersFriendsProportion;

			SetNextRunningDate();
		}

		public override void Run()
		{
			MonitorUser nextMontiroUser = m_DataRepository.GetNextMonitorUser();
			if (nextMontiroUser != null)
			{
				//searchin for prospects for the current monitor user
				SearchOptions so = new SearchOptions() { Q = String.Format("@{0}", nextMontiroUser.UserName), Count = 100, Resulttype = TwitterSearchResultType.Recent };
				TwitterSearchResult sr = m_TwitterService.Search(so);

				TwitterError error = m_TwitterService.Response.Error;
				if (error != null)
					throw new TwitterProjectException(-1, error);

				//retrieving a list of friend prospects
				List<TwitterStatus> twitterStatuses = sr.Statuses.Where(st => st.Text.Contains("RT") == false &&
																		st.User != null &&
																		st.User.ScreenName.ToLower() != nextMontiroUser.UserName.ToLower() &&
																		st.User.Language == "en" && st.User.FollowersCount < m_MaxFolloewersCount &&
																		(Convert.ToDouble(st.User.FollowersCount) / st.User.FriendsCount) <= m_FollowersFriendsProportion).ToList<TwitterStatus>();
				List<FriendProspect> friendProspects = new List<FriendProspect>();
				twitterStatuses.ForEach(ts => friendProspects.Add(new FriendProspect() { UserName = ts.User.ScreenName }));

				//adding friends prospects
				m_DataRepository.AddFriendProspects(friendProspects, nextMontiroUser);

				//updating monitor's user last update date
				nextMontiroUser.LastMonitorDate = DateTime.Now;
				m_DataRepository.SaveMonitorUser(nextMontiroUser);
			}
		}

		public override void SetNextRunningDate()
		{
			int minutesInterval = 0;
			Random rnd = new Random(DateTime.Now.Millisecond);
			minutesInterval = rnd.Next(m_MinutesToWaitMin, m_MinutesToWaitMax);
			m_NextRunningDate = DateTime.Now.AddMinutes(minutesInterval);
		}
	}
}
