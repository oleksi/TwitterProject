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

		private PromoPostRepository m_DataRepository = null;
		private string m_StreamateXMLRequest;
		private bool m_IsCurrentlyOnline = false;

		public OnlinePostUpdateTask(PromoPostRepository dataRepository, TwitterService twitterService, Model model, string streamateXMLRequest)
			: base(twitterService, model)
		{
			m_DataRepository = dataRepository;
			m_StreamateXMLRequest = streamateXMLRequest.Replace("[MODEL_NAME]", m_Model.UserName);

			SetNextRunningDate();
		}

		public override void SetNextRunningDate()
		{
			if (IsNoShowTime() == false) //regualr hours
				if (m_IsCurrentlyOnline == true)
					m_NextRunningDate = DateTime.Now.AddMinutes(m_Model.OnlinePost_OnlinePostsIntervalMins);
				else
					m_NextRunningDate = DateTime.Now.AddMinutes(m_Model.OnlinePost_CheckOnlineStatusIntervalMins);
			else
				m_NextRunningDate = GetNoShowTimeEndTime().AddMinutes(m_Model.OnlinePost_CheckOnlineStatusIntervalMins);
		}

		public override void Run()
		{
			bool isModelOnline = false;
			if (m_Model.From == "LiveJasmin")
			{
				//checking onlne status 
				HttpWebRequest httpReq = (HttpWebRequest)HttpWebRequest.Create(m_Model.OnlineStatusXMLFeed);
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

				isModelOnline = (onlineStatusElement != null && onlineStatusElement.Value == "1");
			}
			else //Streamate model
			{
				string responseXml = WebRequestPostData(m_Model.OnlineStatusXMLFeed, m_StreamateXMLRequest);
				XDocument xDoc = XDocument.Parse(responseXml);
				XElement onlineStatusElement = xDoc.Root.Descendants().SingleOrDefault(xe => xe.Name == "Performer");

				isModelOnline = (onlineStatusElement != null && onlineStatusElement.Attribute("StreamType").Value.ToLower() == "live");		
			}

			if (isModelOnline == true)
			{
				PromoPost newPromoPost = m_DataRepository.GetNextPromoPostForModel(m_Model, AffiliateOffers.WebcamSites);
				string twitterMessage = newPromoPost.PromoPostText;
				if (twitterMessage.Contains(C_URL_Placeholder))
					twitterMessage = twitterMessage.Replace(C_URL_Placeholder, "{0}");
				else
					twitterMessage += " {0}";

				string liveChatURL = m_Model.GetAffiliateOfferUrl(AffiliateOffers.WebcamSites);
				if (String.IsNullOrEmpty(liveChatURL) == true)
					throw new ApplicationException(String.Format("Live Chat Url is not defined for Model Id = {0}", m_Model.Id));

				twitterMessage = String.Format(twitterMessage, liveChatURL);

				//publishing online status message
				m_TwitterService.SendTweet(new SendTweetOptions() { Status = twitterMessage });

				TwitterError error = m_TwitterService.Response.Error;
				if (error != null)
					throw new TwitterProjectException(m_Model.Id.Value, error);

				m_DataRepository.LogPromoPostAsPublishedForModel(newPromoPost, m_Model);

				m_IsCurrentlyOnline = true;
			}
			else
			{
				m_IsCurrentlyOnline = false;
			}
		}

		private static string WebRequestPostData(string url, string postData)
		{
			System.Net.WebRequest req = System.Net.WebRequest.Create(url);

			req.ContentType = "text/xml";
			req.Method = "POST";

			byte[] bytes = System.Text.Encoding.ASCII.GetBytes(postData);
			req.ContentLength = bytes.Length;

			using (Stream os = req.GetRequestStream())
			{
				os.Write(bytes, 0, bytes.Length);
			}

			using (System.Net.WebResponse resp = req.GetResponse())
			{
				if (resp == null) return null;

				using (System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream()))
				{
					return sr.ReadToEnd().Trim();
				}
			}
		}
	}
}
