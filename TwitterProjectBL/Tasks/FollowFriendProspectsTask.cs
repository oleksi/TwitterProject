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
	public class FollowFriendProspectsTask : BaseTask
	{
		private ModelRepository m_DataRepository = null;
		private int m_FollowIntervalMinMinutes = 0;
		private int m_FollowIntervalMaxMinutes = 0;
		bool m_LastFollowWasUnseccessful = false;

		public FollowFriendProspectsTask(ModelRepository dataRepository, TwitterService twitterService, Model model, string noShowStartTime, string noShowEndTime, int followIntervalMinMinutes, int followIntervalMaxMinutes) : base(twitterService, model, noShowStartTime, noShowEndTime)
		{
			m_DataRepository = dataRepository;
			m_FollowIntervalMinMinutes = followIntervalMinMinutes;
			m_FollowIntervalMaxMinutes = followIntervalMaxMinutes;

			SetNextRunningDate();
		}

		public override void SetNextRunningDate()
		{
			if (IsNoShowTime() == false) //regualr hours
				if (m_LastFollowWasUnseccessful == false)
				{
					int minutesInterval = 0;
					Random rnd = new Random(DateTime.Now.Millisecond);
					minutesInterval = rnd.Next(m_FollowIntervalMinMinutes, m_FollowIntervalMaxMinutes);

					m_NextRunningDate = DateTime.Now.AddMinutes(minutesInterval);
				}
				else
				{
					//since last follow was unseccessful repeating in 2 mins
					m_NextRunningDate = DateTime.Now.AddMinutes(2);
				}
			else //now show time hours
				m_NextRunningDate = GetNoShowTimeEndTime().AddMinutes(m_FollowIntervalMinMinutes);

		}

		public override void Run()
		{
			FriendProspect nextFriendProspect = m_DataRepository.GetNextFriendProspectToFollowForModel(m_Model);
			if (nextFriendProspect != null)
			{
				FollowUserOptions fuo = new FollowUserOptions() { Follow = true, ScreenName = nextFriendProspect.UserName };
				m_TwitterService.FollowUser(fuo);

				TwitterError error = m_TwitterService.Response.Error;
				if (error != null)
				{
					m_LastFollowWasUnseccessful = true;
					//159 = user's account was suspened; 34 = page doesn't exist; 108 = can't find specified user; 
					if (error.Code == 159 || error.Code == 34 || error.Code == 108)
					{
						//marking as non-active
						m_DataRepository.LogFriendProdspectAsNotActive(nextFriendProspect);
						return;
					}
					//160 = already requested to follow, waiting for respones; 162 = blocked by user
					else if (error.Code == 160 || error.Code == 162) //this theoretically should't happen; keeping this code for fixing what was messed up at the beginning
					{
						//logging it as inactive
						m_DataRepository.LogFriendProspectAsFriendForModel(m_Model, nextFriendProspect, false);
						return;
					}
					else
					{
						//some other error, throwing the exception
						throw new TwitterProjectException(m_Model.Id.Value, error);
					}
				}

				m_DataRepository.LogFriendProspectAsFriendForModel(m_Model, nextFriendProspect, true);
				m_LastFollowWasUnseccessful = false;
			}
		}
	}
}
