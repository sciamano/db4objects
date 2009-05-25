/* Copyright (C) 2004   Versant Inc.   http://www.db4o.com */

package com.db4o.internal.cs.objectexchange;

import com.db4o.foundation.*;
import com.db4o.internal.*;
import com.db4o.internal.cs.*;

public class EagerObjectExchangeStrategy implements ObjectExchangeStrategy {

	private ObjectExchangeConfiguration _config;

	public EagerObjectExchangeStrategy(ObjectExchangeConfiguration config) {
		_config = config;
    }

	public ByteArrayBuffer marshall(LocalTransaction transaction, IntIterator4 ids, int maxCount) {
	   return new EagerObjectWriter(_config, transaction).write(ids, maxCount);
    }

	public FixedSizeIntIterator4 unmarshall(ClientTransaction transaction, ByteArrayBuffer reader) {
		return new EagerObjectReader(transaction, reader).iterator();
    }

}
