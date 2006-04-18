package com.db4o.test.replication.transients;

import com.db4o.ObjectSet;
import com.db4o.ext.Db4oUUID;
import com.db4o.foundation.Collection4;
import com.db4o.foundation.ObjectSetCollection4Facade;
import com.db4o.foundation.Visitor4;
import com.db4o.inside.replication.CollectionHandler;
import com.db4o.inside.replication.CollectionHandlerImpl;
import com.db4o.inside.replication.ReadonlyReplicationProviderSignature;
import com.db4o.inside.replication.ReplicationReference;
import com.db4o.inside.replication.ReplicationReflector;
import com.db4o.inside.replication.TestableReplicationProvider;
import com.db4o.inside.replication.TestableReplicationProviderInside;
import com.db4o.inside.traversal.GenericTraverser;
import com.db4o.inside.traversal.Traverser;
import com.db4o.inside.traversal.Visitor;
import com.db4o.reflect.Reflector;

import java.util.IdentityHashMap;
import java.util.Iterator;
import java.util.Map;

public class TransientReplicationProvider implements TestableReplicationProvider, TestableReplicationProviderInside {
// ------------------------------ FIELDS ------------------------------

	private final String _name;

	private long nextObjectId = 1;

	private final Traverser _traverser;

	private final Map _storedObjects = new IdentityHashMap();
	private final Map _activatedObjects = new IdentityHashMap();

	private final Map _referencesByObject = new IdentityHashMap();

	private final MySignature _signature;
	private ReadonlyReplicationProviderSignature _peerSignature;

	private long _lastReplicationVersion;

	private Collection4 _uuidsDeletedSinceLastReplication = new Collection4();

// --------------------------- CONSTRUCTORS ---------------------------

	public TransientReplicationProvider(byte[] signature) {
		this(signature, null);
	}

	public TransientReplicationProvider(byte[] signature, String name) {
		_signature = new MySignature(signature);
		_name = name;

		ReplicationReflector reflector = ReplicationReflector.getInstance();
		CollectionHandler _collectionHandler = new CollectionHandlerImpl(reflector.reflector());
		_traverser = new MyTraverser(reflector.reflector(), _collectionHandler);
	}

// ------------------------ CANONICAL METHODS ------------------------

	public String toString() {
		return _name;
	}

// ------------------------ INTERFACE METHODS ------------------------

// --------------------- Interface ReplicationProvider ---------------------


	public Object getObject(Db4oUUID uuid) {
		ObjectSet iter = getStoredObjects();
		while (iter.hasNext()) {
			Object candidate = iter.next();
			if (getInfo(candidate)._uuid.equals(uuid)) return candidate;
		}
		return null;
	}

	public ObjectSet objectsChangedSinceLastReplication() {
		return objectsChangedSinceLastReplication(Object.class);
	}

	public ObjectSet objectsChangedSinceLastReplication(Class clazz) {
		Collection4 result = new Collection4();
		for (ObjectSet iterator = getStoredObjects(clazz); iterator.hasNext();) {
			Object candidate = iterator.next();
			if (wasChangedSinceLastReplication(candidate))
				result.add(candidate);
		}
		return new ObjectSetCollection4Facade(result);
	}

	public ObjectSet uuidsDeletedSinceLastReplication() {
		return new ObjectSetCollection4Facade(_uuidsDeletedSinceLastReplication);
	}

// --------------------- Interface ReplicationProviderInside ---------------------


	public void activate(Object object) {
		_activatedObjects.put(object, object);
	}

	public void clearAllReferences() {
		_referencesByObject.clear();
	}

	public void commitReplicationTransaction(long raisedDatabaseVersion) {
		_uuidsDeletedSinceLastReplication.clear();
	}

	public void destroy() {
		// do nothing
	}

	public long getCurrentVersion() {
		return _lastReplicationVersion + 1;
	}

	public Object getMonitor() {
		return this;
	}

	public String getName() {
		return _name;
	}

