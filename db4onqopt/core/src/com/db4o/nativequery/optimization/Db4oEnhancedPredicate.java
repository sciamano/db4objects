/* Copyright (C) 2004 - 2006  db4objects Inc.  http://www.db4o.com */

package com.db4o.nativequery.optimization;

import com.db4o.query.*;

public interface Db4oEnhancedPredicate {
	void optimizeQuery(Query query);
}
