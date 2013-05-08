' | Version 10.1.1
' | Copyright 2012 Esri
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


Imports Esri.ArcGIS.Mobile
Imports Esri.ArcGIS.Mobile.FeatureCaching
Imports System.Windows.Forms
Imports Esri.ArcGIS.Mobile.Geometries
Imports MobileControls.AddressParser
Imports System.Drawing

Imports System.Threading
Imports System.Xml
Imports Esri.ArcGIS.Mobile.DataProducts.StreetMapData
Imports Esri.ArcGISTemplates

Public Class MobileSearch
    Private m_Font As Font = New Font("Veranda", 10, FontStyle.Bold)
    Private WithEvents m_BuffMA As mobileBufferMapAction
    Private Shared m_SearchPoly As Polygon = Nothing
    Dim m_LastM As Esri.ArcGIS.Mobile.WinForms.MapAction = Nothing


    Public Event showIndicator(ByVal state As Boolean)

    Private m_Distinct As String = "Loop"
    Private m_ZoomToVal As Integer

    Private WithEvents m_Map As Esri.ArcGIS.Mobile.WinForms.Map
    'Spacing to adjust controls
    Private m_Rightpad As Integer = 10
    Private m_ButtonSpace As Integer = 10
    'The address layer name
    Private m_AddressLayer As String

    'Address/Street layer fields
    Private m_AddressFieldStreetName As String
    Private m_AddressFieldLeftFrom As String
    Private m_AddressFieldLeftTo As String
    Private m_AddressFieldRightFrom As String
    Private m_AddressFieldRightTo As String
    'Colors for flashing Geo
    Private m_penLine As Pen = Pens.Cyan
    Private m_pen As Pen = New Pen(Color.Purple, 10)
    Private m_brush As SolidBrush = New SolidBrush(Color.Cyan)

    Private lstPreFilterInt As List(Of ComboBox)
    Private Shared lstPreFilterSt As List(Of ComboBox)
    Private lstPreFilterPnt As List(Of ComboBox)

    'Variables to determine if the search and address/geocode are initilized
    Private m_bSearchInit As Boolean = False
    Private m_bAddress_GeocordInit As Boolean = False
    Private m_bAddress_PointInit As Boolean = False
    Private m_RouteToOption As Boolean

    'Events
    Public Event IDLocation(ByVal geo As Geometry, ByVal Layer As String, ByVal oid As Integer)
    Public Event IDResult(ByVal Feature As FeatureDataRow)
    Public Event RouteTo(ByVal location As Esri.ArcGIS.Mobile.Geometries.Geometry, ByVal LocationName As String)


    Private _smd As StreetMapDataset
    Private _streetMapLayer As StreetMapLayer
    'Private lookupLocator As Locator
    'Private stateFilter As LocatorFilter
    'Private cityFilter As LocatorFilter
    'Private streetFilter As LocatorFilter
    'Private houseNumberFilter As LocatorFilter

