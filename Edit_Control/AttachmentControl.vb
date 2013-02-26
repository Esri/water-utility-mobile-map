Imports System.Drawing
Imports System.Windows
Imports Esri.ArcGIS
Imports Esri.ArcGIS.Mobile
Imports System.IO
Imports Esri.ArcGIS.Mobile.Geometries
Imports Esri.ArcGIS.Mobile.FeatureCaching
Imports Esri.ArcGISTemplates

Imports System.Windows.Forms

Public Class AttachmentControl
    Public Event AttachmentSelected(filename As attFiles)
    Public Event DeleteAttachment(id As Integer)
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        lstAtt.DrawMode = DrawMode.OwnerDrawVariable

        lstAtt.Font = New System.Drawing.Font("Tahoma", 12.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0)


        lstAtt.HorizontalScrollbar = True
        lstAtt.ItemHeight = 50

        AddHandler lstAtt.MouseClick, AddressOf attlistclick
        AddHandler lstAtt.DrawItem, AddressOf attdraw
    End Sub
    Public ReadOnly Property ListBox As ListBox
        Get
            Return lstAtt

        End Get

    End Property
    Private Sub attlistclick(sender As Object, e As MouseEventArgs)
        'MsgBox(CType(CType(sender, ListBox).SelectedItem, Attachment).FilePath)
        ' Process.Start(CType(CType(sender, ListBox).SelectedItem, Attachment).FilePath)
    End Sub

    Private Sub attdraw(sender As Object, e As System.Windows.Forms.DrawItemEventArgs)

        ' Draw the background of the ListBox control for each item.
        e.DrawBackground()
        ' Define the default color of the brush as black.
        Dim myBrush As Brush = Brushes.Black



        ' Draw the current item text based on the current Font  
        ' and the custom brush settings.

        If TypeOf (sender.Items(e.Index)) Is attFiles Then
            myBrush = Brushes.Red
            e.Graphics.DrawString(CType(sender.Items(e.Index), attFiles).fileName.ToString(), e.Font, myBrush, e.Bounds, StringFormat.GenericDefault)

        Else
            myBrush = Brushes.Blue
            e.Graphics.DrawString(CType(sender.Items(e.Index), Attachment).Name.ToString(), e.Font, myBrush, e.Bounds, StringFormat.GenericDefault)

        End If

        'e.Graphics.DrawString(sender.Items(e.Index).ToString(), e.Font, myBrush, e.Bounds, StringFormat.GenericDefault)
        ' If the ListBox has focus, draw a focus rectangle around the selected item.
        e.DrawFocusRectangle()

    End Sub

    Private Sub SplitContainer1_Panel2_Paint(sender As System.Object, e As System.Windows.Forms.PaintEventArgs) Handles splitMainAtt.Panel2.Paint

    End Sub


    Private Sub btnAdd_click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Try
            'Opens a dialog to browse out for an image
            Dim openFileDialog1 As System.Windows.Forms.OpenFileDialog

            openFileDialog1 = New System.Windows.Forms.OpenFileDialog()



            'Filter the image types
            ' openFileDialog1.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF"
            'If the user selects an image
            If openFileDialog1.ShowDialog() = DialogResult.OK Then
                'Set the path of the image to the text box
                Dim atfl As attFiles = New attFiles
                atfl.filePath = openFileDialog1.FileName
                atfl.fileName = Path.GetFileName(openFileDialog1.FileName)


                RaiseEvent AttachmentSelected(atfl)

                lstAtt.Items.Add(atfl)

            End If

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try
    End Sub

    Private Sub btnRemove_Click(sender As System.Object, e As System.EventArgs) Handles btnRemove.Click
        Try

            If TypeOf lstAtt.SelectedItem Is Attachment Then
                RaiseEvent DeleteAttachment(CType(lstAtt.SelectedItem, Attachment).Id)

                lstAtt.Items.Remove(lstAtt.SelectedItem)



            Else
                lstAtt.Items.Remove(lstAtt.SelectedItem)
            End If


        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try
    End Sub

    Private Sub AttachmentControl_Resize(sender As Object, e As System.EventArgs) Handles Me.Resize
        Try


            Dim loc As Integer = Me.Width / 5

            btnAdd.Left = loc - (btnAdd.Width / 2)
            btnAttCam.Left = (loc * 2)
            btnRemove.Left = (loc * 4) - (btnRemove.Width / 2)
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try
    End Sub

    Private Sub btnRemove_SizeChanged(sender As System.Object, e As System.EventArgs) Handles btnRemove.SizeChanged

    End Sub

    Private Sub lstAtt_DoubleClick(sender As System.Object, e As System.EventArgs) Handles lstAtt.DoubleClick
        If TypeOf (CType(sender, ListBox).SelectedItem) Is Attachment Then
            Process.Start(CType(CType(sender, ListBox).SelectedItem, Attachment).FilePath)
        Else
            Process.Start(CType(CType(sender, ListBox).SelectedItem, attFiles).filePath)
        End If

    End Sub

    Private Sub btnAttCam_Click(sender As System.Object, e As System.EventArgs) Handles btnAttCam.Click

    End Sub
End Class
