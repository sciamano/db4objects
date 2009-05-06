/* Copyright (C) 2004 - 20067 Versant Inc. http://www.db4o.com */

package com.db4o.db4ounit.common.cs;

import db4ounit.extensions.*;

public class AllTests extends Db4oTestSuite {

	public static void main(String[] args) {
		new AllTests().runAll();
    }
	
	protected Class[] testCases() {
		return new Class[] {
		        CallConstructorsConfigTestCase.class,
		        ClientDisconnectTestCase.class,
	            ClientTimeOutTestCase.class,
	            ClientTransactionHandleTestCase.class,
	            ClientTransactionPoolTestCase.class,
	            CloseServerBeforeClientTestCase.class,
	            CsCascadedDeleteReaddChildReferenceTestCase.class,
	            CsDeleteReaddTestCase.class,
	            IsAliveTestCase.class,
	            NoTestConstructorsQEStringCmpTestCase.class,
	            ObjectServerTestCase.class,
	            PrimitiveMessageTestCase.class,
	            QueryConsistencyTestCase.class,
	            SendMessageToClientTestCase.class,
	            ServerClosedTestCase.class,
	            ServerPortUsedTestCase.class,
	            ServerRevokeAccessTestCase.class,
	            ServerTimeoutTestCase.class,
	            ServerToClientTestCase.class,
	            SetSemaphoreTestCase.class,
	            SwitchingFilesFromClientTestCase.class,
	            SwitchingFilesFromMultipleClientsTestCase.class,
	            SwitchingToFileWithDifferentClassesTestCase.class,
	            CsSchemaUpdateTestCase.class,
		};
	}
	
}