#Region "Public Functions"


    Private m_GPSStatus As String = "Off"

    Public Property GPSStatus As String
        Get
            Return m_GPSStatus
        End Get
        Set(ByVal value As String)
            m_GPSStatus = value
            If m_GPSStatus = "On" Then

                btnWaypoint.Enabled = True
                btnWaypointDrillDown.Enabled = True
                btnWaypointGC.Enabled = True
                btnWaypointOnline.Enabled = True
                btnWaypointSearch.Enabled = True
            Else

                btnWaypoint.Enabled = False
                btnWaypointDrillDown.Enabled = False
                btnWaypointGC.Enabled = False
                btnWaypointOnline.Enabled = False
                btnWaypointSearch.Enabled = False
            End If

        End Set
    End Property
    Private Sub btnWaypoint_EnabledChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnWaypoint.EnabledChanged, btnWaypointDrillDown.EnabledChanged, btnWaypointGC.EnabledChanged, btnWaypointOnline.EnabledChanged, btnWaypointSearch.EnabledChanged
        If CType(sender, Button).Enabled Then
            CType(sender, Button).BackgroundImage = My.Resources.NavTooBlue

        Else
            CType(sender, Button).BackgroundImage = My.Resources.NavTooGray

        End If
    End Sub

    Public Sub New(Optional ByVal RouteToOption As Boolean = False, Optional ByVal LoadSearchType As String = "Loop")
        ' This call is required by the Windows Form Designer.
        InitializeComponent()


        If LoadSearchType Is Nothing Then
            m_Distinct = "Loop"
        Else
            m_Distinct = LoadSearchType

        End If


        m_RouteToOption = RouteToOption


        If RouteToOption = False Then
            If btnDGRouteTo IsNot Nothing Then
                btnDGRouteTo.Visible = False
                btnAddressRouteTo.Visible = False

            End If

        End If
        If GlobalsFunctions.appConfig.NavigationOptions.GPS.WaypointControl.Visible.ToUpper = "TRUE" Then

            btnWaypoint.Visible = True
            btnWaypointDrillDown.Visible = True
            btnWaypointGC.Visible = True
            btnWaypointOnline.Visible = True
            btnWaypointSearch.Visible = True

        Else
            btnWaypoint.Visible = False
            btnWaypointDrillDown.Visible = False
            btnWaypointGC.Visible = False
            btnWaypointOnline.Visible = False
            btnWaypointSearch.Visible = False

        End If

        'set the panels to dock full
        pnlSearch.Dock = DockStyle.Fill
        pnlAddress.Dock = DockStyle.Fill
        pnlGeocode.Dock = DockStyle.Fill
        'hide the address and geocode panels on startup
        pnlAddress.Visible = False
        pnlGeocode.Visible = False
        'Set the search indicitor
        picSearching.Image = My.Resources.GPSLoadingIndicator
        picSearching.Width = picSearching.Image.Width
        picSearching.Height = picSearching.Image.Height

        'btnAddressPointZoomTo.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.ButtonShowText
        'btnAddressZoomTo.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.ButtonShowText
        'btnDGZoomTo.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.ButtonShowText
        'btnDGFlash.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.ButtonFlashText

        btnDGZoomTo.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.ButtonShowText
        btnDGFlash.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.ButtonFlashText

        gpBxGoToXY.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.GoToXYText
        'btnDGRouteTo.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.ButtonRouteText
        ' btnRunGC.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.GeocodeLookupButtonText
        'btnOnlineGC.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.GeocodeLookupButtonText
        btnRunXY.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.GoToXYButtonText
        chkLatLong.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.GoToXYLatLongText
        'btnSearchFind.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.SearchButtonText


        'btnAddressRouteTo.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.IntersectionZoomToButtonText
        'btnAddressReload.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.IntersectionReloadButtonText
          chkSearchSimliar.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.SearchSimilarLabelText
        If GlobalsFunctions.appConfig.SearchPanel.UIComponents.LatLongChecked.ToUpper = "TRUE" Then
            chkLatLong.Checked = True
            lblX.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.LongText
            lblY.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.LatText

        Else
            lblX.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.XText
            lblY.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.YText

        End If

        lblSearchField.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.SearchFieldLabelText
        lblSearchLayer.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.SearchLayerLabelText
        lblSearchValue.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.SearchValueLabelText
        lblSearchGeo.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.SearchBufferLabelText
        lblMatchingResults.Text = String.Format(GlobalsFunctions.appConfig.SearchPanel.UIComponents.DrillDownResultsText, "")
        'cboBufferVal.Items.Add(GlobalsFunctions.appConfig.SearchPanel.AreaSearchValues.NoBufferValue)

        'For Each bufVal In GlobalsFunctions.appConfig.SearchPanel.AreaSearchValues.AreaSearchValue
        '    cboBufferVal.Items.Add(bufVal.AreaSize)
        'Next
        cboBufferVal.DisplayMember = "Display"
        cboBufferVal.ValueMember = "AreaSize"
        cboBufferVal.DataSource = GlobalsFunctions.appConfig.SearchPanel.AreaSearchValues.AreaSearchValue

        cboBufferVal.SelectedIndex = 0

        lblGeocode.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.GeocodeAddressLabelText()
        lblOnlineGCLabel.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.GeocodeAddressLabelText()
        gpBoxPreFiltSt.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.GeocodePreFilterLabelText


    End Sub
    'Public Function InitAddressLookup(ByVal preFilter As String) As Boolean
    '    Dim pLbl As Label
    '    Dim pCbo As ComboBox

    '    For Each fieldName As String In preFilter.Split("|")
    '        pLbl = New Label
    '        pCbo = New ComboBox


    '    Next
    'End Function
    Public Sub disableAddPnt()
        pnlAddressPoint.Visible = False

    End Sub
    Public Function InitAddressPointLookup() As Boolean
        Try


            If m_Map Is Nothing Then
                m_bAddress_GeocordInit = False
                Return False

            End If

            btnAddressPnt.Visible = False

            If GlobalsFunctions.appConfig.SearchPanel.DrillDownSearches Is Nothing Then Return False
            If GlobalsFunctions.appConfig.SearchPanel.DrillDownSearches.DrillDownSearch Is Nothing Then Return False
            If GlobalsFunctions.appConfig.SearchPanel.DrillDownSearches.DrillDownSearch.Count = 0 Then Return False

            If GlobalsFunctions.appConfig.SearchPanel.DrillDownSearches.DrillDownSearch.Count = 1 Then
                gpBxAddPntControlsLayer.Visible = False

            End If
            btnAddressPnt.Visible = True


            cboDrillDownLayer.DisplayMember = "DisplayText"
            cboDrillDownLayer.ValueMember = "LayerName"

            cboDrillDownLayer.DataSource = GlobalsFunctions.appConfig.SearchPanel.DrillDownSearches.DrillDownSearch
            'm_intDrillDownLoadedIdx = 0
            'If CreateAddressPointControls(GlobalsFunctions.appConfig.SearchPanel.DrillDownSearches.DrillDownSearch(m_intDrillDownLoadedIdx)) = False Then Return False

            m_bAddress_GeocordInit = True
            Return True

        Catch ex As Exception
            m_bAddress_GeocordInit = False
            Return False
        End Try
        'm_AddressFieldStreetName = streetName

    End Function
    Public Sub ChangeXYFormat()
        txtY.Visible = False
        lblY.Visible = False

        lblX.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.XText.Replace(":", "") & "," & GlobalsFunctions.appConfig.SearchPanel.UIComponents.YText

        btnRunXY.Top = lblX.Top + lblX.Height + 10


    End Sub
    Public Function InitAddressLookup() As Boolean
        Try
            If m_Map Is Nothing Then
                m_bAddress_GeocordInit = False
                Return False

            End If
            If IsNumeric(GlobalsFunctions.appConfig.ApplicationSettings.ZoomExtent) Then
                m_ZoomToVal = CInt(GlobalsFunctions.appConfig.ApplicationSettings.ZoomExtent)
            Else
                m_ZoomToVal = 1500
            End If

            If GlobalsFunctions.validateStreet() = False Then
                Return False

            End If
            'Set up Street Layer parems
            m_AddressLayer = GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LayerName
            m_AddressFieldStreetName = GlobalsFunctions.appConfig.SearchPanel.AddressSearch.NameField
            m_AddressFieldLeftFrom = GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LowerLeft
            m_AddressFieldLeftTo = GlobalsFunctions.appConfig.SearchPanel.AddressSearch.UpperLeft
            m_AddressFieldRightFrom = GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LowerRight
            m_AddressFieldRightTo = GlobalsFunctions.appConfig.SearchPanel.AddressSearch.UpperRight
            'Check to make sure fields and layer exist
            m_bAddress_GeocordInit = CheckStreetLayer()
            'Return if the paremeters are invalid



            If GlobalsFunctions.appConfig.SearchPanel.AddressSearch.SDC.Path.Trim() <> "" And System.IO.File.Exists(GlobalsFunctions.appConfig.SearchPanel.AddressSearch.SDC.Path) Then

                Try

                    _smd = New StreetMapDataset(GlobalsFunctions.appConfig.SearchPanel.AddressSearch.SDC.Path)
                Catch ex As Exception
                    Dim st As New StackTrace
                    MsgBox("SDC" & st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                    st = Nothing
                End Try

                _streetMapLayer = New StreetMapLayer(_smd)
                _smd.Open()
                _streetMapLayer.Name = "SDC"

                txtState.Text = GlobalsFunctions.appConfig.SearchPanel.AddressSearch.SDC.DefaultState
                txtCity.Text = GlobalsFunctions.appConfig.SearchPanel.AddressSearch.SDC.DefaultCity
                txtStreet.Text = GlobalsFunctions.appConfig.SearchPanel.AddressSearch.SDC.DefaultStreetName
                txtStreetNum.Text = GlobalsFunctions.appConfig.SearchPanel.AddressSearch.SDC.DefaultStreetNum

                pnlSDCGC.Dock = DockStyle.Fill
                pnlSDCGC.Visible = True
                pnlMyGC.Dock = DockStyle.None
                pnlMyGC.Visible = False
                pnlOnlineGC.Dock = DockStyle.None
                pnlOnlineGC.Visible = False

            ElseIf GlobalsFunctions.appConfig.SearchPanel.AddressSearch.OnlineServices.Geocode.GCUrl <> "" Then
                pnlSDCGC.Dock = DockStyle.None
                pnlSDCGC.Visible = False
                pnlMyGC.Dock = DockStyle.None
                pnlMyGC.Visible = False
                pnlOnlineGC.Dock = DockStyle.Fill
                pnlOnlineGC.Visible = True

            Else
                pnlSDCGC.Dock = DockStyle.None
                pnlSDCGC.Visible = False
                pnlOnlineGC.Dock = DockStyle.None
                pnlOnlineGC.Visible = False

                pnlMyGC.Dock = DockStyle.Fill
                pnlMyGC.Visible = True

            End If
            If m_bAddress_GeocordInit = False Then Return False
            'Load the streets to the first drop down

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
            Return False
        End Try
        Return True
    End Function
    'Private WithEvents stDataWorker As New System.ComponentModel.BackgroundWorker
    'Private Shared cboStreetLayer1Shared As ComboBox
    'Private Shared cboStreetLayer2Shared As ComboBox

    'Private Sub loadStreetDataAsync(intPrefilt As Integer, cnt As String)
    '    cboStreetLayer1Shared = cboStreetLayer1
    '    cboStreetLayer2Shared = cboStreetLayer2


    '    pnlAddress.Enabled = False

    '    stDataWorker.RunWorkerAsync(New String() {intPrefilt.ToString, cnt})

    'End Sub

    'Private Sub stDataWorker_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles stDataWorker.DoWork

    '    Dim pArgs As String() = e.Argument

    '    'LoadPrefilters(CInt(pArgs(0)), pArgs(1).ToString())

    '    LoadStreetsPrimary()

    'End Sub
    'Private Sub stDataWorker_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles stDataWorker.RunWorkerCompleted
    '    pnlAddress.Enabled = True

    'End Sub
    Public Sub LoadData()

        'loadStreetDataAsync(-1, "Both")
        LoadPrefilters(-1, "Both")

        LoadStreetsPrimary()


    End Sub
    Public Function InitSearch() As Boolean

        Try
            cboSearchValue.Visible = False

            If m_Map Is Nothing Then
                m_bSearchInit = False
                Return False
            End If
            m_BuffMA = New mobileBufferMapAction
            m_BuffMA.ManualSetMap = m_Map

            m_BuffMA.initControl()

            cboBufferVal_SelectedIndexChanged(Nothing, Nothing)

            If (GlobalsFunctions.appConfig.SearchPanel.SearchLayers.SearchLayer Is Nothing) Then
                'Initilize the search function
                LoadLayers(New List(Of MobileConfigClass.MobileConfigMobileMapConfigSearchPanelSearchLayersSearchLayer)())

            Else
                'Initilize the search function
                LoadLayers(GlobalsFunctions.appConfig.SearchPanel.SearchLayers.SearchLayer)

            End If



            m_bSearchInit = True
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
            Return False
        End Try
        Return True
    End Function
    Public Sub ToggleAddressSearchButtons(ByVal visible As Boolean)
        btnAddress.Visible = visible
        btnGeocode.Visible = visible
    End Sub
    Public Function CreatePrefilterControls() As Boolean

        Dim pLblInt As Label
        Dim pLblSt As Label
        Dim pCboInt As ComboBox
        Dim pCboSt As ComboBox

        If m_AddressLayer Is Nothing Then Return False

        Dim x As Integer = 30
        Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = GlobalsFunctions.GetFeatureSource(m_AddressLayer, m_Map).FeatureSource
        If pFL Is Nothing Then
            MsgBox(String.Format(GlobalsFunctions.appConfig.ApplicationSettings.UIComponents.LayerNotFound, m_AddressLayer))


            Return False

        End If
        If GlobalsFunctions.appConfig.SearchPanel.AddressSearch.PrefilterFields.PrefilterField.Count = 0 Then Return False

        Dim boolAdded As Boolean = False
        Dim pFnt As System.Drawing.Font
        Dim g As Graphics = Nothing
        Dim intLeft As Integer = 0
        lstPreFilterInt = New List(Of ComboBox)
        lstPreFilterSt = New List(Of ComboBox)


        Dim intCBOIndx As Integer = 0
        For Each fieldName In GlobalsFunctions.appConfig.SearchPanel.AddressSearch.PrefilterFields.PrefilterField
            Dim pCol As DataColumn = pFL.Columns(fieldName.Name)

            If pCol IsNot Nothing Then

                pCboInt = New ComboBox
                If fieldName.AutoComplete.ToUpper = "TRUE" Then
                    pCboInt.DropDownStyle = ComboBoxStyle.DropDown
                    pCboInt.AutoCompleteMode = AutoCompleteMode.Suggest
                    pCboInt.AutoCompleteSource = AutoCompleteSource.ListItems
                Else
                    pCboInt.DropDownStyle = ComboBoxStyle.DropDownList

                End If

                pLblInt = New Label
                If fieldName.DisplayText <> "" Then
                    pLblInt.Text = fieldName.DisplayText
                Else
                    pLblInt.Text = pCol.Caption
                End If

                ' pCboInt.Tag = intCBOIndx & "|Int"
                ' pCboInt.Tag = fieldName
                pCboInt.Name = pCol.ColumnName & "|Int" & "|" & intCBOIndx
                pCboInt.Sorted = True

                pLblInt.Left = 5

                pFnt = pLblInt.Font
                g = pLblInt.CreateGraphics()
                pLblInt.Width = CInt(g.MeasureString(pLblInt.Text, pFnt).Width + 50)
                If pLblInt.Width > intLeft Then
                    intLeft = pLblInt.Width
                End If
                pCboInt.Left = pLblInt.Left + pLblInt.Width + 5
                pCboInt.Width = gpBoxPreFiltInt.Width - pCboInt.Left - 20
                pCboInt.Top = x

                pLblInt.Top = x
                pCboInt.Tag = fieldName
                AddHandler pCboInt.SelectedIndexChanged, AddressOf cboPreFilt_SelectChanged

                gpBoxPreFiltInt.Controls.Add(pCboInt)

                gpBoxPreFiltInt.Controls.Add(pLblInt)


                lstPreFilterInt.Add(pCboInt)

                g.Dispose()


                pCboSt = New ComboBox
                If fieldName.AutoComplete.ToUpper = "TRUE" Then

                    pCboSt.DropDownStyle = ComboBoxStyle.DropDown
                    pCboSt.AutoCompleteMode = AutoCompleteMode.Suggest
                    pCboSt.AutoCompleteSource = AutoCompleteSource.ListItems
                Else
                    pCboSt.DropDownStyle = ComboBoxStyle.DropDownList
                End If

                pLblSt = New Label
                If fieldName.DisplayText <> "" Then
                    pLblSt.Text = fieldName.DisplayText
                Else
                    pLblSt.Text = pCol.Caption
                End If
                ' pCboSt.Tag = intCBOIndx & "|St"
                pCboSt.Tag = fieldName
                pCboSt.Name = pCol.ColumnName & "|St" & "|" & intCBOIndx
                pCboSt.Sorted = True

                pLblSt.Left = 5

                pFnt = pLblSt.Font
                pLblSt.Width = pLblInt.Width
                If pLblSt.Width > intLeft Then
                    intLeft = pLblSt.Width
                End If
                pCboSt.Left = pLblSt.Left + pLblSt.Width + 5
                pCboSt.Width = gpBoxPreFiltInt.Width - pCboSt.Left - 20
                pCboSt.Top = x

                pLblSt.Top = x
                AddHandler pCboSt.SelectedIndexChanged, AddressOf cboPreFilt_SelectChanged

                gpBoxPreFiltSt.Controls.Add(pCboSt)

                gpBoxPreFiltSt.Controls.Add(pLblSt)


                lstPreFilterSt.Add(pCboSt)

                boolAdded = True
                x = x + pLblInt.Height + 10
                intCBOIndx = intCBOIndx + 1


            End If

        Next

        If g IsNot Nothing Then
            g.Dispose()
        End If

        pFnt = Nothing

        g = Nothing
        If boolAdded Then
            gpBoxPreFiltInt.Height = x + 12
            gpBoxPreFiltInt.Visible = True
            gpBoxPreFiltSt.Height = x + 12
            gpBoxPreFiltSt.Visible = True
            For Each pCbo As ComboBox In lstPreFilterSt
                pCbo.Left = intLeft
            Next
            For Each pCbo As ComboBox In lstPreFilterInt
                pCbo.Left = intLeft
            Next
        Else
            gpBoxPreFiltInt.Visible = False
            gpBoxPreFiltSt.Visible = False

        End If
    End Function
    Public Function CreateAddressPointControls(ByVal drillDownSearchLayer As MobileConfigClass.MobileConfigMobileMapConfigSearchPanelDrillDownSearchesDrillDownSearch) As Boolean

        Try

            gpBxAddPntControls.Controls.Clear()

            Dim pLblPnt As Label

            Dim pCboPnt As ComboBox

            Dim y As Integer = 10
            Dim pFsWDef As FeatureSourceWithDef = GlobalsFunctions.GetFeatureSource(drillDownSearchLayer.LayerName, m_Map)
            Dim pLayDef As MobileCacheMapLayerDefinition = GlobalsFunctions.GetLayerDefinition(m_Map, pFsWDef)

            If pFsWDef Is Nothing Then
                'MsgBox("The Address Point layer was not found with name of: " & m_AddressPointLayer)

                Return False

            End If
            If pFsWDef.FeatureSource Is Nothing Then
                'MsgBox("The Address Point layer was not found with name of: " & m_AddressPointLayer)

                Return False

            End If

            If drillDownSearchLayer.DefQuery <> "" Then
                pLayDef.DisplayExpression = drillDownSearchLayer.DefQuery

            End If
            Dim pFL As FeatureSource = pFsWDef.FeatureSource
            Dim boolAdded As Boolean = False
            Dim pFnt As System.Drawing.Font
            Dim g As Graphics
            Dim intLeft As Integer = 0
            lstPreFilterPnt = New List(Of ComboBox)

            If drillDownSearchLayer.PointFields Is Nothing Then Return False
            If drillDownSearchLayer.PointFields.PointField Is Nothing Then Return False
            If drillDownSearchLayer.PointFields.PointField.Count = 0 Then Return False


            Dim intCBOIndx As Integer = 0
            For Each fieldName In drillDownSearchLayer.PointFields.PointField
                Dim pCol As DataColumn = pFL.Columns(fieldName.Name)

                If pCol IsNot Nothing Then

                    pCboPnt = New ComboBox
                    If fieldName.AutoComplete.ToUpper = "TRUE" Then
                        pCboPnt.DropDownStyle = ComboBoxStyle.DropDown
                        pCboPnt.AutoCompleteMode = AutoCompleteMode.SuggestAppend

                        pCboPnt.AutoCompleteSource = AutoCompleteSource.ListItems
                    Else
                        pCboPnt.DropDownStyle = ComboBoxStyle.DropDownList


                    End If

                    pLblPnt = New Label
                    pLblPnt.Font = m_Font
                    If fieldName.DisplayText <> "" Then
                        pLblPnt.Text = fieldName.DisplayText
                    Else
                        pLblPnt.Text = pCol.Caption
                    End If

                    'pLblPnt.Text = pCol.Caption
                    ' pCboPnt.Tag = intCBOIndx & "|Pnt"
                    pCboPnt.Name = pCol.ColumnName & "|Pnt" & "|" & intCBOIndx
                    pCboPnt.Sorted = True

                    pLblPnt.Left = 5

                    pFnt = pLblPnt.Font
                    g = pLblPnt.CreateGraphics()
                    pLblPnt.Width = CInt(g.MeasureString(pLblPnt.Text, pFnt).Width + 25)
                    If pLblPnt.Width > intLeft Then
                        intLeft = pLblPnt.Width
                    End If
                    pCboPnt.Left = pLblPnt.Left + pLblPnt.Width + 5
                    pCboPnt.Width = pnlAddressPoint.Width - pCboPnt.Left - 20
                    pCboPnt.Top = y

                    pLblPnt.Top = y
                    AddHandler pCboPnt.SelectedIndexChanged, AddressOf cboAddPnt_SelectChanged

                    gpBxAddPntControls.Controls.Add(pCboPnt)

                    gpBxAddPntControls.Controls.Add(pLblPnt)


                    lstPreFilterPnt.Add(pCboPnt)

                    g.Dispose()
                    g = Nothing


                    boolAdded = True
                    y = y + pLblPnt.Height + 10
                    intCBOIndx = intCBOIndx + 1


                End If

            Next

            If g IsNot Nothing Then
                g.Dispose()
            End If

            pFnt = Nothing

            g = Nothing
            If boolAdded Then
                gpBxAddPntControls.Height = y + 12
                gpBxAddPntControls.Visible = True
                'pnlAddressPoint.Visible = True
                For Each pCbo As ComboBox In lstPreFilterPnt
                    pCbo.Left = intLeft
                Next


            End If
            Return True
        Catch ex As Exception
            Return False

        End Try
    End Function

#End Region
#Region "Properties"
    Public WriteOnly Property map() As Esri.ArcGIS.Mobile.WinForms.Map

        Set(ByVal value As Esri.ArcGIS.Mobile.WinForms.Map)
            m_Map = value
        End Set
    End Property

#End Region
#Region "Events"
    Private Sub pnlSearch_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pnlSearch.MouseDown
        Collapse_Expend_GroupBox(sender, e.Location)

    End Sub
    Private Sub gpBxActGoToXY_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles gpBxGoToXY.MouseDown
        Collapse_Expend_GroupBox(sender, e.Location)

    End Sub
    Private Sub Collapse_Expend_GroupBox(ByVal sender As Object, ByVal ClickLocation As System.Drawing.Point)
        Dim g As Graphics = CType(sender, GroupBox).CreateGraphics
        Dim S As SizeF = g.MeasureString(CType(sender, GroupBox).Text, CType(sender, GroupBox).Font)
        If ClickLocation.IsEmpty Then
            If CType(sender, GroupBox).Height > S.Height + 1 Then


                CType(sender, GroupBox).Height = CInt(S.Height)
                Dim x As Integer = 0
                For Each cnt As Control In CType(sender, GroupBox).Parent.Controls
                    If cnt.Visible = True Then
                        If cnt.Top + cnt.Height > x Then
                            x = cnt.Top + cnt.Height
                        End If
                    End If

                Next
                If x = 0 Then
                    x = CInt(S.Height + 10)
                End If
                CType(sender, GroupBox).Parent.Height = x + CInt(S.Height) - 10
            Else
                Dim x As Integer = 0
                For Each cnt As Control In CType(sender, GroupBox).Controls
                    If cnt.Visible = True Then
                        If cnt.Top + cnt.Height > x Then
                            x = cnt.Top + cnt.Height
                        End If
                    End If

                Next
                If x = 0 Then
                    x = CInt(S.Height + 10)
                End If
                CType(sender, GroupBox).Height = x + 10
                CType(sender, GroupBox).Parent.Height = x + CType(sender, GroupBox).Parent.Height - 10

            End If
        ElseIf ClickLocation.Y < S.Height + 1 Then

            If CType(sender, GroupBox).Height > S.Height + 1 Then


                CType(sender, GroupBox).Height = CInt(S.Height)
                Dim x As Integer = 0
                For Each cnt As Control In CType(sender, GroupBox).Parent.Controls
                    If cnt.Visible = True Then
                        If cnt.Top + cnt.Height > x Then
                            x = cnt.Top + cnt.Height
                        End If
                    End If

                Next
                If x = 0 Then
                    x = CInt(S.Height + 10)
                End If
                CType(sender, GroupBox).Parent.Height = x + CInt(S.Height) - 10
            Else
                Dim x As Integer = 0
                For Each cnt As Control In CType(sender, GroupBox).Controls
                    If cnt.Visible = True Then
                        If cnt.Top + cnt.Height > x Then
                            x = cnt.Top + cnt.Height
                        End If
                    End If

                Next
                If x = 0 Then
                    x = CInt(S.Height + 10)
                End If
                CType(sender, GroupBox).Height = x + 10
                CType(sender, GroupBox).Parent.Height = x + CType(sender, GroupBox).Parent.Height - 10

            End If
        End If
        g = Nothing
        S = Nothing
    End Sub


    Private Sub cboPreFilt_SelectChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        RaiseEvent showIndicator(True)

        Dim pCBO As ComboBox = sender
        Dim tagVal() As String = pCBO.Name.ToString.Split("|")

        If pCBO.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText Then
            LoadPrefilters(CInt(tagVal(2)), tagVal(1))
        Else
            LoadPrefilters(CInt(tagVal(2)), tagVal(1))

        End If
        If tagVal(1) = "Int" Then
            LoadStreetsPrimary()
        End If

        RaiseEvent showIndicator(False)

    End Sub
    Private Sub cboAddPnt_SelectChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        RaiseEvent showIndicator(True)

        Dim pCBO As ComboBox = sender
        Dim tagVal() As String = pCBO.Name.ToString.Split("|")

        If pCBO.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText Then
            LoadValuesDrillDownSearch(CInt(tagVal(2)) - 1)
        Else
            LoadValuesDrillDownSearch(CInt(tagVal(2)))

        End If

        AddPntLocate(True)
        ' LoadAddPoints()
        RaiseEvent showIndicator(False)

    End Sub
    Private Sub btnLocateOnMap_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLocateOnMap.Click
        If dgResults.Rows IsNot Nothing Then
            Try
                dgResults.Rows.Clear()
            Catch ex As Exception

            End Try

        End If
        dgResults.DataSource = Nothing
        dgResults.Columns.Clear()
        'ptLocated = new ESRI.ArcGIS.Mobile.Geometries.Point()
        Try

            If _streetMapLayer.StreetMapDataset.Locators.Count = 0 Then
                'MsgBox("The SDC data is not configured correctly")

                Return

            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


            Return

        End Try


        ' Get the StreetName locator by City and State
        Dim streetLocator As Locator = _streetMapLayer.StreetMapDataset.Locators(0)

        streetLocator.Reset()
        ' Get the first filter
        Dim _stateFilter As LocatorFilter = streetLocator.GetNextFilter()
        _stateFilter.AppendToFilter(txtState.Text.Trim())

        ' Gets the city filter
        Dim _cityFilter As LocatorFilter = streetLocator.GetNextFilter()
        _cityFilter.AppendToFilter(txtCity.Text.Trim())

        ' Gets the street filter
        Dim _streetFilter As LocatorFilter = streetLocator.GetNextFilter()
        _streetFilter.AppendToFilter(txtStreet.Text.Trim())

        ' Gets the housenumber filter
        Dim _houseNumberFilter As LocatorFilter = streetLocator.GetNextFilter()
        _houseNumberFilter.RemoveAllCharacters()
        _houseNumberFilter.AppendToFilter(txtStreetNum.Text.Trim())

        ' get the result

        If (_houseNumberFilter.GetResults().Count() <= 0) Then

            MessageBox.Show("Cannot find this address !")
            Return
        End If
        'Dim spatRefConv As SpatialReferences.SpatialReferenceConverter = New SpatialReferences.SpatialReferenceConverter(m_Map.SpatialReference, _smd.SpatialReference, New SpatialReferences.GeoTransformation(m_Map.SpatialReference))

        dgResults.Columns.Add("colLabel", "Address")
        'dgResults.Columns.Add("colType", "Type")
        'dgResults.Columns.Add("colCoord", "Coords")

        dgResults.Columns.Add("colCoordX", "CoordsX")
        dgResults.Columns.Add("colCoordY", "CoordsY")
        dgResults.Columns(1).Visible = False
        dgResults.Columns(2).Visible = False
        ' only get the first one
        Dim result As LocatorResult '= _houseNumberFilter.GetResults().First()
        For Each result In _houseNumberFilter.GetResults()
            If result IsNot Nothing Then
                Dim candidate As Esri.ArcGIS.Mobile.DataProducts.StreetMapData.Location
                For Each candidate In result.GetLocationCandidates()
                    'Dim candidate As Location = result.GetLocationCandidates().First()
                    If candidate IsNot Nothing Then
                        'MsgBox(candidate.Coordinate.ToString)
                        '                Dim pTransCoord As Coordinate = spatRefConv.FromSpatialReference2ToSpatialReference1(candidate.Coordinate)
                        Dim pTransCoord As Coordinate = m_Map.SpatialReference.FromWgs84(_smd.SpatialReference.ToWgs84(candidate.Coordinate))

                        dgResults.Rows.Add(New String() {candidate.Label.ToString, pTransCoord.X.ToString, pTransCoord.Y.ToString})
                    End If

                Next

            End If
        Next

        'If result IsNot Nothing Then
        '    Dim candidate As Location = result.GetLocationCandidates().First()
        '    If candidate IsNot Nothing Then
        '        'MsgBox(candidate.Coordinate.ToString)
        '        Dim spatRefConv As SpatialReferences.SpatialReferenceConverter = New SpatialReferences.SpatialReferenceConverter(m_Map.SpatialReference, _smd.SpatialReference)
        '        Dim pTransCoord As Coordinate = spatRefConv.FromSpatialReference1ToSpatialReference2(candidate.Coordinate)
        '        dgResults.Rows.Add(New String() {candidate.Label.ToString, candidate.LocationType.ToString, candidate.Coordinate.ToString})





        '    End If

        'End If

        streetLocator.Reset()
        'Show the results
        splContMainSearch.Panel2Collapsed = False
        Call MobileSearch_Resize(Nothing, Nothing)
    End Sub
    Private Sub ctxZoomTo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            ZoomFlashRouteGC(recordClickType.zoom)
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try
    End Sub
    Private Sub ctxFlash_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            ZoomFlashRouteGC(recordClickType.flash)
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try
    End Sub

    Private Sub btnRouteTo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDGRouteTo.Click
        ZoomFlashRouteGC(recordClickType.Route)
    End Sub
    Private Sub cboSearchLayer_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboSearchLayer.SelectedIndexChanged
        Try
            'Loads the fields for a new layer
            If LoadFields(cboSearchLayer.SelectedItem) = False Then Return
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try

    End Sub
    Private Sub MobileSearch_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        Try
            If lstPreFilterInt IsNot Nothing Then
                lstPreFilterInt.Clear()
                lstPreFilterInt = Nothing

            End If
            If lstPreFilterSt IsNot Nothing Then
                lstPreFilterSt.Clear()
                lstPreFilterSt = Nothing

            End If
            If m_Map IsNot Nothing Then
                m_Map.Dispose()
            End If
            m_Map = Nothing

            If m_penLine IsNot Nothing Then
                '   m_penLine.Dispose()
            End If
            m_penLine = Nothing

            If m_pen IsNot Nothing Then
                '   m_pen.Dispose()
            End If
            m_pen = Nothing

            If m_brush IsNot Nothing Then
                m_brush.Dispose()
            End If
            m_brush = Nothing

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub MobileSearch_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            'Reset  settings used to help debug
            lblNumResults.Text = ""
            lblGCNumResults.Text = ""
            pnlSearch.Visible = True
            pnlAddress.Visible = False
            pnlGeocode.Visible = False
            splContMainSearch.Panel2Collapsed = True
            Collapse_Expend_GroupBox(gpBxGoToXY, New System.Drawing.Point())

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub MobileSearch_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        Try
            'recenter the buttons and the splitter distance on a resize
            splContMainSearch.SplitterDistance = 385
            'Search panel
            cboSearchField.Width = splContMainSearch.Width - cboSearchField.Left - m_Rightpad
            cboSearchLayer.Width = splContMainSearch.Width - cboSearchLayer.Left - m_Rightpad
            txtSearchValue.Width = splContMainSearch.Width - txtSearchValue.Left - m_Rightpad
            cboSearchValue.Left = txtSearchValue.Left
            cboSearchValue.Width = splContMainSearch.Width - txtSearchValue.Left - m_Rightpad
            cboBufferVal.Width = splContMainSearch.Width - cboBufferVal.Left - m_Rightpad - btnBuffer.Width - 10
            btnBuffer.Left = cboBufferVal.Left + cboBufferVal.Width + 5


            Dim intBtn As Integer = 0

            If btnLookup.Visible Then
                intBtn = intBtn + 1
            End If
            If btnAddress.Visible Then
                intBtn = intBtn + 1
            End If

            If btnAddressPnt.Visible Then
                intBtn = intBtn + 1
            End If
            If btnGeocode.Visible Then
                intBtn = intBtn + 1
            End If

            Dim splDis As Double = splContMainSearch.Width / (intBtn + 1)
            Dim intNextLeft As Integer = CInt(splDis)

            If btnLookup.Visible Then
                btnLookup.Left = CInt(intNextLeft - btnLookup.Width / 2)
                intNextLeft = CInt(intNextLeft + splDis)
            End If
            If btnAddress.Visible Then
                btnAddress.Left = CInt(intNextLeft - btnAddress.Width / 2)
                intNextLeft = CInt(intNextLeft + splDis)
            End If

            If btnAddressPnt.Visible Then
                btnAddressPnt.Left = CInt(intNextLeft - btnAddressPnt.Width / 2)
                intNextLeft = CInt(intNextLeft + splDis)
            End If
            If btnGeocode.Visible Then
                btnGeocode.Left = CInt(intNextLeft - btnGeocode.Width / 2)
                intNextLeft = CInt(intNextLeft + splDis)
            End If

            'Address Panel
            cboStreetLayer1.Width = splContMainSearch.Width - cboStreetLayer1.Left - m_Rightpad
            cboStreetLayer2.Width = splContMainSearch.Width - cboStreetLayer2.Left - m_Rightpad
            cboStreetRange.Width = splContMainSearch.Width - cboStreetRange.Left - m_Rightpad





            Dim visCnt As Integer = 0

            For Each Control As Control In gpInterSearch.Controls
                If TypeOf (Control) Is Button Then
                    If Control.Visible Then
                        visCnt = visCnt + 1
                    End If


                End If

            Next
            Dim spacing As Double = (gpInterSearch.Width) / (visCnt + 1)
            Dim curloc As Double = spacing
            For Each Control As Control In gpInterSearch.Controls
                If TypeOf (Control) Is Button Then
                    If Control.Visible Then
                        Control.Left = curloc - (Control.Width / 2)

                        curloc = curloc + spacing
                    End If


                End If

            Next

            'btnAddressRouteTo.Left = CInt((splContMainSearch.Width / 2) - (btnAddressRouteTo.Width / 2))
            'btnAddressZoomTo.Left = btnAddressRouteTo.Left - btnAddressZoomTo.Width - m_ButtonSpace
            'btnAddressReload.Left = btnAddressRouteTo.Left + btnAddressRouteTo.Width + m_ButtonSpace






            'GC Panel
            txtGeocodeValue.Width = splContMainSearch.Width - txtGeocodeValue.Left - m_Rightpad
            txtbxOnlineGCAddress.Width = splContMainSearch.Width - txtbxOnlineGCAddress.Left - m_Rightpad


            btnDGRouteTo.Left = CInt(btnDGRouteTo.Parent.Width / 2 - btnDGRouteTo.Width / 2)
            btnDGZoomTo.Left = 10
            btnDGFlash.Left = btnDGFlash.Parent.Width - 10 - btnDGFlash.Width


            resizeDGandScrolls()

            If lstPreFilterSt IsNot Nothing Then



                For Each pCbo As ComboBox In lstPreFilterSt
                    If Me.Width < pCbo.Parent.Width Then
                        pCbo.Width = Me.Width - pCbo.Left - 15
                    Else
                        pCbo.Width = pCbo.Parent.Width - pCbo.Left - 15

                    End If

                Next
            End If
            If lstPreFilterInt IsNot Nothing Then


                For Each pCbo As ComboBox In lstPreFilterInt
                    If Me.Width < pCbo.Parent.Width Then
                        pCbo.Width = Me.Width - pCbo.Left - 15
                    Else
                        pCbo.Width = pCbo.Parent.Width - pCbo.Left - 15

                    End If
                Next
            End If
            resizeDrillDown()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub

    'Private Sub MobileSearch_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
    '    Try
    '        'recenter the buttons and the splitter distance on a resize
    '        splContMainSearch.SplitterDistance = 385
    '        'Search panel
    '        cboSearchField.Width = splContMainSearch.Width - cboSearchField.Left - m_Rightpad
    '        cboSearchLayer.Width = splContMainSearch.Width - cboSearchLayer.Left - m_Rightpad
    '        txtSearchValue.Width = splContMainSearch.Width - txtSearchValue.Left - m_Rightpad
    '        cboSearchValue.Left = txtSearchValue.Left
    '        cboSearchValue.Width = splContMainSearch.Width - txtSearchValue.Left - m_Rightpad
    '        cboBufferVal.Width = splContMainSearch.Width - cboBufferVal.Left - m_Rightpad - btnBuffer.Width - 10
    '        btnBuffer.Left = cboBufferVal.Left + cboBufferVal.Width + 5


    '        Dim intBtn As Integer = 0

    '        If btnLookup.Visible Then
    '            intBtn = intBtn + 1
    '        End If
    '        If btnAddress.Visible Then
    '            intBtn = intBtn + 1
    '        End If

    '        If btnAddressPnt.Visible Then
    '            intBtn = intBtn + 1
    '        End If
    '        If btnGeocode.Visible Then
    '            intBtn = intBtn + 1
    '        End If

    '        Dim splDis As Double = splContMainSearch.Width / (intBtn + 1)
    '        Dim intNextLeft As Integer = CInt(splDis)

    '        If btnLookup.Visible Then
    '            btnLookup.Left = CInt(intNextLeft - btnLookup.Width / 2)
    '            intNextLeft = CInt(intNextLeft + splDis)
    '        End If
    '        If btnAddress.Visible Then
    '            btnAddress.Left = CInt(intNextLeft - btnAddress.Width / 2)
    '            intNextLeft = CInt(intNextLeft + splDis)
    '        End If

    '        If btnAddressPnt.Visible Then
    '            btnAddressPnt.Left = CInt(intNextLeft - btnAddressPnt.Width / 2)
    '            intNextLeft = CInt(intNextLeft + splDis)
    '        End If
    '        If btnGeocode.Visible Then
    '            btnGeocode.Left = CInt(intNextLeft - btnGeocode.Width / 2)
    '            intNextLeft = CInt(intNextLeft + splDis)
    '        End If


    '        'If btnAddressPnt.Visible Then
    '        '    'btnAddress.Left = CInt((splContMainSearch.Width / 2) - btnAddress.Width - 10)
    '        '    'btnAddressPnt.Left = CInt((splContMainSearch.Width / 2) + 10)

    '        '    'btnGeocode.Left = btnAddressPnt.Left + btnAddressPnt.Width + 20
    '        '    'btnLookup.Left = btnAddress.Left - 20 - btnAddress.Width
    '        '    Dim btnDis As Double = btnAddress.Parent.Width / 5

    '        '    btnLookup.Left = btnDis - (btnLookup.Width / 2)

    '        '    btnAddress.Left = (btnDis + btnDis) - (btnAddress.Width / 2)

    '        '    btnAddressPnt.Left = (btnDis + btnDis + btnDis) - (btnAddressPnt.Width / 2)

    '        '    btnGeocode.Left = (btnDis + btnDis + btnDis + btnDis) - (btnGeocode.Width / 2)

    '        'Else
    '        '    Dim btnDis As Double = btnAddress.Parent.Width / 4

    '        '    btnLookup.Left = btnDis - (btnLookup.Width / 2)

    '        '    btnAddress.Left = (btnDis + btnDis) - (btnAddress.Width / 2)

    '        '    btnGeocode.Left = (btnDis + btnDis + btnDis) - (btnGeocode.Width / 2)

    '        'End If





    '        'btnLookup.Left = btnAddress.Left - m_ButtonSpace - btnLookup.Width
    '        'btnGeocode.Left = btnAddress.Left + m_ButtonSpace + btnAddress.Width
    '        'btnAddressPnt.Left = btnAddress.Left + m_ButtonSpace + btnAddress.Width
    '        'Address Panel
    '        cboStreetLayer1.Width = splContMainSearch.Width - cboStreetLayer1.Left - m_Rightpad
    '        cboStreetLayer2.Width = splContMainSearch.Width - cboStreetLayer2.Left - m_Rightpad
    '        cboStreetRange.Width = splContMainSearch.Width - cboStreetRange.Left - m_Rightpad
    '        btnAddressRouteTo.Left = CInt((splContMainSearch.Width / 2) - (btnAddressRouteTo.Width / 2))
    '        btnAddressZoomTo.Left = btnAddressRouteTo.Left - btnAddressZoomTo.Width - m_ButtonSpace
    '        btnAddressReload.Left = btnAddressRouteTo.Left + btnAddressRouteTo.Width + m_ButtonSpace
    '        'GC Panel
    '        txtGeocodeValue.Width = splContMainSearch.Width - txtGeocodeValue.Left - m_Rightpad
    '        txtbxOnlineGCAddress.Width = splContMainSearch.Width - txtbxOnlineGCAddress.Left - m_Rightpad
    '        'DG buttons
    '        'btnRouteTo.Top = CInt((btnRouteTo.Parent.Height / 2) - (btnRouteTo.Height / 2) + 3)
    '        'btnRouteTo.Left = CInt((splContMainSearch.Width / 2) - (btnRouteTo.Width) / 2)

    '        'btnDGZoomTo.Top = CInt((btnDGZoomTo.Parent.Height / 2) - (btnDGZoomTo.Height / 2) + 3)
    '        'btnDGZoomTo.Left = btnRouteTo.Left - btnDGZoomTo.Width - m_ButtonSpace

    '        'btnIDEvent.Top = CInt((btnRouteTo.Parent.Height / 2) - (btnRouteTo.Height / 2) + 3)
    '        'btnIDEvent.Left = btnRouteTo.Left + btnIDEvent.Width + m_ButtonSpace



    '        'btnDGZoomTo.Top = CInt((btnDGZoomTo.Parent.Height / 2) - (btnDGZoomTo.Height / 2) + 3)
    '        'btnDGZoomTo.Left = m_ButtonSpace

    '        'btnDGFlash.Top = CInt((btnDGRouteTo.Parent.Height / 2) - (btnDGRouteTo.Height / 2) + 3)
    '        'btnDGFlash.Left = btnDGZoomTo.Left + btnDGZoomTo.Width + m_ButtonSpace


    '        ''btnIDEvent.Top = CInt((btnRouteTo.Parent.Height / 2) - (btnRouteTo.Height / 2) + 3)
    '        ''btnIDEvent.Left = btnDGFlash.Left + btnDGFlash.Width + m_ButtonSpace

    '        'btnDGRouteTo.Top = CInt((btnDGRouteTo.Parent.Height / 2) - (btnDGRouteTo.Height / 2) + 3)
    '        'btnDGRouteTo.Left = btnDGFlash.Left + btnDGFlash.Width + m_ButtonSpace

    '        btnDGRouteTo.Left = CInt(btnDGRouteTo.Parent.Width / 2 - btnDGRouteTo.Width / 2)
    '        btnDGZoomTo.Left = 10
    '        btnDGFlash.Left = btnDGFlash.Parent.Width - 10 - btnDGFlash.Width


    '        resizeDGandScrolls()

    '        If lstPreFilterSt IsNot Nothing Then



    '            For Each pCbo As ComboBox In lstPreFilterSt
    '                If Me.Width < pCbo.Parent.Width Then
    '                    pCbo.Width = Me.Width - pCbo.Left - 15
    '                Else
    '                    pCbo.Width = pCbo.Parent.Width - pCbo.Left - 15

    '                End If

    '            Next
    '        End If
    '        If lstPreFilterInt IsNot Nothing Then


    '            For Each pCbo As ComboBox In lstPreFilterInt
    '                If Me.Width < pCbo.Parent.Width Then
    '                    pCbo.Width = Me.Width - pCbo.Left - 15
    '                Else
    '                    pCbo.Width = pCbo.Parent.Width - pCbo.Left - 15

    '                End If
    '            Next
    '        End If
    '        resizeDrillDown()
    '    Catch ex As Exception
    '        Dim st As New StackTrace
    '        MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
    '        st = Nothing

    '    End Try
    'End Sub

    Private Sub resizeDrillDown()
        If lstPreFilterPnt IsNot Nothing Then



            For Each pCbo As ComboBox In lstPreFilterPnt
                If Me.Width < pCbo.Parent.Width Then
                    pCbo.Width = Me.Width - pCbo.Left - 15
                Else
                    pCbo.Width = pCbo.Parent.Width - pCbo.Left - 15

                End If

            Next
        End If
    End Sub
    Private Sub resizeDGandScrolls()
        If dgResults.RowCount > 0 Then
            If dgResults.DisplayedRowCount(False) < dgResults.RowCount Then

                'VScrollBar1.Visible = True
                'VScrollBar1.Value = dgResults.CurrentRow.Index
                'VScrollBar1.LargeChange = dgResults.DisplayedRowCount(False)
                'dgResults.Width = VScrollBar1.Left - dgResults.Left
            Else
                'VScrollBar1.Visible = False
                dgResults.Width = dgResults.Parent.Width - dgResults.Left - dgResults.Left
            End If
        End If
        If dgResults.ColumnCount > 0 Then

            If dgResults.DisplayedColumnCount(False) < getVisibleColumns() Then
                'HScrollBar1.Maximum = getVisibleColumns()
                'HScrollBar1.Visible = True
                '   HScrollBar1.Value = dgResults.CurrentCell.ColumnIndex
                '   HScrollBar1.LargeChange = dgResults.DisplayedColumnCount(False)

                'dgResults.Height = HScrollBar1.Top - dgResults.Top
            Else
                'HScrollBar1.Visible = False
                dgResults.Height = dgResults.Parent.Height - dgResults.Top - dgResults.Top
            End If
        End If
    End Sub

    Private Sub cboStreetLayer1_DataSourceChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboStreetLayer1.DataSourceChanged
        '  MsgBox("")

    End Sub

    Private Sub cboStreetLayer1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboStreetLayer1.SelectedIndexChanged
        Try
            'Load the streets ranges and interesection
            RaiseEvent showIndicator(True)

            LoadStreetsIntersection(cboStreetLayer1.Text)
            LoadStreetsRange(cboStreetLayer1.Text)
            RaiseEvent showIndicator(False)

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub cboStreetRange_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboStreetRange.Click
        'Change the radio button to show the range is the search type
        rdoAddressRange.Checked = True
    End Sub
    Private Sub cboStreetLayer2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboStreetLayer2.Click
        'Change the radio button to show the intersection is the search type

        rdoAddressStreet2.Checked = True
    End Sub
    Private Sub btnWaypointDrillDown_Click(sender As System.Object, e As System.EventArgs) Handles btnWaypointDrillDown.Click
        Try



            'Get the Street Layer
            'Dim pML As MobileCacheMapLayer
            'pML = CType(m_Map.MapLayers(GlobalsFunctions.appConfig.SearchPanel.DrillDownSearches.DrillDownSearch(cboDrillDownLayer.SelectedIndex).LayerName), MobileCacheMapLayer)
            ''If not found return
            'If pML Is Nothing Then Return
            Dim pFL As FeatureSourceWithDef = GlobalsFunctions.GetFeatureSource(GlobalsFunctions.appConfig.SearchPanel.DrillDownSearches.DrillDownSearch(cboDrillDownLayer.SelectedIndex).LayerName, m_Map)
            If pFL Is Nothing Then Return
            'Get all streets names
            Dim pFDT As FeatureDataTable
            Dim strSQL As String = ""
            Dim queryFilt As QueryFilter = New QueryFilter

            queryFilt.WhereClause = GenerateSQLCombo(pFL.FeatureSource, "Pnt")


            'End If
            pFDT = pFL.FeatureSource.GetDataTable(queryFilt)
            lblMatchingResults.Text = String.Format(GlobalsFunctions.appConfig.SearchPanel.UIComponents.DrillDownResultsText, pFDT.Rows.Count)

            If pFDT.Rows.Count = 0 Then

                Return

            End If
            '   dgResults.DataSource = pFDT
            '   splContMainSearch.Panel2Collapsed = True

            RaiseEvent Waypoint(CType(pFDT.Rows(0), FeatureDataRow).Geometry, "")


        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub

    Private Sub btnWaypointGC_Click(sender As System.Object, e As System.EventArgs) Handles btnWaypointGC.Click
        ZoomFlashRouteGC(recordClickType.Waypoint)

    End Sub
    Private Sub btnRunGC_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRunGC.Click
        Try
            RemoveHandler dgResults.SelectionChanged, AddressOf dgResults_SelectionChanged

            'Call the functions to geocode the address
            'exit if the address is not set up
            If m_bAddress_GeocordInit = False Then Return
            'Call a function to parse the address for the indiviual elements
            Dim pAddressParse As AddressParser = New AddressParser(txtGeocodeValue.Text)
            ''Gecode the address

            Dim pFLDT As FeatureDataTable
            If pAddressParse.getStreetNumber = "" Then
                pFLDT = Geocode(pAddressParse.getStreetName)
            Else
                pFLDT = Geocode(pAddressParse.getStreetNumber, pAddressParse.getStreetName)

            End If


            'If nothing was found, update the label and return
            If pFLDT Is Nothing Then
                lblGCNumResults.Text = String.Format(GlobalsFunctions.appConfig.SearchPanel.UIComponents.GeocodeResultsText, "0")

                dgResults.DataSource = Nothing

                Return
            Else
                If pFLDT.Rows.Count = 0 Then
                    lblGCNumResults.Text = String.Format(GlobalsFunctions.appConfig.SearchPanel.UIComponents.GeocodeResultsText, "0")
                    dgResults.DataSource = Nothing

                    Return

                Else
                    'update the label with the number of records found
                    lblGCNumResults.Text = String.Format(GlobalsFunctions.appConfig.SearchPanel.UIComponents.GeocodeResultsText, pFLDT.Rows.Count)
                End If
            End If
            addIndex(pFLDT)

            'bind the results to the grid
            dgResults.DataSource = pFLDT
            dgResults.Columns(dgResults.Columns.Count - 1).Visible = False

            'VScrollBar1.Maximum = pFLDT.Rows.Count - 1
            'HScrollBar1.Maximum = pFLDT.Columns.Count
            'Set the tag with the street number so the user can zoom to the GC point location
            dgResults.Tag = pAddressParse.getStreetNumber
            'Show the results
            splContMainSearch.Panel2Collapsed = False
            Call MobileSearch_Resize(Nothing, Nothing)
            ZoomFlashRouteGC(recordClickType.zoom)

        Catch ex As Exception
            'MsgBox("Error in the Run Click event" & vbCrLf & ex.Message)
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        Finally
            AddHandler dgResults.SelectionChanged, AddressOf dgResults_SelectionChanged

        End Try
    End Sub
    Private Sub addIndex(ByRef FLDT As FeatureDataTable)
        Try

            Dim colInt32 As DataColumn = New DataColumn("INDEX_CODE_TEMP")
            colInt32.DataType = System.Type.GetType("System.Int32")

            FLDT.Columns.Add(colInt32)


            For ind As Integer = 0 To FLDT.Rows.Count - 1
                FLDT.Rows(ind)(colInt32) = ind


            Next
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Public Event Waypoint(ByVal location As Geometry, ByVal LocationName As String)
    Private Sub btnAddressZoomTo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddressZoomTo.Click, btnWaypoint.Click
        Try
            'Zooms to intersecting streets

            'Exit if the tool is not initilized
            If m_bAddress_GeocordInit = False Then Return

            'Determine type of search
            If rdoAddressStreet2.Checked = True Then
                'Make sure a valid value is selected
                If cboStreetLayer2.SelectedIndex < 0 Then Return

                'Find the intersection streets
                Dim pPnt As Esri.ArcGIS.Mobile.Geometries.Point
                Dim pGeoStreets As Geometry = getStreetsIntersectionGeom(cboStreetLayer1.Text, cboStreetLayer2.Text, pPnt)
                'Verify the intersection was found
                If pGeoStreets IsNot Nothing Then
                    If pGeoStreets.IsEmpty = False Then
                        If CType(sender, Button).Name.Contains("Waypoint") Then
                            RaiseEvent Waypoint(pGeoStreets, (cboStreetLayer1.Text & ": " & cboStreetLayer2.Text))

                        Else
                            'Zoom and flash it
                            GlobalsFunctions.zoomTo(pGeoStreets, m_Map)
                            If pGeoStreets.GeometryType = GeometryType.Point Then
                                GlobalsFunctions.flashGeo(pGeoStreets, m_Map, m_penLine, CType(m_brush, SolidBrush))
                            Else
                                GlobalsFunctions.flashGeo(pGeoStreets, m_Map, m_pen, m_brush)

                            End If
                        End If
                    End If
                    End If
                'cleanup
                pGeoStreets = Nothing
            Else
                If cboStreetRange.SelectedIndex < 0 Or cboStreetLayer1.SelectedIndex < 0 Then Return
                'Get the range dataset
                Dim pDT As FeatureDataTable = CType(cboStreetRange.DataSource, FeatureDataTable)
                If pDT IsNot Nothing Then
                    'Get the selected value
                    Dim pDRV As DataRowView = CType(cboStreetRange.SelectedItem, DataRowView)

                    Dim pDR As FeatureDataRow = CType(pDRV.Row, FeatureDataRow)

                    'Verify it is a valid row
                    If pDR IsNot Nothing Then
                        If pDR.Geometry IsNot Nothing Then
                            If pDR.Geometry.IsEmpty = False Then
                                If CType(sender, Button).Name.Contains("Waypoint") Then
                                    RaiseEvent Waypoint(pDR.Geometry, (cboStreetRange.Text & ": " & cboStreetLayer1.Text))

                                Else
                                    GlobalsFunctions.zoomTo(pDR.Geometry, m_Map)
                                    If pDR.Geometry.GeometryType = GeometryType.Point Then
                                        GlobalsFunctions.flashGeo(pDR.Geometry, m_Map, m_penLine, CType(m_brush, SolidBrush))
                                    Else
                                        GlobalsFunctions.flashGeo(pDR.Geometry, m_Map, m_pen, m_brush)

                                    End If
                                End If
                            End If

                        End If

                    End If
                    pDR = Nothing
                End If


                pDT = Nothing
            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try


    End Sub
    Private Sub btnAddressPointZoomTo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddressPointZoomTo.Click
        Try



            AddPntLocate(False)


        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try


    End Sub
    Private Sub AddPntLocate(ByVal displayOnly As Boolean)
        'Get the Street Layer
        'Dim pML As MobileCacheMapLayer
        'pML = CType(m_Map.MapLayers(GlobalsFunctions.appConfig.SearchPanel.DrillDownSearches.DrillDownSearch(cboDrillDownLayer.SelectedIndex).LayerName), MobileCacheMapLayer)
        ''If not found return
        'If pML Is Nothing Then Return
        Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = GlobalsFunctions.GetFeatureSource(GlobalsFunctions.appConfig.SearchPanel.DrillDownSearches.DrillDownSearch(cboDrillDownLayer.SelectedIndex).LayerName, m_Map).FeatureSource
        If pFL Is Nothing Then Return
        'Get all streets names
        Dim pFDT As FeatureDataTable
        Dim strSQL As String = ""
        Dim queryFilt As QueryFilter = New QueryFilter

        queryFilt.WhereClause = GenerateSQLCombo(pFL, "Pnt")


        'End If
        pFDT = pFL.GetDataTable(queryFilt)
        lblMatchingResults.Text = String.Format(GlobalsFunctions.appConfig.SearchPanel.UIComponents.DrillDownResultsText, pFDT.Rows.Count)

        If pFDT.Rows.Count = 0 Then

            Return

        End If
        '   dgResults.DataSource = pFDT
        '   splContMainSearch.Panel2Collapsed = True

        If (Not displayOnly) Then
            GlobalsFunctions.zoomTo(CType(pFDT.Rows(0), FeatureDataRow).Geometry, m_Map)
            If CType(pFDT.Rows(0), FeatureDataRow).Geometry.GeometryType = GeometryType.Point Then
                GlobalsFunctions.flashGeo(CType(pFDT.Rows(0), FeatureDataRow).Geometry, m_Map, m_penLine, CType(m_brush, SolidBrush))
            ElseIf CType(pFDT.Rows(0), FeatureDataRow).Geometry.GeometryType = GeometryType.Multipoint Then
                GlobalsFunctions.flashGeo(CType(pFDT.Rows(0), FeatureDataRow).Geometry, m_Map, m_penLine, CType(m_brush, SolidBrush))
            Else
                GlobalsFunctions.flashGeo(CType(pFDT.Rows(0), FeatureDataRow).Geometry, m_Map, m_pen, m_brush)

            End If
        End If

    End Sub
    Private Sub btnAddressRouteTo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddressRouteTo.Click
        Try
            'Flash the selected range or intersection

            'If not initilized, exit
            If m_bAddress_GeocordInit = False Then Return
            'Make sure a item in the list was selected
            If cboStreetRange.SelectedIndex < 0 Or cboStreetLayer1.SelectedIndex < 0 Or cboStreetLayer2.SelectedIndex < 0 Then Return
            'determine intersection or range
            If rdoAddressStreet2.Checked = True Then
                'Find the intersection streets
                Dim pPnt As Esri.ArcGIS.Mobile.Geometries.Point = Nothing

                Dim pGeoStreets As Geometry = getStreetsIntersectionGeom(cboStreetLayer1.Text, cboStreetLayer2.Text, pPnt)
                'Verify the intersection was found
                If pPnt IsNot Nothing Then
                    If pPnt.IsEmpty = False Then

                        'flash it


                        GlobalsFunctions.flashGeo(pPnt, m_Map, m_penLine, CType(m_brush, SolidBrush))


                        RaiseEvent RouteTo(pPnt, "Intersection")

                    End If
                End If
                'cleanup
                pGeoStreets = Nothing
                pPnt = Nothing

            Else
                'Get the range dataset
                Dim pDT As FeatureDataTable = CType(cboStreetRange.DataSource, FeatureDataTable)
                If pDT IsNot Nothing Then
                    'Get the selected value
                    ' Dim pDR As FeatureDataRow = CType(pDT.Rows(cboStreetRange.SelectedIndex), FeatureDataRow)
                    Dim pDRV As DataRowView = CType(cboStreetRange.SelectedItem, DataRowView)

                    Dim pDR As FeatureDataRow = CType(pDRV.Row, FeatureDataRow)

                    'Verify it is a valid row
                    If pDR IsNot Nothing Then
                        If pDR.Geometry IsNot Nothing Then
                            If pDR.Geometry.IsEmpty = False Then


                                RaiseEvent RouteTo(pDR.Geometry, "Street")

                            End If
                        End If

                    End If
                    pDR = Nothing
                End If


                pDT = Nothing
            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub btnDGFlash_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDGFlash.Click
        Try
            'flash a value in the DG
            ZoomFlashRouteGC(recordClickType.flash)
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub btnDGZoomTo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDGZoomTo.Click
        Try
            'Zoom and flash a value in the DG
            ZoomFlashRouteGC(recordClickType.zoom)
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub


    Private Sub btnWaypointSearch_Click(sender As System.Object, e As System.EventArgs) Handles btnWaypointSearch.Click
        'If e.RowIndex = -1 Then Return
        ZoomFlashRouteGC(recordClickType.Waypoint)
    End Sub

    Private Sub btnSearchFind_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearchFind.Click
        If m_bSearchInit = False Then Return
        'List ot hold fields to search on
        Dim pFieldList As IList

        Try
            lblNumResults.Visible = False
            ' lblNumResults.Update()
            picSearching.Visible = True
            'picSearching.Update()
            'Determine field selected
            If cboSearchField.SelectedIndex = 0 Then
                'Get all the fields
                '  pFieldList = cboSearchField.DataSource
                pFieldList = New List(Of String)

                For Each DC As DataColumn In CType(cboSearchField.DataSource, IEnumerable)
                    If DC.ColumnName.Contains("<") = False Then
                        pFieldList.Add(DC.ColumnName)
                    End If

                Next
                'Remove the all fields option
                ' pFieldList.Remove("<All Fields>")

            Else
                'Create a new list
                pFieldList = New List(Of String)
                'Add the selected field
                pFieldList.Add(cboSearchField.SelectedValue)
            End If

            Dim pSearchFeature As SearchFeature = New SearchFeature
            pSearchFeature.Map = m_Map
            pSearchFeature.Fields = pFieldList
            pSearchFeature.layerName = cboSearchLayer.Text
            If txtSearchValue.Visible = True Then
                pSearchFeature.SearchValue = txtSearchValue.Text
            Else
                pSearchFeature.SearchValue = cboSearchValue.SelectedValue
            End If
            cboSearchLayer.SelectedItem = cboSearchLayer.SelectedItem

            Try

                If GlobalsFunctions.IsNumeric(CType(cboSearchLayer.SelectedItem, Esri.ArcGISTemplates.MobileConfigClass.MobileConfigMobileMapConfigSearchPanelSearchLayersSearchLayer).xmax) And _
                           GlobalsFunctions.IsNumeric(CType(cboSearchLayer.SelectedItem, Esri.ArcGISTemplates.MobileConfigClass.MobileConfigMobileMapConfigSearchPanelSearchLayersSearchLayer).xmin) And _
                           GlobalsFunctions.IsNumeric(CType(cboSearchLayer.SelectedItem, Esri.ArcGISTemplates.MobileConfigClass.MobileConfigMobileMapConfigSearchPanelSearchLayersSearchLayer).ymax) And _
                           GlobalsFunctions.IsNumeric(CType(cboSearchLayer.SelectedItem, Esri.ArcGISTemplates.MobileConfigClass.MobileConfigMobileMapConfigSearchPanelSearchLayersSearchLayer).ymin) Then
                    pSearchFeature.SearchEnv = New Envelope(New Coordinate(CType(cboSearchLayer.SelectedItem, Esri.ArcGISTemplates.MobileConfigClass.MobileConfigMobileMapConfigSearchPanelSearchLayersSearchLayer).xmin, CType(cboSearchLayer.SelectedItem, Esri.ArcGISTemplates.MobileConfigClass.MobileConfigMobileMapConfigSearchPanelSearchLayersSearchLayer).ymin), New Coordinate(CType(cboSearchLayer.SelectedItem, Esri.ArcGISTemplates.MobileConfigClass.MobileConfigMobileMapConfigSearchPanelSearchLayersSearchLayer).xmax, CType(cboSearchLayer.SelectedItem, Esri.ArcGISTemplates.MobileConfigClass.MobileConfigMobileMapConfigSearchPanelSearchLayersSearchLayer).ymax))
                End If

                If CType(cboSearchLayer.SelectedItem, Esri.ArcGISTemplates.MobileConfigClass.MobileConfigMobileMapConfigSearchPanelSearchLayersSearchLayer).DefQuery <> "" Then
                    pSearchFeature.SearchDefQ = CType(cboSearchLayer.SelectedItem, Esri.ArcGISTemplates.MobileConfigClass.MobileConfigMobileMapConfigSearchPanelSearchLayersSearchLayer).DefQuery
                End If
            Catch ex As Exception

            End Try

            'pSearchFeature.SearchValue = txtSearchValue.Text
            pSearchFeature.FindAllSimilar = chkSearchSimliar.Checked
            AddHandler pSearchFeature.SearchComplete, AddressOf SearchThreadComplete
            Dim t As Thread
            'Create a new thread to open the GPS
            t = New Thread(AddressOf pSearchFeature.FindFeatures)
            'start the thread
            t.Start()
            ' Search the feature class for all matching features

            pSearchFeature = Nothing


        Catch ex As Exception
            'on a find error, zero out results
            lblNumResults.Text = String.Format(GlobalsFunctions.appConfig.SearchPanel.UIComponents.SearchResultsText, "0")

            picSearching.Visible = False
            lblNumResults.Visible = True

        End Try

        'Cleanup
        pFieldList = Nothing

    End Sub
    Private Delegate Sub SearchThreadCompleteDel(ByVal results As DataTable)
    Private Function updateDT(ByVal results As DataTable) As DataTable
        Dim thistable As DataTable = New DataTable()



        'check the field to see if it is domain field
        'Dim pLay As MobileCacheMapLayer = CType(m_Map.MapLayers(cboSearchLayer.Text), MobileCacheMapLayer)
        Dim pfl As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = GlobalsFunctions.GetFeatureSource(cboSearchLayer.Text, m_Map).FeatureSource

        Dim col As DataColumn
        Dim pHashcol As Hashtable = New Hashtable()
        Dim pCV As CodedValueDomain
        Dim intSubCode As Integer = 0

        If pfl.HasSubtypes Then

            intSubCode = pfl.Columns(pfl.SubtypeColumnName).DefaultValue 'pfl.Subtypes.Rows.Item(0)(0)



        End If

        For Each col In results.Columns
            Dim newcol As DataColumn = New DataColumn()
            'If pfl.HasSubtypes Then



            '    newcol.ColumnName = col.ColumnName
            '    newcol.DataType = System.Type.GetType("System.String")
            '    thistable.Columns.Add(newcol)
            'Else

            'MsgBox("Fix Domain")
            Dim obj As Object
            'Dim obj As Object = pfl.GetDomain(0, col.ColumnName)
            If pfl.Columns(col.ColumnName) IsNot Nothing Then


                obj = pfl.Columns(col.ColumnName).GetDomain(intSubCode)
                If pfl.HasSubtypes And col.ColumnName = pfl.SubtypeColumnName Then
                    newcol.ColumnName = col.ColumnName
                    newcol.DataType = System.Type.GetType("System.String")
                    thistable.Columns.Add(newcol)
                ElseIf obj Is Nothing Then
                    'Dim newcol As DataColumn = New DataColumn()
                    newcol.ColumnName = col.ColumnName
                    newcol.DataType = System.Type.GetType("System.String") 'col.DataType
                    thistable.Columns.Add(newcol)
                ElseIf TypeOf obj Is CodedValueDomain Then

                    pCV = CType(obj, CodedValueDomain)
                    Dim pHashT As Hashtable = New Hashtable()
                    Dim cvrow As DataRow
                    For Each cvrow In pCV.Rows
                        pHashT.Add(cvrow("Code"), cvrow("Value"))
                    Next
                    pHashcol.Add(col, pHashT)
                    'Dim newcol As DataColumn = New DataColumn()
                    newcol.ColumnName = col.ColumnName
                    newcol.DataType = System.Type.GetType("System.String")
                    thistable.Columns.Add(newcol)
                Else
                    newcol.ColumnName = col.ColumnName
                    newcol.DataType = col.DataType
                    thistable.Columns.Add(newcol)
                End If
            Else
                newcol.ColumnName = col.ColumnName
                newcol.DataType = col.DataType
                thistable.Columns.Add(newcol)

            End If
            'End If

        Next


       
        Dim row As DataRow
        Dim newrow As DataRow

        For Each row In results.Rows
            newrow = thistable.NewRow()
            If pfl.HasSubtypes Then

                intSubCode = row(pfl.Columns(pfl.SubtypeColumnName).ColumnName)
                For Each col In results.Columns

                    Dim obj As Object
                    If col.ColumnName = pfl.SubtypeColumnName Then

                        obj = pfl.Subtypes


                    Else
                        Try

                      
                        obj = pfl.Columns(col.ColumnName).GetDomain(intSubCode)
                        Catch ex As Exception
                            obj = Nothing
                        End Try
                    End If



                    If obj Is Nothing Then
                        newrow(col.ColumnName) = row(col)
                    ElseIf TypeOf obj Is CodedValueDomain Then

                        pCV = CType(obj, CodedValueDomain)

                        If row(col) Is DBNull.Value Then
                            newrow(col.ColumnName) = ""
                        ElseIf row(col) Is Nothing Then
                            newrow(col.ColumnName) = ""
                        Else
                            newrow(col.ColumnName) = GlobalsFunctions.getDomainCode(row(col), pCV)
                        End If


                    Else
                        newrow(col.ColumnName) = row(col)
                    End If




                Next
                thistable.Rows.Add(newrow)
            Else

                For Each col In results.Columns



                    If pHashcol.Contains(col) = False Then
                        newrow(col.ColumnName) = row(col)
                    Else
                        Dim pColValue As Hashtable = pHashcol(col)
                        newrow(col.ColumnName) = pColValue(row(col))
                    End If

                Next
                thistable.Rows.Add(newrow)
            End If

        Next

        Return thistable
    End Function
    Private Sub SearchThreadComplete(ByVal results As DataTable)
        Try


            If InvokeRequired Then

                Invoke(New SearchThreadCompleteDel(AddressOf SearchThreadComplete), results)

            Else
                RemoveHandler dgResults.SelectionChanged, AddressOf dgResults_SelectionChanged
                Try
                    'If a results was found
                    Dim visColCnt As Integer = dgResults.Columns.Count

                    If results IsNot Nothing Then
                        If results.Columns.Contains(cboSearchField.Text) Then
                            results.Columns(cboSearchField.Text).SetOrdinal(0)

                        End If
                        addIndex(results)

                        Dim newTable As DataTable = updateDT(results)

                        'Set the results grid to the results
                        dgResults.DataSource = newTable
                        dgResults.Tag = results
                        'hide the blob column from the results
                        'For Each pDC As DataGridViewColumn In dgResults.Columns
                        '    '    MsgBox(pDC.GetType.Name)
                        '    If pDC.GetType.Name.Contains("ImageColumn") Then

                        '        pDC.Visible = False

                        '    End If

                        'Next

                        Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = GlobalsFunctions.GetFeatureSource(cboSearchLayer.Text, m_Map).FeatureSource
                        'For i As Integer = 0 To dgResults.Columns.Count - 1
                        '    dgResults.Columns(i).HeaderText = CType(dgResults.Columns(i), DataColumn).caption

                        'Next

                        For i As Integer = 0 To dgResults.Columns.Count - 1
                            If dgResults.Columns(i).GetType.Name.Contains("ImageColumn") Then

                                dgResults.Columns(i).Visible = False
                                visColCnt = visColCnt - 1
                            ElseIf dgResults.Columns(i).Name = pFL.FidColumnName Then
                                dgResults.Columns(i).Visible = False
                                visColCnt = visColCnt - 1
                            ElseIf dgResults.Columns(i).Name = pFL.GeometryColumnName Then
                                dgResults.Columns(i).Visible = False
                                visColCnt = visColCnt - 1
                            ElseIf dgResults.Columns(i).Name = dgResults.Columns(i).HeaderText Then
                                If pFL.Columns(dgResults.Columns(i).Name) IsNot Nothing Then

                                    If pFL.Columns(dgResults.Columns(i).Name).Caption <> "" Then
                                        If pFL.Columns(dgResults.Columns(i).Name).Caption <> dgResults.Columns(i).Name Then
                                            dgResults.Columns(i).HeaderText = pFL.Columns(dgResults.Columns(i).Name).Caption
                                        End If

                                    End If
                                ElseIf dgResults.Columns(i).Name.Contains("INDEX_CODE_TEMP") Then
                                    dgResults.Columns(i).Visible = False
                                    visColCnt = visColCnt - 1
                                End If

                            ElseIf dgResults.Columns(i).Name.Contains("INDEX_CODE_TEMP") Then
                                dgResults.Columns(i).Visible = False
                                visColCnt = visColCnt - 1
                            End If

                        Next


                        pFL = Nothing
                        'Empty the tag, used to determine Geocode results from Search Results
                        ' dgResults.Tag = ""
                        'Update the records found label
                        lblNumResults.Text = String.Format(GlobalsFunctions.appConfig.SearchPanel.UIComponents.SearchResultsText, results.Rows.Count)
                        m_BuffMA.clearGeo()
                        'Show the results panel
                        splContMainSearch.Panel2Collapsed = False
                    Else
                        'Nothing found
                        'Removes old results
                        dgResults.DataSource = Nothing
                        'Reset the tag, used to determine Geocode results from Search Results
                        dgResults.Tag = ""
                        'Update the records found label

                        lblNumResults.Update()
                        lblNumResults.Text = String.Format(GlobalsFunctions.appConfig.SearchPanel.UIComponents.SearchResultsText, 0)
                        'Hide the results
                        splContMainSearch.Panel2Collapsed = True
                    End If
                    picSearching.Visible = False
                    lblNumResults.Visible = True
                    'Cleanup

                    'dgResults.ScrollBars = ScrollBars.None

                    dgResults.Update()
                    dgResults.Visible = True
                    VScrollBar1.Visible = False
                    HScrollBar1.Visible = False

                    'Set the scroll bars max/min
                    'If dgResults.RowCount = 0 Then

                    '    VScrollBar1.Visible = True
                    '    VScrollBar1.Minimum = 0
                    '    VScrollBar1.Maximum = 0
                    '    HScrollBar1.Visible = True
                    '    HScrollBar1.Minimum = 0
                    '    HScrollBar1.Maximum = 0
                    'Else

                    '    VScrollBar1.Visible = True
                    '    VScrollBar1.Minimum = 0
                    '    VScrollBar1.Maximum = dgResults.RowCount - 1
                    '    HScrollBar1.Visible = True
                    '    HScrollBar1.Minimum = 0
                    '    HScrollBar1.Maximum = visColCnt + 1
                    'End If

                    resizeDGandScrolls()

                    'Try

                    '    HScrollBar1.Value = 0
                    '    VScrollBar1.Value = 0
                    'Catch ex As Exception

                    'End Try

                    m_Map.Invalidate()


                    Call MobileSearch_Resize(Nothing, Nothing)

                Catch ex As Exception
                    Dim st As New StackTrace
                    MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                    st = Nothing


                    'on a find error, zero out results
                    lblNumResults.Text = String.Format(GlobalsFunctions.appConfig.SearchPanel.UIComponents.SearchResultsText, 0)
                    picSearching.Visible = False
                    lblNumResults.Visible = True
                Finally
                    AddHandler dgResults.SelectionChanged, AddressOf dgResults_SelectionChanged
                End Try
            End If


        Catch exOut As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & exOut.Message)
            st = Nothing

        End Try

    End Sub
    Private Sub btnLookup_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLookup.Click
        'update display
        pnlSearch.Visible = True
        pnlAddress.Visible = False
        pnlGeocode.Visible = False
        btnLookup.Checked = True
        btnGeocode.Checked = False
        btnAddress.Checked = False
        splContMainSearch.Panel2Collapsed = True
        btnAddressPnt.Checked = False
        pnlAddressPoint.Visible = False
    End Sub
    Private Sub btnAddress_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddress.Click
        'update display
        pnlSearch.Visible = False
        pnlAddress.Visible = True
        MobileSearch_Resize(Nothing, Nothing)

        pnlGeocode.Visible = False
        splContMainSearch.Panel2Collapsed = True
        btnLookup.Checked = False
        btnGeocode.Checked = False
        btnAddress.Checked = True
        btnAddressPnt.Checked = False
        pnlAddressPoint.Visible = False
    End Sub
    Private Sub btnAddressPnt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddressPnt.Click
        'update display
        pnlSearch.Visible = False
        pnlAddress.Visible = False

        pnlGeocode.Visible = False
        splContMainSearch.Panel2Collapsed = True
        btnLookup.Checked = False
        btnGeocode.Checked = False
        btnAddress.Checked = False
        btnAddressPnt.Checked = True
        If (gpBxAddPntControls.Controls.Count > 0) Then

            pnlAddressPoint.Visible = True


        End If

    End Sub
    Private Sub btnGeoCode_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGeocode.Click
        'update display
        pnlSearch.Visible = False
        pnlAddress.Visible = False
        pnlGeocode.Visible = True
        splContMainSearch.Panel2Collapsed = True
        btnLookup.Checked = False
        btnGeocode.Checked = True
        btnAddress.Checked = False
        btnAddressPnt.Checked = False
        pnlAddressPoint.Visible = False
    End Sub
    Private Sub txtSearchValue_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtSearchValue.TextChanged

        If txtSearchValue.Text = "" And m_SearchPoly Is Nothing Then
            btnSearchFind.Enabled = False
        Else
            btnSearchFind.Enabled = True
        End If
    End Sub
    Private Sub cboSearchValue_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboSearchValue.SelectedIndexChanged
        If cboSearchValue.SelectedItem Is Nothing And m_SearchPoly Is Nothing Then 'And pBuff Is Nothing
            btnSearchFind.Enabled = False
        Else
            btnSearchFind.Enabled = True
        End If
    End Sub
    Private Sub btnIDEvent_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        ' If btnGeocode.CheckState = CheckState.Checked Then
        ZoomFlashRouteGC(recordClickType.IDLocation)
        'Else
        'ZoomFlashRouteGC(recordClickType.ID)
        'End If

    End Sub
    Private Sub btnAddressReload_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddressReload.Click
        LoadData()
    End Sub
    Private Sub IdentifyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        ZoomFlashRouteGC(recordClickType.IDLocation)
    End Sub
