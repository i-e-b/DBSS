using System;

namespace DBSS.Calculations {
	public class CValue {
		protected string CoreValue;
		protected decimal? numericValue;

		public decimal NumericValue {
			get {
				return numericValue ?? 0;
			}
		}

		/// <summary>
		/// New blank value
		/// </summary>
		public CValue () {
			CoreValue = null;
			numericValue = null;
		}

		/// <summary>
		/// Value from string.
		/// If 'tryNumeric' is true, will try to parse as a number first
		/// </summary>
		public CValue (string value, bool tryNumeric) {
			decimal d;
			if (tryNumeric && decimal.TryParse(value ?? "", out d)) {
				CoreValue = null;
				numericValue = d;
			} else {
				CoreValue = value;
				numericValue = null;
			}
		}

		/// <summary>
		/// value from string. Will stay as a string even if it could be numeric
		/// </summary>
		public CValue (string value) {
			CoreValue = value;
			numericValue = null;
		}

		/// <summary>
		/// value from a decimal
		/// </summary>
		public CValue (decimal value) {
			numericValue = value;
			CoreValue = null;
		}

		public bool IsNumeric {
			get { return numericValue != null; }
		}
		public bool IsString {
			get { return CoreValue != null; }
		}
		public override string ToString () {
			return CoreValue ?? numericValue.ToString();
		}

		/// <summary>
		/// Either numeric add, or string concat
		/// </summary>
		public static CValue operator + (CValue l, CValue r) {
			if (l.IsNumeric && r.IsNumeric) {
				return new CValue(l.NumericValue + r.NumericValue);
			}
			return new CValue("" + l + r);
		}
		/// <summary>
		/// Either numeric subtract, or string removal
		/// </summary>
		public static CValue operator - (CValue l, CValue r) {
			if (l.IsNumeric && r.IsNumeric) {
				return new CValue(l.NumericValue - r.NumericValue);
			}
			return new CValue(l.ToString().Replace(r.ToString(), ""));
		}

		/// <summary>
		/// numeric division, or Nan
		/// </summary>
		public static CValue operator / (CValue l, CValue r) {
			if (l.IsNumeric && r.IsNumeric) {
				return new CValue(l.NumericValue / r.NumericValue);
			}
			return new CValue();
		}

		/// <summary>
		/// numeric multiply, or NaN
		/// </summary>
		public static CValue operator * (CValue l, CValue r) {
			if (l.IsNumeric && r.IsNumeric) {
				return new CValue(l.NumericValue * r.NumericValue);
			}
			return new CValue();
		}

		/// <summary>
		/// numeric modulus, or NaN
		/// </summary>
		public static CValue operator % (CValue l, CValue r) {
			if (l.IsNumeric && r.IsNumeric) {
				return new CValue(l.NumericValue % r.NumericValue);
			}
			return new CValue();
		}

		/// <summary>
		/// Numeric exponent, or NaN
		/// </summary>
		public static CValue Pow (CValue l, CValue r) {
			if (l.IsNumeric && r.IsNumeric) {
				return new CValue((decimal)Math.Pow((double)l.NumericValue, (double)r.NumericValue));
			}
			return new CValue();
		}

		/// <summary>
		/// numeric floor, or lowercase
		/// </summary>
		/// <param name="l"></param>
		/// <returns></returns>
		public static CValue Floor (CValue l) {
			if (l.IsNumeric) {
				return new CValue(Math.Floor(l.NumericValue));
			}
			return new CValue(l.ToString().ToLower());
		}
		public static CValue Ceil (CValue l) {
			if (l.IsNumeric) {
				return new CValue(Math.Ceiling(l.NumericValue));
			}
			return new CValue(l.ToString().ToUpper());
		}
	}
}
