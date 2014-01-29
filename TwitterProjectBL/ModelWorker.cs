using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetSharp;
using TwitterProjectBL.Tasks;
using TwitterProjectData;
using TwitterProjectModel;

namespace TwitterProjectBL
{
	public class ModelWorker
	{
		private TwitterService m_TwitterService = null;
		private Model m_Model = null;
		public List<ITask> Tasks { get; private set; }
		public Dictionary<string, string> Settings { get; private set; }

		public ModelWorker(Model model) : this(model, new Dictionary<string,string>())
		{
		}

		public ModelWorker(Model model, Dictionary<string,string> settings)
		{
			m_Model = model;
			Tasks = new List<ITask>();
			Settings = settings;

			m_TwitterService = new TwitterService(m_Model.TwitterConsumerKey, m_Model.TwitterConsumerSecret);
			m_TwitterService.AuthenticateWith(m_Model.TwitterAccessToken, m_Model.TwitterAccessTokenSecret);


			if (m_Model.FriendFinderTask == true)
			{
				FriendFinderTask friendFinderTask = new FriendFinderTask(
																			new FriendFinderRepository(),
																			m_TwitterService,
																			m_Model.RegularPost_NoShowStartTime,
																			m_Model.RegularPost_NoShowEndTime,
																			m_Model.FriendFinder_MinutesToWaitMin,
																			m_Model.FriendFinder_MinutesToWaitMax,
																			m_Model.FriendFinder_MaxFolloewersCount,
																			m_Model.FriendFinder_FollowersFriendsProportion
															);
				Tasks.Add(friendFinderTask);
			}

			if (m_Model.RegularPostUpdateTask == true)
			{
				RegularPostUpdateTask regularPostUpdateTask = new RegularPostUpdateTask(
																			new PromoPostRepository(),
																			m_TwitterService,
																			m_Model
															);
				Tasks.Add(regularPostUpdateTask);
			}

			if (m_Model.OnlinePostUpdateTask == true)
			{
				string streamateXMLRequest = "";

				if (m_Model.From == "Streamate")
				{
					streamateXMLRequest = Settings["StreamateXMLRequest"];
					if (String.IsNullOrEmpty(streamateXMLRequest) == true)
						throw new ApplicationException("StreamateXMLRequest is not defined");
				}

				OnlinePostUpdateTask onlinePostUpdateTask = new OnlinePostUpdateTask(
																			new PromoPostRepository()
																			, m_TwitterService
																			, m_Model
																			, streamateXMLRequest
															);
				Tasks.Add(onlinePostUpdateTask);
			}

			if (m_Model.FollowFriendProspectsTask == true)
			{
				FollowFriendProspectsTask followFriendProspectsTask = new FollowFriendProspectsTask(
																			new ModelRepository()
																			, m_TwitterService
																			, m_Model
															);
				Tasks.Add(followFriendProspectsTask);
			}

			if (m_Model.UnfollowFriendTask == true)
			{
				UnfollowFriendTask unfollowFriendTask = new UnfollowFriendTask(
																			new ModelRepository()
																			, m_TwitterService
																			, m_Model
															);
				Tasks.Add(unfollowFriendTask);
			}

		}
	}
}
