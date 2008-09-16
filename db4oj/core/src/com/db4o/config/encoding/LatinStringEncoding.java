/* Copyright (C) 2008  db4objects Inc.  http://www.db4o.com */

package com.db4o.config.encoding;

import com.db4o.foundation.*;
import com.db4o.internal.*;

/**
 * @exclude
 */
public class LatinStringEncoding extends BuiltInStringEncoding{

	public String decode(byte[] bytes, int start, int length) {
		throw new NotImplementedException();  // special StringIO, should never be called
	}

	public byte[] encode(String str) {
		throw new NotImplementedException();  // special StringIO, should never be called
	}
	
	protected LatinStringIO createStringIo(StringEncoding encoding) {
		return new LatinStringIO();
	}

}
