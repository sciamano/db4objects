/* Copyright (C) 2006 db4objects Inc. http://www.db4o.com */

package com.db4o.db4ounit.jre12.handlers;

import db4ounit.extensions.Db4oTestSuite;

/**
 * @decaf.ignore.jdk11
 */
public class AllTests extends Db4oTestSuite {

	protected Class[] testCases() {
		return new Class[] { 
				TypeHandlerConfigurationTestCase.class, 
		};
	}

}