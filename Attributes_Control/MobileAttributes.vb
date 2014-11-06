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



Imports System.Windows.Forms
Imports Esri.ArcGIS.Mobile
Imports Esri.ArcGIS.Mobile.FeatureCaching
Imports Esri.ArcGIS.Mobile.Geometries
Imports Esri.ArcGIS.Mobile.SpatialReferences
Imports System.Xml
Imports System.Drawing
Imports System.ComponentModel
Imports System.Net
Imports Esri.ArcGISTemplates
Public Class MobileAttributes
    Private m_UseEdit As Boolean = False
    Private m_HasFilter As Boolean = False
    Private WithEvents m_attDis As AttributeDisplay

    'Public WithEvents m_EditPanel As EditControl
    Public Event RouteTo(ByVal location As Geometry, ByVal LocationName As String)
    Public Event Waypoint(ByVal location As Geometry, ByVal LocationName As String)

    Public Event HyperClick(ByVal PathToFile As String)
    Public Event LocationIdentified(ByVal fdr As FeatureDataRow)
    Public Event CheckGPS()

    Private m_GPSVal As GPSLocationDetails = Nothing

    Private m_BoolLenFlds As Boolean = True
    Private m_LengthFormat As String = "{0:0.00}"
    Private m_LengthUnit As Unit

    Private WithEvents m_Map As Esri.ArcGIS.Mobile.WinForms.Map
    'Private m_cont As Control
    Private m_CurLayer As String
    Private m_LayGroup As MobileAttributes.LayersWithGroup
    'used to flash the features
    Private m_penLine As Pen = New Pen(Color.Purple, 10)
    Private m_pen As Pen = New Pen(Color.Purple, 10)
    Private m_brush As SolidBrush = New SolidBrush(Color.Purple)
    Private m_ShowDomain As Boolean = True

    'Create a universal font to use on all controls
    Private m_Fnt As Font '= New System.Drawing.Font("Microsoft Sans Serif", 12.0F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)
    Private m_FntLbl As Font ' = New System.Drawing.Font("Microsoft Sans Serif", 11.0F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)

    Private m_FntHyper As Font ' = New System.Drawing.Font("Microsoft Sans Serif", 12.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0)

    Private m_FColor As Color = Color.Black
    Private m_BColor As Color = Color.White
    'used to buffer a point for selection
    ' Private m_BufferValueforPoint As Integer
    'value used to divide the screen size up and used to determine buffer value
    Public m_BufferDivideValue As Double = 15
    Private m_BaseColor As Color

    Private m_Combo As ComboBox

    Private m_CurrentRow As FeatureDataRow
    Private m_RouteToOption As Boolean
    Private m_wayPoint As Boolean
    Private m_LastScale As Double = 0
    Private m_showCombo As Boolean = False

    '__Private m_Distance As Integer = 5
