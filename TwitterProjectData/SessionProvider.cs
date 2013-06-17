using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace TwitterProjectData
{
	public class SessionProvider
	{
		private static ISessionFactory sessionFactory;
		private static Configuration config;

		public static ISessionFactory SessionFactory
		{
			get
			{
				if (sessionFactory == null)
					sessionFactory = createSessionFactory();

				return sessionFactory;
			}
		}

		public static Configuration Config
		{
			get
			{
				if (config == null)
				{
					config = new Configuration();
					config.Configure();
					config.AddAssembly(Assembly.GetCallingAssembly());
				}

				return config;
			}
		}

		private static ISessionFactory createSessionFactory()
		{
			return Config.BuildSessionFactory();
		}

		public static void RebuildSchema()
		{
			var schema = new SchemaExport(Config);
			schema.Create(true, true);
		}
	}
}
