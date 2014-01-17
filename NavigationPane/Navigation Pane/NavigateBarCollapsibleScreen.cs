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
 * Description  : NavigateBar collapsible screen
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MobileControls
{

    enum WindowMessages
    {
        WM_ACTIVATEAPP = 0x001C,
        WM_LBUTTONDOWN = 0x201,
        WM_RBUTTONDOWN = 0x204,
        WM_MBUTTONDOWN = 0x207,
        WM_NCLBUTTONDOWN = 0x0A1,
        WM_NCRBUTTONDOWN = 0x0A4,
        WM_NCMBUTTONDOWN = 0x0A7
    }

    #region NavigateBarCollapsibleScreen
    /// <summary>
    /// If navigatebar width equal or small minimum size then show related control in this form
    /// </summary>
    class NavigateBarCollapsibleScreen : Form
    {

        const int DISTANCE = 8;

        # region Shadow Form

        private const int CS_DROPSHADOW = 0x00020000;

        protected override CreateParams CreateParams
        {
            [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, UnmanagedCode = true)]
            get
            {
                CreateParams parameters = base.CreateParams;

                // For Shadow

                if (NavigateBarHelper.IsXPOrAbove)
                    parameters.ClassStyle = (parameters.ClassStyle | CS_DROPSHADOW);

                return parameters;
            }
        }
        #endregion

        #region  CaptionBand
        NavigateBarCaption caption;
        public NavigateBarCaption CaptionBand
        {
            get { return caption; }
        }
        #endregion

        #region IsShowWindow
        bool isShowWindow = false;
        public bool IsShowWindow
        {
            get { return isShowWindow; }
            set { isShowWindow = value; }
        }
        #endregion

        NavigateBar navigateBar = null;
        Panel panelControl = new Panel();

        #region Yapıcı Method
        public NavigateBarCollapsibleScreen(NavigateBar tNavigateBar)
        {
            navigateBar = tNavigateBar;

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            // Get positon on desktop screen
            Point p = new Point(navigateBar.Location.X + Width, navigateBar.Location.Y);
            this.DesktopLocation = this.PointToScreen(p);

            // Caption info
            caption = new NavigateBarCaption(navigateBar);
            caption.Height = 20;
            caption.CollapseButton.Visible = false;

            //

            Controls.Add(caption);
            Controls.Add(panelControl);

            //

        }

        #endregion

        #region SetControl
        /// <summary>
        /// Set showed control in collapsed screen
        /// </summary>
        /// <param name="tControl"></param>
        public void SetControl(Control tControl)
        {
            if (tControl == null)
                return;

            tControl.Dock = DockStyle.Fill;
            panelControl.Controls.Clear();
            panelControl.Controls.Add(tControl);
            tControl.Focus();

        }

        #endregion

        #region overrided Methods

        /// <summary>
        /// Kullanıcı kapatamamalı
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {

            if (e.CloseReason != CloseReason.ApplicationExitCall ||
                e.CloseReason != CloseReason.TaskManagerClosing ||
                e.CloseReason != CloseReason.WindowsShutDown)
                e.Cancel = true;
            else
                base.OnFormClosing(e);
        }

        /// <summary>
        /// Resize displayed control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            panelControl.Anchor = AnchorStyles.None;

            panelControl.Left = DISTANCE;
            panelControl.Width = this.Width - DISTANCE * 2 + 1;

            if (navigateBar.SelectedButton != null && navigateBar.SelectedButton.IsShowCollapseScreenCaption)
            {
                panelControl.Top = caption.Height + DISTANCE;
                panelControl.Height = this.Height - caption.Height - DISTANCE * 2 + 1;
            }
            else
            {
                panelControl.Top = DISTANCE;
                panelControl.Height = this.Height - DISTANCE * 2 + 1;
            }

            panelControl.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }

        /// <summary>
        /// Paint form background using colortable
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            NavigateBarHelper.PaintGradientControl(this, e.Graphics,
                navigateBar.NavigateBarColorTable.ButtonNormalBegin,
                navigateBar.NavigateBarColorTable.ButtonNormalBegin,
                navigateBar.NavigateBarColorTable.PaintAngle);

            Rectangle r = this.ClientRectangle;
            r.Width--;
            r.Height--;
            e.Graphics.DrawRectangle(new Pen(navigateBar.NavigateBarColorTable.BorderColor), r);

            r = new Rectangle(panelControl.Left - 1, panelControl.Top - 1,
                panelControl.Width + 1, panelControl.Height + 1);

            e.Graphics.DrawRectangle(new Pen(navigateBar.NavigateBarColorTable.BorderColor), r);
        }

        /// <summary>
        /// if deactivated application then close collapse screen
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (this.IsShowWindow && m.Msg == (int)WindowMessages.WM_ACTIVATEAPP)
            {
                if ((int)m.WParam == 0)
                    navigateBar.HideCollapseScreen();
            }

        }

        #endregion

    }

    #endregion

    #region NavigateBarCollapsibleScreenMessageFilter

    class NavigateBarCollapsibleScreenMessageFilter : IMessageFilter
    {

        public event EventHandler OnNonCollapsibleScreenAreaFocused;

        public bool PreFilterMessage(ref Message m)
        {
            switch (m.Msg)
            {
                case (int)WindowMessages.WM_LBUTTONDOWN:
                case (int)WindowMessages.WM_RBUTTONDOWN:
                case (int)WindowMessages.WM_MBUTTONDOWN:
                case (int)WindowMessages.WM_NCLBUTTONDOWN:
                case (int)WindowMessages.WM_NCRBUTTONDOWN:
                case (int)WindowMessages.WM_NCMBUTTONDOWN:
                    {
                        if (OnNonCollapsibleScreenAreaFocused != null)
                            OnNonCollapsibleScreenAreaFocused(this, EventArgs.Empty);
                        break;
                    }
            }
            return false;
        }

    }
    #endregion

}