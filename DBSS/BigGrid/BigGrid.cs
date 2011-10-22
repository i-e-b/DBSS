using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace DBSS_Test.BigGrid {
	public partial class BigGrid : UserControl {
		private SparseArray _items;

		/// <summary>
		/// Sparse array of items to display
		/// </summary>
		public SparseArray Items {
			get { return _items; }
			set {
				if (value == null) return;
				if (value.Rank != 2) throw new Exception("BigGrid only displays rank 2 arrays");
				_items = value;
				sheetView.DisplayItems = _items;
			}
		}

		/// <summary>
		/// Item value currently selected
		/// </summary>
		public object SelectedItem {
			get {
				if (Items == null) return null;
				return Items[sheetView.selectionX, sheetView.selectionY];
			}
			set {
				if (Items != null) {
					if (value != null) {
						Items[sheetView.selectionX, sheetView.selectionY] = value;
					} else {
						Items.RemoveItemByUniqueKey(Items.GetUniqueKey(sheetView.selectionX, sheetView.selectionY));
					}
				}
			}
		}

		public ulong SelectionKey {
			get { return Items.GetUniqueKey(sheetView.selectionX, sheetView.selectionY); }
		}

		public Point SelectionCoords {
			get { return new Point(sheetView.selectionX, sheetView.selectionY); }
		}

		public event EventHandler SelectedIndexChanged;

		public BigGrid () {
			InitializeComponent();
			sheetView.px = sheetView.py = 0;
			sheetView.CellSize = new Size(100, (int)(Font.Height*1.25));
		}

		public void ResetScrollPositions () {
			vScrollBar1.Maximum = Math.Max(Items.GetUpperBound(1), sheetView.selectionY)+10;
			hScrollBar1.Maximum = Math.Max(Items.GetUpperBound(0), sheetView.selectionX)+10;
			vScrollBar1.Invalidate();
			hScrollBar1.Invalidate();
			sheetView.Invalidate();
		}

		private void vScrollBar1_ValueChanged (object sender, EventArgs e) {
			sheetView.py = vScrollBar1.Value;
			sheetView.Invalidate();
		}

		private void hScrollBar1_ValueChanged (object sender, EventArgs e) {
			sheetView.px = hScrollBar1.Value;
			sheetView.Invalidate();
		}

		private void sheetView_SelectedIndexChanged (object sender, EventArgs e) {
			if (SelectedIndexChanged != null) SelectedIndexChanged(this, new EventArgs());
		}

		internal void ShiftSelection () {
			sheetView.ShiftSelection(false);
			sheetView.Invalidate();
			if (SelectedIndexChanged != null) SelectedIndexChanged(this, new EventArgs());
		}

		internal void ShiftSelection (bool p) {
			sheetView.ShiftSelection(p);
			sheetView.Invalidate();
			if (SelectedIndexChanged != null) SelectedIndexChanged(this, new EventArgs());
		}

		/// <summary>
		/// Returns all contents from all cells in the selection range
		/// </summary>
		internal Dictionary<ulong, GridCell> SelectedCells () {
			int t = Math.Min(sheetView.RangeTop, sheetView.RangeBottom);
			int b = Math.Max(sheetView.RangeTop, sheetView.RangeBottom);
			int l = Math.Min(sheetView.RangeLeft, sheetView.RangeRight);
			int r = Math.Max(sheetView.RangeLeft, sheetView.RangeRight);

			var bits = new Dictionary<ulong, GridCell>();

			for (int y = t; y <= b; y++) {
				for (int x = l; x <= r; x++) {
					GridCell c = Items[x, y] as GridCell;
					if (c != null) {
						bits.Add(Items.GetUniqueKey(x, y), c);
					}
				}
			}
			return bits;
		}

		internal void MoveSelection (int x, int y) {
			sheetView.selectionX += x;
			sheetView.selectionY += y;
			if (sheetView.selectionX < 0) sheetView.selectionX = 0;
			if (sheetView.selectionY < 0) sheetView.selectionY = 0;
			sheetView.SetRangeToSelection();
			if (SelectedIndexChanged != null) SelectedIndexChanged(this, new EventArgs());
		}
	}
}
