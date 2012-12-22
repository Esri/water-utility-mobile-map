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


Public Class CreateFeaturesMapAction
    Inherits Esri.ArcGIS.Mobile.MapActions.MapAction

    Private WithEvents m_cboCreateLayers As ComboBox
    Private m_LongString As String = " "

    Public Event RaiseMessage(ByVal Message As String)

    'Private m_RadioOnTwo As Boolean = True

    'Private m_FLay As FeatureLayer
    Private m_btn As Button
    'Ink Collector
    'Edit panel to capture redlines attributes
    Private WithEvents m_EditPanel As MobileControls.EditControl
    Private m_mousePosition As Coordinate
    ' stores current mouse position
    Private m_coordinates As CoordinateCollection
    ' stores all coordinates used in drawing the measure line
    Private m_lineColor As Color = Color.Red
    Private m_LayerName As String
    'Private m_DoubleTapToFinish As Boolean = False
    'Private m_PointAtTapOnly As Boolean = False
    Private m_SketchMode As String
    Private m_ShowCombo As Boolean = True


    Private m_lineWidth As Integer = 5
    Public Event checkGPS()
    Private m_GPSVal As GPSLocationDetails
    Private m_Fnt As System.Drawing.Font
    Private m_FntSize As Single
    Private m_DT As FeatureDataTable
#Region "Public Methods"
    Public Sub New()
        MyBase.New()
        m_coordinates = New CoordinateCollection()

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
                If m_cboCreateLayers Is Nothing Then
                    m_cboCreateLayers = New ComboBox
                    m_cboCreateLayers.Visible = False
                    m_cboCreateLayers.ForeColor = Drawing.Color.White
                    m_cboCreateLayers.BackColor = Drawing.Color.DarkBlue
                    m_cboCreateLayers.DropDownStyle = ComboBoxStyle.DropDownList
                    '    m_cboCreateLayers.DropDownHeight = 300
                    m_cboCreateLayers.Font = m_Fnt
                    m_cboCreateLayers.Width = 200
                    m_cboCreateLayers.Left = m_btn.Left - m_cboCreateLayers.Width - 10
                    m_cboCreateLayers.Top = CInt(m_btn.Top + (m_btn.Height / 2) - (m_cboCreateLayers.Height / 2))
                    '  AddHandler m_cboCreateLayers.SelectedIndexChanged, AddressOf m_cboCreateLayers_SelectedIndexChanged
                    ' AddHandler m_cboCreateLayers.MouseDoubleClick, AddressOf m_cboCreateLayers_MouseDoubleClick

                    'Locate it to the left of the button and add to the map
                    MyBase.Map.Controls.Add(m_cboCreateLayers)
                End If
                'Add the button to the map
                MyBase.Map.Controls.Add(m_btn)
            End If

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

            Return False
        End Try
        Return True

    End Function
    Public Function InitControl(ByVal container As Control, Optional ByVal LayerName As String = "") As Boolean
        Try
            Single.TryParse(GlobalsFunctions.appConfig.ApplicationSettings.FontSize, m_FntSize)

            If Not IsNumeric(m_FntSize) Then
                m_FntSize = 10
            ElseIf m_FntSize = 0 Then
                m_FntSize = 10
            End If


            m_Fnt = New System.Drawing.Font("Microsoft Sans Serif", m_FntSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)


            ''Get the redline layer
            'Dim pMblLay As Esri.ArcGIS.Mobile.MobileServices.MobileCacheMapLayer

            'pMblLay = CType(Map.MapLayers(m_LayerName), MobileCacheMapLayer)

            ''Return if it was not found
            'If pMblLay Is Nothing Then
            '    MyBase.Map.Controls.Remove(m_btn)
            '    Return False

            'End If


            addMapButton()


            'Create the form is not already created
            If m_EditPanel Is Nothing Then
                'Create a new edit control
                m_EditPanel = New MobileControls.EditControl(MyBase.Map, Nothing)
               'Fill the containter
                m_EditPanel.Dock = DockStyle.Fill
                'Tell the panel to draw the geometry
                m_EditPanel.DrawGeo = False

                m_EditPanel.Height = container.Parent.Height



                'Clear the controls on the cotainer
                container.Controls.Clear()
                'Add the edit panel
                container.Dock = DockStyle.Fill

                container.Controls.Add(m_EditPanel)
                container.Refresh()
                container.Update()

                m_EditPanel.ResumeLayout(True)


                m_EditPanel.Refresh()
                m_EditPanel.Update()
                'Create a new row on the panel


            End If
            ''Set the layer to a global
            'pMblLay.Visible = True

            'm_FLay = CType(pMblLay.Layer, FeatureLayer)
            'If m_FLay Is Nothing Then
            '    MyBase.Map.Controls.Remove(m_btn)
            '    Return False
            'End If
            LoadCreateLayers()
            If m_cboCreateLayers.Items.Count > 0 Then
                setDataTable(m_cboCreateLayers.Text)
            End If

            'Create a new row
            SetNewRow()
            setCurrentLayer()
            Return True

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try

    End Function

