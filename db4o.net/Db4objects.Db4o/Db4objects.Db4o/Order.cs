namespace Db4objects.Db4o
{
	internal class Order : Db4objects.Db4o.IOrderable
	{
		private int i_major;

		private int i_minor;

		public virtual int CompareTo(object obj)
		{
			if (obj is Db4objects.Db4o.Order)
			{
				Db4objects.Db4o.Order other = (Db4objects.Db4o.Order)obj;
				int res = other.i_major - i_major;
				if (res != 0)
				{
					return res;
				}
				return other.i_minor - i_minor;
			}
			return 1;
		}

		public virtual void HintOrder(int a_order, bool a_major)
		{
			if (a_major)
			{
				i_major = a_order;
			}
			else
			{
				i_minor = a_order;
			}
		}

		public virtual bool HasDuplicates()
		{
			return true;
		}
	}
}
