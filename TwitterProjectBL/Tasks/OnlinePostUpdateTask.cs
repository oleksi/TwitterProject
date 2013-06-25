using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TweetSharp;
using TwitterProjectData;
using TwitterProjectModel;

namespace TwitterProjectBL.Tasks
{
	public class OnlinePostUpdateTask : BaseTask
	{
		private const string C_URL_Placeholder = "<----URL--->";

		private PostUpdateRepository m_DataRepository = null;
		private string m_ModelXMLFeedURL = "";
		private string m_ModelDestinationURL = "";
		private int m_CheckOnlineStatusIntervalMins = 0;
		private int m_OnlinePostsIntervalMins = 0;
		private bool m_IsCurrentlyOnline = false;

		public OnlinePostUpdateTask(PostUpdateRepository dataRepository, TwitterService twitterService, Model model, string noShowStartTime, string noShowEndTime, string modelXMLFeedURL, string modelDestinationURL, int checkOnlineStatusIntervalMins, int onlinePostsIntervalMins) : base(twitterService, model, noShowStartTime, noShowEndTime)
		{
			m_DataRepository = dataRepository;
			m_ModelXMLFeedURL = modelXMLFeedURL;
			m_ModelDestinationURL = modelDestinationURL;
			m_CheckOnlineStatusIntervalMins = checkOnlineStatusIntervalMins;
			m_OnlinePostsIntervalMins = onlinePostsIntervalMins;

			SetNextRunningDate();
		}

		public override void SetNextRunningDate()
		{
			if (IsNoShowTime() == false) //regualr hours
				if (m_IsCurrentlyOnline == true)
					m_NextRunningDate = DateTime.Now.AddMinutes(m_OnlinePostsIntervalMins);
				else
					m_NextRunningDate = DateTime.Now.AddMinutes(m_CheckOnlineStatusIntervalMins);
			else
				m_NextRunningDate = GetNoShowTimeEndTime().AddMinutes(m_CheckOnlineStatusIntervalMins);
		}

		public override void Run()
		{
			//checking onlne status 
			HttpWebRequest httpReq = (HttpWebRequest)HttpWebRequest.Create(m_ModelXMLFeedURL);
			string respXmlStr = "";
			using (HttpWebResponse httpResp = (HttpWebResponse)httpReq.GetResponse())
			{
				using (StreamReader respStream = new StreamReader(httpResp.GetResponseStream()))
				{
					respXmlStr = respStream.ReadToEnd();
				}
			}
 
			XDocument xDoc = XDocument.Parse(respXmlStr);
			XElement onlineStatusElement = xDoc.Root.Descendants().SingleOrDefault(xe => xe.Name == "onlinestatus");

			if (onlineStatusElement.Value == "1")
			{
				PostUpdate newPostUpdate = m_DataRepository.GetNextPostUpdateForModel(m_Model, PostUpdateType.Online);
				string twitterMessage = newPostUpdate.PostText;
				if (twitterMessage.Contains(C_URL_Placeholder))
					twitterMessage = twitterMessage.Replace(C_URL_Placeholder, "{0}");
				else
					twitterMessage += " {0}";

				twitterMessage = String.Format(twitterMessage, m_ModelDestinationURL);

				//publishing online status message
				m_TwitterService.SendTweet(new SendTweetOptions() { Status = twitterMessage });

				TwitterError error = m_TwitterService.Response.Error;
				if (error != null)
					throw new TwitterProjectException(m_Model.Id.Value, error);

				m_DataRepository.LogPostUpdateAsPublishedForModel(newPostUpdate, m_Model, PostUpdateType.Online);

				m_IsCurrentlyOnline = true;
			}
			else
			{
				m_IsCurrentlyOnline = false;
			}
		}
	}
}
