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
Public Class DrawableEllipse
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
        ' Make a Rectangle representing this ellipse.
        Dim rect As Rectangle = GetBounds()

        ' Fill the ellipse as usual.
        Dim fill_brush As New SolidBrush(FillColor)
        gr.FillEllipse(fill_brush, rect)
        fill_brush.Dispose()

        ' See if we're selected.
        If IsSelected Then
            ' Draw the ellipse highlighted.
            Dim highlight_pen As New Pen(Color.Yellow, LineWidth)
            gr.DrawEllipse(highlight_pen, rect)
            highlight_pen.Dispose()

            ' Draw grab handles.
            DrawGrabHandle(gr, X1, Y1)
            DrawGrabHandle(gr, X1, Y2)
            DrawGrabHandle(gr, X2, Y2)
            DrawGrabHandle(gr, X2, Y1)
        Else
            ' Just draw the ellipse.
            Dim fg_pen As New Pen(ForeColor, LineWidth)
            gr.DrawEllipse(fg_pen, rect)
            fg_pen.Dispose()
        End If
    End Sub

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
        Return PointNearEllipse(x, y, X1, Y1, X2, Y2)
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
