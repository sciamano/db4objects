/* Copyright (C) 2004 - 2006  Versant Inc.  http://www.db4o.com */

package com.db4o.config.annotations.reflect;

import java.lang.annotation.*;
import java.lang.reflect.*;

import com.db4o.config.annotations.*;

/**
 * @exclude
 * @sharpen.ignore
 */
@decaf.Ignore
public class StoredTransientFieldsFactory implements Db4oConfiguratorFactory {

	public Db4oConfigurator configuratorFor(AnnotatedElement element,
			Annotation annotation) {
		if (!annotation.annotationType().equals(StoredTransientFields.class)) {
			return null;
		}
		String className=null;
		
		if(element instanceof Class) {
			className=((Class)element).getName();
		}
		
		boolean value= ((StoredTransientFields) annotation).value();
		return new StoredTransientFieldsConfigurator(className, value);
	}

}
