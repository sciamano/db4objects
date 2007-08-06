/* Copyright (C) 2004 - 2006 db4objects Inc. http://www.db4o.com */

package com.db4o.db4ounit.common.assorted;

import com.db4o.*;
import com.db4o.ext.*;
import com.db4o.foundation.Hashtable4;
import com.db4o.internal.*;
import com.db4o.query.Query;

import db4ounit.Assert;
import db4ounit.extensions.AbstractDb4oTestCase;

public class DatabaseUnicityTest extends AbstractDb4oTestCase  {

	public void test() {
        Hashtable4 ht = new Hashtable4();
        ObjectContainerBase container = stream();
        container.showInternalClasses(true);
        Query q = db().query();
        q.constrain(Db4oDatabase.class);
        ObjectSet objectSet = q.execute();
        while (objectSet.hasNext()) {
        	Db4oDatabase identity = (Db4oDatabase) objectSet.next();
        	Assert.isFalse(ht.containsKey(identity.i_signature));
        	ht.put(identity.i_signature, "");
        }
        container.showInternalClasses(false);
    }

}
