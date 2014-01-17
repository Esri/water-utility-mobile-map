' | Version 10.2
' | Copyright 2014 Esri
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
Public Class DrawablePolyLine
    Inherits Drawable
    Private m_Font As Font = New Font("Veranda", 10, FontStyle.Bold)
    Private m_dblDistance As Double = 0
    Private m_Angle As Double = 0
    ' Constructors.
    Public Sub New()
    End Sub
    Public Sub New(ByVal fore_color As Color, Optional ByVal line_width As Integer = 0, Optional ByVal new_x1 As Integer = 0, Optional ByVal new_y1 As Integer = 0, Optional ByVal new_x2 As Integer = 1, Optional ByVal new_y2 As Integer = 1, Optional ByVal Label As Boolean = True, Optional ByVal Block_Size As Integer = 25, Optional ByVal Feet_Per_Block As Integer = 10, Optional ByVal ParentWidth As Integer = 0, Optional ByVal ParentHeight As Integer = 0)
        MyBase.New(fore_color, Nothing, line_width, Label, Block_Size, Feet_Per_Block)

        X1 = new_x1
        Y1 = new_y1
        X2 = new_x2
        Y2 = new_y2
    End Sub

    ' Draw the object on this Graphics surface.
    ' Draw the object on this Graphics surface.
    Public Overrides Sub Draw(ByVal gr As System.Drawing.Graphics)
        If IsSelected Then
            ' Draw the line highlighted.
            Dim highlight_pen As New Pen(Color.Yellow, LineWidth)
            gr.DrawLine(highlight_pen, X1, Y1, X2, Y2)
            highlight_pen.Dispose()

            If Label Then

                Dim string_format As New StringFormat
                string_format.Alignment = StringAlignment.Center
                string_format.LineAlignment = StringAlignment.Far
                '  gr.TextRenderingHint = Text.TextRenderingHint.AntiAliasGridFit
                If m_Angle <> 0 Then



                    gr.RotateTransform(CSng(m_Angle))
                    gr.TranslateTransform(CSng((X1 + X2) / 2), CSng((Y1 + Y2) / 2), Drawing2D.MatrixOrder.Append)
                    gr.DrawString(CStr(m_dblDistance), m_Font, New SolidBrush(ForeColor), 0, 0, string_format)
                Else
                    gr.DrawString(CStr(m_dblDistance), m_Font, New SolidBrush(ForeColor), CSng((X1 + X2) / 2), CSng((Y1 + Y2) / 2), string_format)

                End If
                gr.ResetTransform()

            End If
        ElseIf IsDraw = False Then
        Else
            ' Just draw the line.
            Dim fg_pen As New Pen(ForeColor, LineWidth)
            gr.DrawLine(fg_pen, X1, Y1, X2, Y2)
            If Label Then

                Dim string_format As New StringFormat
                string_format.Alignment = StringAlignment.Center
                string_format.LineAlignment = StringAlignment.Far
                '  gr.TextRenderingHint = Text.TextRenderingHint.AntiAliasGridFit
                If m_Angle <> 0 Then



                    gr.RotateTransform(CSng(m_Angle))
                    gr.TranslateTransform(CSng((X1 + X2) / 2), CSng((Y1 + Y2) / 2), Drawing2D.MatrixOrder.Append)
                    gr.DrawString(CStr(m_dblDistance), m_Font, New SolidBrush(ForeColor), 0, 0, string_format)
                Else
                    gr.DrawString(CStr(m_dblDistance), m_Font, New SolidBrush(ForeColor), CSng((X1 + X2) / 2), CSng((Y1 + Y2) / 2), string_format)

                End If
                gr.ResetTransform()

            End If


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
        Return PointNearSegment(x, y, X1, Y1, X2, Y2)
    End Function

    ' Move the second point.
    Public Overrides Sub NewPoint(ByVal x As Integer, ByVal y As Integer)
        X2 = x
        Y2 = y
        m_dblDistance = Math.Sqrt((Math.Abs(X1 - X2) * Abs(X1 - X2)) + (Math.Abs(Y1 - Y2) * Math.Abs(Y1 - Y2)))
        m_dblDistance = CDbl(FormatNumber(m_dblDistance, 0, TriState.UseDefault, TriState.False, TriState.False))

        If BlockSize = 0.0 Then Return
        m_dblDistance = m_dblDistance / BlockSize
        m_dblDistance = m_dblDistance * FeetPerBlock

        Dim dblSlope As Double
        Dim radians As Double
        If (X1 - X2) = 0 Then

            m_Angle = 90
        ElseIf (Y1 - Y2) = 0 Then
            dblSlope = 0

            radians = Math.Atan(dblSlope)
            m_Angle = radians * (180 / Math.PI)
        Else
            dblSlope = (Y1 - Y2) / (X1 - X2)
            radians = Math.Atan(dblSlope)
            m_Angle = radians * (180 / Math.PI)
        End If


    End Sub

    ' Return True if the object is empty (e.g. a zero-length line).
    Public Overrides Function IsEmpty() As Boolean
        Return (X1 = X2) AndAlso (Y1 = Y2)
    End Function
End Class
