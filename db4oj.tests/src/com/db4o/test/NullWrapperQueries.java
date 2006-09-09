/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

package com.db4o.test;

import java.util.*;

import com.db4o.*;
import com.db4o.inside.marshall.*;
import com.db4o.query.*;


/**
 * 
 */
public class NullWrapperQueries {
    
    public Boolean m1;
//    Byte m2;   Byte will not work, since we always use 0 for null.
    public Boolean m2;
    public Character m3;
    public Date m4;
    public Double m5;
    public Float m6;
    public Integer m7;
    public Long m8;
    public Short m9;
    public String m10;
    
    public void configure(){
        for (int i = 1; i < 11; i++) {
            String desc = "m" + i;
            Db4o.configure().objectClass(this).objectField(desc).indexed(true);
        }
    }
    
    public void store(){
        Test.deleteAllInstances(this);
        NullWrapperQueries nwq = new NullWrapperQueries();
        nwq.fill1();
        Test.store(nwq);
        nwq = new NullWrapperQueries();
        nwq.fill0();
        Test.store(nwq);
        nwq = new NullWrapperQueries();
        nwq.fill0();
        Test.store(nwq);
        nwq = new NullWrapperQueries();
        nwq.fill1();
        Test.store(nwq);
        nwq = new NullWrapperQueries();
        Test.store(nwq);
        nwq = new NullWrapperQueries();
        Test.store(nwq);
    }
    
    public void test(){
        for (int i = 1; i < 11; i++) {
            Query q = Test.query();
            q.constrain(NullWrapperQueries.class);
            String desc = "m" + i;
            q.descend(desc).constrain(null);
            Test.ensure(q.execute().size() == 2);
        }
    }
    
    private void fill0(){
        m1 = new Boolean(false);
        // m2 = new Byte((byte)0);
        m2 = new Boolean(false);

        m3 = new Character((char)0);
        m4 = new Date(0);
        m5 = new Double(0);
        m6 = new Float(0);
        m7 = new Integer(0);
        m8 = new Long(0);
        m9 = new Short((short)0);
        m10 = "";
    }
    
    private void fill1(){
        m1 = new Boolean(true);
        // m2 = new Byte((byte)1);
        m2 = new Boolean(true);
        m3 = new Character((char)1);
        m4 = new Date(1);
        m5 = new Double(1);
        m6 = new Float(1);
        m7 = new Integer(1);
        m8 = new Long(1);
        m9 = new Short((short)1);
        m10 = "1";
    }
    
    
    
    
    
    
    
    
    
    
    

}
