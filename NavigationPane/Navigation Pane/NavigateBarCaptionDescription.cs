/*
 | Version 10.1.1
 | Copyright 2012 Esri
 |
 | Licensed under the Apache License, Version 2.0 (the "License");
 | you may not use this file except in compliance with the License.
 | You may obtain a copy of the License at
 |
 |    http://www.apache.org/licenses/LICENSE-2.0
 |
 | Unless required by applicable law or agreed to in writing, software
 | distributed under the License is distributed on an "AS IS" BASIS,
 | WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 | See the License for the specific language governing permissions and
 | limitations under the License.
 */


/*
 * Project	    : Outlook 2003 Style Navigation Pane
 *
 * Author       : Muhammed ŞAHİN
 * eMail        : muhammed.sahin@gmail.com
 *
 * Description  : NavigateBar caption description band
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace MobileControls
{
    #region Class : NavigateBarCaptionDescription

    [Browsable(false)]
    [ToolboxItem(false)]
    class NavigateBarCaptionDescription : UserControl
    {

        #region Caption
        private string caption = "";
        public string Caption
        {
            get { return caption; }
            set
            {
                caption = value;
                Invalidate();
            }
        }
        #endregion

        #region NavigateBar
        NavigateBar navigateBar = null;
        internal NavigateBar NavigateBar
        {
            get { return navigateBar; }
            set
            {
                navigateBar = value;
                Invalidate();
            }
        }
        #endregion

        #region Yapıcı Metod
        public NavigateBarCaptionDescription()
        {
            // Control
            this.Height = 20;
            this.Dock = DockStyle.Top;
            this.TabStop = false;

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);

        }
        #endregion

        #region Overrided Method
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            PaintThisControl(e.Graphics);
        }

        protected override void OnResize(EventArgs e)
        {
            Invalidate();
            base.OnResize(e);
        }
        #endregion

        #region Diğer Metodlar
        void PaintThisControl(Graphics g)
        {

            NavigateBarHelper.PaintGradientControl(this, g,
                navigateBar.NavigateBarColorTable.CaptionDescBegin,
                navigateBar.NavigateBarColorTable.CaptionDescEnd,
                navigateBar.NavigateBarColorTable.PaintAngle);

            // Gradient olarak boyama işlemi
            // Paint gradient


            // Yazıyı yazma
            // Draw caption description text

            int capWidth = (int)g.MeasureString(Caption, SystemFonts.DialogFont).Width;
            g.DrawString(Caption,
                new Font(SystemFonts.DialogFont.Name, 8, FontStyle.Regular),
                new SolidBrush(navigateBar.NavigateBarColorTable.TextColor),
                (this.NavigateBar.RightToLeft == RightToLeft.Yes ? this.Width - capWidth - 8 : 8),
                (this.Height - SystemFonts.DialogFont.Height) / 2);


            // Etrafın çizgisi
            // Draw rectangle

            g.DrawRectangle(new Pen(navigateBar.NavigateBarColorTable.BorderColor), new Rectangle(0, 0, Width - 1, Height - 1));


        }
        #endregion

    }

    #endregion
}
