/* Copyright (C) 2009 Versant Inc. http://www.db4o.com */

package com.db4o.monitoring;

/**
 * @since 7.12
 */ 
@decaf.Ignore(unlessCompatible=decaf.Platform.JDK15)
public interface ResettableMBean {
	void resetCounters();
}
