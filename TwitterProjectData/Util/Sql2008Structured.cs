using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace TwitterProjectData.Util
{
	public class Sql2008Structured : IType
	{
		private static readonly SqlType[] x = new[] { new SqlType(DbType.Object) };
		public SqlType[] SqlTypes(NHibernate.Engine.IMapping mapping)
		{
			return x;
		}

		public int Compare(object x, object y, NHibernate.EntityMode? entityMode)
		{
			throw new NotImplementedException();
		}

		public object DeepCopy(object val, NHibernate.EntityMode entityMode, NHibernate.Engine.ISessionFactoryImplementor factory)
		{
			throw new NotImplementedException();
		}

		public object FromXMLNode(System.Xml.XmlNode xml, NHibernate.Engine.IMapping factory)
		{
			throw new NotImplementedException();
		}

		public int GetColumnSpan(NHibernate.Engine.IMapping mapping)
		{
			return 1;
		}

		public int GetHashCode(object x, NHibernate.EntityMode entityMode, NHibernate.Engine.ISessionFactoryImplementor factory)
		{
			throw new NotImplementedException();
		}

		public int GetHashCode(object x, NHibernate.EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		public IType GetSemiResolvedType(NHibernate.Engine.ISessionFactoryImplementor factory)
		{
			throw new NotImplementedException();
		}

		public object Hydrate(System.Data.IDataReader rs, string[] names, NHibernate.Engine.ISessionImplementor session, object owner)
		{
			throw new NotImplementedException();
		}

		public bool IsAnyType
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsAssociationType
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsCollectionType
		{
			get { return true; }
		}

		public bool IsComponentType
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsDirty(object old, object current, bool[] checkable, NHibernate.Engine.ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public bool IsDirty(object old, object current, NHibernate.Engine.ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public bool IsEntityType
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsEqual(object x, object y, NHibernate.EntityMode entityMode, NHibernate.Engine.ISessionFactoryImplementor factory)
		{
			throw new NotImplementedException();
		}

		public bool IsEqual(object x, object y, NHibernate.EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		public bool IsModified(object oldHydratedState, object currentState, bool[] checkable, NHibernate.Engine.ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public bool IsMutable
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsSame(object x, object y, NHibernate.EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		public bool IsXMLElement
		{
			get { throw new NotImplementedException(); }
		}

		public string Name
		{
			get { throw new NotImplementedException(); }
		}

		public object NullSafeGet(System.Data.IDataReader rs, string name, NHibernate.Engine.ISessionImplementor session, object owner)
		{
			throw new NotImplementedException();
		}

		public object NullSafeGet(System.Data.IDataReader rs, string[] names, NHibernate.Engine.ISessionImplementor session, object owner)
		{
			throw new NotImplementedException();
		}

		public void NullSafeSet(System.Data.IDbCommand st, object value, int index, NHibernate.Engine.ISessionImplementor session)
		{
			var s = st as SqlCommand;
			if (s != null)
			{
				s.Parameters[index].SqlDbType = SqlDbType.Structured;
				s.Parameters[index].TypeName = "UserNamesType";
				s.Parameters[index].Value = value;
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		public void NullSafeSet(System.Data.IDbCommand st, object value, int index, bool[] settable, NHibernate.Engine.ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public object Replace(object original, object target, NHibernate.Engine.ISessionImplementor session, object owner, System.Collections.IDictionary copyCache, ForeignKeyDirection foreignKeyDirection)
		{
			throw new NotImplementedException();
		}

		public object Replace(object original, object target, NHibernate.Engine.ISessionImplementor session, object owner, System.Collections.IDictionary copiedAlready)
		{
			throw new NotImplementedException();
		}

		public object ResolveIdentifier(object value, NHibernate.Engine.ISessionImplementor session, object owner)
		{
			throw new NotImplementedException();
		}

		public Type ReturnedClass
		{
			get { throw new NotImplementedException(); }
		}

		public object SemiResolve(object value, NHibernate.Engine.ISessionImplementor session, object owner)
		{
			throw new NotImplementedException();
		}

		public void SetToXMLNode(System.Xml.XmlNode node, object value, NHibernate.Engine.ISessionFactoryImplementor factory)
		{
			throw new NotImplementedException();
		}

		public bool[] ToColumnNullness(object value, NHibernate.Engine.IMapping mapping)
		{
			throw new NotImplementedException();
		}

		public string ToLoggableString(object value, NHibernate.Engine.ISessionFactoryImplementor factory)
		{
			throw new NotImplementedException();
		}

		public object Assemble(object cached, NHibernate.Engine.ISessionImplementor session, object owner)
		{
			throw new NotImplementedException();
		}

		public void BeforeAssemble(object cached, NHibernate.Engine.ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public object Disassemble(object value, NHibernate.Engine.ISessionImplementor session, object owner)
		{
			throw new NotImplementedException();
		}
	}

	public static class StructuredExtensions
	{
		private static readonly Sql2008Structured structured = new Sql2008Structured();

		public static IQuery SetStructured(this IQuery query, string name, DataTable dt)
		{
			return query.SetParameter(name, dt, structured);
		}
	}
}
