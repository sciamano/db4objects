package com.db4o.test.unit.test;

import com.db4o.test.unit.Assert;
import com.db4o.test.unit.db4o.*;

public class SimpleDb4oTestCase extends Db4oTestCase {
	private static class Data {}
	
	private boolean[] _everythingCalled=new boolean[3];
	
	protected void configure() {
		Assert.isTrue(everythingCalledBefore(0));
		_everythingCalled[0]=true;
	}
	
	protected void store() {
		Assert.isTrue(everythingCalledBefore(1));
		_everythingCalled[1]=true;
		fixture().db().set(new Data());
	}
	
	public void testResultSize() {
		Assert.isTrue(everythingCalledBefore(2));
		_everythingCalled[2]=true;
		Assert.areEqual(1,fixture().db().query(Data.class).size());
	}
	
	public boolean everythingCalled() {
		return everythingCalledBefore(_everythingCalled.length);
	}

	public boolean everythingCalledBefore(int idx) {
		for (int i = 0; i < idx; i++) {
			if(!_everythingCalled[i]) {
				return false;
			}
		}
		for (int i = idx; i < _everythingCalled.length; i++) {
			if(_everythingCalled[i]) {
				return false;
			}
		}
		return true;
	}
}
