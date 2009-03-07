using System;
using System.Collections;
using System.Collections.Generic;
using Db4objects.Db4o.Reflect;
using OManager.BusinessLayer.QueryManager;
using OManager.DataLayer.ObjectsModification;
using OManager.DataLayer.Modal;
using OManager.DataLayer.QueryParser;
using OManager.DataLayer.Connection;
using OManager.BusinessLayer.Login;
using OManager.DataLayer.PropertyTable;
using System.Windows.Forms;
using OManager.DataLayer.Maintanence;
using OManager.DataLayer.Reflection;
using OME.AdvancedDataGridView;
using OManager.DataLayer.CommonDatalayer;
using OManager.DataLayer.DemoDBCreation;
using OME.Logging.Common;

namespace OManager.BusinessLayer.UIHelper
{
	public class dbInteraction
	{
		readonly DbInformation  dbInfo;
		readonly RenderHierarchy clsRenderHierarchy;

		public dbInteraction()
		{
			dbInfo = new DbInformation(); 
			clsRenderHierarchy = new RenderHierarchy();
		}

		public static void EditObject(object parentobjm, string attribute, string value)
		{
			ModifyObjects.EditObjects(parentobjm, attribute, value);
		}

		public static void DeleteObject(object obj, bool boolcascadeOnDelete)
		{
			new ModifyObjects(obj).CascadeonDelete(boolcascadeOnDelete);  
		}

        public static IType GetFieldType(string declaringClassName, string name)
        {
            return new FieldDetails(declaringClassName, name).GetFieldType();
        }

	    public static object SaveObjects(object parentobjm)
		{
			ModifyObjects.SaveObjects(parentobjm);
			return parentobjm;
		}

		public static void RefreshObject(object obj, int level)
		{
			Db4oClient.Client.Ext().Refresh(obj, level);
		}

		public void UpdateCollection(IList objList, IList<int> offsetList, IList<string> names, IList<IType> types, object value)
		{
			try
			{
				ModifyCollections modColl = new ModifyCollections();
				modColl.EditCollections(objList, offsetList, names, types, value);
			}
			catch (Exception oEx)
			{
				LoggingHelper.HandleException(oEx);
			}
		}

		public static void SetFieldToNull(object obj, string fieldName)
		{
			try
			{
				IReflectClass klass = DataLayerCommon.ReflectClassFor(obj);
				if (klass != null)
				{
					IReflectField field = DataLayerCommon.GetDeclaredField(klass, fieldName);
					if (field == null)
						return;
					
					field.Set(obj, null);
					Db4oClient.Client.Store(obj);
					Db4oClient.Client.Commit();
				}
			}
			catch (Exception oEx)
			{
				LoggingHelper.HandleException(oEx);
			}
		}

		public void SaveCollection(object obj, int depth)
		{
			ModifyCollections.SaveCollections(obj, depth);
		}

		public Hashtable FetchAllStoredClasses()
		{
			return dbInfo.StoredClasses();
		}

		public Hashtable FetchAllStoredClassesForAssembly()
		{
			return dbInfo.StoredClassesByAssembly(); 
		}
		public static Hashtable FetchStoredFields(string classname)
		{
			ClassDetails clsDetails = new ClassDetails(classname);
			return clsDetails.GetFields();
		}

		public int NoOfClassesInDb()
		{
			return dbInfo.GetNumberOfClassesinDB();
		}

		public long GetFreeSizeOfDb()
		{
			return dbInfo.GetFreeSizeofDatabase();
		}

		public long GetTotalDbSize()
		{
			return dbInfo.getTotalDatabaseSize();
		}

		public static int NoOfObjectsforAClass(string classname)
		{
			ClassDetails db = new ClassDetails(classname);
			return db.GetNumberOfObjects();
		}

		public static bool CheckForPrimitiveFields(string classname, string fieldname)
		{
			FieldDetails fl = new FieldDetails(classname, fieldname);
			return fl.IsPrimitive();
		}

		public static bool CheckForCollection(string classname, string fieldname)
		{
			FieldDetails fl = new FieldDetails(classname, fieldname);
			return fl.IsCollection();
		}

