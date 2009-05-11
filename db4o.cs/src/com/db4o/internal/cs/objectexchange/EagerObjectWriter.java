/* Copyright (C) 2004   Versant Inc.   http://www.db4o.com */

package com.db4o.internal.cs.objectexchange;

import java.util.*;

import com.db4o.foundation.*;
import com.db4o.internal.*;
import com.db4o.internal.slots.*;

public class EagerObjectWriter {

	private LocalTransaction _transaction;

	public EagerObjectWriter(LocalTransaction transaction) {
		_transaction = transaction;
    }
	
	/**
	 * options:
	 * 
	 * (1) precalculate complete buffer size by iterating through slots
	 * (1') keep the slots in a arraylist first to avoid IO overhead
	 * (2) resize the buffer as needed
	 * (2') allow garbage to be transmitted to avoid reallocation of the complete buffer
	 * (3) stream directly to the output socket
	 * @return 
	 */
	public ByteArrayBuffer write(IntIterator4 idIterator, int maxCount) {
		
		List<Pair<Integer, Slot>> slots = readSlots(idIterator, maxCount);
		int marshalledSize = marshalledSizeFor(slots);
		
		ByteArrayBuffer buffer = new ByteArrayBuffer(marshalledSize);
		buffer.writeInt(slots.size());
		for (Pair<Integer, Slot> idSlotPair : slots) {
			final int id = idSlotPair.first;
			final Slot slot = idSlotPair.second;
			final ByteArrayBuffer slotBuffer = _transaction.file().readSlotBuffer(slot);
			buffer.writeInt(id);
			buffer.writeInt(slot.length());
			buffer.writeBytes(slotBuffer._buffer);
		}
		return buffer;
	}

	private int marshalledSizeFor(List<Pair<Integer, Slot>> slots) {
		int total = Const4.INT_LENGTH; // count
		for (Pair<Integer, Slot> idSlotPair : slots) {
			total += Const4.INT_LENGTH; // id
			total += Const4.INT_LENGTH; // length
			total += idSlotPair.second.length();
		}
		return total;
    }

	private List<Pair<Integer, Slot>> readSlots(IntIterator4 idIterator, int maxCount) {
	    
		final ArrayList slots = new ArrayList();
		
        while(idIterator.moveNext()){
            final int id = idIterator.currentInt();
            final Slot slot = _transaction.getCurrentSlotOfID(id);
            
            slots.add(Pair.of(id, slot));
            
            if(slots.size() >= maxCount){
            	break;
            }
        }
		return slots;
    }
	

}