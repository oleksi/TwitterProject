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
	public class RegularPostUpdateTask : BaseTask
	{
		private PostUpdateRepository m_DataRepository = null;
		private int m_NextPublishMinMinute = 0;
		private int m_NextPublishMaxMinute = 0;

		public RegularPostUpdateTask(PostUpdateRepository dataRepository, TwitterService twitterService, Model model, string noShowStartTime, string noShowEndTime, int nextPublishMinMinute, int nextPublishMaxMinute) : base(twitterService, model, noShowStartTime, noShowEndTime)
		{
			m_DataRepository = dataRepository;
			m_NextPublishMinMinute = nextPublishMinMinute;
			m_NextPublishMaxMinute = nextPublishMaxMinute;

			SetNextRunningDate();
		}

		public override void SetNextRunningDate()
		{
			int minutesInterval = 0;
			Random rnd = new Random(DateTime.Now.Millisecond);
			minutesInterval = rnd.Next(m_NextPublishMinMinute, m_NextPublishMaxMinute);

			if (IsNoShowTime() == false) //regualr hours
				m_NextRunningDate = DateTime.Now.AddMinutes(minutesInterval);
			else //now show time hours
				m_NextRunningDate = GetNoShowTimeEndTime().AddMinutes(minutesInterval);
		}

		public override void Run()
		{
			PostUpdate newPostUpdate = m_DataRepository.GetNextPostUpdateForModel(m_Model, PostUpdateType.Regular);
			m_TwitterService.SendTweet(new SendTweetOptions() { Status = newPostUpdate.PostText });

			TwitterError error = m_TwitterService.Response.Error;
			if (error != null)
				throw new TwitterProjectException(m_Model.Id.Value, error);

			m_DataRepository.LogPostUpdateAsPublishedForModel(newPostUpdate, m_Model, PostUpdateType.Regular);
		}
	}
}
