/* Copyright (C) 2004 - 2005  db4objects Inc.   http://www.db4o.com */

package com.db4o.test;

import com.db4o.test.cluster.*;
import com.db4o.test.collections.*;
import com.db4o.test.reflect.*;

/**
 */
@decaf.Ignore(decaf.Platform.JDK11)
public class Jdk1_2TestSuite extends TestSuite{
    
    public Class[] tests(){
        return new Class[] {
            ClusterQueryImplementsList.class,
            DeleteRemovedMapElements.class,
            DiscreteArrayInMap.class,
            GenericObjects.class,
            HashMapClearUnsaved.class,
            JdkComparatorSort.class,
            MultipleEvaluationGetObjectCalls.class,
            ObjectSetAsList.class,
            OrClassConstraintInList.class,
            PrimitivesInCollection.class,
            QueryForList.class,
            Reflection.class,
            RefreshList.class,
            SelectDistinct.class,
            SodaEvaluation.class,
            SortedSameOrder.class,
            StoreBigDecimal.class,
            StringCaseInsensitive.class,
            StringInLists.class,
            TestHashMap.class,
            TestStringBuffer.class,
            TestTreeMap.class,
            TestTreeSet.class,
            TransientClone.class,
        };
    }
}