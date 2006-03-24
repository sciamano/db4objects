package com.db4o.test.replication.hibernate.oracle;

import com.db4o.replication.hibernate.impl.HibernateReplicationProviderImpl;
import com.db4o.test.replication.hibernate.HibernateReplicationFeaturesMain;
import com.db4o.test.replication.hibernate.HibernateUtil;

public class OracleFeaturesMain extends HibernateReplicationFeaturesMain {
// --------------------------- CONSTRUCTORS ---------------------------

	public OracleFeaturesMain() {

	}

	public void test() {
		hA = new HibernateReplicationProviderImpl(HibernateUtil.produceOracleConfigA());
		hB = new HibernateReplicationProviderImpl(HibernateUtil.produceOracleConfigB());

		super.test();
	}
}
