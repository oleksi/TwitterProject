using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using GenerateModelAccessTokens.Properties;
using TweetSharp;

namespace GenerateModelAccessTokens
{
	class Program
	{
		static void Main(string[] args)
		{
			TwitterService service = new TwitterService(Settings.Default.TwitterConsumerKey, Settings.Default.TwitterConsumerSecret);
			OAuthRequestToken requestToken = service.GetRequestToken();

			Uri uri = service.GetAuthorizationUri(requestToken);
			Process.Start(uri.ToString());
			string verifier = Console.ReadLine(); // <-- This is input into your application by your user
			OAuthAccessToken access = service.GetAccessToken(requestToken, verifier);

			using (StreamWriter sw = new StreamWriter("AccessToken.txt"))
			{
				sw.WriteLine(access.Token);
				sw.WriteLine(access.TokenSecret);
			}

			Console.WriteLine("Ok");
		}
	}
}
