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



Imports ESRI.ArcGIS.Mobile
Imports Esri.ArcGIS.Mobile.FeatureCaching
Imports ESRI.ArcGIS.Mobile.Geometries
Imports System.Windows.Forms
Imports System
Imports System.Drawing

Imports MobileControls
Imports Esri.ArcGISTemplates


Public Class SketchMapAction
    Inherits Esri.ArcGIS.Mobile.WinForms.MapAction
  

    Private WithEvents m_cboSketchLayers As ComboBox
    Private m_LongString As String = " "

    Public Event RaiseMessage(ByVal Message As String)

    Private m_btn As Button
   
    Private m_mousePosition As Coordinate
   

    Private m_CoordsCurrent As CoordinateCollection


    Private m_lineColor As Color = Color.Red
    Private m_LayerName As String
   
    Private m_SketchMode As String
    Private m_ShowCombo As Boolean = True


    Private m_LineWidth As Integer = 5
    Private m_PointWidth As Integer = 5
    Public Event checkGPS()
    Private m_GPSVal As GPSLocationDetails

    Private m_Fnt As System.Drawing.Font
    Private m_FntSize As Single
    Private m_DT As FeatureDataTable
    Private WithEvents m_Map As Esri.ArcGIS.Mobile.WinForms.Map
    Public Property ManualSetMap As Esri.ArcGIS.Mobile.WinForms.Map
        Get
            Return Map
        End Get
        Set(value As Esri.ArcGIS.Mobile.WinForms.Map)
            m_Map = value


        End Set
    End Property


    Protected Overrides Sub OnSetMap(ByVal map As Esri.ArcGIS.Mobile.WinForms.Map)
        MyBase.OnSetMap(map)
        '  m_Map = map

    End Sub
#Region "Public Methods"

  
    Public Sub New()
        MyBase.New()
        'for Multipart
        'm_CoordsList = New List(Of CoordinateCollection)

        m_CoordsCurrent = New CoordinateCollection
        m_LineWidth = CInt(GlobalsFunctions.appConfig.EditControlOptions.SketchLineWidth)
        m_PointWidth = CInt(GlobalsFunctions.appConfig.EditControlOptions.SketchPointWidth)

    End Sub
   
    Public Function addMapButton(Optional ByVal TopX As Integer = -1, Optional ByVal TopY As Integer = -1) As Boolean
        Try
            'Create and add the button to toggle the redline map action
            If m_btn Is Nothing Then
                'Create a new button
                m_btn = New Button
                With m_btn
                    'Set the buttons look and feel
                    .BackColor = System.Drawing.SystemColors.Info
                    .BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
                    .FlatAppearance.BorderSize = 0
                    .FlatStyle = System.Windows.Forms.FlatStyle.Flat
                    .BackgroundImage = My.Resources.sketchpen
                    .Name = "btnSketch"
                    .Size = New System.Drawing.Size(50, 50)
                    .UseVisualStyleBackColor = False
                    'Locate the button
                    'TopX = m_map.Width - 80
                    TopX = m_Map.Width - 80
                    TopY = 150
                    .Location = New System.Drawing.Point(TopX, TopY)
                    'Add the handler for relocating the button

                    'AddHandler m_map.Resize, AddressOf resize
                    AddHandler m_Map.Resize, AddressOf resize
                    'Add a handler for the click event
                    AddHandler .MouseClick, AddressOf InkBtnClick

                End With
                If m_cboSketchLayers Is Nothing Then
                    m_cboSketchLayers = New ComboBox
                    m_cboSketchLayers.Visible = False
                    m_cboSketchLayers.ForeColor = Drawing.Color.White
                    m_cboSketchLayers.BackColor = Drawing.Color.DarkBlue
                    m_cboSketchLayers.DropDownStyle = ComboBoxStyle.DropDownList
                    '    m_cboSketchLayers.DropDownHeight = 300
                    m_cboSketchLayers.Font = m_Fnt
                    m_cboSketchLayers.Width = 200
                    m_cboSketchLayers.Left = m_btn.Left - m_cboSketchLayers.Width - 10
                    m_cboSketchLayers.Top = CInt(m_btn.Top + (m_btn.Height / 2) - (m_cboSketchLayers.Height / 2))
                    '  AddHandler m_cboSketchLayers.SelectedIndexChanged, AddressOf m_cboSketchLayers_SelectedIndexChanged
                    ' AddHandler m_cboSketchLayers.MouseDoubleClick, AddressOf m_cboSketchLayers_MouseDoubleClick

                    'Locate it to the left of the button and add to the map
                    'm_map.Controls.Add(m_cboSketchLayers)
                    m_Map.Controls.Add(m_cboSketchLayers)
                    m_cboSketchLayers.Visible = False

                End If
                'Add the button to the map
                m_Map.Controls.Add(m_btn)

                'm_map.Controls.Add(m_btn)
            End If

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

            Return False
        End Try
        Return True

    End Function
    'Public Function InitControl(ByVal container As Control, Optional ByVal LayerName As String = "") As Boolean
    Public Function InitControl(Optional ByVal LayerName As String = "") As Boolean
        Try
            Single.TryParse(GlobalsFunctions.appConfig.ApplicationSettings.FontSize, m_FntSize)

            If Not IsNumeric(m_FntSize) Then
                m_FntSize = 10
            ElseIf m_FntSize = 0 Then
                m_FntSize = 10
            End If


            m_Fnt = New System.Drawing.Font("Microsoft Sans Serif", m_FntSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)


            ''Get the redline layer
            'Dim pMblLay As Esri.ArcGIS.Mobile.FeatureCaching.MobileCacheMapLayer

            'pMblLay = CType(Map.MapLayers(m_LayerName), MobileCacheMapLayer)

            ''Return if it was not found
            'If pMblLay Is Nothing Then
            '    m_map.Controls.Remove(m_btn)
            '    Return False

            'End If


            addMapButton()


            LoadSketchLayers()
            If m_cboSketchLayers.Items.Count > 0 Then
                '     SetCurrentEditLayer(CType(m_cboSketchLayers.SelectedItem, MobileConfigClass.MobileConfigMobileMapConfigEditControlOptionsLayersLayer))
            End If


            AddHandler m_Map.MapPaint, AddressOf MapPaint
            Return True

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
            Return False

        End Try

    End Function

