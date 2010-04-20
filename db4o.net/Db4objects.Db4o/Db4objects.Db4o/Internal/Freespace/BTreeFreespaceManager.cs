/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Ids;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Freespace
{
	/// <exclude></exclude>
	public class BTreeFreespaceManager : AbstractFreespaceManager
	{
		private readonly LocalObjectContainer _file;

		private InMemoryFreespaceManager _delegate;

		private BTree _slotsByAddress;

		private BTree _slotsByLength;

		private PersistentIntegerArray _idArray;

		private int _delegateIndirectionID;

		private int _delegationRequests;

		private IFreespaceListener _listener = NullFreespaceListener.Instance;

		private ITransactionalIdSystem _idSystem;

		public BTreeFreespaceManager(LocalObjectContainer file, IProcedure4 slotFreedCallback
			, int discardLimit) : base(slotFreedCallback, discardLimit)
		{
			_file = file;
			_delegate = new InMemoryFreespaceManager(slotFreedCallback, discardLimit);
			_idSystem = file.IdSystem().FreespaceIdSystem();
		}

		private void AddSlot(Slot slot)
		{
			_slotsByLength.Add(Transaction(), slot);
			_slotsByAddress.Add(Transaction(), slot);
			_listener.SlotAdded(slot.Length());
		}

		public override Slot AllocateTransactionLogSlot(int length)
		{
			return _delegate.AllocateTransactionLogSlot(length);
		}

		public override void BeginCommit()
		{
			BeginDelegation();
		}

		private void BeginDelegation()
		{
			_delegationRequests++;
		}

		public override void Commit()
		{
			_slotsByAddress.Commit(Transaction());
			_slotsByLength.Commit(Transaction());
		}

		private void CreateBTrees(int addressID, int lengthID)
		{
			BTreeConfiguration config = new BTreeConfiguration(_idSystem, SlotChangeFactory.FreeSpace
				, 64, false);
			_slotsByAddress = new BTree(Transaction(), config, addressID, new AddressKeySlotHandler
				());
			_slotsByLength = new BTree(Transaction(), config, lengthID, new LengthKeySlotHandler
				());
		}

		public override void EndCommit()
		{
			EndDelegation();
		}

		private void EndDelegation()
		{
			_delegationRequests--;
		}

		public override void Free(Slot slot)
		{
			if (!IsStarted())
			{
				return;
			}
			if (IsDelegating())
			{
				_delegate.Free(slot);
				return;
			}
			try
			{
				BeginDelegation();
				if (DTrace.enabled)
				{
					DTrace.FreespacemanagerBtreeFree.LogLength(slot.Address(), slot.Length());
				}
				Slot[] remove = new Slot[2];
				Slot newFreeSlot = slot;
				BTreePointer pointer = SearchBTree(_slotsByAddress, slot, SearchTarget.Lowest);
				BTreePointer previousPointer = pointer != null ? pointer.Previous() : _slotsByAddress
					.LastPointer(Transaction());
				if (previousPointer != null)
				{
					Slot previousSlot = (Slot)previousPointer.Key();
					if (previousSlot.IsDirectlyPreceding(newFreeSlot))
					{
						remove[0] = previousSlot;
						newFreeSlot = previousSlot.Append(newFreeSlot);
					}
				}
				if (pointer != null)
				{
					Slot nextSlot = (Slot)pointer.Key();
					if (newFreeSlot.IsDirectlyPreceding(nextSlot))
					{
						remove[1] = nextSlot;
						newFreeSlot = newFreeSlot.Append(nextSlot);
					}
				}
				for (int i = 0; i < remove.Length; i++)
				{
					if (remove[i] != null)
					{
						RemoveSlot(remove[i]);
					}
				}
				if (!CanDiscard(newFreeSlot.Length()))
				{
					AddSlot(newFreeSlot);
				}
				SlotFreed(slot);
			}
			finally
			{
				EndDelegation();
			}
		}

		public override void FreeSelf()
		{
			_slotsByAddress.Free(Transaction());
			_slotsByLength.Free(Transaction());
		}

		public override void FreeTransactionLogSlot(Slot slot)
		{
			_delegate.FreeTransactionLogSlot(slot);
		}

		public override Slot AllocateSlot(int length)
		{
			if (!IsStarted())
			{
				return null;
			}
			if (IsDelegating())
			{
				return _delegate.AllocateSlot(length);
			}
			try
			{
				BeginDelegation();
				BTreePointer pointer = SearchBTree(_slotsByLength, new Slot(0, length), SearchTarget
					.Highest);
				if (pointer == null)
				{
					return null;
				}
				Slot slot = (Slot)pointer.Key();
				RemoveSlot(slot);
				int remainingLength = slot.Length() - length;
				if (!CanDiscard(remainingLength))
				{
					AddSlot(slot.SubSlot(length));
					slot = slot.Truncate(length);
				}
				if (DTrace.enabled)
				{
					DTrace.FreespacemanagerGetSlot.LogLength(slot.Address(), slot.Length());
				}
				return slot;
			}
			finally
			{
				EndDelegation();
			}
		}

		private void InitializeExisting(int slotAddress)
		{
			_idArray = new PersistentIntegerArray(_idSystem, slotAddress);
			_idArray.Read(Transaction());
			int[] ids = _idArray.Array();
			int addressId = ids[0];
			int lengthID = ids[1];
			_delegateIndirectionID = ids[2];
			CreateBTrees(addressId, lengthID);
			_slotsByAddress.Read(Transaction());
			_slotsByLength.Read(Transaction());
			Slot slot = _file.ReadPointerSlot(_delegateIndirectionID);
			_file.WritePointer(_delegateIndirectionID, Slot.Zero);
			_file.SyncFiles();
			_delegate.Read(_file, slot);
		}

		private void InitializeNew()
		{
			CreateBTrees(0, 0);
			_slotsByAddress.Write(Transaction());
			_slotsByLength.Write(Transaction());
			LocalObjectContainer container = (LocalObjectContainer)Transaction().Container();
			_delegateIndirectionID = container.AllocatePointerSlot();
			int[] ids = new int[] { _slotsByAddress.GetID(), _slotsByLength.GetID(), _delegateIndirectionID
				 };
			_idArray = new PersistentIntegerArray(_idSystem, ids);
			_idArray.Write(Transaction());
			_file.SystemData().FreespaceAddress(_idArray.GetID());
		}

		private bool IsDelegating()
		{
			return _delegationRequests > 0;
		}

		public override void Read(LocalObjectContainer container, int freeSpaceID)
		{
		}

		// do nothing
		private void RemoveSlot(Slot slot)
		{
			_slotsByLength.Remove(Transaction(), slot);
			_slotsByAddress.Remove(Transaction(), slot);
			_listener.SlotRemoved(slot.Length());
		}

		private BTreePointer SearchBTree(BTree bTree, Slot slot, SearchTarget target)
		{
			BTreeNodeSearchResult searchResult = bTree.SearchLeaf(Transaction(), slot, target
				);
			return searchResult.FirstValidPointer();
		}

		public override int SlotCount()
		{
			return _slotsByAddress.Size(Transaction()) + _delegate.SlotCount();
		}

		public override void Start(int slotAddress)
		{
			try
			{
				BeginDelegation();
				if (slotAddress == 0)
				{
					InitializeNew();
				}
				else
				{
					InitializeExisting(slotAddress);
				}
			}
			finally
			{
				EndDelegation();
			}
		}

		public override bool IsStarted()
		{
			return _idArray != null;
		}

		public override byte SystemType()
		{
			return FmBtree;
		}

		public override string ToString()
		{
			return _slotsByLength.ToString();
		}

		public override int TotalFreespace()
		{
			return base.TotalFreespace() + _delegate.TotalFreespace();
		}

		public override void Traverse(IVisitor4 visitor)
		{
			_slotsByAddress.TraverseKeys(Transaction(), visitor);
		}

		public override void MigrateTo(IFreespaceManager fm)
		{
			base.MigrateTo(fm);
			_delegate.MigrateTo(fm);
		}

		public override int Write(LocalObjectContainer container)
		{
			try
			{
				BeginDelegation();
				Slot slot = _file.AllocateSlot(_delegate.MarshalledLength());
				Pointer4 pointer = new Pointer4(_delegateIndirectionID, slot);
				_delegate.Write(_file, pointer);
				return _idArray.GetID();
			}
			finally
			{
				EndDelegation();
			}
		}

		public override void Listener(IFreespaceListener listener)
		{
			_listener = listener;
		}

		private LocalTransaction Transaction()
		{
			return (LocalTransaction)_file.SystemTransaction();
		}
	}
}
