/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MObjectSetFinalized : MsgD, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			int queryResultID = ReadInt();
			ServerMessageDispatcher().QueryResultFinalized(queryResultID);
			return true;
		}
	}
}