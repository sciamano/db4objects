﻿/* Copyright (C) 2004 - 2009  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using System.Collections.Generic;
using Db4objects.Db4o.Reflect.Net;
using NUnit.Framework;
using OManager.DataLayer.Reflection;
using Sharpen.Lang;
using Type=System.Type;

namespace OMNUnitTest
{
	[TestFixture]
	public class ReflectionTestCase
	{
		private TypeResolver _resolver;

		[SetUp]
		public void Setup()
		{
			_resolver = new TypeResolver(new NetReflector());
		}

		[Test]
		public void TestResolveToSameTypeReference()
		{
			Assert.AreSame(Resolve(typeof(int)), Resolve(typeof(int)));
		}

		[Test]
		public void TestIsEditable()
		{
			Assert.IsTrue(Resolve(typeof(int)).IsEditable);
			Assert.IsTrue(Resolve(typeof(string)).IsEditable);
			Assert.IsTrue(Resolve(typeof(bool)).IsEditable);

			Assert.IsFalse(Resolve(typeof(List<int>)).IsEditable);
			Assert.IsFalse(Resolve(typeof(ReflectionTestCase)).IsEditable);
			Assert.IsFalse(Resolve(typeof(object)).IsEditable);
		}

		[Test]
		public void TestIsPrimitive()
		{
			Assert.IsTrue(Resolve(typeof(int)).IsPrimitive);
			Assert.IsTrue(Resolve(typeof(string)).IsPrimitive);
			Assert.IsTrue(Resolve(typeof(bool)).IsPrimitive);
			Assert.IsTrue(Resolve(typeof(decimal)).IsPrimitive);

			Assert.IsFalse(Resolve(typeof(ReflectionTestCase)).IsPrimitive);
			Assert.IsFalse(Resolve(typeof(object)).IsPrimitive);
		}

		[Test]
		public void TestIsCollection()
		{
			Assert.IsTrue(Resolve(typeof(ArrayList)).IsCollection);
			Assert.IsTrue(Resolve(typeof(IList)).IsCollection);
			
			Assert.IsFalse(Resolve(typeof(int[])).IsCollection);
			Assert.IsFalse(Resolve(typeof(string)).IsCollection);
			Assert.IsFalse(Resolve(typeof(ReflectionTestCase)).IsCollection);
		}

		[Test]
		public void TestIsArray()
		{
			Assert.IsTrue(Resolve(typeof(int[])).IsArray);
			Assert.IsTrue(Resolve(typeof(int[,])).IsArray);
			Assert.IsTrue(Resolve(typeof(ReflectionTestCase[][])).IsArray);

			Assert.IsFalse(Resolve(typeof(ArrayList)).IsArray);
			Assert.IsFalse(Resolve(typeof(string)).IsArray);
			Assert.IsFalse(Resolve(typeof(ReflectionTestCase)).IsArray);
			Assert.IsFalse(Resolve(typeof(IList)).IsArray);
		}

		[Test]
		public void TestIsNullable()
		{
			Assert.IsTrue(Resolve(typeof(int?)).IsNullable);
			//Assert.IsTrue(Resolve(typeof(EnumTest?)).IsNullable);
			//Assert.IsTrue(Resolve(typeof(DateTime?)).IsNullable);

			Assert.IsFalse(Resolve(FakeNullableType()).IsNullable);
			Assert.IsFalse(Resolve(typeof(int?[])).IsNullable);
			Assert.IsFalse(Resolve(typeof(ArrayList)).IsNullable);
			Assert.IsFalse(Resolve(typeof(string)).IsNullable);
		}

		[Test]
		public void TestDisplayName()
		{
			Assert.AreEqual("Nullable<System.Int32>", Resolve(typeof(int?)).DisplayName);
			Assert.AreEqual("System.Int32", Resolve(typeof(int)).DisplayName);
			Assert.AreEqual("System.Int32[]", Resolve(typeof(int[])).DisplayName);
			Assert.AreEqual("OMNUnitTest.ReflectionTestCase", Resolve(typeof(ReflectionTestCase)).DisplayName);
			Assert.AreEqual("OMNUnitTest.ReflectionTestCase+Item", typeof(Item).NewGenericType().DisplayName);
		}

		[Test]
		public void TestFullName()
		{
			Assert.AreEqual(UnversionedNameFor<int?>(), Resolve(typeof(int?)).FullName);
			Assert.AreEqual(UnversionedNameFor<int[]>(), Resolve(typeof(int[])).FullName);
			Assert.AreEqual(UnversionedNameFor<int>(), Resolve(typeof(int)).FullName);
			Assert.AreEqual(UnversionedNameFor<ReflectionTestCase>(), Resolve(typeof(ReflectionTestCase)).FullName);
			Assert.AreEqual(UnversionedNameFor<Item>(), typeof(Item).NewGenericType().FullName);			
		}

		[Test]
		public void TestCast()
		{
			Assert.AreEqual(10, Resolve(typeof (int)).Cast("10"));
			Assert.AreEqual(10, Resolve(typeof(int)).Cast(10.0));
			Assert.AreEqual(true, Resolve(typeof(bool)).Cast("true"));
			object trueValue = true;
			Assert.AreEqual(true, Resolve(typeof(bool)).Cast(trueValue));
			Assert.AreEqual(42, Resolve(typeof(int?)).Cast(42));
			Assert.AreEqual(42, Resolve(typeof(int?)).Cast(42.0));
			Assert.AreEqual(42, Resolve(typeof(int?)).Cast("42"));
		}

		[Test]
		public void TestHasIdentity()
		{
			Assert.IsTrue(Resolve(typeof (object)).HasIdentity);
			Assert.IsTrue(Resolve(typeof(List<int>)).HasIdentity);
			Assert.IsTrue(Resolve(typeof(IList<int>)).HasIdentity);
			Assert.IsTrue(Resolve(typeof(Item)).HasIdentity);
			
			Assert.IsFalse(Resolve(typeof(string)).HasIdentity);
			Assert.IsFalse(Resolve(typeof(byte)).HasIdentity);
			Assert.IsFalse(Resolve(typeof(byte?)).HasIdentity);
			Assert.IsFalse(Resolve(typeof(SimpleStruct)).HasIdentity);
		}

		private static string UnversionedNameFor<T>()
		{
			return TypeReference.FromType(typeof(T)).GetUnversionedName();
		}

		private static Type FakeNullableType()
		{
			return typeof(Nullable<int>);
		}

		private IType Resolve(Type type)
		{
			return _resolver.Resolve(TypeReference.FromType(type).GetUnversionedName());
		}

		public class Item
		{
		}

		public class Nullable<T>
		{
			public T _dummy;
		}

		private enum EnumTest
		{
			First,
			Second
		}
	}

	internal struct SimpleStruct
	{
	}
}