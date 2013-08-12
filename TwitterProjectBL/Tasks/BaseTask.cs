using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetSharp;
using TwitterProjectModel;

namespace TwitterProjectBL.Tasks
{
	public class BaseTask : ITask
	{
		protected TwitterService m_TwitterService = null;
		protected Model m_Model = null;
		protected DateTime m_NextRunningDate = DateTime.MinValue;

		public BaseTask(TwitterService twitterService, Model model)
		{
			m_TwitterService = twitterService;
			m_Model = model;
		}

		public DateTime GetNextRunningDate()
		{
			return m_NextRunningDate;
		}

		public virtual void SetNextRunningDate()
		{
		}

		public virtual void Run()
		{
		}

		protected bool IsNoShowTime()
		{
			DateTime noShowStartTime = DateTime.Parse(String.Format("{0} {1}", DateTime.Now.ToString("MM/d/yyyy"), m_Model.RegularPost_NoShowStartTime));
			DateTime noShowEndTime = DateTime.Parse(String.Format("{0} {1}", DateTime.Now.ToString("MM/d/yyyy"), m_Model.RegularPost_NoShowEndTime));

			return (DateTime.Now >= noShowStartTime && DateTime.Now < noShowEndTime);		
		}

		protected DateTime GetNoShowTimeEndTime()
		{
			DateTime noShowEndTime = DateTime.Parse(String.Format("{0} {1}", DateTime.Now.ToString("MM/d/yyyy"), m_Model.RegularPost_NoShowEndTime));
			return noShowEndTime;
		}
	}
}
