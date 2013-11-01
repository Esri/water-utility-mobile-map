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

Public Class AttributeDisplay
    Private m_AttMan As AttachmentManager



    Private m_HasFilter As Boolean = False

    ' Public WithEvents m_EditPanel As EditControl
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
    Public m_TabControl As TabControl

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

    Private m_WayPoint As Boolean
    Private m_CurrentRow As FeatureDataRow
    Private m_RouteToOption As Boolean
    Private m_LastScale As Double = 0
    Private m_FS As FeatureSource
    '__Private m_Distance As Integer = 5
    Private m_Mode As String = "ID"
#Region "Public Functions"
    Public Sub showEditButton()
        btnEdit.Visible = True
        relocateButtons()


    End Sub
    Public Sub hideEditButton()
        btnEdit.Visible = False

        relocateButtons()
    End Sub

    Private m_GPSStatus As String = "Off"

    Public Property GPSStatus
        Get
            Return m_GPSStatus
        End Get
        Set(ByVal value)
            m_GPSStatus = value
            If m_GPSStatus = "On" Then
                btnGPSLoc.Enabled = True
                btnWaypoint.Enabled = True
            Else
                btnGPSLoc.Enabled = False
                btnWaypoint.Enabled = False
            End If
        End Set
    End Property
    'Private m_specialCase As String = ""

    'Public Property specialCase As String
    '    Get
    '        Return m_specialCase
    '    End Get
    '    Set(ByVal value As String)



    '        m_specialCase = value



    '    End Set
    'End Property

    Public Sub disposes()


        If m_TabControl IsNot Nothing Then
            m_TabControl.Dispose()
        End If
        m_TabControl = Nothing


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
    Public Sub New(ByVal map As Esri.ArcGIS.Mobile.WinForms.Map, Optional ByVal LayerName As String = "", _
                   Optional ByVal RouteToOption As Boolean = False, Optional ByVal fntSize As Single = 12.0F, _
                   Optional ByVal BoolLenFlds As Boolean = False, Optional ByVal LengthFormat As String = "{0:0.00}", _
                   Optional ByVal LengthUnit As String = "MAP", Optional ByVal showGPSButton As Boolean = True, Optional ByVal showLayerLabel As Boolean = True)

        InitializeComponent()
        lblCurrentLayer.Dock = DockStyle.Fill
        If m_GPSStatus = "On" Then
            btnGPSLoc.Enabled = True
        Else
            btnGPSLoc.Enabled = False
        End If
        lblCurrentLayer.Text = ""
        m_BoolLenFlds = BoolLenFlds
        m_LengthFormat = LengthFormat


        m_Fnt = New System.Drawing.Font("Microsoft Sans Serif", fntSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)
        m_FntLbl = New System.Drawing.Font("Microsoft Sans Serif", fntSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)
        lblCurrentLayer.Font = m_Fnt
        m_FntHyper = New System.Drawing.Font("Microsoft Sans Serif", fntSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0)

        m_RouteToOption = RouteToOption
        If m_RouteToOption Then
            btnRouteTo.Visible = True
        Else
            btnRouteTo.Visible = False

        End If

        If m_GPSStatus = "On" Then
            btnGPSLoc.Enabled = True
            btnWaypoint.Enabled = True
        Else
            btnGPSLoc.Enabled = False
            btnWaypoint.Enabled = False
        End If
        toggleGPSButton = showGPSButton

        'Set the Map
        m_Map = map

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
        If LayerName <> "" Then
            CurrentLayer = LayerName

        End If

        toggleLayerLabel = showLayerLabel

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

                    RaiseEvent LocationIdentified(CType(pDT.Rows(idx), FeatureDataRow))
                    m_CurrentRow = CType(pDT.Rows(idx), FeatureDataRow)

                    PopulateAttributes()
                    Return True
                    'double ddist = map.SpatialReference.FromGeometry(dist); 


                End If
            End If
            btnEdit.Visible = False

            relocateButtons()
            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function
    Public Function IdentifyLocation(ByVal Location As Esri.ArcGIS.Mobile.Geometries.Coordinate) As Boolean
        Return IdentifyLocation(New Esri.ArcGIS.Mobile.Geometries.Point(Location))


    End Function

#End Region
#Region "Properties"

    Public Property Mode() As String
        Get
            Return m_Mode
        End Get
        Set(ByVal value As String)
            m_Mode = value


        End Set
    End Property
    Public Property toggleGPSButton() As Boolean
        Get
            Return btnGPSLoc.Visible
        End Get
        Set(ByVal value As Boolean)
            If GlobalsFunctions.appConfig.NavigationOptions.GPS.Visible.ToUpper = "TRUE" Then

                btnGPSLoc.Visible = value
            Else
                btnGPSLoc.Visible = False

            End If
            If GlobalsFunctions.appConfig.NavigationOptions.GPS.WaypointControl.Visible.ToUpper = "TRUE" Then

                btnWaypoint.Visible = value
            Else
                btnWaypoint.Visible = False

            End If

        End Set
    End Property
    Public Property toggleLayerLabel() As Boolean
        Get
            Return lblCurrentLayer.Parent.Visible
        End Get
        Set(ByVal value As Boolean)


            lblCurrentLayer.Parent.Visible = value

        End Set
    End Property

    Public Property GPSLocation() As GPSLocationDetails
        Get
            Return m_GPSVal
        End Get
        Set(ByVal value As GPSLocationDetails)
            m_GPSVal = value
        End Set
    End Property
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
    Public Property CurrentLayer() As String
        Get
            If m_FS Is Nothing Then
                Return Nothing
            Else
                Return m_FS.Name
            End If
        End Get
        Set(ByVal value As String)


            If m_FS Is Nothing Then
                m_CurrentRow = Nothing

                m_FS = GlobalsFunctions.GetFeatureSource(value, m_Map).FeatureSource 'CType(m_Map.MapLayers(value), MobileCacheMapLayer)
                If m_FS IsNot Nothing Then
                    AddControls()
                Else
                    If m_TabControl IsNot Nothing Then


                        m_TabControl.TabPages.Clear()
                        m_TabControl.Visible = False
                    End If
                End If
            Else
                If m_FS.Name <> value Then
                    m_FS = GlobalsFunctions.GetFeatureSource(value, m_Map).FeatureSource 'CType(m_Map.MapLayers(value), MobileCacheMapLayer)
                    m_CurrentRow = Nothing
                    If m_FS IsNot Nothing Then


                        AddControls()
                    Else
                        lblCurrentLayer.Text = ""
                        If m_TabControl IsNot Nothing Then
                            m_TabControl.TabPages.Clear()
                            m_TabControl.Visible = False
                        End If

                    End If
                End If
            End If



        End Set
    End Property
    Public Property CurrentRow() As FeatureDataRow
        Get
            Return m_CurrentRow
        End Get
        Set(ByVal value As FeatureDataRow)


            If value Is Nothing Then
                m_CurrentRow = Nothing
                ClearAttributes()

                'CurrentLayer = Nothing

                'If m_TabControl IsNot Nothing Then
                '    m_TabControl.TabPages.Clear()
                '    m_TabControl.Visible = False
                'End If
            Else
                CurrentLayer = value.FeatureSource.Name
                m_CurrentRow = value

                PopulateAttributes()
            End If

        End Set
    End Property
