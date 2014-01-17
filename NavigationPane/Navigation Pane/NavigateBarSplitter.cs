/*
 | Version 10.2
 | Copyright 2014 Esri
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
 * Description  : Splitter control for NavigateBar
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MobileControls
{
    /// <summary>
    /// Splitter control for NavigateBar
    /// </summary>
    class NavigateBarSplitter : Panel
    {

        NavigateBar navigateBar = null;
        int splitterPointCount = 5;

        public NavigateBarSplitter(NavigateBar tNavigateBar)
        {
            navigateBar = tNavigateBar;
            this.Height = 6;
            this.Width = 120;
            this.Cursor = Cursors.SizeNS;

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            this.Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            if (navigateBar == null)
                return;

            Rectangle splitRectangle = this.ClientRectangle;
            Brush darkColor = new SolidBrush(navigateBar.NavigateBarColorTable.BorderColor);
            Brush lightColor = new SolidBrush(navigateBar.NavigateBarColorTable.SeparatorLight);

            NavigateBarHelper.PaintGradientControl(this, e.Graphics,
                navigateBar.NavigateBarColorTable.SeparatorLight,
                navigateBar.NavigateBarColorTable.SeparatorDark,
                navigateBar.NavigateBarColorTable.PaintAngle);

            // Point Position
            int pointWidth = 4, pointHeight = 2;
            int firstPointPos = (splitRectangle.Width - (splitterPointCount * pointWidth + this.splitterPointCount)) / 2;
            int Y = (int)((splitRectangle.Height - 1) / 2);

            // Draw Points
            for (int i = 0; i < this.splitterPointCount; i++)
            {
                e.Graphics.FillRectangle(darkColor, firstPointPos, Y, pointHeight, pointHeight);
                e.Graphics.FillRectangle(lightColor, firstPointPos + 1, Y + 1, pointHeight, pointHeight);
                e.Graphics.FillRectangle(SystemBrushes.GrayText, firstPointPos + 1, Y, pointHeight, pointHeight);

                firstPointPos += pointWidth + 1;
            }

            splitRectangle.Width--;
            e.Graphics.DrawRectangle(new Pen(navigateBar.NavigateBarColorTable.BorderColor), splitRectangle);

        }

    }
}
