package com.db4o.drs.inside.traversal;

import com.db4o.foundation.*;
import com.db4o.internal.handlers.*;
import com.db4o.reflect.*;

public class GenericTraverser implements Traverser {
	protected final Reflector _reflector;
	protected final CollectionHandler _collectionHandler;
	protected final Queue4 _queue = new NonblockingQueue();

	public GenericTraverser(Reflector reflector, CollectionHandler collectionHandler) {
		_reflector = reflector;
		_collectionHandler = collectionHandler;
	}

	public void traverseGraph(Object object, Visitor visitor) {
		queueUpForTraversing(object);
		while (true) {
			Object next = _queue.next();
			if (next == null) return;
			traverseObject(next, visitor);
		}
	}

	protected void traverseObject(Object object, Visitor visitor) {
		if (!visitor.visit(object))
			return;

		ReflectClass claxx = _reflector.forObject(object);
		traverseAllFields(object, claxx);
	}

	protected void traverseAllFields(Object object, ReflectClass claxx) {
		traverseFields(object, claxx);
		traverseSuperclass(object, claxx);
	}

	private void traverseSuperclass(Object object, ReflectClass claxx) {
		ReflectClass superclass = claxx.getSuperclass();
		if (superclass == null) return;
		traverseAllFields(object, superclass);
	}

	private void traverseFields(Object object, ReflectClass claxx) {
		final Iterator4 fields = FieldIterators.persistentFields(claxx);
		while (fields.moveNext()) {
			final ReflectField field = (ReflectField) fields.current();
			field.setAccessible(); //TODO Optimize: Change the reflector so I dont have to call setAcessible all the time.
			Object value = field.get(object);
			queueUpForTraversing(value);
		}
	}

	protected void traverseCollection(Object collection) {
		Iterator4 elements = _collectionHandler.iteratorFor(collection); //TODO Optimization: visit instead of flattening.
		while (elements.moveNext()) {
			Object element = elements.current();
			if (element == null) continue;
			queueUpForTraversing(element);
		}
	}

	protected void traverseArray(ReflectClass claxx, Object array) {
		Iterator4 contents = ArrayHandler.iterator(claxx, array);
		while (contents.moveNext()) {
			queueUpForTraversing(contents.current());
		}
	}

	protected void queueUpForTraversing(Object object) {
		if (object == null) {
			return;
		}

		ReflectClass claxx = _reflector.forObject(object);
		if (isSecondClass(claxx)) {
			return;
		}

		if (_collectionHandler.canHandle(claxx)) {
			traverseCollection(object);
			return;
		}
		
		if (claxx.isArray()) {
			traverseArray(claxx, object);
			return;
		}
		
		queueAdd(object);
	}

	protected void queueAdd(Object object) {
		_queue.add(object);
	}

	protected boolean isSecondClass(ReflectClass claxx) {
		//      TODO Optimization: Compute this lazily in ReflectClass;
		if (claxx.isSecondClass()) return true;
		return claxx.isArray() && claxx.getComponentType().isSecondClass();
	}

	public void extendTraversalTo(Object disconnected) {
		queueUpForTraversing(disconnected);
	}
}