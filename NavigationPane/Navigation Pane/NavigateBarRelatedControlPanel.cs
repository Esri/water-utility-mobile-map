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
 * Description  : NavigateBar related control container panel
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

using MobileControls.Design;

namespace MobileControls
{
    /// <summary>
    /// Show related control on this panel
    /// </summary>
    class NavigateBarControlPanel : UserControl
    {

        #region NavigateBar
        NavigateBar navigateBar = null;
        public NavigateBar NavigateBar
        {
            get { return navigateBar; }
            set
            {
                navigateBar = value;
                Invalidate();
            }
        }
        #endregion

        public NavigateBarControlPanel()
        {

            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw, true);
        }

        #region Overrided Methodlar

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (this.Controls.Count == 0 && !this.DesignMode)
            {
                base.OnPaintBackground(e);

                NavigateBarHelper.PaintGradientControl(this, e.Graphics,
                    navigateBar.NavigateBarColorTable.ButtonNormalBegin,
                    navigateBar.NavigateBarColorTable.ButtonNormalEnd,
                    navigateBar.NavigateBarColorTable.PaintAngle);
            }
        }
        #endregion
    }
}
