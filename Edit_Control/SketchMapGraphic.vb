Imports ESRI.ArcGIS
Imports ESRI.ArcGIS.Mobile.FeatureCaching

Imports ESRI.ArcGIS.Mobile.WinForms
'Imports System.Windows.Media
Imports System.Drawing


Public Class SketchMapGraphic
    Inherits ESRI.ArcGIS.Mobile.WinForms.MapGraphic
    Private textToDisplay As String
    Private m_LineStyle As Drawing2D.DashStyle = Drawing2D.DashStyle.Solid
    Private m_LineArrow As Boolean = False
    Private m_isAnno As Boolean = False
    Private m_FillTrans As Integer = 0


    Public Property fillTrans As Integer
        Get
            Return m_FillTrans
        End Get
        Set(value As Integer)
            m_FillTrans = value

        End Set
    End Property

    Public Property Text As String
        Get
            Return textToDisplay
        End Get
        Set(value As String)
            textToDisplay = value

        End Set
    End Property

    Public Property LineArrow As Boolean
        Get
            Return m_LineArrow
        End Get
        Set(value As Boolean)
            m_LineArrow = value

        End Set
    End Property
    Public Property isAnno As Boolean
        Get
            Return m_isAnno
        End Get
        Set(value As Boolean)
            m_isAnno = value

        End Set
    End Property
    Public Property LineStyle As Drawing2D.DashStyle

        Get
            Return m_LineStyle
        End Get
        Set(value As Drawing2D.DashStyle)
            m_LineStyle = value


        End Set
    End Property
    Public Overrides Sub Draw(mapSurface As ESRI.ArcGIS.Mobile.MapSurface)

        If m_isAnno Then
            Dim sts As SimpleTextSymbol = Me.Symbol

            mapSurface.DrawText(textToDisplay, sts.Font, sts.TextColor, sts.TextColor, CType(Me.Geometry, Esri.ArcGIS.Mobile.Geometries.Point).Coordinate, Mobile.TextAlignment.MiddleLeft)

        ElseIf Me.Geometry.GeometryType = Mobile.Geometries.GeometryType.Point Then

            Dim sms As PictureMarkerSymbol = Me.Symbol
          
            Dim client As System.Drawing.Point = mapSurface.Map.ToClient(CType(Geometry, ESRI.ArcGIS.Mobile.Geometries.Point).Coordinate)
            client.X -= sms.Bitmap.Width / 2
            client.Y -= sms.Bitmap.Height / 2
            Dim rect As System.Drawing.Rectangle = New System.Drawing.Rectangle(client.X, client.Y, sms.Bitmap.Width, sms.Bitmap.Height)

            mapSurface.Graphics.DrawImage(sms.Bitmap, rect)

        ElseIf Me.Geometry.GeometryType = Mobile.Geometries.GeometryType.Polyline Then

            Dim ls As LineSymbol = Me.Symbol
            Dim pen As Pen = PenFromLineSymbol(ls)
            If m_LineArrow Then
                Dim bigArrow As Drawing2D.AdjustableArrowCap = New Drawing2D.AdjustableArrowCap(5, 5)
                pen.CustomEndCap = bigArrow

            End If

            pen.DashStyle = Me.m_LineStyle
            mapSurface.DrawPolyline(pen, CType(Me.Geometry, Esri.ArcGIS.Mobile.Geometries.Polyline))


        ElseIf Me.Geometry.GeometryType = Mobile.Geometries.GeometryType.Polygon Then

            Dim sfs As SimpleFillSymbol = Me.Symbol

            Dim pen As Pen = PenFromLineSymbol(sfs.LineColor, sfs.LineWidth)
            'If m_LineArrow Then
            '    Dim bigArrow As Drawing2D.AdjustableArrowCap = New Drawing2D.AdjustableArrowCap(5, 5)
            '    pen.CustomEndCap = bigArrow

            'End If

            pen.DashStyle = Me.m_LineStyle

            '            Dim pBrush As SolidBrush = New System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(sfs.FillColor.A, sfs.FillColor.R, sfs.FillColor.G, sfs.FillColor.B))
            Dim pBrush As SolidBrush = New System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(Me.m_FillTrans, sfs.FillColor.R, sfs.FillColor.G, sfs.FillColor.B))

            mapSurface.DrawPolygon(pen, pbrush, CType(Me.Geometry, Esri.ArcGIS.Mobile.Geometries.Polygon))





            Else
                MyBase.Draw(mapSurface)
            End If
    End Sub



    Public Shared Function PenFromLineSymbol(sym As LineSymbol) As System.Drawing.Pen
        Dim pen As System.Drawing.Pen
        pen = New Pen(New System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(sym.LineColor.A, sym.LineColor.R, sym.LineColor.G, sym.LineColor.B)))
        pen.Width = sym.LineWidth
        pen.DashStyle = Drawing2D.DashStyle.Solid


        '      Select Case sketchSymbol.LineStyle
        '          Case eLineStyles.Dash
        '              pen.DashStyle = Media.DashStyles.Dash
        '              Exit Select
        '          Case eLineStyles.DashDot
        '              pen.DashStyle = Media.DashStyles.DashDot
        '              Exit Select
        '          Case eLineStyles.DashDotDot
        '              pen.DashStyle = Media.DashStyles.DashDotDot
        '              Exit Select
        '          Case eLineStyles.Dot
        '              pen.DashStyle = Media.DashStyles.Dot
        '              Exit Select
        'Case eLineStyles.Solid, Else
        '              pen.DashStyle = Media.DashStyles.Solid
        '              Exit Select
        '      End Select

        Return pen
    End Function
    Public Shared Function PenFromLineSymbol(lineColor As Color, lineWidth As Integer) As System.Drawing.Pen
        Dim pen As System.Drawing.Pen
        pen = New Pen(New System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(lineColor.A, lineColor.R, lineColor.G, lineColor.B)))
        pen.Width = lineWidth
        pen.DashStyle = Drawing2D.DashStyle.Solid


        Return pen
    End Function

    'Public Shared Sub DrawLineArrow(ms As ESRI.ArcGIS.Mobile.MapSurface, lineSymbol As LineSymbol, from As System.Drawing.Point, [to] As System.Drawing.Point, fill As Boolean, angle As Double, _
    ' size As Double)
    '    Dim penBrush As System.Drawing.Brush = New System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(lineSymbol.LineColor.A, lineSymbol.LineColor.R, lineSymbol.LineColor.G, lineSymbol.LineColor.B))
    '    Dim fillBrush As System.Drawing.Brush = If((fill), penBrush, System.Drawing.Brushes.Transparent)

    '    Dim figure As PathFigure = CalculateArrow(from, [to], angle, size)
    '    Dim geo As New PathGeometry(New PathFigure() {figure})
    '    ms.DrawGeometry(New System.Drawing.Pen(penBrush, lineSymbol.LineWidth), fillBrush, 24, geo)
    'End Sub

    'Private Shared Function CalculateArrow(from As Point, [to] As System.Drawing.Point, angle As Double, size As Double) As PathFigure
    '    Dim matx As New Matrix()

    '    Dim vect As System.Windows.Vector = from - [to]
    '    vect.Normalize()
    '    vect *= size

    '    Dim pf As New PathFigure()
    '    pf.IsClosed = False

    '    matx.Rotate(angle)
    '    pf.StartPoint = [to] + (vect * matx)

    '    Dim segment1 As New LineSegment()
    '    segment1.Point = [to]
    '    pf.Segments.Add(segment1)

    '    Dim segment2 As New LineSegment()
    '    matx.Rotate(-(angle * 2))
    '    segment2.Point = [to] + (vect * matx)
    '    pf.Segments.Add(segment2)

    '    Return pf
    'End Function
End Class
