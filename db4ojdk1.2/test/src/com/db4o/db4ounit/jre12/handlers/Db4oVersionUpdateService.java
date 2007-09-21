/* Copyright (C) 2007  db4objects Inc.  http://www.db4o.com */

package com.db4o.db4ounit.jre12.handlers;

import java.io.*;
import java.lang.reflect.*;
import java.net.*;
import java.util.*;

import com.db4o.db4ounit.util.*;

/**
 * @exclude
 */
public class Db4oVersionUpdateService {

	private static Map _cache = new HashMap();

	private final static String[] PREFIXES = { "com.db4o" };

	public static void createDatabase(File file, Class test) throws Exception {
		ClassLoader loader = getVersionClassLoader(file.toURL());
		Class clazz = loader.loadClass(test.getName());
		Object obj = clazz.newInstance();
		Method method = clazz.getMethod("createDatabase", new Class[] {});
		method.invoke(obj, new Object[] {});
	}
	
	public static String getDb4oVersion(URL db4oEngineURL) throws Exception {
        ClassLoader loader = getVersionClassLoader(db4oEngineURL);
        Class clazz = loader.loadClass("com.db4o.Db4o");
        Method method = clazz.getMethod("version", new Class[] {});
        String version = (String) method.invoke(null, new Object[] {});
        return version.replace(' ', '_');
    }
	
	public static File[] getDb4oLibFiles(File path) {
        File[] files = path.listFiles(new FilenameFilter() {
            public boolean accept(File file, String name) {
                return name.endsWith(".jar");
            }
        });
        return files;
    }
	
	private static ClassLoader getVersionClassLoader(URL url)
			throws MalformedURLException {
		ClassLoader loader = (ClassLoader) _cache.get(url);
		if (loader != null) {
			return loader;
		}
		URL[] urls = new URL[] { Db4oVersionRunner._testBinPath.toURL(), url, };
		loader = new VersionClassLoader(urls, PREFIXES);
		_cache.put(url, loader);
		return loader;
	}
	
}