#Region "Public Functions"
    Public Sub showEditButton()
        m_attDis.showEditButton()

    End Sub
    Public Sub hideEditButton()
        m_attDis.hideEditButton()
    End Sub

    Private m_GPSStatus As Boolean

    Public Property GPSStatus As Boolean
        Get
            Return m_GPSStatus
        End Get
        Set(ByVal value As Boolean)

            m_GPSStatus = value
            If m_attDis IsNot Nothing Then
                m_attDis.GPSStatus = value

            End If
        End Set
    End Property
    Public Property toggleCombo As Boolean
        Get
            Return m_showCombo
        End Get
        Set(ByVal value As Boolean)



            m_showCombo = value
            If m_Combo.Items.Count < 2 Then
                gpBxLayer.Visible = False

            Else

                gpBxLayer.Visible = value
            End If

        End Set
    End Property
    Private m_specialCase As String = ""

    Public Property specialCase As String
        Get
            Return m_specialCase
        End Get
        Set(ByVal value As String)



            m_specialCase = value



        End Set
    End Property

    Public Sub disposes()


        If m_penLine IsNot Nothing Then

            ' m_penLine.Dispose()
        End If
        m_penLine = Nothing
        If m_pen IsNot Nothing Then
            '      m_pen.Dispose()
        End If
        m_pen = Nothing

        If m_brush IsNot Nothing Then
            m_brush.Dispose()
        End If
        m_brush = Nothing

        If m_Fnt IsNot Nothing Then
            m_Fnt.Dispose()
        End If
        m_Fnt = Nothing
        If m_FntLbl IsNot Nothing Then
            m_FntLbl.Dispose()
        End If
        m_FntLbl = Nothing

        If m_FntHyper IsNot Nothing Then
            m_FntHyper.Dispose()
        End If
        m_FntHyper = Nothing


        m_BaseColor = Nothing


        If m_Combo IsNot Nothing Then
            m_Combo.Dispose()
        End If
        m_Combo = Nothing



        If m_CurrentRow IsNot Nothing Then
            m_CurrentRow.Delete()
        End If
        m_CurrentRow = Nothing
        If m_Map IsNot Nothing Then
            m_Map.Dispose()
        End If
        m_Map = Nothing
        m_FColor = Nothing

        m_BColor = Nothing

    End Sub
    Public Sub New(ByVal map As Esri.ArcGIS.Mobile.WinForms.Map, Optional ByVal LayerName As String = "", Optional ByVal RouteToOption As Boolean = False, Optional ByVal fntSize As Single = 12.0F, Optional ByVal BoolLenFlds As Boolean = False, Optional ByVal LengthFormat As String = "{0:0.00}", Optional ByVal LengthUnit As String = "MAP")

        InitializeComponent()
        m_Map = map

        m_attDis = New AttributeDisplay(map, LayerName, RouteToOption, fntSize, BoolLenFlds, LengthFormat, LengthUnit)
        m_attDis.Dock = DockStyle.Fill
        gpBoxAttLay.Controls.Add(m_attDis)


        m_BoolLenFlds = BoolLenFlds
        m_LengthFormat = LengthFormat


        m_Fnt = New System.Drawing.Font("Microsoft Sans Serif", fntSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)
        m_FntLbl = New System.Drawing.Font("Microsoft Sans Serif", fntSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)

        m_FntHyper = New System.Drawing.Font("Microsoft Sans Serif", fntSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0)

        m_RouteToOption = RouteToOption

        'Set the Map

        Select Case UCase(LengthUnit)
            Case "FOOT"
                m_LengthUnit = Unit.Foot
            Case "FEET"
                m_LengthUnit = Unit.Foot
            Case "METER"
                m_LengthUnit = Unit.Meter
            Case "KILOMETER"
                m_LengthUnit = Unit.Kilometer
            Case "MILE"
                m_LengthUnit = Unit.Mile
            Case "INCH"
                m_LengthUnit = Unit.Inch
            Case "YARD"
                m_LengthUnit = Unit.Yard
            Case "NAUTICALMILE"
                m_LengthUnit = Unit.NauticalMile
            Case "MAP"
                m_LengthUnit = m_Map.SpatialReference.Unit
            Case Else
                m_LengthUnit = m_Map.SpatialReference.Unit
        End Select
        'Clear them

        'Set the layer of intereset
        If LayerName <> "" Then
            If m_LayGroup IsNot Nothing Then
                For Each lay As String In m_LayGroup.LayersInGroup
                    If lay = LayerName Then
                        m_CurLayer = lay
                    End If

                Next
            End If
        End If


        Try


            AddLayerBox(LayerName)
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try


    End Sub
    Public Sub currentRecord(ByVal dr As FeatureDataRow)
        m_attDis.CurrentRow = dr

    End Sub
    Public Function IdentifyLocation(ByVal Location As Esri.ArcGIS.Mobile.Geometries.Geometry) As Boolean
        'used to identify features
        Try
            Dim pDT As FeatureDataTable = selectFeature(Location)
            Dim idx As Integer = -1
            If Not pDT Is Nothing Then
                If pDT.Rows.Count > 0 Then
                    Dim positionA As Coordinate
                    Dim positionB As Coordinate
                    Dim dist As Double = 9999999
                    positionA = Location.Extent.Center


                    For i As Integer = 0 To pDT.Rows.Count - 1
                        Dim pFDR As FeatureDataRow = CType(pDT.Rows(i), FeatureDataRow)

                        positionB = pFDR.Geometry.Extent.Center

                        If positionA.Distance(positionB) < dist Then
                            idx = i
                            dist = positionA.Distance(positionB)
                        End If


                    Next
                    currentRecord(CType(pDT.Rows(idx), FeatureDataRow))

                    RaiseEvent LocationIdentified(CType(pDT.Rows(idx), FeatureDataRow))
                  

                    Return True
                    'double ddist = map.SpatialReference.FromGeometry(dist); 


                End If
            End If
            Return False
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try

    End Function
    Public Function IdentifyLocation(ByVal Location As Esri.ArcGIS.Mobile.Geometries.Coordinate) As Boolean
        '   Dim pGeo As IGeometry = New ESRI.ArcGIS.Mobile.Geometries.Point(Location)
        Return IdentifyLocation(New Esri.ArcGIS.Mobile.Geometries.Point(Location))


    End Function
    Public Function selectFeature(ByVal Location As Esri.ArcGIS.Mobile.Geometries.Coordinate) As FeatureDataTable
        Return selectFeature(New Esri.ArcGIS.Mobile.Geometries.Point(Location))


    End Function
    Public Function selectFeature(ByVal Location As Esri.ArcGIS.Mobile.Geometries.Geometry) As FeatureDataTable
        Try
            Dim pDT As DataTable

            'get the feature layer to ID
            If m_Combo.Text = "" Then
                pDT = spatialQFeature(GlobalsFunctions.appConfig.IDPanel.UIComponents.TopMostIDText, Location)
                Return pDT
            ElseIf m_Combo.Text <> GlobalsFunctions.appConfig.IDPanel.UIComponents.TopMostIDText Then
                pDT = spatialQFeature(m_Combo.Text, Location)
                Return pDT

            Else

                For Each strLay As String In m_Combo.DataSource
                    pDT = spatialQFeature(strLay, Location)
                    If pDT IsNot Nothing Then
                        Return pDT
                    End If
                Next
            End If
            Return Nothing

        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Private Function spatialQFeature(ByVal layername As String, ByVal Location As Esri.ArcGIS.Mobile.Geometries.Geometry) As FeatureDataTable
        Dim pFS As FeatureSourceWithDef = Nothing
        Dim pLayDef As MobileCacheMapLayerDefinition


        Dim pEnv As Envelope
        Try



            pFS = GlobalsFunctions.GetFeatureSource(layername, m_Map) '(m_Map.MapLayers(layername), MobileCacheMapLayer)
            If pFS IsNot Nothing Then
                If pFS.FeatureSource IsNot Nothing Then


                    pLayDef = GlobalsFunctions.GetLayerDefinition(m_Map, pFS)

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
                    Dim pQf As QueryFilter = New QueryFilter(pEnv, Geometries.GeometricRelationshipType.Intersect)
                    'Build an array to limit returned fields
                    Dim pStr(pFS.FeatureSource.Columns.Count) As String
                    'Add all fields for the result
                    For i As Integer = 0 To pFS.FeatureSource.Columns.Count - 1
                        pStr(i) = pFS.FeatureSource.Columns(i).ColumnName
                    Next
                    If pFS.FeatureSource.GetFeatureCount(pQf) > 0 Then


                        LayerChanged(layername)
                        Try
                            'Return the resulting rows

                            Return pFS.FeatureSource.GetDataTable(pQf, pStr)
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

    Public Sub reload()

        m_Combo.SelectedItem = m_CurLayer
        If m_CurLayer Is Nothing Then
            m_Combo.Text = GlobalsFunctions.appConfig.IDPanel.UIComponents.TopMostIDText

        End If
    End Sub
    'Public Sub ShowHideLayerBox(ByVal show As Boolean)
    '    gpBxLayer.Visible = show

    'End Sub
