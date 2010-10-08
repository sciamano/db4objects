package com.db4o.drs.versant;

import java.util.*;

import javax.jdo.*;

import com.db4o.drs.versant.VodJdo.PreStoreListener;

public interface VodJdoFacade {

	PersistenceManager persistenceManager();

	long loid(Object obj);

	<T> T objectByLoid(long loid);

	void close();

	void commit();

	void rollback();

	<T> Collection<T> query(Class<T> extent);

	void store(Object obj);

	<T> T peek(T obj);

	int deleteAll(Class clazz);

	void delete(Object obj);

	<T> Collection<T> query(Class<T> clazz, String filter);

	<T> T queryOneOrNone(Class<T> clazz, String filter);

	<T> T queryOne(Class<T> clazz, String filter);

	void refresh(Object obj);

	void addPreStoreListener(PreStoreListener preStoreListener);

}