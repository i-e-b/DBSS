using System;
using System.Collections;

namespace System.Collections {
	/// <summary>
	/// Sparse Array acts as an n-dimensional array,
	/// but uses memory where needed.
	/// Useful for very large arrays which don't have a large proportion
	/// of cells occupied.
	/// </summary>
	/// <remarks>
	/// Use like:
	///		var Arry = new SparseArray(2);
	///		Arry[3,6] = myValue;
	///		var otherValue = Arry[10, 200];
	/// </remarks>
	public class SparseArray : IList {
		protected int dimensions = 1;
		protected Hashtable hashtable;
		protected int[] lowerBounds, upperBounds;

		public SparseArray () {
			hashtable = new Hashtable();
			lowerBounds = new int[dimensions];
			upperBounds = new int[dimensions];
		}

		public SparseArray (int dimensions) {
			this.dimensions = dimensions;
			hashtable = new Hashtable();
			lowerBounds = new int[dimensions];
			upperBounds = new int[dimensions];
		}

		protected ulong IndexToHash (int[] indices) {
			if (indices == null | indices.Length < 1)
				throw new ArgumentException("Null indices passed to hash function");
			if (indices.Length != dimensions)
				throw new ArgumentException("The number of indices must match the number of dimensions");

			ulong a = 0;
			int p = 0;
			int dim = indices.Length;
			unchecked {
				for (long j = 1; j < long.MaxValue && j > 0; j <<= 1) {
					for (int idx = 0; idx < dim; idx++) {
						a |= (ulong)(j & indices[idx]) << (p); //compare this bit, and add it into the correct position in 'a'
						p++; // increment position offset
					}
					p--; // 'j' will now shift left one, so adjust position offset.
				}
			}
			return a;
		}

		public int[] HashToIndex (ulong hash) {
			int[] res = new int[dimensions];
			int i = -1, r = 0;
			for (ulong j = 1; j < ulong.MaxValue && j > 0; j <<= 1) {
				i = (i + 1) % dimensions;
				r += (i == 0) ? (0) : (1);
				res[i] |= (int)((hash & j) >> r);
			}
			return res;
		}

		public bool IsFixedSize { get { return false; } }
		public bool IsReadOnly { get { return false; } }
		public bool IsSynchronized { get { return false; } }
		public int Count { get { return hashtable.Count; } }
		public int Rank { get { return dimensions; } }
		public object SyncRoot { get { return null; } }

		public void CopyTo (Array array, int index) {
			throw new NotImplementedException();
		}

		public int GetLowerBound (int dimension) {
			if (dimension > dimensions)
				throw new ArgumentOutOfRangeException("dimension");
			return lowerBounds[dimension];
		}

		public int GetUpperBound (int dimension) {
			if (dimension > dimensions)
				throw new ArgumentOutOfRangeException("dimension");
			return upperBounds[dimension];
		}

		public object GetValue (int[] indices) {
			ulong key = IndexToHash(indices);
			if (hashtable.Contains(key))
				return hashtable[key];
			return null;
		}

		public object GetValue (int index) {
			return GetValue(new int[] { index });
		}

		public object GetValue (int index1, int index2) {
			return GetValue(new int[] { index1, index2 });
		}

		public void RemoveItemByUniqueKey (ulong key) {
			hashtable.Remove(key);
		}

		public ulong GetUniqueKey (int[] indices) {
			return IndexToHash(indices);
		}

		public ulong GetUniqueKey (int index) {
			return IndexToHash(new int[] { index });
		}

		public ulong GetUniqueKey (int index1, int index2) {
			return IndexToHash(new int[] { index1, index2 });
		}


		public void SetValue (object value, int[] indices) {
			ulong hash = IndexToHash(indices);
			try {
				hashtable.Add(hash, value);
			} catch {
				hashtable[hash] = value;
			}
			for (int i = 0; i < dimensions; i++) {
				if (lowerBounds[i] > indices[i])
					lowerBounds[i] = indices[i];
				if (upperBounds[i] < indices[i])
					upperBounds[i] = indices[i];
			}
		}

		public void SetValue (object value, int index) {
			SetValue(value, new int[] { index });
		}

		public void SetValue (object value, int index1, int index2) {
			SetValue(value, new int[] { index1, index2 });
		}

		private class SparseArrayEnumerator : IEnumerator {
			private IDictionaryEnumerator dict;
			private SortedList sl;
			private SparseArray parent;

			public SparseArrayEnumerator (SparseArray array) {
				parent = array;
				sl = new SortedList(array.hashtable);
				dict = sl.GetEnumerator();
			}

			public void Reset () { dict.Reset(); }
			public bool MoveNext () { return dict.MoveNext(); }
			public object Current { get { return dict.Value; } }
			public int[] Index { get { return parent.HashToIndex((ulong)dict.Key); } }
		}

		public System.Collections.IEnumerator GetEnumerator () {
			return new SparseArrayEnumerator(this);
		}

		public void RemoveAt (int index) {
			throw new NotImplementedException();
		}

		public void Insert (int index, object value) {
			throw new NotImplementedException();
		}

		public void Remove (object value) {
			throw new NotImplementedException();
		}

		public bool Contains (object value) {
			return hashtable.ContainsValue(value);
		}

		public void Clear () {
			hashtable.Clear();
		}

		public int IndexOf (object value) {
			if (dimensions != 1)
				throw new RankException();
			return 0;
		}

		public int Add (object value) {
			throw new NotImplementedException();
		}

		public object this[int[] indicies] {
			get { return GetValue(indicies); }
			set { SetValue(value, indicies); }
		}

		public object this[int index] {
			get { return GetValue(index); }
			set { SetValue(value, index); }
		}

		public object this[int index1, int index2] {
			get { return GetValue(index1, index2); }
			set { SetValue(value, index1, index2); }
		}

	}
}