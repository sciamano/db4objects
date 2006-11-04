/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

package com.db4o.cs.messages;

import com.db4o.cs.*;

public final class MTaIsDeleted extends MsgD {
	
	public final boolean processAtServer(YapServerThread serverThread) {
		synchronized (streamLock()) {
			boolean isDeleted = transaction().isDeleted(readInt());
			int ret = isDeleted ? 1 : 0;
			serverThread.write(Msg.TA_IS_DELETED.getWriterForInt(transaction(), ret));
		}
		return true;
	}
}