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
Imports System.Xml.Serialization
Imports System.Xml
Imports System.IO
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Drawing.Image

Imports System.Windows.Forms

<Serializable()> _
Public Class DrawablePicture
    ' The list where we will store objects.
    <XmlElement(GetType(Drawable)), _
     XmlElement(GetType(DrawableLine)), _
     XmlElement(GetType(DrawableRectangle)), _
     XmlElement(GetType(DrawableEllipse)), _
    XmlElement(GetType(DrawableText)), _
    XmlElement(GetType(DrawablePolyLine)), _
    XmlElement(GetType(DrawablePolygonFreehand)), _
    XmlElement(GetType(DrawableStar))> _
    Public Drawables As New ArrayList

    ' The background color.
    <XmlIgnore()> Public BackColor As Color = SystemColors.Control
    ' Property procedures to serialize and
    ' deserialize BackColor.
    <XmlAttributeAttribute("BackColor")> _
    Public Property BackColorArgb() As Integer
        Get
            Return BackColor.ToArgb()
        End Get
        Set(ByVal Value As Integer)
            BackColor = Color.FromArgb(Value)
        End Set
    End Property
    Public Enum ImageType
        Jpg
        Bmp
        Gif
    End Enum
    ' Constructors.
    Public Sub New()
    End Sub
    Public Sub New(ByVal background_color As Color)
        BackColor = background_color
    End Sub

    ' The currently selected object. A more elaborate
    ' application might use a selection list and make
    ' this a collection.
    Private m_SelectedDrawable As Drawable
    Public Property SelectedDrawable() As Drawable
        Get
            Return m_SelectedDrawable
        End Get
        Set(ByVal Value As Drawable)
            ' Mark the currently selected object
            ' as not selected.
            If Not (m_SelectedDrawable Is Nothing) Then
                m_SelectedDrawable.IsSelected = False
            End If

            ' Select the new object.
            m_SelectedDrawable = Value
            If Not (m_SelectedDrawable Is Nothing) Then
                m_SelectedDrawable.IsSelected = True
            End If
        End Set
    End Property

    ' Add a new Drawable object to the list.
    Public Sub Add(ByVal new_drawable As Drawable)
        Drawables.Add(new_drawable)
    End Sub

    ' Remove this object from the list.
    Public Sub Remove(ByVal target As Drawable)
        Drawables.Remove(target)
    End Sub
    ' Remove this object from the list.
    Public Sub RemoveAll()
        Drawables.Clear()

    End Sub
    ' Select the Drawable at this point. Highlight it
    ' and return it.
    Public Function SelectObjectAt(ByVal x As Integer, ByVal y As Integer) As Drawable
        ' Deselect the previously selected object.
        SelectedDrawable = Nothing

        ' Find the object at this point (if any).
        ' Search the list backwards so we find objects
        ' at the top of the stack first.
        For i As Integer = Drawables.Count - 1 To 0 Step -1

            Dim dr As Drawable = DirectCast(Drawables(i), Drawable)



            If dr.IsAt(x, y) Then
                SelectedDrawable = dr
                Exit For
            End If
        Next i

        ' Return the selected object.
        Return SelectedDrawable
    End Function

    ' Draw all the objects.
    Public Sub Draw(ByVal gr As Graphics)
        ' Clear the background.
        'gr.Clear(BackColor)
        gr.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

        ' Draw the objects.
        For Each dr As Drawable In Drawables
            dr.Draw(gr)
        Next dr
    End Sub

    ' Send the object to the back of the stack.
    Public Sub SendToBack(ByVal dr As Drawable)
        If Not (dr Is Nothing) Then
            Drawables.Remove(dr)
            Drawables.Insert(0, dr)
        End If
    End Sub

    ' Bring the object to the front of the stack.
    Public Sub BringToFront(ByVal dr As Drawable)
        If Not (dr Is Nothing) Then
            Drawables.Remove(dr)
            Drawables.Insert(Drawables.Count, dr)
        End If
    End Sub

    ' Delete the object.
    Public Sub Delete(ByVal dr As Drawable)
        If Not (dr Is Nothing) Then
            Drawables.Remove(dr)
        End If
    End Sub

    ' Save the picture into the file.
    Public Sub SavePictureSerialize(ByVal file_name As String)
        Try
            Dim xml_serializer As New XmlSerializer(GetType(DrawablePicture))
            Dim stream_writer As New StreamWriter(file_name)
            xml_serializer.Serialize(stream_writer, Me)
            stream_writer.Close()
        Catch ex As Exception
            If MessageBox.Show(ex.Message & vbCrLf & _
                "Show internal error?", "Save Error", _
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                MessageBox.Show(ex.InnerException.ToString, "Internal Error", _
                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If
        End Try
    End Sub

    ' Laod the picture from the file.
    Public Shared Function LoadPictureSerialize(ByVal file_name As String) As DrawablePicture
        Try
            Dim xml_serializer As New XmlSerializer(GetType(DrawablePicture))
            Dim file_stream As New FileStream(file_name, FileMode.Open)
            Dim new_picture As DrawablePicture = _
                DirectCast(xml_serializer.Deserialize(file_stream), DrawablePicture)
            file_stream.Close()
            Return new_picture
        Catch ex As Exception
            If MessageBox.Show(ex.Message & vbCrLf & _
                "Show internal error?", "Save Error", _
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                MessageBox.Show(ex.InnerException.ToString, "Internal Error", _
                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If
            Return Nothing
        End Try
    End Function
    'Public Function CreateBitmapClip(ByVal width As Integer, ByVal Height As Integer) As Bitmap

    '    Dim b2 As Bitmap
    '    Dim g1 As Graphics


    '    b2 = New Bitmap(width, Height)

    '    g1 = Graphics.FromImage(b2)




    '    Me.Draw(g1)
    '    Return b2


    'End Function
    Public Function CreateBitmapClipByDrawables(ByVal backgroundImage As Image, ByVal clip As Boolean) As Bitmap

        If clip Then

            Dim pRect As Rectangle = getBoundingBox()
            Dim b2 As Bitmap
            Dim g1 As Graphics
            If pRect.Right = 0 And pRect.Bottom = 0 Then

                b2 = New Bitmap(backgroundImage)
                g1 = Graphics.FromImage(b2)
            Else
                Dim pWidth As Integer = pRect.Right + pRect.Right
                Dim pHeight As Integer = pRect.Bottom + pRect.Bottom
                b2 = New Bitmap(pWidth, pHeight)
                g1 = Graphics.FromImage(b2)
                g1.DrawImage(backgroundImage, 0, 0)
            End If



            Me.Draw(g1)
            Return b2
        Else
            Dim b2 As Bitmap = New Bitmap(backgroundImage)
            Dim g1 As Graphics = Graphics.FromImage(b2)

            Me.Draw(g1)
            Return b2

        End If



    End Function

    Public Sub SaveBitMapToFile(ByVal image As Bitmap, ByVal filename As String)
        '  image.SetResolution(384, 384)
        Dim ep As EncoderParameters = New EncoderParameters(1)
        'Set Quality Parameter
        ep.Param(0) = New EncoderParameter(Encoder.Quality, 80)
        Dim ici As ImageCodecInfo = Nothing
        Dim ext As String = System.IO.Path.GetExtension(filename)

        For Each codec As ImageCodecInfo In ImageCodecInfo.GetImageDecoders
            Select Case ext
                Case ".jpg"
                    If codec.MimeType = "image/jpeg" Then
                        ici = codec
                        Exit For
                    End If

                Case ".bmp"
                    If codec.MimeType = "image/bmp" Then
                        ici = codec
                        Exit For
                    End If
                Case ".gif"
                    If codec.MimeType = "image/gif" Then
                        ici = codec
                        Exit For
                    End If

            End Select
            
        Next

        image.Save(filename, ici, ep)

        'image.Save(filename, GetImageFormat(filename))
    End Sub
    Private Function GetImageFormat(ByVal filename As String) As ImageFormat

        Dim ext As String = System.IO.Path.GetExtension(filename)


        Dim imgFmt As ImageFormat = Nothing


        Select Case ext


            Case ".jpg"
                imgFmt = ImageFormat.Jpeg

                Exit Select
            Case ".gif"
                imgFmt = ImageFormat.Gif
                Exit Select
            Case ".bmp"
                imgFmt = ImageFormat.Jpeg
                Exit Select
            Case Else

                imgFmt = ImageFormat.Jpeg
                filename += ".jpg"
                Exit Select
        End Select

        Return imgFmt
    End Function
    Public Function getBoundingBox() As Rectangle
        Dim pXMax As Integer = -1
        Dim pYMax As Integer = -1

        Dim pRect As Rectangle = New Rectangle

        For Each pDraw As Drawable In Drawables
            pRect = Rectangle.Union(pRect, pDraw.GetBounds())

        Next
        pRect.Width = CInt(pRect.Width + (pRect.Width / 10))
        pRect.Height = CInt(pRect.Height + (pRect.Height / 10))
        Return pRect
    End Function
End Class