#End Region
#Region "Private Functions"
    Private Function selectFeature(ByVal Location As Esri.ArcGIS.Mobile.Geometries.Coordinate) As FeatureDataTable
        Return selectFeature(New Esri.ArcGIS.Mobile.Geometries.Point(Location))


    End Function
    Private Function selectFeature(ByVal Location As Esri.ArcGIS.Mobile.Geometries.Geometry) As FeatureDataTable
        Try
            Dim pDT As DataTable

            pDT = spatialQFeature(lblCurrentLayer.Text, Location)

            Return pDT


        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Private Function spatialQFeature(ByVal layername As String, ByVal Location As Esri.ArcGIS.Mobile.Geometries.Geometry) As FeatureDataTable
        Dim pFS As FeatureSourceWithDef = Nothing

        Dim pEnv As Envelope
        pFS = GlobalsFunctions.GetFeatureSource(layername, m_Map)
        If pFS IsNot Nothing Then

            Dim pMapDef As MobileCacheMapLayerDefinition = GlobalsFunctions.GetLayerDefinition(m_Map, pFS)




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



                Try
                    'Return the resulting rows

                    Return pFS.FeatureSource.GetDataTable(pQf, pStr)
                Catch ex As Exception

                End Try
            End If
        End If
        Return Nothing

    End Function
    Private Sub ClearAttributes()


        'The current field
        Dim strFld As String
        'Get the tab control from the container
        Dim pTabCntrl As TabControl
        'Button Added for hyperlinking
        Dim pBtn As Button
        'button location
        '  Dim pBtnPadding As Integer
        'Feature layer to get subtypes and domains
        'Dim pFL As FeatureDataTable
        Dim pSubTypeExist As Boolean = False

        Try

            'If the first control is not a tab control, exit
            pTabCntrl = CType(spltAttTools.Panel1.Controls(0), TabControl)
        Catch ex As Exception
            Return
        End Try

        'Remove all hyperlink buttons
        RemoveButton(pTabCntrl)


        'Loop through all controls and load the values from the feature to the display
        For Each pCntrl As Control In pTabCntrl.Controls
            'If the control is a tabpage, loop through each control on the page
            If TypeOf pCntrl Is TabPage Then
                For Each kCntrl As Control In pCntrl.Controls

                    If TypeOf kCntrl Is PictureBox Then
                        'Field name from the tag
                        strFld = CType(kCntrl, PictureBox).Tag.ToString
                        'Get a refrence to the picture control
                        Dim pPic As PictureBox
                        pPic = CType(kCntrl, PictureBox)
                        'load image from DB
                        'Dim pImgArray() As Byte
                        If m_CurrentRow.Item(strFld).ToString = "System.Drawing.Bitmap" Then
                            Try
                                'Catch any errors converting a byte array to an image
                                pPic.BackgroundImage = Nothing
                            Catch ex2 As Exception

                            End Try

                        Else
                            'pImgArray = CType(m_CurrentRow.Item(strFld), Byte())

                            Try
                                'Catch any errors converting a byte array to an image
                                pPic.BackgroundImage = Nothing
                            Catch ex2 As Exception

                            End Try

                        End If

                        pPic.Refresh()



                    ElseIf TypeOf kCntrl Is Panel Then


                        For Each cCntrl As Control In kCntrl.Controls
                            'Get the type of control
                            If TypeOf cCntrl Is TextBox Then

                                cCntrl.Text = ""

                            End If

                        Next
                    End If

                Next
            End If
        Next

        If m_HasFilter Then
            ShuffleControls()

        End If
        pTabCntrl = Nothing
        pBtn = Nothing
    End Sub
    Private Sub PopulateAttributes()


        'The current field
        Dim strFld As String
        'Get the tab control from the container
        Dim pTabCntrl As TabControl
        'Button Added for hyperlinking
        Dim pBtn As Button
        'button location
        Dim pBtnPadding As Integer
        'Feature layer to get subtypes and domains
        Dim pFDT As FeatureDataTable
        Dim pSubTypeExist As Boolean = False

        Try

            'If the first control is not a tab control, exit
            pTabCntrl = CType(spltAttTools.Panel1.Controls(0), TabControl)
        Catch ex As Exception
            Return
        End Try

        'Remove all hyperlink buttons
        RemoveButton(pTabCntrl)

        'Check if there is a subtype defined for this featureclass
        pFDT = m_CurrentRow.Table


        If pFDT.SubtypeColumnIndex >= 0 Then
            pSubTypeExist = True

        End If

        lblCurrentLayer.Text = pFDT.TableName

        'Loop through all controls and load the values from the feature to the display
        For Each pCntrl As Control In pTabCntrl.Controls
            'If the control is a tabpage, loop through each control on the page
            If TypeOf pCntrl Is TabPage Then
                'If pCntrl.Name.Contains("Attachments") Then

                'End If
                For Each kCntrl As Control In pCntrl.Controls
                    If TypeOf kCntrl Is ListBox Then
                        m_AttMan = m_CurrentRow.FeatureSource.AttachmentManager
                        Dim pAttList As List(Of Attachment) = m_AttMan.GetAttachments(m_CurrentRow.Fid)
                        CType(kCntrl, ListBox).DataSource = pAttList

                        'm_AttMan.GetAttachments(m_CurrentRow.Fid)

                        'For Each att As Attachment In pAttList
                        '    pLstBox.Items.Add(att)

                        'Next

                    ElseIf TypeOf kCntrl Is PictureBox Then
                        'Field name from the tag
                        strFld = CType(kCntrl, PictureBox).Tag.ToString
                        'Get a refrence to the picture control
                        Dim pPic As PictureBox
                        pPic = CType(kCntrl, PictureBox)
                        If m_CurrentRow.Item(strFld) Is DBNull.Value Then
                            'If the field is empty, skip it
                        Else
                            'load image from DB
                            Dim pImgArray() As Byte
                            If m_CurrentRow.Item(strFld).ToString = "System.Drawing.Bitmap" Then
                                Try
                                    'Catch any errors converting a byte array to an image
                                    pPic.BackgroundImage = m_CurrentRow.Item(strFld)
                                Catch ex2 As Exception

                                End Try

                            Else
                                pImgArray = CType(m_CurrentRow.Item(strFld), Byte())

                                Try
                                    'Catch any errors converting a byte array to an image
                                    pPic.BackgroundImage = GlobalsFunctions.ByteToImage(pImgArray)
                                Catch ex2 As Exception

                                End Try

                            End If

                            pPic.Refresh()

                        End If

                    ElseIf TypeOf kCntrl Is Panel Then


                        For Each cCntrl As Control In kCntrl.Controls
                            'Get the type of control
                            If TypeOf cCntrl Is TextBox Then



                                Dim obj As Object

                                Dim pFlOp As MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOption = CType(cCntrl.Parent.Tag, MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOption)

                                'Field name from the tag
                                strFld = cCntrl.Tag


                                If strFld = "MyShape_Len" Then
                                    Dim pPolyline As Esri.ArcGIS.Mobile.Geometries.Polyline
                                    pPolyline = m_CurrentRow.Geometry
                                    Dim distance As Double = 0

                                    For Each prt As CoordinateCollection In pPolyline.Parts
                                        'Dim pNewPoly As New Polyline(m_Map.SpatialReference.ToWgs84(prt))

                                        For i As Integer = 0 To prt.Count - 2
                                            distance += GlobalsFunctions.SegmentMeasures(m_Map, m_LengthUnit, prt(i), prt(i + 1))

                                        Next
                                    Next

                                    ' Dim dist2 As Double = pNewPoly.GetLength()
                                    'GlobalsFunctions.ConvertDistance(pPolyline.GetLength, m_Map.SpatialReference.Unit, m_LengthUnit)
                                    cCntrl.Text = String.Format(m_LengthFormat, distance)




                                Else


                                    'Used to parse out the hyper link if added
                                    If strFld.IndexOf("|") > 0 Then
                                        strFld = Trim(strFld.Substring(0, strFld.IndexOf("|")))
                                    End If
                                    Try


                                        If pSubTypeExist Then
                                            If strFld = pFDT.FeatureSource.Columns(pFDT.FeatureSource.SubtypeColumnIndex).ColumnName Then
                                                obj = pFDT.FeatureSource.Subtypes

                                            Else
                                                'MsgBox("Fix Domain")
                                                obj = pFDT.FeatureSource.Columns(strFld).GetDomain(m_CurrentRow.Item(pFDT.FeatureSource.SubtypeColumnIndex))
                                                '  obj = pFL.FeatureSource.GetDomain(m_CurrentRow.Item(pFL.SubtypeColumnIndex), strFld)
                                            End If

                                        Else
                                            'MsgBox("Fix Domain")
                                            ' obj = pFL.FeatureSource.GetDomain(0, strFld)
                                            obj = pFDT.FeatureSource.Columns(strFld).GetDomain(0)
                                        End If
                                    Catch ex As Exception
                                        obj = Nothing

                                    End Try
                                    'If the feature layer has a value
                                    If m_CurrentRow.Table.Columns.IndexOf(strFld) >= 0 Then

                                        Dim pDataColumn = m_CurrentRow.Table.Columns.Item(strFld)
                                        Dim pDisplayString As String

                                        If m_CurrentRow.Item(strFld) IsNot DBNull.Value Then
                                            'Used to check for hyper links
                                            pDisplayString = m_CurrentRow.Item(strFld).ToString
                                            If pDisplayString.Contains("http") Or pDisplayString.Contains(":\") Or pDisplayString.Contains("\\") Then
                                                'Create a new button to launch the hyperlink
                                                pBtn = New Button
                                                'Resize text box 
                                                cCntrl.Width = cCntrl.Width - cCntrl.Height
                                                cCntrl.Cursor = Cursors.Hand
                                                'cCntrl.ForeColor = Color.LightBlue
                                                m_BaseColor = cCntrl.BackColor
                                                cCntrl.BackColor = Color.LightBlue
                                                'cCntrl.Font = m_FntHyper
                                                'cCntrl.Invalidate()
                                                'Size and locate contrl
                                                pBtn.Width = 30
                                                pBtn.Height = 30

                                                pBtn.Left = cCntrl.Left + cCntrl.Width + pBtnPadding
                                                pBtn.Top = cCntrl.Top
                                                'Remove any text on the button
                                                pBtn.Text = ""
                                                'Apply and allign the hyperlnk image
                                                pBtn.ImageAlign = ContentAlignment.MiddleCenter
                                                pBtn.BackgroundImage = Global.MobileControls.My.Resources.hyper
                                                pBtn.BackgroundImageLayout = ImageLayout.Stretch
                                                'Tag the button with the hyperlink value
                                                pBtn.Tag = pDisplayString
                                                CType(cCntrl, TextBox).Tag = pDisplayString
                                                'Name the button with the field
                                                pBtn.Name = "btnHyper" & strFld
                                                'add the handler to launc the hyperlink
                                                AddHandler pBtn.Click, AddressOf btnHyper_Click
                                                'add the button
                                                kCntrl.Controls.Add(pBtn)
                                                'Attach a handler to the text box it self
                                                AddHandler cCntrl.Click, AddressOf btnHyper_Click
                                                'Add the hyperlink cursor


                                            ElseIf pFlOp.HyperFormat <> "" Then
                                                'Create a new button to launch the hyperlink
                                                pBtn = New Button
                                                'Resize text box 
                                                cCntrl.Width = cCntrl.Width - cCntrl.Height
                                                cCntrl.Cursor = Cursors.Hand
                                                'cCntrl.ForeColor = Color.LightBlue
                                                m_BaseColor = cCntrl.BackColor
                                                cCntrl.BackColor = Color.LightBlue
                                                'cCntrl.Font = m_FntHyper
                                                'cCntrl.Invalidate()
                                                'Size and locate contrl
                                                pBtn.Width = 30
                                                pBtn.Height = 30

                                                pBtn.Left = cCntrl.Left + cCntrl.Width + pBtnPadding
                                                pBtn.Top = cCntrl.Top
                                                'Remove any text on the button
                                                pBtn.Text = ""
                                                'Apply and allign the hyperlnk image
                                                pBtn.ImageAlign = ContentAlignment.MiddleCenter
                                                pBtn.BackgroundImage = Global.MobileControls.My.Resources.hyper
                                                pBtn.BackgroundImageLayout = ImageLayout.Stretch
                                                'Tag the button with the hyperlink value
                                                '
                                                'pDisplayString = pFlOp.HyperFormat.Replace("{value}", m_CurrentRow.Item(strFld).ToString)
                                                pDisplayString = m_CurrentRow.Item(strFld).ToString

                                                pBtn.Tag = pFlOp.HyperFormat.Replace("{value}", m_CurrentRow.Item(strFld).ToString)
                                                'Name the button with the field
                                                pBtn.Name = "btnHyper" & strFld
                                                'add the handler to launc the hyperlink
                                                AddHandler pBtn.Click, AddressOf btnHyper_Click
                                                'add the button
                                                kCntrl.Controls.Add(pBtn)
                                                'Attach a handler to the text box it self
                                                AddHandler cCntrl.Click, AddressOf btnHyper_Click
                                                'Add the hyperlink cursor
                                                CType(cCntrl, TextBox).Tag = pFlOp.HyperFormat.Replace("{value}", m_CurrentRow.Item(strFld).ToString)

                                            End If


                                            If pDataColumn IsNot Nothing And pDataColumn.DataType Is System.Type.GetType("System.DateTime") Then

                                                ''Dim myutils As ConfigUtils = New ConfigUtils()
                                                Dim format As String = GlobalsFunctions.appConfig.EditControlOptions.DateTimeDisplayFormat
                                                If format Is Nothing Then
                                                    'MessageBox.Show("DateTimeDisplayFormat configuration not found.")
                                                    format = "MM/dd/yyyy"
                                                ElseIf format.Trim = "" Then
                                                    'MessageBox.Show("DateTimeDisplayFormat configuration not found.")
                                                    format = "MM/dd/yyyy"
                                                End If
                                                pDisplayString = CType(m_CurrentRow.Item(strFld), DateTime).ToString(format)



                                            End If

                                            'Check to see if we need to translate to value to description from a domain
                                            If m_ShowDomain Then
                                                If TypeOf obj Is CodedValueDomain Then
                                                    'Get the coded value domain
                                                    Dim pCvd As CodedValueDomain = CType(obj, CodedValueDomain)

                                                    Dim m_CurrentRows() As DataRow
                                                    'Check value column data type
                                                    If pCvd.Columns("Code").DataType IsNot System.Type.GetType("System.String") Then
                                                        m_CurrentRows = pCvd.Select("Code = " & pDisplayString)
                                                    Else
                                                        m_CurrentRows = pCvd.Select("Code = '" & pDisplayString & "'")
                                                    End If
                                                    'Get the domain description
                                                    If m_CurrentRows.Length > 0 Then
                                                        If m_CurrentRows(0)("Value") IsNot DBNull.Value Then
                                                            pDisplayString = m_CurrentRows(0)("Value").ToString
                                                        End If
                                                    End If

                                                Else

                                                End If

                                            Else

                                            End If
                                            'Add the value of the field to the text box
                                            CType(cCntrl, TextBox).Text = pDisplayString
                                        Else
                                            'Set it empty
                                            CType(cCntrl, TextBox).Text = ""
                                        End If

                                    Else
                                        'Set it empty
                                        CType(cCntrl, TextBox).Text = ""
                                    End If
                                End If

                            ElseIf TypeOf cCntrl Is PictureBox Then
                                'Field name from the tag
                                strFld = CType(cCntrl, PictureBox).Tag.ToString
                                'Get a refrence to the picture control
                                Dim pPic As PictureBox
                                pPic = CType(cCntrl, PictureBox)
                                If m_CurrentRow.Item(strFld) Is DBNull.Value Then
                                    'If the field is empty, skip it
                                Else
                                    'load image from DB
                                    Dim pImgArray() As Byte
                                    If m_CurrentRow.Item(strFld).ToString = "System.Drawing.Bitmap" Then
                                        Try
                                            'Catch any errors converting a byte array to an image
                                            pPic.BackgroundImage = m_CurrentRow.Item(strFld)
                                        Catch ex2 As Exception

                                        End Try

                                    Else
                                        pImgArray = CType(m_CurrentRow.Item(strFld), Byte())

                                        Try
                                            'Catch any errors converting a byte array to an image
                                            pPic.BackgroundImage = GlobalsFunctions.ByteToImage(pImgArray)
                                        Catch ex2 As Exception

                                        End Try

                                    End If

                                    pPic.Refresh()

                                End If
                            End If

                        Next
                    End If

                Next
            End If
        Next

        'Flash geometry
        Dim pFLDT As Esri.ArcGIS.Mobile.FeatureCaching.FeatureDataTable = m_CurrentRow.Table

        If CType(m_CurrentRow(pFLDT.GeometryColumnIndex), Geometry).GeometryType = GeometryType.Point Then
            GlobalsFunctions.flashGeo(CType(m_CurrentRow(pFLDT.GeometryColumnIndex), Geometry), m_Map, m_pen, m_brush)
        Else
            GlobalsFunctions.flashGeo(CType(m_CurrentRow(pFLDT.GeometryColumnIndex), Geometry), m_Map, m_penLine, m_brush)
        End If
        If m_HasFilter Then
            ShuffleControls()

        End If
        'release variables
        pFLDT = Nothing
        pTabCntrl = Nothing
        pBtn = Nothing
    End Sub
    Private Sub RemoveButton(ByVal cont As Control)
        Try

            For Each pCntrl As Control In cont.Controls
                'If the control is a tabpage, loop through each control on the page
                If TypeOf pCntrl Is TabPage Then
                    For Each cCntrl As Control In pCntrl.Controls
                        'If it is button
                        If TypeOf cCntrl Is Button Then
                            If cCntrl.Name.Contains("btnHyper") Then
                                'Get the text field linked to it and resize it
                                Dim lContr() As Control = pCntrl.Controls.Find("txtAtt" & cCntrl.Name.Remove(0, 8), False)
                                If lContr.Length > 0 Then
                                    Dim pTextBox As TextBox = CType(lContr(0), TextBox)
                                    pTextBox.Width = pTextBox.Width + pTextBox.Height
                                    'Add the hyperlink cursor
                                    pTextBox.Cursor = Cursors.Default

                                    'Reset the color
                                    'lContr(0).ForeColor = Color.Black


                                    pTextBox.BackColor = m_BaseColor

                                    'reset the font
                                    'cCntrl.Font = m_Fnt
                                    'cCntrl.Invalidate()

                                    RemoveHandler pTextBox.Click, AddressOf btnHyper_Click
                                End If
                                'remove the control
                                pCntrl.Controls.Remove(cCntrl)
                            End If
                        End If

                    Next
                End If
            Next
        Catch ex As Exception
            'RaiseEvent "Error in Remove Button"
        End Try

    End Sub
    Private Sub AddControls()
        Try

            If m_FS Is Nothing Then Exit Sub

            lblCurrentLayer.Text = m_FS.Name

            If m_TabControl Is Nothing Then
                'Create the tab control and set the found
                m_TabControl = New TabControl

                'Set the control to fill the entire space
                m_TabControl.Dock = DockStyle.Fill
                'Add the tab controls
                spltAttTools.Panel1.Controls.Add(m_TabControl)

                'Assign the global font
                m_TabControl.Font = m_Fnt

            Else

                'Clear out the controls from the container
                m_TabControl.TabPages.Clear()
                m_TabControl.Controls.Clear()
            End If


            m_TabControl.Visible = True

            'Controls to display attributes
            Dim pTbPg As TabPage = Nothing
            'TabControl for Pictures
            Dim pTbPgPic As TabPage = Nothing
            Dim pTabAtt As TabPage = Nothing

            Dim pTxtBox As TextBox
            Dim pLbl As Label
            Dim pPic As PictureBox
            Dim pBtn As New Button
            'Spacing between each control
            Dim intCtrlSpace As Integer = 5
            'Spacing between a lable and a control
            Dim intLabelCtrlSpace As Integer = 0

            'Spacing between last control and the bottom of the page
            Dim pBottomPadding As Integer = 100
            'Padding for the left of each control
            Dim pLeftPadding As Integer = 10
            'Spacing between firstcontrol and the top
            Dim pTopPadding As Integer = 10
            'Padding for the right of each control
            Dim pRightPadding As Integer = 10
            'Location of next control
            Dim pNextControlTop As Integer = pTopPadding
            Dim pNextControlTopPic As Integer = pTopPadding
            'Set the width of each control
            Dim pControlWidth As Integer = pnlAttributes.Width - 10
            'used for sizing text, only used when text is larger then display
            Dim g As Graphics
            Dim s As SizeF


            'Used to loop through FeatureSource
            Dim pDCs As ReadOnlyColumnCollection
            Dim pDc As DataColumn
            'Get the columns for hte layer
            pDCs = m_FS.Columns

            'Field Name
            Dim strfld As String
            'Field Alias
            Dim strAli As String

            'The current tab count
            Dim intPgIdx As Integer = 1
            Dim intPgIdxPic As Integer = 1




            Dim pDcArrListTemp As New ArrayList(pDCs.ToArray())
            If m_BoolLenFlds And m_FS.GeometryType = GeometryType.Polyline Then


                Dim pNewDC As DataColumn = New DataColumn
                pNewDC.Caption = "Geometric Length"
                pNewDC.ColumnName = "MyShape_Len"

                pDcArrListTemp.Add(pNewDC)
                pNewDC = Nothing

            End If
            Dim pDcArrList As New List(Of MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOption)


            If GlobalsFunctions.appConfig.LayerOptions.LayersFieldOptions.Layer.Count > 0 Then
                For Each layFldOrd As MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerName In GlobalsFunctions.appConfig.LayerOptions.LayersFieldOptions.Layer
                    If layFldOrd.Name.ToUpper = m_FS.Name.ToUpper Then

                        pDcArrList.Clear()

                        For Each fldsOrd As MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOption In layFldOrd.Field

                            ' For Each fldOrd As MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOption In fldsOrd.OrderField
                            'CType(pDcArrList.Item(iCount), DataColumn)
                            If m_FS.Columns.Contains(fldsOrd.Name) Then
                                If Not pDcArrList.Contains(fldsOrd) Then
                                    pDcArrList.Add(fldsOrd)

                                End If


                            ElseIf fldsOrd.Name = "Geometric Length" Then
                                If Not pDcArrList.Contains(fldsOrd) Then
                                    pDcArrList.Add(fldsOrd)

                                End If

                            End If
                            'Next
                        Next
                        Exit For

                    End If
                Next
            End If
            If pDcArrList.Count = 0 Then


                For Each pDc In pDcArrListTemp
                    Dim pTmp As New MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOption
                    pTmp.Name = pDc.ColumnName
                    pTmp.Caption = pDc.Caption

                    pTmp.VisibleDefault = "True"
                    pDcArrList.Add(pTmp)
                Next
            End If
            Dim pDcOpt As New MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOption





            For Each pDcOpt In pDcArrList

                pDc = m_FS.Columns(pDcOpt.Name)
                If pDc IsNot Nothing Then
                    'Get the field names
                    strfld = pDc.ColumnName
                    strAli = pDc.Caption
                    If pDcOpt IsNot Nothing Then
                        If pDcOpt.Caption IsNot Nothing Then
                            If pDcOpt.Caption.Trim <> "" Then
                                strAli = pDcOpt.Caption
                            End If
                        End If
                    End If
                Else
                    strfld = pDcOpt.Name
                    strAli = pDcOpt.Caption

                End If



                'if currently on  tab page, check to see if a new page is needed
                If pTbPg IsNot Nothing Then
                    'check If the top of the next control will fit
                    If pNextControlTop > pTbPg.Height - pBottomPadding Then


                        'a new page is needed
                        pNextControlTop = pTopPadding
                        'Change the page count
                        intPgIdx = intPgIdx + 1
                        'Add a new page
                        If intPgIdxPic > 1 Then
                            pTbPg = New TabPage(GlobalsFunctions.appConfig.IDPanel.UIComponents.PageName & ":" & intPgIdx)


                            m_TabControl.TabPages.Insert(m_TabControl.TabPages.Count - intPgIdxPic + 1, pTbPg)


                        Else
                            m_TabControl.TabPages.Add(GlobalsFunctions.appConfig.IDPanel.UIComponents.PageName & ":" & intPgIdx, GlobalsFunctions.appConfig.IDPanel.UIComponents.PageName & ":" & intPgIdx)
                            pTbPg = m_TabControl.TabPages(GlobalsFunctions.appConfig.IDPanel.UIComponents.PageName & ":" & intPgIdx)

                        End If

                    End If
                Else

                    m_TabControl.TabPages.Add(GlobalsFunctions.appConfig.IDPanel.UIComponents.PageName & ":" & intPgIdx, GlobalsFunctions.appConfig.IDPanel.UIComponents.PageName & ":" & intPgIdx)
                    pTbPg = m_TabControl.TabPages(GlobalsFunctions.appConfig.IDPanel.UIComponents.PageName & ":" & intPgIdx)

                End If
                'Check for columns that do not need to be displayed
                If m_FS.GeometryColumnName = strfld Or m_FS.FidColumnName = strfld Or _
                   UCase(strfld) = UCase("shape.len") Or UCase(strfld) = UCase("shape.area") Then

                ElseIf pDcOpt.Name = "MyShape_Len" Then
                    'Create a lable for the field name
                    pLbl = New Label
                    'Apply the field alias to the field name
                    pLbl.Text = strAli
                    'Link the field to the name of the control
                    pLbl.Name = "lblAtt" & strfld
                    'Add the control at the determined locaiton
                    pLbl.Left = pLeftPadding

                    pLbl.Top = pNextControlTop
                    'Apply global font
                    pLbl.Font = m_FntLbl
                    'Create a graphics object to messure the text
                    g = pLbl.CreateGraphics
                    s = g.MeasureString(pLbl.Text, pLbl.Font)

                    pLbl.Height = CInt(s.Height)
                    'If the text is larger then the control, truncate the control
                    If s.Width >= pnlAttributes.Width Then
                        pLbl.Width = pControlWidth - pLeftPadding - pRightPadding
                    Else 'Use autosize if it fits
                        pLbl.AutoSize = True
                    End If
                    'Determine the locaiton for the next control
                    '**Used when not grouping text and label in a panel
                    'pNextControlTop = CInt(pLbl.Top + s.Height + intLabelCtrlSpace)

                    'Create a new control to display the attributes                    
                    pTxtBox = New TextBox
                    'Disable the control
                    pTxtBox.ReadOnly = True
                    'Tag the control with the field it represents
                    pTxtBox.Tag = Trim(strfld)
                    'Name the field with the field name
                    pTxtBox.Name = "txtAtt" & strfld
                    'Locate the control on the display
                    pTxtBox.Left = pLeftPadding
                    pTxtBox.Top = pNextControlTop
                    pTxtBox.Width = pControlWidth - pLeftPadding - pRightPadding
                    pTxtBox.BackColor = m_BColor
                    pTxtBox.ForeColor = m_FColor
                    'Apply global font
                    pTxtBox.Font = m_Fnt
                    '**Used when not grouping text and label in a panel
                    'Determine the locaiton for the next contro
                    'pNextControlTop = pTxtBox.Top + pTxtBox.Height + intCtrlSpace
                    ''Add the controls
                    'pTbPg.Controls.Add(pLbl)
                    'pTbPg.Controls.Add(pTxtBox)





                    Dim pPnl As Panel = New Panel
                    pPnl.Tag = pDcOpt
                    If pDcOpt.FilterFields IsNot Nothing Then
                        If pDcOpt.FilterFields.Count > 0 Then
                            m_HasFilter = True
                        End If


                    End If
                    If pDcOpt.ForeColor <> "" Then
                        pLbl.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)

                    End If
                    If pDcOpt.BackColor <> "" Then
                        pLbl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)

                    End If
                    If pDcOpt.ForeColor <> "" Then
                        pTxtBox.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)

                    End If
                    If pDcOpt.BackColor <> "" Then

                        pTxtBox.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)


                    End If
                    If pDcOpt.BoxColor <> "" Then
                        pPnl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BoxColor)

                    End If
                    pPnl.BorderStyle = Windows.Forms.BorderStyle.None

                    pLbl.Top = 0
                    pTxtBox.Top = 5 + pLbl.Height
                    pPnl.Width = pControlWidth
                    pPnl.Margin = New Padding(0)
                    pPnl.Padding = New Padding(0)





                    pPnl.Top = pNextControlTop
                    pPnl.Left = 0
                    pPnl.Height = pTxtBox.Height + pLbl.Height + 10
                    pPnl.Controls.Add(pLbl)
                    pPnl.Controls.Add(pTxtBox)
                    pTbPg.Controls.Add(pPnl)

                    pNextControlTop = pPnl.Top + pPnl.Height + intCtrlSpace



                Else
                    'Determine type of field
                    If pDc.DataType Is System.Type.GetType("System.String") Or _
                       pDc.DataType Is System.Type.GetType("System.Integer") Or _
                       pDc.DataType Is System.Type.GetType("System.Int32") Or _
                        pDc.DataType Is System.Type.GetType("System.Int16") Or _
                       pDc.DataType Is System.Type.GetType("System.Boolean") Or _
                       pDc.DataType Is System.Type.GetType("System.Double") Or _
                       pDc.DataType Is System.Type.GetType("System.DateTime") Or _
                       pDc.DataType Is System.Type.GetType("System.Single") Then
                        'Simple Text display


                        'Create a lable for the field name
                        pLbl = New Label
                        'Apply the field alias to the field name
                        pLbl.Text = strAli
                        'Link the field to the name of the control
                        pLbl.Name = "lblAtt" & strfld
                        'Add the control at the determined locaiton
                        pLbl.Left = pLeftPadding

                        pLbl.Top = pNextControlTop
                        'Apply global font
                        pLbl.Font = m_FntLbl
                        'Create a graphics object to messure the text
                        g = pLbl.CreateGraphics
                        s = g.MeasureString(pLbl.Text, pLbl.Font)

                        pLbl.Height = CInt(s.Height)
                        'If the text is larger then the control, truncate the control
                        If s.Width >= pnlAttributes.Width Then
                            pLbl.Width = pControlWidth - pLeftPadding - pRightPadding
                        Else 'Use autosize if it fits
                            pLbl.AutoSize = True
                        End If
                        'Determine the locaiton for the next control
                        '**Used when not grouping text and label in a panel
                        'pNextControlTop = CInt(pLbl.Top + s.Height + intLabelCtrlSpace)

                        'Create a new control to display the attributes                    
                        pTxtBox = New TextBox
                        'Disable the control
                        pTxtBox.ReadOnly = True
                        'Tag the control with the field it represents
                        pTxtBox.Tag = Trim(strfld)
                        'Name the field with the field name
                        pTxtBox.Name = "txtAtt" & strfld
                        'Locate the control on the display
                        pTxtBox.Left = pLeftPadding
                        pTxtBox.Top = pNextControlTop
                        pTxtBox.Width = pControlWidth - pLeftPadding - pRightPadding
                        pTxtBox.BackColor = m_BColor
                        pTxtBox.ForeColor = m_FColor
                        'Apply global font
                        pTxtBox.Font = m_Fnt
                        '**Used when not grouping text and label in a panel
                        'Determine the locaiton for the next contro
                        'pNextControlTop = pTxtBox.Top + pTxtBox.Height + intCtrlSpace
                        ''Add the controls
                        'pTbPg.Controls.Add(pLbl)
                        'pTbPg.Controls.Add(pTxtBox)

                        Try
                            If pDc.MaxLength > CInt(GlobalsFunctions.appConfig.ApplicationSettings.MultiLineThreshold.Threshold) Then
                                pTxtBox.Multiline = True
                                pTxtBox.Height = pTxtBox.Height * CInt(GlobalsFunctions.appConfig.ApplicationSettings.MultiLineThreshold.SizeFactor)

                            End If
                        Catch ex As Exception
                            If pDc.MaxLength > 125 Then
                                pTxtBox.Multiline = True
                                pTxtBox.Height = pTxtBox.Height * 3

                            End If
                        End Try




                        Dim pPnl As Panel = New Panel
                        pPnl.Tag = pDcOpt
                        If pDcOpt.FilterFields IsNot Nothing Then
                            If pDcOpt.FilterFields.Count > 0 Then
                                m_HasFilter = True
                            End If


                        End If
                        If pDcOpt.ForeColor <> "" Then
                            pLbl.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)

                        End If
                        If pDcOpt.BackColor <> "" Then
                            pLbl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)

                        End If
                        If pDcOpt.ForeColor <> "" Then
                            pTxtBox.ForeColor = GlobalsFunctions.stringToColor(pDcOpt.ForeColor)

                        End If
                        If pDcOpt.BackColor <> "" Then
                            If pDcOpt.BackColor <> "buttonface" Then
                                pTxtBox.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BackColor)
                            End If


                        End If
                        If pDcOpt.BoxColor <> "" Then
                            pPnl.BackColor = GlobalsFunctions.stringToColor(pDcOpt.BoxColor)

                        End If
                        pPnl.BorderStyle = Windows.Forms.BorderStyle.None

                        pLbl.Top = 0
                        pTxtBox.Top = 5 + pLbl.Height
                        pPnl.Width = pControlWidth
                        pPnl.Margin = New Padding(0)
                        pPnl.Padding = New Padding(0)





                        pPnl.Top = pNextControlTop
                        pPnl.Left = 0
                        pPnl.Height = pTxtBox.Height + pLbl.Height + 10
                        pPnl.Controls.Add(pLbl)
                        pPnl.Controls.Add(pTxtBox)
                        pTbPg.Controls.Add(pPnl)

                        pNextControlTop = pPnl.Top + pPnl.Height + intCtrlSpace



                    ElseIf pDc.DataType.FullName = "System.Drawing.Bitmap" Then
                        '   If pTbPgPic Is Nothing Then
                        m_TabControl.TabPages.Add("Image" & ":" & intPgIdxPic, strAli) '("Image" & ":" & intPgIdxPic, "Image" & ":" & intPgIdxPic)
                        pTbPgPic = m_TabControl.TabPages("Image" & ":" & intPgIdxPic)
                        intPgIdxPic = intPgIdxPic + 1
                        pNextControlTopPic = pTopPadding
                        pTbPgPic.Tag = pDcOpt

                        'Create a lable for the field name
                        pLbl = New Label
                        'Apply the field alias to the field name
                        pLbl.Text = strAli
                        'Link the field to the name of the control
                        pLbl.Name = "lblAtt" & strfld
                        'Add the control at the determined locaiton
                        pLbl.Left = pLeftPadding
                        pLbl.Top = pNextControlTopPic
                        'Apply global font
                        pLbl.Font = m_FntLbl
                        'Create a graphics object to messure the text
                        g = pLbl.CreateGraphics
                        s = g.MeasureString(pLbl.Text, pLbl.Font)
                        pLbl.Height = CInt(s.Height)
                        'If the text is larger then the control, truncate the control
                        If s.Width >= pnlAttributes.Width Then
                            pLbl.Width = pControlWidth - pLeftPadding - pRightPadding
                        Else 'Use autosize if it fits
                            pLbl.AutoSize = True
                        End If
                        'Determine the locaiton for the next control
                        pNextControlTopPic = CInt(pLbl.Top + s.Height + intLabelCtrlSpace)



                        'Create a new control to display the attributes                    
                        pPic = New PictureBox
                        'Disable the control
                        '  pPic.ReadOnly = True
                        'Tag the control with the field it represents
                        pPic.Tag = Trim(strfld)
                        'Name the field with the field name
                        pPic.Name = "picAtt" & strfld
                        'Locate the control on the display
                        pPic.Left = pLeftPadding
                        pPic.Top = pNextControlTopPic
                        pPic.Width = pControlWidth - pLeftPadding - pRightPadding
                        pPic.Height = pControlWidth - pLeftPadding - pRightPadding
                        pPic.BackgroundImageLayout = ImageLayout.Stretch

                        'Apply global font
                        pPic.Font = m_Fnt



                        'Add the controls
                        pTbPgPic.Controls.Add(pLbl)
                        pTbPgPic.Controls.Add(pPic)




                    ElseIf pDc.DataType Is System.Type.GetType("System.Byte[]") Then
                        '   If pTbPgPic Is Nothing Then
                        m_TabControl.TabPages.Add("Image" & ":" & intPgIdxPic, strAli)
                        pTbPgPic = m_TabControl.TabPages("Image" & ":" & intPgIdxPic)
                        intPgIdxPic = intPgIdxPic + 1
                        pNextControlTopPic = pTopPadding

                        pTbPgPic.Tag = pDcOpt

                        'Create a lable for the field name
                        pLbl = New Label
                        'Apply the field alias to the field name
                        pLbl.Text = strAli
                        'Link the field to the name of the control
                        pLbl.Name = "lblAtt" & strfld
                        'Add the control at the determined locaiton
                        pLbl.Left = pLeftPadding
                        pLbl.Top = pNextControlTopPic
                        'Apply global font
                        pLbl.Font = m_FntLbl
                        'Create a graphics object to messure the text
                        g = pLbl.CreateGraphics
                        s = g.MeasureString(pLbl.Text, pLbl.Font)
                        pLbl.Height = CInt(s.Height)
                        'If the text is larger then the control, truncate the control
                        If s.Width >= pnlAttributes.Width Then
                            pLbl.Width = pControlWidth - pLeftPadding - pRightPadding
                        Else 'Use autosize if it fits
                            pLbl.AutoSize = True
                        End If
                        'Determine the locaiton for the next control
                        pNextControlTopPic = CInt(pLbl.Top + s.Height + intLabelCtrlSpace)



                        'Create a new control to display the attributes                    
                        pPic = New PictureBox
                        'Disable the control
                        '  pPic.ReadOnly = True
                        'Tag the control with the field it represents
                        pPic.Tag = Trim(strfld)
                        'Name the field with the field name
                        pPic.Name = "picAtt" & strfld
                        'Locate the control on the display
                        pPic.Left = pLeftPadding
                        pPic.Top = pNextControlTopPic
                        pPic.Width = pControlWidth - pLeftPadding - pRightPadding
                        pPic.Height = pControlWidth - pLeftPadding - pRightPadding
                        pPic.BackgroundImageLayout = ImageLayout.Stretch

                        'Apply global font
                        pPic.Font = m_Fnt



                        'Add the controls
                        pTbPgPic.Controls.Add(pLbl)
                        pTbPgPic.Controls.Add(pPic)


                    End If

                End If





            Next 'pDC
            m_AttMan = m_FS.AttachmentManager


            If m_AttMan.HasAttachments Then
                m_TabControl.TabPages.Add("Attachments", "Attachments") '("Image" & ":" & intPgIdxPic, "Image" & ":" & intPgIdxPic)
                pTabAtt = m_TabControl.TabPages("Attachments")


                Dim pLstBox As ListBox = New ListBox()
                pLstBox.DrawMode = DrawMode.OwnerDrawVariable

                pLstBox.Font = New System.Drawing.Font("Tahoma", 12.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0)


                pLstBox.HorizontalScrollbar = True
                pLstBox.ItemHeight = 50
                'pLstBox.Margin = New Padding(0, 25, 0, 0)

                'pLstBox.Padding = New Padding(0, 25, 0, 0)

                AddHandler pLstBox.MouseClick, AddressOf attlistclick
                AddHandler pLstBox.DrawItem, AddressOf attdraw
                pLstBox.ValueMember = "Name"
                pLstBox.Dock = DockStyle.Fill
                'Dim pAttList As List(Of Attachment) = m_AttMan.GetAttachments(m_CurrentRow.Fid)
                ' pLstBox.DataSource = pAttList
                'For Each att As Attachment In pAttList
                '    pLstBox.Items.Add(att)

                'Next
                pTabAtt.Controls.Add(pLstBox)
                pTabAtt.Visible = True
                'm_TabControl.TabPages.Add(pTabAtt)

                pTabAtt.Update()
            End If
            If pTbPg.Controls.Count = 0 Then
                'if a tabpage was added with no controls, remove it
                m_TabControl.TabPages.Remove(pTbPg)

            Else


            End If

            ShuffleControls()

            'cleanup
            pBtn = Nothing
            pDCs = Nothing
            pDc = Nothing

            pTbPg = Nothing

            pTbPgPic = Nothing
            pTxtBox = Nothing
            pLbl = Nothing
            pPic = Nothing

            m_FS = Nothing

            g = Nothing
            s = Nothing
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try

    End Sub
    Private Sub ShuffleControls()


        'Spacing between last control and the bottom of the page
        Dim pBottomPadding As Integer = 75
        'Padding for the left of each control
        Dim pLeftPadding As Integer = 10
        'Spacing between firstcontrol and the top
        Dim pTopPadding As Integer = 3
        'Padding for the right of each control
        Dim pRightPadding As Integer = 15

        If m_TabControl Is Nothing Then Return

        Dim pCurTabIdx As Integer = m_TabControl.SelectedIndex

        Dim pTbPageCo() As TabPage = Nothing
        Dim pTbPageCoPic() As TabPage = Nothing

        Dim pCurTabPage As TabPage = New TabPage
        Dim pCurTabPic As TabPage = New TabPage
        Dim pTabAtt As TabPage = Nothing

        Dim idxPage As Integer = 1

        Dim strName As String = GlobalsFunctions.appConfig.IDPanel.UIComponents.PageName & ":" & idxPage
        idxPage = idxPage + 1

        pCurTabPage.Name = strName
        pCurTabPage.Text = strName
        Dim pCntlNextTop As Integer = pTopPadding
        Dim pQFilt As QueryFilter = Nothing
        For Each tb As TabPage In m_TabControl.TabPages

            If tb.Name.Contains("Image") Then
                If pTbPageCoPic Is Nothing Then
                    ReDim Preserve pTbPageCoPic(0)
                Else
                    ReDim Preserve pTbPageCoPic(pTbPageCoPic.Length)
                End If
                pTbPageCoPic(pTbPageCoPic.Length - 1) = tb

            ElseIf tb.Name.Contains("Attachment") Then
                pTabAtt = tb

            Else



                Dim bLoop As Boolean = True
                While bLoop = True
                    If tb.Controls Is Nothing Then
                        Exit While

                    End If
                    If tb.Controls.Count = 0 Then
                        Exit While

                    End If

                    Dim cnt As Control = tb.Controls(0)

                    If TypeOf cnt Is Button Then
                        tb.Controls.Remove(cnt)

                    Else


                        cnt.Top = pCntlNextTop
                        cnt.Width = m_TabControl.Width
                        Dim valsMatch As Boolean = False
                        Dim visToSet As Boolean = False
                        If TypeOf cnt Is Panel Then

                            If cnt.Tag IsNot Nothing And m_CurrentRow IsNot Nothing Then
                                If TypeOf (cnt.Tag) Is MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOption Then
                                    Dim fieldDet As MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOption = cnt.Tag
                                    If fieldDet.FilterFields IsNot Nothing Then
                                        If fieldDet.FilterFields.Count > 0 Then
                                            For Each filtVals As MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOptionFilterFields In fieldDet.FilterFields

                                                Dim bFiltvalid As Boolean = True

                                                If filtVals.Mode <> "" Then
                                                    If m_Mode = filtVals.Mode Then
                                                        bFiltvalid = True
                                                    Else
                                                        bFiltvalid = False
                                                    End If
                                                Else
                                                    bFiltvalid = True
                                                End If
                                                If bFiltvalid Then
                                                    'If filtVals.sqlValue <> "" Then

                                                    '    pQFilt = New QueryFilter

                                                    '    pQFilt.WhereClause = filtVals.sqlValue & " AND " & m_CurrentRow.FeatureSource.FidColumnName & " = " & m_CurrentRow.Fid

                                                    '    'Dim pDRead As FeatureDataReader = pFl.GetDataReader(pQFilt)
                                                    '    'If pDRead.ReadFirst Then
                                                    '    'm_FDR
                                                    '    If m_CurrentRow.FeatureSource.GetFeatureCount(pQFilt) > 0 Then
                                                    '        valsMatch = True
                                                    '        Boolean.TryParse(filtVals.Visibility, visToSet)
                                                    '        Exit For
                                                    '    End If
                                                    'Else
                                                    Boolean.TryParse(filtVals.Visibility, visToSet)
                                                    valsMatch = True

                                                    For Each filtVal As MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOptionFilterFieldsFilterInfo In filtVals.FilterInfo

                                                        ' For Each filtInfo As MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOptionFilterFieldsFilterInfo In filtVal.FilterInfo
                                                        For Each vl As String In filtVal.FieldValue
                                                            If m_CurrentRow.Item(filtVal.FieldName).ToString() = vl Then
                                                                valsMatch = True
                                                                Exit For
                                                            ElseIf vl = "[ISNULL]" And m_CurrentRow.Item(filtVal.FieldName) Is Nothing Then
                                                                valsMatch = True
                                                                Exit For

                                                            ElseIf vl = "[ISNULL]" And m_CurrentRow.Item(filtVal.FieldName) Is DBNull.Value Then
                                                                valsMatch = True
                                                                Exit For

                                                            ElseIf vl = "[NOTNULL]" And m_CurrentRow.Item(filtVal.FieldName) IsNot Nothing Then
                                                                valsMatch = True
                                                                Exit For
                                                            ElseIf vl = "[NOTNULL]" And m_CurrentRow.Item(filtVal.FieldName) IsNot DBNull.Value Then
                                                                valsMatch = True
                                                                Exit For
                                                            Else
                                                                valsMatch = False



                                                            End If

                                                        Next

                                                        'Next
                                                    Next
                                                End If

                                                ' End If

                                            Next

                                        End If

                                    End If
                                End If
                            End If

                            For Each pnlCnt As Control In cnt.Controls

                                'If TypeOf pnlCnt Is TextBox Then
                                'Else
                                If TypeOf pnlCnt Is Button Then

                                    Dim controls() As Control = CType(CType(pnlCnt, Button).Parent, Panel).Controls.Find("txtAtt" & Replace(CType(pnlCnt, Button).Name, "btnHyper", ""), False)

                                    If controls.Length = 1 Then
                                        controls(0).Width = controls(0).Width - pnlCnt.Width - 5
                                        pnlCnt.Left = controls(0).Width + controls(0).Left + 5

                                    End If
                                ElseIf TypeOf pnlCnt Is CustomPanel Then
                                    pnlCnt.Width = cnt.Width - pRightPadding - pLeftPadding
                                    If pnlCnt.Controls.Count = 2 Then
                                        pnlCnt.Controls(0).Left = pLeftPadding
                                        pnlCnt.Controls(1).Left = CInt((pnlCnt.Width / 2))
                                    End If

                                Else
                                    pnlCnt.Width = m_TabControl.Width - pLeftPadding - pRightPadding
                                End If

                                'End If


                            Next

                        End If

                        pCurTabPage.Controls.Add(cnt)
                        'pCntlNextTop = pCntlNextTop + cnt.Height + pTopPadding
                        'If valsMatch Then
                        '    cnt.Visible = visToSet
                        'Else
                        '    cnt.Visible = Not visToSet

                        'End If
                        'If cnt.Visible Then
                        '    pCntlNextTop = pCntlNextTop + cnt.Height + pTopPadding
                        'End If
                        If valsMatch Then
                            cnt.Visible = visToSet
                        Else
                            cnt.Visible = Not visToSet


                        End If
                        If cnt.Visible Then
                            pCntlNextTop = pCntlNextTop + cnt.Height + pTopPadding
                        End If
                        If pCntlNextTop >= m_TabControl.Height - pBottomPadding Then

                            If pCurTabPage IsNot Nothing Then





                                If pTbPageCo Is Nothing Then
                                    ReDim Preserve pTbPageCo(0)
                                Else
                                    ReDim Preserve pTbPageCo(pTbPageCo.Length)
                                End If
                                pTbPageCo(pTbPageCo.Length - 1) = pCurTabPage
                                pCurTabPage = New TabPage


                                strName = GlobalsFunctions.appConfig.IDPanel.UIComponents.PageName & ":" & idxPage
                                idxPage = idxPage + 1
                                pCurTabPage.Name = strName
                                pCurTabPage.Text = strName

                                pCntlNextTop = pTopPadding
                                'pBtn = Nothing
                            End If
                        End If
                    End If
                End While
            End If
        Next



        If pCurTabPage IsNot Nothing Then


            If pCurTabPage.Controls.Count > 0 Then




                If pTbPageCo Is Nothing Then
                    ReDim Preserve pTbPageCo(0)
                Else
                    ReDim Preserve pTbPageCo(pTbPageCo.Length)
                End If

                pTbPageCo(pTbPageCo.Length - 1) = pCurTabPage
            Else

            End If
        End If

        m_TabControl.TabPages.Clear()
        If pTbPageCo IsNot Nothing Then
            For Each tbp As TabPage In pTbPageCo

                m_TabControl.TabPages.Add(tbp)

                tbp.Visible = True

                tbp.Update()
            Next
        End If
        If pTabAtt IsNot Nothing Then
            m_TabControl.TabPages.Add(pTabAtt)


            pTabAtt.Visible = True
            pTabAtt.Update()

        End If
        If pTbPageCoPic IsNot Nothing Then

            For Each tbp As TabPage In pTbPageCoPic

                Dim bPgVis As Boolean = True



                Dim valFnd As Boolean = False
                If tbp.Tag IsNot Nothing Then
                    If TypeOf (tbp.Tag) Is MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOption And m_CurrentRow IsNot Nothing Then
                        Dim fieldDet As MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOption = tbp.Tag
                        If fieldDet.FilterFields IsNot Nothing Then
                            If fieldDet.FilterFields.Count > 0 Then
                                For Each filtVals As MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOptionFilterFields In fieldDet.FilterFields
                                    For Each filtVal As MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOptionFilterFieldsFilterInfo In filtVals.FilterInfo
                                        If valFnd Then
                                            Exit For

                                        End If
                                        ' For Each filtInfo As MobileConfigClass.MobileConfigMobileMapConfigLayerOptionsLayersFieldOptionsLayerNameFieldOptionFilterFieldsFilterInfo In filtVal.FilterInfo
                                        For Each vl As String In filtVal.FieldValue
                                            If m_CurrentRow.Item(filtVal.FieldName).ToString() = vl Then

                                                valFnd = True

                                                bPgVis = False
                                                Exit For
                                            ElseIf vl = "[ISNULL]" And m_CurrentRow.Item(filtVal.FieldName) Is Nothing Then
                                                valFnd = True

                                                bPgVis = False
                                                Exit For
                                            ElseIf vl = "[ISNULL]" And m_CurrentRow.Item(filtVal.FieldName) Is DBNull.Value Then
                                                valFnd = True

                                                bPgVis = False
                                                Exit For
                                            ElseIf vl = "[NOTNULL]" And m_CurrentRow.Item(filtVal.FieldName) IsNot Nothing Then
                                                valFnd = True

                                                bPgVis = False
                                                Exit For
                                            ElseIf vl = "[NOTNULL]" And m_CurrentRow.Item(filtVal.FieldName) IsNot DBNull.Value Then
                                                valFnd = True

                                                bPgVis = False
                                                Exit For
                                            Else
                                                bPgVis = True


                                            End If
                                        Next


                                        'Next
                                    Next
                                Next

                            End If

                        End If
                    End If
                End If
                If (bPgVis) Then
                    m_TabControl.TabPages.Add(tbp)


                    tbp.Visible = True
                    tbp.Update()
                Else
                    tbp.Visible = False
                End If

            Next
        End If

        If pCurTabIdx = -1 Then
            If m_TabControl.TabPages.Count > 0 Then
                m_TabControl.SelectedIndex = 0

            End If

        Else
            If m_TabControl.TabPages.Count >= pCurTabIdx Then
                m_TabControl.SelectedIndex = pCurTabIdx
            Else
                m_TabControl.SelectedIndex = m_TabControl.TabPages.Count - 1
            End If


        End If
        pTbPageCo = Nothing
        pCurTabPage = Nothing
    End Sub

