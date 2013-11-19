using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NetworkWithModels.Properties;
using TweetSharp;
using TwitterProjectData;
using TwitterProjectModel;

namespace NetworkWithModels
{
	class Program
	{
		const int c_NetworkingModelId = 21;

		static void Main(string[] args)
		{
			ModelRepository modelRepository = new ModelRepository();

			IList<Model> models = modelRepository.GetActiveModels();
			Model networkingModel = modelRepository.GetModelById(c_NetworkingModelId);

			TwitterService service = new TwitterService(Settings.Default.TwitterConsumerKey, Settings.Default.TwitterConsumerSecret);
			service.AuthenticateWith(networkingModel.TwitterAccessToken, networkingModel.TwitterAccessTokenSecret);

			//getting Twitter username for each Model
			Dictionary<string, string> modelsTwitterUsernames = new Dictionary<string, string>();
			foreach (Model currModel in models)
			{
				service.AuthenticateWith(currModel.TwitterAccessToken, currModel.TwitterAccessTokenSecret);
				TwitterUser currTwitterUser = service.GetUserProfile(new GetUserProfileOptions() { });

				modelsTwitterUsernames[currModel.UserName] = currTwitterUser.ScreenName;
			}

			//following all other models
			service.AuthenticateWith(networkingModel.TwitterAccessToken, networkingModel.TwitterAccessTokenSecret);
			foreach (Model currModel in models)
			{
				if (currModel.Id == networkingModel.Id)
					continue;

				try
				{
					FollowUserOptions fuo = new FollowUserOptions() { Follow = true, ScreenName = modelsTwitterUsernames[currModel.UserName] };
					service.FollowUser(fuo);

					Console.WriteLine(String.Format("Successfully followed {0} at {1}", currModel.UserName, DateTime.Now.ToString()));
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex.ToString());	
				}

				Random rnd = new Random(DateTime.Now.Millisecond);
				int sleepInMls = rnd.Next(900000, 1200000);
				Thread.Sleep(sleepInMls);
			}

			//let all other models follow us
			FollowUserOptions fuo1 = new FollowUserOptions() { Follow = true, ScreenName = modelsTwitterUsernames[networkingModel.UserName] };
			foreach (Model currModel in models)
			{
				if (currModel.Id == networkingModel.Id)
					continue;

				try
				{
					service.AuthenticateWith(currModel.TwitterAccessToken, currModel.TwitterAccessTokenSecret);
					service.FollowUser(fuo1);

					Console.WriteLine(String.Format("Successfully followed by {0}", currModel.UserName));
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}

				Thread.Sleep(1000);
			}

			Console.WriteLine("Ok");
			Console.ReadLine();
		}
	}
}
