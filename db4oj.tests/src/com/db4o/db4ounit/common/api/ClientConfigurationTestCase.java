/* Copyright (C) 2009  Versant Inc.   http://www.db4o.com */
package com.db4o.db4ounit.common.api;

import com.db4o.cs.*;
import com.db4o.cs.config.*;
import com.db4o.foundation.*;
import com.db4o.internal.*;
import com.db4o.internal.config.*;

import db4ounit.*;

/**
 * @sharpen.remove
 */
public class ClientConfigurationTestCase implements TestCase {
	
	final ClientConfiguration _subject = Db4oClientServer.newClientConfiguration();
	final Config4Impl _legacy = Db4oLegacyConfigurationBridge.asLegacy(_subject);
	
	public void testPrefetchDepth() {
	
		_subject.prefetchDepth(42);
		Assert.areEqual(42, _legacy.prefetchDepth());
		
	}
	
	public void testPrefetchIDCount() {
		
		_subject.prefetchIDCount(42);
		Assert.areEqual(42, _legacy.prefetchIDCount());
		
	}
	
	public void testMessageSender() {
		
		Assert.areSame(_legacy.getMessageSender(), _subject.messageSender());
		
	}
	
	public void testPrefetchObjectCount() {
		
		_subject.prefetchObjectCount(42);
		Assert.areEqual(42, _legacy.prefetchObjectCount());
		
	}
	
	public void testDefaultPrefetchSlotCacheSize(){
    	Environment environment = Environments.newConventionBasedEnvironment(_legacy);
    	Environments.runWith(environment, new Runnable() {
			public void run() {
				_subject.prefetchDepth(7);
				_subject.prefetchObjectCount(11);
				Assert.areEqual(7 * 11 * Config4Impl.PREFETCH_SLOT_CACHE_SIZE_FACTOR, _legacy.prefetchSlotCacheSize());
			}
		});
	}
	
	public void testPrefetchSlotCacheSize(){
		_subject.prefetchSlotCacheSize(42);
		Assert.areEqual(42, _legacy.prefetchSlotCacheSize());
	}

}
