/* Copyright (C) 2005   db4objects Inc.   http://www.db4o.com */

package com.db4o.reflect.dataobjects;

import com.db4o.*;
import com.db4o.reflect.*;

public class DataObjectReflector implements IReflect {

    private final IReflect _delegate;
    private final Hashtable4 _dataClassByName = new Hashtable4(1);

    public DataObjectReflector(IReflect delegate) {
        _delegate = delegate;
    }

    public IArray array() {
        return _delegate.array();
    }

    public int collectionUpdateDepth(IClass claxx) {
        return _delegate.collectionUpdateDepth(claxx);
    }

    public boolean constructorCallsSupported() {
        return _delegate.constructorCallsSupported();
    }

    public IClass forClass(Class clazz) {
        return _delegate.forClass(clazz);
    }

    public IClass forName(String className) throws ClassNotFoundException {
        IClass dataClass = (IClass)_dataClassByName.get(className);
        return dataClass != null ? dataClass : _delegate.forName(className);
    }

    public IClass forObject(Object a_object) {
        return _delegate.forObject(a_object);
    }

    public boolean isCollection(IClass claxx) {
        return _delegate.isCollection(claxx);
    }

    public void registerCollection(Class clazz) {
        _delegate.registerCollection(clazz);
    }

    public void registerCollectionUpdateDepth(Class clazz, int depth) {
        _delegate.registerCollectionUpdateDepth(clazz, depth);
    }

    public void registerDataClass(DataClass dataClass) {
        _dataClassByName.put(dataClass.getName(), dataClass);
    }

}
