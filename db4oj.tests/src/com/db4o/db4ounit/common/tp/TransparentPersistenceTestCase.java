/* Copyright (C) 2004 - 2006 db4objects Inc. http://www.db4o.com */

package com.db4o.db4ounit.common.tp;

import com.db4o.*;
import com.db4o.activation.*;
import com.db4o.config.*;
import com.db4o.events.*;
import com.db4o.ext.*;
import com.db4o.foundation.*;
import com.db4o.query.*;
import com.db4o.ta.*;

import db4ounit.*;
import db4ounit.extensions.*;

public class TransparentPersistenceTestCase extends AbstractDb4oTestCase {
	
	public static class Item implements Activatable {
		
		private transient Activator _activator;
		
		public String name;
		
		public Item() {
		}
		
		public Item(String initialName) {
			name = initialName;
		}
		
		public String getName() {
			activate(ActivationPurpose.READ);
			return name;
		}
		
		public void setName(String newName) {
			activate(ActivationPurpose.WRITE);
			name = newName;
		}

		public void activate(ActivationPurpose purpose) {
			_activator.activate(purpose);
		}

		public void bind(Activator activator) {
			_activator = activator;
		}
		
		public String toString() {
			return "Item(" + getName() + ")";
		}
	}
	
	protected void configure(Configuration config) throws Exception {
		config.add(new TransparentActivationSupport());
	}
	
	protected void store() throws Exception {
		store(new Item("Foo"));
		store(new Item("Bar"));
	}
	
	public void testActivateOnWrite() throws Exception {
		
		Item foo = itemByName("Foo");
		foo.setName("Foo*");
		Assert.areEqual("Foo*", foo.getName());
	}
	
	public void testConcurrentClientModification() throws Exception {
		if (!isClientServer()) {
			return;
		}
		
		final ExtObjectContainer client1 = db();
		final ExtObjectContainer client2 = openNewClient();
		try {
			Item foo1 = itemByName(client1, "Foo");
			foo1.setName("Foo*");
			
			Item foo2 = itemByName(client2, "Foo");
			foo2.setName("Foo**");
			
			assertUpdatedObjects(client1, foo1);
			assertUpdatedObjects(client2, foo2);
			
			client1.refresh(foo1, 1);
			Assert.areEqual(foo2.getName(), foo1.getName());
		} finally {
			client2.close();
		}
	}

	private ExtObjectContainer openNewClient() {
		return ((Db4oClientServerFixture)this.fixture()).openNewClient();
	}
	
	public void testTransparentUpdate() throws Exception {
		
		Item foo = itemByName("Foo");
		foo.setName("Bar"); // changing more than once shouldn't be a problem
		foo.setName("Foo*");
		
		Item bar = itemByName("Bar"); 
		Assert.areEqual("Bar", bar.getName()); // accessed but not changed
		
		assertUpdatedObjects(foo);
		
		reopen();
		
		Assert.isNotNull(itemByName("Foo*"));
		Assert.isNull(itemByName("Foo"));
		Assert.isNotNull(itemByName("Bar"));
	}
	
	public void testUpdateAfterActivation() throws Exception {
		Item foo = itemByName("Foo");
		Assert.areEqual("Foo", foo.getName());
		foo.setName("Foo*");
		assertUpdatedObjects(foo);
	}

	private void assertUpdatedObjects(Item expected) {
		assertUpdatedObjects(db(), expected);
	}

	private void assertUpdatedObjects(final ExtObjectContainer container,
			Item expected) {
		Collection4 updated = commitCapturingUpdatedObjects(container);
		Assert.areEqual(1, updated.size(), updated.toString());
		Assert.areSame(expected, updated.singleElement());
	}
	
	private Collection4 commitCapturingUpdatedObjects(
			final ExtObjectContainer container) {
		final Collection4 updated = new Collection4();
		eventRegistryFor(container).updated().addListener(new EventListener4() {
			public void onEvent(Event4 e, EventArgs args) {
				ObjectEventArgs objectArgs = (ObjectEventArgs)args;
				updated.add(objectArgs.object());
			}
		});
		container.commit();
		return updated;
	}

	private Item itemByName(String name) {
		return itemByName(db(), name);
	}

	private Item itemByName(final ExtObjectContainer container, String name) {
		Query q = newQuery(container, Item.class);
		q.descend("name").constrain(name);
		ObjectSet result = q.execute();
		if (result.hasNext()) {
			return (Item)result.next();
		}
		return null;
	}

}
