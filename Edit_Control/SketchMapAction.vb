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
Imports Esri.ArcGIS.Mobile.FeatureCaching
Imports Esri.ArcGIS.Mobile.Geometries
Imports System.Windows.Forms
Imports System
Imports System.Drawing
Imports MobileControls.EditControl
Imports MobileControls
Imports Esri.ArcGISTemplates


Public Class SketchMapAction
    Inherits Esri.ArcGIS.Mobile.WinForms.MapAction
    Private m_mousePosition As Coordinate
    Private m_CoordsCurrent As CoordinateCollection

    Private m_lineColor As Color = Color.Red
    Private m_LayerName As String

    Private m_SketchMode As String = "Freehand"
    Private m_LineWidth As Integer = 5
    Private m_PointWidth As Integer = 5

    Private m_Fnt As System.Drawing.Font
    Private m_FntSize As Single
    Private WithEvents m_Map As Esri.ArcGIS.Mobile.WinForms.Map
    Private m_SketchType As GeometryType
    Private m_CurGeo As Geometry = Nothing
   
    Private m_Pen As Pen
    Private m_Brush As New SolidBrush(Color.Transparent)
    Private m_penLineFlash As Pen = Pens.Cyan
    Private m_penFlash As Pen = New Pen(Color.Purple, 10)
    Private m_brushFlash As SolidBrush = New SolidBrush(Color.Cyan)
    Private m_PointSize As Integer = 10

    Public Event SketchComplete()
    Public Property SketchMode As String
        Get
            Return m_SketchMode
        End Get
        Set(value As String)
            m_SketchMode = value


        End Set
    End Property
    Public Property GeoType As GeometryType
        Get
            Return m_SketchType
        End Get
        Set(value As GeometryType)
            m_SketchType = value


        End Set
    End Property

    
    Public Property ManualSetMap As Esri.ArcGIS.Mobile.WinForms.Map
        Get
            Return Map
        End Get
        Set(value As Esri.ArcGIS.Mobile.WinForms.Map)
            m_Map = value


        End Set
    End Property
    Public Property geometery As Geometry
        Get
            Return m_CurGeo
        End Get
        Set(value As Geometry)
            m_CurGeo = value
            m_CoordsCurrent.Clear()


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

        m_Pen = New Pen(Color.Cyan)
        m_Pen.Width = 3

    End Sub

    Public Function InitControl(ByVal container As Control, Optional ByVal LayerName As String = "") As Boolean
        Try
            Single.TryParse(GlobalsFunctions.appConfig.ApplicationSettings.FontSize, m_FntSize)

            If Not IsNumeric(m_FntSize) Then
                m_FntSize = 10
            ElseIf m_FntSize = 0 Then
                m_FntSize = 10
            End If


            m_Fnt = New System.Drawing.Font("Microsoft Sans Serif", m_FntSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)




            Return True

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try

    End Function

#End Region

