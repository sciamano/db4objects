/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.Internal;

namespace Db4objects.Db4o.Tests.Common.Internal
{
	public class SerializerTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new SerializerTestCase().RunAll();
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestMarshall()
		{
			ReflectException e = new ReflectException(new ArgumentNullException());
			SerializedGraph marshalled = Serializer.Marshall(Stream().Container(), e);
			Assert.IsTrue(marshalled.Length() > 0);
		}
	}
}
