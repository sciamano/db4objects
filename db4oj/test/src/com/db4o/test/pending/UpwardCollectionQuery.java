package com.db4o.test.pending;

import com.db4o.*;
import com.db4o.query.*;
import com.db4o.test.*;


public class UpwardCollectionQuery {
    public void store(){
	    SimpleNode sub=new SimpleNode("sub",new SimpleNode[0]);
	    SimpleNode sup=new SimpleNode("sup",new SimpleNode[]{sub});
	    Test.store(sup);
	}

	public void test() {
		Query supq=Test.query();
		supq.constrain(SimpleNode.class);
		Query subq=supq.descend("children");
		subq.constrain(SimpleNode.class);
		subq.descend("name").constrain(new Evaluation() {
            public void evaluate(Candidate candidate) {
                candidate.include(true);
            }		    
		});
		ObjectSet objectSet = subq.execute();
		Test.ensure(objectSet.size() == 1);
		Object found=objectSet.next();
		System.err.println(found.getClass());
		Test.ensure(found.getClass()==SimpleNode.class);
	}
}
