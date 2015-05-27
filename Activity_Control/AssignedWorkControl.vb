' | Version 10.2
' | Copyright 2014 Esri
' |
' | Licensed under the Apache License, Version 2.0 (the "License");
' | you may not use this file except in compliance with the License.
' | You may obtain a copy of the License at
' |
' |    http://www.apache.org/licenses/LICENSE-2.0
' |
' | Unless required by applicable law or agreed to in writing, software
' | distributed under the License is distributed on an "AS IS" BASIS,
' | WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' | See the License for the specific language governing permissions and
' | limitations under the License.


Imports System.Drawing
Imports System.Windows.Forms
Imports MobileControls
Imports Esri.ArcGISTemplates
Imports Esri.ArcGIS.Mobile
Imports Esri.ArcGIS.Mobile.Geometries
Imports Esri.ArcGIS.Mobile.SpatialReferences
Imports Esri.ArcGIS.Mobile.FeatureCaching

Imports Esri.ArcGIS.Mobile.CatalogServices


Public Class AssignedWorkControl

    Private m_penLine As Pen = Pens.Cyan
    Private m_pen As Pen = New Pen(Color.Purple, 10)
    Private m_brush As SolidBrush = New SolidBrush(Color.Cyan)
    Private Class MyListViewItem
        Inherits ListViewItem
        Private m_geo As Geometry
        Private m_FID As Integer
        Private m_FDR As FeatureDataRow
        Private m_ID As String

        Public Sub New() 'ByVal Geo As IGeometry, ByVal FID As Integer)
            ' m_geo = Geo
            ' m_FID = FID


        End Sub

        Public Property FID() As Integer
            Get
                Return Me.m_FID
            End Get
            Set(ByVal value As Integer)
                Me.m_FID = value
            End Set
        End Property
        Public Property ID() As String
            Get
                Return Me.m_ID
            End Get
            Set(ByVal value As String)
                Me.m_ID = value
            End Set
        End Property
        Public Property FeatureDataRow() As FeatureDataRow
            Get
                Return Me.m_FDR
            End Get
            Set(ByVal value As FeatureDataRow)
                Me.m_FDR = value
            End Set
        End Property

        Public Property Geometry() As Geometry
            Get
                Return Me.m_geo
            End Get
            Set(ByVal value As Geometry)
                Me.m_geo = value
            End Set
        End Property
        Public Overrides Function ToString() As String
            Return FID.ToString


        End Function
    End Class
#Region "Var"
    ' NavigateBar
    Public m_BufferDivideValue As Double = 15
    Private m_CurrentWOID As String = ""
    Public Event RaiseMessage(ByVal Message As String)
    Public Event RaisePermMessage(ByVal Message As String, ByVal Hide As Boolean)
    Public Event WOChanged(WOOID As String, WOCrew As String, WODisplayText As String)
    Private m_OutlookNavigatePane As NavigateBar
    Private m_NVBButton As List(Of NavigateBarButton)
    Private splitterNavigateMenu As MTSplitter = Nothing
    Private m_WOFSwD As FeatureSourceWithDef
    Private m_AttInfoDisplay As AttributeDisplay
    Private WithEvents m_EdtClose As EditControl
    Private WithEvents m_EdtCreate As EditControl
    Private m_EditOp As MobileConfigClass.MobileConfigMobileMapConfigEditControlOptionsLayersLayer = Nothing


    Private WithEvents m_Map As Esri.ArcGIS.Mobile.WinForms.Map