#End Region
#Region "private Functions"
    'Private Function GetFeatureSource(ByVal LayerName As String, ByVal Map as Esri.ArcGIS.Mobile.Winforms.Map) as Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource
    '    Dim pMblLayInspect As Esri.ArcGIS.Mobile.FeatureCaching.MobileCacheMapLayer = CType(Map.MapLayers(LayerName), MobileCacheMapLayer)
    '    If pMblLayInspect Is Nothing Then

    '        For Each pL As MobileCacheMapLayer In Map.MapLayers
    '            'If the layer is a feature layer
    '            If TypeOf pL.Layer is Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource Then

    '                If CType(pL.Layer, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource).Name = LayerName Then
    '                    Return CType(pL.Layer, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)

    '                End If
    '                If CType(pL.Layer, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource).ServerFeatureClassName = LayerName Then
    '                    Return CType(pL.Layer, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)

    '                End If
    '                If CType(pL.Layer, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource).ServerFeatureClassName.Substring(CType(pL.Layer, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource).ServerFeatureClassName.LastIndexOf(".") + 1) = LayerName Then

    '                    Return CType(pL.Layer, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)

    '                End If

    '                pL = Nothing
    '            End If

    '        Next
    '    Else
    '        Return CType(pMblLayInspect.Layer, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)
    '    End If

    '    Return Nothing

    'End Function

    'Private Function GetFeatureSource(ByVal LayerName As String, ByVal thisMap as Esri.ArcGIS.Mobile.Winforms.Map) as Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource
    '    Try
    '        If thisMap Is Nothing Then Return Nothing
    '        'Get the mobile service that is associated with the map
    '        Dim pMblService As MobileCache = Nothing

    '        For Each pDS As DataSource In thisMap.DataSources
    '            If TypeOf pDS Is MobileCache Then
    '                pMblService = CType(pDS, MobileCache)

    '                Exit For
    '            End If
    '        Next
    '        'If thisMap.DataSources.Count > 1 Then
    '        '    MsgBox("Fix GetFeatureSource for many datasources - Navigation Control")
    '        'End If

    '        If pMblService Is Nothing Then Return Nothing
    '        If Not thisMap.IsValid Or Not pMblService.IsOpen Then Return Nothing


    '        Dim pMblLayInspect As Esri.ArcGIS.Mobile.FeatureCaching.MobileCacheMapLayer = CType(GlobalsFunctions.GetMapLayer(LayerName, m_Map), MobileCacheMapLayer)
    '        If pMblLayInspect Is Nothing Then




    '            ' Total number of MapLayers in the current map
    '            Dim playersCount As Integer = thisMap.MapLayers.Count


    '            'Current map layer
    '            Dim pMapLayer as Esri.ArcGIS.Mobile.MapLayer

    '            'map layer's name
    '            Dim pMapLayerName As String = ""

    '            'Map layer's index
    '            Dim pLayerIndex As Integer = 0

    '            'Loops through current map layers collection
    '            For i As Integer = 0 To playersCount - 1

    '                'Gets the map layer for that index
    '                pMapLayer = thisMap.MapLayers(i)
    '                'Gets the map layer's name
    '                pMapLayerName = pMapLayer.Name


    '                If TypeOf pMapLayer Is MobileCacheMapLayer Then

    '                    'Current map layer
    '                    Dim pMobileCacheLayer As MobileCacheMapLayer

    '                    pMobileCacheLayer = TryCast(pMapLayer, MobileCacheMapLayer)
    '                    If TypeOf pMobileCacheLayer.Layer is Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource Then


    '                        If TypeOf pMobileCacheLayer.Layer is Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource Then
    '                            If CType(pMobileCacheLayer.Layer, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource).Name = LayerName Then
    '                                Return CType(pMobileCacheLayer.Layer, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)

    '                            End If
    '                            If CType(pMobileCacheLayer.Layer, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource).ServerFeatureClassName = LayerName Then
    '                                Return CType(pMobileCacheLayer.Layer, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)

    '                            End If
    '                            If CType(pMobileCacheLayer.Layer, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource).ServerFeatureClassName.Substring(CType(pMobileCacheLayer.Layer, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource).ServerFeatureClassName.LastIndexOf(".") + 1) = LayerName Then

    '                                Return CType(pMobileCacheLayer.Layer, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)

    '                            End If


    '                        End If




    '                    Else
    '                        ' MsgBox("Raster Cache Layer, not supported in TOC")

    '                    End If
    '                    pMobileCacheLayer = Nothing
    '                ElseIf TypeOf pMapLayer Is Esri.ArcGIS.Mobile.DataProducts.RasterData.TileCacheMapLayer Then


    '                ElseIf TypeOf pMapLayer Is Esri.ArcGIS.Mobile.DataProducts.StreetMapData.StreetMapLayer Then

    '                ElseIf TypeOf pMapLayer Is Esri.ArcGIS.Mobile.WebServices.ArcGISServices.MapServices.TileServiceMapLayer Then


    '                Else



    '                End If




    '            Next i
    '            pMapLayer = Nothing
    '            '   pMblService = Nothing
    '        Else

    '            Return CType(pMblLayInspect.Layer, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)
    '        End If
    '        pMblLayInspect = Nothing
    '        Return Nothing


    '    Catch ex As Exception
    '        Dim st As New StackTrace
    '        MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
    '        st = Nothing

    '        Return Nothing

    '    End Try
    'End Function

    Private Function CheckStreetLayer() As Boolean
        'Check to make sure street layer and fields exist
        Try



            Dim pFsWDef As FeatureSourceWithDef = GlobalsFunctions.GetFeatureSource(m_AddressLayer, m_Map)
            Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = pFsWDef.FeatureSource
            'verify layer and fields exist
            If pFL Is Nothing Then Return False
            Dim pLayDef As MobileCacheMapLayerDefinition = GlobalsFunctions.GetLayerDefinition(m_Map, pFsWDef)

            If GlobalsFunctions.appConfig.SearchPanel.AddressSearch.DefQuery <> "" Then
                pLayDef.DisplayExpression = GlobalsFunctions.appConfig.SearchPanel.AddressSearch.DefQuery
            End If

            If pFL.Columns(m_AddressFieldStreetName) Is Nothing Then Return False
            If pFL.Columns(m_AddressFieldLeftFrom) Is Nothing Then Return False
            If pFL.Columns(m_AddressFieldLeftTo) Is Nothing Then Return False
            If pFL.Columns(m_AddressFieldRightFrom) Is Nothing Then Return False
            If pFL.Columns(m_AddressFieldRightTo) Is Nothing Then Return False


            pFL = Nothing



        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function
    Private Enum recordClickType
        zoom
        flash
        ID
        Route
        IDLocation
        Waypoint
    End Enum

    Private Sub ZoomFlashRouteGC(ByVal action As recordClickType)

        'Verify a row is selected
        If dgResults.SelectedRows.Count = 0 Then Return
        Dim pFLDT As FeatureDataTable = Nothing
        Dim pFDR As FeatureDataRow = Nothing
        Dim pGeo As Esri.ArcGIS.Mobile.Geometries.Geometry = Nothing
        Dim pDR As DataGridViewRow = Nothing
        Dim pCoord As Coordinate = Nothing
        Try

            If dgResults.DataSource Is Nothing Then
                pGeo = New Esri.ArcGIS.Mobile.Geometries.Point(New Coordinate(dgResults.SelectedRows(0).Cells(1).Value, dgResults.SelectedRows(0).Cells(2).Value))


            Else


                If Not TypeOf (dgResults.Tag) Is FeatureDataTable Then
                    'GEocdoe
                    'Get the datatable bound to the grid
                    pFLDT = CType(dgResults.DataSource, FeatureDataTable)
                    'Make sure a datasource was found
                    If pFLDT Is Nothing Then Return

                    pDR = dgResults.SelectedRows(0)
                    Dim rowIdx As Integer = pDR.Cells("INDEX_CODE_TEMP").Value


                    pFDR = CType(pFLDT.Rows(rowIdx), FeatureDataRow)
                    'pFDR = CType(pFLDT.Rows(dgResults.SelectedRows(0).Index), FeatureDataRow)
                    'make sure a row was found
                    If pFDR Is Nothing Then Return
                    'Check the tag to see if the results are from a Geocode or a search
                    'Get the address from the Geocode
                    Dim strCurAdd As String = CStr(dgResults.Tag)
                    'Determine side of the street
                    Dim bAddEven As Boolean = True
                    If IsNumeric(strCurAdd) Then


                        If CDbl(strCurAdd) Mod 2 <> 0 Then
                            bAddEven = False
                        End If
                        'Set up the side of the street to check for the address 
                        Dim pFromField, pToField As String
                        If IsNumeric(pFDR(m_AddressFieldLeftFrom)) And
                            IsNumeric(pFDR(m_AddressFieldRightTo)) And
                            IsNumeric(pFDR(m_AddressFieldRightFrom)) And
                            IsNumeric(pFDR(m_AddressFieldLeftTo)) Then

                            If CDbl(pFDR(m_AddressFieldLeftFrom)) Mod 2 = 0 And bAddEven Then

                                pFromField = m_AddressFieldLeftFrom
                                pToField = m_AddressFieldRightTo
                            ElseIf CDbl(pFDR(m_AddressFieldLeftFrom)) Mod 2 <> 0 And bAddEven Then

                                pFromField = m_AddressFieldLeftFrom
                                pToField = m_AddressFieldRightTo
                            ElseIf CDbl(pFDR(m_AddressFieldLeftFrom)) Mod 2 = 0 And bAddEven = False Then

                                pFromField = m_AddressFieldRightFrom
                                pToField = m_AddressFieldLeftTo
                            Else
                                pFromField = m_AddressFieldRightFrom
                                pToField = m_AddressFieldLeftTo

                            End If
                            'Get the Geocode Location

                            pCoord = DetermineAddressLocation(pFDR.Geometry, CInt(strCurAdd), CInt(pFDR(pFromField)), CInt(pFDR(pToField)))
                            If pCoord Is Nothing Then Return
                            pGeo = New Esri.ArcGIS.Mobile.Geometries.Point(pCoord)
                        Else
                            pGeo = pFDR.Geometry

                        End If


                    Else
                        pGeo = pFDR.Geometry
                    End If


                Else
                    'Get the datatable bound to the grid
                    pFLDT = CType(dgResults.Tag, FeatureDataTable)
                    'Make sure a datasource was found
                    If pFLDT Is Nothing Then Return

                    pDR = dgResults.SelectedRows(0)
                    Dim rowIdx As Integer = pDR.Cells("INDEX_CODE_TEMP").Value


                    pFDR = CType(pFLDT.Rows(rowIdx), FeatureDataRow)
                    'pFDR = CType(pFLDT.Rows(dgResults.SelectedRows(0).Index), FeatureDataRow)
                    'make sure a row was found
                    If pFDR Is Nothing Then Return
                    'Check the tag to see if the results are from a Geocode or a search
                    pGeo = pFDR.Geometry

                End If

            End If


            'Get the geo from the row and zoom/flash it
            If pGeo IsNot Nothing Then
                If pGeo.IsEmpty = False Then
                    If action = recordClickType.zoom Then
                        '  If m_Map.Extent.Intersects(pFDR.Geometry) = False Then
                        GlobalsFunctions.zoomTo(pGeo, m_Map)
                        If pGeo.GeometryType = GeometryType.Point Then
                            GlobalsFunctions.flashGeo(pGeo, m_Map, m_penLine, CType(m_brush, SolidBrush))
                        Else
                            GlobalsFunctions.flashGeo(pGeo, m_Map, m_pen, m_brush)

                        End If
                    ElseIf action = recordClickType.Waypoint Then
                        RaiseEvent Waypoint(pGeo, "")

                    ElseIf action = recordClickType.flash Then

                        If pGeo.GeometryType = GeometryType.Point Then
                            GlobalsFunctions.flashGeo(pGeo, m_Map, m_penLine, CType(m_brush, SolidBrush))
                        Else
                            GlobalsFunctions.flashGeo(pGeo, m_Map, m_pen, m_brush)

                        End If

                    ElseIf action = recordClickType.ID Then
                        '  RaiseEvent IDLocation(pFDR.Geometry, pFDR.FeatureSource.Name, pFDR.Fid)
                        If pFDR IsNot Nothing Then
                            RaiseEvent IDResult(pFDR)
                        End If
                    ElseIf action = recordClickType.IDLocation Then



                        'RaiseEvent IDLocation(pGeo, pFDR.FeatureSource.Name, pFDR.Fid)
                        RaiseEvent IDLocation(pGeo, "", -1)


                    ElseIf action = recordClickType.Route Then
                        RaiseEvent RouteTo(pGeo, "ID Result")
                    End If
                End If
            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        Finally
            'Cleanup
            pCoord = Nothing
            pFLDT = Nothing
            pFDR = Nothing
            pGeo = Nothing
            pDR = Nothing
        End Try



    End Sub
    Private Function LoadLayers(ByVal Layers As List(Of MobileConfigClass.MobileConfigMobileMapConfigSearchPanelSearchLayersSearchLayer)) As Boolean
        'Create a new list to hold layer names
        'Dim pList As IList = New List(Of String)
        Dim lyTmp As MobileConfigClass.MobileConfigMobileMapConfigSearchPanelSearchLayersSearchLayer
        'Get all layers
        If Layers Is Nothing Then
            Layers = New List(Of MobileConfigClass.MobileConfigMobileMapConfigSearchPanelSearchLayersSearchLayer)

            For Each pL As Esri.ArcGIS.Mobile.MapLayer In m_Map.MapLayers
                'If the layer is a feature layer
                If TypeOf pL Is MobileCacheMapLayer Then
                    If TypeOf CType(pL, MobileCacheMapLayer).MobileCache.FeatureSources(pL.Name) Is Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource Then
                        'Add to list
                        lyTmp = New MobileConfigClass.MobileConfigMobileMapConfigSearchPanelSearchLayersSearchLayer()
                        lyTmp.Name = pL.Name
                        Layers.Add(lyTmp)

                        'pList.Add(pL.Name)


                    End If
                End If
            Next
        ElseIf Layers.Count = 0 Then
            For Each pL As Esri.ArcGIS.Mobile.MapLayer In m_Map.MapLayers
                'If the layer is a feature layer
                If TypeOf pL Is MobileCacheMapLayer Then
                    If TypeOf CType(pL, MobileCacheMapLayer).MobileCache.FeatureSources(pL.Name) Is Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource Then
                        'Add to list
                        lyTmp = New MobileConfigClass.MobileConfigMobileMapConfigSearchPanelSearchLayersSearchLayer()
                        lyTmp.Name = pL.Name
                        Layers.Add(lyTmp)
                        'pList.Add(pL.Name)


                    End If
                End If
            Next
        Else

            'For Each lay In Layers
            '    If GlobalsFunctions.GetFeatureSource(lay.Name, m_Map) IsNot Nothing Then
            '        'pList.Add(lay.Name)
            '    Else

            '    End If
            'Next
        End If

        'Set layer list to the search drop down
        'cboSearchLayer.DataSource = pList
        cboSearchLayer.DisplayMember = "Name"
        cboSearchLayer.DataSource = Layers

    End Function
    Private Function LoadPrefilters(ByVal intCboChanged As Integer, ByVal whichCont As String) As Boolean
        Dim pDT As DataTable
        'Remove any duplicate street names *GetDataTable does not handle distinct
        ' Dim pSDistinct As MobileControls.DataTableTools
        Try
            If (gpBoxPreFiltInt.Controls.Count = 0) Then
                Return False
            End If
            'Get the Street Layer
            'Dim pML As MobileCacheMapLayer
            ' pML = CType(m_Map.MapLayers(m_AddressLayer), MobileCacheMapLayer)
            'If not found return
            'If pML Is Nothing Then Return False
            Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = GlobalsFunctions.GetFeatureSource(m_AddressLayer, m_Map).FeatureSource
            If pFL Is Nothing Then Return False
            'Get all streets names
            Dim pFDT As FeatureDataTable
            ' Dim cboCnt As Integer = 0

            Dim sql As String = ""
            Dim QF As QueryFilter
            Dim cboCnt As Integer = 0

            For i As Integer = 0 To lstPreFilterInt.Count - 1

                If cboCnt > intCboChanged Then
                    Dim strFldName() As String = lstPreFilterInt(i).Name.Split("|")


                    QF = New QueryFilter
                    If sql = "" Then
                        QF.WhereClause = "LTRIM(RTRIM(" & strFldName(0) & "))" & " <> ''"
                    Else
                        QF.WhereClause = sql & " AND " + "LTRIM(RTRIM(" & strFldName(0) & "))" & " <> ''"
                    End If


                    pFDT = pFL.GetDataTable(QF, New String(0) {strFldName(0)})
                    If pFDT.Rows.Count = 0 Then
                        If whichCont = "St" Then

                            lstPreFilterSt(i).DataSource = Nothing
                            lstPreFilterSt(i).Items.Clear()
                            lstPreFilterSt(i).DisplayMember = Nothing
                            lstPreFilterSt(i).ValueMember = Nothing
                            lstPreFilterSt(i).DataBindings.Clear()
                        ElseIf whichCont = "Int" Then
                            lstPreFilterInt(i).DataSource = Nothing
                            lstPreFilterInt(i).Items.Clear()
                            lstPreFilterInt(i).DisplayMember = Nothing
                            lstPreFilterInt(i).ValueMember = Nothing
                            lstPreFilterInt(i).DataBindings.Clear()

                        Else
                            lstPreFilterInt(i).DataSource = Nothing
                            lstPreFilterInt(i).Items.Clear()
                            lstPreFilterInt(i).DisplayMember = Nothing
                            lstPreFilterInt(i).ValueMember = Nothing
                            lstPreFilterInt(i).DataBindings.Clear()


                            lstPreFilterSt(i).DataSource = Nothing
                            lstPreFilterSt(i).Items.Clear()
                            lstPreFilterSt(i).DisplayMember = Nothing
                            lstPreFilterSt(i).ValueMember = Nothing
                            lstPreFilterSt(i).DataBindings.Clear()
                        End If
                        '  MsgBox("The " & m_AddressLayer & " layer does not have any records in it, please refresh this layer")
                        Return False

                    End If
                    If whichCont = "St" Then
                        RemoveHandler lstPreFilterSt(i).SelectedIndexChanged, AddressOf cboPreFilt_SelectChanged

                        lstPreFilterSt(i).DataSource = Nothing
                        lstPreFilterSt(i).Items.Clear()
                        lstPreFilterSt(i).DisplayMember = Nothing
                        lstPreFilterSt(i).ValueMember = Nothing
                        lstPreFilterSt(i).DataBindings.Clear()


                    ElseIf whichCont = "Int" Then
                        RemoveHandler lstPreFilterInt(i).SelectedIndexChanged, AddressOf cboPreFilt_SelectChanged

                        lstPreFilterInt(i).DataSource = Nothing
                        lstPreFilterInt(i).Items.Clear()
                        lstPreFilterInt(i).DisplayMember = Nothing
                        lstPreFilterInt(i).ValueMember = Nothing
                        lstPreFilterInt(i).DataBindings.Clear()

                    Else


                        RemoveHandler lstPreFilterSt(i).SelectedIndexChanged, AddressOf cboPreFilt_SelectChanged

                        lstPreFilterSt(i).DataSource = Nothing
                        lstPreFilterSt(i).Items.Clear()
                        lstPreFilterSt(i).DisplayMember = Nothing
                        lstPreFilterSt(i).ValueMember = Nothing
                        lstPreFilterSt(i).DataBindings.Clear()

                        RemoveHandler lstPreFilterInt(i).SelectedIndexChanged, AddressOf cboPreFilt_SelectChanged

                        lstPreFilterInt(i).DataSource = Nothing
                        lstPreFilterInt(i).Items.Clear()
                        lstPreFilterInt(i).DisplayMember = Nothing
                        lstPreFilterInt(i).ValueMember = Nothing
                        lstPreFilterInt(i).DataBindings.Clear()

                    End If
                    pDT = pFDT
                    'pDT = pDT.DefaultView.ToTable(True, New String(0) {lstPreFilterInt(i).Name})

                    'pDT.Rows.Add("<All>")

                    If whichCont = "St" Then
                        If m_Distinct = "ADO" Then
                            pDT = pDT.DefaultView.ToTable(True, pFDT.Columns(0).ColumnName) 'New String(0) {m_AddressFieldStreetName})
                            If lstPreFilterSt(i).Tag IsNot Nothing Then
                                If CType(lstPreFilterSt(i).Tag, MobileConfigClass.MobileConfigMobileMapConfigSearchPanelAddressSearchPrefilterFieldsPrefilterField).ShowAll.ToUpper = "FALSE" Then
                                Else

                                    pDT.Rows.Add(GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)

                                End If
                            Else

                                pDT.Rows.Add(GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)

                            End If


                            lstPreFilterSt(i).DataSource = pDT
                            lstPreFilterSt(i).DisplayMember = pDT.Columns(0).ColumnName
                            lstPreFilterSt(i).ValueMember = pDT.Columns(0).ColumnName

                        ElseIf m_Distinct = "DataTools" Then
                            'Remove any duplicate street names *GetDataTable does not handle distinct
                            ' Dim pSDistinct As MobileControls.DataTableTools = New MobileControls.DataTableTools
                            ' pDT = pSDistinct.SelectDistinct(pDT, New String(0) {m_AddressFieldStreetName})
                            'Load the street names to the combobox
                        Else
                            lstPreFilterSt(i).Items.Clear()
                            For Each pDR As DataRow In pDT.Rows
                                If pDR(0) IsNot Nothing And Trim(pDR(0).ToString) <> "" Then
                                    If (lstPreFilterSt(i).Items.Contains(pDR(0)) = False) Then
                                        lstPreFilterSt(i).Items.Add(pDR(0))
                                    End If

                                End If

                            Next
                            If lstPreFilterSt(i).Tag IsNot Nothing Then
                                If CType(lstPreFilterSt(i).Tag, MobileConfigClass.MobileConfigMobileMapConfigSearchPanelAddressSearchPrefilterFieldsPrefilterField).ShowAll.ToUpper = "FALSE" Then
                                Else
                                    lstPreFilterSt(i).Items.Insert(0, GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)
                                    'pDT.Rows.Add(GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)

                                End If
                            Else
                                lstPreFilterSt(i).Items.Insert(0, GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)
                                'pDT.Rows.Add(GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)

                            End If



                        End If



                    ElseIf whichCont = "Int" Then
                        If m_Distinct = "ADO" Then
                            pDT = pDT.DefaultView.ToTable(True, pFDT.Columns(0).ColumnName) 'New String(0) {m_AddressFieldStreetName})
                            If lstPreFilterInt(i).Tag IsNot Nothing Then
                                If CType(lstPreFilterInt(i).Tag, MobileConfigClass.MobileConfigMobileMapConfigSearchPanelAddressSearchPrefilterFieldsPrefilterField).ShowAll.ToUpper = "FALSE" Then
                                Else
                                    'lstPreFilterInt(i).Items.Insert(0, GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)
                                    pDT.Rows.Add(GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)

                                End If
                            Else
                                'lstPreFilterInt(i).Items.Insert(0, GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)
                                pDT.Rows.Add(GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)

                            End If
                            'pDT.Rows.Add(GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)

                            lstPreFilterInt(i).DataSource = pDT
                            lstPreFilterInt(i).DisplayMember = pDT.Columns(0).ColumnName
                            lstPreFilterInt(i).ValueMember = pDT.Columns(0).ColumnName

                        ElseIf m_Distinct = "DataTools" Then
                            'Remove any duplicate street names *GetDataTable does not handle distinct
                            ' Dim pSDistinct As MobileControls.DataTableTools = New MobileControls.DataTableTools
                            ' pDT = pSDistinct.SelectDistinct(pDT, New String(0) {m_AddressFieldStreetName})
                            'Load the street names to the combobox
                        Else
                            lstPreFilterInt(i).Items.Clear()

                            For Each pDR As DataRow In pDT.Rows
                                If pDR(0) IsNot Nothing And Trim(pDR(0).ToString) <> "" Then
                                    If (lstPreFilterInt(i).Items.Contains(pDR(0)) = False) Then
                                        lstPreFilterInt(i).Items.Add(pDR(0))
                                    End If

                                End If

                            Next
                            'lstPreFilterInt(i).Items.Insert(0, GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)
                            If lstPreFilterInt(i).Tag IsNot Nothing Then
                                If CType(lstPreFilterInt(i).Tag, MobileConfigClass.MobileConfigMobileMapConfigSearchPanelAddressSearchPrefilterFieldsPrefilterField).ShowAll.ToUpper = "FALSE" Then
                                Else
                                    lstPreFilterInt(i).Items.Insert(0, GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)
                                    'pDT.Rows.Add(GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)

                                End If
                            Else
                                lstPreFilterInt(i).Items.Insert(0, GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)
                                'pDT.Rows.Add(GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)

                            End If
                        End If

                    Else

                        'If m_Distinct = "ADO" Then
                        '    pDT.Rows.Add("<All>")
                        '    pDT = pDT.DefaultView.ToTable(True, pFDT.Columns(0).ColumnName) 'New String(0) {m_AddressFieldStreetName})
                        '    lstPreFilterInt(i).DataSource = pDT
                        '    lstPreFilterInt(i).DisplayMember = pDT.Columns(0).ColumnName
                        '    lstPreFilterInt(i).ValueMember = pDT.Columns(0).ColumnName

                        'ElseIf m_Distinct = "DataTools" Then
                        '    'Remove any duplicate street names *GetDataTable does not handle distinct
                        '    ' Dim pSDistinct As MobileControls.DataTableTools = New MobileControls.DataTableTools
                        '    ' pDT = pSDistinct.SelectDistinct(pDT, New String(0) {m_AddressFieldStreetName})
                        '    'Load the street names to the combobox
                        'Else
                        '    lstPreFilterInt(i).Items.Clear()

                        '    For Each pDR As DataRow In pDT.Rows
                        '        If pDR(0) IsNot Nothing And Trim(pDR(0).ToString) <> "" Then
                        '            If (lstPreFilterInt(i).Items.Contains(pDR(0)) = False) Then
                        '                lstPreFilterInt(i).Items.Add(pDR(0))
                        '            End If

                        '        End If

                        '    Next
                        '    lstPreFilterInt(i).Items.Insert(0, "<All>")
                        'End If
                        If m_Distinct = "ADO" Then
                            pDT = pDT.DefaultView.ToTable(True, pFDT.Columns(0).ColumnName) 'New String(0) {m_AddressFieldStreetName})
                            'pDT.Rows.Add(GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)
                            If lstPreFilterInt(i).Tag IsNot Nothing Then
                                If CType(lstPreFilterInt(i).Tag, MobileConfigClass.MobileConfigMobileMapConfigSearchPanelAddressSearchPrefilterFieldsPrefilterField).ShowAll.ToUpper = "FALSE" Then
                                Else
                                    'lstPreFilterInt(i).Items.Insert(0, GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)
                                    pDT.Rows.Add(GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)

                                End If
                            Else
                                'lstPreFilterInt(i).Items.Insert(0, GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)
                                pDT.Rows.Add(GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)

                            End If
                            lstPreFilterInt(i).DataSource = pDT
                            lstPreFilterInt(i).DisplayMember = pDT.Columns(0).ColumnName
                            lstPreFilterInt(i).ValueMember = pDT.Columns(0).ColumnName
                            lstPreFilterSt(i).DataSource = pDT
                            lstPreFilterSt(i).DisplayMember = pDT.Columns(0).ColumnName
                            lstPreFilterSt(i).ValueMember = pDT.Columns(0).ColumnName

                        ElseIf m_Distinct = "DataTools" Then
                            'Remove any duplicate street names *GetDataTable does not handle distinct
                            ' Dim pSDistinct As MobileControls.DataTableTools = New MobileControls.DataTableTools
                            ' pDT = pSDistinct.SelectDistinct(pDT, New String(0) {m_AddressFieldStreetName})
                            'Load the street names to the combobox
                        Else
                            lstPreFilterInt(i).Items.Clear()
                            lstPreFilterSt(i).Items.Clear()
                            For Each pDR As DataRow In pDT.Rows
                                If pDR(0) IsNot Nothing And Trim(pDR(0).ToString) <> "" Then
                                    If (lstPreFilterInt(i).Items.Contains(pDR(0)) = False) Then
                                        lstPreFilterInt(i).Items.Add(pDR(0))
                                    End If
                                    If (lstPreFilterSt(i).Items.Contains(pDR(0)) = False) Then
                                        lstPreFilterSt(i).Items.Add(pDR(0))
                                    End If

                                End If

                            Next
                            If lstPreFilterInt(i).Tag IsNot Nothing Then
                                If CType(lstPreFilterInt(i).Tag, MobileConfigClass.MobileConfigMobileMapConfigSearchPanelAddressSearchPrefilterFieldsPrefilterField).ShowAll.ToUpper = "FALSE" Then
                                Else
                                    lstPreFilterInt(i).Items.Insert(0, GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)
                                    'pDT.Rows.Add(GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)

                                End If
                            Else
                                lstPreFilterInt(i).Items.Insert(0, GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)
                                'pDT.Rows.Add(GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)

                            End If
                            If lstPreFilterSt(i).Tag IsNot Nothing Then
                                If CType(lstPreFilterSt(i).Tag, MobileConfigClass.MobileConfigMobileMapConfigSearchPanelAddressSearchPrefilterFieldsPrefilterField).ShowAll.ToUpper = "FALSE" Then
                                Else
                                    lstPreFilterSt(i).Items.Insert(0, GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)
                                    'pDT.Rows.Add(GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)

                                End If
                            Else
                                lstPreFilterSt(i).Items.Insert(0, GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)
                                'pDT.Rows.Add(GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)

                            End If

                            'lstPreFilterInt(i).Items.Insert(0, GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)
                            'lstPreFilterSt(i).Items.Insert(0, GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)
                        End If

                    End If
                    'For Each pDR As DataRow In pDT.Rows
                    '    If pDR(0) IsNot Nothing And Trim(pDR(0).ToString) <> "" Then
                    '        If whichCont = "St" Then
                    '            lstPreFilterSt(i).Items.Add(pDR(0))
                    '        ElseIf whichCont = "Int" Then

                    '            lstPreFilterInt(i).Items.Add(pDR(0))
                    '        Else

                    '            lstPreFilterInt(i).Items.Add(pDR(0))
                    '            lstPreFilterSt(i).Items.Add(pDR(0))
                    '        End If
                    '    End If

                    'Next

                    '    Dim pDRv As DataRowView = cboStreetLayer1.Items(0)



                    ' MsgBox(pDRv.DataView.Table.Columns(0).ColumnName)
                    'Display the street name field
                    If whichCont = "St" Then
                        ' lstPreFilterSt(i).Items.Insert(0, "<All>")
                        lstPreFilterSt(i).SelectedIndex = 0
                        AddHandler lstPreFilterSt(i).SelectedIndexChanged, AddressOf cboPreFilt_SelectChanged

                    ElseIf whichCont = "Int" Then

                        'lstPreFilterInt(i).Items.Insert(0, "<All>")
                        lstPreFilterInt(i).SelectedIndex = 0
                        AddHandler lstPreFilterInt(i).SelectedIndexChanged, AddressOf cboPreFilt_SelectChanged

                    Else

                        'lstPreFilterInt(i).Items.Insert(0, "<All>")
                        'lstPreFilterSt(i).Items.Insert(0, "<All>")
                        lstPreFilterSt(i).SelectedIndex = 0
                        lstPreFilterInt(i).SelectedIndex = 0

                        AddHandler lstPreFilterSt(i).SelectedIndexChanged, AddressOf cboPreFilt_SelectChanged

                        AddHandler lstPreFilterInt(i).SelectedIndexChanged, AddressOf cboPreFilt_SelectChanged

                    End If








                    cboCnt = cboCnt + 1


                Else

                    If whichCont = "St" Then
                        Dim strFldName() As String = lstPreFilterSt(i).Name.Split("|")

                        If lstPreFilterSt(i).Text <> GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText Then

                            If sql = "" Then
                                If pFL.Columns(strFldName(0)).DataType Is System.Type.GetType("System.String") Then

                                    sql = strFldName(0) & " = '" & lstPreFilterSt(i).Text & "'"
                                Else
                                    sql = strFldName(0) & " = " & lstPreFilterSt(i).Text & ""
                                End If

                            Else
                                If pFL.Columns(strFldName(0)).DataType Is System.Type.GetType("System.String") Then

                                    sql = sql & " AND " & strFldName(0) & " = '" & lstPreFilterSt(i).Text & "'"
                                Else
                                    sql = sql & " AND " & strFldName(0) & " = " & lstPreFilterSt(i).Text & ""
                                End If

                            End If
                        End If


                    ElseIf whichCont = "Int" Then
                        Dim strFldName() As String = lstPreFilterInt(i).Name.Split("|")

                        If sql = "" Then


                            If lstPreFilterInt(i).Text <> GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText Then


                                If pFL.Columns(strFldName(0)).DataType Is System.Type.GetType("System.String") Then

                                    sql = strFldName(0) & " = '" & lstPreFilterInt(i).Text & "'"
                                Else
                                    sql = strFldName(0) & " = " & lstPreFilterInt(i).Text & ""
                                End If
                            End If

                        Else
                            If lstPreFilterInt(i).Text <> GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText Then

                                If pFL.Columns(lstPreFilterInt(i).Name).DataType Is System.Type.GetType("System.String") Then

                                    sql = sql & " AND " & strFldName(0) & " = '" & lstPreFilterInt(i).Text & "'"
                                Else
                                    sql = sql & " AND " & strFldName(0) & " = " & lstPreFilterInt(i).Text & ""
                                End If

                            End If
                        End If

                    Else
                        Dim strFldName() As String = lstPreFilterInt(i).Name.Split("|")

                        If lstPreFilterInt(i).Text <> GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText Then

                            If sql = "" Then
                                If pFL.Columns(strFldName(0)).DataType Is System.Type.GetType("System.String") Then

                                    sql = strFldName(0) & " = '" & lstPreFilterInt(i).Text & "'"
                                Else
                                    sql = strFldName(0) & " = " & lstPreFilterInt(i).Text & ""
                                End If

                            Else
                                If pFL.Columns(strFldName(0)).DataType Is System.Type.GetType("System.String") Then

                                    sql = sql & " AND " & strFldName(0) & " = '" & lstPreFilterInt(i).Text & "'"
                                Else
                                    sql = sql & " AND " & strFldName(0) & " = " & lstPreFilterInt(i).Text & ""
                                End If

                            End If
                        End If


                    End If


                    cboCnt = cboCnt + 1
                End If

            Next



            'Cleanup
            '   pML.Dispose()
            pFL = Nothing
            ' pFDT.Dispose()
            pFDT = Nothing
            ' pDT.Dispose()
            pDT = Nothing
            ' pSDistinct = Nothing

        Catch ex As Exception


        End Try
    End Function
    Private Function LoadValuesDrillDownSearch(ByVal intCboChanged As Integer) As Boolean
        Dim pDT As DataTable
        'Remove any duplicate street names *GetDataTable does not handle distinct
        'Dim pSDistinct As MobileControls.DataTableTools
        Try
            If (gpBxAddPntControls.Controls.Count = 0) Then
                Return False
            End If
            'Get the Street Layer
            'Dim pML As MobileCacheMapLayer

            'pML = CType(m_Map.MapLayers(GlobalsFunctions.appConfig.SearchPanel.DrillDownSearches.DrillDownSearch(cboDrillDownLayer.SelectedIndex).LayerName), MobileCacheMapLayer)
            'If not found return
            'If pML Is Nothing Then Return False
            Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = GlobalsFunctions.GetFeatureSource(GlobalsFunctions.appConfig.SearchPanel.DrillDownSearches.DrillDownSearch(cboDrillDownLayer.SelectedIndex).LayerName, m_Map).FeatureSource
            If pFL Is Nothing Then Return False
            'Get all streets names
            Dim pFDT As FeatureDataTable
            ' Dim cboCnt As Integer = 0

            Dim sql As String = ""
            If GlobalsFunctions.appConfig.SearchPanel.DrillDownSearches.DrillDownSearch(cboDrillDownLayer.SelectedIndex).DefQuery <> "" Then
                sql = sql & IIf(sql = "", "", " AND ") & GlobalsFunctions.appConfig.SearchPanel.DrillDownSearches.DrillDownSearch(cboDrillDownLayer.SelectedIndex).DefQuery

            End If
            Dim QF As QueryFilter

            Dim cboCnt As Integer = 0

            For i As Integer = 0 To lstPreFilterPnt.Count - 1

                If cboCnt > intCboChanged Then
                    Dim strFldName() As String = lstPreFilterPnt(i).Name.Split("|")



                    QF = New QueryFilter


                    If GlobalsFunctions.IsNumeric(GlobalsFunctions.appConfig.SearchPanel.DrillDownSearches.DrillDownSearch(cboDrillDownLayer.SelectedIndex).xmax) And _
                       GlobalsFunctions.IsNumeric(GlobalsFunctions.appConfig.SearchPanel.DrillDownSearches.DrillDownSearch(cboDrillDownLayer.SelectedIndex).xmin) And _
                       GlobalsFunctions.IsNumeric(GlobalsFunctions.appConfig.SearchPanel.DrillDownSearches.DrillDownSearch(cboDrillDownLayer.SelectedIndex).ymax) And _
                       GlobalsFunctions.IsNumeric(GlobalsFunctions.appConfig.SearchPanel.DrillDownSearches.DrillDownSearch(cboDrillDownLayer.SelectedIndex).ymin) Then
                        QF.Geometry = New Envelope(New Coordinate(GlobalsFunctions.appConfig.SearchPanel.DrillDownSearches.DrillDownSearch(cboDrillDownLayer.SelectedIndex).xmin, GlobalsFunctions.appConfig.SearchPanel.DrillDownSearches.DrillDownSearch(cboDrillDownLayer.SelectedIndex).ymin), New Coordinate(GlobalsFunctions.appConfig.SearchPanel.DrillDownSearches.DrillDownSearch(cboDrillDownLayer.SelectedIndex).xmax, GlobalsFunctions.appConfig.SearchPanel.DrillDownSearches.DrillDownSearch(cboDrillDownLayer.SelectedIndex).ymax))
                    End If
                    If sql = "" Then
                        QF.WhereClause = "1=1" '"LTRIM(RTRIM(" & strFldName(0) & "))" & " <> ''"
                    Else
                        QF.WhereClause = sql '& " AND " + "LTRIM(RTRIM(" & strFldName(0) & "))" & " <> ''"
                    End If

                    'QF.WhereClause = sql
                    pFDT = pFL.GetDataTable(QF, New String(0) {strFldName(0)})
                    If pFDT.Rows.Count = 0 Then

                        lstPreFilterPnt(i).DataSource = Nothing
                        lstPreFilterPnt(i).Items.Clear()
                        lstPreFilterPnt(i).DisplayMember = Nothing
                        lstPreFilterPnt(i).ValueMember = Nothing
                        lstPreFilterPnt(i).DataBindings.Clear()



                    End If
                    RemoveHandler lstPreFilterPnt(i).SelectedIndexChanged, AddressOf cboAddPnt_SelectChanged

                    pDT = pFDT

                    'If pDT.Columns(0).DataType IsNot GetType(String) Then

                    '    Dim newDataTable As DataTable = pDT.Clone()
                    '    For Each dc As DataColumn In newDataTable.Columns
                    '        If dc.DataType IsNot GetType(String) Then
                    '            dc.DataType = GetType(String)
                    '        End If
                    '    Next

                    '    For Each dr As DataRow In pDT.Rows
                    '        newDataTable.ImportRow(dr)
                    '    Next
                    '    pDT = newDataTable

                    'End If
                    If m_Distinct = "ADO" Then

                        pDT = pDT.DefaultView.ToTable(True, New String(0) {pFDT.Columns(0).ColumnName})

                        lstPreFilterPnt(i).DataSource = pDT
                        lstPreFilterPnt(i).DisplayMember = pFDT.Columns(0).ColumnName 'lstPreFilterPnt(i).Name
                        lstPreFilterPnt(i).ValueMember = pFDT.Columns(0).ColumnName 'lstPreFilterPnt(i).Name

                    ElseIf m_Distinct = "DataTools" Then
                        'Remove any duplicate street names *GetDataTable does not handle distinct
                        ' Dim pSDistinct As MobileControls.DataTableTools = New MobileControls.DataTableTools
                        ' pDT = pSDistinct.SelectDistinct(pDT, New String(0) {m_AddressFieldStreetName})
                        'Load the street names to the combobox
                    Else
                        lstPreFilterPnt(i).Items.Clear()

                        For Each pDR As DataRow In pDT.Rows
                            'If pDR(0) IsNot Nothing And Trim(pDR(0).ToString) <> "" The
                            If pDR(0) Is DBNull.Value Then
                                If (lstPreFilterPnt(i).Items.Contains("<NULL>") = False) Then
                                    lstPreFilterPnt(i).Items.Add("<NULL>")
                                End If

                            Else
                                If (lstPreFilterPnt(i).Items.Contains(pDR(0)) = False) Then
                                    lstPreFilterPnt(i).Items.Add(pDR(0))
                                End If

                            End If

                            ' End If

                        Next
                    End If

                    AddHandler lstPreFilterPnt(i).SelectedIndexChanged, AddressOf cboAddPnt_SelectChanged




                    If sql = "" Then
                        If lstPreFilterPnt(i).Text = "<NULL>" Then
                            sql = strFldName(0) & " is NULL"
                        Else
                            If pFL.Columns(strFldName(0)).DataType Is System.Type.GetType("System.String") Then

                                sql = strFldName(0) & " = '" & lstPreFilterPnt(i).Text & "'"
                            Else
                                sql = strFldName(0) & " = " & lstPreFilterPnt(i).Text & ""
                            End If
                        End If

                    Else
                        If lstPreFilterPnt(i).Text = "<NULL>" Then
                            sql = sql & " AND " & strFldName(0) & " is NULL"
                        Else
                            If pFL.Columns(strFldName(0)).DataType Is System.Type.GetType("System.String") Then

                                sql = sql & " AND " & strFldName(0) & " = '" & lstPreFilterPnt(i).Text & "'"
                            Else
                                sql = sql & " AND " & strFldName(0) & " = " & lstPreFilterPnt(i).Text & ""
                            End If

                        End If
                    End If



                    cboCnt = cboCnt + 1


                Else

                    Dim strFldName() As String = lstPreFilterPnt(i).Name.Split("|")

                    If sql = "" Then
                        If lstPreFilterPnt(i).Text = "<NULL>" Then
                            sql = strFldName(0) & " is NULL"
                        Else
                            If pFL.Columns(strFldName(0)).DataType Is System.Type.GetType("System.String") Then


                                sql = strFldName(0) & " = '" & lstPreFilterPnt(i).Text & "'"

                            Else
                                sql = strFldName(0) & " = " & lstPreFilterPnt(i).Text & ""
                            End If
                        End If

                    Else
                        If lstPreFilterPnt(i).Text = "<NULL>" Then
                            sql = sql & " AND " & strFldName(0) & " is NULL"
                        Else
                            If pFL.Columns(strFldName(0)).DataType Is System.Type.GetType("System.String") Then

                                sql = sql & " AND " & strFldName(0) & " = '" & lstPreFilterPnt(i).Text & "'"
                            Else
                                sql = sql & " AND " & strFldName(0) & " = " & lstPreFilterPnt(i).Text & ""
                            End If

                        End If
                    End If


                    cboCnt = cboCnt + 1
                End If

            Next



            'Cleanup
            '   pML.Dispose()
            pFL = Nothing
            ' pFDT.Dispose()
            pFDT = Nothing
            ' pDT.Dispose()
            pDT = Nothing
            ' pSDistinct = Nothing

        Catch ex As Exception


        End Try
    End Function
    Private Function GenerateFilterEnv() As Envelope
        Dim strSQL As String = ""



        If GlobalsFunctions.IsNumeric(GlobalsFunctions.appConfig.SearchPanel.AddressSearch.xmax) And _
           GlobalsFunctions.IsNumeric(GlobalsFunctions.appConfig.SearchPanel.AddressSearch.xmin) And _
           GlobalsFunctions.IsNumeric(GlobalsFunctions.appConfig.SearchPanel.AddressSearch.ymax) And _
           GlobalsFunctions.IsNumeric(GlobalsFunctions.appConfig.SearchPanel.AddressSearch.ymin) Then
            Return New Envelope(New Coordinate(GlobalsFunctions.appConfig.SearchPanel.AddressSearch.xmin, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.ymin), New Coordinate(GlobalsFunctions.appConfig.SearchPanel.AddressSearch.xmax, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.ymax))
        End If
        Return Nothing


    End Function

    Private Function GenerateSQLCombo(ByVal pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource, ByVal whichContr As String) As String
        Dim strSQL As String = ""
        Dim queryFilt As QueryFilter = New QueryFilter
        Dim pCbo As ComboBox
        'If gpBoxPreFilt.Visible = True Then
        Dim pControlColl As ControlCollection
        If whichContr = "St" Then
            pControlColl = gpBoxPreFiltSt.Controls
        ElseIf whichContr = "Int" Then

            pControlColl = gpBoxPreFiltInt.Controls
        Else
            pControlColl = gpBxAddPntControls.Controls

        End If
        If pControlColl.Count > 0 Then
            For Each cnt As Control In pControlColl
                If TypeOf (cnt) Is ComboBox Then

                    pCbo = cnt
                    Dim strFld() As String = pCbo.Name.Split("|")

                    If pCbo.Text <> GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText And pCbo.Text.Trim <> "" Then
                        If strSQL = "" Then
                            If pCbo.Text = "<NULL>" Then
                                strSQL = strFld(0) & " is NULL"
                            Else
                                If pFL.Columns(strFld(0)).DataType Is System.Type.GetType("System.String") Then
                                    strSQL = strFld(0) & " = '" & pCbo.Text & "'"
                                Else
                                    strSQL = strFld(0) & " = " & pCbo.Text
                                End If
                            End If

                        Else
                            If pCbo.Text = "<NULL>" Then
                                strSQL = strFld(0) & " is NULL"
                            Else
                                If pFL.Columns(strFld(0)).DataType Is System.Type.GetType("System.String") Then

                                    strSQL = strSQL & " AND " & strFld(0) & " = '" & pCbo.Text & "'"
                                Else
                                    strSQL = strSQL & " AND " & strFld(0) & " = " & pCbo.Text
                                End If
                            End If
                        End If
                    Else

                        If strSQL = "" Then
                            If pCbo.Text = "<NULL>" Then
                                strSQL = strFld(0) & " is NULL"
                            Else
                                If pFL.Columns(strFld(0)).DataType Is System.Type.GetType("System.String") Then

                                    strSQL = "LTRIM(RTRIM(" & strFld(0) & "))" & " <> ''"
                                End If
                            End If
                        Else
                            If pCbo.Text = "<NULL>" Then
                                strSQL = strFld(0) & " is NULL"
                            Else
                                If pFL.Columns(strFld(0)).DataType Is System.Type.GetType("System.String") Then

                                    strSQL = strSQL & " AND " & "LTRIM(RTRIM(" & strFld(0) & "))" & " <> ''"
                                End If
                            End If
                        End If


                    End If

                End If
            Next

        End If
        If whichContr = "Int" Then
            If GlobalsFunctions.appConfig.SearchPanel.AddressSearch.DefQuery <> "" Then
                strSQL = strSQL & IIf(strSQL = "", "", " AND ") & GlobalsFunctions.appConfig.SearchPanel.AddressSearch.DefQuery

            End If


        End If
        Return strSQL

    End Function
    'Private Function LoadAddPoints() As Boolean
    '    Try
    '        cboStreetLayer1.DataSource = Nothing
    '        cboStreetLayer1.Items.Clear()
    '        cboStreetLayer1.DisplayMember = Nothing
    '        cboStreetLayer1.ValueMember = Nothing
    '        cboStreetLayer1.DataBindings.Clear()

    '        'Get the Street Layer
    '        Dim pML As MobileCacheMapLayer
    '        pML = CType(m_Map.MapLayers(m_AddressLayer), MobileCacheMapLayer)
    '        'If not found return
    '        If pML Is Nothing Then Return False
    '        Dim pFL as Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = CType(pML.Layer, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)
    '        'Get all streets names
    '        Dim pFDT As FeatureDataTable
    '        Dim strSQL As String = ""
    '        Dim queryFilt As QueryFilter = New QueryFilter

    '        queryFilt.WhereClause = GenerateSQLCombo(pFL, "Pnt")


    '        'End If
    '        pFDT = pFL.GetDataTable(queryFilt, New String(0) {m_AddressFieldStreetName})
    '        If pFDT.Rows.Count = 0 Then
    '            '  MsgBox("The " & m_AddressLayer & " layer does not have any records in it, please refresh this layer")
    '            cboStreetLayer1.Text = ""
    '            cboStreetLayer1.Enabled = False
    '            cboStreetLayer2.DataSource = Nothing
    '            cboStreetLayer2.Text = ""
    '            cboStreetLayer2.Enabled = False


    '            cboStreetRange.DataSource = Nothing
    '            cboStreetRange.Text = ""
    '            cboStreetRange.Enabled = False

    '            Return False

    '        End If
    '        Dim pDT As DataTable = pFDT
    '        'Remove any duplicate street names *GetDataTable does not handle distinct
    '        Dim pSDistinct As MobileControls.DataTableTools = New MobileControls.DataTableTools
    '        pDT = pSDistinct.SelectDistinct(pDT, New String(0) {m_AddressFieldStreetName})
    '        'Load the street names to the combobox
    '        ' cboStreetLayer1.DisplayMember = pDT.Columns(0).ColumnName
    '        'cboStreetLayer1.ValueMember = pDT.Columns(0).ColumnName
    '        'cboStreetLayer1.DataSource = pDT
    '        For Each pDR As DataRow In pDT.Rows
    '            If pDR(0) IsNot Nothing And Trim(pDR(0).ToString) <> "" Then

    '                cboStreetLayer1.Items.Add(pDR(0))
    '            End If

    '        Next
    '        cboStreetLayer1.Enabled = True
    '        cboStreetLayer2.Enabled = True
    '        cboStreetRange.Enabled = True

    '        '    Dim pDRv As DataRowView = cboStreetLayer1.Items(0)



    '        ' MsgBox(pDRv.DataView.Table.Columns(0).ColumnName)
    '        'Display the street name field
    '        cboStreetLayer1.SelectedIndex = 0


    '        'Cleanup
    '        '   pML.Dispose()
    '        pFL = Nothing
    '        ' pFDT.Dispose()
    '        pFDT = Nothing
    '        ' pDT.Dispose()
    '        pDT = Nothing
    '        pSDistinct = Nothing

    '    Catch ex As Exception
    '        MsgBox(ex.Message)


    '    End Try
    'End Function
    Private Function LoadStreetsPrimary() As Boolean
        Try
            cboStreetLayer1.DataSource = Nothing
            cboStreetLayer1.Items.Clear()
            cboStreetLayer1.DisplayMember = Nothing
            cboStreetLayer1.ValueMember = Nothing
            cboStreetLayer1.DataBindings.Clear()

            'Get the Street Layer
            'Dim pML As MobileCacheMapLayer
            'pML = CType(m_Map.MapLayers(m_AddressLayer), MobileCacheMapLayer)
            'If not found return
            'If pML Is Nothing Then Return False
            Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = GlobalsFunctions.GetFeatureSource(m_AddressLayer, m_Map).FeatureSource
            If pFL Is Nothing Then Return False
            'Get all streets names

            Dim pFDT As FeatureDataTable
            Dim strSQL As String = ""
            Dim queryFilt As QueryFilter = New QueryFilter

            Dim sql As String

            'queryFilt.WhereClause 
            sql = GenerateSQLCombo(pFL, "Int")
            If sql = "" Then
                queryFilt.WhereClause = "LTRIM(RTRIM(" & m_AddressFieldStreetName & "))" & " <> ''"
            Else
                queryFilt.WhereClause = sql & " AND " + "LTRIM(RTRIM(" & m_AddressFieldStreetName & "))" & " <> ''"
            End If
            queryFilt.Geometry = GenerateFilterEnv()

            ' queryFilt.WhereClause = sql


            'End If
            If pFL.Columns(m_AddressFieldStreetName) Is Nothing Then
                Return Nothing

            End If

            pFDT = pFL.GetDataTable(queryFilt, New String(0) {m_AddressFieldStreetName})

            If pFDT.Rows.Count = 0 Then
                '  MsgBox("The " & m_AddressLayer & " layer does not have any records in it, please refresh this layer")
                cboStreetLayer1.Text = ""
                cboStreetLayer1.Enabled = False
                cboStreetLayer2.DataSource = Nothing
                cboStreetLayer2.Text = ""
                cboStreetLayer2.Enabled = False


                cboStreetRange.DataSource = Nothing
                cboStreetRange.Text = ""
                cboStreetRange.Enabled = False

                Return False

            End If
            Dim pDT As DataTable = pFDT
            If m_Distinct = "ADO" Then

                pDT = pDT.DefaultView.ToTable(True, pFDT.Columns(0).ColumnName) 'New String(0) {m_AddressFieldStreetName})

                cboStreetLayer1.DataSource = pDT
                cboStreetLayer1.DisplayMember = pDT.Columns(0).ColumnName
                cboStreetLayer1.ValueMember = pDT.Columns(0).ColumnName

            ElseIf m_Distinct = "DataTools" Then
                'Remove any duplicate street names *GetDataTable does not handle distinct
                ' Dim pSDistinct As MobileControls.DataTableTools = New MobileControls.DataTableTools
                ' pDT = pSDistinct.SelectDistinct(pDT, New String(0) {m_AddressFieldStreetName})
                'Load the street names to the combobox
            Else
                cboStreetLayer1.Items.Clear()

                For Each pDR As DataRow In pDT.Rows
                    If pDR(0) IsNot Nothing And Trim(pDR(0).ToString) <> "" Then
                        If (cboStreetLayer1.Items.Contains(pDR(0)) = False) Then
                            cboStreetLayer1.Items.Add(pDR(0))
                        End If

                    End If

                Next
            End If


            cboStreetLayer1.Enabled = True
            cboStreetLayer2.Enabled = True
            cboStreetRange.Enabled = True

            cboStreetLayer1.SelectedIndex = 0


            'Cleanup
            '   pML.Dispose()
            pFL = Nothing
            ' pFDT.Dispose()
            pFDT = Nothing
            ' pDT.Dispose()
            pDT = Nothing
            '  pSDistinct = Nothing

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing



        End Try
    End Function
    Private Function LoadStreetsRange(ByVal primaryStreet As String) As Boolean
        Try
            'Get all street ranges for a street

            'If layer is not set up correctly, return
            If m_bAddress_GeocordInit = False Then Return False
            'Clear out the existing data
            cboStreetRange.DataSource = Nothing
            'Create a new query filter
            'Dim pML As MobileCacheMapLayer
            'pML = CType(m_Map.MapLayers(m_AddressLayer), MobileCacheMapLayer)
            Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = GlobalsFunctions.GetFeatureSource(m_AddressLayer, m_Map).FeatureSource

            Dim pQFilt As QueryFilter = New QueryFilter
            'Query for all primary streets
            Dim strAddSQL As String = GenerateSQLCombo(pFL, "Int")
            If strAddSQL <> "" Then
                pQFilt.WhereClause = m_AddressFieldStreetName & " = '" & primaryStreet & "'" & " AND " & strAddSQL
            Else
                pQFilt.WhereClause = m_AddressFieldStreetName & " = '" & primaryStreet & "'"
            End If
            pQFilt.Geometry = GenerateFilterEnv()

            '            pQFilt.WhereClause = m_AddressFieldStreetName & " = '" & primaryStreet & "'"
            'Get the Street Layer
            Dim pFDT As FeatureDataTable
            'Get a datatable of all primary streets, return only fields of interest
            pFDT = pFL.GetDataTable(pQFilt, New String(5) {m_AddressFieldStreetName, m_AddressFieldLeftFrom, m_AddressFieldLeftTo, m_AddressFieldRightFrom, m_AddressFieldRightTo, pFL.GeometryColumnName})
            'Add a new column to combine ranges for dislay
            pFDT.Columns.Add(New DataColumn("DisplayText", System.Type.GetType("System.String")))
            'Loop through each rows and calc display
            For Each pDr As DataRow In pFDT.Rows
                Dim strDisplay As String

                strDisplay = "Ranges: " & CStr(pDr.Item(m_AddressFieldLeftFrom)) & " - " & CStr(pDr.Item(m_AddressFieldLeftTo))
                strDisplay = strDisplay & " and " & CStr(pDr.Item(m_AddressFieldRightFrom)) & " - " & CStr(pDr.Item(m_AddressFieldRightTo))
                'Set up display text
                pDr("DisplayText") = strDisplay
            Next

            'Bind to drop down

            cboStreetRange.DataSource = pFDT
            cboStreetRange.DisplayMember = "DisplayText"
            cboStreetRange.ValueMember = "DisplayText"

            'Clean up

            pQFilt = Nothing
            pFL = Nothing
            'pFDT.Dispose()
            pFDT = Nothing
            ' pML.Dispose()
            ' pML = Nothing

        Catch ex As Exception
            Return False

        End Try
        Return True

    End Function
    Private Function LoadStreetsIntersection(ByVal primaryStreet As String) As Boolean
        Try
            'Get all intersecting streets

            'If layer is not set up correctly, return
            If m_bAddress_GeocordInit = False Then Return False
            'clear out existing data binds
            cboStreetLayer2.DataSource = Nothing

            'create a new query filter
            Dim pQFilt As QueryFilter = New QueryFilter
            'Set up select sql
            'Dim pML As MobileCacheMapLayer
            'pML = CType(m_Map.MapLayers(m_AddressLayer), MobileCacheMapLayer)

            Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = GlobalsFunctions.GetFeatureSource(m_AddressLayer, m_Map).FeatureSource

            Dim strAddSQL As String = GenerateSQLCombo(pFL, "Int")
            If strAddSQL <> "" Then
                pQFilt.WhereClause = m_AddressFieldStreetName & " = '" & primaryStreet & "'" & " AND " & strAddSQL
            Else
                pQFilt.WhereClause = m_AddressFieldStreetName & " = '" & primaryStreet & "'"
            End If
            pQFilt.Geometry = GenerateFilterEnv()

            'Get the Street layer
            'Loop through all layers using a data reader
            Dim pFDR As FeatureDataReader
            pFDR = pFL.GetDataReader(pQFilt, New String(1) {m_AddressFieldStreetName, pFL.GeometryColumnName})
            'Create a new datatable for hold all intersecting streets
            Dim pDT As DataTable = Nothing
            'Init DataTable Tools
            'Dim pDTTools As New MobileControls.DataTableTools
            'Loop through each street

            While pFDR.Read()
                'Geometry of primary street 
                Dim pGeo As Geometry
                pGeo = CType(pFDR.Item(pFL.GeometryColumnName), Geometry)

                'Create a new query filter to select intersection streets
                pQFilt = New QueryFilter
                'use Primary street geometry
                pQFilt.Geometry = pGeo
                If GlobalsFunctions.appConfig.SearchPanel.AddressSearch.DefQuery <> "" Then
                    pQFilt.WhereClause = GlobalsFunctions.appConfig.SearchPanel.AddressSearch.DefQuery

                End If

                'All streets that intersect
                pQFilt.GeometricRelationship = Geometries.GeometricRelationshipType.Intersect
                'Get all intersection streets
                Dim pDTTemp As DataTable = pFL.GetDataTable(pQFilt, New String(0) {m_AddressFieldStreetName})
                'Loop through all results
                'pDT = pDTTemp.Copy
                For Each pDr As DataRow In pDTTemp.Rows
                    If pDT Is Nothing Then

                        '   pDT = New FeatureDataTable(pFDR)


                        'pDT = pDTTemp.Clone
                        'pDT.Rows.Clear()
                        'pDT.
                        pDT = GlobalsFunctions.copySchema(pDTTemp)
                    End If
                    If CStr(pDr(m_AddressFieldStreetName).ToString) <> primaryStreet And pDr(m_AddressFieldStreetName) IsNot Nothing Then



                        ''Load the intersection street to the intersecting streets datatable
                        Try
                            If pDr(m_AddressFieldStreetName) Is Nothing Then
                                pDr(m_AddressFieldStreetName) = ""

                            ElseIf CStr(pDr(m_AddressFieldStreetName).ToString).Trim = "" Then

                            Else

                                pDT.ImportRow(pDr)
                            End If

                        Catch ex As Exception
                            'Trap the error when trying to load a street that had already loaded

                        End Try

                    End If

                Next pDr
                'Cleanup


                'Clean up
                '    pDTTemp.Dispose()

                pGeo = Nothing
            End While
            'Close the data reader
            pFDR.Close()
            'Remove any duplicates
            If pDT IsNot Nothing Then



                If m_Distinct = "ADO" Then

                    pDT = pDT.DefaultView.ToTable(True, pDT.Columns(0).ColumnName) 'New String(0) {m_AddressFieldStreetName})

                    cboStreetLayer2.DataSource = pDT
                    cboStreetLayer2.DisplayMember = pDT.Columns(0).ColumnName
                    cboStreetLayer2.ValueMember = pDT.Columns(0).ColumnName

                ElseIf m_Distinct = "DataTools" Then
                    'Remove any duplicate street names *GetDataTable does not handle distinct
                    ' Dim pSDistinct As MobileControls.DataTableTools = New MobileControls.DataTableTools
                    ' pDT = pSDistinct.SelectDistinct(pDT, New String(0) {m_AddressFieldStreetName})
                    'Load the street names to the combobox
                Else
                    cboStreetLayer2.Items.Clear()

                    For Each pDR As DataRow In pDT.Rows
                        If pDR(0) IsNot Nothing And Trim(pDR(0).ToString) <> "" Then
                            If (cboStreetLayer2.Items.Contains(pDR(0)) = False) Then
                                cboStreetLayer2.Items.Add(pDR(0))
                            End If

                        End If

                    Next
                    If cboStreetLayer2.Items.Count > 0 Then
                        cboStreetLayer2.SelectedIndex = 0
                    End If


                End If







            End If
            'Clean up
            ' pDTDistinct.Dispose()

            '   pDT.Dispose()
            pDT = Nothing
            '   pFDR.Dispose()
            pFDR = Nothing
            pFL = Nothing
            '   pML.Dispose()
            'pML = Nothing
            '   pFDR.Dispose()
            pFDR = Nothing
            pQFilt = Nothing

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try
    End Function
    Private Function LoadFields(ByVal layerDet As MobileConfigClass.MobileConfigMobileMapConfigSearchPanelSearchLayersSearchLayer) As Boolean
        Try

            'Load all fields for a specfied layer to the Search Drop down

            'Get the target layer
            ' Dim pLay As MobileCacheMapLayer = CType(GlobalsFunctions.GetMapLayer(layerDet.Name, m_Map), MobileCacheMapLayer)

            ' If pLay Is Nothing Then Return False
            Dim pFl As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = GlobalsFunctions.GetFeatureSource(layerDet.Name, m_Map).FeatureSource
            If pFl Is Nothing Then Return False
            'Create a new list of field names
            Dim pList As IList = New List(Of DataColumn)
            'Add All Fields option
            Dim pAllVal As DataColumn = New DataColumn(GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText)
            pAllVal.Caption = GlobalsFunctions.appConfig.SearchPanel.UIComponents.AllText
            pList.Add(pAllVal)

            Dim pDisplayCap As String = ""
            'Loop through each field
            If layerDet.Fields Is Nothing Then
                For Each pDC As DataColumn In pFl.Columns
                    'Remove any fields not searchable
                    If pDC.ColumnName <> pFl.GeometryColumnName And _
                            pDC.ColumnName <> pFl.GlobalIdColumnName And _
                            UCase(pDC.ColumnName) <> UCase("shape.area") And _
                            UCase(pDC.ColumnName) <> UCase("shape.len") And _
                            UCase(pDC.ColumnName) <> UCase("shape_len") And _
                            UCase(pDC.ColumnName) <> UCase("shape_area") And _
                            pDC.DataType IsNot System.Type.GetType("System.Byte[]") And _
                            pDC.DataType.FullName <> "System.Drawing.Bitmap" And _
                            pDC.ColumnName <> pFl.GeometryColumnName Then
                        'Add field

                        pList.Add(pDC)
                        If UCase(pFl.DisplayColumnName) = UCase(pDC.ColumnName) Then
                            pDisplayCap = pDC.Caption
                        End If
                    End If
                    ' pDC.Dispose()
                    pDC = Nothing

                Next
            ElseIf layerDet.Fields.Field Is Nothing Then
                For Each pDC As DataColumn In pFl.Columns
                    'Remove any fields not searchable
                    If pDC.ColumnName <> pFl.GeometryColumnName And _
                            pDC.ColumnName <> pFl.GlobalIdColumnName And _
                            UCase(pDC.ColumnName) <> UCase("shape.area") And _
                            UCase(pDC.ColumnName) <> UCase("shape.len") And _
                            UCase(pDC.ColumnName) <> UCase("shape_len") And _
                            UCase(pDC.ColumnName) <> UCase("shape_area") And _
                            pDC.DataType IsNot System.Type.GetType("System.Byte[]") And _
                            pDC.DataType.FullName <> "System.Drawing.Bitmap" And _
                            pDC.ColumnName <> pFl.GeometryColumnName Then
                        'Add field

                        pList.Add(pDC)
                        If UCase(pFl.DisplayColumnName) = UCase(pDC.ColumnName) Then
                            pDisplayCap = pDC.Caption
                        End If
                    End If
                    ' pDC.Dispose()
                    pDC = Nothing

                Next
            ElseIf layerDet.Fields.Field.Count = 0 Then
                For Each pDC As DataColumn In pFl.Columns
                    'Remove any fields not searchable
                    If pDC.ColumnName <> pFl.GeometryColumnName And _
                            pDC.ColumnName <> pFl.GlobalIdColumnName And _
                            UCase(pDC.ColumnName) <> UCase("shape.area") And _
                            UCase(pDC.ColumnName) <> UCase("shape.len") And _
                            UCase(pDC.ColumnName) <> UCase("shape_len") And _
                            UCase(pDC.ColumnName) <> UCase("shape_area") And _
                            pDC.DataType IsNot System.Type.GetType("System.Byte[]") And _
                            pDC.DataType.FullName <> "System.Drawing.Bitmap" And _
                            pDC.ColumnName <> pFl.GeometryColumnName Then
                        'Add field

                        pList.Add(pDC)
                        If UCase(pFl.DisplayColumnName) = UCase(pDC.ColumnName) Then
                            pDisplayCap = pDC.Caption
                        End If
                    End If
                    ' pDC.Dispose()
                    pDC = Nothing

                Next
            Else
                For Each fldRest As MobileConfigClass.MobileConfigMobileMapConfigSearchPanelSearchLayersSearchLayerFieldsField In layerDet.Fields.Field
                    If (pFl.Columns.Contains(fldRest.Name)) Then
                        Dim pDC As DataColumn = pFl.Columns(fldRest.Name)
                        pList.Add(pDC)
                        If UCase(pFl.DisplayColumnName) = UCase(pDC.ColumnName) Then
                            pDisplayCap = pDC.Caption
                        End If
                    End If
                Next
                If pList.Count = 1 Then
                    For Each pDC As DataColumn In pFl.Columns
                        'Remove any fields not searchable
                        If pDC.ColumnName <> pFl.GeometryColumnName And _
                                pDC.ColumnName <> pFl.GlobalIdColumnName And _
                                UCase(pDC.ColumnName) <> UCase("shape.area") And _
                                UCase(pDC.ColumnName) <> UCase("shape.len") And _
                                UCase(pDC.ColumnName) <> UCase("shape_len") And _
                                UCase(pDC.ColumnName) <> UCase("shape_area") And _
                                pDC.DataType IsNot System.Type.GetType("System.Byte[]") And _
                                pDC.DataType.FullName <> "System.Drawing.Bitmap" And _
                                pDC.ColumnName <> pFl.GeometryColumnName Then
                            'Add field

                            pList.Add(pDC)
                            If UCase(pFl.DisplayColumnName) = UCase(pDC.ColumnName) Then
                                pDisplayCap = pDC.Caption
                            End If
                        End If
                        ' pDC.Dispose()
                        pDC = Nothing

                    Next
                End If
            End If

            'Bind to drop down
            cboSearchField.DataSource = pList
            cboSearchField.DisplayMember = "Caption"
            cboSearchField.ValueMember = "ColumnName"
            'chkSearchField.DataSource = pList
            'chkSearchField.DisplayMember = "Caption"
            'chkSearchField.ValueMember = "ColumnName"
            'Set default search field to display field specifed by the service
            If pDisplayCap <> "" Then
                cboSearchField.Text = pDisplayCap
            End If

            'Cleanup
            pFl = Nothing
            '   pLay.Dispose()
            'pLay = Nothing

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try

    End Function
    Private Function findFeatures(ByVal strLayName As String, ByVal strFields As IList, _
                                 ByVal strSearchValue As String, ByVal bFindSim As Boolean) As DataTable
        'Create a new query filter
        Dim pQueryFilter As QueryFilter
        pQueryFilter = New QueryFilter
        pQueryFilter.WhereClause = ""

        'Get the layer to search
        'Dim pLay As MobileCacheMapLayer = CType(m_Map.MapLayers(strLayName), MobileCacheMapLayer)
        'If pLay Is Nothing Then Return Nothing
        'Get the FeatureSource
        Dim pFl As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = GlobalsFunctions.GetFeatureSource(strLayName, m_Map).FeatureSource 'CType(pLay.Layer, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)
        'Get all the fields for the layer
        Dim pDcs As ReadOnlyColumnCollection = pFl.Columns

        Dim pDc As DataColumn
        Dim pStrWhere As String = ""
        Dim pStrFindAll As String
        Dim pStrFindOper As String
        'If finding like features, set up connecting operators
        If bFindSim Then
            pStrFindAll = "*"
            pStrFindOper = " LIKE "
        Else
            pStrFindAll = ""
            pStrFindOper = " = "
        End If
        'Loop through each field in the search field list
        For Each strFld As String In strFields
            'Get the field
            pDc = pDcs.Item(strFld)
            'Make sure the field exist
            If Not pDc Is Nothing Then
                'If the where clause isnot empty, add a connecting word
                If pStrWhere <> "" Then
                    pStrWhere = pStrWhere & " or "
                End If
                'Check for type of field 
                If pDc.DataType Is System.Type.GetType("System.String") Then
                    pStrWhere = pStrWhere & strFld & pStrFindOper & "'" & pStrFindAll & strSearchValue & pStrFindAll & "'"
                ElseIf pDc.DataType Is System.Type.GetType("System.Integer") Or pDc.DataType Is System.Type.GetType("System.Int32") Or pDc.DataType Is System.Type.GetType("System.Int16") Then
                    'Make sure the value to search for is numeric
                    If IsNumeric(strSearchValue) Then
                        'If it is a decimal numer, except integers field
                        If strSearchValue.IndexOf(".") > 0 Then
                            'Reset the where cause to remove the connecting word
                            If pStrWhere.Length <= 4 Then
                                pStrWhere = ""
                            Else

                                pStrWhere = pStrWhere.Substring(0, pStrWhere.Length - 4)
                            End If


                        Else
                            'Add the field to the search list
                            pStrWhere = pStrWhere & strFld & " = " & strSearchValue & ""
                        End If
                    Else
                        'Not numeric, exlude it 
                        If pStrWhere.Length <= 4 Then
                            pStrWhere = ""
                        Else
                            pStrWhere = pStrWhere.Substring(0, pStrWhere.Length - 4)
                        End If
                    End If
                ElseIf pDc.DataType Is System.Type.GetType("System.Boolean") Then
                    'if the search value is not true or false, exclude it 
                    If UCase(strSearchValue) = "TRUE" Or UCase(strSearchValue) = "FALSE" Then
                        pStrWhere = pStrWhere & strFld & pStrFindOper & pStrFindAll & strSearchValue & pStrFindAll & ""

                    Else
                        'Remove the connecting words, exclude the field
                        If pStrWhere.Length <= 4 Then
                            pStrWhere = ""
                        Else
                            pStrWhere = pStrWhere.Substring(0, pStrWhere.Length - 4)
                        End If
                    End If

                ElseIf pDc.DataType Is System.Type.GetType("System.Decimal") Or pDc.DataType Is System.Type.GetType("System.Double") Or pDc.DataType Is System.Type.GetType("System.Single") Then
                    'Determine if the search values is numeric
                    If IsNumeric(strSearchValue) Then

                        pStrWhere = pStrWhere & strFld & " = " & strSearchValue & ""
                    Else
                        'Remove the connecting words
                        If pStrWhere.Length <= 4 Then
                            pStrWhere = ""
                        Else
                            pStrWhere = pStrWhere.Substring(0, pStrWhere.Length - 4)
                        End If
                    End If
                ElseIf pDc.DataType Is System.Type.GetType("System.DateTime") Then
                    'Check to see if search value is a date
                    If IsDate(strSearchValue) Then
                        pStrWhere = pStrWhere & strFld & " = #" & Convert.ToDateTime(strSearchValue) & "#"
                    Else
                        If pStrWhere.Length <= 4 Then
                            pStrWhere = ""
                        Else
                            pStrWhere = pStrWhere.Substring(0, pStrWhere.Length - 4)
                        End If
                    End If
                End If
            End If
        Next
        'Apply where clause to filter
        pQueryFilter.WhereClause = pStrWhere
        Try
            'If the where clause is empty exit
            If pQueryFilter.WhereClause = "" Then Return Nothing
            'Check to make sure features match our search
            Dim pCnt As Integer = pFl.GetFeatureCount(pQueryFilter)
            If pCnt = 0 Then
                Return Nothing
                Exit Function
            End If
            'Get the features that match the query
            Dim pFLDT As FeatureDataTable = pFl.GetDataTable(pQueryFilter)


            'Cleanup

            pQueryFilter = Nothing
            'pLay = Nothing
            pFl = Nothing
            pDcs = Nothing
            pDc = Nothing

            Return pFLDT

        Catch ex As Exception
            Return Nothing

        End Try


    End Function

    Private Function DetermineAddressLocation(ByVal pGeo As Geometry, ByVal currentAddress As Integer, ByVal StartRange As Integer, ByVal endRange As Integer) As Coordinate
        'function to determine the address location on the line

        'If the geometry is not a polyline, return
        If Not TypeOf pGeo Is Polyline Then Return Nothing
        'get the polyline passed in
        Dim pPl As Polyline = CType(pGeo, Polyline)
        'If the target address is the start or the end 
        If currentAddress = StartRange Then
            'Return the starting coordinate
            Dim pCoordColl As CoordinateCollection = pPl.GetPart(0)
            pPl = Nothing
            Return pCoordColl(0)
        ElseIf currentAddress = endRange Then
            'return the last coordinate
            Dim pCoordColl As CoordinateCollection = pPl.GetPart(pPl.PartCount - 1)
            pPl = Nothing
            Return pCoordColl(pCoordColl.Count - 1)
        End If
        'Get the length of the line
        Dim dblLen As Double = GlobalsFunctions.getPolylineLength(pPl, m_Map, m_Map.SpatialReference.Unit)

        'Get the range of address
        Dim intRdRange As Integer = endRange - StartRange
        'Get the number of address down the street
        Dim dblDistDownRd As Double = currentAddress - StartRange
        'Determine percentage that address is down the range
        Dim percDownRd As Double = dblDistDownRd / intRdRange
        'Determine length down the road
        Dim lenDownRoad As Double = dblLen * percDownRd

        Dim pDistSoFar As Double
        Dim pLastCoord As Coordinate = Nothing
        Dim pEndCoord As Coordinate = Nothing
        'Find the segment the address will fall on
        For Each pRt As CoordinateCollection In pPl.Parts
            pLastCoord = Nothing
            'loop through all coordinates, creating two point line segments
            For Each pCoord As Coordinate In pRt
                'If the first time through, store the first coordinate
                If pLastCoord Is Nothing Then
                    pLastCoord = pCoord
                Else
                    'check the length between the last point and the current one
                    If lenDownRoad < pDistSoFar + GlobalsFunctions.distanceBetweenPoints(pLastCoord, pCoord) Then
                        'If the length of the past segments and the current is greater then the distance of the address, store the coordinates and exit loop
                        pEndCoord = pCoord
                        Exit For
                    Else
                        'Check the next segment
                        pDistSoFar = pDistSoFar + GlobalsFunctions.distanceBetweenPoints(pLastCoord, pCoord)
                        pEndCoord = pCoord
                    End If
                End If
                pCoord = Nothing
            Next
        Next
        'Find the xy for the address location
        Dim pGCXY() As Double = locationDownLine(lenDownRoad - pDistSoFar, pLastCoord, pEndCoord)
        'Create a new point at the GC location
        Dim pNewCoord As New Coordinate(pGCXY(0), pGCXY(1))

        'Cleanup
        pPl = Nothing
        pEndCoord = Nothing
        pLastCoord = Nothing

        'Return
        Return pNewCoord

    End Function
    Private Function locationDownLine(ByVal distance As Double, ByVal pcoord1 As Coordinate, ByVal pcoord2 As Coordinate) As Double()
        'find the slop of the sline
        Dim dblSlope As Double = GlobalsFunctions.slope(pcoord1, pcoord2)
        'Determine x direction of the line
        Dim bLineEastToWest As Boolean = False
        If pcoord1.X > pcoord2.X Then
            bLineEastToWest = True

        End If

        Dim xGuess As Double
        Dim yGuess As Double
        'if the line is going from the right to the left
        If bLineEastToWest = True Then
            'set up the outer boundaries
            Dim pOutsideVal As Double = pcoord1.X
            Dim pInsideVal As Double = pcoord2.X

            Dim pDbl As Double
            'loop until the location is within a tolerence
            Do

                'Split the distance that the point falls on in half
                xGuess = (pOutsideVal + pInsideVal) / 2
                'Get the Y coordinate based on the slope and xguess
                yGuess = dblSlope * (xGuess - pcoord1.X) + pcoord1.Y
                '  Dim pOutCoord As New Coordinate(pOutsideVal, dblSlope * (pOutsideVal - pcoord1.X) + pcoord1.Y)

                'find the distance between the points
                pDbl = GlobalsFunctions.distanceBetweenPoints(pcoord1, New Coordinate(xGuess, yGuess))
                'If the guess distance less then the address distance, move the guess to the outside
                If pDbl < distance Then
                    'pInsideVal = pOutsideVal

                    pOutsideVal = xGuess

                Else

                    pInsideVal = xGuess

                End If
            Loop Until ((Math.Abs(CInt(pDbl)) - Math.Abs(CInt(distance)))) < 5


        Else
            Dim pOutsideVal As Double = pcoord2.X
            Dim pInsideVal As Double = pcoord1.X
            xGuess = (pOutsideVal + pInsideVal) / 2

            Dim pDbl As Double
            'loop until the location is within a tolerence
            Do
                'Split the distance that the point falls on in half
                xGuess = (pOutsideVal + pInsideVal) / 2
                'Get the Y coordinate based on the slope and xguess
                yGuess = dblSlope * (xGuess - pcoord1.X) + pcoord1.Y
                'Get the coordinate of the inner boundary
                '   Dim pInCoord As New Coordinate(pInsideVal, dblSlope * (pInsideVal - pcoord1.X) + pcoord1.Y)

                'Get the distance between the guess 
                pDbl = GlobalsFunctions.distanceBetweenPoints(pcoord1, New Coordinate(xGuess, yGuess))
                'if the guess distance is great then the address distance
                If pDbl > distance Then
                    pOutsideVal = xGuess

                Else

                    pInsideVal = xGuess

                End If
            Loop Until (Math.Abs(CInt(pDbl)) - Math.Abs(CInt(distance))) < 5

        End If

        Dim pNewXY(1) As Double

        pNewXY(0) = xGuess
        pNewXY(1) = yGuess
        'Return the found XY
        Return pNewXY

    End Function

    Private Function Geocode(ByVal street As String) As FeatureDataTable
        Try


            'return is address parems are not set up

            If m_bAddress_GeocordInit = False Then Return Nothing

            'functions to locate a street based on the range values
            'Dim pLay As MobileCacheMapLayer = CType(m_Map.MapLayers(m_AddressLayer), MobileCacheMapLayer)

            ' If pLay Is Nothing Then Return Nothing
            'Get the feature layer for the map layer
            Dim pFl As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = GlobalsFunctions.GetFeatureSource(m_AddressLayer, m_Map).FeatureSource
            'Create a new query filter
            Dim pQFilt As QueryFilter = New QueryFilter

            Dim pSqlString As String
            'Find a street like the value entered
            pSqlString = "(" & m_AddressFieldStreetName & " LIKE '*" & street & "*' OR "
            pSqlString = pSqlString & m_AddressFieldStreetName & " = '" & street & "' OR "
            pSqlString = pSqlString & m_AddressFieldStreetName & " LIKE '%" & street & "%'" & ")"

            Dim addSql As String = GenerateSQLCombo(pFl, "St")
            If addSql <> "" Then
                pSqlString = pSqlString & " AND " & addSql
            End If

            'pSqlString = pSqlString & " AND (('" & housenumber & "' >= " & m_AddressFieldLeftFrom & " AND " & "'" & housenumber & "' <= " & m_AddressFieldLeftTo & ")"
            '  pSqlString = pSqlString & " OR ('" & housenumber & "' >= " & m_AddressFieldRightFrom & " AND " & "'" & housenumber & "' <= " & m_AddressFieldRightTo & "))"
            'Apply the query string
            pQFilt.WhereClause = pSqlString
            'Limit the return fields
            Dim pStr(5) As String
            pStr(0) = m_AddressFieldStreetName
            pStr(1) = m_AddressFieldLeftFrom
            pStr(2) = m_AddressFieldLeftTo
            pStr(3) = m_AddressFieldRightFrom
            pStr(4) = m_AddressFieldRightTo
            pStr(5) = pFl.GeometryColumnName
            'Get the features that match
            'pFl.GetDataReader(pQFilt, pStr)
            Dim pFDT As FeatureDataTable = pFl.GetDataTable(pQFilt, pStr)



            'cleanup

            'pLay = Nothing
            pFl = Nothing
            pQFilt = Nothing

            'Return the found records
            Return pFDT
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
            Return Nothing


        End Try
    End Function

    Private Function Geocode(ByVal housenumber As String, ByVal street As String) As FeatureDataTable
        Try


            'return is address parems are not set up

            If m_bAddress_GeocordInit = False Then Return Nothing

            'functions to locate a street based on the range values
            'Dim pLay As MobileCacheMapLayer = CType(m_Map.MapLayers(m_AddressLayer), MobileCacheMapLayer)

            'If pLay Is Nothing Then Return Nothing
            'Get the feature layer for the map layer
            Dim pFl As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = GlobalsFunctions.GetFeatureSource(m_AddressLayer, m_Map).FeatureSource
            'Create a new query filter
            Dim pQFilt As QueryFilter = New QueryFilter

            Dim pSqlString As String
            'Find a street like the value entered
            pSqlString = "(" & m_AddressFieldStreetName & " LIKE '*" & street & "*' OR "
            pSqlString = pSqlString & m_AddressFieldStreetName & " = '" & street & "' OR "
            pSqlString = pSqlString & m_AddressFieldStreetName & " LIKE '%" & street & "%'" & ")"

            '   'Check both address range fields
            If pFl.Columns(m_AddressFieldLeftFrom).DataType Is System.Type.GetType("System.String") Then
                pSqlString = pSqlString & " AND (('" & housenumber & "' >= " & m_AddressFieldLeftFrom & ""
            Else
                pSqlString = pSqlString & " AND ((" & housenumber & " >= " & m_AddressFieldLeftFrom & ""
            End If

            If pFl.Columns(m_AddressFieldLeftTo).DataType Is System.Type.GetType("System.String") Then
                pSqlString = pSqlString & " AND '" & housenumber & "' <= " & m_AddressFieldLeftTo & ""
            Else
                pSqlString = pSqlString & " AND " & housenumber & " <= " & m_AddressFieldLeftTo & ""
            End If
            pSqlString = pSqlString & ") OR ("
            If pFl.Columns(m_AddressFieldRightFrom).DataType Is System.Type.GetType("System.String") Then
                pSqlString = pSqlString & "'" & housenumber & "' >= " & m_AddressFieldRightFrom & ""
            Else
                pSqlString = pSqlString & "" & housenumber & " >= " & m_AddressFieldRightFrom & ""
            End If

            If pFl.Columns(m_AddressFieldRightTo).DataType Is System.Type.GetType("System.String") Then
                pSqlString = pSqlString & " AND '" & housenumber & "'  <= " & m_AddressFieldRightTo & ")"
            Else
                pSqlString = pSqlString & " AND " & housenumber & "  <= " & m_AddressFieldRightTo & ")"
            End If


            'pSqlString = pSqlString & " OR ("
            If pFl.Columns(m_AddressFieldLeftFrom).DataType Is System.Type.GetType("System.String") Then
                pSqlString = pSqlString & " OR ('" & housenumber & "' >= " & m_AddressFieldLeftTo & ""
            Else
                pSqlString = pSqlString & " OR (" & housenumber & " >= " & m_AddressFieldLeftTo & ""
            End If

            If pFl.Columns(m_AddressFieldLeftTo).DataType Is System.Type.GetType("System.String") Then
                pSqlString = pSqlString & " AND '" & housenumber & "' <= " & m_AddressFieldLeftFrom & ""
            Else
                pSqlString = pSqlString & " AND " & housenumber & " <= " & m_AddressFieldLeftFrom & ""
            End If
            pSqlString = pSqlString & ") OR ("
            If pFl.Columns(m_AddressFieldRightFrom).DataType Is System.Type.GetType("System.String") Then
                pSqlString = pSqlString & "'" & housenumber & "' >= " & m_AddressFieldRightTo & ""
            Else
                pSqlString = pSqlString & "" & housenumber & " >= " & m_AddressFieldRightTo & ""
            End If

            If pFl.Columns(m_AddressFieldRightTo).DataType Is System.Type.GetType("System.String") Then
                pSqlString = pSqlString & " AND '" & housenumber & "'  <= " & m_AddressFieldRightFrom & "))"
            Else
                pSqlString = pSqlString & " AND " & housenumber & "  <= " & m_AddressFieldRightFrom & "))"
            End If



            Dim addSql As String = GenerateSQLCombo(pFl, "St")
            If addSql <> "" Then
                pSqlString = pSqlString & " AND " & addSql
            End If


            'pSqlString = pSqlString & " AND (('" & housenumber & "' >= " & m_AddressFieldLeftFrom & " AND " & "'" & housenumber & "' <= " & m_AddressFieldLeftTo & ")"
            '  pSqlString = pSqlString & " OR ('" & housenumber & "' >= " & m_AddressFieldRightFrom & " AND " & "'" & housenumber & "' <= " & m_AddressFieldRightTo & "))"
            'Apply the query string
            pQFilt.WhereClause = pSqlString
            'Limit the return fields
            Dim pStr(5) As String
            pStr(0) = m_AddressFieldStreetName
            pStr(1) = m_AddressFieldLeftFrom
            pStr(2) = m_AddressFieldLeftTo
            pStr(3) = m_AddressFieldRightFrom
            pStr(4) = m_AddressFieldRightTo
            pStr(5) = pFl.GeometryColumnName
            'Get the features that match
            'pFl.GetDataReader(pQFilt, pStr)
            Dim pFDT As FeatureDataTable = pFl.GetDataTable(pQFilt, pStr)

            'If nothign was found, try agin with half the street name
            If pFDT Is Nothing Then
                'Find a street like the value entered
                pSqlString = "'" & street.Substring(0, CInt(street.Length / 2)) & "' LIKE " & m_AddressFieldStreetName
                'Check both address range fields
                pSqlString = pSqlString & " AND (('" & housenumber & "' >= " & m_AddressFieldLeftFrom & " AND " & "'" & housenumber & "' <= " & m_AddressFieldLeftTo & ")"
                pSqlString = pSqlString & " OR ('" & housenumber & "' >= " & m_AddressFieldRightFrom & " AND " & "'" & housenumber & "' <= " & m_AddressFieldRightTo & "))"
                pQFilt.WhereClause = pSqlString
                pFDT = pFl.GetDataTable(pQFilt, pStr)
            ElseIf pFDT.Rows.Count = 0 Then
                'Find a street like the value entered
                pSqlString = "'" & street.Substring(0, CInt(street.Length / 2)) & "' LIKE " & m_AddressFieldStreetName
                'Check both address range fields
                pSqlString = pSqlString & " AND (('" & housenumber & "' >= " & m_AddressFieldLeftFrom & " AND " & "'" & housenumber & "' <= " & m_AddressFieldLeftTo & ")"
                pSqlString = pSqlString & " OR ('" & housenumber & "' >= " & m_AddressFieldRightFrom & " AND " & "'" & housenumber & "' <= " & m_AddressFieldRightTo & "))"
                pQFilt.WhereClause = pSqlString
                pFDT = pFl.GetDataTable(pQFilt, pStr)
            End If

            'cleanup

            'pLay = Nothing
            pFl = Nothing
            pQFilt = Nothing

            'Return the found records
            Return pFDT
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
            Return Nothing


        End Try
    End Function
    Private Function ServerToMobileGeom(ByVal DistanceInFeet As Double) As Double
        'Convert feet to map units
        Return Esri.ArcGIS.Mobile.SpatialReferences.Unit.FromUnitToUnit(DistanceInFeet, Esri.ArcGIS.Mobile.SpatialReferences.Unit.Foot, m_Map.SpatialReference.Unit)

    End Function
    Private Function CalcEnvelope(ByVal g As Geometry, ByVal mapWidth As Double, ByVal mapHeight As Double) As Envelope
        'Used to adjsut extent based on map aspect ration
        Try


            'get the extent of the geometry
            Dim originalEnvelope As Envelope = g.Extent()
            If TypeOf g Is Polyline Then
                If originalEnvelope.Height > originalEnvelope.Width Then
                    originalEnvelope = New Envelope(originalEnvelope.Center, originalEnvelope.Height, originalEnvelope.Height)
                Else
                    originalEnvelope = New Envelope(originalEnvelope.Center, originalEnvelope.Width, originalEnvelope.Width)
                End If
            End If
            'Get the center
            Dim centralCoordinate As Coordinate = originalEnvelope.Center()
            Dim pEnvelope As Envelope
            'Determine map aspect ration
            Dim mapRatio As Double = mapWidth / mapHeight
            'Get feature ration

            Dim featureRatio As Double = originalEnvelope.Width / originalEnvelope.Height

            ' feature width exceeds expectation, need to resize its height
            If (featureRatio >= mapRatio) Then
                'Create a new envelope adjusted for aspect
                pEnvelope = New Envelope(centralCoordinate, originalEnvelope.Width, (originalEnvelope.Height * mapHeight / mapWidth))

            Else
                'Create a new envelope adjusted for aspect
                pEnvelope = New Envelope(centralCoordinate, originalEnvelope.Height * mapWidth / mapHeight, originalEnvelope.Height)


            End If
            'expand it some
            pEnvelope = pEnvelope.Resize(1.2)

            'cleanup
            originalEnvelope = Nothing
            centralCoordinate = Nothing

            'return it
            Return pEnvelope
        Catch ex As Exception
            'on error return original extent
            Return g.Extent()
        End Try
    End Function
    'Private Sub zoomTo(ByVal pGeo As Geometry)
    '    'Zoom to a feature
    '    Try
    '        If pGeo Is Nothing Then Return
    '        If pGeo.IsEmpty Then Return

    '        Dim pEnv As Esri.ArcGIS.Mobile.Geometries.Envelope
    '        'Determine type of feature
    '        If pGeo.GeometryType = Esri.ArcGIS.Mobile.Geometries.GeometryType.Point Then
    '            'If a point, center on it and create an envelope 50 around around it
    '            Dim pIntExtGeo As Double = ServerToMobileGeom(m_ZoomToVal)
    '            pEnv = New Envelope(0, 0, pIntExtGeo, pIntExtGeo)
    '            pEnv.CenterAt(CType(pGeo, Esri.ArcGIS.Mobile.Geometries.Point).Coordinate)
    '        Else

    '            'Calc the envelope based on the map aspect ration
    '            pEnv = CalcEnvelope(pGeo, m_Map.Width, m_Map.Height)
    '            pEnv.Resize(1.3)


    '        End If

    '        'Set the extent to the map
    '        m_Map.Extent = pEnv)
    '        'Cleanup
    '        pEnv = Nothing
    '    Catch ex As Exception
    '        Dim st As New StackTrace
    '        MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
    '        st = Nothing

    '    End Try
    'End Sub
    'Private Sub zoomTo(ByVal pCoord As Coordinate)
    '    'Zoom to a coordinate
    '    Try
    '        If pCoord Is Nothing Then Return
    '        If pCoord.IsEmpty Then Return

    '        'Construct a new envelope
    '        Dim pEnv As Esri.ArcGIS.Mobile.Geometries.Envelope
    '        'center on the coordinate and create an envelope 50 around around it
    '        Dim pIntExtGeo As Double = ServerToMobileGeom(m_ZoomToVal)
    '        pEnv = New Envelope(0, 0, pIntExtGeo, pIntExtGeo)
    '        pEnv.CenterAt(CType(New Esri.ArcGIS.Mobile.Geometries.Point(pCoord.X, pCoord.Y), Esri.ArcGIS.Mobile.Geometries.Point).Coordinate)
    '        'set map extent
    '        m_Map.Extent = pEnv)
    '        'clean up
    '        pEnv = Nothing
    '    Catch ex As Exception
    '        Dim st As New StackTrace
    '        MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
    '        st = Nothing

    '    End Try
    'End Sub
    'Private Sub flashGeo(ByVal pGeo As Geometry)
    '    Try


    '        'Flash the geometry, different for points and other geo
    '        If pGeo.GeometryType = GeometryType.Point Then
    '            m_Map.FlashGeometry(m_penLine, CType(m_brush, SolidBrush), 20, 100, 5, pGeo)
    '        Else

    '            m_Map.FlashGeometry(m_pen, m_brush, 20, 100, 5, pGeo)
    '        End If
    '    Catch ex As Exception
    '        Dim st As New StackTrace
    '        MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
    '        st = Nothing

    '    End Try
    'End Sub
    'Private Sub flashGeo(ByVal pGeo As Coordinate)
    '    Try

    '        m_Map.FlashGeometry(m_penLine, m_brush, 20, 100, 5, New Esri.ArcGIS.Mobile.Geometries.Point(pGeo))

    '    Catch ex As Exception
    '        Dim st As New StackTrace
    '        MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
    '        st = Nothing

    '    End Try
    'End Sub
    Private Function getStreetsIntersectionGeom(ByVal primaryStreet As String, ByVal secondaryStreet As String, ByRef IntersectionPoint As Esri.ArcGIS.Mobile.Geometries.Point) As Geometry
        'Zooms to the extent of two streets and their intersection

        'Exit if streets are not initilized
        If m_bAddress_GeocordInit = False Then Return Nothing

        Try
            Dim pClosestCoord As Coordinate = New Coordinate
            'Dim ClosestsegIdx As Integer
            ' Dim ClosestpartIdx As Integer
            Dim ClosestDist As Double = -1
            'Geometry of streets combines
            Dim pGeoStreets As Geometry = Nothing
            'New filter to find streets

            Dim pQFilt As QueryFilter = New QueryFilter

            Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = GlobalsFunctions.GetFeatureSource(m_AddressLayer, m_Map).FeatureSource
            Dim strAddSQLpreFilt As String = GenerateSQLCombo(pFL, "Int")
            If strAddSQLpreFilt <> "" Then
                pQFilt.WhereClause = m_AddressFieldStreetName & " = '" & primaryStreet & "'" & " AND " & strAddSQLpreFilt
            Else
                pQFilt.WhereClause = m_AddressFieldStreetName & " = '" & primaryStreet & "'"
            End If

            'Get the street layer
            'Dim pML As MobileCacheMapLayer
            ' pML = CType(m_Map.MapLayers(m_AddressLayer), MobileCacheMapLayer)
            'get the feature layer
            'Data reader to loop through all found primary streets
            Dim pFDR As FeatureDataReader
            'Get all primary streets
            pFDR = pFL.GetDataReader(pQFilt, New String(1) {m_AddressFieldStreetName, pFL.GeometryColumnName})
            'Data table to hold intersecting streets
            Dim pDT As DataTable = Nothing
            'Loop through all primary streets
            While pFDR.Read()
                'get the street geometry
                Dim pGeo As Geometry
                pGeo = CType(pFDR.Item(pFL.GeometryColumnName), Geometry)
                'Set up a filter to select all secondary streets by name and primary street geo
                pQFilt = New QueryFilter
                pQFilt.Geometry = pGeo
                'If strAddSQLpreFilt <> "" Then
                '    pQFilt.WhereClause = m_AddressFieldStreetName & " = '" & secondaryStreet & "'" & " AND " & strAddSQLpreFilt
                'Else
                pQFilt.WhereClause = m_AddressFieldStreetName & " = '" & secondaryStreet & "'"
                '  End If

                pQFilt.GeometricRelationship = Geometries.GeometricRelationshipType.Intersect
                'Get the streets that touch
                Dim pDTTemp As DataTable = pFL.GetDataTable(pQFilt, New String(1) {m_AddressFieldStreetName, pFL.GeometryColumnName})
                'If one found

                If pDTTemp.Rows.Count > 0 Then
                    'Apply the primary streets Geo
                    pGeoStreets = pGeo

                    'Loop through intersecting secondary streets and union the geo with the primary
                    For Each pDRIntersectGeo As FeatureDataRow In pDTTemp.Rows

                        For Each pPart As CoordinateCollection In pDRIntersectGeo.Geometry.Parts

                            If (pGeo.Intersects(New Polyline(pPart))) Then

                                For Each pPartCoord As Coordinate In pPart
                                    Dim pCoord As Coordinate = New Coordinate
                                    Dim segIdx As Integer
                                    Dim partIdx As Integer
                                    Dim dist As Double = -1
                                    pGeo.GetNearestCoordinate(pPartCoord, pCoord, partIdx, segIdx, dist)
                                    If ClosestDist = -1 Or dist < ClosestDist Then
                                        ClosestDist = dist
                                        pClosestCoord = pCoord
                                    End If

                                Next
                                ' pGeoStreets.Parts.Add(pPart)
                            End If
                            pGeoStreets.Parts.Add(pPart)
                            ' pGeoStreets.InsertCoordinates(pPart)

                        Next

                    Next
                    pDTTemp = Nothing
                    pGeo = Nothing
                    'Exit the loop
                    Exit While

                End If
                pGeo = Nothing
                pDTTemp = Nothing

            End While
            'Close the data reader
            pFDR.Close()
            'Cleanup
            pFDR = Nothing
            pFL = Nothing
            'pML = Nothing
            pQFilt = Nothing
            pDT = Nothing
            'Return the unioned Geometry
            If pClosestCoord IsNot Nothing Then
                If Not pClosestCoord.IsEmpty Then
                    IntersectionPoint = New Esri.ArcGIS.Mobile.Geometries.Point(pClosestCoord)
                End If

            End If

            Return pGeoStreets
        Catch ex As Exception
            Return Nothing

        End Try
    End Function
    Private Class SearchFeature
        'Class used to search the feature in a thread
        Public Map As Esri.ArcGIS.Mobile.WinForms.Map
        Public layerName As String
        Public Fields As IList
        Public SearchValue As String
        Public SearchEnv As Envelope
        Public SearchDefQ As String

        Public FindAllSimilar As Boolean = False
        'Event raised to pase the results back up
        Public Event SearchComplete(ByVal results As DataTable)
        Public Sub Dispose()
            Fields = Nothing
            Map = Nothing
            SearchEnv = Nothing

        End Sub
        Public Sub FindFeatures()
            'Create a new query filter
            Dim pQueryFilter As QueryFilter
            pQueryFilter = New QueryFilter
            pQueryFilter.WhereClause = ""
            If Map Is Nothing Then
                RaiseEvent SearchComplete(Nothing)
                Return
            End If
            If SearchEnv IsNot Nothing Then
                pQueryFilter.Geometry = SearchEnv
                pQueryFilter.GeometricRelationship = Geometries.GeometricRelationshipType.Intersect
            End If
            'Get the layer to search
            'Dim pLay As MobileCacheMapLayer = CType(Map.MapLayers(layerName), MobileCacheMapLayer)

            'If pLay Is Nothing Then
            '    RaiseEvent SearchComplete(Nothing)
            '    Return
            'End If


            'Get the FeatureSource
            Dim pFl As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = GlobalsFunctions.GetFeatureSource(layerName, Map).FeatureSource

            If pFl Is Nothing Then
                RaiseEvent SearchComplete(Nothing)
                Return
            End If

            'Get all the fields for the layer
            Dim pDcs As ReadOnlyColumnCollection = pFl.Columns

            Dim pDc As DataColumn
            Dim pStrWhere As String = ""
            
            Dim pStrFindAll As String
            Dim pStrFindOper As String
            'If finding like features, set up connecting operators
            If FindAllSimilar Then
                pStrFindAll = "%"
                pStrFindOper = " LIKE "
            Else
                pStrFindAll = ""
                pStrFindOper = " = "
            End If

            If SearchValue = "" Then

            Else


                'Loop through each field in the search field list
                For Each strFld As String In Fields
                    'Get the field
                    pDc = pDcs.Item(strFld)
                    'Make sure the field exist
                    If Not pDc Is Nothing Then
                        'If the where clause isnot empty, add a connecting word
                        If pStrWhere <> "" Then
                            pStrWhere = pStrWhere & " or "
                        End If
                        'Check for type of field 
                        If pDc.DataType Is System.Type.GetType("System.String") Then
                            pStrWhere = pStrWhere & strFld & pStrFindOper & "'" & pStrFindAll & SearchValue & pStrFindAll & "'"
                        ElseIf pDc.DataType Is System.Type.GetType("System.Integer") Or pDc.DataType Is System.Type.GetType("System.Int32") Or pDc.DataType Is System.Type.GetType("System.Int16") Then
                            'Make sure the value to search for is numeric
                            If IsNumeric(SearchValue) Then
                                'If it is a decimal numer, except integers field
                                If SearchValue.IndexOf(".") > 0 Then
                                    'Reset the where cause to remove the connecting word
                                    If pStrWhere.Length <= 4 Then
                                        pStrWhere = ""
                                    Else

                                        pStrWhere = pStrWhere.Substring(0, pStrWhere.Length - 4)
                                    End If


                                Else
                                    'CAST("FIELDNAME" as VARCHAR(20))
                                    'Add the field to the search list
                                    pStrWhere = pStrWhere & "CAST(" & strFld & " as VARCHAR(20))" & "" & pStrFindOper & "'" & pStrFindAll & SearchValue & pStrFindAll & "'"
                                End If
                            Else
                                'Not numeric, exlude it 
                                If pStrWhere.Length <= 4 Then
                                    pStrWhere = ""
                                Else
                                    pStrWhere = pStrWhere.Substring(0, pStrWhere.Length - 4)
                                End If
                            End If
                        ElseIf pDc.DataType Is System.Type.GetType("System.Boolean") Then
                            'if the search value is not true or false, exclude it 
                            If UCase(SearchValue) = "TRUE" Or UCase(SearchValue) = "FALSE" Then
                                pStrWhere = pStrWhere & strFld & pStrFindOper & pStrFindAll & SearchValue & pStrFindAll & ""

                            Else
                                'Remove the connecting words, exclude the field
                                If pStrWhere.Length <= 4 Then
                                    pStrWhere = ""
                                Else
                                    pStrWhere = pStrWhere.Substring(0, pStrWhere.Length - 4)
                                End If
                            End If

                        ElseIf pDc.DataType Is System.Type.GetType("System.Decimal") Or pDc.DataType Is System.Type.GetType("System.Double") Or pDc.DataType Is System.Type.GetType("System.Single") Then
                            'Determine if the search values is numeric
                            If IsNumeric(SearchValue) Then

                                pStrWhere = pStrWhere & "CAST(" & strFld & " as VARCHAR(20))" & "" & pStrFindOper & "'" & pStrFindAll & SearchValue & pStrFindAll & "'"
                            Else
                                'Remove the connecting words
                                If pStrWhere.Length <= 4 Then
                                    pStrWhere = ""
                                Else
                                    pStrWhere = pStrWhere.Substring(0, pStrWhere.Length - 4)
                                End If
                            End If
                        ElseIf pDc.DataType Is System.Type.GetType("System.DateTime") Then
                            'Check to see if search value is a date
                            If IsDate(SearchValue) Then
                                pStrWhere = pStrWhere & strFld & " = #" & Convert.ToDateTime(SearchValue) & "#"
                            Else
                                If pStrWhere.Length <= 4 Then
                                    pStrWhere = ""
                                Else
                                    pStrWhere = pStrWhere.Substring(0, pStrWhere.Length - 4)
                                End If
                            End If
                        End If
                    End If
                Next
            End If
            If SearchDefQ <> "" Then
                pQueryFilter.WhereClause = IIf(pQueryFilter.WhereClause = "", SearchDefQ, "(" & pQueryFilter.WhereClause & ") AND " & SearchDefQ)

            End If

            'Apply where clause to filter
            pQueryFilter.WhereClause = pStrWhere
            If m_SearchPoly IsNot Nothing Then
                pQueryFilter.Geometry = m_SearchPoly
                pQueryFilter.GeometricRelationship = Geometries.GeometricRelationshipType.Intersect
            End If

            Try
                'If the where clause is empty exit
                'If pQueryFilter.WhereClause = "" Then
                '    RaiseEvent SearchComplete(Nothing)
                '    Return
                'End If

                'Check to make sure features match our search
                Dim pCnt As Integer = pFl.GetFeatureCount(pQueryFilter)
                If pCnt = 0 Then
                    RaiseEvent SearchComplete(Nothing)
                    Exit Sub
                End If
                'Get the features that match the query
                Dim pFLDT As FeatureDataTable = pFl.GetDataTable(pQueryFilter)

                'Cleanup

                pQueryFilter = Nothing
                '  pLay = Nothing
                pFl = Nothing
                pDcs = Nothing
                pDc = Nothing

                pFLDT = GlobalsFunctions.AddDomainValueToDataTable(pFLDT)
                RaiseEvent SearchComplete(pFLDT)

            Catch ex As Exception
                RaiseEvent SearchComplete(Nothing)
                Return
            End Try
        End Sub

    End Class

    Private Sub VScrollBar1_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles VScrollBar1.Scroll
        Try

            If e.NewValue > dgResults.RowCount - 1 Then
                e.NewValue = dgResults.RowCount
            ElseIf e.NewValue = 0 Then
                dgResults.FirstDisplayedScrollingRowIndex = 0
            ElseIf e.NewValue = dgResults.RowCount - 1 Then
                dgResults.FirstDisplayedScrollingRowIndex = dgResults.RowCount - 1
                'ElseIf e.OldValue - e.NewValue = 1 Then
                '    dgResults.FirstDisplayedScrollingRowIndex = dgResults.FirstDisplayedScrollingRowIndex + 1
            Else
                dgResults.FirstDisplayedScrollingRowIndex = e.NewValue
                'ElseIf e.OldValue - e.NewValue > 1 Then
                '    dgResults.FirstDisplayedScrollingRowIndex = dgResults.FirstDisplayedScrollingRowIndex + (e.OldValue - e.NewValue)
                'ElseIf e.OldValue - e.NewValue < 1 Then
                '    dgResults.FirstDisplayedScrollingRowIndex = dgResults.FirstDisplayedScrollingRowIndex + (e.OldValue - e.NewValue)
            End If


        Catch ex As Exception

        End Try

    End Sub


    Private Sub HScrollBar1_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles HScrollBar1.Scroll
        Try
            If e.NewValue = 0 Then
                dgResults.FirstDisplayedScrollingColumnIndex = 0
            ElseIf e.NewValue - e.OldValue > 0 Then

                dgResults.FirstDisplayedScrollingColumnIndex = dgResults.FirstDisplayedScrollingColumnIndex + e.NewValue - e.OldValue

                Dim idxLstCol As Integer = getLastVisibleColumnIdx()
                If dgResults.Columns(idxLstCol).Displayed() Then
                    If dgResults.DisplayedColumnCount(False) + e.NewValue >= getVisibleColumns() Then
                        HScrollBar1.Maximum = e.NewValue + 1
                        'RemoveHandler HScrollBar1.Scroll, AddressOf HScrollBar1_Scroll
                        HScrollBar1.Value = e.NewValue - 1
                        'AddHandler HScrollBar1.Scroll, AddressOf HScrollBar1_Scroll

                    End If

                End If


            ElseIf e.NewValue - e.OldValue < 0 Then
                '         Dim idxLstCol As Integer = getLastVisibleColumnIdx()
                'If dgResults.Columns(idxLstCol).Displayed() Then
                '    If dgResults.DisplayedColumnCount(False) + e.OldValue = getVisibleColumns() Then

                '        dgResults.FirstDisplayedScrollingColumnIndex = dgResults.FirstDisplayedScrollingColumnIndex - 1
                '    Else
                '        dgResults.FirstDisplayedScrollingColumnIndex = e.NewValue
                '    End If

                'Else
                '    dgResults.FirstDisplayedScrollingColumnIndex = e.NewValue
                'End If
                If dgResults.FirstDisplayedScrollingColumnIndex = 0 Then
                    HScrollBar1.Value = 0
                    e.NewValue = 0
                Else
                    dgResults.FirstDisplayedScrollingColumnIndex = dgResults.FirstDisplayedScrollingColumnIndex + e.NewValue - e.OldValue
                    'If dgResults.FirstDisplayedScrollingColumnIndex = 0 Then
                    '    'HScrollBar1. = 0
                    '    HScrollBar1.Minimum = e.NewValue - 1

                    'End If
                End If


            End If


        Catch ex As Exception

        End Try

    End Sub
    Private Function getVisibleColumns() As Integer
        Dim i As Integer = 0
        For Each col As DataGridViewColumn In dgResults.Columns

            If col.Visible Then i = i + 1
        Next
        Return i
    End Function
    Private Function getLastVisibleColumnIdx() As Integer
        Dim idx As Integer = 0
        For i = 0 To dgResults.Columns.Count - 1
            Dim col As DataGridViewColumn = dgResults.Columns(i)

            If col.Visible Then idx = i
        Next
        Return idx
    End Function

