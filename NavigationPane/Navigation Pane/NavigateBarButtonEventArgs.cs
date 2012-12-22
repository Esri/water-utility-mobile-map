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
 * Description  : NavigateBarButton event args
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace MobileControls
{
    #region Class : NavigateBarButtonEventArgs

    /// <summary>
    /// NavigateBarButton EventArgs
    /// </summary>
    public sealed class NavigateBarButtonEventArgs : EventArgs
    {

        #region NavigateBarButton
        NavigateBarButton navigateBarButton;
        /// <summary>
        /// Selected NavigateBarButton
        /// </summary>
        public NavigateBarButton NavigateBarButton
        {
            get { return navigateBarButton; }
        }
        #endregion

        public NavigateBarButtonEventArgs(NavigateBarButton tNavigateBarButton)
        {
            if (tNavigateBarButton == null)
                throw new NullReferenceException("Cannot null tNavigateBarButton");

            navigateBarButton = tNavigateBarButton;
        }

    }
    #endregion
}