#End Region
#Region "Properties"
    Public Property GPSLocation() As GPSLocationDetails
        Get
            Return m_GPSVal
        End Get
        Set(ByVal value As GPSLocationDetails)
            m_GPSVal = value
        End Set
    End Property
    Public Class LayersWithGroup
        Public Sub New()
            m_Layers = New List(Of String)
            m_GroupName = ""

        End Sub
        Public Sub New(ByVal GroupName As String, ByVal LayersInGroup As List(Of String))
            m_Layers = LayersInGroup
            m_GroupName = GroupName

        End Sub
        Public Sub New(ByVal GroupName As String, ByVal Layer As String)
            m_Layers = New List(Of String)
            m_Layers.Add(Layer)
            m_GroupName = GroupName

        End Sub
        'Public Sub New(ByVal GroupName As String, ByVal LayersInGroup As String, ByVal splitChar As Char)
        '    m_Layers = New List(Of String)
        '    Dim stLays() As String = LayersInGroup.Split(splitChar)
        '    For Each strLay As String In stLays
        '        m_Layers.Add(strLay)

        '    Next
        '    If GroupName = "" Then

        '        m_GroupName = m_Layers(0)

        '    Else

        '        m_GroupName = GroupName
        '    End If

        'End Sub
        Public Sub AddLayer(ByVal LayerName As String)
            m_Layers.Add(LayerName)

        End Sub
        Private m_GroupName As String
        Public Property GroupName() As String
            Get
                Return m_GroupName
            End Get
            Set(ByVal value As String)
                m_GroupName = value
            End Set
        End Property

        Private m_Layers As List(Of String)
        Public Property LayersInGroup() As List(Of String)
            Get
                Return m_Layers
            End Get
            Set(ByVal value As List(Of String))
                m_Layers = value
            End Set
        End Property


    End Class

    Public WriteOnly Property DisplayFont() As Font
        Set(ByVal value As Font)
            m_Fnt = value
        End Set
    End Property
    Public WriteOnly Property ShowDomainDescriptions() As Boolean
        Set(ByVal value As Boolean)
            m_ShowDomain = value
        End Set
    End Property
    Public Property mapControl() As Esri.ArcGIS.Mobile.WinForms.Map
        Get
            Return m_Map
        End Get
        Set(ByVal value As Esri.ArcGIS.Mobile.WinForms.Map)
            m_Map = value
        End Set
    End Property
    Public Property ActiveGroup() As MobileAttributes.LayersWithGroup
        Get
            Return m_LayGroup
        End Get
        Set(ByVal value As MobileAttributes.LayersWithGroup)
            m_LayGroup = value
            Dim lst As List(Of String) = getLayerList()

            m_Combo.DataSource = lst
            If lst.Count > 0 Then
                m_Combo.Text = lst(0)
            End If

            m_Combo.Width = gpBxLayer.Width - 10

            If m_Combo.Items.Count < 2 Then
                gpBxLayer.Visible = False
            Else

                gpBxLayer.Visible = m_showCombo
            End If

            'AddControls()
            m_attDis.CurrentLayer = m_Combo.Text

            m_CurrentRow = Nothing

        End Set
    End Property
    Public Property ActiveLayer() As String
        Get
            Return m_Combo.Text
        End Get
        Set(ByVal value As String)
            m_Combo.Text = value
            m_attDis.CurrentLayer = value

            'AddControls()
            m_CurrentRow = Nothing

        End Set
    End Property
