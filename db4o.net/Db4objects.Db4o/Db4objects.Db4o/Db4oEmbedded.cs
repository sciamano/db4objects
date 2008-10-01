/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o
{
	public class Db4oEmbedded
	{
		/// <summary>
		/// Creates a fresh
		/// <see cref="IConfiguration">IConfiguration</see>
		/// instance.
		/// </summary>
		/// <returns>a fresh, independent configuration with all options set to their default values
		/// 	</returns>
		public static IConfiguration NewConfiguration()
		{
			Config4Impl config = new Config4Impl();
			Platform4.GetDefaultConfiguration(config);
			return config;
		}

		/// <summary>
		/// opens an
		/// <see cref="IObjectContainer">IObjectContainer</see>
		/// on the specified database file for local use.
		/// <br /><br />A database file can only be opened once, subsequent attempts to open
		/// another
		/// <see cref="IObjectContainer">IObjectContainer</see>
		/// against the same file will result in
		/// a
		/// <see cref="DatabaseFileLockedException">DatabaseFileLockedException</see>
		/// .<br /><br />
		/// Database files can only be accessed for readwrite access from one process
		/// (one Java VM) at one time. All versions except for db4o mobile edition use an
		/// internal mechanism to lock the database file for other processes.
		/// <br /><br />
		/// </summary>
		/// <param name="config">
		/// a custom
		/// <see cref="IConfiguration">IConfiguration</see>
		/// instance to be obtained via
		/// <see cref="newConfiguration">newConfiguration</see>
		/// </param>
		/// <param name="databaseFileName">an absolute or relative path to the database file</param>
		/// <returns>
		/// an open
		/// <see cref="IObjectContainer">IObjectContainer</see>
		/// </returns>
		/// <seealso cref="IConfiguration.ReadOnly">IConfiguration.ReadOnly</seealso>
		/// <seealso cref="IConfiguration.Encrypt">IConfiguration.Encrypt</seealso>
		/// <seealso cref="IConfiguration.Password">IConfiguration.Password</seealso>
		/// <exception cref="Db4oIOException">I/O operation failed or was unexpectedly interrupted.
		/// 	</exception>
		/// <exception cref="DatabaseFileLockedException">
		/// the required database file is locked by
		/// another process.
		/// </exception>
		/// <exception cref="IncompatibleFileFormatException">
		/// runtime
		/// <see cref="IConfiguration">configuration</see>
		/// is not compatible
		/// with the configuration of the database file.
		/// </exception>
		/// <exception cref="OldFormatException">
		/// open operation failed because the database file
		/// is in old format and
		/// <see cref="IConfiguration.AllowVersionUpdates">IConfiguration.AllowVersionUpdates
		/// 	</see>
		/// 
		/// is set to false.
		/// </exception>
		/// <exception cref="DatabaseReadOnlyException">database was configured as read-only.
		/// 	</exception>
		public static IObjectContainer OpenFile(IConfiguration config, string databaseFileName
			)
		{
			if (null == config)
			{
				throw new ArgumentNullException();
			}
			return ObjectContainerFactory.OpenObjectContainer(config, databaseFileName);
		}
	}
}