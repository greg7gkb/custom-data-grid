using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CustomDataGrid
{
    public partial class CustomDataGrid : UserControl //, ISupportInitialize, IDataGridEditingService
    {
        //-------------------------------------------------------------------------------------
        // Data members
        private DataSet dataSource;
        private string dataMember;

        private int[] rowHeights;
        private int[] columnWidths;

        private int numColumns = 0;
        private int numRows = 0;
        private int visibleRowStartIndex = 0;
        private int maxVisibleRowStartIndex = 0;

        private int selectedDataCellCol = 0;
        private int selectedDataCellRow = 0;

        //------------------
        // Exposed data

        private Color captionBackColor;
        [Browsable(true)]
        public Color CaptionBackColor
        {
            get { return captionBackColor; }
            set { captionBackColor = value; }
        }

        private int captionHeight;
        private Font captionFont;
        [Browsable(true)]
        public Font CaptionFont
        {
            get { return captionFont; }
            set
            {
                captionFont = value;
                captionHeight = (int)(captionFont.GetHeight() * 1.5); // Fudge factor for padding around text
            }
        }

        private Color captionForeColor;
        [Browsable(true)]
        public Color CaptionForeColor
        {
            get { return captionForeColor; }
            set { captionForeColor = value; }
        }

        private string captionText;
        [Browsable(true)]
        public string CaptionText
        {
            get { return captionText; }
            set { captionText = value; }
        }

        private bool readOnly;
        [Browsable(true)]
        public bool ReadOnly
        {
            get { return readOnly; }
            set { readOnly = value; }
        }

        private bool rowHeadersVisible;
        [Browsable(true)]
        public bool RowHeadersVisible
        {
            get { return rowHeadersVisible; }
            set { rowHeadersVisible = value; }
        }

        private int columnHeaderHeight;

        private int rowHeaderWidth;
        [Browsable(true)]
        public int RowHeaderWidth
        {
            get { return rowHeaderWidth; }
            set { rowHeaderWidth = value; }
        }

        private int preferredColumnWidth;
        [Browsable(true)]
        public int PreferredColumnWidth
        {
            get { return preferredColumnWidth; }
            set { preferredColumnWidth = value; }
        }

        private int preferredRowHeight;
        [Browsable(true)]
        public int PreferredRowHeight
        {
            get { return preferredRowHeight; }
            set { preferredRowHeight = value; }
        }

        private Color headerBackColor;
        [Browsable(true)]
        public Color HeaderBackColor
        {
            get { return headerBackColor; }
            set { headerBackColor = value; }
        }

        private Font headerFont;
        [Browsable(true)]
        public Font HeaderFont
        {
            get { return headerFont; }
            set { headerFont = value; }
        }

        private Color headerForeColor;
        [Browsable(true)]
        public Color HeaderForeColor
        {
            get { return headerForeColor; }
            set { headerForeColor = value; }
        }

        //-------------------------------------------------------------------------------------
        // Methods

        public CustomDataGrid()
        {
            InitializeComponent();
            this.DoubleBuffered = true; // Prevents control from flickering during scrolling
            vSB.Visible = false;

            // Set default values & init data
            CaptionBackColor = Color.FromKnownColor(KnownColor.ActiveCaption);
            CaptionFont = new Font(this.Font.FontFamily, this.Font.SizeInPoints + 2);
            CaptionForeColor = Color.FromKnownColor(KnownColor.ControlText);
            CaptionText = "";

            PreferredColumnWidth = 75;
            PreferredRowHeight = 16;

            ReadOnly = true;
            RowHeadersVisible = false;
            RowHeaderWidth = 35;
            columnHeaderHeight = preferredRowHeight + 2;

            HeaderBackColor = Color.FromKnownColor(KnownColor.Control);
            HeaderFont = this.Font;
            HeaderForeColor = Color.FromKnownColor(KnownColor.ControlText);

            dataSource = null;
            dataMember = "";

            this.AutoScroll = true;
        }

        // SetDataBinding currently only supports a DataSet type as input. In the future, this 
        // could be changed to an Object type (with a bunch of switch/cases based on the object
        // type) to match what the .NET DataGrid class does.
        public void SetDataBinding(DataSet dataSource, string dataMember)
        {
            this.dataSource = dataSource;
            this.dataMember = dataMember;

            InitTableSizes(); // Table sizes are stored based on the number of rows and cols in the data
                              // set so we init these after the data set has changed.
            UpdateVScrollBar();
        }

        private void InitTableSizes()
        {
            try
            {
                // TODO: Increase height of a row if any of the text in the row is larger than
                //       the preferred row height.

                numColumns = dataSource.Tables[dataMember].Columns.Count;
                numRows = dataSource.Tables[dataMember].Rows.Count;

                rowHeights = new int[numRows];
                for (int i = 0; i < rowHeights.Length; i++) rowHeights[i] = preferredRowHeight;

                columnWidths = new int[numColumns];
                for (int i = 0; i < columnWidths.Length; i++) columnWidths[i] = preferredColumnWidth;

                visibleRowStartIndex = 0;
                CalcMaxVisibleRowIndex();
            }
            catch (Exception excp)
            {
#if DEBUG
                MessageBox.Show(excp.Message, "DEBUG - Exception");
#endif
            }
        }

        private void CalcMaxVisibleRowIndex()
        {
            // Calculate the maximum row start index based on the row heights at
            // the end of the data set
            int visibleHeight = this.Height - (captionHeight + 1) - (columnHeaderHeight + 1);
            int currentHeight = 0; int i;
            for (i = (numRows - 1); i >= 0; i--)
            {
                currentHeight += rowHeights[i] + 1;
                if (currentHeight > visibleHeight) break;
            }
            maxVisibleRowStartIndex = Math.Min(i + 1, numRows - 1);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            try // Draw what we can
            {
                DrawCaption(g);
                if (dataSource != null)
                {
                    DrawColumnHeaders(g);
                    DrawRowHeaders(g);
                    DrawDataRows(g);
                }
            }
            catch (Exception excp)
            {
#if DEBUG
                MessageBox.Show(excp.Message, "DEBUG - Exception in OnPaint");
#endif
            }
        }

        private void DrawCaption(Graphics g)
        {
            // Draw BG
            SolidBrush bgBrush = new SolidBrush(CaptionBackColor);
            RectangleF drawingRect = new Rectangle(0, 0, this.Width, captionHeight);
            g.FillRectangle(bgBrush, drawingRect);

            // Draw BG baseline
            Pen baselinePen = new Pen(Color.Black);
            g.DrawLine(baselinePen, 0, captionHeight, this.Width, captionHeight);

            // Draw caption text
            SolidBrush fgBrush = new SolidBrush(captionForeColor);
            drawingRect.Inflate(-3, -2);
            g.DrawString(captionText, captionFont, fgBrush, drawingRect);
        }

        private void DrawColumnHeaders(Graphics g)
        {
            int topOffset = captionHeight + 1;
            int leftOffset = 0;
            if (rowHeadersVisible) leftOffset = rowHeaderWidth + 1;

            for (int i = 0; i < numColumns; i++)
            {
                DataColumn column = dataSource.Tables[dataMember].Columns[i];
                Rectangle cellRect = new Rectangle(
                    leftOffset, topOffset, columnWidths[i], columnHeaderHeight);
                DrawHeaderCell(g, cellRect, headerFont, column.ColumnName, 
                    headerBackColor, headerForeColor);

                leftOffset += columnWidths[i] + 1;
            }
        }

        private void DrawRowHeaders(Graphics g)
        {
            if (!rowHeadersVisible) return;

            int topOffset = captionHeight + 1;

            // Draw empty column header cell
            Rectangle cellRect = new Rectangle(
                0, topOffset, rowHeaderWidth, columnHeaderHeight);
            DrawHeaderCell(g, cellRect, headerFont, "", headerBackColor, headerForeColor);
            topOffset += columnHeaderHeight + 1;

            for (int i = visibleRowStartIndex; i < numRows; i++)
            {
                // Draw empty header cell to create BG first
                cellRect = new Rectangle(0, topOffset, rowHeaderWidth, rowHeights[i]);
                DrawHeaderCell(g, cellRect, headerFont, "", headerBackColor, headerForeColor);

                // Overlay images onto cell
                Pen symPen = new Pen(headerForeColor);
                Rectangle symBox = new Rectangle(
                    cellRect.Right - 12, cellRect.Top + 3, 8, 8);
                g.DrawRectangle(symPen, symBox);

                // Draw "plus" symbol
                Point p1 = new Point(cellRect.Right - 8, cellRect.Top + 5);
                Point p2 = new Point(cellRect.Right - 8, cellRect.Top + 9);
                g.DrawLine(symPen, p1, p2);
                Point p3 = new Point(cellRect.Right - 10, cellRect.Top + 7);
                Point p4 = new Point(cellRect.Right - 6, cellRect.Top + 7);
                g.DrawLine(symPen, p3, p4);

                if (i == selectedDataCellRow)
                {
                    SolidBrush br = new SolidBrush(headerForeColor);
                    Point[] triangle = new Point[3];
                    triangle[0] = new Point(cellRect.X + 5, cellRect.Y + 3);
                    triangle[1] = new Point(cellRect.X + 5, cellRect.Y + 3 + 10);
                    triangle[2] = new Point(cellRect.X + 10, cellRect.Y + 3 + 5);
                    g.FillPolygon(br, triangle);
                }

                topOffset += rowHeights[i] + 1;
            }
        }

        private void DrawHeaderCell(Graphics g, Rectangle rect, Font font, string text, 
            Color bg, Color fg)
        {
            // Draw face
            SolidBrush bgBrush = new SolidBrush(bg);
            g.FillRectangle(bgBrush, rect);
            Pen bgPen = new Pen(bg);
            g.DrawRectangle(bgPen, rect); // Apparently fill doesn't do the border of the rect as well

            const int vGap = 2;

            // Draw border (highlight)
            Pen highlight = new Pen(Color.FromKnownColor(KnownColor.ButtonHighlight));
            g.DrawLine(highlight, rect.Left, rect.Top, rect.Right, rect.Top);
            g.DrawLine(highlight, rect.Left, rect.Top + vGap, rect.Left, rect.Bottom - vGap);

            // Draw border (shadow)
            Pen shadow = new Pen(Color.FromKnownColor(KnownColor.ButtonShadow));
            g.DrawLine(shadow, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
            g.DrawLine(shadow, rect.Right, rect.Top + vGap, rect.Right, rect.Bottom - vGap);

            // Draw Text
            SolidBrush fgBrush = new SolidBrush(fg);
            rect.Inflate(-2, -2);
            RectangleF textRect = new RectangleF(
                rect.X, rect.Y,
                rect.Width, rect.Height);
            g.DrawString(text, font, fgBrush, textRect);
        }

        private void DrawDataRows(Graphics g)
        {
            DestroyCaret(); // Delete caret here - it will automatically be redrawn by DrawDataCell()

            int leftOffset = 0;
            if (rowHeadersVisible) leftOffset = rowHeaderWidth + 1;

            for (int i = 0; i < numColumns; i++)
            {
                int topOffset = captionHeight + 1 + columnHeaderHeight + 1; // Push below caption and header row
                for (int j = visibleRowStartIndex; j < numRows; j++)
                {
                    string cellValue = dataSource.Tables[dataMember].Rows[j][i].ToString();
                    Rectangle cellRect = new Rectangle(
                        leftOffset, topOffset, columnWidths[i], rowHeights[j]);

                    bool cellSelected = false;
                    if ((i == selectedDataCellCol) && (j == selectedDataCellRow)) cellSelected = true;

                    DrawDataCell(g, cellRect, headerFont, cellValue,
                        Color.White, Color.Black, cellSelected);

                    topOffset += rowHeights[j] + 1;
                }

                leftOffset += columnWidths[i] + 1;
            }
        }

        private void DrawDataCell(Graphics g, Rectangle rect, Font font, string text,
            Color bg, Color fg, bool selected)
        {
            // Draw face
            SolidBrush bgBrush = new SolidBrush(bg);
            g.FillRectangle(bgBrush, rect);
            Pen bgPen = new Pen(bg);
            g.DrawRectangle(bgPen, rect); // Apparently fill doesn't do the border of the rect as well

            // Draw border
            Pen outline = new Pen(Color.FromKnownColor(KnownColor.Control));
            //g.DrawRectangle(outline, rect);
            g.DrawLine(outline, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
            g.DrawLine(outline, rect.Right, rect.Top, rect.Right, rect.Bottom);

            if (selected && (this.Focused || vSB.Focused))
            {
                // Draw cell as selected
                Rectangle selectionRect = new Rectangle(
                    rect.X + 2, rect.Y + 2, rect.Width - 2, rect.Height - 2);
                SolidBrush selectionBrush = new SolidBrush(
                    Color.FromKnownColor(KnownColor.Control));
                g.FillRectangle(selectionBrush, selectionRect);

                if (!string.IsNullOrEmpty(text))
                {
                    // Draw dark highlighting under text
                    Size textSize = TextRenderer.MeasureText(text, font);
                    Rectangle textHighlightRect = new Rectangle(
                        rect.X + 2, rect.Y + 2,
                        textSize.Width - 6, // Fudge factor - not sure why MeasureText is returning extra width
                        textSize.Height);
                    SolidBrush textHighlightBrush = new SolidBrush(
                        Color.FromKnownColor(KnownColor.ControlDark));
                    g.FillRectangle(textHighlightBrush, textHighlightRect);

                    // Draw the control caret
                    Point caretLocation = new Point(textHighlightRect.Right, textHighlightRect.Top);
                    Size caretSize = new Size(1, textHighlightRect.Height);
                    UpdateCaret(caretLocation, caretSize);
                }
            }

            // Draw Text
            SolidBrush fgBrush = new SolidBrush(fg);
            rect.Inflate(0, -2);
            RectangleF textRect = new RectangleF(
                rect.X, rect.Y,
                rect.Width, rect.Height);
            g.DrawString(text, font, fgBrush, textRect);
        }

        private void UpdateVScrollBar()
        {
            int dataGridHeight = this.Height - (captionHeight + 1);
            if ((dataGridHeight < TotalRowHeights()) &&
                (dataSource != null)) // Show the scroll bar
            {
                int borderOffset = 2;
                if (this.BorderStyle == BorderStyle.Fixed3D) borderOffset = 4; // Extra padding for thicker border

                try
                {
                    vSB.Size = new Size(vSB.Width, this.Height - (captionHeight + 1) - borderOffset);
                    vSB.Location = new Point(this.Width - vSB.Width - borderOffset, captionHeight + 1);

                    vSB.Maximum = numRows;
                    vSB.Minimum = 0;

                    vSB.LargeChange = (int)((this.Height - captionHeight) / (float)preferredRowHeight);
                    vSB.SmallChange = 1;

                    vSB.BringToFront();
                    vSB.Visible = true;
                    vSB.Enabled = true;
                }
                catch (Exception excp)
                {
#if DEBUG
                    MessageBox.Show(excp.Message, "DEBUG - Exception");
#endif
                    vSB.Visible = false;
                }
            }
            else
            {
                vSB.Visible = false;
            }
        }

        private int TotalRowHeights()
        {
            int rtn = 0;
            if (dataSource != null)
            {
                rtn += columnHeaderHeight;
                for (int i = 0; i < numRows; i++) rtn += rowHeights[i];
            }
            return rtn;
        }

        private void CustomDataGrid_Resize(object sender, EventArgs e)
        {
            UpdateVScrollBar();
        }

        private void vSB_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.EndScroll) return;

            switch (e.Type)
            {
                case ScrollEventType.SmallIncrement:
                    if (visibleRowStartIndex < maxVisibleRowStartIndex)
                    {
                        visibleRowStartIndex = Math.Min(
                            visibleRowStartIndex + vSB.SmallChange, maxVisibleRowStartIndex);
                        this.Invalidate(true);
                    }
                    break;
                case ScrollEventType.SmallDecrement:
                    if (visibleRowStartIndex > 0)
                    {
                        visibleRowStartIndex = Math.Max(
                            visibleRowStartIndex - vSB.SmallChange, 0);
                        this.Invalidate(true);
                    }
                    break;
                case ScrollEventType.LargeIncrement:
                    if (visibleRowStartIndex < maxVisibleRowStartIndex)
                    {
                        visibleRowStartIndex = Math.Min(
                            visibleRowStartIndex + vSB.LargeChange, maxVisibleRowStartIndex);
                        this.Invalidate(true);
                    }
                    break;
                case ScrollEventType.LargeDecrement:
                    if (visibleRowStartIndex > 0)
                    {
                        visibleRowStartIndex = Math.Max(
                            visibleRowStartIndex - vSB.LargeChange, 0);
                        this.Invalidate(true);
                    }
                    break;
                case ScrollEventType.ThumbTrack:
                case ScrollEventType.ThumbPosition:
                    visibleRowStartIndex = e.NewValue;
                    this.Invalidate(true);
                    break;
                default:
                    // Do nothing
                    break;
            }
        }

        #region Include system DLLs to draw a caret (cursor)
        // From: http://stackoverflow.com/questions/539132/how-do-you-include-a-cursor-caret-in-a-custom-control
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool CreateCaret(IntPtr hWnd, IntPtr hBmp, int w, int h);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetCaretPos(int x, int y);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowCaret(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyCaret();
        #endregion

        private void CustomDataGrid_Enter(object sender, EventArgs e)
        {
            base.OnGotFocus(e);
            this.Invalidate(true);
        }

        private void CustomDataGrid_Leave(object sender, EventArgs e)
        {
            base.OnLostFocus(e);
            this.Invalidate(true);
        }

        private void UpdateCaret(Point location, Size size)
        {
            try
            {
                CreateCaret(this.Handle, IntPtr.Zero, size.Width, size.Height);
                SetCaretPos(location.X, location.Y);
                ShowCaret(this.Handle);
            }
            catch
            {
                DestroyCaret();
            }
        }

        private void CustomDataGrid_MouseClick(object sender, MouseEventArgs e)
        {
            int comparisonHeight = captionHeight + 1;
            if (e.Location.Y < comparisonHeight)
            {
                // Do nothing if the caption is clicked
                return;
            }
            comparisonHeight += columnHeaderHeight + 1;
            if (e.Location.Y < comparisonHeight)
            {
                // TODO:
                // Sort the rows based on the column clicked
                return;
            }
            else // User clicked in data rows
            {
                if ((rowHeadersVisible) && (e.Location.X <= rowHeaderWidth))
                {
                    // Pretend for now that the user clicked on the left-most column
                    Point fakeLocation = new Point(rowHeaderWidth + 1, e.Location.Y);
                    UpdateDataCellSelection(fakeLocation);
                }
                else
                {
                    UpdateDataCellSelection(e.Location);
                }
                return;
            }
        }

        private void UpdateDataCellSelection(Point clickLocation)
        {
            try
            {
                Point cellLocation = ConvertLocationToDataCell(clickLocation);
                selectedDataCellCol = cellLocation.X;
                selectedDataCellRow = cellLocation.Y;
            }
            catch
            {
                selectedDataCellCol = -1;
                selectedDataCellRow = -1;
            }

            this.Invalidate(true);
        }

        /// <returns>Returns a point representing the x,y cell location within the data grid</returns>
        private Point ConvertLocationToDataCell(Point clickLocation)
        {
            Point rtn = new Point();
            int col, row;

            // Find column
            int horizontalPosition = 0;
            if (rowHeadersVisible)
            {
                horizontalPosition = rowHeaderWidth;
                if (clickLocation.X <= horizontalPosition)
                    return new Point(-1, -1); // Set data cell location to invalid
            }
            for (col = 0; col < numColumns; col++)
            {
                horizontalPosition += columnWidths[col] + 1;
                if (clickLocation.X <= horizontalPosition) break;
            }

            // Find row
            int verticalPosition = captionHeight + 1 + columnHeaderHeight + 1;
            for (row = visibleRowStartIndex; row < numRows; row++)
            {
                verticalPosition += rowHeights[row] + 1;
                if (clickLocation.Y <= verticalPosition) break;
            }

            if ((clickLocation.X > horizontalPosition) ||
                (clickLocation.Y > verticalPosition))
            {
                // User clicked outside of the data so don't change selected cell
                rtn.X = selectedDataCellCol;
                rtn.Y = selectedDataCellRow;
            }
            else
            {
                rtn.X = col;
                rtn.Y = row;
            }
            return rtn;
        }
    }
}
