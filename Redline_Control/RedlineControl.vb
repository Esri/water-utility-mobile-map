Imports ESRI.ArcGIS.Mobile.MapActions
Imports ESRI.ArcGIS.Mobile
Imports ESRI.ArcGIS.Mobile.MobileServices
Imports ESRI.ArcGIS.Mobile.Geometries
Imports System.Windows.Forms
Imports System
Imports System.Drawing
Imports MobileControls.EditControl
Imports MobileControls
Imports Esri.ArcGISTemplates


Public Class RedlineMapAction
    Inherits Esri.ArcGIS.Mobile.MapActions.MapAction
    Public Event RaiseMessage(ByVal Message As String)

    'Private m_RadioOnTwo As Boolean = True

    Private m_FLay As FeatureLayer
    Private m_btn As Button
    'Ink Collector
    'Edit panel to capture redlines attributes
    Private WithEvents m_RedlineForm As MobileControls.EditControl
    Private m_mousePosition As Coordinate
    ' stores current mouse position
    Private m_coordinates As CoordinateCollection
    ' stores all coordinates used in drawing the measure line
    Private m_lineColor As Color = Color.Red
    Private m_LayerName As String
    'Private m_DoubleTapToFinish As Boolean = False
    'Private m_PointAtTapOnly As Boolean = False
    Private m_SketchMode As String

    ' Private m_FntSize As Single

    ' Private m_fillColor As Color = Color.Blue

    ' Private m_transparency As Integer = 255

    Private m_lineWidth As Integer = 5
    Public Event checkGPS()
    Private m_GPSVal As GPSLocationDetails

#Region "Public Methods"
    Public Sub New()
        MyBase.New()
        m_coordinates = New CoordinateCollection()

    End Sub
    Public Function addRedlineButton(Optional ByVal TopX As Integer = -1, Optional ByVal TopY As Integer = -1) As Boolean
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
                    .BackgroundImage = My.Resources.Pen
                    .Name = "btnRedline"
                    .Size = New System.Drawing.Size(50, 50)
                    .UseVisualStyleBackColor = False
                    'Locate the button
                    TopX = MyBase.Map.Width - 80
                    TopY = 85
                    .Location = New System.Drawing.Point(TopX, TopY)
                    'Add the handler for relocating the button
                    AddHandler MyBase.Map.Resize, AddressOf resize
                    'Add a handler for the click event
                    AddHandler .MouseClick, AddressOf InkBtnClick

                End With
                'Add the button to the map
                MyBase.Map.Controls.Add(m_btn)
            End If

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

            Return False
        End Try
        Return True

    End Function
    Public Function InitRedlineForm(ByVal container As Control, Optional ByVal LayerName As String = "")
        Try
            intConfigValues()
            'Get the redline layer
            Dim pMblLay As Esri.ArcGIS.Mobile.MobileServices.MobileCacheMapLayer

            pMblLay = CType(Map.MapLayers(m_LayerName), MobileCacheMapLayer)

            'Return if it was not found
            If pMblLay Is Nothing Then
                MyBase.Map.Controls.Remove(m_btn)
                Return False

            End If

            'Set the layer to a global
            pMblLay.Visible = True

            m_FLay = CType(pMblLay.Layer, FeatureLayer)
            If m_FLay Is Nothing Then
                MyBase.Map.Controls.Remove(m_btn)
                Return False
            End If


            'Create the form is not already created
            If m_RedlineForm Is Nothing Then
                'Create a new edit control
                m_RedlineForm = New MobileControls.EditControl(MyBase.Map, Nothing)
                m_RedlineForm.GPSButtonVisible = False

                'Fill the containter
                m_RedlineForm.Dock = DockStyle.Fill
                'Tell the panel to draw the geometry
                m_RedlineForm.DrawGeo = True
                m_RedlineForm.Height = container.Parent.Height



                'Clear the controls on the cotainer
                container.Controls.Clear()
                'Add the edit panel
                container.Dock = DockStyle.Fill

                container.Controls.Add(m_RedlineForm)
                container.Refresh()
                container.Update()

                m_RedlineForm.ResumeLayout(True)


                m_RedlineForm.Refresh()
                m_RedlineForm.Update()
                'Create a new row on the panel
                NewRow()
            End If
            Return True

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try

    End Function

