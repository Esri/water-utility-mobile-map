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


'*******************
'** Thanks to http://www.vb-helper.com/howto_net_drawing_framework.html
'** for the orignal class design and the basis of this tool
'***************
Imports System.Drawing

Module MiscGraphics

    ' Width of grab rectangles. Should be odd.
    Public GrabHandleWidth As Integer = 5
    Public GrabHandleHalfWidth As Integer = GrabHandleWidth \ 2

    Public Sub DrawGrabHandle(ByVal gr As System.Drawing.Graphics, ByVal x As Integer, ByVal y As Integer)
        ' Fill a white rectangle.
        gr.FillRectangle(Brushes.White, _
            x - GrabHandleHalfWidth, _
            y - GrabHandleHalfWidth, _
            GrabHandleWidth, _
            GrabHandleWidth)

        ' Outline it in black.
        gr.DrawRectangle(Pens.Black, _
            x - GrabHandleHalfWidth, _
            y - GrabHandleHalfWidth, _
            GrabHandleWidth, _
            GrabHandleWidth)
    End Sub
End Module
