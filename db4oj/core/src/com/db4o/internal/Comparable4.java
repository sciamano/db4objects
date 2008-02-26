/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

package com.db4o.internal;

import com.db4o.foundation.*;
import com.db4o.marshall.*;


/**
 * @exclude
 */
public interface Comparable4 {
	
	PreparedComparison prepareComparison(Context context, Object obj);
	
}