#End Region
#Region "Private Methods"

    Private Sub intConfigValues()
        Try


            'get the redline layer from the app.config file
            ''Dim myutils As ConfigUtils = New ConfigUtils()

            m_LayerName = Globals.appConfig.RedlinePanel.LayerName
            ' m_DoubleTapToFinish = Globals.toBoolean(Globals.FindConfigKey("RedlineDoubleTapToComplete"))
            'm_PointAtTapOnly = Globals.toBoolean(Globals.FindConfigKey("RedlinePointAtTapOnly"))
            m_SketchMode = Globals.appConfig.RedlinePanel.RedlineMode
            'm_FntSize = Globals.appConfig.ApplicationSettings.FontSize

            'If m_FntSize = 0 Then
            '    m_FntSize = 10
            'End If


            'Clean up
            'myutils = Nothing


        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub

    Private Sub NewRow()
        Try

            'If the redline layer was not set, return
            If m_FLay Is Nothing Then Return
            'if the form is not initilize, then exit
            If m_RedlineForm Is Nothing Then Return
            'Get a table from the layer
            Dim pDT As FeatureDataTable = m_FLay.GetDataTable()
            'Create a new row and assign it to the edit form
            m_RedlineForm.CurrentRecord = pDT.NewRow
            'Cleanup
            pDT = Nothing

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
            Return
        End Try

    End Sub
