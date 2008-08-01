/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Sharpen;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public abstract class ClassMarshaller
	{
		public MarshallerFamily _family;

		public virtual RawClassSpec ReadSpec(Transaction trans, ByteArrayBuffer reader)
		{
			byte[] nameBytes = ReadName(trans, reader);
			string className = trans.Container().StringIO().Read(nameBytes);
			ReadMetaClassID(reader);
			// skip
			int ancestorID = reader.ReadInt();
			reader.IncrementOffset(Const4.IntLength);
			// index ID
			int numFields = reader.ReadInt();
			return new RawClassSpec(className, ancestorID, numFields);
		}

		public virtual void Write(Transaction trans, ClassMetadata clazz, ByteArrayBuffer
			 writer)
		{
			writer.WriteShortString(trans, clazz.NameToWrite());
			int intFormerlyKnownAsMetaClassID = 0;
			writer.WriteInt(intFormerlyKnownAsMetaClassID);
			writer.WriteIDOf(trans, clazz.i_ancestor);
			WriteIndex(trans, clazz, writer);
			writer.WriteInt(clazz.DeclaredAspectCount());
			clazz.ForEachDeclaredAspect(new _IProcedure4_38(this, trans, clazz, writer));
		}

		private sealed class _IProcedure4_38 : IProcedure4
		{
			public _IProcedure4_38(ClassMarshaller _enclosing, Transaction trans, ClassMetadata
				 clazz, ByteArrayBuffer writer)
			{
				this._enclosing = _enclosing;
				this.trans = trans;
				this.clazz = clazz;
				this.writer = writer;
			}

			public void Apply(object arg)
			{
				this._enclosing._family._field.Write(trans, clazz, (ClassAspect)arg, writer);
			}

			private readonly ClassMarshaller _enclosing;

			private readonly Transaction trans;

			private readonly ClassMetadata clazz;

			private readonly ByteArrayBuffer writer;
		}

		protected virtual void WriteIndex(Transaction trans, ClassMetadata clazz, ByteArrayBuffer
			 writer)
		{
			int indexID = clazz.Index().Write(trans);
			writer.WriteInt(IndexIDForWriting(indexID));
		}

		protected abstract int IndexIDForWriting(int indexID);

		public virtual byte[] ReadName(Transaction trans, ByteArrayBuffer reader)
		{
			byte[] name = ReadName(trans.Container().StringIO(), reader);
			return name;
		}

		public int ReadMetaClassID(ByteArrayBuffer reader)
		{
			return reader.ReadInt();
		}

		private byte[] ReadName(LatinStringIO sio, ByteArrayBuffer reader)
		{
			int len = reader.ReadInt();
			len = len * sio.BytesPerChar();
			byte[] nameBytes = new byte[len];
			System.Array.Copy(reader._buffer, reader._offset, nameBytes, 0, len);
			nameBytes = Platform4.UpdateClassName(nameBytes);
			reader.IncrementOffset(len);
			return nameBytes;
		}

		public void Read(ObjectContainerBase stream, ClassMetadata clazz, ByteArrayBuffer
			 reader)
		{
			clazz.SetAncestor(stream.ClassMetadataForId(reader.ReadInt()));
			if (clazz.CallConstructor())
			{
				// The logic further down checks the ancestor YapClass, whether
				// or not it is allowed, not to call constructors. The ancestor
				// YapClass may possibly have not been loaded yet.
				clazz.CreateConstructor(stream, clazz.ClassReflector(), clazz.GetName(), true);
			}
			clazz.CheckType();
			ReadIndex(stream, clazz, reader);
			clazz._aspects = CreateFields(clazz, reader.ReadInt());
			ReadFields(stream, reader, clazz._aspects);
		}

		protected abstract void ReadIndex(ObjectContainerBase stream, ClassMetadata clazz
			, ByteArrayBuffer reader);

		private ClassAspect[] CreateFields(ClassMetadata clazz, int fieldCount)
		{
			ClassAspect[] aspects = new ClassAspect[fieldCount];
			for (int i = 0; i < aspects.Length; i++)
			{
				aspects[i] = new FieldMetadata(clazz);
				aspects[i].SetHandle(i);
			}
			return aspects;
		}

		private void ReadFields(ObjectContainerBase stream, ByteArrayBuffer reader, ClassAspect
			[] fields)
		{
			for (int i = 0; i < fields.Length; i++)
			{
				fields[i] = _family._field.Read(stream, (FieldMetadata)fields[i], reader);
			}
		}

		public virtual int MarshalledLength(ObjectContainerBase stream, ClassMetadata clazz
			)
		{
			IntByRef len = new IntByRef(stream.StringIO().ShortLength(clazz.NameToWrite()) + 
				Const4.ObjectLength + (Const4.IntLength * 2) + (Const4.IdLength));
			len.value += clazz.Index().OwnLength();
			clazz.ForEachDeclaredAspect(new _IProcedure4_118(this, len, stream));
			return len.value;
		}

		private sealed class _IProcedure4_118 : IProcedure4
		{
			public _IProcedure4_118(ClassMarshaller _enclosing, IntByRef len, ObjectContainerBase
				 stream)
			{
				this._enclosing = _enclosing;
				this.len = len;
				this.stream = stream;
			}

			public void Apply(object arg)
			{
				len.value += this._enclosing._family._field.MarshalledLength(stream, (ClassAspect
					)arg);
			}

			private readonly ClassMarshaller _enclosing;

			private readonly IntByRef len;

			private readonly ObjectContainerBase stream;
		}

		public virtual void Defrag(ClassMetadata classMetadata, LatinStringIO sio, DefragmentContextImpl
			 context, int classIndexID)
		{
			ReadName(sio, context.SourceBuffer());
			ReadName(sio, context.TargetBuffer());
			int metaClassID = 0;
			context.WriteInt(metaClassID);
			// ancestor ID
			context.CopyID();
			context.WriteInt(IndexIDForWriting(classIndexID));
			// field length
			int numFields = context.ReadInt();
			if (numFields > classMetadata.DeclaredAspectCount())
			{
				throw new InvalidOperationException();
			}
			classMetadata.ForEachDeclaredAspect(new _IProcedure4_144(this, classMetadata, sio
				, context));
		}

		private sealed class _IProcedure4_144 : IProcedure4
		{
			public _IProcedure4_144(ClassMarshaller _enclosing, ClassMetadata classMetadata, 
				LatinStringIO sio, DefragmentContextImpl context)
			{
				this._enclosing = _enclosing;
				this.classMetadata = classMetadata;
				this.sio = sio;
				this.context = context;
			}

			public void Apply(object arg)
			{
				ClassAspect aspect = (ClassAspect)arg;
				this._enclosing._family._field.Defrag(classMetadata, aspect, sio, context);
			}

			private readonly ClassMarshaller _enclosing;

			private readonly ClassMetadata classMetadata;

			private readonly LatinStringIO sio;

			private readonly DefragmentContextImpl context;
		}
	}
}
