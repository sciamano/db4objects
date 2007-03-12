namespace com.db4o.@internal.btree
{
	/// <exclude></exclude>
	public class BTreePointer
	{
		public static com.db4o.@internal.btree.BTreePointer Max(com.db4o.@internal.btree.BTreePointer
			 x, com.db4o.@internal.btree.BTreePointer y)
		{
			if (x == null)
			{
				return x;
			}
			if (y == null)
			{
				return y;
			}
			if (x.CompareTo(y) > 0)
			{
				return x;
			}
			return y;
		}

		public static com.db4o.@internal.btree.BTreePointer Min(com.db4o.@internal.btree.BTreePointer
			 x, com.db4o.@internal.btree.BTreePointer y)
		{
			if (x == null)
			{
				return y;
			}
			if (y == null)
			{
				return x;
			}
			if (x.CompareTo(y) < 0)
			{
				return x;
			}
			return y;
		}

		private readonly com.db4o.@internal.btree.BTreeNode _node;

		private readonly int _index;

		private readonly com.db4o.@internal.Transaction _transaction;

		private readonly com.db4o.@internal.Buffer _nodeReader;

		public BTreePointer(com.db4o.@internal.Transaction transaction, com.db4o.@internal.Buffer
			 nodeReader, com.db4o.@internal.btree.BTreeNode node, int index)
		{
			if (transaction == null || node == null)
			{
				throw new System.ArgumentNullException();
			}
			_transaction = transaction;
			_nodeReader = nodeReader;
			_node = node;
			_index = index;
		}

		public com.db4o.@internal.Transaction Transaction()
		{
			return _transaction;
		}

		public int Index()
		{
			return _index;
		}

		public com.db4o.@internal.btree.BTreeNode Node()
		{
			return _node;
		}

		public object Key()
		{
			return Node().Key(Transaction(), NodeReader(), Index());
		}

		private com.db4o.@internal.Buffer NodeReader()
		{
			return _nodeReader;
		}

		public virtual com.db4o.@internal.btree.BTreePointer Next()
		{
			int indexInMyNode = Index() + 1;
			while (indexInMyNode < Node().Count())
			{
				if (Node().IndexIsValid(Transaction(), indexInMyNode))
				{
					return new com.db4o.@internal.btree.BTreePointer(Transaction(), NodeReader(), Node
						(), indexInMyNode);
				}
				indexInMyNode++;
			}
			int newIndex = -1;
			com.db4o.@internal.btree.BTreeNode nextNode = Node();
			com.db4o.@internal.Buffer nextReader = null;
			while (newIndex == -1)
			{
				nextNode = nextNode.NextNode();
				if (nextNode == null)
				{
					return null;
				}
				nextReader = nextNode.PrepareRead(Transaction());
				newIndex = nextNode.FirstKeyIndex(Transaction());
			}
			return new com.db4o.@internal.btree.BTreePointer(Transaction(), nextReader, nextNode
				, newIndex);
		}

		public virtual com.db4o.@internal.btree.BTreePointer Previous()
		{
			int indexInMyNode = Index() - 1;
			while (indexInMyNode >= 0)
			{
				if (Node().IndexIsValid(Transaction(), indexInMyNode))
				{
					return new com.db4o.@internal.btree.BTreePointer(Transaction(), NodeReader(), Node
						(), indexInMyNode);
				}
				indexInMyNode--;
			}
			int newIndex = -1;
			com.db4o.@internal.btree.BTreeNode previousNode = Node();
			com.db4o.@internal.Buffer previousReader = null;
			while (newIndex == -1)
			{
				previousNode = previousNode.PreviousNode();
				if (previousNode == null)
				{
					return null;
				}
				previousReader = previousNode.PrepareRead(Transaction());
				newIndex = previousNode.LastKeyIndex(Transaction());
			}
			return new com.db4o.@internal.btree.BTreePointer(Transaction(), previousReader, previousNode
				, newIndex);
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (!(obj is com.db4o.@internal.btree.BTreePointer))
			{
				return false;
			}
			com.db4o.@internal.btree.BTreePointer other = (com.db4o.@internal.btree.BTreePointer
				)obj;
			if (Index() != other.Index())
			{
				return false;
			}
			return Node().Equals(other.Node());
		}

		public override int GetHashCode()
		{
			return Node().GetHashCode();
		}

		public override string ToString()
		{
			return "BTreePointer(index=" + Index() + ", node=" + Node() + ")";
		}

		public virtual int CompareTo(com.db4o.@internal.btree.BTreePointer y)
		{
			if (null == y)
			{
				throw new System.ArgumentNullException();
			}
			if (Btree() != y.Btree())
			{
				throw new System.ArgumentException();
			}
			return Btree().CompareKeys(Key(), y.Key());
		}

		private com.db4o.@internal.btree.BTree Btree()
		{
			return Node().Btree();
		}

		public static bool LessThan(com.db4o.@internal.btree.BTreePointer x, com.db4o.@internal.btree.BTreePointer
			 y)
		{
			return com.db4o.@internal.btree.BTreePointer.Min(x, y) == x && !Equals(x, y);
		}

		public static bool Equals(com.db4o.@internal.btree.BTreePointer x, com.db4o.@internal.btree.BTreePointer
			 y)
		{
			if (x == null)
			{
				return y == null;
			}
			return x.Equals(y);
		}

		public virtual bool IsValid()
		{
			return Node().IndexIsValid(Transaction(), Index());
		}
	}
}
