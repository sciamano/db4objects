/* Copyright (C) 2004 - 2006  db4objects Inc.  http://www.db4o.com */

package com.db4o.test.lib;

import java.io.*;

import com.db4o.foundation.*;

/**
 * simple annotated stack trace for debugging
 * 
 * @exclude
 */
public class StackTrace {	
	// ignore top 2 lines: exception name and 'StackTrace#<init>'
	private static final int IGNORELINES = 2;
	// 0: trace line, 1: case info (or null)
	private String[][] _trace;
	// cache hashcode for faster equals()
	private int _hash;
	
	public StackTrace(int exclude,String caseInfo,StackTrace old) {
		String traceStr=readStackTrace(new Throwable());
		Collection4 split = parseStackTrace(exclude, traceStr);
		_trace=buildStackTrace(split, caseInfo);
		copyCaseInfo(old);
	}

	private void copyCaseInfo(StackTrace old) {
		if(old==null) {
			return;
		}
		int length=(_trace.length<old._trace.length ? _trace.length : old._trace.length);
		for(int idx=0;idx<length;idx++) {
			if(!trace(idx).equals(old.trace(idx))) {
				break;
			}
			if(old.caseInfo(idx)!=null&&caseInfo(idx)==null) {
				_trace[idx][1]=old.caseInfo(idx);
			}
		}
	}
	
	public boolean equals(Object obj) {
		if(this==obj) {
			return true;
		}
		if(obj==null||getClass()!=obj.getClass()) {
			return false;
		}
		StackTrace casted=(StackTrace)obj;
		if(hashCode()!=casted.hashCode()) {
			return false;
		}
		if(_trace.length!=casted._trace.length) {
			return false;
		}
		for (int idx = 0; idx < _trace.length; idx++) {
			if(!trace(idx).equals(casted.trace(idx))) {
				return false;
			}
			if((caseInfo(idx)!=null)^(casted.caseInfo(idx)!=null)) {
				return false;
			}
			if(caseInfo(idx)!=null&&!caseInfo(idx).equals(casted.caseInfo(idx))) {
				return false;
			}
		}
		return true;
	}
	
	public int hashCode() {
		if(_hash!=0) {
			return _hash;
		}
		int hash=0;
		for (int idx = 0; idx < _trace.length; idx++) {
			hash=29*hash+trace(idx).hashCode();
			if(caseInfo(idx)!=null) {
				hash=29*hash+caseInfo(idx).hashCode();
			}
		}
		_hash=hash;
		return hash;
	}
	
	public String toString() {
		StringBuffer buf=new StringBuffer();
		for (int idx = 0; idx < _trace.length; idx++) {
			if(idx>0) {
				buf.append('\n');
			}
			buf.append(trace(idx));
			if(caseInfo(idx)!=null) {
				buf.append('\n');
				buf.append("["+caseInfo(idx)+"]");
			}
		}
		return buf.toString();
	}

	private String trace(int idx) {
		return _trace[idx][0];
	}

	private String caseInfo(int idx) {
		return _trace[idx][1];
	}

	private String readStackTrace(Throwable exc) {
		StringWriter writer=new StringWriter();
		exc.printStackTrace(new PrintWriter(writer));
		return writer.toString();
	}
	
	private Collection4 parseStackTrace(int exclude, String traceStr) {
		Collection4 split=new Collection4();
		int nlIdx=traceStr.indexOf('\n');
		for(int i=0;i<(IGNORELINES+exclude);i++) {
			traceStr=traceStr.substring(nlIdx+1);
			nlIdx=traceStr.indexOf('\n');
			if(nlIdx<0) {
				break;
			}
		}
		while(nlIdx>-1) {
			String cur = traceStr.substring(0,nlIdx);
			split.add(cleanUpTraceLine(cur));
			traceStr=traceStr.substring(nlIdx+1);
			nlIdx=traceStr.indexOf('\n');
		}
		// last one is empty (trailing newline)
		return split;
	}
	
	private String[][] buildStackTrace(Collection4 split,String caseInfo) {
		String[][] trace=new String[split.size()][2];
		Iterator4 iter=split.iterator();
		for (int idx = 0; idx < trace.length; idx++) {
			trace[idx][0]=(String)iter.next();
		}
		trace[trace.length-1][1]=caseInfo;
		return trace;
	}

	private String cleanUpTraceLine(String str) {
		int atIdx=str.indexOf("at ");
		if(atIdx>=0) {
			str=str.substring(atIdx+3);
		}
		int colonIdx=str.lastIndexOf(':');
		if(colonIdx>=0) {
			str=str.substring(0,colonIdx)+')';
		}
		return str;
	}
}
