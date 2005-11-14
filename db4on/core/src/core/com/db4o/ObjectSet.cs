namespace com.db4o
{
	/// <summary>query resultset.</summary>
	/// <remarks>
	/// query resultset.
	/// <br /><br />An ObjectSet is a representation for a set of objects returned
	/// by a query.
	/// <br /><br />ObjectSet extends the system collection interfaces
	/// java.util.List/System.Collections.IList where they are available. It is
	/// recommended, never to reference ObjectSet directly in code but to use
	/// List / IList instead.
	/// <br /><br />Note that the underlying
	/// <see cref="com.db4o.ObjectContainer">ObjectContainer</see>
	/// of an ObjectSet
	/// needs to remain open as long as an ObjectSet is used. This is necessary
	/// for lazy instantiation. The objects in an ObjectSet are only instantiated
	/// when they are actually being used by the application.
	/// </remarks>
	/// <seealso cref="com.db4o.ext.ExtObjectSet">for extended functionality.</seealso>
	public interface ObjectSet : System.Collections.IList
	{
		/// <summary>returns an ObjectSet with extended functionality.</summary>
		/// <remarks>
		/// returns an ObjectSet with extended functionality.
		/// <br /><br />Every ObjectSet that db4o provides can be casted to
		/// an ExtObjectSet. This method is supplied for your convenience
		/// to work without a cast.
		/// <br /><br />The ObjectSet functionality is split to two interfaces
		/// to allow newcomers to focus on the essential methods.
		/// </remarks>
		com.db4o.ext.ExtObjectSet ext();

		/// <summary>returns <code>true</code> if the <code>ObjectSet</code> has more elements.
		/// 	</summary>
		/// <remarks>returns <code>true</code> if the <code>ObjectSet</code> has more elements.
		/// 	</remarks>
		/// <returns>
		/// boolean - <code>true</code> if the <code>ObjectSet</code> has more
		/// elements.
		/// </returns>
		bool hasNext();

		/// <summary>returns the next object in the <code>ObjectSet</code>.</summary>
		/// <remarks>
		/// returns the next object in the <code>ObjectSet</code>.
		/// <br /><br />
		/// Before returning the Object, next() triggers automatic activation of the
		/// Object with the respective
		/// <see cref="com.db4o.config.Configuration.activationDepth">global</see>
		/// or
		/// <see cref="com.db4o.config.ObjectClass.maximumActivationDepth">class specific</see>
		/// setting.<br /><br />
		/// </remarks>
		/// <returns>the next object in the <code>ObjectSet</code>.</returns>
		object next();

		/// <summary>resets the <code>ObjectSet</code> cursor before the first element.</summary>
		/// <remarks>
		/// resets the <code>ObjectSet</code> cursor before the first element.
		/// <br /><br />A subsequent call to <code>next()</code> will return the first element.
		/// </remarks>
		void reset();

		/// <summary>returns the number of elements in the <code>ObjectSet</code>.</summary>
		/// <remarks>returns the number of elements in the <code>ObjectSet</code>.</remarks>
		/// <returns>the number of elements in the <code>ObjectSet</code>.</returns>
		int size();
	}
}