#End Region
#Region "Events"

    Private Sub relocateButtons()
        Dim visCnt As Integer = 0

        For Each Control As Control In spltAttTools.Panel2.Controls
            If TypeOf (Control) Is Button Then
                If Control.Visible Then
                    visCnt = visCnt + 1
                End If


            End If

        Next
        Dim spacing As Double = (spltAttTools.Width) / (visCnt + 1)
        Dim curloc As Double = spacing
        For Each Control As Control In spltAttTools.Panel2.Controls
            If TypeOf (Control) Is Button Then
                If Control.Visible Then
                    Control.Left = curloc - (Control.Width / 2)

                    curloc = curloc + spacing
                End If


            End If

        Next

        'btnZoomTo.Left = 10
        'btnFlash.Left = btnFlash.Parent.Width - btnFlash.Width - 10

        'If btnGPSLoc.Visible And btnRouteTo.Visible Then
        '    btnGPSLoc.Left = btnGPSLoc.Parent.Width / 2 - btnGPSLoc.Width - 5
        '    btnRouteTo.Left = btnRouteTo.Parent.Width / 2 + 5

        'ElseIf btnGPSLoc.Visible Then
        '    btnGPSLoc.Left = btnGPSLoc.Parent.Width / 2 - btnGPSLoc.Width / 2
        'ElseIf btnRouteTo.Visible Then
        '    btnRouteTo.Left = btnRouteTo.Parent.Width / 2 - btnRouteTo.Width / 2
        'End If


    End Sub
    Private Sub pBtnFlashClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnZoomTo.Click, btnRouteTo.Click, btnGPSLoc.Click, btnFlash.Click, btnWaypoint.Click
        If CType(sender, Button).Name.Contains("btnGPSLoc") Then
            m_GPSVal = Nothing

            RaiseEvent CheckGPS()
            Return

            'If m_GPSVal IsNot Nothing Then


            '    IdentifyLocation(m_Map.SpatialReference.FromGps(m_GPSVal.Coordinate))

            'End If
        End If

        'called by the zoom or flash button
        If m_CurrentRow Is Nothing Then Return
        If m_CurrentRow.Geometry Is Nothing Then Return
        If m_CurrentRow.Geometry.IsEmpty Then Return
        Dim pGeo As IGeometry = m_CurrentRow.Geometry.Clone


        If CType(sender, Button).Name.Contains("btnFlash") Then
            If pGeo.GeometryType = GeometryType.Point Then
                GlobalsFunctions.flashGeo(pGeo, m_Map, m_pen, m_brush)
            Else
                GlobalsFunctions.flashGeo(pGeo, m_Map, m_penLine, m_brush)
            End If


        ElseIf CType(sender, Button).Name.Contains("btnRouteTo") Then
            RaiseEvent RouteTo(m_CurrentRow.Geometry, m_CurrentRow(m_CurrentRow.FeatureSource.DisplayColumnIndex).ToString)
        ElseIf CType(sender, Button).Name.Contains("btnWaypoint") Then
            RaiseEvent Waypoint(m_CurrentRow.Geometry, m_CurrentRow(m_CurrentRow.FeatureSource.DisplayColumnIndex).ToString)

        Else

            GlobalsFunctions.zoomTo(pGeo, m_Map)
            If pGeo.GeometryType = GeometryType.Point Then
                GlobalsFunctions.flashGeo(pGeo, m_Map, m_pen, m_brush)
            Else
                GlobalsFunctions.flashGeo(pGeo, m_Map, m_penLine, m_brush)
            End If
        End If


    End Sub
    Private Sub btnHyper_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Try

            'Opens the linked doc in the default program
            Dim pCnt As Control = CType(sender, Control)
            Dim pHyperString As String
            'Checks to see if the link is in the text or tag
            'If TypeOf pCnt Is TextBox Then
            '    pHyperString = pCnt.Text
            'Else
            pHyperString = pCnt.Tag.ToString
            ' End If
            'Make sure there is no custom linking in the hyperlink
            If pHyperString.IndexOf("|") > 0 Then

                pHyperString = Trim(pHyperString.Substring(pHyperString.IndexOf("|") + 1))

            End If
            RaiseEvent HyperClick(pHyperString)



        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try


    End Sub
    Private Sub m_UCAtt_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize

        'Readds the controls based on the current size
        ShuffleControls()


        relocateButtons()
    End Sub

