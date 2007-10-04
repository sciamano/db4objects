/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	public class PrimitiveMarshaller1 : PrimitiveMarshaller
	{
		public override bool UseNormalClassRead()
		{
			return false;
		}

		public override DateTime ReadDate(Db4objects.Db4o.Internal.Buffer bytes)
		{
			return new DateTime(bytes.ReadLong());
		}

		public override object ReadInteger(Db4objects.Db4o.Internal.Buffer bytes)
		{
			return bytes.ReadInt();
		}

		public override object ReadFloat(Db4objects.Db4o.Internal.Buffer bytes)
		{
			return PrimitiveMarshaller0.UnmarshallFloat(bytes);
		}

		public override object ReadDouble(Db4objects.Db4o.Internal.Buffer buffer)
		{
			return PrimitiveMarshaller0.UnmarshalDouble(buffer);
		}

		public override object ReadLong(Db4objects.Db4o.Internal.Buffer buffer)
		{
			return buffer.ReadLong();
		}

		public override object ReadShort(Db4objects.Db4o.Internal.Buffer buffer)
		{
			return PrimitiveMarshaller0.UnmarshallShort(buffer);
		}
	}
}
