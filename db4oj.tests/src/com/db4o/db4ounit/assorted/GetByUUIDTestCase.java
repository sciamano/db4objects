/* Copyright (C) 2004 - 2005  db4objects Inc.  http://www.db4o.com */

package com.db4o.db4ounit.assorted;

import com.db4o.Db4o;
import com.db4o.foundation.Hashtable4;

import db4ounit.extensions.Db4oTestCase;

public class GetByUUIDTestCase extends Db4oTestCase {
	
	public static void main(String[] args) {
		new GetByUUIDTestCase().runSolo();
	}

	protected void configure() {
		Db4o.configure().objectClass(UUIDTestItem.class).generateUUIDs(true);
	}

	protected void store() {
		db().set(new UUIDTestItem("one"));
		db().set(new UUIDTestItem("two"));
	}

	public void test() throws Exception {
		
		Hashtable4 uuidCache = new Hashtable4();
		
		assertItemsCanBeRetrievedByUUID(uuidCache);
		
		reopen();
		
		assertItemsCanBeRetrievedByUUID(uuidCache);
	}

	private void assertItemsCanBeRetrievedByUUID(Hashtable4 uuidCache) {
		UUIDTestItem.assertItemsCanBeRetrievedByUUID(db(), uuidCache);
	}
}
