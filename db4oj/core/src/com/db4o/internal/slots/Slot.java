/* Copyright (C) 2004 - 2006  Versant Inc.  http://www.db4o.com */

package com.db4o.internal.slots;

import com.db4o.internal.*;

/**
 * @exclude
 */
public class Slot {
    
    private final int _address;
    
    private final int _length;
    
    public static final Slot ZERO = new Slot(0, 0);
    
    public static final int NEW = -1;
    
    public static final int UPDATE = -2;

    public Slot(int address, int length){
        _address = address;
        _length = length;
    }
    
    public int address() {
        return _address;
    }

	public int length() {
		return _length;
	}

    public boolean equals(Object obj) {
        if(obj == this){
            return true;
        }
        if(! (obj instanceof Slot)){
            return false;
        }
        Slot other = (Slot) obj;
        return (_address == other._address) && (length() == other.length());
    }
    
    public int hashCode() {
        return _address ^ length();
    }
    
	public Slot subSlot(int offset) {
		return new Slot(_address + offset, length() - offset);
	}

    public String toString() {
    	return "[A:"+_address+",L:"+length()+"]";
    }
    
	public Slot truncate(int requiredLength) {
		return new Slot(_address, requiredLength);
	}
    
    public static int MARSHALLED_LENGTH = Const4.INT_LENGTH * 2;

	public int compareByAddress(Slot slot) {
		
		// FIXME: This is the wrong way around !!!
		// Fix here and in all referers.
		
        int res = slot._address - _address;
        if(res != 0){
            return res;
        }
        return slot.length() - length();
	}
	
	public int compareByLength(Slot slot) {
		
		// FIXME: This is the wrong way around !!!
		// Fix here and in all referers.
		
		int res = slot.length() - length();
		if(res != 0){
			return res;
		}
		return slot._address - _address;
	}

	public boolean isDirectlyPreceding(Slot other) {
		return _address + length() == other._address;
	}

	public Slot append(Slot slot) {
		return new Slot(address(), _length + slot.length());
	}

	public boolean isNull() {
		return address() == 0
			|| length() == 0;
    }
	
	public boolean isNew(){
		return _address == NEW;
	}
	
	public boolean isUpdate() {
		return _address == UPDATE;
	}
	
	public static boolean isNull(Slot slot){
		return slot == null || slot.isNull();
	}
	
}
