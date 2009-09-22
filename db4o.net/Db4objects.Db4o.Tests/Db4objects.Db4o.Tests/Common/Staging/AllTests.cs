/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Staging;

namespace Db4objects.Db4o.Tests.Common.Staging
{
	public class AllTests : ComposibleTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Staging.AllTests().RunSolo();
		}

		protected override Type[] TestCases()
		{
			return ComposeTests(new Type[] { typeof(ActivateDepthTestCase), typeof(InterfaceQueryTestCase
				), typeof(LazyQueryDeleteTestCase), typeof(SODAClassTypeDescend), typeof(StoredClassUnknownClassQueryTestCase
				) });
		}

		// COR-1131
		#if !SILVERLIGHT
		protected override Type[] ComposeWith()
		{
			return new Type[] { typeof(ClientServerPingTestCase), typeof(DeepPrefetchingCacheConcurrencyTestCase
				), typeof(PingTestCase) };
		}
		#endif // !SILVERLIGHT
		// COR-1762
	}
}
