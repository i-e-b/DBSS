using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBSS_Test {
	public class CellReference {
		public int X { get; set; }
		public int Y { get; set; }
		public CellReference (int x, int y) {
			X = x;
			Y = y;
		}
	}
}
