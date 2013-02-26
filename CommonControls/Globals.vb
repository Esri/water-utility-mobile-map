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
Imports System.Windows.Forms
Imports System.IO
Imports System.Xml
Imports System.Collections.Specialized
Imports System.Collections
Imports System.Text
Imports System.Xml.Serialization
Imports Esri.ArcGIS.Mobile
Imports Esri.ArcGIS.Mobile.Geometries
Imports Esri.ArcGIS.Mobile.SpatialReferences
Imports Esri.ArcGIS.Mobile.FeatureCaching
Imports MobileMapConfig
Imports Esri.ArcGIS.Mobile.CatalogServices
Imports System.Drawing
Imports System.Web.Http
Imports System.Web.Script.Serialization

Imports System.Net

Imports System.Web
Imports System.Security.Cryptography.X509Certificates
Imports System.Net.Security


Public Class GlobalsFunctions

    Public Shared WithEvents m_GPS As Gps.GpsDisplay

    Public Shared appConfig As MobileConfigClass.MobileConfigMobileMapConfig = Nothing
    Public Shared conParent As MobileConfigClass.MobileConfig = Nothing


    Public Shared a, b, f As Double


    Public Shared Function GetFileContents(ByVal FullPath As String, _
       Optional ByRef ErrInfo As String = "") As String

        Dim strContents As String
        Dim objReader As StreamReader
        Try

            objReader = New StreamReader(FullPath)
            strContents = objReader.ReadToEnd()
            objReader.Close()
            Return strContents
        Catch Ex As Exception
            ErrInfo = Ex.Message
        End Try
    End Function

    Public Shared Function SaveTextToFile(ByVal strData As String, _
     ByVal FullPath As String, _
       Optional ByVal ErrInfo As String = "") As Boolean

        Dim Contents As String
        Dim bAns As Boolean = False
        Dim objReader As StreamWriter
        Try


            objReader = New StreamWriter(FullPath)
            objReader.Write(strData)
            objReader.Close()
            bAns = True
        Catch Ex As Exception
            ErrInfo = Ex.Message

        End Try
        Return bAns
    End Function

    Public Shared Function getClosest(pFdt As FeatureDataTable, point As Esri.ArcGIS.Mobile.Geometries.Point) As FeatureDataRow
        Dim ClosestDist As Double = 99999

        Dim dist As Double = -1
        Dim pClosestRow As FeatureDataRow
        For Each pFDR As FeatureDataRow In pFdt.Rows
            dist = pFDR.Geometry.Distance(point)
            If dist < ClosestDist Then
                ClosestDist = dist
                pClosestRow = pFDR
            End If
        Next
        Return pClosestRow

    End Function
    Public Shared Function spatialQFeature(ByVal layername As String, ByVal Location As Esri.ArcGIS.Mobile.Geometries.Geometry, map As Esri.ArcGIS.Mobile.WinForms.Map, searchBuffer As Integer) As FeatureDataTable

        Dim pFS As FeatureSourceWithDef
        Dim pEnv As Envelope
        pFS = GlobalsFunctions.GetFeatureSource(layername, map) '(m_Map.MapLayers(layername), MobileCacheMapLayer)
        If pFS.FeatureSource IsNot Nothing Then


            'If a point is passed in, expand the envelope 
            If TypeOf Location Is Esri.ArcGIS.Mobile.Geometries.Point Then
                Dim pPnt As Esri.ArcGIS.Mobile.Geometries.Point = CType(Location, Esri.ArcGIS.Mobile.Geometries.Point)

                Dim intBufferValueforPoint As Double
                intBufferValueforPoint = GlobalsFunctions.bufferToMap(map, searchBuffer * 4) 'maptobuffer()

                pEnv = New Envelope(0, 0, intBufferValueforPoint, intBufferValueforPoint)
                pEnv.CenterAt(pPnt.Coordinate)

            Else
                pEnv = Location.Extent
            End If


            'Set up query filter used to id features
            Dim pQf As QueryFilter = New QueryFilter(pEnv, Geometries.GeometricRelationshipType.Intersect)
            'Build an array to limit returned fields
            Dim pStr(pFS.FeatureSource.Columns.Count) As String
            'Add all fields for the result
            For i As Integer = 0 To pFS.FeatureSource.Columns.Count - 1
                pStr(i) = pFS.FeatureSource.Columns(i).ColumnName
            Next
            If pFS.FeatureSource.GetFeatureCount(pQf) > 0 Then


                Try
                    'Return the resulting rows

                    Return pFS.FeatureSource.GetDataTable(pQf, pStr)
                Catch ex As Exception

                End Try
            End If
        End If
        Return Nothing

    End Function


    Public Shared Function GPSFixToText(fixStat As String) As String
        If IsNumeric(fixStat) = False Then Return fixStat

        Select Case CInt(fixStat)
            Case 0
                Return "No Fix"

            Case 1
                Return "GPS Fix"

            Case 2

                Return "DGps Fix"
            Case 3

                Return "PPS Fix"
            Case 4

                Return "Real Time Kinematic"
            Case 5

                Return "Float RTK"
            Case 6
                Return "Estimated"

            Case 7
                Return "Manual Input Mode"

            Case 8
                Return "Simulation mode"
            Case Else
                Return ""
        End Select
    End Function
    Public Shared Function GPSTextToFix(fixText As String) As Esri.ArcGIS.Mobile.Gps.GpsFixStatus

        Select Case fixText
            Case "No Fix"
                Return Esri.ArcGIS.Mobile.Gps.GpsFixStatus.Invalid
            Case "0"
                Return Esri.ArcGIS.Mobile.Gps.GpsFixStatus.Invalid
            Case "GPS Fix"
                Return Gps.GpsFixStatus.GpsFix
            Case "1"
                Return Gps.GpsFixStatus.GpsFix
            Case "DGps Fix"
                Return Gps.GpsFixStatus.DGpsFix
            Case "2"
                Return Gps.GpsFixStatus.DGpsFix
            Case "PPS Fix"
                Return Gps.GpsFixStatus.PpsFix
            Case "3"
                Return Gps.GpsFixStatus.PpsFix
            Case "Real Time Kinematic"
                Return Gps.GpsFixStatus.RealTimeKinematic
            Case "4"
                Return Gps.GpsFixStatus.RealTimeKinematic
            Case "Float RTK"
                Return Gps.GpsFixStatus.FloatRealTimeKinematic
            Case "5"
                Return Gps.GpsFixStatus.FloatRealTimeKinematic
            Case "Estimated"
                Return Gps.GpsFixStatus.Estimated
            Case "6"
                Return Gps.GpsFixStatus.Estimated

            Case "Manual Input Mode"
                Return Gps.GpsFixStatus.ManualInputMode
            Case "7"
                Return Gps.GpsFixStatus.ManualInputMode
            Case "Simulation mode"
                Return Gps.GpsFixStatus.SimulationMode
            Case "8"
                Return Gps.GpsFixStatus.SimulationMode
            Case Else
                Return -1
        End Select
    End Function
    Public Shared Function GetAddressOnline(streetAddress As String, url As String) As gcResponse

        streetAddress = Trim(streetAddress)

        Dim request As HttpWebRequest
        Dim response As HttpWebResponse = Nothing
        Dim reader As StreamReader
        Dim address As Uri
        Dim f As String = "JSON"

        Dim outFields As String

        Dim query As String
        Dim data As StringBuilder
        Dim byteData() As Byte
        Dim postStream As Stream = Nothing

        address = New Uri(url)

        ' Create the web request  
        request = DirectCast(WebRequest.Create(address), HttpWebRequest)

        ' Set type to POST  
        request.Method = "POST"
        request.ContentType = "application/x-www-form-urlencoded"

        ' Create the data we want to send  



        data = New StringBuilder()

        'data.Append("Single+Line+Input=" + WebUtility.HtmlEncode(streetAddress))

        'data.Append("&outSR=" + WebUtility.HtmlEncode("4326"))
        'data.Append("&f=" + WebUtility.HtmlEncode(f))
        'data.Append("&outFields=" + WebUtility.HtmlEncode(outFields))

        data.Append("Single+Line+Input=" + System.Web.HttpUtility.HtmlEncode(streetAddress))

        data.Append("&outSR=" + System.Web.HttpUtility.HtmlEncode("4326"))
        data.Append("&f=" + System.Web.HttpUtility.HtmlEncode(f))
        data.Append("&outFields=" + System.Web.HttpUtility.HtmlEncode(outFields))

        ' Create a byte array of the data we want to send  
        byteData = UTF8Encoding.UTF8.GetBytes(data.ToString())

        ' Set the content length in the request headers  
        request.ContentLength = byteData.Length

        ' Write data  
        Try
            postStream = request.GetRequestStream()
            postStream.Write(byteData, 0, byteData.Length)
        Catch e As WebException

            Return Nothing

        Finally
            If Not postStream Is Nothing Then postStream.Close()
        End Try

        Try
            ' Get response  
            response = DirectCast(request.GetResponse(), HttpWebResponse)

            ' Get the response stream into a reader  
            reader = New StreamReader(response.GetResponseStream())

            ' Console application output  
            'Console.WriteLine(reader.ReadToEnd())
            Dim pJSSe As JavaScriptSerializer = New JavaScriptSerializer
            Dim obj As gcResponse = pJSSe.Deserialize(Of gcResponse)(reader.ReadToEnd())
            Return obj





        Finally
            If Not response Is Nothing Then response.Close()
        End Try
    End Function
    Public Shared Function RotateImage(ByVal image As Image, ByVal angle As Single) As Bitmap
        If image Is Nothing Then
            Throw New ArgumentNullException("image")
        End If

        Const pi2 As Double = Math.PI / 2.0

        ' Why can't C# allow these to be const, or at least readonly
        ' *sigh*  I'm starting to talk like Christian Graus :omg:
        Dim oldWidth As Double = CDbl(image.Width)
        Dim oldHeight As Double = CDbl(image.Height)

        ' Convert degrees to radians
        Dim theta As Double = CDbl(angle) * Math.PI / 180.0
        Dim locked_theta As Double = theta

        ' Ensure theta is now [0, 2pi)
        While locked_theta < 0.0
            locked_theta += 2 * Math.PI
        End While

        Dim newWidth As Double, newHeight As Double
        Dim nWidth As Integer, nHeight As Integer
        ' The newWidth/newHeight expressed as ints
        '#Region "Explaination of the calculations"
        '
        '			 * The trig involved in calculating the new width and height
        '			 * is fairly simple; the hard part was remembering that when 
        '			 * PI/2 <= theta <= PI and 3PI/2 <= theta < 2PI the width and 
        '			 * height are switched.
        '			 * 
        '			 * When you rotate a rectangle, r, the bounding box surrounding r
        '			 * contains for right-triangles of empty space.  Each of the 
        '			 * triangles hypotenuse's are a known length, either the width or
        '			 * the height of r.  Because we know the length of the hypotenuse
        '			 * and we have a known angle of rotation, we can use the trig
        '			 * function identities to find the length of the other two sides.
        '			 * 
        '			 * sine = opposite/hypotenuse
        '			 * cosine = adjacent/hypotenuse
        '			 * 
        '			 * solving for the unknown we get
        '			 * 
        '			 * opposite = sine * hypotenuse
        '			 * adjacent = cosine * hypotenuse
        '			 * 
        '			 * Another interesting point about these triangles is that there
        '			 * are only two different triangles. The proof for which is easy
        '			 * to see, but its been too long since I've written a proof that
        '			 * I can't explain it well enough to want to publish it.  
        '			 * 
        '			 * Just trust me when I say the triangles formed by the lengths 
        '			 * width are always the same (for a given theta) and the same 
        '			 * goes for the height of r.
        '			 * 
        '			 * Rather than associate the opposite/adjacent sides with the
        '			 * width and height of the original bitmap, I'll associate them
        '			 * based on their position.
        '			 * 
        '			 * adjacent/oppositeTop will refer to the triangles making up the 
        '			 * upper right and lower left corners
        '			 * 
        '			 * adjacent/oppositeBottom will refer to the triangles making up 
        '			 * the upper left and lower right corners
        '			 * 
        '			 * The names are based on the right side corners, because thats 
        '			 * where I did my work on paper (the right side).
        '			 * 
        '			 * Now if you draw this out, you will see that the width of the 
        '			 * bounding box is calculated by adding together adjacentTop and 
        '			 * oppositeBottom while the height is calculate by adding 
        '			 * together adjacentBottom and oppositeTop.
        '			 

        '#End Region

        Dim adjacentTop As Double, oppositeTop As Double
        Dim adjacentBottom As Double, oppositeBottom As Double

        ' We need to calculate the sides of the triangles based
        ' on how much rotation is being done to the bitmap.
        '   Refer to the first paragraph in the explaination above for 
        '   reasons why.
        If (locked_theta >= 0.0 AndAlso locked_theta < pi2) OrElse (locked_theta >= Math.PI AndAlso locked_theta < (Math.PI + pi2)) Then
            adjacentTop = Math.Abs(Math.Cos(locked_theta)) * oldWidth
            oppositeTop = Math.Abs(Math.Sin(locked_theta)) * oldWidth

            adjacentBottom = Math.Abs(Math.Cos(locked_theta)) * oldHeight
            oppositeBottom = Math.Abs(Math.Sin(locked_theta)) * oldHeight
        Else
            adjacentTop = Math.Abs(Math.Sin(locked_theta)) * oldHeight
            oppositeTop = Math.Abs(Math.Cos(locked_theta)) * oldHeight

            adjacentBottom = Math.Abs(Math.Sin(locked_theta)) * oldWidth
            oppositeBottom = Math.Abs(Math.Cos(locked_theta)) * oldWidth
        End If

        newWidth = adjacentTop + oppositeBottom
        newHeight = adjacentBottom + oppositeTop

        nWidth = CInt(Math.Ceiling(newWidth))
        nHeight = CInt(Math.Ceiling(newHeight))

        Dim rotatedBmp As New Bitmap(nWidth, nHeight)

        Using g As Graphics = Graphics.FromImage(rotatedBmp)
            ' This array will be used to pass in the three points that 
            ' make up the rotated image
            Dim points As System.Drawing.Point()

            '
            '				 * The values of opposite/adjacentTop/Bottom are referring to 
            '				 * fixed locations instead of in relation to the
            '				 * rotating image so I need to change which values are used
            '				 * based on the how much the image is rotating.
            '				 * 
            '				 * For each point, one of the coordinates will always be 0, 
            '				 * nWidth, or nHeight.  This because the Bitmap we are drawing on
            '				 * is the bounding box for the rotated bitmap.  If both of the 
            '				 * corrdinates for any of the given points wasn't in the set above
            '				 * then the bitmap we are drawing on WOULDN'T be the bounding box
            '				 * as required.
            '				 

            If locked_theta >= 0.0 AndAlso locked_theta < pi2 Then

                points = New System.Drawing.Point() {New System.Drawing.Point(CInt(Math.Truncate(oppositeBottom)), 0), New System.Drawing.Point(nWidth, CInt(Math.Truncate(oppositeTop))), New System.Drawing.Point(0, CInt(Math.Truncate(adjacentBottom)))}
            ElseIf locked_theta >= pi2 AndAlso locked_theta < Math.PI Then
                points = New System.Drawing.Point() {New System.Drawing.Point(nWidth, CInt(Math.Truncate(oppositeTop))), New System.Drawing.Point(CInt(Math.Truncate(adjacentTop)), nHeight), New System.Drawing.Point(CInt(Math.Truncate(oppositeBottom)), 0)}
            ElseIf locked_theta >= Math.PI AndAlso locked_theta < (Math.PI + pi2) Then
                points = New System.Drawing.Point() {New System.Drawing.Point(CInt(Math.Truncate(adjacentTop)), nHeight), New System.Drawing.Point(0, CInt(Math.Truncate(adjacentBottom))), New System.Drawing.Point(nWidth, CInt(Math.Truncate(oppositeTop)))}
            Else
                points = New System.Drawing.Point() {New System.Drawing.Point(0, CInt(Math.Truncate(adjacentBottom))), New System.Drawing.Point(CInt(Math.Truncate(oppositeBottom)), 0), New System.Drawing.Point(CInt(Math.Truncate(adjacentTop)), nHeight)}
            End If

            g.DrawImage(image, points)
        End Using

        Return rotatedBmp
    End Function

    Public Shared Function distanceWGS(ByVal lat1 As Double, ByVal lon1 As Double, ByVal lat2 As Double, ByVal lon2 As Double, ByVal unit As Char) As Double

        Dim theta As Double = lon1 - lon2
        Dim dist As Double = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta))
        dist = Math.Acos(dist)
        dist = rad2deg(dist)
        dist = dist * 60 * 1.1515
        If unit = "K" Then
            dist = dist * 1.609344
        ElseIf unit = "N" Then
            dist = dist * 0.8684
        End If
        Return dist
    End Function

    Public Shared Function deg2rad(ByVal deg As Double) As Double
        Return (deg * Math.PI / 180.0)
    End Function

    Public Shared Function rad2deg(ByVal rad As Double) As Double
        Return rad / Math.PI * 180.0
    End Function

    Public Shared Sub Geodesic(ByVal semiMajor As Double, ByVal semiMinor As Double)
        '        new Geodesic(6378137.0, 6356752.31424518);

        a = semiMajor
        b = semiMinor
        f = (a - b) / a
    End Sub

    Public Shared Function GeodesicLength(ByVal coord1 As Coordinate, ByVal coord2 As Coordinate) As Double

        Dim num2 As Double = 0.0

        Dim lon As Double = coord1.X * 0.0174532925199433
        Dim lat As Double = coord1.Y * 0.0174532925199433

        Dim num3 As Double = coord2.X * 0.0174532925199433
        Dim num4 As Double = coord2.Y * 0.0174532925199433
        Dim num5 As Double
        Dim num6 As Double
        Dim num7 As Double
        inverseGeodeticSolver(lat, lon, num4, num3, num5, num6, _
                         num7)
        If Not Double.IsNaN(num6) Then
            num2 += num6
        End If

        Return num2
    End Function
    Public Shared Sub inverseGeodeticSolver(ByVal lat1 As Double, ByVal lon1 As Double, ByVal lat2 As Double, ByVal lon2 As Double, ByRef azimuth As Double, ByRef geodesicDistance As Double, _
 ByRef reverseAzimuth As Double)
        Dim num As Double = lon2 - lon1
        Dim num2 As Double = Math.Atan((1.0 - f) * Math.Tan(lat1))
        Dim num3 As Double = Math.Atan((1.0 - f) * Math.Tan(lat2))
        Dim num4 As Double = Math.Sin(num2)
        Dim num5 As Double = Math.Cos(num2)
        Dim num6 As Double = Math.Sin(num3)
        Dim num7 As Double = Math.Cos(num3)
        Dim num8 As Double = num
        Dim num9 As Integer = 1000
        Dim num12 As Double
        Dim num13 As Double
        Dim num14 As Double
        Dim num16 As Double
        Dim num17 As Double
        While True
            Dim num10 As Double = Math.Sin(num8)
            Dim num11 As Double = Math.Cos(num8)
            num12 = Math.Sqrt(num7 * num10 * (num7 * num10) + (num5 * num6 - num4 * num7 * num11) * (num5 * num6 - num4 * num7 * num11))
            If num12 = 0.0 Then
                Exit While
            End If
            num13 = num4 * num6 + num5 * num7 * num11
            num14 = Math.Atan2(num12, num13)
            Dim num15 As Double = num5 * num7 * num10 / num12
            num16 = 1.0 - num15 * num15
            num17 = num13 - 2.0 * num4 * num6 / num16
            If Double.IsNaN(num17) OrElse Double.IsInfinity(num17) Then
                num17 = 0.0
            End If
            Dim num18 As Double = f / 16.0 * num16 * (4.0 + f * (4.0 - 3.0 * num16))
            Dim num19 As Double = num8
            num8 = num + (1.0 - num18) * f * num15 * (num14 + num18 * num12 * (num17 + num18 * num13 * (-1.0 + 2.0 * num17 * num17)))
            If Math.Abs(num8 - num19) <= 0.000000000001 OrElse System.Threading.Interlocked.Decrement(num9) <= 0 Then
                GoTo IL_1EE
            End If
        End While
        geodesicDistance = Double.NaN
        azimuth = Double.NaN
        reverseAzimuth = Double.NaN
        Return