#End Region
#Region "Private Methods"




    Private Sub LoadSketchLayers()
        Try
            m_LongString = ""
            'Load the inspection types to the inspection dropdown
            If m_cboSketchLayers Is Nothing Then Return
            If GlobalsFunctions.appConfig.EditControlOptions Is Nothing Then Return
            If GlobalsFunctions.appConfig.EditControlOptions.Layers Is Nothing Then Return
            If GlobalsFunctions.appConfig.EditControlOptions.Layers.Layer Is Nothing Then Return
            If GlobalsFunctions.appConfig.EditControlOptions.Layers.Layer.Count = 0 Then Return
            RemoveHandler m_cboSketchLayers.SelectedIndexChanged, AddressOf m_cboSketchLayers_SelectedIndexChanged


            For Each createLay In GlobalsFunctions.appConfig.EditControlOptions.Layers.Layer
                If createLay.Name.Length > m_LongString.Length Then
                    m_LongString = createLay.Name
                End If
                m_cboSketchLayers.Items.Add(createLay)

            Next

            m_cboSketchLayers.SelectedItem = m_cboSketchLayers.Items(0)

            ' m_cboSketchLayers.DataSource = GlobalsFunctions.appConfig.EditControlOptions.Layers.Layer
            'm_cboSketchLayers.Visible = True


            m_cboSketchLayers.DisplayMember = "DisplayText"
            'm_cboSketchLayers.ValueMember = "EditTool"

            AddHandler m_cboSketchLayers.SelectedIndexChanged, AddressOf m_cboSketchLayers_SelectedIndexChanged

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try
    End Sub