#Region "Events"



    Private Sub UpdateGeoWithCoordinate()
        Try
            'Convert the stoke to a mobile geo

            If m_SketchType = GeometryType.Polygon Then





                m_CurGeo = New Polygon(m_CoordsCurrent.Clone)
                If UCase(m_SketchMode) = UCase("FreeHand") And ActionInProgress Then

                Else
                    ActionInProgress = False
                End If



            ElseIf m_SketchType = GeometryType.Polyline Then


                m_CurGeo = New Polyline(m_CoordsCurrent.Clone)



                'If UCase(m_SketchMode) = UCase("FreeHand") And ActionInProgress Then

                'Else
                '    ActionInProgress = False
                'End If


            ElseIf m_SketchType = GeometryType.Point Then


                If m_CoordsCurrent.Count = 1 Then
                    m_CurGeo = New Esri.ArcGIS.Mobile.Geometries.Point(m_CoordsCurrent(0).Clone)

                End If


                ActionInProgress = False

            ElseIf m_SketchType = 6 Then


                If m_CoordsCurrent.Count = 1 Then
                    m_CurGeo = New Esri.ArcGIS.Mobile.Geometries.Point(m_CoordsCurrent(0).Clone)

                End If


                ActionInProgress = False

            End If
            '
            'If there is not shape, prompt the user and exit
            If m_CurGeo Is Nothing Then

                Map.Invalidate()
                'delete the strokes
                ' m_CoordsList.Clear()
                m_CoordsCurrent.Clear()
                Return
            End If
            Try

                'Try to set the geometry from the stroke to the edit panel
                'm_EditPanel.Geometry = pGeo
                'Cleanup

            Catch ex As Exception
                'Capture a invalid geometry

                'delete the strokes

                m_CoordsCurrent.Clear()
                'Prompt the user
                Dim st As New StackTrace
                MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                st = Nothing
                'Clean up and return

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
            ActionInProgress = False
            UpdateGeoWithCoordinate()
            RaiseEvent SketchComplete()



        End If

    End Sub
    Protected Overrides Sub OnMapMouseUp(ByVal e As Esri.ArcGIS.Mobile.MapMouseEventArgs)
        MyBase.OnMapMouseUp(e)
        'If m_DT Is Nothing Then Return

        If UCase(m_SketchMode) = UCase("FreeHand") And ActionInProgress And (m_SketchType <> GeometryType.Point And m_SketchType <> 6) Then
            UpdateGeoWithCoordinate()


            m_Map.Cursor = New Cursor(My.Resources.Freehand.Handle)

            ' signal to other mapactions that the action is completed
            ActionInProgress = False
            RaiseEvent SketchComplete()
        ElseIf m_SketchType = 6 Or m_SketchType = GeometryType.Point Then
            UpdateGeoWithCoordinate()
          
            m_Map.Cursor = New Cursor(My.Resources.Freehand.Handle)

            ' signal to other mapactions that the action is completed
            ActionInProgress = False

            RaiseEvent SketchComplete()

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

        If UCase(m_SketchMode).ToUpper = UCase("FreeHand").ToUpper And (m_SketchType <> GeometryType.Point And m_SketchType <> 6) Then
            If m_SketchType = GeometryType.Polygon Then
                If (m_CoordsCurrent.Count < 2) Then
                    Return
                End If
                m_CoordsCurrent.Insert(m_CoordsCurrent.Count - 1, m_mousePosition)
            Else
                m_CoordsCurrent.Add(m_mousePosition)

            End If

            If (m_Map IsNot Nothing) Then
                m_Map.Invalidate()
            End If

        End If

    End Sub
    Protected Overrides Sub OnMapMouseDown(ByVal e As Esri.ArcGIS.Mobile.MapMouseEventArgs)
        ' MyBase.OnMapMouseDown(e)
        Map.Focus()

        'If m_DT Is Nothing Then Return

        If (e.MapCoordinate Is Nothing) Then
            Return
        End If
        ActionInProgress = True
        If UCase(m_SketchMode).ToUpper = UCase("FreeHand").ToUpper Then
            m_CoordsCurrent.Clear()


        End If

        ' signal to other mapactions that the action is executing

        addCoordToGeo(e.MapCoordinate)



        Map.Invalidate()

    End Sub
  
    Protected Overrides Sub OnActiveChanged(ByVal active As Boolean)

        If active Then

            AddHandler Map.MouseDoubleClick, AddressOf MouseDoubleClick
            AddHandler Map.KeyUp, AddressOf MapKeyUp
            AddHandler Map.KeyDown, AddressOf MapKeyDown

        Else
            RemoveHandler Map.MouseDoubleClick, AddressOf MouseDoubleClick
            RemoveHandler Map.KeyUp, AddressOf MapKeyUp
            RemoveHandler Map.KeyDown, AddressOf MapKeyDown
            m_Map.Cursor = Cursors.Default
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
        ElseIf e.KeyCode = Keys.Enter Then
            If Me.Active Then
                If ActionInProgress Then
                    UpdateGeoWithCoordinate()

                    m_Map.Cursor = New Cursor(My.Resources.Freehand.Handle)

                    ' signal to other mapactions that the action is completed
                    ActionInProgress = False

                    RaiseEvent SketchComplete()
                End If

            End If
            

        End If


    End Sub
    Private Sub MapKeyUp(ByVal sender As Object, ByVal e As KeyEventArgs)
        MyBase.OnMapKeyUp(e)



    End Sub


    Private Sub SketchingMapAction_StatusChanged(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.WinForms.MapActionStatusChangedEventArgs) Handles Me.StatusChanged
        Try
            'Check the map actions status
            If e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Activated Then
                'If the button has been created, update the buttons image




                If m_SketchMode.ToUpper = "Freehand".ToUpper And m_SketchType <> GeometryType.Point Then

                    m_Map.Cursor = New Cursor(My.Resources.Freehand.Handle)

                Else
                    m_Map.Cursor = New Cursor(My.Resources.target.Handle)

                End If

            ElseIf e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Deactivated Then


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

    Private Sub addCoordToGeo(ByVal newCoord As Coordinate)
        'If m_DT Is Nothing Then Return

        If m_CoordsCurrent Is Nothing Then
            m_CoordsCurrent = New CoordinateCollection

        End If

        If UCase(m_SketchMode).ToUpper = UCase("FreeHand").ToUpper And (m_SketchType <> GeometryType.Point And m_SketchType <> 6) Then
            '  m_CoordsCurrent = New CoordinateCollection
            m_CoordsCurrent.Clear()

            'm_CoordsList.Add(m_CoordsCurrent)

        End If




        If (m_CoordsCurrent.Count = 0) Then
            ' m_CoordsList.Add(m_CoordsCurrent)

            m_CoordsCurrent.Add(newCoord)
            If m_SketchType = GeometryType.Polygon Then

                m_CoordsCurrent.Add(newCoord)


            End If


        Else
            If m_SketchType = GeometryType.Polygon Then
                If m_CoordsCurrent(m_CoordsCurrent.Count - 1).X = newCoord.X And m_CoordsCurrent(m_CoordsCurrent.Count - 1).Y = newCoord.Y Then
                    ' MsgBox("SameCoord:")

                Else
                    m_CoordsCurrent.Insert(m_CoordsCurrent.Count - 1, newCoord)
                End If

            ElseIf m_SketchType = GeometryType.Point Then
                ' m_CoordsList.Add(m_CoordsCurrent)
                m_CoordsCurrent.Clear()

                m_CoordsCurrent.Add(newCoord)

            ElseIf m_SketchType = 6 Then
                ' m_CoordsList.Add(m_CoordsCurrent)
                m_CoordsCurrent.Clear()

                m_CoordsCurrent.Add(newCoord)

            Else


                m_CoordsCurrent.Add(newCoord)

            End If
        End If

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
        Try
           
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
            Return
        End Try
    End Sub

    Private Sub m_Map_Paint(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.WinForms.MapPaintEventArgs) Handles m_Map.MapPaint
        Try

            If e.MapSurface Is Nothing Then Return




            If m_SketchType = GeometryType.Point Or m_SketchType = GeometryType.Multipoint Then

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

            End If



            'If drawing the geo of the editable record
            '   If m_CurGeo IsNot Nothing Then
            '        If m_CurGeo.IsValid Then


            '        e.MapSurface.DrawGeometry(m_Pen, m_Brush, m_PointSize, m_CurGeo)
            '        If (m_CurGeo.GeometryType = GeometryType.Polyline) Then
            '            If m_CurGeo.CurrentCoordinate IsNot Nothing Then
            '                e.MapSurface.DrawGeometry(m_Pen, m_Brush, m_PointSize, New Esri.ArcGIS.Mobile.Geometries.Point(m_CurGeo.CurrentCoordinate))
            '            End If


            '        End If
            '        Else
            '            If (m_CurGeo.GeometryType = GeometryType.Polygon) Then

            '            ElseIf (m_CurGeo.GeometryType = GeometryType.Polyline) Then

            '                If CType(m_CurGeo, Esri.ArcGIS.Mobile.Geometries.Polyline).Parts(0).Count = 1 Then
            '                    e.MapSurface.DrawGeometry(m_Pen, m_Brush, m_PointSize, New Esri.ArcGIS.Mobile.Geometries.Point(CType(m_CurGeo, Esri.ArcGIS.Mobile.Geometries.Polyline).Parts(0)(0)))

            '                End If
            '            End If


            '        End If
            '    End If


        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try

    End Sub

End Class
