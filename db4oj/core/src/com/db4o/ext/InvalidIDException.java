/* Copyright (C) 2007 db4objects Inc. http://www.db4o.com */
package com.db4o.ext;

/**
 * db4o-specific exception.<br><br>
 * This exception is thrown when the supplied object ID
 * is incorrect (wrong format).
 * @see com.db4o.ext.ExtObjectContainer#bind(Object, long)
 * @see com.db4o.ext.ExtObjectContainer#getByID(long)
 */
public class InvalidIDException extends Db4oException {
	
	/**
	 * Constructor allowing to specify the exception cause
	 * @param e cause exception
	 */
	public InvalidIDException(Throwable e) {
		super(e);
	}
}
