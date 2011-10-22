using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBSS_Test {
	public class GridCell {
		public string Name { get; set; }
		public string Formula { get; set; }
		public string RPN { get; set; }
		public string Value { get; set; }

		public override string ToString () {
			if (!String.IsNullOrEmpty(Name)) return Name;
			if (!String.IsNullOrEmpty(Value)) return Value;
			if (!String.IsNullOrEmpty(Formula)) return Formula;

			return null;
		}

		public bool IsEmpty () {
			return String.IsNullOrEmpty(Name) && String.IsNullOrEmpty(Value) && String.IsNullOrEmpty(Formula);
		}
	}
}
