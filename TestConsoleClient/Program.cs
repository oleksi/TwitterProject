using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TwitterProjectData;
using TwitterProjectModel;

namespace TestConsoleClient
{
	class Program
	{
		static void Main(string[] args)
		{
			//SessionProvider.RebuildSchema();

			PromoPostRepository repo = new PromoPostRepository();

			PromoPost promoPost = repo.GetNextPromoPostForModel(new Model { Id = 19 }, AffiliateOffers.None);
			repo.LogPromoPostAsPublishedForModel(promoPost, new Model { Id = 19 });

		}
	}
}