#End Region
#Region "Private Methods"

    Private Sub setCurrentLayer()
        Dim pCurConfgi As MobileConfigClass.MobileConfigMobileMapConfigCreateFeaturePanelLayersLayer = m_cboCreateLayers.SelectedItem

        m_SketchMode = m_cboCreateLayers.SelectedValue
        If m_SketchMode.ToUpper = "Freehand".ToUpper Then
            m_EditPanel.GPSButtonVisible = False
        Else
            m_EditPanel.GPSButtonVisible = True

        End If
        If pCurConfgi.SketchColor <> "" Then
            m_lineColor = System.Drawing.Color.FromArgb(Int32.Parse(pCurConfgi.SketchColor.Replace("#", ""), Globalization.NumberStyles.HexNumber))

        End If
    End Sub

    Private Sub setDataTable(ByVal LayerName As String)
        Try

            'Gets a datatable from a feature layer
            Dim pFl As Esri.ArcGIS.Mobile.MobileServices.FeatureLayer = GlobalsFunctions.GetFeatureLayer(LayerName, MyBase.Map)
            If pFl Is Nothing Then Return

            m_DT = pFl.GetDataTable()

            pFl = Nothing


        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub SetNewRow()

        If m_DT IsNot Nothing Then

            'Create a new row
            Dim pDR As DataRow = m_DT.NewRow

            m_EditPanel.CurrentRecord = CType(pDR, FeatureDataRow)
        End If
    End Sub
    'Private Sub SetNewRow()
    '    Try

    '        'If the redline layer was not set, return
    '        If m_FLay Is Nothing Then Return
    '        'if the form is not initilize, then exit
    '        If m_EditPanel Is Nothing Then Return
    '        'Get a table from the layer
    '        Dim pDT As FeatureDataTable = m_FLay.GetDataTable()
    '        'Create a new row and assign it to the edit form
    '        m_EditPanel.CurrentRecord = pDT.NewRow
    '        'Cleanup
    '        pDT = Nothing

    '    Catch ex As Exception
    '        Dim st As New StackTrace
    '        MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
    '        st = Nothing
    '        Return
    '    End Try

    'End Sub
    Private Sub LoadCreateLayers()
        Try
            m_LongString = ""
            'Load the inspection types to the inspection dropdown
            If m_cboCreateLayers Is Nothing Then Return
            RemoveHandler m_cboCreateLayers.SelectedIndexChanged, AddressOf m_cboCreateLayers_SelectedIndexChanged


            For Each createLay In GlobalsFunctions.appConfig.CreateFeaturePanel.Layers.Layer
                If createLay.Name.Length > m_LongString.Length Then
                    m_LongString = createLay.Name
                End If
            Next
            m_cboCreateLayers.DataSource = GlobalsFunctions.appConfig.CreateFeaturePanel.Layers.Layer


            m_cboCreateLayers.DisplayMember = "Name"
            m_cboCreateLayers.ValueMember = "EditTool"
            If GlobalsFunctions.appConfig.CreateFeaturePanel.Layers.Layer.Count = 1 Then
                m_cboCreateLayers.Visible = False
                m_ShowCombo = False

            End If
            AddHandler m_cboCreateLayers.SelectedIndexChanged, AddressOf m_cboCreateLayers_SelectedIndexChanged

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try
    End Sub
