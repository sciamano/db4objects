﻿/* Copyright (C) 2011 Versant Inc.   http://www.db4o.com */
using System.IO;

namespace Db4objects.Db4o.Internal.CLI
{
	internal class Silverlight4 : SilverlightCLIBase
	{
		public override void Flush(FileStream stream)
		{
			stream.Flush(true);
		}
	}
}
