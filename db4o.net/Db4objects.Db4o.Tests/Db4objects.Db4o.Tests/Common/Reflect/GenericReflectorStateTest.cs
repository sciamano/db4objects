/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.Common.Reflect
{
	public class GenericReflectorStateTest : AbstractDb4oTestCase
	{
		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
		}

		public virtual void TestKnownClasses()
		{
			Db().Reflector().KnownClasses();
		}
	}
}
