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
    Private m_CurrentWOID As String = ""
    Public Event RaiseMessage(ByVal Message As String)
    Public Event RaisePermMessage(ByVal Message As String, ByVal Hide As Boolean)

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
            If m_WOFSwD.FeatureSource Is Nothing Then Return




            AddHandler m_WOFSwD.FeatureSource.DataChanged, AddressOf layer_DataChanged


            If m_WOFSwD.FeatureSource.Columns(GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.AssignedToField) Is Nothing Then Return

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

            'btnViewWorkDetails_Click(Nothing, Nothing)


            'btnCloseWork_Click(Nothing, Nothing)


            'btnCreateWork_Click(Nothing, Nothing)


            'btnViewAllWork_Click(Nothing, Nothing)



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

            LoadWorkOrders()


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

                    strSql = GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.AssignedToField & " = '" & m_AssignedTo & "'"
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
    Public Sub LoadWorkOrders()

        Dim woFilt As Esri.ArcGISTemplates.MobileConfigClass.MobileConfigMobileMapConfigWorkorderPanelWorkOrderFiltersWorkOrderFilter = Nothing
        Dim pCrewDC As DataColumn = Nothing
        Dim pQF As QueryFilter = Nothing
        Dim pDt As FeatureDataTable = Nothing
        Dim strLayFilt As String = ""
        Dim strSql As String

        Try

            For Each nvBtn As NavigateBarButton In m_NVBButton
                woFilt = nvBtn.Tag

                strSql = workorderSQL(woFilt)


                pQF = New QueryFilter(strSql)
                If m_WOFSwD.FeatureSource.GetFeatureCount(pQF) > 0 Then
                    If strLayFilt = "" Then
                        strLayFilt = strSql

                    End If
                    Dim strFldArr As New List(Of String)

                    strFldArr.Add(m_WOFSwD.FeatureSource.GeometryColumnName)

                    If woFilt.Fields IsNot Nothing Then
                        If woFilt.Fields.Field IsNot Nothing Then
                            If woFilt.Fields.Field.Count > 0 Then
                                For Each fld As Esri.ArcGISTemplates.MobileConfigClass.MobileConfigMobileMapConfigWorkorderPanelWorkOrderFiltersWorkOrderFilterFieldsField In woFilt.Fields.Field
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
                    pDt = m_WOFSwD.FeatureSource.GetDataTable(pQF) ', strFldArr.ToArray())

                    If TypeOf (nvBtn.RelatedControl) Is DataGridView Then
                        LoadWOListControlDataGrid(nvBtn.RelatedControl, pDt)
                    ElseIf TypeOf (nvBtn.RelatedControl) Is ListView Then
                        LoadWOListControlListView(nvBtn.RelatedControl, pDt, strFldArr, woFilt.Fields.JoinString)

                    End If
                    nvBtn.Caption = woFilt.Label & ": " & m_WOFSwD.FeatureSource.GetFeatureCount(pQF)
                    ' nvBtn.PerformClick()

                End If

            Next

            Dim pFSInfo As MobileCacheMapLayerDefinition = CType(m_Map.MapLayers(m_WOFSwD.MapLayerIndex), MobileCacheMapLayer).LayerDefinitions(m_WOFSwD.LayerIndex)

            pFSInfo.DisplayExpression = strLayFilt
            pFSInfo.Visibility = True
            pFSInfo = Nothing


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
            btnCrew.Text = m_AssignedTo

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
        If lstView.Columns.Count > 0 Then
            'If lstView.Parent IsNot Nothing Then
            'If lstView.Parent.Parent IsNot Nothing Then
            ' If lstView.Parent.Parent.Parent IsNot Nothing Then
            'lstView.Columns(0).Width = lstView.Parent.Parent.Parent.Width - ImageList1.ImageSize.Width
            lstView.Columns(0).Width = lstView.Width - ImageList1.ImageSize.Width
            'End If
            'End If
            'End If

        End If
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
#Region "Events"
    Public Function getWO() As String
        Return m_CurrentWOID


    End Function
    Public Function getCrew() As String
        Return m_AssignedTo


    End Function
    Private Sub ListView_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If CType(sender, ListView).SelectedItems IsNot Nothing Then

            If CType(sender, ListView).SelectedItems.Count > 0 Then
                '.
                Dim woFilt As Esri.ArcGISTemplates.MobileConfigClass.MobileConfigMobileMapConfigWorkorderPanelWorkOrderFiltersWorkOrderFilter = m_OutlookNavigatePane.SelectedButton.Tag

                lblCurrentWO.Text = CType(sender, ListView).SelectedItems(0).Text
                m_CurrentWOID = CType(CType(sender, ListView).SelectedItems(0), MyListViewItem).ID
                RaiseEvent RaisePermMessage(CType(sender, ListView).SelectedItems(0).Text, False)

                If (woFilt.Action = "") Then
                    GlobalsFunctions.zoomTo(CType(CType(sender, ListView).SelectedItems(0), MyListViewItem).Geometry, m_Map)
                    GlobalsFunctions.flashGeo(CType(CType(sender, ListView).SelectedItems(0), MyListViewItem).Geometry, m_Map, m_penLine, CType(m_brush, SolidBrush))

                Else


                    Dim strToDos As String() = woFilt.Action.Split(",")
                    For Each strToDo As String In strToDos
                        Select Case strToDo.ToUpper
                            Case "ZOOM"
                                GlobalsFunctions.zoomTo(CType(CType(sender, ListView).SelectedItems(0), MyListViewItem).Geometry, m_Map)
                            Case "FLASH"

                                If CType(CType(sender, ListView).SelectedItems(0), MyListViewItem).Geometry.GeometryType = GeometryType.Point Then
                                    GlobalsFunctions.flashGeo(CType(CType(sender, ListView).SelectedItems(0), MyListViewItem).Geometry, m_Map, m_penLine, CType(m_brush, SolidBrush))
                                Else
                                    GlobalsFunctions.flashGeo(CType(CType(sender, ListView).SelectedItems(0), MyListViewItem).Geometry, m_Map, m_pen, m_brush)

                                End If
                            Case "PAN"
                                GlobalsFunctions.panTo(CType(CType(sender, ListView).SelectedItems(0), MyListViewItem).Geometry, m_Map)
                        End Select
                    Next
                    'MsgBox(CType(CType(sender, ListView).SelectedItems(0), MyListViewItem).FID.ToString())
                End If
                m_EdtClose.Visible = True

                m_EdtClose.setCurrentRecord(CType(CType(sender, ListView).SelectedItems(0), MyListViewItem).FeatureDataRow, m_EditOp)
                m_AttInfoDisplay.IdentifyLocation(CType(CType(sender, ListView).SelectedItems(0), MyListViewItem).Geometry)

                'm_EdtCreate.setCurrentRecord(CType(CType(sender, ListView).SelectedItems(0), MyListViewItem).FeatureDataRow, Nothing)


            End If
        End If



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
        'btnHt = CInt(btnViewAllWork.Parent.Height / 2 - (btnCreateWork.Height)) + 3
        btnHt = btnCrew.Top + btnCrew.Height + 5
        If btnCreateWork.Visible Then

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

            btnCreateWork.Left = CInt(curLoc - (btnCreateWork.Width / 2))
            btnCreateWork.Top = btnHt

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
        btnCreateWork.Checked = False
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
        btnCreateWork.Checked = False
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
        btnCreateWork.Checked = False
        btnViewWorkDetails.Checked = False

    End Sub

    Private Sub btnCreateWork_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreateWork.CheckedChanged
        If btnCreateWork.Checked = True Then
            btnCreateWork.BackgroundImage = Global.MobileControls.My.Resources.EmergRed

        Else
            btnCreateWork.BackgroundImage = Global.MobileControls.My.Resources.EmergBlue


        End If
    End Sub

    Private Sub btnCreateWork_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreateWork.Click
        'update display
        gpBoxWOList.Visible = False
        gpBoxWOClose.Visible = False
        gpBoxWOCreate.Visible = True
        gpBoxWODetails.Visible = False

        btnViewAllWork.Checked = False
        btnCloseWork.Checked = False
        btnCreateWork.Checked = True
        btnViewWorkDetails.Checked = False

    End Sub
