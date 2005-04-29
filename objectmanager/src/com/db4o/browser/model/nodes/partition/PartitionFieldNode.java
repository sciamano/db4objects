/*
 * Copyright (C) 2005 db4objects Inc.  http://www.db4o.com
 */
package com.db4o.browser.model.nodes.partition;

import com.db4o.browser.model.IDatabase;
import com.db4o.browser.model.nodes.IModelNode;

public class PartitionFieldNode implements IModelNode {

    private IDatabase _database;
    private IModelNode[] _sourceNodes;
    private int _startIdx;
    private int _endIdx;

    public PartitionFieldNode(IDatabase database, IModelNode[] sourceNodes, int startIdx, int endIdx) {
        _database=database;
        _sourceNodes=sourceNodes;
        _startIdx=startIdx;
        _endIdx=endIdx;
    }

    public boolean hasChildren() {
        return true;
    }

    public IModelNode[] children() {
        return PartitionFieldNodeFactory.create(_sourceNodes, _startIdx, _endIdx, _database);
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
