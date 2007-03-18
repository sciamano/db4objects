namespace Db4objects.Db4o.Internal.Freespace
{
	/// <exclude></exclude>
	public sealed class FreeSlotNode : Db4objects.Db4o.Internal.TreeInt
	{
		internal static int sizeLimit;

		internal Db4objects.Db4o.Internal.Freespace.FreeSlotNode _peer;

		internal FreeSlotNode(int a_key) : base(a_key)
		{
		}

		public override object ShallowClone()
		{
			Db4objects.Db4o.Internal.Freespace.FreeSlotNode frslot = new Db4objects.Db4o.Internal.Freespace.FreeSlotNode
				(_key);
			frslot._peer = _peer;
			return base.ShallowCloneInternal(frslot);
		}

		internal void CreatePeer(int a_key)
		{
			_peer = new Db4objects.Db4o.Internal.Freespace.FreeSlotNode(a_key);
			_peer._peer = this;
		}

		public override bool Duplicates()
		{
			return true;
		}

		public sealed override int OwnLength()
		{
			return Db4objects.Db4o.Internal.Const4.INT_LENGTH * 2;
		}

		internal static Db4objects.Db4o.Foundation.Tree RemoveGreaterOrEqual(Db4objects.Db4o.Internal.Freespace.FreeSlotNode
			 a_in, Db4objects.Db4o.Internal.TreeIntObject a_finder)
		{
			if (a_in == null)
			{
				return null;
			}
			int cmp = a_in._key - a_finder._key;
			if (cmp == 0)
			{
				a_finder._object = a_in;
				return a_in.Remove();
			}
			if (cmp > 0)
			{
				a_in._preceding = RemoveGreaterOrEqual((Db4objects.Db4o.Internal.Freespace.FreeSlotNode
					)a_in._preceding, a_finder);
				if (a_finder._object != null)
				{
					a_in._size--;
					return a_in;
				}
				a_finder._object = a_in;
				return a_in.Remove();
			}
			a_in._subsequent = RemoveGreaterOrEqual((Db4objects.Db4o.Internal.Freespace.FreeSlotNode
				)a_in._subsequent, a_finder);
			if (a_finder._object != null)
			{
				a_in._size--;
			}
			return a_in;
		}

		public override object Read(Db4objects.Db4o.Internal.Buffer buffer)
		{
			int size = buffer.ReadInt();
			int address = buffer.ReadInt();
			if (size > sizeLimit)
			{
				Db4objects.Db4o.Internal.Freespace.FreeSlotNode node = new Db4objects.Db4o.Internal.Freespace.FreeSlotNode
					(size);
				node.CreatePeer(address);
				return node;
			}
			return null;
		}

		private void DebugCheckBuffer(Db4objects.Db4o.Internal.Buffer buffer, Db4objects.Db4o.Internal.Freespace.FreeSlotNode
			 node)
		{
			if (!(buffer is Db4objects.Db4o.Internal.StatefulBuffer))
			{
				return;
			}
			Db4objects.Db4o.Internal.Transaction trans = ((Db4objects.Db4o.Internal.StatefulBuffer
				)buffer).GetTransaction();
			if (!(trans.Stream() is Db4objects.Db4o.Internal.IoAdaptedObjectContainer))
			{
				return;
			}
			Db4objects.Db4o.Internal.StatefulBuffer checker = trans.Stream().GetWriter(trans, 
				node._peer._key, node._key);
			try
			{
				checker.Read();
				for (int i = 0; i < node._key; i++)
				{
					if (checker.ReadByte() != (byte)'X')
					{
						Sharpen.Runtime.Out.WriteLine("!!! Free space corruption at:" + node._peer._key);
						break;
					}
				}
			}
			catch (System.IO.IOException e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
			}
		}

		public sealed override void Write(Db4objects.Db4o.Internal.Buffer a_writer)
		{
			a_writer.WriteInt(_key);
			a_writer.WriteInt(_peer._key);
		}

		public override string ToString()
		{
			return base.ToString();
			string str = "FreeSlotNode " + _key;
			if (_peer != null)
			{
				str += " peer: " + _peer._key;
			}
			return str;
		}
	}
}
