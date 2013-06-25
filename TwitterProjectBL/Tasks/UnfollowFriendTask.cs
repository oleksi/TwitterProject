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
	public class UnfollowFriendTask : BaseTask
	{
		private ModelRepository m_DataRepository = null;
		private int m_UnfollowIntervalMinMinutes = 0;
		private int m_UnfollowIntervalMaxMinutes = 0;
		bool m_LastUnfollowWasUnseccessful = false;

		public UnfollowFriendTask(ModelRepository dataRepository, TwitterService twitterService, Model model, string noShowStartTime, string noShowEndTime, int unfollowIntervalMinMinutes, int unfollowIntervalMaxMinutes) : base(twitterService, model, noShowStartTime, noShowEndTime)
		{
			m_DataRepository = dataRepository;
			m_UnfollowIntervalMinMinutes = unfollowIntervalMinMinutes;
			m_UnfollowIntervalMaxMinutes = unfollowIntervalMaxMinutes;

			SetNextRunningDate();
		}

		public override void SetNextRunningDate()
		{
			if (IsNoShowTime() == false) //regualr hours
				if (m_LastUnfollowWasUnseccessful == false)
				{
					int minutesInterval = 0;
					Random rnd = new Random(DateTime.Now.Millisecond);
					minutesInterval = rnd.Next(m_UnfollowIntervalMinMinutes, m_UnfollowIntervalMaxMinutes);

					m_NextRunningDate = DateTime.Now.AddMinutes(minutesInterval);
				}
				else
				{
					//since last unfollow was unseccessful repeating in 2 mins
					m_NextRunningDate = DateTime.Now.AddMinutes(2);
				}
			else //now show time hours
				m_NextRunningDate = GetNoShowTimeEndTime().AddMinutes(m_UnfollowIntervalMinMinutes);
		}

		public override void Run()
		{
			ModelFriendsLog modelFriendsLog = m_DataRepository.GetNextFriendToUnfollowForModel(m_Model);
			if (modelFriendsLog != null)
			{
				UnfollowUserOptions unfollowUserOptions = new UnfollowUserOptions() { ScreenName = modelFriendsLog.Friend.UserName };
				m_TwitterService.UnfollowUser(unfollowUserOptions);

				TwitterError error = m_TwitterService.Response.Error;
				if (error != null)
				{
					m_LastUnfollowWasUnseccessful = true;
					//159 = user's account was suspened; 34 = page doesn't exist; 108 = can't find specified user; 
					if (error.Code == 159 || error.Code == 34 || error.Code == 108)
					{
						//marking as non-active
						m_DataRepository.LogFriendProdspectAsNotActive(modelFriendsLog.Friend);
						m_DataRepository.LogFriendAsUnfollowedForModel(m_Model, modelFriendsLog);
						return;
					}
					else
					{
						throw new TwitterProjectException(m_Model.Id.Value, error);
					}
				}

				m_DataRepository.LogFriendAsUnfollowedForModel(m_Model, modelFriendsLog);
				m_LastUnfollowWasUnseccessful = false;
			}
		}
	}
}