#End Region



    Private Sub txtGeocodeValue_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtGeocodeValue.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.Handled = True
            e.SuppressKeyPress = True
            Call btnRunGC_Click(Nothing, Nothing)

        ElseIf e.KeyCode = Keys.Return Then
            e.Handled = True
            e.SuppressKeyPress = True
            Call btnRunGC_Click(Nothing, Nothing)

        End If

    End Sub
    Public Class MyTextBox
        Inherits TextBox
        Protected Overrides Sub OnKeyPress(ByVal e As KeyPressEventArgs)
            If e.KeyChar = ControlChars.Cr Then
                e.Handled = True
            Else
                MyBase.OnKeyPress(e)
            End If
        End Sub 'OnKeyPress 
    End Class

    Private Sub dgResults_CellMouseClick(sender As Object, e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles dgResults.CellMouseClick
        If e.RowIndex = -1 Then Return
        ZoomFlashRouteGC(recordClickType.zoom)

    End Sub

    Private Sub dgResults_Click(sender As Object, e As System.EventArgs) Handles dgResults.Click


    End Sub 'MyTextBox 

    Private Sub dgResults_SelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dgResults.SelectionChanged
        'Zoom and flash a value in the DG
        'ZoomFlashRouteGC(recordClickType.zoom)
    End Sub

    Private Sub lblAddressStreet2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub
    Private Sub lblAddressRange_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub btnRunXY_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRunXY.Click
        Try
            RemoveHandler dgResults.SelectionChanged, AddressOf dgResults_SelectionChanged

            Dim pGeo As Esri.ArcGIS.Mobile.Geometries.Geometry
            Dim xVal As Double
            Dim yVal As Double
            If txtY.Visible Then
                xVal = CDbl(txtX.Text)
                yVal = CDbl(txtY.Text)

            Else
                Dim strVals() As String = txtX.Text.Split(",")
                If strVals.Length <> 2 Then Return

                Double.TryParse(strVals(0).Trim(), xVal)
                Double.TryParse(strVals(1).Trim(), yVal)


            End If
            If chkLatLong.Checked = True Then
                pGeo = New Esri.ArcGIS.Mobile.Geometries.Point(m_Map.SpatialReference.FromWgs84(New Coordinate(xVal, yVal)))

            Else
                pGeo = New Esri.ArcGIS.Mobile.Geometries.Point(New Coordinate(xVal, yVal))
            End If

            GlobalsFunctions.zoomTo(pGeo, m_Map)
            If pGeo.GeometryType = GeometryType.Point Then
                GlobalsFunctions.flashGeo(pGeo, m_Map, m_penLine, CType(m_brush, SolidBrush))
            Else
                GlobalsFunctions.flashGeo(pGeo, m_Map, m_pen, m_brush)

            End If

        Catch ex As Exception
            'MsgBox("Error in the Run Click event" & vbCrLf & ex.Message)
            MsgBox(GlobalsFunctions.appConfig.SearchPanel.UIComponents.GoToXYErrorText)

        Finally
            AddHandler dgResults.SelectionChanged, AddressOf dgResults_SelectionChanged

        End Try
    End Sub

    Private Sub btnAddress_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddress.CheckedChanged
        If btnAddress.Checked = True Then
            btnAddress.BackgroundImage = Global.MobileControls.My.Resources.Resources.AddIntersectDown

        Else
            btnAddress.BackgroundImage = Global.MobileControls.My.Resources.Resources.AddIntersect


        End If
    End Sub

    Private Sub btnAddressPnt_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddressPnt.CheckedChanged
        If btnAddressPnt.Checked = True Then
            btnAddressPnt.BackgroundImage = Global.MobileControls.My.Resources.Resources.AddPntDown

        Else
            btnAddressPnt.BackgroundImage = Global.MobileControls.My.Resources.Resources.AddPoint


        End If
    End Sub


    Private Sub btnLookup_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLookup.CheckedChanged

        If btnLookup.Checked = True Then
            btnLookup.BackgroundImage = Global.MobileControls.My.Resources.Resources.LookupDown

        Else
            btnLookup.BackgroundImage = Global.MobileControls.My.Resources.Resources.Lookup


        End If
    End Sub

    Private Sub btnGeocode_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGeocode.CheckedChanged
        If btnGeocode.Checked = True Then
            btnGeocode.BackgroundImage = Global.MobileControls.My.Resources.Resources.GeocodeDown

        Else
            btnGeocode.BackgroundImage = Global.MobileControls.My.Resources.Resources.Geocode


        End If
    End Sub

    Private Sub cboStreetLayer1_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboStreetLayer1.SelectedValueChanged


    End Sub


    Private Sub cboStreetLayer1_TextUpdate(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboStreetLayer1.TextUpdate


    End Sub

    Private Sub gpBxAddPntControlsLayer_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles gpBxAddPntControlsLayer.Paint

    End Sub

    Private Sub cboDrillDownLayer_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboDrillDownLayer.SelectedIndexChanged


        CreateAddressPointControls(GlobalsFunctions.appConfig.SearchPanel.DrillDownSearches.DrillDownSearch(cboDrillDownLayer.SelectedIndex))

        LoadValuesDrillDownSearch(-1)
        resizeDrillDown()
    End Sub


    Private Sub pnlSearchOptions_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnlSearchOptions.Enter

    End Sub

    Private Sub chkLatLong_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkLatLong.CheckedChanged
        If chkLatLong.Checked Then
            lblX.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.LongText
            lblY.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.LatText

        Else
            lblX.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.XText
            lblY.Text = GlobalsFunctions.appConfig.SearchPanel.UIComponents.YText

        End If
    End Sub


    Private Sub cboSearchField_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboSearchField.SelectedIndexChanged
        Try
            'check the field to see if it is domain field
            ' Dim pLay As MobileCacheMapLayer = CType(m_Map.MapLayers(cboSearchLayer.Text), MobileCacheMapLayer)

            '  If pLay Is Nothing Then Return
            Dim pfl As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = GlobalsFunctions.GetFeatureSource(cboSearchLayer.Text, m_Map).FeatureSource

            'MsgBox("Fix Domain")
            Dim obj As Object
            'obj = pfl.6GetDomain(0, cboSearchField.Text)
            If TypeOf cboSearchField.SelectedItem Is SourceDataColumn Then
                'pfl.Columns((cboSearchField.SelectedItem)) IsNot Nothing Then
                obj = CType(cboSearchField.SelectedItem, SourceDataColumn).GetDomain(0) 'pfl.Columns(cboSearchField.Text).GetDomain(0)
            Else
                obj = Nothing

            End If
            If obj Is Nothing Then
                txtSearchValue.Visible = True
                cboSearchValue.Visible = False
            ElseIf TypeOf obj Is CodedValueDomain Then
                txtSearchValue.Visible = False
                cboSearchValue.Visible = True
                Dim pCV As CodedValueDomain
                pCV = CType(obj, CodedValueDomain)
                cboSearchValue.DataSource = pCV
                cboSearchValue.DisplayMember = "Value"
                cboSearchValue.ValueMember = "Code"
            End If

        Catch ex As Exception

        End Try
    End Sub





    Private Sub btnBuffer_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBuffer.CheckedChanged
        If btnBuffer.Checked = True Then
            btnBuffer.BackgroundImage = Global.MobileControls.My.Resources.Resources.CircleBufferRed
            '   m_LastM = m_Map.MapAction
            '  m_Map.MapAction = m_BuffMA
            'm_BuffMA.Active = True
        Else
            btnBuffer.BackgroundImage = Global.MobileControls.My.Resources.Resources.CircleBuffer
          
            'm_BuffMA.Active = False


        End If
    End Sub

    Private Sub m_BuffMA_GeoCreated(ByVal buffer As Esri.ArcGIS.Mobile.Geometries.Polygon) Handles m_BuffMA.GeoCreated
        m_SearchPoly = buffer
        If m_SearchPoly Is Nothing And (((txtSearchValue.Text = "" And txtSearchValue.Visible) Or txtSearchValue.Visible = False) Or ((cboSearchValue.SelectedIndex <= 0 And cboSearchValue.Visible) Or cboSearchValue.Visible = False)) Then

            btnSearchFind.Enabled = False
        Else

            btnSearchFind.Enabled = True
        End If
    End Sub

    Private Sub m_BuffMA_StatusChanged(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.WinForms.MapActionStatusChangedEventArgs) Handles m_BuffMA.StatusChanged
        If e.StatusId = 1002 Then 'active
        ElseIf e.StatusId = 1003 Then
            btnBuffer.Checked = False

        ElseIf e.StatusId = 1100 Then


        End If
    End Sub

    Private Sub btnBuffer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBuffer.Click
        If btnBuffer.Checked = True Then
            m_LastM = m_Map.MapAction
            m_Map.MapAction = m_BuffMA
            'm_BuffMA.Active = True
        Else


        m_Map.MapAction = m_LastM

        End If


        'btnBuffer.Checked = True

    End Sub
    'New Geometries.Envelope(m_ClickPoint, m_Dist * 2, m_Dist * 2).ToPolygon
    Private Sub cboBufferVal_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboBufferVal.SelectedIndexChanged
        If m_BuffMA IsNot Nothing Then
            If cboBufferVal.SelectedValue.ToString().ToUpper() = "{Empty}".ToUpper Then
                m_BuffMA.setExtent(Nothing, False)
                m_BuffMA.clearGeo()
                btnBuffer.Enabled = False
                m_SearchPoly = Nothing
                If m_Map.MapAction Is m_BuffMA Then
                    m_Map.MapAction = m_LastM
                End If

            ElseIf cboBufferVal.SelectedValue.ToString().ToUpper() = "{Extent}".ToUpper Then

                btnBuffer.Enabled = False
                m_BuffMA.clearGeo()
                If m_Map.MapAction Is m_BuffMA Then
                    m_Map.MapAction = m_LastM
                End If
                Dim pTmpEnv As Envelope = m_Map.Extent

                'pTmpEnv.ResizeX(0.7)
                'pTmpEnv.ResizeY(0.7)
                m_BuffMA.setExtent(pTmpEnv, True)


            Else
                If IsNumeric(cboBufferVal.SelectedValue.ToString) Then
                    m_BuffMA.setExtent(New Geometries.Envelope(New Coordinate(0, 0), CInt(cboBufferVal.SelectedValue.ToString) * 2, CInt(cboBufferVal.SelectedValue.ToString) * 2), False)
                    btnBuffer.Enabled = True
                Else
                    m_BuffMA.setExtent(Nothing, False)
                    m_BuffMA.clearGeo()
                    btnBuffer.Enabled = False
                    m_SearchPoly = Nothing
                    If m_Map.MapAction Is m_BuffMA Then
                        m_Map.MapAction = m_LastM
                    End If
                End If


            End If
        End If




    End Sub

    Private Sub btnBuffer_EnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBuffer.EnabledChanged
        If btnBuffer.Enabled Then
            btnBuffer.BackgroundImage = My.Resources.CircleBuffer

        Else
            btnBuffer.BackgroundImage = My.Resources.CircleGray

        End If
    End Sub

    Private Sub m_Map_ExtentChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_Map.ExtentChanged
        If m_BuffMA IsNot Nothing Then

            If cboBufferVal.SelectedValue.ToString().ToUpper() = "{Extent}".ToUpper Then
                m_BuffMA.setExtent(m_Map.Extent, True)
            End If
        End If

    End Sub


    Private Sub btnWaypointOnline_Click(sender As System.Object, e As System.EventArgs) Handles btnWaypointOnline.Click

        Dim gcResp As gcResponse = GlobalsFunctions.GetAddressOnline(txtbxOnlineGCAddress.Text & " " & GlobalsFunctions.appConfig.SearchPanel.AddressSearch.OnlineServices.Geocode.TextToAdd, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.OnlineServices.Geocode.GCUrl)
        If gcResp IsNot Nothing Then


            If gcResp.candidates IsNot Nothing Then
                If gcResp.candidates.Length > 0 Then
                    Dim pPnt As New Esri.ArcGIS.Mobile.Geometries.Point(m_Map.SpatialReference.FromWgs84(New Esri.ArcGIS.Mobile.Geometries.Coordinate(gcResp.candidates(0).location.x, gcResp.candidates(0).location.y)))


                    RaiseEvent Waypoint(pPnt, "")


                End If
            End If
        End If
    End Sub
    Private Sub btnOnlineGC_Click(sender As System.Object, e As System.EventArgs) Handles btnOnlineGC.Click

        Dim gcResp As gcResponse = GlobalsFunctions.GetAddressOnline(txtbxOnlineGCAddress.Text & " " & GlobalsFunctions.appConfig.SearchPanel.AddressSearch.OnlineServices.Geocode.TextToAdd, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.OnlineServices.Geocode.GCUrl)
        If gcResp IsNot Nothing Then


            If gcResp.candidates IsNot Nothing Then
                If gcResp.candidates.Length > 0 Then
                    Dim pPnt As New Esri.ArcGIS.Mobile.Geometries.Point(m_Map.SpatialReference.FromWgs84(New Esri.ArcGIS.Mobile.Geometries.Coordinate(gcResp.candidates(0).location.x, gcResp.candidates(0).location.y)))



                    GlobalsFunctions.zoomTo(pPnt, m_Map)
                    If pPnt.GeometryType = GeometryType.Point Then
                        GlobalsFunctions.flashGeo(pPnt, m_Map, m_penLine, CType(m_brush, SolidBrush))
                    Else
                        GlobalsFunctions.flashGeo(pPnt, m_Map, m_pen, m_brush)

                    End If

                End If
            End If
        End If
        'For Each o As candidates In gcResp.candidates

        '    MsgBox(o.score & " " & o.address)


        'Next
    End Sub
End Class
