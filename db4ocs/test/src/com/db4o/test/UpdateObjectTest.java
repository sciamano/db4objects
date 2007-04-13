/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

package com.db4o.test;

import com.db4o.*;
import com.db4o.ext.*;
import com.db4o.query.*;
import com.db4o.test.config.*;
import com.db4o.test.persistent.*;

import db4ounit.*;

public class UpdateObjectTest extends ClientServerTestCase {

	private static String testString = "simple test string";

	protected void store(ExtObjectContainer oc) throws Exception {
		for (int i = 0; i < TestConfigure.CONCURRENCY_THREAD_COUNT; i++) {
			oc.set(new SimpleObject(testString + i, i));
		}

	}

	public void concUpdateSameObject(ExtObjectContainer oc, int seq)
			throws Exception {

		int mid = TestConfigure.CONCURRENCY_THREAD_COUNT / 2;
		Query query = oc.query();
		query.descend("_s").constrain(testString + mid);
		ObjectSet result = query.execute();
		Assert.areEqual(1, result.size());
		SimpleObject o = (SimpleObject) result.next();
		o.setI(TestConfigure.CONCURRENCY_THREAD_COUNT + seq);
		oc.set(o);

	}

	public void checkUpdateSameObject(ExtObjectContainer oc) throws Exception {

		int mid = TestConfigure.CONCURRENCY_THREAD_COUNT / 2;
		Query query = oc.query();
		query.descend("_s").constrain(testString + mid);
		ObjectSet result = query.execute();
		Assert.areEqual(1, result.size());
		SimpleObject o = (SimpleObject) result.next();
		int i = o.getI();
		Assert.isTrue(TestConfigure.CONCURRENCY_THREAD_COUNT <= i
				&& i <= 2 * TestConfigure.CONCURRENCY_THREAD_COUNT);

	}

	public void concUpdateDifferentObject(ExtObjectContainer oc, int seq)
			throws Exception {

		Query query = oc.query();
		query.descend("_s").constrain(testString + seq).and(
				query.descend("_i").constrain(new Integer(seq)));
		ObjectSet result = query.execute();
		Assert.areEqual(1, result.size());
		SimpleObject o = (SimpleObject) result.next();
		o.setI(seq + 1);
		oc.set(o);

	}

	public void checkUpdateDifferentObject(ExtObjectContainer oc)
			throws Exception {

		ObjectSet result = oc.query(SimpleObject.class);
		Assert.areEqual(TestConfigure.CONCURRENCY_THREAD_COUNT, result.size());
		while (result.hasNext()) {
			SimpleObject o = (SimpleObject) result.next();
			int i = o.getI() - 1;
			Assert.areEqual(testString + i, o.getS());
		}

	}

}
