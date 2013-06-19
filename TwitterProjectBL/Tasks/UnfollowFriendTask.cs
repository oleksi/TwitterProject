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
	public class UnfollowFriendTask : ITask
	{
		private ModelRepository m_DataRepository = null;
		private TwitterService m_TwitterService = null;
		private Model m_Model = null;
		private int m_UnfollowIntervalMinMinutes = 0;
		private int m_UnfollowIntervalMaxMinutes = 0;
		private DateTime m_NextRunningDate = DateTime.MinValue;

		public UnfollowFriendTask(ModelRepository dataRepository, TwitterService twitterService, Model model, int unfollowIntervalMinMinutes, int unfollowIntervalMaxMinutes)
		{
			m_DataRepository = dataRepository;
			m_TwitterService = twitterService;
			m_Model = model;
			m_UnfollowIntervalMinMinutes = unfollowIntervalMinMinutes;
			m_UnfollowIntervalMaxMinutes = unfollowIntervalMaxMinutes;

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
			minutesInterval = rnd.Next(m_UnfollowIntervalMinMinutes, m_UnfollowIntervalMaxMinutes);

			m_NextRunningDate = DateTime.Now.AddMinutes(minutesInterval);
		}

		public void Run()
		{
			ModelFriendsLog modelFriendsLog = m_DataRepository.GetNextFriendToUnfollowForModel(m_Model);
			if (modelFriendsLog != null)
			{
				UnfollowUserOptions unfollowUserOptions = new UnfollowUserOptions() { ScreenName = modelFriendsLog.Friend.UserName };
				m_TwitterService.UnfollowUser(unfollowUserOptions);

				TwitterError error = m_TwitterService.Response.Error;
				if (error != null)
					throw new ApplicationException(error.ToString());

				m_DataRepository.LogFriendAsUnfollowedForModel(m_Model, modelFriendsLog);
			}
		}
	}
}
