/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

package com.db4o.test.soda.classes.simple;

import com.db4o.*;
import com.db4o.query.*;
import com.db4o.test.soda.*;

public class STInteger implements STClass1{
	
	public static transient SodaTest st;
	
	public int i_int;
	
	public STInteger(){
	}
	
	private STInteger(int a_int){
		i_int = a_int;
	}
	
	public Object[] store() {
		return new Object[]{
			new STInteger(0),
			new STInteger(1),
			new STInteger(99),
			new STInteger(909)
		};
	}
	
	public void testEquals(){
		Query q = st.query();
		q.constrain(new STInteger(0));  
		
		// Primitive default values are ignored, so we need an 
		// additional constraint:
		q.descend("i_int").constrain(new Integer(0));
		st.expectOne(q, store()[0]);
	}
	
	public void testNotEquals(){
		Query q = st.query();
		Object[] r = store();
		Constraint c = q.constrain(r[0]);
		q.descend("i_int").constrain(new Integer(0)).not();
		st.expect(q, new Object[] {r[1], r[2], r[3]});
	}
	
	public void testGreater(){
		Query q = st.query();
		Constraint c = q.constrain(new STInteger(9));
		q.descend("i_int").constraints().greater();
		Object[] r = store();
		st.expect(q, new Object[] {r[2], r[3]});
	}
	
	public void testSmaller(){
		Query q = st.query();
		Constraint c = q.constrain(new STInteger(1));
		q.descend("i_int").constraints().smaller();
		st.expectOne(q, store()[0]);
	}
	
	public void testContains(){
		Query q = st.query();
		Constraint c = q.constrain(new STInteger(9));
		q.descend("i_int").constraints().contains();
		Object[] r = store();
		st.expect(q, new Object[] {r[2], r[3]});
	}
	
	public void testNotContains(){
		Query q = st.query();
		Constraint c = q.constrain(new STInteger(0));
		q.descend("i_int").constrain(new Integer(0)).contains().not();
		Object[] r = store();
		st.expect(q, new Object[] {r[1], r[2]});
	}
	
	public void testLike(){
		Query q = st.query();
		Constraint c = q.constrain(new STInteger(90));
		q.descend("i_int").constraints().like();
		st.expectOne(q, new STInteger(909));
		q = st.query();
		c = q.constrain(new STInteger(10));
		q.descend("i_int").constraints().like();
		st.expectNone(q);
	}
	
	public void testNotLike(){
		Query q = st.query();
		Constraint c = q.constrain(new STInteger(1));
		q.descend("i_int").constraints().like().not();
		Object[] r = store();
		st.expect(q, new Object[] {r[0], r[2], r[3]});
	}
	
	public void testIdentity(){
		Query q = st.query();
		Constraint c = q.constrain(new STInteger(1));
		ObjectSet set = q.execute();
		STInteger identityConstraint = (STInteger)set.next();
		identityConstraint.i_int = 9999;
		q = st.query();
		q.constrain(identityConstraint).identity();
		identityConstraint.i_int = 1;
		st.expectOne(q,store()[1]);
	}
	
	public void testNotIdentity(){
		Query q = st.query();
		Constraint c = q.constrain(new STInteger(1));
		ObjectSet set = q.execute();
		STInteger identityConstraint = (STInteger)set.next();
		identityConstraint.i_int = 9080;
		q = st.query();
		q.constrain(identityConstraint).identity().not();
		identityConstraint.i_int = 1;
		Object[] r = store();
		st.expect(q, new Object[] {r[0], r[2], r[3]});
	}
	
	public void testConstraints(){
		Query q = st.query();
		q.constrain(new STInteger(1));
		q.constrain(new STInteger(0));
		Constraints cs = q.constraints();
		Constraint[] csa = cs.toArray();
		if(csa.length != 2){
			st.error("Constraints not returned");
		}
	}
	
	public void testEvaluation(){
		Query q = st.query();
		q.constrain(new STInteger(0));
		q.constrain(new Evaluation() {
			public void evaluate(Candidate candidate) {
				STInteger sti = (STInteger)candidate.getObject();
				candidate.include((sti.i_int + 2) > 100);
			}
		});
		Object[] r = store();
		st.expect(q, new Object[] {r[2], r[3]});
	}
	
}

