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


Imports Esri.ArcGIS.Mobile
Imports Esri.ArcGIS.Mobile.WinForms
'Imports Esri.ArcGIS.Mobile.Geometries

Imports Esri.ArcGIS.Mobile.FeatureCaching
Imports Esri.ArcGIS.Mobile.SpatialReferences
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Math
Imports System.Threading
Imports Esri.ArcGISTemplates


Public Class mobileNavigation
    Private m_gpsD As GPSLocationDetails
    Public Event RaiseStatMessage(message As String, hide As Boolean)
    Public Event GPSComplete(gpsDet As GPSLocationDetails)
    Private m_LastExtents As IList(Of Esri.ArcGIS.Mobile.Geometries.Envelope)
    Private m_CurrentStep As Integer

    Private m_picBox As PictureBox

    Private m_BkMark As ComboBox
    Private m_BkAddBtn As Button
    Private m_BkDelBtn As Button
    Private m_BkEditBtn As CheckBox


    Private WithEvents m_Map As Esri.ArcGIS.Mobile.WinForms.Map
    Private m_BackColor As Color = Drawing.Color.DarkBlue
    Private m_BackColorBK As System.Drawing.Color = Drawing.Color.LightGray

    Private m_BarWidth As Integer = 20
    Private m_BarBorderStyle As BorderStyle = Windows.Forms.BorderStyle.None
    Private m_LeftOffset As Integer = 0
    Private m_RightOffset As Integer = 0
    Private m_TopOffset As Integer = 0
    Private m_BottomOffset As Integer = 0
    Private m_EnableMouseWheelZoom As Boolean = True
    'Private WithEvents m_GPS As Gps.GpsDisplay
    Private WithEvents m_FileGPS As Gps.FileGpsConnection
    Private WithEvents m_SerialGPS As Gps.SerialPortGpsConnection


    Private m_PenArrow As Pen = New Pen(Drawing.Color.Yellow)
    Private m_BrushArrow As Brush = New SolidBrush(Drawing.Color.Yellow)
    Private m_ZoomTimer As System.Windows.Forms.Timer
    Private m_bZoomOut As Boolean = False

    Private m_DrawScale As Boolean = True
    Private m_scaleFont As Font = New System.Drawing.Font("Tahoma", 14.0F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)
    Private m_scaleFontLarge As Font = New System.Drawing.Font("Tahoma", 16.0F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)
    Private m_offset As Integer = 1
    Private m_ScaleX As Integer
    Private m_ScaleY As Integer
    Private m_NorthArrowX As Integer
    Private m_NorthArrowY As Integer
    Private m_ScaleBrush As Brush = Brushes.White
    Private m_ScaleBrushFront As Brush = Brushes.Black
    Private WithEvents m_PanMA As panExt
    Private WithEvents m_ZoomInOutMA As Esri.ArcGIS.Mobile.WinForms.ZoomInOutMapAction
    Private WithEvents m_MeasureMA As MobileControls.MeasureMapAction
    Private WithEvents m_ZoomInMA As Esri.ArcGIS.Mobile.WinForms.ZoomInMapAction
    Private WithEvents m_ZoomOutMA As Esri.ArcGIS.Mobile.WinForms.ZoomOutMapAction
    Private m_PanBtn As Button
    Private m_GPSBtn As Button
    Private m_MeasureBtn As Button
    Private m_BookmarkBtn As Button

    Private WithEvents m_BookmarkForm As CustomPanel


    Private m_ZoomFullBtn As Button
    Private m_ZoomInOutBtn As Button
    Private m_ZoomInBtn As Button

    Private m_RotRightBtn As Button
    Private m_RotLeftBtn As Button

    Private m_ZoomOutBtn As Button
    Private m_ZoomPrevBtn As Button
    Private m_ZoomNextBtn As Button
    Public Event GPSStarted()
    Public Event GPSStopped()

    Private Event GPSTimeout()
    Private m_GPSLoadingPic As PictureBox

    Private m_MaxScale As Integer = 0
    Dim m_t As Thread
    Private m_LogGPS As Boolean = False
    Private m_GPSFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = Nothing
    Private m_GPSFL_UserField As String = Nothing
    Private m_GPSFL_DateField As String = Nothing

    Private m_LogInterval As Integer = 60
    Private m_GPSTimer As System.Threading.Timer
    'Private m_GPSTimer As System.Windows.Forms.Timer
    '   Private m_invokeCount As Integer = 0
    Public Event ToolChange(ByVal toolType As ToolType)
    Private m_FixZoom As Boolean = True
    Private WithEvents m_GPSAvgTool As Esri.ArcGIS.Mobile.Gps.GpsAveragingTool

#Region "Properties"
    Public Property setTopOffset() As Integer
        Get
            Return m_TopOffset
        End Get
        Set(ByVal value As Integer)
            m_TopOffset = value

        End Set
    End Property
    Public Property setRightOffset() As Integer
        Get
            Return m_RightOffset
        End Get
        Set(ByVal value As Integer)
            m_RightOffset = value

        End Set
    End Property
    Public Property setLeftOffset() As Integer
        Get
            Return m_LeftOffset
        End Get
        Set(ByVal value As Integer)
            m_LeftOffset = value

        End Set
    End Property
    Public Property setBottomOffset() As Integer
        Get
            Return m_BottomOffset
        End Get
        Set(ByVal value As Integer)
            m_BottomOffset = value

        End Set
    End Property
    Public WriteOnly Property MobileMap() As Esri.ArcGIS.Mobile.WinForms.Map
        Set(ByVal value As Esri.ArcGIS.Mobile.WinForms.Map)
            m_Map = value
        End Set
    End Property
    Public Property BackColor() As Color
        Get
            Return m_BackColor
        End Get
        Set(ByVal value As Color)
            m_BackColor = value
        End Set
    End Property
    Public Property borderStyle() As BorderStyle
        Get
            Return m_BarBorderStyle
        End Get
        Set(ByVal value As BorderStyle)
            m_BarBorderStyle = value
        End Set
    End Property
    Public Property BarWidth() As Integer
        Get
            Return m_BarWidth
        End Get
        Set(ByVal value As Integer)
            m_BarWidth = value

        End Set
    End Property
    Public Property EnableMouseWheelZoom() As Boolean
        Get
            Return m_EnableMouseWheelZoom
        End Get
        Set(ByVal value As Boolean)
            m_EnableMouseWheelZoom = value
        End Set
    End Property
    Public Property ScaleBarColor() As Brush
        Get
            Return m_ScaleBrush
        End Get
        Set(ByVal value As Brush)
            m_ScaleBrush = value
        End Set
    End Property
    Public Property ScaleBarFont() As Font
        Get
            Return m_scaleFont
        End Get
        Set(ByVal value As Font)
            m_scaleFont = value
        End Set
    End Property
    Public WriteOnly Property FileGPSFile() As String

        Set(ByVal value As String)

            Dim bReOpen As Boolean = False
            'Checks to see if the file GPS is open, meaning it is active
            If m_FileGPS.IsOpen Then
                'Close the GPS
                m_FileGPS.Close()
                'Store a variable to determine if it should be reopened
                bReOpen = True
            End If
            m_FileGPS.FileName = value
            'Reopen the GPS if it was open before
            If bReOpen Then
                m_FileGPS.Open()
            End If
        End Set
    End Property
