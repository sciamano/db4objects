/* Copyright (C) 2004 - 2010  Versant Inc.  http://www.db4o.com */

package com.db4o.drs.versant;

import java.io.*;

import com.db4o.internal.*;

public class EventConfiguration {
	
	public final String databaseName;
	
	public final String logFileName;
	
	public final String serverHost;
	
	public final int serverPort;
	
	public final String clientHost;
	
	public final int clientPort;
	
	public final boolean verbose;
	
	public EventConfiguration(String databaseName, String logFileName, String serverHost, int serverPort, String clientHost, int clientPort, boolean verbose) {
		this.databaseName = databaseName;
		this.logFileName = logFileName;
		this.serverHost = serverHost;
		this.serverPort = serverPort;
		this.clientHost = clientHost;
		this.clientPort = clientPort;
		this.verbose = verbose;
	}

	public void writeConfigFile(File file) throws IOException {
		FileWriter fileWriter = new FileWriter(file);
		PrintWriter writer = new PrintWriter(fileWriter);
		writer.println("ChannelServicePort " + serverPort);
		writer.println("Log " + logFileName);
		writer.println("LogLevel 2 ");
		
		System.err.println("Channel engine is only configured for windows here.");
		
		writer.println("<EngineLibs>");
		writer.println("vedse.dll");
		writer.println("</EngineLibs>");
		
		writer.flush();
		writer.close();
	}
	
	@Override
	public String toString() {
		return Reflection4.dump(this);
	}

}