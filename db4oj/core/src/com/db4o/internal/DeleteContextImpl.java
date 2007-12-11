/* Copyright (C) 2007  db4objects Inc.  http://www.db4o.com */

package com.db4o.internal;

import com.db4o.diagnostic.DefragmentRecommendation.*;
import com.db4o.internal.diagnostic.*;
import com.db4o.internal.slots.*;

/**
 * @exclude
 */
public class DeleteContextImpl extends BufferContext implements DeleteContext {
	
	private final int _handlerVersion;

	public DeleteContextImpl(StatefulBuffer buffer, int handlerVersion){
		super(buffer.getTransaction(), buffer);
		_handlerVersion = handlerVersion;
	}

	public void cascadeDeleteDepth(int depth) {
		((StatefulBuffer)_buffer).setCascadeDeletes(depth);
	}

	public int cascadeDeleteDepth() {
		return ((StatefulBuffer)_buffer).cascadeDeletes();
	}

	public void defragmentRecommended() {
        DiagnosticProcessor dp = container()._handlers._diagnosticProcessor;
        if(dp.enabled()){
            dp.defragmentRecommended(DefragmentRecommendationReason.DELETE_EMBEDED);
        }
	}

	public Slot readSlot() {
		return new Slot(_buffer.readInt(), _buffer.readInt());
	}

	public int handlerVersion() {
		return _handlerVersion;
	}

}