#End Region
#Region "Private Functions"
    Private Sub AddLayerBox(Optional ByVal LayerName As String = "")



        m_Combo = New ComboBox
        m_Combo.Font = m_Fnt
        m_Combo.Dock = DockStyle.Fill
        m_Combo.DropDownStyle = ComboBoxStyle.DropDownList
        m_Combo.DataSource = getLayerList()
        m_Combo.Width = gpBxLayer.Width - 10
        m_Combo.Text = GlobalsFunctions.appConfig.IDPanel.UIComponents.TopMostIDText

        gpBxLayer.Controls.Add(m_Combo)




        If LayerName <> "" Then

            m_Combo.SelectedItem = LayerName
            m_CurLayer = LayerName
        Else
            '  m_Combo.SelectedIndex = 0
            '   m_LayName = m_Combo.SelectedValue
        End If

        AddHandler m_Combo.SelectedIndexChanged, AddressOf LayerChange_SelectedIndexChanged

    End Sub
    Private Function getLayerList() As List(Of String)

        Try
            If m_Map Is Nothing Then Return Nothing
            'Get the mobile service that is associated with the map
            Dim pMblService As MobileCache = Nothing

            If m_LayGroup Is Nothing Then Return Nothing


            'Current map layer
            ' Dim pMapLayer As Esri.ArcGIS.Mobile.MapLayer
            Dim pList As List(Of String) = New List(Of String)

            Dim pFSwD As FeatureSourceWithDef

            'Loops through current map layers collection
            For Each strLay As String In m_LayGroup.LayersInGroup
                If strLay = GlobalsFunctions.appConfig.IDPanel.UIComponents.TopMostIDText Then
                    ' pList.Add("<Top Most Layer>")
                    Continue For

                End If
                pFSwD = GlobalsFunctions.GetFeatureSource(strLay, m_Map) 'm_Map.MapLayers(strLay)
                If pFSwD.FeatureSource IsNot Nothing Then




                    Dim pFSInfo As MobileCacheMapLayerDefinition = CType(m_Map.MapLayers(pFSwD.MapLayerIndex), MobileCacheMapLayer).LayerDefinitions(pFSwD.LayerIndex)

                    If pFSInfo.MinScale = 0 And pFSInfo.MaxScale = 0 And pFSInfo.Visibility = True Then
                        pList.Add(pFSInfo.Name)
                    ElseIf pFSInfo.MinScale = 0 And m_Map.Scale > pFSInfo.MaxScale And pFSInfo.Visibility = True Then
                        pList.Add(pFSInfo.Name)
                    ElseIf m_Map.Scale < pFSInfo.MinScale And pFSInfo.MaxScale = 0 And pFSInfo.Visibility = True Then
                        pList.Add(pFSInfo.Name)
                    ElseIf m_Map.Scale < pFSInfo.MinScale And m_Map.Scale > pFSInfo.MaxScale And pFSInfo.Visibility = True Then
                        pList.Add(pFSInfo.Name)
                    End If

                    pFSInfo = Nothing


                End If
                pFSwD = Nothing


            Next




            If pList.Count > 1 Then
                If Not pList.Contains(GlobalsFunctions.appConfig.IDPanel.UIComponents.TopMostIDText) Then
                    pList.Insert(0, GlobalsFunctions.appConfig.IDPanel.UIComponents.TopMostIDText)

                End If
            End If

            Return pList

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
            Return Nothing

        End Try
    End Function
    'Private Function getListOfLayers() As List(Of String)
    '    Try
    '        If m_LayGroup IsNot Nothing Then
    '            If m_LayGroup.LayersInGroup.Count > 1 Then
    '                If Not m_LayGroup.LayersInGroup.Contains("<Top Most Layer>") Then
    '                    m_LayGroup.LayersInGroup.Insert(0, "<Top Most Layer>")

    '                End If

    '                Return m_LayGroup.LayersInGroup
    '            Else
    '                Return m_LayGroup.LayersInGroup

    '            End If
    '            Return m_LayGroup.LayersInGroup
    '        Else
    '            Return Nothing

    '        End If



    '    Catch ex As Exception
    '        MsgBox("Error getting layer list in att panel" & vbCrLf & ex.Message)
    '        Return Nothing

    '    End Try
    'End Function
    'Private Function getListOfLayers() As List(Of String)
    '    Try
    '        If m_Map Is Nothing Then Return Nothing
    '        'Get the mobile service that is associated with the map
    '        Dim pMblService As MobileCache = Nothing

    '        For Each pDS As DataSource In m_Map.DataSources
    '            If TypeOf pDS Is MobileCache Then
    '                pMblService = CType(pDS, MobileCache)

    '                Exit For
    '            End If
    '        Next
    '        If m_Map.DataSources.Count > 1 Then
    '            MsgBox("Fix Att Panel")
    '        End If

    '        If pMblService Is Nothing Then Return Nothing
    '        If Not m_Map.IsValid Or Not pMblService.IsOpen Then Return Nothing

    '        ' Total number of MapLayers in the current map
    '        Dim playersCount As Integer = m_Map.MapLayers.Count


    '        'Current map layer
    '        Dim pMapLayer as Esri.ArcGIS.Mobile.MapLayer

    '        'map layer's name
    '        Dim pMapLayerName As String = ""
    '        'Create a new list to hold layer names
    '        Dim pList As List(Of String) = New List(Of String)

    '        'Loops through current map layers collection
    '        For i As Integer = 0 To playersCount - 1

    '            'Gets the map layer for that index
    '            pMapLayer = m_Map.MapLayers(i)
    '            pMapLayerName = pMapLayer.Name


    '            If TypeOf pMapLayer Is MobileCacheMapLayer Then

    '                'Current map layer
    '                Dim pMobileCacheLayer As MobileCacheMapLayer

    '                pMobileCacheLayer = TryCast(pMapLayer, MobileCacheMapLayer)
    '                If TypeOf pMobileCacheLayer.Layer is Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource Then



    '                    'Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = CType(pMobileCacheLayer.Layer, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)

    '                    pList.Add(pMobileCacheLayer.Name)

    '                Else

    '                End If
    '            ElseIf TypeOf pMapLayer Is ESRI.ArcGIS.Mobile.DataProducts.RasterData.TileCacheMapLayer Then

    '            ElseIf TypeOf pMapLayer Is ESRI.ArcGIS.Mobile.DataProducts.StreetMapData.StreetMapLayer Then

    '            ElseIf TypeOf pMapLayer Is ESRI.ArcGIS.Mobile.WebServices.ArcGISServices.MapServices.TileServiceMapLayer Then

    '            Else

    '                MsgBox("Unsupported Layer")

    '            End If





    '        Next i
    '        pMapLayer = Nothing
    '        '  pMblService = Nothing
    '        Return pList


    '    Catch ex As Exception
    '        MsgBox("Error getting layer list in att panel" & vbCrLf & ex.Message)
    '    End Try
    'End Function


