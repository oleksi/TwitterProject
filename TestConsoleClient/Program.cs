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

			ModelRepository modelRepo = new ModelRepository();
			Model model = modelRepo.GetModelById(19);

			string url = model.GetAffiliateOfferUrl(AffiliateOffers.SizeGenetics);
		}
	}
}
