/* Copyright (C) 2004 - 2006  db4objects Inc.  http://www.db4o.com */

package com.db4o.config.annotations.reflect;

import java.lang.annotation.*;
import java.lang.reflect.*;
import java.util.*;

import com.db4o.config.*;
import com.db4o.config.annotations.*;
import com.db4o.internal.*;

/**
 * sets db4o configurations accordingly annotations
 * 
 * @exclude
 * @decaf.ignore
 * @sharpen.ignore
 */
public class ConfigurationIntrospector {

	Map<Class<? extends Annotation>, Db4oConfiguratorFactory> _configurators;

	Config4Class _classConfig;

	Class _clazz;

	Configuration _config;

	public ConfigurationIntrospector(Class clazz, Configuration config,
			Config4Class classConfig) throws Exception {
		this._classConfig = classConfig;
		this._clazz = clazz;
		this._config = config;

		initMap();
	}

	private void initMap() throws NoSuchMethodException {
		_configurators = new HashMap<Class<? extends Annotation>, Db4oConfiguratorFactory>();
		_configurators.put(UpdatedDepth.class, new UpdatedDepthFactory());
		_configurators.put(Indexed.class, new NoArgsFieldConfiguratorFactory(
				IndexedConfigurator.class));
		_configurators.put(CalledConstructor.class, new CalledConstructorFactory());
		_configurators.put(GeneratedUUIDs.class,
				new GeneratedUUIDsFactory());
		_configurators.put(GeneratedVersionNumbers.class,
				new GeneratedVersionNumbersFactory());
		_configurators.put(StoredTransientFields.class,
				new StoredTransientFieldsFactory());
		_configurators.put(PersistedStaticFieldValues.class,
				new PersistedStaticFieldValuesFactory());
	}

	/**
	 * the start methode to reflect user class and fields <br>
	 * in order to set appropriate configurations
	 * 
	 * @param clazz
	 *            Java class to reflect
	 * @return classConfig configurations of class
	 */
	public Config4Class apply() {
		try {
			reflectClass();
			reflectFields();

		} catch (SecurityException e) {
			e.printStackTrace();
		}
		return _classConfig;
	}

	private void reflectClass() {
		Annotation[] annotations = _clazz.getAnnotations();
		for (Annotation a : annotations) {
			applyAnnotation(_clazz, a);
		}
	}

	private void reflectFields() {

		Field[] declaredFields;
		try {
			declaredFields = _clazz.getDeclaredFields();
			for (Field f : declaredFields) {
				for (Annotation a : f.getAnnotations()) {
					applyAnnotation(f, a);
				}
			}
		} 
		catch (SecurityException e) {
			e.printStackTrace();
		}
		catch (NoClassDefFoundError e) {
		}
	}

	private void applyAnnotation(AnnotatedElement element, Annotation a) {
		if (_configurators.containsKey(a.annotationType())) {
			Db4oConfigurator configurator = _configurators.get(
					a.annotationType()).configuratorFor(element, a);
			_classConfig = (Config4Class) configurator.configure(_config);
		}
	}
}