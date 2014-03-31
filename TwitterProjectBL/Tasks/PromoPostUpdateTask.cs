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
	public class PromoPostUpdateTask : BaseTask
	{
		private const string C_URL_Placeholder = "<----URL--->";

		private PromoPostRepository m_DataRepository = null;

		public PromoPostUpdateTask(PromoPostRepository dataRepository, TwitterService twitterService, Model model)
			: base(twitterService, model)
		{
			m_DataRepository = dataRepository;

			SetNextRunningDate();
		}

		public override void SetNextRunningDate()
		{
			int minutesInterval = 0;
			Random rnd = new Random(DateTime.Now.Millisecond);
			minutesInterval = rnd.Next(m_Model.PromoPost_NextPublishMinMinute, m_Model.PromoPost_NextPublishMaxMinute);

			if (IsNoShowTime() == false) //regualr hours
				m_NextRunningDate = DateTime.Now.AddMinutes(minutesInterval);
			else //now show time hours
				m_NextRunningDate = GetNoShowTimeEndTime().AddMinutes(minutesInterval);
		}

		public override void Run()
		{
			PromoPost newPromoPost = m_DataRepository.GetNextPromoPostForModel(m_Model, AffiliateOffers.Anastasia);
			string twitterMessage = newPromoPost.PromoPostText;
			if (twitterMessage.Contains(C_URL_Placeholder))
				twitterMessage = twitterMessage.Replace(C_URL_Placeholder, "{0}");
			else
				twitterMessage += " {0}";

			string affiliateURL = m_Model.GetAffiliateOfferUrl(AffiliateOffers.SizeGenetics);
			if (String.IsNullOrEmpty(affiliateURL) == true)
				throw new ApplicationException(String.Format("Affiliate Url is not defined for Model Id = {0}", m_Model.Id));

			twitterMessage = String.Format(twitterMessage, affiliateURL);
			m_TwitterService.SendTweet(new SendTweetOptions() { Status = twitterMessage });

			m_DataRepository.LogPromoPostAsPublishedForModel(newPromoPost, m_Model);

			TwitterError error = m_TwitterService.Response.Error;
			if (error != null)
				throw new TwitterProjectException(m_Model.Id.Value, error);
		}
	}
}
