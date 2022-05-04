using System.ComponentModel;
using System.Windows.Forms;

namespace STSC_TextToGeometry_v19

{
    partial class FrmTxt2GeomUi
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;



        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTxt2Convert = new System.Windows.Forms.Label();
            this.txtBxText = new System.Windows.Forms.TextBox();
            this.grpBoxFont = new System.Windows.Forms.GroupBox();
            this.ComboFontCollection = new System.Windows.Forms.ComboBox();
            this.chkBxItalic = new System.Windows.Forms.CheckBox();
            this.chkBxBold = new System.Windows.Forms.CheckBox();
            this.lblFontName = new System.Windows.Forms.Label();
            this.grpBoxHeight = new System.Windows.Forms.GroupBox();
            this.chkBxAsReg = new System.Windows.Forms.CheckBox();
            this.txtBoxHeight = new System.Windows.Forms.TextBox();
            this.lblHeight = new System.Windows.Forms.Label();
            this.btnPlaceGeom = new System.Windows.Forms.Button();
            this.lblHelpTxt = new System.Windows.Forms.Label();
            this.grpBoxFont.SuspendLayout();
            this.grpBoxHeight.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTxt2Convert
            // 
            this.lblTxt2Convert.AutoSize = true;
            this.lblTxt2Convert.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTxt2Convert.Location = new System.Drawing.Point(9, 9);
            this.lblTxt2Convert.Name = "lblTxt2Convert";
            this.lblTxt2Convert.Size = new System.Drawing.Size(173, 16);
            this.lblTxt2Convert.TabIndex = 0;
            this.lblTxt2Convert.Text = "Text to Convert to Geometry";
            // 
            // txtBxText
            // 
            this.txtBxText.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtBxText.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtBxText.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBxText.Location = new System.Drawing.Point(12, 30);
            this.txtBxText.Name = "txtBxText";
            this.txtBxText.Size = new System.Drawing.Size(265, 39);
            this.txtBxText.TabIndex = 1;
            this.txtBxText.Tag = "Enter text that will be rendered to geometry.";
            this.txtBxText.Text = "Sample";
            this.txtBxText.TextChanged += new System.EventHandler(this.txtBxText_TextChanged);
            this.txtBxText.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.textBox_HelpRequested);
            this.txtBxText.DoubleClick += new System.EventHandler(this.DblClickDelegate);
            this.txtBxText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDownDelegate);
            this.txtBxText.Leave += new System.EventHandler(this.txtBxText_Leave);
            // 
            // grpBoxFont
            // 
            this.grpBoxFont.Controls.Add(this.ComboFontCollection);
            this.grpBoxFont.Controls.Add(this.chkBxItalic);
            this.grpBoxFont.Controls.Add(this.chkBxBold);
            this.grpBoxFont.Controls.Add(this.lblFontName);
            this.grpBoxFont.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBoxFont.Location = new System.Drawing.Point(12, 81);
            this.grpBoxFont.Name = "grpBoxFont";
            this.grpBoxFont.Size = new System.Drawing.Size(265, 94);
            this.grpBoxFont.TabIndex = 2;
            this.grpBoxFont.TabStop = false;
            this.grpBoxFont.Text = "Font";
            // 
            // ComboFontCollection
            // 
            this.ComboFontCollection.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.ComboFontCollection.FormattingEnabled = true;
            this.ComboFontCollection.Location = new System.Drawing.Point(56, 16);
            this.ComboFontCollection.Name = "ComboFontCollection";
            this.ComboFontCollection.Size = new System.Drawing.Size(200, 24);
            this.ComboFontCollection.TabIndex = 2;
            this.ComboFontCollection.Text = "Arial";
            this.ComboFontCollection.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ComboFontCollection_DrawItem);
            // 
            // chkBxItalic
            // 
            this.chkBxItalic.AutoSize = true;
            this.chkBxItalic.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkBxItalic.Location = new System.Drawing.Point(199, 63);
            this.chkBxItalic.Name = "chkBxItalic";
            this.chkBxItalic.Size = new System.Drawing.Size(55, 21);
            this.chkBxItalic.TabIndex = 4;
            this.chkBxItalic.Text = "Italic";
            this.chkBxItalic.UseVisualStyleBackColor = true;
            this.chkBxItalic.CheckedChanged += new System.EventHandler(this.chkBxItalic_CheckedChanged);
            // 
            // chkBxBold
            // 
            this.chkBxBold.AutoSize = true;
            this.chkBxBold.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkBxBold.Location = new System.Drawing.Point(55, 63);
            this.chkBxBold.Name = "chkBxBold";
            this.chkBxBold.Size = new System.Drawing.Size(59, 21);
            this.chkBxBold.TabIndex = 3;
            this.chkBxBold.Text = "Bold";
            this.chkBxBold.UseVisualStyleBackColor = true;
            this.chkBxBold.CheckedChanged += new System.EventHandler(this.chkBxBold_CheckedChanged);
            // 
            // lblFontName
            // 
            this.lblFontName.AutoSize = true;
            this.lblFontName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFontName.Location = new System.Drawing.Point(7, 23);
            this.lblFontName.Name = "lblFontName";
            this.lblFontName.Size = new System.Drawing.Size(41, 15);
            this.lblFontName.TabIndex = 0;
            this.lblFontName.Text = "Name";
            // 
            // grpBoxHeight
            // 
            this.grpBoxHeight.Controls.Add(this.chkBxAsReg);
            this.grpBoxHeight.Controls.Add(this.txtBoxHeight);
            this.grpBoxHeight.Controls.Add(this.lblHeight);
            this.grpBoxHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBoxHeight.Location = new System.Drawing.Point(12, 186);
            this.grpBoxHeight.Name = "grpBoxHeight";
            this.grpBoxHeight.Size = new System.Drawing.Size(265, 94);
            this.grpBoxHeight.TabIndex = 4;
            this.grpBoxHeight.TabStop = false;
            this.grpBoxHeight.Text = "Geometry";
            // 
            // chkBxAsReg
            // 
            this.chkBxAsReg.AutoSize = true;
            this.chkBxAsReg.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkBxAsReg.Location = new System.Drawing.Point(55, 62);
            this.chkBxAsReg.Name = "chkBxAsReg";
            this.chkBxAsReg.Size = new System.Drawing.Size(82, 19);
            this.chkBxAsReg.TabIndex = 6;
            this.chkBxAsReg.Tag = "Not all font description are Region compatible.\nIf geometry creation fails, unche" + "ck box\nto insert geometry as Block Reference.";
            this.chkBxAsReg.Text = "As Region";
            this.chkBxAsReg.UseVisualStyleBackColor = true;
            this.chkBxAsReg.CheckedChanged += new System.EventHandler(this.chkBxAsReg_CheckedChanged);
            this.chkBxAsReg.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.textBox_HelpRequested);
            // 
            // txtBoxHeight
            // 
            this.txtBoxHeight.Location = new System.Drawing.Point(56, 23);
            this.txtBoxHeight.Name = "txtBoxHeight";
            this.txtBoxHeight.Size = new System.Drawing.Size(200, 23);
            this.txtBoxHeight.TabIndex = 5;
            this.txtBoxHeight.Tag = "Height based on drawing units: \nEnter height for bounding box of text geometry.";
            this.txtBoxHeight.Text = "1";
            this.txtBoxHeight.TextChanged += new System.EventHandler(this.txtBoxHeight_TextChanged);
            this.txtBoxHeight.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.textBox_HelpRequested);
            this.txtBoxHeight.DoubleClick += new System.EventHandler(this.DblClickDelegate);
            this.txtBoxHeight.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDownDelegate);
            this.txtBoxHeight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressDelegate);
            this.txtBoxHeight.Leave += new System.EventHandler(this.txtBoxHeight_Leave);
            // 
            // lblHeight
            // 
            this.lblHeight.AutoSize = true;
            this.lblHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeight.Location = new System.Drawing.Point(7, 24);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(43, 15);
            this.lblHeight.TabIndex = 0;
            this.lblHeight.Text = "Height";
            // 
            // btnPlaceGeom
            // 
            this.btnPlaceGeom.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPlaceGeom.Location = new System.Drawing.Point(12, 294);
            this.btnPlaceGeom.Name = "btnPlaceGeom";
            this.btnPlaceGeom.Size = new System.Drawing.Size(265, 32);
            this.btnPlaceGeom.TabIndex = 7;
            this.btnPlaceGeom.Text = "Insert Geometry";
            this.btnPlaceGeom.UseVisualStyleBackColor = true;
            this.btnPlaceGeom.Click += new System.EventHandler(this.btnPlaceGeom_Click);
            this.btnPlaceGeom.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnPlaceGeom_MouseClick);
            // 
            // lblHelpTxt
            // 
            this.lblHelpTxt.AutoSize = true;
            this.lblHelpTxt.Location = new System.Drawing.Point(12, 333);
            this.lblHelpTxt.MinimumSize = new System.Drawing.Size(265, 50);
            this.lblHelpTxt.Name = "lblHelpTxt";
            this.lblHelpTxt.Size = new System.Drawing.Size(265, 50);
            this.lblHelpTxt.TabIndex = 5;
            this.lblHelpTxt.Text = "Text to Geometry (t2g)                             Version 1.3.4\r\n\r\n";
            // 
            // FrmTxt2GeomUi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 391);
            this.Controls.Add(this.lblHelpTxt);
            this.Controls.Add(this.btnPlaceGeom);
            this.Controls.Add(this.grpBoxHeight);
            this.Controls.Add(this.grpBoxFont);
            this.Controls.Add(this.txtBxText);
            this.Controls.Add(this.lblTxt2Convert);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(305, 430);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(305, 430);
            this.Name = "FrmTxt2GeomUi";
            this.Tag = "Author:  Sean Tessier\n\nContributor:  Daniel Marcotte - Dialog Box control \nand ge" + "neral OOP guidance.";
            this.Text = "Text Converter";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.textBox_HelpRequested);
            this.grpBoxFont.ResumeLayout(false);
            this.grpBoxFont.PerformLayout();
            this.grpBoxHeight.ResumeLayout(false);
            this.grpBoxHeight.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }




        #endregion

        private Label lblTxt2Convert;
        private TextBox txtBxText;
        private GroupBox grpBoxFont;
        private Label lblFontName;
        private CheckBox chkBxItalic;
        private CheckBox chkBxBold;
        private GroupBox grpBoxHeight;
        private Label lblHeight;
        private TextBox txtBoxHeight;
        private System.Windows.Forms.Button btnPlaceGeom;
        private CheckBox chkBxAsReg;
        private Label lblHelpTxt;
        private ComboBox ComboFontCollection;
    }
}