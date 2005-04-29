/*
 * Copyright (C) 2005 db4objects Inc.  http://www.db4o.com
 */
package com.db4o.browser.model;

import java.util.HashMap;
import java.util.Iterator;
import java.util.LinkedList;

import com.db4o.browser.gui.standalone.ICloseListener;
import com.db4o.browser.prefs.PreferencesCore;

/**
 * BrowserCore.  The root of the model hierarchy in the browser.
 *
 * @author djo
 */
public class BrowserCore implements ICloseListener {
    private static BrowserCore model = null;
    
	public static BrowserCore getDefault() {
        if (model == null) {
            model = new BrowserCore();
			PreferencesCore.initialize();
        }
        return model;
    }
    
	private LinkedList databases = new LinkedList();
    private HashMap dbMap = new HashMap();  // Maps path/filename to database
    
    /**
     * @param databasePath
     * @return the database corresponding to databasePath
     */
    public IDatabase getDatabase(String databasePath) {
		Db4oConnectionSpec spec = new Db4oFileConnectionSpec(databasePath, true);
        return getDatabaseInternal(spec);
    }
	
    private IDatabase getDatabase(String host, int port, String user, String password) throws Exception {
		Db4oConnectionSpec spec = new Db4oSocketConnectionSpec(host, port,
                user, password, true);
        return getDatabaseInternal(spec);
    }

	private IDatabase getDatabaseInternal(Db4oConnectionSpec spec) {
		IDatabase requested = (IDatabase) dbMap.get(spec.path());
		if (requested == null) {
            requested = new Db4oDatabase();
            requested.open(spec);
            dbMap.put(spec.path(), requested);
            databases.addLast(requested);
        }
		return requested;
	}

	/**
	 * Gets an array of all open databases
	 * 
	 * @return Database[] all open databases
	 */
	public IDatabase[] getAllDatabases() {
		return (IDatabase[]) dbMap.values().toArray(new IDatabase[dbMap.size()]);
	}

    /* (non-Javadoc)
	 * @see com.db4o.browser.gui.standalone.ICloseListener#closing()
	 */
	public void closing() {
		for (Iterator i = databases.iterator(); i.hasNext();) {
			IDatabase database = (IDatabase) i.next();
			database.closeIfOpen();
		}
		PreferencesCore.close();
	}
    
    /**
     * Method iterator.  Returns an IGraphIterator on the most recently opened
     * database.  Returns null if there is no open database.
     * 
     * @return IGraphIterator an iterator on the current open database; null if
     * no database is open.
     */
    public IGraphIterator iterator() {
        if (databases.isEmpty()) {
            return null;
        }
        IDatabase current = (IDatabase) databases.getLast();
        return current.graphIterator();
    }
    
    /**
     * Method iterator.  Returns an IGraphIterator on the specified database
     * file.  If the specified database file is not open, it will be opened.
     * 
     * @param databasePath The platform-specific path string.
     * @return IGraphIterator.  An Iterator on the contents of the specified database.
     */
    public IGraphIterator iterator(String databasePath) {
        IDatabase requested = getDatabase(databasePath);
        return requested.graphIterator();
    }
    
    /**
     * Method iterator.  Returns an IGraphIterator on the specified class in
     * the specified database file.  If the specified database file is not 
     * open, it will be opened.
     * 
     * @param databasePath The platform-specific path string.
     * @param selectedClass The name of the class.
     * @return IGraphIterator.  An Iterator on the contents of the specified database.
     */
    public IGraphIterator iterator(String databasePath, String selectedClass) {
        IDatabase requested = getDatabase(databasePath);
        return requested.graphIterator(selectedClass);
    }


    public IGraphIterator iterator(String host, int port, String user, String password) throws Exception {
        IDatabase requested = getDatabase(host, port, user, password);
        return requested.graphIterator();
    }
    
    /**
     * @return true if at least one database is open; false otherwise.
     */
    public boolean isOpen() {
        return databases.size() > 0;
    }

    public void updateClasspath() {
        for (Iterator databaseIter = databases.iterator(); databaseIter.hasNext();) {
            IDatabase database = (IDatabase) databaseIter.next();
            database.reopen();
        }
        fireBrowserCoreEvent();
    }
    
    private LinkedList coreListeners = new LinkedList();
    
    public void addBrowserCoreListener(IBrowserCoreListener listener) {
        coreListeners.add(listener);
    }
    
    public void removeBrowserCoreListener(IBrowserCoreListener listener) {
        coreListeners.remove(listener);
    }
    
    private void fireBrowserCoreEvent() {
        for (Iterator listeners = coreListeners.iterator(); listeners.hasNext();) {
            IBrowserCoreListener listener = (IBrowserCoreListener) listeners.next();
            listener.classpathChanged(this);
        }
    }

}
