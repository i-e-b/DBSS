using System;
using System.Collections.Generic;
using System.Collections;

namespace DBSS_Test {
	/// <summary>
	/// Function and variable lookup,
	/// uses the DBSS for variables and the lookup function '$(,)'.
	/// Variables are case-insensitive.
	/// </summary>
	/// <remarks>
	/// Special variables:
	///		'R' - Containing cell's row (y co-ord) -- currently handled in Calculator.cs
	///		'C' - Containing cell's column (x co-ord) -- currently handled in Calculator.cs
	///	
	/// Special functions:
	///		'$(x,y)' - value field of cell at co-ords. Empty if no value.
	///		'$f(x,y)' - function of cell as a string. Empty if no formula.
	///		'$n(x,y)' - name of cell as a string. Empty if no name.
	/// </remarks>
	public class Functions {
		private SparseArray _sourceArray;

		public Functions (SparseArray array) {
			_sourceArray = array;
		}

		public List<CellReference> HandleFunction (string token, Stack<CValue> values, int x, int y) {
			List<CellReference> refs = new List<CellReference>();
			if (token.StartsWith("/")) {
				if (!function(token.Substring(1), values, x, y, refs)) throw new Exception("Unrecognised function");
			} else {
				if (!variable(token, values, x, y, refs)) throw new Exception("Unrecognised name or constant");
			}
			return refs;
		}

		// Note: due to the way the tokeniser works, any non-alphanumeric name can only be 1 character long

		private bool variable (string token, Stack<CValue> values, int x, int y, List<CellReference> refs) {
			switch (token.ToLowerInvariant()) {
				case "pi":
				case "π":
					values.Push(new CValue((decimal)Math.PI));
					return true;
				case "e":
					values.Push(new CValue((decimal)Math.E));
					return true;
				case "r":
					values.Push(new CValue((decimal)y));
					return true;
				case "c":
					values.Push(new CValue((decimal)x));
					return true;
			}
			return false;
		}

		private bool function (string token, Stack<CValue> values, int x, int y, List<CellReference> refs) {
			switch (token.ToLowerInvariant()) {
				case "$": {// value at coords
						int rx = (int)values.Pop().NumericValue;
						int ry = (int)values.Pop().NumericValue;
						GridCell g = _sourceArray[rx, ry] as GridCell;

						if (g == null) values.Push(new CValue());
						else values.Push(new CValue(g.Value, true));

						refs.Add(new CellReference(rx, ry));
						return true;
					}
				case "$f": {// function string at coords
						int rx = (int)values.Pop().NumericValue;
						int ry = (int)values.Pop().NumericValue;
						GridCell g = _sourceArray[rx, ry] as GridCell;

						if (g == null) values.Push(new CValue());
						else values.Push(new CValue(g.Formula));

						refs.Add(new CellReference(rx, ry));
						return true;
					}
				case "$n": {// name at coords
						int rx = (int)values.Pop().NumericValue;
						int ry = (int)values.Pop().NumericValue;
						GridCell g = _sourceArray[rx, ry] as GridCell;

						if (g == null) values.Push(new CValue());
						else values.Push(new CValue(g.Name));

						refs.Add(new CellReference(rx, ry));
						return true;
					}
				case "floor":
					values.Push(CValue.Floor(values.Pop()));
					return true;
				case "ceil":
					values.Push(CValue.Ceil(values.Pop()));
					return true;
				case "sqrt":
					values.Push(new CValue((decimal)Math.Sqrt((double)values.Pop().NumericValue)));
					return true;
			}
			return false;
		}
	}
}
