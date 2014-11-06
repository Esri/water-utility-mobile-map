<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EditRetriever
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
        Me.lstMsg = New System.Windows.Forms.ListBox()
        Me.chkLstLayers = New System.Windows.Forms.CheckedListBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.lblTotCnt = New System.Windows.Forms.Label()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lstMsg
        '
        Me.lstMsg.FormattingEnabled = True
        Me.lstMsg.Location = New System.Drawing.Point(12, 215)
        Me.lstMsg.Name = "lstMsg"
        Me.lstMsg.Size = New System.Drawing.Size(788, 147)
        Me.lstMsg.TabIndex = 0
        '
        'chkLstLayers
        '
        Me.chkLstLayers.FormattingEnabled = True
        Me.chkLstLayers.Location = New System.Drawing.Point(12, 27)
        Me.chkLstLayers.Name = "chkLstLayers"
        Me.chkLstLayers.Size = New System.Drawing.Size(788, 184)
        Me.chkLstLayers.TabIndex = 1
        '
        'Button1
        '
        Me.Button1.Enabled = False
        Me.Button1.Location = New System.Drawing.Point(275, 404)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 2
        Me.Button1.Text = "Export"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'lblTotCnt
        '
        Me.lblTotCnt.AutoSize = True
        Me.lblTotCnt.Location = New System.Drawing.Point(13, 414)
        Me.lblTotCnt.Name = "lblTotCnt"
        Me.lblTotCnt.Size = New System.Drawing.Size(0, 13)
        Me.lblTotCnt.TabIndex = 3
        '
        'TextBox1
        '
        Me.TextBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox1.Location = New System.Drawing.Point(16, 368)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(319, 26)
        Me.TextBox1.TabIndex = 4
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(342, 368)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(28, 26)
        Me.Button2.TabIndex = 5
        Me.Button2.Text = "..."
        Me.Button2.UseVisualStyleBackColor = True
        '
        'EditRetriever
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(799, 494)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.lblTotCnt)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.chkLstLayers)
        Me.Controls.Add(Me.lstMsg)
        Me.Name = "EditRetriever"
        Me.Text = "Export Edits to CSV"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lstMsg As System.Windows.Forms.ListBox
    Friend WithEvents chkLstLayers As System.Windows.Forms.CheckedListBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents lblTotCnt As System.Windows.Forms.Label
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents Button2 As System.Windows.Forms.Button

    Private Sub EditRetriever_Disposed(sender As Object, e As System.EventArgs) Handles Me.Disposed
        If m_MobileConnect IsNot Nothing Then

            m_MobileConnect.Dispose()
        End If
        m_MobileConnect = Nothing

        If (m_MobileCache IsNot Nothing) Then
            If m_MobileCache.IsOpen Then
                m_MobileCache.Close()
            End If
            m_MobileCache.Dispose()

        End If
        If (m_Map IsNot Nothing) Then
            m_Map.Dispose()

        End If
        m_Map = Nothing

        m_StreamWriter.Flush()
        m_StreamWriter.Close()
        m_StreamWriter = Nothing
    End Sub
End Class
