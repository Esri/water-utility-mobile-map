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


Imports ESRI.ArcGIS.Mobile
Imports Esri.ArcGIS.Mobile.FeatureCaching
Imports ESRI.ArcGIS.Mobile.WebServices
Imports Esri.ArcGIS.Mobile.DataProducts
Imports Esri.ArcGIS.Mobile.Geometries

Imports System.Windows.Forms
Imports System.Windows.Forms.Control
Imports System.Xml
Imports System.Diagnostics.Process
Imports System.IO
Imports System.Threading
Imports System.Resources
Imports System.Drawing

Imports Esri.ArcGISTemplates
Public Class mobileBufferMapAction
    Inherits Esri.ArcGIS.Mobile.WinForms.PanMapAction

    Private m_DownCur As System.Windows.Forms.Cursor
    Private m_NormCur As System.Windows.Forms.Cursor
    Private m_LastMouseLoc As System.Drawing.Point = New System.Drawing.Point



    Private m_ClickPoint As Coordinate
    Private m_BufferedPoint As Polygon
    Private m_Env As Envelope



    Private m_PntSize As Integer
    Private m_PointColor As Color = Color.Black
    Private m_PolyColor As Color = Color.Black


    Public Event GeoCreated(ByVal buffer As Polygon)


    Public Sub New()
        MyBase.New()
        '  MyBase.
        Dim s As System.IO.Stream = New System.IO.MemoryStream(My.Resources.PanCur)
        m_DownCur = New System.Windows.Forms.Cursor(s)
        s = New System.IO.MemoryStream(My.Resources.Finger)
        m_NormCur = New System.Windows.Forms.Cursor(s)


    End Sub
    Public Property ManualSetMap As Esri.ArcGIS.Mobile.WinForms.Map
        Get
            Return Map
        End Get
        Set(value As Esri.ArcGIS.Mobile.WinForms.Map)
            m_Map = value

            AddHandler m_Map.MapPaint, AddressOf Map_Paint

        End Set
    End Property

#Region "Defualt Overrides"



    Protected Overrides Sub OnActiveChanging(ByVal active As Boolean)
        Try

            'Me.Activating
            MyBase.OnActiveChanging(active)
        Catch ex As Exception

        End Try
    End Sub
  
    Public Overrides Function GetHashCode() As Integer
        Return MyBase.GetHashCode()
    End Function
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Return MyBase.Equals(obj)

    End Function
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub
  
    Public Overrides Sub Cancel()
        MyBase.Cancel()
    End Sub
    Protected Overrides Sub OnActiveChanged(ByVal active As Boolean)
        MyBase.OnActiveChanged(active)
    End Sub

    Protected Overrides Sub OnMapPaint(e As Esri.ArcGIS.Mobile.WinForms.MapPaintEventArgs)
        MyBase.OnMapPaint(e)

    End Sub
   
    Private WithEvents m_Map As Esri.ArcGIS.Mobile.WinForms.Map


    Protected Overrides Sub OnSetMap(ByVal map As Esri.ArcGIS.Mobile.WinForms.Map)
        MyBase.OnSetMap(map)
        'm_Map = map

    End Sub
#End Region
#Region "Public Functions"
    Public Sub initControl()
        'AddHandler m_map.MapPaint, AddressOf Map_Paint

        m_PntSize = GlobalsFunctions.appConfig.SearchPanel.AreaSearchValues.PointSize
        m_PointColor = GlobalsFunctions.stringToColor(GlobalsFunctions.appConfig.SearchPanel.AreaSearchValues.PointColor)
        m_PolyColor = GlobalsFunctions.stringToColor(GlobalsFunctions.appConfig.SearchPanel.AreaSearchValues.AreaColor)




    End Sub
    Public Sub clearGeo()
        m_ClickPoint = Nothing
        m_BufferedPoint = Nothing
    End Sub

    Public Sub setExtent(ByVal env As Envelope, bFull As Boolean)
        m_Env = env

        If m_Env IsNot Nothing Then


            If m_ClickPoint IsNot Nothing Then
                If m_Env IsNot Nothing Then

                    m_Env.CenterAt(m_ClickPoint)
                    m_BufferedPoint = m_Env.ToPolygon


                Else
                    m_BufferedPoint = Nothing

                End If

            ElseIf bFull Then
                m_BufferedPoint = m_Env.ToPolygon

            Else


                m_BufferedPoint = Nothing


            End If
        End If
        If m_Map IsNot Nothing Then
            m_Map.Invalidate()
        End If

        If m_BufferedPoint IsNot Nothing Then
            RaiseEvent GeoCreated(m_BufferedPoint)
        End If

    End Sub
#End Region
#Region "Events"
   
    Private Sub mobileBufferMapAction_StatusChanged(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.WinForms.MapActionStatusChangedEventArgs) Handles Me.StatusChanged

        'Monitors the status of this map action
        If e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Activated Then
            'Change the button image to display the action is activated

            m_Map.Cursor = m_NormCur
        ElseIf e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Deactivated Then

        ElseIf e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Deactivating Then
        End If

    End Sub


#End Region
#Region "PrivateFunctions"

#End Region
#Region "Overrides"
    Protected Overrides Sub OnMapMouseDown(ByVal e As Esri.ArcGIS.Mobile.MapMouseEventArgs)
        m_LastMouseLoc = e.Location
        m_Map.Cursor = m_DownCur
        MyBase.OnMapMouseDown(e)
    End Sub
    Protected Overrides Sub OnMapMouseUp(ByVal e As Esri.ArcGIS.Mobile.MapMouseEventArgs)
        MyBase.OnMapMouseUp(e)
        m_Map.Cursor = m_NormCur

        If m_LastMouseLoc.Equals(e.Location) Then
            'If the map action has not been added to the map return

            'verify the map is valid
            If m_Map.IsValid = False Then Return
            'Determine buffer distance

            m_ClickPoint = e.MapCoordinate()

            'Create a new envelope to search by
            m_Env.CenterAt(m_ClickPoint)
            m_BufferedPoint = m_Env.ToPolygon
            m_Map.Invalidate()



            RaiseEvent GeoCreated(m_BufferedPoint)      'Set the enveloper to the query
        End If

    End Sub
#End Region



    Private Sub Map_Paint(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.WinForms.MapPaintEventArgs)
        If m_ClickPoint IsNot Nothing Then

            If m_ClickPoint.IsEmpty = False Then

                e.MapSurface.DrawPoint(New Pen(m_PointColor), New SolidBrush(m_PointColor), m_PntSize, m_ClickPoint)
            End If
        End If

        If m_BufferedPoint IsNot Nothing Then

            If m_BufferedPoint.IsEmpty = False Then
                e.MapSurface.DrawPolygon(New Pen(m_PolyColor, 5), New SolidBrush(Color.Transparent), m_BufferedPoint)
            End If
        End If

        'e.Display.DrawPoint(New Pen(m_lineColor, m_PointWidth), New SolidBrush(m_lineColor), m_PointWidth, m_CoordsCurrent(0))



        'e.Display.DrawPolyline(New Pen(m_lineColor, m_LineWidth), m_CoordsCurrent)
        'e.Display.DrawPoint(New Pen(m_lineColor, m_PointWidth), New SolidBrush(m_lineColor), m_PointWidth, m_CoordsCurrent(m_CoordsCurrent.Count - 1))

    End Sub
End Class
