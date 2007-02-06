namespace Db4oUnit.Extensions
{
	public class AbstractDb4oTestCase : Db4oUnit.Extensions.Db4oTestCase
	{
		[System.NonSerialized]
		private Db4oUnit.Extensions.Db4oFixture _fixture;

		public virtual void Fixture(Db4oUnit.Extensions.Db4oFixture fixture)
		{
			_fixture = fixture;
		}

		public virtual Db4oUnit.Extensions.Db4oFixture Fixture()
		{
			return _fixture;
		}

		public virtual bool IsClientServer()
		{
			return Fixture() is Db4oUnit.Extensions.Fixtures.AbstractClientServerDb4oFixture;
		}

		protected virtual void Reopen()
		{
			_fixture.Reopen();
		}

		public void SetUp()
		{
			_fixture.Clean();
			Configure(_fixture.Config());
			_fixture.Open();
			Db4oSetupBeforeStore();
			Store();
			_fixture.Db().Commit();
			_fixture.Close();
			_fixture.Open();
			Db4oSetupAfterStore();
		}

		public void TearDown()
		{
			Db4oCustomTearDown();
			_fixture.Close();
			_fixture.Clean();
		}

		protected virtual void Db4oSetupBeforeStore()
		{
		}

		protected virtual void Db4oSetupAfterStore()
		{
		}

		protected virtual void Db4oCustomTearDown()
		{
		}

		protected virtual void Configure(com.db4o.config.Configuration config)
		{
		}

		protected virtual void Store()
		{
		}

		public virtual com.db4o.ext.ExtObjectContainer Db()
		{
			return Fixture().Db();
		}

		protected virtual System.Type[] TestCases()
		{
			return new System.Type[] { GetType() };
		}

		public virtual int RunSoloAndClientServer()
		{
			return RunSoloAndClientServer(true);
		}

		private int RunSoloAndClientServer(bool independentConfig)
		{
			return new Db4oUnit.TestRunner(new Db4oUnit.TestSuite(new Db4oUnit.Test[] { SoloSuite
				(independentConfig).Build(), ClientServerSuite(independentConfig).Build() })).Run
				();
		}

		public virtual int RunSolo()
		{
			return RunSolo(true);
		}

		public virtual int RunSolo(bool independentConfig)
		{
			return new Db4oUnit.TestRunner(SoloSuite(independentConfig)).Run();
		}

		public virtual int RunClientServer()
		{
			return RunClientServer(true);
		}

		public virtual int RunClientServer(bool independentConfig)
		{
			return new Db4oUnit.TestRunner(ClientServerSuite(independentConfig)).Run();
		}

		private Db4oUnit.Extensions.Db4oTestSuiteBuilder SoloSuite(bool independentConfig
			)
		{
			return new Db4oUnit.Extensions.Db4oTestSuiteBuilder(new Db4oUnit.Extensions.Fixtures.Db4oSolo
				(ConfigSource(independentConfig)), TestCases());
		}

		private Db4oUnit.Extensions.Db4oTestSuiteBuilder ClientServerSuite(bool independentConfig
			)
		{
			return new Db4oUnit.Extensions.Db4oTestSuiteBuilder(new Db4oUnit.Extensions.Fixtures.Db4oSingleClient
				(ConfigSource(independentConfig)), TestCases());
		}

		private Db4oUnit.Extensions.Fixtures.ConfigurationSource ConfigSource(bool independentConfig
			)
		{
			return (independentConfig ? (Db4oUnit.Extensions.Fixtures.ConfigurationSource)new 
				Db4oUnit.Extensions.Fixtures.IndependentConfigurationSource() : new Db4oUnit.Extensions.Fixtures.GlobalConfigurationSource
				());
		}

		protected virtual com.db4o.@internal.ObjectContainerBase Stream()
		{
			return (com.db4o.@internal.ObjectContainerBase)Db();
		}

		protected virtual com.db4o.@internal.LocalObjectContainer FileSession()
		{
			return Fixture().FileSession();
		}

		protected virtual com.db4o.@internal.Transaction Trans()
		{
			return Stream().GetTransaction();
		}

		protected virtual com.db4o.@internal.Transaction SystemTrans()
		{
			return Stream().GetSystemTransaction();
		}

		protected virtual com.db4o.query.Query NewQuery(com.db4o.@internal.Transaction transaction
			, System.Type clazz)
		{
			com.db4o.query.Query query = NewQuery(transaction);
			query.Constrain(clazz);
			return query;
		}

		protected virtual com.db4o.query.Query NewQuery(com.db4o.@internal.Transaction transaction
			)
		{
			return Stream().Query(transaction);
		}

		protected virtual com.db4o.query.Query NewQuery()
		{
			return Db().Query();
		}

		protected virtual com.db4o.reflect.Reflector Reflector()
		{
			return Stream().Reflector();
		}

		protected virtual void IndexField(com.db4o.config.Configuration config, System.Type
			 clazz, string fieldName)
		{
			config.ObjectClass(clazz).ObjectField(fieldName).Indexed(true);
		}

		protected virtual com.db4o.@internal.Transaction NewTransaction()
		{
			return Stream().NewTransaction();
		}

		protected virtual com.db4o.query.Query NewQuery(System.Type clazz)
		{
			com.db4o.query.Query query = NewQuery();
			query.Constrain(clazz);
			return query;
		}

		protected virtual object RetrieveOnlyInstance(System.Type clazz)
		{
			com.db4o.ObjectSet result = NewQuery(clazz).Execute();
			Db4oUnit.Assert.AreEqual(1, result.Size());
			return result.Next();
		}

		protected virtual int CountOccurences(System.Type clazz)
		{
			com.db4o.ObjectSet result = NewQuery(clazz).Execute();
			return result.Size();
		}

		protected virtual void Foreach(System.Type clazz, com.db4o.foundation.Visitor4 visitor
			)
		{
			com.db4o.ext.ExtObjectContainer oc = Db();
			oc.Deactivate(clazz, int.MaxValue);
			com.db4o.ObjectSet set = NewQuery(clazz).Execute();
			while (set.HasNext())
			{
				visitor.Visit(set.Next());
			}
		}

		protected virtual void DeleteAll(System.Type clazz)
		{
			Foreach(clazz, new _AnonymousInnerClass192(this));
		}

		private sealed class _AnonymousInnerClass192 : com.db4o.foundation.Visitor4
		{
			public _AnonymousInnerClass192(AbstractDb4oTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object obj)
			{
				this._enclosing.Db().Delete(obj);
			}

			private readonly AbstractDb4oTestCase _enclosing;
		}

		protected void Store(object obj)
		{
			Db().Set(obj);
		}

		protected virtual com.db4o.reflect.ReflectClass ReflectClass(System.Type clazz)
		{
			return Db4oUnit.Extensions.Db4oUnitPlatform.GetReflectClass(Reflector(), clazz);
		}
	}
}
