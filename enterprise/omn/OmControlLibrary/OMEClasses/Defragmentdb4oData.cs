using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OManager.BusinessLayer.UIHelper;
using OManager.DataLayer.Connection;
using OMControlLibrary.Common;
using OManager.BusinessLayer.Login;
using OME.Logging.Common;

namespace OMControlLibrary
{
	public class Defragmentdb4oData
	{
		string strLocation;
		delegate void DefragData();
		ProgressBar p = new ProgressBar();

		public Defragmentdb4oData(string strLocation)
		{
			try
			{
				this.strLocation = strLocation;
				DefragFile();
				//DefragData UpdateProgress = new DefragData(DefragFile);
				//UpdateProgress.Invoke();
			}
			catch (Exception oEx)
			{
				LoggingHelper.ShowMessage(oEx);
			}
		}

		//private void ShowDialogforProgressBar()
		//{
		//    try
		//    {
		//        p.ShowDialog();
		//    }
		//    catch (ThreadAbortException)
		//    {
		//        System.Threading.Thread.ResetAbort();

		//    }
		//    catch (Exception oEx)
		//    {
		//        LoggingHelper.ShowMessage(oEx);
		//    }
		//}

		public void DefragFile()
		{
			// System.Threading.Thread t = null;
			try
			{
				bool checkExecption = dbInteraction.DefragDatabase(strLocation);
				ConnParams conparam = new ConnParams(strLocation);
				RecentQueries currRecentConnection = new RecentQueries(conparam);
				RecentQueries tempRc = currRecentConnection.ChkIfRecentConnIsInDb();
				if (tempRc != null)
					currRecentConnection = tempRc;
				currRecentConnection.Timestamp = DateTime.Now;
				dbInteraction.ConnectoToDB(currRecentConnection);
				dbInteraction.SetCurrentRecentConnection(currRecentConnection);

				if (checkExecption)
				{
					MessageBox.Show(Helper.GetResourceString(Constants.ERROR_MSG_DEFRAGMENT),
						Helper.GetResourceString(Constants.PRODUCT_CAPTION),
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);
				}
			}
			catch (Exception e)
			{
				LoggingHelper.ShowMessage(e);
			}
		}
	}
}
