/* Copyright (C) 2006  Versant Inc.  http://www.db4o.com */

package com.db4o.internal.fileheader;

import com.db4o.*;
import com.db4o.ext.*;
import com.db4o.internal.*;


/**
 * @exclude
 */
public abstract class FileHeader {
    
    public static final int TRANSACTION_POINTER_LENGTH = Const4.INT_LENGTH * 2;
    
	private static final FileHeader[] AVAILABLE_FILE_HEADERS = new FileHeader[]{
        new FileHeader1(),
        new FileHeader2(),
    };
	
	public static NewFileHeaderBase newCurrentFileHeader(){
		return new FileHeader2();
	}
    
    private static int readerLength(){
        int length = AVAILABLE_FILE_HEADERS[0].length();
        for (int i = 1; i < AVAILABLE_FILE_HEADERS.length; i++) {
            length = Math.max(length, AVAILABLE_FILE_HEADERS[i].length());
        }
        return length;
    }

    public static FileHeader read(LocalObjectContainer file) throws OldFormatException {
        ByteArrayBuffer reader = prepareFileHeaderReader(file);
        FileHeader header = detectFileHeader(file, reader);
        if(header == null){
            Exceptions4.throwRuntimeException(Messages.INCOMPATIBLE_FORMAT, file.toString());
        } else {
        	header.read(file, reader);
        }
        return header;
    }

	public FileHeader convert(LocalObjectContainer file){
		FileHeader2 fileHeader = new FileHeader2();
		fileHeader.initNew(file);
		return fileHeader;
	}

	private static ByteArrayBuffer prepareFileHeaderReader(LocalObjectContainer file) {
		ByteArrayBuffer reader = new ByteArrayBuffer(readerLength()); 
        reader.read(file, 0, 0);
		return reader;
	}

	private static FileHeader detectFileHeader(LocalObjectContainer file, ByteArrayBuffer reader) {
        for (int i = 0; i < AVAILABLE_FILE_HEADERS.length; i++) {
            reader.seek(0);
            FileHeader result = AVAILABLE_FILE_HEADERS[i].newOnSignatureMatch(file, reader);
            if(result != null) {
            	return result;
            }
        }
		return null;
	}

    public abstract void close() throws Db4oIOException;

    public abstract void initNew(LocalObjectContainer file) throws Db4oIOException;

    public abstract void completeInterruptedTransaction(LocalObjectContainer container);

    public abstract int length();
    
    protected abstract FileHeader newOnSignatureMatch(LocalObjectContainer file, ByteArrayBuffer reader);
    
    protected long timeToWrite(long time, boolean shuttingDown) {
    	return shuttingDown ? 0 : time;
    }

    protected abstract void read(LocalObjectContainer file, ByteArrayBuffer reader);

    protected boolean signatureMatches(ByteArrayBuffer reader, byte[] signature, byte version){
        for (int i = 0; i < signature.length; i++) {
            if(reader.readByte() != signature[i]){
                return false;
            }
        }
        return reader.readByte() == version; 
    }
    
    // TODO: freespaceID should not be passed here, it should be taken from SystemData
    public abstract void writeFixedPart(
        LocalObjectContainer file, boolean startFileLockingThread, boolean shuttingDown, StatefulBuffer writer, int blockSize);
    
    public abstract void writeTransactionPointer(Transaction systemTransaction, int transactionPointer);

    protected void writeTransactionPointer(Transaction systemTransaction, int transactionPointer, final int address, final int offset) {
        StatefulBuffer bytes = new StatefulBuffer(systemTransaction, address, TRANSACTION_POINTER_LENGTH);
        bytes.moveForward(offset);
        bytes.writeInt(transactionPointer);
        bytes.writeInt(transactionPointer);
        if (Debug4.xbytes) {
        	bytes.checkXBytes(false);
        }
        // Dangerous write. 
        // On corruption transaction pointers will not be the same and nothing will happen.
        bytes.write();
    }
    
    public void writeVariablePart(LocalObjectContainer file){
    	writeVariablePart(file, false);
    }
    
    public abstract void writeVariablePart(LocalObjectContainer file, boolean shuttingDown);

    public static boolean lockedByOtherSession(LocalObjectContainer container, long lastAccessTime) {
		return container.needsLockFileThread() && ( lastAccessTime != 0);
	}

	public abstract void readIdentity(LocalObjectContainer container);
	
	public abstract Runnable commit(boolean shuttingDown);

}
