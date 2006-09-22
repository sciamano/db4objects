namespace com.db4o
{
	internal class PendingClassInits
	{
		private readonly com.db4o.Transaction _systemTransaction;

		private com.db4o.foundation.Collection4 _pending = new com.db4o.foundation.Collection4
			();

		private com.db4o.foundation.Queue4 _members = new com.db4o.foundation.Queue4();

		private com.db4o.foundation.Queue4 _statics = new com.db4o.foundation.Queue4();

		private com.db4o.foundation.Queue4 _writes = new com.db4o.foundation.Queue4();

		private com.db4o.foundation.Queue4 _inits = new com.db4o.foundation.Queue4();

		private bool _running = false;

		internal PendingClassInits(com.db4o.Transaction systemTransaction)
		{
			_systemTransaction = systemTransaction;
		}

		internal virtual void Process(com.db4o.YapClass newYapClass)
		{
			if (_pending.Contains(newYapClass))
			{
				return;
			}
			com.db4o.YapClass ancestor = newYapClass.GetAncestor();
			if (ancestor != null)
			{
				Process(ancestor);
			}
			_pending.Add(newYapClass);
			_members.Add(newYapClass);
			if (_running)
			{
				return;
			}
			_running = true;
			CheckInits();
			_pending = new com.db4o.foundation.Collection4();
			_running = false;
		}

		private void CheckMembers()
		{
			while (_members.HasNext())
			{
				com.db4o.YapClass yc = (com.db4o.YapClass)_members.Next();
				yc.AddMembers(Stream());
				_statics.Add(yc);
			}
		}

		private com.db4o.YapStream Stream()
		{
			return _systemTransaction.Stream();
		}

		private void CheckStatics()
		{
			CheckMembers();
			while (_statics.HasNext())
			{
				com.db4o.YapClass yc = (com.db4o.YapClass)_statics.Next();
				yc.StoreStaticFieldValues(_systemTransaction, true);
				_writes.Add(yc);
				CheckMembers();
			}
		}

		private void CheckWrites()
		{
			CheckStatics();
			while (_writes.HasNext())
			{
				com.db4o.YapClass yc = (com.db4o.YapClass)_writes.Next();
				yc.SetStateDirty();
				yc.Write(_systemTransaction);
				_inits.Add(yc);
				CheckStatics();
			}
		}

		private void CheckInits()
		{
			CheckWrites();
			while (_inits.HasNext())
			{
				com.db4o.YapClass yc = (com.db4o.YapClass)_inits.Next();
				yc.InitConfigOnUp(_systemTransaction);
				CheckWrites();
			}
		}
	}
}
