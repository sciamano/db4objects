/* Copyright (C) 2007  db4objects Inc.  http://www.db4o.com */

package com.db4o.internal.handlers;

import com.db4o.*;
import com.db4o.ext.*;
import com.db4o.internal.*;
import com.db4o.internal.marshall.*;
import com.db4o.internal.query.processor.*;
import com.db4o.marshall.*;


/**
 * @exclude
 */
public class ArrayHandler0 extends ArrayHandler {

    public ArrayHandler0(TypeHandler4 template) {
        super(template);
    }
    
    public void delete(DeleteContext context) throws Db4oIOException {
    	context.readSlot();
    	context.defragmentRecommended();
    }
    
    public void readCandidates(int handlerVersion, Buffer reader, QCandidates candidates) throws Db4oIOException {
        Transaction transaction = candidates.transaction();
        Buffer arrayBuffer = reader.readEmbeddedObject(transaction);
        if(Deploy.debug){
            arrayBuffer.readBegin(identifier());
        }
        int count = elementCount(transaction, arrayBuffer);
        for (int i = 0; i < count; i++) {
            candidates.addByIdentity(new QCandidate(candidates, null, arrayBuffer.readInt(), true));
        }
    }

    public Object read(ReadContext readContext) {
        
        InternalReadContext context = (InternalReadContext) readContext;
        
        Buffer buffer = readIndirectedBuffer(context); 
        if (buffer == null) {
            return null;
        }
        
        // With the following line we ask the context to work with 
        // a different buffer. Should this logic ever be needed by
        // a user handler, it should be implemented by using a Queue
        // in the UnmarshallingContext.
        
        // The buffer has to be set back from the outside!  See below
        Buffer contextBuffer = context.buffer(buffer);
        
        Object array = super.read(context);
        
        // The context buffer has to be set back.
        context.buffer(contextBuffer);
        
        return array;
    }
}
