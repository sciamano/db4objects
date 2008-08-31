package com.db4o.db4ounit.jre12.concurrency;

import com.db4o.db4ounit.common.concurrency.*;
import com.db4o.db4ounit.jre12.querying.*;

import db4ounit.extensions.*;

/**
 * @decaf.ignore.jdk11
 * @sharpen.ignore
 */
public class AllTestsJdk1_2 extends Db4oConcurrencyTestSuite {

	public static void main(String[] args) {
		System.exit(new AllTestsJdk1_2().runConcurrency());
    }

	protected Class[] testCases() {
		return new Class[] {
			com.db4o.db4ounit.jre11.concurrency.AllTests.class,
			ReadCollectionNQTestCase.class,
			ReadCollectionQBETestCase.class,
			ReadCollectionSODATestCase.class,
			UpdateCollectionTestCase.class,
			ObjectSetCollectionAPITestCase.class,
		};
	}
}