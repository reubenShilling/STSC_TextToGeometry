using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace STSC_TextToGeometry_v19

{
    public partial class FrmTxt2GeomUi : Form
    {
        //Fields
        private FontFamily m_drawingFontFam;
        private bool m_bold;
        private bool m_italisized;
        private bool m_asRegion;
        private FontStyle m_drawingFontStyle;
        private readonly float m_sz;
        private Font m_font;
        private Double m_height = 1.0;
        private string m_textString = "Sample";
        private string m_fontName = "Arial";
        private bool m_isValid;
        internal TextConverter Tc;
        private bool m_htUnitValid;
        private SolidBrush m_foreColorBrush;
        private List<string> m_italicOnly;
        private List<string> m_regularOnly;
        private bool m_configIsSet;
        private readonly bool m_isInitializing = true;

        private const string SectionName = "STSCT2GSettings";


        //Constructor
        public FrmTxt2GeomUi()
        {
            InitializeComponent();
            m_font = txtBxText.Font;
            m_fontName = m_font.Name;
            m_drawingFontStyle = m_font.Style;
            m_sz = m_font.Size;
            m_height = 1.0;
            m_drawingFontFam = m_font.FontFamily;
            PopulateComboBoxWithFonts();
            Tc = null;

            m_configIsSet = RetrieveRegistrySettings();            
            m_isInitializing = false;
            m_htUnitValid = true;
            ConfirmReadiness(false);
        }

        //Destuctor
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                if (m_font != null) m_font.Dispose();
                if (m_drawingFontFam != null) m_drawingFontFam.Dispose();
                if (m_foreColorBrush != null) m_foreColorBrush.Dispose();
                if (Tc != null) Tc.Dispose();
            }
            base.Dispose(disposing);
        }

        //Properties
        public string TextString
        {
            get { return m_textString; }
        }

        public Double TextHeight
        {
            get { return m_height; }
        }

        public Font TextFont
        {
            get { return m_font; }
        }

        internal TextConverter TextConverter
        {
            get { return Tc; }
        }


        //Internals
        private void PopulateComboBoxWithFonts()//I've left this as is, i.e., not as the last sucessful build
        {
            m_italicOnly = new List<string>();
            m_regularOnly = new List<string>();
            Font tester = null;
            var fsr = FontStyle.Regular;
            var fsi = FontStyle.Italic;
            foreach (var indvFontFamily in FontFamily.Families)
            {
                try
                {
                    tester = new Font(indvFontFamily.Name, ComboFontCollection.Font.Size, fsi, GraphicsUnit.Point);
                }
                catch
                {
                    m_regularOnly.Add(indvFontFamily.Name);
                    try
                    {
                        tester = new Font(indvFontFamily.Name, ComboFontCollection.Font.Size, fsr, GraphicsUnit.Point);
                    }
                    catch
                    {

                    }
                }

                try
                {
                    tester = new Font(indvFontFamily.Name, ComboFontCollection.Font.Size, fsr, GraphicsUnit.Point);
                }
                catch
                {
                    m_italicOnly.Add(indvFontFamily.Name);
                    try
                    {
                        tester = new Font(indvFontFamily.Name, ComboFontCollection.Font.Size, fsi, GraphicsUnit.Point);
                    }
                    catch
                    {

                    }
                }
                AddToCombo(new ComboBoxFontItem(tester));
            }
        }


        private void AddToCombo(ComboBoxFontItem FontToAdd)
        {
            ComboFontCollection.Items.Add(FontToAdd);
            ComboFontCollection.AutoCompleteCustomSource.Add(FontToAdd.ToString());
        }


        private void KeyPressDelegate(object sender, KeyPressEventArgs e)
        {

            if (ValidateKeyPress(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private bool ValidateKeyPress(int TestSubject)
        {
            switch (TestSubject)
            {
                case 8:
                    return false;

                case 32:
                    return false;

                case 34:
                    return false;

                case 39:
                    return false;

                case 43:
                case 44:
                    return false;
                case 46:
                case 47:
                case 48:
                case 49:
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                case 56:
                case 57:
                    return false;

                default:
                    return true;
            }
        }

        private void DblClickDelegate(object sender, EventArgs e)
        {
            var txtDelegate = sender as TextBox;
            txtDelegate.Text = "";

            btnPlaceGeom.Enabled = false;
            if (txtDelegate.Name == "txtBxText")
            {
                lblHelpTxt.Text = "Enter new text string!";
                m_textString = "";
            }
            else
            {
                lblHelpTxt.Text = "Enter new text height!";
                m_height = 0.0;
                m_htUnitValid = false;
            }
        }


        private void KeyDownDelegate(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void btnPlaceGeom_MouseClick(object sender, MouseEventArgs e)
        {
            Tc = new TextConverter(TextString, TextFont, m_height, m_asRegion);
            
            DialogResult = DialogResult.OK;
            SetRegistrySettings();
            
            Close();
        }



        private bool CompileFont()
        {
            try
            {
                m_drawingFontFam = new FontFamily(m_fontName);
                if (m_italicOnly.Contains(m_fontName)) m_font = new Font(m_drawingFontFam, m_sz, FontStyle.Italic);
                else if (m_regularOnly.Contains(m_fontName)) m_font = new Font(m_drawingFontFam, m_sz, FontStyle.Regular);
                else m_font = new Font(m_drawingFontFam, m_sz, m_drawingFontStyle);
                return true;
            }
            catch
            {
                return false;
            }
        }


        private void ShowWarning()
        {
            btnPlaceGeom.Enabled = false;
            lblHelpTxt.Text = "Setting incompatible with font!";

        }

        private void ShowGoAhead()
        {
            btnPlaceGeom.Enabled = false;
            lblHelpTxt.Text = "Text ready for insertion.";

        }



        //All this should work
        private void chkBxBold_CheckedChanged(object sender, EventArgs e)
        {
            m_bold = (sender as CheckBox).Checked;
            UpdatedFontStyle();
            if (!m_isInitializing) ConfirmReadiness(true);
            if (!m_isValid) ShowWarning();
        }

        private void chkBxItalic_CheckedChanged(object sender, EventArgs e)
        {
            m_italisized = (sender as CheckBox).Checked;
            UpdatedFontStyle();
            if (!m_isInitializing) ConfirmReadiness(true);
            if (!m_isValid) ShowWarning();
        }

        private void chkBxAsReg_CheckedChanged(object sender, EventArgs e)
        {
            m_asRegion = (sender as CheckBox).Checked;
            if (m_asRegion) lblHelpTxt.Text = (string)chkBxAsReg.Tag;
            else lblHelpTxt.Text = "Text geometry to be inserted as Block Reference.";
        }
        //End All this should work


        private void UpdatedFontStyle()
        {
            m_drawingFontStyle = FontStyle.Regular;
            if (m_bold && m_italisized)
            {
                m_drawingFontStyle = FontStyle.Bold | FontStyle.Italic;
                return;
            }

            if (m_bold) m_drawingFontStyle = FontStyle.Bold;
            if (m_italisized) m_drawingFontStyle = FontStyle.Italic;
            
        }


        private void txtBoxHeight_TextChanged(object sender, EventArgs e)
        {
            var fromSender = sender as TextBox;
            var trimmed = fromSender.Text.Trim();
            if (trimmed == "" ||
                trimmed == "0." ||
                trimmed == ".")
            {
                m_height = 0.0;
                m_htUnitValid = false;
                if (!m_isInitializing) btnPlaceGeom.Enabled = false;
            }

            else
            {
                try
                {
                    m_height = Converter.StringToDistance(trimmed);
                    if (m_height > 0.0)
                    {
                        m_htUnitValid = true;
                        btnPlaceGeom.Enabled = true;
                    }
                    else
                    {
                        m_htUnitValid = false;
                        btnPlaceGeom.Enabled = false;
                    }
                }
                catch
                {
                    m_htUnitValid = false;
                    btnPlaceGeom.Enabled = false;
                }
            }
        }


        private void txtBoxHeight_Leave(object sender, EventArgs e)
        {
            if (!m_htUnitValid)
            {
                lblHelpTxt.Text = "Error while resolving geometry height!";
                txtBoxHeight.Text = "";
                m_isValid = false;
                btnPlaceGeom.Enabled = false;
            }
            else
            {
                ConfirmReadiness(false);
                txtBoxHeight.Text = m_height.ToString();
            }
        }


        private void txtBxText_TextChanged(object sender, EventArgs e)
        {
            m_textString = (sender as TextBox).Text.Trim();
            if (!m_isInitializing) ConfirmReadiness(false);
        }


        private void txtBxText_Leave(object sender, EventArgs e)
        {
            ConfirmReadiness(false);
        }




        private void ConfirmReadiness(bool Compile)
        {
            var compileSuccess = true;
            if (Compile)
            {
                compileSuccess = CompileFont();
            }

            if (compileSuccess)
            {
                if (m_htUnitValid &&
                    m_textString != "")
                {
                    txtBxText.Font = m_font;
                    ShowGoAhead();
                    btnPlaceGeom.Enabled = true;
                }
                else btnPlaceGeom.Enabled = false;
            }
            else btnPlaceGeom.Enabled = false;
        }




        private void textBox_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            // This event is raised when the F1 key is pressed or the
            // Help cursor is clicked on any of the address fields.
            // The Help text for the field is in the control's
            // Tag property. It is retrieved and displayed in the label.

            var requestingControl = (Control)sender;
            lblHelpTxt.Text = (string)requestingControl.Tag;
            hlpevent.Handled = true;
        }


        private void ComboFontCollection_DrawItem(object sender, DrawItemEventArgs e)
        {
            Brush brush;

            // Create the brush using the ForeColor specified by the DrawItemEventArgs
            if (m_foreColorBrush == null)
                m_foreColorBrush = new SolidBrush(e.ForeColor);
            else if (m_foreColorBrush.Color != e.ForeColor)
            {
                // The control's ForeColor has changed, so dispose of the cached brush and
                // create a new one.
                m_foreColorBrush.Dispose();
                m_foreColorBrush = new SolidBrush(e.ForeColor);
            }
            m_fontName = ComboFontCollection.Items[e.Index].ToString();

            // Select the appropriate brush depending on if the item is selected.
            // Since State can be a combinateion (bit-flag) of enum values, you can't use
            // "==" to compare them.
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                brush = SystemBrushes.HighlightText;
                m_isValid = CompileFont();

                if (m_isValid) txtBxText.Font = m_font;
            }
            else
                brush = m_foreColorBrush;

            // Perform the painting.
            var font = ComboFontCollection.Font;
            e.DrawBackground();
            e.Graphics.DrawString(m_fontName, font, brush, e.Bounds);
            e.DrawFocusRectangle();
        }


        private bool RetrieveRegistrySettings()
        {
            var prf = Autodesk.AutoCAD.ApplicationServices.Core.Application.UserConfigurationManager.OpenCurrentProfile();

            using (prf)
            {
                if (!prf.ContainsSubsection(SectionName))
                {
                    m_configIsSet = false;
                    SetRegistrySettings();
                    return true;
                }


                var sec = prf.OpenSubsection(SectionName);
                using (sec)
                {
                    m_height = (double)sec.ReadProperty("HeightProperty", 1.0);
                    txtBoxHeight.Text = m_height.ToString();
                    m_textString = (string)sec.ReadProperty("TextProperty", "Sample");
                    txtBxText.Text = m_textString;
                    m_fontName = (string)sec.ReadProperty("FontNameProperty", "Arial");

                    ComboFontCollection.Text = m_fontName;
                    m_bold = (bool)sec.ReadProperty("BoldProperty", false);
                    chkBxBold.Checked = m_bold;
                    m_italisized = (bool)sec.ReadProperty("ItalicProperty", false);
                    chkBxItalic.Checked = m_italisized;

                    m_isValid = CompileFont();

                    m_asRegion = (bool)sec.ReadProperty("AsRegionProperty", false);
                    chkBxAsReg.Checked = m_asRegion;
                }
            }
            return true;
        }



        private void SetRegistrySettings()
        {
            using (var con = Autodesk.AutoCAD.ApplicationServices.Core.Application.UserConfigurationManager.OpenCurrentProfile())
            {
                IConfigurationSection sec;
                if (m_configIsSet) sec = con.OpenSubsection(SectionName);
                else
                {
                    sec = CreateRegistrySection(con);
                }

                using (sec)
                {
                    sec.WriteProperty("HeightProperty", m_height);
                    sec.WriteProperty("TextProperty", m_textString);
                    sec.WriteProperty("FontNameProperty", m_fontName);
                    sec.WriteProperty("BoldProperty", m_bold);
                    sec.WriteProperty("ItalicProperty", m_italisized);
                    sec.WriteProperty("AsRegionProperty", m_asRegion);
                }
            }
        }


        private IConfigurationSection CreateRegistrySection(IConfigurationSection con)
        {
            IConfigurationSection sec;
            sec = con.CreateSubsection(SectionName);
            m_configIsSet = true;
            return sec;
        }

        private void btnPlaceGeom_Click(object sender, EventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }



    public class ComboBoxFontItem
    {
        public Font Font;

        public ComboBoxFontItem(Font f)
        {
            Font = f;
        }

        public override string ToString()
        {
            return Font.Name;
        }
    }
}
