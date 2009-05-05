/* Copyright (C) 2008  Versant Inc.  http://www.db4o.com */

package com.db4o.bench.logging.replay.commands;

import com.db4o.io.*;

public class ReadCommand extends ReadWriteCommand implements IoCommand{
	
	public ReadCommand(int length) {
		super(length);
	}
	
	public void replay(IoAdapter adapter){
		adapter.read(prepareBuffer(), _length);
	}

}
