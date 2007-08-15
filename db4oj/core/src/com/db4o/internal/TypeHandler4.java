/* Copyright (C) 2004 - 2007  db4objects Inc.   http://www.db4o.com */

package com.db4o.internal;

import com.db4o.*;
import com.db4o.internal.marshall.*;
import com.db4o.internal.query.processor.*;
import com.db4o.marshall.*;
import com.db4o.reflect.*;


/**
 * @exclude
 */
public interface TypeHandler4 extends Comparable4 {
	
	void cascadeActivation(Transaction trans, Object obj, int depth, boolean activate);
	
	ReflectClass classReflector();
    
	void deleteEmbedded(MarshallerFamily mf, StatefulBuffer buffer) throws Db4oIOException;
	
	int getID();
	
    boolean hasFixedLength();
    
    int linkLength();
   
    /**
     * The length calculation is different, depending from where we 
     * calculate. If we are still in the link area at the beginning of
     * the slot, no data needs to be written to the payload area for
     * primitive types, since they fully fit into the link area. If
     * we are already writing something like an array (or deeper) to
     * the payload area when we come here, a primitive does require
     * space in the payload area.
     * Differentiation is expressed with the 'topLevel' parameter.
     * If 'topLevel==true' we are asking for a size calculation for
     * the link area. If 'topLevel==false' we are asking for a size
     * calculation for the payload area at the end of the slot.
     */
    void calculateLengths(Transaction trans, ObjectHeaderAttributes header, boolean topLevel, Object obj, boolean withIndirection);
    
	Object read(MarshallerFamily mf, StatefulBuffer buffer, boolean redirect) throws CorruptionException, Db4oIOException;
    
	Object readQuery(Transaction trans, MarshallerFamily mf, boolean withRedirection, Buffer buffer, boolean toArray) throws CorruptionException, Db4oIOException;
	
    Object write(MarshallerFamily mf, Object obj, boolean topLevel, StatefulBuffer buffer, boolean withIndirection, boolean restoreLinkOffset);
	
    QCandidate readSubCandidate(MarshallerFamily mf, Buffer buffer, QCandidates candidates, boolean withIndirection);

	void defrag(MarshallerFamily mf, BufferPair readers, boolean redirect);

	Object read(ReadContext context);
	
    void write(WriteContext context, Object obj);
	
}
