/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

package com.db4o.test.soda;

import com.db4o.query.*;

public interface STEngine {
	
	public void reset();
	
	public Query query();
	
	public void open();
	
	public void close();

	public void store(Object obj);
	
	public void commit();
	
	public void delete(Object obj);
}

