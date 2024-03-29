﻿using System;
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

		public IList<Model> GetActiveModels()
		{
			IList<Model> models = new List<Model>();
			using (var session = getSession())
			{
				models = session.QueryOver<Model>().Where(mdl => mdl.IsActive == true).OrderBy(mdl => mdl.Id).Asc.List<Model>();
			}

			return models;
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
				ISQLQuery sqlQuery = session.CreateSQLQuery("EXEC usp_GetNextFriendProspectToFollowForModel @ModelId = :ModelId");
				sqlQuery.AddEntity(typeof(FriendProspect));
				sqlQuery.SetParameter("ModelId", model.Id);
				IList<FriendProspect> friendProspectList = sqlQuery.List<FriendProspect>();

				if (friendProspectList.Count > 0)
				{
					string username = friendProspectList[0].ReferredBy.UserName; //need to prevent lazy initialization
					return friendProspectList[0];
				}
				else
					return null;

				//var friendProspect = session.QueryOver<FriendProspect>().WhereRestrictionOn(fp => fp.Id).Not.IsIn(session.QueryOver<ModelFriendsLog>().Where(mfl => mfl.Model.Id == model.Id).Select(mfl => mfl.Friend.Id).List<int>().ToArray()).Where(fp => fp.IsActive == true).Take(1).SingleOrDefault();
				//string userName = friendProspect.ReferredBy.UserName; //need to prevent lazy initialization
				//return friendProspect;
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

		public void LogFriendProdspectAsNotActive(FriendProspect friendProspect)
		{
			using (var session = getSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					friendProspect.IsActive = false;
					session.SaveOrUpdate(friendProspect);
					transaction.Commit();
				}
			}
		}
	}
}
