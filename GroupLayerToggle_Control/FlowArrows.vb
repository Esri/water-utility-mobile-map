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
Imports System.Collections.Generic
Imports System.Text
Imports System.Drawing
Imports ESRI.ArcGIS.Mobile
Imports ESRI.ArcGIS.Mobile.Geometries
Imports Esri.ArcGIS.Mobile.FeatureCaching
Imports Esri.ArcGISTemplates

Public Class FlowArrowsLayer
    Inherits Esri.ArcGIS.Mobile.WinForms.MapGraphicLayer
    Private m_layerName As String

    Private m_fontColor As Color = Color.Black
    Private m_font As Font = New Font("Arial", 20.0F, FontStyle.Bold)
    Private m_sformat As StringFormat

    Private displayString As String = "«"


    Public Sub New(ByVal strLayerName As String)
        MyBase.New(strLayerName)
        'Initialize the graphics layer

        'Set the layer name
        m_layerName = strLayerName

        'Initialize the list to hold the results
        resetCreateCollection()


        m_sformat = New StringFormat(StringFormatFlags.NoClip)

        m_sformat.Alignment = StringAlignment.Center
        m_sformat.LineAlignment = StringAlignment.Center
    End Sub

#Region "Overrides"

    Protected Overrides Sub Draw(ByVal mapSurface As Esri.ArcGIS.Mobile.MapSurface)
        If Map Is Nothing Then Return


        If Me.Visible = False Then Return

        If (GlobalsFunctions.appConfig.LayerOptions.FlowArrowsLayers.FlowArrowsLayer.Count = 0) Then Return

        Dim pFsWD As FeatureSourceWithDef = Nothing
        Dim pLayDef As Esri.ArcGIS.Mobile.FeatureCaching.MobileCacheMapLayerDefinition = Nothing
        Dim msMapLayer As MobileCacheMapLayer


        'return if mapSurface is null
        If (mapSurface Is Nothing) Then Return

        'return if drawing is cancelled
        If (mapSurface.DrawingCanceled) Then Return
        'Lines


        Dim pCol() As String = New String() {}
        Dim pFDR As FeatureDataReader
        Dim pGeo As Geometry
        Dim pCoordColl As CoordinateCollection

        Dim g As Graphics

        Dim qFilt As QueryFilter = New QueryFilter()
        qFilt.Geometry = mapSurface.Map.Extent
        qFilt.GeometricRelationship = Geometries.GeometricRelationshipType.Intersect
        g = mapSurface.Graphics

        ' For Each pMobileCacheLayer as Esri.ArcGIS.Mobile.MapLayer In m_LabelLayerList
        For Each strLayName In GlobalsFunctions.appConfig.LayerOptions.FlowArrowsLayers.FlowArrowsLayer

            
            pFsWD = GlobalsFunctions.GetFeatureSource(strLayName.Name, Map)
            pLayDef = GlobalsFunctions.GetLayerDefinition(Map, pFsWD)


            If pFsWD.FeatureSource.GeometryType = GeometryType.Polyline Then


                If (pLayDef.MinScale = 0 And pLayDef.MaxScale = 0 And pLayDef.Visibility = True) Or _
                   (pLayDef.MinScale = 0 And Map.Scale > pLayDef.MaxScale And pLayDef.Visibility = True) Or _
                   (Map.Scale < pLayDef.MinScale And pLayDef.MaxScale = 0 And pLayDef.Visibility = True) Or _
                   (Map.Scale < pLayDef.MinScale And Map.Scale > pLayDef.MaxScale And pLayDef.Visibility = True) Then

                    Dim fCount As Integer = pFsWD.FeatureSource.GetFeatureCount(qFilt)
                    If fCount > 0 Then
                        pFDR = pFsWD.FeatureSource.GetDataReader(qFilt, pCol)
                        While pFDR.Read()

                            pGeo = CType(pFDR.Item(pFsWD.FeatureSource.GeometryColumnName), Geometry)
                            Dim pPolyLine As Polyline = CType(pGeo, Polyline)
                            For i As Integer = 0 To pPolyLine.PartCount - 1

                                pCoordColl = pPolyLine.GetPart(i)
                                Dim curPoint As System.Drawing.Point = Nothing
                                Dim lastPoint As System.Drawing.Point = Nothing
                                ' Dim curPoint As Coordinate = Nothing
                                ' Dim lastPoint As Coordinate = Nothing
                                Dim midPointX As Double = Nothing
                                Dim midPointY As Double = Nothing
                                Dim midPointXTri As Double = Nothing
                                Dim midPointYTri As Double = Nothing
                                Dim triSlope As Double = Nothing
                                Dim triX1 As Double = Nothing
                                Dim triX2 As Double = Nothing
                                Dim triY1 As Double = Nothing
                                Dim triY2 As Double = Nothing
                                Dim midPointSlope As Double = Nothing

                                For j As Integer = 0 To pCoordColl.Count - 1
                                    curPoint = mapSurface.Map.ToClient(pCoordColl.Item(j))

                                    If Not lastPoint = Nothing Then

                                        'Dim pPnt As System.Drawing.Point
                                        'pPnt = mapSurface.ToClient(curPoint)

                                        g.ResetTransform()

                                        midPointX = (curPoint.X + lastPoint.X) / 2
                                        midPointY = (curPoint.Y + lastPoint.Y) / 2

                                        g.TranslateTransform(CSng(midPointX), CSng(midPointY))
                                        midPointSlope = (curPoint.Y - lastPoint.Y) / (curPoint.X - lastPoint.X)
                                        Dim angle As Single
                                        Single.TryParse(Math.Atan2(lastPoint.Y - curPoint.Y, lastPoint.X - curPoint.X).ToString, angle)
                                        angle = CSng(RadiansToDegrees(angle))

                                        g.RotateTransform(angle) 'RadiansToDegrees(midPointSlope))
                                        '  g.DrawString("«", New Font("Arial", 20.0F, FontStyle.Bold), New SolidBrush(Color.Black), 0, 0)
                                        ' g.DrawString(mapSurfaceString, m_font, New SolidBrush(m_fontColor), g.MeasureString(mapSurfaceString, m_font).Height / 4, g.MeasureString(mapSurfaceString, m_font).Height / 4, m_sformat)
                                        g.DrawString(displayString, m_font, New SolidBrush(m_fontColor), 0, 0, m_sformat)

                                    End If

                                    lastPoint = curPoint
                                Next

                            Next
                        End While
                    End If

                End If
            End If
            
        Next

        pFDR = Nothing
        pGeo = Nothing
        pCoordColl = Nothing

        If g IsNot Nothing Then
            g.Dispose()

        End If
        g = Nothing

    End Sub
    Public Function RadiansToDegrees(ByVal radian As Double) As Double
        '  Return Math.Atan(radian)
        Return (180 / Math.PI) * radian
    End Function
    Public Function DegreesToRadians(ByVal degrees As Double) As Double

        Return (Math.PI / 180) * degrees

    End Function

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)

        'Dispose symbol implementing IDisposible
        Try

            If (disposing) Then



                'm_OtherPointCoordinateCollection = Nothing

            End If


        Finally

            MyBase.Dispose(disposing)

        End Try


    End Sub

#End Region



    Private Sub resetCreateCollection()
        Try



            'Points
            '   m_MeterCoordinateCollection = New List(Of TraceGraphics)
        Catch ex As Exception

        End Try
    End Sub


End Class
