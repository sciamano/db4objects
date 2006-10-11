/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

package com.db4o;

/**
 * @exclude
 * @persistent
 */
public class User implements Internal4{
	public String name;
	public String password;
	
	public User() {
	}
	
	public User(String name, String password) {
		this.name = name;
		this.password = password;
	}
}

