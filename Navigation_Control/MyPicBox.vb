Imports System.Windows.Forms
Imports System.Drawing

Public Class MyPicBox

    Inherits PictureBox
    ' Public navCoord As System.Drawing.Point
    Private redbr As SolidBrush = New SolidBrush(Color.Red)
    Private blckbr As SolidBrush = New SolidBrush(Color.Black)
    Public Sub New()
        MyBase.New()
    End Sub
    Protected Overrides Sub OnPaint(pe As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(pe)
        ' destination bearing
        'Dim rad As Integer = (navCoord.X - 10)
        'Dim desx As Integer = CType((navCoord.X _
        '            + (rad * Math.Sin((destDir * (3.1416 / 180))))), Integer)
        'Dim desy As Integer = CType((navCoord.Y _
        '            - (rad * Math.Cos((destDir * (3.1416 / 180))))), Integer)


        pe.Graphics.FillRectangle(blckbr, 0, 0, Me.Width, Me.Height)
        pe.Graphics.FillRectangle(redbr, 2, 2, Me.Width - 4, Me.Height - 4)
    End Sub

    Private Sub InitializeComponent()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'nm
        '
        Me.Name = "nm"
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

End Class
