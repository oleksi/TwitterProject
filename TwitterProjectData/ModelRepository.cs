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
				return friendProspect;
			}
		}

		public void LogFriendProspectAsFriendForModel(Model model, FriendProspect friendProspect)
		{
			using (var session = getSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					var modelFriendsLog = new ModelFriendsLog() { Model = model, Friend = friendProspect, DateFriended = DateTime.Now };
					session.SaveOrUpdate(modelFriendsLog);
					transaction.Commit();
				}
			}
		}

		public string GetNextFriendToUnfollowForModel(Model model)
		{
			string friendUserName = "";
			using (var session = getSession())
			{
				ModelFriendsLog modelFriendsLog = session.QueryOver<ModelFriendsLog>().Where(mfl => mfl.Model.Id == model.Id).OrderBy(mfl => mfl.DateFriended).Asc.Take(1).SingleOrDefault();
				if (modelFriendsLog != null)
					friendUserName = modelFriendsLog.Friend.UserName;
			}

			return friendUserName;
		}
	}
}