IL_1EE:
        If num9 = 0 Then
            Dim num20 As Double = (a + b) / 2.0
            Dim num21 As Double = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon2 - lon1)) * num20
            Dim num22 As Double = lon2 - lon1
            Dim num23 As Double = Math.Sin(num22) * Math.Cos(lat2)
            Dim num24 As Double = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(num22)
            Dim num25 As Double = Math.Atan2(num23, num24)
            azimuth = num25
            geodesicDistance = num21
            reverseAzimuth = Double.NaN
            Return
        End If
        Dim num26 As Double = num16 * (a * a - b * b) / (b * b)
        Dim num27 As Double = 1.0 + num26 / 16384.0 * (4096.0 + num26 * (-768.0 + num26 * (320.0 - 175.0 * num26)))
        Dim num28 As Double = num26 / 1024.0 * (256.0 + num26 * (-128.0 + num26 * (74.0 - 47.0 * num26)))
        Dim num29 As Double = num28 * num12 * (num17 + num28 / 4.0 * (num13 * (-1.0 + 2.0 * num17 * num17) - num28 / 6.0 * num17 * (-3.0 + 4.0 * num12 * num12) * (-3.0 + 4.0 * num17 * num17)))
        Dim num30 As Double = b * num27 * (num14 - num29)
        Dim num31 As Double = Math.Atan2(num7 * Math.Sin(num8), num5 * num6 - num4 * num7 * Math.Cos(num8))
        Dim num32 As Double = Math.Atan2(num5 * Math.Sin(num8), num5 * num6 * Math.Cos(num8) - num4 * num7)
        azimuth = num31
        geodesicDistance = num30
        reverseAzimuth = num32
    End Sub
    Public Shared Function getPolylineLength(ByVal pPolyline As Polyline, ByVal Map As Esri.ArcGIS.Mobile.WinForms.Map, ByVal LengthUnit As SpatialReferences.Unit) As Double

        Dim distance As Double = 0.0

        For Each prt As CoordinateCollection In pPolyline.Parts
            'Dim pNewPoly As New Polyline(m_Map.SpatialReference.ToWgs84(prt))

            For i As Integer = 0 To prt.Count - 2
                distance += GlobalsFunctions.SegmentMeasures(Map, LengthUnit, prt(i), prt(i + 1))

            Next
        Next
        Return distance
    End Function
    Public Shared Function SegmentMeasures(ByVal map As Esri.ArcGIS.Mobile.WinForms.Map, ByVal displayUnit As Esri.ArcGIS.Mobile.SpatialReferences.Unit, ByVal coord1 As Coordinate, ByVal coord2 As Coordinate) As Double
        If appConfig.MeasureOptions.UseGeodeticMeasure.ToUpper = "FALSE" Then
            Dim pLine As New Polyline
            pLine.AddCoordinate(coord1)
            pLine.AddCoordinate(coord2)

            Return pLine.Length



        End If
        ' determine segment distance
        Geodesic(6378137.0, 6356752.31424518)

        ' Dim distance As Double = coord1.Distance(coord2)
        Dim wgsCoord1 As Coordinate = map.SpatialReference.ToWgs84(coord1)
        Dim wgsCoord2 As Coordinate = map.SpatialReference.ToWgs84(coord2)

        Dim wgsDistance As Double = wgsCoord1.Distance(wgsCoord2)

        'distance = GlobalsFunctions.ConvertDistance(distance, map.SpatialReference.Unit, displayUnit)

        wgsDistance = GeodesicLength(wgsCoord1, wgsCoord2)
        ' Dim wgsDistance2 As Double = distanceWGS(wgsCoord1.Y, wgsCoord1.X, wgsCoord2.Y, wgsCoord2.X, "K")
        wgsDistance = GlobalsFunctions.ConvertDistance(wgsDistance, Esri.ArcGIS.Mobile.SpatialReferences.Unit.Meter, displayUnit)
        'wgsDistance2 = GlobalsFunctions.ConvertDistance(wgsDistance2, Esri.ArcGIS.Mobile.SpatialReferences.Unit.Kilometer, displayUnit)
        Return wgsDistance


    End Function



    Public Shared Sub GetServerToken(ByVal mobileConnect As Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.MobileServiceConnection)

        Try

            'Dim cs As CatalogService = New CatalogService()
            'cs.
            'cs.Url = mobileConnect.Url


            'Dim bRequiretokens As Boolean = cs.RequiresTokens()

            ' '' if token is required
            ''If (m_bRequiretokens) Then

            ''    ' step 2. get the url for token server
            ''    Dim tokenserviceurl As String = cs.GetTokenServiceURL()

            ''    'step 3. create a tokencredential
            ''    Dim tokencredential As TokenCredential = New TokenCredential(getServiceUserName, getServicePassword)


            ''    ' step 4. use a tokengenerator to get the token from token server
            ''    Dim tokengenerator As TokenGenerator = New TokenGenerator()


            ''    m_Token = tokengenerator.GenerateToken(tokenserviceurl, tokencredential)

            ''    m_MobileService.ServiceConnection.TokenCredential = tokencredential

            ''Else

            'm_bRequiretokens = False
            'm_Token = "NoTokenNeeded"
            ''End If



        Catch ex As Exception


            ' m_Token = "-99"

        End Try

    End Sub
    Public Shared Sub OverrideCertificateValidation()
        ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf RemoteCertValidate)
    End Sub



    Private Shared Function RemoteCertValidate(ByVal sender As Object, ByVal cert As X509Certificate, ByVal chain As X509Chain, ByVal [error] As System.Net.Security.SslPolicyErrors) As Boolean
        Return True
    End Function
    Public Shared Function URLIsMobileServer(ByVal url As String) As Boolean
        If url Is Nothing Then Return False
        If url.Contains("/MobileServer") Then Return True
        Return False

    End Function
    Public Shared Function UrlIsValid(ByVal url As String) As Boolean
        If url Is Nothing Then Return False

        If url.ToLower().StartsWith("www.") Then url = "http://" & url

        If UCase(url).Contains(UCase("http://")) Then
            Return True
        ElseIf UCase(url).Contains(UCase("https://")) Then
            Return True
        Else
            Return False
        End If


        'Dim webResponse As HttpWebResponse = Nothing

        'Try

        '    Dim webRequest As HttpWebRequest = HttpWebRequest.Create(url)

        '    webResponse = DirectCast(WebRequest.GetResponse(), HttpWebResponse)

        '    Return True

        'Catch

        '    Return False

        'Finally

        '    If webResponse IsNot Nothing Then webResponse.Close()

        'End Try

    End Function
    Public Shared Function getDomainCode(ByVal value As String, ByVal CodedValue As CodedValueDomain) As String
        For Each pDR As DataRow In CodedValue.Rows
            If pDR(0).ToString() = value Then
                Return pDR(1).ToString
            End If
        Next
        Return value

    End Function
    Public Shared Function getDomainValue(ByVal Code As String, ByVal CodedValue As CodedValueDomain) As String
        For Each pDR As DataRow In CodedValue.Rows
            If pDR(1).ToString() = Code Then
                Return pDR(0).ToString
            End If
        Next
        Return Code

    End Function
    Public Shared Function AddDomainValueToDataTable(ByVal pFDT As FeatureDataTable) As FeatureDataTable
        Return pFDT
        'Function cannot translate domains and store the code back in the datatable because of the field types.  
        'Function needs to be translated to create a new table and then the datagrid needs to look up the feature for zooming and flashing

        Dim pFDR As FeatureDataReader
        Dim pQFilt As QueryFilter = New QueryFilter
        pQFilt.WhereClause = "1=0"

        pFDR = pFDT.FeatureSource.GetDataReader(pQFilt)

        'Dim pNFDT As New FeatureDataTable(pFDR)
        Dim pNFDT As New FeatureDataTable(pFDT.FeatureSource)

        'Dim pFDTNew As DataTable = copySchema(pFDT)
        For Each pDC As DataColumn In pNFDT.Columns
            pDC.DataType = System.Type.GetType("System.String")

        Next

        For Each dr As FeatureDataRow In pFDT.Rows
            pNFDT.ImportRow(dr)

        Next
        ''pFDT = pFDTNew

        Dim pSubCol As DataColumn
        'Dim pSubVal As Integer
        Dim obj As Object
        Dim pCV As CodedValueDomain
        Try


            If pFDT.FeatureSource.HasSubtypes Then
                pSubCol = pFDT.FeatureSource.Columns(pFDT.FeatureSource.SubtypeColumnIndex)

            End If
            For Each pDC As DataColumn In pFDT.Columns
                If pSubCol IsNot Nothing Then

                    '  obj = pFDT.FeatureSource.GetDomain(pSubCol.DefaultValue, pDC.ColumnName)
                Else

                    '  obj = pFDT.FeatureSource.GetDomain(0, pDC.ColumnName)
                End If

                If obj IsNot Nothing Then
                    If TypeOf (obj) Is CodedValueDomain Then
                        pDC.DataType = System.Type.GetType("System.String")

                        For Each pRow As DataRow In pFDT.Rows
                            If pSubCol IsNot Nothing Then
                                'MsgBox("Fix Domain")
                                'obj = pFDT.FeatureSource.GetDomain(pRow.Item(pSubCol), pDC.ColumnName)
                                obj = CType(pDC, SourceDataColumn).GetDomain(pRow.Item(pSubCol))

                            End If

                            pCV = obj
                            pRow.Item(pDC.ColumnName) = getDomainCode(pRow.Item(pDC.ColumnName), pCV)

                        Next

                    End If


                End If

            Next

            Return pFDT

        Catch ex As Exception

        End Try
    End Function
    Public Shared Function getImageFromCam() As String
        Dim pCamForm As CameraForm
        Try
            pCamForm = New CameraForm()
            pCamForm.ShowDialog()
            If pCamForm.Photo IsNot Nothing Then
                Dim strPath As String = pCamForm.Photo
                Return strPath
            Else
                Return ""
            End If


        Catch ex As Exception
        Finally
            If pCamForm IsNot Nothing Then
                pCamForm.Dispose()
            End If

            pCamForm = Nothing

        End Try



    End Function

    Public Shared Function loadMapConfig()
        If File.Exists("MobileMapConfig.xml") Then

            conParent = MobileConfigClass.MobileConfig.LoadFromFile("MobileMapConfig.xml")
            'MsgBox(conParent.Config.LayerOptions.LayersFieldOptions.LayerName(0).FieldOption(0).FilterFields.Count)
            appConfig = conParent.Config
        End If

    End Function
    Public Shared Function writeConfig()
        appConfig.SaveToFile("MobileMapConfig.xml")

    End Function
    Public Shared Function CalcEnvelope(ByVal g As Geometry, ByVal mapWidth As Double, ByVal mapHeight As Double) As Envelope
        'Used to adjsut extent based on map aspect ration
        Try


            'get the extent of the geometry
            Dim originalEnvelope As Envelope = g.Extent()
            'Get the center
            Dim centralCoordinate As Coordinate = originalEnvelope.Center()
            Dim pEnvelope As Envelope
            'Determine map aspect ration
            Dim mapRatio As Double = mapWidth / mapHeight
            'Get feature ration
            Dim featureRatio As Double = originalEnvelope.Width / originalEnvelope.Height

            ' feature width exceeds expectation, need to resize its height
            If (featureRatio >= mapRatio) Then
                'Create a new envelope adjusted for aspect
                pEnvelope = New Envelope(centralCoordinate, originalEnvelope.Width, (originalEnvelope.Height * mapHeight / mapWidth))

            Else
                'Create a new envelope adjusted for aspect
                pEnvelope = New Envelope(centralCoordinate, originalEnvelope.Height * mapWidth / mapHeight, originalEnvelope.Height)


            End If
            'expand it some
            pEnvelope = pEnvelope.Resize(1.2)

            'cleanup
            originalEnvelope = Nothing
            centralCoordinate = Nothing

            'return it
            Return pEnvelope
        Catch ex As Exception
            'on error return original extent
            Return g.Extent()
        End Try
    End Function
    Public Shared Function ServerToMobileGeom(ByVal DistanceInFeet As Double, ByVal Map As Esri.ArcGIS.Mobile.WinForms.Map) As Double
        'Convert feet to map units
        Return Esri.ArcGIS.Mobile.SpatialReferences.Unit.FromUnitToUnit(DistanceInFeet, Esri.ArcGIS.Mobile.SpatialReferences.Unit.Foot, Map.SpatialReference.Unit)

    End Function

    Public Shared Function FolderFromFileName _
     (ByVal FileFullPath As String) As String
        'EXAMPLE: input ="C:\winnt\system32\kernel.dll, 
        'output = C:\winnt\system32\


        Dim strParts As String() = FileFullPath.Split("\")
        For i As Integer = strParts.Length - 1 To 0 Step -1
            If strParts(i).Length > 0 Then
                Return strParts(i)
            End If
        Next
        Return ""


    End Function

    Public Shared Function NameOnlyFromFullPath _
      (ByVal FileFullPath As String) As String

        'EXAMPLE: input ="C:\winnt\system32\kernel.dll, 
        'output = kernel.dll

        Dim intPos As Integer

        intPos = FileFullPath.LastIndexOfAny("\")
        intPos += 1


        Return FileFullPath.Substring(intPos, _
            (Len(FileFullPath) - intPos))

    End Function
    Private Sub FixZoom(ByVal bOut As Boolean, ByVal xCent As Double, ByVal yCent As Double, ByVal Map As Esri.ArcGIS.Mobile.WinForms.Map)
        'Make sure the map is valid
        If Map Is Nothing Then Return
        If Map.IsValid = False Then Return
        'Determine which direction to zoom
        If (bOut) Then
            Dim pExt As Esri.ArcGIS.Mobile.Geometries.Envelope
            'Get the map extent
            pExt = Map.Extent()
            'Resize it
            pExt.Resize(1.5)
            'Center it
            pExt.CenterAt(xCent, yCent)

            'Set the map extent
            Map.Extent = pExt
            'cleanup
            pExt = Nothing
        Else
            Dim pExt As Esri.ArcGIS.Mobile.Geometries.Envelope
            'Get the map extent
            pExt = Map.Extent()
            'Resize it
            pExt.Resize(0.5)
            'Center it
            pExt.CenterAt(xCent, yCent)

            'Set the map extent
            Map.Extent = pExt
            'cleanup
            pExt = Nothing
        End If


    End Sub

    Public Shared Sub zoomTo(ByVal pGeo As Geometry, ByVal Map As Esri.ArcGIS.Mobile.WinForms.Map)
        'Zoom to a feature
        Try
            If pGeo Is Nothing Then Return
            If pGeo.IsEmpty Then Return

            Dim pEnv As Esri.ArcGIS.Mobile.Geometries.Envelope
            'Determine type of feature
            If pGeo.GeometryType = Esri.ArcGIS.Mobile.Geometries.GeometryType.Point Then
                'If a point, center on it and create an envelope 50 around around it
                Dim pIntExtGeo As Integer = CInt(ServerToMobileGeom(appConfig.ApplicationSettings.ZoomExtent, Map))
                pEnv = New Envelope(0, 0, pIntExtGeo, pIntExtGeo)
                pEnv.CenterAt(CType(pGeo, Esri.ArcGIS.Mobile.Geometries.Point).Coordinate)
            Else

                'Calc the envelope based on the map aspect ration
                pEnv = CalcEnvelope(pGeo, Map.Width, Map.Height)


            End If

            'Set the extent to the map
            Map.Extent = pEnv
            'Cleanup
            pEnv = Nothing
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Public Shared Sub panTo(ByVal pGeo As Geometry, ByVal Map As Esri.ArcGIS.Mobile.WinForms.Map)
        'Zoom to a feature
        Try

            If pGeo Is Nothing Then Return
            If pGeo.IsEmpty Then Return

            Dim pEnv As Esri.ArcGIS.Mobile.Geometries.Envelope
            'Determine type of feature
            pEnv = Map.Extent

            If pGeo.GeometryType = Esri.ArcGIS.Mobile.Geometries.GeometryType.Point Then

                'If a point, center on it and create an envelope 50 around around it


                pEnv.CenterAt(CType(pGeo, Esri.ArcGIS.Mobile.Geometries.Point).Coordinate)
            Else
                Dim centralCoordinate As Coordinate = pGeo.Extent.Center()
                pEnv.CenterAt(centralCoordinate)
                centralCoordinate = Nothing



            End If

            'Set the extent to the map
            Map.Extent = pEnv
            'Cleanup
            pEnv = Nothing
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub

    Public Shared Sub flashGeo(ByVal pGeo As Geometry, ByVal Map As Esri.ArcGIS.Mobile.WinForms.Map, ByVal pen As System.Drawing.Pen, ByVal brush As System.Drawing.Brush)
        Try

            If pGeo.IsEmpty Then Return
            Map.FlashGeometry(pen, brush, 20, 100, 5, pGeo)




        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Public Shared Function GetGeometryCenterPoint(ByVal Geom As Geometries.Geometry) As Esri.ArcGIS.Mobile.Geometries.Point
        Dim pCoord As Coordinate = GetGeometryCenter(Geom)
        If pCoord Is Nothing Then Return Nothing

        Dim pPnt As Esri.ArcGIS.Mobile.Geometries.Point = New Esri.ArcGIS.Mobile.Geometries.Point(GetGeometryCenter(Geom))
        Return pPnt
    End Function

    Public Shared Function GetGeometryCenter(ByVal Geom As Geometries.Geometry) As Esri.ArcGIS.Mobile.Geometries.Coordinate
        Dim pt As Esri.ArcGIS.Mobile.Geometries.Point = Nothing


        Select Case Geom.GeometryType
            Case GeometryType.Point
                pt = CType(Geom, Esri.ArcGIS.Mobile.Geometries.Point)

            Case GeometryType.Polygon
                Dim poly As Polygon = CType(Geom, Esri.ArcGIS.Mobile.Geometries.Polygon)

                pt = New Esri.ArcGIS.Mobile.Geometries.Point(poly.Extent.Center)



            Case GeometryType.Polyline

                Dim pl As Polyline = CType(Geom, Polyline)
                Dim pathCount As Integer = pl.PartCount

                Dim pathIndex As Integer
                If (pathCount <> 1) Then
                    pathIndex = CInt((pathCount / 2)) - 1
                Else

                    pathIndex = 0
                End If
                If (pathCount <> 1) Then
                    pathIndex = CInt((pathCount / 2)) - 1

                Else
                    Dim midPath As CoordinateCollection = pl.Parts(pathIndex)
                    Dim ptCount As Integer = midPath.Count

                    If (ptCount = 2) Then


                        pt = New Esri.ArcGIS.Mobile.Geometries.Point((midPath.Item(0).X + midPath.Item(1).X) / 2, (midPath.Item(0).Y + midPath.Item(1).Y) / 2)
                    Else

                        Dim ptIndex As Integer = CInt(ptCount / 2) - 1
                        pt = New Esri.ArcGIS.Mobile.Geometries.Point(midPath.Item(ptIndex))

                    End If
                End If


            Case GeometryType.Multipoint
                Dim multi As Multipoint = CType(Geom, Esri.ArcGIS.Mobile.Geometries.Multipoint)

                pt = New Esri.ArcGIS.Mobile.Geometries.Point(multi.Extent.Center)



        End Select
        If pt Is Nothing Then Return Nothing

        Return pt.Coordinate

    End Function


    'Private Sub zoomTo(ByVal pGeo As Geometry)
    '    'Zoom to a feature
    '    Try
    '        If pGeo Is Nothing Then Return
    '        If pGeo.IsEmpty Then Return

    '        Dim pEnv As Esri.ArcGIS.Mobile.Geometries.Envelope
    '        'Determine type of feature
    '        If pGeo.GeometryType = Esri.ArcGIS.Mobile.Geometries.GeometryType.Point Then
    '            'If a point, center on it and create an envelope 50 around around it
    '            Dim pIntExtGeo As Double = ServerToMobileGeom(m_ZoomToVal)
    '            pEnv = New Envelope(0, 0, pIntExtGeo, pIntExtGeo)
    '            pEnv.CenterAt(CType(pGeo, Esri.ArcGIS.Mobile.Geometries.Point).Coordinate)
    '        Else

    '            'Calc the envelope based on the map aspect ration
    '            pEnv = CalcEnvelope(pGeo, m_Map.Width, m_Map.Height)
    '            pEnv.Resize(1.3)


    '        End If

    '        'Set the extent to the map
    '        m_Map.Extent = pEnv)
    '        'Cleanup
    '        pEnv = Nothing
    '    Catch ex As Exception
    '        Dim st As New StackTrace
    '        MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
    '        st = Nothing

    '    End Try
    'End Sub
    'Private Sub zoomTo(ByVal pCoord As Coordinate)
    '    'Zoom to a coordinate
    '    Try
    '        If pCoord Is Nothing Then Return
    '        If pCoord.IsEmpty Then Return

    '        'Construct a new envelope
    '        Dim pEnv As Esri.ArcGIS.Mobile.Geometries.Envelope
    '        'center on the coordinate and create an envelope 50 around around it
    '        Dim pIntExtGeo As Double = ServerToMobileGeom(m_ZoomToVal)
    '        pEnv = New Envelope(0, 0, pIntExtGeo, pIntExtGeo)
    '        pEnv.CenterAt(CType(New Esri.ArcGIS.Mobile.Geometries.Point(pCoord.X, pCoord.Y), Esri.ArcGIS.Mobile.Geometries.Point).Coordinate)
    '        'set map extent
    '        m_Map.Extent = pEnv)
    '        'clean up
    '        pEnv = Nothing
    '    Catch ex As Exception
    '        Dim st As New StackTrace
    '        MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
    '        st = Nothing

    '    End Try
    'End Sub
    'Private Sub flashGeo(ByVal pGeo As Geometry)
    '    Try


    '        'Flash the geometry, different for points and other geo
    '        If pGeo.GeometryType = GeometryType.Point Then
    '            m_Map.FlashGeometry(m_penLine, CType(m_brush, SolidBrush), 20, 100, 5, pGeo)
    '        Else

    '            m_Map.FlashGeometry(m_pen, m_brush, 20, 100, 5, pGeo)
    '        End If
    '    Catch ex As Exception
    '        Dim st As New StackTrace
    '        MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
    '        st = Nothing

    '    End Try
    'End Sub
    'Private Sub flashGeo(ByVal pGeo As Coordinate)
    '    Try

    '        m_Map.FlashGeometry(m_penLine, m_brush, 20, 100, 5, New Esri.ArcGIS.Mobile.Geometries.Point(pGeo))

    '    Catch ex As Exception
    '        Dim st As New StackTrace
    '        MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
    '        st = Nothing

    '    End Try
    'End Sub




    Public Shared Function mapToBuffer(ByVal Map As Esri.ArcGIS.Mobile.WinForms.Map, ByVal buffVal As Double) As Double
        Try
            'Verify the map is valid
            If Map Is Nothing Then Return 0

            If Map.IsValid = False Then Return 0

            'Get the buffer amount for the ID based on screen size
            Dim intBufferValueforPoint As Double

            intBufferValueforPoint = (Map.Width + Map.Height) / 4
            intBufferValueforPoint = Map.ToMap((intBufferValueforPoint / buffVal))
            If intBufferValueforPoint = 0 Then

                intBufferValueforPoint = 1
            End If
            Return intBufferValueforPoint

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try


    End Function
    Public Shared Function bufferToMap(ByVal Map As Esri.ArcGIS.Mobile.WinForms.Map, ByVal buffVal As Double) As Double
        Try
            'Verify the map is valid
            If Map Is Nothing Then Return 0

            If Map.IsValid = False Then Return 0
            'Get the buffer amount for the ID based on screen size
            Dim intBufferValueforPoint As Double


            intBufferValueforPoint = Map.ToMap((buffVal))

            If intBufferValueforPoint = 0 Then

                intBufferValueforPoint = 1
            End If
            Return intBufferValueforPoint

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try


    End Function

    Public Shared Function ByteToImage(ByVal imgBytes() As Byte) As System.Drawing.Bitmap
        'Convert a bit array to an image


        Try
            'Check to see if the byte array has data
            If imgBytes.Length = 0 Then Return Nothing
            'Convert to image
            Dim image As Byte() = imgBytes
            Dim memStream As System.IO.MemoryStream = New System.IO.MemoryStream(image)
            Dim bitImage As System.Drawing.Bitmap = New System.Drawing.Bitmap(System.Drawing.Image.FromStream(memStream))
            Return bitImage
        Catch ex As Exception
            'The byte array was not a valid image
            Return Nothing
        End Try

    End Function
    Public Shared Function LayerNameMatches(featLay As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource, Name As String) As Boolean
        Try
            If featLay.Name = Name Then Return True
            If featLay.Name.Substring(featLay.Name.LastIndexOf(".") + 1) = Name Then Return True

            If featLay.ServerFeatureClassName.Substring(featLay.ServerFeatureClassName.LastIndexOf(".") + 1) = Name Then Return True
            Return False
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
            Return False
        End Try



    End Function

    Public Shared Function validateStreet() As Boolean
        If GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LayerName Is Nothing Then
            ' MsgBox("Error in the Address Init, the Street Layer Name Tag is missing or not set")
            Return False

        End If
        If GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LayerName.Trim = "" Then
            'MsgBox("Error in the Address Init, the Street Layer Name Tag is missing or not set")
            Return False

        End If
        If GlobalsFunctions.appConfig.SearchPanel.AddressSearch.NameField Is Nothing Then
            MsgBox(String.Format(GlobalsFunctions.appConfig.ApplicationSettings.UIComponents.FieldNotFound, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.NameField, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LayerName))

            Return False

        End If
        If GlobalsFunctions.appConfig.SearchPanel.AddressSearch.NameField.Trim = "" Then
            MsgBox(String.Format(GlobalsFunctions.appConfig.ApplicationSettings.UIComponents.FieldNotFound, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.NameField, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LayerName))

            Return False
        End If

        If GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LowerLeft Is Nothing Then
            MsgBox(String.Format(GlobalsFunctions.appConfig.ApplicationSettings.UIComponents.FieldNotFound, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LowerLeft, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LayerName))
            Return False
        End If

        If GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LowerLeft.Trim = "" Then
            MsgBox(String.Format(GlobalsFunctions.appConfig.ApplicationSettings.UIComponents.FieldNotFound, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LowerLeft, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LayerName))
            Return False
        End If


        If GlobalsFunctions.appConfig.SearchPanel.AddressSearch.UpperLeft Is Nothing Then
            MsgBox(String.Format(GlobalsFunctions.appConfig.ApplicationSettings.UIComponents.FieldNotFound, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.UpperLeft, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LayerName))
            Return False
        End If
        If GlobalsFunctions.appConfig.SearchPanel.AddressSearch.UpperLeft.Trim = "" Then
            MsgBox(String.Format(GlobalsFunctions.appConfig.ApplicationSettings.UIComponents.FieldNotFound, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.UpperLeft, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LayerName))
            Return False
        End If


        If GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LowerRight Is Nothing Then
            MsgBox(String.Format(GlobalsFunctions.appConfig.ApplicationSettings.UIComponents.FieldNotFound, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LowerRight, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LayerName))
            Return False
        End If
        If GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LowerRight.Trim = "" Then
            MsgBox(String.Format(GlobalsFunctions.appConfig.ApplicationSettings.UIComponents.FieldNotFound, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LowerRight, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LayerName))
            Return False
        End If
        If GlobalsFunctions.appConfig.SearchPanel.AddressSearch.UpperRight Is Nothing Then
            MsgBox(String.Format(GlobalsFunctions.appConfig.ApplicationSettings.UIComponents.FieldNotFound, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.UpperRight, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LayerName))
            Return False
        End If
        If GlobalsFunctions.appConfig.SearchPanel.AddressSearch.UpperRight.Trim = "" Then
            MsgBox(String.Format(GlobalsFunctions.appConfig.ApplicationSettings.UIComponents.FieldNotFound, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.UpperRight, GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LayerName))
            Return False
        End If
        Return True

        ''Set up Street Layer parems
        'm_AddressLayer = GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LayerName
        'm_AddressFieldStreetName = GlobalsFunctions.appConfig.SearchPanel.AddressSearch.NameField
        'm_AddressFieldLeftFrom = GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LowerLeft
        'm_AddressFieldLeftTo = GlobalsFunctions.appConfig.SearchPanel.AddressSearch.UpperLeft
        'm_AddressFieldRightFrom = GlobalsFunctions.appConfig.SearchPanel.AddressSearch.LowerRight
        'm_AddressFieldRightTo = GlobalsFunctions.appConfig.SearchPanel.AddressSearch.UpperRight
    End Function
    Public Shared Function GetLayerDefinition(ByVal Map As Esri.ArcGIS.Mobile.WinForms.Map, FeatureSourceWithDef As FeatureSourceWithDef) As MobileCacheMapLayerDefinition

        Return CType(Map.MapLayers(FeatureSourceWithDef.MapLayerIndex), MobileCacheMapLayer).LayerDefinitions(FeatureSourceWithDef.LayerIndex)



    End Function
    Public Shared Function GetMapLayer(ByVal LayerName As String, ByVal Map As ESRI.ArcGIS.Mobile.WinForms.Map) As ESRI.ArcGIS.Mobile.MapLayer


        For Each pL As ESRI.ArcGIS.Mobile.MapLayer In Map.MapLayers
            'If the layer is a feature layer

            If TypeOf pL Is ESRI.ArcGIS.Mobile.FeatureCaching.MobileCacheMapLayer Then
                If pL.Name = LayerName Then
                    Return pL

                End If
            ElseIf TypeOf pL Is ESRI.ArcGIS.Mobile.DataProducts.RasterData.TileCacheMapLayer Then
                If pL.Name = LayerName Then
                    Return pL

                End If

            ElseIf TypeOf pL Is ESRI.ArcGIS.Mobile.WebServices.ArcGISServices.MapServices.TileServiceMapLayer Then
                If pL.Name = LayerName Then
                    Return pL

                End If

            End If

        Next


        Return Nothing

    End Function

    Public Shared Function GetFeatureSource(ByVal LayerName As String, ByVal thisMap As Esri.ArcGIS.Mobile.WinForms.Map) As FeatureSourceWithDef
        Try
            If thisMap Is Nothing Then Return Nothing

            Dim pFS As FeatureSource = Nothing
            Dim mapLayIdx As Integer = 0
            Dim layIdx As Integer = 0
            Dim MapLayer As MapLayer

            For mapLayIdx = 0 To thisMap.MapLayers.Count - 1 'For Each MapLayer In thisMap.MapLayers
                MapLayer = thisMap.MapLayers(mapLayIdx)
                If TypeOf (MapLayer) Is MobileCacheMapLayer Then
                    For layIdx = 0 To CType(MapLayer, MobileCacheMapLayer).MobileCache.FeatureSources.Count - 1
                        pFS = CType(MapLayer, MobileCacheMapLayer).MobileCache.FeatureSources(layIdx)

                        If LayerNameMatches(pFS, LayerName) Then
                            Return New FeatureSourceWithDef(pFS, MapLayer, layIdx, mapLayIdx)

                        End If
                    Next




                End If
            Next


            Return New FeatureSourceWithDef(Nothing, Nothing, -1, -1)



        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

            Return Nothing

        End Try
    End Function
    Public Shared Function slope(ByVal pcoord1 As Coordinate, ByVal pcoord2 As Coordinate) As Double
        'Find Slope
        Return (pcoord2.Y - pcoord1.Y) / (pcoord2.X - pcoord1.X)

    End Function
    Public Shared Function distanceBetweenPoints(ByVal pCoordFirst As Coordinate, ByVal pCoordEnd As Coordinate) As Double

        'Function to get the distance between two points
        Dim xDist As Double = pCoordFirst.X - pCoordEnd.X

        Dim yDist As Double = pCoordFirst.Y - pCoordEnd.Y

        Return Math.Sqrt(xDist * xDist + yDist * yDist)


    End Function
    Public Shared Sub SetDefQueryOnLayers(sqlString As String, ByVal thisMap As Esri.ArcGIS.Mobile.WinForms.Map)
        Try
            If thisMap Is Nothing Then Return

            Dim pFS As FeatureSource = Nothing
            Dim mapLayIdx As Integer = 0
            Dim layIdx As Integer = 0
            Dim MapLayer As MapLayer

            For mapLayIdx = 0 To thisMap.MapLayers.Count - 1 'For Each MapLayer In thisMap.MapLayers
                MapLayer = thisMap.MapLayers(mapLayIdx)
                If TypeOf (MapLayer) Is MobileCacheMapLayer Then

                    For layIdx = 0 To CType(MapLayer, MobileCacheMapLayer).LayerDefinitions.Count - 1
                        Try

                            CType(MapLayer, MobileCacheMapLayer).LayerDefinitions(layIdx).DisplayExpression = sqlString

                        Catch ex As Exception

                        End Try


                    Next




                End If
            Next




        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing



        End Try
    End Sub
    Public Shared Function ConvertDistance(ByVal distance As Double, ByVal sourceUnit As Unit, ByVal targetUnit As Unit)
        Try


            If sourceUnit Is targetUnit Then
                Return distance

            End If
            Dim strUnit As String = sourceUnit.ToString()

            Select Case sourceUnit.ToString()
                Case "Meter"
                    strUnit = "Meter"
                Case "Centimeter"
                    distance = distance * 100
                    strUnit = "Meter"
                Case "Millimeter"
                    distance = distance * 1000
                    strUnit = "Meter"
                Case "Kilometer"
                    distance = distance * 1000
                    strUnit = "Meter"
                Case "Mile"
                    distance = distance * 5280
                    strUnit = "Foot"
                Case "Yard"
                    distance = distance * 3
                    strUnit = "Foot"
                Case "NauticalMile"
                    distance = distance * 6076.115
                    strUnit = "Foot"
            End Select

            Select Case strUnit
                Case "Meter"

                    If targetUnit Is Unit.Centimeter Then
                        Return distance * 100

                    ElseIf targetUnit Is Unit.Degree Then
                        Return distance

                    ElseIf targetUnit Is Unit.Foot Then
                        Return distance * 3.2808399
                    ElseIf targetUnit Is Unit.Inch Then
                        Return distance * 3.2808399 * 12

                    ElseIf targetUnit Is Unit.Kilometer Then
                        Return distance * 0.001

                    ElseIf targetUnit Is Unit.Mile Then
                        Return distance * 0.000621371192
                    ElseIf targetUnit Is Unit.Millimeter Then
                        Return distance * 1000
                    ElseIf targetUnit Is Unit.Yard Then
                        Return distance * 1.0936133

                    ElseIf targetUnit Is Unit.NauticalMile Then
                        Return distance * 0.000539956803
                    Else
                        Return distance



                    End If
                Case "Foot"

                    If targetUnit Is Unit.Centimeter Then
                        Return distance * 30.48
                    ElseIf targetUnit Is Unit.Degree Then
                        Return distance
                    ElseIf targetUnit Is Unit.Foot Then
                        Return distance
                    ElseIf targetUnit Is Unit.Meter Then
                        Return distance * 0.3048
                    ElseIf targetUnit Is Unit.Inch Then
                        Return distance * 12

                    ElseIf targetUnit Is Unit.Kilometer Then
                        Return distance * 0.0003048

                    ElseIf targetUnit Is Unit.Mile Then
                        Return distance * 0.000189393939
                    ElseIf targetUnit Is Unit.Millimeter Then
                        Return distance * 304.8
                    ElseIf targetUnit Is Unit.Yard Then
                        Return distance * 0.0333333333

                    ElseIf targetUnit Is Unit.NauticalMile Then
                        Return distance * 0.000164578834
                    Else
                        Return distance



                    End If

            End Select

            Return distance

        Catch ex As Exception
            Return distance

        End Try
    End Function
    Public Shared Function copySchema(ByRef pSourceDT As DataTable) As DataTable

        Dim stream As System.IO.MemoryStream


        Dim pDT As New DataTable

        Try
            stream = New System.IO.MemoryStream()
            pSourceDT.WriteXmlSchema(stream)
            pDT.Namespace = pSourceDT.Namespace
            pDT.TableName = pSourceDT.TableName
            stream.Position = 0
            pDT.ReadXml(stream)


            Return pDT
        Catch ex As Exception
            Return Nothing
        Finally
            stream = Nothing

        End Try

    End Function
    Public Shared Function GetBookmarks() As Bookmarks

        Dim xmld As XmlDocument = Nothing
        Dim nodelist As XmlNodeList = Nothing
        Dim node As XmlNode = Nothing
        Dim pEntries As List(Of BookmarkDetails) = Nothing
        Dim pSingleEntries As BookmarkDetails = Nothing

        Try

            Dim path As String = GetBookmarkFile()
            If path = "" Then Return Nothing

            xmld = New XmlDocument

            xmld.Load(path)

            If xmld Is Nothing Then
                Return Nothing
            End If

            'Get the list of name nodes 
            Dim bk As Bookmarks
            node = xmld.SelectSingleNode("BookmarkEntries")
            bk = DirectCast(GlobalsFunctions.DeserializeObject(node, GetType(Bookmarks)), Bookmarks)
            Return bk


            'nodelist = xmld.SelectNodes("Bookmarks/Bookmark")
            'If nodelist Is Nothing Then
            '    Return Nothing
            'End If
            ''Loop through the nodes 
            'pEntries = New List(Of BookmarkDetails)()


            'For i As Integer = 0 To nodelist.Count - 1
            '    node = nodelist.Item(i)



            '    pSingleEntries = DirectCast(GlobalsFunctions.DeserializeObject(node, GetType(BookmarkDetails)), BookmarkDetails)
            '    'pSingleEntries = GlobalsFunctions.DeserializeObject(node, GetType(BookmarkDetails))

            '    If pSingleEntries IsNot Nothing Then
            '        pEntries.Add(pSingleEntries)

            '    End If
            'Next


            'Return pEntries
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing



            Return Nothing
        Finally

            xmld = Nothing
            nodelist = Nothing
            node = Nothing
            pSingleEntries = Nothing
        End Try
    End Function
    Public Shared Function AddBookmarks(ByVal bookMark As BookmarkDetails, ByVal delete As Boolean) As Bookmarks

        Dim xmld As XmlDocument = Nothing
        Dim bookMarks As Bookmarks = Nothing
        Dim encodedString() As Byte
        Dim ms As MemoryStream
        Dim boolFnd As Boolean = delete
        Try

            bookMarks = GetBookmarks()

            For Each ent As BookmarkDetails In bookMarks.Bookmark

                If ent.Name = bookMark.Name And delete = False Then
                    ent.XMax = bookMark.XMax
                    ent.XMin = bookMark.XMin
                    ent.YMax = bookMark.YMax
                    ent.YMin = bookMark.YMin
                    boolFnd = True
                    Exit For
                ElseIf ent.Name = bookMark.Name And delete Then
                    'entToDel = ent
                    bookMarks.Bookmark.Remove(ent)
                    boolFnd = True
                    Exit For
                End If
            Next
            If boolFnd = False Then
                bookMarks.Bookmark.Add(bookMark)

            End If
            Dim stringTest As String = GlobalsFunctions.SerializeObject(bookMarks, GetType(Bookmarks))

            Dim path As String = GetBookmarkFile()
            If path = "" Then Return Nothing


            xmld = New XmlDocument
            encodedString = Encoding.UTF8.GetBytes(stringTest)
            ms = New MemoryStream(encodedString)
            ms.Flush()
            ms.Position = 0
            xmld.Load(ms)
            xmld.Save(path)

            Return bookMarks

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

            xmld = Nothing
            bookMarks = Nothing
            encodedString = Nothing
            ms = Nothing


            Return Nothing
        Finally


        End Try
    End Function

    Public Shared Function createTextFile(ByVal sFileName As String, ByVal flMode As FileMode) As StreamWriter
        ' create the filename object - the while loop allows
        ' us to keep trying with different filenames until
        ' we succeed
        Dim sw As StreamWriter = Nothing
        Dim origname As String = sFileName
        Dim fs As System.IO.FileStream = Nothing
        Try

            ' open file for writing; throw an exception if the
            ' file already exists:
            '   FileMode.CreateNew to create a file if it
            '                   doesn't already exist or throw
            '                   an exception if file exists
            '   FileMode.Append to create a new file or append
            '                   to an existing file
            '   FileMode.Create to create a new file or 
            '                   truncate an existing file

            '   FileAccess possibilities are:
            '                   FileAccess.Read, 
            '                   FileAccess.Write,
            '                   FileAccess.ReadWrite
            fs = File.Open(sFileName, flMode, FileAccess.Write)


            ' generate a file stream with UTF8 characters
            sw = New StreamWriter(fs, System.Text.Encoding.UTF8)
            sw.WriteLine("Edit Log Started at: " + DateTime.Now)
            sw.WriteLine("-----------------------------------------")

            ' read one string at a time, outputting each to the
            ' FileStream open for writing
            ' Console.WriteLine("Enter text; enter blank line to stop");


            Return sw
        Catch fe As IOException
            MessageBox.Show("Unable to create the log file" & vbCr & vbLf + fe.Message)
            Return Nothing
        Finally
            fs = Nothing
        End Try

    End Function
    Public Shared Function SerializeObject(ByVal pObject As Object, ByVal type As Type) As String


        Dim memoryStream As MemoryStream = Nothing

        Dim xs As XmlSerializer = Nothing

        Dim xmlTextWriter As XmlTextWriter = Nothing

        Try

            Dim XmlizedString As String = Nothing

            memoryStream = New MemoryStream()

            xs = New XmlSerializer(type)

            xmlTextWriter = New XmlTextWriter(memoryStream, Encoding.UTF8)



            xs.Serialize(xmlTextWriter, pObject)

            memoryStream = DirectCast(xmlTextWriter.BaseStream, MemoryStream)

            XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray())


            Return XmlizedString

        Catch e As Exception

            System.Console.WriteLine(e)


            Return Nothing
        Finally
            memoryStream = Nothing

            xs = Nothing

            xmlTextWriter = Nothing
        End Try

    End Function
    Public Shared Function DeserializeObject(ByVal pXmlizedString As String, ByVal type As Type) As Object
        Dim xs As XmlSerializer = Nothing
        Dim memoryStream As MemoryStream = Nothing
        Dim xmlTextWriter As XmlTextWriter = Nothing
        Try
            xs = New XmlSerializer(type)

            memoryStream = New MemoryStream(StringToUTF8ByteArray(pXmlizedString))

            xmlTextWriter = New XmlTextWriter(memoryStream, Encoding.UTF8)



            Return xs.Deserialize(memoryStream)
        Catch ex As Exception
            MessageBox.Show("Error Reading the xml config file- DeserializeObject(string)" & vbLf & ex.InnerException.ToString())
            Return Nothing
        Finally
            xs = Nothing
            memoryStream = Nothing
            xmlTextWriter = Nothing
        End Try

    End Function
    Public Shared Function DeserializeObject(ByVal XMLNode As XmlNode, ByVal type As Type) As Object
        Try

            Dim xs As New XmlSerializer(type)

            Return xs.Deserialize(New XmlNodeReader(XMLNode))
        Catch ex As Exception
            MessageBox.Show("Error Reading the xml config file - DeserializeObject " & vbLf & ex.InnerException.ToString())
            Return Nothing
        End Try

    End Function
    Private Shared Function UTF8ByteArrayToString(ByVal characters As [Byte]()) As String
        Dim encoding As UTF8Encoding = Nothing
        Try
            encoding = New UTF8Encoding()

            Dim constructedString As String = encoding.GetString(characters)

            Return (constructedString)
        Catch ex As Exception
            Return ""
        Finally
            encoding = Nothing
        End Try
    End Function
    Private Shared Function StringToUTF8ByteArray(ByVal pXmlString As [String]) As Byte()

        Dim encoding As UTF8Encoding = Nothing
        Try
            encoding = New UTF8Encoding()
            Dim byteArray As Byte() = encoding.GetBytes(pXmlString)

            Return byteArray
        Catch ex As Exception

            Return Nothing
        Finally
            encoding = Nothing
        End Try


    End Function
    Public Shared Function returnToXML(ByVal XMLMessage As String) As XmlDocument
        Dim xmld As XmlDocument = Nothing
        Try
            'Create the XML Document
            xmld = New XmlDocument()

            'Load the Xml file
            'xmld.Load(GlobalsFunctions.getFileAtDLLLocation("LayerViewerConfig.xml"));
            Try

                ' xmld.Load(GlobalsFunctions.getFileAtDLLLocation("ESRI.WaterUtilitiesTemplate.DesktopFunctions.config"));
                ' XmlDocument xmlDoc = new XmlDocument();

                xmld.Load(New StringReader(XMLMessage))

                ' xmld.Load();


                Return xmld
            Catch ex As Exception
                '  System.Windows.Forms.MessageBox.Show(ex.Message + "\nTypically an error here is from an improperly formatted config file. \nThe structure(XML) is compermised by a change you made.");
                Return Nothing
            End Try
        Catch
            '(Exception ex)



            Return Nothing



        Finally
        End Try
    End Function
    Public Shared Function getFileAtDLLLocation(ByVal configFileName As String) As String
        Try
            Dim AppPath As String = Nothing
            AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)

            If (AppPath.IndexOf("file:\") >= 0) Then
                AppPath = AppPath.Replace("file:\", "")
            End If
            Dim pConfigFiles As String = ""

            Dim fullAssemblyName As String = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name
            Dim assemblyName As String = fullAssemblyName.Split("."c).GetValue(fullAssemblyName.Split("."c).Length - 1).ToString()
            'assemblyName & ".Config"
            If (System.IO.File.Exists(System.IO.Path.Combine(AppPath.ToString(), configFileName))) Then
                pConfigFiles = System.IO.Path.Combine(AppPath.ToString(), configFileName)
            ElseIf (System.IO.File.Exists(System.IO.Path.Combine(System.IO.Directory.GetParent(AppPath).ToString(), configFileName))) Then
                pConfigFiles = System.IO.Path.Combine(System.IO.Directory.GetParent(AppPath).ToString(), configFileName)
            ElseIf (System.IO.File.Exists(System.IO.Path.Combine(AppPath, configFileName))) Then
                pConfigFiles = System.IO.Path.Combine(AppPath, configFileName)
            End If
            Return pConfigFiles
        Catch ex As Exception
            MessageBox.Show("Error in Global Functions - getFileAtDLLLocation" + Environment.NewLine + ex.Message)


            Return ""
        Finally
        End Try
    End Function

    Public Shared Function generateWebMapsCachePath() As String


        Try

            Dim sPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ArcGIS4LocalGovernment\MobileTemplates\WebMapsCache")

            If Directory.Exists(sPath) = False Then
                Directory.CreateDirectory(sPath)

            End If
            Return sPath

        Catch ex As Exception
            Return Nothing
        End Try


    End Function
    Public Shared Function getRandonFileName() As String
        Return My.Computer.FileSystem.GetTempFileName


    End Function
    Public Shared Function stringToColor(ByVal key As String) As System.Drawing.Color


        Return System.Drawing.ColorTranslator.FromHtml(key) 'System.Drawing.Color.FromArgb(Int32.Parse(key.Replace("#", ""), Globalization.NumberStyles.HexNumber))
    End Function
    Public Shared Function ColorToString(ByVal color As System.Drawing.Color) As String


        Return System.Drawing.ColorTranslator.ToHtml(color) 'System.Drawing.Color.FromArgb(Int32.Parse(key.Replace("#", ""), Globalization.NumberStyles.HexNumber))
    End Function
    Public Shared Function getRandomFileNameInLocation(fileName As String) As String
        Dim strPath As String = getTempPhotoField()

        Dim i As Integer = 0
        '"MobilePhoto_{0}.bmp"
        Dim FileMask As String = System.IO.Path.Combine(strPath, fileName)



        fileName = String.Format(FileMask, i)
        Do While My.Computer.FileSystem.FileExists(fileName)
            i += 1
            fileName = String.Format(FileMask, i)
        Loop
        Return fileName

        ' When loop exits, you should have a unique name
    End Function
    Public Shared Function getTempPhotoField() As String


        Try

            Dim sPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ArcGIS4LocalGovernment\MobileTemplates\PhotoCache")

            If Directory.Exists(sPath) = False Then
                Directory.CreateDirectory(sPath)

            End If
            Return sPath

        Catch ex As Exception
            Return Nothing
        End Try


    End Function
    Public Shared Function deleteTempPhoto()
        Try


            For Each fl As String In System.IO.Directory.GetFiles(getTempPhotoField)
                'For Each foundFile As String In My.Computer.FileSystem.GetFiles(getTempPhotoField, FileIO.SearchOption.SearchAllSubDirectories)
                System.IO.File.Delete(fl)
                ' My.Computer.FileSystem.DeleteFile(foundFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
            Next
        Catch ex As Exception

        End Try
    End Function
    Public Shared Function generateUserCachePath() As String

        Try
            'ArcGIS4LocalGovernment\ConfigFiles

            Return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ArcGIS4LocalGovernment\ConfigFiles")
        Catch ex As Exception
            MessageBox.Show("generateUserCachePath:  " + ex.Message)
            Return ""
        End Try


    End Function
    Public Shared Function GetAllConfigFiles(ByVal includeLoaded As Boolean) As List(Of String)
        Try
            Dim pathToUserProf As String = generateUserCachePath()
            If System.IO.Directory.Exists(pathToUserProf) = False Then
                System.IO.Directory.CreateDirectory(pathToUserProf)
            End If



            Dim pConfFiles As New List(Of String)(Directory.GetFiles(pathToUserProf, "*.*onfig*", System.IO.SearchOption.AllDirectories))
            If pConfFiles.Count = 0 Then
                pConfFiles.Add(getInstalledConfig(generateUserCachePath()))
            ElseIf File.Exists(Path.Combine(pathToUserProf, "Loaded.config")) = False Then
                pConfFiles.Add(getInstalledConfig(generateUserCachePath()))
            End If
            If includeLoaded = False Then
                pConfFiles.Remove(Path.Combine(pathToUserProf, "Loaded.config"))
            End If
            Return pConfFiles
        Catch ex As Exception
            MessageBox.Show("GetAllConfigFiles:  " + ex.Message)
            Return Nothing
        End Try


    End Function
    Public Shared Function GetAllConfigFilesNames(ByVal includeLoaded As Boolean) As List(Of ConfigEntries)


        Dim pConfFiles As List(Of String) = Nothing
        Dim pConfFileNames As List(Of ConfigEntries) = Nothing
        Dim oXml As XmlDocument = Nothing
        Dim oList As XmlNodeList = Nothing
        Dim confEn As ConfigEntries = Nothing
        Dim oNode As XmlNode = Nothing
        Try

            pConfFiles = GetAllConfigFiles(True)
            pConfFileNames = New List(Of ConfigEntries)()

            For i As Integer = 0 To pConfFiles.Count - 1
                oXml = New XmlDocument()
                oXml.Load(pConfFiles(i))

                ' XmlNode pXMLNode = oXml.FirstChild;

                confEn = New ConfigEntries()
                confEn.FullName = pConfFiles(i)
                confEn.Path = Path.GetDirectoryName(pConfFiles(i))
                confEn.FileName = Path.GetFileName(pConfFiles(i))
                If confEn.FileName.ToUpper() = "Loaded.config".ToUpper() Then
                    confEn.Loaded = True
                Else
                    confEn.Loaded = False
                End If
                oList = oXml.GetElementsByTagName("Name")
                If oList Is Nothing Then
                    confEn.Name = ""
                ElseIf oList.Count = 0 Then
                    confEn.Name = confEn.FileName
                Else
                    oNode = oList.Item(0)
                    confEn.Name = oNode.InnerText
                    oNode = Nothing
                End If
                If Not (includeLoaded = False AndAlso confEn.Loaded = True) Then
                    pConfFileNames.Add(confEn)
                End If

                oXml = Nothing
            Next



            Return pConfFileNames
        Catch ex As Exception
            MessageBox.Show("GetAllConfigFilesNames:  " + ex.Message)

            Return Nothing
        Finally

            pConfFiles = Nothing
            pConfFileNames = Nothing
            oXml = Nothing

            oList = Nothing
            confEn = Nothing
            oNode = Nothing
        End Try
    End Function
    Public Shared Function copyFileContents(ByVal SourceFile As String, ByVal TargetFile As String) As Boolean
        Try
            Using sr As New StreamReader(SourceFile)
                Dim line As [String] = sr.ReadToEnd()



                If File.Exists(TargetFile) Then
                    Using sw As New System.IO.StreamWriter(TargetFile)
                        sw.Write(line)

                    End Using
                Else
                    Dim dir As String = Path.GetDirectoryName(TargetFile)
                    If Directory.Exists(dir) = False Then
                        Directory.CreateDirectory(dir)
                    End If
                    'Directory.CreateDirectory()
                    Using sw As StreamWriter = File.CreateText(TargetFile)
                        sw.Write(line)
                    End Using
                End If
            End Using
            Return True
        Catch
            Return False
        End Try
    End Function
    Public Shared Function ChangeConfig(ByVal LoadedConfig As ConfigEntries, ByVal ConfigToLoad As ConfigEntries) As Boolean
        Try
            Dim SourceFile As String = Convert.ToString(ConfigToLoad.Path) & "\" & Convert.ToString(LoadedConfig.FileName)
            Dim SourceCopyFile As String = Convert.ToString(ConfigToLoad.Path) & "\" & Convert.ToString(LoadedConfig.Name) & ".config"
            Dim TargetFile As String = Convert.ToString(LoadedConfig.Path) & "\" & Convert.ToString(ConfigToLoad.FileName)

            If copyFileContents(SourceFile, SourceCopyFile) Then
                If copyFileContents(TargetFile, SourceFile) Then
                    'LoadedConfig.Name = ConfigToLoad.Name;
                    Return True
                Else
                    Return False
                End If
            Else

                Return False

                'File.Copy(LoadedConfig.FullName, LoadedConfig.Path + "\\" + LoadedConfig.Name + ".config", true);

                'File.Copy(ConfigToLoad.FullName, ConfigToLoad.Path + "\\" + "Config" + ".config", true);
                ' NotifyBase pNot = new NotifyBase();
                ' invokeEvent();


                'return true;
            End If
        Catch ex As Exception
            MessageBox.Show("ChangeConfig:  " + ex.Message)

            Return False
            'LoadedConfig = null;
            'ConfigToLoad = null;
        Finally
        End Try
    End Function
    Public Shared Function GetConfigFile() As String
        Try
            Dim pathToUserProf As String = generateUserCachePath()
            If System.IO.Directory.Exists(pathToUserProf) = False Then
                System.IO.Directory.CreateDirectory(pathToUserProf)
            End If

            If pathToUserProf <> "" Then
                If File.Exists(Path.Combine(pathToUserProf, "Loaded.config")) Then


                    Return Path.Combine(pathToUserProf, "Loaded.config")
                Else
                    Return getInstalledConfig(pathToUserProf)

                End If
            Else
                Return ""

            End If
        Catch ex As Exception
            MessageBox.Show("GetConfigFile:  " + ex.Message)
            Return ""
        End Try

    End Function
    Public Shared Function GetBookmarkFile() As String
        Try
            Dim pathToEXE As String = getEXEPath()
            If System.IO.Directory.Exists(pathToEXE) = False Then
                Return ""
            End If

            If pathToEXE <> "" Then
                pathToEXE = Path.Combine(pathToEXE, "Bookmarks.xml")
                If File.Exists(pathToEXE) Then


                    Return pathToEXE
                Else

                    Using sw As StreamWriter = File.CreateText(pathToEXE)
                        Dim Bookmarks As Bookmarks = New Bookmarks

                        Dim stringTest As String = GlobalsFunctions.SerializeObject(Bookmarks, GetType(Bookmarks))


                        sw.Write(stringTest)

                        sw.Close()
                    End Using


                    Return pathToEXE

                End If
            Else
                Return ""

            End If
        Catch ex As Exception
            MessageBox.Show("GetBookmarkFile:  " + ex.Message)
            Return ""
        End Try

    End Function
    Public Shared Function getInstalledConfig(ByVal pathToUserProf As String) As String
        Try
            Dim pConfigFiles As String = ""

            Dim AppPath As String = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)
            If AppPath.IndexOf("file:\") >= 0 Then
                AppPath = AppPath.Replace("file:\", "")
            End If



            If File.Exists(Path.Combine(AppPath, "Loaded.config")) Then
                pConfigFiles = Path.Combine(AppPath, "Loaded.config")
                If File.Exists(Path.Combine(pathToUserProf, "Loaded.config")) = False Then

                    '   System.IO.File.Copy(pConfigFiles, Path.Combine(pathToUserProf, "Loaded.config"));
                    copyFileContents(pConfigFiles, Path.Combine(pathToUserProf, "Loaded.config"))
                End If
                pConfigFiles = Path.Combine(pathToUserProf, "Loaded.config")
            ElseIf File.Exists(Path.Combine(AppPath, "Config.config")) Then
                pConfigFiles = Path.Combine(AppPath, "Config.config")
                If File.Exists(Path.Combine(pathToUserProf, "Loaded.config")) = False Then

                    '   System.IO.File.Copy(pConfigFiles, Path.Combine(pathToUserProf, "Loaded.config"));
                    copyFileContents(pConfigFiles, Path.Combine(pathToUserProf, "Loaded.config"))
                End If
                pConfigFiles = Path.Combine(pathToUserProf, "Loaded.config")
            End If
            Return pConfigFiles
        Catch ex As Exception
            MessageBox.Show("getInstalledConfig:  " + ex.Message)
            Return ""
        End Try
    End Function
    Public Shared Function getEXEPath() As String
        Try

            Dim AppPath As String = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)
            If AppPath.IndexOf("file:\") >= 0 Then
                AppPath = AppPath.Replace("file:\", "")
            End If
            Return AppPath
        Catch ex As Exception
            MessageBox.Show("getEXEPath:  " + ex.Message)
            Return ""
        End Try
    End Function
    Public Shared Function GetConfigValue(ByVal keyname As String, ByVal defaultValue As Double) As Double
        Dim oXml As XmlDocument = Nothing
        Dim oList As XmlNodeList = Nothing
        Try
            Dim pConfigFiles As String = GetConfigFile()
            Dim keyvalue As String = ""



            If File.Exists(pConfigFiles) Then
                oXml = New XmlDocument()
                oXml.Load(pConfigFiles)

                oList = oXml.GetElementsByTagName("appSettings")
                If oList Is Nothing Then
                    Return defaultValue
                End If

                For Each oNode As XmlNode In oList
                    For Each oKey As XmlNode In oNode.ChildNodes
                        If (oKey IsNot Nothing) AndAlso (oKey.Attributes IsNot Nothing) Then
                            If oKey.Attributes("key").Value.Equals(keyname) Then
                                If oKey.Attributes("value").Value.Trim().Length > 0 Then
                                    keyvalue = oKey.Attributes("value").Value

                                    'Try to convert to double
                                    If GlobalsFunctions.IsDouble(keyvalue) Then
                                        Return (Convert.ToDouble(keyvalue))
                                    End If
                                End If
                            End If
                        End If
                    Next
                Next

                oXml = Nothing
            End If


            Return defaultValue
        Catch ex As Exception
            System.Windows.Forms.MessageBox.Show(ex.Message + vbLf & "Typically an error here is from an improperly formatted config file. " & vbLf & "The structure(XML) is compermised by a change you made.")
            Return defaultValue
        Finally
            oXml = Nothing
            oList = Nothing
        End Try
    End Function
    Public Shared Function GetConfigValue(ByVal keyname As String, ByVal defaultValue As Integer) As Integer
        Dim oXml As XmlDocument = Nothing
        Dim oList As XmlNodeList = Nothing
        Try
            Dim pConfigFiles As String = GetConfigFile()
            Dim keyvalue As String = ""

            If File.Exists(pConfigFiles) Then
                'NameValueCollection AppSettings = new NameValueCollection();
                oXml = New XmlDocument()

                oXml.Load(pConfigFiles)

                oList = oXml.GetElementsByTagName("appSettings")
                If oList Is Nothing Then
                    Return defaultValue
                End If

                'AppSettings = new NameValueCollection();
                For Each oNode As XmlNode In oList
                    For Each oKey As XmlNode In oNode.ChildNodes
                        If (oKey IsNot Nothing) AndAlso (oKey.Attributes IsNot Nothing) Then
                            If oKey.Attributes("key").Value.Equals(keyname) Then
                                If oKey.Attributes("value").Value.Trim().Length > 0 Then
                                    keyvalue = oKey.Attributes("value").Value

                                    'Try to convert to integer32
                                    If GlobalsFunctions.IsInteger(keyvalue) Then
                                        Return (Convert.ToInt32(keyvalue))
                                    End If
                                End If
                            End If
                        End If
                    Next
                Next

                oXml = Nothing
            End If

            Return defaultValue
        Catch ex As Exception
            System.Windows.Forms.MessageBox.Show(ex.Message + vbLf & "Typically an error here is from an improperly formatted config file. " & vbLf & "The structure(XML) is compermised by a change you made.")
            Return defaultValue
        Finally
            oXml = Nothing
            oList = Nothing
        End Try
    End Function
    Public Shared Function GetConfigValue(ByVal keyname As String, ByVal defaultValue As Boolean) As Boolean
        Dim oXml As XmlDocument = Nothing
        Dim oList As XmlNodeList = Nothing
        Try
            Dim pConfigFile As String = GetConfigFile()
            Dim keyvalue As String = ""
            If File.Exists(pConfigFile) Then
                'NameValueCollection AppSettings = new NameValueCollection();
                oXml = New XmlDocument()

                oXml.Load(pConfigFile)

                oList = oXml.GetElementsByTagName("appSettings")
                If oList Is Nothing Then
                    Return defaultValue
                End If

                'AppSettings = new NameValueCollection();
                For Each oNode As XmlNode In oList
                    For Each oKey As XmlNode In oNode.ChildNodes
                        If (oKey IsNot Nothing) AndAlso (oKey.Attributes IsNot Nothing) Then
                            If oKey.Attributes("key").Value.Equals(keyname) Then
                                If oKey.Attributes("value").Value.Trim().Length > 0 Then
                                    keyvalue = oKey.Attributes("value").Value

                                    'Try to convert to integer32
                                    If GlobalsFunctions.IsBoolean(keyvalue) Then
                                        Return (Convert.ToBoolean(keyvalue))
                                    End If
                                End If
                            End If
                        End If
                    Next
                Next
                oXml = Nothing
            End If

            Return defaultValue
        Catch ex As Exception
            System.Windows.Forms.MessageBox.Show(ex.Message + vbLf & "Typically an error here is from an improperly formatted config file. " & vbLf & "The structure(XML) is compermised by a change you made.")
            Return defaultValue
        Finally
            oXml = Nothing
            oList = Nothing
        End Try
    End Function
    Public Shared Function compareConfigValue(ByVal keyname As String, ByVal value As String) As Boolean
        Try
            If String.Compare(GetConfigValue(keyname), value) = 0 Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            MessageBox.Show("compareConfigValue:  " + ex.Message)
            Return False
        End Try
    End Function
    Public Shared Function GetConfigValue(ByVal keyname As String) As String
        Dim oXml As XmlDocument = Nothing
        Dim oList As XmlNodeList = Nothing
        Try
            Dim pConfigFile As String = GetConfigFile()
            Dim keyvalue As String = ""

            If File.Exists(pConfigFile) Then
                'NameValueCollection AppSettings = new NameValueCollection();
                oXml = New XmlDocument()

                oXml.Load(pConfigFile)

                oList = oXml.GetElementsByTagName("appSettings")
                If oList Is Nothing Then
                    Return ""
                End If

                'AppSettings = new NameValueCollection();
                For Each oNode As XmlNode In oList
                    For Each oKey As XmlNode In oNode.ChildNodes
                        If (oKey IsNot Nothing) AndAlso (oKey.Attributes IsNot Nothing) Then
                            If oKey.Attributes("key").Value.Equals(keyname) Then
                                If oKey.Attributes("value").Value.Trim().Length > 0 Then
                                    keyvalue = oKey.Attributes("value").Value

                                    'Try to convert to integer32
                                    Return keyvalue
                                End If
                            End If
                        End If
                    Next
                Next
                oXml = Nothing
            End If


            Return ""
        Catch ex As Exception
            System.Windows.Forms.MessageBox.Show(ex.Message + vbLf & "Typically an error here is from an improperly formatted config file. " & vbLf & "The structure(XML) is compermised by a change you made.")
            Return ""
        Finally
            oXml = Nothing
            oList = Nothing
        End Try
    End Function
    Public Shared Function GetConfigValue(ByVal keyname As String, ByVal defaultValue As String) As String

        Try
            Dim strConfigVal As String = GetConfigValue(keyname)
            If strConfigVal = "" Then
                Return defaultValue
            Else
                Return strConfigVal
            End If
        Catch ex As Exception
            System.Windows.Forms.MessageBox.Show("GetConfigValue:" + ex.Message)
            Return defaultValue
        End Try
    End Function
    Private Shared Function KeyExists(ByVal xmlDoc As XmlDocument, ByVal strKey As String) As Boolean
        Dim appSettingsNode As XmlNode = Nothing
        Try
            appSettingsNode = xmlDoc.SelectSingleNode("configuration/appSettings")

            ' Attempt to locate the requested setting.
            For Each childNode As XmlNode In appSettingsNode
                If childNode IsNot Nothing Then
                    If childNode.NodeType = XmlNodeType.Element Then
                        If childNode.Attributes.Count > 0 Then
                            If childNode.Attributes("key") IsNot Nothing Then
                                If childNode.Attributes("key").Value IsNot Nothing Then
                                    If childNode.Attributes("key").Value = strKey Then
                                        Return True
                                    End If
                                End If
                            End If
                        End If
                    End If

                End If
            Next
            Return False
        Catch ex As Exception
            MessageBox.Show("KeyExists:  " + ex.Message)
            Return False
        Finally
            appSettingsNode = Nothing
        End Try
    End Function
    Private Shared Function getConfigAsXMLDoc() As XmlDocument
        Dim xmld As XmlDocument = Nothing
        Try
            'Create the XML Document
            xmld = New XmlDocument()

            'Load the Xml file
            'xmld.Load(GlobalsFunctions.getFileAtDLLLocation("LayerViewerConfig.xml"));
            Try
                Dim confFiles As String = GetConfigFile()
                If confFiles IsNot Nothing Then
                    If confFiles.Trim() <> "" Then
                        ' xmld.Load(GlobalsFunctions.getFileAtDLLLocation("ESRI.WaterUtilitiesTemplate.DesktopFunctions.config"));
                        xmld.Load(confFiles)

                        Return xmld


                    End If
                End If

                Return Nothing
            Catch ex As Exception
                System.Windows.Forms.MessageBox.Show(ex.Message + vbLf & "Typically an error here is from an improperly formatted config file. " & vbLf & "The structure(XML) is compermised by a change you made.")
                Return Nothing
            End Try
        Catch ex As Exception

            MessageBox.Show("getConfigAsXMLDoc:  " + ex.Message)

            Return Nothing



        Finally
        End Try
    End Function
    Public Shared Function generateCachePath() As String


        Try
            Return Path.Combine(getEXEPath, "Cache")


        Catch ex As Exception
            Return Nothing
        End Try


    End Function
    'Public Shared Function FindConfigKey(ByVal skeyname As String) As String


    '    Try
    '        Dim AppPath As String = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)
    '        If AppPath.IndexOf("file:\") >= 0 Then
    '            AppPath = AppPath.Replace("file:\", "")
    '        End If


    '        Dim pConfigFiles(3) As String
    '        pConfigFiles(0) = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile
    '        pConfigFiles(1) = Path.Combine(AppPath, "app.config")
    '        pConfigFiles(2) = Path.Combine(AppPath, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name & ".Config")
    '        pConfigFiles(3) = Path.Combine(AppPath, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name & ".dll.config")

    '        Dim skeyvalue As String = ""

    '        For Each configFile As String In pConfigFiles
    '            If File.Exists(configFile) Then

    '                Dim AppSettings As NameValueCollection

    '                Dim oXml As XmlDocument = New XmlDocument()

    '                oXml.Load(configFile)

    '                Dim oList As XmlNodeList = oXml.GetElementsByTagName("appSettings")
    '                If oList Is Nothing Then Return Nothing

    '                AppSettings = New NameValueCollection()
    '                For Each oNode As XmlNode In oList
    '                    For Each oKey As XmlNode In oNode.ChildNodes
    '                        If oKey IsNot Nothing Then

    '                            If oKey.Attributes IsNot Nothing Then


    '                                If (oKey.Attributes("key").Value.Equals(skeyname)) Then


    '                                    If (oKey.Attributes("value").Value.Trim().Length > 0) Then

    '                                        skeyvalue = oKey.Attributes("value").Value
    '                                        Return skeyvalue
    '                                    End If
    '                                End If
    '                            End If

    '                        End If

    '                    Next
    '                Next
    '            End If

    '        Next

    '        Return Nothing

    '    Catch ex As Exception
    '        Return Nothing
    '    End Try
    'End Function
    Public Shared Function writeConfigKey(ByVal skeyname As String, ByVal skeyvalue As String) As Boolean

        Try
            Dim AppPath As String = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)
            If AppPath.IndexOf("file:\") >= 0 Then
                AppPath = AppPath.Replace("file:\", "")
            End If
            Dim pConfigFiles(3) As String
            pConfigFiles(0) = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile
            pConfigFiles(1) = Path.Combine(AppPath, "app.config")
            pConfigFiles(2) = Path.Combine(AppPath, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name & ".Config")
            pConfigFiles(3) = Path.Combine(AppPath, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name & ".dll.config")

            For Each configFile As String In pConfigFiles
                If File.Exists(configFile) Then
                    Dim AppSettings As NameValueCollection

                    Dim oXml As XmlDocument = New XmlDocument()
                    oXml.Load(configFile)

                    If KeyExists(oXml, skeyname) Then

                        Dim oList As XmlNodeList = oXml.GetElementsByTagName("appSettings")
                        AppSettings = New NameValueCollection()

                        For Each oNode As XmlNode In oList
                            For Each oKey As XmlNode In oNode.ChildNodes
                                If oKey IsNot Nothing Then If oKey.Attributes IsNot Nothing Then If (oKey.Attributes("key").Value.Equals(skeyname)) Then oKey.Attributes("value").Value = skeyvalue
                            Next
                        Next
                        oList = Nothing

                    Else
                        Dim Node As Xml.XmlElement
                        Dim Root As Xml.XmlNode = oXml.DocumentElement.SelectSingleNode("/configuration/appSettings")
                        Node = oXml.CreateElement("add")
                        Node.SetAttribute("key", skeyname)
                        Node.SetAttribute("value", skeyvalue)

                        'add the new child node (this key)
                        If Not Root Is Nothing Then
                            Root.AppendChild(Node)
                        End If
                        Node = Nothing
                        Root = Nothing

                    End If



                    oXml.Save(configFile)
                    oXml = Nothing
                End If

            Next

        Catch

            Return False
        End Try

    End Function
    Public Shared Function IsNumeric(ByVal PossibleNumber As String) As Boolean
        Return Information.IsNumeric(PossibleNumber)
        '''/@"^\d+$"
        'Regex objNotWholePattern = new Regex(@"^\d+$");//"[^0-9]");
        'return !objNotWholePattern.IsMatch(PossibleNumber)
        '     && (PossibleNumber != "");
    End Function
    Public Shared Function IsDouble(ByVal theValue As String) As Boolean
        Try
            Convert.ToDouble(theValue)
            Return True
        Catch
            Return False
        End Try
    End Function
    'IsDecimal
    Public Shared Function IsInteger(ByVal theValue As String) As Boolean
        Try
            Convert.ToInt32(theValue)
            Return True
        Catch
            Return False
        End Try
    End Function
    'IsInteger
    Public Shared Function IsBoolean(ByVal theValue As String) As Boolean
        Try
            Convert.ToBoolean(theValue)
            Return True
        Catch
            Return False
        End Try
    End Function
    'IsBoolean
    Public Shared Function IsOdd(ByVal value As Integer) As Boolean
        Return value Mod 2 <> 0
    End Function
    Public Shared Function IsOdd(ByVal value As Double) As Boolean
        Return Convert.ToInt32(value) Mod 2 <> 0
    End Function

    Public Enum EvenOdd
        Even
        Odd
    End Enum

    Public Shared Function RoundToEvenOdd(ByVal evenOdd__1 As EvenOdd, ByVal value As Double) As Double
        If evenOdd__1 = EvenOdd.Odd Then
            Dim rdNum As Double = Convert.ToInt32(Math.Round(value, 0))
            If GlobalsFunctions.IsOdd(rdNum) Then
                Return rdNum
            Else
                If rdNum > value Then

                    Return rdNum - 1
                Else
                    Return rdNum + 1
                End If




            End If
        Else

            Dim rdNum As Double = Convert.ToInt32(Math.Round(value, 0))
            If GlobalsFunctions.IsOdd(rdNum) = False Then
                Return rdNum
            Else
                If rdNum > value Then

                    Return rdNum - 1
                Else
                    Return rdNum + 1
                End If


            End If
        End If
    End Function
    'Public Shared Function ConvertPixelsToMap(ByVal pixelUnits As Double, ByVal pMap As IMap) As Double
    '    ' convert pixels to map coordinates
    '    Dim realWorldDisplayExtent As Double
    '    Dim pixelExtent As Long
    '    Dim sizeOfOnePixel As Double
    '    Dim pDT As IDisplayTransformation = Nothing
    '    Dim deviceRECT As tagRECT
    '    Dim pEnv As IEnvelope = Nothing
    '    Dim pActiveView As IActiveView = Nothing

    '    Try
    '        ' Get the width of the display extents in Pixels
    '        ' and get the extent of the displayed data
    '        ' work out the size of one pixel and then return
    '        ''' the pixels units passed in mulitplied by that value
    '        pActiveView = DirectCast(pMap, IActiveView)
    '        pDT = pActiveView.ScreenDisplay.DisplayTransformation
    '        deviceRECT = pDT.get_DeviceFrame()
    '        pixelExtent = deviceRECT.right - deviceRECT.left
    '        pEnv = pDT.VisibleBounds

    '        realWorldDisplayExtent = pEnv.Width
    '        sizeOfOnePixel = realWorldDisplayExtent / pixelExtent
    '        Return pixelUnits * sizeOfOnePixel
    '    Catch
    '        Return 0
    '    Finally
    '        pDT = Nothing

    '        pEnv = Nothing
    '        pActiveView = Nothing
    '    End Try



    'End Function
    'Public Shared Function GetColor(ByVal r As Integer, ByVal g As Integer, ByVal b As Integer) As IRgbColor
    '    'Create color
    '    Dim color As IRgbColor
    '    color = New RgbColorClass()
    '    color.Red = r
    '    'TODO: UserConfig
    '    color.Green = g
    '    'TODO: UserConfig
    '    color.Blue = b
    '    'TODO: UserConfig
    '    Return color
    'End Function
    Public Enum LengthUnit
        Mile
        NauticalMile
        Kilometer
        Meter
    End Enum
    Private Const Miles2Kilometers As Double = 1.609344
    Private Const Miles2Meters As Double = 1609.344
    Private Const Miles2Nautical As Double = 0.8684

    ''' Converts degrees to Radians.
    Public Shared Function ToRadian(degree As [Double]) As Double
        Return (degree * Math.PI / 180.0)
    End Function

    ''' To degress from a radian value.
    Public Shared Function ToDegree(radian As [Double]) As Double
        Return (radian / Math.PI * 180.0)
    End Function

    ' convert dd.ddddd to dddmm.mmmm
    Public Shared Function d2dm(d As Double) As Double
        Dim degree As Integer = CInt(Math.Truncate(d))
        Dim minute As Double = (d - degree) * 60
        Dim dm As Double = degree * 100 + minute
        Return dm
    End Function

    'return distance in Kilometer, Meter, or NaticalMile
    Public Shared Function Distance(Longitude1 As Double, Latitude1 As Double, Longitude2 As Double, Latitude2 As Double, unit As LengthUnit) As Double
        Dim deltaLong = Longitude1 - Longitude2
        Dim distance__1 = Math.Sin(ToRadian(Latitude1)) * Math.Sin(ToRadian(Latitude2)) + Math.Cos(ToRadian(Latitude1)) * Math.Cos(ToRadian(Latitude2)) * Math.Cos(ToRadian(deltaLong))

        ' distance is in miles
        distance__1 = Math.Acos(distance__1)
        distance__1 = ToDegree(distance__1) * 60 * 1.1515

        If unit = LengthUnit.Kilometer Then
            distance__1 = distance__1 * Miles2Kilometers
        End If
        ' distance in km
        If unit = LengthUnit.Meter Then
            distance__1 = distance__1 * Miles2Meters
            ' distance in meter
        ElseIf unit = LengthUnit.NauticalMile Then
            distance__1 = distance__1 * Miles2Nautical
        End If
        ' distance in nautical mile
        Return (distance__1)
    End Function

    ''' Accepts two coordinates in degrees. double value in degrees.  From 0 to 360.
    Public Shared Function Bearing(Longitude1 As Double, Latitude1__1 As Double, Longitude2 As Double, Latitude2__2 As Double) As Double
        Dim latitude1__3 As Double = ToRadian(Latitude1__1)
        Dim latitude2__4 As Double = ToRadian(Latitude2__2)

        Dim deltaLong As Double = ToRadian((Longitude2 - Longitude1))

        Dim y As Double = Math.Sin(deltaLong) * Math.Cos(latitude2__4)
        Dim x As Double = Math.Cos(latitude1__3) * Math.Sin(latitude2__4) - Math.Sin(latitude1__3) * Math.Cos(latitude2__4) * Math.Cos(deltaLong)

        Return (ToDegree(Math.Atan2(y, x)) + 360) Mod 360
    End Function

End Class

























