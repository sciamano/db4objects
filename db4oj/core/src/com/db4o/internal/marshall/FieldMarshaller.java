/* Copyright (C) 2006  db4objects Inc.  http://www.db4o.com */

package com.db4o.internal.marshall;

import java.io.IOException;

import com.db4o.*;
import com.db4o.internal.*;


/**
 * @exclude
 */
public interface FieldMarshaller {

    void write(Transaction trans, ClassMetadata clazz, FieldMetadata field, BufferImpl writer);

    RawFieldSpec readSpec(ObjectContainerBase stream,BufferImpl reader);
    
    FieldMetadata read(ObjectContainerBase stream, FieldMetadata field, BufferImpl reader);

    int marshalledLength(ObjectContainerBase stream, FieldMetadata field);

	void defrag(ClassMetadata yapClass, FieldMetadata yapField, LatinStringIO sio,BufferPair readers) throws CorruptionException, IOException;

}