#End Region
#Region "Events"

    Private Sub LayerChange_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

        LayerChanged(m_Combo.Text)
    End Sub
    Private Sub LayerChanged(ByVal layerName As String)
        If m_CurLayer = layerName Then Return


        'If the layer changes, update the global layuer name
        m_CurLayer = layerName
        'Set it to the selected item
        If m_Combo.Text <> GlobalsFunctions.appConfig.IDPanel.UIComponents.TopMostIDText Then
            m_Combo.SelectedItem = m_CurLayer

            m_attDis.CurrentLayer = layerName
            m_attDis.toggleLayerLabel = True
        Else
            m_attDis.toggleLayerLabel = True
            '    m_SelectedLayer.Text = layerName
        End If



    End Sub


    Private Sub m_UCAtt_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        'handles the resize of the control
        If m_Combo IsNot Nothing Then
            m_Combo.Left = 5

            m_Combo.Width = gpBxLayer.Width - 10
            m_Combo.Top = CInt((gpBxLayer.Height / 2) - (m_Combo.Height / 2))
        End If


    End Sub


#End Region
    Public Sub resetIDLayers()
        Try




            Dim lastLay As String = m_Combo.Text
            Dim lastSelectedLay As String = m_attDis.CurrentLayer
            Dim currentRow As FeatureDataRow = m_attDis.CurrentRow
            
            Dim layList As List(Of String) = getLayerList()

            If layList Is Nothing Then

                gpBxLayer.Visible = False
                If m_attDis.m_TabControl IsNot Nothing Then
                    m_attDis.m_TabControl.TabPages.Clear()
                    m_attDis.m_TabControl.Visible = False

                End If


                'CType(pnlAttributes.Controls(0), TabControl).TabPages.Clear()

                m_attDis.CurrentLayer = ""

            ElseIf layList.Count = 0 Then

                gpBxLayer.Visible = False



                m_attDis.CurrentLayer = ""


            Else
                If lastLay.Contains(lastLay) Then
                    m_Combo.DataSource = layList
                    m_Combo.Text = lastLay
                Else
                    m_Combo.DataSource = layList
                End If


                If m_Combo.Items.Count < 2 Then
                    gpBxLayer.Visible = False


                    If m_attDis.CurrentLayer <> layList(0) Then

                        m_attDis.CurrentLayer = ""
                        If m_attDis.m_TabControl IsNot Nothing Then

                            m_attDis.m_TabControl.TabPages.Clear()
                            m_attDis.m_TabControl.Visible = False
                        End If

                    End If


                Else
                    gpBxLayer.Visible = m_showCombo
                    If currentRow IsNot Nothing Then
                        If layList.Contains(currentRow.FeatureSource.Name) Then
                        Else
                            If Not layList.Contains(lastSelectedLay) Then

                                m_attDis.CurrentLayer = ""
                                If m_attDis.m_TabControl IsNot Nothing Then

                                    m_attDis.m_TabControl.TabPages.Clear()
                                    m_attDis.m_TabControl.Visible = False
                                End If

                            End If
                        End If

                    End If
                    'gpBxLayer.Visible = True
                    'If Not layList.Contains(lastSelectedLay) Then

                    '    m_attDis.CurrentLayer = ""
                    '    If m_attDis.m_TabControl IsNot Nothing Then

                    '        m_attDis.m_TabControl.TabPages.Clear()
                    '        m_attDis.m_TabControl.Visible = False
                    '    End If

                    'End If
                    'm_attDis.CurrentRow = currentRow

                End If

            End If

            m_Combo.Width = gpBxLayer.Width - 10
            resizeLabels()



        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
    Public Sub resizeLabels()
        'Dim g As Graphics
        'Dim s As SizeF

        'g = m_SelectedLayer.CreateGraphics
        's = g.MeasureString(m_SelectedLayer.Text, m_SelectedLayer.Font)

        ''If m_SelectedLayer.Parent.Width < s.Width Then
        ''    m_SelectedLayer.Parent.Parent.Width = s.Width + 25
        ''End If
        'm_SelectedLayer.Width = s.Width + 10
        'm_SelectedLayer.Parent.Refresh
        'm_SelectedLayer.Refresh()

        'g.Dispose()
        'g = Nothing
        's = Nothing
    End Sub
    Private Sub m_Map_ExtentChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_Map.ExtentChanged
        If m_LastScale = m_Map.Scale Then Return
        Dim lst As List(Of String) = getLayerList()
        Dim lastLay As String = m_Combo.Text

        m_Combo.DataSource = lst
        If Not lst Is Nothing Then
            If lst.Contains(lastLay) Then
                RemoveHandler m_Combo.SelectedIndexChanged, AddressOf LayerChange_SelectedIndexChanged
                m_Combo.Text = lastLay
                AddHandler m_Combo.SelectedIndexChanged, AddressOf LayerChange_SelectedIndexChanged

            ElseIf lst.Count > 0 Then
                m_Combo.Text = lst(0)
            End If
        End If

        m_Combo.Width = gpBxLayer.Width - 10

        If m_Combo.Items.Count < 2 Then
            gpBxLayer.Visible = False
        Else

            gpBxLayer.Visible = m_showCombo
        End If

    End Sub

    Private Sub m_attDis_CheckGPS() Handles m_attDis.CheckGPS
        RaiseEvent CheckGPS()

    End Sub

    Private Sub m_attDis_HyperClick(ByVal PathToFile As String) Handles m_attDis.HyperClick
        RaiseEvent HyperClick(PathToFile)

    End Sub

    Private Sub m_attDis_LocationIdentified(ByVal fdr As Esri.ArcGIS.Mobile.FeatureCaching.FeatureDataRow) Handles m_attDis.LocationIdentified
        RaiseEvent LocationIdentified(fdr)

    End Sub

    Private Sub m_attDis_RouteTo(ByVal location As Esri.ArcGIS.Mobile.Geometries.Geometry, ByVal LocationName As String) Handles m_attDis.RouteTo
        RaiseEvent RouteTo(location, LocationName)



    End Sub
    Public Event showEditor()

    Private Sub m_attDis_showEditor() Handles m_attDis.showEditor
        RaiseEvent showEditor()

    End Sub
    Private Sub m_attDis_Waypoint(ByVal location As Esri.ArcGIS.Mobile.Geometries.Geometry, ByVal LocationName As String) Handles m_attDis.Waypoint
        RaiseEvent Waypoint(location, LocationName)



    End Sub
End Class
