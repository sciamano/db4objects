/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

package com.db4o;

import com.db4o.foundation.network.*;

final class MGetAll extends Msg {
	final boolean processMessageAtServer(YapSocket sock) {
		QResult qr;
		YapStream stream = getStream();
		synchronized (stream.i_lock) {
			try {
				qr = stream.get1(getTransaction(), null)._delegate;
			} catch (Exception e) {
				qr = new QResult(getTransaction());
			}
		}
		this.writeQueryResult(getTransaction(), qr, sock);
		return true;
	}
}