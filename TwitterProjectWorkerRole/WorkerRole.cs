using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using TweetSharp;
using TwitterProjectBL.Tasks;
using TwitterProjectData;
using TwitterProjectModel;

namespace TwitterProjectWorkerRole
{
	public class WorkerRole : RoleEntryPoint
	{
		private TwitterService m_TwitterService = null;
		private List<ITask> m_Tasks = null;

		public override void Run()
		{
			// This is a sample worker implementation. Replace with your logic.
			Trace.WriteLine("TwitterProjectWorkerRole entry point called", "Information");

			while (true)
			{
				foreach (ITask task in m_Tasks)
				{
					if (task.GetNextRunningDate() <= DateTime.Now)
					{
						try
						{
							task.Run();
						}
						catch (Exception ex)
						{
							Trace.TraceError(ex.ToString());
						}
						finally
						{
							task.SetNextRunningDate();
						}
					}
				}

				Thread.Sleep(10000);
				Trace.WriteLine("Working", "Information");
			}
		}

		public override bool OnStart()
		{
			ModelRepository modelRepository = new ModelRepository();
			Model currentModel = modelRepository.GetModelById(Convert.ToInt32(RoleEnvironment.GetConfigurationSettingValue("ModelId")));

			m_TwitterService = new TwitterService(currentModel.TwitterConsumerKey, currentModel.TwitterConsumerSecret);
			m_TwitterService.AuthenticateWith(currentModel.TwitterAccessToken, currentModel.TwitterAccessTokenSecret);

			m_Tasks = new List<ITask>();

			if (Convert.ToBoolean(RoleEnvironment.GetConfigurationSettingValue("FriendFinderTask")) == true)
			{
				FriendFinderTask friendFinderTask = new FriendFinderTask(
																			new FriendFinderRepository(),
																			m_TwitterService,
																			Convert.ToInt32(RoleEnvironment.GetConfigurationSettingValue("FriendFinder_MinutesToWaitMin")),
																			Convert.ToInt32(RoleEnvironment.GetConfigurationSettingValue("FriendFinder_MinutesToWaitMax")),
																			Convert.ToInt32(RoleEnvironment.GetConfigurationSettingValue("FriendFinder_MaxFolloewersCount")),
																			Convert.ToDouble(RoleEnvironment.GetConfigurationSettingValue("FriendFinder_FollowersFriendsProportion"))
															);
				m_Tasks.Add(friendFinderTask);
			}

			if (Convert.ToBoolean(RoleEnvironment.GetConfigurationSettingValue("RegularPostUpdateTask")) == true)
			{
				RegularPostUpdateTask regularPostUpdateTask = new RegularPostUpdateTask(
																			new PostUpdateRepository(),
																			m_TwitterService,
																			currentModel,
																			RoleEnvironment.GetConfigurationSettingValue("RegularPost_NoShowStartTime"),
																			RoleEnvironment.GetConfigurationSettingValue("RegularPost_NoShowEndTime"),
																			Convert.ToInt32(RoleEnvironment.GetConfigurationSettingValue("RegularPost_NextPublishMinMinute")),
																			Convert.ToInt32(RoleEnvironment.GetConfigurationSettingValue("RegularPost_NextPublishMaxMinute"))
															);
				m_Tasks.Add(regularPostUpdateTask);
			}

			if (Convert.ToBoolean(RoleEnvironment.GetConfigurationSettingValue("OnlinePostUpdateTask")) == true)
			{
				OnlinePostUpdateTask onlinePostUpdateTask = new OnlinePostUpdateTask(
																			new PostUpdateRepository()
																			, m_TwitterService
																			, currentModel
																			, currentModel.OnlineStatusXMLFeed
																			, currentModel.LiveChatURL
																			, Convert.ToInt32(RoleEnvironment.GetConfigurationSettingValue("OnlinePost_CheckOnlineStatusIntervalMins"))
																			, Convert.ToInt32(RoleEnvironment.GetConfigurationSettingValue("OnlinePost_OnlinePostsIntervalMins"))
															);
				m_Tasks.Add(onlinePostUpdateTask);
			}

			if (Convert.ToBoolean(RoleEnvironment.GetConfigurationSettingValue("FollowFriendProspectsTask")) == true)
			{
				FollowFriendProspectsTask followFriendProspects = new FollowFriendProspectsTask(
																			new ModelRepository()
																			, m_TwitterService
																			, currentModel
																			, Convert.ToInt32(RoleEnvironment.GetConfigurationSettingValue("FollowFriend_FollowIntervalMinMinutes"))
																			, Convert.ToInt32(RoleEnvironment.GetConfigurationSettingValue("FollowFriend_FollowIntervalMaxMinutes"))
															);
				m_Tasks.Add(followFriendProspects);
			}

			// Set the maximum number of concurrent connections 
			ServicePointManager.DefaultConnectionLimit = 12;

			// For information on handling configuration changes
			// see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

			return base.OnStart();
		}
	}
}
