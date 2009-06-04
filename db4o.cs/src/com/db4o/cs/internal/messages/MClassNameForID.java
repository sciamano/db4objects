/* Copyright (C) 2004   Versant Inc.   http://www.db4o.com */

package com.db4o.cs.internal.messages;

import com.db4o.internal.*;


/**
 * get the classname for an internal ID
 */
public final class MClassNameForID extends MsgD implements MessageWithResponse {
    public final boolean processAtServer() {
        int id = _payLoad.readInt();
        String name = "";
        synchronized (streamLock()) {
			ClassMetadata yapClass = stream().classMetadataForID(id);
			if (yapClass != null) {
				name = yapClass.getName();
			}
		}
        write(Msg.CLASS_NAME_FOR_ID.getWriterForString(transaction(), name));
        return true;
    }
}