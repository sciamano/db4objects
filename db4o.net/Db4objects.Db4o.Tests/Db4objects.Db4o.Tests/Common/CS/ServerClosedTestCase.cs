/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class ServerClosedTestCase : Db4oClientServerTestCase
	{
		public static void Main(string[] args)
		{
			new ServerClosedTestCase().RunAll();
		}

		/// <exception cref="Exception"></exception>
		public virtual void Test()
		{
			if (IsMTOC())
			{
				return;
			}
			IExtObjectContainer db = Fixture().Db();
			ObjectServerImpl serverImpl = (ObjectServerImpl)ClientServerFixture().Server();
			try
			{
				IEnumerator iter = serverImpl.IterateDispatchers();
				iter.MoveNext();
				ServerMessageDispatcherImpl serverDispatcher = (ServerMessageDispatcherImpl)iter.
					Current;
				serverDispatcher.Socket().Close();
				Cool.SleepIgnoringInterruption(1000);
				Assert.Expect(typeof(DatabaseClosedException), new _ICodeBlock_34(this, db));
				Assert.IsTrue(db.IsClosed());
			}
			finally
			{
				serverImpl.Close();
			}
		}

		private sealed class _ICodeBlock_34 : ICodeBlock
		{
			public _ICodeBlock_34(ServerClosedTestCase _enclosing, IExtObjectContainer db)
			{
				this._enclosing = _enclosing;
				this.db = db;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				db.Get(null);
			}

			private readonly ServerClosedTestCase _enclosing;

			private readonly IExtObjectContainer db;
		}
	}
}
