/* Copyright (C) 2008   db4objects Inc.   http://www.db4o.com */
package com.db4o.db4ounit.common.events;

import java.util.*;

import com.db4o.*;
import com.db4o.events.*;
import com.db4o.foundation.*;
import com.db4o.query.*;

import db4ounit.*;

public class ExceptionPropagationInEventsTestUnit extends EventsTestCaseBase {

	public ExceptionPropagationInEventsTestUnit() {
		_eventFirer.put("insert", newObjectInserter());
		_eventFirer.put("query", newQueryRunner());
		_eventFirer.put("update", newObjectUpdater());
		_eventFirer.put("delete", newObjectDeleter());		
	}
	
	@Override
	protected void store() throws Exception {
		store(new Item(1));
		store(new Item(2));		
	}
	
	public void testEvents() {
		final EventInfo event = (EventInfo) ExceptionPropagationInEventsTestVariables.EVENT_SELECTOR.value();
		assertEventThrows(_eventFirer.get(event.eventFirerName()), event.listenerSetter());
	}	
	
	private void assertEventThrows(final CodeBlock codeBlock, final Procedure4<EventRegistry> listenerSetter) {
		final EventRegistry eventRegistry = EventRegistryFactory.forObjectContainer(db());		
		listenerSetter.apply(eventRegistry);		
		Assert.expect(EventException.class, NotImplementedException.class, codeBlock);
	}
	
	private CodeBlock newObjectUpdater() {
		return new CodeBlock() {
			
			public void run() throws Throwable {
				final Item item = retrieveItem(1);
				item.id = 10;
				
				db().store(item);
				db().commit();
			}
			
		};
	}

	private CodeBlock newObjectDeleter() {
		return new CodeBlock() {
			public void run() throws Throwable {
				db().delete(retrieveItem(1));
				db().commit();
			}			
		};
	}
	
	private CodeBlock newQueryRunner() {
		return new CodeBlock() {
			public void run() {
				retrieveItem(1);
			}		
		};
	}
	
	private CodeBlock newObjectInserter() {		
		return new CodeBlock() {

			public void run() throws Throwable {				
				db().store(new Item());
				db().commit();
			}			
		};
	}

	private Item retrieveItem(int id) {
		final Query query = newQuery(Item.class);
		query.descend("id").constrain(id);
		final ObjectSet<Item> results = query.execute();
		
		Assert.areEqual(1, results.size());
		
		return results.next();
	}	
	
	private HashMap<String, CodeBlock> _eventFirer = new HashMap();	
}