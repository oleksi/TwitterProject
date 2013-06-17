using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using TwitterProjectModel;
using TwitterProjectData.Util;

namespace TwitterProjectData
{
	public class FriendFinderRepository
	{
		private static ISession getSession()
		{
			return SessionProvider.SessionFactory.OpenSession();
		}

		public MonitorUser GetMonitorUserById(int id)
		{
			using (var session = getSession())
			{
				return session.Get<MonitorUser>(id);
			}
		}

		public MonitorUser GetNextMonitorUser()
		{
			using (var session = getSession())
			{
				var result = session.QueryOver<MonitorUser>().Where( mu => mu.IsActive == true).OrderBy(mu => mu.LastMonitorDate).Asc.Take(1).List();

				if (result.Count > 0)
					return result[0];
				else
					return null;
			}
		}

		public void SaveMonitorUser(MonitorUser monitorUser)
		{
			using (var session = getSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					session.SaveOrUpdate(monitorUser);
					transaction.Commit();
				}
			}
		}

		public void AddFriendProspects(List<FriendProspect> friendProspects, MonitorUser referredBy)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("UserName", typeof(string));
			friendProspects.ForEach(mu => dt.Rows.Add(mu.UserName));

			using (var session = getSession())
			{
				ISQLQuery sqlQuery = session.CreateSQLQuery("EXEC usp_AddFriendProspects @UserNames = :UserNames, @ReferredById = :ReferredById");
				sqlQuery.SetStructured("UserNames", dt);
				sqlQuery.SetParameter("ReferredById", referredBy.Id);
				sqlQuery.ExecuteUpdate();
			}
		}
	}
}
