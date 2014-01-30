using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterProjectModel;

namespace TwitterProjectData
{
	public class PromoPostRepository
	{
		private static ISession getSession()
		{
			return SessionProvider.SessionFactory.OpenSession();
		}

		public PromoPost GetNextPromoPostForModel(Model model, AffiliateOffers affiliateOffer)
		{
			using (var session = getSession())
			{
				IList<int> notPublishedPostIDs = getUnpublishedPostUpdatesForModel(model, affiliateOffer);

				//all were posted => clearing log to prepare for the next cycle
				if (notPublishedPostIDs.Count == 0)
				{
					session.CreateSQLQuery("delete PromoPostLogs from PromoPostLogs ppl join PromoPosts pp on pp.PromoPostId = ppl.PromoPostId where pp.AffiliateOfferId = :AffiliateOfferId and ppl.ModelId = :ModelId").SetParameter("AffiliateOfferId", (int)affiliateOffer).SetParameter("ModelId", model.Id).ExecuteUpdate();
					notPublishedPostIDs = getUnpublishedPostUpdatesForModel(model, affiliateOffer);
				}

				Random rnd = new Random(DateTime.Now.Millisecond);
				int rndIndex = rnd.Next(0, notPublishedPostIDs.Count - 1);

				return session.Get<PromoPost>(notPublishedPostIDs[rndIndex]);
			}
		}

		public void LogPromoPostAsPublishedForModel(PromoPost promoPost, Model model)
		{
			using (var session = getSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					PromoPostLog promoPostLog = new PromoPostLog() { Model = model, PromoPost = promoPost, LastPublishedDate = DateTime.Now };
					session.SaveOrUpdate(promoPostLog);

					transaction.Commit();
				}
			}
		}

		private IList<int> getUnpublishedPostUpdatesForModel(Model model, AffiliateOffers affiliateOffer)
		{
			using (var session = getSession())
			{
				return session.QueryOver<PromoPost>().Where(ppl => ppl.AffiliateOffer == affiliateOffer).AndRestrictionOn(pp => pp.Id).Not.IsIn(session.QueryOver<PromoPostLog>().Where(ppl => ppl.Model.Id == model.Id).Select(ppl => ppl.PromoPost.Id).List<int>().ToArray()).Select(pp => pp.Id).List<int>();
			}
		}
	}
}
