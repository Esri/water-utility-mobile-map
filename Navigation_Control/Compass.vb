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

    Private validDest As Boolean
    Private destDir As Double

    Private WithEvents m_Map As Esri.ArcGIS.Mobile.WinForms.Map


#Region "Properties"
    Public WriteOnly Property map() As Esri.ArcGIS.Mobile.WinForms.Map

        Set(ByVal value As Esri.ArcGIS.Mobile.WinForms.Map)
            m_Map = value
        End Set
    End Property

#End Region



    Public Sub New()
        InitializeComponent()
     


        If System.IO.File.Exists(Application.StartupPath & "\AppImages\" & "compass.png") Then
            Try


                picGPS.BackgroundImage = New Bitmap(Application.StartupPath & "\AppImages\" & "compass.png")
            Catch ex As Exception

            End Try


        End If

        If GlobalsFunctions.m_GPS Is Nothing Then Return
        If GlobalsFunctions.m_GPS.GpsConnection Is Nothing Then Return


        AddHandler GlobalsFunctions.m_GPS.GpsConnection.GpsChanged, AddressOf gps_GpsChanged

        AddHandler GlobalsFunctions.m_GPS.GpsConnection.GpsClosed, AddressOf gps_GpsClosed
        validDest = False


        ' if gps is not on, open it.
        If Not GlobalsFunctions.m_GPS.GpsConnection.IsOpen Then
            If txtCurX.InvokeRequired Then
                Dim display As New Action(Of String)(AddressOf DisplayOutputCurrLoc)

                txtCurX.Invoke(display, "N/A")
            Else
                DisplayOutputCurrLoc("N/A")

            End If


        End If


    End Sub
    Private pPicBox As MyPicBox
    Public Sub addWayPointImage()
        pPicBox = New MyPicBox
        pPicBox.BorderStyle = Windows.Forms.BorderStyle.None
        pPicBox.Width = 24
        pPicBox.Height = 24
        pPicBox.BackColor = Color.Transparent
        pPicBox.BackgroundImage = Nothing

        pPicBox.Image = Nothing

        pPicBox.Visible = False
        m_map.Controls.Add(pPicBox)
        m_map.Controls.SetChildIndex(pPicBox, 0)

    End Sub
    Private Sub DisplayOutputCurrLoc(msg As String)
        txtCurX.Text = msg

    End Sub
    Private Sub DisplayOutputLatLong(msg As String)
        Dim strings() As String = msg.Split(",")
        If strings.Length = 2 Then
            txtLongX.Text = strings(0)
            txtLongY.Text = strings(1)
        Else
            txtLongX.Text = "N/A"
            txtLongY.Text = "N/A"
        End If
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
        If txtCurX.InvokeRequired Then
            Dim display As New Action(Of String)(AddressOf DisplayOutputCurrLoc)

            txtCurX.Invoke(display, "")
            'lbTurnInfo.Text = ""
        Else
            DisplayOutputCurrLoc("")
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
            ' MessageBox.Show("invalid longitude")
            validDest = False
            txtCurX.Text = "N/A"
            txtCurY.Text = "N/A"

            lblArriveIn.Text = "N/A"
            lblDistance.Text = "N/A"
            lblBearing.Text = "N/A"
            lblTurnIn.Text = "N/A"
            pPicBox.Visible = False
            Return
        End Try

        ' check X, longitude
        If (destX > 180) OrElse (destX < -180) Then
            ' MessageBox.Show("invalid longitude")
            validDest = False
            txtCurX.Text = "N/A"
            txtCurY.Text = "N/A"

            lblArriveIn.Text = "N/A"
            lblDistance.Text = "N/A"
            lblBearing.Text = "N/A"
            lblTurnIn.Text = "N/A"
            pPicBox.Visible = False
            Return
        End If
        ' check Y, latitude
        If (destY > 90) OrElse (destY < -90) Then
            ' MessageBox.Show("Invalid latitude")
            txtCurX.Text = "N/A"
            txtCurY.Text = "N/A"

            lblArriveIn.Text = "N/A"
            lblDistance.Text = "N/A"
            lblBearing.Text = "N/A"
            lblTurnIn.Text = "N/A"
            pPicBox.Visible = False

            validDest = False
            Return
        End If

        validDest = True

    End Sub

    Private Delegate Sub gps_GpsChangedDelegate(sender As Object, e As EventArgs)

    Private Sub gps_GpsChanged(sender As Object, e As EventArgs)
        If txtCurX.InvokeRequired Then
            txtCurX.Invoke(New gps_GpsChangedDelegate(AddressOf gps_GpsChanged), sender, e)
        Else
            Try
                ' update current loc


                txtCurY.Text = String.Format("{0:0.000000}", GlobalsFunctions.m_GPS.GpsConnection.Latitude)
                txtCurX.Text = String.Format("{0:0.000000}", GlobalsFunctions.m_GPS.GpsConnection.Longitude)
                lblFix.text = GlobalsFunctions.m_GPS.GpsConnection.FixStatus.ToString()
                lblSpeed.text = String.Format("{0:0.0} km/h", GlobalsFunctions.m_GPS.GpsConnection.Speed)
                lblCourse.text = String.Format("{0:0.0} degrees ", GlobalsFunctions.m_GPS.GpsConnection.Course)
                lblNumSat.text = GlobalsFunctions.m_GPS.GpsConnection.FixSatelliteCount


                currX = GlobalsFunctions.m_GPS.GpsConnection.Longitude
                currY = GlobalsFunctions.m_GPS.GpsConnection.Latitude

                ' if invalid current location
                If GlobalsFunctions.m_GPS.GpsConnection.FixStatus.ToString().ToLower() = "invalid" Then
                    'lbTurnInfo.Text = "waiting for your current location"
                    Return
                End If

                ' if invalid dest, return 
                If validDest Then
                    'lbTurnInfo.Text = "invalid destination !"
                    ' Return

                    '''//////////////////////////////

                    'calculate bearing/distance
                    Dim distMile As Double = GlobalsFunctions.Distance(currX, currY, destX, destY, GlobalsFunctions.LengthUnit.Mile)
                    destDir = GlobalsFunctions.Bearing(currX, currY, destX, destY)
                    ' arrive time
                    If (GlobalsFunctions.m_GPS.GpsConnection.Speed.ToString().ToLower() <> "nan") OrElse (GlobalsFunctions.m_GPS.GpsConnection.Speed <> 0) Then
                        Try

                        
                        Dim t As Double = distMile * 1.609344 / GlobalsFunctions.m_GPS.GpsConnection.Speed
                        Dim hr As Integer = CInt(Math.Truncate(t))
                        Dim min As Integer = CInt(Math.Truncate((t - hr) * 60.0))
                            lblArriveIn.Text = String.Format("{0}hr,{1}min", hr, min)
                        Catch ex As Exception
                            lblArriveIn.Text = "waiting for gps speed"
                        End Try
                    Else
                        lblArriveIn.Text = "waiting for gps speed"
                    End If

                    Dim turntxt As String = ""
                    If GlobalsFunctions.m_GPS.GpsConnection.Course.ToString().ToLower() <> "nan" Then
                        Try

                        
                        Dim turndir As Double = destDir - GlobalsFunctions.m_GPS.GpsConnection.Course
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
                        Catch ex As Exception
                            turntxt = "waiting for GPS bearing..."
                        End Try
                    Else
                        turntxt = "waiting for GPS bearing..."
                    End If
                    If (distMile < 0.5) Then

                        lblDistance.Text = String.Format("{0:0.000} feet", distMile * 5280)
                    Else
                        lblDistance.Text = String.Format("{0:0.000} miles", distMile)
                    End If

                    lblBearing.Text = String.Format("{0:0.0}", destDir)
                    lblTurnIn.Text = turntxt

                End If

                ' redraw the compass
                'drawcompass
                picGPS.Refresh()

                'If validDest Then
                'Dim pCoord As Coordinate = m_map.SpatialReference.FromWgs84(destX, destY)

                'Dim pPnt As System.Drawing.Point = m_map.ToClient(pCoord)
                'If pPnt.X < 10 Then
                '    pPnt.X = 10

                'End If
                'If pPnt.Y < 10 Then
                '    pPnt.Y = 10

                'End If
                'm_map.navCoord = pPnt

                'End If

            Catch ex As Exception
                '  MessageBox.Show(ex.ToString())
            End Try

        End If

    End Sub

    Private Sub picGPS_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles picGPS.Paint
        Dim wPic As Integer = picGPS.Height 'picGPS.BackgroundImage.Size.Width
        Dim hPic As Integer = picGPS.Height 'picGPS.BackgroundImage.Size.Height

        Dim w As Integer = picGPS.Width
        Dim h As Integer = picGPS.Height

        Dim cx As Integer = (w / 2)
        Dim cy As Integer = (h / 2)

        Dim cxPic As Integer = (wPic / 2)
        Dim cyPic As Integer = (hPic / 2)
        Dim rad As Integer = (cy + 10)
        Dim radTo As Integer = (cy - 10)
        Dim apen As Pen = New Pen(Color.Black)
        apen.Width = 2
        If (GlobalsFunctions.m_GPS.GpsConnection.FixStatus.ToString.ToLower = "invalid") Then
            Return
        End If
        If (GlobalsFunctions.m_GPS.GpsConnection.Course.ToString.ToLower <> "nan") Then
            ' current GPS bearing
            Dim gpsx As Integer = CType((cx _
                        + ((rad - 20) _
                        * Math.Sin((GlobalsFunctions.m_GPS.GpsConnection.Course * (3.1416 / 180))))), Integer)
            Dim gpsy As Integer = CType((cy _
                        - ((rad - 20) _
                        * Math.Cos((GlobalsFunctions.m_GPS.GpsConnection.Course * (3.1416 / 180))))), Integer)
            'arrow
            Dim t1x As Integer = CType((cx _
                        + ((rad - 30) _
                        * Math.Sin(((GlobalsFunctions.m_GPS.GpsConnection.Course + 10) * (3.1416 / 180))))), Integer)
            Dim t1y As Integer = CType((cy _
                        - ((rad - 30) _
                        * Math.Cos(((GlobalsFunctions.m_GPS.GpsConnection.Course + 10) * (3.1416 / 180))))), Integer)
            Dim t2x As Integer = CType((cx _
                        + ((rad - 30) _
                        * Math.Sin(((GlobalsFunctions.m_GPS.GpsConnection.Course - 10) * (3.1416 / 180))))), Integer)
            Dim t2y As Integer = CType((cy _
                        - ((rad - 30) _
                        * Math.Cos(((GlobalsFunctions.m_GPS.GpsConnection.Course - 10) * (3.1416 / 180))))), Integer)
            Dim gpsPen As Pen = New Pen(Color.Black)
            gpsPen.Width = 5
            e.Graphics.DrawLine(gpsPen, cx + 2, cy - 2, gpsx, gpsy)
            e.Graphics.DrawLine(gpsPen, t1x, t1y, gpsx, gpsy)
            e.Graphics.DrawLine(gpsPen, t2x, t2y, gpsx, gpsy)
        End If
        If validDest Then
            ' destination bearing
            Dim desx As Integer = CType((cx _
                        + (radTo * Math.Sin((destDir * (3.1416 / 180))))), Integer)
            Dim desy As Integer = CType((cy _
                        - (radTo * Math.Cos((destDir * (3.1416 / 180))))), Integer)
            Dim redbr As SolidBrush = New SolidBrush(Color.Red)
            e.Graphics.FillEllipse(redbr, (desx - 8), (desy - 8), 16, 16)
        End If
    End Sub



    Private Sub SplitContainer1_Panel1_Resize(sender As System.Object, e As System.EventArgs) Handles SplitContainer1.Panel1.Resize
        If picGPS Is Nothing Then Return
        If picGPS.Parent Is Nothing Then Return

        picGPS.Left = (picGPS.Parent.Width / 2) - (picGPS.Width / 2)

    End Sub

    Private Sub Map1_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles m_Map.Paint
        If m_Map Is Nothing Then Return
    
        If validDest Then
            Dim pCoord As Coordinate = m_Map.SpatialReference.FromWgs84(destX, destY)

            Dim pPnt As System.Drawing.Point = m_Map.ToClient(pCoord)
            pPnt.X = pPnt.X - (pPicBox.Width / 2)
            pPnt.Y = pPnt.Y - (pPicBox.Height / 2)
            If pPnt.X < (pPicBox.Width / 2) Then
                pPnt.X = 0 '(pPicBox.Width / 2)

            End If
            If pPnt.Y < (pPicBox.Height / 2) Then
                pPnt.Y = 0 ' (pPicBox.Height / 2)

            End If



            If pPnt.X > m_Map.Width - (pPicBox.Width) Then
                pPnt.X = m_Map.Width - (pPicBox.Width)

            End If
            If pPnt.Y > m_Map.Height - (pPicBox.Height) Then
                pPnt.Y = m_Map.Height - (pPicBox.Height)

            End If
            pPicBox.Visible = True

            pPicBox.Location = pPnt


            ' e.Graphics.DrawImage(My.Resources.RedCircle, pPnt)

            ' e.Display.DrawGeometry(Pens.Black, New SolidBrush(Color.Aquamarine), 20, New Esri.ArcGIS.Mobile.Geometries.Point(pCoord))


        End If

    End Sub

    'Private Sub m_Map_Paint(ByVal sender As Object, ByVal e As ESRI.ArcGIS.Mobile.WinForms.MapPaintEventArgs) Handles m_map.Paint
    '    Try
    '        If m_map Is Nothing Then Return
    '        If e.MapSurface Is Nothing Then Return

    '        If validDest Then
    '            Dim pCoord As Coordinate = m_map.SpatialReference.FromWgs84(destX, destY)

    '            Dim pPnt As System.Drawing.Point = m_map.ToClient(pCoord)
    '            pPnt.X = pPnt.X - (pPicBox.Width / 2)
    '            pPnt.Y = pPnt.Y - (pPicBox.Height / 2)
    '            If pPnt.X < (pPicBox.Width / 2) Then
    '                pPnt.X = 0 '(pPicBox.Width / 2)

    '            End If
    '            If pPnt.Y < (pPicBox.Height / 2) Then
    '                pPnt.Y = 0 ' (pPicBox.Height / 2)

    '            End If



    '            If pPnt.X > m_map.Width - (pPicBox.Width) Then
    '                pPnt.X = m_map.Width - (pPicBox.Width)

    '            End If
    '            If pPnt.Y > m_map.Height - (pPicBox.Height) Then
    '                pPnt.Y = m_map.Height - (pPicBox.Height)

    '            End If
    '            pPicBox.Visible = True

    '            pPicBox.Location = pPnt


    '            ' e.Graphics.DrawImage(My.Resources.RedCircle, pPnt)

    '            ' e.Display.DrawGeometry(Pens.Black, New SolidBrush(Color.Aquamarine), 20, New Esri.ArcGIS.Mobile.Geometries.Point(pCoord))



    '        End If




    '    Catch ex As Exception
    '        Dim st As New StackTrace
    '        MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
    '        st = Nothing

    '    End Try

    'End Sub


    Private Sub btnClear_Click(sender As System.Object, e As System.EventArgs) Handles btnClear.Click
        txtLongY.Text = "N/A"
        txtLongX.Text = "N/A"
        validateDestination()
    End Sub

    Private Sub Compass_Resize(sender As Object, e As System.EventArgs) Handles Me.Resize

        Try

            SplitContainer1.SplitterDistance = SplitContainer1.Parent.Width


        Catch ex As Exception

        End Try
        'SplitContainer1.Panel1.Height = SplitContainer1.Parent.Height - 330



        'lblTurnIn.Top = lblArriveIn.Top + lblArriveIn.Height + 50
    End Sub

End Class
