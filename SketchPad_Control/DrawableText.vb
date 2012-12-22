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


Imports System.Math
Imports System.Xml.Serialization
Imports System.Drawing

<Serializable()> _
Public Class DrawableText
    Inherits Drawable
    Private m_text As String
    Private m_Font As Font = New Font("Veranda", 10, FontStyle.Bold)

    ' Constructors.
    Public Sub New()
    End Sub
    Public Property Text() As String
        Get
            Return m_text
        End Get
        Set(ByVal value As String)
            m_text = value
        End Set
    End Property
    Public Sub New(ByVal fore_color As Color, ByVal text As String, Optional ByVal line_width As Integer = 0, Optional ByVal new_x1 As Integer = 0, Optional ByVal new_y1 As Integer = 0, Optional ByVal new_x2 As Integer = 1, Optional ByVal new_y2 As Integer = 1)
        MyBase.New(fore_color, Nothing, line_width)
        m_text = text

        X1 = new_x1
        Y1 = new_y1
        X2 = new_x2
        Y2 = new_y2
    End Sub

    ' Draw the object on this Graphics surface.
    Public Overrides Sub Draw(ByVal gr As System.Drawing.Graphics)
        If IsSelected Then
            ' Draw the line highlighted.
            Dim highlight_pen As New SolidBrush(Color.Red)
            gr.DrawString(m_text, m_Font, highlight_pen, X1, Y1)
            highlight_pen.Dispose()

            ' Draw grab handles.
            DrawGrabHandle(gr, X1, Y1)
            highlight_pen.Dispose()
        ElseIf IsDraw = False Then
        Else
            ' Just draw the line.
            Dim draw_pen As New SolidBrush(ForeColor)

            gr.DrawString(m_text, m_Font, draw_pen, X1, Y1)
            draw_pen.Dispose()
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
        Return PointNearSegment(x, y, X1, Y1, X2, Y2)
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
