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


Imports Esri.ArcGIS.Mobile.WinForms.MapActions
Imports Esri.ArcGIS.Mobile
Imports Esri.ArcGIS.Mobile.FeatureCaching
Imports Esri.ArcGIS.Mobile.Geometries
Imports System.Windows.Forms

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.ComponentModel
Imports System.Drawing

Imports MobileControls
Imports Esri.ArcGIS.Mobile.SpatialReferences
Imports Esri.ArcGISTemplates


Public Class MeasureMapAction
    Inherits Esri.ArcGIS.Mobile.WinForms.MapAction


    Private m_tempValueSet As Boolean = False
    'used when temporarily overwrriding units displayed next to the measure using key strokes 
    Private m_OldUnit As Unit
    'when overriding displayed units stores originally used unit
    Private m_mousePosition As Coordinate
    ' stores current mouse position
    Private m_coordinates As CoordinateCollection
    ' stores all coordinates used in drawing the measure line
    Private m_unit As Unit = If(System.Globalization.RegionInfo.CurrentRegion.IsMetric, Unit.Meter, Unit.Foot)

    Private m_lineColor As Color = Color.Red

    ' Private m_fillColor As Color = Color.Blue

    ' Private m_transparency As Integer = 255

    Private m_lineWidth As Integer = 2

    Private m_font As New Font("Arial", 10.0F, FontStyle.Bold)

    Private m_fontColor As Color = Color.Black

    Private m_brush As SolidBrush

    Private m_measureMethod As EsriMeasureMethod = EsriMeasureMethod.MultiPoint
    ' measure method can be either two points or multi point
    Private m_showIndividualSegments As Boolean = False

    Private m_digits As Integer = 2
    ' determines the precision of the distance shown in the 
    Private m_sformat As StringFormat
    Private m_format As String
    ' Private m_f As Font
#Region "Public Methods"
    ''' <summary>
    ''' Initializes a new instance of MeasureMapAction.
    ''' </summary>
    <Description("Measures distance between features on the map")> _
    Public Sub New()
        MyBase.New()
        m_coordinates = New CoordinateCollection()
        initVaris()
    End Sub

    ''' <summary>
    ''' Initializes a new instance of IdentifyMapAction.
    ''' </summary>
    Public Sub New(ByVal container As IContainer)
        MyBase.New()
        If container Is Nothing Then
            Throw New ArgumentNullException("container")
        End If
        container.Add(Me)
        initVaris()
    End Sub
    Private Sub initVaris()

        m_sformat = New StringFormat(StringFormatFlags.NoClip)

        m_sformat.Alignment = StringAlignment.Center
        m_sformat.LineAlignment = StringAlignment.Center
        'Dim m_f = Font(m_font.Name, m_font.Size, m_font.Style)

        m_format = "########0."
        For i As Integer = 0 To m_digits - 1
            m_format += "0"
        Next

    End Sub
    'Public Function addMeasureButton(Optional ByVal TopX As Integer = -1, Optional ByVal TopY As Integer = -1) As Boolean
    '    Try
    '        'Create and add the button to toggle the redline map action
    '        If m_btn Is Nothing Then
    '            'Create a new button
    '            m_btn = New Button
    '            With m_btn
    '                'Set the buttons look and feel
    '                .BackColor = System.Drawing.SystemColors.Info
    '                .BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
    '                .FlatAppearance.BorderSize = 0
    '                .FlatStyle = System.Windows.Forms.FlatStyle.Flat
    '                .BackgroundImage = My.Resources.measure
    '                .Name = "btnMeasure"
    '                .Size = New System.Drawing.Size(50, 50)
    '                .UseVisualStyleBackColor = False
    '                'Locate the button
    '                TopX = MyBase.Map.Width - 80
    '                TopY = 85
    '                .Location = New System.Drawing.Point(TopX, TopY)
    '                'Add the handler for relocating the button
    '                AddHandler MyBase.Map.Resize, AddressOf resize
    '                'Add a handler for the click event
    '                AddHandler .MouseClick, AddressOf measureBtnClick

    '            End With
    '            'Add the button to the map
    '            MyBase.Map.Controls.Add(m_btn)
    '        End If

    '    Catch ex As Exception
    '        MsgBox("Error creating Measure button" & vbCrLf & ex.Message)

    '        Return False
    '    End Try
    '    Return True

    'End Function
    'Public Sub InitMeasureForm(ByVal container As Control)
    '    Try

    '        If m_MeasureForm Is Nothing Then
    '            'Create a new edit control
    '            'm_MeasureForm = New MobileControls.MeasureForm(MyBase.Map, Nothing)
    '            m_MeasureForm = New MobileControls.MeasureForm()
    '            'Fill the containter
    '            m_MeasureForm.Dock = DockStyle.Fill
    '            'Tell the panel to draw the geometry
    '            'm_MeasureForm.DrawGeo = True
    '            'Clear the controls on the cotainer
    '            container.Controls.Clear()
    '            'Add the edit panel
    '            container.Controls.Add(m_MeasureForm)
    '            'Create a new row on the panel

    '        End If

    '    Catch ex As Exception
    '        MsgBox("Error initializing Redline Attribute form" & vbCrLf & ex.Message)
    '    End Try

    'End Sub

