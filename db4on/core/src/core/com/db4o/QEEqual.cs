namespace com.db4o
{
	/// <exclude></exclude>
	public class QEEqual : com.db4o.QEAbstract
	{
		public override void indexBitMap(bool[] bits)
		{
			bits[com.db4o.inside.ix.IxTraverser.EQUAL] = true;
		}
	}
}
