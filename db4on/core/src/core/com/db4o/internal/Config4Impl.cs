namespace com.db4o.@internal
{
	/// <summary>Configuration template for creating new db4o files</summary>
	/// <exclude></exclude>
	public sealed class Config4Impl : com.db4o.config.Configuration, com.db4o.foundation.DeepClone
		, com.db4o.messaging.MessageSender, com.db4o.config.FreespaceConfiguration, com.db4o.config.QueryConfiguration
		, com.db4o.config.ClientServerConfiguration
	{
		private com.db4o.foundation.KeySpecHashtable4 _config = new com.db4o.foundation.KeySpecHashtable4
			(50);

		private static readonly com.db4o.foundation.KeySpec ACTIVATION_DEPTH = new com.db4o.foundation.KeySpec
			(5);

		private static readonly com.db4o.foundation.KeySpec ALLOW_VERSION_UPDATES = new com.db4o.foundation.KeySpec
			(false);

		private static readonly com.db4o.foundation.KeySpec AUTOMATIC_SHUTDOWN = new com.db4o.foundation.KeySpec
			(true);

		private static readonly com.db4o.foundation.KeySpec BLOCKSIZE = new com.db4o.foundation.KeySpec
			((byte)1);

		private static readonly com.db4o.foundation.KeySpec BLOBPATH = new com.db4o.foundation.KeySpec
			(null);

		private static readonly com.db4o.foundation.KeySpec BTREE_NODE_SIZE = new com.db4o.foundation.KeySpec
			(119);

		private static readonly com.db4o.foundation.KeySpec BTREE_CACHE_HEIGHT = new com.db4o.foundation.KeySpec
			(1);

		private static readonly com.db4o.foundation.KeySpec CALLBACKS = new com.db4o.foundation.KeySpec
			(true);

		private static readonly com.db4o.foundation.KeySpec CALL_CONSTRUCTORS = new com.db4o.foundation.KeySpec
			(com.db4o.foundation.TernaryBool.UNSPECIFIED);

		private static readonly com.db4o.foundation.KeySpec CLASS_ACTIVATION_DEPTH_CONFIGURABLE
			 = new com.db4o.foundation.KeySpec(true);

		private static readonly com.db4o.foundation.KeySpec CLASSLOADER = new com.db4o.foundation.KeySpec
			(null);

		private static readonly com.db4o.foundation.KeySpec DETECT_SCHEMA_CHANGES = new com.db4o.foundation.KeySpec
			(true);

		private static readonly com.db4o.foundation.KeySpec DIAGNOSTIC = new com.db4o.foundation.KeySpec
			(new com.db4o.@internal.diagnostic.DiagnosticProcessor());

		private static readonly com.db4o.foundation.KeySpec DISABLE_COMMIT_RECOVERY = new 
			com.db4o.foundation.KeySpec(false);

		private static readonly com.db4o.foundation.KeySpec DISCARD_FREESPACE = new com.db4o.foundation.KeySpec
			(0);

		private static readonly com.db4o.foundation.KeySpec ENCODING = new com.db4o.foundation.KeySpec
			(com.db4o.@internal.Const4.UNICODE);

		private static readonly com.db4o.foundation.KeySpec ENCRYPT = new com.db4o.foundation.KeySpec
			(false);

		private static readonly com.db4o.foundation.KeySpec EXCEPTIONAL_CLASSES = new com.db4o.foundation.KeySpec
			(null);

		private static readonly com.db4o.foundation.KeySpec EXCEPTIONS_ON_NOT_STORABLE = 
			new com.db4o.foundation.KeySpec(false);

		private static readonly com.db4o.foundation.KeySpec FLUSH_FILE_BUFFERS = new com.db4o.foundation.KeySpec
			(true);

		private static readonly com.db4o.foundation.KeySpec FREESPACE_FILLER = new com.db4o.foundation.KeySpec
			(null);

		private static readonly com.db4o.foundation.KeySpec FREESPACE_SYSTEM = new com.db4o.foundation.KeySpec
			(com.db4o.@internal.freespace.FreespaceManager.FM_DEFAULT);

		private static readonly com.db4o.foundation.KeySpec GENERATE_UUIDS = new com.db4o.foundation.KeySpec
			(com.db4o.config.ConfigScope.INDIVIDUALLY);

		private static readonly com.db4o.foundation.KeySpec GENERATE_VERSION_NUMBERS = new 
			com.db4o.foundation.KeySpec(com.db4o.config.ConfigScope.INDIVIDUALLY);

		private static readonly com.db4o.foundation.KeySpec IS_SERVER = new com.db4o.foundation.KeySpec
			(false);

		private static readonly com.db4o.foundation.KeySpec QUERY_EVALUATION_MODE = new com.db4o.foundation.KeySpec
			(com.db4o.config.QueryEvaluationMode.IMMEDIATE);

		private static readonly com.db4o.foundation.KeySpec LOCK_FILE = new com.db4o.foundation.KeySpec
			(true);

		private static readonly com.db4o.foundation.KeySpec MESSAGE_RECIPIENT = new com.db4o.foundation.KeySpec
			(null);

		private static readonly com.db4o.foundation.KeySpec OPTIMIZE_NQ = new com.db4o.foundation.KeySpec
			(true);

		private static readonly com.db4o.foundation.KeySpec OUTSTREAM = new com.db4o.foundation.KeySpec
			(null);

		private static readonly com.db4o.foundation.KeySpec PASSWORD = new com.db4o.foundation.KeySpec
			((string)null);

		private static readonly com.db4o.foundation.KeySpec CLIENT_QUERY_RESULT_ITERATOR_FACTORY
			 = new com.db4o.foundation.KeySpec(null);

		private static readonly com.db4o.foundation.KeySpec PREFETCH_ID_COUNT = new com.db4o.foundation.KeySpec
			(10);

		private static readonly com.db4o.foundation.KeySpec PREFETCH_OBJECT_COUNT = new com.db4o.foundation.KeySpec
			(10);

		private static readonly com.db4o.foundation.KeySpec READ_AS = new com.db4o.foundation.KeySpec
			(new com.db4o.foundation.Hashtable4(16));

		private static readonly com.db4o.foundation.KeySpec CONFIGURED_REFLECTOR = new com.db4o.foundation.KeySpec
			(null);

		private static readonly com.db4o.foundation.KeySpec REFLECTOR = new com.db4o.foundation.KeySpec
			(null);

		private static readonly com.db4o.foundation.KeySpec RENAME = new com.db4o.foundation.KeySpec
			(null);

		private static readonly com.db4o.foundation.KeySpec RESERVED_STORAGE_SPACE = new 
			com.db4o.foundation.KeySpec(0);

		private static readonly com.db4o.foundation.KeySpec SINGLE_THREADED_CLIENT = new 
			com.db4o.foundation.KeySpec(false);

		private static readonly com.db4o.foundation.KeySpec TEST_CONSTRUCTORS = new com.db4o.foundation.KeySpec
			(true);

		private static readonly com.db4o.foundation.KeySpec TIMEOUT_CLIENT_SOCKET = new com.db4o.foundation.KeySpec
			(com.db4o.@internal.Const4.CLIENT_SOCKET_TIMEOUT);

		private static readonly com.db4o.foundation.KeySpec TIMEOUT_PING_CLIENTS = new com.db4o.foundation.KeySpec
			(com.db4o.@internal.Const4.CONNECTION_TIMEOUT);

		private static readonly com.db4o.foundation.KeySpec TIMEOUT_SERVER_SOCKET = new com.db4o.foundation.KeySpec
			(com.db4o.@internal.Const4.SERVER_SOCKET_TIMEOUT);

		private static readonly com.db4o.foundation.KeySpec UPDATE_DEPTH = new com.db4o.foundation.KeySpec
			(0);

		private static readonly com.db4o.foundation.KeySpec WEAK_REFERENCE_COLLECTION_INTERVAL
			 = new com.db4o.foundation.KeySpec(1000);

		private static readonly com.db4o.foundation.KeySpec WEAK_REFERENCES = new com.db4o.foundation.KeySpec
			(true);

		private static readonly com.db4o.foundation.KeySpec IOADAPTER = new com.db4o.foundation.KeySpec
			(new com.db4o.io.RandomAccessFileAdapter());

		private static readonly com.db4o.foundation.KeySpec ALIASES = new com.db4o.foundation.KeySpec
			(null);

		private static readonly com.db4o.foundation.KeySpec BATCH_MESSAGES = new com.db4o.foundation.KeySpec
			(false);

		private static readonly com.db4o.foundation.KeySpec MAX_BATCH_QUEUE_SIZE = new com.db4o.foundation.KeySpec
			(int.MaxValue);

		private com.db4o.@internal.ObjectContainerBase i_stream;

		private bool _internStrings;

		private int _messageLevel;

		private bool _readOnly;

		public int ActivationDepth()
		{
			return _config.GetAsInt(ACTIVATION_DEPTH);
		}

		public void ActivationDepth(int depth)
		{
			_config.Put(ACTIVATION_DEPTH, depth);
		}

		public void AllowVersionUpdates(bool flag)
		{
			_config.Put(ALLOW_VERSION_UPDATES, flag);
		}

		public void AutomaticShutDown(bool flag)
		{
			_config.Put(AUTOMATIC_SHUTDOWN, flag);
		}

		public void BlockSize(int bytes)
		{
			if (bytes < 1 || bytes > 127)
			{
				com.db4o.@internal.Exceptions4.ThrowRuntimeException(1);
			}
			if (i_stream != null)
			{
				com.db4o.@internal.Exceptions4.ThrowRuntimeException(46);
			}
			_config.Put(BLOCKSIZE, (byte)bytes);
		}

		public void BTreeNodeSize(int size)
		{
			_config.Put(BTREE_NODE_SIZE, size);
		}

		public void BTreeCacheHeight(int height)
		{
			_config.Put(BTREE_CACHE_HEIGHT, height);
		}

		public void Callbacks(bool turnOn)
		{
			_config.Put(CALLBACKS, turnOn);
		}

		public void CallConstructors(bool flag)
		{
			_config.Put(CALL_CONSTRUCTORS, com.db4o.foundation.TernaryBool.ForBoolean(flag));
		}

		public void ClassActivationDepthConfigurable(bool turnOn)
		{
			_config.Put(CLASS_ACTIVATION_DEPTH_CONFIGURABLE, turnOn);
		}

		internal com.db4o.@internal.Config4Class ConfigClass(string className)
		{
			com.db4o.@internal.Config4Class config = (com.db4o.@internal.Config4Class)ExceptionalClasses
				().Get(className);
			return config;
		}

		public object DeepClone(object param)
		{
			com.db4o.@internal.Config4Impl ret = new com.db4o.@internal.Config4Impl();
			ret._config = (com.db4o.foundation.KeySpecHashtable4)_config.DeepClone(this);
			ret._internStrings = _internStrings;
			ret._messageLevel = _messageLevel;
			ret._readOnly = _readOnly;
			return ret;
		}

		public void Stream(com.db4o.@internal.ObjectContainerBase stream)
		{
			i_stream = stream;
		}

		public void DetectSchemaChanges(bool flag)
		{
			_config.Put(DETECT_SCHEMA_CHANGES, flag);
		}

		public void DisableCommitRecovery()
		{
			_config.Put(DISABLE_COMMIT_RECOVERY, true);
		}

		public void DiscardFreeSpace(int bytes)
		{
			_config.Put(DISCARD_FREESPACE, bytes);
		}

		public void DiscardSmallerThan(int byteCount)
		{
			DiscardFreeSpace(byteCount);
		}

		public void Encrypt(bool flag)
		{
			GlobalSettingOnly();
			_config.Put(ENCRYPT, flag);
		}

		internal void OldEncryptionOff()
		{
			_config.Put(ENCRYPT, false);
		}

		internal void EnsureDirExists(string path)
		{
			j4o.io.File file = new j4o.io.File(path);
			if (!file.Exists())
			{
				file.Mkdirs();
			}
			if (file.Exists() && file.IsDirectory())
			{
			}
			else
			{
				throw new System.IO.IOException(com.db4o.@internal.Messages.Get(37, path));
			}
		}

		internal System.IO.TextWriter ErrStream()
		{
			System.IO.TextWriter outStream = OutStreamOrNull();
			return outStream == null ? j4o.lang.JavaSystem.Err : outStream;
		}

		public void ExceptionsOnNotStorable(bool flag)
		{
			_config.Put(EXCEPTIONS_ON_NOT_STORABLE, flag);
		}

		public void FlushFileBuffers(bool flag)
		{
			_config.Put(FLUSH_FILE_BUFFERS, flag);
		}

		public com.db4o.config.FreespaceConfiguration Freespace()
		{
			return this;
		}

		public void FreespaceFiller(com.db4o.config.FreespaceFiller freespaceFiller)
		{
			_config.Put(FREESPACE_FILLER, freespaceFiller);
		}

		public com.db4o.config.FreespaceFiller FreespaceFiller()
		{
			return (com.db4o.config.FreespaceFiller)_config.Get(FREESPACE_FILLER);
		}

		/// <deprecated>
		/// Use
		/// <see cref="com.db4o.@internal.Config4Impl.GenerateUUIDs">com.db4o.@internal.Config4Impl.GenerateUUIDs
		/// 	</see>
		/// instead.
		/// </deprecated>
		public void GenerateUUIDs(int setting)
		{
			GenerateUUIDs(com.db4o.config.ConfigScope.ForID(setting));
		}

		public void GenerateUUIDs(com.db4o.config.ConfigScope scope)
		{
			_config.Put(GENERATE_UUIDS, scope);
		}

		/// <deprecated>
		/// Use
		/// <see cref="com.db4o.@internal.Config4Impl.GenerateVersionNumbers">com.db4o.@internal.Config4Impl.GenerateVersionNumbers
		/// 	</see>
		/// instead.
		/// </deprecated>
		public void GenerateVersionNumbers(int setting)
		{
			GenerateVersionNumbers(com.db4o.config.ConfigScope.ForID(setting));
		}

		public void GenerateVersionNumbers(com.db4o.config.ConfigScope scope)
		{
			_config.Put(GENERATE_VERSION_NUMBERS, scope);
		}

		public com.db4o.messaging.MessageSender GetMessageSender()
		{
			return this;
		}

		private void GlobalSettingOnly()
		{
			if (i_stream != null)
			{
				j4o.lang.JavaSystem.PrintStackTrace(new System.Exception());
				com.db4o.@internal.Exceptions4.ThrowRuntimeException(46);
			}
		}

		public void InternStrings(bool doIntern)
		{
			_internStrings = doIntern;
		}

		public void Io(com.db4o.io.IoAdapter adapter)
		{
			GlobalSettingOnly();
			_config.Put(IOADAPTER, adapter);
		}

		public void LockDatabaseFile(bool flag)
		{
			_config.Put(LOCK_FILE, flag);
		}

		public void MarkTransient(string marker)
		{
			com.db4o.@internal.Platform4.MarkTransient(marker);
		}

		public void MessageLevel(int level)
		{
			_messageLevel = level;
			if (OutStream() == null)
			{
				SetOut(j4o.lang.JavaSystem.Out);
			}
		}

		public void OptimizeNativeQueries(bool optimizeNQ)
		{
			_config.Put(OPTIMIZE_NQ, optimizeNQ);
		}

		public bool OptimizeNativeQueries()
		{
			return _config.GetAsBoolean(OPTIMIZE_NQ);
		}

		public com.db4o.config.ObjectClass ObjectClass(object clazz)
		{
			string className = null;
			if (clazz is string)
			{
				className = (string)clazz;
			}
			else
			{
				com.db4o.reflect.ReflectClass claxx = ReflectorFor(clazz);
				if (claxx == null)
				{
					return null;
				}
				className = claxx.GetName();
			}
			com.db4o.foundation.Hashtable4 xClasses = ExceptionalClasses();
			com.db4o.@internal.Config4Class c4c = (com.db4o.@internal.Config4Class)xClasses.Get
				(className);
			if (c4c == null)
			{
				c4c = new com.db4o.@internal.Config4Class(this, className);
				xClasses.Put(className, c4c);
			}
			return c4c;
		}

		private System.IO.TextWriter OutStreamOrNull()
		{
			return (System.IO.TextWriter)_config.Get(OUTSTREAM);
		}

		internal System.IO.TextWriter OutStream()
		{
			System.IO.TextWriter outStream = OutStreamOrNull();
			return outStream == null ? j4o.lang.JavaSystem.Out : outStream;
		}

		public void Password(string pw)
		{
			GlobalSettingOnly();
			_config.Put(PASSWORD, pw);
		}

		public void ReadOnly(bool flag)
		{
			_readOnly = flag;
		}

		public com.db4o.reflect.generic.GenericReflector Reflector()
		{
			com.db4o.reflect.generic.GenericReflector reflector = (com.db4o.reflect.generic.GenericReflector
				)_config.Get(REFLECTOR);
			if (reflector == null)
			{
				com.db4o.reflect.Reflector configuredReflector = (com.db4o.reflect.Reflector)_config
					.Get(CONFIGURED_REFLECTOR);
				if (configuredReflector == null)
				{
					configuredReflector = com.db4o.@internal.Platform4.CreateReflector(ClassLoader());
					_config.Put(CONFIGURED_REFLECTOR, configuredReflector);
				}
				reflector = new com.db4o.reflect.generic.GenericReflector(null, configuredReflector
					);
				_config.Put(REFLECTOR, reflector);
				configuredReflector.SetParent(reflector);
			}
			return reflector;
		}

		public void ReflectWith(com.db4o.reflect.Reflector reflect)
		{
			if (i_stream != null)
			{
				com.db4o.@internal.Exceptions4.ThrowRuntimeException(46);
			}
			if (reflect == null)
			{
				throw new System.ArgumentNullException();
			}
			_config.Put(CONFIGURED_REFLECTOR, reflect);
			_config.Put(REFLECTOR, null);
		}

		public void RefreshClasses()
		{
			if (i_stream != null)
			{
				i_stream.RefreshClasses();
			}
		}

		internal void Rename(com.db4o.Rename a_rename)
		{
			com.db4o.foundation.Collection4 renameCollection = Rename();
			if (renameCollection == null)
			{
				renameCollection = new com.db4o.foundation.Collection4();
				_config.Put(RENAME, renameCollection);
			}
			renameCollection.Add(a_rename);
		}

		public void ReserveStorageSpace(long byteCount)
		{
			int reservedStorageSpace = (int)byteCount;
			if (reservedStorageSpace < 0)
			{
				reservedStorageSpace = 0;
			}
			_config.Put(RESERVED_STORAGE_SPACE, reservedStorageSpace);
			if (i_stream != null)
			{
				i_stream.Reserve(reservedStorageSpace);
			}
		}

		/// <summary>The ConfigImpl also is our messageSender</summary>
		public void Send(object obj)
		{
			if (i_stream != null)
			{
				i_stream.Send(obj);
			}
		}

		public void SetBlobPath(string path)
		{
			EnsureDirExists(path);
			_config.Put(BLOBPATH, path);
		}

		public void SetClassLoader(object classLoader)
		{
			ReflectWith(com.db4o.@internal.Platform4.CreateReflector(classLoader));
		}

		public void SetMessageRecipient(com.db4o.messaging.MessageRecipient messageRecipient
			)
		{
			_config.Put(MESSAGE_RECIPIENT, messageRecipient);
		}

		public void SetOut(System.IO.TextWriter outStream)
		{
			_config.Put(OUTSTREAM, outStream);
			if (i_stream != null)
			{
				i_stream.LogMsg(19, com.db4o.Db4o.Version());
			}
			else
			{
				com.db4o.@internal.Messages.LogMsg(com.db4o.Db4o.Configure(), 19, com.db4o.Db4o.Version
					());
			}
		}

		public void SingleThreadedClient(bool flag)
		{
			_config.Put(SINGLE_THREADED_CLIENT, flag);
		}

		public void TestConstructors(bool flag)
		{
			_config.Put(TEST_CONSTRUCTORS, flag);
		}

		public void TimeoutClientSocket(int milliseconds)
		{
			_config.Put(TIMEOUT_CLIENT_SOCKET, milliseconds);
		}

		public void TimeoutPingClients(int milliseconds)
		{
			_config.Put(TIMEOUT_PING_CLIENTS, milliseconds);
		}

		public void TimeoutServerSocket(int milliseconds)
		{
			_config.Put(TIMEOUT_SERVER_SOCKET, milliseconds);
		}

		public void Unicode(bool unicodeOn)
		{
			_config.Put(ENCODING, (unicodeOn ? com.db4o.@internal.Const4.UNICODE : com.db4o.@internal.Const4
				.ISO8859));
		}

		public void UpdateDepth(int depth)
		{
			com.db4o.@internal.diagnostic.DiagnosticProcessor dp = DiagnosticProcessor();
			if (dp.Enabled())
			{
				dp.CheckUpdateDepth(depth);
			}
			_config.Put(UPDATE_DEPTH, depth);
		}

		public void UseRamSystem()
		{
			_config.Put(FREESPACE_SYSTEM, com.db4o.@internal.freespace.FreespaceManager.FM_RAM
				);
		}

		public void UseIndexSystem()
		{
			_config.Put(FREESPACE_SYSTEM, com.db4o.@internal.freespace.FreespaceManager.FM_IX
				);
		}

		public void WeakReferenceCollectionInterval(int milliseconds)
		{
			_config.Put(WEAK_REFERENCE_COLLECTION_INTERVAL, milliseconds);
		}

		public void WeakReferences(bool flag)
		{
			_config.Put(WEAK_REFERENCES, flag);
		}

		private com.db4o.foundation.Collection4 Aliases()
		{
			com.db4o.foundation.Collection4 aliasesCollection = (com.db4o.foundation.Collection4
				)_config.Get(ALIASES);
			if (null == aliasesCollection)
			{
				aliasesCollection = new com.db4o.foundation.Collection4();
				_config.Put(ALIASES, aliasesCollection);
			}
			return aliasesCollection;
		}

		public void AddAlias(com.db4o.config.Alias alias)
		{
			if (null == alias)
			{
				throw new System.ArgumentNullException("alias");
			}
			Aliases().Add(alias);
		}

		public void RemoveAlias(com.db4o.config.Alias alias)
		{
			if (null == alias)
			{
				throw new System.ArgumentNullException("alias");
			}
			Aliases().Remove(alias);
		}

		public string ResolveAliasRuntimeName(string runtimeType)
		{
			com.db4o.foundation.Collection4 configuredAliases = Aliases();
			if (null == configuredAliases)
			{
				return runtimeType;
			}
			System.Collections.IEnumerator i = configuredAliases.GetEnumerator();
			while (i.MoveNext())
			{
				string resolved = ((com.db4o.config.Alias)i.Current).ResolveRuntimeName(runtimeType
					);
				if (null != resolved)
				{
					return resolved;
				}
			}
			return runtimeType;
		}

		public string ResolveAliasStoredName(string storedType)
		{
			com.db4o.foundation.Collection4 configuredAliases = Aliases();
			if (null == configuredAliases)
			{
				return storedType;
			}
			System.Collections.IEnumerator i = configuredAliases.GetEnumerator();
			while (i.MoveNext())
			{
				string resolved = ((com.db4o.config.Alias)i.Current).ResolveStoredName(storedType
					);
				if (null != resolved)
				{
					return resolved;
				}
			}
			return storedType;
		}

		internal com.db4o.reflect.ReflectClass ReflectorFor(object clazz)
		{
			clazz = com.db4o.@internal.Platform4.GetClassForType(clazz);
			if (clazz is com.db4o.reflect.ReflectClass)
			{
				return (com.db4o.reflect.ReflectClass)clazz;
			}
			if (clazz is j4o.lang.Class)
			{
				return Reflector().ForClass((j4o.lang.Class)clazz);
			}
			if (clazz is string)
			{
				return Reflector().ForName((string)clazz);
			}
			return Reflector().ForObject(clazz);
		}

		public bool AllowVersionUpdates()
		{
			return _config.GetAsBoolean(ALLOW_VERSION_UPDATES);
		}

		internal bool AutomaticShutDown()
		{
			return _config.GetAsBoolean(AUTOMATIC_SHUTDOWN);
		}

		public byte BlockSize()
		{
			return _config.GetAsByte(BLOCKSIZE);
		}

		public int BTreeNodeSize()
		{
			return _config.GetAsInt(BTREE_NODE_SIZE);
		}

		public int BTreeCacheHeight()
		{
			return _config.GetAsInt(BTREE_CACHE_HEIGHT);
		}

		internal string BlobPath()
		{
			return _config.GetAsString(BLOBPATH);
		}

		internal bool Callbacks()
		{
			return _config.GetAsBoolean(CALLBACKS);
		}

		internal com.db4o.foundation.TernaryBool CallConstructors()
		{
			return _config.GetAsTernaryBool(CALL_CONSTRUCTORS);
		}

		internal bool ClassActivationDepthConfigurable()
		{
			return _config.GetAsBoolean(CLASS_ACTIVATION_DEPTH_CONFIGURABLE);
		}

		internal object ClassLoader()
		{
			return _config.Get(CLASSLOADER);
		}

		internal bool DetectSchemaChanges()
		{
			return _config.GetAsBoolean(DETECT_SCHEMA_CHANGES);
		}

		internal bool CommitRecoveryDisabled()
		{
			return _config.GetAsBoolean(DISABLE_COMMIT_RECOVERY);
		}

		public com.db4o.diagnostic.DiagnosticConfiguration Diagnostic()
		{
			return (com.db4o.diagnostic.DiagnosticConfiguration)_config.Get(DIAGNOSTIC);
		}

		public com.db4o.@internal.diagnostic.DiagnosticProcessor DiagnosticProcessor()
		{
			return (com.db4o.@internal.diagnostic.DiagnosticProcessor)_config.Get(DIAGNOSTIC);
		}

		public int DiscardFreeSpace()
		{
			return _config.GetAsInt(DISCARD_FREESPACE);
		}

		internal byte Encoding()
		{
			return _config.GetAsByte(ENCODING);
		}

		internal bool Encrypt()
		{
			return _config.GetAsBoolean(ENCRYPT);
		}

		public com.db4o.foundation.Hashtable4 ExceptionalClasses()
		{
			com.db4o.foundation.Hashtable4 exceptionalClasses = (com.db4o.foundation.Hashtable4
				)_config.Get(EXCEPTIONAL_CLASSES);
			if (exceptionalClasses == null)
			{
				exceptionalClasses = new com.db4o.foundation.Hashtable4(16);
				_config.Put(EXCEPTIONAL_CLASSES, exceptionalClasses);
			}
			return exceptionalClasses;
		}

		public bool ExceptionsOnNotStorable()
		{
			return _config.GetAsBoolean(EXCEPTIONS_ON_NOT_STORABLE);
		}

		public bool FlushFileBuffers()
		{
			return _config.GetAsBoolean(FLUSH_FILE_BUFFERS);
		}

		internal byte FreespaceSystem()
		{
			return _config.GetAsByte(FREESPACE_SYSTEM);
		}

		public com.db4o.config.ConfigScope GenerateUUIDs()
		{
			return (com.db4o.config.ConfigScope)_config.Get(GENERATE_UUIDS);
		}

		public com.db4o.config.ConfigScope GenerateVersionNumbers()
		{
			return (com.db4o.config.ConfigScope)_config.Get(GENERATE_VERSION_NUMBERS);
		}

		public bool InternStrings()
		{
			return _internStrings;
		}

		public void IsServer(bool flag)
		{
			_config.Put(IS_SERVER, flag);
		}

		internal bool IsServer()
		{
			return _config.GetAsBoolean(IS_SERVER);
		}

		internal bool LockFile()
		{
			return _config.GetAsBoolean(LOCK_FILE);
		}

		internal int MessageLevel()
		{
			return _messageLevel;
		}

		public com.db4o.messaging.MessageRecipient MessageRecipient()
		{
			return (com.db4o.messaging.MessageRecipient)_config.Get(MESSAGE_RECIPIENT);
		}

		internal bool OptimizeNQ()
		{
			return _config.GetAsBoolean(OPTIMIZE_NQ);
		}

		internal string Password()
		{
			return _config.GetAsString(PASSWORD);
		}

		public void PrefetchIDCount(int prefetchIDCount)
		{
			_config.Put(PREFETCH_ID_COUNT, prefetchIDCount);
		}

		public int PrefetchIDCount()
		{
			return _config.GetAsInt(PREFETCH_ID_COUNT);
		}

		public void PrefetchObjectCount(int prefetchObjectCount)
		{
			_config.Put(PREFETCH_OBJECT_COUNT, prefetchObjectCount);
		}

		public int PrefetchObjectCount()
		{
			return _config.GetAsInt(PREFETCH_OBJECT_COUNT);
		}

		internal com.db4o.foundation.Hashtable4 ReadAs()
		{
			return (com.db4o.foundation.Hashtable4)_config.Get(READ_AS);
		}

		public bool IsReadOnly()
		{
			return _readOnly;
		}

		internal com.db4o.foundation.Collection4 Rename()
		{
			return (com.db4o.foundation.Collection4)_config.Get(RENAME);
		}

		internal int ReservedStorageSpace()
		{
			return _config.GetAsInt(RESERVED_STORAGE_SPACE);
		}

		public bool SingleThreadedClient()
		{
			return _config.GetAsBoolean(SINGLE_THREADED_CLIENT);
		}

		internal bool TestConstructors()
		{
			return _config.GetAsBoolean(TEST_CONSTRUCTORS);
		}

		public int TimeoutClientSocket()
		{
			return _config.GetAsInt(TIMEOUT_CLIENT_SOCKET);
		}

		public int TimeoutPingClients()
		{
			return _config.GetAsInt(TIMEOUT_PING_CLIENTS);
		}

		public int TimeoutServerSocket()
		{
			return _config.GetAsInt(TIMEOUT_SERVER_SOCKET);
		}

		internal int UpdateDepth()
		{
			return _config.GetAsInt(UPDATE_DEPTH);
		}

		internal int WeakReferenceCollectionInterval()
		{
			return _config.GetAsInt(WEAK_REFERENCE_COLLECTION_INTERVAL);
		}

		internal bool WeakReferences()
		{
			return _config.GetAsBoolean(WEAK_REFERENCES);
		}

		internal com.db4o.io.IoAdapter IoAdapter()
		{
			return (com.db4o.io.IoAdapter)_config.Get(IOADAPTER);
		}

		public com.db4o.config.QueryConfiguration Queries()
		{
			return this;
		}

		public void EvaluationMode(com.db4o.config.QueryEvaluationMode mode)
		{
			_config.Put(QUERY_EVALUATION_MODE, mode);
		}

		public com.db4o.config.QueryEvaluationMode QueryEvaluationMode()
		{
			return (com.db4o.config.QueryEvaluationMode)_config.Get(QUERY_EVALUATION_MODE);
		}

		public void QueryResultIteratorFactory(com.db4o.@internal.cs.QueryResultIteratorFactory
			 factory)
		{
			_config.Put(CLIENT_QUERY_RESULT_ITERATOR_FACTORY, factory);
		}

		public com.db4o.@internal.cs.QueryResultIteratorFactory QueryResultIteratorFactory
			()
		{
			return (com.db4o.@internal.cs.QueryResultIteratorFactory)_config.Get(CLIENT_QUERY_RESULT_ITERATOR_FACTORY
				);
		}

		public com.db4o.config.ClientServerConfiguration ClientServer()
		{
			return this;
		}

		public void BatchMessages(bool flag)
		{
			_config.Put(BATCH_MESSAGES, flag);
		}

		public bool BatchMessages()
		{
			return _config.GetAsBoolean(BATCH_MESSAGES);
		}

		public void MaxBatchQueueSize(int maxSize)
		{
			_config.Put(MAX_BATCH_QUEUE_SIZE, maxSize);
		}

		public int MaxBatchQueueSize()
		{
			return _config.GetAsInt(MAX_BATCH_QUEUE_SIZE);
		}
	}
}