#End Region
#Region "PublicFunctions"
    Public Enum ToolType
        ZoomIn
        ZoomOut
        ZoomInOut
        Pan
        none
        Measure
    End Enum
    Public Sub New(ByRef mobileMap As Esri.ArcGIS.Mobile.WinForms.Map, ByVal bShowBar As Boolean, ByVal bShowButtons As Boolean, ByVal drawScale As Boolean)

        Try


            If mobileMap Is Nothing Then Return

            m_LastExtents = New List(Of Esri.ArcGIS.Mobile.Geometries.Envelope)
            m_CurrentStep = 0
            'Assign map to global variable
            m_Map = mobileMap
            'apply the brush to the pen
            m_PenArrow.Brush = m_BrushArrow
            If bShowBar Then
                'Generate the navigation controls
                AddNavigationBar()
            End If
            If bShowButtons Then
                'Add navigaiton buttons
                AddNavButtons()
            End If

            m_DrawScale = drawScale

            'Size them based on the map size
            Resize()
            'Create the pan and the zoom in out map action
            m_PanMA = New panExt 'New Esri.ArcGIS.Mobile.WinForms.MapActions.PanMapAction
            m_ZoomInOutMA = New Esri.ArcGIS.Mobile.WinForms.ZoomInOutMapAction
            m_ZoomInMA = New Esri.ArcGIS.Mobile.WinForms.ZoomInMapAction
            m_ZoomOutMA = New Esri.ArcGIS.Mobile.WinForms.ZoomOutMapAction

            m_Map.MapAction = m_PanMA

            Dim key As String = ""

            key = GlobalsFunctions.appConfig.MeasureOptions.Visible
            If key.ToUpper() = "TRUE" Then
                m_MeasureMA = New MobileControls.MeasureMapAction
                key = GlobalsFunctions.appConfig.MeasureOptions.MeasureMethod
                Select Case UCase(key)
                    Case "FREEHAND"
                        m_MeasureMA.MeasureMethod = MeasureMapAction.EsriMeasureMethod.Freehand
                    Case "TWOPOINT"
                        m_MeasureMA.MeasureMethod = MeasureMapAction.EsriMeasureMethod.TwoPoint
                    Case "MULTIPOINT"
                        m_MeasureMA.MeasureMethod = MeasureMapAction.EsriMeasureMethod.MultiPoint

                    Case Else
                End Select

                m_MeasureMA.Font = m_scaleFontLarge


                key = GlobalsFunctions.appConfig.MeasureOptions.MeasureFontColor
                m_MeasureMA.FontColor = System.Drawing.Color.Red
                If key IsNot Nothing Then
                    ' If IsNumeric(key) 





                    m_MeasureMA.FontColor = System.Drawing.Color.FromArgb(Int32.Parse(key.Replace("#", ""), Globalization.NumberStyles.HexNumber))

                    'System.Drawing.Color.FromArgb(CInt(key))

                    'End If

                End If


                key = GlobalsFunctions.appConfig.MeasureOptions.MeasureUnit
                Select Case UCase(key)
                    Case "FOOT"
                        m_MeasureMA.DisplayedUnit = Unit.Foot
                    Case "FEET"
                        m_MeasureMA.DisplayedUnit = Unit.Foot
                    Case "METER"
                        m_MeasureMA.DisplayedUnit = Unit.Meter
                    Case "KILOMETER"
                        m_MeasureMA.DisplayedUnit = Unit.Kilometer
                    Case "MILE"
                        m_MeasureMA.DisplayedUnit = Unit.Mile
                    Case "INCH"
                        m_MeasureMA.DisplayedUnit = Unit.Inch
                    Case "YARD"
                        m_MeasureMA.DisplayedUnit = Unit.Yard
                    Case "NAUTICALMILE"
                        m_MeasureMA.DisplayedUnit = Unit.NauticalMile
                    Case "MAP"
                        m_MeasureMA.DisplayedUnit = m_Map.SpatialReference.Unit
                    Case Else
                        m_MeasureMA.DisplayedUnit = m_Map.SpatialReference.Unit
                End Select

                key = GlobalsFunctions.appConfig.MeasureOptions.MeasureLineColor
                m_MeasureMA.LineColor = System.Drawing.Color.Red
                If key IsNot Nothing Then
                    ' If IsNumeric("&H" & key) Then
                    ' m_MeasureMA.LineColor = System.Drawing.Color.FromArgb(

                    m_MeasureMA.LineColor = System.Drawing.Color.FromArgb(Int32.Parse(key.Replace("#", ""), Globalization.NumberStyles.HexNumber))
                    'End If

                End If


                key = GlobalsFunctions.appConfig.MeasureOptions.MeasureWidth
                m_MeasureMA.LineWidth = 6
                If key IsNot Nothing Then
                    If IsNumeric(key) Then
                        m_MeasureMA.LineWidth = CInt(key)
                    End If

                End If
                key = GlobalsFunctions.appConfig.MeasureOptions.MeasureSegmentMeasures

                If UCase(key) = "TRUE" Then
                    m_MeasureMA.ShowSegmentMeasures = True
                Else
                    m_MeasureMA.ShowSegmentMeasures = False
                End If

                key = GlobalsFunctions.appConfig.MeasureOptions.MeasureSigDigits
                m_MeasureMA.SignificantDigits = 2
                If key IsNot Nothing Then
                    If IsNumeric(key) Then
                        m_MeasureMA.SignificantDigits = CInt(key)
                    End If

                End If


            End If




            'initialize the GPS display
            GlobalsFunctions.m_GPS = New Gps.GpsDisplay

            AddHandler GlobalsFunctions.m_GPS.Disposed, AddressOf m_GPS_Disposed

            'Asign the map
            GlobalsFunctions.m_GPS.Map = mobileMap
            'GlobalsFunctions.m_GPS.AutoPan = True

            'Initilize both GPS types
            m_FileGPS = New Gps.FileGpsConnection
            m_SerialGPS = New Gps.SerialPortGpsConnection
            'Set the defaults for the serial
            ' m_SerialGPS.BaudRate = Gps.GpsBaudRate.BaudRate9600
            m_SerialGPS.PortName = "AUTO"

            'Set the GPS to serial by default
            GlobalsFunctions.m_GPS.GpsConnection = m_SerialGPS





            'Determine the Max Exetent
            MaxExtent()
            m_picBox = New PictureBox
            m_picBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            m_picBox.Left = 45
            m_picBox.Visible = False
            m_picBox.BackColor = Drawing.Color.Transparent
            m_picBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
            m_picBox.Top = m_Map.Height - 120
            m_Map.Controls.Add(m_picBox)
            'm_FixZoom = 
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
    'Private Shared Sub TimerEventProcessor(ByVal sender As Object, ByVal e As System.EventArgs)
    '    Console.WriteLine("{0} Checking status {1,2}.", _
    '    DateTime.Now.ToString("h:mm:ss.fff"))


    'End Sub
    Private Delegate Sub DelegateLogGPS()
    Private WithEvents gpsDataWorker As New System.ComponentModel.BackgroundWorker
    Public Sub truncateLog()
        If m_GPSFL IsNot Nothing Then
            If GlobalsFunctions.appConfig.NavigationOptions.GPS.GPSLogLayer_TruncateAfterPost.ToString().ToUpper() = "TRUE" Then
                m_GPSFL.RemoveFeatures(m_Map.FullExtent)

            End If
            
        End If

    End Sub
    Public Sub getGPSAsync()
        gpsDataWorker.WorkerReportsProgress = True

        gpsDataWorker.RunWorkerAsync()
    End Sub

    Private Sub gpsDataWorker_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles gpsDataWorker.DoWork
        m_gpsD = getGPS()


    End Sub

    Private Sub gpsDataWorker_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles gpsDataWorker.ProgressChanged

    End Sub

    Private Sub gpsDataWorker_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles gpsDataWorker.RunWorkerCompleted
        RaiseEvent GPSComplete(m_gpsD)
    End Sub



    Private Function getGPS() As GPSLocationDetails

        If False Then
            getGPSMobileVersion()
        Else
            Return getGPSMine()

        End If
    End Function
    Private Function getGPSMobileVersion()
        If GlobalsFunctions.m_GPS.GpsConnection IsNot Nothing Then

            If GlobalsFunctions.m_GPS.GpsConnection.IsOpen Then
                Dim pt As Esri.ArcGIS.Mobile.Geometries.Point


                pt = New Esri.ArcGIS.Mobile.Geometries.Point()
                Dim quality As Esri.ArcGIS.Mobile.Gps.GpsQualityFilter = New Esri.ArcGIS.Mobile.Gps.GpsQualityFilter

                If IsNumeric(GlobalsFunctions.appConfig.NavigationOptions.GPS.GPSPDOP) Then
                    quality.MaximumPdop = CDbl(GlobalsFunctions.appConfig.NavigationOptions.GPS.GPSPDOP)
                End If

                quality.FixStatus = GlobalsFunctions.GPSTextToFix(GlobalsFunctions.appConfig.NavigationOptions.GPS.GPSFixType)

                m_GPSAvgTool = New Esri.ArcGIS.Mobile.Gps.GpsAveragingTool(quality, GlobalsFunctions.m_GPS.GpsConnection, pt, m_Map.SpatialReference)
                'm_GPSAvgTool.
                m_GPSAvgTool.Start()


            End If
        End If
        Return Nothing
    End Function
    Private Function getGPSMine() As GPSLocationDetails
        If GlobalsFunctions.m_GPS.GpsConnection IsNot Nothing Then

            Dim intMax As Integer = GlobalsFunctions.appConfig.NavigationOptions.GPS.GPSPointMaxTries
            Dim numLoops As Integer = 0
            Dim gpsD As GPSLocationDetails = Nothing

            If GlobalsFunctions.m_GPS.GpsConnection.IsOpen Then
                Dim gpsDList As List(Of GPSLocationDetails) = New List(Of GPSLocationDetails)()
                While True
                    If numLoops > intMax Then
                        Exit While

                    End If
                    If gpsDList.Count > CInt(GlobalsFunctions.appConfig.NavigationOptions.GPS.GPSPointAvg) - 1 Then
                        Exit While

                    End If
                    'Dim pgs As ESRI.ArcGIS.Mobile.Gps.GpsConstructionTool = New ESRI.ArcGIS.Mobile.Gps.GpsConstructionTool


                    ' If GlobalsFunctions.m_GPS.GpsConnection.FixStatus = GlobalsFunctions.GPSTextToFix(GlobalsFunctions.appConfig.NavigationOptions.GPS.GPSFixType) And _
                    If ((Double.IsNaN(GlobalsFunctions.m_GPS.GpsConnection.PositionDilutionOfPrecision)) Or CDbl(GlobalsFunctions.m_GPS.GpsConnection.PositionDilutionOfPrecision) <= CDbl(GlobalsFunctions.appConfig.NavigationOptions.GPS.GPSPDOP)) Then

                        ' Dim pNewCoord As Esri.ArcGIS.Mobile.Geometries.Coordinate = New Esri.ArcGIS.Mobile.Geometries.Coordinate(GlobalsFunctions.m_GPS.GpsConnection.Longitude, GlobalsFunctions.GlobalsFunctions.m_GPS.GpsConnection.Latitude)
                        gpsD = New GPSLocationDetails()
                        gpsD.Altitude = GlobalsFunctions.m_GPS.GpsConnection.Altitude
                        gpsD.Course = GlobalsFunctions.m_GPS.GpsConnection.Course
                        gpsD.CourseMagnetic = GlobalsFunctions.m_GPS.GpsConnection.CourseMagnetic
                        gpsD.DateTime = GlobalsFunctions.m_GPS.GpsConnection.DateTime
                        gpsD.FixSatelliteCount = GlobalsFunctions.m_GPS.GpsConnection.FixSatelliteCount
                        gpsD.FixStatus = GlobalsFunctions.m_GPS.GpsConnection.FixStatus

                        gpsD.GeoidHeight = GlobalsFunctions.m_GPS.GpsConnection.GeoidHeight
                        gpsD.HorizontalDilutionOfPrecision = GlobalsFunctions.m_GPS.GpsConnection.HorizontalDilutionOfPrecision
                        gpsD.Latitude = GlobalsFunctions.m_GPS.GpsConnection.Latitude
                        ' gpsD.LatitudeToDegreeMinutesSeconds = GlobalsFunctions.m_GPS.GpsConnection.LatitudeToDegreeMinutesSeconds
                        gpsD.Longitude = GlobalsFunctions.m_GPS.GpsConnection.Longitude
                        ' gpsD.LongitudeToDegreeMinutesSeconds = GlobalsFunctions.m_GPS.GpsConnection.LongitudeToDegreeMinutesSeconds
                        gpsD.PositionDilutionOfPrecision = GlobalsFunctions.m_GPS.GpsConnection.PositionDilutionOfPrecision
                        gpsD.SpatialReference = GlobalsFunctions.m_GPS.GpsConnection.SpatialReference
                        gpsD.Speed = GlobalsFunctions.m_GPS.GpsConnection.Speed
                        gpsD.VerticalDilutionOfPrecision = GlobalsFunctions.m_GPS.GpsConnection.VerticalDilutionOfPrecision

                        gpsDList.Add(gpsD)
                        RaiseEvent RaiseStatMessage(GlobalsFunctions.GPSFixToText(gpsD.FixStatus) & " " & gpsDList.Count & " " & GlobalsFunctions.appConfig.NavigationOptions.UIComponents.PointsAvgText & " " & numLoops & "/" & intMax, False)


                    Else
                        RaiseEvent RaiseStatMessage(GlobalsFunctions.GPSFixToText(GlobalsFunctions.m_GPS.GpsConnection.FixStatus) & " " & numLoops & "/" & intMax, False)

                    End If

                    Thread.Sleep(CInt(GlobalsFunctions.appConfig.NavigationOptions.GPS.GPSPointAvgWaitInterval))
                    numLoops = numLoops + 1

                End While
                RaiseEvent RaiseStatMessage(GlobalsFunctions.GPSFixToText(GlobalsFunctions.m_GPS.GpsConnection.FixStatus), False)


                Return avgGPSMine(gpsDList)

            End If
        End If
        Return Nothing

    End Function
    Private Function avgGPSMine(gpsList As List(Of GPSLocationDetails)) As GPSLocationDetails
        If gpsList Is Nothing Then Return Nothing
        If gpsList.Count = 0 Then Return Nothing

        Dim gpsD As GPSLocationDetails = New GPSLocationDetails
        Dim intT As Integer = 0

        For Each gpsEntry As GPSLocationDetails In gpsList

            If intT = 0 Then

                gpsD.DateTime = gpsEntry.DateTime
                gpsD.FixStatus = gpsEntry.FixStatus.ToString
                gpsD.SpatialReference = gpsEntry.SpatialReference
                intT = 1
            End If
            gpsD.Altitude = gpsD.Altitude + gpsEntry.Altitude
            gpsD.Course = gpsD.Course + gpsEntry.Course
            gpsD.CourseMagnetic = gpsD.CourseMagnetic + gpsEntry.CourseMagnetic
            ' gpsD.DateTime = gpsD.DateTime + gpsEntry.DateTime
            gpsD.FixSatelliteCount = gpsD.FixSatelliteCount + gpsEntry.FixSatelliteCount
            'gpsD.FixStatus = gpsD.FixStatus + gpsEntry.FixStatus.ToString

            gpsD.GeoidHeight = gpsD.GeoidHeight + gpsEntry.GeoidHeight
            gpsD.HorizontalDilutionOfPrecision = gpsD.HorizontalDilutionOfPrecision + gpsEntry.HorizontalDilutionOfPrecision
            gpsD.Latitude = gpsD.Latitude + gpsEntry.Latitude
            ' gpsD.LatitudeToDegreeMinutesSeconds = gpsD.LatitudeToDegreeMinutesSeconds + gpsEntry.LatitudeToDegreeMinutesSeconds
            gpsD.Longitude = gpsD.Longitude + gpsEntry.Longitude
            ' gpsD.LongitudeToDegreeMinutesSeconds = gpsD.LongitudeToDegreeMinutesSeconds + gpsEntry.LongitudeToDegreeMinutesSeconds
            gpsD.PositionDilutionOfPrecision = gpsD.PositionDilutionOfPrecision + gpsEntry.PositionDilutionOfPrecision
            ' gpsD.SpatialReference = gpsD.Altitude + gpsEntry.SpatialReference
            gpsD.Speed = gpsD.Speed + gpsEntry.Speed
            gpsD.VerticalDilutionOfPrecision = gpsD.VerticalDilutionOfPrecision + gpsEntry.VerticalDilutionOfPrecision


        Next
        gpsD.Altitude = gpsD.Altitude / gpsList.Count
        gpsD.Course = gpsD.Course / gpsList.Count
        gpsD.CourseMagnetic = gpsD.CourseMagnetic / gpsList.Count
        ' gpsD.DateTime = gpsD.DateTime + gpsEntry.DateTime
        gpsD.FixSatelliteCount = gpsD.FixSatelliteCount / gpsList.Count
        'gpsD.FixStatus = gpsD.FixStatus + gpsEntry.FixStatus.ToString

        gpsD.GeoidHeight = gpsD.GeoidHeight / gpsList.Count
        gpsD.HorizontalDilutionOfPrecision = gpsD.HorizontalDilutionOfPrecision / gpsList.Count
        gpsD.Latitude = gpsD.Latitude / gpsList.Count
        'gpsD.LatitudeToDegreeMinutesSeconds = gpsD.LatitudeToDegreeMinutesSeconds / gpsList.Count
        gpsD.Longitude = gpsD.Longitude / gpsList.Count
        ' gpsD.LongitudeToDegreeMinutesSeconds = gpsD.LongitudeToDegreeMinutesSeconds / gpsList.Count
        gpsD.PositionDilutionOfPrecision = gpsD.PositionDilutionOfPrecision / gpsList.Count
        ' gpsD.SpatialReference = gpsD.Altitude + gpsEntry.SpatialReference
        gpsD.Speed = gpsD.Speed / gpsList.Count
        gpsD.VerticalDilutionOfPrecision = gpsD.VerticalDilutionOfPrecision / gpsList.Count
        Return gpsD

    End Function
    Private Sub LogGPS()
        Try

            If m_GPSFL IsNot Nothing Then


                If GlobalsFunctions.m_GPS.GpsConnection.Longitude.ToString() <> "NaN" And GlobalsFunctions.m_GPS.GpsConnection.Latitude.ToString() <> "NaN" Then
                  
                    Dim pDT As FeatureDataTable = m_GPSFL.GetDataTable()
                    Dim pFDR As FeatureDataRow = pDT.NewRow
                    pFDR.Geometry = New Esri.ArcGIS.Mobile.Geometries.Point(m_Map.SpatialReference.FromWgs84(GlobalsFunctions.m_GPS.GpsConnection.Longitude, GlobalsFunctions.m_GPS.GpsConnection.Latitude))
                    If pDT.Columns(m_GPSFL_UserField) IsNot Nothing Then
                        'Environment.UserDomainName & "\\" & Environment.UserName
                        pFDR.Item(m_GPSFL_UserField) = Environment.UserName
                    End If
                    If pDT.Columns(m_GPSFL_DateField) IsNot Nothing Then
                        pFDR.Item(m_GPSFL_DateField) = Now.ToString()
                    End If


                    pDT.Rows.Add(pFDR)

             
                    m_GPSFL.SaveEdits(pDT)
                    pDT = Nothing
                    pFDR = Nothing

                End If

                '   Case "TIME".ToUpper
                'UpdateField(autoAttFld.Name, CStr(Now.ToShortTimeString), True, setRead)
                '                Case "Date".ToUpper
                'UpdateField(autoAttFld.Name, DateTime.Today.ToString("MM-dd-yyyy"), True, setRead)
                '                Case "CPUNAME".ToUpper
                'UpdateField(autoAttFld.Name, Environment.MachineName.ToString(), True, setRead)
                '                Case "DateTime".ToUpper

                'UpdateField(autoAttFld.Name, Now.ToString(), True, setRead)

                '                Case "User".ToUpper
                'UpdateField(autoAttFld.Name, Environment.UserName, True, setRead)
                '                Case "FullUser".ToUpper
                'UpdateField(autoAttFld.Name, Environment.UserDomainName & "\\" & Environment.UserName, True, setRead)
            End If

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
    Private Sub GPSLogger_Tick(ByVal stateInfo As Object)
        'Console.WriteLine("{0} Checking status {1,2}.", _
        '    DateTime.Now.ToString("h:mm:ss.fff"), _
        '    m_invokeCount.ToString())

        'Dim autoEvent As AutoResetEvent = _
        '    DirectCast(stateInfo, AutoResetEvent)
        'm_invokeCount += 1
        'Console.WriteLine("{0} Checking status {1,2}.", _
        '    DateTime.Now.ToString("h:mm:ss.fff"), _
        '    m_invokeCount.ToString())

        'If m_invokeCount = m_LogInterval Then

        '    ' Reset the counter and signal to stop the timer.
        '    m_invokeCount = 0
        '    autoEvent.Set()
        'End If\
        If m_LogGPS Then


            If m_Map IsNot Nothing Then

                If m_Map.TopLevelControl IsNot Nothing Then


                    If m_Map.TopLevelControl.InvokeRequired Then
                        m_Map.TopLevelControl.Invoke(New DelegateLogGPS(AddressOf LogGPS))

                    End If

                End If
            End If
        End If

    End Sub

    Public Sub Refresh()
        'Manual refresh control
        Resize()
        Color()

    End Sub
    Public Sub ToggleFixZoom()
        m_FixZoom = Not m_FixZoom

    End Sub
    Public Sub ToggleGPSDisplay()
        If m_GPSBtn IsNot Nothing Then
            m_GPSBtn.Visible = m_GPSBtn.Visible
        End If

    End Sub
    Public Function SetGPSType(ByVal File As Boolean, Optional ByVal FileName_Port As String = "AUTO", Optional ByVal baudRate As String = "4800") As Boolean
        'Sets up the GPS type

        'close the GPS if open
        If GlobalsFunctions.m_GPS.GpsConnection.IsOpen Then
            GlobalsFunctions.m_GPS.GpsConnection.Close()

        End If
        'Determine the gps type
        If File Then
            'Set the file used to simulate GPS
            If FileName_Port <> "AUTO" Then
                GlobalsFunctions.m_GPS.GpsConnection = m_FileGPS

                If System.IO.File.Exists(FileName_Port) Then
                    m_FileGPS.FileName = FileName_Port
                    m_FileGPS.Cycling = True
                    m_FileGPS.Enabled = True


                    m_FileGPS.ReadInterval = 125

                Else
                    'MsgBox("GPS Sim File does not exist")
                    Return False
                End If

            End If


        Else
            Try

            
            m_SerialGPS.PortName = FileName_Port
            Select Case baudRate
                Case "115200"
                    m_SerialGPS.BaudRate = Esri.ArcGIS.Mobile.Gps.GpsBaudRate.BaudRate115200
                Case "1200"
                    m_SerialGPS.BaudRate = Esri.ArcGIS.Mobile.Gps.GpsBaudRate.BaudRate1200
                Case "14400"
                    m_SerialGPS.BaudRate = Esri.ArcGIS.Mobile.Gps.GpsBaudRate.BaudRate14400
                Case "19200"
                    m_SerialGPS.BaudRate = Esri.ArcGIS.Mobile.Gps.GpsBaudRate.BaudRate19200
                Case "2400"
                    m_SerialGPS.BaudRate = Esri.ArcGIS.Mobile.Gps.GpsBaudRate.BaudRate2400
                Case "38400"
                    m_SerialGPS.BaudRate = Esri.ArcGIS.Mobile.Gps.GpsBaudRate.BaudRate38400
                Case "4800"
                    m_SerialGPS.BaudRate = Esri.ArcGIS.Mobile.Gps.GpsBaudRate.BaudRate4800
                Case "56000"
                    m_SerialGPS.BaudRate = Esri.ArcGIS.Mobile.Gps.GpsBaudRate.BaudRate56000
                Case "57600"
                    m_SerialGPS.BaudRate = Esri.ArcGIS.Mobile.Gps.GpsBaudRate.BaudRate57600
                Case "9600"
                    m_SerialGPS.BaudRate = Esri.ArcGIS.Mobile.Gps.GpsBaudRate.BaudRate9600
                Case Else
                    m_SerialGPS.BaudRate = Esri.ArcGIS.Mobile.Gps.GpsBaudRate.BaudRate4800


            End Select
            Catch ex As Exception
                MsgBox(GlobalsFunctions.appConfig.NavigationOptions.UIComponents.GPSNoConnect, MsgBoxStyle.Exclamation, "GPS Load")

            End Try
            GlobalsFunctions.m_GPS.GpsConnection = m_SerialGPS


        End If
        Return True

    End Function
    Public Sub ActivateGPS()
        'Turn on the GPS
        GlobalsFunctions.m_GPS.GpsConnection.Open()

    End Sub
    Public Sub DeactivateGPS()
        'Close the GPS
        GlobalsFunctions.m_GPS.GpsConnection.Close()

    End Sub
    Public Sub Dispose()

        If m_FileGPS IsNot Nothing Then
            If m_FileGPS.IsOpen Then
                m_FileGPS.Close()
            End If
            m_FileGPS.Dispose()
        End If
        m_FileGPS = Nothing


        If m_SerialGPS IsNot Nothing Then
            If m_SerialGPS.IsOpen Then
                m_SerialGPS.Close()
            End If
            m_SerialGPS.Dispose()
        End If
        m_SerialGPS = Nothing

        If GlobalsFunctions.m_GPS IsNot Nothing Then
            If GlobalsFunctions.m_GPS.GpsConnection IsNot Nothing Then
                GlobalsFunctions.m_GPS.GpsConnection.Close()
            End If
            GlobalsFunctions.m_GPS.Dispose()
        End If
        GlobalsFunctions.m_GPS = Nothing


        If m_PanMA IsNot Nothing Then

            m_PanMA.Dispose()
        End If
        m_PanMA = Nothing


        If m_ZoomInOutMA IsNot Nothing Then
            m_ZoomInOutMA.Dispose()
        End If
        m_ZoomInOutMA = Nothing

        If m_MeasureMA IsNot Nothing Then
            m_MeasureMA.Dispose()
        End If
        m_MeasureMA = Nothing

        If m_ZoomOutMA IsNot Nothing Then
            m_ZoomOutMA.Dispose()
        End If
        m_ZoomOutMA = Nothing

        If m_ZoomInMA IsNot Nothing Then
            m_ZoomInMA.Dispose()
        End If
        m_ZoomInMA = Nothing

        If m_Map IsNot Nothing Then
            ' m_Map.Dispose()
        End If
        m_Map = Nothing

        m_BackColor = Nothing
        m_BackColorBK = Nothing

        If m_PenArrow IsNot Nothing Then
            m_PenArrow.Dispose()
        End If
        m_PenArrow = Nothing

        If m_BrushArrow IsNot Nothing Then
            m_BrushArrow.Dispose()
        End If
        m_BrushArrow = Nothing

        If m_ZoomTimer IsNot Nothing Then
            m_ZoomTimer.Dispose()
        End If
        m_ZoomTimer = Nothing

        If m_scaleFont IsNot Nothing Then
            m_scaleFont.Dispose()
        End If
        m_scaleFont = Nothing

        If m_scaleFontLarge IsNot Nothing Then
            m_scaleFontLarge.Dispose()
        End If
        m_scaleFontLarge = Nothing


        If m_ScaleBrush IsNot Nothing Then
            m_ScaleBrush.Dispose()
        End If
        m_ScaleBrush = Nothing

        If m_PanBtn IsNot Nothing Then
            m_PanBtn.Dispose()
        End If
        m_PanBtn = Nothing

        If m_GPSBtn IsNot Nothing Then
            m_GPSBtn.Dispose()
        End If
        m_GPSBtn = Nothing

        If m_MeasureBtn IsNot Nothing Then
            m_MeasureBtn.Dispose()
        End If
        m_MeasureBtn = Nothing

        If m_BookmarkBtn IsNot Nothing Then
            m_BookmarkBtn.Dispose()
        End If
        m_BookmarkBtn = Nothing

        If m_ZoomInOutBtn IsNot Nothing Then
            m_ZoomInOutBtn.Dispose()
        End If
        m_ZoomInOutBtn = Nothing

        If m_ZoomFullBtn IsNot Nothing Then
            m_ZoomFullBtn.Dispose()
        End If
        m_ZoomFullBtn = Nothing

        If m_RotLeftBtn IsNot Nothing Then
            m_RotLeftBtn.Dispose()
        End If
        m_RotLeftBtn = Nothing

        If m_RotRightBtn IsNot Nothing Then
            m_RotRightBtn.Dispose()
        End If
        m_RotRightBtn = Nothing

        If m_GPSLoadingPic IsNot Nothing Then
            m_GPSLoadingPic.Dispose()
        End If
        m_GPSLoadingPic = Nothing
        If m_t IsNot Nothing Then
            m_t.Abort()

        End If

        m_t = Nothing


        If m_GPSTimer IsNot Nothing Then
            ' m_GPSTimer.Stop()
            m_GPSTimer.Dispose()

        End If
        m_GPSTimer = Nothing

    End Sub
