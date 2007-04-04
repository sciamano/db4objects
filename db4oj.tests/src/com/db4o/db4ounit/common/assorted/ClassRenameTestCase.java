/* Copyright (C) 2007  db4objects Inc.  http://www.db4o.com */

package com.db4o.db4ounit.common.assorted;

import com.db4o.config.*;

import db4ounit.*;
import db4ounit.extensions.*;


public class ClassRenameTestCase extends AbstractDb4oTestCase{

	public static void main(String[] args) {
		new ClassRenameTestCase().runClientServer();
	}
	
	
    public static class Original {
    	
        public String originalName;

        public Original() {

        }

        public Original(String name) {
            originalName = name;
        }
    }

    public static class Changed {

        public String changedName;

    }

	
	public void test() throws Exception{
		
		store(new Original("original"));
		
		db().commit();
		
		Assert.areEqual(1, countOccurences(Original.class));
		
        // Rename messages are visible at level 1
        // fixture().config().messageLevel(1);
		
        ObjectClass oc = fixture().config().objectClass(Original.class);

        // allways rename fields first
        oc.objectField("originalName").rename("changedName");
        oc.rename(Changed.class.getName());

        reopen();
        
        Assert.areEqual(0, countOccurences(Original.class));
        Assert.areEqual(1, countOccurences(Changed.class));
        
        Changed changed = (Changed) retrieveOnlyInstance(Changed.class);
        
        Assert.areEqual("original", changed.changedName);

	}

}
