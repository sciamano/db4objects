/* Copyright (C) 2010  Versant Inc.   http://www.db4o.com */

package com.db4o.omplus;

public interface ErrorMessageSink {
	void showError(String msg);
	void logExc(Throwable exc);
}
