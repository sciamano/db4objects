namespace com.db4o
{
	/// <exclude></exclude>
	public abstract class YapFile : com.db4o.YapStream
	{
		protected com.db4o.YapConfigBlock _configBlock;

		private com.db4o.PBootRecord _bootRecord;

		private com.db4o.foundation.Collection4 i_dirty;

		private com.db4o.inside.freespace.FreespaceManager _freespaceManager;

		private com.db4o.inside.freespace.FreespaceManager _fmChecker;

		private bool i_isServer = false;

		private com.db4o.Tree i_prefetchedIDs;

		private com.db4o.foundation.Hashtable4 i_semaphores;

		internal int i_writeAt;

		internal YapFile(com.db4o.YapStream a_parent) : base(a_parent)
		{
		}

		public virtual byte blockSize()
		{
			return 1;
		}

		internal virtual void blockSize(int blockSize)
		{
		}

		public override com.db4o.PBootRecord bootRecord()
		{
			return _bootRecord;
		}

		internal override bool close2()
		{
			bool ret = base.close2();
			i_dirty = null;
			return ret;
		}

		internal override void commit1()
		{
			checkClosed();
			i_entryCounter++;
			try
			{
				write(false);
			}
			catch (System.Exception t)
			{
				fatalException(t);
			}
			i_entryCounter--;
		}

		internal virtual void configureNewFile()
		{
			_freespaceManager = com.db4o.inside.freespace.FreespaceManager.createNew(this, i_config
				._freespaceSystem);
			blockSize(i_config.i_blockSize);
			i_writeAt = blocksFor(HEADER_LENGTH);
			_configBlock = new com.db4o.YapConfigBlock(this);
			_configBlock.write();
			_configBlock.go();
			initNewClassCollection();
			initializeEssentialClasses();
			initBootRecord();
			_freespaceManager.start(_configBlock._freespaceAddress);
		}

		internal override long currentVersion()
		{
			return _bootRecord.i_versionGenerator;
		}

		internal virtual void initNewClassCollection()
		{
			i_classCollection.initTables(1);
		}

		internal sealed override com.db4o.ClassIndex createClassIndex(com.db4o.YapClass yapClass
			)
		{
			return new com.db4o.ClassIndex(yapClass);
		}

		internal sealed override com.db4o.QueryResultImpl createQResult(com.db4o.Transaction
			 a_ta)
		{
			return new com.db4o.QueryResultImpl(a_ta);
		}

		internal sealed override bool delete5(com.db4o.Transaction ta, com.db4o.YapObject
			 yo, int a_cascade, bool userCall)
		{
			int id = yo.getID();
			com.db4o.YapWriter reader = readWriterByID(ta, id);
			if (reader != null)
			{
				object obj = yo.getObject();
				if (obj != null)
				{
					if ((!showInternalClasses()) && com.db4o.YapConst.CLASS_INTERNAL.isAssignableFrom
						(j4o.lang.Class.getClassForObject(obj)))
					{
						return false;
					}
				}
				reader.setCascadeDeletes(a_cascade);
				ta.setPointer(id, 0, 0);
				com.db4o.YapClass yc = yo.getYapClass();
				yc.delete(reader, obj);
				ta.freeOnCommit(id, reader.getAddress(), reader.getLength());
				return true;
			}
			return false;
		}

		internal abstract long fileLength();

		internal abstract string fileName();

		public virtual void free(int a_address, int a_length)
		{
			_freespaceManager.free(a_address, a_length);
		}

		internal void freePrefetchedPointers()
		{
			if (i_prefetchedIDs != null)
			{
				i_prefetchedIDs.traverse(new _AnonymousInnerClass155(this));
			}
			i_prefetchedIDs = null;
		}

		private sealed class _AnonymousInnerClass155 : com.db4o.foundation.Visitor4
		{
			public _AnonymousInnerClass155(YapFile _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void visit(object a_object)
			{
				this._enclosing.free(((com.db4o.TreeInt)a_object).i_key, com.db4o.YapConst.POINTER_LENGTH
					);
			}

			private readonly YapFile _enclosing;
		}

		internal void freeSpaceBeginCommit()
		{
			if (_freespaceManager == null)
			{
				return;
			}
			_freespaceManager.beginCommit();
		}

		internal void freeSpaceEndCommit()
		{
			if (_freespaceManager == null)
			{
				return;
			}
			_freespaceManager.endCommit();
		}

		internal override void getAll(com.db4o.Transaction ta, com.db4o.QueryResultImpl a_res
			)
		{
			com.db4o.Tree[] duplicates = new com.db4o.Tree[1];
			com.db4o.YapClassCollectionIterator i = i_classCollection.iterator();
			while (i.hasNext())
			{
				com.db4o.YapClass yapClass = i.nextClass();
				if (yapClass.getName() != null)
				{
					com.db4o.reflect.ReflectClass claxx = yapClass.classReflector();
					if (claxx == null || !(i_handlers.ICLASS_INTERNAL.isAssignableFrom(claxx)))
					{
						com.db4o.Tree tree = yapClass.getIndex(ta);
						if (tree != null)
						{
							tree.traverse(new _AnonymousInnerClass194(this, duplicates, a_res));
						}
					}
				}
			}
			a_res.reset();
		}

		private sealed class _AnonymousInnerClass194 : com.db4o.foundation.Visitor4
		{
			public _AnonymousInnerClass194(YapFile _enclosing, com.db4o.Tree[] duplicates, com.db4o.QueryResultImpl
				 a_res)
			{
				this._enclosing = _enclosing;
				this.duplicates = duplicates;
				this.a_res = a_res;
			}

			public void visit(object obj)
			{
				int id = ((com.db4o.TreeInt)obj).i_key;
				com.db4o.TreeInt newNode = new com.db4o.TreeInt(id);
				duplicates[0] = com.db4o.Tree.add(duplicates[0], newNode);
				if (newNode.size() != 0)
				{
					a_res.add(id);
				}
			}

			private readonly YapFile _enclosing;

			private readonly com.db4o.Tree[] duplicates;

			private readonly com.db4o.QueryResultImpl a_res;
		}

		internal int getPointerSlot()
		{
			int id = getSlot(com.db4o.YapConst.POINTER_LENGTH);
			i_systemTrans.writePointer(id, 0, 0);
			if (id <= i_handlers.maxTypeID())
			{
				return getPointerSlot();
			}
			return id;
		}

		public virtual int blocksFor(long bytes)
		{
			int blockLen = blockSize();
			int result = (int)(bytes / blockLen);
			if (bytes % blockLen != 0)
			{
				result++;
			}
			return result;
		}

		public virtual int getSlot(int a_length)
		{
			return getSlot1(a_length);
			int address = getSlot1(a_length);
			com.db4o.DTrace.GET_SLOT.logLength(address, a_length);
			return address;
		}

		private int getSlot1(int bytes)
		{
			if (_freespaceManager != null)
			{
				int freeAddress = _freespaceManager.getSlot(bytes);
				if (freeAddress > 0)
				{
					return freeAddress;
				}
			}
			int blocksNeeded = blocksFor(bytes);
			if (com.db4o.Debug.xbytes && com.db4o.Deploy.overwrite)
			{
				writeXBytes(i_writeAt, blocksNeeded * blockSize());
			}
			int address = i_writeAt;
			i_writeAt += blocksNeeded;
			return address;
		}

		internal virtual void ensureLastSlotWritten()
		{
			if (i_writeAt > blocksFor(fileLength()))
			{
				com.db4o.YapWriter writer = getWriter(i_systemTrans, i_writeAt - 1, blockSize());
				writer.write();
			}
		}

		public override com.db4o.ext.Db4oDatabase identity()
		{
			if (_bootRecord == null)
			{
				return null;
			}
			return _bootRecord.i_db;
		}

		internal override void initialize2()
		{
			i_dirty = new com.db4o.foundation.Collection4();
			base.initialize2();
		}

		private void initBootRecord()
		{
			showInternalClasses(true);
			_bootRecord = new com.db4o.PBootRecord();
			_bootRecord.i_stream = this;
			_bootRecord.init(i_config);
			setInternal(i_systemTrans, _bootRecord, false);
			_configBlock._bootRecordID = getID1(i_systemTrans, _bootRecord);
			_configBlock.write();
			showInternalClasses(false);
		}

		internal override bool isServer()
		{
			return i_isServer;
		}

		internal sealed override com.db4o.YapWriter newObject(com.db4o.Transaction a_trans
			, com.db4o.YapMeta a_object)
		{
			int length = a_object.ownLength();
			int[] slot = newSlot(a_trans, length);
			a_object.setID(this, slot[0]);
			com.db4o.YapWriter writer = new com.db4o.YapWriter(a_trans, length);
			writer.useSlot(slot[0], slot[1], length);
			return writer;
		}

		public int[] newSlot(com.db4o.Transaction a_trans, int a_length)
		{
			int id = getPointerSlot();
			int address = getSlot(a_length);
			a_trans.setPointer(id, address, a_length);
			return new int[] { id, address };
		}

		internal sealed override int newUserObject()
		{
			return getPointerSlot();
		}

		internal virtual void prefetchedIDConsumed(int a_id)
		{
			i_prefetchedIDs = i_prefetchedIDs.removeLike(new com.db4o.TreeIntObject(a_id));
		}

		internal virtual int prefetchID()
		{
			int id = getPointerSlot();
			i_prefetchedIDs = com.db4o.Tree.add(i_prefetchedIDs, new com.db4o.TreeInt(id));
			return id;
		}

		internal override void raiseVersion(long a_minimumVersion)
		{
			if (_bootRecord.i_versionGenerator < a_minimumVersion)
			{
				_bootRecord.i_versionGenerator = a_minimumVersion;
				_bootRecord.setDirty();
				_bootRecord.store(1);
			}
		}

		public override com.db4o.YapWriter readWriterByID(com.db4o.Transaction a_ta, int 
			a_id)
		{
			return (com.db4o.YapWriter)readReaderOrWriterByID(a_ta, a_id, false);
		}

		internal override com.db4o.YapReader readReaderByID(com.db4o.Transaction a_ta, int
			 a_id)
		{
			return readReaderOrWriterByID(a_ta, a_id, true);
		}

		private com.db4o.YapReader readReaderOrWriterByID(com.db4o.Transaction a_ta, int 
			a_id, bool useReader)
		{
			if (a_id == 0)
			{
				return null;
			}
			int[] addressLength = new int[2];
			try
			{
				a_ta.getSlotInformation(a_id, addressLength);
				if (addressLength[0] == 0)
				{
					return null;
				}
				com.db4o.YapReader reader = null;
				if (useReader)
				{
					reader = new com.db4o.YapReader(addressLength[1]);
				}
				else
				{
					reader = getWriter(a_ta, addressLength[0], addressLength[1]);
					((com.db4o.YapWriter)reader).setID(a_id);
				}
				reader.readEncrypt(this, addressLength[0]);
				return reader;
			}
			catch (System.Exception e)
			{
			}
			return null;
		}

		internal virtual void readThis()
		{
			com.db4o.YapWriter myreader = getWriter(i_systemTrans, 0, HEADER_LENGTH);
			myreader.read();
			byte firstFileByte = myreader.readByte();
			byte blockLen = 1;
			if (firstFileByte != com.db4o.YapConst.YAPBEGIN)
			{
				if (firstFileByte != com.db4o.YapConst.YAPFILEVERSION)
				{
					com.db4o.inside.Exceptions4.throwRuntimeException(17);
				}
				blockLen = myreader.readByte();
			}
			else
			{
				if (myreader.readByte() != com.db4o.YapConst.YAPFILE)
				{
					com.db4o.inside.Exceptions4.throwRuntimeException(17);
				}
			}
			blockSize(blockLen);
			i_writeAt = blocksFor(fileLength());
			_configBlock = new com.db4o.YapConfigBlock(this);
			_configBlock.read(myreader.readInt());
			myreader.incrementOffset(com.db4o.YapConst.YAPID_LENGTH);
			i_classCollection.setID(this, myreader.readInt());
			i_classCollection.read(i_systemTrans);
			int freespaceID = myreader.readInt();
			_freespaceManager = com.db4o.inside.freespace.FreespaceManager.createNew(this, _configBlock
				._freespaceSystem);
			_freespaceManager.read(freespaceID);
			_freespaceManager.start(_configBlock._freespaceAddress);
			if (i_config._freespaceSystem != 0 || _configBlock._freespaceSystem == com.db4o.inside.freespace.FreespaceManager
				.FM_LEGACY_RAM)
			{
				if (_freespaceManager.systemType() != i_config._freespaceSystem)
				{
					com.db4o.inside.freespace.FreespaceManager newFM = com.db4o.inside.freespace.FreespaceManager
						.createNew(this, i_config._freespaceSystem);
					int fmSlot = _configBlock.newFreespaceSlot(i_config._freespaceSystem);
					newFM.start(fmSlot);
					_freespaceManager.migrate(newFM);
					com.db4o.inside.freespace.FreespaceManager oldFM = _freespaceManager;
					_freespaceManager = newFM;
					oldFM.freeSelf();
					_freespaceManager.beginCommit();
					_freespaceManager.endCommit();
					_configBlock.write();
				}
			}
			showInternalClasses(true);
			object bootRecord = null;
			if (_configBlock._bootRecordID > 0)
			{
				bootRecord = getByID1(i_systemTrans, _configBlock._bootRecordID);
			}
			if (bootRecord is com.db4o.PBootRecord)
			{
				_bootRecord = (com.db4o.PBootRecord)bootRecord;
				_bootRecord.checkActive();
				_bootRecord.i_stream = this;
				if (_bootRecord.initConfig(i_config))
				{
					i_classCollection.reReadYapClass(getYapClass(i_handlers.ICLASS_PBOOTRECORD, false
						));
					setInternal(i_systemTrans, _bootRecord, false);
				}
			}
			else
			{
				initBootRecord();
			}
			showInternalClasses(false);
			writeHeader(false);
			com.db4o.Transaction trans = _configBlock.getTransactionToCommit();
			if (trans != null)
			{
				if (!i_config.i_disableCommitRecovery)
				{
					trans.writeOld();
				}
			}
		}

		public override void releaseSemaphore(string name)
		{
			releaseSemaphore(checkTransaction(null), name);
		}

		internal virtual void releaseSemaphore(com.db4o.Transaction ta, string name)
		{
			if (i_semaphores != null)
			{
				lock (i_semaphores)
				{
					if (i_semaphores != null && ta == i_semaphores.get(name))
					{
						i_semaphores.remove(name);
						j4o.lang.JavaSystem.notifyAll(i_semaphores);
					}
				}
			}
		}

		internal override void releaseSemaphores(com.db4o.Transaction ta)
		{
			if (i_semaphores != null)
			{
				lock (i_semaphores)
				{
					i_semaphores.forEachKeyForIdentity(new _AnonymousInnerClass555(this), ta);
					j4o.lang.JavaSystem.notifyAll(i_semaphores);
				}
			}
		}

		private sealed class _AnonymousInnerClass555 : com.db4o.foundation.Visitor4
		{
			public _AnonymousInnerClass555(YapFile _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void visit(object a_object)
			{
				this._enclosing.i_semaphores.remove(a_object);
			}

			private readonly YapFile _enclosing;
		}

		internal sealed override void rollback1()
		{
			checkClosed();
			i_entryCounter++;
			getTransaction().rollback();
			i_entryCounter--;
		}

		internal sealed override void setDirty(com.db4o.UseSystemTransaction a_object)
		{
			((com.db4o.YapMeta)a_object).setStateDirty();
			((com.db4o.YapMeta)a_object).cacheDirty(i_dirty);
		}

		public override bool setSemaphore(string name, int timeout)
		{
			return setSemaphore(checkTransaction(null), name, timeout);
		}

		internal virtual bool setSemaphore(com.db4o.Transaction ta, string name, int timeout
			)
		{
			if (name == null)
			{
				throw new System.ArgumentNullException();
			}
			if (i_semaphores == null)
			{
				lock (i_lock)
				{
					if (i_semaphores == null)
					{
						i_semaphores = new com.db4o.foundation.Hashtable4(10);
					}
				}
			}
			lock (i_semaphores)
			{
				object obj = i_semaphores.get(name);
				if (obj == null)
				{
					i_semaphores.put(name, ta);
					return true;
				}
				if (ta == obj)
				{
					return true;
				}
				long endtime = j4o.lang.JavaSystem.currentTimeMillis() + timeout;
				long waitTime = timeout;
				while (waitTime > 0)
				{
					try
					{
						j4o.lang.JavaSystem.wait(i_semaphores, waitTime);
					}
					catch (System.Exception e)
					{
					}
					if (i_classCollection == null)
					{
						return false;
					}
					obj = i_semaphores.get(name);
					if (obj == null)
					{
						i_semaphores.put(name, ta);
						return true;
					}
					waitTime = endtime - j4o.lang.JavaSystem.currentTimeMillis();
				}
				return false;
			}
		}

		internal virtual void setServer(bool flag)
		{
			i_isServer = flag;
		}

		public abstract void copy(int oldAddress, int oldAddressOffset, int newAddress, int
			 newAddressOffset, int length);

		internal abstract void syncFiles();

		public override string ToString()
		{
			return fileName();
		}

		internal sealed override com.db4o.YapWriter updateObject(com.db4o.Transaction a_trans
			, com.db4o.YapMeta a_object)
		{
			int length = a_object.ownLength();
			int id = a_object.getID();
			int address = getSlot(length);
			int[] oldAddressLength = new int[2];
			a_trans.getSlotInformation(id, oldAddressLength);
			a_trans.freeOnCommit(id, oldAddressLength[0], oldAddressLength[1]);
			a_trans.freeOnRollback(id, address, length);
			a_trans.setPointer(id, address, length);
			com.db4o.YapWriter writer = a_trans.i_stream.getWriter(a_trans, length);
			writer.useSlot(id, address, length);
			return writer;
		}

		internal override void write(bool shuttingDown)
		{
			i_trans.commit();
			if (shuttingDown)
			{
				writeHeader(shuttingDown);
			}
		}

		internal abstract bool writeAccessTime();

		internal abstract void writeBytes(com.db4o.YapWriter a_Bytes);

		internal sealed override void writeDirty()
		{
			com.db4o.YapMeta dirty;
			com.db4o.foundation.Iterator4 i = i_dirty.iterator();
			while (i.hasNext())
			{
				dirty = (com.db4o.YapMeta)i.next();
				dirty.write(this, i_systemTrans);
				dirty.notCachedDirty();
			}
			i_dirty.clear();
			writeBootRecord();
		}

		internal sealed override void writeEmbedded(com.db4o.YapWriter a_parent, com.db4o.YapWriter
			 a_child)
		{
			int length = a_child.getLength();
			int address = getSlot(length);
			a_child.getTransaction().freeOnRollback(address, address, length);
			a_child.address(address);
			a_child.writeEncrypt();
			int offsetBackup = a_parent._offset;
			a_parent._offset = a_child.getID();
			a_parent.writeInt(address);
			a_parent._offset = offsetBackup;
		}

		internal virtual void writeHeader(bool shuttingDown)
		{
			int freespaceID = _freespaceManager.write(shuttingDown);
			if (shuttingDown)
			{
				_freespaceManager = null;
			}
			com.db4o.YapWriter writer = getWriter(i_systemTrans, 0, HEADER_LENGTH);
			writer.append(com.db4o.YapConst.YAPFILEVERSION);
			writer.append(blockSize());
			writer.writeInt(_configBlock._address);
			writer.writeInt(0);
			writer.writeInt(i_classCollection.getID());
			writer.writeInt(freespaceID);
			if (com.db4o.Debug.xbytes && com.db4o.Deploy.overwrite)
			{
				writer.setID(com.db4o.YapConst.IGNORE_ID);
			}
			writer.write();
			if (shuttingDown)
			{
				ensureLastSlotWritten();
			}
		}

		internal sealed override void writeNew(com.db4o.YapClass a_yapClass, com.db4o.YapWriter
			 aWriter)
		{
			writeObject(null, aWriter);
			if (maintainsIndices())
			{
				a_yapClass.addToIndex(this, aWriter.getTransaction(), aWriter.getID());
			}
		}

		internal void writeObject(com.db4o.YapMeta a_object, com.db4o.YapWriter a_writer)
		{
			i_handlers.encrypt(a_writer);
			writeBytes(a_writer);
		}

		internal virtual void writeBootRecord()
		{
			_bootRecord.store(1);
		}

		public abstract void writeXBytes(int a_address, int a_length);

		internal virtual com.db4o.YapWriter xBytes(int a_address, int a_length)
		{
			throw com.db4o.YapConst.virtualException();
		}

		internal sealed override void writeTransactionPointer(int a_address)
		{
			com.db4o.YapWriter bytes = new com.db4o.YapWriter(i_systemTrans, _configBlock._address
				, com.db4o.YapConst.YAPINT_LENGTH * 2);
			bytes.moveForward(com.db4o.YapConfigBlock.TRANSACTION_OFFSET);
			bytes.writeInt(a_address);
			bytes.writeInt(a_address);
			if (com.db4o.Debug.xbytes && com.db4o.Deploy.overwrite)
			{
				bytes.setID(com.db4o.YapConst.IGNORE_ID);
			}
			bytes.write();
		}

		internal sealed override void writeUpdate(com.db4o.YapClass a_yapClass, com.db4o.YapWriter
			 a_bytes)
		{
			com.db4o.Transaction trans = a_bytes.getTransaction();
			int id = a_bytes.getID();
			int length = a_bytes.getLength();
			int address = getSlot(length);
			a_bytes.address(address);
			trans.setPointer(id, address, length);
			trans.freeOnRollback(id, address, length);
			i_handlers.encrypt(a_bytes);
			a_bytes.write();
		}
	}
}
