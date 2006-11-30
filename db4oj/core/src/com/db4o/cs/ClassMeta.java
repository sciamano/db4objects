/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

package com.db4o.cs;

public class ClassMeta {
	private String _className;

	private boolean _isSystemClass;

	private ClassMeta _superClass;

	private FieldMeta[] _fields;
	
	public ClassMeta() {
	}
	
	public ClassMeta(String className, boolean systemClass) {
		_className = className;
		_isSystemClass = systemClass;
	}

	public FieldMeta[] getFields() {
		return _fields;
	}

	public void setFields(FieldMeta[] fields) {
		this._fields = fields;
	}

	public ClassMeta getSuperClass() {
		return _superClass;
	}

	public void setSuperClass(ClassMeta superClass) {
		this._superClass = superClass;
	}

	public String getClassName() {
		return _className;
	}

	public boolean isSystemClass() {
		return _isSystemClass;
	}

}
