using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterProjectModel
{
	public class AffiliateOfferUrl
	{
		public virtual AffiliateOffers AffiliateOffer { get; set; }
		public virtual string Url { get; set; }
	}
}