#End Region



    Private Sub btnGPSLoc_EnabledChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGPSLoc.EnabledChanged
        If btnGPSLoc.Enabled Then
            btnGPSLoc.BackgroundImage = My.Resources.SatImageBlue

        Else
            btnGPSLoc.BackgroundImage = My.Resources.SatImageGraypng

        End If
    End Sub
    Private Sub btnWaypoint_EnabledChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnWaypoint.EnabledChanged
        If btnWaypoint.Enabled Then
            btnWaypoint.BackgroundImage = My.Resources.NavTooBlue

        Else
            btnWaypoint.BackgroundImage = My.Resources.NavTooGray

        End If
    End Sub
    Private Sub spltAttTools_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles spltAttTools.Resize
        If spltAttTools.Height > 52 Then
            spltAttTools.SplitterDistance = spltAttTools.Height - 51
        End If


    End Sub

    Private Sub attlistclick(sender As Object, e As MouseEventArgs)
        'MsgBox(CType(CType(sender, ListBox).SelectedItem, Attachment).FilePath)
        Process.Start(CType(CType(sender, ListBox).SelectedItem, Attachment).FilePath)
    End Sub

    Private Sub attdraw(sender As Object, e As System.Windows.Forms.DrawItemEventArgs)

        ' Draw the background of the ListBox control for each item.
        ' e.DrawBackground()
        ' Define the default color of the brush as black.
        Dim myBrush As Brush = Brushes.Black

        '' Determine the color of the brush to draw each item based  
        '' on the index of the item to draw. 
        Select Case e.Index Mod 3


            Case 0
                myBrush = Brushes.Red

            Case 1
                myBrush = Brushes.Blue

            Case 2
                myBrush = Brushes.Black

        End Select


        ' Draw the current item text based on the current Font  
        ' and the custom brush settings.
        e.Graphics.DrawString(CType(sender.Items(e.Index), Attachment).Name.ToString(), e.Font, myBrush, e.Bounds, StringFormat.GenericDefault)

        ' If the ListBox has focus, draw a focus rectangle around the selected item.
        e.DrawFocusRectangle()

    End Sub
    Public Event showEditor()
    Private Sub btnEdit_Click(sender As System.Object, e As System.EventArgs) Handles btnEdit.Click
        RaiseEvent showEditor()
        m_Map.Invalidate()

    End Sub
End Class