#End Region
#Region "Events"
    Private Sub RedlineFinished()
        Try
            'Convert the stoke to a mobile geo
            Dim pGeo As Geometry = Nothing
            If m_FLay.GeometryType = GeometryType.Polygon Then

                pGeo = New Polygon(m_coordinates)
                m_coordinates.Clear()

            ElseIf m_FLay.GeometryType = GeometryType.Polyline Then
                pGeo = New Polyline(m_coordinates)
                m_coordinates.Clear()
            Else
                'delete the strokes
                m_coordinates.Clear()


                Return

            End If
            '
            'If there is not shape, prompt the user and exit
            If pGeo Is Nothing Then

                Map.Invalidate()
                'delete the strokes
                m_coordinates.Clear()

                Return
            End If
            Try

                'Try to set the geometry from the stroke to the edit panel
                m_RedlineForm.Geometry = pGeo
                'Cleanup
                pGeo = Nothing
            Catch ex As Exception
                'Capture a invalid geometry

                'delete the strokes
                m_coordinates.Clear()
                'Prompt the user
                Dim st As New StackTrace
                MsgBox(st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                st = Nothing
                'Clean up and return
                pGeo = Nothing
                Map.Invalidate()
                Return

            End Try

            'The geometry was save to the edit panel, delete the strokes
            m_coordinates.Clear()

            'refresh the map
            Map.Refresh()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
            Return
        End Try

    End Sub
    Private Sub MouseDoubleClick(ByVal sender As Object, ByVal e As MapMouseEventArgs)
        If UCase(m_SketchMode) <> UCase("FreeHand") And ActionInProgress Then
            RedlineFinished()

            m_coordinates = New CoordinateCollection()
            Map.Invalidate()
            MyBase.Map.Cursor = New Cursor(My.Resources.redline.Handle)

            ' signal to other mapactions that the action is completed
            ActionInProgress = False
        End If
        'If m_DoubleTapToFinish = True Then
        '    RedlineFinished()

        '    m_coordinates = New CoordinateCollection()
        '    Map.Invalidate()
        '    MyBase.Map.Cursor = New Cursor(My.Resources.redline.Handle)

        '    ' signal to other mapactions that the action is completed
        '    ActionInProgress = False
        'ElseIf m_DoubleTapToFinish = False And m_PointAtTapOnly = True Then
        '    RedlineFinished()

        '    m_coordinates = New CoordinateCollection()
        '    Map.Invalidate()
        '    MyBase.Map.Cursor = New Cursor(My.Resources.redline.Handle)

        '    ' signal to other mapactions that the action is completed
        '    ActionInProgress = False
        'End If

    End Sub
    Protected Overrides Sub OnMapMouseUp(ByVal e As Esri.ArcGIS.Mobile.MapMouseEventArgs)
        MyBase.OnMapMouseUp(e)
        If UCase(m_SketchMode) = UCase("FreeHand") And ActionInProgress Then
            RedlineFinished()

            m_coordinates = New CoordinateCollection()
            Map.Invalidate()
            MyBase.Map.Cursor = New Cursor(My.Resources.redline.Handle)

            ' signal to other mapactions that the action is completed
            ActionInProgress = False
        End If
    End Sub
    Protected Overrides Sub OnMapMouseMove(ByVal e As Esri.ArcGIS.Mobile.MapMouseEventArgs)
        'MyBase.OnMapMouseMove(e)
        m_mousePosition = e.MapCoordinate

        If (m_coordinates.Count < 2) Then
            Return
        End If

        If UCase(m_SketchMode) = UCase("FreeHand") Then
            m_coordinates.Insert(m_coordinates.Count - 1, m_mousePosition)

            If (Map IsNot Nothing) Then
                Map.Invalidate()
            End If

        End If


    End Sub
    Protected Overrides Sub OnMapMouseDown(ByVal e As Esri.ArcGIS.Mobile.MapMouseEventArgs)
        ' MyBase.OnMapMouseDown(e)
        Map.Focus()


        If (e.MapCoordinate Is Nothing) Then
            Return
        End If
        'If ActionInProgress And m_PointAtTapOnly = True Then
        '    m_coordinates.Insert(m_coordinates.Count - 1, m_mousePosition)

        '    If (Map IsNot Nothing) Then
        '        Map.Invalidate()
        '    End If
        'ElseIf ActionInProgress And m_DoubleTapToFinish = False Then
        '    ActionInProgress = False
        '    RedlineFinished()
        '    MyBase.Map.Cursor = New Cursor(My.Resources.redline.Handle)

        'Else
        ' signal to other mapactions that the action is executing
        ActionInProgress = True

        If (m_coordinates.Count = 0) Then

            m_coordinates.Add(e.MapCoordinate)
            m_coordinates.Add(e.MapCoordinate)
            If (Map IsNot Nothing) Then
                Map.Invalidate()
            End If
            'ElseIf (m_coordinates.Count = 2) Then

            '    If (m_measureMethod = EsriMeasureMethod.MultiPoint) Then
            '        m_coordinates.Insert(1, e.MapCoordinate)
            '    End If

            'ElseIf (m_measureMethod = EsriMeasureMethod.MultiPoint) Then
            '    m_coordinates.Insert(m_coordinates.Count - 1, e.MapCoordinate)
        Else
            m_coordinates.Insert(m_coordinates.Count - 1, m_mousePosition)
            If (Map IsNot Nothing) Then
                Map.Invalidate()
            End If
        End If

    End Sub
    Private Sub MapPaint(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.MapPaintEventArgs)

        If m_coordinates.Count = 0 Then Return
        If m_coordinates.Count = 1 Then
            e.Display.DrawPoint(New Pen(m_lineColor, m_lineWidth), New SolidBrush(m_lineColor), 1, m_coordinates(0))

            Return

        End If
        If m_coordinates.Count = 2 Then
            If m_coordinates(0).EquivalentTo(m_coordinates(1)) Then
                e.Display.DrawPoint(New Pen(m_lineColor, m_lineWidth), New SolidBrush(m_lineColor), 1, m_coordinates(0))

                Return
            End If

        End If
        Dim g As Graphics = e.Graphics

        'If m_FLay.GeometryType = GeometryType.Polygon Then
        '    e.Display.DrawPolygon(New Pen(m_lineColor, m_lineWidth), Nothing, m_coordinates)
        'Else
        e.Display.DrawPolyline(New Pen(m_lineColor, m_lineWidth), m_coordinates)
        'End If




    End Sub
    Protected Overrides Sub OnActiveChanged(ByVal active As Boolean)

        If active Then

            AddHandler Map.MouseDoubleClick, AddressOf MouseDoubleClick
            AddHandler Map.KeyUp, AddressOf MapKeyUp
            AddHandler Map.KeyDown, AddressOf MapKeyDown
            AddHandler Map.Paint, AddressOf MapPaint
            MyBase.Map.Cursor = New Cursor(My.Resources.redline.Handle)


        Else
            RemoveHandler Map.MouseDoubleClick, AddressOf MouseDoubleClick
            RemoveHandler Map.KeyUp, AddressOf MapKeyUp
            RemoveHandler Map.KeyDown, AddressOf MapKeyDown
            RemoveHandler Map.Paint, AddressOf MapPaint
            MyBase.Map.Cursor = Cursors.Default
            m_coordinates = New CoordinateCollection()
            Map.Invalidate()
            ' signal to other mapactions that the action is completed
            ActionInProgress = False
        End If
        MyBase.OnActiveChanged(active)

    End Sub
    Private Sub MapKeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)

        MyBase.OnMapKeyDown(e)
        If e.KeyValue = 27 Then
            m_coordinates.Clear()
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
            If m_btn IsNot Nothing And MyBase.Map IsNot Nothing Then
                m_btn.Location = New System.Drawing.Point(MyBase.Map.Width - 80, 85)
            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
    Private Sub redlineMapAction_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        Try
            'Remove the button from the map 
            If m_btn IsNot Nothing Then
                If m_btn.Parent IsNot Nothing Then
                    m_btn.Parent.Controls.Remove(m_btn)
                End If

            End If
            'Release the other objects
            m_FLay = Nothing

            If m_RedlineForm IsNot Nothing Then
                m_RedlineForm.Dispose()
            End If
            m_RedlineForm = Nothing

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
            Return
        End Try
    End Sub
    Private Sub redlineMapAction_StatusChanged(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.MapActions.MapActionStatusChangedEventArgs) Handles Me.StatusChanged
        Try
            'Check the map actions status
            If e.StatusId = Esri.ArcGIS.Mobile.MapActions.MapAction.Activated Then
                'If the button has been created, update the buttons image
                If m_btn IsNot Nothing Then
                    m_btn.BackgroundImage = My.Resources.penDown
                End If

            ElseIf e.StatusId = Esri.ArcGIS.Mobile.MapActions.MapAction.Deactivated Then
                'If the button has been created, update the buttons image
                If m_btn IsNot Nothing Then
                    m_btn.BackgroundImage = My.Resources.Pen
                End If


            End If

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
            Return
        End Try
    End Sub
    Public Sub InkBtnClick(ByVal sender As Object, ByVal e As MouseEventArgs)
        Try
            'Release or assign the map action
            If MyBase.Map.CurrentMapAction Is Me Then
                MyBase.Map.CurrentMapAction = Nothing
            Else
                MyBase.Map.CurrentMapAction = Me
            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
