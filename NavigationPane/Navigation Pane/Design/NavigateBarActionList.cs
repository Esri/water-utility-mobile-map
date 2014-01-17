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
 * Description  : Navigation Pane desinger commands
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using System.ComponentModel;

using MobileControls;
using System.Windows.Forms;

namespace MobileControls.Design
{

    enum ColorTables
    {
        SystemColor,
        Office2003Blue,
        Office2003Olive,
        Office2003Silver,
        Office2007Blue,
        Office2007Black,
        Office2007Silver
    }


    class NavigateBarActionList : DesignerActionList
    {

        DesignerActionUIService actionUISvc;
        IDesignerHost designerHost;

        #region NavigateBar Color Table

        ColorTables colorTable = ColorTables.SystemColor;

        public ColorTables ColorTable
        {
            get { return colorTable; }
            set
            {
                switch (value)
                {
                    case ColorTables.SystemColor:
                        SetProperty("NavigateBarColorTable", NavigateBarColorTable.SystemColor);
                        break;
                    case ColorTables.Office2003Blue:
                        SetProperty("NavigateBarColorTable", NavigateBarColorTable.Office2003Blue);
                        break;
                    case ColorTables.Office2003Olive:
                        SetProperty("NavigateBarColorTable", NavigateBarColorTable.Office2003Olive);
                        break;
                    case ColorTables.Office2003Silver:
                        SetProperty("NavigateBarColorTable", NavigateBarColorTable.Office2003Silver);
                        break;
                    case ColorTables.Office2007Black:
                        SetProperty("NavigateBarColorTable", NavigateBarColorTable.Office2007Black);
                        break;
                    case ColorTables.Office2007Blue:
                        SetProperty("NavigateBarColorTable", NavigateBarColorTable.Office2007Blue);
                        break;
                    case ColorTables.Office2007Silver:
                        SetProperty("NavigateBarColorTable", NavigateBarColorTable.Office2007Silver);
                        break;
                }

                colorTable = value;

            }
        }

        private void ResetColorTable()
        {
            this.ColorTable = ColorTables.SystemColor;
        }

        #endregion

        #region RightToLeft
        public RightToLeft RightToLeft
        {
            get { return ((NavigateBar)this.Component).RightToLeft; }
            set { SetProperty("RightToLeft", value); }
        }
        #endregion

        #region Dock
        public DockStyle Dock
        {
            get { return ((NavigateBar)this.Component).Dock; }
            set { SetProperty("Dock", value); }
        }
        #endregion

        #region Constructor
        public NavigateBarActionList(IComponent component)
            : base(component)
        {

            if (!(component is NavigateBar))
            {
                throw new System.ArgumentException("The NavigateBarActionList only defines a list of smart tag items for controls of type, or controls that derive from type, NavigateBar.", "component");
            }

            this.actionUISvc = GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
            this.designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));

            // Set Designer Item Collection

