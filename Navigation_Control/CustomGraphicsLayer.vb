
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Drawing

Imports ESRI.ArcGIS.Mobile
Imports ESRI.ArcGIS.Mobile.FeatureCaching
Imports ESRI.ArcGIS.Mobile.Geometries
Imports ESRI.ArcGIS.Mobile.WinForms

'Custom class for our own graphic layer. This class exposes
'several properties a user might be interested in changing or using
'when creating own graphic layer

Namespace CustomGraphicsLayer
    Structure gpsLoc
        Public _coordinate As Esri.ArcGIS.Mobile.Geometries.Coordinate
        Public _user As String
        Public _date As String
        Public _wo As String

    End Structure

    Class CustomGraphicLayer
        Inherits MapGraphicLayer

        Public Enum DrawType
            Points
            Line
            Area
        End Enum

        'define a symbol instances for point, line, and area
        Private _pointSymbol As Symbol
        Private _lineSymbol As Symbol
        Private _areaSymbol As Symbol

        'attributes of how to draw graphics layer
        Private _transparency As Short
        Private _color As Color
        Private _paintStyle As SimpleMarkerStyle
        Private _lineSize As Short
        Private _pointSize As Short
        Private _drawType As DrawType

        'define custom graphics coordinate collection
        Private _coordinateCollection As New CoordinateCollection()
        Private _locationsList As List(Of gpsLoc)
        'default constructor, sets default style
        Public Sub New()
            Me.New(50, Color.Green, SimpleMarkerStyle.Square, 3, 20, DrawType.Points)
        End Sub

        'constructor with style parameters
        Public Sub New(transparency As Short, c As Color, ps As SimpleMarkerStyle, lineSize As Short, pointSize As Short, drawType As DrawType)
            MyBase.New("CustomGraphicLayer")

            _transparency = transparency
            _color = c
            _paintStyle = ps
            _lineSize = lineSize
            _pointSize = pointSize
            _drawType = drawType
            _locationsList = New List(Of gpsLoc)
            setSymbolAttributes()
        End Sub
        Public Function count() As Integer
            Return _locationsList.Count

        End Function
        Public Sub addGPSLoc(coord As Coordinate, userName As String, dateTimeInfo As String, wo As String)
            If _coordinateCollection.Count > 10 Then
                _coordinateCollection.RemoveAt(0)
            End If
            _coordinateCollection.Add(coord)

            Dim gps As gpsLoc = New gpsLoc
            gps._coordinate = coord
            gps._date = dateTimeInfo
            gps._user = userName
            gps._wo = wo
            _locationsList.Add(gps)

        End Sub
        Public Function getGPSData() As List(Of gpsLoc)
            Return _locationsList
        End Function
        Public Sub clearGPS()
            _locationsList.Clear()
            _coordinateCollection.Clear()

        End Sub
        'This method is called implicitly by the display with which it is contained, 
        'This will more than likely be a map control
        Protected Overrides Sub Draw(display As MapSurface)
            'If the display is null or if drawing has been canceled, cancel the drawing
            'of the layer
            If Me.Visible = False Then Return

            If display Is Nothing Then
                Return
            End If

            If display.DrawingCanceled Then
                Return
            End If

            'based on what feature class user wanted to draw, call appropriate function
            'with the layers collection of coordinates
            Select Case _drawType
                Case DrawType.Points
                    _pointSymbol.DrawMultipoint(display, _coordinateCollection)
                    Exit Select
                Case DrawType.Line
                    _lineSymbol.DrawLine(display, _coordinateCollection)
                    Exit Select
                Case DrawType.Area
                    _areaSymbol.DrawArea(display, _coordinateCollection)
                    Exit Select
            End Select

        End Sub

        'Since MapLayer implements the IDisposable interface, we should
        'override the method here to deallocate any resources we don't anymore
        'when the graphic layer is removed
        Protected Overrides Sub Dispose(disposing As Boolean)
            Try
                If disposing Then
                    If _pointSymbol IsNot Nothing Then
                        _pointSymbol.Dispose()
                    End If

                    If _lineSymbol IsNot Nothing Then
                        _lineSymbol.Dispose()
                    End If

                    If _areaSymbol IsNot Nothing Then
                        _areaSymbol.Dispose()
                    End If


                    _pointSymbol = Nothing
                    _lineSymbol = Nothing
                    _areaSymbol = Nothing
                    _coordinateCollection = Nothing
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub


        'define get/set functionality for coordinates
        Public Property Coordinates() As CoordinateCollection
            Get
                Return _coordinateCollection
            End Get
            Set(value As CoordinateCollection)
                _coordinateCollection = value
            End Set
        End Property


        Public Property Transparency() As Short
            Get
                Return _transparency
            End Get
            Set(value As Short)
                _transparency = value
                setSymbolAttributes()
            End Set
        End Property

        Public Property ColorLayer() As Color
            Get
                Return _color
            End Get
            Set(value As Color)
                _color = value
                setSymbolAttributes()
            End Set
        End Property

        Public Property PaintStyle() As SimpleMarkerStyle
            Get
                Return _paintStyle
            End Get
            Set(value As SimpleMarkerStyle)
                _paintStyle = value
                setSymbolAttributes()
            End Set
        End Property

        Public Property LineSize() As Short
            Get
                Return _lineSize
            End Get
            Set(value As Short)
                _lineSize = value
                setSymbolAttributes()
            End Set
        End Property

        Public Property PointSize() As Short
            Get
                Return _pointSize
            End Get
            Set(value As Short)
                _pointSize = value
                setSymbolAttributes()
            End Set
        End Property

        'Creates a new Symbol object with the attributes defined by the set properties
        Private Sub setSymbolAttributes()
            If _pointSymbol IsNot Nothing Then
                _pointSymbol = Nothing
            End If

            If _lineSymbol IsNot Nothing Then
                _lineSymbol = Nothing
            End If

            '          SimpleMarkerSymbol vertex = new SimpleMarkerSymbol(Color.Cyan, 1, Color.Cyan, 6, SimpleMarkerStyle.Circle);
            'Painting each kind of geometry requires a different type of PaintOperation. Here
            'we illustrate this by instantiating three different types of symbols, each of 
            'which are rendered differently by a focused paint operation
            _pointSymbol = New SimpleMarkerSymbol(ColorLayer, LineSize, ColorLayer, PointSize, PaintStyle)

            '         new VertexPaintOperation(ColorLayer, LineSize, Transparency,
            '                                 ColorLayer, Transparency, PointSize, PaintStyle)); 

            _lineSymbol = New LineSymbol(ColorLayer, LineSize)


            'Symbol(
            '       new StrokePaintOperation(ColorLayer, LineSize, Transparency)); 

            _areaSymbol = New FillSymbol(ColorLayer)

            'Symbol(
            '       new FillPaintOperation(ColorLayer, Transparency));

        End Sub

    End Class
End Namespace
