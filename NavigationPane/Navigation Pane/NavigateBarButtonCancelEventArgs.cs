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
 * Description  : NavigateBarButton cancel event args
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace MobileControls
{
    #region Class : NavigateBarButtonEventArgs
    /// <summary>
    /// CancelEventArgs for NavigateBar.OnNavigateBarButtonSelecting event
    /// </summary>
    public class NavigateBarButtonCancelEventArgs : CancelEventArgs
    {

        #region Selected
        NavigateBarButton selected;
        /// <summary>
        /// New Selected NavigateBarButton
        /// <para>get</para>
        /// </summary>
        public NavigateBarButton Selected
        {
            get { return selected; }
        }
        #endregion

        #region PreviousSelected
        NavigateBarButton previousSelected;
        /// <summary>
        /// Previous Selected NavigateBarButton
        /// <para>Get</para>
        /// </summary>
        public NavigateBarButton PreviousSelected
        {
            get { return previousSelected; }
        }
        #endregion

        public NavigateBarButtonCancelEventArgs(NavigateBarButton tSelected, NavigateBarButton tPreviousSelected)
        {
            selected = tSelected;
            previousSelected = tPreviousSelected;
            this.Cancel = false;
        }
    }
    #endregion
}