	public ReadonlyReplicationProviderSignature getSignature() {
		return _signature;
	}

	public boolean hasReplicationReferenceAlready(Object obj) {
		return getCachedReference(obj) != null;
	}

	public ReplicationReference produceReference(Object obj, Object unused, String unused2) {
		ReplicationReference cached = getCachedReference(obj);
		if (cached != null) return cached;

		if (!isStored(obj)) return null;

		return createReferenceFor(obj);
	}

	public ReplicationReference produceReferenceByUUID(Db4oUUID uuid, Class hintIgnored) {
		if (uuid == null) {
			return null;
		}
		Object object = getObject(uuid);
		if (object == null) return null;
		return produceReference(object, null, null);
	}

	public ReplicationReference referenceNewObject(Object obj, ReplicationReference counterpartReference, ReplicationReference unused, String unused2) {
		//System.out.println("referenceNewObject: " + obj + "  UUID: " + counterpartReference.uuid());
		Db4oUUID uuid = counterpartReference.uuid();
		long version = counterpartReference.version();

		if (getObject(uuid) != null) throw new RuntimeException("Object exists already.");

		ReplicationReference result = createReferenceFor(obj);
		store(obj, uuid, version);
		return result;
	}

	public void replicateDeletion(ReplicationReference reference) {
		_storedObjects.remove(reference.object());
	}

	public void rollbackReplication() {
		throw new UnsupportedOperationException();
	}

	public void startReplicationTransaction(ReadonlyReplicationProviderSignature peerSignature) {
		if (_peerSignature != null)
			if (! _peerSignature.equals(peerSignature)) {
				throw new IllegalArgumentException("This provider can only replicate with a single peer.");
			}
		_peerSignature = peerSignature;
	}

	public void storeReplica(Object obj) {
		ReplicationReference ref = getCachedReference(obj);
		if (ref == null) {
			throw new RuntimeException();
		}
		store(obj, ref.uuid(), ref.version());
	}

	public void syncVersionWithPeer(long version) {
		_lastReplicationVersion = version;
	}

	public void updateCounterpart(Object obj) {
		storeReplica(obj);
	}

	public void visitCachedReferences(Visitor4 visitor) {
		Iterator i = _referencesByObject.values().iterator();
		while (i.hasNext()) {
			visitor.visit(i.next());
		}
	}

	public boolean wasDeletedSinceLastReplication(Db4oUUID uuid) {
		return _uuidsDeletedSinceLastReplication.contains(uuid);
	}

	public boolean wasModifiedSinceLastReplication(ReplicationReference reference) {
		return reference.version() > _lastReplicationVersion;
	}

// --------------------- Interface SimpleObjectContainer ---------------------


	public void commit() {
		// do nothing
	}

	public void delete(Object obj) {
		Db4oUUID uuid = produceReference(obj, null, null).uuid();
		_uuidsDeletedSinceLastReplication.add(uuid);
		_storedObjects.remove(obj);
	}

	public void deleteAllInstances(Class clazz) {
		ObjectSet iterator = getStoredObjects(clazz);
		while (iterator.hasNext()) delete(iterator.next());
	}

	public ObjectSet getStoredObjects(Class clazz) {
		Collection4 result = new Collection4();
		for (Iterator iterator = _storedObjects.keySet().iterator(); iterator.hasNext();) {
			Object candidate = iterator.next();
			if (clazz.isAssignableFrom(candidate.getClass()))
				result.add(candidate);
		}
		return new ObjectSetCollection4Facade(result);
	}

	public void storeNew(Object o) {
		_traverser.traverseGraph(o, new Visitor() {
			public boolean visit(Object obj) {
				if (isStored(obj)) return false;
				transientProviderSpecificStore(obj);
				return true;
			}
		});
	}

	public void update(Object o) {
		//TODO klaus, did you forget to traverse?
		transientProviderSpecificStore(o);
	}

// --------------------- Interface TestableReplicationProviderInside ---------------------


