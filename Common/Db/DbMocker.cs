using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Moq;

namespace Switchr.API.Tests.Common
{
	public class DbMocker<T> where T : class
	{
		private MockMoq<DbSet<T>> _dbSet;
		private IQueryable<T> _queryableDataSet;

		public DbMocker(MockMoq<DbSet<T>> dbSet, IQueryable<T> queryableDataSet)
		{
			_dbSet = dbSet;
			_queryableDataSet = queryableDataSet;
		}

		public DbMocker(MockMoq<DbSet<T>> userDbSet, IEnumerable<T> dataSet) : this(userDbSet, dataSet.AsQueryable())
		{
		}

		public void MockDbProp(string propName, string resultAccessName)
		{
			var returnValue = GetQueryValue(resultAccessName);
			_dbSet.As<IQueryable<T>>().OnGetProperty(propName).Returns(returnValue);
		}

		public MockMoq<DbSet<TB>> CreateMockedDb<TB>(IEnumerable<string> list) where TB : class
		{
			return CreateMockedDbSet<TB>(list.ToDictionary(prop => prop));
		}

		public MockMoq<DbSet<TS>> CreateMockedDbSet<TS>(IDictionary<string, string> dict) where TS : class
		{
			var userDbSet = CreateMocked<DbSet<TS>>();
			foreach (var entry in dict)
			{
				var propName = entry.Key;
				var resultPropName = entry.Value;
				MockDbProp(propName, resultPropName);
			}
			return userDbSet;
		}

		// internal

		protected bool IsMethodName(string resultAccessName)
		{
			return resultAccessName[0] == ':';
		}

		protected TP GetPropValue<TP>(object source, string name)
		{
			return (TP) source.GetType().GetProperty(name).GetValue(source, null);
		}

		protected TP GetMethodValue<TP>(object source, string name, object[] args = null)
		{
			return (TP) source.GetType().GetMethod(name).Invoke(source, args);
		}

		protected IQueryProvider GetQueryValue(string name)
		{
			return IsMethodName(name) ? GetQueryMethodValue(name) : GetQueryPropValue(name);
		}

		protected IQueryProvider GetQueryMethodValue(string name)
		{
			return GetMethodValue<IQueryProvider>(_queryableDataSet, name);
		}

		protected IQueryProvider GetQueryPropValue(string name)
		{
			return GetPropValue<IQueryProvider>(_queryableDataSet, name);
		}

		protected MockMoq<TM> CreateMocked<TM>() where TM : class
		{
			return MockMoq<TM>.Create();
		}

	}
}
