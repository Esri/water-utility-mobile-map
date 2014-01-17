﻿/*
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
 * Description  : Context menu item for OverFlowPanel
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MobileControls
{
    /// <summary>
    /// ContextMenu item for Overflowpanel
    /// </summary>
    class NavigateBarOverFlowPanelMenuItem : ToolStripMenuItem
    {

        #region NavigateBarButton
        readonly NavigateBarButton navigateBarButton = null;
        public NavigateBarButton NavigateBarButton
        {
            get { return navigateBarButton; }
        }
        #endregion

        #region Yapıcı Method

        public NavigateBarOverFlowPanelMenuItem(NavigateBarButton tNavigateBarButton, bool tCheckMenu)
        {
            navigateBarButton = tNavigateBarButton;

            if (navigateBarButton == null)
                return;

            this.Text = navigateBarButton.Caption;
            this.Image = navigateBarButton.Image;

            if (tCheckMenu)
            {
                this.CheckOnClick = true;
                this.CheckState = tNavigateBarButton.IsDisplayed ? CheckState.Checked : CheckState.Unchecked;
            }

        }
        #endregion

    }
}
