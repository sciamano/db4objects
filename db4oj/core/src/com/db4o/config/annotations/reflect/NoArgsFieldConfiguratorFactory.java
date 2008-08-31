/* Copyright (C) 2004 - 2006  db4objects Inc.  http://www.db4o.com */

package com.db4o.config.annotations.reflect;

import java.lang.annotation.*;
import java.lang.reflect.*;


/**
 * @exclude
 * @decaf.ignore
 * @sharpen.ignore
 */
public class NoArgsFieldConfiguratorFactory implements Db4oConfiguratorFactory {
	private Constructor _constructor;

	public NoArgsFieldConfiguratorFactory(Class<?> configuratorClass) throws NoSuchMethodException {
		_constructor=configuratorClass.getConstructor(new Class[]{String.class,String.class});
	}

	public Db4oConfigurator configuratorFor(AnnotatedElement element, Annotation annotation) {
		try {
			if(!(element instanceof Field)) {
				return null;
			}
			Field field=(Field)element;
			String className=field.getDeclaringClass().getName();
			String fieldName=field.getName();
			return (Db4oConfigurator)_constructor.newInstance(new Object[]{className,fieldName});
		} catch (Exception exc) {
			return null;
		}
	}
}