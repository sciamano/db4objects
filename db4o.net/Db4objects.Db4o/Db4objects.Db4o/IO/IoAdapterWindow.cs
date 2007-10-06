/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.IO
{
	/// <summary>Bounded handle into an IoAdapter: Can only access a restricted area.</summary>
	/// <remarks>Bounded handle into an IoAdapter: Can only access a restricted area.</remarks>
	public class IoAdapterWindow
	{
		private IoAdapter _io;

		private int _blockOff;

		private int _len;

		private bool _disabled;

		/// <param name="io">The delegate I/O adapter</param>
		/// <param name="blockOff">The block offset address into the I/O adapter that maps to the start index (0) of this window
		/// 	</param>
		/// <param name="len">The size of this window in bytes</param>
		public IoAdapterWindow(IoAdapter io, int blockOff, int len)
		{
			_io = io;
			_blockOff = blockOff;
			_len = len;
			_disabled = false;
		}

		/// <returns>Size of this I/O adapter window in bytes.</returns>
		public virtual int Length()
		{
			return _len;
		}

		/// <param name="off">Offset in bytes relative to the window start</param>
		/// <param name="data">Data to write into the window starting from the given offset</param>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public virtual void Write(int off, byte[] data)
		{
			CheckBounds(off, data);
			_io.BlockSeek(_blockOff + off);
			_io.Write(data);
		}

		/// <param name="off">Offset in bytes relative to the window start</param>
		/// <param name="data">Data buffer to read from the window starting from the given offset
		/// 	</param>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public virtual int Read(int off, byte[] data)
		{
			CheckBounds(off, data);
			_io.BlockSeek(_blockOff + off);
			return _io.Read(data);
		}

		/// <summary>Disable IO Adapter Window</summary>
		public virtual void Disable()
		{
			_disabled = true;
		}

		/// <summary>Flush IO Adapter Window</summary>
		public virtual void Flush()
		{
			if (!_disabled)
			{
				_io.Sync();
			}
		}

		private void CheckBounds(int off, byte[] data)
		{
			if (_disabled)
			{
				throw new InvalidOperationException();
			}
			if (data == null || off < 0 || off + data.Length > _len)
			{
				throw new ArgumentException();
			}
		}
	}
}
