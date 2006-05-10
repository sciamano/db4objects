package com.db4o.test.replication.provider;

import com.db4o.test.Test;
import com.db4o.test.replication.SPCChild;
import com.db4o.ext.ExtObjectContainer;
import com.db4o.ext.ObjectInfo;

public class Db4oObjectUpdateTest {
	public void test(){
		ExtObjectContainer container = Test.objectContainer();

		SPCChild child = new SPCChild("c1");
		container.set(child);
		container.commit();

		ObjectInfo objectInfo1 = container.getObjectInfo(child);
		long oldVer = objectInfo1.getVersion();

		child.setName("c3");

		container.set(child);
		container.commit();

		ObjectInfo objectInfo2 = container.getObjectInfo(child);
		long newVer = objectInfo2.getVersion();

		Test.ensureEquals(objectInfo1.getUUID(), objectInfo2.getUUID());
		Test.ensure(newVer > oldVer);
	}
}
