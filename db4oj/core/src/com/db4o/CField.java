/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

package com.db4o;

import java.lang.reflect.*;
import com.db4o.reflect.*;

/**
 * Reflection implementation for Field to map to JDK reflection.
 */
class CField implements IField {

    private final Field field;

    public CField(Field field) {
        this.field = field;
    }

    public String getName() {
        return field.getName();
    }

    public Class getType() {
        return field.getType();
    }

    public boolean isPublic() {
        return Modifier.isPublic(field.getModifiers());
    }

    public boolean isStatic() {
        return Modifier.isStatic(field.getModifiers());
    }

    public boolean isTransient() {
        return Modifier.isTransient(field.getModifiers());
    }

    public void setAccessible() {
        Platform.setAccessible(field);
    }

    public Object get(Object onObject) {
        try {
            return field.get(onObject);
        } catch (Exception e) {
            return null;
        }
    }

    public void set(Object onObject, Object attribute) {
        try {
            field.set(onObject, attribute);
        } catch (Exception e) {
            if(Debug.atHome){
                e.printStackTrace();
            }
        }
    }
}
