/* Copyright (C) 2008  Versant Inc.  http://www.db4o.com */

package com.db4o.bench.logging.replay.commands;

import com.db4o.io.*;

public class SeekCommand implements IoCommand{
	
	private final int _address;
	
	public SeekCommand(int address) {
		_address = address;
	}
	
	public void replay(IoAdapter adapter){
		adapter.seek(_address);
	}

}
