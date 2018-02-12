Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Windows.Forms

Namespace WaterPipe
    <DesignerGenerated> _
    Public Class HighScoreDialog
        Inherits Form
        ' Methods
        Public Sub New()
            Me.InitializeComponent
        End Sub

        Private Sub Cancel_Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            Me.DialogResult = DialogResult.Cancel
            Me.Close
        End Sub

        <DebuggerNonUserCode> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If (disposing AndAlso (Not Me.components Is Nothing)) Then
                Me.components.Dispose
            End If
            MyBase.Dispose(disposing)
        End Sub

        Public Sub Hide_InputBox(ByVal Rank As Integer)
            Me.set_BoxCollection
            NewLateBinding.LateCall(Me.HighScoreNameBoxCollection.Item(Rank), Nothing, "Hide", New Object(0  - 1) {}, Nothing, Nothing, Nothing, True)
            NewLateBinding.LateSetComplex(Me.HighScoreNameBoxCollection.Item(Rank), Nothing, "Enabled", New Object() { False }, Nothing, Nothing, False, True)
        End Sub

        <DebuggerStepThrough> _
        Private Sub InitializeComponent()
            Me.TableLayoutPanel1 = New TableLayoutPanel
            Me.OK_Button = New Button
            Me.Cancel_Button = New Button
            Me.HighScorePanel = New Panel
            Me.NameTextBox10 = New TextBox
            Me.NameTextBox9 = New TextBox
            Me.NameTextBox8 = New TextBox
            Me.NameTextBox7 = New TextBox
            Me.NameTextBox6 = New TextBox
            Me.NameTextBox5 = New TextBox
            Me.NameTextBox4 = New TextBox
            Me.NameTextBox3 = New TextBox
            Me.NameTextBox2 = New TextBox
            Me.NameTextBox1 = New TextBox
            Me.NameListLabel = New Label
            Me.ScoreListLabel = New Label
            Me.RankListLabel = New Label
            Me.HighScoreLabel = New Label
            Me.NameLabel = New Label
            Me.RankLabel = New Label
            Me.HighScoresHandLabel = New Label
            Me.TableLayoutPanel1.SuspendLayout
            Me.HighScorePanel.SuspendLayout
            Me.SuspendLayout
            Me.TableLayoutPanel1.Anchor = (AnchorStyles.Right Or AnchorStyles.Bottom)
            Me.TableLayoutPanel1.ColumnCount = 2
            Me.TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50!))
            Me.TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50!))
            Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
            Dim point As New Point(&HB2, &H1B7)
            Me.TableLayoutPanel1.Location = point
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            Me.TableLayoutPanel1.RowCount = 1
            Me.TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 50!))
            Dim size As New Size(&H92, &H1D)
            Me.TableLayoutPanel1.Size = size
            Me.TableLayoutPanel1.TabIndex = 0
            Me.OK_Button.Anchor = AnchorStyles.None
            Me.OK_Button.BackColor = Color.Gold
            point = New Point(3, 3)
            Me.OK_Button.Location = point
            Me.OK_Button.Name = "OK_Button"
            size = New Size(&H43, &H17)
            Me.OK_Button.Size = size
            Me.OK_Button.TabIndex = 0
            Me.OK_Button.Text = "OK"
            Me.OK_Button.UseVisualStyleBackColor = False
            Me.Cancel_Button.Anchor = AnchorStyles.None
            Me.Cancel_Button.BackColor = Color.DeepPink
            Me.Cancel_Button.DialogResult = DialogResult.Cancel
            point = New Point(&H4C, 3)
            Me.Cancel_Button.Location = point
            Me.Cancel_Button.Name = "Cancel_Button"
            size = New Size(&H43, &H17)
            Me.Cancel_Button.Size = size
            Me.Cancel_Button.TabIndex = 1
            Me.Cancel_Button.Text = "Cancel"
            Me.Cancel_Button.UseVisualStyleBackColor = False
            Me.HighScorePanel.BackColor = Color.Transparent
            Me.HighScorePanel.Controls.Add(Me.NameTextBox10)
            Me.HighScorePanel.Controls.Add(Me.NameTextBox9)
            Me.HighScorePanel.Controls.Add(Me.NameTextBox8)
            Me.HighScorePanel.Controls.Add(Me.NameTextBox7)
            Me.HighScorePanel.Controls.Add(Me.NameTextBox6)
            Me.HighScorePanel.Controls.Add(Me.NameTextBox5)
            Me.HighScorePanel.Controls.Add(Me.NameTextBox4)
            Me.HighScorePanel.Controls.Add(Me.NameTextBox3)
            Me.HighScorePanel.Controls.Add(Me.NameTextBox2)
            Me.HighScorePanel.Controls.Add(Me.NameTextBox1)
            Me.HighScorePanel.Controls.Add(Me.NameListLabel)
            Me.HighScorePanel.Controls.Add(Me.ScoreListLabel)
            Me.HighScorePanel.Controls.Add(Me.RankListLabel)
            Me.HighScorePanel.Controls.Add(Me.HighScoreLabel)
            Me.HighScorePanel.Controls.Add(Me.NameLabel)
            Me.HighScorePanel.Controls.Add(Me.RankLabel)
            Me.HighScorePanel.Controls.Add(Me.HighScoresHandLabel)
            Me.HighScorePanel.ForeColor = Color.Gold
            point = New Point(6, 6)
            Me.HighScorePanel.Location = point
            Me.HighScorePanel.Name = "HighScorePanel"
            size = New Size(&H145, &H1AB)
            Me.HighScorePanel.Size = size
            Me.HighScorePanel.TabIndex = &HA5
            Me.NameTextBox10.Enabled = False
            point = New Point(90, &H17F)
            Me.NameTextBox10.Location = point
            Me.NameTextBox10.Name = "NameTextBox10"
            size = New Size(&H80, 20)
            Me.NameTextBox10.Size = size
            Me.NameTextBox10.TabIndex = 15
            Me.NameTextBox10.Visible = False
            Me.NameTextBox9.Enabled = False
            point = New Point(90, &H161)
            Me.NameTextBox9.Location = point
            Me.NameTextBox9.Name = "NameTextBox9"
            size = New Size(&H80, 20)
            Me.NameTextBox9.Size = size
            Me.NameTextBox9.TabIndex = 14
            Me.NameTextBox9.Visible = False
            Me.NameTextBox8.Enabled = False
            point = New Point(90, &H143)
            Me.NameTextBox8.Location = point
            Me.NameTextBox8.Name = "NameTextBox8"
            size = New Size(&H80, 20)
            Me.NameTextBox8.Size = size
            Me.NameTextBox8.TabIndex = 13
            Me.NameTextBox8.Visible = False
            Me.NameTextBox7.Enabled = False
            point = New Point(90, &H125)
            Me.NameTextBox7.Location = point
            Me.NameTextBox7.Name = "NameTextBox7"
            size = New Size(&H80, 20)
            Me.NameTextBox7.Size = size
            Me.NameTextBox7.TabIndex = 12
            Me.NameTextBox7.Visible = False
            Me.NameTextBox6.Enabled = False
            point = New Point(90, &H107)
            Me.NameTextBox6.Location = point
            Me.NameTextBox6.Name = "NameTextBox6"
            size = New Size(&H80, 20)
            Me.NameTextBox6.Size = size
            Me.NameTextBox6.TabIndex = 11
            Me.NameTextBox6.Visible = False
            Me.NameTextBox5.Enabled = False
            point = New Point(90, &HE9)
            Me.NameTextBox5.Location = point
            Me.NameTextBox5.Name = "NameTextBox5"
            size = New Size(&H80, 20)
            Me.NameTextBox5.Size = size
            Me.NameTextBox5.TabIndex = 10
            Me.NameTextBox5.Visible = False
            Me.NameTextBox4.Enabled = False
            point = New Point(90, &HCB)
            Me.NameTextBox4.Location = point
            Me.NameTextBox4.Name = "NameTextBox4"
            size = New Size(&H80, 20)
            Me.NameTextBox4.Size = size
            Me.NameTextBox4.TabIndex = 9
            Me.NameTextBox4.Visible = False
            Me.NameTextBox3.Enabled = False
            point = New Point(90, &HAD)
            Me.NameTextBox3.Location = point
            Me.NameTextBox3.Name = "NameTextBox3"
            size = New Size(&H80, 20)
            Me.NameTextBox3.Size = size
            Me.NameTextBox3.TabIndex = 8
            Me.NameTextBox3.Visible = False
            Me.NameTextBox2.Enabled = False
            point = New Point(90, &H8F)
            Me.NameTextBox2.Location = point
            Me.NameTextBox2.Name = "NameTextBox2"
            size = New Size(&H80, 20)
            Me.NameTextBox2.Size = size
            Me.NameTextBox2.TabIndex = 7
            Me.NameTextBox2.Visible = False
            Me.NameTextBox1.Enabled = False
            point = New Point(90, &H71)
            Me.NameTextBox1.Location = point
            Me.NameTextBox1.Name = "NameTextBox1"
            size = New Size(&H80, 20)
            Me.NameTextBox1.Size = size
            Me.NameTextBox1.TabIndex = 4
            Me.NameTextBox1.Visible = False
            Me.NameListLabel.AutoSize = True
            Me.NameListLabel.Font = New Font("Microsoft Sans Serif", 9!, FontStyle.Regular, GraphicsUnit.Point, 0)
            point = New Point(90, &H73)
            Me.NameListLabel.Location = point
            Me.NameListLabel.Name = "NameListLabel"
            size = New Size(&H15, &H11D)
            Me.NameListLabel.Size = size
            Me.NameListLabel.TabIndex = &H10
            Me.NameListLabel.Text = "1" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "2" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "3" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "4" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "5" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "6" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "7" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "8" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "9" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "10"
            Me.ScoreListLabel.AutoSize = True
            Me.ScoreListLabel.Font = New Font("Microsoft Sans Serif", 9!, FontStyle.Regular, GraphicsUnit.Point, 0)
            point = New Point(240, &H73)
            Me.ScoreListLabel.Location = point
            Me.ScoreListLabel.Name = "ScoreListLabel"
            size = New Size(&H15, &H11D)
            Me.ScoreListLabel.Size = size
            Me.ScoreListLabel.TabIndex = 6
            Me.ScoreListLabel.Text = "1" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "2" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "3" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "4" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "5" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "6" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "7" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "8" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "9" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "10"
            Me.RankListLabel.AutoSize = True
            Me.RankListLabel.Font = New Font("Microsoft Sans Serif", 9!, FontStyle.Regular, GraphicsUnit.Point, 0)
            point = New Point(&H20, &H73)
            Me.RankListLabel.Location = point
            Me.RankListLabel.Name = "RankListLabel"
            size = New Size(&H15, &H11D)
            Me.RankListLabel.Size = size
            Me.RankListLabel.TabIndex = 5
            Me.RankListLabel.Text = "1" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "2" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "3" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "4" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "5" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "6" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "7" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "8" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "9" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "10"
            Me.HighScoreLabel.AutoSize = True
            Me.HighScoreLabel.Font = New Font("Microsoft Sans Serif", 9.75!, FontStyle.Regular, GraphicsUnit.Point, 0)
            point = New Point(240, 80)
            Me.HighScoreLabel.Location = point
            Me.HighScoreLabel.Name = "HighScoreLabel"
            size = New Size(&H2C, &H10)
            Me.HighScoreLabel.Size = size
            Me.HighScoreLabel.TabIndex = 3
            Me.HighScoreLabel.Tag = "Score"
            Me.HighScoreLabel.Text = "Score"
            Me.NameLabel.AutoSize = True
            Me.NameLabel.Font = New Font("Microsoft Sans Serif", 9.75!, FontStyle.Regular, GraphicsUnit.Point, 0)
            point = New Point(90, 80)
            Me.NameLabel.Location = point
            Me.NameLabel.Name = "NameLabel"
            size = New Size(&H2D, &H10)
            Me.NameLabel.Size = size
            Me.NameLabel.TabIndex = 2
            Me.NameLabel.Tag = "Name"
            Me.NameLabel.Text = "Name"
            Me.RankLabel.AutoSize = True
            Me.RankLabel.Font = New Font("Microsoft Sans Serif", 9.75!, FontStyle.Regular, GraphicsUnit.Point, 0)
            point = New Point(&H20, 80)
            Me.RankLabel.Location = point
            Me.RankLabel.Name = "RankLabel"
            size = New Size(40, &H10)
            Me.RankLabel.Size = size
            Me.RankLabel.TabIndex = 1
            Me.RankLabel.Tag = "Rank"
            Me.RankLabel.Text = "Rank"
            Me.HighScoresHandLabel.AutoSize = True
            Me.HighScoresHandLabel.Font = New Font("Monotype Corsiva", 36!, FontStyle.Italic, GraphicsUnit.Point, 0)
            point = New Point(&H3A, 5)
            Me.HighScoresHandLabel.Location = point
            Me.HighScoresHandLabel.Name = "HighScoresHandLabel"
            size = New Size(&HDF, &H39)
            Me.HighScoresHandLabel.Size = size
            Me.HighScoresHandLabel.TabIndex = 0
            Me.HighScoresHandLabel.Tag = "HighScoresHand"
            Me.HighScoresHandLabel.Text = "High Scores"
            Me.AcceptButton = Me.OK_Button
            Dim ef As New SizeF(6!, 13!)
            Me.AutoScaleDimensions = ef
            Me.AutoScaleMode = AutoScaleMode.Font
            Me.BackColor = Color.Green
            Me.CancelButton = Me.Cancel_Button
            size = New Size(&H150, 480)
            Me.ClientSize = size
            Me.Controls.Add(Me.HighScorePanel)
            Me.Controls.Add(Me.TableLayoutPanel1)
            Me.FormBorderStyle = FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "HighScoreDialog"
            Me.ShowInTaskbar = False
            Me.StartPosition = FormStartPosition.CenterParent
            Me.Text = "HighScoreDialog"
            Me.TopMost = True
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.HighScorePanel.ResumeLayout(False)
            Me.HighScorePanel.PerformLayout
            Me.ResumeLayout(False)
        End Sub

        Private Sub OK_Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            Me.DialogResult = DialogResult.OK
            Me.Close
        End Sub

        Private Sub set_BoxCollection()
            If (Me.HighScoreNameBoxCollection.Count = 0) Then
                Me.HighScoreNameBoxCollection.Add(Me.NameTextBox1, Conversions.ToString(1), Nothing, Nothing)
                Me.HighScoreNameBoxCollection.Add(Me.NameTextBox2, Conversions.ToString(2), Nothing, Nothing)
                Me.HighScoreNameBoxCollection.Add(Me.NameTextBox3, Conversions.ToString(3), Nothing, Nothing)
                Me.HighScoreNameBoxCollection.Add(Me.NameTextBox4, Conversions.ToString(4), Nothing, Nothing)
                Me.HighScoreNameBoxCollection.Add(Me.NameTextBox5, Conversions.ToString(5), Nothing, Nothing)
                Me.HighScoreNameBoxCollection.Add(Me.NameTextBox6, Conversions.ToString(6), Nothing, Nothing)
                Me.HighScoreNameBoxCollection.Add(Me.NameTextBox7, Conversions.ToString(7), Nothing, Nothing)
                Me.HighScoreNameBoxCollection.Add(Me.NameTextBox8, Conversions.ToString(8), Nothing, Nothing)
                Me.HighScoreNameBoxCollection.Add(Me.NameTextBox9, Conversions.ToString(9), Nothing, Nothing)
                Me.HighScoreNameBoxCollection.Add(Me.NameTextBox10, Conversions.ToString(10), Nothing, Nothing)
            End If
        End Sub

        Public Sub Set_HighScore(ByVal HighScore As Integer(), ByVal HighScoreName As String())
            Dim str2 As String = ""
            Dim str As String = ""
            Dim num2 As Integer = (HighScore.Length - 2)
            Dim i As Integer = 0
            Do While (i <= num2)
                str2 = (str2 & Conversions.ToString(HighScore(i)) & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10))
                str = (str & HighScoreName(i) & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10))
                i += 1
            Loop
            Me.ScoreListLabel.Text = str2
            Me.NameListLabel.Text = str
        End Sub

        Public Sub Show_InputBox(ByVal Rank As Integer, ByVal Name As String)
            Me.set_BoxCollection
            NewLateBinding.LateSetComplex(Me.HighScoreNameBoxCollection.Item(Rank), Nothing, "Text", New Object() { Name }, Nothing, Nothing, False, True)
            NewLateBinding.LateCall(Me.HighScoreNameBoxCollection.Item(Rank), Nothing, "Show", New Object(0  - 1) {}, Nothing, Nothing, Nothing, True)
            NewLateBinding.LateSetComplex(Me.HighScoreNameBoxCollection.Item(Rank), Nothing, "Enabled", New Object() { True }, Nothing, Nothing, False, True)
            NewLateBinding.LateCall(Me.HighScoreNameBoxCollection.Item(Rank), Nothing, "Focus", New Object(0  - 1) {}, Nothing, Nothing, Nothing, True)
        End Sub


        ' Properties
        Friend Overridable Property Cancel_Button As Button
            Get
                Return Me._Cancel_Button
            End Get
            Set(ByVal WithEventsValue As Button)
                If (Not Me._Cancel_Button Is Nothing) Then
                    RemoveHandler Me._Cancel_Button.Click, New EventHandler(AddressOf Me.Cancel_Button_Click)
                End If
                Me._Cancel_Button = WithEventsValue
                If (Not Me._Cancel_Button Is Nothing) Then
                    AddHandler Me._Cancel_Button.Click, New EventHandler(AddressOf Me.Cancel_Button_Click)
                End If
            End Set
        End Property

        Friend Overridable Property HighScoreLabel As Label
            Get
                Return Me._HighScoreLabel
            End Get
            Set(ByVal WithEventsValue As Label)
                Me._HighScoreLabel = WithEventsValue
            End Set
        End Property

        Friend Overridable Property HighScorePanel As Panel
            Get
                Return Me._HighScorePanel
            End Get
            Set(ByVal WithEventsValue As Panel)
                Me._HighScorePanel = WithEventsValue
            End Set
        End Property

        Friend Overridable Property HighScoresHandLabel As Label
            Get
                Return Me._HighScoresHandLabel
            End Get
            Set(ByVal WithEventsValue As Label)
                Me._HighScoresHandLabel = WithEventsValue
            End Set
        End Property

        Friend Overridable Property NameLabel As Label
            Get
                Return Me._NameLabel
            End Get
            Set(ByVal WithEventsValue As Label)
                Me._NameLabel = WithEventsValue
            End Set
        End Property

        Friend Overridable Property NameListLabel As Label
            Get
                Return Me._NameListLabel
            End Get
            Set(ByVal WithEventsValue As Label)
                Me._NameListLabel = WithEventsValue
            End Set
        End Property

        Friend Overridable Property NameTextBox1 As TextBox
            Get
                Return Me._NameTextBox1
            End Get
            Set(ByVal WithEventsValue As TextBox)
                Me._NameTextBox1 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property NameTextBox10 As TextBox
            Get
                Return Me._NameTextBox10
            End Get
            Set(ByVal WithEventsValue As TextBox)
                Me._NameTextBox10 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property NameTextBox2 As TextBox
            Get
                Return Me._NameTextBox2
            End Get
            Set(ByVal WithEventsValue As TextBox)
                Me._NameTextBox2 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property NameTextBox3 As TextBox
            Get
                Return Me._NameTextBox3
            End Get
            Set(ByVal WithEventsValue As TextBox)
                Me._NameTextBox3 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property NameTextBox4 As TextBox
            Get
                Return Me._NameTextBox4
            End Get
            Set(ByVal WithEventsValue As TextBox)
                Me._NameTextBox4 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property NameTextBox5 As TextBox
            Get
                Return Me._NameTextBox5
            End Get
            Set(ByVal WithEventsValue As TextBox)
                Me._NameTextBox5 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property NameTextBox6 As TextBox
            Get
                Return Me._NameTextBox6
            End Get
            Set(ByVal WithEventsValue As TextBox)
                Me._NameTextBox6 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property NameTextBox7 As TextBox
            Get
                Return Me._NameTextBox7
            End Get
            Set(ByVal WithEventsValue As TextBox)
                Me._NameTextBox7 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property NameTextBox8 As TextBox
            Get
                Return Me._NameTextBox8
            End Get
            Set(ByVal WithEventsValue As TextBox)
                Me._NameTextBox8 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property NameTextBox9 As TextBox
            Get
                Return Me._NameTextBox9
            End Get
            Set(ByVal WithEventsValue As TextBox)
                Me._NameTextBox9 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property OK_Button As Button
            Get
                Return Me._OK_Button
            End Get
            Set(ByVal WithEventsValue As Button)
                If (Not Me._OK_Button Is Nothing) Then
                    RemoveHandler Me._OK_Button.Click, New EventHandler(AddressOf Me.OK_Button_Click)
                End If
                Me._OK_Button = WithEventsValue
                If (Not Me._OK_Button Is Nothing) Then
                    AddHandler Me._OK_Button.Click, New EventHandler(AddressOf Me.OK_Button_Click)
                End If
            End Set
        End Property

        Friend Overridable Property RankLabel As Label
            Get
                Return Me._RankLabel
            End Get
            Set(ByVal WithEventsValue As Label)
                Me._RankLabel = WithEventsValue
            End Set
        End Property

        Friend Overridable Property RankListLabel As Label
            Get
                Return Me._RankListLabel
            End Get
            Set(ByVal WithEventsValue As Label)
                Me._RankListLabel = WithEventsValue
            End Set
        End Property

        Friend Overridable Property ScoreListLabel As Label
            Get
                Return Me._ScoreListLabel
            End Get
            Set(ByVal WithEventsValue As Label)
                Me._ScoreListLabel = WithEventsValue
            End Set
        End Property

        Friend Overridable Property TableLayoutPanel1 As TableLayoutPanel
            Get
                Return Me._TableLayoutPanel1
            End Get
            Set(ByVal WithEventsValue As TableLayoutPanel)
                Me._TableLayoutPanel1 = WithEventsValue
            End Set
        End Property


        ' Fields
        <AccessedThroughProperty("Cancel_Button")> _
        Private _Cancel_Button As Button
        <AccessedThroughProperty("HighScoreLabel")> _
        Private _HighScoreLabel As Label
        <AccessedThroughProperty("HighScorePanel")> _
        Private _HighScorePanel As Panel
        <AccessedThroughProperty("HighScoresHandLabel")> _
        Private _HighScoresHandLabel As Label
        <AccessedThroughProperty("NameLabel")> _
        Private _NameLabel As Label
        <AccessedThroughProperty("NameListLabel")> _
        Private _NameListLabel As Label
        <AccessedThroughProperty("NameTextBox1")> _
        Private _NameTextBox1 As TextBox
        <AccessedThroughProperty("NameTextBox10")> _
        Private _NameTextBox10 As TextBox
        <AccessedThroughProperty("NameTextBox2")> _
        Private _NameTextBox2 As TextBox
        <AccessedThroughProperty("NameTextBox3")> _
        Private _NameTextBox3 As TextBox
        <AccessedThroughProperty("NameTextBox4")> _
        Private _NameTextBox4 As TextBox
        <AccessedThroughProperty("NameTextBox5")> _
        Private _NameTextBox5 As TextBox
        <AccessedThroughProperty("NameTextBox6")> _
        Private _NameTextBox6 As TextBox
        <AccessedThroughProperty("NameTextBox7")> _
        Private _NameTextBox7 As TextBox
        <AccessedThroughProperty("NameTextBox8")> _
        Private _NameTextBox8 As TextBox
        <AccessedThroughProperty("NameTextBox9")> _
        Private _NameTextBox9 As TextBox
        <AccessedThroughProperty("OK_Button")> _
        Private _OK_Button As Button
        <AccessedThroughProperty("RankLabel")> _
        Private _RankLabel As Label
        <AccessedThroughProperty("RankListLabel")> _
        Private _RankListLabel As Label
        <AccessedThroughProperty("ScoreListLabel")> _
        Private _ScoreListLabel As Label
        <AccessedThroughProperty("TableLayoutPanel1")> _
        Private _TableLayoutPanel1 As TableLayoutPanel
        Private components As IContainer
        Public HighScoreNameBoxCollection As Collection = New Collection
    End Class
End Namespace

