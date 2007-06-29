/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers
{
	public sealed class ByteHandler : PrimitiveHandler
	{
		internal const int LENGTH = 1 + Const4.ADDED_LENGTH;

		private static readonly byte DEFAULT_VALUE = (byte)0;

		public ByteHandler(ObjectContainerBase stream) : base(stream)
		{
		}

		public override object Coerce(IReflectClass claxx, object obj)
		{
			return Coercion4.ToSByte(obj);
		}

		public override int GetID()
		{
			return 6;
		}

		public override object DefaultValue()
		{
			return DEFAULT_VALUE;
		}

		public override int LinkLength()
		{
			return LENGTH;
		}

		protected override Type PrimitiveJavaClass()
		{
			return typeof(byte);
		}

		public override object PrimitiveNull()
		{
			return DEFAULT_VALUE;
		}

		internal override object Read1(Db4objects.Db4o.Internal.Buffer a_bytes)
		{
			byte ret = a_bytes.ReadByte();
			return ret;
		}

		public override void Write(object a_object, Db4objects.Db4o.Internal.Buffer a_bytes
			)
		{
			a_bytes.Append(((byte)a_object));
		}

		private byte i_compareTo;

		private byte Val(object obj)
		{
			return ((byte)obj);
		}

		internal override void PrepareComparison1(object obj)
		{
			i_compareTo = Val(obj);
		}

		internal override bool IsEqual1(object obj)
		{
			return obj is byte && Val(obj) == i_compareTo;
		}

		internal override bool IsGreater1(object obj)
		{
			return obj is byte && Val(obj) > i_compareTo;
		}

		internal override bool IsSmaller1(object obj)
		{
			return obj is byte && Val(obj) < i_compareTo;
		}
	}
}