#End Region
    Public Property GPSLocation() As GPSLocationDetails
        Get
            Return m_GPSVal
        End Get
        Set(ByVal value As GPSLocationDetails)
            m_GPSVal = value
        End Set
    End Property
    Private Sub m_RedlineForm_CheckGPS() Handles m_RedlineForm.CheckGPS
        RaiseEvent checkGPS()
        If (m_GPSVal IsNot Nothing) Then
            '    MsgBox("")
            If m_GPSVal.SpatialReference.FactoryCode <> Map.SpatialReference.FactoryCode Then

                m_coordinates.Add(Map.SpatialReference.FromGps(m_GPSVal.Coordinate))
                'Me.Geometry = New Esri.ArcGIS.Mobile.Geometries.Point(m_Map.SpatialReference.FromDegreeMinuteSecond(m_GPSVal.LongitudeToDegreeMinutesSeconds), m_Map.SpatialReference.FromDegreeMinuteSecond(m_GPSVal.LatitudeToDegreeMinutesSeconds))

                'Me.Geometry = m_GPSVal()
            Else
                m_coordinates.Add(m_GPSVal.Coordinate)



            End If

        End If

        'm_EditPanel.GPSLocation = m_GPSVal
    End Sub

    Private Sub m_RedlineForm_RaiseMessage(ByVal Message As String) Handles m_RedlineForm.RaiseMessage
        RaiseEvent RaiseMessage(Message)

    End Sub

    Private Sub m_RedlineForm_RecordSaved(ByVal LayerName As String, ByVal pGeo As Esri.ArcGIS.Mobile.Geometries.Geometry, ByVal OID As Integer) Handles m_RedlineForm.RecordSaved
        ' MsgBox("Record Saved", MsgBoxStyle.Information)
        RaiseEvent RaiseMessage(Globals.appConfig.EditControlOptions.UIComponents.SavedMessage)

    End Sub
End Class