#End Region
    Public m_AssignedTo As String = ""
   
    Public Sub New(ByVal Map As Esri.ArcGIS.Mobile.WinForms.Map)

        ' This call is required by the designer.
        InitializeComponent()
        Try
            m_Map = Map

            m_AssignedTo = GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.AssignedTo

            ' Add any initialization after the InitializeComponent() call.
            'gpBoxWOList.Text = Esri.ArcGISTemplates.GlobalsFunctions.appConfig.WorkorderPanel.UIComponents.ViewWOLabel
            gpBoxWOList.Text = ""
            gpBoxWOList.Visible = True
            gpBoxWOList.Dock = Windows.Forms.DockStyle.Fill
            btnViewAllWork.Checked = True

            'btnViewAllWork.BackgroundImage = My.Resources.ClipboardRed

            'gpBoxWOClose.Text = Esri.ArcGISTemplates.GlobalsFunctions.appConfig.WorkorderPanel.UIComponents.CloseWOLabel
            gpBoxWOClose.Text = ""
            gpBoxWOClose.Visible = False
            gpBoxWOClose.Dock = Windows.Forms.DockStyle.Fill

            'gpBoxWOCreate.Text = Esri.ArcGISTemplates.GlobalsFunctions.appConfig.WorkorderPanel.UIComponents.CreateWOLabel
            gpBoxWOCreate.Text = ""

            gpBoxWOCreate.Visible = False
            gpBoxWOCreate.Dock = Windows.Forms.DockStyle.Fill

            'gpBoxWODetails.Text = Esri.ArcGISTemplates.GlobalsFunctions.appConfig.WorkorderPanel.UIComponents.DetailedViewWOLabel
            gpBoxWODetails.Text = ""
            gpBoxWODetails.Visible = False
            gpBoxWODetails.Dock = Windows.Forms.DockStyle.Fill




            m_WOFSwD = GlobalsFunctions.GetFeatureSource(GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.LayerName, m_Map)
            If GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.LayerName IsNot Nothing Then
                If Len(GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.LayerName) > 0 Then
                    If m_WOFSwD.FeatureSource Is Nothing Then
                        MsgBox(GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.LayerName & " layer not found, check the name, activity control will not function")
                    End If
                End If
            End If
            If m_WOFSwD.FeatureSource Is Nothing Then Return




            AddHandler m_WOFSwD.FeatureSource.DataChanged, AddressOf layer_DataChanged


            If m_WOFSwD.FeatureSource.Columns(GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.AssignedToField) Is Nothing Then Return

            'If GlobalsFunctions.appConfig.WorkorderPanel.WorkOrderFilters.ShowFilterGeo.ToUpper() = "TRUE" Then
            '    btnFiltGeo.Visible = True
            '    If GlobalsFunctions.appConfig.WorkorderPanel.WorkOrderFilters.FilterOn.ToUpper() = "TRUE" Then
            '        btnFiltGeo.Checked = True
            '    Else
            '        btnFiltGeo.Checked = False

            '    End If
            'Else
            '    btnFiltGeo.Visible = False
            'End If


            btnFiltGeo.Visible = False
            InitNavigateBar()
            InitNavigateButtons()

            AddPanels()
            LoadWorkOrders()

            relocateButtons()





            For Each createLay In GlobalsFunctions.appConfig.EditControlOptions.Layers.Layer
                If GlobalsFunctions.LayerNameMatches(m_WOFSwD.FeatureSource, createLay.Name) Then
                    m_EditOp = createLay
                    Exit For


                End If
            Next
            '   m_EdtCreate.setCurrentLayer(m_WOFeatLayer, m_EditOp)
            Try
                m_EdtCreate.setCurrentRecord(m_WOFSwD.FeatureSource.GetDataTable().NewRow, m_EditOp)

            Catch ex As Exception
                Dim st As New StackTrace
                MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & m_WOFSwD.FeatureSource.Name & " - " & ex.Message)
                st = Nothing
            End Try
            m_EdtClose.Visible = False
            m_AttInfoDisplay.Visible = False

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try

    End Sub

    Private Sub layer_DataChanged(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.DataChangedEventArgs)
        Try


            'Monitors data change events to a layer.  This handler is removed during sync as it is called when data is being created from the server

            'If called from a different thread, call the parent thread
            If InvokeRequired Then
                Try
                    Invoke(New EventHandler(AddressOf layer_DataChanged), sender, e)

                Catch ex As Exception

                End Try
                Return

            End If

            LoadWorkOrders(True, False)
            Call btnViewAllWork_Click(Nothing, Nothing)

            m_OutlookNavigatePane.SelectedButton.PerformClick()
            outlookNavigatePane_OnNavigateBarButtonSelected(m_OutlookNavigatePane.SelectedButton)
            m_Map.Refresh()


        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub

    Private Sub AddPanels()

        m_AttInfoDisplay = New AttributeDisplay(m_Map, GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.LayerName, False)
        gpBoxWODetails.Controls.Add(m_AttInfoDisplay)
        m_AttInfoDisplay.Dock = DockStyle.Fill
        m_AttInfoDisplay.Mode = "WO_ID"
        m_AttInfoDisplay.toggleGPSButton = False
        m_AttInfoDisplay.toggleLayerLabel = False



        m_EdtClose = New EditControl(m_Map, Nothing)
        gpBoxWOClose.Controls.Add(m_EdtClose)
        m_EdtClose.Dock = DockStyle.Fill
        m_EdtClose.Mode = "WO_Close"
        m_EdtClose.MoveGeoButtonVisible = False
        m_EdtClose.NewRecordAfterSave = False

        m_EdtClose.GPSButtonVisible = False
        m_EdtClose.ClearButtonVisible = False

        'm_EdtClose.toggleGPSButton = False
        'm_EdtClose.toggleLayerLabel = False

        m_EdtCreate = New EditControl(m_Map, Nothing)
        gpBoxWOCreate.Controls.Add(m_EdtCreate)
        m_EdtCreate.Dock = DockStyle.Fill
        m_EdtCreate.Mode = "WO_Create"






    End Sub
    Private Sub InitNavigateBar()
        Try

            m_OutlookNavigatePane = New NavigateBar()
            m_OutlookNavigatePane.SaveAndRestoreSettings = False

            m_OutlookNavigatePane.Dock = DockStyle.Left
            m_OutlookNavigatePane.IsShowCollapsibleScreen = True
            m_OutlookNavigatePane.IsCollapseScreenShowOnButtonSelect = False
            m_OutlookNavigatePane.IsCollapsible = False
            m_OutlookNavigatePane.IsShowCollapseButton = False
            m_OutlookNavigatePane.IsShowCollapsibleScreen = False
            m_OutlookNavigatePane.IsNavButtonSplitterVisible = False

            Try


                If (GlobalsFunctions.IsNumeric(GlobalsFunctions.appConfig.WorkorderPanel.WorkOrderFilters.ButtonSize)) Then

                    m_OutlookNavigatePane.NavigateBarButtonHeight = CInt(GlobalsFunctions.appConfig.WorkorderPanel.WorkOrderFilters.ButtonSize)
                Else
                    m_OutlookNavigatePane.NavigateBarButtonHeight = 40
                End If
            Catch ex As Exception
                m_OutlookNavigatePane.NavigateBarButtonHeight = 40
            End Try




            m_OutlookNavigatePane.IsOverFlowPanelVisible = False

            AddHandler m_OutlookNavigatePane.OnNavigateBarDisplayedButtonCountChanged, AddressOf NavigationPane_OnNavigateBarDisplayedButtonCountChanged



            AddHandler m_OutlookNavigatePane.OnNavigateBarButtonSelected, AddressOf outlookNavigatePane_OnNavigateBarButtonSelected
            'AddHandler m_OutlookNavigatePane.OnNavigateBarButtonSelecting, AddressOf outlookNavigatePane_OnNavigateBarButtonSelecting


            AddHandler m_OutlookNavigatePane.OnNavigateBarColorChanged, AddressOf outlookNavigatePane_OnNavigateBarColorChanged
            AddHandler m_OutlookNavigatePane.HandleCreated, AddressOf outlookNavigatePane_HandleCreated
            'm_OutlookNavigatePane.Width = 150
            Try


                If (GlobalsFunctions.IsNumeric(GlobalsFunctions.appConfig.WorkorderPanel.WorkOrderFilters.ListFontSize)) Then

                    m_OutlookNavigatePane.Font = New Font("Tahoma", CInt(GlobalsFunctions.appConfig.WorkorderPanel.WorkOrderFilters.ListFontSize), FontStyle.Bold)
                Else
                    m_OutlookNavigatePane.Font = New Font("Tahoma", 14, FontStyle.Bold)
                End If
            Catch ex As Exception
                m_OutlookNavigatePane.Font = New Font("Tahoma", 14, FontStyle.Bold)
            End Try

            ' Splitter

            splitterNavigateMenu = New MTSplitter()
            splitterNavigateMenu.Size = New Size(7, 100)
            splitterNavigateMenu.SplitterPointCount = 10
            splitterNavigateMenu.SplitterPaintAngle = 360.0F
            splitterNavigateMenu.Dock = DockStyle.Left

            ' Navigatebar Remote Control
            m_OutlookNavigatePane.IsCollapsible = False


            splitterNavigateMenu.Visible = False
            m_OutlookNavigatePane.Dock = DockStyle.Fill
            gpBoxWOList.Controls.AddRange(New Control() {splitterNavigateMenu, m_OutlookNavigatePane})
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try

    End Sub
  
    Private Sub InitNavigateButtons()
        Dim nvbBtn As NavigateBarButton
        Dim pLst As ListView

        Dim woFilt As Esri.ArcGISTemplates.MobileConfigClass.MobileConfigMobileMapConfigWorkorderPanelWorkOrderFiltersWorkOrderFilter

        Try
            If m_OutlookNavigatePane Is Nothing Then Return


            If GlobalsFunctions.appConfig.WorkorderPanel IsNot Nothing Then
                If GlobalsFunctions.appConfig.WorkorderPanel.WorkOrderFilters IsNot Nothing Then
                    If GlobalsFunctions.appConfig.WorkorderPanel.WorkOrderFilters.WorkOrderFilter IsNot Nothing Then
                        If GlobalsFunctions.appConfig.WorkorderPanel.WorkOrderFilters.WorkOrderFilter.Count > 0 Then
                            m_NVBButton = New List(Of NavigateBarButton)
                            For Each woFilt In GlobalsFunctions.appConfig.WorkorderPanel.WorkOrderFilters.WorkOrderFilter

                                nvbBtn = New NavigateBarButton()
                                pLst = New ListView
                                pLst.HideSelection = False
                                pLst.FullRowSelect = True

                                AddHandler pLst.SelectedIndexChanged, AddressOf ListView_SelectedIndexChanged

                                AddHandler pLst.Resize, AddressOf resizeLst
                                AddHandler pLst.MouseDoubleClick, AddressOf ListView_MouseDoubleClick
                                pLst.View = View.Details
                                pLst.Columns.Add("")
                                'pLst.Columns.Add("FID")
                                'pLst.Columns("FID").Width = 0

                                pLst.GridLines = True
                                pLst.HeaderStyle = ColumnHeaderStyle.None
                                pLst.SmallImageList = ImageList1



                                Try
                                    If (GlobalsFunctions.IsNumeric(GlobalsFunctions.appConfig.WorkorderPanel.WorkOrderFilters.ListFontSize)) Then
                                        pLst.Font = New Font("Tahoma", CInt(GlobalsFunctions.appConfig.WorkorderPanel.WorkOrderFilters.ListFontSize), FontStyle.Bold)
                                    Else
                                        pLst.Font = New Font("Tahoma", 12, FontStyle.Bold)
                                    End If
                                Catch ex As Exception
                                    pLst.Font = New Font("Tahoma", 12, FontStyle.Bold)
                                End Try

                                nvbBtn.RelatedControl = pLst
                                'nvbBtn.RelatedControl = New DataGridView()
                                'AddHandler nvbBtn.RelatedControl.VisibleChanged, AddressOf testEvent

                                nvbBtn.Caption = woFilt.Label
                                'nvbBtn.CaptionDescription = "High Priority Workorders"
                                'nvbHigh.Image = Properties.Resources.Mail24
                                nvbBtn.Enabled = True
                                nvbBtn.Key = woFilt.Label
                                nvbBtn.IsShowCaptionImage = False
                                nvbBtn.IsShowCaption = True
                                nvbBtn.IsShowCaptionDescription = False

                                Try


                                    If (GlobalsFunctions.IsNumeric(GlobalsFunctions.appConfig.WorkorderPanel.WorkOrderFilters.ButtonFontSize)) Then

                                        nvbBtn.Font = New Font("Tahoma", CInt(GlobalsFunctions.appConfig.WorkorderPanel.WorkOrderFilters.ButtonFontSize), FontStyle.Bold)
                                    Else
                                        nvbBtn.Font = New Font("Tahoma", 16, FontStyle.Italic Or FontStyle.Bold)
                                    End If
                                Catch ex As Exception
                                    nvbBtn.Font = New Font("Tahoma", 16, FontStyle.Italic Or FontStyle.Bold)
                                End Try



                                nvbBtn.Tag = woFilt
                                AddHandler nvbBtn.OnNavigateBarButtonSelected, AddressOf nvbMail_OnNavigateBarButtonSelected
                                m_NVBButton.Add(nvbBtn)

                            Next



                        End If

                    End If

                End If
            End If

            If m_NVBButton IsNot Nothing Then
                m_OutlookNavigatePane.NavigateBarButtons.AddRange(m_NVBButton.ToArray)


            End If

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        Finally
            nvbBtn = Nothing
            pLst = Nothing

            woFilt = Nothing

        End Try


    End Sub
    Private Function workorderSQL(ByVal woFilt As Esri.ArcGISTemplates.MobileConfigClass.MobileConfigMobileMapConfigWorkorderPanelWorkOrderFiltersWorkOrderFilter) As String

        Dim pCrewDC As DataColumn = Nothing
        Try


            pCrewDC = m_WOFSwD.FeatureSource.Columns(GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.AssignedToField)
            Dim strSql As String = ""
            If GlobalsFunctions.appConfig.WorkorderPanel.UIComponents.AllWOText = m_AssignedTo Then
            Else

                If pCrewDC.DataType Is System.Type.GetType("System.String") Then
                    Dim newValue As String = m_AssignedTo.Replace("'", "''")
                    strSql = GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.AssignedToField & " = '" & newValue & "'"
                Else
                    strSql = GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.AssignedToField & " = " & m_AssignedTo
                End If
            End If

            If strSql = "" Then
                strSql = woFilt.QueryValue.ToString()
            Else
                strSql = strSql & " AND " & woFilt.QueryValue.ToString()
            End If
            Return strSql

        Catch ex As Exception
            Return ""
        End Try

    End Function
    Public Sub LoadWorkOrders(Optional reset As Boolean = True, Optional zoomTo As Boolean = True)
        If CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView) Is Nothing Then Return

        Dim fid As String = "NAN"
        If CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).SelectedItems().Count = 1 Then
            fid = CType(CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).SelectedItems.Item(0), MyListViewItem).FID.ToString


        End If



        Dim filtByExtent As Boolean = btnFiltGeo.Checked

        Dim woFilt As ESRI.ArcGISTemplates.MobileConfigClass.MobileConfigMobileMapConfigWorkorderPanelWorkOrderFiltersWorkOrderFilter = Nothing
        Dim pCrewDC As DataColumn = Nothing
        Dim pQF As QueryFilter = Nothing
        Dim pDt As FeatureDataTable = Nothing
        Dim strLayFilt As String = ""
        Dim strSql As String

        Try
            Dim WOExt As ESRI.ArcGIS.Mobile.Geometries.Envelope = New ESRI.ArcGIS.Mobile.Geometries.Envelope

            For Each nvBtn As NavigateBarButton In m_NVBButton
                woFilt = nvBtn.Tag

                strSql = workorderSQL(woFilt)


                pQF = New QueryFilter(strSql)
                If filtByExtent Then
                    pQF.Geometry = m_Map.Extent
                    pQF.GeometricRelationship = GeometricRelationshipType.Intersect

                End If
                Dim featCnt As Integer = m_WOFSwD.FeatureSource.GetFeatureCount(pQF)

                If featCnt > 0 Then
                    If strLayFilt = "" Then
                        strLayFilt = strSql

                    End If
                    Dim strFldArr As New List(Of String)

                    strFldArr.Add(m_WOFSwD.FeatureSource.GeometryColumnName)

                    If woFilt.Fields IsNot Nothing Then
                        If woFilt.Fields.Field IsNot Nothing Then
                            If woFilt.Fields.Field.Count > 0 Then
                                For Each fld As ESRI.ArcGISTemplates.MobileConfigClass.MobileConfigMobileMapConfigWorkorderPanelWorkOrderFiltersWorkOrderFilterFieldsField In woFilt.Fields.Field
                                    If m_WOFSwD.FeatureSource.Columns(fld.Name) IsNot Nothing Then
                                        strFldArr.Add(fld.Name)


                                    End If
                                Next
                            Else
                                Continue For
                            End If
                        Else
                            Continue For
                        End If
                    Else
                        Continue For
                    End If


                    pDt = m_WOFSwD.FeatureSource.GetDataTable(pQF)

                    Dim pDRs() As DataRow

                    If woFilt.SortField <> "" Then
                        Dim sortFlds() As String = woFilt.SortField.Split(",")
                        Dim sorting As String = ""

                        For Each sortFld As String In sortFlds
                            If m_WOFSwD.FeatureSource.Columns(sortFld) IsNot Nothing Then
                                If sorting = "" Then
                                    sorting = sortFld
                                Else
                                    sorting = sorting + "," + sortFld
                                End If

                            End If
                        Next
                        If sorting <> "" Then
                            If woFilt.SortDirection <> "DESC" And woFilt.SortDirection <> "ASC" Then
                                woFilt.SortDirection = "ASC"
                            End If

                            pDRs = pDt.Select("1=1", sorting & " " & woFilt.SortDirection)

                        Else
                            pDRs = pDt.Select("1=1")

                        End If
                    Else
                        pDRs = pDt.Select("1=1")

                    End If

                    WOExt.Union(LoadWOListControlListView(nvBtn.RelatedControl, pDRs, strFldArr, woFilt.Fields.JoinString))


                    nvBtn.Caption = woFilt.Label & ": " & featCnt
                    ' nvBtn.PerformClick()
                Else
                    nvBtn.Caption = woFilt.Label

                    clearDG(nvBtn.RelatedControl)

                End If

            Next
            If fid <> "NAN" Then

                For Each itm As MyListViewItem In CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).Items
                    If itm.FeatureDataRow.Fid.ToString = fid Then
                        RemoveHandler CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).SelectedIndexChanged, AddressOf ListView_SelectedIndexChanged


                        m_OutlookNavigatePane.SelectedButton.PerformClick()
                        CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).EnsureVisible(itm.Index)
                        itm.Selected = True
                        itm.Focused = True
                        AddHandler CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).SelectedIndexChanged, AddressOf ListView_SelectedIndexChanged
                        CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).Focus()
                        Dim pCoord As ESRI.ArcGIS.Mobile.Geometries.Coordinate = GlobalsFunctions.GetGeometryCenter(itm.FeatureDataRow.Geometry)

                        Exit For



                    End If
                Next
            End If

            If reset = False Then
                Return

            End If
            Dim pFSInfo As MobileCacheMapLayerDefinition = CType(m_Map.MapLayers(m_WOFSwD.MapLayerIndex), MobileCacheMapLayer).LayerDefinitions(m_WOFSwD.LayerIndex)

            pFSInfo.DisplayExpression = strLayFilt
            pFSInfo.Visibility = True
            pFSInfo = Nothing

            btnCrew.Text = m_AssignedTo

            If m_EdtClose IsNot Nothing Then
                m_EdtClose.setCurrentRecord(Nothing, Nothing)
            End If

            If m_AttInfoDisplay IsNot Nothing Then

                m_AttInfoDisplay.IdentifyLocation(CType(Nothing, Geometry))
            End If
            lblCurrentWO.Text = ""
            RaiseEvent RaisePermMessage("", True)

            m_CurrentWOID = ""

            'lblCrewName.Text = GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.AssignedTo

            If GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.ZoomOnLoad.ToUpper = "TRUE" And zoomTo = True Then
                m_Map.Extent = WOExt
            End If


        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & GlobalsFunctions.appConfig.WorkorderPanel.UIComponents.InvalidSqlMessage)
            st = Nothing

        End Try


    End Sub
    Public Function GetCrewNames() As DataTable


        Dim pDt As FeatureDataTable = Nothing

        Try
            Dim pList As List(Of String) = New List(Of String)

            pDt = m_WOFSwD.FeatureSource.GetDataTable(Nothing)
            Dim pRT As DataTable

            pRT = pDt.DefaultView.ToTable(True, GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.AssignedToField)

            Return pRT


        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & GlobalsFunctions.appConfig.WorkorderPanel.UIComponents.InvalidSqlMessage)
            st = Nothing

        End Try


    End Function

    Private Sub resizeLst(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Dim lstView As ListView = sender
        RemoveHandler lstView.Resize, AddressOf resizeLst

        'For Each ch As ColumnHeader In lstView.Columns
        '    ch.Width = -2
        'Next
        If lstView.Columns.Count > 0 Then
                        lstView.Columns(0).Width = lstView.Width - ImageList1.ImageSize.Width - 18
            'If lstView.Parent IsNot Nothing Then
            '    If lstView.Parent.Parent IsNot Nothing Then
            '        If lstView.Parent.Parent.Parent IsNot Nothing Then
            '            '                        lstView.Columns(0).Width = lstView.Parent.Parent.Parent.Width - ImageList1.ImageSize.Width

            '        End If
            '    End If
            'End If

        End If
        AddHandler lstView.Resize, AddressOf resizeLst

    End Sub
    Private Sub LoadWOListControlListView(ByVal LstView As ListView, ByVal DataTable As FeatureDataTable, ByVal FieldsList As List(Of String), ByVal JoinString As String)
        Try

            Dim pLstViewItm As MyListViewItem

            Dim strDis As String = ""
            LstView.Items.Clear()
            For Each pDr As FeatureDataRow In DataTable.Rows
                pLstViewItm = New MyListViewItem

                strDis = ""

                For i As Integer = 0 To FieldsList.Count - 1
                    If FieldsList(i) <> pDr.FeatureSource.GeometryColumnName Then

                        If strDis = "" Then
                            strDis = pDr(DataTable.Columns(FieldsList(i))).ToString()

                            'pLstViewItm.Text = (pDr(i).ToString)
                        Else
                            'pLstViewItm.SubItems.Add(pDr(i).ToString)
                            strDis = strDis & JoinString & pDr(DataTable.Columns(FieldsList(i))).ToString()

                        End If
                    End If


                Next
                If strDis = "" Then
                    strDis = GlobalsFunctions.appConfig.WorkorderPanel.UIComponents.NoDisplayText

                End If
                pLstViewItm.Text = strDis
                pLstViewItm.Geometry = pDr.Geometry
                pLstViewItm.FID = pDr.Fid
                If pDr.FeatureSource.Columns(GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.IDField) IsNot Nothing Then
                    If pDr(GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.IDField) Is Nothing Then
                        pLstViewItm.ID = GlobalsFunctions.appConfig.WorkorderPanel.UIComponents.FieldCreateWOText & pDr.Fid.ToString()
                    ElseIf pDr(GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.IDField) Is DBNull.Value Then


                        pLstViewItm.ID = GlobalsFunctions.appConfig.WorkorderPanel.UIComponents.FieldCreateWOText & pDr.Fid.ToString()
                    Else
                        pLstViewItm.ID = pDr(GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.IDField)
                    End If

                Else
                    pLstViewItm.ID = pDr.Fid
                End If


                pLstViewItm.FeatureDataRow = pDr

                ' pLstViewItm.Tag = New IDandGeo(pDr.Geometry, pDr.Fid)

                LstView.Items.Add(pLstViewItm)
            Next


        Catch ex As Exception

        End Try

    End Sub
    Private Function LoadWOListControlListView(ByVal LstView As ListView, ByVal datarows() As DataRow, ByVal FieldsList As List(Of String), ByVal JoinString As String) As Envelope
        Try

            Dim pLstViewItm As MyListViewItem

            Dim strDis As String = ""
            LstView.Items.Clear()
            Dim pDr As FeatureDataRow
            Dim WOExt As Esri.ArcGIS.Mobile.Geometries.Envelope = New Esri.ArcGIS.Mobile.Geometries.Envelope
            Dim obj As Object
            Dim pCV As CodedValueDomain

            For Each dr As DataRow In datarows
                pDr = dr

                pLstViewItm = New MyListViewItem

                strDis = ""
                Dim subVal As Integer = 0
                If pDr.FeatureSource.HasSubtypes() Then
                    subVal = pDr.Item(pDr.FeatureSource.SubtypeColumnIndex)
                End If

                For i As Integer = 0 To FieldsList.Count - 1
                    If FieldsList(i) <> pDr.FeatureSource.GeometryColumnName Then
                        Dim valToAdd As Object = pDr.Item(FieldsList(i))
                        obj = CType(pDr.FeatureSource.Columns(FieldsList(i)), SourceDataColumn).GetDomain(subVal)

                        If obj IsNot Nothing Then
                            If TypeOf (obj) Is CodedValueDomain Then
                                pCV = obj

                                valToAdd = GlobalsFunctions.getDomainCode(valToAdd, pCV)
                            End If
                        End If


                        If strDis = "" Then
                         
                           
                            strDis = valToAdd.ToString()

                            'pLstViewItm.Text = (pDr(i).ToString)
                        Else
                            'pLstViewItm.SubItems.Add(pDr(i).ToString)
                            strDis = strDis & JoinString & valToAdd.ToString()

                        End If
                    End If


                Next
                If strDis = "" Then
                    strDis = GlobalsFunctions.appConfig.WorkorderPanel.UIComponents.NoDisplayText

                End If
                pLstViewItm.Text = strDis
                pLstViewItm.Geometry = pDr.Geometry
                WOExt.Union(pDr.Geometry.Extent)
                pLstViewItm.FID = pDr.Fid
                If pDr.FeatureSource.Columns(GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.IDField) IsNot Nothing Then
                    If pDr(GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.IDField) Is Nothing Then
                        pLstViewItm.ID = GlobalsFunctions.appConfig.WorkorderPanel.UIComponents.FieldCreateWOText & pDr.Fid.ToString()
                    ElseIf pDr(GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.IDField) Is DBNull.Value Then


                        pLstViewItm.ID = GlobalsFunctions.appConfig.WorkorderPanel.UIComponents.FieldCreateWOText & pDr.Fid.ToString()
                    Else
                        pLstViewItm.ID = pDr.Item(GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.IDField)
                    End If

                Else
                    pLstViewItm.ID = pDr.Fid
                End If


                pLstViewItm.FeatureDataRow = pDr

                ' pLstViewItm.Tag = New IDandGeo(pDr.Geometry, pDr.Fid)

                LstView.Items.Add(pLstViewItm)
            Next

            Return WOExt

        Catch ex As Exception

        End Try

    End Function
    Private Sub LoadWOListControlDataGrid(ByVal DGView As DataGridView, ByVal DataTable As FeatureDataTable)
        Try


            DGView.AllowUserToAddRows = False
            DGView.AllowUserToDeleteRows = False
            DGView.AllowUserToOrderColumns = False
            DGView.AllowUserToResizeRows = False
            DGView.AllowUserToResizeColumns = False
            DGView.Font = New Font("Tahoma", 12, FontStyle.Bold)
            'DGView.GridColor = Color.Transparent
            DGView.BackgroundColor = Color.White
            DGView.ColumnHeadersVisible = False
            DGView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
            DGView.EditMode = DataGridViewEditMode.EditProgrammatically
            DGView.AutoSize = True
            DGView.ColumnHeadersVisible = False
            DGView.MultiSelect = False
            DGView.ReadOnly = True
            DGView.RowHeadersVisible = False
            DGView.SelectionMode = DataGridViewSelectionMode.FullRowSelect


            DGView.DataSource = DataTable
            DGView.AutoResizeColumns(System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader)

            DGView.AutoResizeRows(System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells)
            If DGView.Columns.Count > 0 Then
                DGView.Columns(DGView.Columns.Count - 1).Visible = False
            End If




        Catch ex As Exception

        End Try

    End Sub
    Private Sub clearDG(ByVal control As Object)
        Try
            If TypeOf (control) Is DataGridView Then
                Dim DGView As DataGridView = control
                DGView.AllowUserToAddRows = False
                DGView.AllowUserToDeleteRows = False
                DGView.AllowUserToOrderColumns = False
                DGView.AllowUserToResizeRows = False
                DGView.AllowUserToResizeColumns = False
                DGView.Font = New Font("Tahoma", 12, FontStyle.Bold)
                'DGView.GridColor = Color.Transparent
                DGView.BackgroundColor = Color.White
                DGView.ColumnHeadersVisible = False
                DGView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
                DGView.EditMode = DataGridViewEditMode.EditProgrammatically
                DGView.AutoSize = True
                DGView.ColumnHeadersVisible = False
                DGView.MultiSelect = False
                DGView.ReadOnly = True
                DGView.RowHeadersVisible = False
                DGView.SelectionMode = DataGridViewSelectionMode.FullRowSelect


                DGView.DataSource = Nothing
                DGView.AutoResizeColumns(System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader)

                DGView.AutoResizeRows(System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells)
                If DGView.Columns.Count > 0 Then
                    DGView.Columns(DGView.Columns.Count - 1).Visible = False
                End If




            ElseIf TypeOf (control) Is ListView Then

                Dim LstView As ListView = control

                LstView.Items.Clear()

            End If
        Catch ex As Exception

        End Try

    End Sub
    Public Sub selectWOatLocation(coord As Coordinate)
        Dim selectedID As Integer = -1
        Dim strSql As String
   
        strSql = workorderSQL(m_OutlookNavigatePane.SelectedButton.Tag)
    

  
        Dim pFDRs As DataRowCollection = selectFeatures(New Esri.ArcGIS.Mobile.Geometries.Point(coord), strSql)

        If pFDRs IsNot Nothing Then
            If pFDRs.Count = 0 Then Return

            If pFDRs.Count > 1 Then


                Dim strFldArr As List(Of String) = New List(Of String)

                If GlobalsFunctions.appConfig.WorkorderPanel.WorkOrderFilters.WorkOrderFilter(0).Fields IsNot Nothing Then
                    If GlobalsFunctions.appConfig.WorkorderPanel.WorkOrderFilters.WorkOrderFilter(0).Fields.Field IsNot Nothing Then
                        If GlobalsFunctions.appConfig.WorkorderPanel.WorkOrderFilters.WorkOrderFilter(0).Fields.Field.Count > 0 Then
                            For Each fld As Esri.ArcGISTemplates.MobileConfigClass.MobileConfigMobileMapConfigWorkorderPanelWorkOrderFiltersWorkOrderFilterFieldsField In GlobalsFunctions.appConfig.WorkorderPanel.WorkOrderFilters.WorkOrderFilter(0).Fields.Field
                                If m_WOFSwD.FeatureSource.Columns(fld.Name) IsNot Nothing Then
                                    strFldArr.Add(fld.Name)
                                End If

                            Next
                        End If
                    End If
                End If
                Dim strDis As String
                Dim obj As Object
                Dim pCV As CodedValueDomain
                Dim table As New DataTable

                ' Create four typed columns in the DataTable.
                table.Columns.Add("DISPLAY", GetType(String))
                table.Columns.Add("VALUE", GetType(Integer))
                For Each pFDR As FeatureDataRow In pFDRs

                    strDis = ""
                    Dim subVal As Integer = 0
                    If m_WOFSwD.FeatureSource.HasSubtypes() Then
                        subVal = pFDR.Item(m_WOFSwD.FeatureSource.SubtypeColumnIndex)
                    End If

                    For i As Integer = 0 To strFldArr.Count - 1
                        If strFldArr(i) <> pFDR.FeatureSource.GeometryColumnName Then
                            Dim valToAdd As Object = pFDR.Item(strFldArr(i))
                            obj = CType(pFDR.FeatureSource.Columns(strFldArr(i)), SourceDataColumn).GetDomain(subVal)

                            If obj IsNot Nothing Then
                                If TypeOf (obj) Is CodedValueDomain Then
                                    pCV = obj

                                    valToAdd = GlobalsFunctions.getDomainCode(valToAdd, pCV)
                                End If
                            End If

                            If strDis = "" Then
                                strDis = valToAdd.ToString()

                                'pLstViewItm.Text = (pDr(i).ToString)
                            Else
                                'pLstViewItm.SubItems.Add(pDr(i).ToString)
                                strDis = strDis & GlobalsFunctions.appConfig.WorkorderPanel.WorkOrderFilters.WorkOrderFilter(0).Fields.JoinString & valToAdd.ToString()

                            End If
                        End If


                    Next
                    If strDis = "" Then
                        strDis = GlobalsFunctions.appConfig.WorkorderPanel.UIComponents.NoDisplayText

                    End If

                    table.Rows.Add(strDis, pFDR.Fid)


                Next

                Dim frmSelect As frmSelectOptionCombo = New frmSelectOptionCombo("Multiple Assignments at that location", table, "DISPLAY", "VALUE", "", "", False, False)

                Dim res As String = frmSelect.ShowDialog()
                If frmSelect.selectedValue <> "" Then
                    selectedID = CType(frmSelect.selectedValue, Integer)
                Else
                    Return
                End If


            Else
                selectedID = CType(pFDRs(0), FeatureDataRow).Fid
            End If

            m_CurrentWOID = ""
            CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).SelectedItems.Clear()

            For Each itm As MyListViewItem In CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).Items
                If itm.FeatureDataRow.Fid = selectedID Then
                    Call btnViewAllWork_Click(Nothing, Nothing)
                    '  RemoveHandler CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).SelectedIndexChanged, AddressOf ListView_SelectedIndexChanged


                    m_OutlookNavigatePane.SelectedButton.PerformClick()
                    CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).EnsureVisible(itm.Index)
                    itm.Selected = True
                    itm.Focused = True
                    '   AddHandler CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).SelectedIndexChanged, AddressOf ListView_SelectedIndexChanged
                    CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).Focus()
                    'Dim pCoord As Esri.ArcGIS.Mobile.Geometries.Coordinate = GlobalsFunctions.GetGeometryCenter(pFDR.Geometry)
                    'm_Map.CenterAt(pCoord)
                    Return


                End If
            Next

            '    Next
            'For Each nvBtn As NavigateBarButton In m_NVBButton
            '    For Each itm As MyListViewItem In CType(nvBtn.RelatedControl, ListView).Items
            '        If itm.FeatureDataRow.Fid = pFDR.Fid Then
            '            RemoveHandler CType(nvBtn.RelatedControl, ListView).SelectedIndexChanged, AddressOf ListView_SelectedIndexChanged

            '            m_OutlookNavigatePane.SelectedButton = nvBtn
            '            nvBtn.PerformClick()
            '            CType(nvBtn.RelatedControl, ListView).EnsureVisible(itm.Index)
            '            itm.Selected = True
            '            itm.Focused = True
            '            AddHandler CType(nvBtn.RelatedControl, ListView).SelectedIndexChanged, AddressOf ListView_SelectedIndexChanged
            '            CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).Focus()
            '            Return


            '        End If


            '    Next
            'Next

        End If

    End Sub
    Public Function selectFeature(ByVal Location As Esri.ArcGIS.Mobile.Geometries.Geometry, sql As String) As FeatureDataRow
        Try
            Dim pDT As FeatureDataTable

            pDT = spatialQFeature(Location, Sql)
            If pDT.Rows.Count = 0 Then Return Nothing

            Return pDT.Rows(0)


        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Public Function selectFeatures(ByVal Location As Esri.ArcGIS.Mobile.Geometries.Geometry, sql As String) As DataRowCollection
        Try
            Dim pDT As FeatureDataTable

            pDT = spatialQFeature(Location, sql)
            If pDT.Rows.Count = 0 Then Return Nothing

            Return pDT.Rows


        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Private Function spatialQFeature(ByVal Location As Esri.ArcGIS.Mobile.Geometries.Geometry, sql As String) As FeatureDataTable
        Dim pLayDef As MobileCacheMapLayerDefinition


        Dim pEnv As Envelope
        Try


            If m_WOFSwD IsNot Nothing Then
                If m_WOFSwD.FeatureSource IsNot Nothing Then


                    pLayDef = GlobalsFunctions.GetLayerDefinition(m_Map, m_WOFSwD)

                    If pLayDef.Visibility = False Then Return Nothing


                    'If a point is passed in, expand the envelope 
                    If TypeOf Location Is Esri.ArcGIS.Mobile.Geometries.Point Then
                        Dim pPnt As Esri.ArcGIS.Mobile.Geometries.Point = CType(Location, Esri.ArcGIS.Mobile.Geometries.Point)

                        Dim intBufferValueforPoint As Double
                        intBufferValueforPoint = GlobalsFunctions.bufferToMap(m_Map, m_BufferDivideValue) 'maptobuffer()

                        pEnv = New Envelope(0, 0, intBufferValueforPoint, intBufferValueforPoint)
                        pEnv.CenterAt(pPnt.Coordinate)

                    Else
                        pEnv = Location.Extent
                    End If



                    'Set up query filter used to id features
                    Dim pQf As QueryFilter = New QueryFilter(pEnv, Geometries.GeometricRelationshipType.Intersect, sql)
                    'Build an array to limit returned fields
                    Dim pStr(m_WOFSwD.FeatureSource.Columns.Count) As String

                    If m_WOFSwD.FeatureSource.GetFeatureCount(pQf) > 0 Then


                        Try
                            'Return the resulting rows

                            Return m_WOFSwD.FeatureSource.GetDataTable(pQf)
                        Catch ex As Exception

                        End Try
                    End If
                End If
            End If

            Return Nothing
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
#Region "Events"

    Public Function getWO() As String
        Return m_CurrentWOID
    End Function
    Public Function getCrew() As String
        Return m_AssignedTo
    End Function

    Public Function getText() As String
        Return lblCurrentWO.Text
    End Function
    Private Sub ListView_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

        If CType(sender, ListView).SelectedItems IsNot Nothing Then

            If CType(sender, ListView).SelectedItems.Count > 0 Then
                '.
                Dim woFilt As Esri.ArcGISTemplates.MobileConfigClass.MobileConfigMobileMapConfigWorkorderPanelWorkOrderFiltersWorkOrderFilter = m_OutlookNavigatePane.SelectedButton.Tag

                lblCurrentWO.Text = CType(sender, ListView).SelectedItems(0).Text
                m_CurrentWOID = CType(CType(sender, ListView).SelectedItems(0), MyListViewItem).ID
                RaiseEvent WOChanged(m_CurrentWOID, m_AssignedTo, lblCurrentWO.Text)
                Dim prevWO As String = CType(CType(sender, ListView).SelectedItems(0), MyListViewItem).ID
                RaiseEvent RaisePermMessage(CType(sender, ListView).SelectedItems(0).Text, False)
                Dim pGeo As Geometry = CType(CType(sender, ListView).SelectedItems(0), MyListViewItem).Geometry
                Dim pFDR As FeatureDataRow = CType(CType(sender, ListView).SelectedItems(0), MyListViewItem).FeatureDataRow

                If (woFilt.Action = "") Then
                    GlobalsFunctions.zoomTo(pGeo, m_Map)
                    GlobalsFunctions.flashGeo(pGeo, m_Map, m_penLine, CType(m_brush, SolidBrush))

                Else


                    Dim strToDos As String() = woFilt.Action.Split(",")

                    For Each strToDo As String In strToDos
                        Select Case strToDo.ToUpper
                            Case "ZOOM"
                                GlobalsFunctions.zoomTo(pGeo, m_Map)
                            Case "FLASH"

                                If pGeo.GeometryType = GeometryType.Point Then
                                    GlobalsFunctions.flashGeo(pGeo, m_Map, m_penLine, CType(m_brush, SolidBrush))
                                Else
                                    GlobalsFunctions.flashGeo(pGeo, m_Map, m_pen, m_brush)

                                End If
                            Case "PAN"
                                GlobalsFunctions.panTo(pGeo, m_Map)
                        End Select
                    Next
                    'MsgBox(CType(CType(sender, ListView).SelectedItems(0), MyListViewItem).FID.ToString())
                End If

                m_EdtClose.Visible = True
                m_AttInfoDisplay.Visible = True

                m_EdtClose.setCurrentRecord(pFDR, m_EditOp)
                m_AttInfoDisplay.CurrentRow = pFDR

                'm_AttInfoDisplay.IdentifyLocation(pGeo)
                'CType(sender, ListView).Focus()
                'CType(sender, ListView).Refresh()

                'm_EdtCreate.setCurrentRecord(CType(CType(sender, ListView).SelectedItems(0), MyListViewItem).FeatureDataRow, Nothing)
                CType(sender, ListView).Focus()
                For Each itm As MyListViewItem In CType(sender, ListView).Items
                    If itm.ID = prevWO And itm.Geometry.EquivalentTo(pGeo) Then

                        RemoveHandler CType(sender, ListView).SelectedIndexChanged, AddressOf ListView_SelectedIndexChanged


                        CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).EnsureVisible(itm.Index)
                        itm.Selected = True
                        itm.Focused = True
                        AddHandler CType(sender, ListView).SelectedIndexChanged, AddressOf ListView_SelectedIndexChanged

                        Exit For


                    End If
                Next

            End If
        End If
        Try
            CType(m_OutlookNavigatePane.SelectedButton.RelatedControl, ListView).Focus()
        Catch ex As Exception

        End Try




    End Sub
    Private Sub ListView_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If CType(sender, ListView).SelectedItems IsNot Nothing Then

            If CType(sender, ListView).SelectedItems.Count > 0 Then

                Dim woFilt As Esri.ArcGISTemplates.MobileConfigClass.MobileConfigMobileMapConfigWorkorderPanelWorkOrderFiltersWorkOrderFilter = m_OutlookNavigatePane.SelectedButton.Tag

                btnViewWorkDetails_Click(Nothing, Nothing)

                'm_AttDisplay.IdentifyLocation(CType(CType(sender, ListView).SelectedItems(0), MyListViewItem).Geometry)




            End If
        End If


    End Sub

