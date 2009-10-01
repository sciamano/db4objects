/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <summary>Class constraint on queries</summary>
	/// <exclude></exclude>
	public class QConClass : QConObject
	{
		[System.NonSerialized]
		private IReflectClass _claxx;

		private string _className;

		private bool i_equal;

		public QConClass()
		{
		}

		internal QConClass(Transaction a_trans, QCon a_parent, QField a_field, IReflectClass
			 claxx) : base(a_trans, a_parent, a_field, null)
		{
			// C/S
			if (claxx != null)
			{
				i_classMetadata = a_trans.Container().ProduceClassMetadata(claxx);
				if (claxx.Equals(a_trans.Container()._handlers.IclassObject))
				{
					i_classMetadata = (ClassMetadata)i_classMetadata.TypeHandler();
				}
			}
			_claxx = claxx;
		}

		internal QConClass(Transaction trans, IReflectClass claxx) : this(trans, null, null
			, claxx)
		{
		}

		public virtual string GetClassName()
		{
			return _claxx.GetName();
		}

		public override bool CanBeIndexLeaf()
		{
			return false;
		}

		internal override bool Evaluate(QCandidate a_candidate)
		{
			bool res = true;
			IReflectClass claxx = a_candidate.ClassReflector();
			if (claxx == null)
			{
				res = false;
			}
			else
			{
				res = i_equal ? _claxx.Equals(claxx) : _claxx.IsAssignableFrom(claxx);
			}
			return i_evaluator.Not(res);
		}

		internal override void EvaluateSelf()
		{
			// optimization for simple class queries: 
			// No instantiation of objects, if not necessary.
			// Does not handle the special comparison of the
			// Compare interface.
			//
			if (i_candidates.WasLoadedFromClassIndex())
			{
				if (i_evaluator.IsDefault())
				{
					if (!HasJoins())
					{
						if (i_classMetadata != null && i_candidates.i_classMetadata != null)
						{
							if (i_classMetadata.GetHigherHierarchy(i_candidates.i_classMetadata) == i_classMetadata)
							{
								return;
							}
						}
					}
				}
			}
			i_candidates.Filter(this);
		}

		public override IConstraint Equal()
		{
			lock (StreamLock())
			{
				i_equal = true;
				return this;
			}
		}

		internal override bool IsNullConstraint()
		{
			return false;
		}

		internal override string LogObject()
		{
			return string.Empty;
		}

		internal override void Marshall()
		{
			base.Marshall();
			if (_claxx != null)
			{
				_className = Container().Config().ResolveAliasRuntimeName(_claxx.GetName());
			}
		}

		public override string ToString()
		{
			string str = "QConClass ";
			if (_claxx != null)
			{
				str += _claxx.ToString() + " ";
			}
			return str + base.ToString();
		}

		internal override void Unmarshall(Transaction a_trans)
		{
			if (i_trans == null)
			{
				base.Unmarshall(a_trans);
				if (_className != null)
				{
					_className = Container().Config().ResolveAliasStoredName(_className);
					_claxx = a_trans.Reflector().ForName(_className);
				}
			}
		}

		internal override void SetEvaluationMode()
		{
			IEnumerator children = IterateChildren();
			while (children.MoveNext())
			{
				object child = children.Current;
				if (child is QConObject)
				{
					((QConObject)child).SetEvaluationMode();
				}
			}
		}

		public override void SetProcessedByIndex()
		{
		}
		// do nothing, QConClass needs to stay in the evaluation graph.
	}
}
