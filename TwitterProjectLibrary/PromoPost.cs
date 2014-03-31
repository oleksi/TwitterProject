using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterProjectModel
{
	public enum AffiliateOffers
	{
		None = 1,
		WebcamSites = 2,
		SizeGenetics = 3,
		Anastasia = 1001
	}

	public class PromoPost
	{
		public virtual int? Id { get; set; }
		public virtual AffiliateOffers AffiliateOffer { get; set; }
		public virtual string PromoPostText { get; set; }

		public PromoPost()
		{
		}
	}
}