            SetDesignerActionItemObjects();

        }

        #endregion

        #region DesignerActionItemCollection

        const string APPEARANCE = "Appearance";
        const string BUTTONS = "Buttons";

        DesignerActionItemCollection actionItemCollection;

        void SetDesignerActionItemObjects()
        {

            actionItemCollection = new DesignerActionItemCollection();
            // Görünüm
            actionItemCollection.Add(new DesignerActionHeaderItem(APPEARANCE));
            actionItemCollection.Add(new DesignerActionTextItem("Properties that are related to the controls appearance.", APPEARANCE));
            actionItemCollection.Add(new DesignerActionPropertyItem("ColorTable", "Select Color Table", APPEARANCE, "Change color table navigation pane."));
            actionItemCollection.Add(new DesignerActionMethodItem(this, "ResetColorTable", "Reset Color Table", APPEARANCE, "Return system colors."));
            actionItemCollection.Add(new DesignerActionPropertyItem("RightToLeft", "Right To Left", APPEARANCE, "Change RightToLeft by the control."));
            actionItemCollection.Add(new DesignerActionPropertyItem("Dock", "Dock", APPEARANCE, "Change Dock by the control."));

            // Buttons
            actionItemCollection.Add(new DesignerActionHeaderItem(BUTTONS));
            actionItemCollection.Add(new DesignerActionTextItem("Add a new button or remove a exist button from navigation pane.", BUTTONS));
            actionItemCollection.Add(new DesignerActionMethodItem(this, "AddButton", "Add Button", BUTTONS, "Add a new button.", true));
            actionItemCollection.Add(new DesignerActionMethodItem(this, "RemoveButton", "Remove Button", BUTTONS, "Remove last button.", true));
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            return actionItemCollection;
        }

        #endregion

        #region ReFreshUI
        void ReFreshUI()
        {
            if (this.actionUISvc != null)
                this.actionUISvc.Refresh(this.Component);
        }
        #endregion

        #region SetProperty
        /// <summary>
        /// Componentin özelliğini değiştirmek için
        /// </summary>
        /// <param name="tPropertyName">Özelliğin adı</param>
        /// <param name="tPropertyValue">Yeni değeri</param>
        void SetProperty(string tPropertyName, object tPropertyValue)
        {
            PropertyDescriptor propDescriptor = TypeDescriptor.GetProperties(this.Component)[tPropertyName];
            propDescriptor.SetValue(this.Component, tPropertyValue);

            ReFreshUI();
        }
        #endregion

        #region AddButton

        /// <summary>
        /// Add a new button
        /// </summary>
        void AddButton()
        {
            if (this.designerHost == null)
                return;

            if (!(this.Component is NavigateBar))
                throw new Exception("Component is not NavigateBar or derived");

            try
            {
                NavigateBar navigationPane = (NavigateBar)this.Component;

                // Yeni button
                NavigateBarButton nvButton = (NavigateBarButton)this.designerHost.CreateComponent(typeof(NavigateBarButton));

                if (nvButton == null)
                {
                    System.Windows.Forms.MessageBox.Show("Component is null");
                    return;
                }
                nvButton.Caption = nvButton.Name;
                navigationPane.NavigateBarButtons.Add(nvButton);

                navigationPane.NavigateBarDisplayedButtonCount = navigationPane.NavigateBarButtons.Count;

                navigationPane.PerformNavigationPane(); // Panelin görünümünü yenile

                ReFreshUI();

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "AddButton");
            }
        }

        #endregion

        #region RemoveButton
        /// <summary>
        /// Remove button from collection
        /// </summary>
        void RemoveButton()
        {

            if (this.designerHost == null)
                return;

            if (!(this.Component is NavigateBar))
                throw new Exception("Component is not NavigateBar or derived");

            try
            {
                NavigateBar navigationPane = (NavigateBar)this.Component;


                if (navigationPane.NavigateBarButtons.Count <= 0)
                    return;

                // Get last button in collection

                NavigateBarButton nvButton = navigationPane.NavigateBarButtons[navigationPane.NavigateBarButtons.Count - 1];

                if (nvButton == null)
                    return;

                navigationPane.NavigateBarButtons.Remove(nvButton);

                if (navigationPane.NavigateBarDisplayedButtonCount > 0)
                {
                    navigationPane.NavigateBarDisplayedButtonCount--;
                    if (navigationPane.DisplayedButtonCount > navigationPane.NavigateBarDisplayedButtonCount)
                        navigationPane.DisplayedButtonCount = navigationPane.NavigateBarDisplayedButtonCount;
                }

                if (navigationPane.NavigateBarButtons.Count == 0)
                    navigationPane.SetCaptionText(null);

                navigationPane.PerformNavigationPane(); // Panelin görünümünü yenile

                this.designerHost.DestroyComponent(nvButton); // buttonu kaldır

                ReFreshUI();

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "RemoveButton");
            }
        }
        #endregion

    }
}
