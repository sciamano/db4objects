/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

package com.db4o;

import com.db4o.foundation.network.*;

final class MCommit extends Msg {
	final boolean processMessageAtServer(YapSocket in) {
		getTransaction().commit();
		return true;
	}
}