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
			SessionProvider.RebuildSchema();

			ModelRepository modelRepo = new ModelRepository();
			Model model = modelRepo.GetModelById(1);

			FriendProspect nextFriendProspect = modelRepo.GetNextFriendProspectToFollowForModel(model);
			//modelRepo.LogFriendProspectAsFriendForModel(model, nextFriendProspect);

		}
	}
}
