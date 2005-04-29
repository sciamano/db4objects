/*
 * Copyright (C) 2005 db4objects Inc.  http://www.db4o.com
 */
package com.db4o.browser.model.nodes.partition;

import com.db4o.ObjectSet;
import com.db4o.browser.model.IDatabase;
import com.db4o.browser.model.nodes.IModelNode;

public class PartitionObjectSetNode implements IModelNode {

    private ObjectSet _source;
    private IDatabase _database;
    private long[] _sourceIds;
    private int _startIdx;
    private int _endIdx;

    public PartitionObjectSetNode(ObjectSet source, IDatabase database, long[] sourceIds, int startIdx, int endIdx) {
        _source = source;
        _database=database;
        _sourceIds=sourceIds;
        _startIdx=startIdx;
        _endIdx=endIdx;
    }

    public boolean hasChildren() {
        return true;
    }

    public IModelNode[] children() {
        return PartitionFieldNodeFactory.create(_source, _sourceIds, _startIdx, _endIdx, _database);
    }

    public String getText() {
        return "More: ["+_startIdx+".."+_endIdx+"]";
    }

    public String getName() {
        return "More: ["+_startIdx+".."+_endIdx+"]";
    }

    public String getValueString() {
        return "";
    }

}
