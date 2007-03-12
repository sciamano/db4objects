namespace com.db4o.@internal.query.processor
{
	/// <summary>Represents an actual object in the database.</summary>
	/// <remarks>
	/// Represents an actual object in the database. Forms a tree structure, indexed
	/// by id. Can have dependents that are doNotInclude'd in the query result when
	/// this is doNotInclude'd.
	/// </remarks>
	/// <exclude></exclude>
	public class QCandidate : com.db4o.@internal.TreeInt, com.db4o.query.Candidate, com.db4o.@internal.query.processor.Orderable
	{
		internal com.db4o.@internal.Buffer _bytes;

		internal readonly com.db4o.@internal.query.processor.QCandidates _candidates;

		private com.db4o.foundation.List4 _dependants;

		internal bool _include = true;

		private object _member;

		internal com.db4o.@internal.query.processor.Orderable _order;

		internal com.db4o.foundation.Tree _pendingJoins;

		private com.db4o.@internal.query.processor.QCandidate _root;

		internal com.db4o.@internal.ClassMetadata _yapClass;

		internal com.db4o.@internal.FieldMetadata _yapField;

		internal com.db4o.@internal.marshall.MarshallerFamily _marshallerFamily;

		private QCandidate(com.db4o.@internal.query.processor.QCandidates qcandidates) : 
			base(0)
		{
			_candidates = qcandidates;
		}

		private QCandidate() : this(null)
		{
		}

		public QCandidate(com.db4o.@internal.query.processor.QCandidates candidates, object
			 obj, int id, bool include) : base(id)
		{
			_candidates = candidates;
			_order = this;
			_member = obj;
			_include = include;
			if (id == 0)
			{
				_key = candidates.GenerateCandidateId();
			}
		}

		public override object ShallowClone()
		{
			com.db4o.@internal.query.processor.QCandidate qcan = new com.db4o.@internal.query.processor.QCandidate
				(_candidates);
			qcan.SetBytes(_bytes);
			qcan._dependants = _dependants;
			qcan._include = _include;
			qcan._member = _member;
			qcan._order = _order;
			qcan._pendingJoins = _pendingJoins;
			qcan._root = _root;
			qcan._yapClass = _yapClass;
			qcan._yapField = _yapField;
			return base.ShallowCloneInternal(qcan);
		}

		internal virtual void AddDependant(com.db4o.@internal.query.processor.QCandidate 
			a_candidate)
		{
			_dependants = new com.db4o.foundation.List4(_dependants, a_candidate);
		}

		private void CheckInstanceOfCompare()
		{
			if (_member is com.db4o.config.Compare)
			{
				_member = ((com.db4o.config.Compare)_member).Compare();
				com.db4o.@internal.LocalObjectContainer stream = GetStream();
				_yapClass = stream.GetYapClass(stream.Reflector().ForObject(_member));
				_key = (int)stream.GetID(_member);
				SetBytes(stream.ReadReaderByID(GetTransaction(), _key));
			}
		}

		public override int Compare(com.db4o.foundation.Tree a_to)
		{
			return _order.CompareTo(((com.db4o.@internal.query.processor.QCandidate)a_to)._order
				);
		}

		public virtual int CompareTo(object a_object)
		{
			if (a_object is com.db4o.@internal.query.processor.Order)
			{
				return -((com.db4o.@internal.query.processor.Order)a_object).CompareTo(this);
			}
			return _key - ((com.db4o.@internal.TreeInt)a_object)._key;
		}

		internal virtual bool CreateChild(com.db4o.@internal.query.processor.QCandidates 
			a_candidates)
		{
			if (!_include)
			{
				return false;
			}
			if (_yapField != null)
			{
				com.db4o.@internal.TypeHandler4 handler = _yapField.GetHandler();
				if (handler != null)
				{
					com.db4o.@internal.Buffer[] arrayBytes = new com.db4o.@internal.Buffer[] { _bytes
						 };
					com.db4o.@internal.TypeHandler4 arrayHandler = handler.ReadArrayHandler(GetTransaction
						(), _marshallerFamily, arrayBytes);
					if (arrayHandler != null)
					{
						int offset = arrayBytes[0]._offset;
						bool outerRes = true;
						System.Collections.IEnumerator i = a_candidates.IterateConstraints();
						while (i.MoveNext())
						{
							com.db4o.@internal.query.processor.QCon qcon = (com.db4o.@internal.query.processor.QCon
								)i.Current;
							com.db4o.@internal.query.processor.QField qf = qcon.GetField();
							if (qf == null || qf.i_name.Equals(_yapField.GetName()))
							{
								com.db4o.@internal.query.processor.QCon tempParent = qcon.i_parent;
								qcon.SetParent(null);
								com.db4o.@internal.query.processor.QCandidates candidates = new com.db4o.@internal.query.processor.QCandidates
									(a_candidates.i_trans, null, qf);
								candidates.AddConstraint(qcon);
								qcon.SetCandidates(candidates);
								arrayHandler.ReadCandidates(_marshallerFamily, arrayBytes[0], candidates);
								arrayBytes[0]._offset = offset;
								bool isNot = qcon.IsNot();
								if (isNot)
								{
									qcon.RemoveNot();
								}
								candidates.Evaluate();
								com.db4o.foundation.Tree.ByRef pending = new com.db4o.foundation.Tree.ByRef();
								bool[] innerRes = new bool[] { isNot };
								candidates.Traverse(new _AnonymousInnerClass172(this, innerRes, isNot, pending));
								if (isNot)
								{
									qcon.Not();
								}
								if (pending.value != null)
								{
									pending.value.Traverse(new _AnonymousInnerClass241(this));
								}
								if (!innerRes[0])
								{
									qcon.Visit(GetRoot(), qcon.Evaluator().Not(false));
									outerRes = false;
								}
								qcon.SetParent(tempParent);
							}
						}
						return outerRes;
					}
					if (handler.GetTypeID() == com.db4o.@internal.Const4.TYPE_SIMPLE)
					{
						a_candidates.i_currentConstraint.Visit(this);
						return true;
					}
				}
			}
			if (_yapField == null || _yapField is com.db4o.@internal.NullFieldMetadata)
			{
				return false;
			}
			_yapClass.FindOffset(_bytes, _yapField);
			com.db4o.@internal.query.processor.QCandidate candidate = ReadSubCandidate(a_candidates
				);
			if (candidate == null)
			{
				return false;
			}
			if (a_candidates.i_yapClass != null && a_candidates.i_yapClass.IsStrongTyped())
			{
				if (_yapField != null)
				{
					com.db4o.@internal.TypeHandler4 handler = _yapField.GetHandler();
					if (handler != null && (handler.GetTypeID() == com.db4o.@internal.Const4.TYPE_CLASS
						))
					{
						com.db4o.@internal.ClassMetadata yc = (com.db4o.@internal.ClassMetadata)handler;
						if (yc is com.db4o.@internal.UntypedFieldHandler)
						{
							yc = candidate.ReadYapClass();
						}
						if (yc == null)
						{
							return false;
						}
						if (!yc.CanHold(a_candidates.i_yapClass.ClassReflector()))
						{
							return false;
						}
					}
				}
			}
			AddDependant(a_candidates.AddByIdentity(candidate));
			return true;
		}

		private sealed class _AnonymousInnerClass172 : com.db4o.foundation.Visitor4
		{
			public _AnonymousInnerClass172(QCandidate _enclosing, bool[] innerRes, bool isNot
				, com.db4o.foundation.Tree.ByRef pending)
			{
				this._enclosing = _enclosing;
				this.innerRes = innerRes;
				this.isNot = isNot;
				this.pending = pending;
			}

			public void Visit(object obj)
			{
				com.db4o.@internal.query.processor.QCandidate cand = (com.db4o.@internal.query.processor.QCandidate
					)obj;
				if (cand.Include())
				{
					innerRes[0] = !isNot;
				}
				if (cand._pendingJoins != null)
				{
					cand._pendingJoins.Traverse(new _AnonymousInnerClass185(this, pending));
				}
			}

			private sealed class _AnonymousInnerClass185 : com.db4o.foundation.Visitor4
			{
				public _AnonymousInnerClass185(_AnonymousInnerClass172 _enclosing, com.db4o.foundation.Tree.ByRef
					 pending)
				{
					this._enclosing = _enclosing;
					this.pending = pending;
				}

				public void Visit(object a_object)
				{
					com.db4o.@internal.query.processor.QPending newPending = (com.db4o.@internal.query.processor.QPending
						)a_object;
					newPending.ChangeConstraint();
					com.db4o.@internal.query.processor.QPending oldPending = (com.db4o.@internal.query.processor.QPending
						)com.db4o.foundation.Tree.Find(pending.value, newPending);
					if (oldPending != null)
					{
						if (oldPending._result != newPending._result)
						{
							oldPending._result = com.db4o.@internal.query.processor.QPending.BOTH;
						}
					}
					else
					{
						pending.value = com.db4o.foundation.Tree.Add(pending.value, newPending);
					}
				}

				private readonly _AnonymousInnerClass172 _enclosing;

				private readonly com.db4o.foundation.Tree.ByRef pending;
			}

			private readonly QCandidate _enclosing;

			private readonly bool[] innerRes;

			private readonly bool isNot;

			private readonly com.db4o.foundation.Tree.ByRef pending;
		}

		private sealed class _AnonymousInnerClass241 : com.db4o.foundation.Visitor4
		{
			public _AnonymousInnerClass241(QCandidate _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				this._enclosing.GetRoot().Evaluate((com.db4o.@internal.query.processor.QPending)a_object
					);
			}

			private readonly QCandidate _enclosing;
		}

		internal virtual void DoNotInclude()
		{
			_include = false;
			if (_dependants != null)
			{
				System.Collections.IEnumerator i = new com.db4o.foundation.Iterator4Impl(_dependants
					);
				_dependants = null;
				while (i.MoveNext())
				{
					((com.db4o.@internal.query.processor.QCandidate)i.Current).DoNotInclude();
				}
			}
		}

		public override bool Duplicates()
		{
			return _order.HasDuplicates();
		}

		internal virtual bool Evaluate(com.db4o.@internal.query.processor.QConObject a_constraint
			, com.db4o.@internal.query.processor.QE a_evaluator)
		{
			if (a_evaluator.Identity())
			{
				return a_evaluator.Evaluate(a_constraint, this, null);
			}
			if (_member == null)
			{
				_member = Value();
			}
			return a_evaluator.Evaluate(a_constraint, this, a_constraint.Translate(_member));
		}

		internal virtual bool Evaluate(com.db4o.@internal.query.processor.QPending a_pending
			)
		{
			com.db4o.@internal.query.processor.QPending oldPending = (com.db4o.@internal.query.processor.QPending
				)com.db4o.foundation.Tree.Find(_pendingJoins, a_pending);
			if (oldPending == null)
			{
				a_pending.ChangeConstraint();
				_pendingJoins = com.db4o.foundation.Tree.Add(_pendingJoins, a_pending);
				return true;
			}
			_pendingJoins = _pendingJoins.RemoveNode(oldPending);
			oldPending._join.EvaluatePending(this, oldPending, a_pending._result);
			return false;
		}

		internal virtual com.db4o.reflect.ReflectClass ClassReflector()
		{
			ReadYapClass();
			if (_yapClass == null)
			{
				return null;
			}
			return _yapClass.ClassReflector();
		}

		internal virtual bool FieldIsAvailable()
		{
			return ClassReflector() != null;
		}

		public virtual com.db4o.ObjectContainer ObjectContainer()
		{
			return GetStream();
		}

		public virtual object GetObject()
		{
			object obj = Value(true);
			if (obj is com.db4o.@internal.Buffer)
			{
				com.db4o.@internal.Buffer reader = (com.db4o.@internal.Buffer)obj;
				int offset = reader._offset;
				obj = _marshallerFamily._string.ReadFromOwnSlot(GetStream(), reader);
				reader._offset = offset;
			}
			return obj;
		}

		internal virtual com.db4o.@internal.query.processor.QCandidate GetRoot()
		{
			return _root == null ? this : _root;
		}

		private com.db4o.@internal.LocalObjectContainer GetStream()
		{
			return GetTransaction().i_file;
		}

		private com.db4o.@internal.Transaction GetTransaction()
		{
			return _candidates.i_trans;
		}

		public virtual bool HasDuplicates()
		{
			return _root != null;
		}

		public virtual void HintOrder(int a_order, bool a_major)
		{
			if (_order == this)
			{
				_order = new com.db4o.@internal.query.processor.Order();
			}
			_order.HintOrder(a_order, a_major);
		}

		public virtual bool Include()
		{
			return _include;
		}

		/// <summary>For external interface use only.</summary>
		/// <remarks>
		/// For external interface use only. Call doNotInclude() internally so
		/// dependancies can be checked.
		/// </remarks>
		public virtual void Include(bool flag)
		{
			_include = flag;
		}

		public override void OnAttemptToAddDuplicate(com.db4o.foundation.Tree a_tree)
		{
			_size = 0;
			_root = (com.db4o.@internal.query.processor.QCandidate)a_tree;
		}

		private com.db4o.reflect.ReflectClass MemberClass()
		{
			return GetTransaction().Reflector().ForObject(_member);
		}

		internal virtual com.db4o.@internal.Comparable4 PrepareComparison(com.db4o.@internal.ObjectContainerBase
			 a_stream, object a_constraint)
		{
			if (_yapField != null)
			{
				return _yapField.PrepareComparison(a_constraint);
			}
			if (_yapClass == null)
			{
				com.db4o.@internal.ClassMetadata yc = null;
				if (_bytes != null)
				{
					yc = a_stream.ProduceYapClass(a_stream.Reflector().ForObject(a_constraint));
				}
				else
				{
					if (_member != null)
					{
						yc = a_stream.GetYapClass(a_stream.Reflector().ForObject(_member));
					}
				}
				if (yc != null)
				{
					if (_member != null && j4o.lang.JavaSystem.GetClassForObject(_member).IsArray())
					{
						com.db4o.@internal.TypeHandler4 ydt = (com.db4o.@internal.TypeHandler4)yc.PrepareComparison
							(a_constraint);
						if (a_stream.Reflector().Array().IsNDimensional(MemberClass()))
						{
							com.db4o.@internal.handlers.MultidimensionalArrayHandler yan = new com.db4o.@internal.handlers.MultidimensionalArrayHandler
								(a_stream, ydt, false);
							return yan;
						}
						com.db4o.@internal.handlers.ArrayHandler ya = new com.db4o.@internal.handlers.ArrayHandler
							(a_stream, ydt, false);
						return ya;
					}
					return yc.PrepareComparison(a_constraint);
				}
				return null;
			}
			return _yapClass.PrepareComparison(a_constraint);
		}

		private void Read()
		{
			if (_include)
			{
				if (_bytes == null)
				{
					if (_key > 0)
					{
						SetBytes(GetStream().ReadReaderByID(GetTransaction(), _key));
						if (_bytes == null)
						{
							_include = false;
						}
					}
					else
					{
						_include = false;
					}
				}
			}
		}

		private com.db4o.@internal.query.processor.QCandidate ReadSubCandidate(com.db4o.@internal.query.processor.QCandidates
			 candidateCollection)
		{
			Read();
			if (_bytes != null)
			{
				com.db4o.@internal.query.processor.QCandidate subCandidate = null;
				int offset = _bytes._offset;
				try
				{
					subCandidate = _yapField.GetHandler().ReadSubCandidate(_marshallerFamily, _bytes, 
						candidateCollection, false);
				}
				catch
				{
					return null;
				}
				_bytes._offset = offset;
				if (subCandidate != null)
				{
					subCandidate._root = GetRoot();
					return subCandidate;
				}
			}
			return null;
		}

		private void ReadThis(bool a_activate)
		{
			Read();
			com.db4o.@internal.Transaction trans = GetTransaction();
			if (trans != null)
			{
				_member = trans.Stream().GetByID1(trans, _key);
				if (_member != null && (a_activate || _member is com.db4o.config.Compare))
				{
					trans.Stream().Activate1(trans, _member);
					CheckInstanceOfCompare();
				}
			}
		}

		internal virtual com.db4o.@internal.ClassMetadata ReadYapClass()
		{
			if (_yapClass == null)
			{
				Read();
				if (_bytes != null)
				{
					_bytes._offset = 0;
					com.db4o.@internal.ObjectContainerBase stream = GetStream();
					com.db4o.@internal.marshall.ObjectHeader objectHeader = new com.db4o.@internal.marshall.ObjectHeader
						(stream, _bytes);
					_yapClass = objectHeader.YapClass();
					if (_yapClass != null)
					{
						if (stream.i_handlers.ICLASS_COMPARE.IsAssignableFrom(_yapClass.ClassReflector())
							)
						{
							ReadThis(false);
						}
					}
				}
			}
			return _yapClass;
		}

		public override string ToString()
		{
			return base.ToString();
			string str = "QCandidate ";
			if (_yapClass != null)
			{
				str += "\n   YapClass " + _yapClass.GetName();
			}
			if (_yapField != null)
			{
				str += "\n   YapField " + _yapField.GetName();
			}
			if (_member != null)
			{
				str += "\n   Member " + _member.ToString();
			}
			if (_root != null)
			{
				str += "\n  rooted by:\n";
				str += _root.ToString();
			}
			else
			{
				str += "\n  ROOT";
			}
			return str;
		}

		internal virtual void UseField(com.db4o.@internal.query.processor.QField a_field)
		{
			Read();
			if (_bytes == null)
			{
				_yapField = null;
				return;
			}
			ReadYapClass();
			_member = null;
			if (a_field == null)
			{
				_yapField = null;
				return;
			}
			if (_yapClass == null)
			{
				_yapField = null;
				return;
			}
			_yapField = a_field.GetYapField(_yapClass);
			_marshallerFamily = _yapClass.FindOffset(_bytes, _yapField);
			if (_yapField == null || _marshallerFamily == null)
			{
				if (_yapClass.HoldsAnyClass())
				{
					_yapField = null;
				}
				else
				{
					_yapField = new com.db4o.@internal.NullFieldMetadata();
				}
			}
		}

		internal virtual object Value()
		{
			return Value(false);
		}

		internal virtual object Value(bool a_activate)
		{
			if (_member == null)
			{
				if (_yapField == null)
				{
					ReadThis(a_activate);
				}
				else
				{
					int offset = _bytes._offset;
					try
					{
						_member = _yapField.ReadQuery(GetTransaction(), _marshallerFamily, _bytes);
					}
					catch (com.db4o.CorruptionException)
					{
						_member = null;
					}
					_bytes._offset = offset;
					CheckInstanceOfCompare();
				}
			}
			return _member;
		}

		internal virtual void SetBytes(com.db4o.@internal.Buffer bytes)
		{
			_bytes = bytes;
		}
	}
}
