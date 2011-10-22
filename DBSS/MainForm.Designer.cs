namespace DBSS_Test {
	partial class MainForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose (bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent () {
			this.NameBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.FormulaBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.ValueBox = new System.Windows.Forms.TextBox();
			this.bigGrid1 = new DBSS_Test.BigGrid.BigGrid();
			this.SuspendLayout();
			// 
			// NameBox
			// 
			this.NameBox.AcceptsReturn = true;
			this.NameBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.NameBox.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.NameBox.Location = new System.Drawing.Point(47, 1);
			this.NameBox.Name = "NameBox";
			this.NameBox.Size = new System.Drawing.Size(496, 18);
			this.NameBox.TabIndex = 1;
			this.NameBox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.NameBox_PreviewKeyDown);
			this.NameBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Box_KeyPress);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 4);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 13);
			this.label1.TabIndex = 6;
			this.label1.Text = "Name";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(-3, 22);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(44, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Formula";
			// 
			// FormulaBox
			// 
			this.FormulaBox.AcceptsReturn = true;
			this.FormulaBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.FormulaBox.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormulaBox.Location = new System.Drawing.Point(47, 19);
			this.FormulaBox.Name = "FormulaBox";
			this.FormulaBox.Size = new System.Drawing.Size(496, 18);
			this.FormulaBox.TabIndex = 2;
			this.FormulaBox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.FormulaBox_PreviewKeyDown);
			this.FormulaBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Box_KeyPress);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(7, 40);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(34, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Value";
			// 
			// ValueBox
			// 
			this.ValueBox.AcceptsReturn = true;
			this.ValueBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ValueBox.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ValueBox.Location = new System.Drawing.Point(47, 37);
			this.ValueBox.Name = "ValueBox";
			this.ValueBox.Size = new System.Drawing.Size(496, 18);
			this.ValueBox.TabIndex = 0;
			this.ValueBox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.ValueBox_PreviewKeyDown);
			this.ValueBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Box_KeyPress);
			// 
			// bigGrid1
			// 
			this.bigGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.bigGrid1.CausesValidation = false;
			this.bigGrid1.Cursor = System.Windows.Forms.Cursors.Cross;
			this.bigGrid1.Items = null;
			this.bigGrid1.Location = new System.Drawing.Point(0, 56);
			this.bigGrid1.Name = "bigGrid1";
			this.bigGrid1.SelectedItem = null;
			this.bigGrid1.Size = new System.Drawing.Size(543, 363);
			this.bigGrid1.TabIndex = 3;
			this.bigGrid1.TabStop = false;
			this.bigGrid1.SelectedIndexChanged += new System.EventHandler(this.bigGrid1_SelectedIndexChanged);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(543, 417);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.ValueBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.FormulaBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.NameBox);
			this.Controls.Add(this.bigGrid1);
			this.Name = "MainForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "DBSS - Prototyping Prototype";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private DBSS_Test.BigGrid.BigGrid bigGrid1;
		private System.Windows.Forms.TextBox NameBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox FormulaBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox ValueBox;


	}
}

