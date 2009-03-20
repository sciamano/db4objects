/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

package com.db4o.internal;

import com.db4o.foundation.*;
import com.db4o.reflect.*;

/** @exclude */
public final class EventDispatchers {

	public static final EventDispatcher NULL_DISPATCHER = new EventDispatcher() {
		public boolean dispatch(Transaction trans, Object obj, int eventID) {
			return true;
		}

		public boolean hasEventRegistered(int eventID) {
			return false;
		}
	};

	private static final String[] events = {
		"objectCanDelete",
		"objectOnDelete",
		"objectOnActivate",
		"objectOnDeactivate",
		"objectOnNew",
		"objectOnUpdate",
		"objectCanActivate",
		"objectCanDeactivate",
	    "objectCanNew",
	    "objectCanUpdate"
	};

	static final int CAN_DELETE = 0;
	static final int DELETE = 1;
	static final int ACTIVATE = 2;
	static final int DEACTIVATE = 3;
	static final int NEW = 4;
	public static final int UPDATE = 5;
	static final int CAN_ACTIVATE = 6;
	static final int CAN_DEACTIVATE = 7;
	static final int CAN_NEW = 8;
	static final int CAN_UPDATE = 9;
	
	static final int SERVER_COUNT = 2;
	static final int COUNT = 10;

	private static class EventDispatcherImpl implements EventDispatcher {
		private final ReflectMethod[] methods;

		public EventDispatcherImpl(ReflectMethod[] methods_) {
			methods = methods_;
		}

		public boolean hasEventRegistered(int eventID) {
			return methods[eventID] != null;
		}

		public boolean dispatch(Transaction trans, Object obj, int eventID) {
			if (methods[eventID] == null) {
				return true;
			}
			Object[] parameters = new Object[] { trans.objectContainer() };
			ObjectContainerBase container = trans.container();
			int stackDepth = container.stackDepth();
			int topLevelCallId = container.topLevelCallId();
			container.stackDepth(0);
			try {
				Object res = methods[eventID].invoke(obj, parameters);
				if (res instanceof Boolean) {
					return ((Boolean) res).booleanValue();
				}
			} finally {
				container.stackDepth(stackDepth);
				container.topLevelCallId(topLevelCallId);
			}
			return true;
		}
	}

	public static EventDispatcher forClass(ObjectContainerBase container, ReflectClass classReflector) {

		if (container == null || classReflector == null) {
			throw new ArgumentNullException();
		}
		
		if (!container.dispatchsEvents()) {
			return NULL_DISPATCHER;
		}

		int count = eventCountFor(container);
		if (count == 0) {
			return NULL_DISPATCHER;
		}
			
		final ReflectMethod[] handlers = eventHandlerTableFor(container, classReflector);
		return hasEventHandler(handlers)
			? new EventDispatcherImpl(handlers)
			: NULL_DISPATCHER;
	}

	private static ReflectMethod[] eventHandlerTableFor(ObjectContainerBase container, ReflectClass classReflector) {
	    ReflectClass[] parameterClasses = { container._handlers.ICLASS_OBJECTCONTAINER };
		ReflectMethod[] methods = new ReflectMethod[COUNT];
		for (int i = COUNT - 1; i >= 0; i--) {
			ReflectMethod method = classReflector.getMethod(events[i], parameterClasses);
			if (null == method) {
				method = classReflector.getMethod(toPascalCase(events[i]), parameterClasses);
			}
			if (method != null) {
				methods[i] = method;
			}
		}
	    return methods;
    }

	private static boolean hasEventHandler(ReflectMethod[] methods) {
	    return Iterators.any(Iterators.iterate(methods), new Predicate4() {

			public boolean match(Object candidate) {
	           return candidate != null;
            }});
    }

	private static int eventCountFor(ObjectContainerBase container) {
	    if (container.configImpl().callbacks()) {
			return COUNT;
		}
		if (container.configImpl().isServer()) {
			return SERVER_COUNT;
		}
	    return 0;
    }
	
	private static String toPascalCase(String name) {
		return name.substring(0, 1).toUpperCase() + name.substring(1);
	}
}