#End Region
#Region "PrivateFunctions"

    'Enum for type of pan and label side
    Private Enum side
        Left
        Right
        Top
        Bottom
        TopLeft
        TopRight
        BottomLeft
        BottomRight
    End Enum
    Private Sub Color()
        Dim pLbl As Label
        'Change the color and style of the border
        For Each ct As Control In m_Map.Controls
            Select Case ct.Name
                Case "lblPanTop"
                    pLbl = CType(ct, Label)
                    pLbl.BackColor = m_BackColor
                    pLbl.BorderStyle = m_BarBorderStyle
                Case "lblPanBottom"
                    pLbl = CType(ct, Label)
                    pLbl.BackColor = m_BackColor
                    pLbl.BorderStyle = m_BarBorderStyle
                Case "lblPanLeft"
                    pLbl = CType(ct, Label)
                    pLbl.BackColor = m_BackColor
                    pLbl.BorderStyle = m_BarBorderStyle
                Case "lblPanRight"
                    pLbl = CType(ct, Label)
                    pLbl.BackColor = m_BackColor
                    pLbl.BorderStyle = m_BarBorderStyle
                Case "lblPanTopRight"
                    pLbl = CType(ct, Label)
                    pLbl.BackColor = m_BackColor
                    pLbl.BorderStyle = m_BarBorderStyle
                Case "lblPanTopLeft"
                    pLbl = CType(ct, Label)
                    pLbl.BackColor = m_BackColor
                    pLbl.BorderStyle = m_BarBorderStyle

                Case "lblPanBottomRight"
                    pLbl = CType(ct, Label)
                    pLbl.BackColor = m_BackColor
                    pLbl.BorderStyle = m_BarBorderStyle
                Case "lblPanBottomLeft"
                    pLbl = CType(ct, Label)
                    pLbl.BackColor = m_BackColor
                    pLbl.BorderStyle = m_BarBorderStyle

            End Select
        Next
        pLbl = Nothing

    End Sub
    Private Sub Resize()
        Try


            'Resize the bar based on the defined width and the control size
            Dim pLbl As Label
            ' Dim pBtn As Button
            Dim intSpace As Integer = 10

            m_ZoomInBtn.Location = New System.Drawing.Point(m_BarWidth + intSpace, m_BarWidth + intSpace)
            m_ZoomOutBtn.Location = New System.Drawing.Point(m_ZoomInBtn.Left + m_ZoomInBtn.Width + intSpace, m_BarWidth + intSpace)
            m_ZoomInOutBtn.Location = New System.Drawing.Point(m_ZoomOutBtn.Left + m_ZoomOutBtn.Width + intSpace, m_BarWidth + intSpace)
            m_ZoomFullBtn.Location = New System.Drawing.Point(m_ZoomInOutBtn.Left + m_ZoomInOutBtn.Width + intSpace, m_BarWidth + intSpace)

            m_PanBtn.Location = New System.Drawing.Point(m_ZoomInBtn.Left, m_ZoomInBtn.Top + m_ZoomInBtn.Height + intSpace)
            m_ZoomPrevBtn.Location = New System.Drawing.Point(m_PanBtn.Left + m_PanBtn.Width + intSpace, m_PanBtn.Top)
            m_ZoomNextBtn.Location = New System.Drawing.Point(m_ZoomPrevBtn.Left + m_ZoomPrevBtn.Width + intSpace, m_PanBtn.Top)

            If m_MeasureBtn IsNot Nothing Then
                m_MeasureBtn.Location = New System.Drawing.Point(m_PanBtn.Left, m_PanBtn.Top + m_PanBtn.Height + intSpace)

                If m_GPSBtn IsNot Nothing Then
                    m_GPSBtn.Location = New System.Drawing.Point(m_MeasureBtn.Left + m_MeasureBtn.Width + intSpace, m_MeasureBtn.Top)
                    m_GPSLoadingPic.Location = New System.Drawing.Point(m_GPSBtn.Left + m_GPSBtn.Width + intSpace, CInt(m_GPSBtn.Top + (m_GPSBtn.Height / 2) - (m_GPSLoadingPic.Height / 2)))
                    If m_RotLeftBtn IsNot Nothing Then

                        m_RotLeftBtn.Location = New System.Drawing.Point(m_PanBtn.Left, m_MeasureBtn.Top + m_MeasureBtn.Height + intSpace)
                        m_RotRightBtn.Location = New System.Drawing.Point(m_RotLeftBtn.Left + m_RotRightBtn.Width + intSpace, m_RotLeftBtn.Top)
                    End If
                Else
                    If m_RotLeftBtn IsNot Nothing Then

                        m_RotLeftBtn.Location = New System.Drawing.Point(m_PanBtn.Left, m_MeasureBtn.Top + m_MeasureBtn.Height + intSpace)
                        m_RotRightBtn.Location = New System.Drawing.Point(m_RotLeftBtn.Left + m_RotRightBtn.Width + intSpace, m_RotLeftBtn.Top)
                    End If
                End If



            Else
                If m_GPSBtn IsNot Nothing Then


                    m_GPSBtn.Location = New System.Drawing.Point(m_PanBtn.Left, m_PanBtn.Top + m_PanBtn.Height + intSpace)
                    m_GPSLoadingPic.Location = New System.Drawing.Point(m_GPSBtn.Left + m_GPSBtn.Width + intSpace, CInt(m_GPSBtn.Top + (m_GPSBtn.Height / 2) - (m_GPSLoadingPic.Height / 2)))
                    If m_RotLeftBtn IsNot Nothing Then
                        m_RotLeftBtn.Location = New System.Drawing.Point(m_PanBtn.Left, m_GPSBtn.Top + m_GPSBtn.Height + intSpace)
                        m_RotRightBtn.Location = New System.Drawing.Point(m_RotLeftBtn.Left + m_RotRightBtn.Width + intSpace, m_RotLeftBtn.Top)

                    End If
                Else
                    If m_RotLeftBtn IsNot Nothing Then
                        m_RotLeftBtn.Location = New System.Drawing.Point(m_PanBtn.Left, m_PanBtn.Top + m_PanBtn.Height + intSpace)
                        m_RotRightBtn.Location = New System.Drawing.Point(m_RotLeftBtn.Left + m_RotRightBtn.Width + intSpace, m_RotLeftBtn.Top)

                    End If
                End If



            End If


            For Each ct As Control In m_Map.Controls

                Select Case ct.Name
                    Case "btnZoomIn"
                        'pBtn = CType(ct, Button)
                        'pBtn.Location = New System.Drawing.Point(m_BarWidth + 5, m_BarWidth + 5)
                    Case "btnZoomOut"
                        'pBtn = CType(ct, Button)
                        'pBtn.Location = New System.Drawing.Point(m_BarWidth + 60, m_BarWidth + 5)
                    Case "btnZoomInOut"
                        'pBtn = CType(ct, Button)
                        'pBtn.Location = New System.Drawing.Point(m_BarWidth + 120, m_BarWidth + 5)

                    Case "btnPan"
                        'pBtn = CType(ct, Button)
                        'pBtn.Location = New System.Drawing.Point(m_BarWidth + 5, m_BarWidth + 5 + (pBtn.Height * 1) + 20)

                    Case "btnZoomPrev"
                        'pBtn = CType(ct, Button)
                        'pBtn.Location = New System.Drawing.Point(m_BarWidth + 5 + 65 + 10, m_BarWidth + 5 + (pBtn.Height * 1) + 20)
                    Case "btnZoomNext"
                        'pBtn = CType(ct, Button)
                        'pBtn.Location = New System.Drawing.Point(m_BarWidth + 5 + 65 + 10, m_BarWidth + 5 + (pBtn.Height * 1) + 20)

                    Case "btnMeasure"
                        'pBtn = CType(ct, Button)
                        'pBtn.Location = New System.Drawing.Point(m_BarWidth + 5, m_BarWidth + 5 + (pBtn.Height * 2) + 20)


                    Case "btnGPS"
                        'pBtn = CType(ct, Button)
                        'If m_MeasureBtn Is Nothing Then
                        '    pBtn.Location = New System.Drawing.Point(m_BarWidth + 5, m_BarWidth + 5 + (pBtn.Height * 2) + 20)

                        'Else
                        '    pBtn.Location = New System.Drawing.Point(m_BarWidth + 5 + 65 + 10, m_BarWidth + 5 + (pBtn.Height * 2) + 20)

                        'End If

                    Case "picGPS"
                        'If m_MeasureBtn Is Nothing Then
                        '    ct.Location = New System.Drawing.Point(m_BarWidth + 5 + 65 + 10, m_BarWidth + 5 + (pBtn.Height * 2) + 20)

                        'Else
                        '    ct.Location = New System.Drawing.Point(m_BarWidth + 5 + 130 + 10, m_BarWidth + 5 + (pBtn.Height * 2) + 20)


                        'End If


                    Case "lblPanTop"
                        pLbl = CType(ct, Label)
                        pLbl.Left = m_BarWidth + m_LeftOffset
                        pLbl.Top = 0 + m_TopOffset
                        pLbl.Width = m_Map.Width - (m_BarWidth * 2) - m_RightOffset - m_LeftOffset
                        pLbl.Height = m_BarWidth

                        GenerateArrows(side.Top, pLbl)
                    Case "lblPanBottom"
                        pLbl = CType(ct, Label)

                        pLbl.Width = m_Map.Width - (m_BarWidth * 2) - m_RightOffset - m_LeftOffset
                        pLbl.Height = m_BarWidth
                        pLbl.Left = m_BarWidth + m_LeftOffset
                        pLbl.Top = m_Map.Height - m_BarWidth - m_BottomOffset
                        GenerateArrows(side.Bottom, pLbl)
                    Case "lblPanLeft"
                        pLbl = CType(ct, Label)

                        pLbl.Width = m_BarWidth
                        pLbl.Height = m_Map.Height - (m_BarWidth * 2) - m_BottomOffset - m_TopOffset
                        pLbl.Left = 0 + m_LeftOffset
                        pLbl.Top = m_BarWidth + m_TopOffset
                        GenerateArrows(side.Left, pLbl)

                    Case "lblPanRight"
                        pLbl = CType(ct, Label)
                        pLbl.Width = m_BarWidth
                        pLbl.Height = m_Map.Height - (m_BarWidth * 2) - m_BottomOffset - m_TopOffset
                        pLbl.Left = m_Map.Width - m_BarWidth - m_RightOffset - 1
                        pLbl.Top = m_BarWidth + m_TopOffset
                        GenerateArrows(side.Right, pLbl)
                    Case "lblPanTopRight"
                        pLbl = CType(ct, Label)
                        pLbl.Left = m_Map.Width - m_BarWidth - m_RightOffset
                        pLbl.Top = 0 + m_TopOffset
                        pLbl.Width = m_BarWidth
                        pLbl.Height = m_BarWidth
                        GenerateArrows(side.TopRight, pLbl)
                    Case "lblPanTopLeft"
                        pLbl = CType(ct, Label)
                        pLbl.Left = 0 + m_LeftOffset
                        pLbl.Top = 0 + m_TopOffset
                        pLbl.Width = m_BarWidth
                        pLbl.Height = m_BarWidth
                        GenerateArrows(side.TopLeft, pLbl)
                    Case "lblPanBottomRight"
                        pLbl = CType(ct, Label)
                        pLbl.Left = m_Map.Width - m_BarWidth - m_RightOffset
                        pLbl.Top = m_Map.Height - m_BarWidth - m_BottomOffset
                        pLbl.Width = m_BarWidth
                        pLbl.Height = m_BarWidth

                        GenerateArrows(side.BottomRight, pLbl)
                    Case "lblPanBottomLeft"
                        pLbl = CType(ct, Label)
                        pLbl.Left = 0 + m_LeftOffset
                        pLbl.Top = m_Map.Height - m_BarWidth - m_BottomOffset
                        pLbl.Width = m_BarWidth
                        pLbl.Height = m_BarWidth
                        GenerateArrows(side.BottomLeft, pLbl)
                End Select
                'Select Case ct.Name
                '    Case "btnZoomIn"
                '        pBtn = CType(ct, Button)
                '        pBtn.Location = New System.Drawing.Point(m_BarWidth + 5, m_BarWidth + 5)
                '    Case "btnZoomOut"
                '        pBtn = CType(ct, Button)
                '        pBtn.Location = New System.Drawing.Point(m_BarWidth + 5, m_BarWidth + 5 + pBtn.Height + 10)
                '    Case "btnZoomInOut"
                '        pBtn = CType(ct, Button)
                '        pBtn.Location = New System.Drawing.Point(m_BarWidth + 5, m_BarWidth + 5 + (pBtn.Height * 3) + 30)
                '    Case "btnMeasure"
                '        pBtn = CType(ct, Button)
                '        If m_GPSBtn Is Nothing Then
                '            pBtn.Location = New System.Drawing.Point(m_BarWidth + 65, m_BarWidth + 5)
                '        Else
                '            pBtn.Location = New System.Drawing.Point(m_BarWidth + 125, m_BarWidth + 5)
                '        End If

                '    Case "btnPan"
                '        pBtn = CType(ct, Button)
                '        pBtn.Location = New System.Drawing.Point(m_BarWidth + 5, m_BarWidth + 5 + (pBtn.Height * 2) + 20)
                '    Case "btnGPS"
                '        pBtn = CType(ct, Button)
                '        pBtn.Location = New System.Drawing.Point(m_BarWidth + 65, m_BarWidth + 5)
                '    Case "picGPS"

                '        ct.Location = New System.Drawing.Point(m_BarWidth + 125, m_BarWidth + 15)
                '    Case "lblPanTop"
                '        pLbl = CType(ct, Label)
                '        pLbl.Left = m_BarWidth + m_LeftOffset
                '        pLbl.Top = 0 + m_TopOffset
                '        pLbl.Width = m_Map.Width - (m_BarWidth * 2) - m_RightOffset - m_LeftOffset
                '        pLbl.Height = m_BarWidth

                '        GenerateArrows(side.Top, pLbl)
                '    Case "lblPanBottom"
                '        pLbl = CType(ct, Label)

                '        pLbl.Width = m_Map.Width - (m_BarWidth * 2) - m_RightOffset - m_LeftOffset
                '        pLbl.Height = m_BarWidth
                '        pLbl.Left = m_BarWidth + m_LeftOffset
                '        pLbl.Top = m_Map.Height - m_BarWidth - m_BottomOffset
                '        GenerateArrows(side.Bottom, pLbl)
                '    Case "lblPanLeft"
                '        pLbl = CType(ct, Label)

                '        pLbl.Width = m_BarWidth
                '        pLbl.Height = m_Map.Height - (m_BarWidth * 2) - m_BottomOffset - m_TopOffset
                '        pLbl.Left = 0 + m_LeftOffset
                '        pLbl.Top = m_BarWidth + m_TopOffset
                '        GenerateArrows(side.Left, pLbl)

                '    Case "lblPanRight"
                '        pLbl = CType(ct, Label)
                '        pLbl.Width = m_BarWidth
                '        pLbl.Height = m_Map.Height - (m_BarWidth * 2) - m_BottomOffset - m_TopOffset
                '        pLbl.Left = m_Map.Width - m_BarWidth - m_RightOffset - 1
                '        pLbl.Top = m_BarWidth + m_TopOffset
                '        GenerateArrows(side.Right, pLbl)
                '    Case "lblPanTopRight"
                '        pLbl = CType(ct, Label)
                '        pLbl.Left = m_Map.Width - m_BarWidth - m_RightOffset
                '        pLbl.Top = 0 + m_TopOffset
                '        pLbl.Width = m_BarWidth
                '        pLbl.Height = m_BarWidth
                '        GenerateArrows(side.TopRight, pLbl)
                '    Case "lblPanTopLeft"
                '        pLbl = CType(ct, Label)
                '        pLbl.Left = 0 + m_LeftOffset
                '        pLbl.Top = 0 + m_TopOffset
                '        pLbl.Width = m_BarWidth
                '        pLbl.Height = m_BarWidth
                '        GenerateArrows(side.TopLeft, pLbl)
                '    Case "lblPanBottomRight"
                '        pLbl = CType(ct, Label)
                '        pLbl.Left = m_Map.Width - m_BarWidth - m_RightOffset
                '        pLbl.Top = m_Map.Height - m_BarWidth - m_BottomOffset
                '        pLbl.Width = m_BarWidth
                '        pLbl.Height = m_BarWidth

                '        GenerateArrows(side.BottomRight, pLbl)
                '    Case "lblPanBottomLeft"
                '        pLbl = CType(ct, Label)
                '        pLbl.Left = 0 + m_LeftOffset
                '        pLbl.Top = m_Map.Height - m_BarWidth - m_BottomOffset
                '        pLbl.Width = m_BarWidth
                '        pLbl.Height = m_BarWidth
                '        GenerateArrows(side.BottomLeft, pLbl)
                'End Select
            Next

            'For Top Middle
            'm_ScaleY = m_BarWidth + 15
            'm_ScaleX = CInt(m_Map.Width / 2)

            'For Bottom Right
            m_ScaleX = m_BarWidth + 10
            m_ScaleY = m_Map.Height - m_BarWidth - 25
            m_NorthArrowX = m_ScaleX
            m_NorthArrowY = CInt(m_ScaleY - 10 - My.Resources.NorthArrow.Height / 2)

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try


    End Sub
    Private Sub GenerateArrows(ByVal labelSide As side, ByVal panel As Label)
        Try

            'Adds the arrows to the lable based on label size and padding
            Dim g As Graphics
            'Create a new graphic for the label
            g = panel.CreateGraphics
            'Set up parems
            Dim pCenterPoint, pLeftPoint, pRightPoint As Point
            Dim pPad As Integer = 1
            Dim pSizePad As Integer = 5
            Dim pPadTopBottom As Integer = 2
            'Determine arrow sizing and position
            Select Case labelSide
                Case side.Top

                    pCenterPoint = New Point(CInt(panel.Width / 2), pPad)


                    pRightPoint = New Point(CInt((panel.Width / 2) + panel.Height - pSizePad), panel.Height - pPad)
                    pLeftPoint = New Point(CInt((panel.Width / 2) - panel.Height + pSizePad), panel.Height - pPad)



                Case side.Left

                    pCenterPoint = New Point(pPad, CInt(panel.Height / 2))

                    pRightPoint = New Point(panel.Width - pPad, CInt(panel.Height / 2) + panel.Width - pSizePad)
                    pLeftPoint = New Point(panel.Width - pPad, CInt(panel.Height / 2) - panel.Width + pSizePad)
                Case side.Bottom
                    pCenterPoint = New Point(CInt(panel.Width / 2), panel.Height - pPadTopBottom)

                    pRightPoint = New Point(CInt((panel.Width / 2) + panel.Height - pSizePad), pPadTopBottom)
                    pLeftPoint = New Point(CInt((panel.Width / 2) - panel.Height + pSizePad), pPadTopBottom)




                Case side.Right

                    pCenterPoint = New Point(panel.Width - pPadTopBottom, CInt(panel.Height / 2))

                    pRightPoint = New Point(pPad, CInt((panel.Height / 2) + panel.Width - pSizePad))
                    pLeftPoint = New Point(pPad, CInt((panel.Height / 2) - panel.Width + pSizePad))
                Case side.TopLeft

                    pCenterPoint = New Point(-1, -1)

                    pRightPoint = New Point(panel.Width - 1, -1)

                    pLeftPoint = New Point(-1, panel.Height - 1)



                Case side.TopRight
                    pCenterPoint = New Point(panel.Width + 1, -1)

                    pRightPoint = New Point(-1, -1)

                    pLeftPoint = New Point(panel.Width + 1, panel.Height + 1)

                Case side.BottomLeft
                    pCenterPoint = New Point(-1, panel.Height + 1)

                    pRightPoint = New Point(panel.Width + 1, panel.Height + 1)

                    pLeftPoint = New Point(-1, -1)


                Case side.BottomRight
                    pCenterPoint = New Point(panel.Width - pPadTopBottom, panel.Height - pPadTopBottom)

                    pRightPoint = New Point(panel.Width, -2)

                    pLeftPoint = New Point(-2, panel.Height)

                Case Else

            End Select
            'Assigned the arrow to the tag of the label so it can be drawn
            Dim pPoint(2) As Point
            pPoint(0) = pCenterPoint
            pPoint(1) = pLeftPoint
            pPoint(2) = pRightPoint

            panel.Tag = pPoint
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
    Private Sub AddNavigationBar()
        Try


            'Adds the navigation bar to the map container
            Dim strLabels() As String = {"lblPanTop", "lblPanBottom", "lblPanLeft", "lblPanRight", "lblPanTopRight", "lblPanTopLeft", "lblPanBottomRight", "lblPanBottomLeft"}
            For Each strLbl As String In strLabels
                'Use a label to draw the navigation bar
                Dim pLbl As Label
                'Create new label
                pLbl = New Label
                'Remove the default parems
                pLbl.Text = ""
                pLbl.AutoSize = False
                'Bar on Top of hte map
                pLbl.Name = strLbl
                'Set the color of the bar
                pLbl.BackColor = m_BackColor
                pLbl.BorderStyle = m_BarBorderStyle
                Select Case strLbl
                    Case "lblPanTop"
                        pLbl.Cursor = Cursors.PanNorth
                    Case "lblPanBottom"
                        pLbl.Cursor = Cursors.PanSouth
                    Case "lblPanLeft"
                        pLbl.Cursor = Cursors.PanWest
                    Case "lblPanRight"
                        pLbl.Cursor = Cursors.PanEast
                    Case "lblPanTopRight"
                        pLbl.Cursor = Cursors.PanNE
                    Case "lblPanTopLeft"
                        pLbl.Cursor = Cursors.PanNW
                    Case "lblPanBottomRight"
                        pLbl.Cursor = Cursors.PanSE
                    Case "lblPanBottomLeft"
                        pLbl.Cursor = Cursors.PanSW
                End Select

                'Add handler to handle click and paint events
                AddHandler pLbl.Click, AddressOf navigateArrows
                AddHandler pLbl.Paint, AddressOf DrawArrows
                'Add the contorl to the screen
                m_Map.Controls.Add(pLbl)

            Next


        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
    Private Sub panByDirection(ByVal mblMap As Esri.ArcGIS.Mobile.WinForms.Map, ByVal pPanDir As side)
        Try

            Dim angle As Double = Math.PI * mblMap.RotationAngle / 180.0

            '// Get the sine and cosine of the angle.
            Dim sinAngle As Double = Math.Sin(angle)
            Dim cosAngle As Double = Math.Cos(angle)

            '// Convert the distance from pixels to map coordinates.
            Dim mapDx As Double
            Dim mapDy As Double
            Dim pSclFact As Double = 0.1
            pSclFact = pSclFact * mblMap.Scale

            Dim dblPanVal As Double = CDbl(CustomServerToMobileGeom(pSclFact))

            'Set up pan value
            Select Case pPanDir
                Case side.Bottom
                    mapDx = 0
                    mapDy = -dblPanVal


                Case side.BottomLeft
                    mapDx = -dblPanVal
                    mapDy = -dblPanVal

                Case side.BottomRight
                    mapDx = dblPanVal
                    mapDy = -dblPanVal

                Case side.Left
                    mapDx = -dblPanVal
                    mapDy = 0

                Case side.Right
                    mapDx = dblPanVal
                    mapDy = 0

                Case side.Top

                    mapDx = 0
                    mapDy = dblPanVal

                Case side.TopLeft
                    mapDx = -dblPanVal
                    mapDy = dblPanVal

                Case side.TopRight
                    mapDx = dblPanVal
                    mapDy = dblPanVal

            End Select


            'Rotate it.
            Dim x As Double = (mapDx * cosAngle) - (mapDy * sinAngle)
            Dim y As Double = mapDx * sinAngle + mapDy * cosAngle

            'Offset the extent.
            Dim pext As Esri.ArcGIS.Mobile.Geometries.Envelope = mblMap.Extent()

            pext.Offset(x, y)

            mblMap.Extent = pext

            ' mblMap.Refresh()


        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try


    End Sub
    Private Function AzimuthToGeoAngle(ByVal AzimuthDegrees As Double) As Double

        ' Azimuth angles start at due north and increase clockwise
        ' GeoAngles start at due east (90 deg. azimuth) and increase counterclockwise
        Dim numerator As Long = CLng(450 - AzimuthDegrees)
        Dim rawangle As Long = numerator Mod 360
        Dim remainder As Double = (450.0F - AzimuthDegrees) - numerator
        Dim geoangle As Double = rawangle + remainder
        Return geoangle

    End Function
    Private Sub navigateArrows(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Select Case CType(sender, Control).Name
            Case "lblPanRight"
                panByDirection(m_Map, side.Right)
            Case "lblPanTop"

                panByDirection(m_Map, side.Top)
            Case "lblPanLeft"

                panByDirection(m_Map, side.Left)
            Case "lblPanBottom"

                panByDirection(m_Map, side.Bottom)
            Case "lblPanBottomRight"

                panByDirection(m_Map, side.BottomRight)
            Case "lblPanBottomLeft"

                panByDirection(m_Map, side.BottomLeft)
            Case "lblPanTopRight"

                panByDirection(m_Map, side.TopRight)
            Case "lblPanTopLeft"

                panByDirection(m_Map, side.TopLeft)

        End Select

    End Sub
    Private Function CustomServerToMobileGeom(ByVal DistanceInFeet As Double) As Double
        ';convert feet to map units, used to be custom in 9.2
        Return Esri.ArcGIS.Mobile.SpatialReferences.Unit.FromUnitToUnit(DistanceInFeet, Esri.ArcGIS.Mobile.SpatialReferences.Unit.Foot, m_Map.SpatialReference.Unit)

    End Function
    Private Sub DrawArrows(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs)
        'Draw the arrows on the navigation bar, 
        e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

        If CType(sender, Label).Tag Is Nothing Then
            Return

        End If
        Dim pPolygon() As Point = CType(CType(sender, Label).Tag, Point())

        e.Graphics.FillPolygon(m_BrushArrow, pPolygon)

    End Sub
    Private Sub FixZoomMouseWheel(ByVal bOut As Boolean, ByVal centerCoord As Esri.ArcGIS.Mobile.Geometries.Coordinate)
        'Handles the mouse wheel zoom in and out
        'Current this recenters on the mouse wheel, not sure if I like this
        If (bOut) Then
            Dim pExt As Esri.ArcGIS.Mobile.Geometries.Envelope
            'Get the current extent
            pExt = m_Map.Extent()
            'Center on the mouse
            pExt.CenterAt(centerCoord.X, centerCoord.Y)
            'expand the extent
            pExt.Resize(1.2)
            'Set it back to the map
            m_Map.Extent = pExt
            'cleanup
            pExt = Nothing
        Else
            Dim pExt As Esri.ArcGIS.Mobile.Geometries.Envelope
            'Get the current extent
            pExt = m_Map.Extent()
            'Reduce the extent
            pExt.Resize(0.8)
            'Center on the mouse
            pExt.CenterAt(centerCoord.X, centerCoord.Y)
            'Set it back to the map
            m_Map.Extent = pExt
            pExt = Nothing

        End If


    End Sub

    Private Sub m_Map_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_Map.Disposed
        If GlobalsFunctions.m_GPS.GpsConnection IsNot Nothing Then
            If GlobalsFunctions.m_GPS.GpsConnection.IsOpen Then
                GlobalsFunctions.m_GPS.GpsConnection.Close()

            End If
        End If
    End Sub
    Private Sub m_Map_ExtentChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_Map.ExtentChanged
        ''check the scale on the extent change to make sure you are not at the max extent, if so reduce it slightly
        ''If the scale is greater then the max scale
        'If m_MaxScale = -1 Then
        '    MaxExtent()
        '    Return
        'End If

        'If m_MaxScale = 0 Then
        '    MaxExtent()
        '    Return
        'End If

        'If m_Map.Scale > m_MaxScale Then
        '    'Remove the handler to monitor extent change
        '    RemoveHandler m_Map.ExtentChanged, AddressOf m_Map_ExtentChanged
        '    'Reduce the map scale
        '    m_Map.Scale = m_MaxScale - 200
        '    'add the handler
        '    AddHandler m_Map.ExtentChanged, AddressOf m_Map_ExtentChanged
        '    'Exit
        '    Return

        'End If
        ''Just another check, 
        'If Abs(CInt(m_Map.Scale) - m_MaxScale) < 1000 Then
        '    'Remove the handler to monitor extent change

        '    RemoveHandler m_Map.ExtentChanged, AddressOf m_Map_ExtentChanged
        '    'Reduce the map scale
        '    m_Map.Scale = m_MaxScale - 200
        '    'add the handler
        '    AddHandler m_Map.ExtentChanged, AddressOf m_Map_ExtentChanged
        '    'exit
        '    Return

        'End If
        ' m_LastExtents.Push(m_Map.Extent())
        m_LastExtents.Add(m_Map.Extent())

        If m_LastExtents.Count > 10 Then
            m_LastExtents.RemoveAt(0)

        End If
        m_CurrentStep = m_LastExtents.Count - 1



    End Sub

    Private Sub m_Map_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles m_Map.KeyPress
        If (e.KeyChar = "+") Then
            FixZoom(False)
        ElseIf (e.KeyChar = "-") Then
            FixZoom(True)
            'ElseIf (e.KeyChar = "0") Then
            '    rotateMap(90)
            'ElseIf (e.KeyChar = "9") Then
            '    rotateMap(-90)
        End If
    End Sub



    Private Sub m_Map_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles m_Map.MouseUp

        If m_ZoomTimer IsNot Nothing Then
            If m_ZoomTimer.Enabled Then
                m_ZoomTimer.Stop()
                If m_FixZoom Then

                    m_ZoomInBtn.BackgroundImage = My.Resources.ZoomIn
                    m_ZoomOutBtn.BackgroundImage = My.Resources.ZoomOut
                End If
            End If
        End If

    End Sub
    Private Sub m_Map_MouseWheel(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.MapMouseEventArgs) Handles m_Map.MouseWheel
        'check whether to zoom on the mouse wheel
        If Not m_EnableMouseWheelZoom Then Return
        'Determine which way the mouse wheel was spun
        If e.Delta > 0 Then
            'Zoom
            FixZoomMouseWheel(False, e.MapCoordinate)
        Else
            FixZoomMouseWheel(True, e.MapCoordinate)
        End If

    End Sub
    Private Sub m_Map_Paint(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.WinForms.MapPaintEventArgs) Handles m_Map.MapPaint
        If m_Map Is Nothing Then Return

        'If m_Map.RotationAngle <> 0 Then
        '    My.Resources.NorthArrow.RotateFlip(RotateFlipType.Rotate90FlipNone)
        '    e.Graphics.DrawImage(My.Resources.NorthArrow, m_NorthArrowX, m_NorthArrowY)
        '    'DrawImageRotatedAroundCenter(e.Graphics, New System.Drawing.Point(m_NorthArrowX, m_NorthArrowY), My.Resources.NorthArrow, m_Map.RotationAngle)

        '    ' e.Graphics.DrawImage(My.Resources.NorthArrow, m_NorthArrowX, m_NorthArrowY, My.Resources.NorthArrow.Width, My.Resources.NorthArrow.Height)
        'End If

        'check whether to draw the scale
        If m_DrawScale Then

            'Make sure the map is valid
            If m_Map.Scale <= 0 Then Return

            'Draw the scale 



            'e.Graphics.DrawString("1: " & CInt(m_Map.Scale), m_scaleFontLarge, Brushes.Red, m_ScaleX, m_ScaleY)
            'For top Center
            'm_ScaleX = CInt(m_Map.Width / 2 - e.Graphics.MeasureString("1:" & CInt(m_Map.Scale), m_scaleFont).Width / 2)
            'e.Graphics.DrawString("1:" & CInt(m_Map.Scale), m_scaleFont, m_ScaleBrush, m_ScaleX - m_offset, m_ScaleY - m_offset)
            'e.Graphics.DrawString("1:" & CInt(m_Map.Scale), m_scaleFont, m_ScaleBrush, m_ScaleX + m_offset, m_ScaleY - m_offset)
            'e.Graphics.DrawString("1:" & CInt(m_Map.Scale), m_scaleFont, m_ScaleBrush, m_ScaleX - m_offset, m_ScaleY + m_offset)
            'e.Graphics.DrawString("1:" & CInt(m_Map.Scale), m_scaleFont, m_ScaleBrush, m_ScaleX + m_offset, m_ScaleY + m_offset)
            'e.Graphics.DrawString("1:" & CInt(m_Map.Scale), m_scaleFont, m_ScaleBrushFront, m_ScaleX, m_ScaleY)

            'For bottom left

            e.Graphics.DrawString("1:" & CInt(m_Map.Scale), m_scaleFont, m_ScaleBrush, m_ScaleX - m_offset, m_ScaleY - m_offset)
            e.Graphics.DrawString("1:" & CInt(m_Map.Scale), m_scaleFont, m_ScaleBrush, m_ScaleX + m_offset, m_ScaleY - m_offset)
            e.Graphics.DrawString("1:" & CInt(m_Map.Scale), m_scaleFont, m_ScaleBrush, m_ScaleX - m_offset, m_ScaleY + m_offset)
            e.Graphics.DrawString("1:" & CInt(m_Map.Scale), m_scaleFont, m_ScaleBrush, m_ScaleX + m_offset, m_ScaleY + m_offset)
            e.Graphics.DrawString("1:" & CInt(m_Map.Scale), m_scaleFont, m_ScaleBrushFront, m_ScaleX, m_ScaleY)
        End If
    End Sub

    Private Sub MaxExtent()
        'Try
        '    If m_Map.IsValid = False Then Return

        '    'Remove the extent change hanlder
        '    RemoveHandler m_Map.ExtentChanged, AddressOf m_Map_ExtentChanged

        '    'Determines the max extent, this is to solve a redraw issue that happens at full extent
        '    'Suspend the map
        '    m_Map.SuspendLayout()
        '    m_Map.SuspendDrawing = True
        '    'Get the current extent
        '    Dim pCurExt As ESRI.ArcGIS.Mobile.Geometries.Envelope = m_Map.Extent
        '    'Set the map to full extent
        '    m_Map.Extent = m_Map.GetFullExtent)
        '    'Get the Scale
        '    m_MaxScale = m_Map.Scale
        '    'Set the map back to the exsiting extent
        '    m_Map.Extent = pCurExt)
        '    'resume drawing
        '    m_Map.SuspendDrawing = False
        '    m_Map.ResumeLayout()
        '    'Cleanup
        '    pCurExt = Nothing
        '    'readd the handler
        '    AddHandler m_Map.ExtentChanged, AddressOf m_Map_ExtentChanged
        'Catch ex As Exception

        'End Try

    End Sub
    Private Sub AddNavButtons()
        'Add the buttons for the navigation control
        ' Dim pBtn As New Button
        'Set up the buttons look and feel\
        m_ZoomInBtn = New Button

        m_ZoomInBtn.BackColor = System.Drawing.SystemColors.Info
        m_ZoomInBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        m_ZoomInBtn.FlatAppearance.BorderSize = 0
        m_ZoomInBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        m_ZoomInBtn.BackgroundImage = Global.MobileControls.My.Resources.Resources.ZoomIn
        m_ZoomInBtn.Cursor = Cursors.Arrow
        m_ZoomInBtn.Name = "btnZoomIn"
        m_ZoomInBtn.Size = New System.Drawing.Size(50, 50)
        m_ZoomInBtn.UseVisualStyleBackColor = False
        'Add the handlers for fixed zoom in, 
        AddHandler m_ZoomInBtn.MouseDown, AddressOf btnZoomIn_MouseDown
        AddHandler m_ZoomInBtn.MouseUp, AddressOf btnZoomIn_MouseUp
        'add it to the map
        m_Map.Controls.Add(m_ZoomInBtn)

        'Create the next button
        m_ZoomOutBtn = New Button
        m_ZoomOutBtn.BackColor = System.Drawing.SystemColors.Info
        m_ZoomOutBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        m_ZoomOutBtn.FlatAppearance.BorderSize = 0
        m_ZoomOutBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        m_ZoomOutBtn.BackgroundImage = Global.MobileControls.My.Resources.Resources.ZoomOut
        m_ZoomOutBtn.Cursor = Cursors.Arrow
        m_ZoomOutBtn.Name = "btnZoomOut"
        m_ZoomOutBtn.Size = New System.Drawing.Size(50, 50)

        m_ZoomOutBtn.UseVisualStyleBackColor = False
        'Add the handlers for fixed zoom out
        AddHandler m_ZoomOutBtn.MouseDown, AddressOf btnZoomOut_MouseDown
        AddHandler m_ZoomOutBtn.MouseUp, AddressOf btnZoomOut_MouseUp
        'add it to the map
        m_Map.Controls.Add(m_ZoomOutBtn)
        'Create a new timer to repeat the fix zooms when the user holds the button down
        m_ZoomTimer = New System.Windows.Forms.Timer
        m_ZoomTimer.Interval = 250
        'Add the handler for each tick
        AddHandler m_ZoomTimer.Tick, AddressOf m_ZoomTimer_Tick

        ' 'Dim myutils As ConfigUtils = New ConfigUtils()
        Dim key As String = ""
        key = GlobalsFunctions.appConfig.NavigationOptions.GPS.GPSTrackLog
        If UCase(key) <> "FALSE" Then
            key = GlobalsFunctions.appConfig.NavigationOptions.GPS.GPSLogLayer
            If key <> "" Then
                GlobalsFunctions.GetMapLayer(key, m_Map)
              
                m_GPSFL = GlobalsFunctions.GetFeatureSource(key, m_Map).FeatureSource
                If m_GPSFL Is Nothing Then
                    m_LogGPS = False
                Else
                    m_LogGPS = True
                    m_GPSFL_UserField = GlobalsFunctions.appConfig.NavigationOptions.GPS.GPSLogLayer_UserNameField
                    m_GPSFL_DateField = GlobalsFunctions.appConfig.NavigationOptions.GPS.GPSLogLayer_DateField
                    key = GlobalsFunctions.appConfig.NavigationOptions.GPS.GPSLogInterval
                    If IsNumeric(key) Then
                        m_LogInterval = CInt(key)
                        'm_GPSTimer = New System.Threading.Timer

                        '  m_GPSTimer = New System.Windows.Forms.Timer


                        '    m_GPSTimer.Interval = m_LogInterval * 1000
                        'AddHandler m_GPSTimer.Tick, AddressOf TimerEventProcessor

                    End If

                End If
            End If
        End If

        'create the Zoom in/out button
        m_ZoomInOutBtn = New Button

        'Set up the buttons look and feel
        m_ZoomInOutBtn.BackColor = System.Drawing.SystemColors.Info
        m_ZoomInOutBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        m_ZoomInOutBtn.FlatAppearance.BorderSize = 0
        m_ZoomInOutBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        m_ZoomInOutBtn.BackgroundImage = Global.MobileControls.My.Resources.Resources.ZoomInOut
        m_ZoomInOutBtn.Cursor = Cursors.Arrow
        m_ZoomInOutBtn.Name = "btnZoomInOut"
        m_ZoomInOutBtn.Size = New System.Drawing.Size(50, 50)

        m_ZoomInOutBtn.UseVisualStyleBackColor = False
        'Add the handler for the zoom in/out button click
        AddHandler m_ZoomInOutBtn.MouseClick, AddressOf btnMA_Click
        'add it to the map
        m_Map.Controls.Add(m_ZoomInOutBtn)

        'create the Zoom Full Extent button
        m_ZoomFullBtn = New Button

        'Set up the buttons look and feel
        m_ZoomFullBtn.BackColor = System.Drawing.SystemColors.Info
        m_ZoomFullBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        m_ZoomFullBtn.FlatAppearance.BorderSize = 0
        m_ZoomFullBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        m_ZoomFullBtn.BackgroundImage = Global.MobileControls.My.Resources.Resources.FullExtent
        m_ZoomFullBtn.Cursor = Cursors.Arrow
        m_ZoomFullBtn.Name = "btnZoomFull"
        m_ZoomFullBtn.Size = New System.Drawing.Size(50, 50)

        m_ZoomFullBtn.UseVisualStyleBackColor = False
        'Add the handler for the zoom in/out button click
        AddHandler m_ZoomFullBtn.MouseClick, AddressOf btnFullExtent_Click
        'add it to the map
        m_Map.Controls.Add(m_ZoomFullBtn)


        'create the pan button
        m_PanBtn = New Button

        'Set up the buttons look and feel
        m_PanBtn.BackColor = System.Drawing.SystemColors.Info
        m_PanBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        m_PanBtn.FlatAppearance.BorderSize = 0
        m_PanBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        m_PanBtn.BackgroundImage = Global.MobileControls.My.Resources.Resources.Pan
        m_PanBtn.Cursor = Cursors.Hand

        m_PanBtn.Name = "btnPan"
        m_PanBtn.Size = New System.Drawing.Size(50, 50)

        m_PanBtn.UseVisualStyleBackColor = False
        'Add the handler for the pan button click
        AddHandler m_PanBtn.MouseClick, AddressOf btnMA_Click
        'add it to the map
        m_Map.Controls.Add(m_PanBtn)


        m_ZoomPrevBtn = New Button
        m_ZoomPrevBtn.BackColor = System.Drawing.SystemColors.Info
        m_ZoomPrevBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        m_ZoomPrevBtn.FlatAppearance.BorderSize = 0
        m_ZoomPrevBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        m_ZoomPrevBtn.BackgroundImage = Global.MobileControls.My.Resources.Resources.Left
        m_ZoomPrevBtn.Cursor = Cursors.Arrow
        m_ZoomPrevBtn.Name = "btnZoomPrev"
        m_ZoomPrevBtn.Size = New System.Drawing.Size(50, 50)

        m_ZoomPrevBtn.UseVisualStyleBackColor = False
        'Add the handlers for fixed zoom out
        AddHandler m_ZoomPrevBtn.Click, AddressOf btnZoomExt_Click


        'add it to the map
        m_Map.Controls.Add(m_ZoomPrevBtn)


        m_ZoomNextBtn = New Button
        m_ZoomNextBtn.BackColor = System.Drawing.SystemColors.Info
        m_ZoomNextBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        m_ZoomNextBtn.FlatAppearance.BorderSize = 0
        m_ZoomNextBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        m_ZoomNextBtn.BackgroundImage = Global.MobileControls.My.Resources.Resources.right
        m_ZoomNextBtn.Cursor = Cursors.Arrow
        m_ZoomNextBtn.Name = "btnZoomNext"
        m_ZoomNextBtn.Size = New System.Drawing.Size(50, 50)

        m_ZoomNextBtn.UseVisualStyleBackColor = False
        'Add the handlers for fixed zoom out
        AddHandler m_ZoomNextBtn.Click, AddressOf btnZoomExt_Click


        'add it to the map
        m_Map.Controls.Add(m_ZoomNextBtn)


        'create the GPS activate button
        m_GPSBtn = New Button
        'Set up the buttons look and feel
        m_GPSBtn.BackColor = System.Drawing.SystemColors.Info
        m_GPSBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        m_GPSBtn.FlatAppearance.BorderSize = 0
        m_GPSBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        m_GPSBtn.BackgroundImage = Global.MobileControls.My.Resources.Resources.SatBlue
        m_GPSBtn.Cursor = Cursors.Arrow
        m_GPSBtn.Name = "btnGPS"
        m_GPSBtn.Size = New System.Drawing.Size(50, 50)

        m_GPSBtn.UseVisualStyleBackColor = False
        'Add the handler for the GPS button click
        AddHandler m_GPSBtn.MouseDown, AddressOf btnGPS_MouseDown
        'add it to the map
        m_Map.Controls.Add(m_GPSBtn)
        'Create a pic to display the GPS init status
        m_GPSLoadingPic = New PictureBox
        'Set up the box  look and feel
        m_GPSLoadingPic.Image = My.Resources.GPSLoadingIndicator
        m_GPSLoadingPic.Width = My.Resources.GPSLoadingIndicator.Width
        m_GPSLoadingPic.Height = My.Resources.GPSLoadingIndicator.Height
        m_GPSLoadingPic.Name = "picGPS"
        m_GPSLoadingPic.Visible = False

        'add it to the map
        m_Map.Controls.Add(m_GPSLoadingPic)
        'cleaup
        'pBtn = Nothing


        key = GlobalsFunctions.appConfig.MeasureOptions.Visible
        If UCase(key) = "TRUE" Then

            'create the Zoom in/out button
            m_MeasureBtn = New Button

            'Set up the buttons look and feel
            m_MeasureBtn.BackColor = System.Drawing.SystemColors.Info
            m_MeasureBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
            m_MeasureBtn.FlatAppearance.BorderSize = 0
            m_MeasureBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            m_MeasureBtn.BackgroundImage = Global.MobileControls.My.Resources.Resources.measure
            m_MeasureBtn.Cursor = Cursors.Arrow
            m_MeasureBtn.Name = "btnMeasure"
            m_MeasureBtn.Size = New System.Drawing.Size(50, 50)

            m_MeasureBtn.UseVisualStyleBackColor = False
            'Add the handler for the zoom in/out button click
            AddHandler m_MeasureBtn.MouseClick, AddressOf btnMA_Click
            'add it to the map
            m_Map.Controls.Add(m_MeasureBtn)



        End If


        key = GlobalsFunctions.appConfig.NavigationOptions.ZoomPan.RotateVisible
        If UCase(key) = "TRUE" Then

            'create the Zoom in/out button
            m_RotLeftBtn = New Button

            'Set up the buttons look and feel
            m_RotLeftBtn.BackColor = System.Drawing.SystemColors.Info
            m_RotLeftBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
            m_RotLeftBtn.FlatAppearance.BorderSize = 0
            m_RotLeftBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            m_RotLeftBtn.BackgroundImage = Global.MobileControls.My.Resources.Resources.RotateLeft
            m_RotLeftBtn.Cursor = Cursors.Arrow
            m_RotLeftBtn.Name = "btnRotLeft"
            m_RotLeftBtn.Size = New System.Drawing.Size(50, 50)

            m_RotLeftBtn.UseVisualStyleBackColor = False
            'Add the handler for the zoom in/out button click
            AddHandler m_RotLeftBtn.MouseClick, AddressOf btnLeftRight_Click
            'add it to the map
            m_Map.Controls.Add(m_RotLeftBtn)





            'create the Zoom in/out button
            m_RotRightBtn = New Button

            'Set up the buttons look and feel
            m_RotRightBtn.BackColor = System.Drawing.SystemColors.Info
            m_RotRightBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
            m_RotRightBtn.FlatAppearance.BorderSize = 0
            m_RotRightBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            m_RotRightBtn.BackgroundImage = Global.MobileControls.My.Resources.Resources.RotateRight
            m_RotRightBtn.Cursor = Cursors.Arrow
            m_RotRightBtn.Name = "btnRotRight"
            m_RotRightBtn.Size = New System.Drawing.Size(50, 50)

            m_RotRightBtn.UseVisualStyleBackColor = False
            'Add the handler for the zoom in/out button click
            AddHandler m_RotRightBtn.MouseClick, AddressOf btnRotRight_Click
            'add it to the map
            m_Map.Controls.Add(m_RotRightBtn)



        End If


        key = GlobalsFunctions.appConfig.NavigationOptions.Bookmarks.Visible
        If UCase(key) = "TRUE" Then


            m_BookmarkBtn = New Button

            'Set up the buttons look and feel
            ' m_BookmarkBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)

            m_BookmarkBtn.BackColor = System.Drawing.SystemColors.Info
            m_BookmarkBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
            m_BookmarkBtn.FlatAppearance.BorderSize = 0
            m_BookmarkBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            m_BookmarkBtn.BackgroundImage = Global.MobileControls.My.Resources.Resources.Bookmark2
            m_BookmarkBtn.Cursor = Cursors.Arrow
            m_BookmarkBtn.Name = "btnBookmark"
            m_BookmarkBtn.Size = New System.Drawing.Size(50, 50)
            'm_BookmarkBtn.Location = New System.Drawing.Point(542, 572)
            m_BookmarkBtn.Tag = "Off"
            m_BookmarkBtn.UseVisualStyleBackColor = False
            'Add the handler for the zoom in/out button click
            'AddHandler m_BookmarkBtn.MouseClick, AddressOf btnBM_Click()
            'add it to the map
            m_Map.Controls.Add(m_BookmarkBtn)
            AddHandler m_BookmarkBtn.Click, AddressOf buttonShowBookmarksClick


            m_BookmarkForm = New CustomPanel
            m_BookmarkForm.Width = 350

            m_BookmarkForm.Name = "Bookmark"
            m_Map.Controls.Add(m_BookmarkForm)
            m_BookmarkForm.Visible = False
            m_BookmarkForm.BorderStyle = borderStyle.FixedSingle
            'm_GroupForm.BorderColor = Drawing.Pens.Black
            'm_GroupForm.BorderColor.Width = 3

            m_BookmarkForm.BackColor = m_BackColorBK

            Dim pBookMrk As Bookmarks = GlobalsFunctions.GetBookmarks()

            m_BkMark = New ComboBox
            m_BkMark.Font = m_scaleFontLarge
            m_BkMark.DropDownStyle = ComboBoxStyle.DropDownList

            m_BkMark.DataSource = pBookMrk.Bookmark
            m_BkMark.DisplayMember = "Name"
            m_BkMark.Width = m_BookmarkForm.Width - 9
            m_BkMark.Left = 3
            m_BkMark.Top = 3
            AddHandler m_BkMark.SelectedIndexChanged, AddressOf BookMarkSelectChange

            m_BookmarkForm.Controls.Add(m_BkMark)


            m_BkAddBtn = New Button
            m_BkAddBtn.Font = m_scaleFontLarge
            'pBtn.AutoSize = True
            m_BkAddBtn.Text = "Add"
            m_BkAddBtn.Name = "Add"
            m_BkAddBtn.Width = CInt(m_BookmarkForm.Width / 4)
            m_BkAddBtn.Height = 40
            m_BkAddBtn.Left = CInt((m_BookmarkForm.Width / 2) - (m_BkAddBtn.Width / 2) - 20 - m_BkAddBtn.Width)
            m_BkAddBtn.Top = m_BkMark.Height + m_BkMark.Top + 10
            m_BkAddBtn.Enabled = False
            AddHandler m_BkAddBtn.Click, AddressOf BookMarkButtonClick
            m_BookmarkForm.Controls.Add(m_BkAddBtn)


            m_BkEditBtn = New CheckBox

            m_BkEditBtn.Font = m_scaleFontLarge
            m_BkEditBtn.Text = "Edit"
            m_BkEditBtn.TextAlign = ContentAlignment.MiddleCenter
            m_BkEditBtn.Name = "Edit"
            m_BkEditBtn.Appearance = Appearance.Button
            'pBtn.AutoSize = True
            m_BkEditBtn.Width = CInt(m_BookmarkForm.Width / 4)
            m_BkEditBtn.Height = 40
            m_BkEditBtn.Left = CInt((m_BookmarkForm.Width / 2) - (m_BkEditBtn.Width / 2))
            m_BkEditBtn.Top = m_BkMark.Height + m_BkMark.Top + 10
            AddHandler m_BkEditBtn.Click, AddressOf BookMarkButtonClick
            m_BookmarkForm.Controls.Add(m_BkEditBtn)

            m_BkDelBtn = New Button
            m_BkDelBtn.Font = m_scaleFontLarge
            m_BkDelBtn.Text = "Del"
            m_BkDelBtn.Name = "Del"
            m_BkDelBtn.Enabled = False
            'pBtn.AutoSize = True
            m_BkDelBtn.Width = CInt(m_BookmarkForm.Width / 4)
            m_BkDelBtn.Height = 40
            m_BkDelBtn.Left = CInt((m_BookmarkForm.Width / 2) + (m_BkDelBtn.Width / 2) + 20)
            m_BkDelBtn.Top = m_BkMark.Height + m_BkMark.Top + 10
            AddHandler m_BkDelBtn.Click, AddressOf BookMarkButtonClick
            m_BookmarkForm.Controls.Add(m_BkDelBtn)



        End If
    End Sub

    Private Sub BookMarkSelectChange(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If (m_BkEditBtn.CheckState = CheckState.Unchecked) Then

            If m_BkMark.SelectedIndex = -1 Then
            Else
                Dim bkMk As BookmarkDetails = CType(m_BkMark.SelectedItem, BookmarkDetails)
                zoomTo(bkMk)
                bkMk = Nothing

            End If
        End If

    End Sub
    Private Sub BookMarkButtonClick(ByVal sender As System.Object, ByVal e As System.EventArgs)

        If TypeOf (sender) Is CheckBox Then
            If (CType(sender, CheckBox).CheckState = CheckState.Checked) Then

                m_BkAddBtn.Enabled = True
                m_BkDelBtn.Enabled = True
                m_BkMark.DropDownStyle = ComboBoxStyle.DropDown

                'CType(sender, CheckBox).CheckState = CheckState.Unchecked
            Else
                m_BkMark.DropDownStyle = ComboBoxStyle.DropDownList

                m_BkAddBtn.Enabled = False
                m_BkDelBtn.Enabled = False

                'CType(sender, CheckBox).CheckState = CheckState.Checked


            End If
        Else

            Select Case CType(sender, Button).Name
                Case "Del"
                    '  Dim strPreVal = m_BkMark.Text

                    Dim pBookMrk As Bookmarks = GlobalsFunctions.AddBookmarks(New BookmarkDetails(m_BkMark.Text, m_Map.Extent()), True)
                    m_BkMark.DataSource = pBookMrk.Bookmark
                    If pBookMrk.Bookmark.Count = 0 Then
                        m_BkMark.Text = ""
                    End If
                    ' m_BkMark.Text = strPreVal
                Case "Add"
                    If (m_BkMark.Text.Trim = "") Then Return

                    Dim strPreVal As String = m_BkMark.Text
                    Dim pBookMrk As Bookmarks = GlobalsFunctions.AddBookmarks(New BookmarkDetails(m_BkMark.Text, m_Map.Extent()), False)
                    m_BkMark.DataSource = pBookMrk.Bookmark
                    m_BkMark.Text = strPreVal
                Case "GoTo"
                    If m_BkMark.SelectedIndex = -1 Then
                    Else
                        Dim bkMk As BookmarkDetails = m_BkMark.SelectedItem
                        zoomTo(bkMk)
                        bkMk = Nothing

                    End If
            End Select

        End If
    End Sub
    Private Sub zoomTo(ByVal bookmark As BookmarkDetails)
        'Zoom to a feature
        Try
            If bookmark Is Nothing Then Return
            If bookmark.isEmpty Then Return

            Dim pEnv As Esri.ArcGIS.Mobile.Geometries.Envelope
            'If a point, center on it and create an envelope 50 around around it

            pEnv = New Esri.ArcGIS.Mobile.Geometries.Envelope(bookmark.XMin, bookmark.YMin, bookmark.XMax, bookmark.YMax)

            'Set the extent to the map
            m_Map.Extent = pEnv
            'Cleanup
            pEnv = Nothing
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub

    Private Sub zoomTo(ByVal coord As Esri.ArcGIS.Mobile.Geometries.Coordinate)
        'Zoom to a feature
        Try
            If coord Is Nothing Then Return
            If coord.IsEmpty Then Return

            Dim pEnv As Esri.ArcGIS.Mobile.Geometries.Envelope
            'If a point, center on it and create an envelope 50 around around it
            Dim pIntExtGeo As Integer = CInt(Esri.ArcGIS.Mobile.SpatialReferences.Unit.FromUnitToUnit(400, Esri.ArcGIS.Mobile.SpatialReferences.Unit.Foot, m_Map.SpatialReference.Unit))
            pEnv = New Esri.ArcGIS.Mobile.Geometries.Envelope(0, 0, pIntExtGeo, pIntExtGeo)
            pEnv.CenterAt(coord)



            'Set the extent to the map
            m_Map.Extent = pEnv
            'Cleanup
            pEnv = Nothing
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
    Private Sub btnRotRight_Click(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        RotateMap(90)
    End Sub
    Private Sub btnLeftRight_Click(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        RotateMap(-90)
    End Sub

    Private Sub btnGPS_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If GlobalsFunctions.m_GPS Is Nothing Then Return
        If GlobalsFunctions.m_GPS.GpsConnection Is Nothing Then Return
        'recenter if the user right clicks
        If e.Button = MouseButtons.Right Then
            zoomTo(m_Map.SpatialReference.FromWgs84(GlobalsFunctions.m_GPS.GpsConnection.Longitude, GlobalsFunctions.m_GPS.GpsConnection.Latitude))

        ElseIf e.Clicks > 1 Then
            zoomTo(m_Map.SpatialReference.FromWgs84(GlobalsFunctions.m_GPS.GpsConnection.Longitude, GlobalsFunctions.m_GPS.GpsConnection.Latitude))

        Else

            'if the gps is open, close it and reset button image
            If GlobalsFunctions.m_GPS.GpsConnection.IsOpen Then
                GlobalsFunctions.m_GPS.GpsConnection.Close()
                CType(sender, Button).BackgroundImage = My.Resources.SatBlue
                If m_t IsNot Nothing Then
                    If m_t.IsAlive Then
                        m_t.Abort()

                    End If
                End If
            Else

                Try
                    'Change the GPS button to show it is trying to activate
                    CType(sender, Button).BackgroundImage = My.Resources.SatRed
                    'Show the gps activating icon
                    m_GPSLoadingPic.Visible = True

                    'Create a new thread to open the GPS
                    m_t = New Thread(AddressOf Me.OpenGPS)
                    'start the thread
                    m_t.Start()
                Catch ex As Exception
                    'Reset the image back to GPS off
                    CType(sender, Button).BackgroundImage = My.Resources.SatBlue
                End Try

            End If
        End If

    End Sub
    Private Delegate Sub DelegatehidePicBox()
    Private Sub HidePicBox()
        'Hide the GPS loading box
        m_GPSLoadingPic.Visible = False

    End Sub
    Private Sub OpenGPS()
        Try
            'Try to open the GPS
            GlobalsFunctions.m_GPS.GpsConnection.Open()

        Catch ex As Exception
            Try
                Dim st As New StackTrace
                MsgBox(GlobalsFunctions.appConfig.NavigationOptions.UIComponents.GPSNoConnect, MsgBoxStyle.Exclamation, "GPS")

                st = Nothing

                If m_GPSLoadingPic.InvokeRequired Then
                    m_GPSLoadingPic.Invoke(New DelegatehidePicBox(AddressOf HidePicBox))

                End If

                'Rasie the event that the GPS time outed
                RaiseEvent GPSTimeout()


            Catch exinner As Exception

            End Try
        End Try



    End Sub
    Public Sub FixZoom(ByVal bOut As Boolean)
        'Make sure the map is valid
        If m_Map Is Nothing Then Return
        If m_Map.IsValid = False Then Return
        'Determine which direction to zoom
        If (bOut) Then
            Dim pExt As Esri.ArcGIS.Mobile.Geometries.Envelope
            'Get the map extent
            pExt = m_Map.Extent()
            'Resize it
            pExt.Resize(1.5)
            'Center it
            pExt.CenterAt(pExt.Center().X, pExt.Center().Y)

            'Set the map extent
            m_Map.Extent = pExt
            'cleanup
            pExt = Nothing
        Else
            Dim pExt As Esri.ArcGIS.Mobile.Geometries.Envelope
            'Get the map extent
            pExt = m_Map.Extent()
            'Resize it
            pExt.Resize(0.5)
            'Center it
            pExt.CenterAt(pExt.Center().X, pExt.Center().Y)
            'Set the map extent
            m_Map.Extent = pExt
            'cleanup
            pExt = Nothing
        End If


    End Sub
    Public Sub RotateMap(ByVal angle As Double)
        'Make sure the map is valid
        If m_Map Is Nothing Then Return
        If m_Map.IsValid = False Then Return
        m_Map.RotationAngle = m_Map.RotationAngle + angle
        If m_Map.RotationAngle = 360 Then
            m_Map.RotationAngle = 0
        ElseIf m_Map.RotationAngle > 360 Then
            m_Map.RotationAngle = m_Map.RotationAngle - 360
        ElseIf m_Map.RotationAngle < 0 Then
            m_Map.RotationAngle = m_Map.RotationAngle + 360
        End If
        If m_Map.RotationAngle <> 0 Then
            m_picBox.Visible = True
            Dim oldImage As Image = m_picBox.Image
            m_picBox.Image = GlobalsFunctions.RotateImage(My.Resources.NorthArrow2, CSng(m_Map.RotationAngle))
            m_picBox.Width = m_picBox.Image.Width
            m_picBox.Height = m_picBox.Image.Height
            If oldImage IsNot Nothing Then
                oldImage.Dispose()

            End If


        Else
            m_picBox.Visible = False

        End If


    End Sub
#End Region
#Region "Events"
    Private Sub m_FileGPS_GpsClosed(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_FileGPS.GpsClosed
        'Handles the GPS Closed event, updates the GPS icon
        Try

            'Change the GPS button to show the inactive GPS icon

            m_GPSBtn.BackgroundImage = My.Resources.SatBlue
            'Invoke the parent thread of hte loading box if requried
            If m_GPSLoadingPic.InvokeRequired Then
                m_GPSLoadingPic.Invoke(New DelegatehidePicBox(AddressOf HidePicBox))
            Else
                HidePicBox()
            End If
            If m_Map.TopLevelControl.InvokeRequired Then
                m_Map.TopLevelControl.Invoke(New DelGPS(AddressOf StopGPS))

            Else
                StopGPS()

            End If
            RaiseEvent RaiseStatMessage("", True)
        Catch ex As Exception
            If m_Map.TopLevelControl.InvokeRequired Then
                m_Map.TopLevelControl.Invoke(New DelGPS(AddressOf StopGPS))
            Else
                StopGPS()

            End If

        End Try
    End Sub
    'Private Delegate Sub DelegateStartGPS()

    'Private Sub StartGPS()
    '    If m_LogGPS Then

    '        m_GPSTimer.Start()

    '    End If
    'End Sub
    Private Delegate Sub DelGPS()
    Private Sub StartGPS()
        RaiseEvent GPSStarted()

        If m_LogGPS Then


            Dim timerDelegate As TimerCallback = AddressOf GPSLogger_Tick
            m_GPSTimer = New System.Threading.Timer(timerDelegate, "GPS", 0, m_LogInterval * 1000)
        End If
    End Sub

    Private Sub StopGPS()
        RaiseEvent GPSStopped()

        If m_GPSTimer IsNot Nothing Then
            m_GPSTimer.Dispose()
        End If
        m_GPSTimer = Nothing


    End Sub
    Private Sub m_FileGPS_GpsOpened(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_FileGPS.GpsOpened

        Try

            'Change the GPS button to show the active GPS icon
            m_GPSBtn.BackgroundImage = My.Resources.SatGreen
            'Invoke the parent thread to hide the initializing GPS box
            
            If m_GPSLoadingPic.InvokeRequired Then
                m_GPSLoadingPic.Invoke(New DelegatehidePicBox(AddressOf HidePicBox))
                ' m_GPSLoadingPic.Invoke(New DelegateStartGPS(AddressOf StartGPS))
            Else
                HidePicBox()

            End If
            If m_Map.TopLevelControl.InvokeRequired Then
                m_Map.TopLevelControl.Invoke(New DelGPS(AddressOf StartGPS))


            End If

            '  m_FileGPS.ReadInterval = 100

        Catch ex As Exception

        End Try

    End Sub


    Private Sub m_SerialGPS_GpsClosed(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_SerialGPS.GpsClosed
        Try

            'Change the GPS button to show the active GPS icon

            m_GPSBtn.BackgroundImage = My.Resources.SatBlue
            'Invoke the parent thread to hide the initializing GPS box 
            If m_GPSLoadingPic.InvokeRequired Then
                m_GPSLoadingPic.Invoke(New DelegatehidePicBox(AddressOf HidePicBox))

            Else
                HidePicBox()
            End If
            If m_Map.TopLevelControl.InvokeRequired Then
                m_Map.TopLevelControl.Invoke(New DelGPS(AddressOf StopGPS))

            Else
                StopGPS()

            End If
            RaiseEvent RaiseStatMessage("", True)

        Catch ex As Exception
            If m_Map.TopLevelControl.InvokeRequired Then
                m_Map.TopLevelControl.Invoke(New DelGPS(AddressOf StopGPS))


            End If

        End Try
    End Sub

    Private Sub m_SerialGPS_GpsError(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.Gps.GpsErrorEventArgs) Handles m_SerialGPS.GpsError
        '  MsgBox(e.Exception.Message.ToString)

    End Sub
    Private Sub m_SerialGPS_GpsOpened(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_SerialGPS.GpsOpened
        Try
            'Change the GPS button to show the active GPS icon

            m_GPSBtn.BackgroundImage = My.Resources.SatGreen
            'Invoke the parent thread to hide the initializing GPS box 
            If m_GPSLoadingPic.InvokeRequired Then
                m_GPSLoadingPic.Invoke(New DelegatehidePicBox(AddressOf HidePicBox))

            End If
            'Dim timerDelegate As TimerCallback = AddressOf CheckStatus
            'm_GPSTimer = New System.Threading.Timer(timerDelegate, "GPS", m_LogInterval * 100, m_LogInterval * 100)

            'If m_LogGPS Then

            '    m_GPSTimer.Start()

            If m_Map.TopLevelControl.InvokeRequired Then
                m_Map.TopLevelControl.Invoke(New DelGPS(AddressOf StartGPS))


            End If

            'End If
        Catch ex As Exception

        End Try

    End Sub
    Private Sub mobileNavigation_GPSTimeout() Handles Me.GPSTimeout
        Try
            'The app timed out initializing the GPS
            m_GPSBtn.BackgroundImage = My.Resources.SatBlue
            'Invoke the parent thread to hide the initializing GPS box 
            If m_GPSLoadingPic.InvokeRequired Then
                m_GPSLoadingPic.Invoke(New DelegatehidePicBox(AddressOf HidePicBox))
            Else
                HidePicBox()

            End If
            If m_Map.TopLevelControl.InvokeRequired Then
                m_Map.TopLevelControl.Invoke(New DelGPS(AddressOf StopGPS))


            End If

        Catch ex As Exception

        End Try


    End Sub
    Private Sub m_PanMA_StatusChanged(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.WinForms.MapActionStatusChangedEventArgs) Handles m_PanMA.StatusChanged
        'Changes the pan button based on the status of the pan map action
        If e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Activated Then
            m_PanBtn.BackgroundImage = My.Resources.PanDown
            ' m_Map.Cursor = Cursors.Hand
        ElseIf e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Deactivated Then
            m_PanBtn.BackgroundImage = My.Resources.Pan

        End If
    End Sub
    Private Sub m_ZoomOutMA_StatusChanged(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.WinForms.MapActionStatusChangedEventArgs) Handles m_ZoomOutMA.StatusChanged
        'Changes the pan button based on the status of the pan map action
        If e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Activated Then
            m_ZoomOutBtn.BackgroundImage = My.Resources.ZoomOutDown
            ' m_Map.Cursor = Cursors.Hand
        ElseIf e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Deactivated Then
            m_ZoomOutBtn.BackgroundImage = My.Resources.ZoomOut

        End If
    End Sub
    Private Sub m_ZoomInMA_StatusChanged(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.WinForms.MapActionStatusChangedEventArgs) Handles m_ZoomInMA.StatusChanged
        'Changes the pan button based on the status of the pan map action
        If e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Activated Then
            m_ZoomInBtn.BackgroundImage = My.Resources.ZoomInDown
            ' m_Map.Cursor = Cursors.Hand
        ElseIf e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Deactivated Then
            m_ZoomInBtn.BackgroundImage = My.Resources.ZoomIn

        End If
    End Sub

    Private Sub m_MeasureMA_StatusChanged(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.WinForms.MapActionStatusChangedEventArgs) Handles m_MeasureMA.StatusChanged
        'Changes the pan button based on the status of the Zoom in out map action

        If e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Activated Then
            m_MeasureBtn.BackgroundImage = My.Resources.measureDown
        ElseIf e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Deactivated Then
            m_MeasureBtn.BackgroundImage = My.Resources.measure

        End If
    End Sub
    Private Sub m_ZoomInOutMA_StatusChanged(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.WinForms.MapActionStatusChangedEventArgs) Handles m_ZoomInOutMA.StatusChanged
        'Changes the pan button based on the status of the Zoom in out map action

        If e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Activated Then
            m_ZoomInOutBtn.BackgroundImage = My.Resources.ZoomInOutDown
        ElseIf e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Deactivated Then
            m_ZoomInOutBtn.BackgroundImage = My.Resources.ZoomInOut

        End If
    End Sub
    Private Sub m_Map_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_Map.Resize
        'handles the map resize envet
        Resize()
        MaxExtent()
        reLocateButtonsGrouped()
    End Sub
    Private Sub reLocateButtonsGrouped()

        If m_BookmarkBtn IsNot Nothing Then
            CType(m_BookmarkBtn, Button).Left = m_Map.Width - 35 - CType(m_BookmarkBtn, Button).Width
            CType(m_BookmarkBtn, Button).Top = m_Map.Height - 85 - CType(m_BookmarkBtn, Button).Height
            m_BookmarkForm.Left = m_BookmarkBtn.Left - m_BookmarkForm.Width - 15
            m_BookmarkForm.Top = m_BookmarkBtn.Top - m_BookmarkForm.Height

        End If


    End Sub
    Private Sub buttonShowBookmarksClick(ByVal sender As Object, ByVal e As System.EventArgs)

        If m_BookmarkBtn.Tag = "Off" Then
            turnOffCustomPanels(m_BookmarkForm)

            m_BookmarkBtn.BackgroundImage = My.Resources.BookmarkDown2
            m_BookmarkBtn.Tag = "On"
            m_BookmarkForm.Visible = True
            ' reLocateButtonsCenter(True, m_GroupForm)

        Else
            m_BookmarkBtn.BackgroundImage = My.Resources.Bookmark2
            m_BookmarkBtn.Tag = "Off"
            m_BookmarkForm.Visible = False
            'reLocateButtonsCenter(False, m_GroupForm)
        End If
    End Sub
    Private Sub m_ZoomTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs)
        'Preform the zoom on the ick
        FixZoom(m_bZoomOut)
    End Sub
    Private Sub btnZoomIn_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If Not m_FixZoom Then
        Else


            'Change the background image to show the button is presseed
            CType(sender, Button).BackgroundImage = My.Resources.ZoomInDown
            m_bZoomOut = False
            'Zoom 
            FixZoom(m_bZoomOut)
            'Start the timer to handle multi zooms
            m_ZoomTimer.Start()
        End If

    End Sub
    Private Sub btnZoomOut_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If Not m_FixZoom Then
        Else

            'Change the background image to show the button is presseed
            CType(sender, Button).BackgroundImage = My.Resources.ZoomOutDown

            m_bZoomOut = True
            'Zoom 
            FixZoom(m_bZoomOut)
            'Start the timer to handle multi zooms
            m_ZoomTimer.Start()
        End If

    End Sub
    '  Private Sub btnZoomExt_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
    Private Sub btnZoomExt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            RemoveHandler m_Map.ExtentChanged, AddressOf m_Map_ExtentChanged

            If sender.name = "btnZoomPrev" Then
                If m_LastExtents.Count = 1 Then Return


                m_CurrentStep = m_CurrentStep - 1
                If m_CurrentStep < 0 Then
                    m_CurrentStep = 0

                End If
                m_Map.Extent = m_LastExtents.Item(m_CurrentStep)


            Else
                If m_CurrentStep + 1 = m_LastExtents.Count Then
                    Return

                End If
                m_CurrentStep = m_CurrentStep + 1
                If m_CurrentStep >= m_LastExtents.Count Then
                    m_CurrentStep = m_LastExtents.Count - 1

                End If
                m_Map.Extent = m_LastExtents.Item(m_CurrentStep)

            End If
        Catch ex As Exception

        Finally
            AddHandler m_Map.ExtentChanged, AddressOf m_Map_ExtentChanged
        End Try


    End Sub
    Private Sub btnZoomOut_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If Not m_FixZoom Then
            btnMA_Click(sender, Nothing)
        Else
            'Change the background image to show the button is released
            CType(sender, Button).BackgroundImage = My.Resources.ZoomOut
            'Stop the Timer
            m_ZoomTimer.Stop()
        End If


    End Sub
    Private Sub btnZoomIn_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If Not m_FixZoom Then
            btnMA_Click(sender, Nothing)
        Else

            'Stop the Timer
            m_ZoomTimer.Stop()
            'Change the background image to show the button is released

            CType(sender, Button).BackgroundImage = My.Resources.ZoomIn
        End If


    End Sub
    Private Sub btnFullExtent_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        GlobalsFunctions.zoomTo(m_Map.FullExtent.ToPolygon, m_Map)

    End Sub

    Private Sub btnMA_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'Handles that pan and zoomin out click 
        Select Case CType(sender, Button).Name
            Case "btnPan"
                'Deactivate/Activate the pan map action
                If m_Map.MapAction Is m_PanMA Then
                    'm_Map.MapAction = m_PanMA
                    'RaiseEvent ToolChange(ToolType.Pan)
                Else
                    m_Map.MapAction = m_PanMA
                    RaiseEvent ToolChange(ToolType.Pan)
                End If
            Case "btnZoomInOut"
                'Deactivate/Activate the zoom in/out action
                If m_Map.MapAction Is m_ZoomInOutMA Then
                    m_Map.MapAction = m_PanMA
                    RaiseEvent ToolChange(ToolType.Pan)
                Else
                    m_Map.MapAction = m_ZoomInOutMA
                    RaiseEvent ToolChange(ToolType.ZoomInOut)
                End If


            Case "btnMeasure"
                'Deactivate/Activate the zoom in/out action
                If m_Map.MapAction Is m_MeasureMA Then
                    m_Map.MapAction = m_PanMA
                    RaiseEvent ToolChange(ToolType.Pan)
                Else
                    m_Map.MapAction = m_MeasureMA
                    RaiseEvent ToolChange(ToolType.Measure)
                End If

            Case "btnZoomOut"
                'Deactivate/Activate the zoom in/out action
                If m_Map.MapAction Is m_ZoomOutMA Then
                    m_Map.MapAction = m_PanMA
                    RaiseEvent ToolChange(ToolType.Pan)
                Else
                    m_Map.MapAction = m_ZoomOutMA
                    RaiseEvent ToolChange(ToolType.ZoomOut)
                End If
            Case "btnZoomIn"
                'Deactivate/Activate the zoom in/out action
                If m_Map.MapAction Is m_ZoomInMA Then
                    m_Map.MapAction = m_PanMA
                    RaiseEvent ToolChange(ToolType.Pan)
                Else
                    m_Map.MapAction = m_ZoomInMA
                    RaiseEvent ToolChange(ToolType.ZoomIn)
                End If
        End Select
    End Sub
#End Region
    Private Sub GPS_GpsChanged(sender As Object, e As System.EventArgs) Handles m_SerialGPS.GpsChanged, m_FileGPS.GpsChanged

        If CInt(GlobalsFunctions.m_GPS.GpsConnection.FixStatus) = CInt(GlobalsFunctions.appConfig.NavigationOptions.GPS.GPSFixType) Then
            m_GPSBtn.BackgroundImage = My.Resources.SatGreen
        Else
            m_GPSBtn.BackgroundImage = My.Resources.SatRed
        End If

        RaiseEvent RaiseStatMessage(GlobalsFunctions.GPSFixToText(GlobalsFunctions.m_GPS.GpsConnection.FixStatus), False)


        '0 - No Fix, 1 - Gps Fix, 2 - DGps Fix, 3 = PPS fix, 4 = Real Time Kinematic, 5 = Float RTK, 6 = estimated (dead reckoning), 7 = Manual input mode, 8 = Simulation mode
    End Sub
    Private Sub GPS_PropertyChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs) Handles m_SerialGPS.PropertyChanged, m_FileGPS.PropertyChanged

    End Sub


    Private Sub GPS_SentenceReceived(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.Gps.NmeaSentenceEventArgs) Handles m_FileGPS.SentenceReceived, m_SerialGPS.SentenceReceived
        '     MsgBox(e.Sentence)

    End Sub
    Private Sub m_GPS_Disposed(ByVal sender As Object, ByVal e As System.EventArgs)
        If GlobalsFunctions.m_GPS.GpsConnection IsNot Nothing Then

            If GlobalsFunctions.m_GPS.GpsConnection.IsOpen Then
                GlobalsFunctions.m_GPS.GpsConnection.Close()
                RaiseEvent RaiseStatMessage("", True)

            End If

        End If
    End Sub


    Protected Overrides Sub Finalize()
        If GlobalsFunctions.m_GPS IsNot Nothing Then

            If GlobalsFunctions.m_GPS.GpsConnection IsNot Nothing Then

                If GlobalsFunctions.m_GPS.GpsConnection.IsOpen Then
                    GlobalsFunctions.m_GPS.GpsConnection.Close()

                End If
            End If

        End If
        MyBase.Finalize()

    End Sub

    Private Sub m_BookmarkForm_VisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_BookmarkForm.VisibleChanged
        If m_BookmarkForm.Visible = False Then
            m_BookmarkBtn.Tag = "Off"

            m_BookmarkBtn.BackgroundImage = My.Resources.Bookmark2
            m_BookmarkBtn.Tag = "Off"

        End If
    End Sub
    Private Sub turnOffCustomPanels(ByVal exclude As Control)

        For Each ct As Control In m_Map.Controls

            If TypeOf (ct) Is CustomPanel And ct IsNot exclude Then
                ct.Visible = False

            End If
        Next

    End Sub



    Private Sub m_GPSAvgTool_GoodPositionAcquired(sender As Object, e As System.EventArgs) Handles m_GPSAvgTool.GoodPositionAcquired
        Dim str As String = ""
    End Sub

    Private Sub m_GPSAvgTool_GpsQualityChanged(sender As Object, e As System.EventArgs) Handles m_GPSAvgTool.GpsQualityChanged

    End Sub

    Private Sub m_FileGPS_GpsOpening(sender As Object, e As System.EventArgs) Handles m_FileGPS.GpsOpening

    End Sub

    Private Sub m_FileGPS_GpsError(sender As Object, e As Esri.ArcGIS.Mobile.Gps.GpsErrorEventArgs) Handles m_FileGPS.GpsError
        ' MsgBox(e.Exception.Message)
    End Sub
End Class