		public static bool CheckForArray(string classname, string fieldname)
		{
			FieldDetails fl = new FieldDetails(classname, fieldname);
			return fl.IsArray();
		}

		public static void SetProxyInfo(ProxyAuthentication proxyInfo)
		{
			ProxyAuthenticator proxyAuth = new ProxyAuthenticator();
			proxyAuth.AddProxyInfoToDb(proxyInfo);
		}

		public static ProxyAuthentication RetrieveProxyInfo()
		{
			ProxyAuthenticator proxyAuth = new ProxyAuthenticator();
			proxyAuth = proxyAuth.ReturnProxyAuthenticationInfo();
			if (proxyAuth != null)
				return proxyAuth.ProxyAuthObj;
			
			return null;
		}

		public static List<string> GetSearchString(ConnParams conn)
		{
			GroupofSearchStrings searchStrings = new GroupofSearchStrings(conn);
			return searchStrings.ReturnStringList();
		}


		public static void SaveSearchString(ConnParams conn, SeachString searchString)
		{
			GroupofSearchStrings searchStrings = new GroupofSearchStrings(conn);
			if (searchString.SearchString != string.Empty)
				searchStrings.AddSearchStringToList(searchString);
		}

		public static List<FavouriteFolder> GetFavourites(ConnParams conn)
		{
			FavouriteList lstFav = new FavouriteList(conn);
			return lstFav.ReturnFavouritFolderList(); 
		}

		public static void SaveFavourite(ConnParams conn, FavouriteFolder FavFolder)
		{
			FavouriteList lstFav = new FavouriteList(conn);
			lstFav.AddFolderToDatabase(FavFolder);
		}
		public static void UpdateFavourite(ConnParams conn, FavouriteFolder FavFolder)
		{
			FavouriteList lstFav = new FavouriteList(conn);
			lstFav.RemoveFolderfromDatabase(FavFolder);  
		}

		public static void RenameFolderInDatabase(ConnParams conn, FavouriteFolder oldFav, FavouriteFolder newFav)
		{
			FavouriteList lstFav = new FavouriteList(conn);
			lstFav.RenameFolderInDatabase(oldFav, newFav);  
		}

		public static FavouriteFolder GetFolderfromDatabaseByFoldername(ConnParams conn, string folderName)
		{
			FavouriteList lstFav = new FavouriteList(conn);
			lstFav = lstFav.FindFolderWithClassesByFolderName(folderName);
			return lstFav.lstFavFolder[0]; 
		}
       
		public void ExpandTreeNode(TreeGridNode node,bool activate)
		{
			if (IsCollection(node.Tag))
				clsRenderHierarchy.ExpandCollectionNode(node);
			else if (IsArray(node.Tag))
				clsRenderHierarchy.ExpandArrayNode(node);
			else if (IsPrimitive(node.Tag))
				return;
			else
				clsRenderHierarchy.ExpandObjectNode(node, activate);
		}

		public static bool IsCollection(object expandedObject)
		{
			return DataLayerCommon.IsCollection(expandedObject);
		}

		public static bool IsPrimitive(object expandedObject)
		{
			return DataLayerCommon.IsPrimitive(expandedObject);
		}

		public static bool CheckForDateTimeOrString(object expandedObject)
		{
			return DataLayerCommon.CheckForDatetimeOrString(expandedObject);
		}

		public static object GetObjById(long id)
		{
			ObjectDetails objDetails = new ObjectDetails(null);
			return objDetails.GetObjById(id); 

		}
		public static long GetLocalID(object obj)
		{
			ObjectDetails objDetails = new ObjectDetails(obj);
			return objDetails.GetLocalID();
		}
		public static int GetDepth(object obj)
		{
			ObjectDetails objDetails = new ObjectDetails(obj);
			return objDetails.GetDepth(obj);
		}

		public static bool IsArray(object expandedObject)
		{
			return DataLayerCommon.IsArray(expandedObject);
		}

		public static TreeGridView GetObjectHierarchy(object selectedObj,string classname)
		{
			return new RenderHierarchy().ReturnHierarchy(selectedObj, classname);
		}

