/* Copyright (C) 2008  Versant Inc.  http://www.db4o.com */

package com.db4o.internal.handlers;

import com.db4o.internal.*;
import com.db4o.internal.delete.*;
import com.db4o.internal.marshall.*;
import com.db4o.internal.slots.*;
import com.db4o.typehandlers.*;


/**
 * @exclude
 */
public interface FieldAwareTypeHandler extends ReferenceTypeHandler, VersionedTypeHandler, CascadingTypeHandler, VirtualAttributeHandler{
    
    public void addFieldIndices(ObjectIdContextImpl context, Slot oldSlot);

    public void collectIDs(CollectIdContext context, String fieldName);
    
    public void deleteMembers(DeleteContextImpl deleteContext, boolean isUpdate);

    public void readVirtualAttributes(ObjectReferenceContext context);
    
    public void classMetadata(ClassMetadata classMetadata);

    public boolean seekToField(ObjectHeaderContext context, ClassAspect aspect);

}