#End Region
#Region "Events"

    Private Sub m_cboSketchLayers_DropDown(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_cboSketchLayers.DropDown

        Dim g As Graphics = m_cboSketchLayers.CreateGraphics
        For i = 0 To 4
            ' m_layCBO.Width = m_layCBO.Width + 40
            '    m_cboSketchLayers.Height = m_cboSketchLayers.Height + 5

            Dim pFnt As System.Drawing.Font = New System.Drawing.Font(m_cboSketchLayers.Font.Name, m_cboSketchLayers.Font.Size + i, System.Drawing.FontStyle.Bold)
            m_cboSketchLayers.Font = pFnt

            m_cboSketchLayers.Width = g.MeasureString(m_LongString, pFnt).Width - 35



            m_cboSketchLayers.Left = m_btn.Left - m_cboSketchLayers.Width - 10
            m_cboSketchLayers.Top = CInt(m_btn.Top + (m_btn.Height / 2) - (m_cboSketchLayers.Height / 2))

        Next
        g.Dispose()
        g = Nothing
        ' End If
    End Sub

    Private Sub m_cboSketchLayers_DropDownClosed(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_cboSketchLayers.DropDownClosed

        Dim g As Graphics = m_cboSketchLayers.CreateGraphics

        m_cboSketchLayers.Width = g.MeasureString(m_cboSketchLayers.Text, m_Fnt).Width + 50


        m_cboSketchLayers.Font = m_Fnt

        m_cboSketchLayers.Left = m_btn.Left - m_cboSketchLayers.Width - 10
        m_cboSketchLayers.Top = CInt(m_btn.Top + (m_btn.Height / 2) - (m_cboSketchLayers.Height / 2))
        g.Dispose()
        g = Nothing
    End Sub



    Private Sub m_cboSketchLayers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_cboSketchLayers.SelectedIndexChanged

        If (m_CoordsCurrent IsNot Nothing) Then
            m_CoordsCurrent.Clear()
        End If



        m_Map.Invalidate()
    End Sub

    Private Sub UpdateGeoWithCoordinate()
        Try
            'Convert the stoke to a mobile geo
            Dim pGeo As Geometry = Nothing
            If m_DT.FeatureSource.GeometryType = GeometryType.Polygon Then

                pGeo = New Polygon(m_CoordsCurrent.Clone)
                If UCase(m_SketchMode) = UCase("FreeHand") And ActionInProgress Then

                Else
                    ActionInProgress = False
                End If



            ElseIf m_DT.FeatureSource.GeometryType = GeometryType.Polyline Then


                pGeo = New Polyline(m_CoordsCurrent.Clone)



                If UCase(m_SketchMode) = UCase("FreeHand") And ActionInProgress Then

                Else
                    ActionInProgress = False
                End If


            ElseIf m_DT.FeatureSource.GeometryType = GeometryType.Point Then


                If m_CoordsCurrent.Count = 1 Then
                    pGeo = New Esri.ArcGIS.Mobile.Geometries.Point(m_CoordsCurrent(0).Clone)

                End If


                ActionInProgress = False


            End If
            '
            'If there is not shape, prompt the user and exit
            If pGeo Is Nothing Then

                Map.Invalidate()
                'delete the strokes
                ' m_CoordsList.Clear()
                m_CoordsCurrent.Clear()
                Return
            End If
            Try


                'Cleanup
                pGeo = Nothing
            Catch ex As Exception
                'Capture a invalid geometry

                'delete the strokes
                'm_CoordsList.Clear()
                m_CoordsCurrent.Clear()
                'Prompt the user
                Dim st As New StackTrace
                MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                st = Nothing
                'Clean up and return
                pGeo = Nothing
                Map.Invalidate()
                Return

            End Try

            Map.Invalidate()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
            Return
        End Try

    End Sub
    Private Sub MouseDoubleClick(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.MapMouseEventArgs)
        If UCase(m_SketchMode) <> UCase("FreeHand") And ActionInProgress Then
            'Used for multipart geo
            '   CreateFinished()



        End If

    End Sub
    Protected Overrides Sub OnMapMouseUp(ByVal e As Esri.ArcGIS.Mobile.MapMouseEventArgs)
        MyBase.OnMapMouseUp(e)
        If m_DT Is Nothing Then Return

        If UCase(m_SketchMode) = UCase("FreeHand") And ActionInProgress And m_DT.FeatureSource.GeometryType <> GeometryType.Point Then
            UpdateGeoWithCoordinate()

            'm_CoordsCurrent = New CoordinateCollection()
            'm_CoordsList.Add(m_CoordsCurrent)
            Map.Invalidate()
            m_Map.Cursor = New Cursor(My.Resources.Freehand.Handle)

            ' signal to other mapactions that the action is completed
            ActionInProgress = False
        ElseIf ActionInProgress Then
            'm_map.Cursor = New Cursor(My.Resources.target.Handle)

            'UpdateGeoWithCoordinate()
        End If
    End Sub
    Protected Overrides Sub OnMapMouseMove(ByVal e As Esri.ArcGIS.Mobile.MapMouseEventArgs)
        'MyBase.OnMapMouseMove(e)
        m_mousePosition = e.MapCoordinate

        If ActionInProgress = False Then Return


        If (m_CoordsCurrent.Count < 1) Then
            Return
        End If

        If UCase(m_SketchMode).ToUpper = UCase("FreeHand").ToUpper And m_DT.FeatureSource.GeometryType <> GeometryType.Point Then
            If m_DT.FeatureSource.GeometryType = GeometryType.Polygon Then
                If (m_CoordsCurrent.Count < 2) Then
                    Return
                End If
                m_CoordsCurrent.Insert(m_CoordsCurrent.Count - 1, m_mousePosition)
            Else
                m_CoordsCurrent.Add(m_mousePosition)

            End If

            If (Map IsNot Nothing) Then
                Map.Invalidate()
            End If

        End If


    End Sub
    Protected Overrides Sub OnMapMouseDown(ByVal e As Esri.ArcGIS.Mobile.MapMouseEventArgs)
        ' MyBase.OnMapMouseDown(e)
        Map.Focus()

        If m_DT Is Nothing Then Return

        If (e.MapCoordinate Is Nothing) Then
            Return
        End If
        If UCase(m_SketchMode).ToUpper = UCase("FreeHand").ToUpper Then
            m_CoordsCurrent.Clear()
            ActionInProgress = True

        End If
        'If ActionInProgress And m_PointAtTapOnly = True Then
        '    m_coordinates.Insert(m_coordinates.Count - 1, m_mousePosition)

        '    If (Map IsNot Nothing) Then
        '        Map.Invalidate()
        '    End If
        'Els-eIf ActionInProgress And m_DoubleTapToFinish = False Then
        '    ActionInProgress = False
        '    RedlineFinished()
        '    m_map.Cursor = New Cursor(My.Resources.redline.Handle)

        'Else
        ' signal to other mapactions that the action is executing

        addCoordToGeo(e.MapCoordinate)

        UpdateGeoWithCoordinate()

        Map.Invalidate()

    End Sub
    Private Sub addCoordToGeo(ByVal newCoord As Coordinate)
        If m_DT Is Nothing Then Return

        If m_CoordsCurrent Is Nothing Then
            m_CoordsCurrent = New CoordinateCollection

        End If

        If UCase(m_SketchMode).ToUpper = UCase("FreeHand").ToUpper And m_DT.FeatureSource.GeometryType <> GeometryType.Point Then
            '  m_CoordsCurrent = New CoordinateCollection
            m_CoordsCurrent.Clear()

            'm_CoordsList.Add(m_CoordsCurrent)

        End If




        If (m_CoordsCurrent.Count = 0) Then
            ' m_CoordsList.Add(m_CoordsCurrent)

            m_CoordsCurrent.Add(newCoord)
            If m_DT.FeatureSource.GeometryType = GeometryType.Polygon Then

                m_CoordsCurrent.Add(newCoord)


            End If

            'If (Map IsNot Nothing) Then
            '    Map.Invalidate()
            'End If
            'ElseIf (m_coordinates.Count = 2) Then

            '    If (m_measureMethod = EsriMeasureMethod.MultiPoint) Then
            '        m_coordinates.Insert(1, e.MapCoordinate)
            '    End If

            'ElseIf (m_measureMethod = EsriMeasureMethod.MultiPoint) Then
            '    m_coordinates.Insert(m_coordinates.Count - 1, e.MapCoordinate)
        Else
            If m_DT.FeatureSource.GeometryType = GeometryType.Polygon Then
                If m_CoordsCurrent(m_CoordsCurrent.Count - 1).X = newCoord.X And m_CoordsCurrent(m_CoordsCurrent.Count - 1).Y = newCoord.Y Then
                    ' MsgBox("SameCoord:")

                Else
                    m_CoordsCurrent.Insert(m_CoordsCurrent.Count - 1, newCoord)
                End If

            ElseIf m_DT.FeatureSource.GeometryType = GeometryType.Point Then
                ' m_CoordsList.Add(m_CoordsCurrent)
                m_CoordsCurrent.Clear()

                m_CoordsCurrent.Add(newCoord)

            Else


                m_CoordsCurrent.Add(newCoord)

            End If
            'm_coordinates.Add(m_mousePosition)

            'If (Map IsNot Nothing) Then
            '    Map.Invalidate()
            'End If
        End If

    End Sub

    Private Sub MapPaint(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.WinForms.MapPaintEventArgs)
        If m_DT Is Nothing Then Return

        If m_DT.FeatureSource.GeometryType = GeometryType.Point Or m_DT.FeatureSource.GeometryType = GeometryType.Multipoint Then
            'for Multipart
            'For Each coord As CoordinateCollection In m_CoordsList
            '    For Each crd In coord
            '        e.Display.DrawPoint(New Pen(m_lineColor, m_PointWidth), New SolidBrush(m_lineColor), m_PointWidth, crd)

            '    Next
            'Next
            For Each crd In m_CoordsCurrent
                e.MapSurface.DrawPoint(New Pen(m_lineColor, m_PointWidth), New SolidBrush(m_lineColor), m_PointWidth, crd)

            Next

        Else


            If m_CoordsCurrent.Count > 0 Then
                If m_CoordsCurrent.Count = 1 Then
                    e.MapSurface.DrawPoint(New Pen(m_lineColor, m_PointWidth), New SolidBrush(m_lineColor), m_PointWidth, m_CoordsCurrent(0))



                Else

                    If m_CoordsCurrent.Count = 2 Then
                        If m_CoordsCurrent(0).EquivalentTo(m_CoordsCurrent(1)) Then
                            e.MapSurface.DrawPoint(New Pen(m_lineColor, m_PointWidth), New SolidBrush(m_lineColor), m_PointWidth, m_CoordsCurrent(0))

                        Else
                            e.MapSurface.DrawPolyline(New Pen(m_lineColor, m_LineWidth), m_CoordsCurrent)
                            e.MapSurface.DrawPoint(New Pen(m_lineColor, m_PointWidth), New SolidBrush(m_lineColor), m_PointWidth, m_CoordsCurrent(m_CoordsCurrent.Count - 1))

                        End If
                    Else
                        e.MapSurface.DrawPolyline(New Pen(m_lineColor, m_LineWidth), m_CoordsCurrent)
                        e.MapSurface.DrawPoint(New Pen(m_lineColor, m_PointWidth), New SolidBrush(m_lineColor), m_PointWidth, m_CoordsCurrent(m_CoordsCurrent.Count - 1))

                    End If


                End If
            End If
            'End If


            'for Multipart
            'For Each coord As CoordinateCollection In m_CoordsList
            '    If coord.Count = 0 Then Continue For
            '    If coord.Count = 1 Then
            '        e.Display.DrawPoint(New Pen(m_lineColor, m_PointWidth), New SolidBrush(m_lineColor), m_PointWidth, coord(0))

            '        Continue For


            '    End If
            '    If coord.Count = 2 Then
            '        If coord(0).EquivalentTo(coord(1)) Then
            '            e.Display.DrawPoint(New Pen(m_lineColor, m_PointWidth), New SolidBrush(m_lineColor), m_PointWidth, coord(0))

            '            Continue For

            '        End If

            '    End If
            '    'Dim g As Graphics = e.Graphics

            '    'If m_DT.FeatureSource.GeometryType = GeometryType.Polygon Then
            '    '    If m_coordinates.Count > 3 Then
            '    '        e.Display.DrawPolygon(New Pen(m_lineColor, m_lineWidth), Nothing, m_coordinates)
            '    '    End If

            '    'ElseIf m_DT.FeatureSource.GeometryType = GeometryType.Polygon Then
            '    e.Display.DrawPolyline(New Pen(m_lineColor, m_LineWidth), coord)
            '    e.Display.DrawPoint(New Pen(m_lineColor, m_PointWidth), New SolidBrush(m_lineColor), m_PointWidth, coord(coord.Count - 1))
            '    'End If

            'Next
        End If






    End Sub
    Protected Overrides Sub OnActiveChanged(ByVal active As Boolean)

        If active Then

            AddHandler Map.MouseDoubleClick, AddressOf MouseDoubleClick
            AddHandler Map.KeyUp, AddressOf MapKeyUp
            AddHandler Map.KeyDown, AddressOf MapKeyDown
            ' AddHandler Map.Paint, AddressOf MapPaint
            ' m_map.Cursor = New Cursor(My.Resources.redline.Handle)

            '
        Else
            RemoveHandler Map.MouseDoubleClick, AddressOf MouseDoubleClick
            RemoveHandler Map.KeyUp, AddressOf MapKeyUp
            RemoveHandler Map.KeyDown, AddressOf MapKeyDown
            '  RemoveHandler Map.Paint, AddressOf MapPaint
            m_Map.Cursor = Cursors.Default
            'm_CoordsList = New List(Of CoordinateCollection)
            ' m_CoordsCurrent = New CoordinateCollection
            Map.Invalidate()
            ' signal to other mapactions that the action is completed
            ActionInProgress = False
        End If
        MyBase.OnActiveChanged(active)

    End Sub
    Private Sub MapKeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)

        MyBase.OnMapKeyDown(e)
        If e.KeyValue = 27 Then
            ' m_CoordsList = New List(Of CoordinateCollection)
            m_CoordsCurrent = New CoordinateCollection
            Map.Invalidate()
            ActionInProgress = False

            Return
        End If


    End Sub
    Private Sub MapKeyUp(ByVal sender As Object, ByVal e As KeyEventArgs)
        MyBase.OnMapKeyUp(e)



    End Sub

    Private Sub resize()
        Try
            'Relocate the button
            If m_btn IsNot Nothing And m_Map IsNot Nothing Then
                m_btn.Location = New System.Drawing.Point(m_Map.Width - 80, 145)
            End If
            m_cboSketchLayers.Width = 200
            m_cboSketchLayers.Left = m_btn.Left - m_cboSketchLayers.Width - 10
            m_cboSketchLayers.Top = CInt(m_btn.Top + (m_btn.Height / 2) - (m_cboSketchLayers.Height / 2))

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
    'Private Sub CreateFeaturesMapAction_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
    '    'Try
    '    '    If Map IsNot Nothing Then
    '    '        RemoveHandler Map.MapPaint, AddressOf MapPaint
    '    '    End If
    '    '    'Remove the button from the map 
    '    '    If m_btn IsNot Nothing Then
    '    '        If m_btn.Parent IsNot Nothing Then
    '    '            m_btn.Parent.Controls.Remove(m_btn)
    '    '        End If

    '    '    End If
    '    '    'Release the other objects

    '    '    If m_EditPanel IsNot Nothing Then
    '    '        m_EditPanel.Dispose()
    '    '    End If
    '    '    m_EditPanel = Nothing

    '    'Catch ex As Exception
    '    '    Dim st As New StackTrace
    '    '    MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
    '    '    st = Nothing
    '    '    Return
    '    'End Try
    'End Sub

    Private Sub SketchMapAction_StatusChanged(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.WinForms.MapActionStatusChangedEventArgs) Handles Me.StatusChanged
        Try
            'Check the map actions status
            If e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Activated Then
                'If the button has been created, update the buttons image
                If m_btn IsNot Nothing Then
                    m_btn.BackgroundImage = My.Resources.sketchpenred
                    m_cboSketchLayers.Visible = m_ShowCombo
                End If

                If m_DT IsNot Nothing Then


                    If m_SketchMode.ToUpper = "Freehand".ToUpper And m_DT.FeatureSource.GeometryType <> GeometryType.Point Then

                        m_Map.Cursor = New Cursor(My.Resources.Freehand.Handle)

                    Else
                        m_Map.Cursor = New Cursor(My.Resources.target.Handle)

                    End If
                End If

            ElseIf e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Deactivated Then
                'If the button has been created, update the buttons image
                If m_btn IsNot Nothing Then
                    m_btn.BackgroundImage = My.Resources.sketchpen
                    m_cboSketchLayers.Visible = False
                End If


            End If

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
            Return
        End Try
    End Sub
    Public Sub InkBtnClick(ByVal sender As Object, ByVal e As MouseEventArgs)
        Try
            'Release or assign the map action
            If m_Map.MapAction Is Me Then
                m_Map.MapAction = Nothing
            Else
                m_Map.MapAction = Me
            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
#End Region


    Protected Overrides Sub Finalize()
        MyBase.Finalize()
        Try
            If Map IsNot Nothing Then
                RemoveHandler Map.MapPaint, AddressOf MapPaint
            End If
            'Remove the button from the map 
            If m_btn IsNot Nothing Then
                If m_btn.Parent IsNot Nothing Then
                    m_btn.Parent.Controls.Remove(m_btn)
                End If

            End If
            'Release the other objects


        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
            Return
        End Try
    End Sub


End Class
