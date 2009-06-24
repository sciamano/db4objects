/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Sharpen.IO;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	public class MWriteBlob : MsgBlob, IMessageWithResponse
	{
		/// <exception cref="System.IO.IOException"></exception>
		public override void ProcessClient(ISocket4 sock)
		{
			Msg message = Msg.ReadMessage(MessageDispatcher(), Transaction(), sock);
			if (message.Equals(Msg.Ok))
			{
				try
				{
					_currentByte = 0;
					_length = this._blob.GetLength();
					_blob.GetStatusFrom(this);
					_blob.SetStatus(Status.Processing);
					FileInputStream inBlob = this._blob.GetClientInputStream();
					Copy(inBlob, sock, true);
					sock.Flush();
					ObjectContainerBase stream = Stream();
					message = Msg.ReadMessage(MessageDispatcher(), Transaction(), sock);
					if (message.Equals(Msg.Ok))
					{
						// make sure to load the filename to i_blob
						// to allow client databasefile switching
						stream.Deactivate(Transaction(), _blob, int.MaxValue);
						stream.Activate(Transaction(), _blob, new FullActivationDepth());
						this._blob.SetStatus(Status.Completed);
					}
					else
					{
						this._blob.SetStatus(Status.Error);
					}
				}
				catch (Exception e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}
		}

		public virtual bool ProcessAtServer()
		{
			try
			{
				BlobImpl blobImpl = this.ServerGetBlobImpl();
				if (blobImpl != null)
				{
					blobImpl.SetTrans(Transaction());
					Sharpen.IO.File file = blobImpl.ServerFile(null, true);
					ISocket4 sock = ServerMessageDispatcher().Socket();
					Msg.Ok.Write(sock);
					FileOutputStream fout = new FileOutputStream(file);
					Copy(sock, fout, blobImpl.GetLength(), false);
					Msg.Ok.Write(sock);
				}
			}
			catch (Exception)
			{
			}
			return true;
		}
	}
}