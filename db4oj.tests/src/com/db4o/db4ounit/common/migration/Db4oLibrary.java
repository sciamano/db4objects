/* Copyright (C) 2004 - 2006 db4objects Inc. http://www.db4o.com */

package com.db4o.db4ounit.common.migration;


/**
 * @decaf.ignore.jdk11
 */
public class Db4oLibrary {
	
	public final String path;
	
	public final Db4oLibraryEnvironment environment;
	
	public Db4oLibrary(String path, Db4oLibraryEnvironment environment) {
		this.path = path;
		this.environment = environment;
	}
}