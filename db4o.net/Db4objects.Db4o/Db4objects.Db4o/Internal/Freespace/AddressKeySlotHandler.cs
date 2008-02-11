/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Freespace
{
	/// <exclude></exclude>
	public class AddressKeySlotHandler : SlotHandler
	{
		public virtual int CompareTo(object obj)
		{
			return _current.CompareByAddress((Slot)obj);
		}

		public override IPreparedComparison PrepareComparison(object slot)
		{
			Slot sourceSlot = (Slot)slot;
			return new _IPreparedComparison_20(this, sourceSlot);
		}

		private sealed class _IPreparedComparison_20 : IPreparedComparison
		{
			public _IPreparedComparison_20(AddressKeySlotHandler _enclosing, Slot sourceSlot)
			{
				this._enclosing = _enclosing;
				this.sourceSlot = sourceSlot;
			}

			public int CompareTo(object obj)
			{
				Slot targetSlot = (Slot)obj;
				// FIXME: The comparison method in #compareByAddress is the wrong way around.
				// Fix there and here after other references are fixed.
				return -sourceSlot.CompareByAddress(targetSlot);
			}

			private readonly AddressKeySlotHandler _enclosing;

			private readonly Slot sourceSlot;
		}
	}
}
