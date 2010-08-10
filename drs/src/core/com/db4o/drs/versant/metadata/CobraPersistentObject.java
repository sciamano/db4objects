/* Copyright (C) 2004 - 2010  Versant Inc.  http://www.db4o.com */

package com.db4o.drs.versant.metadata;

public abstract class CobraPersistentObject {
	
	private transient long _loid;
	
	public long loid(){
		return _loid;
	}
	
	public void loid(long loid){
		_loid = loid;
	}

}
