using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using TwitterProjectModel;

namespace TwitterProjectData
{
	public class PostUpdateRepository
	{
		private static ISession getSession()
		{
			return SessionProvider.SessionFactory.OpenSession();
		}

		public PostUpdate GetNextPostUpdateForModel(Model model, PostUpdateType type)
		{
			using (var session = getSession())
			{
				//getting the list of IDs that were not published yet in this cycle
				IList<int> notPublishedPostIDs = getUnpublishedPostUpdatesForModel(model, type);
				
				//all were posted => clearing log to prepare for the next cycle
				if (notPublishedPostIDs.Count == 0)
				{
					string tableName = (type == PostUpdateType.Regular) ? "RegularPostUpdateLogs" : "OnlinePostUpdateLogs";
					session.CreateSQLQuery(String.Format("delete from {0} where ModelId = :ModelId", tableName)).SetParameter("ModelId", model.Id).ExecuteUpdate();
					notPublishedPostIDs = getUnpublishedPostUpdatesForModel(model, type);
				}
				
				Random rnd = new Random(DateTime.Now.Millisecond);
				int rndIndex = rnd.Next(0, notPublishedPostIDs.Count - 1);

				if (type == PostUpdateType.Regular)
					return session.Get<RegularPostUpdate>(notPublishedPostIDs[rndIndex]);
				else
					return session.Get<OnlinePostUpdate>(notPublishedPostIDs[rndIndex]);
			}
		}

		public void LogPostUpdateAsPublishedForModel(PostUpdate postUpdate, Model model, PostUpdateType type)
		{
			using (var session = getSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					if (type == PostUpdateType.Regular)
					{
						RegularPostUpdateLog regularPostUpdateLog = new RegularPostUpdateLog() { Model = model, RegularPostUpdate = new RegularPostUpdate(postUpdate), LastPublishedDate = DateTime.Now };
						session.SaveOrUpdate(regularPostUpdateLog);
					}
					else
					{
						OnlinePostUpdateLog regularPostUpdateLog = new OnlinePostUpdateLog() { Model = model, OnlinePostUpdate = new OnlinePostUpdate(postUpdate), LastPublishedDate = DateTime.Now };
						session.SaveOrUpdate(regularPostUpdateLog);
					}
					transaction.Commit();
				}
			}
		}

		private IList<int> getUnpublishedPostUpdatesForModel(Model model, PostUpdateType type)
		{
			using (var session = getSession())
			{
				if (type == PostUpdateType.Regular)
					return session.QueryOver<RegularPostUpdate>().WhereRestrictionOn(pu => pu.Id).Not.IsIn(session.QueryOver<RegularPostUpdateLog>().Where(pul => pul.Model.Id == model.Id).Select(pul => pul.RegularPostUpdate.Id).List<int>().ToArray()).Select(pu => pu.Id).List<int>();
				else
					return session.QueryOver<OnlinePostUpdate>().WhereRestrictionOn(pu => pu.Id).Not.IsIn(session.QueryOver<OnlinePostUpdateLog>().Where(pul => pul.Model.Id == model.Id).Select(pul => pul.OnlinePostUpdate.Id).List<int>().ToArray()).Select(pu => pu.Id).List<int>();
			}
		}
	}
}
