<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmLink
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmLink))
        Me.splMain = New System.Windows.Forms.SplitContainer()
        Me.picBox = New System.Windows.Forms.PictureBox()
        Me.btnEmail = New System.Windows.Forms.Button()
        Me.btnOpenLoc = New System.Windows.Forms.Button()
        Me.btnPreview = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.splMain.Panel1.SuspendLayout()
        Me.splMain.Panel2.SuspendLayout()
        Me.splMain.SuspendLayout()
        CType(Me.picBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'splMain
        '
        Me.splMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.splMain.IsSplitterFixed = True
        Me.splMain.Location = New System.Drawing.Point(0, 0)
        Me.splMain.Name = "splMain"
        Me.splMain.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'splMain.Panel1
        '
        Me.splMain.Panel1.Controls.Add(Me.picBox)
        '
        'splMain.Panel2
        '
        Me.splMain.Panel2.Controls.Add(Me.btnEmail)
        Me.splMain.Panel2.Controls.Add(Me.btnOpenLoc)
        Me.splMain.Panel2.Controls.Add(Me.btnPreview)
        Me.splMain.Panel2.Controls.Add(Me.btnClose)
        Me.splMain.Panel2MinSize = 105
        Me.splMain.Size = New System.Drawing.Size(565, 385)
        Me.splMain.SplitterDistance = 274
        Me.splMain.TabIndex = 0
        '
        'picBox
        '
        Me.picBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.picBox.Location = New System.Drawing.Point(0, 0)
        Me.picBox.Name = "picBox"
        Me.picBox.Size = New System.Drawing.Size(565, 274)
        Me.picBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picBox.TabIndex = 0
        Me.picBox.TabStop = False
        '
        'btnEmail
        '
        Me.btnEmail.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.btnEmail.BackColor = System.Drawing.Color.Transparent
        Me.btnEmail.BackgroundImage = CType(resources.GetObject("btnEmail.BackgroundImage"), System.Drawing.Image)
        Me.btnEmail.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnEmail.FlatAppearance.BorderSize = 0
        Me.btnEmail.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnEmail.Location = New System.Drawing.Point(299, 3)
        Me.btnEmail.Name = "btnEmail"
        Me.btnEmail.Size = New System.Drawing.Size(100, 100)
        Me.btnEmail.TabIndex = 18
        Me.btnEmail.UseVisualStyleBackColor = False
        '
        'btnOpenLoc
        '
        Me.btnOpenLoc.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.btnOpenLoc.BackColor = System.Drawing.Color.Transparent
        Me.btnOpenLoc.BackgroundImage = CType(resources.GetObject("btnOpenLoc.BackgroundImage"), System.Drawing.Image)
        Me.btnOpenLoc.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnOpenLoc.FlatAppearance.BorderSize = 0
        Me.btnOpenLoc.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnOpenLoc.Location = New System.Drawing.Point(24, 3)
        Me.btnOpenLoc.Name = "btnOpenLoc"
        Me.btnOpenLoc.Size = New System.Drawing.Size(100, 100)
        Me.btnOpenLoc.TabIndex = 17
        Me.btnOpenLoc.UseVisualStyleBackColor = False
        '
        'btnPreview
        '
        Me.btnPreview.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.btnPreview.BackColor = System.Drawing.Color.Transparent
        Me.btnPreview.BackgroundImage = CType(resources.GetObject("btnPreview.BackgroundImage"), System.Drawing.Image)
        Me.btnPreview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnPreview.FlatAppearance.BorderSize = 0
        Me.btnPreview.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnPreview.Location = New System.Drawing.Point(160, 3)
        Me.btnPreview.Name = "btnPreview"
        Me.btnPreview.Size = New System.Drawing.Size(100, 100)
        Me.btnPreview.TabIndex = 16
        Me.btnPreview.UseVisualStyleBackColor = False
        '
        'btnClose
        '
        Me.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.btnClose.BackColor = System.Drawing.Color.Transparent
        Me.btnClose.BackgroundImage = CType(resources.GetObject("btnClose.BackgroundImage"), System.Drawing.Image)
        Me.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnClose.FlatAppearance.BorderSize = 0
        Me.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnClose.Location = New System.Drawing.Point(431, 3)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(100, 100)
        Me.btnClose.TabIndex = 15
        Me.btnClose.UseVisualStyleBackColor = False
        '
        'frmLink
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(565, 385)
        Me.ControlBox = False
        Me.Controls.Add(Me.splMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmLink"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "  "
        Me.splMain.Panel1.ResumeLayout(False)
        Me.splMain.Panel2.ResumeLayout(False)
        Me.splMain.ResumeLayout(False)
        CType(Me.picBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents splMain As System.Windows.Forms.SplitContainer
    Friend WithEvents picBox As System.Windows.Forms.PictureBox
    Friend WithEvents btnEmail As System.Windows.Forms.Button
    Friend WithEvents btnOpenLoc As System.Windows.Forms.Button
    Friend WithEvents btnPreview As System.Windows.Forms.Button
    Friend WithEvents btnClose As System.Windows.Forms.Button
End Class