#End Region
#Region "Private Methods"

#End Region
#Region "Events"
    Protected Overrides Sub OnMapMouseDown(ByVal e As Esri.ArcGIS.Mobile.MapMouseEventArgs)
        ' MyBase.OnMapMouseDown(e)
        Map.Focus()


        If (e.MapCoordinate Is Nothing) Then
            Return
        End If

        ' signal to other mapactions that the action is executing
        ActionInProgress = True

        If (m_coordinates.Count = 0) Then

            m_coordinates.Add(e.MapCoordinate)
            m_coordinates.Add(e.MapCoordinate)

        ElseIf (m_coordinates.Count = 2) Then

            If (m_measureMethod = EsriMeasureMethod.MultiPoint) Then
                m_coordinates.Insert(1, e.MapCoordinate)
            End If

        ElseIf (m_measureMethod = EsriMeasureMethod.MultiPoint) Then
            m_coordinates.Insert(m_coordinates.Count - 1, e.MapCoordinate)

        End If

    End Sub
    Protected Overrides Sub OnMapMouseUp(ByVal e As Esri.ArcGIS.Mobile.MapMouseEventArgs)
        MyBase.OnMapMouseUp(e)

    End Sub
    Protected Overrides Sub OnMapMouseMove(ByVal e As Esri.ArcGIS.Mobile.MapMouseEventArgs)
        'MyBase.OnMapMouseMove(e)
        m_mousePosition = e.MapCoordinate

        If (m_coordinates.Count < 2) Then
            Return
        End If

        'freehand operates only when left mouse button is pressed
        If (m_measureMethod = EsriMeasureMethod.Freehand And e.Button = MouseButtons.Left) Then
            m_coordinates.Add(m_mousePosition)
        Else
            m_coordinates(m_coordinates.Count - 1) = m_mousePosition
        End If

        If (Map IsNot Nothing) Then
            Map.Invalidate()
        End If

    End Sub
    Private Sub MapKeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)

        MyBase.OnMapKeyDown(e)
        If e.KeyValue <> 67 AndAlso e.KeyValue <> 70 AndAlso e.KeyValue <> 73 AndAlso e.KeyValue <> 75 AndAlso e.KeyValue <> 76 AndAlso e.KeyValue <> 77 OrElse m_tempValueSet = True Then
            Return
        End If
        m_tempValueSet = True
        m_OldUnit = m_unit

        Select Case e.KeyValue
            Case 67
                m_unit = Unit.Centimeter
                Exit Select
            Case 70
                m_unit = Unit.Foot
                Exit Select
            Case 73
                m_unit = Unit.Inch
                Exit Select
            Case 75
                m_unit = Unit.Kilometer
                Exit Select
            Case 76
                m_unit = Unit.Mile
                Exit Select
            Case 77
                m_unit = Unit.Meter
                Exit Select
        End Select
        Map.Invalidate()

    End Sub
    Private Sub MapKeyUp(ByVal sender As Object, ByVal e As KeyEventArgs)
        'MyBase.OnMapKeyUp(e)
        ' finish measure unit override
        If e.KeyValue <> 67 AndAlso e.KeyValue <> 70 AndAlso e.KeyValue <> 73 AndAlso e.KeyValue <> 75 AndAlso e.KeyValue <> 76 AndAlso e.KeyValue <> 77 Then
            Return
        End If

        m_tempValueSet = False
        m_unit = m_OldUnit
        Map.Invalidate()

    End Sub
    Private Sub MouseDoubleClick(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.MapMouseEventArgs)
        m_coordinates = New CoordinateCollection()
        Map.Invalidate()
        ' signal to other mapactions that the action is completed
        ActionInProgress = False
    End Sub
    'Public Function ConvertDistance(ByVal distance As Double, ByVal sourceUnit As Unit, ByVal targetUnit As Unit)
    '    Try

    '        If sourceUnit Is targetUnit Then
    '            Return distance

    '        End If
    '        Dim strUnit As String = sourceUnit.ToString()

    '        Select Case sourceUnit.ToString()
    '            Case "Meter"
    '                strUnit = "Meter"
    '            Case "Centimeter"
    '                distance = distance * 100
    '                strUnit = "Meter"
    '            Case "Millimeter"
    '                distance = distance * 1000
    '                strUnit = "Meter"
    '            Case "Kilometer"
    '                distance = distance * 0.001
    '                strUnit = "Meter"
    '            Case "Mile"
    '                distance = distance * 5280
    '                strUnit = "Foot"
    '            Case "Yard"
    '                distance = distance * 3
    '                strUnit = "Foot"
    '            Case "NauticalMile"
    '                distance = distance * 6076.115
    '                strUnit = "Foot"
    '        End Select

    '        Select Case strUnit
    '            Case "Meter"

    '                If targetUnit Is Unit.Centimeter Then
    '                    Return distance * 100

    '                ElseIf targetUnit Is Unit.Degree Then
    '                    Return distance

    '                ElseIf targetUnit Is Unit.Foot Then
    '                    Return distance * 3.2808399
    '                ElseIf targetUnit Is Unit.Inch Then
    '                    Return distance * 3.2808399 * 12

    '                ElseIf targetUnit Is Unit.Kilometer Then
    '                    Return distance * 0.001

    '                ElseIf targetUnit Is Unit.Mile Then
    '                    Return distance * 0.000621371192
    '                ElseIf targetUnit Is Unit.Millimeter Then
    '                    Return distance * 1000
    '                ElseIf targetUnit Is Unit.Yard Then
    '                    Return distance * 1.0936133

    '                ElseIf targetUnit Is Unit.NauticalMile Then
    '                    Return distance * 0.000539956803
    '                Else
    '                    Return distance



    '                End If
    '            Case "Foot"

    '                If targetUnit Is Unit.Centimeter Then
    '                    Return distance * 30.48
    '                ElseIf targetUnit Is Unit.Degree Then
    '                    Return distance
    '                ElseIf targetUnit Is Unit.Foot Then
    '                    Return distance
    '                ElseIf targetUnit Is Unit.Meter Then
    '                    Return distance * 0.3048
    '                ElseIf targetUnit Is Unit.Inch Then
    '                    Return distance * 12

    '                ElseIf targetUnit Is Unit.Kilometer Then
    '                    Return distance * 0.0003048

    '                ElseIf targetUnit Is Unit.Mile Then
    '                    Return distance * 0.000189393939
    '                ElseIf targetUnit Is Unit.Millimeter Then
    '                    Return distance * 304.8
    '                ElseIf targetUnit Is Unit.Yard Then
    '                    Return distance * 0.0333333333

    '                ElseIf targetUnit Is Unit.NauticalMile Then
    '                    Return distance * 0.000164578834
    '                Else
    '                    Return distance



    '                End If

    '        End Select

    '        Return distance

    '    Catch ex As Exception
    '        Return distance

    '    End Try
    'End Function
    Private Sub MapPaint(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.WinForms.MapPaintEventArgs)
        If m_coordinates.Count < 2 Then
            Return
        End If
        Dim g As Graphics = e.Graphics



        Dim distance As Double = 0
        For i As Integer = 0 To m_coordinates.Count - 2
            distance += GlobalsFunctions.SegmentMeasures(Map, m_unit, m_coordinates(i), m_coordinates(i + 1))
            If m_showIndividualSegments OrElse m_measureMethod = EsriMeasureMethod.TwoPoint Then

                drawMeasurement(GlobalsFunctions.SegmentMeasures(Map, m_unit, m_coordinates(i), m_coordinates(i + 1)), m_coordinates(i), m_coordinates(i + 1), e.MapSurface)

            End If
        Next
        'double ddistance = Map.SpatialReference.MobileToServerDistance(distance, m_unit);

        Dim midpoint As Integer = CInt(m_coordinates.Count) \ 2
        Dim c As Coordinate
        c = m_mousePosition
        ' Dim pPt As System.Drawing.Point = Map.ToClient(c)

        'Dim format As String = "########0."
        'For i As Integer = 0 To m_digits - 1
        '    format += "0"
        'Next
        e.MapSurface.Graphics.ResetTransform()

        Dim scrpnt As System.Drawing.Point = e.MapSurface.Map.ToClient(m_coordinates(0))
        Dim pC As Coordinate = e.MapSurface.Map.ToMap(scrpnt)


        e.MapSurface.DrawPolyline(New Pen(m_lineColor, m_lineWidth), New Polyline(m_coordinates))
        Dim total As String = ""
        Dim displayString As String
        displayString = distance.ToString(m_format) & " " & m_unit.ToString()
        displayString = displayString.Replace("Foot", "Feet")
        displayString = displayString.Replace("Yard", "Yards")
        displayString = displayString.Replace("Mile", "Miles")
        displayString = displayString.Replace(" Kilometers", " Kilometers")
        displayString = displayString.Replace(" Meter", " Meters")

        If m_measureMethod = EsriMeasureMethod.MultiPoint Then

            If m_showIndividualSegments Then
                displayString = displayString & " total."
            End If


            Dim size As SizeF = e.Graphics.MeasureString(displayString, m_font)
        End If
        'Dim f As New Font(m_font.Name, m_font.Size, m_font.Style)
        'Dim sformat As New StringFormat(StringFormatFlags.NoClip)
        'sformat.Alignment = StringAlignment.Center
        Dim angle As Single = 0
        Dim Middle As System.Drawing.Point = Map.ToClient(c)

        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias
        'sformat.LineAlignment = StringAlignment.Center
        If m_measureMethod = EsriMeasureMethod.Freehand Then
            'e.Display.DrawText(distance.ToString(m_format) & " " & m_unit.ToString() & total, m_font, New SolidBrush(m_fontColor), c, TextAlignment.BottomCenter)
            'e.Graphics.DrawString(distance.ToString(Format) & " " & m_unit.ToString() & total, f, New SolidBrush(m_fontColor), pPt.X, pPt.Y, sformat)
            'e.Display.Graphics.DrawString(distance.ToString(format) & " " & m_unit.ToString() & total, f, New SolidBrush(m_fontColor), pPt.X, pPt.Y, sformat)
            'e.Display.DrawText(distance.ToString(format) & " " & m_unit.ToString() & total, m_font, New SolidBrush(m_fontColor), c, TextAlignment.BottomCenter)
            g.ResetTransform()
            g.TranslateTransform(Middle.X, Middle.Y)
            g.RotateTransform(angle)

            g.DrawString(displayString, m_font, New SolidBrush(m_fontColor), -g.MeasureString(angle.ToString("#0.0"), m_font).Height, -g.MeasureString(angle.ToString("#0.0"), m_font).Height, m_sformat)



        End If

        g.ResetTransform()
        g.DrawString(displayString, m_font, New SolidBrush(m_fontColor), 30, Map.Height - 80)
    End Sub
    Protected Overrides Sub OnActiveChanged(ByVal active As Boolean)

        If active Then

            AddHandler Map.MouseDoubleClick, AddressOf MouseDoubleClick
            AddHandler Map.KeyUp, AddressOf MapKeyUp
            AddHandler Map.KeyDown, AddressOf MapKeyDown
            AddHandler Map.MapPaint, AddressOf MapPaint
        Else
            RemoveHandler Map.MouseDoubleClick, AddressOf MouseDoubleClick
            RemoveHandler Map.KeyUp, AddressOf MapKeyUp
            RemoveHandler Map.KeyDown, AddressOf MapKeyDown
            RemoveHandler Map.MapPaint, AddressOf MapPaint
            m_coordinates = New CoordinateCollection()
            Map.Invalidate()
            ' signal to other mapactions that the action is completed
            'ActionInProgress = False
        End If
        MyBase.OnActiveChanged(active)

    End Sub

    'Private Sub resize()
    '    Try

    '        'Relocate Show Group Button
    '        ' CType(m_BtnShowLayers, Button).Left = m_Map.Width - 35 - CType(m_BtnShowLayers, Button).Width
    '        ' CType(m_BtnShowLayers, Button).Top = m_Map.Height - 35 - CType(m_BtnShowLayers, Button).Height
    '        'Relocate the button
    '        If m_btn IsNot Nothing And MyBase.Map IsNot Nothing Then
    '            ' m_btn.Location = New System.Drawing.Point(MyBase.Map.Width - 80, 85)
    '            m_btn.Left = Map.Width - 35 - m_btn.Width
    '            m_btn.Top = Map.Height - 95 - m_btn.Height
    '        End If
    '    Catch ex As Exception
    '        MsgBox("Error relocating measure Button" & vbCrLf & ex.Message)
    '    End Try
    'End Sub
    'Private Sub measureMapAction_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
    '    Try
    '        'Remove the button from the map 
    '        If m_btn IsNot Nothing Then
    '            If m_btn.Parent IsNot Nothing Then
    '                m_btn.Parent.Controls.Remove(m_btn)
    '            End If

    '        End If


    '    Catch ex As Exception
    '        MsgBox("Error disposing the measure map action" & vbCrLf & ex.Message)
    '        Return
    '    End Try
    'End Sub
    'Private Sub measureMapAction_StatusChanged(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.WinForms.MapActionStatusChangedEventArgs) Handles Me.StatusChanged
    '    Try
    '        'Check the map actions status
    '        If e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Activated Then
    '            'If the button has been created, update the buttons image
    '            If m_btn IsNot Nothing Then
    '                m_btn.BackgroundImage = My.Resources.measureDown
    '            End If
    '            'Enable the ink capture libraries

    '        ElseIf e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Deactivated Then
    '            'If the button has been created, update the buttons image
    '            If m_btn IsNot Nothing Then
    '                m_btn.BackgroundImage = My.Resources.measure
    '            End If

    '        ElseIf e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Deactivating Then
    '        End If

    '    Catch ex As Exception
    '        MsgBox("Error handline the measure map action status change" & vbCrLf & ex.Message)
    '        Return
    '    End Try
    'End Sub

    'Public Sub measureBtnClick(ByVal sender As Object, ByVal e As MouseEventArgs)
    '    Try
    '        'Release or assign the map action
    '        If MyBase.Map.MapAction Is Me Then
    '            MyBase.Map.MapAction = Nothing
    '        Else
    '            MyBase.Map.MapAction = Me
    '        End If
    '    Catch ex As Exception
    '        MsgBox("Error handling the measure mapaction button" & vbCrLf & ex.Message)
    '    End Try
    'End Sub