	public boolean supportsHybridCollection() {
		return true;
	}

	public boolean supportsRollback() {
		return false;
	}

	public boolean supportsCascadeDelete() {
		return false;
	}

	public boolean supportsMultiDimensionalArrays() {
		return true;
	}

	public Map activatedObjects() {
		return _activatedObjects;
	}

	private ReplicationReference createReferenceFor(Object obj) {
		MyReplicationReference result = new MyReplicationReference(obj);
		_referencesByObject.put(obj, result);
		return result;
	}

	private ReplicationReference getCachedReference(Object obj) {
		return (ReplicationReference) _referencesByObject.get(obj);
	}

	private ObjectInfo getInfo(Object candidate) {
		return (ObjectInfo) _storedObjects.get(candidate);
	}

	public long getLastReplicationVersion() {
		return _lastReplicationVersion;
	}

	public ObjectSet getStoredObjects() {
		return getStoredObjects(Object.class);
	}

	private boolean isStored(Object obj) {
		return getInfo(obj) != null;
	}

	private void store(Object obj, Db4oUUID uuid, long version) {
		if (obj == null) throw new RuntimeException();
		_storedObjects.put(obj, new ObjectInfo(uuid, version));
	}

	public void transientProviderSpecificStore(Object obj) {
		ObjectInfo info = getInfo(obj);
		if (info == null)
			store(obj, new Db4oUUID(nextObjectId++, _signature.getBytes()), getCurrentVersion());
		else
			info._version = getCurrentVersion();
	}

	private boolean wasChangedSinceLastReplication(Object candidate) {
		return getInfo(candidate)._version > _lastReplicationVersion;
	}

// -------------------------- INNER CLASSES --------------------------

	public class MySignature implements ReadonlyReplicationProviderSignature {
		private final byte[] _bytes;

		public MySignature(byte[] signature) {
			_bytes = signature;
		}

		public long getId() {
			throw new RuntimeException("Never used?");
		}

		public byte[] getBytes() {
			return _bytes;
		}

		public long getCreationTime() {
			throw new RuntimeException("Never used?");
		}
	}

	private class MyReplicationReference implements ReplicationReference {
		private final Object _object;
		private Object _counterpart;
		private boolean _isMarkedForReplicating;
		private boolean _isMarkedForDeleting;

		MyReplicationReference(Object object) {
			if (object == null) throw new IllegalArgumentException();
			_object = object;
		}

		public Object object() {
			return _object;
		}

		public Object counterpart() {
			return _counterpart;
		}

		public long version() {
			return getInfo(_object)._version;
		}

		public Db4oUUID uuid() {
			return getInfo(_object)._uuid;
		}

		public void setCounterpart(Object obj) {
			_counterpart = obj;
		}

		public void markForReplicating() {
			_isMarkedForReplicating = true;
		}

		public boolean isMarkedForReplicating() {
			return _isMarkedForReplicating;
		}

		public void markForDeleting() {
			_isMarkedForDeleting = true;
		}

		public boolean isMarkedForDeleting() {
			return _isMarkedForDeleting;
		}

		boolean objectIsNew;

		public void markCounterpartAsNew() {
			objectIsNew = true;
		}

		public boolean isCounterpartNew() {
			return objectIsNew;
		}
	}

	private static class ObjectInfo {
		private final Db4oUUID _uuid;
		private long _version;

		public ObjectInfo(Db4oUUID uuid, long version) {
			_uuid = uuid;
			_version = version;
		}
	}

	public class MyTraverser implements Traverser {
		Traverser _delegate;

		public MyTraverser(Reflector reflector, CollectionHandler collectionHandler) {
			_delegate = new GenericTraverser(reflector, collectionHandler);
		}

		public void traverseGraph(Object object, Visitor visitor) {
			_delegate.traverseGraph(object, visitor);
		}

		public void extendTraversalTo(Object disconnected, Visitor visitor) {
			_delegate.extendTraversalTo(disconnected, visitor);
		}
	}
}