#End Region

#Region "DisplayChangeEvents"
    Private Sub outlookNavigatePane_OnNavigateBarColorChanged(ByVal sender As Object, ByVal e As EventArgs)
        splitterNavigateMenu.SplitterLightColor = m_OutlookNavigatePane.NavigateBarColorTable.ButtonNormalBegin
        splitterNavigateMenu.SplitterDarkColor = m_OutlookNavigatePane.NavigateBarColorTable.ButtonNormalEnd
        splitterNavigateMenu.SplitterBorderColor = Color.Transparent
    End Sub

    ''' <summary>
    ''' Set new color (passing saved color table in settings file )
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub outlookNavigatePane_HandleCreated(ByVal sender As Object, ByVal e As EventArgs)
        'if (MessageBox.Show("Do you want use system colors", "Theme Color", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        '    outlookNavigatePane.NavigateBarColorTable = NavigateBarColorTable.SystemColor;
    End Sub


    ''' <summary>
    ''' Any button selected
    ''' </summary>
    ''' <param name="tNavigateBarButton"></param>
    Private Sub outlookNavigatePane_OnNavigateBarButtonSelected(ByVal tNavigateBarButton As NavigateBarButton)
        If TypeOf (tNavigateBarButton.RelatedControl) Is DataGridView Then
            Dim pDGrid As DataGridView = tNavigateBarButton.RelatedControl
            If pDGrid.Columns(m_WOFSwD.FeatureSource.FidColumnName) IsNot Nothing Then
                pDGrid.Columns(m_WOFSwD.FeatureSource.FidColumnName).Visible = False

            End If
        ElseIf TypeOf (tNavigateBarButton.RelatedControl) Is ListView Then

            Dim pFSInfo As MobileCacheMapLayerDefinition = CType(m_Map.MapLayers(m_WOFSwD.MapLayerIndex), MobileCacheMapLayer).LayerDefinitions(m_WOFSwD.LayerIndex)

            pFSInfo.DisplayExpression = workorderSQL(tNavigateBarButton.Tag)
            pFSInfo.Visibility = True
            pFSInfo = Nothing

            'If CType(tNavigateBarButton.RelatedControl, ListView).Columns.Count > 0 Then
            '    CType(tNavigateBarButton.RelatedControl, ListView).Columns(0).Width = tNavigateBarButton.RelatedControl.Parent.Width - ImageList1.ImageSize.Width

            'End If

        End If

        ' MsgBox(pDGrid.ColumnCount)

    End Sub

    ''' <summary>
    ''' Check selecting for any button
    ''' </summary>
    ''' <param name="e"></param>
    'Private Sub outlookNavigatePane_OnNavigateBarButtonSelecting(ByVal e As NavigateBarButtonCancelEventArgs)
    '    Dim pDGrid As DataGridView = e.Selected.RelatedControl
    '    MsgBox(pDGrid.ColumnCount)

    'End Sub
    'Private Sub testEvent(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    Dim pDGrid As DataGridView = sender
    '    MsgBox(pDGrid.ColumnCount)

    'End Sub
    Private Sub nvbMail_OnNavigateBarButtonSelected(ByVal e As NavigateBarButtonEventArgs)

    End Sub

    Private Sub NavigationPane_OnNavigateBarDisplayedButtonCountChanged()

    End Sub



    Private Sub gpBoxButtons_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles gpBoxButtons.Resize
        relocateButtons()
    End Sub
    Private Sub relocateButtons()


        Dim btnLoc As Double
        Dim btnHt As Double
        Dim curLoc As Double
        'btnHt = lblCrewName.Top + lblCrewName.Height + 5
        'btnHt = CInt(btnViewAllWork.Parent.Height / 2 - (btnFiltGeo.Height)) + 3
        btnHt = btnCrew.Top + btnCrew.Height + 5
        If btnFiltGeo.Visible Then


            btnLoc = btnViewAllWork.Parent.Width / 6
            curLoc = btnLoc

            btnViewAllWork.Left = CInt(curLoc - (btnViewAllWork.Width / 2))
            btnViewAllWork.Top = btnHt
            curLoc = curLoc + btnLoc

            btnViewWorkDetails.Left = CInt(curLoc - (btnViewWorkDetails.Width / 2))
            btnViewWorkDetails.Top = btnHt
            curLoc = curLoc + btnLoc

            btnCloseWork.Left = CInt(curLoc - (btnCloseWork.Width / 2))
            btnCloseWork.Top = btnHt
            curLoc = curLoc + btnLoc

            btnClear.Left = CInt(curLoc - (btnClear.Width / 2))
            btnClear.Top = btnHt
            curLoc = curLoc + btnLoc

            btnFiltGeo.Left = CInt(curLoc - (btnClear.Width / 2))
            btnFiltGeo.Top = btnHt

        ElseIf btnClear.Visible Then

            btnLoc = btnViewAllWork.Parent.Width / 5
            curLoc = btnLoc

            btnViewAllWork.Left = CInt(curLoc - (btnViewAllWork.Width / 2))
            btnViewAllWork.Top = btnHt
            curLoc = curLoc + btnLoc

            btnViewWorkDetails.Left = CInt(curLoc - (btnViewWorkDetails.Width / 2))
            btnViewWorkDetails.Top = btnHt
            curLoc = curLoc + btnLoc

            btnCloseWork.Left = CInt(curLoc - (btnCloseWork.Width / 2))
            btnCloseWork.Top = btnHt
            curLoc = curLoc + btnLoc

            btnClear.Left = CInt(curLoc - (btnClear.Width / 2))
            btnClear.Top = btnHt


        Else
            btnLoc = btnViewAllWork.Parent.Width / 4

            curLoc = btnLoc

            btnViewAllWork.Left = CInt(curLoc - (btnViewAllWork.Width / 2))
            btnViewAllWork.Top = btnHt
            curLoc = curLoc + btnLoc

            btnViewWorkDetails.Left = CInt(curLoc - (btnViewWorkDetails.Width / 2))
            btnViewWorkDetails.Top = btnHt
            curLoc = curLoc + btnLoc

            btnCloseWork.Left = CInt(curLoc - (btnCloseWork.Width / 2))
            btnCloseWork.Top = btnHt
            curLoc = curLoc + btnLoc


        End If

    End Sub

    Private Sub btnViewAllWork_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnViewAllWork.CheckedChanged

        If btnViewAllWork.Checked = True Then
            btnViewAllWork.BackgroundImage = Global.MobileControls.My.Resources.ClipboardRed

        Else
            btnViewAllWork.BackgroundImage = Global.MobileControls.My.Resources.ClipboardBlue


        End If
    End Sub

    Private Sub btnViewAllWork_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnViewAllWork.Click

        If m_OutlookNavigatePane Is Nothing Then Return

        If m_OutlookNavigatePane.SelectedButton Is Nothing Then Return

        If m_OutlookNavigatePane.SelectedButton.RelatedControl Is Nothing Then Return

        'update display
        gpBoxWOList.Visible = True
        gpBoxWOClose.Visible = False
        gpBoxWOCreate.Visible = False
        gpBoxWODetails.Visible = False

        btnViewAllWork.Checked = True
        btnCloseWork.Checked = False

        btnViewWorkDetails.Checked = False

        Dim pLstView As ListView = m_OutlookNavigatePane.SelectedButton.RelatedControl
        pLstView.Focus()
        'If pLstView.SelectedItems.Count > 0 Then
        '    'pLstView.HideSelection = False
        '    pLstView.Focus()


        '    m_AttDisplay.IdentifyLocation(CType(pLstView.SelectedItems(0), MyListViewItem).Geometry)



        'End If
    End Sub

    Private Sub btnViewWorkDetails_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnViewWorkDetails.CheckedChanged
        If btnViewWorkDetails.Checked = True Then
            btnViewWorkDetails.BackgroundImage = Global.MobileControls.My.Resources.DocRed

        Else
            btnViewWorkDetails.BackgroundImage = Global.MobileControls.My.Resources.DocBlue


        End If
    End Sub

    Private Sub btnViewWorkDetails_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnViewWorkDetails.Click
        'update display
        gpBoxWOList.Visible = False
        gpBoxWOClose.Visible = False
        gpBoxWOCreate.Visible = False
        gpBoxWODetails.Visible = True

        btnViewAllWork.Checked = False
        btnCloseWork.Checked = False

        btnViewWorkDetails.Checked = True

    End Sub

    Private Sub btnCloseWork_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCloseWork.CheckedChanged
        If btnCloseWork.Checked = True Then
            btnCloseWork.BackgroundImage = Global.MobileControls.My.Resources.CheckRed

        Else
            btnCloseWork.BackgroundImage = Global.MobileControls.My.Resources.CheckBlue


        End If
    End Sub

    Private Sub btnCloseWork_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCloseWork.Click
        'update display
        gpBoxWOList.Visible = False
        gpBoxWOClose.Visible = True
        gpBoxWOCreate.Visible = False
        gpBoxWODetails.Visible = False

        btnViewAllWork.Checked = False
        btnCloseWork.Checked = True

        btnViewWorkDetails.Checked = False

    End Sub

    Private Sub btnFiltGeo_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFiltGeo.CheckedChanged
        If btnFiltGeo.Checked = True Then
            btnFiltGeo.BackgroundImage = Global.MobileControls.My.Resources.FilterOn

        Else
            btnFiltGeo.BackgroundImage = Global.MobileControls.My.Resources.FilterOff


        End If
    End Sub

    Private Sub btnFiltGeo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFiltGeo.Click
        btnFiltGeo.Checked = Not btnFiltGeo.Checked
        LoadWorkOrders(False)
        m_OutlookNavigatePane.SelectedButton.PerformClick()

    End Sub
