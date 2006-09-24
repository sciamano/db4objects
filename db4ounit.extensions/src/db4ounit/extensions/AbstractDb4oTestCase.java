/* Copyright (C) 2004 - 2006  db4objects Inc.  http://www.db4o.com */

package db4ounit.extensions;

import com.db4o.*;
import com.db4o.config.*;
import com.db4o.ext.*;
import com.db4o.query.*;
import com.db4o.reflect.*;

import db4ounit.*;
import db4ounit.extensions.fixtures.*;

public class AbstractDb4oTestCase implements Db4oTestCase {
    
	private transient Db4oFixture _fixture;
	
	/* (non-Javadoc)
	 * @see db4ounit.extensions.Db4oTestCase#fixture(db4ounit.extensions.Db4oFixture)
	 */
	public void fixture(Db4oFixture fixture) {
		_fixture = fixture;
	}

	/* (non-Javadoc)
	 * @see db4ounit.extensions.Db4oTestCase#fixture()
	 */
	public Db4oFixture fixture() {
		return _fixture;
	}
    
    protected void reopen() throws Exception{
    	_fixture.reopen();
    }
	
	public void setUp() throws Exception {
        _fixture.clean();
		configure(_fixture.config());
		_fixture.open();
		store();
        _fixture.close();
        _fixture.open();
	}
	
	public void tearDown() throws Exception {
		_fixture.close();
        _fixture.clean();
	}

	protected void configure(Configuration config) {}
	
	protected void store() throws Exception {}

	/* (non-Javadoc)
	 * @see db4ounit.extensions.Db4oTestCase#db()
	 */
	public ExtObjectContainer db() {
		return fixture().db();
	}
	
	protected Class[] testCases() {
		return new Class[] { getClass() };
	}

	public int runSolo() {
		return runSolo(true);
	}

	public int runSolo(boolean independentConfig) {
		return new TestRunner(
					new Db4oTestSuiteBuilder(
							new Db4oSolo(configSource(independentConfig)), testCases())).run();
	}
    
    public int runClientServer() {
        return new TestRunner(
                    new Db4oTestSuiteBuilder(
                            new Db4oClientServer(), testCases())).run();
    }

    private ConfigurationSource configSource(boolean independentConfig) {
        return (independentConfig ? (ConfigurationSource)new IndependentConfigurationSource() : new GlobalConfigurationSource());
    }

	protected YapStream stream() {
	    return (YapStream) db();
	}

	protected Transaction trans() {
	    return stream().getTransaction();
	}

	protected Transaction systemTrans() {
	    return stream().getSystemTransaction();
	}
    
    protected Query newQuery(){
        return db().query();
    }
    
    protected Reflector reflector(){
        return stream().reflector();
    }

	protected void indexField(Configuration config,Class clazz, String fieldName) {
		config.objectClass(clazz).objectField(fieldName).indexed(true);
	}

	protected Transaction newTransaction() {
		return stream().newTransaction();
	}

	protected Query newQuery(Class clazz) {
		final Query query = newQuery();
		query.constrain(clazz);
		return query;
	}
}
