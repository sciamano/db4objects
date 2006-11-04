/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

package com.db4o.cs.messages;

import com.db4o.cs.*;

final class MCommit extends Msg {
	public final boolean processAtServer(YapServerThread serverThread) {
		transaction().commit();
		return true;
	}
}