#End Region



    ''' <summary>
    ''' Gets the ScaleBar's units system.
    ''' </summary>
    ''' <remarks>
    ''' Returns the current spatial map units system that the ScaleBar is using. 
    ''' </remarks>
    <Category("Map")> _
    <Description("Measure Units.")> _
    Public Property DisplayedUnit() As Unit
        Get
            Return m_unit
        End Get
        Set(ByVal value As Unit)
            If m_unit Is value Then
                Return
            End If

            m_unit = value
        End Set
    End Property

    ''' <summary>
    ''' Sets measure line color
    ''' </summary>
    <Category("Appearance")> _
    <Description("Color of the measurement line.")> _
    <DefaultValue(GetType(Color), "Black")> _
    Public Property LineColor() As Color
        Get
            Return m_lineColor
        End Get
        Set(ByVal value As Color)
            '#If Not COMPACT_FRAMEWORK Then
            m_brush = New SolidBrush(m_lineColor)
            '#Else

            'm_barBrush = New SolidBrush(m_backColor)

            '#End If
            ' Refreshing client area
            'InvalidateMap();
            m_lineColor = value
        End Set
    End Property

    ''' <summary>
    ''' Sets whether the distance values for individual measure line segments are shown
    ''' </summary>
    <Category("Behavior")> _
    <DefaultValue(False)> _
    <Description("Show measures for individual segments.")> _
    Public Property ShowSegmentMeasures() As Boolean
        Get
            Return m_showIndividualSegments
        End Get
        Set(ByVal value As Boolean)
            If m_showIndividualSegments = value Then
                Return
            End If
            m_showIndividualSegments = value

            ' the value cannot be set if the measure method is freehand
            If MeasureMethod = EsriMeasureMethod.Freehand Then
                m_showIndividualSegments = False
            End If


            ' Refreshing client area
            If Map IsNot Nothing Then
                Map.Invalidate()
            End If
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the font of the measure tool.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    <Category("Appearance")> _
    <Description("The font used to display text in the measure tool.")> _
    Public Property Font() As Font
        Get
            Return m_font
        End Get
        Set(ByVal value As Font)
            If m_font Is value Then
                Return
            End If

            m_font = value
        End Set
    End Property

    ''' <summary>
    ''' Sets the font color for the measure label
    ''' </summary>
    <Category("Appearance")> _
    <Description("The color of the font used to display text in the measure tool.")> _
    Public Property FontColor() As Color
        Get
            Return m_fontColor
        End Get
        Set(ByVal value As Color)
            If m_fontColor = value Then
                Return
            End If

            m_fontColor = value
        End Set
    End Property

    ''' <summary>
    ''' Specifies the thickness of the line used by the measure tool
    ''' </summary>
    <Category("Appearance")> _
    <DefaultValue(2)> _
    <Description("Line width.")> _
    Public Property LineWidth() As Integer
        Get
            Return m_lineWidth
        End Get
        Set(ByVal value As Integer)
            If m_lineWidth = value Then
                Return
            End If

            If Convert.ToInt32(value) < 0 Then
                Throw New Exception("Invalid Value")
            End If



            ' Refreshing client area
            'InvalidateMap();
            m_lineWidth = value
        End Set
    End Property

    ''' <summary>
    ''' Specifies the measuring method. Either two point or multipoint
    ''' </summary>
    <Category("Behavior")> _
    <Description("Measuring method.")> _
    Public Property MeasureMethod() As EsriMeasureMethod
        Get
            Return m_measureMethod
        End Get
        Set(ByVal value As EsriMeasureMethod)
            If m_measureMethod = value Then
                Return
            End If

            m_measureMethod = value

            If m_measureMethod = EsriMeasureMethod.TwoPoint OrElse m_measureMethod = EsriMeasureMethod.Freehand Then
                m_showIndividualSegments = False
            End If

            ' Refreshing client area
            If Map IsNot Nothing Then
                Map.Invalidate()
            End If
        End Set
    End Property

    ''' <summary>
    ''' Specifies the transparency of the measuring line
    ''' </summary>
    '<Category("Appearance")> _
    '<DefaultValue(255)> _
    '<Description("Transparency. 0 = fully transparent opaque, 255 = opaque.")> _
    'Public Property Transparency() As Integer
    '    Get
    '        Return m_transparency
    '    End Get
    '    Set(ByVal value As Integer)
    '        If m_transparency = value Then
    '            Return
    '        End If

    '        m_transparency = value


    '        If m_transparency < 0 OrElse m_transparency > 255 Then
    '            Throw New Exception("Transparency value must be within  a range of 0 to 255, with 0 being opaque and 255 being fully transparent.")

    '            ' Refreshing client area
    '            ' InvalidateMap();
    '        End If
    '    End Set
    'End Property

    ''' <summary>
    ''' Specifies the number of digits displayed after the decimal point in the label.
    ''' </summary>
    <Category("Behavior")> _
    <DefaultValue(2)> _
    <Description("Number of digits displayed after the decimal point.")> _
    Public Property SignificantDigits() As Integer
        Get
            Return m_digits
        End Get
        Set(ByVal value As Integer)
            If m_digits = value Then
                Return
            End If
            If m_digits < 0 OrElse m_digits > Integer.MaxValue Then
                Throw New Exception("Invalid value. Value must be larger than zero.")
            End If

            m_digits = value
            m_format = "########0."
            For i As Integer = 0 To m_digits - 1
                m_format += "0"
            Next

            ' Refreshing client area
            If Map IsNot Nothing Then
                Map.Invalidate()
            End If
        End Set
    End Property



    ' ENUM
    Public Enum EsriMeasureMethod
        ''' <summary>
        ''' Measure tool returns a distance between two points.
        ''' </summary>
        TwoPoint

        ''' <summary>
        ''' Measure tool returns a distance between multiple points.
        ''' </summary>
        MultiPoint

        '''<summary>
        '''Measure tool returns a total length of a freehand line
        '''</summary>
        Freehand
    End Enum

    Public Enum EsriMeasureTextPosition
        ''' <summary>
        ''' Measure tool text is displayed above mouse pointer.
        ''' </summary>
        Mouse

        ''' <summary>
        ''' Measure tool text is displayed next to starting point
        ''' </summary>
        StartPoint

        ''' <summary>
        ''' Measure tool text is displayed next to end point
        ''' </summary>
        EndPoint

        ''' <summary>
        ''' Measure tool text is displayed in the middle of a measure line.
        ''' </summary>
        MidPoint
    End Enum

    Private Sub drawMeasurement(ByVal distance As Double, ByVal coord1 As Coordinate, ByVal coord2 As Coordinate, ByVal g As Graphics)

        ' segment angle
        Dim angle As Single = 0 - Convert.ToSingle((Math.Atan2(coord2.Y - coord1.Y, coord2.X - coord1.X) * (180 / Math.PI)))
        'Dim angle2 As Single = Math.Abs(Convert.ToSingle((Math.Atan2(coord2.Y - coord1.Y, coord2.X - coord1.X) * (180))))
        'If angle2 > 180 Then
        '    angle2 = angle2 - 180

        'End If
        ' angle = 0 - Convert.ToSingle(angle2 / Math.PI)
        Dim c1 As Coordinate, c2 As Coordinate
        c1 = DirectCast(coord1.Clone(), Coordinate)
        c2 = DirectCast(coord2.Clone(), Coordinate)

        If angle < 0 Then
            angle = angle + 180


        End If
        If angle > 90 And angle < 180 Then
            angle = angle + 180


        End If
        Dim midpoint As New Coordinate(Convert.ToInt32(0.5 * c1.X + 0.5 * c2.X), Convert.ToInt32(0.5 * c1.Y + 0.5 * c2.Y))

        '-----------------------------------------------------------------------------------------------------------------------------------------------------
        Dim sz As Size = Map.Size
        Dim Middle As System.Drawing.Point = Map.ToClient(midpoint)
        g.ResetTransform()
        g.TranslateTransform(Middle.X, Middle.Y)

        g.RotateTransform(angle)

        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias
        Dim displayString As String = distance.ToString(m_format) & " " & m_unit.ToString()
        displayString = displayString.Replace("Foot", "Feet")
        displayString = displayString.Replace("Yard", "Yards")
        displayString = displayString.Replace("Mile", "Miles")
        displayString = displayString.Replace(" Kilometers", " Kilometers")
        displayString = displayString.Replace(" Meter", " Meters")
        g.DrawString(displayString, m_font, New SolidBrush(m_fontColor), 0 - g.MeasureString(displayString, m_font).Height, 0 - g.MeasureString(displayString, m_font).Height, m_sformat)
    End Sub

    Private Sub drawMeasurement(ByVal distance As Double, ByVal coord1 As Coordinate, ByVal coord2 As Coordinate, ByVal mapSurface As MapSurface)

        ' segment angle
        Dim angle As Single = 0 - Convert.ToSingle((Math.Atan2(coord2.Y - coord1.Y, coord2.X - coord1.X) * (180 / Math.PI)))
        'Dim angle2 As Single = Math.Abs(Convert.ToSingle((Math.Atan2(coord2.Y - coord1.Y, coord2.X - coord1.X) * (180))))
        'If angle2 > 180 Then
        '    angle2 = angle2 - 180

        'End If
        ' angle = 0 - Convert.ToSingle(angle2 / Math.PI)
        Dim c1 As Coordinate, c2 As Coordinate
        c1 = DirectCast(coord1.Clone(), Coordinate)
        c2 = DirectCast(coord2.Clone(), Coordinate)
        angle = angle + CSng(mapSurface.Map.RotationAngle)
        If angle < 0 Then
            angle = angle + 180


        End If
        If angle > 90 And angle < 180 Then
            angle = angle + 180


        End If
        If angle > 180 And angle < 270 Then
            angle = angle - 180


        End If
        Dim midpoint As New Coordinate(Convert.ToInt32(0.5 * c1.X + 0.5 * c2.X), Convert.ToInt32(0.5 * c1.Y + 0.5 * c2.Y))

        '-----------------------------------------------------------------------------------------------------------------------------------------------------
        Dim sz As Size = Map.Size
        Dim Middle As System.Drawing.Point = Map.ToClient(midpoint)

        mapSurface.Graphics.ResetTransform()
        mapSurface.Graphics.TranslateTransform(Middle.X, Middle.Y)

        mapSurface.Graphics.RotateTransform(angle)

        mapSurface.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality
        mapSurface.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias
        Dim displayString As String = distance.ToString(m_format) & " " & m_unit.ToString()
        displayString = displayString.Replace("Foot", "Feet")
        displayString = displayString.Replace("Yard", "Yards")
        displayString = displayString.Replace("Mile", "Miles")
        displayString = displayString.Replace(" Kilometers", " Kilometers")
        displayString = displayString.Replace(" Meter", " Meters")
        mapSurface.Graphics.DrawString(displayString, m_font, New SolidBrush(m_fontColor), 0 - mapSurface.Graphics.MeasureString(displayString, m_font).Height, 0 - mapSurface.Graphics.MeasureString(displayString, m_font).Height, m_sformat)
    End Sub

End Class
