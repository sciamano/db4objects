/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

package com.db4o;

import com.db4o.foundation.*;
import com.db4o.foundation.network.*;

class YapClientBlobThread extends Thread{
	private YapClient			stream;
	private Queue4 				queue = new Queue4();
	private boolean				terminated = false;
	
	YapClientBlobThread(YapClient aStream){
		stream = aStream;
		setPriority(MIN_PRIORITY);
	}
	
	void add(MsgBlob msg){
		synchronized(queue){
			queue.add(msg);
		}
	}
	
	synchronized boolean isTerminated(){
		return terminated;
	}
	
	public void run(){
		try{
			YapSocket socket = stream.createParalellSocket();
			
			MsgBlob msg = null;
			
			// no blobLock synchronisation here, since our first msg is valid
			synchronized(queue){
				msg = (MsgBlob)queue.next();
			}
			
			while(msg != null){
				msg.write(stream, socket);
				msg.processClient(socket);
				synchronized(stream.blobLock){
					synchronized(queue){
						msg = (MsgBlob)queue.next();
					}
					if(msg == null){
						terminated = true;
						Msg.CLOSE.write(stream, socket);
						try{
							socket.close();
						}catch(Exception e){
						}
					}
				}
			}
		}catch(Exception e){
			e.printStackTrace();
		}
	}
	
}
