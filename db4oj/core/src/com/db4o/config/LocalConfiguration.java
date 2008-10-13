/* Copyright (C) 2008  db4objects Inc.   http://www.db4o.com */

package com.db4o.config;

import java.io.IOException;

import com.db4o.ext.DatabaseReadOnlyException;
import com.db4o.foundation.NotSupportedException;
import com.db4o.io.IoAdapter;

/**
 * @since 7.5
 */
public interface LocalConfiguration {

    /**
     * sets the storage data blocksize for new ObjectContainers. 
     * <br><br>The standard setting is 1 allowing for a maximum
     * database file size of 2GB. This value can be increased
     * to allow larger database files, although some space will
     * be lost to padding because the size of some stored objects
     * will not be an exact multiple of the block size. A 
     * recommended setting for large database files is 8, since
     * internal pointers have this length.<br><br>
     * This setting is only effective when the database is first created, in 
     * client-server environment in most cases it means that the setting 
     * should be used on the server side.
     * @param bytes the size in bytes from 1 to 127
     * 
     * @sharpen.property
     */
    public void blockSize(int bytes);
    
	/**
	 * configures the size database files should grow in bytes, when no 
	 * free slot is found within.
	 * <br><br>Tuning setting.
	 * <br><br>Whenever no free slot of sufficient length can be found 
	 * within the current database file, the database file's length
	 * is extended. This configuration setting configures by how much
	 * it should be extended, in bytes.<br><br>
	 * This configuration setting is intended to reduce fragmentation.
	 * Higher values will produce bigger database files and less
	 * fragmentation.<br><br>
	 * To extend the database file, a single byte array is created 
	 * and written to the end of the file in one write operation. Be 
	 * aware that a high setting will require allocating memory for 
	 * this byte array.
	 *  
     * @param bytes amount of bytes
     * 
     * @sharpen.property
     */
    public void databaseGrowthSize(int bytes);

    /**
     * turns commit recovery off.
     * <br><br>db4o uses a two-phase commit algorithm. In a first step all intended
     * changes are written to a free place in the database file, the "transaction commit
     * record". In a second step the
     * actual changes are performed. If the system breaks down during commit, the
     * commit process is restarted when the database file is opened the next time.
     * On very rare occasions (possibilities: hardware failure or editing the database
     * file with an external tool) the transaction commit record may be broken. In this
     * case, this method can be used to try to open the database file without commit
     * recovery. The method should only be used in emergency situations after consulting
     * db4o support. 
     */
    public void disableCommitRecovery();
    
    /**
     * returns the freespace configuration interface.
     * 
     * @sharpen.property
     */
    public FreespaceConfiguration freespace();

    /**
     * configures db4o to generate UUIDs for stored objects.
     * 
     * This setting should be used when the database is first created.<br><br>
     * @param setting the scope for UUID generation: disabled, generate for all classes, or configure individually
     * 
     * @sharpen.property
     */
    public void generateUUIDs(ConfigScope setting);

    /**
     * configures db4o to generate version numbers for stored objects.
     * 
     * This setting should be used when the database is first created.
     * @param setting the scope for version number generation: disabled, generate for all classes, or configure individually
     * 
     * @sharpen.property
     */
    public void generateVersionNumbers(ConfigScope setting);

    /**
     * allows to configure db4o to use a customized byte IO adapter.
     * <br><br>Derive from the abstract class {@link IoAdapter} to
     * write your own. Possible usecases could be improved performance
     * with a native library, mirrored write to two files, encryption or 
     * read-on-write fail-safety control.<br><br>An example of a custom
     * io adapter can be found in xtea_db4o community project:<br>
     * http://developer.db4o.com/ProjectSpaces/view.aspx/XTEA<br><br>
     * In client-server environment this setting should be used on the server 
     * (adapter class must be available)<br><br>
     * @param adapter - the IoAdapter
     * 
     * @sharpen.property
     */
    public void io(IoAdapter adapter) throws GlobalOnlyConfigException;

    /**
     * returns the configured {@link IoAdapter}.
     * 
     * @return
     * 
     * @sharpen.property
     */
    public IoAdapter io();

    /**
     * can be used to turn the database file locking thread off. 
     * <br><br>Since Java does not support file locking up to JDK 1.4,
     * db4o uses an additional thread per open database file to prohibit
     * concurrent access to the same database file by different db4o
     * sessions in different VMs.<br><br>
     * To improve performance and to lower ressource consumption, this
     * method provides the possibility to prevent the locking thread
     * from being started.<br><br><b>Caution!</b><br>If database file
     * locking is turned off, concurrent write access to the same
     * database file from different JVM sessions will <b>corrupt</b> the
     * database file immediately.<br><br> This method
     * has no effect on open ObjectContainers. It will only affect how
     * ObjectContainers are opened.<br><br>
     * The default setting is <code>true</code>.<br><br>
     * In client-server environment this setting should be used on both client and server.<br><br>  
     * @param flag <code>false</code> to turn database file locking off.
     * 
     * @sharpen.property
     */
    public void lockDatabaseFile(boolean flag);

    /**
     * tuning feature only: reserves a number of bytes in database files.
     * <br><br>The global setting is used for the creation of new database
     * files. Continous calls on an ObjectContainer Configuration context
     * (see  {@link com.db4o.ext.ExtObjectContainer#configure()}) will
     * continually allocate space. 
     * <br><br>The allocation of a fixed number of bytes at one time
     * makes it more likely that the database will be stored in one
     * chunk on the mass storage. Less read/write head movement can result
     * in improved performance.<br><br>
     * <b>Note:</b><br> Allocated space will be lost on abnormal termination
     * of the database engine (hardware crash, VM crash). A Defragment run
     * will recover the lost space. For the best possible performance, this
     * method should be called before the Defragment run to configure the
     * allocation of storage space to be slightly greater than the anticipated
     * database file size.
     * <br><br> 
     * In client-server environment this setting should be used on the server side. <br><br>
     * Default configuration: 0<br><br> 
     * @param byteCount the number of bytes to reserve
     * 
     * @sharpen.property
     */
    public void reserveStorageSpace(long byteCount) throws DatabaseReadOnlyException, NotSupportedException;
    
    /**
     * configures the path to be used to store and read 
     * Blob data.
     * <br><br>
     * In client-server environment this setting should be used on the
     * server side. <br><br>
     * @param path the path to be used
     * 
     * @sharpen.property
     */
    public void blobPath(String path) throws IOException;
    
    /**
     * turns readOnly mode on and off.
     * <br><br>This method configures the mode in which subsequent calls to
     * {@link com.db4o.Db4o#openFile Db4o.openFile()} will open files.
     * <br><br>Readonly mode allows to open an unlimited number of reading
     * processes on one database file. It is also convenient
     * for deploying db4o database files on CD-ROM.<br><br>
     * In client-server environment this setting should be used on the server side 
     * in embedded mode and ONLY on client side in networked mode.<br><br>
     * @param flag <code>true</code> for configuring readOnly mode for subsequent
     * calls to {@link com.db4o.Db4o#openFile Db4o.openFile()}.
     * 
     * TODO: this is rather embedded + client than base?
     * 
     * @sharpen.property
     */
    public void readOnly(boolean flag);


}