#End Region
#Region "Events"

    Private Sub m_cboCreateLayers_DropDown(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_cboCreateLayers.DropDown

        Dim g As Graphics = m_cboCreateLayers.CreateGraphics
        For i = 0 To 5
            ' m_layCBO.Width = m_layCBO.Width + 40
            '    m_cboCreateLayers.Height = m_cboCreateLayers.Height + 5

            Dim pFnt As System.Drawing.Font = New System.Drawing.Font(m_cboCreateLayers.Font.Name, m_cboCreateLayers.Font.Size + i, System.Drawing.FontStyle.Bold)
            m_cboCreateLayers.Width = g.MeasureString(m_LongString, pFnt).Width + 50


            m_cboCreateLayers.Font = pFnt

            m_cboCreateLayers.Left = m_btn.Left - m_cboCreateLayers.Width - 10
            m_cboCreateLayers.Top = CInt(m_btn.Top + (m_btn.Height / 2) - (m_cboCreateLayers.Height / 2))

        Next
        g.Dispose()
        g = Nothing
        ' End If
    End Sub

    Private Sub m_cboCreateLayers_DropDownClosed(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_cboCreateLayers.DropDownClosed

        Dim g As Graphics = m_cboCreateLayers.CreateGraphics

        m_cboCreateLayers.Width = g.MeasureString(m_cboCreateLayers.Text, m_Fnt).Width + 50


        m_cboCreateLayers.Font = m_Fnt

        m_cboCreateLayers.Left = m_btn.Left - m_cboCreateLayers.Width - 10
        m_cboCreateLayers.Top = CInt(m_btn.Top + (m_btn.Height / 2) - (m_cboCreateLayers.Height / 2))
        g.Dispose()
        g = Nothing
    End Sub

    Private Sub m_cboCreateLayers_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_cboCreateLayers.Enter


    End Sub
    Private Sub m_cboCreateLayers_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles m_cboCreateLayers.MouseClick


    End Sub

    Private Sub m_cboCreateLayers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_cboCreateLayers.SelectedIndexChanged


        setDataTable(m_cboCreateLayers.Text)

        'Create a new row
        SetNewRow()

        setCurrentLayer()



    End Sub
    Private Sub GPSCreateFinished()
        Try
            'Convert the stoke to a mobile geo
            Dim pGeo As Geometry = Nothing
            If m_DT.FeatureLayer.GeometryType = GeometryType.Polygon And m_coordinates.Count > 2 Then

                pGeo = New Polygon(m_coordinates.Clone)


            ElseIf m_DT.FeatureLayer.GeometryType = GeometryType.Polyline And m_coordinates.Count >= 2 Then
                pGeo = New Polyline(m_coordinates.Clone)

            Else


                Return

            End If
            '
            'If there is not shape, prompt the user and exit
            If pGeo Is Nothing Then

                Map.Invalidate()
                'delete the strokes


                Return
            End If
            Try

                'Try to set the geometry from the stroke to the edit panel
                m_EditPanel.Geometry = pGeo
                'Cleanup
                pGeo = Nothing
            Catch ex As Exception
                'Capture a invalid geometry

                'delete the strokes
                m_coordinates.Clear()
                'Prompt the user
                Dim st As New StackTrace
                MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                st = Nothing
                'Clean up and return
                pGeo = Nothing
                Map.Invalidate()
                Return

            End Try

            'The geometry was save to the edit panel, delete the strokes
            'm_coordinates.Clear()

            'refresh the map
            '  Map.Refresh()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
            Return
        End Try

    End Sub
    Private Sub CreateFinished()
        Try
            'Convert the stoke to a mobile geo
            Dim pGeo As Geometry = Nothing
            If m_DT.FeatureLayer.GeometryType = GeometryType.Polygon Then

                pGeo = New Polygon(m_coordinates)
                m_coordinates.Clear()

            ElseIf m_DT.FeatureLayer.GeometryType = GeometryType.Polyline Then
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
                m_EditPanel.Geometry = pGeo
                'Cleanup
                pGeo = Nothing
            Catch ex As Exception
                'Capture a invalid geometry

                'delete the strokes
                m_coordinates.Clear()
                'Prompt the user
                Dim st As New StackTrace
                MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
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
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
            Return
        End Try

    End Sub
    Private Sub MouseDoubleClick(ByVal sender As Object, ByVal e As MapMouseEventArgs)
        'If UCase(m_SketchMode) <> UCase("FreeHand") And ActionInProgress Then
        '    CreateFinished()

        '    m_coordinates = New CoordinateCollection()
        '    Map.Invalidate()
        '    MyBase.Map.Cursor = New Cursor(My.Resources.redline.Handle)

        '    ' signal to other mapactions that the action is completed
        '    ActionInProgress = False

        'End If
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
            CreateFinished()

            m_coordinates = New CoordinateCollection()
            Map.Invalidate()
            MyBase.Map.Cursor = New Cursor(My.Resources.redline.Handle)

            ' signal to other mapactions that the action is completed
            ActionInProgress = False
        ElseIf ActionInProgress Then
            GPSCreateFinished()
        End If
    End Sub
    Protected Overrides Sub OnMapMouseMove(ByVal e As Esri.ArcGIS.Mobile.MapMouseEventArgs)
        'MyBase.OnMapMouseMove(e)
        m_mousePosition = e.MapCoordinate


        If (m_coordinates.Count < 1) Then
            Return
        End If

        If UCase(m_SketchMode).ToUpper = UCase("FreeHand").ToUpper Then
            If m_DT.FeatureLayer.GeometryType = GeometryType.Polygon Then
                If (m_coordinates.Count < 2) Then
                    Return
                End If
                m_coordinates.Insert(m_coordinates.Count - 1, m_mousePosition)
            Else
                m_coordinates.Add(m_mousePosition)

            End If

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
            If m_DT.FeatureLayer.GeometryType = GeometryType.Polygon Then
                m_coordinates.Add(e.MapCoordinate)
            End If

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
            If m_DT.FeatureLayer.GeometryType = GeometryType.Polygon Then
                m_coordinates.Insert(m_coordinates.Count - 1, m_mousePosition)
            Else
                m_coordinates.Add(m_mousePosition)

            End If
            'm_coordinates.Add(m_mousePosition)

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

        'If m_DT.FeatureLayer.GeometryType = GeometryType.Polygon Then
        '    If m_coordinates.Count > 3 Then
        '        e.Display.DrawPolygon(New Pen(m_lineColor, m_lineWidth), Nothing, m_coordinates)
        '    End If

        'ElseIf m_DT.FeatureLayer.GeometryType = GeometryType.Polygon Then
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
            m_cboCreateLayers.Width = 200
            m_cboCreateLayers.Left = m_btn.Left - m_cboCreateLayers.Width - 10
            m_cboCreateLayers.Top = CInt(m_btn.Top + (m_btn.Height / 2) - (m_cboCreateLayers.Height / 2))

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
    Private Sub CreateFeaturesMapAction_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        Try
            'Remove the button from the map 
            If m_btn IsNot Nothing Then
                If m_btn.Parent IsNot Nothing Then
                    m_btn.Parent.Controls.Remove(m_btn)
                End If

            End If
            'Release the other objects

            If m_EditPanel IsNot Nothing Then
                m_EditPanel.Dispose()
            End If
            m_EditPanel = Nothing

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
            Return
        End Try
    End Sub
    Private Sub CreateFeaturesMapAction_StatusChanged(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.MapActions.MapActionStatusChangedEventArgs) Handles Me.StatusChanged
        Try
            'Check the map actions status
            If e.StatusId = Esri.ArcGIS.Mobile.MapActions.MapAction.Activated Then
                'If the button has been created, update the buttons image
                If m_btn IsNot Nothing Then
                    m_btn.BackgroundImage = My.Resources.penDown
                    m_cboCreateLayers.Visible = m_ShowCombo
                End If

            ElseIf e.StatusId = Esri.ArcGIS.Mobile.MapActions.MapAction.Deactivated Then
                'If the button has been created, update the buttons image
                If m_btn IsNot Nothing Then
                    m_btn.BackgroundImage = My.Resources.Pen
                    m_cboCreateLayers.Visible = False
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
            If MyBase.Map.CurrentMapAction Is Me Then
                MyBase.Map.CurrentMapAction = Nothing
            Else
                MyBase.Map.CurrentMapAction = Me
            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
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
    Private Sub m_EditPanel_CheckGPS() Handles m_EditPanel.CheckGPS
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
            GPSCreateFinished()

            Map.Invalidate()

        End If

        'm_EditPanel.GPSLocation = m_GPSVal
    End Sub

    Private Sub m_EditPanel_RaiseMessage(ByVal Message As String) Handles m_EditPanel.RaiseMessage
        RaiseEvent RaiseMessage(Message)

    End Sub

    Private Sub m_EditPanel_RecordClear() Handles m_EditPanel.RecordClear
        m_coordinates.Clear()

    End Sub

    Private Sub m_EditPanel_RecordSaved(ByVal LayerName As String, ByVal pGeo As Esri.ArcGIS.Mobile.Geometries.Geometry, ByVal OID As Integer) Handles m_EditPanel.RecordSaved
        m_coordinates.Clear()

        ' MsgBox("Record Saved", MsgBoxStyle.Information)
        RaiseEvent RaiseMessage(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.SavedMessage)

    End Sub
End Class
