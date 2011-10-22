using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace DBSS_Test.BigGrid {
	public partial class SheetView : Control {
		private bool _moving = false;
		public SparseArray DisplayItems { get; set; }
		public Size CellSize { get; set; }
		//public Point Position { get; set; }
		public int px { get; set; }
		public int py { get; set; }

		public int selectionX { get; set; }
		public int selectionY { get; set; }

		public int RangeLeft { get; set; }
		public int RangeTop { get; set; }
		public int RangeRight { get; set; }
		public int RangeBottom { get; set; }

		public SheetView () {
			selectionX = selectionY = 0;
			SetRangeToSelection();
			InitializeComponent();
		}

		public event EventHandler SelectedIndexChanged;

		/// <summary>
		/// Is the currently selected cell inside a range selection?
		/// </summary>
		/// <returns></returns>
		internal bool SelectionInRange () {
			int dx = RangeRight - RangeLeft;
			int dy = RangeBottom - RangeTop;
			if (dx == 0 && dy == 0) return false;

			return (selectionX >= Math.Min(RangeLeft, RangeRight))
				&& (selectionX <= Math.Max(RangeLeft, RangeRight))
				&& (selectionY >= Math.Min(RangeTop, RangeBottom))
				&& (selectionY <= Math.Max(RangeTop, RangeBottom));
		}

		internal void SetRangeToSelection () {
			RangeLeft = RangeRight = selectionX;
			RangeTop = RangeBottom = selectionY;
		}

		/// <summary>
		/// Move selection in default direction, inside range if one is present
		/// </summary>
		internal void ShiftSelection (bool alt_direction) {
			if (!SelectionInRange()) {
				if (alt_direction) selectionY++;
				else selectionX++;
				SetRangeToSelection();
				return;
			} else {
				if (alt_direction) selectionY++;
				else selectionX++;

				if (alt_direction) {
					if (selectionY > Math.Max(RangeTop, RangeBottom)) {
						selectionX++;
						selectionY = Math.Min(RangeTop, RangeBottom);
					}
					if (selectionX > Math.Max(RangeLeft, RangeRight)) {
						selectionX = Math.Min(RangeLeft, RangeRight);
						selectionY = Math.Min(RangeTop, RangeBottom);
					}
				} else {


					if (selectionX > Math.Max(RangeLeft, RangeRight)) {
						selectionY++;
						selectionX = Math.Min(RangeLeft, RangeRight);
					}
					if (selectionY > Math.Max(RangeTop, RangeBottom)) {
						selectionY = Math.Min(RangeTop, RangeBottom);
						selectionX = Math.Min(RangeLeft, RangeRight);
					}


				}
			}

		}

		private Point CellForMouseClick (Point location) {
			if (CellSize.Width == 0 || CellSize.Height == 0) return new Point(-1,-1);

			int rx, ry; // rule-line positions
			int vx, vy; // visible cell extents

			vx = (this.Width / CellSize.Width) + 1;
			vy = (this.Height / CellSize.Height) + 1;

			// Draw content
			for (int y = py; y <= py + vy; y++) {
				ry = ((y - py) * CellSize.Height);
				for (int x = px; x <= px + vx; x++) {
					rx = ((x - px) * CellSize.Width);
					Rectangle cell = new Rectangle(rx, ry, CellSize.Width, CellSize.Height);

					if (cell.Contains(location)) {
						return new Point(x,y);
					}
				}
			}
			return new Point(-1, -1);
		}

		protected override void OnMouseDown (MouseEventArgs e) {
			if (e.Button == MouseButtons.Left) {
				_moving = true; // hack for Vista's dumb windowing
				Point p = CellForMouseClick(e.Location);
				RangeLeft = p.X;
				RangeTop = p.Y;
				RangeRight = p.X;
				RangeBottom = p.Y;
				selectionX = p.X;
				selectionY = p.Y;
				this.Invalidate();
			}
		}

		protected override void OnMouseMove (MouseEventArgs e) {
			if (e.Button == MouseButtons.Left && _moving) {
				Point p = CellForMouseClick(e.Location);

				int dx = p.X - RangeRight;
				int dy = p.Y - RangeBottom;
				if (dx != 0 || dy != 0) {
					RangeRight = p.X;
					RangeBottom = p.Y;
					this.Invalidate();
				}
			}
		}

		protected override void OnMouseUp (MouseEventArgs e) {
			_moving = false; // hack for Vista's dumb windowing
			/*selectionX = Math.Min(RangeLeft, RangeRight);
			selectionY = Math.Min(RangeTop, RangeBottom);*/
			this.Invalidate();
			if (SelectedIndexChanged != null) SelectedIndexChanged(this, new EventArgs());
		}

		protected override void OnPaintBackground (PaintEventArgs pe) {
			if (CellSize.Width == 0 || CellSize.Height == 0) return;
			if (DisplayItems == null) DisplayItems = new SparseArray(2);

			int rx, ry; // rule-line positions
			int vx, vy; // visible cell extents

			vx = (this.Width / CellSize.Width) + 1;
			vy = (this.Height / CellSize.Height) + 1;

			
			BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(pe.Graphics, this.Bounds);
			Graphics g = bg.Graphics;

			Pen p = new Pen(Brushes.Gray, 2);
			Brush odd = Brushes.White;
			Brush even = Brushes.WhiteSmoke;
			Brush sel_even = SystemBrushes.Highlight;
			Brush sel_odd = new SolidBrush(Lighter(SystemColors.Highlight, 20));
			Rectangle cell;

			int s_t = Math.Min(RangeTop, RangeBottom);
			int s_l = Math.Min(RangeLeft, RangeRight);
			int s_b = Math.Max(RangeTop, RangeBottom);
			int s_r = Math.Max(RangeLeft, RangeRight);

			// Draw content
			for (int y = py; y <= py + vy; y++) {
				ry = ((y - py) * CellSize.Height);
				for (int x = px; x <= px + vx; x++) {
					rx = ((x - px) * CellSize.Width);
					cell = new Rectangle(rx, ry, CellSize.Width, CellSize.Height);

					g.Clip = new Region(cell);
					Brush b = ((x + y) % 2 == 0) ? (even) : (odd);

					// if drawn cell is in the selection range, but isn't the currently focused cell
					if (y >= s_t && y <= s_b && x >= s_l && x <= s_r && !(y == selectionY && x == selectionX)) {
						b = ((x + y) % 2 == 0) ? (sel_even) : (sel_odd);
					}
					g.FillRectangle(b, cell);

					object di = DisplayItems[x, y];
					if (di != null)
						g.DrawString(DisplayItems[x,y].ToString(), Font, Brushes.Black, new PointF(rx, ry));
					g.ResetClip();
				}
			}

			// draw primary focus box
			ry = ((selectionY - py) * CellSize.Height);
			rx = ((selectionX - px) * CellSize.Width);
			cell = new Rectangle(rx-1, ry-1, CellSize.Width+2, CellSize.Height+2);
			g.DrawRectangle(p, cell);

			bg.Render();
		}

		private Color Lighter (Color color, int v) {
			int r = color.R + v;
			int g = color.G + v;
			int b = color.B + v;
			r = (r > 255) ? (255) : (r);
			g = (g > 255) ? (255) : (g);
			b = (b > 255) ? (255) : (b);
			return Color.FromArgb(r, g, b);
		}
	}
}
