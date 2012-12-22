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


Imports ESRI.ArcGIS.Mobile.WinForms
Imports Esri.ArcGIS.Mobile.Gps
Imports System.Threading
Imports Esri.ArcGIS.Mobile.Geometries
Imports ESRI.ArcGIS.Mobile.WinForms.Map
Imports ESRI.ArcGIS.Mobile
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports Esri.ArcGISTemplates

Public Class Compass
    Private destY As Double
    Private destX As Double


    ' in WGS84
    Private currY As Double
    Private currX As Double
    Private gps As GpsConnection
    Private validDest As Boolean
    Private destDir As Double



    Public Sub New(gpsCon As GpsConnection)
        InitializeComponent()

        gps = gpsCon
        AddHandler gps.GpsChanged, AddressOf gps_GpsChanged

        AddHandler gps.GpsClosed, AddressOf gps_GpsClosed
        validDest = False


        ' if gps is not on, open it.
        If Not gps.IsOpen Then
            If txtCurrLoc.InvokeRequired Then
                Dim display As New Action(Of String)(AddressOf DisplayOutputCurrLoc)

                txtCurrLoc.Invoke(display, "GPS is not connected")
            Else
                DisplayOutputCurrLoc("GPS is not connected")

            End If
            

        End If
    End Sub
    Private Sub DisplayOutputCurrLoc(msg As String)
        txtCurrLoc.Text = msg

    End Sub
    Private Sub DisplayOutputLatLong(msg As String)
        Dim strings() As String = msg.Split(",")

        txtLongX.Text = strings(0)
        txtLongY.Text = strings(1)
        validateDestination()

    End Sub
    ' called by the nav map page, for setting the destination picked on the map
    Public Sub setDestination(x As Double, y As Double)
        destX = x
        destY = y

        Dim strings As String = String.Format("{0:0.00000}", x) & "," & String.Format("{0:0.00000}", y)

        If txtLongX.InvokeRequired Then
            Dim display As New Action(Of String)(AddressOf DisplayOutputLatLong)
            txtLongX.Invoke(display, strings)
        Else
            DisplayOutputLatLong(strings)

        End If


        

    End Sub

    Private Sub gps_GpsClosed(sender As Object, e As EventArgs)
        If txtCurrLoc.InvokeRequired Then
            Dim display As New Action(Of String)(AddressOf DisplayOutputCurrLoc)

            txtCurrLoc.Invoke(display, "GPS Disconnected.")
            'lbTurnInfo.Text = ""
        Else
            DisplayOutputCurrLoc("GPS Disconnected.")
            'lbTurnInfo.Text = ""
        End If

    End Sub

    Private Sub buttonSetDest_Click(sender As Object, e As EventArgs)
        validateDestination()

        If validDest = False Then
            Return
        End If
        MsgBox("set location on map")
        'mp.setDestCoorWGS84(New Coordinate(destX, destY))

    End Sub

    Private Sub validateDestination()
        Try
            destY = Double.Parse(txtLongY.Text.Trim())
            destX = Double.Parse(txtLongX.Text.Trim())
        Catch ex As Exception
            MessageBox.Show("invalid longitude")
            validDest = False
            Return
        End Try

        ' check X, longitude
        If (destX > 180) OrElse (destX < -180) Then
            MessageBox.Show("invalid longitude")
            validDest = False
            Return
        End If
        ' check Y, latitude
        If (destY > 90) OrElse (destY < -90) Then
            MessageBox.Show("Invalid latitude")
            validDest = False
            Return
        End If

        validDest = True

    End Sub

    Private Delegate Sub gps_GpsChangedDelegate(sender As Object, e As EventArgs)

    Private Sub gps_GpsChanged(sender As Object, e As EventArgs)
        If txtCurrLoc.InvokeRequired Then
            txtCurrLoc.Invoke(New gps_GpsChangedDelegate(AddressOf gps_GpsChanged), sender, e)
        Else
                Try
                    ' update current loc
                    Dim gpsmsg As String = String.Format("Lati:{0:0.000000} Long:{1:0.000000}" & vbCr & vbLf & "{2} {3:0.0}km/h {4:0.0}degrees Sat#:{5}", gps.Latitude, gps.Longitude, gps.FixStatus.ToString(), gps.Speed, gps.Course, _
                     gps.FixSatelliteCount)

                txtCurrLoc.Text = gpsmsg
                    currX = gps.Longitude
                    currY = gps.Latitude

                    ' if invalid current location
                    If gps.FixStatus.ToString().ToLower() = "invalid" Then
                    'lbTurnInfo.Text = "waiting for your current location"
                        Return
                    End If

                    ' if invalid dest, return 
                    If Not validDest Then
                    'lbTurnInfo.Text = "invalid destination !"
                        Return
                    End If
                    '''//////////////////////////////

                    'calculate bearing/distance
                Dim distMile As Double = GlobalsFunctions.Distance(currX, currY, destX, destY, GlobalsFunctions.LengthUnit.Mile)
                destDir = GlobalsFunctions.Bearing(currX, currY, destX, destY)
                    ' arrive time
                    If (gps.Speed.ToString().ToLower() <> "nan") OrElse (gps.Speed <> 0) Then
                        Dim t As Double = distMile * 1.609344 / gps.Speed
                        Dim hr As Integer = CInt(Math.Truncate(t))
                        Dim min As Integer = CInt(Math.Truncate((t - hr) * 60.0))
                    'lbTime.Text = String.Format("{0}hr,{1}min", hr, min)
                    Else
                    ' lbTime.Text = "waiting for gps speed"
                    End If

                    Dim turntxt As String = ""
                    If gps.Course.ToString().ToLower() <> "nan" Then
                        Dim turndir As Double = destDir - gps.Course
                        If turndir > 180 Then
                            turndir = turndir - 360
                        End If
                        If turndir < -180 Then
                            turndir = turndir + 360
                        End If

                        If turndir > 0 Then
                            turntxt = String.Format("Turn {0:0.0} degrees to your right.", turndir)
                        ElseIf turndir < 0 Then
                            turntxt = String.Format("Turn {0:0.0} degrees to your left.", Math.Abs(turndir))
                        Else
                            turntxt = "Right Direction."
                        End If
                    Else
                        turntxt = "waiting for GPS bearing..."
                    End If

                ' lbdist.Text = String.Format("{0:0.000}", distMile)
                'lbBearing.Text = String.Format("{0:0.0}", destDir)
                'lbTurnInfo.Text = turntxt

                    ' redraw the compass
                    'drawcompass
                    PictureBox1.Refresh()
                Catch ex As Exception
                    MessageBox.Show(ex.ToString())
                End Try

        End If
      
    End Sub

    Private Sub pictureBox1_Paint(ByVal sender As Object, ByVal e As PaintEventArgs)
        Dim w As Integer = PictureBox1.Size.Width
        Dim h As Integer = PictureBox1.Size.Height
        Dim cx As Integer = (w / 2)
        Dim cy As Integer = (h / 2)
        Dim rad As Integer = (cx - 10)
        Dim apen As Pen = New Pen(Color.Black)
        apen.Width = 2
        If (gps.FixStatus.ToString.ToLower = "invalid") Then
            Return
        End If
        If (gps.Course.ToString.ToLower <> "nan") Then
            ' current GPS bearing
            Dim gpsx As Integer = CType((cx _
                        + ((rad - 20) _
                        * Math.Sin((gps.Course * (3.1416 / 180))))), Integer)
            Dim gpsy As Integer = CType((cy _
                        - ((rad - 20) _
                        * Math.Cos((gps.Course * (3.1416 / 180))))), Integer)
            'arrow
            Dim t1x As Integer = CType((cx _
                        + ((rad - 30) _
                        * Math.Sin(((gps.Course + 10) * (3.1416 / 180))))), Integer)
            Dim t1y As Integer = CType((cy _
                        - ((rad - 30) _
                        * Math.Cos(((gps.Course + 10) * (3.1416 / 180))))), Integer)
            Dim t2x As Integer = CType((cx _
                        + ((rad - 30) _
                        * Math.Sin(((gps.Course - 10) * (3.1416 / 180))))), Integer)
            Dim t2y As Integer = CType((cy _
                        - ((rad - 30) _
                        * Math.Cos(((gps.Course - 10) * (3.1416 / 180))))), Integer)
            Dim gpsPen As Pen = New Pen(Color.Black)
            gpsPen.Width = 2
            e.Graphics.DrawLine(gpsPen, cx, cy, gpsx, gpsy)
            e.Graphics.DrawLine(gpsPen, t1x, t1y, gpsx, gpsy)
            e.Graphics.DrawLine(gpsPen, t2x, t2y, gpsx, gpsy)
        End If
        If validDest Then
            ' destination bearing
            Dim desx As Integer = CType((cx _
                        + (rad * Math.Sin((destDir * (3.1416 / 180))))), Integer)
            Dim desy As Integer = CType((cy _
                        - (rad * Math.Cos((destDir * (3.1416 / 180))))), Integer)
            Dim redbr As SolidBrush = New SolidBrush(Color.Red)
            e.Graphics.FillEllipse(redbr, (desx - 8), (desy - 8), 16, 16)
        End If
    End Sub
End Class
