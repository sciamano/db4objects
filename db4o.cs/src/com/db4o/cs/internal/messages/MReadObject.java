/* Copyright (C) 2004   Versant Inc.   http://www.db4o.com */

package com.db4o.cs.internal.messages;

import com.db4o.internal.*;

public final class MReadObject extends MsgD implements MessageWithResponse {
	
	public final boolean processAtServer() {
		StatefulBuffer bytes = null;
		// readWriterByID may fail in certain cases, for instance if
		// and object was deleted by another client
		int id = _payLoad.readInt();
		int lastCommitted = _payLoad.readInt();
		synchronized (streamLock()) {
			bytes = stream().readWriterByID(transaction(), id, lastCommitted==1);
		}
		if (bytes == null) {
			bytes = new StatefulBuffer(transaction(), 0, 0);
		}
		write(Msg.OBJECT_TO_CLIENT.getWriter(bytes));
		return true;
	}
}