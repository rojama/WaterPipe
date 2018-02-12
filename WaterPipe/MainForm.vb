Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports Microsoft.VisualBasic.FileIO
Imports System
Imports System.Collections
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Media
Imports System.Resources
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Windows.Forms
Imports WaterPipe.My
Imports WaterPipe.My.Resources
Imports WMPLib

Namespace WaterPipe
    <DesignerGenerated> _
    Public Class MainForm
        Inherits Form
        ' Methods
        Public Sub New()
            AddHandler MyBase.Disposed, New EventHandler(AddressOf Me.MainForm_Disposed)
            AddHandler MyBase.Load, New EventHandler(AddressOf Me.MainForm_Load)
            Me.WATERSPEED = 10
            Me.PIPENO = &H51
            Me.NextBoxCollection = New Collection
            Me.BoxCollection = New Collection
            Me.ImageCollection = New Collection
            Me.SetBoxCollection = New Collection
            Me.AppPath = MyProject.Application.Info.DirectoryPath
            Me.AppName = MyProject.Application.Info.ProductName
            Me.ResMana = Resources.ResourceManager
            Me.Entrance = 1
            Me.FlowBoxCollection = New Collection
            Me.AnimoThreadCollection = New Collection
            Me.PanelBackColor = Color.LightSkyBlue
            Me.FontColor = Color.Black
            Me.GridColor = Color.DeepSkyBlue
            Me.WaterColor = Color.OrangeRed
            Me.PanelBackgroundImage = DirectCast(Me.ResMana.GetObject("Background"), Image)
            Me.AppSounds_PlaceImage = New SoundPlayer
            Me.AppSounds_WaterFlow = New SoundPlayer
            Me.AppSounds_WaterOverflow = New SoundPlayer
            Me.AppSounds_NewGame = New SoundPlayer
            Me.AppSounds_Breaking = New SoundPlayer
            Me.MediaPlayer = New WindowsMediaPlayerClass
            Me.Playlist = Me.MediaPlayer.newPlaylist("NowPlaylist", "")
            Me.LastInputName = "Your Name"
            Me.MediaError = "Can't play the media"
            Me.NewGame = "New Game"
            Me.Somebody = "Somebody"
            Me.HighScore = New Integer() { 600, 550, 500, 450, 400, 350, 300, 250, 200, 150, 0 }
            Me.HighScoreName = New String() { Me.Somebody, Me.Somebody, Me.Somebody, Me.Somebody, Me.Somebody, Me.Somebody, Me.Somebody, Me.Somebody, Me.Somebody, Me.Somebody, "" }
            Me.NowRank = 0
            Me.InitializeComponent
        End Sub

        Private Sub AboutOJPipeToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyProject.Forms.AboutBox.ShowDialog
        End Sub

        Private Sub Add_BoxImage(ByVal box As PictureBox)
            Dim image As Image = DirectCast(NewLateBinding.LateGet(Me.ResMana.GetObject("none"), Nothing, "Clone", New Object(0  - 1) {}, Nothing, Nothing, Nothing), Image)
            Dim graphics As Graphics = Graphics.FromImage(image)
            Dim destRect As New Rectangle(0, 0, 100, 100)
            Dim imageAttr As New ImageAttributes
            If (Not box.Image Is Nothing) Then
                imageAttr.SetGamma(0.3!)
                graphics.DrawImage(box.Image, destRect, 0, 0, 100, 100, GraphicsUnit.Pixel, imageAttr)
            End If
            If (Not Me.Next_1.Image Is Nothing) Then
                imageAttr.SetGamma(3!)
                graphics.DrawImage(Me.Next_1.Image, destRect, 0, 0, 100, 100, GraphicsUnit.Pixel, imageAttr)
            End If
            box.Image = image
        End Sub

        Private Sub Box_Click(ByVal sender As Object, ByVal e As EventArgs)
            NewLateBinding.LateSet(sender, Nothing, "Image", New Object() { Me.TempImage }, Nothing, Nothing)
            Me.Place_Image(DirectCast(sender, PictureBox))
            Me.TempImage = DirectCast(NewLateBinding.LateGet(sender, Nothing, "Image", New Object(0  - 1) {}, Nothing, Nothing, Nothing), Image)
            Me.Add_BoxImage(DirectCast(sender, PictureBox))
        End Sub

        Private Sub Box_MouseEnter(ByVal sender As Object, ByVal e As EventArgs)
            Me.TempImage = DirectCast(NewLateBinding.LateGet(sender, Nothing, "Image", New Object(0  - 1) {}, Nothing, Nothing, Nothing), Image)
            Me.Add_BoxImage(DirectCast(sender, PictureBox))
        End Sub

        Private Sub Box_MouseLeave(ByVal sender As Object, ByVal e As EventArgs)
            NewLateBinding.LateSet(sender, Nothing, "Image", New Object() { Me.TempImage }, Nothing, Nothing)
        End Sub

        Private Sub ColorToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
            Me.MainPanel.BackgroundImage = Nothing
            Me.MainPanel.BackColor = Me.PanelBackColor
            Me.ColorToolStripMenuItem.Checked = True
            Me.ImageToolStripMenuItem.Checked = False
        End Sub

        <DebuggerNonUserCode> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If (disposing AndAlso (Not Me.components Is Nothing)) Then
                Me.components.Dispose
            End If
            MyBase.Dispose(disposing)
        End Sub

        Private Sub EntranceToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
            Me.set_Entrance(Conversions.ToString(NewLateBinding.LateGet(sender, Nothing, "text", New Object(0  - 1) {}, Nothing, Nothing, Nothing)))
            Me.New_Game
        End Sub

        Private Sub ExitToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
            Me.write_Config
            Me.Write_HighScore
            ProjectData.EndApp
        End Sub

        Private Sub Final()
            Dim collection As New Collection
            Dim collection3 As New Collection
            Dim key As String = ""
            Dim collection2 As New Collection
            collection2.Add("R", "L", Nothing, Nothing)
            collection2.Add("L", "R", Nothing, Nothing)
            collection2.Add("D", "U", Nothing, Nothing)
            collection2.Add("U", "D", Nothing, Nothing)
            Dim flag As Boolean = False
            Me.Invoke(New SetNewGameMenuItemCallback(AddressOf Me.SetNewGameMenuItem), New Object() { False })
            Me.Invoke(New SetEntranceMenuItemCallback(AddressOf Me.SetEntranceMenuItem), New Object() { False })
            Me.Invoke(New SetOJGoMenuItemCallback(AddressOf Me.SetOJGoMenuItem), New Object() { False })
            Me.Invoke(New SetInWaterCallback(AddressOf Me.SetInWater), New Object() { False })
            Me.Invoke(New SetMainBoxCallback(AddressOf Me.SetMainBox), New Object() { False })
            Me.Set_BoxImageTag
            If Me.SoundsToolStripMenuItem.Checked Then
                Me.AppSounds_WaterFlow.PlayLooping
            End If
            Do While Not flag
                Dim str As String
                Dim strArray As String()
                Dim str2 As String
                Dim current As PictureBox
                Dim box2 As PictureBox
                Dim enumerator As IEnumerator
                Dim enumerator2 As IEnumerator
                Dim enumerator3 As IEnumerator
                Try 
                    enumerator = Me.FlowBoxCollection.GetEnumerator
                    Do While enumerator.MoveNext
                        current = DirectCast(enumerator.Current, PictureBox)
                        str = Me.get_BoxDire(current, 1)
                        str2 = Me.get_BoxImageDire(current, str)
                        Console.WriteLine(String.Concat(New String() { current.Name, "**", str, "**", str2 }))
                        Me.Full_Box(current, str, str2)
                    Loop
                Finally
                    If TypeOf enumerator Is IDisposable Then
                        [TryCast](enumerator,IDisposable).Dispose
                    End If
                End Try
                Dim count As Integer = Me.AnimoThreadCollection.Count
                Dim i As Integer = 1
                Do While (i <= count)
                    Dim collection4 As Collection = DirectCast(Me.AnimoThreadCollection.Item(i), Collection)
                    Dim thread As Thread = DirectCast(collection4.Item("Thread"), Thread)
                    Dim parameter As Object() = DirectCast(collection4.Item("Sender"), Object())
                    thread.Start(parameter)
                    i += 1
                Loop
                Dim num6 As Integer = Me.AnimoThreadCollection.Count
                Dim j As Integer = 1
                Do While (j <= num6)
                    Dim collection5 As Collection = DirectCast(Me.AnimoThreadCollection.Item(j), Collection)
                    DirectCast(collection5.Item("Thread"), Thread).Join
                    j += 1
                Loop
                Me.AnimoThreadCollection.Clear
                Try 
                    enumerator2 = Me.FlowBoxCollection.GetEnumerator
                    Do While enumerator2.MoveNext
                        current = DirectCast(enumerator2.Current, PictureBox)
                        str = Me.get_BoxDire(current, 2)
                        Dim chArray2 As Char() = Me.get_BoxImageDire(current, str).ToCharArray
                        Dim k As Integer
                        For k = 0 To chArray2.Length - 1
                            Dim str4 As String = Conversions.ToString(chArray2(k))
                            If Not str.Contains(str4) Then
                                Dim num As Integer
                                Dim ch2 As Char
                                If (str4 = "L") Then
                                    ch2 = current.Name.Chars(6)
                                    num = CInt(Math.Round(CDbl((Conversions.ToDouble(ch2.ToString) - 1))))
                                    key = (current.Name.Substring(4, 2) & Conversions.ToString(num))
                                ElseIf (str4 = "R") Then
                                    ch2 = current.Name.Chars(6)
                                    num = CInt(Math.Round(CDbl((Conversions.ToDouble(ch2.ToString) + 1))))
                                    key = (current.Name.Substring(4, 2) & Conversions.ToString(num))
                                ElseIf (str4 = "U") Then
                                    ch2 = current.Name.Chars(4)
                                    num = CInt(Math.Round(CDbl((Conversions.ToDouble(ch2.ToString) - 1))))
                                    key = (Conversions.ToString(num) & current.Name.Substring(5, 2))
                                ElseIf (str4 = "D") Then
                                    ch2 = current.Name.Chars(4)
                                    num = CInt(Math.Round(CDbl((Conversions.ToDouble(ch2.ToString) + 1))))
                                    key = (Conversions.ToString(num) & current.Name.Substring(5, 2))
                                End If
                                If ((num >= 1) And (num <= 9)) Then
                                    box2 = DirectCast(Me.BoxCollection.Item(key), PictureBox)
                                    box2.Tag = Operators.ConcatenateObject(box2.Tag, collection2.Item(str4))
                                    If Not collection.Contains(key) Then
                                        collection.Add(box2, key, Nothing, Nothing)
                                    End If
                                Else
                                    flag = True
                                    If Not collection3.Contains(current.Name) Then
                                        collection3.Add(current, current.Name, Nothing, Nothing)
                                    End If
                                End If
                            End If
                        Next k
                    Loop
                Finally
                    If TypeOf enumerator2 Is IDisposable Then
                        [TryCast](enumerator2,IDisposable).Dispose
                    End If
                End Try
                Try 
                    enumerator3 = collection.GetEnumerator
                    Do While enumerator3.MoveNext
                        box2 = DirectCast(enumerator3.Current, PictureBox)
                        str = Conversions.ToString(box2.Tag)
                        If Me.SetBoxCollection.Contains(box2.Name) Then
                            str2 = Conversions.ToString(box2.Image.Tag)
                            If str.Contains("F") Then
                                strArray = str.Split(New Char() { "F"c })
                                str = strArray((strArray.Length - 1))
                            End If
                            Dim ch As Char
                            For Each ch In str.ToCharArray
                                If Not str2.Contains(Conversions.ToString(ch)) Then
                                    flag = True
                                    If Not collection3.Contains(box2.Name) Then
                                        collection3.Add(box2, box2.Name, Nothing, Nothing)
                                    End If
                                End If
                            Next
                        Else
                            flag = True
                            If Not collection3.Contains(box2.Name) Then
                                collection3.Add(box2, box2.Name, Nothing, Nothing)
                            End If
                        End If
                    Loop
                Finally
                    If TypeOf enumerator3 Is IDisposable Then
                        [TryCast](enumerator3,IDisposable).Dispose
                    End If
                End Try
                If Not flag Then
                    Dim enumerator4 As IEnumerator
                    Dim enumerator5 As IEnumerator
                    Try 
                        enumerator4 = collection.GetEnumerator
                        Do While enumerator4.MoveNext
                            box2 = DirectCast(enumerator4.Current, PictureBox)
                            str = Conversions.ToString(box2.Tag)
                            str2 = Conversions.ToString(box2.Image.Tag)
                            If str.Contains("F") Then
                                strArray = str.Split(New Char() { "F"c })
                                If str2.Contains("X") Then
                                    If ((strArray(0).Contains("L") Or strArray(0).Contains("R")) AndAlso (strArray(1).Contains("L") Or strArray(1).Contains("R"))) Then
                                        collection.Remove(box2.Name.Substring(4, 3))
                                    ElseIf ((strArray(0).Contains("U") Or strArray(0).Contains("D")) AndAlso (strArray(1).Contains("U") Or strArray(1).Contains("D"))) Then
                                        collection.Remove(box2.Name.Substring(4, 3))
                                    End If
                                Else
                                    If str2.Contains("/") Then
                                        If ((strArray(0).Contains("L") Or strArray(0).Contains("U")) AndAlso (strArray(1).Contains("L") Or strArray(1).Contains("U"))) Then
                                            collection.Remove(box2.Name.Substring(4, 3))
                                        ElseIf ((strArray(0).Contains("R") Or strArray(0).Contains("D")) AndAlso (strArray(1).Contains("R") Or strArray(1).Contains("D"))) Then
                                            collection.Remove(box2.Name.Substring(4, 3))
                                        End If
                                        [Continue] Do
                                    End If
                                    If str2.Contains("\") Then
                                        If ((strArray(0).Contains("L") Or strArray(0).Contains("D")) AndAlso (strArray(1).Contains("L") Or strArray(1).Contains("D"))) Then
                                            collection.Remove(box2.Name.Substring(4, 3))
                                        ElseIf ((strArray(0).Contains("U") Or strArray(0).Contains("R")) AndAlso (strArray(1).Contains("U") Or strArray(1).Contains("R"))) Then
                                            collection.Remove(box2.Name.Substring(4, 3))
                                        End If
                                        [Continue] Do
                                    End If
                                    collection.Remove(box2.Name.Substring(4, 3))
                                End If
                            End If
                        Loop
                    Finally
                        If TypeOf enumerator4 Is IDisposable Then
                            [TryCast](enumerator4,IDisposable).Dispose
                        End If
                    End Try
                    Me.FlowBoxCollection.Clear
                    Try 
                        enumerator5 = collection.GetEnumerator
                        Do While enumerator5.MoveNext
                            box2 = DirectCast(enumerator5.Current, PictureBox)
                            Me.FlowBoxCollection.Add(box2, Nothing, Nothing, Nothing)
                        Loop
                    Finally
                        If TypeOf enumerator5 Is IDisposable Then
                            [TryCast](enumerator5,IDisposable).Dispose
                        End If
                    End Try
                    collection.Clear
                    If (Me.FlowBoxCollection.Count = 0) Then
                        Exit Do
                    End If
                End If
            Loop
            If Me.SoundsToolStripMenuItem.Checked Then
                Me.AppSounds_WaterFlow.Stop
            End If
            If flag Then
                Dim num9 As Integer = collection3.Count
                Dim m As Integer = 1
                Do While (m <= num9)
                    New Thread(New ParameterizedThreadStart(AddressOf Me.Thread_OverflowAnimo)).Start(RuntimeHelpers.GetObjectValue(collection3.Item(m)))
                    m += 1
                Loop
                If Me.SoundsToolStripMenuItem.Checked Then
                    Me.AppSounds_WaterOverflow.PlayLooping
                End If
                Thread.Sleep(&H7D0)
                If Me.SoundsToolStripMenuItem.Checked Then
                    Me.AppSounds_WaterOverflow.Stop
                End If
            End If
            Thread.Sleep(&H3E8)
            Me.Process_HighScore
            Me.Invoke(New SetInWaterTextCallback(AddressOf Me.SetInWaterText), New Object() { Me.NewGame })
            Me.Invoke(New SetNewGameMenuItemCallback(AddressOf Me.SetNewGameMenuItem), New Object() { True })
            Me.Invoke(New SetEntranceMenuItemCallback(AddressOf Me.SetEntranceMenuItem), New Object() { True })
            Me.Invoke(New SetInWaterCallback(AddressOf Me.SetInWater), New Object() { True })
        End Sub

        Private Sub FontColorToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
            If (Me.PanelColorDialog.ShowDialog = DialogResult.OK) Then
                Me.FontColor = Me.PanelColorDialog.Color
                Me.MainPanel.ForeColor = Me.FontColor
            End If
        End Sub

        Private Sub Full_Box(ByVal Box As PictureBox, ByVal BoxDire As String, ByVal BoxImageDire As String)
            Dim num As Integer = Integer.Parse(Conversions.ToString(Box.Image.Tag).Split(New Char() { "|"c })(0))
            Select Case num
                Case 0
                    Me.Score = (Me.Score + 5)
                    Exit Select
                Case 1, 2
                    Me.Score = (Me.Score + 7)
                    Exit Select
                Case 3, 4, 5, 6
                    Me.Score = (Me.Score + 10)
                    Exit Select
                Case 7
                    Me.Score = (Me.Score + 13)
                    Exit Select
                Case 8
                    If Not ((BoxDire.Contains("U") Or BoxDire.Contains("D")) And (BoxDire.Contains("L") Or BoxDire.Contains("R"))) Then
                        Me.Score = (Me.Score + 9)
                        Exit Select
                    End If
                    Me.Score = (Me.Score + &H12)
                    Exit Select
                Case 9, 10, 11, 12
                    Me.Score = (Me.Score + 6)
                    Exit Select
                Case 13
                    If Not ((BoxDire.Contains("U") Or BoxDire.Contains("R")) And (BoxDire.Contains("L") Or BoxDire.Contains("D"))) Then
                        Me.Score = (Me.Score + 6)
                        Exit Select
                    End If
                    Me.Score = (Me.Score + 12)
                    Exit Select
                Case 14
                    If Not ((BoxDire.Contains("U") Or BoxDire.Contains("L")) And (BoxDire.Contains("D") Or BoxDire.Contains("R"))) Then
                        Me.Score = (Me.Score + 6)
                        Exit Select
                    End If
                    Me.Score = (Me.Score + 12)
                    Exit Select
                Case Else
                    Me.Score += 1
                    Exit Select
            End Select
            Me.Invoke(New SetScoreBarValueCallback(AddressOf Me.SetScoreBarValue), New Object() { (CDbl(Me.Score) / 10) })
            Me.Invoke(New SetScoreNumberLabelTextCallback(AddressOf Me.SetScoreNumberLabelText), New Object() { Me.Score.ToString })
            Dim item As New Collection
            Dim thread As New Thread(New ParameterizedThreadStart(AddressOf Me.Thread_BoxAnimo))
            Dim objArray As Object() = New Object() { Box, num, BoxDire, BoxImageDire }
            item.Add(thread, "Thread", Nothing, Nothing)
            item.Add(objArray, "Sender", Nothing, Nothing)
            Me.AnimoThreadCollection.Add(item, Nothing, Nothing, Nothing)
            Box.Tag = Operators.ConcatenateObject(Box.Tag, "F")
        End Sub

        Private Function get_BoxDire(ByVal Box As PictureBox, ByVal No As Integer) As String
            Dim str As String = Conversions.ToString(Box.Tag)
            If str.Contains("F") Then
                Dim strArray As String() = str.Split(New Char() { "F"c })
                str = strArray((strArray.Length - No))
            End If
            Return str
        End Function

        Private Function get_BoxImageDire(ByVal Box As PictureBox, ByVal BoxDire As String) As String
            Dim str As String = Conversions.ToString(NewLateBinding.LateIndexGet(NewLateBinding.LateGet(Box.Image.Tag, Nothing, "Split", New Object() { "|" }, Nothing, Nothing, Nothing), New Object() { 1 }, Nothing))
            If str.Contains("X") Then
                str = ""
                If (BoxDire.Contains("L") Or BoxDire.Contains("R")) Then
                    str = (str & "LR")
                End If
                If (BoxDire.Contains("U") Or BoxDire.Contains("D")) Then
                    str = (str & "UD")
                End If
            End If
            If str.Contains("/") Then
                str = ""
                If (BoxDire.Contains("L") Or BoxDire.Contains("U")) Then
                    str = (str & "LU")
                End If
                If (BoxDire.Contains("R") Or BoxDire.Contains("D")) Then
                    str = (str & "RD")
                End If
            End If
            If str.Contains("\") Then
                str = ""
                If (BoxDire.Contains("L") Or BoxDire.Contains("D")) Then
                    str = (str & "LD")
                End If
                If (BoxDire.Contains("U") Or BoxDire.Contains("R")) Then
                    str = (str & "UR")
                End If
            End If
            Return str
        End Function

        Private Sub get_Config()
            Me.MediaPlayer.settings.setMode("loop", True)
            Me.PanelBackgroundImageFileDialog.InitialDirectory = (Me.AppPath & "\Resources\Picture")
            Me.SelectMusicDialog.InitialDirectory = (Me.AppPath & "\Resources\Music")
            Select Case Me.GetINI("Setting", "Language")
                Case "cn"
                    Me.setLanguage(Me.ChineseToolStripMenuItem)
                    Exit Select
                Case "en"
                    Me.setLanguage(Me.EnglishToolStripMenuItem)
                    Exit Select
            End Select
            Dim iNI As String = Me.GetINI("Setting", "Entrance")
            If (iNI <> "") Then
                Me.set_Entrance(iNI)
            End If
            Dim left As Object = Me.GetINI("Setting", "Music")
            If Operators.ConditionalCompareObjectEqual(left, "on", False) Then
                Dim num As Integer = Integer.Parse(("0" & Me.GetINI("Playlist", "MusicNo")))
                Dim num3 As Integer = num
                Dim i As Integer = 1
                Do While (i <= num3)
                    Dim file As String = Me.GetINI("Playlist", Conversions.ToString(i))
                    If FileSystem.FileExists(file) Then
                        Me.Playlist.appendItem(Me.MediaPlayer.newMedia(file))
                    End If
                    i += 1
                Loop
                Me.MusicToolStripMenuItem.Checked = True
            ElseIf Operators.ConditionalCompareObjectEqual(left, "off", False) Then
                Me.MusicToolStripMenuItem.Checked = False
            End If
            Dim obj6 As Object = Me.GetINI("Setting", "Sounds")
            If Operators.ConditionalCompareObjectEqual(obj6, "on", False) Then
                Me.SoundsToolStripMenuItem.Checked = True
            ElseIf Operators.ConditionalCompareObjectEqual(obj6, "off", False) Then
                Me.SoundsToolStripMenuItem.Checked = False
            End If
            Dim obj2 As Object = Me.GetINI("Setting", "Background")
            If (obj2.ToString.Split(New Char() { ":"c })(0) = "color") Then
                Me.PanelBackColor = Color.FromArgb(Conversions.ToInteger(obj2.ToString.Split(New Char() { ":"c })(1)))
                Me.MainPanel.BackgroundImage = Nothing
                Me.MainPanel.BackColor = Me.PanelBackColor
                Me.ColorToolStripMenuItem.Checked = True
                Me.ImageToolStripMenuItem.Checked = False
            ElseIf Operators.ConditionalCompareObjectEqual(obj2, "image", False) Then
                If FileSystem.FileExists((Me.AppPath & "\Background")) Then
                    FileSystem.CopyFile((Me.AppPath & "\Background"), (Me.AppPath & "\nowBackground"), True)
                    Me.PanelBackgroundImage = Image.FromFile((Me.AppPath & "\nowBackground"))
                End If
                Me.MainPanel.BackgroundImage = Me.PanelBackgroundImage
                Me.ImageToolStripMenuItem.Checked = True
                Me.ColorToolStripMenuItem.Checked = False
            End If
            Dim obj3 As Object = Me.GetINI("Setting", "FontColor")
            If Operators.ConditionalCompareObjectNotEqual(obj3, "", False) Then
                Me.FontColor = Color.FromArgb(Conversions.ToInteger(obj3))
                Me.MainPanel.ForeColor = Me.FontColor
            End If
            obj3 = Me.GetINI("Setting", "WaterColor")
            If Operators.ConditionalCompareObjectNotEqual(obj3, "", False) Then
                Me.WaterColor = Color.FromArgb(Conversions.ToInteger(obj3))
            End If
            obj3 = Me.GetINI("Setting", "GridColor")
            If Operators.ConditionalCompareObjectNotEqual(obj3, "", False) Then
                Me.GridColor = Color.FromArgb(Conversions.ToInteger(obj3))
            End If
            Dim obj4 As Object = Me.GetINI("Setting", "Grid")
            If Operators.ConditionalCompareObjectEqual(obj4, "on", False) Then
                Me.ShowGridToolStripMenuItem.Checked = True
                Me.OutBondPictureBox.BackColor = Me.GridColor
            ElseIf Operators.ConditionalCompareObjectEqual(obj4, "off", False) Then
                Me.ShowGridToolStripMenuItem.Checked = False
                Me.OutBondPictureBox.BackColor = Color.Transparent
            End If
        End Sub

        Public Function GetINI(ByVal Section As String, ByVal Key As String) As String
            Dim lpReturnedString As String = Strings.LSet("", &H100)
            Dim lpDefault As String = ""
            MainForm.GetPrivateProfileString((Section), (Key), (lpDefault), (lpReturnedString), Strings.Len(lpReturnedString), ((Me.AppPath & "\Setting.ini")))
            Return Strings.Left(lpReturnedString, (Strings.InStr(lpReturnedString, ChrW(0), CompareMethod.Binary) - 1))
        End Function

        <DllImport("kernel32", EntryPoint:="GetPrivateProfileStringA", CharSet:=CharSet.Ansi, SetLastError:=True, ExactSpelling:=True)> _
        Private Shared Function GetPrivateProfileString(<MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpApplicationName As String, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpKeyName As String, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpDefault As String, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpReturnedString As String, ByVal nSize As Integer, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpFileName As String) As Integer
        End Function

        Private Sub GridColorToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
            If (Me.PanelColorDialog.ShowDialog = DialogResult.OK) Then
                Me.GridColor = Me.PanelColorDialog.Color
                Me.OutBondPictureBox.BackColor = Me.GridColor
            End If
        End Sub

        Private Sub HighScoresToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyProject.Forms.HighScoreDialog.Set_HighScore(Me.HighScore, Me.HighScoreName)
            MyProject.Forms.HighScoreDialog.ShowDialog
        End Sub

        Private Sub ImageToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
            Me.MainPanel.BackgroundImage = Me.PanelBackgroundImage
            Me.ImageToolStripMenuItem.Checked = True
            Me.ColorToolStripMenuItem.Checked = False
        End Sub

        Private Sub InitCollection()
            Me.NextBoxCollection.Add(Me.Next_1, Conversions.ToString(1), Nothing, Nothing)
            Me.NextBoxCollection.Add(Me.Next_2, Conversions.ToString(2), Nothing, Nothing)
            Me.NextBoxCollection.Add(Me.Next_3, Conversions.ToString(3), Nothing, Nothing)
            Me.NextBoxCollection.Add(Me.Next_4, Conversions.ToString(4), Nothing, Nothing)
            Me.NextBoxCollection.Add(Me.Next_5, Conversions.ToString(5), Nothing, Nothing)
            Me.NextBoxCollection.Add(Me.Next_6, Conversions.ToString(6), Nothing, Nothing)
            Me.NextBoxCollection.Add(Me.Next_7, Conversions.ToString(7), Nothing, Nothing)
            Me.NextBoxCollection.Add(Me.Next_8, Conversions.ToString(8), Nothing, Nothing)
            Me.NextBoxCollection.Add(Me.Next_9, Conversions.ToString(9), Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_1_1, "1_1", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_1_2, "1_2", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_1_3, "1_3", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_1_4, "1_4", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_1_5, "1_5", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_1_6, "1_6", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_1_7, "1_7", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_1_8, "1_8", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_1_9, "1_9", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_2_1, "2_1", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_2_2, "2_2", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_2_3, "2_3", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_2_4, "2_4", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_2_5, "2_5", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_2_6, "2_6", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_2_7, "2_7", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_2_8, "2_8", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_2_9, "2_9", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_3_1, "3_1", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_3_2, "3_2", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_3_3, "3_3", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_3_4, "3_4", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_3_5, "3_5", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_3_6, "3_6", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_3_7, "3_7", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_3_8, "3_8", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_3_9, "3_9", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_4_1, "4_1", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_4_2, "4_2", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_4_3, "4_3", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_4_4, "4_4", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_4_5, "4_5", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_4_6, "4_6", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_4_7, "4_7", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_4_8, "4_8", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_4_9, "4_9", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_5_1, "5_1", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_5_2, "5_2", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_5_3, "5_3", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_5_4, "5_4", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_5_5, "5_5", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_5_6, "5_6", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_5_7, "5_7", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_5_8, "5_8", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_5_9, "5_9", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_6_1, "6_1", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_6_2, "6_2", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_6_3, "6_3", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_6_4, "6_4", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_6_5, "6_5", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_6_6, "6_6", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_6_7, "6_7", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_6_8, "6_8", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_6_9, "6_9", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_7_1, "7_1", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_7_2, "7_2", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_7_3, "7_3", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_7_4, "7_4", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_7_5, "7_5", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_7_6, "7_6", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_7_7, "7_7", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_7_8, "7_8", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_7_9, "7_9", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_8_1, "8_1", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_8_2, "8_2", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_8_3, "8_3", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_8_4, "8_4", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_8_5, "8_5", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_8_6, "8_6", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_8_7, "8_7", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_8_8, "8_8", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_8_9, "8_9", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_9_1, "9_1", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_9_2, "9_2", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_9_3, "9_3", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_9_4, "9_4", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_9_5, "9_5", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_9_6, "9_6", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_9_7, "9_7", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_9_8, "9_8", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_9_9, "9_9", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_1_0, "1_0", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_2_0, "2_0", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_3_0, "3_0", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_4_0, "4_0", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_5_0, "5_0", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_6_0, "6_0", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_7_0, "7_0", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_8_0, "8_0", Nothing, Nothing)
            Me.BoxCollection.Add(Me.Box_9_0, "9_0", Nothing, Nothing)
            Me.ImageCollection.Add(RuntimeHelpers.GetObjectValue(Me.ResMana.GetObject("Pipe0")), "R", Nothing, Nothing)
            Me.ImageCollection.Add(RuntimeHelpers.GetObjectValue(Me.ResMana.GetObject("Pipe1")), "LR", Nothing, Nothing)
            Me.ImageCollection.Add(RuntimeHelpers.GetObjectValue(Me.ResMana.GetObject("Pipe2")), "UD", Nothing, Nothing)
            Me.ImageCollection.Add(RuntimeHelpers.GetObjectValue(Me.ResMana.GetObject("Pipe3")), "LUR", Nothing, Nothing)
            Me.ImageCollection.Add(RuntimeHelpers.GetObjectValue(Me.ResMana.GetObject("Pipe4")), "URD", Nothing, Nothing)
            Me.ImageCollection.Add(RuntimeHelpers.GetObjectValue(Me.ResMana.GetObject("Pipe5")), "LRD", Nothing, Nothing)
            Me.ImageCollection.Add(RuntimeHelpers.GetObjectValue(Me.ResMana.GetObject("Pipe6")), "LUD", Nothing, Nothing)
            Me.ImageCollection.Add(RuntimeHelpers.GetObjectValue(Me.ResMana.GetObject("Pipe7")), "LURD", Nothing, Nothing)
            Me.ImageCollection.Add(RuntimeHelpers.GetObjectValue(Me.ResMana.GetObject("Pipe8")), "LURDX", Nothing, Nothing)
            Me.ImageCollection.Add(RuntimeHelpers.GetObjectValue(Me.ResMana.GetObject("Pipe9")), "LU", Nothing, Nothing)
            Me.ImageCollection.Add(RuntimeHelpers.GetObjectValue(Me.ResMana.GetObject("Pipe10")), "RU", Nothing, Nothing)
            Me.ImageCollection.Add(RuntimeHelpers.GetObjectValue(Me.ResMana.GetObject("Pipe11")), "RD", Nothing, Nothing)
            Me.ImageCollection.Add(RuntimeHelpers.GetObjectValue(Me.ResMana.GetObject("Pipe12")), "LD", Nothing, Nothing)
            Me.ImageCollection.Add(RuntimeHelpers.GetObjectValue(Me.ResMana.GetObject("Pipe13")), "LURD\", Nothing, Nothing)
            Me.ImageCollection.Add(RuntimeHelpers.GetObjectValue(Me.ResMana.GetObject("Pipe14")), "LURD/", Nothing, Nothing)
            Me.AppSounds_NewGame.Stream = DirectCast(Me.ResMana.GetObject("klunk"), Stream)
            Me.AppSounds_PlaceImage.Stream = DirectCast(Me.ResMana.GetObject("Button"), Stream)
            Me.AppSounds_WaterFlow.Stream = DirectCast(Me.ResMana.GetObject("water_flow"), Stream)
            Me.AppSounds_WaterOverflow.Stream = DirectCast(Me.ResMana.GetObject("warning"), Stream)
            Me.AppSounds_Breaking.Stream = DirectCast(Me.ResMana.GetObject("glass_breaking"), Stream)
        End Sub

        <DebuggerStepThrough> _
        Private Sub InitializeComponent()
            Dim manager As New ComponentResourceManager(GetType(MainForm))
            Me.ScoreNumberLabel = New Label
            Me.ScoreBar = New ProgressBar
            Me.ScoreLabel = New Label
            Me.InWater = New Button
            Me.NextLabel = New Label
            Me.MainPanel = New Panel
            Me.Box_6_0 = New PictureBox
            Me.Box_7_0 = New PictureBox
            Me.Box_8_0 = New PictureBox
            Me.Box_9_0 = New PictureBox
            Me.Box_1_0 = New PictureBox
            Me.Box_2_0 = New PictureBox
            Me.Box_3_0 = New PictureBox
            Me.Box_4_0 = New PictureBox
            Me.Box_5_0 = New PictureBox
            Me.Box_1_1 = New PictureBox
            Me.Box_9_9 = New PictureBox
            Me.Box_9_3 = New PictureBox
            Me.Box_1_2 = New PictureBox
            Me.Box_9_4 = New PictureBox
            Me.Box_1_8 = New PictureBox
            Me.Box_9_5 = New PictureBox
            Me.Box_1_7 = New PictureBox
            Me.Box_9_6 = New PictureBox
            Me.Box_1_6 = New PictureBox
            Me.Box_9_7 = New PictureBox
            Me.Box_9_8 = New PictureBox
            Me.Box_1_4 = New PictureBox
            Me.Box_9_2 = New PictureBox
            Me.Box_1_3 = New PictureBox
            Me.Box_9_1 = New PictureBox
            Me.Box_1_9 = New PictureBox
            Me.Box_8_9 = New PictureBox
            Me.Box_8_3 = New PictureBox
            Me.Box_8_4 = New PictureBox
            Me.Box_8_5 = New PictureBox
            Me.Box_8_6 = New PictureBox
            Me.Box_2_1 = New PictureBox
            Me.Box_8_7 = New PictureBox
            Me.Box_2_2 = New PictureBox
            Me.Box_8_8 = New PictureBox
            Me.Box_2_8 = New PictureBox
            Me.Box_8_2 = New PictureBox
            Me.Box_2_7 = New PictureBox
            Me.Box_8_1 = New PictureBox
            Me.Box_2_6 = New PictureBox
            Me.Box_7_9 = New PictureBox
            Me.Box_2_5 = New PictureBox
            Me.Box_7_3 = New PictureBox
            Me.Box_2_4 = New PictureBox
            Me.Box_7_4 = New PictureBox
            Me.Box_2_3 = New PictureBox
            Me.Box_7_5 = New PictureBox
            Me.Box_2_9 = New PictureBox
            Me.Box_7_6 = New PictureBox
            Me.Box_3_1 = New PictureBox
            Me.Box_7_7 = New PictureBox
            Me.Box_3_2 = New PictureBox
            Me.Box_7_8 = New PictureBox
            Me.Box_3_8 = New PictureBox
            Me.Box_7_2 = New PictureBox
            Me.Box_3_7 = New PictureBox
            Me.Box_7_1 = New PictureBox
            Me.Box_3_6 = New PictureBox
            Me.Box_6_9 = New PictureBox
            Me.Box_3_5 = New PictureBox
            Me.Box_6_3 = New PictureBox
            Me.Box_3_4 = New PictureBox
            Me.Box_6_4 = New PictureBox
            Me.Box_3_3 = New PictureBox
            Me.Box_6_5 = New PictureBox
            Me.Box_3_9 = New PictureBox
            Me.Box_6_6 = New PictureBox
            Me.Box_4_1 = New PictureBox
            Me.Box_6_7 = New PictureBox
            Me.Box_4_2 = New PictureBox
            Me.Box_6_8 = New PictureBox
            Me.Box_4_8 = New PictureBox
            Me.Box_6_2 = New PictureBox
            Me.Box_4_7 = New PictureBox
            Me.Box_6_1 = New PictureBox
            Me.Box_4_6 = New PictureBox
            Me.Box_5_9 = New PictureBox
            Me.Box_4_5 = New PictureBox
            Me.Box_5_3 = New PictureBox
            Me.Box_4_4 = New PictureBox
            Me.Box_5_4 = New PictureBox
            Me.Box_4_3 = New PictureBox
            Me.Box_5_5 = New PictureBox
            Me.Box_4_9 = New PictureBox
            Me.Box_5_6 = New PictureBox
            Me.Box_5_1 = New PictureBox
            Me.Box_5_7 = New PictureBox
            Me.Box_5_2 = New PictureBox
            Me.Box_5_8 = New PictureBox
            Me.Box_1_5 = New PictureBox
            Me.OutBondPictureBox = New PictureBox
            Me.Next_9 = New PictureBox
            Me.Next_8 = New PictureBox
            Me.Next_7 = New PictureBox
            Me.Next_6 = New PictureBox
            Me.Next_5 = New PictureBox
            Me.Next_4 = New PictureBox
            Me.Next_3 = New PictureBox
            Me.Next_2 = New PictureBox
            Me.Next_1 = New PictureBox
            Me.MenuStrip = New MenuStrip
            Me.GameToolStripMenuItem = New ToolStripMenuItem
            Me.NewGameToolStripMenuItem = New ToolStripMenuItem
            Me.WaterGoToolStripMenuItem = New ToolStripMenuItem
            Me.ToolStripSeparator4 = New ToolStripSeparator
            Me.HighScoresToolStripMenuItem = New ToolStripMenuItem
            Me.ExitToolStripMenuItem = New ToolStripMenuItem
            Me.OptionsToolStripMenuItem = New ToolStripMenuItem
            Me.GridToolStripMenuItem = New ToolStripMenuItem
            Me.ShowGridToolStripMenuItem = New ToolStripMenuItem
            Me.ToolStripSeparator6 = New ToolStripSeparator
            Me.GridColorToolStripMenuItem = New ToolStripMenuItem
            Me.WaterColorToolStripMenuItem = New ToolStripMenuItem
            Me.FontColorToolStripMenuItem = New ToolStripMenuItem
            Me.BackgroundToolStripMenuItem = New ToolStripMenuItem
            Me.ColorToolStripMenuItem = New ToolStripMenuItem
            Me.ImageToolStripMenuItem = New ToolStripMenuItem
            Me.ToolStripSeparator5 = New ToolStripSeparator
            Me.SelectColorToolStripMenuItem = New ToolStripMenuItem
            Me.SelectImageToolStripMenuItem = New ToolStripMenuItem
            Me.ToolStripSeparator2 = New ToolStripSeparator
            Me.SoundsToolStripMenuItem = New ToolStripMenuItem
            Me.MusicToolStripMenuItem = New ToolStripMenuItem
            Me.SelectMusicToolStripMenuItem = New ToolStripMenuItem
            Me.ToolStripSeparator3 = New ToolStripSeparator
            Me.EntranceQuantityToolStripMenuItem = New ToolStripMenuItem
            Me.Entrance1ToolStripMenuItem = New ToolStripMenuItem
            Me.Entrance2ToolStripMenuItem = New ToolStripMenuItem
            Me.Entrance3ToolStripMenuItem = New ToolStripMenuItem
            Me.Entrance4ToolStripMenuItem = New ToolStripMenuItem
            Me.Entrance5ToolStripMenuItem = New ToolStripMenuItem
            Me.Entrance6ToolStripMenuItem = New ToolStripMenuItem
            Me.Entrance7ToolStripMenuItem = New ToolStripMenuItem
            Me.Entrance8ToolStripMenuItem = New ToolStripMenuItem
            Me.Entrance9ToolStripMenuItem = New ToolStripMenuItem
            Me.ToolStripSeparator7 = New ToolStripSeparator
            Me.LanguageToolStripMenuItem = New ToolStripMenuItem
            Me.EnglishToolStripMenuItem = New ToolStripMenuItem
            Me.ChineseToolStripMenuItem = New ToolStripMenuItem
            Me.HelpToolStripMenuItem = New ToolStripMenuItem
            Me.HelpTopicsToolStripMenuItem = New ToolStripMenuItem
            Me.ToolStripSeparator1 = New ToolStripSeparator
            Me.AboutWaterPipeToolStripMenuItem = New ToolStripMenuItem
            Me.PanelColorDialog = New ColorDialog
            Me.PanelBackgroundImageFileDialog = New OpenFileDialog
            Me.SelectMusicDialog = New OpenFileDialog
            Me.MainPanel.SuspendLayout
            DirectCast(Me.Box_6_0, ISupportInitialize).BeginInit
            DirectCast(Me.Box_7_0, ISupportInitialize).BeginInit
            DirectCast(Me.Box_8_0, ISupportInitialize).BeginInit
            DirectCast(Me.Box_9_0, ISupportInitialize).BeginInit
            DirectCast(Me.Box_1_0, ISupportInitialize).BeginInit
            DirectCast(Me.Box_2_0, ISupportInitialize).BeginInit
            DirectCast(Me.Box_3_0, ISupportInitialize).BeginInit
            DirectCast(Me.Box_4_0, ISupportInitialize).BeginInit
            DirectCast(Me.Box_5_0, ISupportInitialize).BeginInit
            DirectCast(Me.Box_1_1, ISupportInitialize).BeginInit
            DirectCast(Me.Box_9_9, ISupportInitialize).BeginInit
            DirectCast(Me.Box_9_3, ISupportInitialize).BeginInit
            DirectCast(Me.Box_1_2, ISupportInitialize).BeginInit
            DirectCast(Me.Box_9_4, ISupportInitialize).BeginInit
            DirectCast(Me.Box_1_8, ISupportInitialize).BeginInit
            DirectCast(Me.Box_9_5, ISupportInitialize).BeginInit
            DirectCast(Me.Box_1_7, ISupportInitialize).BeginInit
            DirectCast(Me.Box_9_6, ISupportInitialize).BeginInit
            DirectCast(Me.Box_1_6, ISupportInitialize).BeginInit
            DirectCast(Me.Box_9_7, ISupportInitialize).BeginInit
            DirectCast(Me.Box_9_8, ISupportInitialize).BeginInit
            DirectCast(Me.Box_1_4, ISupportInitialize).BeginInit
            DirectCast(Me.Box_9_2, ISupportInitialize).BeginInit
            DirectCast(Me.Box_1_3, ISupportInitialize).BeginInit
            DirectCast(Me.Box_9_1, ISupportInitialize).BeginInit
            DirectCast(Me.Box_1_9, ISupportInitialize).BeginInit
            DirectCast(Me.Box_8_9, ISupportInitialize).BeginInit
            DirectCast(Me.Box_8_3, ISupportInitialize).BeginInit
            DirectCast(Me.Box_8_4, ISupportInitialize).BeginInit
            DirectCast(Me.Box_8_5, ISupportInitialize).BeginInit
            DirectCast(Me.Box_8_6, ISupportInitialize).BeginInit
            DirectCast(Me.Box_2_1, ISupportInitialize).BeginInit
            DirectCast(Me.Box_8_7, ISupportInitialize).BeginInit
            DirectCast(Me.Box_2_2, ISupportInitialize).BeginInit
            DirectCast(Me.Box_8_8, ISupportInitialize).BeginInit
            DirectCast(Me.Box_2_8, ISupportInitialize).BeginInit
            DirectCast(Me.Box_8_2, ISupportInitialize).BeginInit
            DirectCast(Me.Box_2_7, ISupportInitialize).BeginInit
            DirectCast(Me.Box_8_1, ISupportInitialize).BeginInit
            DirectCast(Me.Box_2_6, ISupportInitialize).BeginInit
            DirectCast(Me.Box_7_9, ISupportInitialize).BeginInit
            DirectCast(Me.Box_2_5, ISupportInitialize).BeginInit
            DirectCast(Me.Box_7_3, ISupportInitialize).BeginInit
            DirectCast(Me.Box_2_4, ISupportInitialize).BeginInit
            DirectCast(Me.Box_7_4, ISupportInitialize).BeginInit
            DirectCast(Me.Box_2_3, ISupportInitialize).BeginInit
            DirectCast(Me.Box_7_5, ISupportInitialize).BeginInit
            DirectCast(Me.Box_2_9, ISupportInitialize).BeginInit
            DirectCast(Me.Box_7_6, ISupportInitialize).BeginInit
            DirectCast(Me.Box_3_1, ISupportInitialize).BeginInit
            DirectCast(Me.Box_7_7, ISupportInitialize).BeginInit
            DirectCast(Me.Box_3_2, ISupportInitialize).BeginInit
            DirectCast(Me.Box_7_8, ISupportInitialize).BeginInit
            DirectCast(Me.Box_3_8, ISupportInitialize).BeginInit
            DirectCast(Me.Box_7_2, ISupportInitialize).BeginInit
            DirectCast(Me.Box_3_7, ISupportInitialize).BeginInit
            DirectCast(Me.Box_7_1, ISupportInitialize).BeginInit
            DirectCast(Me.Box_3_6, ISupportInitialize).BeginInit
            DirectCast(Me.Box_6_9, ISupportInitialize).BeginInit
            DirectCast(Me.Box_3_5, ISupportInitialize).BeginInit
            DirectCast(Me.Box_6_3, ISupportInitialize).BeginInit
            DirectCast(Me.Box_3_4, ISupportInitialize).BeginInit
            DirectCast(Me.Box_6_4, ISupportInitialize).BeginInit
            DirectCast(Me.Box_3_3, ISupportInitialize).BeginInit
            DirectCast(Me.Box_6_5, ISupportInitialize).BeginInit
            DirectCast(Me.Box_3_9, ISupportInitialize).BeginInit
            DirectCast(Me.Box_6_6, ISupportInitialize).BeginInit
            DirectCast(Me.Box_4_1, ISupportInitialize).BeginInit
            DirectCast(Me.Box_6_7, ISupportInitialize).BeginInit
            DirectCast(Me.Box_4_2, ISupportInitialize).BeginInit
            DirectCast(Me.Box_6_8, ISupportInitialize).BeginInit
            DirectCast(Me.Box_4_8, ISupportInitialize).BeginInit
            DirectCast(Me.Box_6_2, ISupportInitialize).BeginInit
            DirectCast(Me.Box_4_7, ISupportInitialize).BeginInit
            DirectCast(Me.Box_6_1, ISupportInitialize).BeginInit
            DirectCast(Me.Box_4_6, ISupportInitialize).BeginInit
            DirectCast(Me.Box_5_9, ISupportInitialize).BeginInit
            DirectCast(Me.Box_4_5, ISupportInitialize).BeginInit
            DirectCast(Me.Box_5_3, ISupportInitialize).BeginInit
            DirectCast(Me.Box_4_4, ISupportInitialize).BeginInit
            DirectCast(Me.Box_5_4, ISupportInitialize).BeginInit
            DirectCast(Me.Box_4_3, ISupportInitialize).BeginInit
            DirectCast(Me.Box_5_5, ISupportInitialize).BeginInit
            DirectCast(Me.Box_4_9, ISupportInitialize).BeginInit
            DirectCast(Me.Box_5_6, ISupportInitialize).BeginInit
            DirectCast(Me.Box_5_1, ISupportInitialize).BeginInit
            DirectCast(Me.Box_5_7, ISupportInitialize).BeginInit
            DirectCast(Me.Box_5_2, ISupportInitialize).BeginInit
            DirectCast(Me.Box_5_8, ISupportInitialize).BeginInit
            DirectCast(Me.Box_1_5, ISupportInitialize).BeginInit
            DirectCast(Me.OutBondPictureBox, ISupportInitialize).BeginInit
            DirectCast(Me.Next_9, ISupportInitialize).BeginInit
            DirectCast(Me.Next_8, ISupportInitialize).BeginInit
            DirectCast(Me.Next_7, ISupportInitialize).BeginInit
            DirectCast(Me.Next_6, ISupportInitialize).BeginInit
            DirectCast(Me.Next_5, ISupportInitialize).BeginInit
            DirectCast(Me.Next_4, ISupportInitialize).BeginInit
            DirectCast(Me.Next_3, ISupportInitialize).BeginInit
            DirectCast(Me.Next_2, ISupportInitialize).BeginInit
            DirectCast(Me.Next_1, ISupportInitialize).BeginInit
            Me.MenuStrip.SuspendLayout
            Me.SuspendLayout
            manager.ApplyResources(Me.ScoreNumberLabel, "ScoreNumberLabel")
            Me.ScoreNumberLabel.BackColor = Color.Transparent
            Me.ScoreNumberLabel.Name = "ScoreNumberLabel"
            manager.ApplyResources(Me.ScoreBar, "ScoreBar")
            Me.ScoreBar.Name = "ScoreBar"
            manager.ApplyResources(Me.ScoreLabel, "ScoreLabel")
            Me.ScoreLabel.BackColor = Color.Transparent
            Me.ScoreLabel.Name = "ScoreLabel"
            Me.ScoreLabel.Tag = "Score"
            Me.InWater.BackColor = Color.Orange
            manager.ApplyResources(Me.InWater, "InWater")
            Me.InWater.Name = "InWater"
            Me.InWater.Tag = ""
            Me.InWater.UseVisualStyleBackColor = False
            manager.ApplyResources(Me.NextLabel, "NextLabel")
            Me.NextLabel.BackColor = Color.Transparent
            Me.NextLabel.Name = "NextLabel"
            Me.NextLabel.Tag = "Next"
            Me.MainPanel.BackColor = Color.LightSkyBlue
            manager.ApplyResources(Me.MainPanel, "MainPanel")
            Me.MainPanel.Controls.Add(Me.Box_6_0)
            Me.MainPanel.Controls.Add(Me.Box_7_0)
            Me.MainPanel.Controls.Add(Me.Box_8_0)
            Me.MainPanel.Controls.Add(Me.Box_9_0)
            Me.MainPanel.Controls.Add(Me.Box_1_0)
            Me.MainPanel.Controls.Add(Me.Box_2_0)
            Me.MainPanel.Controls.Add(Me.Box_3_0)
            Me.MainPanel.Controls.Add(Me.Box_4_0)
            Me.MainPanel.Controls.Add(Me.Box_5_0)
            Me.MainPanel.Controls.Add(Me.Box_1_1)
            Me.MainPanel.Controls.Add(Me.Box_9_9)
            Me.MainPanel.Controls.Add(Me.Box_9_3)
            Me.MainPanel.Controls.Add(Me.Box_1_2)
            Me.MainPanel.Controls.Add(Me.Box_9_4)
            Me.MainPanel.Controls.Add(Me.Box_1_8)
            Me.MainPanel.Controls.Add(Me.Box_9_5)
            Me.MainPanel.Controls.Add(Me.Box_1_7)
            Me.MainPanel.Controls.Add(Me.Box_9_6)
            Me.MainPanel.Controls.Add(Me.Box_1_6)
            Me.MainPanel.Controls.Add(Me.Box_9_7)
            Me.MainPanel.Controls.Add(Me.Box_9_8)
            Me.MainPanel.Controls.Add(Me.Box_1_4)
            Me.MainPanel.Controls.Add(Me.Box_9_2)
            Me.MainPanel.Controls.Add(Me.Box_1_3)
            Me.MainPanel.Controls.Add(Me.Box_9_1)
            Me.MainPanel.Controls.Add(Me.Box_1_9)
            Me.MainPanel.Controls.Add(Me.Box_8_9)
            Me.MainPanel.Controls.Add(Me.Box_8_3)
            Me.MainPanel.Controls.Add(Me.Box_8_4)
            Me.MainPanel.Controls.Add(Me.Box_8_5)
            Me.MainPanel.Controls.Add(Me.Box_8_6)
            Me.MainPanel.Controls.Add(Me.Box_2_1)
            Me.MainPanel.Controls.Add(Me.Box_8_7)
            Me.MainPanel.Controls.Add(Me.Box_2_2)
            Me.MainPanel.Controls.Add(Me.Box_8_8)
            Me.MainPanel.Controls.Add(Me.Box_2_8)
            Me.MainPanel.Controls.Add(Me.Box_8_2)
            Me.MainPanel.Controls.Add(Me.Box_2_7)
            Me.MainPanel.Controls.Add(Me.Box_8_1)
            Me.MainPanel.Controls.Add(Me.Box_2_6)
            Me.MainPanel.Controls.Add(Me.Box_7_9)
            Me.MainPanel.Controls.Add(Me.Box_2_5)
            Me.MainPanel.Controls.Add(Me.Box_7_3)
            Me.MainPanel.Controls.Add(Me.Box_2_4)
            Me.MainPanel.Controls.Add(Me.Box_7_4)
            Me.MainPanel.Controls.Add(Me.Box_2_3)
            Me.MainPanel.Controls.Add(Me.Box_7_5)
            Me.MainPanel.Controls.Add(Me.Box_2_9)
            Me.MainPanel.Controls.Add(Me.Box_7_6)
            Me.MainPanel.Controls.Add(Me.Box_3_1)
            Me.MainPanel.Controls.Add(Me.Box_7_7)
            Me.MainPanel.Controls.Add(Me.Box_3_2)
            Me.MainPanel.Controls.Add(Me.Box_7_8)
            Me.MainPanel.Controls.Add(Me.Box_3_8)
            Me.MainPanel.Controls.Add(Me.Box_7_2)
            Me.MainPanel.Controls.Add(Me.Box_3_7)
            Me.MainPanel.Controls.Add(Me.Box_7_1)
            Me.MainPanel.Controls.Add(Me.Box_3_6)
            Me.MainPanel.Controls.Add(Me.Box_6_9)
            Me.MainPanel.Controls.Add(Me.Box_3_5)
            Me.MainPanel.Controls.Add(Me.Box_6_3)
            Me.MainPanel.Controls.Add(Me.Box_3_4)
            Me.MainPanel.Controls.Add(Me.Box_6_4)
            Me.MainPanel.Controls.Add(Me.Box_3_3)
            Me.MainPanel.Controls.Add(Me.Box_6_5)
            Me.MainPanel.Controls.Add(Me.Box_3_9)
            Me.MainPanel.Controls.Add(Me.Box_6_6)
            Me.MainPanel.Controls.Add(Me.Box_4_1)
            Me.MainPanel.Controls.Add(Me.Box_6_7)
            Me.MainPanel.Controls.Add(Me.Box_4_2)
            Me.MainPanel.Controls.Add(Me.Box_6_8)
            Me.MainPanel.Controls.Add(Me.Box_4_8)
            Me.MainPanel.Controls.Add(Me.Box_6_2)
            Me.MainPanel.Controls.Add(Me.Box_4_7)
            Me.MainPanel.Controls.Add(Me.Box_6_1)
            Me.MainPanel.Controls.Add(Me.Box_4_6)
            Me.MainPanel.Controls.Add(Me.Box_5_9)
            Me.MainPanel.Controls.Add(Me.Box_4_5)
            Me.MainPanel.Controls.Add(Me.Box_5_3)
            Me.MainPanel.Controls.Add(Me.Box_4_4)
            Me.MainPanel.Controls.Add(Me.Box_5_4)
            Me.MainPanel.Controls.Add(Me.Box_4_3)
            Me.MainPanel.Controls.Add(Me.Box_5_5)
            Me.MainPanel.Controls.Add(Me.Box_4_9)
            Me.MainPanel.Controls.Add(Me.Box_5_6)
            Me.MainPanel.Controls.Add(Me.Box_5_1)
            Me.MainPanel.Controls.Add(Me.Box_5_7)
            Me.MainPanel.Controls.Add(Me.Box_5_2)
            Me.MainPanel.Controls.Add(Me.Box_5_8)
            Me.MainPanel.Controls.Add(Me.Box_1_5)
            Me.MainPanel.Controls.Add(Me.OutBondPictureBox)
            Me.MainPanel.Controls.Add(Me.NextLabel)
            Me.MainPanel.Controls.Add(Me.Next_9)
            Me.MainPanel.Controls.Add(Me.Next_8)
            Me.MainPanel.Controls.Add(Me.Next_7)
            Me.MainPanel.Controls.Add(Me.Next_6)
            Me.MainPanel.Controls.Add(Me.Next_5)
            Me.MainPanel.Controls.Add(Me.Next_4)
            Me.MainPanel.Controls.Add(Me.Next_3)
            Me.MainPanel.Controls.Add(Me.Next_2)
            Me.MainPanel.Controls.Add(Me.Next_1)
            Me.MainPanel.Controls.Add(Me.InWater)
            Me.MainPanel.Controls.Add(Me.ScoreLabel)
            Me.MainPanel.Controls.Add(Me.ScoreBar)
            Me.MainPanel.Controls.Add(Me.ScoreNumberLabel)
            Me.MainPanel.Controls.Add(Me.MenuStrip)
            Me.MainPanel.ForeColor = SystemColors.ControlText
            Me.MainPanel.Name = "MainPanel"
            Me.Box_6_0.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_6_0, "Box_6_0")
            Me.Box_6_0.Name = "Box_6_0"
            Me.Box_6_0.TabStop = False
            Me.Box_6_0.Tag = "L"
            Me.Box_7_0.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_7_0, "Box_7_0")
            Me.Box_7_0.Name = "Box_7_0"
            Me.Box_7_0.TabStop = False
            Me.Box_7_0.Tag = "L"
            Me.Box_8_0.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_8_0, "Box_8_0")
            Me.Box_8_0.Name = "Box_8_0"
            Me.Box_8_0.TabStop = False
            Me.Box_8_0.Tag = "L"
            Me.Box_9_0.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_9_0, "Box_9_0")
            Me.Box_9_0.Name = "Box_9_0"
            Me.Box_9_0.TabStop = False
            Me.Box_9_0.Tag = "L"
            Me.Box_1_0.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_1_0, "Box_1_0")
            Me.Box_1_0.Name = "Box_1_0"
            Me.Box_1_0.TabStop = False
            Me.Box_1_0.Tag = "L"
            Me.Box_2_0.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_2_0, "Box_2_0")
            Me.Box_2_0.Name = "Box_2_0"
            Me.Box_2_0.TabStop = False
            Me.Box_2_0.Tag = "L"
            Me.Box_3_0.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_3_0, "Box_3_0")
            Me.Box_3_0.Name = "Box_3_0"
            Me.Box_3_0.TabStop = False
            Me.Box_3_0.Tag = "L"
            Me.Box_4_0.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_4_0, "Box_4_0")
            Me.Box_4_0.Name = "Box_4_0"
            Me.Box_4_0.TabStop = False
            Me.Box_4_0.Tag = "L"
            Me.Box_5_0.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_5_0, "Box_5_0")
            Me.Box_5_0.Name = "Box_5_0"
            Me.Box_5_0.TabStop = False
            Me.Box_5_0.Tag = "L"
            Me.Box_1_1.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_1_1, "Box_1_1")
            Me.Box_1_1.Name = "Box_1_1"
            Me.Box_1_1.TabStop = False
            Me.Box_9_9.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_9_9, "Box_9_9")
            Me.Box_9_9.Name = "Box_9_9"
            Me.Box_9_9.TabStop = False
            Me.Box_9_3.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_9_3, "Box_9_3")
            Me.Box_9_3.Name = "Box_9_3"
            Me.Box_9_3.TabStop = False
            Me.Box_1_2.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_1_2, "Box_1_2")
            Me.Box_1_2.Name = "Box_1_2"
            Me.Box_1_2.TabStop = False
            Me.Box_9_4.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_9_4, "Box_9_4")
            Me.Box_9_4.Name = "Box_9_4"
            Me.Box_9_4.TabStop = False
            Me.Box_1_8.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_1_8, "Box_1_8")
            Me.Box_1_8.Name = "Box_1_8"
            Me.Box_1_8.TabStop = False
            Me.Box_9_5.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_9_5, "Box_9_5")
            Me.Box_9_5.Name = "Box_9_5"
            Me.Box_9_5.TabStop = False
            Me.Box_1_7.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_1_7, "Box_1_7")
            Me.Box_1_7.Name = "Box_1_7"
            Me.Box_1_7.TabStop = False
            Me.Box_9_6.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_9_6, "Box_9_6")
            Me.Box_9_6.Name = "Box_9_6"
            Me.Box_9_6.TabStop = False
            Me.Box_1_6.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_1_6, "Box_1_6")
            Me.Box_1_6.Name = "Box_1_6"
            Me.Box_1_6.TabStop = False
            Me.Box_9_7.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_9_7, "Box_9_7")
            Me.Box_9_7.Name = "Box_9_7"
            Me.Box_9_7.TabStop = False
            Me.Box_9_8.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_9_8, "Box_9_8")
            Me.Box_9_8.Name = "Box_9_8"
            Me.Box_9_8.TabStop = False
            Me.Box_1_4.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_1_4, "Box_1_4")
            Me.Box_1_4.Name = "Box_1_4"
            Me.Box_1_4.TabStop = False
            Me.Box_9_2.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_9_2, "Box_9_2")
            Me.Box_9_2.Name = "Box_9_2"
            Me.Box_9_2.TabStop = False
            Me.Box_1_3.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_1_3, "Box_1_3")
            Me.Box_1_3.Name = "Box_1_3"
            Me.Box_1_3.TabStop = False
            Me.Box_9_1.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_9_1, "Box_9_1")
            Me.Box_9_1.Name = "Box_9_1"
            Me.Box_9_1.TabStop = False
            Me.Box_1_9.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_1_9, "Box_1_9")
            Me.Box_1_9.Name = "Box_1_9"
            Me.Box_1_9.TabStop = False
            Me.Box_8_9.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_8_9, "Box_8_9")
            Me.Box_8_9.Name = "Box_8_9"
            Me.Box_8_9.TabStop = False
            Me.Box_8_3.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_8_3, "Box_8_3")
            Me.Box_8_3.Name = "Box_8_3"
            Me.Box_8_3.TabStop = False
            Me.Box_8_4.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_8_4, "Box_8_4")
            Me.Box_8_4.Name = "Box_8_4"
            Me.Box_8_4.TabStop = False
            Me.Box_8_5.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_8_5, "Box_8_5")
            Me.Box_8_5.Name = "Box_8_5"
            Me.Box_8_5.TabStop = False
            Me.Box_8_6.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_8_6, "Box_8_6")
            Me.Box_8_6.Name = "Box_8_6"
            Me.Box_8_6.TabStop = False
            Me.Box_2_1.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_2_1, "Box_2_1")
            Me.Box_2_1.Name = "Box_2_1"
            Me.Box_2_1.TabStop = False
            Me.Box_8_7.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_8_7, "Box_8_7")
            Me.Box_8_7.Name = "Box_8_7"
            Me.Box_8_7.TabStop = False
            Me.Box_2_2.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_2_2, "Box_2_2")
            Me.Box_2_2.Name = "Box_2_2"
            Me.Box_2_2.TabStop = False
            Me.Box_8_8.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_8_8, "Box_8_8")
            Me.Box_8_8.Name = "Box_8_8"
            Me.Box_8_8.TabStop = False
            Me.Box_2_8.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_2_8, "Box_2_8")
            Me.Box_2_8.Name = "Box_2_8"
            Me.Box_2_8.TabStop = False
            Me.Box_8_2.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_8_2, "Box_8_2")
            Me.Box_8_2.Name = "Box_8_2"
            Me.Box_8_2.TabStop = False
            Me.Box_2_7.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_2_7, "Box_2_7")
            Me.Box_2_7.Name = "Box_2_7"
            Me.Box_2_7.TabStop = False
            Me.Box_8_1.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_8_1, "Box_8_1")
            Me.Box_8_1.Name = "Box_8_1"
            Me.Box_8_1.TabStop = False
            Me.Box_2_6.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_2_6, "Box_2_6")
            Me.Box_2_6.Name = "Box_2_6"
            Me.Box_2_6.TabStop = False
            Me.Box_7_9.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_7_9, "Box_7_9")
            Me.Box_7_9.Name = "Box_7_9"
            Me.Box_7_9.TabStop = False
            Me.Box_2_5.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_2_5, "Box_2_5")
            Me.Box_2_5.Name = "Box_2_5"
            Me.Box_2_5.TabStop = False
            Me.Box_7_3.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_7_3, "Box_7_3")
            Me.Box_7_3.Name = "Box_7_3"
            Me.Box_7_3.TabStop = False
            Me.Box_2_4.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_2_4, "Box_2_4")
            Me.Box_2_4.Name = "Box_2_4"
            Me.Box_2_4.TabStop = False
            Me.Box_7_4.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_7_4, "Box_7_4")
            Me.Box_7_4.Name = "Box_7_4"
            Me.Box_7_4.TabStop = False
            Me.Box_2_3.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_2_3, "Box_2_3")
            Me.Box_2_3.Name = "Box_2_3"
            Me.Box_2_3.TabStop = False
            Me.Box_7_5.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_7_5, "Box_7_5")
            Me.Box_7_5.Name = "Box_7_5"
            Me.Box_7_5.TabStop = False
            Me.Box_2_9.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_2_9, "Box_2_9")
            Me.Box_2_9.Name = "Box_2_9"
            Me.Box_2_9.TabStop = False
            Me.Box_7_6.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_7_6, "Box_7_6")
            Me.Box_7_6.Name = "Box_7_6"
            Me.Box_7_6.TabStop = False
            Me.Box_3_1.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_3_1, "Box_3_1")
            Me.Box_3_1.Name = "Box_3_1"
            Me.Box_3_1.TabStop = False
            Me.Box_7_7.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_7_7, "Box_7_7")
            Me.Box_7_7.Name = "Box_7_7"
            Me.Box_7_7.TabStop = False
            Me.Box_3_2.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_3_2, "Box_3_2")
            Me.Box_3_2.Name = "Box_3_2"
            Me.Box_3_2.TabStop = False
            Me.Box_7_8.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_7_8, "Box_7_8")
            Me.Box_7_8.Name = "Box_7_8"
            Me.Box_7_8.TabStop = False
            Me.Box_3_8.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_3_8, "Box_3_8")
            Me.Box_3_8.Name = "Box_3_8"
            Me.Box_3_8.TabStop = False
            Me.Box_7_2.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_7_2, "Box_7_2")
            Me.Box_7_2.Name = "Box_7_2"
            Me.Box_7_2.TabStop = False
            Me.Box_3_7.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_3_7, "Box_3_7")
            Me.Box_3_7.Name = "Box_3_7"
            Me.Box_3_7.TabStop = False
            Me.Box_7_1.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_7_1, "Box_7_1")
            Me.Box_7_1.Name = "Box_7_1"
            Me.Box_7_1.TabStop = False
            Me.Box_3_6.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_3_6, "Box_3_6")
            Me.Box_3_6.Name = "Box_3_6"
            Me.Box_3_6.TabStop = False
            Me.Box_6_9.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_6_9, "Box_6_9")
            Me.Box_6_9.Name = "Box_6_9"
            Me.Box_6_9.TabStop = False
            Me.Box_3_5.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_3_5, "Box_3_5")
            Me.Box_3_5.Name = "Box_3_5"
            Me.Box_3_5.TabStop = False
            Me.Box_6_3.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_6_3, "Box_6_3")
            Me.Box_6_3.Name = "Box_6_3"
            Me.Box_6_3.TabStop = False
            Me.Box_3_4.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_3_4, "Box_3_4")
            Me.Box_3_4.Name = "Box_3_4"
            Me.Box_3_4.TabStop = False
            Me.Box_6_4.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_6_4, "Box_6_4")
            Me.Box_6_4.Name = "Box_6_4"
            Me.Box_6_4.TabStop = False
            Me.Box_3_3.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_3_3, "Box_3_3")
            Me.Box_3_3.Name = "Box_3_3"
            Me.Box_3_3.TabStop = False
            Me.Box_6_5.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_6_5, "Box_6_5")
            Me.Box_6_5.Name = "Box_6_5"
            Me.Box_6_5.TabStop = False
            Me.Box_3_9.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_3_9, "Box_3_9")
            Me.Box_3_9.Name = "Box_3_9"
            Me.Box_3_9.TabStop = False
            Me.Box_6_6.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_6_6, "Box_6_6")
            Me.Box_6_6.Name = "Box_6_6"
            Me.Box_6_6.TabStop = False
            Me.Box_4_1.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_4_1, "Box_4_1")
            Me.Box_4_1.Name = "Box_4_1"
            Me.Box_4_1.TabStop = False
            Me.Box_6_7.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_6_7, "Box_6_7")
            Me.Box_6_7.Name = "Box_6_7"
            Me.Box_6_7.TabStop = False
            Me.Box_4_2.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_4_2, "Box_4_2")
            Me.Box_4_2.Name = "Box_4_2"
            Me.Box_4_2.TabStop = False
            Me.Box_6_8.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_6_8, "Box_6_8")
            Me.Box_6_8.Name = "Box_6_8"
            Me.Box_6_8.TabStop = False
            Me.Box_4_8.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_4_8, "Box_4_8")
            Me.Box_4_8.Name = "Box_4_8"
            Me.Box_4_8.TabStop = False
            Me.Box_6_2.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_6_2, "Box_6_2")
            Me.Box_6_2.Name = "Box_6_2"
            Me.Box_6_2.TabStop = False
            Me.Box_4_7.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_4_7, "Box_4_7")
            Me.Box_4_7.Name = "Box_4_7"
            Me.Box_4_7.TabStop = False
            Me.Box_6_1.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_6_1, "Box_6_1")
            Me.Box_6_1.Name = "Box_6_1"
            Me.Box_6_1.TabStop = False
            Me.Box_4_6.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_4_6, "Box_4_6")
            Me.Box_4_6.Name = "Box_4_6"
            Me.Box_4_6.TabStop = False
            Me.Box_5_9.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_5_9, "Box_5_9")
            Me.Box_5_9.Name = "Box_5_9"
            Me.Box_5_9.TabStop = False
            Me.Box_4_5.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_4_5, "Box_4_5")
            Me.Box_4_5.Name = "Box_4_5"
            Me.Box_4_5.TabStop = False
            Me.Box_5_3.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_5_3, "Box_5_3")
            Me.Box_5_3.Name = "Box_5_3"
            Me.Box_5_3.TabStop = False
            Me.Box_4_4.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_4_4, "Box_4_4")
            Me.Box_4_4.Name = "Box_4_4"
            Me.Box_4_4.TabStop = False
            Me.Box_5_4.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_5_4, "Box_5_4")
            Me.Box_5_4.Name = "Box_5_4"
            Me.Box_5_4.TabStop = False
            Me.Box_4_3.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_4_3, "Box_4_3")
            Me.Box_4_3.Name = "Box_4_3"
            Me.Box_4_3.TabStop = False
            Me.Box_5_5.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_5_5, "Box_5_5")
            Me.Box_5_5.Name = "Box_5_5"
            Me.Box_5_5.TabStop = False
            Me.Box_4_9.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_4_9, "Box_4_9")
            Me.Box_4_9.Name = "Box_4_9"
            Me.Box_4_9.TabStop = False
            Me.Box_5_6.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_5_6, "Box_5_6")
            Me.Box_5_6.Name = "Box_5_6"
            Me.Box_5_6.TabStop = False
            Me.Box_5_1.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_5_1, "Box_5_1")
            Me.Box_5_1.Name = "Box_5_1"
            Me.Box_5_1.TabStop = False
            Me.Box_5_7.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_5_7, "Box_5_7")
            Me.Box_5_7.Name = "Box_5_7"
            Me.Box_5_7.TabStop = False
            Me.Box_5_2.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_5_2, "Box_5_2")
            Me.Box_5_2.Name = "Box_5_2"
            Me.Box_5_2.TabStop = False
            Me.Box_5_8.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_5_8, "Box_5_8")
            Me.Box_5_8.Name = "Box_5_8"
            Me.Box_5_8.TabStop = False
            Me.Box_1_5.BackColor = Color.Transparent
            manager.ApplyResources(Me.Box_1_5, "Box_1_5")
            Me.Box_1_5.Name = "Box_1_5"
            Me.Box_1_5.TabStop = False
            Me.OutBondPictureBox.BackColor = Color.DeepSkyBlue
            manager.ApplyResources(Me.OutBondPictureBox, "OutBondPictureBox")
            Me.OutBondPictureBox.Name = "OutBondPictureBox"
            Me.OutBondPictureBox.TabStop = False
            Me.Next_9.BackColor = Color.Transparent
            manager.ApplyResources(Me.Next_9, "Next_9")
            Me.Next_9.Name = "Next_9"
            Me.Next_9.TabStop = False
            Me.Next_8.BackColor = Color.Transparent
            manager.ApplyResources(Me.Next_8, "Next_8")
            Me.Next_8.Name = "Next_8"
            Me.Next_8.TabStop = False
            Me.Next_7.BackColor = Color.Transparent
            manager.ApplyResources(Me.Next_7, "Next_7")
            Me.Next_7.Name = "Next_7"
            Me.Next_7.TabStop = False
            Me.Next_6.BackColor = Color.Transparent
            manager.ApplyResources(Me.Next_6, "Next_6")
            Me.Next_6.Name = "Next_6"
            Me.Next_6.TabStop = False
            Me.Next_5.BackColor = Color.Transparent
            manager.ApplyResources(Me.Next_5, "Next_5")
            Me.Next_5.Name = "Next_5"
            Me.Next_5.TabStop = False
            Me.Next_4.BackColor = Color.Transparent
            manager.ApplyResources(Me.Next_4, "Next_4")
            Me.Next_4.Name = "Next_4"
            Me.Next_4.TabStop = False
            Me.Next_3.BackColor = Color.Transparent
            manager.ApplyResources(Me.Next_3, "Next_3")
            Me.Next_3.Name = "Next_3"
            Me.Next_3.TabStop = False
            Me.Next_2.BackColor = Color.Transparent
            manager.ApplyResources(Me.Next_2, "Next_2")
            Me.Next_2.Name = "Next_2"
            Me.Next_2.TabStop = False
            Me.Next_1.BackColor = Color.Transparent
            manager.ApplyResources(Me.Next_1, "Next_1")
            Me.Next_1.Name = "Next_1"
            Me.Next_1.TabStop = False
            Me.MenuStrip.BackColor = SystemColors.Control
            Me.MenuStrip.Items.AddRange(New ToolStripItem() { Me.GameToolStripMenuItem, Me.OptionsToolStripMenuItem, Me.HelpToolStripMenuItem })
            manager.ApplyResources(Me.MenuStrip, "MenuStrip")
            Me.MenuStrip.Name = "MenuStrip"
            Me.MenuStrip.RenderMode = ToolStripRenderMode.System
            Me.GameToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() { Me.NewGameToolStripMenuItem, Me.WaterGoToolStripMenuItem, Me.ToolStripSeparator4, Me.HighScoresToolStripMenuItem, Me.ExitToolStripMenuItem })
            Me.GameToolStripMenuItem.Name = "GameToolStripMenuItem"
            manager.ApplyResources(Me.GameToolStripMenuItem, "GameToolStripMenuItem")
            Me.GameToolStripMenuItem.Tag = "Game"
            Me.NewGameToolStripMenuItem.Name = "NewGameToolStripMenuItem"
            manager.ApplyResources(Me.NewGameToolStripMenuItem, "NewGameToolStripMenuItem")
            Me.NewGameToolStripMenuItem.Tag = "NewGame"
            Me.WaterGoToolStripMenuItem.Name = "WaterGoToolStripMenuItem"
            manager.ApplyResources(Me.WaterGoToolStripMenuItem, "WaterGoToolStripMenuItem")
            Me.WaterGoToolStripMenuItem.Tag = "WaterGo"
            Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
            manager.ApplyResources(Me.ToolStripSeparator4, "ToolStripSeparator4")
            Me.HighScoresToolStripMenuItem.Name = "HighScoresToolStripMenuItem"
            manager.ApplyResources(Me.HighScoresToolStripMenuItem, "HighScoresToolStripMenuItem")
            Me.HighScoresToolStripMenuItem.Tag = "HighScores"
            Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
            manager.ApplyResources(Me.ExitToolStripMenuItem, "ExitToolStripMenuItem")
            Me.ExitToolStripMenuItem.Tag = "Exit"
            Me.OptionsToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() { Me.GridToolStripMenuItem, Me.WaterColorToolStripMenuItem, Me.FontColorToolStripMenuItem, Me.BackgroundToolStripMenuItem, Me.ToolStripSeparator2, Me.SoundsToolStripMenuItem, Me.MusicToolStripMenuItem, Me.SelectMusicToolStripMenuItem, Me.ToolStripSeparator3, Me.EntranceQuantityToolStripMenuItem, Me.ToolStripSeparator7, Me.LanguageToolStripMenuItem })
            Me.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
            manager.ApplyResources(Me.OptionsToolStripMenuItem, "OptionsToolStripMenuItem")
            Me.OptionsToolStripMenuItem.Tag = "Options"
            Me.GridToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() { Me.ShowGridToolStripMenuItem, Me.ToolStripSeparator6, Me.GridColorToolStripMenuItem })
            Me.GridToolStripMenuItem.Name = "GridToolStripMenuItem"
            manager.ApplyResources(Me.GridToolStripMenuItem, "GridToolStripMenuItem")
            Me.GridToolStripMenuItem.Tag = "Grid"
            Me.ShowGridToolStripMenuItem.Checked = True
            Me.ShowGridToolStripMenuItem.CheckOnClick = True
            Me.ShowGridToolStripMenuItem.CheckState = CheckState.Checked
            Me.ShowGridToolStripMenuItem.Name = "ShowGridToolStripMenuItem"
            manager.ApplyResources(Me.ShowGridToolStripMenuItem, "ShowGridToolStripMenuItem")
            Me.ShowGridToolStripMenuItem.Tag = "ShowGrid"
            Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
            manager.ApplyResources(Me.ToolStripSeparator6, "ToolStripSeparator6")
            Me.GridColorToolStripMenuItem.Name = "GridColorToolStripMenuItem"
            manager.ApplyResources(Me.GridColorToolStripMenuItem, "GridColorToolStripMenuItem")
            Me.GridColorToolStripMenuItem.Tag = "SelectColor"
            Me.WaterColorToolStripMenuItem.Name = "WaterColorToolStripMenuItem"
            manager.ApplyResources(Me.WaterColorToolStripMenuItem, "WaterColorToolStripMenuItem")
            Me.WaterColorToolStripMenuItem.Tag = "WaterColor"
            Me.FontColorToolStripMenuItem.Name = "FontColorToolStripMenuItem"
            manager.ApplyResources(Me.FontColorToolStripMenuItem, "FontColorToolStripMenuItem")
            Me.FontColorToolStripMenuItem.Tag = "FontColor"
            Me.BackgroundToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() { Me.ColorToolStripMenuItem, Me.ImageToolStripMenuItem, Me.ToolStripSeparator5, Me.SelectColorToolStripMenuItem, Me.SelectImageToolStripMenuItem })
            Me.BackgroundToolStripMenuItem.Name = "BackgroundToolStripMenuItem"
            manager.ApplyResources(Me.BackgroundToolStripMenuItem, "BackgroundToolStripMenuItem")
            Me.BackgroundToolStripMenuItem.Tag = "Background"
            Me.ColorToolStripMenuItem.Checked = True
            Me.ColorToolStripMenuItem.CheckState = CheckState.Checked
            Me.ColorToolStripMenuItem.Name = "ColorToolStripMenuItem"
            manager.ApplyResources(Me.ColorToolStripMenuItem, "ColorToolStripMenuItem")
            Me.ColorToolStripMenuItem.Tag = "Color"
            Me.ImageToolStripMenuItem.Name = "ImageToolStripMenuItem"
            manager.ApplyResources(Me.ImageToolStripMenuItem, "ImageToolStripMenuItem")
            Me.ImageToolStripMenuItem.Tag = "Image"
            Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
            manager.ApplyResources(Me.ToolStripSeparator5, "ToolStripSeparator5")
            Me.SelectColorToolStripMenuItem.Name = "SelectColorToolStripMenuItem"
            manager.ApplyResources(Me.SelectColorToolStripMenuItem, "SelectColorToolStripMenuItem")
            Me.SelectColorToolStripMenuItem.Tag = "SelectColor"
            Me.SelectImageToolStripMenuItem.Name = "SelectImageToolStripMenuItem"
            manager.ApplyResources(Me.SelectImageToolStripMenuItem, "SelectImageToolStripMenuItem")
            Me.SelectImageToolStripMenuItem.Tag = "SelectImage"
            Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
            manager.ApplyResources(Me.ToolStripSeparator2, "ToolStripSeparator2")
            Me.SoundsToolStripMenuItem.Checked = True
            Me.SoundsToolStripMenuItem.CheckOnClick = True
            Me.SoundsToolStripMenuItem.CheckState = CheckState.Checked
            Me.SoundsToolStripMenuItem.Name = "SoundsToolStripMenuItem"
            manager.ApplyResources(Me.SoundsToolStripMenuItem, "SoundsToolStripMenuItem")
            Me.SoundsToolStripMenuItem.Tag = "Sounds"
            Me.MusicToolStripMenuItem.Checked = True
            Me.MusicToolStripMenuItem.CheckOnClick = True
            Me.MusicToolStripMenuItem.CheckState = CheckState.Checked
            Me.MusicToolStripMenuItem.Name = "MusicToolStripMenuItem"
            manager.ApplyResources(Me.MusicToolStripMenuItem, "MusicToolStripMenuItem")
            Me.MusicToolStripMenuItem.Tag = "Music"
            Me.SelectMusicToolStripMenuItem.Name = "SelectMusicToolStripMenuItem"
            manager.ApplyResources(Me.SelectMusicToolStripMenuItem, "SelectMusicToolStripMenuItem")
            Me.SelectMusicToolStripMenuItem.Tag = "SelectMusic"
            Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
            manager.ApplyResources(Me.ToolStripSeparator3, "ToolStripSeparator3")
            Me.EntranceQuantityToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() { Me.Entrance1ToolStripMenuItem, Me.Entrance2ToolStripMenuItem, Me.Entrance3ToolStripMenuItem, Me.Entrance4ToolStripMenuItem, Me.Entrance5ToolStripMenuItem, Me.Entrance6ToolStripMenuItem, Me.Entrance7ToolStripMenuItem, Me.Entrance8ToolStripMenuItem, Me.Entrance9ToolStripMenuItem })
            Me.EntranceQuantityToolStripMenuItem.Name = "EntranceQuantityToolStripMenuItem"
            manager.ApplyResources(Me.EntranceQuantityToolStripMenuItem, "EntranceQuantityToolStripMenuItem")
            Me.EntranceQuantityToolStripMenuItem.Tag = "EntranceQuantity"
            Me.Entrance1ToolStripMenuItem.Checked = True
            Me.Entrance1ToolStripMenuItem.CheckState = CheckState.Checked
            Me.Entrance1ToolStripMenuItem.Name = "Entrance1ToolStripMenuItem"
            manager.ApplyResources(Me.Entrance1ToolStripMenuItem, "Entrance1ToolStripMenuItem")
            Me.Entrance2ToolStripMenuItem.Name = "Entrance2ToolStripMenuItem"
            manager.ApplyResources(Me.Entrance2ToolStripMenuItem, "Entrance2ToolStripMenuItem")
            Me.Entrance3ToolStripMenuItem.Name = "Entrance3ToolStripMenuItem"
            manager.ApplyResources(Me.Entrance3ToolStripMenuItem, "Entrance3ToolStripMenuItem")
            Me.Entrance4ToolStripMenuItem.Name = "Entrance4ToolStripMenuItem"
            manager.ApplyResources(Me.Entrance4ToolStripMenuItem, "Entrance4ToolStripMenuItem")
            Me.Entrance5ToolStripMenuItem.Name = "Entrance5ToolStripMenuItem"
            manager.ApplyResources(Me.Entrance5ToolStripMenuItem, "Entrance5ToolStripMenuItem")
            Me.Entrance6ToolStripMenuItem.Name = "Entrance6ToolStripMenuItem"
            manager.ApplyResources(Me.Entrance6ToolStripMenuItem, "Entrance6ToolStripMenuItem")
            Me.Entrance7ToolStripMenuItem.Name = "Entrance7ToolStripMenuItem"
            manager.ApplyResources(Me.Entrance7ToolStripMenuItem, "Entrance7ToolStripMenuItem")
            Me.Entrance8ToolStripMenuItem.Name = "Entrance8ToolStripMenuItem"
            manager.ApplyResources(Me.Entrance8ToolStripMenuItem, "Entrance8ToolStripMenuItem")
            Me.Entrance9ToolStripMenuItem.Name = "Entrance9ToolStripMenuItem"
            manager.ApplyResources(Me.Entrance9ToolStripMenuItem, "Entrance9ToolStripMenuItem")
            Me.ToolStripSeparator7.Name = "ToolStripSeparator7"
            manager.ApplyResources(Me.ToolStripSeparator7, "ToolStripSeparator7")
            Me.LanguageToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() { Me.EnglishToolStripMenuItem, Me.ChineseToolStripMenuItem })
            Me.LanguageToolStripMenuItem.Name = "LanguageToolStripMenuItem"
            manager.ApplyResources(Me.LanguageToolStripMenuItem, "LanguageToolStripMenuItem")
            Me.LanguageToolStripMenuItem.Tag = "Language"
            Me.EnglishToolStripMenuItem.Checked = True
            Me.EnglishToolStripMenuItem.CheckState = CheckState.Checked
            Me.EnglishToolStripMenuItem.Name = "EnglishToolStripMenuItem"
            manager.ApplyResources(Me.EnglishToolStripMenuItem, "EnglishToolStripMenuItem")
            Me.EnglishToolStripMenuItem.Tag = "en"
            Me.ChineseToolStripMenuItem.Name = "ChineseToolStripMenuItem"
            manager.ApplyResources(Me.ChineseToolStripMenuItem, "ChineseToolStripMenuItem")
            Me.ChineseToolStripMenuItem.Tag = "cn"
            Me.HelpToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() { Me.HelpTopicsToolStripMenuItem, Me.ToolStripSeparator1, Me.AboutWaterPipeToolStripMenuItem })
            Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
            manager.ApplyResources(Me.HelpToolStripMenuItem, "HelpToolStripMenuItem")
            Me.HelpToolStripMenuItem.Tag = "Help"
            Me.HelpTopicsToolStripMenuItem.Name = "HelpTopicsToolStripMenuItem"
            manager.ApplyResources(Me.HelpTopicsToolStripMenuItem, "HelpTopicsToolStripMenuItem")
            Me.HelpTopicsToolStripMenuItem.Tag = "HelpTopics"
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            manager.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
            Me.AboutWaterPipeToolStripMenuItem.Name = "AboutWaterPipeToolStripMenuItem"
            manager.ApplyResources(Me.AboutWaterPipeToolStripMenuItem, "AboutWaterPipeToolStripMenuItem")
            Me.AboutWaterPipeToolStripMenuItem.Tag = "AboutWaterPipe"
            Me.PanelColorDialog.Color = SystemColors.Control
            Me.PanelBackgroundImageFileDialog.FileName = "Background Image"
            manager.ApplyResources(Me.PanelBackgroundImageFileDialog, "PanelBackgroundImageFileDialog")
            Me.SelectMusicDialog.FileName = "Music Files"
            manager.ApplyResources(Me.SelectMusicDialog, "SelectMusicDialog")
            Me.SelectMusicDialog.Multiselect = True
            manager.ApplyResources(Me, "$this")
            Me.AutoScaleMode = AutoScaleMode.Font
            Me.Controls.Add(Me.MainPanel)
            Me.DoubleBuffered = True
            Me.ForeColor = SystemColors.ControlText
            Me.FormBorderStyle = FormBorderStyle.FixedSingle
            Me.Name = "MainForm"
            Me.MainPanel.ResumeLayout(False)
            Me.MainPanel.PerformLayout
            DirectCast(Me.Box_6_0, ISupportInitialize).EndInit
            DirectCast(Me.Box_7_0, ISupportInitialize).EndInit
            DirectCast(Me.Box_8_0, ISupportInitialize).EndInit
            DirectCast(Me.Box_9_0, ISupportInitialize).EndInit
            DirectCast(Me.Box_1_0, ISupportInitialize).EndInit
            DirectCast(Me.Box_2_0, ISupportInitialize).EndInit
            DirectCast(Me.Box_3_0, ISupportInitialize).EndInit
            DirectCast(Me.Box_4_0, ISupportInitialize).EndInit
            DirectCast(Me.Box_5_0, ISupportInitialize).EndInit
            DirectCast(Me.Box_1_1, ISupportInitialize).EndInit
            DirectCast(Me.Box_9_9, ISupportInitialize).EndInit
            DirectCast(Me.Box_9_3, ISupportInitialize).EndInit
            DirectCast(Me.Box_1_2, ISupportInitialize).EndInit
            DirectCast(Me.Box_9_4, ISupportInitialize).EndInit
            DirectCast(Me.Box_1_8, ISupportInitialize).EndInit
            DirectCast(Me.Box_9_5, ISupportInitialize).EndInit
            DirectCast(Me.Box_1_7, ISupportInitialize).EndInit
            DirectCast(Me.Box_9_6, ISupportInitialize).EndInit
            DirectCast(Me.Box_1_6, ISupportInitialize).EndInit
            DirectCast(Me.Box_9_7, ISupportInitialize).EndInit
            DirectCast(Me.Box_9_8, ISupportInitialize).EndInit
            DirectCast(Me.Box_1_4, ISupportInitialize).EndInit
            DirectCast(Me.Box_9_2, ISupportInitialize).EndInit
            DirectCast(Me.Box_1_3, ISupportInitialize).EndInit
            DirectCast(Me.Box_9_1, ISupportInitialize).EndInit
            DirectCast(Me.Box_1_9, ISupportInitialize).EndInit
            DirectCast(Me.Box_8_9, ISupportInitialize).EndInit
            DirectCast(Me.Box_8_3, ISupportInitialize).EndInit
            DirectCast(Me.Box_8_4, ISupportInitialize).EndInit
            DirectCast(Me.Box_8_5, ISupportInitialize).EndInit
            DirectCast(Me.Box_8_6, ISupportInitialize).EndInit
            DirectCast(Me.Box_2_1, ISupportInitialize).EndInit
            DirectCast(Me.Box_8_7, ISupportInitialize).EndInit
            DirectCast(Me.Box_2_2, ISupportInitialize).EndInit
            DirectCast(Me.Box_8_8, ISupportInitialize).EndInit
            DirectCast(Me.Box_2_8, ISupportInitialize).EndInit
            DirectCast(Me.Box_8_2, ISupportInitialize).EndInit
            DirectCast(Me.Box_2_7, ISupportInitialize).EndInit
            DirectCast(Me.Box_8_1, ISupportInitialize).EndInit
            DirectCast(Me.Box_2_6, ISupportInitialize).EndInit
            DirectCast(Me.Box_7_9, ISupportInitialize).EndInit
            DirectCast(Me.Box_2_5, ISupportInitialize).EndInit
            DirectCast(Me.Box_7_3, ISupportInitialize).EndInit
            DirectCast(Me.Box_2_4, ISupportInitialize).EndInit
            DirectCast(Me.Box_7_4, ISupportInitialize).EndInit
            DirectCast(Me.Box_2_3, ISupportInitialize).EndInit
            DirectCast(Me.Box_7_5, ISupportInitialize).EndInit
            DirectCast(Me.Box_2_9, ISupportInitialize).EndInit
            DirectCast(Me.Box_7_6, ISupportInitialize).EndInit
            DirectCast(Me.Box_3_1, ISupportInitialize).EndInit
            DirectCast(Me.Box_7_7, ISupportInitialize).EndInit
            DirectCast(Me.Box_3_2, ISupportInitialize).EndInit
            DirectCast(Me.Box_7_8, ISupportInitialize).EndInit
            DirectCast(Me.Box_3_8, ISupportInitialize).EndInit
            DirectCast(Me.Box_7_2, ISupportInitialize).EndInit
            DirectCast(Me.Box_3_7, ISupportInitialize).EndInit
            DirectCast(Me.Box_7_1, ISupportInitialize).EndInit
            DirectCast(Me.Box_3_6, ISupportInitialize).EndInit
            DirectCast(Me.Box_6_9, ISupportInitialize).EndInit
            DirectCast(Me.Box_3_5, ISupportInitialize).EndInit
            DirectCast(Me.Box_6_3, ISupportInitialize).EndInit
            DirectCast(Me.Box_3_4, ISupportInitialize).EndInit
            DirectCast(Me.Box_6_4, ISupportInitialize).EndInit
            DirectCast(Me.Box_3_3, ISupportInitialize).EndInit
            DirectCast(Me.Box_6_5, ISupportInitialize).EndInit
            DirectCast(Me.Box_3_9, ISupportInitialize).EndInit
            DirectCast(Me.Box_6_6, ISupportInitialize).EndInit
            DirectCast(Me.Box_4_1, ISupportInitialize).EndInit
            DirectCast(Me.Box_6_7, ISupportInitialize).EndInit
            DirectCast(Me.Box_4_2, ISupportInitialize).EndInit
            DirectCast(Me.Box_6_8, ISupportInitialize).EndInit
            DirectCast(Me.Box_4_8, ISupportInitialize).EndInit
            DirectCast(Me.Box_6_2, ISupportInitialize).EndInit
            DirectCast(Me.Box_4_7, ISupportInitialize).EndInit
            DirectCast(Me.Box_6_1, ISupportInitialize).EndInit
            DirectCast(Me.Box_4_6, ISupportInitialize).EndInit
            DirectCast(Me.Box_5_9, ISupportInitialize).EndInit
            DirectCast(Me.Box_4_5, ISupportInitialize).EndInit
            DirectCast(Me.Box_5_3, ISupportInitialize).EndInit
            DirectCast(Me.Box_4_4, ISupportInitialize).EndInit
            DirectCast(Me.Box_5_4, ISupportInitialize).EndInit
            DirectCast(Me.Box_4_3, ISupportInitialize).EndInit
            DirectCast(Me.Box_5_5, ISupportInitialize).EndInit
            DirectCast(Me.Box_4_9, ISupportInitialize).EndInit
            DirectCast(Me.Box_5_6, ISupportInitialize).EndInit
            DirectCast(Me.Box_5_1, ISupportInitialize).EndInit
            DirectCast(Me.Box_5_7, ISupportInitialize).EndInit
            DirectCast(Me.Box_5_2, ISupportInitialize).EndInit
            DirectCast(Me.Box_5_8, ISupportInitialize).EndInit
            DirectCast(Me.Box_1_5, ISupportInitialize).EndInit
            DirectCast(Me.OutBondPictureBox, ISupportInitialize).EndInit
            DirectCast(Me.Next_9, ISupportInitialize).EndInit
            DirectCast(Me.Next_8, ISupportInitialize).EndInit
            DirectCast(Me.Next_7, ISupportInitialize).EndInit
            DirectCast(Me.Next_6, ISupportInitialize).EndInit
            DirectCast(Me.Next_5, ISupportInitialize).EndInit
            DirectCast(Me.Next_4, ISupportInitialize).EndInit
            DirectCast(Me.Next_3, ISupportInitialize).EndInit
            DirectCast(Me.Next_2, ISupportInitialize).EndInit
            DirectCast(Me.Next_1, ISupportInitialize).EndInit
            Me.MenuStrip.ResumeLayout(False)
            Me.MenuStrip.PerformLayout
            Me.ResumeLayout(False)
        End Sub

        Private Sub InWater_Click(ByVal sender As Object, ByVal e As EventArgs)
            If (Me.Score = 0) Then
                New Thread(New ThreadStart(AddressOf Me.Final)).Start
            Else
                Me.New_Game
            End If
        End Sub

        Private Sub LanguageChange_Click(ByVal sender As Object, ByVal e As EventArgs)
            Me.setLanguage(DirectCast(sender, ToolStripMenuItem))
        End Sub

        Private Sub MainForm_Disposed(ByVal sender As Object, ByVal e As EventArgs)
            Me.write_Config
            Me.Write_HighScore
        End Sub

        Private Sub MainForm_Load(ByVal sender As Object, ByVal e As EventArgs)
            Me.get_Config
            Me.InitCollection
            Me.Read_HighScore
            Me.New_Game
            New Thread(New ThreadStart(AddressOf Me.playMediaFiles)).Start
        End Sub

        Private Sub MusicToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
            If Me.MusicToolStripMenuItem.Checked Then
                Me.MediaPlayer.controls.play
            Else
                Me.MediaPlayer.controls.pause
            End If
        End Sub

        Private Sub New_Game()
            If Me.SoundsToolStripMenuItem.Checked Then
                Me.AppSounds_NewGame.Play
            End If
            Me.Lave = Me.PIPENO
            Me.Score = 0
            Me.SetBoxCollection.Clear
            Dim num As Integer = 1
            Do
                NewLateBinding.LateSetComplex(Me.BoxCollection.Item(num), Nothing, "Image", New Object() { Nothing }, Nothing, Nothing, False, True)
                NewLateBinding.LateSetComplex(Me.BoxCollection.Item(num), Nothing, "Tag", New Object() { "" }, Nothing, Nothing, False, True)
                NewLateBinding.LateSetComplex(Me.BoxCollection.Item(num), Nothing, "BackColor", New Object() { Color.Transparent }, Nothing, Nothing, False, True)
                NewLateBinding.LateSetComplex(Me.BoxCollection.Item(num), Nothing, "Enabled", New Object() { True }, Nothing, Nothing, False, True)
                num += 1
            Loop While (num <= &H51)
            Dim strArray As String() = New String(10  - 1) {}
            Select Case Me.Entrance
                Case 1
                    strArray(0) = "5_0"
                    Exit Select
                Case 2
                    strArray(0) = "3_0"
                    strArray(1) = "7_0"
                    Exit Select
                Case 3
                    strArray(0) = "2_0"
                    strArray(1) = "5_0"
                    strArray(2) = "8_0"
                    Exit Select
                Case 4
                    strArray(0) = "2_0"
                    strArray(1) = "4_0"
                    strArray(2) = "6_0"
                    strArray(3) = "8_0"
                    Exit Select
                Case 5
                    strArray(0) = "1_0"
                    strArray(1) = "3_0"
                    strArray(2) = "5_0"
                    strArray(3) = "7_0"
                    strArray(4) = "9_0"
                    Exit Select
                Case 6
                    strArray(0) = "2_0"
                    strArray(1) = "3_0"
                    strArray(2) = "4_0"
                    strArray(3) = "6_0"
                    strArray(4) = "7_0"
                    strArray(5) = "8_0"
                    Exit Select
                Case 7
                    strArray(0) = "2_0"
                    strArray(1) = "3_0"
                    strArray(2) = "4_0"
                    strArray(3) = "5_0"
                    strArray(4) = "6_0"
                    strArray(5) = "7_0"
                    strArray(6) = "8_0"
                    Exit Select
                Case 8
                    strArray(0) = "1_0"
                    strArray(1) = "2_0"
                    strArray(2) = "3_0"
                    strArray(3) = "4_0"
                    strArray(4) = "6_0"
                    strArray(5) = "7_0"
                    strArray(6) = "8_0"
                    strArray(7) = "9_0"
                    Exit Select
                Case 9
                    strArray(0) = "1_0"
                    strArray(1) = "2_0"
                    strArray(2) = "3_0"
                    strArray(3) = "4_0"
                    strArray(4) = "5_0"
                    strArray(5) = "6_0"
                    strArray(6) = "7_0"
                    strArray(7) = "8_0"
                    strArray(8) = "9_0"
                    Exit Select
            End Select
            Dim num3 As Integer = 1
            Do
                NewLateBinding.LateSetComplex(Me.BoxCollection.Item((Conversions.ToString(num3) & "_0")), Nothing, "Image", New Object() { RuntimeHelpers.GetObjectValue(Me.ImageCollection.Item("R")) }, Nothing, Nothing, False, True)
                NewLateBinding.LateSetComplex(Me.BoxCollection.Item((Conversions.ToString(num3) & "_0")), Nothing, "Tag", New Object() { "C" }, Nothing, Nothing, False, True)
                NewLateBinding.LateSetComplex(NewLateBinding.LateGet(Me.BoxCollection.Item((Conversions.ToString(num3) & "_0")), Nothing, "Image", New Object(0  - 1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object() { "R" }, Nothing, Nothing, False, True)
                NewLateBinding.LateSetComplex(Me.BoxCollection.Item((Conversions.ToString(num3) & "_0")), Nothing, "BackColor", New Object() { Color.Transparent }, Nothing, Nothing, False, True)
                NewLateBinding.LateSetComplex(Me.BoxCollection.Item((Conversions.ToString(num3) & "_0")), Nothing, "Visible", New Object() { False }, Nothing, Nothing, False, True)
                num3 += 1
            Loop While (num3 <= 9)
            Me.FlowBoxCollection.Clear
            Dim num6 As Integer = (Me.Entrance - 1)
            Dim i As Integer = 0
            Do While (i <= num6)
                NewLateBinding.LateSetComplex(Me.BoxCollection.Item(strArray(i)), Nothing, "Visible", New Object() { True }, Nothing, Nothing, False, True)
                Me.FlowBoxCollection.Add(RuntimeHelpers.GetObjectValue(Me.BoxCollection.Item(strArray(i))), Nothing, Nothing, Nothing)
                i += 1
            Loop
            Me.ScoreNumberLabel.Text = Conversions.ToString(Me.Score)
            Me.ScoreBar.Value = CInt(Math.Round(CDbl((CDbl(Me.Score) / 10))))
            Me.InWater.Text = Conversions.ToString(Me.Lave)
            Me.WaterGoToolStripMenuItem.Enabled = True
            Dim num2 As Integer = 1
            Do
                Me.Set_RandomImage(DirectCast(Me.NextBoxCollection.Item(num2), PictureBox))
                num2 += 1
            Loop While (num2 <= 9)
            If Me.SoundsToolStripMenuItem.Checked Then
                Me.AppSounds_NewGame.Stop
            End If
        End Sub

        Private Sub NewGameToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
            Me.New_Game
        End Sub

        Private Sub OJGoToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
            New Thread(New ThreadStart(AddressOf Me.Final)).Start
        End Sub

        Private Sub Place_Image(ByVal Box As PictureBox)
            If (Me.Next_1.Image Is Nothing) Then
                Me.ScoreNumberLabel.Text = "OK"
            Else
                Me.Set_BoxImage(Box)
                Me.Next_1.Image = Me.Next_2.Image
                Me.Next_2.Image = Me.Next_3.Image
                Me.Next_3.Image = Me.Next_4.Image
                Me.Next_4.Image = Me.Next_5.Image
                Me.Next_5.Image = Me.Next_6.Image
                Me.Next_6.Image = Me.Next_7.Image
                Me.Next_7.Image = Me.Next_8.Image
                Me.Next_8.Image = Me.Next_9.Image
                Me.Set_RandomImage(Me.Next_9)
                If Not Me.SetBoxCollection.Contains(Box.Name) Then
                    Me.SetBoxCollection.Add(Box, Box.Name, Nothing, Nothing)
                End If
                Me.Lave -= 1
                Me.InWater.Text = Conversions.ToString(Me.Lave)
                If (Me.Lave = 0) Then
                    New Thread(New ThreadStart(AddressOf Me.Final)).Start
                End If
            End If
        End Sub

        Private Sub Play_BoxAnimo(ByVal Box As PictureBox, ByVal ImageNo As Integer, ByVal BoxDire As String, ByVal BoxImageDire As String)
            Dim num5 As Integer
            Dim wATERSPEED As Integer = Me.WATERSPEED
            Dim strArray2 As String() = New String(3  - 1) {}
            Dim strArray As String() = New String(5  - 1) {}
            Dim strArray3 As String() = New String(5  - 1) {}
            Dim numArray4 As Integer() = New Integer() { 0, 0, 0, 0 }
            Dim numArray As Integer() = New Integer() { wATERSPEED, wATERSPEED, wATERSPEED, wATERSPEED }
            Dim width As Integer = 100
            Dim height As Integer = 100
            Dim pen As New Pen(Me.WaterColor, 30!)
            Dim numArray9 As Integer() = New Integer(5  - 1) {}
            Dim numArray10 As Integer() = New Integer(5  - 1) {}
            Dim rectangleArray As Rectangle() = New Rectangle(5  - 1) {}
            Dim numArray3 As Integer() = New Integer(5  - 1) {}
            Dim numArray2 As Integer() = New Integer(5  - 1) {}
            Dim numArray5 As Integer() = New Integer(5  - 1) {}
            Dim numArray6 As Integer() = New Integer(5  - 1) {}
            Dim numArray7 As Integer() = New Integer(5  - 1) {}
            Dim numArray8 As Integer() = New Integer(5  - 1) {}
            If (ImageNo = 0) Then
                strArray(num5) = "L"
                strArray3(num5) = "R"
                numArray4(num5) = CInt(Math.Round(CDbl((CDbl((wATERSPEED * 2)) / 5))))
                num5 = 1
            ElseIf ((ImageNo >= 3) And (ImageNo <= 7)) Then
                Dim strArray4 As String() = New String() { "U", "R", "D", "L" }
                Dim str2 As String = ""
                Dim str3 As String = ""
                Dim collection As New Collection
                collection.Add("R", "L", Nothing, Nothing)
                collection.Add("L", "R", Nothing, Nothing)
                collection.Add("D", "U", Nothing, Nothing)
                collection.Add("U", "D", Nothing, Nothing)
                Dim str4 As String
                For Each str4 In strArray4
                    If BoxDire.Contains(str4) Then
                        str2 = (str2 & str4)
                    ElseIf BoxImageDire.Contains(str4) Then
                        str3 = (str3 & str4)
                    End If
                Next
                Dim str5 As String
                For Each str5 In strArray4
                    If str2.Contains(str5) Then
                        strArray(num5) = str5
                        strArray3(num5) = Conversions.ToString(collection.Item(str5))
                        numArray(num5) = CInt(Math.Round(CDbl((CDbl(wATERSPEED) / 2))))
                        num5 += 1
                    End If
                Next
                Dim str6 As String
                For Each str6 In strArray4
                    If str3.Contains(str6) Then
                        strArray(num5) = Conversions.ToString(collection.Item(str6))
                        strArray3(num5) = str6
                        numArray4(num5) = CInt(Math.Round(CDbl((CDbl(wATERSPEED) / 2))))
                        num5 += 1
                    End If
                Next
                If (str3 = "") Then
                    wATERSPEED = CInt(Math.Round(CDbl((CDbl(wATERSPEED) / 2))))
                End If
            Else
                Dim num4 As Integer
                If (((ImageNo >= 1) And (ImageNo <= 2)) Or ((ImageNo >= 9) And (ImageNo <= 12))) Then
                    strArray2(0) = BoxImageDire
                    num4 = 1
                ElseIf ((ImageNo = 8) Or ((ImageNo >= 13) And (ImageNo <= 14))) Then
                    strArray2(0) = BoxImageDire.Substring(0, 2)
                    num4 += 1
                    If (BoxImageDire.Length > 2) Then
                        strArray2(1) = BoxImageDire.Substring(2, 2)
                        num4 += 1
                    End If
                End If
                Dim num18 As Integer = (num4 - 1)
                Dim k As Integer = 0
                Do While (k <= num18)
                    If (((ImageNo >= 1) And (ImageNo <= 2)) Or (ImageNo >= 8)) Then
                        If BoxDire.Contains(Conversions.ToString(strArray2(k).Chars(0))) Then
                            strArray(num5) = Conversions.ToString(strArray2(k).Chars(0))
                            strArray3(num5) = Conversions.ToString(strArray2(k).Chars(1))
                            num5 += 1
                        End If
                        If BoxDire.Contains(Conversions.ToString(strArray2(k).Chars(1))) Then
                            strArray(num5) = Conversions.ToString(strArray2(k).Chars(1))
                            strArray3(num5) = Conversions.ToString(strArray2(k).Chars(0))
                            num5 += 1
                        End If
                        If (BoxDire.Contains(Conversions.ToString(strArray2(k).Chars(0))) And BoxDire.Contains(Conversions.ToString(strArray2(k).Chars(1)))) Then
                            numArray(num5) = CInt(Math.Round(CDbl((CDbl(wATERSPEED) / 2))))
                        End If
                    End If
                    k += 1
                Loop
            End If
            Dim num19 As Integer = (num5 - 1)
            Dim i As Integer = 0
            Do While (i <= num19)
                If (ImageNo >= 9) Then
                    If (strArray(i) = "L") Then
                        If (strArray3(i) = "U") Then
                            numArray9(i) = CInt(Math.Round(CDbl((CDbl((0 - width)) / 2))))
                            numArray10(i) = CInt(Math.Round(CDbl((CDbl((0 - height)) / 2))))
                            numArray3(i) = &H5B
                            numArray2(i) = -92
                        ElseIf (strArray3(i) = "D") Then
                            numArray9(i) = CInt(Math.Round(CDbl((CDbl((0 - width)) / 2))))
                            numArray10(i) = CInt(Math.Round(CDbl((CDbl(height) / 2))))
                            numArray3(i) = &H10D
                            numArray2(i) = &H5C
                        End If
                    ElseIf (strArray(i) = "R") Then
                        If (strArray3(i) = "U") Then
                            numArray9(i) = CInt(Math.Round(CDbl((CDbl(width) / 2))))
                            numArray10(i) = CInt(Math.Round(CDbl((CDbl((0 - height)) / 2))))
                            numArray3(i) = &H59
                            numArray2(i) = &H5C
                        ElseIf (strArray3(i) = "D") Then
                            numArray9(i) = CInt(Math.Round(CDbl((CDbl(width) / 2))))
                            numArray10(i) = CInt(Math.Round(CDbl((CDbl(height) / 2))))
                            numArray3(i) = &H10F
                            numArray2(i) = -92
                        End If
                    ElseIf (strArray(i) = "U") Then
                        If (strArray3(i) = "L") Then
                            numArray9(i) = CInt(Math.Round(CDbl((CDbl((0 - width)) / 2))))
                            numArray10(i) = CInt(Math.Round(CDbl((CDbl((0 - height)) / 2))))
                            numArray3(i) = -1
                            numArray2(i) = &H5C
                        ElseIf (strArray3(i) = "R") Then
                            numArray9(i) = CInt(Math.Round(CDbl((CDbl(width) / 2))))
                            numArray10(i) = CInt(Math.Round(CDbl((CDbl((0 - height)) / 2))))
                            numArray3(i) = &HB5
                            numArray2(i) = -92
                        End If
                    ElseIf (strArray(i) = "D") Then
                        If (strArray3(i) = "L") Then
                            numArray9(i) = CInt(Math.Round(CDbl((CDbl((0 - width)) / 2))))
                            numArray10(i) = CInt(Math.Round(CDbl((CDbl(height) / 2))))
                            numArray3(i) = 1
                            numArray2(i) = -92
                        ElseIf (strArray3(i) = "R") Then
                            numArray9(i) = CInt(Math.Round(CDbl((CDbl(width) / 2))))
                            numArray10(i) = CInt(Math.Round(CDbl((CDbl(height) / 2))))
                            numArray3(i) = &HB3
                            numArray2(i) = &H5C
                        End If
                    End If
                    rectangleArray(i) = New Rectangle(numArray9(i), numArray10(i), width, height)
                Else
                    Dim num6 As Integer = CInt(Math.Round(CDbl((CDbl(width) / CDbl(Me.WATERSPEED)))))
                    Dim num7 As Integer = CInt(Math.Round(CDbl((CDbl(height) / CDbl(Me.WATERSPEED)))))
                    If ((strArray(i) = "L") And (strArray3(i) = "R")) Then
                        numArray5(i) = 0
                        numArray6(i) = CInt(Math.Round(CDbl((CDbl(height) / 2))))
                        numArray7(i) = num6
                        numArray8(i) = 0
                    ElseIf ((strArray(i) = "R") And (strArray3(i) = "L")) Then
                        numArray5(i) = width
                        numArray6(i) = CInt(Math.Round(CDbl((CDbl(height) / 2))))
                        numArray7(i) = (0 - num6)
                        numArray8(i) = 0
                    ElseIf ((strArray(i) = "U") And (strArray3(i) = "D")) Then
                        numArray5(i) = CInt(Math.Round(CDbl((CDbl(width) / 2))))
                        numArray6(i) = 0
                        numArray7(i) = 0
                        numArray8(i) = num7
                    ElseIf ((strArray(i) = "D") And (strArray3(i) = "U")) Then
                        numArray5(i) = CInt(Math.Round(CDbl((CDbl(width) / 2))))
                        numArray6(i) = height
                        numArray7(i) = 0
                        numArray8(i) = (0 - num7)
                    End If
                    Dim index As Integer = i
                    numArray5(index) = (numArray5(index) + (numArray7(i) * numArray4(i)))
                    index = i
                    numArray6(index) = (numArray6(index) + (numArray8(i) * numArray4(i)))
                End If
                i += 1
            Loop
            Dim image As Image = DirectCast(NewLateBinding.LateGet(Me.ResMana.GetObject("none"), Nothing, "Clone", New Object(0  - 1) {}, Nothing, Nothing, Nothing), Image)
            Dim graphics As Graphics = Graphics.FromImage(image)
            Dim image2 As Image = DirectCast(Box.Image.Clone, Image)
            Dim str As String = Conversions.ToString(Box.Image.Tag)
            Dim num21 As Integer = wATERSPEED
            Dim j As Integer = 1
            Do While (j <= num21)
                graphics.Clear(Color.Transparent)
                Dim num22 As Integer = (num5 - 1)
                Dim m As Integer = 0
                Do While (m <= num22)
                    If (j >= numArray4(m)) Then
                        If (ImageNo >= 9) Then
                            Dim num9 As Integer
                            If (j <= numArray(m)) Then
                                num9 = CInt(Math.Round(Conversion.Int(CDbl((CDbl((numArray2(m) * j)) / CDbl(wATERSPEED))))))
                            Else
                                num9 = CInt(Math.Round(Conversion.Int(CDbl((CDbl((numArray2(m) * numArray(m))) / CDbl(wATERSPEED))))))
                            End If
                            graphics.DrawArc(pen, rectangleArray(m), CSng(numArray3(m)), CSng(num9))
                        Else
                            Dim num As Integer
                            Dim num2 As Integer
                            If (j <= numArray(m)) Then
                                num = (numArray5(m) + (numArray7(m) * (j - numArray4(m))))
                                num2 = (numArray6(m) + (numArray8(m) * (j - numArray4(m))))
                            Else
                                num = (numArray5(m) + (numArray7(m) * (numArray(m) - numArray4(m))))
                                num2 = (numArray6(m) + (numArray8(m) * (numArray(m) - numArray4(m))))
                            End If
                            graphics.DrawLine(pen, numArray5(m), numArray6(m), num, num2)
                        End If
                    End If
                    m += 1
                Loop
                graphics.DrawImage(image2, 0, 0, width, height)
                Box.Image = image
                Application.DoEvents
                Thread.Sleep(100)
                j += 1
            Loop
            Box.Image.Tag = str
        End Sub

        Private Sub Player_MediaError(ByVal pMediaObject As Object)
            MessageBox.Show((Me.MediaError & " --- " & Me.MediaPlayer.currentMedia.sourceURL))
            Me.Playlist.removeItem(Me.MediaPlayer.currentMedia)
            Dim currentItem As Object = Me.MediaPlayer.controls.currentItem
            If (Me.Playlist.count <> 0) Then
                Me.MediaPlayer.controls.play
            Else
                Me.MediaPlayer.controls.stop
            End If
        End Sub

        Private Sub playMediaFiles()
            If (Me.Playlist.count = 0) Then
                Me.Playlist.appendItem(Me.MediaPlayer.newMedia((Me.AppPath & "\Resources\Music\puzzle.mid")))
            End If
            Me.MediaPlayer.currentPlaylist = Me.Playlist
            Me.MediaPlayer.controls.stop
            If Me.MusicToolStripMenuItem.Checked Then
                Me.MediaPlayer.controls.play
            End If
        End Sub

        Public Sub Process_HighScore()
            Dim num3 As Integer = (Me.HighScore.Length - 2)
            Dim i As Integer = 0
            Do While (i <= num3)
                If (Me.Score > Me.HighScore(i)) Then
                    Dim highScore As Integer() = Me.HighScore
                    Dim highScoreName As String() = Me.HighScoreName
                    Dim num4 As Integer = i
                    Dim index As Integer = (Me.HighScore.Length - 2)
                    Do While (index >= num4)
                        Me.HighScore((index + 1)) = Me.HighScore(index)
                        Me.HighScoreName((index + 1)) = Me.HighScoreName(index)
                        index = (index + -1)
                    Loop
                    Me.HighScore(i) = Me.Score
                    Me.HighScoreName(i) = ""
                    Me.NowRank = (i + 1)
                    MyProject.Forms.HighScoreDialog.Set_HighScore(Me.HighScore, Me.HighScoreName)
                    MyProject.Forms.HighScoreDialog.Show_InputBox(Me.NowRank, Me.LastInputName)
                    MyProject.Forms.HighScoreDialog.ShowDialog
                    MyProject.Forms.HighScoreDialog.Hide_InputBox(Me.NowRank)
                    If (MyProject.Forms.HighScoreDialog.DialogResult = DialogResult.OK) Then
                        Me.LastInputName = Conversions.ToString(NewLateBinding.LateGet(MyProject.Forms.HighScoreDialog.HighScoreNameBoxCollection.Item(Me.NowRank), Nothing, "Text", New Object(0  - 1) {}, Nothing, Nothing, Nothing))
                        Me.HighScoreName((Me.NowRank - 1)) = Me.LastInputName
                    ElseIf (MyProject.Forms.HighScoreDialog.DialogResult = DialogResult.Cancel) Then
                        Dim num5 As Integer = (Me.HighScore.Length - 2)
                        index = (Me.NowRank - 1)
                        Do While (index <= num5)
                            Me.HighScore(index) = Me.HighScore((index + 1))
                            Me.HighScoreName(index) = Me.HighScoreName((index + 1))
                            index += 1
                        Loop
                    End If
                    Exit Do
                End If
                i += 1
            Loop
        End Sub

        Public Sub Read_HighScore()
            If FileSystem.FileExists((Me.AppPath & "\HighScore.rex")) Then
                Dim strArray As String() = FileSystem.ReadAllText((Me.AppPath & "\HighScore.rex")).Split(New Char() { "$"c })
                Dim num2 As Integer = (Me.HighScore.Length - 2)
                Dim i As Integer = 0
                Do While (i <= num2)
                    Dim strArray2 As String() = strArray(i).Split(New Char() { "@"c })
                    Me.HighScoreName(i) = strArray2(1)
                    Me.HighScore(i) = Conversions.ToInteger(strArray2(2))
                    i += 1
                Loop
            Else
                Me.Write_HighScore
            End If
        End Sub

        Private Sub SelectColorToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
            If (Me.PanelColorDialog.ShowDialog = DialogResult.OK) Then
                Me.PanelBackColor = Me.PanelColorDialog.Color
                Me.MainPanel.BackgroundImage = Nothing
                Me.MainPanel.BackColor = Me.PanelBackColor
                Me.ColorToolStripMenuItem.Checked = True
                Me.ImageToolStripMenuItem.Checked = False
            End If
        End Sub

        Private Sub SelectImageToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
            If (Me.PanelBackgroundImageFileDialog.ShowDialog = DialogResult.OK) Then
                Me.PanelBackgroundImage = Image.FromFile(Me.PanelBackgroundImageFileDialog.FileName)
                Me.MainPanel.BackgroundImage = Me.PanelBackgroundImage
                Me.ImageToolStripMenuItem.Checked = True
                Me.ColorToolStripMenuItem.Checked = False
            End If
        End Sub

        Private Sub SelectMusicToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
            If (Me.SelectMusicDialog.ShowDialog = DialogResult.OK) Then
                Dim fileNames As String() = Me.SelectMusicDialog.FileNames
                Me.MediaPlayer.controls.stop
                Me.Playlist.clear
                Dim num2 As Integer = (fileNames.Length - 1)
                Dim i As Integer = 0
                Do While (i <= num2)
                    Me.Playlist.appendItem(Me.MediaPlayer.newMedia(fileNames(i)))
                    i += 1
                Loop
                Me.MusicToolStripMenuItem.Checked = True
                Me.MediaPlayer.controls.play
            End If
        End Sub

        Private Sub Set_BoxImage(ByVal box As PictureBox)
            If (Me.Lave <> 0) Then
                If (box.Image Is Nothing) Then
                    If Me.SoundsToolStripMenuItem.Checked Then
                        Me.AppSounds_PlaceImage.Play
                    End If
                    box.Image = Me.Next_1.Image
                ElseIf ((box.Image.Equals(RuntimeHelpers.GetObjectValue(Me.ImageCollection.Item("LU"))) And Me.Next_1.Image.Equals(RuntimeHelpers.GetObjectValue(Me.ImageCollection.Item("RD")))) Or (box.Image.Equals(RuntimeHelpers.GetObjectValue(Me.ImageCollection.Item("RD"))) And Me.Next_1.Image.Equals(RuntimeHelpers.GetObjectValue(Me.ImageCollection.Item("LU"))))) Then
                    If Me.SoundsToolStripMenuItem.Checked Then
                        Me.AppSounds_PlaceImage.Play
                    End If
                    box.Image = DirectCast(Me.ImageCollection.Item("LURD/"), Image)
                ElseIf ((box.Image.Equals(RuntimeHelpers.GetObjectValue(Me.ImageCollection.Item("LD"))) And Me.Next_1.Image.Equals(RuntimeHelpers.GetObjectValue(Me.ImageCollection.Item("RU")))) Or (box.Image.Equals(RuntimeHelpers.GetObjectValue(Me.ImageCollection.Item("RU"))) And Me.Next_1.Image.Equals(RuntimeHelpers.GetObjectValue(Me.ImageCollection.Item("LD"))))) Then
                    If Me.SoundsToolStripMenuItem.Checked Then
                        Me.AppSounds_PlaceImage.Play
                    End If
                    box.Image = DirectCast(Me.ImageCollection.Item("LURD\"), Image)
                Else
                    If Me.SoundsToolStripMenuItem.Checked Then
                        Me.AppSounds_Breaking.Play
                    End If
                    box.Image = Me.Next_1.Image
                End If
            End If
        End Sub

        Private Sub Set_BoxImageTag()
            Dim num As Integer = 0
            Dim strArray As String() = New String() { "R", "LR", "UD", "LUR", "URD", "LRD", "LUD", "LURD", "LURDX", "LU", "RU", "RD", "LD", "LURD\", "LURD/" }
            Dim str As String
            For Each str In strArray
                NewLateBinding.LateSetComplex(Me.ImageCollection.Item(str), Nothing, "Tag", New Object() { (Conversions.ToString(num) & "|" & str) }, Nothing, Nothing, False, True)
                num += 1
            Next
        End Sub

        Private Sub set_Entrance(ByVal ent As String)
            Me.Entrance = Integer.Parse(ent)
            Me.Entrance1ToolStripMenuItem.Checked = False
            Me.Entrance2ToolStripMenuItem.Checked = False
            Me.Entrance3ToolStripMenuItem.Checked = False
            Me.Entrance4ToolStripMenuItem.Checked = False
            Me.Entrance5ToolStripMenuItem.Checked = False
            Me.Entrance6ToolStripMenuItem.Checked = False
            Me.Entrance7ToolStripMenuItem.Checked = False
            Me.Entrance8ToolStripMenuItem.Checked = False
            Me.Entrance9ToolStripMenuItem.Checked = False
            If (ent = Me.Entrance1ToolStripMenuItem.Text) Then
                Me.Entrance1ToolStripMenuItem.Checked = True
            ElseIf (ent = Me.Entrance2ToolStripMenuItem.Text) Then
                Me.Entrance2ToolStripMenuItem.Checked = True
            ElseIf (ent = Me.Entrance3ToolStripMenuItem.Text) Then
                Me.Entrance3ToolStripMenuItem.Checked = True
            ElseIf (ent = Me.Entrance4ToolStripMenuItem.Text) Then
                Me.Entrance4ToolStripMenuItem.Checked = True
            ElseIf (ent = Me.Entrance5ToolStripMenuItem.Text) Then
                Me.Entrance5ToolStripMenuItem.Checked = True
            ElseIf (ent = Me.Entrance6ToolStripMenuItem.Text) Then
                Me.Entrance6ToolStripMenuItem.Checked = True
            ElseIf (ent = Me.Entrance7ToolStripMenuItem.Text) Then
                Me.Entrance7ToolStripMenuItem.Checked = True
            ElseIf (ent = Me.Entrance8ToolStripMenuItem.Text) Then
                Me.Entrance8ToolStripMenuItem.Checked = True
            ElseIf (ent = Me.Entrance9ToolStripMenuItem.Text) Then
                Me.Entrance9ToolStripMenuItem.Checked = True
            End If
        End Sub

        Private Sub Set_LableLang(ByVal Label As Label, ByVal lang As String)
            Label.Text = Me.ResMana.GetString(Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Label.Tag, "_"), lang)))
        End Sub

        Private Sub Set_MenuLang(ByVal MenuCollection As ToolStripItemCollection, ByVal lang As String)
            Dim num2 As Integer = (MenuCollection.Count - 1)
            Dim i As Integer = 0
            Do While (i <= num2)
                Dim instance As Object = MenuCollection.Item(i)
                If (Not NewLateBinding.LateGet(instance, Nothing, "Tag", New Object(0  - 1) {}, Nothing, Nothing, Nothing) Is Nothing) Then
                    NewLateBinding.LateSet(instance, Nothing, "Text", New Object() { Me.ResMana.GetString(Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(NewLateBinding.LateGet(instance, Nothing, "Tag", New Object(0  - 1) {}, Nothing, Nothing, Nothing), "_"), lang))) }, Nothing, Nothing)
                    If Conversions.ToBoolean(NewLateBinding.LateGet(instance, Nothing, "HasDropDownItems", New Object(0  - 1) {}, Nothing, Nothing, Nothing)) Then
                        Me.Set_MenuLang(DirectCast(NewLateBinding.LateGet(instance, Nothing, "DropDownItems", New Object(0  - 1) {}, Nothing, Nothing, Nothing), ToolStripItemCollection), lang)
                    End If
                End If
                i += 1
            Loop
        End Sub

        Private Sub Set_RandomImage(ByVal Box As PictureBox)
            If (Me.Lave > 9) Then
                VBMath.Randomize
                Dim obj2 As Object = Conversion.Int(CSng(((12! * VBMath.Rnd) + 2!)))
                Box.Image = DirectCast(Me.ImageCollection.Item(RuntimeHelpers.GetObjectValue(obj2)), Image)
            ElseIf (Me.Lave <= 9) Then
                Box.Image = Nothing
            End If
        End Sub

        Public Sub SetEntranceMenuItem(ByVal sta As Boolean)
            Me.EntranceQuantityToolStripMenuItem.Enabled = sta
        End Sub

        Public Sub SetInWater(ByVal sta As Boolean)
            Me.InWater.Enabled = sta
        End Sub

        Public Sub SetInWaterText(ByVal [text] As String)
            Me.InWater.Text = [text]
        End Sub

        Public Sub setLanguage(ByVal sender As ToolStripMenuItem)
            Me.EnglishToolStripMenuItem.Checked = False
            Me.ChineseToolStripMenuItem.Checked = False
            sender.Checked = True
            Dim lang As String = Conversions.ToString(sender.Tag)
            Me.Set_MenuLang(Me.MenuStrip.Items, lang)
            Me.Set_LableLang(Me.ScoreLabel, lang)
            Me.Set_LableLang(Me.NextLabel, lang)
            Me.Set_LableLang(MyProject.Forms.HighScoreDialog.HighScoresHandLabel, lang)
            Me.Set_LableLang(MyProject.Forms.HighScoreDialog.RankLabel, lang)
            Me.Set_LableLang(MyProject.Forms.HighScoreDialog.NameLabel, lang)
            Me.Set_LableLang(MyProject.Forms.HighScoreDialog.HighScoreLabel, lang)
            Me.LastInputName = Me.ResMana.GetString(("LastInputName_" & lang))
            Me.MediaError = Me.ResMana.GetString(("MediaError_" & lang))
            Me.NewGame = Me.ResMana.GetString(("NewGame_" & lang))
            Me.Somebody = Me.ResMana.GetString(("Somebody_" & lang))
        End Sub

        Public Sub SetMainBox(ByVal sta As Boolean)
            Dim num As Integer = 1
            Do
                NewLateBinding.LateSetComplex(Me.BoxCollection.Item(num), Nothing, "Enabled", New Object() { sta }, Nothing, Nothing, False, True)
                num += 1
            Loop While (num <= &H51)
        End Sub

        Public Sub SetNewGameMenuItem(ByVal sta As Boolean)
            Me.NewGameToolStripMenuItem.Enabled = sta
        End Sub

        Public Sub SetOJGoMenuItem(ByVal sta As Boolean)
            Me.WaterGoToolStripMenuItem.Enabled = sta
        End Sub

        Public Sub SetScoreBarValue(ByVal value As Double)
            Me.ScoreBar.Value = CInt(Math.Round(value))
        End Sub

        Public Sub SetScoreNumberLabelText(ByVal [text] As String)
            Me.ScoreNumberLabel.Text = [text]
        End Sub

        Private Sub ShowGridToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
            If Me.ShowGridToolStripMenuItem.Checked Then
                Me.OutBondPictureBox.BackColor = Me.GridColor
            Else
                Me.OutBondPictureBox.BackColor = Color.Transparent
            End If
        End Sub

        Public Sub Thread_BoxAnimo(ByVal sender As Object)
            Me.Play_BoxAnimo(DirectCast(NewLateBinding.LateIndexGet(sender, New Object() { 0 }, Nothing), PictureBox), Conversions.ToInteger(NewLateBinding.LateIndexGet(sender, New Object() { 1 }, Nothing)), Conversions.ToString(NewLateBinding.LateIndexGet(sender, New Object() { 2 }, Nothing)), Conversions.ToString(NewLateBinding.LateIndexGet(sender, New Object() { 3 }, Nothing)))
        End Sub

        Public Sub Thread_OverflowAnimo(ByVal Box As Object)
            Dim num As Integer = 1
            Do
                NewLateBinding.LateSet(Box, Nothing, "BackColor", New Object() { Color.Red }, Nothing, Nothing)
                Application.DoEvents
                Thread.Sleep(200)
                NewLateBinding.LateSet(Box, Nothing, "BackColor", New Object() { Color.Green }, Nothing, Nothing)
                Application.DoEvents
                Thread.Sleep(200)
                NewLateBinding.LateSet(Box, Nothing, "BackColor", New Object() { Color.Yellow }, Nothing, Nothing)
                Application.DoEvents
                Thread.Sleep(200)
                NewLateBinding.LateSet(Box, Nothing, "BackColor", New Object() { Color.Transparent }, Nothing, Nothing)
                Application.DoEvents
                Thread.Sleep(200)
                num += 1
            Loop While (num <= 3)
        End Sub

        Private Sub WaterColorToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
            If (Me.PanelColorDialog.ShowDialog = DialogResult.OK) Then
                Me.WaterColor = Me.PanelColorDialog.Color
            End If
        End Sub

        Private Sub write_Config()
            If Me.ChineseToolStripMenuItem.Checked Then
                Me.WriteINI("Setting", "Language", "cn")
            ElseIf Me.EnglishToolStripMenuItem.Checked Then
                Me.WriteINI("Setting", "Language", "en")
            End If
            Me.WriteINI("Setting", "Entrance", Conversions.ToString(Me.Entrance))
            If Me.MusicToolStripMenuItem.Checked Then
                Me.WriteINI("Setting", "Music", "on")
            Else
                Me.WriteINI("Setting", "Music", "off")
            End If
            If Me.SoundsToolStripMenuItem.Checked Then
                Me.WriteINI("Setting", "Sounds", "on")
            Else
                Me.WriteINI("Setting", "Sounds", "off")
            End If
            If Me.ColorToolStripMenuItem.Checked Then
                Me.WriteINI("Setting", "Background", ("color:" & Conversions.ToString(Me.PanelBackColor.ToArgb)))
            ElseIf Me.ImageToolStripMenuItem.Checked Then
                Me.PanelBackgroundImage.Save((Me.AppPath & "\Background"))
                Me.WriteINI("Setting", "Background", "image")
            End If
            If Me.ShowGridToolStripMenuItem.Checked Then
                Me.WriteINI("Setting", "Grid", "on")
            Else
                Me.WriteINI("Setting", "Grid", "off")
            End If
            Me.WriteINI("Setting", "FontColor", Conversions.ToString(Me.FontColor.ToArgb))
            Me.WriteINI("Setting", "WaterColor", Conversions.ToString(Me.WaterColor.ToArgb))
            Me.WriteINI("Setting", "GridColor", Conversions.ToString(Me.GridColor.ToArgb))
            Me.WriteINI("Playlist", "MusicNo", Conversions.ToString(Me.Playlist.count))
            Dim count As Integer = Me.Playlist.count
            Dim i As Integer = 1
            Do While (i <= count)
                Me.WriteINI("Playlist", Conversions.ToString(i), Me.Playlist.get_Item((i - 1)).sourceURL)
                i += 1
            Loop
        End Sub

        Public Sub Write_HighScore()
            Dim text As String = ""
            Dim num2 As Integer = (Me.HighScore.Length - 2)
            Dim i As Integer = 0
            Do While (i <= num2)
                [text] = ((([text] & Conversions.ToString(i) & "@") & Me.HighScoreName(i) & "@") & Conversions.ToString(Me.HighScore(i)) & "$")
                i += 1
            Loop
            FileSystem.WriteAllText((Me.AppPath & "\HighScore.rex"), [text], False)
        End Sub

        Public Function WriteINI(ByVal Section As String, ByVal Key As String, ByVal Value As String) As Long
            Return CLng(MainForm.WritePrivateProfileString((Section), (Key), (Value), ((Me.AppPath & "\Setting.ini"))))
        End Function

        <DllImport("kernel32", EntryPoint:="WritePrivateProfileStringA", CharSet:=CharSet.Ansi, SetLastError:=True, ExactSpelling:=True)> _
        Private Shared Function WritePrivateProfileString(<MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpApplicationName As String, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpKeyName As String, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpString As String, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpFileName As String) As Integer
        End Function


        ' Properties
        Friend Overridable Property AboutWaterPipeToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._AboutWaterPipeToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._AboutWaterPipeToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._AboutWaterPipeToolStripMenuItem.Click, New EventHandler(AddressOf Me.AboutOJPipeToolStripMenuItem_Click)
                End If
                Me._AboutWaterPipeToolStripMenuItem = WithEventsValue
                If (Not Me._AboutWaterPipeToolStripMenuItem Is Nothing) Then
                    AddHandler Me._AboutWaterPipeToolStripMenuItem.Click, New EventHandler(AddressOf Me.AboutOJPipeToolStripMenuItem_Click)
                End If
            End Set
        End Property

        Friend Overridable Property BackgroundToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._BackgroundToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                Me._BackgroundToolStripMenuItem = WithEventsValue
            End Set
        End Property

        Friend Overridable Property Box_1_0 As PictureBox
            Get
                Return Me._Box_1_0
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                Me._Box_1_0 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property Box_1_1 As PictureBox
            Get
                Return Me._Box_1_1
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_1_1 Is Nothing) Then
                    RemoveHandler Me._Box_1_1.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_1_1.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_1_1.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_1_1 = WithEventsValue
                If (Not Me._Box_1_1 Is Nothing) Then
                    AddHandler Me._Box_1_1.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_1_1.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_1_1.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_1_2 As PictureBox
            Get
                Return Me._Box_1_2
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_1_2 Is Nothing) Then
                    RemoveHandler Me._Box_1_2.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_1_2.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_1_2.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_1_2 = WithEventsValue
                If (Not Me._Box_1_2 Is Nothing) Then
                    AddHandler Me._Box_1_2.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_1_2.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_1_2.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_1_3 As PictureBox
            Get
                Return Me._Box_1_3
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_1_3 Is Nothing) Then
                    RemoveHandler Me._Box_1_3.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_1_3.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_1_3.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_1_3 = WithEventsValue
                If (Not Me._Box_1_3 Is Nothing) Then
                    AddHandler Me._Box_1_3.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_1_3.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_1_3.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_1_4 As PictureBox
            Get
                Return Me._Box_1_4
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_1_4 Is Nothing) Then
                    RemoveHandler Me._Box_1_4.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_1_4.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_1_4.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_1_4 = WithEventsValue
                If (Not Me._Box_1_4 Is Nothing) Then
                    AddHandler Me._Box_1_4.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_1_4.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_1_4.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_1_5 As PictureBox
            Get
                Return Me._Box_1_5
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_1_5 Is Nothing) Then
                    RemoveHandler Me._Box_1_5.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_1_5.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_1_5.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_1_5 = WithEventsValue
                If (Not Me._Box_1_5 Is Nothing) Then
                    AddHandler Me._Box_1_5.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_1_5.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_1_5.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_1_6 As PictureBox
            Get
                Return Me._Box_1_6
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_1_6 Is Nothing) Then
                    RemoveHandler Me._Box_1_6.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_1_6.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_1_6.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_1_6 = WithEventsValue
                If (Not Me._Box_1_6 Is Nothing) Then
                    AddHandler Me._Box_1_6.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_1_6.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_1_6.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_1_7 As PictureBox
            Get
                Return Me._Box_1_7
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_1_7 Is Nothing) Then
                    RemoveHandler Me._Box_1_7.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_1_7.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_1_7.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_1_7 = WithEventsValue
                If (Not Me._Box_1_7 Is Nothing) Then
                    AddHandler Me._Box_1_7.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_1_7.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_1_7.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_1_8 As PictureBox
            Get
                Return Me._Box_1_8
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_1_8 Is Nothing) Then
                    RemoveHandler Me._Box_1_8.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_1_8.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_1_8.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_1_8 = WithEventsValue
                If (Not Me._Box_1_8 Is Nothing) Then
                    AddHandler Me._Box_1_8.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_1_8.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_1_8.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_1_9 As PictureBox
            Get
                Return Me._Box_1_9
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_1_9 Is Nothing) Then
                    RemoveHandler Me._Box_1_9.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_1_9.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_1_9.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_1_9 = WithEventsValue
                If (Not Me._Box_1_9 Is Nothing) Then
                    AddHandler Me._Box_1_9.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_1_9.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_1_9.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_2_0 As PictureBox
            Get
                Return Me._Box_2_0
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                Me._Box_2_0 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property Box_2_1 As PictureBox
            Get
                Return Me._Box_2_1
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_2_1 Is Nothing) Then
                    RemoveHandler Me._Box_2_1.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_2_1.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_2_1.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_2_1 = WithEventsValue
                If (Not Me._Box_2_1 Is Nothing) Then
                    AddHandler Me._Box_2_1.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_2_1.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_2_1.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_2_2 As PictureBox
            Get
                Return Me._Box_2_2
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_2_2 Is Nothing) Then
                    RemoveHandler Me._Box_2_2.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_2_2.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_2_2.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_2_2 = WithEventsValue
                If (Not Me._Box_2_2 Is Nothing) Then
                    AddHandler Me._Box_2_2.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_2_2.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_2_2.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_2_3 As PictureBox
            Get
                Return Me._Box_2_3
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_2_3 Is Nothing) Then
                    RemoveHandler Me._Box_2_3.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_2_3.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_2_3.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_2_3 = WithEventsValue
                If (Not Me._Box_2_3 Is Nothing) Then
                    AddHandler Me._Box_2_3.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_2_3.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_2_3.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_2_4 As PictureBox
            Get
                Return Me._Box_2_4
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_2_4 Is Nothing) Then
                    RemoveHandler Me._Box_2_4.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_2_4.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_2_4.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_2_4 = WithEventsValue
                If (Not Me._Box_2_4 Is Nothing) Then
                    AddHandler Me._Box_2_4.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_2_4.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_2_4.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_2_5 As PictureBox
            Get
                Return Me._Box_2_5
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_2_5 Is Nothing) Then
                    RemoveHandler Me._Box_2_5.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_2_5.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_2_5.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_2_5 = WithEventsValue
                If (Not Me._Box_2_5 Is Nothing) Then
                    AddHandler Me._Box_2_5.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_2_5.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_2_5.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_2_6 As PictureBox
            Get
                Return Me._Box_2_6
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_2_6 Is Nothing) Then
                    RemoveHandler Me._Box_2_6.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_2_6.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_2_6.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_2_6 = WithEventsValue
                If (Not Me._Box_2_6 Is Nothing) Then
                    AddHandler Me._Box_2_6.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_2_6.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_2_6.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_2_7 As PictureBox
            Get
                Return Me._Box_2_7
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_2_7 Is Nothing) Then
                    RemoveHandler Me._Box_2_7.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_2_7.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_2_7.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_2_7 = WithEventsValue
                If (Not Me._Box_2_7 Is Nothing) Then
                    AddHandler Me._Box_2_7.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_2_7.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_2_7.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_2_8 As PictureBox
            Get
                Return Me._Box_2_8
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_2_8 Is Nothing) Then
                    RemoveHandler Me._Box_2_8.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_2_8.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_2_8.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_2_8 = WithEventsValue
                If (Not Me._Box_2_8 Is Nothing) Then
                    AddHandler Me._Box_2_8.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_2_8.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_2_8.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_2_9 As PictureBox
            Get
                Return Me._Box_2_9
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_2_9 Is Nothing) Then
                    RemoveHandler Me._Box_2_9.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_2_9.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_2_9.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_2_9 = WithEventsValue
                If (Not Me._Box_2_9 Is Nothing) Then
                    AddHandler Me._Box_2_9.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_2_9.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_2_9.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_3_0 As PictureBox
            Get
                Return Me._Box_3_0
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                Me._Box_3_0 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property Box_3_1 As PictureBox
            Get
                Return Me._Box_3_1
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_3_1 Is Nothing) Then
                    RemoveHandler Me._Box_3_1.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_3_1.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_3_1.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_3_1 = WithEventsValue
                If (Not Me._Box_3_1 Is Nothing) Then
                    AddHandler Me._Box_3_1.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_3_1.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_3_1.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_3_2 As PictureBox
            Get
                Return Me._Box_3_2
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_3_2 Is Nothing) Then
                    RemoveHandler Me._Box_3_2.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_3_2.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_3_2.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_3_2 = WithEventsValue
                If (Not Me._Box_3_2 Is Nothing) Then
                    AddHandler Me._Box_3_2.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_3_2.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_3_2.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_3_3 As PictureBox
            Get
                Return Me._Box_3_3
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_3_3 Is Nothing) Then
                    RemoveHandler Me._Box_3_3.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_3_3.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_3_3.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_3_3 = WithEventsValue
                If (Not Me._Box_3_3 Is Nothing) Then
                    AddHandler Me._Box_3_3.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_3_3.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_3_3.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_3_4 As PictureBox
            Get
                Return Me._Box_3_4
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_3_4 Is Nothing) Then
                    RemoveHandler Me._Box_3_4.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_3_4.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_3_4.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_3_4 = WithEventsValue
                If (Not Me._Box_3_4 Is Nothing) Then
                    AddHandler Me._Box_3_4.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_3_4.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_3_4.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_3_5 As PictureBox
            Get
                Return Me._Box_3_5
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_3_5 Is Nothing) Then
                    RemoveHandler Me._Box_3_5.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_3_5.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_3_5.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_3_5 = WithEventsValue
                If (Not Me._Box_3_5 Is Nothing) Then
                    AddHandler Me._Box_3_5.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_3_5.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_3_5.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_3_6 As PictureBox
            Get
                Return Me._Box_3_6
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_3_6 Is Nothing) Then
                    RemoveHandler Me._Box_3_6.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_3_6.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_3_6.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_3_6 = WithEventsValue
                If (Not Me._Box_3_6 Is Nothing) Then
                    AddHandler Me._Box_3_6.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_3_6.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_3_6.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_3_7 As PictureBox
            Get
                Return Me._Box_3_7
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_3_7 Is Nothing) Then
                    RemoveHandler Me._Box_3_7.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_3_7.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_3_7.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_3_7 = WithEventsValue
                If (Not Me._Box_3_7 Is Nothing) Then
                    AddHandler Me._Box_3_7.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_3_7.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_3_7.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_3_8 As PictureBox
            Get
                Return Me._Box_3_8
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_3_8 Is Nothing) Then
                    RemoveHandler Me._Box_3_8.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_3_8.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_3_8.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_3_8 = WithEventsValue
                If (Not Me._Box_3_8 Is Nothing) Then
                    AddHandler Me._Box_3_8.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_3_8.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_3_8.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_3_9 As PictureBox
            Get
                Return Me._Box_3_9
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_3_9 Is Nothing) Then
                    RemoveHandler Me._Box_3_9.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_3_9.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_3_9.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_3_9 = WithEventsValue
                If (Not Me._Box_3_9 Is Nothing) Then
                    AddHandler Me._Box_3_9.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_3_9.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_3_9.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_4_0 As PictureBox
            Get
                Return Me._Box_4_0
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                Me._Box_4_0 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property Box_4_1 As PictureBox
            Get
                Return Me._Box_4_1
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_4_1 Is Nothing) Then
                    RemoveHandler Me._Box_4_1.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_4_1.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_4_1.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_4_1 = WithEventsValue
                If (Not Me._Box_4_1 Is Nothing) Then
                    AddHandler Me._Box_4_1.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_4_1.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_4_1.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_4_2 As PictureBox
            Get
                Return Me._Box_4_2
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_4_2 Is Nothing) Then
                    RemoveHandler Me._Box_4_2.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_4_2.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_4_2.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_4_2 = WithEventsValue
                If (Not Me._Box_4_2 Is Nothing) Then
                    AddHandler Me._Box_4_2.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_4_2.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_4_2.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_4_3 As PictureBox
            Get
                Return Me._Box_4_3
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_4_3 Is Nothing) Then
                    RemoveHandler Me._Box_4_3.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_4_3.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_4_3.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_4_3 = WithEventsValue
                If (Not Me._Box_4_3 Is Nothing) Then
                    AddHandler Me._Box_4_3.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_4_3.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_4_3.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_4_4 As PictureBox
            Get
                Return Me._Box_4_4
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_4_4 Is Nothing) Then
                    RemoveHandler Me._Box_4_4.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_4_4.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_4_4.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_4_4 = WithEventsValue
                If (Not Me._Box_4_4 Is Nothing) Then
                    AddHandler Me._Box_4_4.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_4_4.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_4_4.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_4_5 As PictureBox
            Get
                Return Me._Box_4_5
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_4_5 Is Nothing) Then
                    RemoveHandler Me._Box_4_5.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_4_5.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_4_5.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_4_5 = WithEventsValue
                If (Not Me._Box_4_5 Is Nothing) Then
                    AddHandler Me._Box_4_5.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_4_5.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_4_5.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_4_6 As PictureBox
            Get
                Return Me._Box_4_6
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_4_6 Is Nothing) Then
                    RemoveHandler Me._Box_4_6.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_4_6.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_4_6.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_4_6 = WithEventsValue
                If (Not Me._Box_4_6 Is Nothing) Then
                    AddHandler Me._Box_4_6.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_4_6.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_4_6.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_4_7 As PictureBox
            Get
                Return Me._Box_4_7
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_4_7 Is Nothing) Then
                    RemoveHandler Me._Box_4_7.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_4_7.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_4_7.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_4_7 = WithEventsValue
                If (Not Me._Box_4_7 Is Nothing) Then
                    AddHandler Me._Box_4_7.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_4_7.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_4_7.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_4_8 As PictureBox
            Get
                Return Me._Box_4_8
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_4_8 Is Nothing) Then
                    RemoveHandler Me._Box_4_8.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_4_8.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_4_8.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_4_8 = WithEventsValue
                If (Not Me._Box_4_8 Is Nothing) Then
                    AddHandler Me._Box_4_8.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_4_8.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_4_8.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_4_9 As PictureBox
            Get
                Return Me._Box_4_9
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_4_9 Is Nothing) Then
                    RemoveHandler Me._Box_4_9.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_4_9.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_4_9.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_4_9 = WithEventsValue
                If (Not Me._Box_4_9 Is Nothing) Then
                    AddHandler Me._Box_4_9.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_4_9.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_4_9.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_5_0 As PictureBox
            Get
                Return Me._Box_5_0
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                Me._Box_5_0 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property Box_5_1 As PictureBox
            Get
                Return Me._Box_5_1
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_5_1 Is Nothing) Then
                    RemoveHandler Me._Box_5_1.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_5_1.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_5_1.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_5_1 = WithEventsValue
                If (Not Me._Box_5_1 Is Nothing) Then
                    AddHandler Me._Box_5_1.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_5_1.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_5_1.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_5_2 As PictureBox
            Get
                Return Me._Box_5_2
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_5_2 Is Nothing) Then
                    RemoveHandler Me._Box_5_2.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_5_2.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_5_2.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_5_2 = WithEventsValue
                If (Not Me._Box_5_2 Is Nothing) Then
                    AddHandler Me._Box_5_2.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_5_2.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_5_2.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_5_3 As PictureBox
            Get
                Return Me._Box_5_3
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_5_3 Is Nothing) Then
                    RemoveHandler Me._Box_5_3.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_5_3.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_5_3.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_5_3 = WithEventsValue
                If (Not Me._Box_5_3 Is Nothing) Then
                    AddHandler Me._Box_5_3.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_5_3.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_5_3.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_5_4 As PictureBox
            Get
                Return Me._Box_5_4
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_5_4 Is Nothing) Then
                    RemoveHandler Me._Box_5_4.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_5_4.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_5_4.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_5_4 = WithEventsValue
                If (Not Me._Box_5_4 Is Nothing) Then
                    AddHandler Me._Box_5_4.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_5_4.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_5_4.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_5_5 As PictureBox
            Get
                Return Me._Box_5_5
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_5_5 Is Nothing) Then
                    RemoveHandler Me._Box_5_5.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_5_5.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_5_5.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_5_5 = WithEventsValue
                If (Not Me._Box_5_5 Is Nothing) Then
                    AddHandler Me._Box_5_5.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_5_5.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_5_5.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_5_6 As PictureBox
            Get
                Return Me._Box_5_6
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_5_6 Is Nothing) Then
                    RemoveHandler Me._Box_5_6.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_5_6.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_5_6.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_5_6 = WithEventsValue
                If (Not Me._Box_5_6 Is Nothing) Then
                    AddHandler Me._Box_5_6.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_5_6.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_5_6.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_5_7 As PictureBox
            Get
                Return Me._Box_5_7
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_5_7 Is Nothing) Then
                    RemoveHandler Me._Box_5_7.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_5_7.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_5_7.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_5_7 = WithEventsValue
                If (Not Me._Box_5_7 Is Nothing) Then
                    AddHandler Me._Box_5_7.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_5_7.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_5_7.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_5_8 As PictureBox
            Get
                Return Me._Box_5_8
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_5_8 Is Nothing) Then
                    RemoveHandler Me._Box_5_8.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_5_8.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_5_8.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_5_8 = WithEventsValue
                If (Not Me._Box_5_8 Is Nothing) Then
                    AddHandler Me._Box_5_8.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_5_8.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_5_8.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_5_9 As PictureBox
            Get
                Return Me._Box_5_9
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_5_9 Is Nothing) Then
                    RemoveHandler Me._Box_5_9.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_5_9.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_5_9.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_5_9 = WithEventsValue
                If (Not Me._Box_5_9 Is Nothing) Then
                    AddHandler Me._Box_5_9.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_5_9.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_5_9.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_6_0 As PictureBox
            Get
                Return Me._Box_6_0
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                Me._Box_6_0 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property Box_6_1 As PictureBox
            Get
                Return Me._Box_6_1
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_6_1 Is Nothing) Then
                    RemoveHandler Me._Box_6_1.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_6_1.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_6_1.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_6_1 = WithEventsValue
                If (Not Me._Box_6_1 Is Nothing) Then
                    AddHandler Me._Box_6_1.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_6_1.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_6_1.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_6_2 As PictureBox
            Get
                Return Me._Box_6_2
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_6_2 Is Nothing) Then
                    RemoveHandler Me._Box_6_2.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_6_2.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_6_2.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_6_2 = WithEventsValue
                If (Not Me._Box_6_2 Is Nothing) Then
                    AddHandler Me._Box_6_2.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_6_2.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_6_2.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_6_3 As PictureBox
            Get
                Return Me._Box_6_3
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_6_3 Is Nothing) Then
                    RemoveHandler Me._Box_6_3.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_6_3.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_6_3.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_6_3 = WithEventsValue
                If (Not Me._Box_6_3 Is Nothing) Then
                    AddHandler Me._Box_6_3.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_6_3.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_6_3.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_6_4 As PictureBox
            Get
                Return Me._Box_6_4
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_6_4 Is Nothing) Then
                    RemoveHandler Me._Box_6_4.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_6_4.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_6_4.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_6_4 = WithEventsValue
                If (Not Me._Box_6_4 Is Nothing) Then
                    AddHandler Me._Box_6_4.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_6_4.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_6_4.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_6_5 As PictureBox
            Get
                Return Me._Box_6_5
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_6_5 Is Nothing) Then
                    RemoveHandler Me._Box_6_5.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_6_5.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_6_5.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_6_5 = WithEventsValue
                If (Not Me._Box_6_5 Is Nothing) Then
                    AddHandler Me._Box_6_5.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_6_5.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_6_5.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_6_6 As PictureBox
            Get
                Return Me._Box_6_6
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_6_6 Is Nothing) Then
                    RemoveHandler Me._Box_6_6.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_6_6.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_6_6.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_6_6 = WithEventsValue
                If (Not Me._Box_6_6 Is Nothing) Then
                    AddHandler Me._Box_6_6.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_6_6.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_6_6.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_6_7 As PictureBox
            Get
                Return Me._Box_6_7
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_6_7 Is Nothing) Then
                    RemoveHandler Me._Box_6_7.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_6_7.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_6_7.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_6_7 = WithEventsValue
                If (Not Me._Box_6_7 Is Nothing) Then
                    AddHandler Me._Box_6_7.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_6_7.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_6_7.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_6_8 As PictureBox
            Get
                Return Me._Box_6_8
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_6_8 Is Nothing) Then
                    RemoveHandler Me._Box_6_8.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_6_8.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_6_8.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_6_8 = WithEventsValue
                If (Not Me._Box_6_8 Is Nothing) Then
                    AddHandler Me._Box_6_8.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_6_8.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_6_8.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_6_9 As PictureBox
            Get
                Return Me._Box_6_9
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_6_9 Is Nothing) Then
                    RemoveHandler Me._Box_6_9.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_6_9.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_6_9.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_6_9 = WithEventsValue
                If (Not Me._Box_6_9 Is Nothing) Then
                    AddHandler Me._Box_6_9.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_6_9.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_6_9.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_7_0 As PictureBox
            Get
                Return Me._Box_7_0
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                Me._Box_7_0 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property Box_7_1 As PictureBox
            Get
                Return Me._Box_7_1
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_7_1 Is Nothing) Then
                    RemoveHandler Me._Box_7_1.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_7_1.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_7_1.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_7_1 = WithEventsValue
                If (Not Me._Box_7_1 Is Nothing) Then
                    AddHandler Me._Box_7_1.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_7_1.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_7_1.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_7_2 As PictureBox
            Get
                Return Me._Box_7_2
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_7_2 Is Nothing) Then
                    RemoveHandler Me._Box_7_2.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_7_2.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_7_2.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_7_2 = WithEventsValue
                If (Not Me._Box_7_2 Is Nothing) Then
                    AddHandler Me._Box_7_2.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_7_2.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_7_2.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_7_3 As PictureBox
            Get
                Return Me._Box_7_3
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_7_3 Is Nothing) Then
                    RemoveHandler Me._Box_7_3.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_7_3.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_7_3.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_7_3 = WithEventsValue
                If (Not Me._Box_7_3 Is Nothing) Then
                    AddHandler Me._Box_7_3.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_7_3.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_7_3.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_7_4 As PictureBox
            Get
                Return Me._Box_7_4
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_7_4 Is Nothing) Then
                    RemoveHandler Me._Box_7_4.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_7_4.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_7_4.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_7_4 = WithEventsValue
                If (Not Me._Box_7_4 Is Nothing) Then
                    AddHandler Me._Box_7_4.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_7_4.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_7_4.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_7_5 As PictureBox
            Get
                Return Me._Box_7_5
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_7_5 Is Nothing) Then
                    RemoveHandler Me._Box_7_5.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_7_5.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_7_5.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_7_5 = WithEventsValue
                If (Not Me._Box_7_5 Is Nothing) Then
                    AddHandler Me._Box_7_5.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_7_5.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_7_5.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_7_6 As PictureBox
            Get
                Return Me._Box_7_6
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_7_6 Is Nothing) Then
                    RemoveHandler Me._Box_7_6.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_7_6.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_7_6.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_7_6 = WithEventsValue
                If (Not Me._Box_7_6 Is Nothing) Then
                    AddHandler Me._Box_7_6.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_7_6.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_7_6.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_7_7 As PictureBox
            Get
                Return Me._Box_7_7
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_7_7 Is Nothing) Then
                    RemoveHandler Me._Box_7_7.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_7_7.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_7_7.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_7_7 = WithEventsValue
                If (Not Me._Box_7_7 Is Nothing) Then
                    AddHandler Me._Box_7_7.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_7_7.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_7_7.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_7_8 As PictureBox
            Get
                Return Me._Box_7_8
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_7_8 Is Nothing) Then
                    RemoveHandler Me._Box_7_8.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_7_8.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_7_8.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_7_8 = WithEventsValue
                If (Not Me._Box_7_8 Is Nothing) Then
                    AddHandler Me._Box_7_8.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_7_8.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_7_8.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_7_9 As PictureBox
            Get
                Return Me._Box_7_9
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_7_9 Is Nothing) Then
                    RemoveHandler Me._Box_7_9.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_7_9.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_7_9.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_7_9 = WithEventsValue
                If (Not Me._Box_7_9 Is Nothing) Then
                    AddHandler Me._Box_7_9.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_7_9.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_7_9.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_8_0 As PictureBox
            Get
                Return Me._Box_8_0
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                Me._Box_8_0 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property Box_8_1 As PictureBox
            Get
                Return Me._Box_8_1
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_8_1 Is Nothing) Then
                    RemoveHandler Me._Box_8_1.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_8_1.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_8_1.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_8_1 = WithEventsValue
                If (Not Me._Box_8_1 Is Nothing) Then
                    AddHandler Me._Box_8_1.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_8_1.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_8_1.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_8_2 As PictureBox
            Get
                Return Me._Box_8_2
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_8_2 Is Nothing) Then
                    RemoveHandler Me._Box_8_2.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_8_2.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_8_2.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_8_2 = WithEventsValue
                If (Not Me._Box_8_2 Is Nothing) Then
                    AddHandler Me._Box_8_2.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_8_2.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_8_2.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_8_3 As PictureBox
            Get
                Return Me._Box_8_3
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_8_3 Is Nothing) Then
                    RemoveHandler Me._Box_8_3.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_8_3.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_8_3.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_8_3 = WithEventsValue
                If (Not Me._Box_8_3 Is Nothing) Then
                    AddHandler Me._Box_8_3.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_8_3.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_8_3.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_8_4 As PictureBox
            Get
                Return Me._Box_8_4
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_8_4 Is Nothing) Then
                    RemoveHandler Me._Box_8_4.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_8_4.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_8_4.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_8_4 = WithEventsValue
                If (Not Me._Box_8_4 Is Nothing) Then
                    AddHandler Me._Box_8_4.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_8_4.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_8_4.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_8_5 As PictureBox
            Get
                Return Me._Box_8_5
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_8_5 Is Nothing) Then
                    RemoveHandler Me._Box_8_5.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_8_5.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_8_5.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_8_5 = WithEventsValue
                If (Not Me._Box_8_5 Is Nothing) Then
                    AddHandler Me._Box_8_5.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_8_5.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_8_5.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_8_6 As PictureBox
            Get
                Return Me._Box_8_6
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_8_6 Is Nothing) Then
                    RemoveHandler Me._Box_8_6.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_8_6.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_8_6.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_8_6 = WithEventsValue
                If (Not Me._Box_8_6 Is Nothing) Then
                    AddHandler Me._Box_8_6.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_8_6.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_8_6.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_8_7 As PictureBox
            Get
                Return Me._Box_8_7
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_8_7 Is Nothing) Then
                    RemoveHandler Me._Box_8_7.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_8_7.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_8_7.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_8_7 = WithEventsValue
                If (Not Me._Box_8_7 Is Nothing) Then
                    AddHandler Me._Box_8_7.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_8_7.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_8_7.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_8_8 As PictureBox
            Get
                Return Me._Box_8_8
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_8_8 Is Nothing) Then
                    RemoveHandler Me._Box_8_8.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_8_8.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_8_8.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_8_8 = WithEventsValue
                If (Not Me._Box_8_8 Is Nothing) Then
                    AddHandler Me._Box_8_8.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_8_8.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_8_8.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_8_9 As PictureBox
            Get
                Return Me._Box_8_9
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_8_9 Is Nothing) Then
                    RemoveHandler Me._Box_8_9.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_8_9.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_8_9.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_8_9 = WithEventsValue
                If (Not Me._Box_8_9 Is Nothing) Then
                    AddHandler Me._Box_8_9.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_8_9.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_8_9.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_9_0 As PictureBox
            Get
                Return Me._Box_9_0
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                Me._Box_9_0 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property Box_9_1 As PictureBox
            Get
                Return Me._Box_9_1
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_9_1 Is Nothing) Then
                    RemoveHandler Me._Box_9_1.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_9_1.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_9_1.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_9_1 = WithEventsValue
                If (Not Me._Box_9_1 Is Nothing) Then
                    AddHandler Me._Box_9_1.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_9_1.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_9_1.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_9_2 As PictureBox
            Get
                Return Me._Box_9_2
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_9_2 Is Nothing) Then
                    RemoveHandler Me._Box_9_2.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_9_2.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_9_2.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_9_2 = WithEventsValue
                If (Not Me._Box_9_2 Is Nothing) Then
                    AddHandler Me._Box_9_2.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_9_2.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_9_2.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_9_3 As PictureBox
            Get
                Return Me._Box_9_3
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_9_3 Is Nothing) Then
                    RemoveHandler Me._Box_9_3.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_9_3.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_9_3.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_9_3 = WithEventsValue
                If (Not Me._Box_9_3 Is Nothing) Then
                    AddHandler Me._Box_9_3.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_9_3.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_9_3.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_9_4 As PictureBox
            Get
                Return Me._Box_9_4
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_9_4 Is Nothing) Then
                    RemoveHandler Me._Box_9_4.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_9_4.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_9_4.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_9_4 = WithEventsValue
                If (Not Me._Box_9_4 Is Nothing) Then
                    AddHandler Me._Box_9_4.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_9_4.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_9_4.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_9_5 As PictureBox
            Get
                Return Me._Box_9_5
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_9_5 Is Nothing) Then
                    RemoveHandler Me._Box_9_5.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_9_5.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_9_5.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_9_5 = WithEventsValue
                If (Not Me._Box_9_5 Is Nothing) Then
                    AddHandler Me._Box_9_5.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_9_5.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_9_5.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_9_6 As PictureBox
            Get
                Return Me._Box_9_6
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_9_6 Is Nothing) Then
                    RemoveHandler Me._Box_9_6.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_9_6.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_9_6.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_9_6 = WithEventsValue
                If (Not Me._Box_9_6 Is Nothing) Then
                    AddHandler Me._Box_9_6.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_9_6.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_9_6.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_9_7 As PictureBox
            Get
                Return Me._Box_9_7
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_9_7 Is Nothing) Then
                    RemoveHandler Me._Box_9_7.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_9_7.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_9_7.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_9_7 = WithEventsValue
                If (Not Me._Box_9_7 Is Nothing) Then
                    AddHandler Me._Box_9_7.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_9_7.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_9_7.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_9_8 As PictureBox
            Get
                Return Me._Box_9_8
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_9_8 Is Nothing) Then
                    RemoveHandler Me._Box_9_8.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_9_8.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_9_8.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_9_8 = WithEventsValue
                If (Not Me._Box_9_8 Is Nothing) Then
                    AddHandler Me._Box_9_8.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_9_8.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_9_8.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Box_9_9 As PictureBox
            Get
                Return Me._Box_9_9
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                If (Not Me._Box_9_9 Is Nothing) Then
                    RemoveHandler Me._Box_9_9.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    RemoveHandler Me._Box_9_9.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    RemoveHandler Me._Box_9_9.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
                Me._Box_9_9 = WithEventsValue
                If (Not Me._Box_9_9 Is Nothing) Then
                    AddHandler Me._Box_9_9.MouseLeave, New EventHandler(AddressOf Me.Box_MouseLeave)
                    AddHandler Me._Box_9_9.MouseEnter, New EventHandler(AddressOf Me.Box_MouseEnter)
                    AddHandler Me._Box_9_9.Click, New EventHandler(AddressOf Me.Box_Click)
                End If
            End Set
        End Property

        Friend Overridable Property ChineseToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._ChineseToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._ChineseToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._ChineseToolStripMenuItem.Click, New EventHandler(AddressOf Me.LanguageChange_Click)
                End If
                Me._ChineseToolStripMenuItem = WithEventsValue
                If (Not Me._ChineseToolStripMenuItem Is Nothing) Then
                    AddHandler Me._ChineseToolStripMenuItem.Click, New EventHandler(AddressOf Me.LanguageChange_Click)
                End If
            End Set
        End Property

        Friend Overridable Property ColorToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._ColorToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._ColorToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._ColorToolStripMenuItem.Click, New EventHandler(AddressOf Me.ColorToolStripMenuItem_Click)
                End If
                Me._ColorToolStripMenuItem = WithEventsValue
                If (Not Me._ColorToolStripMenuItem Is Nothing) Then
                    AddHandler Me._ColorToolStripMenuItem.Click, New EventHandler(AddressOf Me.ColorToolStripMenuItem_Click)
                End If
            End Set
        End Property

        Friend Overridable Property EnglishToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._EnglishToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._EnglishToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._EnglishToolStripMenuItem.Click, New EventHandler(AddressOf Me.LanguageChange_Click)
                End If
                Me._EnglishToolStripMenuItem = WithEventsValue
                If (Not Me._EnglishToolStripMenuItem Is Nothing) Then
                    AddHandler Me._EnglishToolStripMenuItem.Click, New EventHandler(AddressOf Me.LanguageChange_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Entrance1ToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._Entrance1ToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._Entrance1ToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._Entrance1ToolStripMenuItem.Click, New EventHandler(AddressOf Me.EntranceToolStripMenuItem_Click)
                End If
                Me._Entrance1ToolStripMenuItem = WithEventsValue
                If (Not Me._Entrance1ToolStripMenuItem Is Nothing) Then
                    AddHandler Me._Entrance1ToolStripMenuItem.Click, New EventHandler(AddressOf Me.EntranceToolStripMenuItem_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Entrance2ToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._Entrance2ToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._Entrance2ToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._Entrance2ToolStripMenuItem.Click, New EventHandler(AddressOf Me.EntranceToolStripMenuItem_Click)
                End If
                Me._Entrance2ToolStripMenuItem = WithEventsValue
                If (Not Me._Entrance2ToolStripMenuItem Is Nothing) Then
                    AddHandler Me._Entrance2ToolStripMenuItem.Click, New EventHandler(AddressOf Me.EntranceToolStripMenuItem_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Entrance3ToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._Entrance3ToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._Entrance3ToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._Entrance3ToolStripMenuItem.Click, New EventHandler(AddressOf Me.EntranceToolStripMenuItem_Click)
                End If
                Me._Entrance3ToolStripMenuItem = WithEventsValue
                If (Not Me._Entrance3ToolStripMenuItem Is Nothing) Then
                    AddHandler Me._Entrance3ToolStripMenuItem.Click, New EventHandler(AddressOf Me.EntranceToolStripMenuItem_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Entrance4ToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._Entrance4ToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._Entrance4ToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._Entrance4ToolStripMenuItem.Click, New EventHandler(AddressOf Me.EntranceToolStripMenuItem_Click)
                End If
                Me._Entrance4ToolStripMenuItem = WithEventsValue
                If (Not Me._Entrance4ToolStripMenuItem Is Nothing) Then
                    AddHandler Me._Entrance4ToolStripMenuItem.Click, New EventHandler(AddressOf Me.EntranceToolStripMenuItem_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Entrance5ToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._Entrance5ToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._Entrance5ToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._Entrance5ToolStripMenuItem.Click, New EventHandler(AddressOf Me.EntranceToolStripMenuItem_Click)
                End If
                Me._Entrance5ToolStripMenuItem = WithEventsValue
                If (Not Me._Entrance5ToolStripMenuItem Is Nothing) Then
                    AddHandler Me._Entrance5ToolStripMenuItem.Click, New EventHandler(AddressOf Me.EntranceToolStripMenuItem_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Entrance6ToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._Entrance6ToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._Entrance6ToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._Entrance6ToolStripMenuItem.Click, New EventHandler(AddressOf Me.EntranceToolStripMenuItem_Click)
                End If
                Me._Entrance6ToolStripMenuItem = WithEventsValue
                If (Not Me._Entrance6ToolStripMenuItem Is Nothing) Then
                    AddHandler Me._Entrance6ToolStripMenuItem.Click, New EventHandler(AddressOf Me.EntranceToolStripMenuItem_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Entrance7ToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._Entrance7ToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._Entrance7ToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._Entrance7ToolStripMenuItem.Click, New EventHandler(AddressOf Me.EntranceToolStripMenuItem_Click)
                End If
                Me._Entrance7ToolStripMenuItem = WithEventsValue
                If (Not Me._Entrance7ToolStripMenuItem Is Nothing) Then
                    AddHandler Me._Entrance7ToolStripMenuItem.Click, New EventHandler(AddressOf Me.EntranceToolStripMenuItem_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Entrance8ToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._Entrance8ToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._Entrance8ToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._Entrance8ToolStripMenuItem.Click, New EventHandler(AddressOf Me.EntranceToolStripMenuItem_Click)
                End If
                Me._Entrance8ToolStripMenuItem = WithEventsValue
                If (Not Me._Entrance8ToolStripMenuItem Is Nothing) Then
                    AddHandler Me._Entrance8ToolStripMenuItem.Click, New EventHandler(AddressOf Me.EntranceToolStripMenuItem_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Entrance9ToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._Entrance9ToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._Entrance9ToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._Entrance9ToolStripMenuItem.Click, New EventHandler(AddressOf Me.EntranceToolStripMenuItem_Click)
                End If
                Me._Entrance9ToolStripMenuItem = WithEventsValue
                If (Not Me._Entrance9ToolStripMenuItem Is Nothing) Then
                    AddHandler Me._Entrance9ToolStripMenuItem.Click, New EventHandler(AddressOf Me.EntranceToolStripMenuItem_Click)
                End If
            End Set
        End Property

        Friend Overridable Property EntranceQuantityToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._EntranceQuantityToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                Me._EntranceQuantityToolStripMenuItem = WithEventsValue
            End Set
        End Property

        Friend Overridable Property ExitToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._ExitToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._ExitToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._ExitToolStripMenuItem.Click, New EventHandler(AddressOf Me.ExitToolStripMenuItem_Click)
                End If
                Me._ExitToolStripMenuItem = WithEventsValue
                If (Not Me._ExitToolStripMenuItem Is Nothing) Then
                    AddHandler Me._ExitToolStripMenuItem.Click, New EventHandler(AddressOf Me.ExitToolStripMenuItem_Click)
                End If
            End Set
        End Property

        Friend Overridable Property FontColorToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._FontColorToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._FontColorToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._FontColorToolStripMenuItem.Click, New EventHandler(AddressOf Me.FontColorToolStripMenuItem_Click)
                End If
                Me._FontColorToolStripMenuItem = WithEventsValue
                If (Not Me._FontColorToolStripMenuItem Is Nothing) Then
                    AddHandler Me._FontColorToolStripMenuItem.Click, New EventHandler(AddressOf Me.FontColorToolStripMenuItem_Click)
                End If
            End Set
        End Property

        Friend Overridable Property GameToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._GameToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                Me._GameToolStripMenuItem = WithEventsValue
            End Set
        End Property

        Friend Overridable Property GridColorToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._GridColorToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._GridColorToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._GridColorToolStripMenuItem.Click, New EventHandler(AddressOf Me.GridColorToolStripMenuItem_Click)
                End If
                Me._GridColorToolStripMenuItem = WithEventsValue
                If (Not Me._GridColorToolStripMenuItem Is Nothing) Then
                    AddHandler Me._GridColorToolStripMenuItem.Click, New EventHandler(AddressOf Me.GridColorToolStripMenuItem_Click)
                End If
            End Set
        End Property

        Friend Overridable Property GridToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._GridToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                Me._GridToolStripMenuItem = WithEventsValue
            End Set
        End Property

        Friend Overridable Property HelpToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._HelpToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                Me._HelpToolStripMenuItem = WithEventsValue
            End Set
        End Property

        Friend Overridable Property HelpTopicsToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._HelpTopicsToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                Me._HelpTopicsToolStripMenuItem = WithEventsValue
            End Set
        End Property

        Friend Overridable Property HighScoresToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._HighScoresToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._HighScoresToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._HighScoresToolStripMenuItem.Click, New EventHandler(AddressOf Me.HighScoresToolStripMenuItem_Click)
                End If
                Me._HighScoresToolStripMenuItem = WithEventsValue
                If (Not Me._HighScoresToolStripMenuItem Is Nothing) Then
                    AddHandler Me._HighScoresToolStripMenuItem.Click, New EventHandler(AddressOf Me.HighScoresToolStripMenuItem_Click)
                End If
            End Set
        End Property

        Friend Overridable Property ImageToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._ImageToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._ImageToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._ImageToolStripMenuItem.Click, New EventHandler(AddressOf Me.ImageToolStripMenuItem_Click)
                End If
                Me._ImageToolStripMenuItem = WithEventsValue
                If (Not Me._ImageToolStripMenuItem Is Nothing) Then
                    AddHandler Me._ImageToolStripMenuItem.Click, New EventHandler(AddressOf Me.ImageToolStripMenuItem_Click)
                End If
            End Set
        End Property

        Friend Overridable Property InWater As Button
            Get
                Return Me._InWater
            End Get
            Set(ByVal WithEventsValue As Button)
                If (Not Me._InWater Is Nothing) Then
                    RemoveHandler Me._InWater.Click, New EventHandler(AddressOf Me.InWater_Click)
                End If
                Me._InWater = WithEventsValue
                If (Not Me._InWater Is Nothing) Then
                    AddHandler Me._InWater.Click, New EventHandler(AddressOf Me.InWater_Click)
                End If
            End Set
        End Property

        Friend Overridable Property LanguageToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._LanguageToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                Me._LanguageToolStripMenuItem = WithEventsValue
            End Set
        End Property

        Friend Overridable Property MainPanel As Panel
            Get
                Return Me._MainPanel
            End Get
            Set(ByVal WithEventsValue As Panel)
                Me._MainPanel = WithEventsValue
            End Set
        End Property

        Private Overridable Property MediaPlayer As WindowsMediaPlayer
            Get
                Return Me._MediaPlayer
            End Get
            Set(ByVal WithEventsValue As WindowsMediaPlayer)
                If (Not Me._MediaPlayer Is Nothing) Then
                    Me._MediaPlayer.remove_MediaError(New _WMPOCXEvents_MediaErrorEventHandler(AddressOf Me.Player_MediaError))
                End If
                Me._MediaPlayer = WithEventsValue
                If (Not Me._MediaPlayer Is Nothing) Then
                    Me._MediaPlayer.add_MediaError(New _WMPOCXEvents_MediaErrorEventHandler(AddressOf Me.Player_MediaError))
                End If
            End Set
        End Property

        Friend Overridable Property MenuStrip As MenuStrip
            Get
                Return Me._MenuStrip
            End Get
            Set(ByVal WithEventsValue As MenuStrip)
                Me._MenuStrip = WithEventsValue
            End Set
        End Property

        Friend Overridable Property MusicToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._MusicToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._MusicToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._MusicToolStripMenuItem.Click, New EventHandler(AddressOf Me.MusicToolStripMenuItem_Click)
                End If
                Me._MusicToolStripMenuItem = WithEventsValue
                If (Not Me._MusicToolStripMenuItem Is Nothing) Then
                    AddHandler Me._MusicToolStripMenuItem.Click, New EventHandler(AddressOf Me.MusicToolStripMenuItem_Click)
                End If
            End Set
        End Property

        Friend Overridable Property NewGameToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._NewGameToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._NewGameToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._NewGameToolStripMenuItem.Click, New EventHandler(AddressOf Me.NewGameToolStripMenuItem_Click)
                End If
                Me._NewGameToolStripMenuItem = WithEventsValue
                If (Not Me._NewGameToolStripMenuItem Is Nothing) Then
                    AddHandler Me._NewGameToolStripMenuItem.Click, New EventHandler(AddressOf Me.NewGameToolStripMenuItem_Click)
                End If
            End Set
        End Property

        Friend Overridable Property Next_1 As PictureBox
            Get
                Return Me._Next_1
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                Me._Next_1 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property Next_2 As PictureBox
            Get
                Return Me._Next_2
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                Me._Next_2 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property Next_3 As PictureBox
            Get
                Return Me._Next_3
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                Me._Next_3 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property Next_4 As PictureBox
            Get
                Return Me._Next_4
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                Me._Next_4 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property Next_5 As PictureBox
            Get
                Return Me._Next_5
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                Me._Next_5 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property Next_6 As PictureBox
            Get
                Return Me._Next_6
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                Me._Next_6 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property Next_7 As PictureBox
            Get
                Return Me._Next_7
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                Me._Next_7 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property Next_8 As PictureBox
            Get
                Return Me._Next_8
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                Me._Next_8 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property Next_9 As PictureBox
            Get
                Return Me._Next_9
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                Me._Next_9 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property NextLabel As Label
            Get
                Return Me._NextLabel
            End Get
            Set(ByVal WithEventsValue As Label)
                Me._NextLabel = WithEventsValue
            End Set
        End Property

        Friend Overridable Property OptionsToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._OptionsToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                Me._OptionsToolStripMenuItem = WithEventsValue
            End Set
        End Property

        Friend Overridable Property OutBondPictureBox As PictureBox
            Get
                Return Me._OutBondPictureBox
            End Get
            Set(ByVal WithEventsValue As PictureBox)
                Me._OutBondPictureBox = WithEventsValue
            End Set
        End Property

        Friend Overridable Property PanelBackgroundImageFileDialog As OpenFileDialog
            Get
                Return Me._PanelBackgroundImageFileDialog
            End Get
            Set(ByVal WithEventsValue As OpenFileDialog)
                Me._PanelBackgroundImageFileDialog = WithEventsValue
            End Set
        End Property

        Private Overridable Property PanelColorDialog As ColorDialog
            Get
                Return Me._PanelColorDialog
            End Get
            Set(ByVal WithEventsValue As ColorDialog)
                Me._PanelColorDialog = WithEventsValue
            End Set
        End Property

        Friend Overridable Property ScoreBar As ProgressBar
            Get
                Return Me._ScoreBar
            End Get
            Set(ByVal WithEventsValue As ProgressBar)
                Me._ScoreBar = WithEventsValue
            End Set
        End Property

        Friend Overridable Property ScoreLabel As Label
            Get
                Return Me._ScoreLabel
            End Get
            Set(ByVal WithEventsValue As Label)
                Me._ScoreLabel = WithEventsValue
            End Set
        End Property

        Friend Overridable Property ScoreNumberLabel As Label
            Get
                Return Me._ScoreNumberLabel
            End Get
            Set(ByVal WithEventsValue As Label)
                Me._ScoreNumberLabel = WithEventsValue
            End Set
        End Property

        Friend Overridable Property SelectColorToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._SelectColorToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._SelectColorToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._SelectColorToolStripMenuItem.Click, New EventHandler(AddressOf Me.SelectColorToolStripMenuItem_Click)
                End If
                Me._SelectColorToolStripMenuItem = WithEventsValue
                If (Not Me._SelectColorToolStripMenuItem Is Nothing) Then
                    AddHandler Me._SelectColorToolStripMenuItem.Click, New EventHandler(AddressOf Me.SelectColorToolStripMenuItem_Click)
                End If
            End Set
        End Property

        Friend Overridable Property SelectImageToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._SelectImageToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._SelectImageToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._SelectImageToolStripMenuItem.Click, New EventHandler(AddressOf Me.SelectImageToolStripMenuItem_Click)
                End If
                Me._SelectImageToolStripMenuItem = WithEventsValue
                If (Not Me._SelectImageToolStripMenuItem Is Nothing) Then
                    AddHandler Me._SelectImageToolStripMenuItem.Click, New EventHandler(AddressOf Me.SelectImageToolStripMenuItem_Click)
                End If
            End Set
        End Property

        Friend Overridable Property SelectMusicDialog As OpenFileDialog
            Get
                Return Me._SelectMusicDialog
            End Get
            Set(ByVal WithEventsValue As OpenFileDialog)
                Me._SelectMusicDialog = WithEventsValue
            End Set
        End Property

        Friend Overridable Property SelectMusicToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._SelectMusicToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._SelectMusicToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._SelectMusicToolStripMenuItem.Click, New EventHandler(AddressOf Me.SelectMusicToolStripMenuItem_Click)
                End If
                Me._SelectMusicToolStripMenuItem = WithEventsValue
                If (Not Me._SelectMusicToolStripMenuItem Is Nothing) Then
                    AddHandler Me._SelectMusicToolStripMenuItem.Click, New EventHandler(AddressOf Me.SelectMusicToolStripMenuItem_Click)
                End If
            End Set
        End Property

        Friend Overridable Property ShowGridToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._ShowGridToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._ShowGridToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._ShowGridToolStripMenuItem.Click, New EventHandler(AddressOf Me.ShowGridToolStripMenuItem_Click)
                End If
                Me._ShowGridToolStripMenuItem = WithEventsValue
                If (Not Me._ShowGridToolStripMenuItem Is Nothing) Then
                    AddHandler Me._ShowGridToolStripMenuItem.Click, New EventHandler(AddressOf Me.ShowGridToolStripMenuItem_Click)
                End If
            End Set
        End Property

        Friend Overridable Property SoundsToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._SoundsToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                Me._SoundsToolStripMenuItem = WithEventsValue
            End Set
        End Property

        Friend Overridable Property ToolStripSeparator1 As ToolStripSeparator
            Get
                Return Me._ToolStripSeparator1
            End Get
            Set(ByVal WithEventsValue As ToolStripSeparator)
                Me._ToolStripSeparator1 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property ToolStripSeparator2 As ToolStripSeparator
            Get
                Return Me._ToolStripSeparator2
            End Get
            Set(ByVal WithEventsValue As ToolStripSeparator)
                Me._ToolStripSeparator2 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property ToolStripSeparator3 As ToolStripSeparator
            Get
                Return Me._ToolStripSeparator3
            End Get
            Set(ByVal WithEventsValue As ToolStripSeparator)
                Me._ToolStripSeparator3 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property ToolStripSeparator4 As ToolStripSeparator
            Get
                Return Me._ToolStripSeparator4
            End Get
            Set(ByVal WithEventsValue As ToolStripSeparator)
                Me._ToolStripSeparator4 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property ToolStripSeparator5 As ToolStripSeparator
            Get
                Return Me._ToolStripSeparator5
            End Get
            Set(ByVal WithEventsValue As ToolStripSeparator)
                Me._ToolStripSeparator5 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property ToolStripSeparator6 As ToolStripSeparator
            Get
                Return Me._ToolStripSeparator6
            End Get
            Set(ByVal WithEventsValue As ToolStripSeparator)
                Me._ToolStripSeparator6 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property ToolStripSeparator7 As ToolStripSeparator
            Get
                Return Me._ToolStripSeparator7
            End Get
            Set(ByVal WithEventsValue As ToolStripSeparator)
                Me._ToolStripSeparator7 = WithEventsValue
            End Set
        End Property

        Friend Overridable Property WaterColorToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._WaterColorToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._WaterColorToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._WaterColorToolStripMenuItem.Click, New EventHandler(AddressOf Me.WaterColorToolStripMenuItem_Click)
                End If
                Me._WaterColorToolStripMenuItem = WithEventsValue
                If (Not Me._WaterColorToolStripMenuItem Is Nothing) Then
                    AddHandler Me._WaterColorToolStripMenuItem.Click, New EventHandler(AddressOf Me.WaterColorToolStripMenuItem_Click)
                End If
            End Set
        End Property

        Friend Overridable Property WaterGoToolStripMenuItem As ToolStripMenuItem
            Get
                Return Me._WaterGoToolStripMenuItem
            End Get
            Set(ByVal WithEventsValue As ToolStripMenuItem)
                If (Not Me._WaterGoToolStripMenuItem Is Nothing) Then
                    RemoveHandler Me._WaterGoToolStripMenuItem.Click, New EventHandler(AddressOf Me.OJGoToolStripMenuItem_Click)
                End If
                Me._WaterGoToolStripMenuItem = WithEventsValue
                If (Not Me._WaterGoToolStripMenuItem Is Nothing) Then
                    AddHandler Me._WaterGoToolStripMenuItem.Click, New EventHandler(AddressOf Me.OJGoToolStripMenuItem_Click)
                End If
            End Set
        End Property


        ' Fields
        <AccessedThroughProperty("AboutWaterPipeToolStripMenuItem")> _
        Private _AboutWaterPipeToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("BackgroundToolStripMenuItem")> _
        Private _BackgroundToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("Box_1_0")> _
        Private _Box_1_0 As PictureBox
        <AccessedThroughProperty("Box_1_1")> _
        Private _Box_1_1 As PictureBox
        <AccessedThroughProperty("Box_1_2")> _
        Private _Box_1_2 As PictureBox
        <AccessedThroughProperty("Box_1_3")> _
        Private _Box_1_3 As PictureBox
        <AccessedThroughProperty("Box_1_4")> _
        Private _Box_1_4 As PictureBox
        <AccessedThroughProperty("Box_1_5")> _
        Private _Box_1_5 As PictureBox
        <AccessedThroughProperty("Box_1_6")> _
        Private _Box_1_6 As PictureBox
        <AccessedThroughProperty("Box_1_7")> _
        Private _Box_1_7 As PictureBox
        <AccessedThroughProperty("Box_1_8")> _
        Private _Box_1_8 As PictureBox
        <AccessedThroughProperty("Box_1_9")> _
        Private _Box_1_9 As PictureBox
        <AccessedThroughProperty("Box_2_0")> _
        Private _Box_2_0 As PictureBox
        <AccessedThroughProperty("Box_2_1")> _
        Private _Box_2_1 As PictureBox
        <AccessedThroughProperty("Box_2_2")> _
        Private _Box_2_2 As PictureBox
        <AccessedThroughProperty("Box_2_3")> _
        Private _Box_2_3 As PictureBox
        <AccessedThroughProperty("Box_2_4")> _
        Private _Box_2_4 As PictureBox
        <AccessedThroughProperty("Box_2_5")> _
        Private _Box_2_5 As PictureBox
        <AccessedThroughProperty("Box_2_6")> _
        Private _Box_2_6 As PictureBox
        <AccessedThroughProperty("Box_2_7")> _
        Private _Box_2_7 As PictureBox
        <AccessedThroughProperty("Box_2_8")> _
        Private _Box_2_8 As PictureBox
        <AccessedThroughProperty("Box_2_9")> _
        Private _Box_2_9 As PictureBox
        <AccessedThroughProperty("Box_3_0")> _
        Private _Box_3_0 As PictureBox
        <AccessedThroughProperty("Box_3_1")> _
        Private _Box_3_1 As PictureBox
        <AccessedThroughProperty("Box_3_2")> _
        Private _Box_3_2 As PictureBox
        <AccessedThroughProperty("Box_3_3")> _
        Private _Box_3_3 As PictureBox
        <AccessedThroughProperty("Box_3_4")> _
        Private _Box_3_4 As PictureBox
        <AccessedThroughProperty("Box_3_5")> _
        Private _Box_3_5 As PictureBox
        <AccessedThroughProperty("Box_3_6")> _
        Private _Box_3_6 As PictureBox
        <AccessedThroughProperty("Box_3_7")> _
        Private _Box_3_7 As PictureBox
        <AccessedThroughProperty("Box_3_8")> _
        Private _Box_3_8 As PictureBox
        <AccessedThroughProperty("Box_3_9")> _
        Private _Box_3_9 As PictureBox
        <AccessedThroughProperty("Box_4_0")> _
        Private _Box_4_0 As PictureBox
        <AccessedThroughProperty("Box_4_1")> _
        Private _Box_4_1 As PictureBox
        <AccessedThroughProperty("Box_4_2")> _
        Private _Box_4_2 As PictureBox
        <AccessedThroughProperty("Box_4_3")> _
        Private _Box_4_3 As PictureBox
        <AccessedThroughProperty("Box_4_4")> _
        Private _Box_4_4 As PictureBox
        <AccessedThroughProperty("Box_4_5")> _
        Private _Box_4_5 As PictureBox
        <AccessedThroughProperty("Box_4_6")> _
        Private _Box_4_6 As PictureBox
        <AccessedThroughProperty("Box_4_7")> _
        Private _Box_4_7 As PictureBox
        <AccessedThroughProperty("Box_4_8")> _
        Private _Box_4_8 As PictureBox
        <AccessedThroughProperty("Box_4_9")> _
        Private _Box_4_9 As PictureBox
        <AccessedThroughProperty("Box_5_0")> _
        Private _Box_5_0 As PictureBox
        <AccessedThroughProperty("Box_5_1")> _
        Private _Box_5_1 As PictureBox
        <AccessedThroughProperty("Box_5_2")> _
        Private _Box_5_2 As PictureBox
        <AccessedThroughProperty("Box_5_3")> _
        Private _Box_5_3 As PictureBox
        <AccessedThroughProperty("Box_5_4")> _
        Private _Box_5_4 As PictureBox
        <AccessedThroughProperty("Box_5_5")> _
        Private _Box_5_5 As PictureBox
        <AccessedThroughProperty("Box_5_6")> _
        Private _Box_5_6 As PictureBox
        <AccessedThroughProperty("Box_5_7")> _
        Private _Box_5_7 As PictureBox
        <AccessedThroughProperty("Box_5_8")> _
        Private _Box_5_8 As PictureBox
        <AccessedThroughProperty("Box_5_9")> _
        Private _Box_5_9 As PictureBox
        <AccessedThroughProperty("Box_6_0")> _
        Private _Box_6_0 As PictureBox
        <AccessedThroughProperty("Box_6_1")> _
        Private _Box_6_1 As PictureBox
        <AccessedThroughProperty("Box_6_2")> _
        Private _Box_6_2 As PictureBox
        <AccessedThroughProperty("Box_6_3")> _
        Private _Box_6_3 As PictureBox
        <AccessedThroughProperty("Box_6_4")> _
        Private _Box_6_4 As PictureBox
        <AccessedThroughProperty("Box_6_5")> _
        Private _Box_6_5 As PictureBox
        <AccessedThroughProperty("Box_6_6")> _
        Private _Box_6_6 As PictureBox
        <AccessedThroughProperty("Box_6_7")> _
        Private _Box_6_7 As PictureBox
        <AccessedThroughProperty("Box_6_8")> _
        Private _Box_6_8 As PictureBox
        <AccessedThroughProperty("Box_6_9")> _
        Private _Box_6_9 As PictureBox
        <AccessedThroughProperty("Box_7_0")> _
        Private _Box_7_0 As PictureBox
        <AccessedThroughProperty("Box_7_1")> _
        Private _Box_7_1 As PictureBox
        <AccessedThroughProperty("Box_7_2")> _
        Private _Box_7_2 As PictureBox
        <AccessedThroughProperty("Box_7_3")> _
        Private _Box_7_3 As PictureBox
        <AccessedThroughProperty("Box_7_4")> _
        Private _Box_7_4 As PictureBox
        <AccessedThroughProperty("Box_7_5")> _
        Private _Box_7_5 As PictureBox
        <AccessedThroughProperty("Box_7_6")> _
        Private _Box_7_6 As PictureBox
        <AccessedThroughProperty("Box_7_7")> _
        Private _Box_7_7 As PictureBox
        <AccessedThroughProperty("Box_7_8")> _
        Private _Box_7_8 As PictureBox
        <AccessedThroughProperty("Box_7_9")> _
        Private _Box_7_9 As PictureBox
        <AccessedThroughProperty("Box_8_0")> _
        Private _Box_8_0 As PictureBox
        <AccessedThroughProperty("Box_8_1")> _
        Private _Box_8_1 As PictureBox
        <AccessedThroughProperty("Box_8_2")> _
        Private _Box_8_2 As PictureBox
        <AccessedThroughProperty("Box_8_3")> _
        Private _Box_8_3 As PictureBox
        <AccessedThroughProperty("Box_8_4")> _
        Private _Box_8_4 As PictureBox
        <AccessedThroughProperty("Box_8_5")> _
        Private _Box_8_5 As PictureBox
        <AccessedThroughProperty("Box_8_6")> _
        Private _Box_8_6 As PictureBox
        <AccessedThroughProperty("Box_8_7")> _
        Private _Box_8_7 As PictureBox
        <AccessedThroughProperty("Box_8_8")> _
        Private _Box_8_8 As PictureBox
        <AccessedThroughProperty("Box_8_9")> _
        Private _Box_8_9 As PictureBox
        <AccessedThroughProperty("Box_9_0")> _
        Private _Box_9_0 As PictureBox
        <AccessedThroughProperty("Box_9_1")> _
        Private _Box_9_1 As PictureBox
        <AccessedThroughProperty("Box_9_2")> _
        Private _Box_9_2 As PictureBox
        <AccessedThroughProperty("Box_9_3")> _
        Private _Box_9_3 As PictureBox
        <AccessedThroughProperty("Box_9_4")> _
        Private _Box_9_4 As PictureBox
        <AccessedThroughProperty("Box_9_5")> _
        Private _Box_9_5 As PictureBox
        <AccessedThroughProperty("Box_9_6")> _
        Private _Box_9_6 As PictureBox
        <AccessedThroughProperty("Box_9_7")> _
        Private _Box_9_7 As PictureBox
        <AccessedThroughProperty("Box_9_8")> _
        Private _Box_9_8 As PictureBox
        <AccessedThroughProperty("Box_9_9")> _
        Private _Box_9_9 As PictureBox
        <AccessedThroughProperty("ChineseToolStripMenuItem")> _
        Private _ChineseToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("ColorToolStripMenuItem")> _
        Private _ColorToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("EnglishToolStripMenuItem")> _
        Private _EnglishToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("Entrance1ToolStripMenuItem")> _
        Private _Entrance1ToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("Entrance2ToolStripMenuItem")> _
        Private _Entrance2ToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("Entrance3ToolStripMenuItem")> _
        Private _Entrance3ToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("Entrance4ToolStripMenuItem")> _
        Private _Entrance4ToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("Entrance5ToolStripMenuItem")> _
        Private _Entrance5ToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("Entrance6ToolStripMenuItem")> _
        Private _Entrance6ToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("Entrance7ToolStripMenuItem")> _
        Private _Entrance7ToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("Entrance8ToolStripMenuItem")> _
        Private _Entrance8ToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("Entrance9ToolStripMenuItem")> _
        Private _Entrance9ToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("EntranceQuantityToolStripMenuItem")> _
        Private _EntranceQuantityToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("ExitToolStripMenuItem")> _
        Private _ExitToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("FontColorToolStripMenuItem")> _
        Private _FontColorToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("GameToolStripMenuItem")> _
        Private _GameToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("GridColorToolStripMenuItem")> _
        Private _GridColorToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("GridToolStripMenuItem")> _
        Private _GridToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("HelpToolStripMenuItem")> _
        Private _HelpToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("HelpTopicsToolStripMenuItem")> _
        Private _HelpTopicsToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("HighScoresToolStripMenuItem")> _
        Private _HighScoresToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("ImageToolStripMenuItem")> _
        Private _ImageToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("InWater")> _
        Private _InWater As Button
        <AccessedThroughProperty("LanguageToolStripMenuItem")> _
        Private _LanguageToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("MainPanel")> _
        Private _MainPanel As Panel
        <AccessedThroughProperty("MediaPlayer")> _
        Private _MediaPlayer As WindowsMediaPlayer
        <AccessedThroughProperty("MenuStrip")> _
        Private _MenuStrip As MenuStrip
        <AccessedThroughProperty("MusicToolStripMenuItem")> _
        Private _MusicToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("NewGameToolStripMenuItem")> _
        Private _NewGameToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("Next_1")> _
        Private _Next_1 As PictureBox
        <AccessedThroughProperty("Next_2")> _
        Private _Next_2 As PictureBox
        <AccessedThroughProperty("Next_3")> _
        Private _Next_3 As PictureBox
        <AccessedThroughProperty("Next_4")> _
        Private _Next_4 As PictureBox
        <AccessedThroughProperty("Next_5")> _
        Private _Next_5 As PictureBox
        <AccessedThroughProperty("Next_6")> _
        Private _Next_6 As PictureBox
        <AccessedThroughProperty("Next_7")> _
        Private _Next_7 As PictureBox
        <AccessedThroughProperty("Next_8")> _
        Private _Next_8 As PictureBox
        <AccessedThroughProperty("Next_9")> _
        Private _Next_9 As PictureBox
        <AccessedThroughProperty("NextLabel")> _
        Private _NextLabel As Label
        <AccessedThroughProperty("OptionsToolStripMenuItem")> _
        Private _OptionsToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("OutBondPictureBox")> _
        Private _OutBondPictureBox As PictureBox
        <AccessedThroughProperty("PanelBackgroundImageFileDialog")> _
        Private _PanelBackgroundImageFileDialog As OpenFileDialog
        <AccessedThroughProperty("PanelColorDialog")> _
        Private _PanelColorDialog As ColorDialog
        <AccessedThroughProperty("ScoreBar")> _
        Private _ScoreBar As ProgressBar
        <AccessedThroughProperty("ScoreLabel")> _
        Private _ScoreLabel As Label
        <AccessedThroughProperty("ScoreNumberLabel")> _
        Private _ScoreNumberLabel As Label
        <AccessedThroughProperty("SelectColorToolStripMenuItem")> _
        Private _SelectColorToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("SelectImageToolStripMenuItem")> _
        Private _SelectImageToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("SelectMusicDialog")> _
        Private _SelectMusicDialog As OpenFileDialog
        <AccessedThroughProperty("SelectMusicToolStripMenuItem")> _
        Private _SelectMusicToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("ShowGridToolStripMenuItem")> _
        Private _ShowGridToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("SoundsToolStripMenuItem")> _
        Private _SoundsToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("ToolStripSeparator1")> _
        Private _ToolStripSeparator1 As ToolStripSeparator
        <AccessedThroughProperty("ToolStripSeparator2")> _
        Private _ToolStripSeparator2 As ToolStripSeparator
        <AccessedThroughProperty("ToolStripSeparator3")> _
        Private _ToolStripSeparator3 As ToolStripSeparator
        <AccessedThroughProperty("ToolStripSeparator4")> _
        Private _ToolStripSeparator4 As ToolStripSeparator
        <AccessedThroughProperty("ToolStripSeparator5")> _
        Private _ToolStripSeparator5 As ToolStripSeparator
        <AccessedThroughProperty("ToolStripSeparator6")> _
        Private _ToolStripSeparator6 As ToolStripSeparator
        <AccessedThroughProperty("ToolStripSeparator7")> _
        Private _ToolStripSeparator7 As ToolStripSeparator
        <AccessedThroughProperty("WaterColorToolStripMenuItem")> _
        Private _WaterColorToolStripMenuItem As ToolStripMenuItem
        <AccessedThroughProperty("WaterGoToolStripMenuItem")> _
        Private _WaterGoToolStripMenuItem As ToolStripMenuItem
        Private AnimoThreadCollection As Collection
        Private AppName As String
        Private AppPath As String
        Private AppSounds_Breaking As SoundPlayer
        Private AppSounds_NewGame As SoundPlayer
        Private AppSounds_PlaceImage As SoundPlayer
        Private AppSounds_WaterFlow As SoundPlayer
        Private AppSounds_WaterOverflow As SoundPlayer
        Private BoxCollection As Collection
        Private components As IContainer
        Private Entrance As Integer
        Private FlowBoxCollection As Collection
        Private FontColor As Color
        Private GridColor As Color
        Private HighScore As Integer()
        Private HighScoreName As String()
        Private ImageCollection As Collection
        Private LastInputName As String
        Private Lave As Integer
        Private MediaError As String
        Private NewGame As String
        Private NextBoxCollection As Collection
        Private NowRank As Integer
        Private PanelBackColor As Color
        Private PanelBackgroundImage As Image
        Private PIPENO As Integer
        Private Playlist As IWMPPlaylist
        Private ResMana As ResourceManager
        Private Score As Integer
        Private SetBoxCollection As Collection
        Private Somebody As String
        Private TempImage As Image
        Private WaterColor As Color
        Private WATERSPEED As Integer

        ' Nested Types
        Public Delegate Sub SetEntranceMenuItemCallback(ByVal sta As Boolean)

        Public Delegate Sub SetInWaterCallback(ByVal sta As Boolean)

        Public Delegate Sub SetInWaterTextCallback(ByVal [text] As String)

        Public Delegate Sub SetMainBoxCallback(ByVal sta As Boolean)

        Public Delegate Sub SetNewGameMenuItemCallback(ByVal sta As Boolean)

        Public Delegate Sub SetOJGoMenuItemCallback(ByVal sta As Boolean)

        Public Delegate Sub SetScoreBarValueCallback(ByVal value As Double)

        Public Delegate Sub SetScoreNumberLabelTextCallback(ByVal [text] As String)
    End Class
End Namespace

