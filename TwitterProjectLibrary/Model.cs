using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterProjectModel
{
	public class Model
	{
		public virtual int? Id { get; set; }
		public virtual string UserName { get; set; }
		public virtual string TwitterConsumerKey { get; set; }
		public virtual string TwitterConsumerSecret { get; set; }
		public virtual string TwitterAccessToken { get; set; }
		public virtual string TwitterAccessTokenSecret { get; set; }
		public virtual string LiveChatURL { get; set; }
		public virtual string OnlineStatusXMLFeed { get; set; }
		public virtual int FriendFinder_MinutesToWaitMin { get; set; }
		public virtual int FriendFinder_MinutesToWaitMax { get; set; }
		public virtual int FriendFinder_MaxFolloewersCount { get; set; }
		public virtual double FriendFinder_FollowersFriendsProportion { get; set; }
		public virtual string RegularPost_NoShowStartTime { get; set; }
		public virtual string RegularPost_NoShowEndTime { get; set; }
		public virtual int RegularPost_NextPublishMinMinute { get; set; }
		public virtual int RegularPost_NextPublishMaxMinute { get; set; }
		public virtual int OnlinePost_CheckOnlineStatusIntervalMins { get; set; }
		public virtual int OnlinePost_OnlinePostsIntervalMins { get; set; }
		public virtual int FollowFriend_FollowIntervalMinMinutes { get; set; }
		public virtual int FollowFriend_FollowIntervalMaxMinutes { get; set; }
		public virtual int FollowFriend_UnfollowIntervalMinMinutes { get; set; }
		public virtual int FollowFriend_UnfollowIntervalMaxMinutes { get; set; }
		public virtual bool FriendFinderTask { get; set; }
		public virtual bool RegularPostUpdateTask { get; set; }
		public virtual bool OnlinePostUpdateTask { get; set; }
		public virtual bool FollowFriendProspectsTask { get; set; }
		public virtual bool UnfollowFriendTask { get; set; }
	}
}
