/* Copyright (C) 2007  db4objects Inc.  http://www.db4o.com */

package com.db4o.internal.marshall;

import com.db4o.*;
import com.db4o.internal.*;
import com.db4o.marshall.*;


/**
 * @exclude
 */
public class UnmarshallingContext implements FieldListInfo, ReadContext{
    
    private final Transaction _transaction;
    
    private final ObjectReference _ref;
    
    private Object _object;
    
    private Buffer _buffer;
    
    private ObjectHeader _objectHeader;
    
    private int _addToIDTree;
    
    private boolean _checkIDTree;
    
    private int _activationDepth;
    
    public UnmarshallingContext(Transaction transaction, ObjectReference ref, int addToIDTree, boolean checkIDTree) {
        _transaction = transaction;
        _ref = ref;
        _addToIDTree = addToIDTree;
        _checkIDTree = checkIDTree;
    }

    public void buffer(Buffer buffer) {
        _buffer = buffer;
    }
    
    public Buffer buffer() {
        return _buffer;
    }
    
    public StatefulBuffer statefulBuffer() {
        StatefulBuffer buffer = new StatefulBuffer(_transaction, _buffer.length());
        buffer.setID(objectID());
        buffer.setInstantiationDepth(activationDepth());
        _buffer.copyTo(buffer, 0, 0, _buffer.length());
        buffer.offset(_buffer.offset());
        return buffer;
    }
    
    public int objectID(){
        return _ref.getID();
    }
    
    public ObjectContainerBase container(){
        return _transaction.container();
    }
    
    public Object read(){
        if(! beginProcessing()){
            return _object;
        }
        
        if (_buffer == null && objectID() > 0) {
            _buffer = container().readReaderByID(_transaction, objectID()); 
        }
        
        if(_buffer == null){
            endProcessing();
            return _object;
        }
        
        _objectHeader = new ObjectHeader(container(), _buffer);
        
        ClassMetadata classMetadata = _objectHeader.classMetadata();
        
        if(classMetadata == null){
            endProcessing();
            return _object;
        }
        
        _ref.classMetadata(classMetadata);
        
        
        if(_checkIDTree){
            Object objectInCacheFromClassCreation = _transaction.objectForIdFromCache(objectID());
            if(objectInCacheFromClassCreation != null){
                _object = objectInCacheFromClassCreation;
                endProcessing();
                return _object;
            }
        }
        
        if(_addToIDTree == Const4.TRANSIENT){
            _object = classMetadata().instantiateTransient(this);
        }else{
            _object = classMetadata().instantiate(this);
        }
        
        endProcessing();
        return _object;
    }
    
    public ClassMetadata classMetadata(){
        return _ref.classMetadata();
    }
        
    private boolean beginProcessing() {
        return _ref.beginProcessing();
    }
    
    private void endProcessing() {
        _ref.endProcessing();
    }

    public void setStateClean() {
        _ref.setStateClean();
    }

    public Object persistentObject() {
        return _object;
    }

    public void setObjectWeak(Object obj) {
        _ref.setObjectWeak(container(), obj);
    }
    
    public Object readAny(){
        int payloadOffset = readInt();
        if(payloadOffset == 0){
            return null;
        }
        int savedOffSet = offset();
        seek(payloadOffset);
        ClassMetadata classMetadata = container().classMetadataForId(readInt());
        if(classMetadata == null){
            seek(savedOffSet);
            return null;
        }
        if(classMetadata instanceof PrimitiveFieldHandler && classMetadata.isArray()){
            // unnecessary secondary offset, consistent with old format
            seek(readInt());
        }
        Object obj = classMetadata.read(this);
        seek(savedOffSet);
        return obj;
    }

    public Object readObject() {
        int id = readInt();
        int depth = _activationDepth - 1;

        if (_activationDepth == Const4.TRANSIENT) {
            return container().peekPersisted(transaction(), id, depth);
        }

        Object obj = container().getByID2(transaction(), id);

        if (obj instanceof Db4oTypeImpl) {
            depth = ((Db4oTypeImpl)obj).adjustReadDepth(depth);
        }

        // this is OK for primitive YapAnys. They will not be added
        // to the list, since they will not be found in the ID tree.
        container().stillToActivate(transaction(), obj, depth);

        return obj;
    }
    
    public Object readObject(TypeHandler4 handler) {
        if(! isIndirected(handler)){
            return handler.read(this);
        }
        int payLoadOffset = readInt();
        readInt(); // length - never used
        if(payLoadOffset == 0){
            return null;
        }
        int savedOffset = offset();
        seek(payLoadOffset);
        Object obj = handler.read(this);
        seek(savedOffset);
        return obj;
    }

    public ObjectContainer objectContainer() {
        return (ObjectContainer) container();
    }

    public Transaction transaction() {
        return _transaction;
    }

    public byte readByte() {
        return _buffer.readByte();
    }

    public void readBytes(byte[] bytes) {
        _buffer.readBytes(bytes);
    }

    public int readInt() {
        return _buffer.readInt();
    }

    public long readLong() {
        return _buffer.readLong();
    }

    public void adjustInstantiationDepth() {
        Config4Class classConfig = classConfig();
        if(classConfig != null){
            _activationDepth = classConfig.adjustActivationDepth(_activationDepth);
        }
    }
    
    public Config4Class classConfig() {
        return classMetadata().config();
    }

    public int offset() {
        return _buffer.offset();
    }

    public void seek(int offset) {
        _buffer.seek(offset);
    }

    public ObjectReference reference() {
        return _ref;
    }

    public void addToIDTree() {
        if(_addToIDTree == Const4.ADD_TO_ID_TREE){
            _ref.addExistingReferenceToIdTree(transaction());    
        }
    }

    public int activationDepth() {
        return _activationDepth;
    }
    
    public void activationDepth(int depth){
        _activationDepth = depth;
    }

    public void persistentObject(Object obj) {
        _object = obj;
    }

    public MarshallerFamily marshallerFamily() {
        return _objectHeader._marshallerFamily;
    }
    
    public ObjectHeaderAttributes headerAttributes(){
        return _objectHeader._headerAttributes;
    }

    public boolean isNull(int fieldIndex) {
        return headerAttributes().isNull(fieldIndex);
    }

    public Object read(TypeHandler4 handler) {
        if(! isIndirected(handler)){
            return handler.read(this);
        }
        int indirectedOffSet = readInt();
        readInt(); // length, not needed
        int offset = offset();
        seek(indirectedOffSet);
        Object obj = handler.read(this);
        seek(offset);
        return obj;
    }

    private boolean isIndirected(TypeHandler4 handler) {
        if(handlerVersion() == 0){
            return false;
        }
        return handlerRegistry().isVariableLength(handler);
    }
    
    private HandlerRegistry handlerRegistry(){
        return container().handlers();
    }

    public boolean oldHandlerVersion() {
        return marshallerFamily() != MarshallerFamily.current();
    }

    public int handlerVersion() {
        return marshallerFamily().handlerVersion();
    }
    
}

