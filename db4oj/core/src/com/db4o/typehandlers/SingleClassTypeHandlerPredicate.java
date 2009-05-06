/* Copyright (C) 2008  Versant Inc.  http://www.db4o.com */

package com.db4o.typehandlers;

import com.db4o.reflect.*;

/**
 * allows installing a Typehandler for a single class.
 */
public final class SingleClassTypeHandlerPredicate implements TypeHandlerPredicate {
    
	private final Class _class;
	
	public SingleClassTypeHandlerPredicate(Class clazz) {
		_class = clazz;
	}
	
	public boolean match(ReflectClass classReflector) {
		final ReflectClass reflectClass = classReflector.reflector().forClass(_class);
		return classReflector == reflectClass;
	}
}