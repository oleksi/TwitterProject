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
	public class RegularPostUpdateTask : ITask
	{
		private PostUpdateRepository m_DataRepository = null;
		private TwitterService m_TwitterService = null;
		private Model m_Model = null;
		private string m_NoShowStartTime = "";
		private string m_NoShowEndTime = "";
		private int m_NextPublishMinMinute = 0;
		private int m_NextPublishMaxMinute = 0;
		private DateTime m_NextRunningDate = DateTime.MinValue;
		private bool m_NoShowTimeSet = false;

		public RegularPostUpdateTask(PostUpdateRepository dataRepository, TwitterService twitterService, Model model, string noShowStartTime, string noShowEndTime, int nextPublishMinMinute, int nextPublishMaxMinute)
		{
			m_DataRepository = dataRepository;
			m_TwitterService = twitterService;
			m_Model = model;
			m_NoShowStartTime = noShowStartTime;
			m_NoShowEndTime = noShowEndTime;
			m_NextPublishMinMinute = nextPublishMinMinute;
			m_NextPublishMaxMinute = nextPublishMaxMinute;

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
			minutesInterval = rnd.Next(m_NextPublishMinMinute, m_NextPublishMaxMinute);

			DateTime noShowStartTime = DateTime.Parse(String.Format("{0} {1}", DateTime.Now.ToString("MM/d/yyyy"), m_NoShowStartTime));
			DateTime noShowEndTime = DateTime.Parse(String.Format("{0} {1}", DateTime.Now.ToString("MM/d/yyyy"), m_NoShowEndTime));

			if (DateTime.Now >= noShowStartTime && DateTime.Now < noShowEndTime)
			{
				if (m_NoShowTimeSet == false)
				{
					m_NextRunningDate = noShowEndTime.AddMinutes(minutesInterval);
					m_NoShowTimeSet = true;
				}
			}
			else 
			{
				m_NextRunningDate = DateTime.Now.AddMinutes(minutesInterval);
				m_NoShowTimeSet = false;
			}
		}

		public void Run()
		{
			PostUpdate newPostUpdate = m_DataRepository.GetNextPostUpdateForModel(m_Model, PostUpdateType.Regular);
			m_TwitterService.SendTweet(new SendTweetOptions() { Status = newPostUpdate.PostText });

			TwitterError error = m_TwitterService.Response.Error;
			if (error != null)
				throw new ApplicationException(error.ToString());

			m_DataRepository.LogPostUpdateAsPublishedForModel(newPostUpdate, m_Model, PostUpdateType.Regular);
		}
	}
}
