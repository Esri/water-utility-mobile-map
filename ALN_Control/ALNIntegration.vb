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


Imports System
Imports System.Xml
Imports ESRI.ALNRemote
Imports ESRI.ArcGIS.Mobile
Imports Esri.ArcGISTemplates
Imports System.IO



Public Class ALNIntegration
    Private grfFileName As String
    Private m_ALN As ALNRemote
    Private m_ALNRoute As ALNRemoteRoute
    Private m_ALNStops As ALNRemoteStops
    Private m_Map As Esri.ArcGIS.Mobile.WinForms.Map
    Public Sub New(ByVal map As Esri.ArcGIS.Mobile.WinForms.Map)
        Try


            m_Map = map
            m_ALN = New ALNRemote()
            m_ALN.SetHostName("ArcGIS Mobile")

            ' after started, load grf file
            AddHandler m_ALN.OnStart, AddressOf m_ALN_OnStart

            m_ALNRoute = m_ALN.Route
            ' when arrived at one stop, back to AGM
            AddHandler m_ALNRoute.OnArriveAtStop, AddressOf m_ALNRoute_OnArriveAtStop

            ' when finish tracking, shutdown ALN?
            AddHandler m_ALNRoute.OnFinishTracking, AddressOf m_ALNRoute_OnFinishTracking


            'm_ALNRouterSettings = m_ALNRoute.RouterSettings;
            'm_ALNRouterSettings.SetTrackingMode(esriALNTrackingMode.esriALNTMSimulation);

            m_ALNStops = m_ALN.Stops
            'when grf load completed
            AddHandler m_ALNStops.OnLoadComplete, AddressOf m_ALNStops_OnLoadComplete

        Catch ex As Exception
           
            MsgBox(GlobalsFunctions.appConfig.NavigationOptions.UIComponents.ALNNotInstalled)

        End Try
    End Sub

    Public Sub Dispose()
        ' Stop listening to events

        'System.Windows.MessageBox.Show("UnInitialize");
        If (m_ALNRoute IsNot Nothing) Then

            RemoveHandler m_ALNRoute.OnArriveAtStop, AddressOf m_ALNRoute_OnArriveAtStop
            RemoveHandler m_ALNRoute.OnFinishTracking, AddressOf m_ALNRoute_OnFinishTracking

            System.Runtime.InteropServices.Marshal.ReleaseComObject(m_ALNRoute)
        End If
        If (m_ALNStops IsNot Nothing) Then

            RemoveHandler m_ALNStops.OnLoadComplete, AddressOf m_ALNStops_OnLoadComplete
            System.Runtime.InteropServices.Marshal.ReleaseComObject(m_ALNStops)


        End If

        If m_ALN IsNot Nothing Then

            ' remove event handlers
            RemoveHandler m_ALN.OnStart, AddressOf m_ALN_OnStart


            ' shutdown the ALN
            Try

                m_ALN.Shutdown()
                System.Runtime.InteropServices.Marshal.ReleaseComObject(m_ALN)

            Catch ex As Exception

                'System.Windows.MessageBox.Show(ex.ToString());
                ' if the ALN is not running, it throws an exception.
            End Try
        End If

        m_ALN = Nothing

        m_ALNRoute = Nothing
        m_ALNStops = Nothing
        m_Map = Nothing
    End Sub

    Public Sub RouteToGeo(ByVal destination As ESRI.ArcGIS.Mobile.Geometries.Coordinate, ByVal DestinationName As String, ByVal StartName As String, Optional ByVal startPoint As ESRI.ArcGIS.Mobile.Geometries.Coordinate = Nothing)


        If destination Is Nothing Then
            MsgBox(GlobalsFunctions.appConfig.NavigationOptions.UIComponents.DestinationNotFound)
            Return
        End If
        Dim startX As Double = 0
        Dim startY As Double = 0

        Try
            If startPoint Is Nothing Then

                If File.Exists(GlobalsFunctions.getEXEPath & "\NavigationSettings.xml") = True Then
                    Dim xdoc As New XmlDocument()
                    xdoc.Load(GlobalsFunctions.getEXEPath & "\NavigationSettings.xml")

                    Dim root As XmlElement = xdoc.DocumentElement
                    Dim xNode As XmlNode = root.SelectSingleNode("/NavigationSettings/StartPoint/X")
                    Dim yNode As XmlNode = root.SelectSingleNode("/NavigationSettings/StartPoint/Y")
                    Dim titleNode As XmlNode = root.SelectSingleNode("/NavigationSettings/StartPoint/Title")
                    startX = Convert.ToDouble(xNode.LastChild.InnerText)
                    startY = Convert.ToDouble(yNode.LastChild.InnerText)
                    If StartName = "" Then
                        StartName = titleNode.LastChild.InnerText
                    End If
                End If

            Else
                startX = startPoint.X
                startY = startPoint.Y
                If StartName = "" Then
                    StartName = "MMT"
                End If
            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

            Exit Sub
        End Try

        ' creat the grf file
        '''/////////////////////////////////////////////////////////////////////////

        grfFileName = GlobalsFunctions.getEXEPath & "\AlnStops.grf"

        Dim coordinate As ESRI.ArcGIS.Mobile.Geometries.Coordinate = m_Map.SpatialReference.ToWgs84(destination)

        Try
            Dim stopname As String = DestinationName
            ' remove < > which messes up the grf file.
            stopname = stopname.Replace("<", "")
            stopname = stopname.Replace(">", "")

            ' write grf file
            Dim grfFile As New FileStream(grfFileName, FileMode.Create, FileAccess.Write)
            Dim grfWriter As New StreamWriter(grfFile)
            grfWriter.WriteLine("<?xml version=""1.0"" encoding=""UTF-8""?> ")
            grfWriter.WriteLine("<GRFDOC version=""1.2"">")
            grfWriter.WriteLine("<ROUTE_INFO>")
            grfWriter.WriteLine("<STOPS>")
            grfWriter.WriteLine((("<STOP enabled=""True""> <LOCATION visible=""True"" closed=""False""> <POINT x=""" & startX.ToString & """ y=""") + startY.ToString & """ /> <TITLE>") + StartName & "</TITLE> </LOCATION> </STOP> ")
            grfWriter.WriteLine((("<STOP enabled=""True""> <LOCATION visible=""True"" closed=""False""> <POINT x=""" & coordinate.X.ToString() & """  y=""") + coordinate.Y.ToString() & """ /> <TITLE>") + stopname & "</TITLE> </LOCATION> </STOP>")
            grfWriter.WriteLine("</STOPS>")
            grfWriter.WriteLine("</ROUTE_INFO>")
            grfWriter.WriteLine("</GRFDOC>")
            grfWriter.Close()
            grfFile.Close()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

            Exit Sub
        End Try

        ' start ALN

        m_ALN.Start(Nothing)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    Private Sub m_ALNStops_OnLoadComplete()

        m_ALNRoute.Navigate(False)
        m_ALN.Show()
    End Sub

    Private Sub m_ALNRoute_OnFinishTracking()

        'm_ALN.Shutdown()

    End Sub

    Public Sub m_ALNRoute_OnArriveAtStop(ByVal StopIdx As UInteger, ByVal StopName As String)

        m_ALN.Hide()
    End Sub

    Public Sub m_ALN_OnStart()
        m_ALNStops.LoadFromFile(grfFileName)
    End Sub



End Class
