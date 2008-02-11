/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Tests.Common.TA;
using Db4objects.Db4o.Tests.Common.TA.TA;

namespace Db4objects.Db4o.Tests.Common.TA.TA
{
	/// <exclude></exclude>
	public class TANArrayTestCase : TAItemTestCaseBase
	{
		private static readonly int[][] Ints1 = new int[][] { new int[] { 1, 2, 3 }, new 
			int[] { 4, 5, 6 } };

		private static readonly int[][] Ints2 = new int[][] { new int[] { 4, 5, 6 }, new 
			int[] { 7, 8, 9 } };

		private static readonly LinkedList[][] List1 = new LinkedList[][] { new LinkedList
			[] { LinkedList.NewList(5) }, new LinkedList[] { LinkedList.NewList(5) } };

		private static readonly LinkedList[][] List2 = new LinkedList[][] { new LinkedList
			[] { LinkedList.NewList(5) }, new LinkedList[] { LinkedList.NewList(5) } };

		public static void Main(string[] args)
		{
			new TANArrayTestCase().RunAll();
		}

		/// <exception cref="Exception"></exception>
		protected override object CreateItem()
		{
			TANArrayItem item = new TANArrayItem();
			item.value = Ints1;
			item.obj = Ints2;
			item.lists = List1;
			item.listsObject = List2;
			return item;
		}

		/// <exception cref="Exception"></exception>
		protected override void AssertItemValue(object obj)
		{
			TANArrayItem item = (TANArrayItem)obj;
			ArrayAssert.AreEqual(Ints1, item.Value());
			ArrayAssert.AreEqual(Ints2, (int[][])item.Object());
			ArrayAssert.AreEqual(List1, item.Lists());
			ArrayAssert.AreEqual(List2, (LinkedList[][])item.ListsObject());
		}

		/// <exception cref="Exception"></exception>
		protected override void AssertRetrievedItem(object obj)
		{
			TANArrayItem item = (TANArrayItem)obj;
			Assert.IsNull(item.value);
			Assert.IsNull(item.obj);
			Assert.IsNull(item.lists);
			Assert.IsNull(item.listsObject);
		}
	}
}
