using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DBSS_Test {
	public partial class MainForm : Form {
		private readonly Calculator calculator;

		public MainForm () {
			InitializeComponent();

			bigGrid1.Items = new SparseArray(2);
			LoadFromDB();
			bigGrid1.ResetScrollPositions();
			calculator = new Calculator(bigGrid1.Items);
		}

		private void bigGrid1_SelectedIndexChanged (object sender, EventArgs e) {
			var gc = bigGrid1.SelectedItem as GridCell;

			if (gc == null) {
				ValueBox.Text = "";
				FormulaBox.Text = "";
				NameBox.Text = "";
				return;
			}

			ValueBox.Text = gc.Value;
			FormulaBox.Text = gc.Formula;
			NameBox.Text = gc.Name;

			// select all text. Focus should stay put, so not a big worry.
			ValueBox.SelectAll();
			FormulaBox.SelectAll();
			NameBox.SelectAll();
		}

		protected void LoadFromDB () {
			var adapter = new SheetDataTableAdapters.SheetAdapter();
			var cache = new SheetDataTableAdapters.CachesAdapter();

			bigGrid1.Items.Clear();
			var table = adapter.GetSheet();
			foreach (var cell in table) {
				var gc = new GridCell {Formula = cell.Formula, Name = cell.Name};
				int x = cell.x;
				int y = cell.y;

				var ctab = cache.GetCacheByHash((long)bigGrid1.Items.GetUniqueKey(x, y));
				if (ctab.Count > 0) {
					gc.Value = ctab[0].Value;
					gc.RPN = ctab[0].RPN;
				}

				bigGrid1.Items[x, y] = gc;
			}
		}

		/// <summary>
		/// Update in-memory grid and database for selected cell
		/// </summary>
		protected void UpdateSelection (GridCell gc) {
			Point c = bigGrid1.SelectionCoords;
			UpdateCell(gc, c.X, c.Y);
		}

		/// <summary>
		/// Update in-memory grid and database
		/// </summary>
		protected void UpdateCell (GridCell gc, int x, int y) {
			bigGrid1.Items[x,y] = (gc.IsEmpty()) ? (null) : (gc);

			ulong key = bigGrid1.Items.GetUniqueKey(x, y);

			// Send changes to DB
			var adapter = new SheetDataTableAdapters.SheetAdapter();
			var cache = new SheetDataTableAdapters.CachesAdapter();
			if (gc.IsEmpty()) {
				adapter.DeleteCellByHash((long)key);
				cache.DeleteCacheByHash((long)key);
			} else {
				adapter.SetCellByHash((long)key, x, y, gc.Name, gc.Formula);
				cache.SetCacheByHash((long)key, gc.RPN, gc.Value);
			}
		}

		private void CheckSpecialKeys (PreviewKeyDownEventArgs e) {
			switch (e.KeyCode) {
				case Keys.Delete:
					if (e.Shift) {
						foreach (ulong key in bigGrid1.SelectedCells().Keys) {
							int[] coord = bigGrid1.Items.HashToIndex(key);
							UpdateCell(new GridCell(), coord[0], coord[1]);
							try {
								RefreshCell(coord[0], coord[1], true);
							} catch { }
						}
					}
					bigGrid1.ResetScrollPositions();
					break;
				case Keys.Left:
					if (e.Control) bigGrid1.MoveSelection(-1, 0);
					bigGrid1.ResetScrollPositions();
					break;
				case Keys.Right:
					if (e.Control) bigGrid1.MoveSelection(1, 0);
					bigGrid1.ResetScrollPositions();
					break;
				case Keys.Up:
					if (e.Control) bigGrid1.MoveSelection(0, -1);
					bigGrid1.ResetScrollPositions();
					break;
				case Keys.Down:
					if (e.Control) bigGrid1.MoveSelection(0, 1);
					bigGrid1.ResetScrollPositions();
					break;
			}
		}

		private void NameBox_PreviewKeyDown (object sender, PreviewKeyDownEventArgs e) {
			CheckSpecialKeys(e);
			if (e.KeyCode != Keys.Return && e.KeyCode != Keys.Enter) return;

			GridCell gc = bigGrid1.SelectedItem as GridCell;
			if (gc == null) gc = new GridCell();
			gc.Name = NameBox.Text;

			// TODO: validate, updates
			UpdateSelection(gc);

			// UI Stuff
			bigGrid1.ShiftSelection(e.Shift);
			bigGrid1.ResetScrollPositions();
		}

		private void FormulaBox_PreviewKeyDown (object sender, PreviewKeyDownEventArgs e) {
			CheckSpecialKeys(e);
			if (e.KeyCode != Keys.Return && e.KeyCode != Keys.Enter) return;

			GridCell gc = bigGrid1.SelectedItem as GridCell;
			if (gc == null) gc = new GridCell();
			gc.Formula = FormulaBox.Text;
			UpdateSelection(gc);
			RefreshCell(bigGrid1.SelectionCoords.X, bigGrid1.SelectionCoords.Y, true);

			// UI stuff
			bigGrid1.ShiftSelection(e.Shift);
			bigGrid1.ResetScrollPositions();
		}

		private void ValueBox_PreviewKeyDown (object sender, PreviewKeyDownEventArgs e) {
			CheckSpecialKeys(e);
			if (e.KeyCode != Keys.Return && e.KeyCode != Keys.Enter) return;

			GridCell gc = bigGrid1.SelectedItem as GridCell;
			if (gc == null) gc = new GridCell();
			gc.Value = ValueBox.Text;

			// TODO: validation & updates
			UpdateSelection(gc);
			RecalculateFromCell(bigGrid1.SelectionCoords.X, bigGrid1.SelectionCoords.Y);

			// UI stuff
			bigGrid1.ShiftSelection(e.Shift);
			bigGrid1.ResetScrollPositions();
		}

		/// <summary>
		/// Recalculate a cell's value from it's formula
		/// </summary>
		private ResultSet RefreshCell (int x, int y, bool formulaChanged) {
			GridCell gc = bigGrid1.Items[x, y] as GridCell;
			if (gc == null) gc = new GridCell();
			ResultSet rs = new ResultSet();
			rs.Result = gc.Value;

			if (formulaChanged) {// Clear out old refs and add new ones:
				rs = RecalculateFromCell(x, y);

				var data = new SheetDataTableAdapters.DependsAdapter();
				ulong key = bigGrid1.Items.GetUniqueKey(x, y);
				data.ClearDependenciesForHash((long)key);
				foreach (var refr in rs.References) {
					data.AddDependency((long)key, (long)bigGrid1.Items.GetUniqueKey(refr.X, refr.Y));
				}

				UpdateCell(gc, x, y);
				return rs;
			}

			Stack<string> rpn = null;
			try {
				rpn = calculator.InfixToPostfix(gc.Formula ?? "");
			} catch (Exception ex) {
				gc.Value = "Formula Error: " + ex.Message;
			}
			if (rpn.Count > 0) { // formula is non-empty
				try {
					rs = calculator.EvaluatePostfix(rpn, y, x);
					gc.Value = rs.Result;
				} catch (Exception ex) {
					gc.Value = "Calculation Error: " + ex.Message;
				}
			}
			UpdateCell(gc,x,y);
			return rs;
		}

		/// <summary>
		/// Recalculate dependent cell's values
		/// </summary>
		private ResultSet RecalculateFromCell (int x, int y) {
			// TODO: self-reference protection!
			ulong key = bigGrid1.Items.GetUniqueKey(x, y);

			// First, get our own values:
			ResultSet rs = RefreshCell(x, y, false);

			var adapter = new SheetDataTableAdapters.DependsAdapter();
			foreach (var dep_row in adapter.GetDependsOf((long)key)) {
				if (dep_row.Depends != ((long)key)) throw new Exception("Nasty mismatch in dependency table");
				int[] coords = bigGrid1.Items.HashToIndex((ulong)dep_row.Hash);
				RecalculateFromCell(coords[0], coords[1]);
			}

			return rs;
		}

		private void Box_KeyPress (object sender, KeyPressEventArgs e) {
			if (e == null) return;
			if (e.KeyChar == '\n' || e.KeyChar == '\r') {
				e.Handled = true;
			}
		}
	}
}