#End Region

    Protected Overrides Sub Finalize()
        RemoveHandler m_WOFSwD.FeatureSource.DataChanged, AddressOf layer_DataChanged

        MyBase.Finalize()
    End Sub

    Private Sub m_EdtClose_RecordSaved(LayerName As String, pGeo As Esri.ArcGIS.Mobile.Geometries.Geometry, OID As Integer) Handles m_EdtClose.RecordSaved
        RaiseEvent RaiseMessage(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.SavedMessage)
        LoadWorkOrders()
        Call btnViewAllWork_Click(Nothing, Nothing)

    End Sub

    Private Sub m_EdtCreate_RecordSaved(LayerName As String, pGeo As Esri.ArcGIS.Mobile.Geometries.Geometry, OID As Integer) Handles m_EdtCreate.RecordSaved
        RaiseEvent RaiseMessage(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.SavedMessage)
        LoadWorkOrders()
        Call btnViewAllWork_Click(Nothing, Nothing)
    End Sub



    Private Sub btnClear_Click(sender As Object, e As System.EventArgs) Handles btnClear.Click
        LoadWorkOrders()
        Call btnViewAllWork_Click(Nothing, Nothing)
    End Sub

    Private Sub btnClear_CheckedChanged(sender As System.Object, e As System.EventArgs)

    End Sub

    Private Sub btnCrew_Click(sender As System.Object, e As System.EventArgs) Handles btnCrew.Click
        Dim frmSelect As frmSelectOptionCombo = New frmSelectOptionCombo(GlobalsFunctions.appConfig.WorkorderPanel.UIComponents.ChangeCrewText, GetCrewNames(), GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.AssignedToField, m_AssignedTo, GlobalsFunctions.appConfig.WorkorderPanel.UIComponents.SaveCrewText, True)

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
End Class