		public static long[] ExecuteQueryResults(OMQuery omQuery)
		{
			return RunQuery.ExecuteQuery(omQuery);
		}

		public static List<Hashtable> ExecuteQueryResults(OMQuery omQuery, PagingData pgData, bool refresh, Hashtable attributeList)
		{
			RunQuery.ExecuteQuery(omQuery);
			return ReturnQueryResults(pgData, refresh, omQuery.BaseClass, attributeList);
		}

		public static List<Hashtable> ReturnQueryResults(PagingData pagData, bool Refresh, string baseclass, Hashtable attributeList)
		{
			return RunQuery.ReturnResults(pagData, Refresh, baseclass, attributeList);  
        }

		public static bool Cascadeondelete(object obj, bool checkforCascade)
		{
			new ModifyObjects(obj).CascadeonDelete(checkforCascade);
			return false;
		}

		public static bool DefragDatabase(string ConnectionPath)
		{
			db4oDefrag defrag = new db4oDefrag(ConnectionPath);
			bool check = false;
			try
			{
				defrag.db4oDefragDatabase();
			}
			catch (Exception oEx)
			{
				check = true;
				LoggingHelper.HandleException(oEx);
			}

			return check;
		}

		public static bool BackUpDatabase(string LocationToBackUp)
		{
			db4oBackup backup = new db4oBackup(LocationToBackUp); 
			bool check = false;
			try
			{
				backup.db4oBackupDatabase();
			}
			catch (Exception oEx)
			{
				check = true;
				LoggingHelper.HandleException(oEx);
				MessageBox.Show(oEx.Message, "ObjectManager Enterprise",MessageBoxButtons.OK,MessageBoxIcon.Error);
			}

			return check;            
		}

		public static List<RecentQueries> FetchRecentQueries()
		{
			return Config.Config.Instance.GetRecentQueries();
		}

		public static int GetFieldCount(string classname)
		{
			ClassDetails clsDetails = new ClassDetails(classname);
			return clsDetails.GetFieldCount();
         
		}

		public static ClassPropertiesTable GetClassProperties(string classname)
		{
			ClassPropertiesTable classtable = new ClassPropertiesTable(classname);            
			return classtable.GetClassProperties();
		}
		public static ObjectPropertiesTable GetObjectProperties(object obj)
		{
			ObjectPropertiesTable objtable = new ObjectPropertiesTable(obj);
			return objtable.GetObjectProperties();
		}

		public static string ConnectoToDB(RecentQueries recConnection)
		{
			DBConnect db = new DBConnect();
			return db.dbConnection(recConnection.ConnParam);                        

		}

		public static void Closedb(RecentQueries recConnection)
		{
			if (Db4oClient.RecentConnFile==null)
			{
				Db4oClient.RecentConnFile = Config.Config.OMNConfigDatabasePath();
			}
			SaveRecentConnection(Db4oClient.CurrentRecentConnection);
			Db4oClient.CloseConnection();
			Db4oClient.CloseRecentConnectionFile(Db4oClient.RecentConn);
		}

		public static void CloseCurrDb()
		{
			Db4oClient.CloseConnection();
		}

		public static void CloseRecentConn()
		{
			Db4oClient.CloseRecentConnectionFile(Db4oClient.RecentConn);
		}

		public static void SetIndexedConfiguration(string fieldname, string className, bool isIndexed)
		{
			ClassPropertiesTable classtable = new ClassPropertiesTable(className);
			classtable.SetIndex(fieldname, className, isIndexed);
		}

		public static RecentQueries GetCurrentRecentConnection()
		{
			return Db4oClient.CurrentRecentConnection;
		}

		public static void SetCurrentRecentConnection(RecentQueries conn)
		{
			Db4oClient.CurrentRecentConnection = conn;
		}

		public static void SaveRecentConnection(RecentQueries recQueries)
		{
			Config.Config.Instance.SaveRecentConnection(recQueries);
		}

		public static bool CreateDemoDb(string demoFilePath)
		{
			try
			{
				DemoDatabaseCreation dbCreationObj = new DemoDatabaseCreation();
				dbCreationObj.CreateDemoDb(demoFilePath);
				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}
	}
}