#End Region

    Protected Overrides Sub Finalize()
        RemoveHandler m_WOFSwD.FeatureSource.DataChanged, AddressOf layer_DataChanged

        MyBase.Finalize()
    End Sub

    Private Sub m_EdtClose_RecordSaved(LayerName As String, pGeo As Esri.ArcGIS.Mobile.Geometries.Geometry, OID As Integer) Handles m_EdtClose.RecordSaved
        RaiseEvent RaiseMessage(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.SavedMessage)
       
    End Sub

    Private Sub m_EdtCreate_RecordSaved(LayerName As String, pGeo As Esri.ArcGIS.Mobile.Geometries.Geometry, OID As Integer) Handles m_EdtCreate.RecordSaved
        RaiseEvent RaiseMessage(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.SavedMessage)
        LoadWorkOrders()
        Call btnViewAllWork_Click(Nothing, Nothing)
    End Sub



    Private Sub btnClear_Click(sender As Object, e As System.EventArgs) Handles btnClear.Click
        RaiseEvent RaisePermMessage("", True)
        m_EdtClose.Visible = False
        m_AttInfoDisplay.Visible = False

        'm_EdtClose.DisplayBlank()
        'm_AttInfoDisplay.DisplayBlank()

        lblCurrentWO.Text = ""
        m_CurrentWOID = ""
        RaiseEvent WOChanged("", "", "")

        LoadWorkOrders(False, False)
        Call btnViewAllWork_Click(Nothing, Nothing)

    End Sub

    Private Sub btnClear_CheckedChanged(sender As System.Object, e As System.EventArgs)

    End Sub

    Private Sub btnCrew_Click(sender As System.Object, e As System.EventArgs) Handles btnCrew.Click
        If m_WOFSwD.FeatureSource Is Nothing Then Return

        Dim frmSelect As frmSelectOptionCombo = New frmSelectOptionCombo(GlobalsFunctions.appConfig.WorkorderPanel.UIComponents.ChangeCrewText,
                                                                         GetCrewNames(), GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.AssignedToField,
                                                                         GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.AssignedToField, m_AssignedTo,
                                                                         GlobalsFunctions.appConfig.WorkorderPanel.UIComponents.SaveCrewText, True, True)

        frmSelect.ShowDialog()
        If frmSelect.selectedOption <> "" Then
            If frmSelect.selectedOption = GlobalsFunctions.appConfig.WorkorderPanel.UIComponents.AllWOText Then
                m_AssignedTo = GlobalsFunctions.appConfig.WorkorderPanel.UIComponents.AllWOText
            Else
                m_AssignedTo = frmSelect.selectedOption
            End If

            LoadWorkOrders()
            If frmSelect.checkboxState Then
                GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.AssignedTo = m_AssignedTo

                GlobalsFunctions.writeConfig()
            End If
        End If


    End Sub


    Private Sub m_Map_ExtentChanged(sender As Object, e As EventArgs) Handles m_Map.ExtentChanged
        If btnFiltGeo.Checked Then
            LoadWorkOrders(False, False)
            If m_OutlookNavigatePane IsNot Nothing Then
                If m_OutlookNavigatePane.SelectedButton IsNot Nothing Then
                    m_OutlookNavigatePane.SelectedButton.PerformClick()

                End If
            End If

        End If
    End Sub

    Private Sub btnClear_CheckedChanged_1(sender As Object, e As EventArgs) Handles btnClear.CheckedChanged

    End Sub
End Class
