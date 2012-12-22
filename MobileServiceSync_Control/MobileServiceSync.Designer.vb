<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MobileServiceSync
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.gpSyncSettings = New System.Windows.Forms.GroupBox()
        Me.lblNumEdits = New System.Windows.Forms.Label()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.btnPost = New System.Windows.Forms.Button()
        Me.cboRefreshOptions = New System.Windows.Forms.ComboBox()
        Me.gpBoxErrors = New System.Windows.Forms.GroupBox()
        Me.lstErrors = New System.Windows.Forms.ListBox()
        Me.gpSyncSettings.SuspendLayout()
        Me.gpBoxErrors.SuspendLayout()
        Me.SuspendLayout()
        '
        'gpSyncSettings
        '
        Me.gpSyncSettings.Controls.Add(Me.lblNumEdits)
        Me.gpSyncSettings.Controls.Add(Me.btnRefresh)
        Me.gpSyncSettings.Controls.Add(Me.btnPost)
        Me.gpSyncSettings.Controls.Add(Me.cboRefreshOptions)
        Me.gpSyncSettings.Dock = System.Windows.Forms.DockStyle.Top
        Me.gpSyncSettings.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gpSyncSettings.Location = New System.Drawing.Point(0, 0)
        Me.gpSyncSettings.Name = "gpSyncSettings"
        Me.gpSyncSettings.Size = New System.Drawing.Size(426, 146)
        Me.gpSyncSettings.TabIndex = 2
        Me.gpSyncSettings.TabStop = False
        '
        'lblNumEdits
        '
        Me.lblNumEdits.Location = New System.Drawing.Point(71, 26)
        Me.lblNumEdits.Name = "lblNumEdits"
        Me.lblNumEdits.Size = New System.Drawing.Size(258, 50)
        Me.lblNumEdits.TabIndex = 13
        Me.lblNumEdits.Text = "Label1"
        Me.lblNumEdits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnRefresh
        '
        Me.btnRefresh.BackgroundImage = Global.MobileControls.My.Resources.Resources.DownloadRed
        Me.btnRefresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnRefresh.Enabled = False
        Me.btnRefresh.FlatAppearance.BorderSize = 0
        Me.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnRefresh.Location = New System.Drawing.Point(13, 82)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(50, 50)
        Me.btnRefresh.TabIndex = 12
        Me.btnRefresh.UseVisualStyleBackColor = True
        '
        'btnPost
        '
        Me.btnPost.BackgroundImage = Global.MobileControls.My.Resources.Resources.UploadRed
        Me.btnPost.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnPost.Enabled = False
        Me.btnPost.FlatAppearance.BorderSize = 0
        Me.btnPost.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnPost.Location = New System.Drawing.Point(13, 26)
        Me.btnPost.Name = "btnPost"
        Me.btnPost.Size = New System.Drawing.Size(50, 50)
        Me.btnPost.TabIndex = 11
        Me.btnPost.UseVisualStyleBackColor = True
        '
        'cboRefreshOptions
        '
        Me.cboRefreshOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboRefreshOptions.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboRefreshOptions.FormattingEnabled = True
        Me.cboRefreshOptions.Location = New System.Drawing.Point(71, 94)
        Me.cboRefreshOptions.Name = "cboRefreshOptions"
        Me.cboRefreshOptions.Size = New System.Drawing.Size(258, 28)
        Me.cboRefreshOptions.TabIndex = 10
        '
        'gpBoxErrors
        '
        Me.gpBoxErrors.AutoSize = True
        Me.gpBoxErrors.Controls.Add(Me.lstErrors)
        Me.gpBoxErrors.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gpBoxErrors.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gpBoxErrors.Location = New System.Drawing.Point(0, 146)
        Me.gpBoxErrors.Name = "gpBoxErrors"
        Me.gpBoxErrors.Size = New System.Drawing.Size(426, 460)
        Me.gpBoxErrors.TabIndex = 6
        Me.gpBoxErrors.TabStop = False
        '
        'lstErrors
        '
        Me.lstErrors.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstErrors.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lstErrors.FormattingEnabled = True
        Me.lstErrors.HorizontalScrollbar = True
        Me.lstErrors.ItemHeight = 20
        Me.lstErrors.Location = New System.Drawing.Point(3, 20)
        Me.lstErrors.Name = "lstErrors"
        Me.lstErrors.Size = New System.Drawing.Size(420, 437)
        Me.lstErrors.TabIndex = 0
        '
        'MobileServiceSync
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.gpBoxErrors)
        Me.Controls.Add(Me.gpSyncSettings)
        Me.Name = "MobileServiceSync"
        Me.Size = New System.Drawing.Size(426, 606)
        Me.gpSyncSettings.ResumeLayout(False)
        Me.gpBoxErrors.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents gpSyncSettings As System.Windows.Forms.GroupBox
    Friend WithEvents cboRefreshOptions As System.Windows.Forms.ComboBox
    Friend WithEvents gpBoxErrors As System.Windows.Forms.GroupBox
    Friend WithEvents lstErrors As System.Windows.Forms.ListBox
    Friend WithEvents lblNumEdits As System.Windows.Forms.Label
    Friend WithEvents btnRefresh As System.Windows.Forms.Button
    Friend WithEvents btnPost As System.Windows.Forms.Button

End Class
