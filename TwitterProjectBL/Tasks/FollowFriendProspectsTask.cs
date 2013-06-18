using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetSharp;
using TwitterProjectData;
using TwitterProjectModel;

namespace TwitterProjectBL.Tasks
{
	public class FollowFriendProspectsTask : ITask
	{
		private ModelRepository m_DataRepository = null;
		private TwitterService m_TwitterService = null;
		private Model m_Model = null;
		private int m_FollowIntervalMinMinutes = 0;
		private int m_FollowIntervalMaxMinutes = 0;
		private DateTime m_NextRunningDate = DateTime.MinValue;

		public FollowFriendProspectsTask(ModelRepository dataRepository, TwitterService twitterService, Model model, int followIntervalMinMinutes, int followIntervalMaxMinutes)
		{
			m_DataRepository = dataRepository;
			m_TwitterService = twitterService;
			m_Model = model;
			m_FollowIntervalMinMinutes = followIntervalMinMinutes;
			m_FollowIntervalMaxMinutes = followIntervalMaxMinutes;

			SetNextRunningDate();
		}

		public DateTime GetNextRunningDate()
		{
			return m_NextRunningDate;
		}

		public void SetNextRunningDate()
		{
			int minutesInterval = 0;
			Random rnd = new Random(DateTime.Now.Millisecond);
			minutesInterval = rnd.Next(m_FollowIntervalMinMinutes, m_FollowIntervalMaxMinutes);

			m_NextRunningDate = DateTime.Now.AddMinutes(minutesInterval);
		}

		public void Run()
		{
			FriendProspect nextFriendProspect = m_DataRepository.GetNextFriendProspectToFollowForModel(m_Model);
			if (nextFriendProspect != null)
			{
				FollowUserOptions fuo = new FollowUserOptions() { Follow = true, ScreenName = nextFriendProspect.UserName };
				m_TwitterService.FollowUser(fuo);

				TwitterError error = m_TwitterService.Response.Error;
				if (error != null)
					throw new ApplicationException(error.ToString());

				m_DataRepository.LogFriendProspectAsFriendForModel(m_Model, nextFriendProspect);
			}
		}
	}
}
