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


Imports Esri.ArcGIS.Mobile.FeatureCaching


Public Class panExt
    Inherits Esri.ArcGIS.Mobile.WinForms.PanMapAction
    Dim m_DownCur As System.Windows.Forms.Cursor
    Dim m_NormCur As System.Windows.Forms.Cursor
    Public Sub New()
        MyBase.New()
        '  MyBase.
        Dim s As System.IO.Stream = New System.IO.MemoryStream(My.Resources.PanCur)
        m_DownCur = New System.Windows.Forms.Cursor(s)
        s = New System.IO.MemoryStream(My.Resources.Finger)
        m_NormCur = New System.Windows.Forms.Cursor(s)
    End Sub
    'Public Sub New(ByVal Cont As System.ComponentModel.IContainer)
    '    MyBase.New(Cont)
    '    Dim s As System.IO.Stream = New System.IO.MemoryStream(My.Resources.PanCur)
    '    m_DownCur = New System.Windows.Forms.Cursor(s)
    '    s = New System.IO.MemoryStream(My.Resources.Finger)
    '    m_NormCur = New System.Windows.Forms.Cursor(s)
    'End Sub
    Protected Overrides Sub OnMapMouseDoubleClick(ByVal e As ESRI.ArcGIS.Mobile.MapMouseEventArgs)
        FixZoom(False, e.MapCoordinate.X, e.MapCoordinate.Y)

        'MyBase.OnMapMouseDoubleClick(e)

    End Sub
    Public Overrides Function ToString() As String
        Return MyBase.ToString()
    End Function
    'Public Overrides Property Site() As System.ComponentModel.ISite
    '    Get
    '        Return MyBase.Site
    '    End Get
    '    Set(ByVal value As System.ComponentModel.ISite)
    '        MyBase.Site = value
    '    End Set
    'End Property
    Protected Overrides Sub OnMapPaint(ByVal e As Esri.ArcGIS.Mobile.WinForms.MapPaintEventArgs)
        MyBase.OnMapPaint(e)
    End Sub
    Protected Overrides Sub OnMapMouseWheel(ByVal e As ESRI.ArcGIS.Mobile.MapMouseEventArgs)
        MyBase.OnMapMouseWheel(e)
    End Sub
    Protected Overrides Sub OnMapMouseUp(ByVal e As ESRI.ArcGIS.Mobile.MapMouseEventArgs)

        MyBase.OnMapMouseUp(e)
        MyBase.Map.Cursor = m_NormCur
    End Sub
    Protected Overrides Sub OnMapMouseMove(ByVal e As ESRI.ArcGIS.Mobile.MapMouseEventArgs)
        MyBase.OnMapMouseMove(e)
    End Sub
    Protected Overrides Sub OnMapMouseDown(ByVal e As ESRI.ArcGIS.Mobile.MapMouseEventArgs)

        MyBase.OnMapMouseDown(e)
        MyBase.Map.Cursor = m_DownCur
    End Sub
    Protected Overrides Sub OnMapKeyUp(ByVal e As System.Windows.Forms.KeyEventArgs)
        MyBase.OnMapKeyUp(e)
    End Sub
    Protected Overrides Sub OnMapKeyPress(ByVal e As System.Windows.Forms.KeyPressEventArgs)
        MyBase.OnMapKeyPress(e)
    End Sub
    Protected Overrides Sub OnMapKeyDown(ByVal e As System.Windows.Forms.KeyEventArgs)
        MyBase.OnMapKeyDown(e)
    End Sub
    Protected Overrides Sub OnMapDoubleClick(ByVal e As System.EventArgs)
        MyBase.OnMapDoubleClick(e)
    End Sub
    Protected Overrides Sub OnActiveChanging(ByVal active As Boolean)
        Try

            'Me.Activating
            MyBase.OnActiveChanging(active)
        Catch ex As Exception

        End Try
    End Sub
    'Public Overrides Function InitializeLifetimeService() As Object
    '    Return MyBase.InitializeLifetimeService()
    'End Function
    'Protected Overrides Function GetService(ByVal service As System.Type) As Object
    '    Return MyBase.GetService(service)
    'End Function
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
    'Public Overrides Function CreateObjRef(ByVal requestedType As System.Type) As System.Runtime.Remoting.ObjRef
    '    Return MyBase.CreateObjRef(requestedType)

    'End Function
    'Protected Overrides ReadOnly Property CanRaiseEvents() As Boolean
    '    Get
    '        Return MyBase.CanRaiseEvents
    '    End Get
    'End Property
    Public Overrides Sub Cancel()
        MyBase.Cancel()
    End Sub
    Protected Overrides Sub OnActiveChanged(ByVal active As Boolean)
        MyBase.OnActiveChanged(active)
    End Sub

    Protected Overrides Sub OnSetMap(ByVal map As Esri.ArcGIS.Mobile.WinForms.Map)
        MyBase.OnSetMap(map)
    End Sub
    Private Sub FixZoom(ByVal bOut As Boolean, ByVal xCent As Double, ByVal yCent As Double)
        'Make sure the map is valid
        If MyBase.Map Is Nothing Then Return
        If MyBase.Map.IsValid = False Then Return
        'Determine which direction to zoom
        If (bOut) Then
            Dim pExt As ESRI.ArcGIS.Mobile.Geometries.Envelope
            'Get the map extent
            pExt = MyBase.Map.Extent()
            'Resize it
            pExt.Resize(1.5)
            'Center it
            pExt.CenterAt(xCent, yCent)

            'Set the map extent
            MyBase.Map.Extent = pExt
            'cleanup
            pExt = Nothing
        Else
            Dim pExt As ESRI.ArcGIS.Mobile.Geometries.Envelope
            'Get the map extent
            pExt = MyBase.Map.Extent
            'Resize it
            pExt.Resize(0.5)
            'Center it
            pExt.CenterAt(xCent, yCent)

            'Set the map extent
            MyBase.Map.Extent = pExt
            'cleanup
            pExt = Nothing
        End If


    End Sub

    Private Sub panExt_StatusChanged(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.WinForms.MapActionStatusChangedEventArgs) Handles Me.StatusChanged
        'Changes the pan button based on the status of the pan map action
        If e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Activated Then
            MyBase.Map.Cursor = m_NormCur
        ElseIf e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Deactivated Then


        End If
    End Sub
End Class
