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


'*******************
'** Thanks to http://www.vb-helper.com/howto_net_drawing_framework.html
'** for the orignal class design and the basis of this tool
'***************
Imports System.Math
Imports System.Xml.Serialization
Imports System.Drawing

<Serializable()> _
Public Class DrawableStar
    Inherits Drawable

    ' Constructors.
    Public Sub New()
    End Sub
    Public Sub New(ByVal fore_color As Color, ByVal fill_color As Color, Optional ByVal line_width As Integer = 0, Optional ByVal new_x1 As Integer = 0, Optional ByVal new_y1 As Integer = 0, Optional ByVal new_x2 As Integer = 1, Optional ByVal new_y2 As Integer = 1)
        MyBase.New(fore_color, fill_color, line_width)

        X1 = new_x1
        Y1 = new_y1
        X2 = new_x2
        Y2 = new_y2
    End Sub

    ' Draw the object on this Graphics surface.
    Public Overrides Sub Draw(ByVal gr As System.Drawing.Graphics)
        ' Make an array of points representing this star.
        Dim pts() As PointF = GetStarPoints()

        ' Fill the star as usual.
        Dim fill_brush As New SolidBrush(FillColor)
        gr.FillPolygon(fill_brush, pts)
        fill_brush.Dispose()

        ' See if we're selected.
        If IsSelected Then
            ' Draw the star highlighted.
            Dim highlight_pen As New Pen(Color.Yellow, LineWidth)
            gr.DrawPolygon(highlight_pen, pts)
            highlight_pen.Dispose()

            ' Draw grab handles.
            DrawGrabHandle(gr, X1, Y1)
            DrawGrabHandle(gr, X1, Y2)
            DrawGrabHandle(gr, X2, Y2)
            DrawGrabHandle(gr, X2, Y1)
        Else
            ' Just draw the star.
            Dim fg_pen As New Pen(ForeColor, LineWidth)
            gr.DrawPolygon(fg_pen, pts)
            fg_pen.Dispose()
        End If
    End Sub

    ' Return an array of points for the star.
    Private Function GetStarPoints() As PointF()
        ' Create a basic star of radius 1.
        Dim r1 As Double = 1
        Dim r2 As Double = 0.5
        Dim dt As Double = 2 * PI / 10
        Dim t As Double = -PI / 2
        Dim pts(9) As PointF
        For i As Integer = 0 To 9 Step 2
            pts(i).X = CSng(r1 * Cos(t))
            pts(i).Y = CSng(r1 * Sin(t))
            t += dt
            pts(i + 1).X = CSng(r2 * Cos(t))
            pts(i + 1).Y = CSng(r2 * Sin(t))
            t += dt
        Next i

        ' Transform to the bounding rectangle.
        Dim x_scale As Double = Abs(X2 - X1) / 2
        Dim y_scale As Double = Abs(Y2 - Y1) / 2
        Dim dx As Double = (X2 + X1) / 2
        Dim dy As Double = (Y2 + Y1) / 2
        For i As Integer = 0 To 9
            pts(i).X = CSng(pts(i).X * x_scale + dx)
            pts(i).Y = CSng(pts(i).Y * y_scale + dy)
        Next i

        Return pts
    End Function

    ' Return the object's bounding rectangle.
    Public Overrides Function GetBounds() As System.Drawing.Rectangle
        Return New Rectangle( _
            Min(X1, X2), _
            Min(Y1, Y2), _
            Abs(X2 - X1), _
            Abs(Y2 - Y1))
    End Function

    ' Return True if this point is on the object.
    Public Overrides Function IsAt(ByVal x As Integer, ByVal y As Integer) As Boolean
        Return PointNearPolygon(x, y, GetStarPoints())
    End Function

    ' Move the second point.
    Public Overrides Sub NewPoint(ByVal x As Integer, ByVal y As Integer)
        X2 = x
        Y2 = y
    End Sub

    ' Return True if the object is empty (e.g. a zero-length line).
    Public Overrides Function IsEmpty() As Boolean
        Return (X1 = X2) AndAlso (Y1 = Y2)
    End Function
End Class
