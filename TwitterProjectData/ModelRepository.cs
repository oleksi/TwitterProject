using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using TwitterProjectModel;

namespace TwitterProjectData
{
	public class ModelRepository
	{
		private static ISession getSession()
		{
			return SessionProvider.SessionFactory.OpenSession();
		}

		public Model GetModelById(int id)
		{
			using (var session = getSession())
			{
				return session.Get<Model>(id);
			}
		}

		public void SaveModel(Model model)
		{
			using (var session = getSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					session.SaveOrUpdate(model);
					transaction.Commit();
				}
			}
		}

		public FriendProspect GetNextFriendProspectToFollowForModel(Model model)
		{
			using (var session = getSession())
			{
				var friendProspect = session.QueryOver<FriendProspect>().WhereRestrictionOn(fp => fp.Id).Not.IsIn(session.QueryOver<ModelFriendsLog>().Where(mfl => mfl.Model.Id == model.Id).Select(mfl => mfl.Friend.Id).List<int>().ToArray()).Take(1).SingleOrDefault();
				string userName = friendProspect.ReferredBy.UserName; //need to prevent lazy initialization
				return friendProspect;
			}
		}

		public void LogFriendProspectAsFriendForModel(Model model, FriendProspect friendProspect, bool isActive)
		{
			using (var session = getSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					var modelFriendsLog = new ModelFriendsLog() { Model = model, Friend = friendProspect, DateFriended = DateTime.Now, IsActive = isActive };
					session.SaveOrUpdate(modelFriendsLog);
					transaction.Commit();
				}
			}
		}

		public ModelFriendsLog GetNextFriendToUnfollowForModel(Model model)
		{
			using (var session = getSession())
			{
				ModelFriendsLog modelFriendsLog = session.QueryOver<ModelFriendsLog>().Where(mfl => mfl.Model.Id == model.Id && mfl.IsActive == true).OrderBy(mfl => mfl.DateFriended).Asc.Take(1).SingleOrDefault();
				string userName = modelFriendsLog.Friend.UserName; //need to prevent lazy initialization
				return modelFriendsLog;
			}
		}

		public void LogFriendAsUnfollowedForModel(Model model, ModelFriendsLog modelExFriend)
		{
			using (var session = getSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					modelExFriend.IsActive = false;
					session.SaveOrUpdate(modelExFriend);
					transaction.Commit();
				}
			}
		}
	}
}
