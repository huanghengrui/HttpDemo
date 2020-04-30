
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System;
using System.Drawing.Text;
using System.ComponentModel;

namespace RenLianShiBie
{
    public class CTabControl : TabControl
    {
        public CTabControl()
            : base()
        {
            SetStyles();
        }

        private void SetStyles()
        {
            base.SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);
            base.UpdateStyles();
        }

        private Color _backColor = Color.FromArgb(23, 169, 254);
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DefaultValue(typeof(Color), "23, 169, 254")]
        public override Color BackColor
        {
            get { return _backColor; }
            set
            {
                _backColor = value;
                base.Invalidate(true);
            }
        }

        private Color _borderColor = Color.FromArgb(23, 169, 254);
        [DefaultValue(typeof(Color), "23, 169, 254")]
        [Description("TabContorl边框色")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                base.Invalidate(true);
            }
        }

        private Color _headSelectedBackColor = Color.FromArgb(23, 169, 254);
        [DefaultValue(typeof(Color), "23, 169, 254")]
        [Description("TabPage头部选中后的背景颜色")]
        public Color HeadSelectedBackColor
        {
            get { return _headSelectedBackColor; }
            set { _headSelectedBackColor = value; }
        }

        private Color _headSelectedBorderColor = Color.FromArgb(23, 169, 254);
        [DefaultValue(typeof(Color), "23, 169, 254")]
        [Description("TabPage头部选中后的边框颜色")]
        public Color HeadSelectedBorderColor
        {
            get { return _headSelectedBorderColor; }
            set { _headSelectedBorderColor = value; }
        }

        private Color _headerBackColor = Color.FromArgb(23, 169, 254);
        [DefaultValue(typeof(Color), "23, 169, 254")]
        [Description("TabPage头部默认边框颜色")]
        public Color HeaderBackColor
        {
            get { return _headerBackColor; }
            set { _headerBackColor = value; }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (this.DesignMode == true)
            {
                LinearGradientBrush backBrush = new LinearGradientBrush(
                            this.Bounds,
                            SystemColors.ControlLightLight,
                            SystemColors.ControlLight,
                            LinearGradientMode.Vertical);
                pevent.Graphics.FillRectangle(backBrush, this.Bounds);
                backBrush.Dispose();
            }
            else
            {
                this.PaintTransparentBackground(pevent.Graphics, this.ClientRectangle);
            }
        }

        /// <summary>
        ///  TabContorl 背景色设置
        /// </summary>
        /// <param name="g"></param>
        /// <param name="clipRect"></param>
        protected void PaintTransparentBackground(Graphics g, Rectangle clipRect)
        {
            if ((this.Parent != null))
            {
                clipRect.Offset(this.Location);
                PaintEventArgs e = new PaintEventArgs(g, clipRect);
                GraphicsState state = g.Save();
                g.SmoothingMode = SmoothingMode.HighSpeed;
                try
                {
                    g.TranslateTransform((float)-this.Location.X, (float)-this.Location.Y);
                    this.InvokePaintBackground(this.Parent, e);
                    this.InvokePaint(this.Parent, e);
                }
                finally
                {
                    g.Restore(state);
                    clipRect.Offset(-this.Location.X, -this.Location.Y);
                    //新加片段,待测试
                    using (SolidBrush brush = new SolidBrush(_backColor))
                    {
                        clipRect.Inflate(1, 1);
                        g.FillRectangle(brush, clipRect);
                    }
                }
            }
            else
            {
                System.Drawing.Drawing2D.LinearGradientBrush backBrush = new System.Drawing.Drawing2D.LinearGradientBrush(this.Bounds, SystemColors.ControlLightLight, SystemColors.ControlLight, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
                g.FillRectangle(backBrush, this.Bounds);
                backBrush.Dispose();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Paint the Background 
            base.OnPaint(e);
            this.PaintTransparentBackground(e.Graphics, this.ClientRectangle);
            this.PaintAllTheTabs(e);
            this.PaintTheTabPageBorder(e);
            this.PaintTheSelectedTab(e);
        }

        private void PaintAllTheTabs(System.Windows.Forms.PaintEventArgs e)
        {
            if (this.TabCount > 0)
            {
                for (int index = 0; index < this.TabCount; index++)
                {
                    this.PaintTab(e, index);
                }
            }
        }

        private void PaintTab(System.Windows.Forms.PaintEventArgs e, int index)
        {
            GraphicsPath path = this.GetPath(index);
            this.PaintTabBackground(e.Graphics, index, path);
            this.PaintTabBorder(e.Graphics, index, path);
            this.PaintTabText(e.Graphics, index);
            this.PaintTabImage(e.Graphics, index);
        }

        /// <summary>
        /// 设置选项卡头部颜色
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="index"></param>
        /// <param name="path"></param>
        private void PaintTabBackground(System.Drawing.Graphics graph, int index, System.Drawing.Drawing2D.GraphicsPath path)
        {
            Rectangle rect = this.GetTabRect(index);
            System.Drawing.Brush buttonBrush;
            buttonBrush = new System.Drawing.SolidBrush(_headSelectedBackColor);
            //System.Drawing.Brush buttonBrush = new System.Drawing.SolidBrush(_headerBackColor);
            if (index == this.SelectedIndex)
            {
                buttonBrush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, Color.FromArgb(44, 157, 249) /*SystemColors.ControlLightLight*/, /*SystemColors.ControlLight*/ Color.FromArgb(21, 124, 210), LinearGradientMode.Vertical);  //非选中时候的 TabPage 页头部背景色

                //buttonBrush = new System.Drawing.SolidBrush(SystemColors.ControlLightLight); // TabPage 选中时候页头部背景色
                
            }
            graph.FillPath(buttonBrush, path);
            buttonBrush.Dispose();
        }

        /// <summary>
        /// 设置选项卡头部边框色
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="index"></param>
        /// <param name="path"></param>
        private void PaintTabBorder(System.Drawing.Graphics graph, int index, System.Drawing.Drawing2D.GraphicsPath path)
        {
            //Pen borderPen = new Pen(SystemColors.ControlDark);
            Pen borderPen = new Pen(_borderColor);// TabPage 非选中时候的 TabPage 头部边框色
            if (index == this.SelectedIndex)
            {
                //borderPen = new Pen(ThemedColors.ToolBorder);
                borderPen = new Pen(_headSelectedBorderColor); // TabPage 选中后的 TabPage 头部边框色
                graph.DrawPath(borderPen, path);
            }
            
            borderPen.Dispose();
        }

        private void PaintTabImage(System.Drawing.Graphics g, int index)
        {
            Image tabImage = null;
            if (this.TabPages[index].ImageIndex > -1 && this.ImageList != null)
            {
                tabImage = this.ImageList.Images[this.TabPages[index].ImageIndex];
            }
            else if (this.TabPages[index].ImageKey.Trim().Length > 0 && this.ImageList != null)
            {
                tabImage = this.ImageList.Images[this.TabPages[index].ImageKey];
            }
            if (tabImage != null)
            {
                Rectangle rect = this.GetTabRect(index);
                g.DrawImage(tabImage, rect.Right - rect.Height - 4, 4, rect.Height - 2, rect.Height - 2);
            }
        }

        private void PaintTabText(System.Drawing.Graphics graph, int index)
        {
            Rectangle rect = this.GetTabRect(index);

            Rectangle rect2 = new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height);
            //Rectangle rect2 = new Rectangle(rect.Left + 16, rect.Top + 1, rect.Width - 6, rect.Height);

            //if (index == 0)
            //{
            //    rect2 = new Rectangle(rect.Left + rect.Height, rect.Top + 1, rect.Width - rect.Height, rect.Height);
            //}

            string tabtext = this.TabPages[index].Text;

            System.Drawing.StringFormat format = new System.Drawing.StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            format.Trimming = StringTrimming.EllipsisCharacter;

            Brush forebrush = null;

            if (this.TabPages[index].Enabled == false)
            {
                forebrush = SystemBrushes.ControlDark;
            }
            else
            {
                forebrush = SystemBrushes.ControlText;
            }

            Font tabFont = this.Font;
            if (index == this.SelectedIndex)
            {
                forebrush = SystemBrushes.ControlLightLight;
                tabFont = new Font(this.Font, FontStyle.Bold);
                //if (index == 0)
                //{
                //    rect2 = new Rectangle(rect.Left + rect.Height, rect.Top + 1, rect.Width - rect.Height + 5, rect.Height);
                //}
            }
            graph.DrawString(tabtext, tabFont, forebrush, rect2, format);
        }

        /// <summary>
        /// 设置 TabPage 内容页边框色
        /// </summary>
        /// <param name="e"></param>
        private void PaintTheTabPageBorder(System.Windows.Forms.PaintEventArgs e)
        {
            if (this.TabCount > 0)
            {
                Rectangle borderRect = this.TabPages[0].Bounds;
                borderRect.Inflate(1, 1);
                //ControlPaint.DrawBorder(e.Graphics, borderRect, ThemedColors.ToolBorder, ButtonBorderStyle.Solid);
                ControlPaint.DrawBorder(e.Graphics, borderRect, this.BorderColor, ButtonBorderStyle.Solid);
            }
        }

        /// <summary>
        /// // TabPage 页头部间隔色
        /// </summary>
        /// <param name="e"></param>
        private void PaintTheSelectedTab(System.Windows.Forms.PaintEventArgs e)
        {
            Rectangle selrect;
            int selrectRight = 0;

            switch (this.SelectedIndex)
            {
                case -1:
                    break;
                /*case 0:
                    selrect = this.GetTabRect(this.SelectedIndex);
                    selrectRight = selrect.Right;
                    e.Graphics.DrawLine(SystemPens.ControlLightLight, selrect.Left + 2, selrect.Bottom + 1, selrectRight - 2, selrect.Bottom + 1);
                    break;*/
                default:
                    selrect = this.GetTabRect(this.SelectedIndex);
                    selrectRight = selrect.Right;
                    e.Graphics.DrawLine(SystemPens.ControlLightLight, selrect.Left + 6 - selrect.Height, selrect.Bottom + 2, selrectRight - 2, selrect.Bottom + 2);
                    break;
            }
        }

        private GraphicsPath GetPath(int index)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.Reset();

            Rectangle rect = this.GetTabRect(index);

            switch (Alignment)
            {
                case TabAlignment.Top:

                    break;
                case TabAlignment.Bottom:

                    break;
                case TabAlignment.Left:

                    break;
                case TabAlignment.Right:

                    break;
            }

           /* if (index == 0)*/
            {
                path.AddLine(rect.Left + 1, rect.Top + 1, rect.Left + 1, rect.Top + 1);
                path.AddLine(rect.Left + 1, rect.Top + 1, rect.Right, rect.Top + 1);
                path.AddLine(rect.Right, rect.Top + 1, rect.Right, rect.Bottom);
                path.AddLine(rect.Right, rect.Bottom, rect.Left + 1, rect.Bottom);
            }
            /*else
            {
                if (index == this.SelectedIndex)
                {
                    path.AddLine(rect.Left - 1, rect.Top, rect.Left - 1, rect.Top);
                    path.AddLine(rect.Left - 1, rect.Top, rect.Right, rect.Top);
                    path.AddLine(rect.Right, rect.Top, rect.Right, rect.Bottom);
                    path.AddLine(rect.Right, rect.Bottom + 1, rect.Left - 1, rect.Bottom + 1);
                }
                else
                {
                    path.AddLine(rect.Left - 1, rect.Top, rect.Left - 1, rect.Top);
                    path.AddLine(rect.Left - 1, rect.Top, rect.Right, rect.Top);
                    path.AddLine(rect.Right, rect.Top, rect.Right, rect.Bottom);
                    path.AddLine(rect.Right, rect.Bottom + 1, rect.Left - 1, rect.Bottom + 1);
                }
            }*/
            return path;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private const int WM_SETFONT = 0x30;
        private const int WM_FONTCHANGE = 0x1d;

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            this.OnFontChanged(EventArgs.Empty);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            IntPtr hFont = this.Font.ToHfont();
            SendMessage(this.Handle, WM_SETFONT, hFont, (IntPtr)(-1));
            SendMessage(this.Handle, WM_FONTCHANGE, IntPtr.Zero, IntPtr.Zero);
            this.UpdateStyles();
            //this.ItemSize = new Size(0, this.Font.Height + 2);
        }
    }
}