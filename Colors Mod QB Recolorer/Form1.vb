Imports System.IO
Imports System.Text

Public Class Form1
    Public basicColors() As dye_color = {New dye_color("red", "Red", "red", "s0", 0, 0),
                                         New dye_color("orange", "Orange", "orange", "s30", 0, 0),
                                         New dye_color("yellow", "Yellow", "yellow", "s60", 0, 0),
                                         New dye_color("green", "Green", "green", "s125", 0, 0),
                                         New dye_color("blue", "Blue", "blue", "s210", 0, 0),
                                         New dye_color("purple", "Purple", "purple", "s275", 0, 0),
                                         New dye_color("undyed", "Undyed", "", "s40", "s30", 0)
                                        }
    Public paleColors() As dye_color = {New dye_color("lt_red", "Light Red", "red", "s0", -85, 100),
                                         New dye_color("lt_orange", "Light Orange", "orange", "s30", -85, 100),
                                         New dye_color("lt_yellow", "Light Yellow", "yellow", "s60", -85, 100),
                                         New dye_color("lt_green", "Light Green", "green", "s125", -85, 100),
                                         New dye_color("lt_blue", "Light Blue", "blue", "s210", -85, 100),
                                         New dye_color("lt_purple", "Light Purple", "purple", "s275", -85, 100)
                                        }
    Public richColors() As dye_color = {New dye_color("dk_red", "Dark Red", "red", "s0", 125, -65),
                                         New dye_color("dk_orange", "Dark Orange", "orange", "s30", 125, -65),
                                         New dye_color("dk_yellow", "Dark Yellow", "yellow", "s60", 125, -65),
                                         New dye_color("dk_green", "Dark Green", "green", "s125", 125, -65),
                                         New dye_color("dk_blue", "Dark Blue", "blue", "s210", 125, -65),
                                         New dye_color("dk_purple", "Dark Purple", "purple", "s275", 125, -65)
                                        }
    'more colors here

    Public currentColor As dye_color = basicColors(0)

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles BTN_start.Click
        If My.Computer.FileSystem.DirectoryExists(TB_input_folder.Text) Then
            BTN_start.Text = "Input Folder does not exist, please try again."

            BTN_start.Text = "This may take a while! Please be patient..."
            BTN_start.Enabled = False
            'Dim cont As Integer = MsgBox("This progess can take a while, during which time the window may become unresponsive. Continue?", vbYesNo, "Continue?")
            'better message box needed here with option to not show it again.
            Dim num_of_files = My.Computer.FileSystem.GetFiles(TB_input_folder.Text).Count

            Dim JSON_files() As String = {}
            Dim JSON_recipe As String = ""
            Dim PNG_File As String = ""
            Dim PNG_NEG_File As String = ""
            Dim QB_NEG_File As String = ""
            Dim QB_iconic_NEG_File As String = ""
            Dim QB_File As String = ""
            Dim QB_iconic_File As String = ""

            Dim QB As New QBModel
            Dim QB_NEG As New QBModel
            Dim QB_iconic As New QBModel
            Dim QB_iconic_NEG As New QBModel
            Dim PNG As Bitmap
            Dim PNG_NEG As Bitmap

            Dim Recipe_List = ""
            Dim Manifest_Aliases = ""

            Dim colors_to_use As New Collection

            ProgressBar1.Value = 0
            ProgressBar2.Value = 0
            ProgressBar3.Value = 0

            Console.WriteLine("Starting...")
            If TB_input_folder.Text = "" Then   'if none of the required fields are empty then get files
                BTN_start.Text = "Input Folder text field is empty, please try again."
            ElseIf TB_iname.Text = "" Then
                BTN_start.Text = "item_name text field is empty, please try again."
            ElseIf TB_mname.Text = "" Then
                BTN_start.Text = "mod_name text field is empty, please try again."
            ElseIf TB_hriname.Text = "" Then
                BTN_start.Text = "Human Readable Name text field is empty, please try again."
            ElseIf TB_ifolder.Text = "" Then
                BTN_start.Text = "Folder location in mod text field is empty, please try again."
            Else    'TODO need to add recipe field checks
                For Each foundFile As String In My.Computer.FileSystem.GetFiles(TB_input_folder.Text)
                    'Console.WriteLine(foundFile)
                    If foundFile.EndsWith(".png") Then
                        If foundFile.EndsWith("NEG.png") Then
                            PNG_NEG_File = foundFile
                            'Console.WriteLine("NEG.png found")
                        Else
                            PNG_File = foundFile
                            'Console.WriteLine(".png found")
                        End If
                    ElseIf foundFile.EndsWith(".qb") Then
                        If foundFile.EndsWith("NEG.qb") Then
                            If foundFile.EndsWith("iconic_NEG.qb") Then
                                QB_iconic_NEG_File = foundFile
                                'Console.WriteLine("iconic_NEG.qb found")
                            Else
                                QB_NEG_File = foundFile
                                'Console.WriteLine("NEG.qb found")
                            End If
                        Else
                            If foundFile.EndsWith("iconic.qb") Then
                                QB_iconic_File = foundFile
                                'Console.WriteLine("iconic.qb found")
                            Else
                                QB_File = foundFile
                                'Console.WriteLine(".qb found")
                            End If
                        End If
                    ElseIf foundFile.EndsWith(".json") Then
                        If foundFile.EndsWith("recipe.json") Then
                            JSON_recipe = foundFile
                            'Console.WriteLine("recipe.json found")
                        Else
                            'JSON_files.Add(foundFile)
                            ReDim Preserve JSON_files(JSON_files.Length)
                            JSON_files(JSON_files.Length - 1) = foundFile
                            'Console.WriteLine(".json found")
                        End If
                    Else
                        Console.WriteLine("unrecognized file found ... skipping")
                        num_of_files = num_of_files - 1
                    End If

                    Dim value = ProgressBar1.Value + (100 / num_of_files)
                    If value > 100 Then value = 100
                    ProgressBar1.Value = value
                Next
                ProgressBar1.Value = 100

                'build color selection
                For i As Integer = 0 To CheckedListBox1.Items.Count - 1
                    If CheckedListBox1.GetItemCheckState(i) = CheckState.Checked Then
                        colors_to_use.Add(basicColors(i))
                        'Console.WriteLine(basicColors(i).name + " is checked")
                    End If
                Next
                For i As Integer = 0 To CheckedListBox2.Items.Count - 1
                    If CheckedListBox2.GetItemCheckState(i) = CheckState.Checked Then
                        colors_to_use.Add(paleColors(i))
                        'Console.WriteLine(paleColors(i).name + " is checked")
                    End If
                Next
                For i As Integer = 0 To CheckedListBox3.Items.Count - 1
                    If CheckedListBox3.GetItemCheckState(i) = CheckState.Checked Then
                        colors_to_use.Add(richColors(i))
                        'Console.WriteLine(richColors(i).name + " is checked")
                    End If
                Next
                If CheckBox4.Checked = True Then
                    Dim lines() As String
                    lines = TB_CC.Lines()
                    For i As Integer = 0 To lines.Length - 1
                        If lines(i) <> "" Then
                            colors_to_use.Add(PARSE.COLOR(lines(i)))
                        End If
                    Next
                End If
                'more colors here



                'import qb
                If QB_File <> "" And CB_o_skip_qb.Checked = False Then
                    QB = QBFile.open(QB_File)
                    'Console.WriteLine("QB_File imported as QB")
                    If QB_NEG_File <> "" And CB_o_skip_qb.Checked = False Then
                        QB_NEG = QBFile.open(QB_NEG_File)
                        'Console.WriteLine("QB_NEG_File imported as QB_NEG")
                    End If
                End If

                If QB_iconic_File <> "" And CB_o_skip_qb.Checked = False Then
                    QB_iconic = QBFile.open(QB_iconic_File)
                    'Console.WriteLine("QB_iconic_File imported as QB_iconic")
                    If QB_iconic_NEG_File <> "" And CB_o_skip_qb.Checked = False Then
                        QB_iconic_NEG = QBFile.open(QB_iconic_NEG_File)
                        'Console.WriteLine("QB_iconic_NEG_File imported as QB_iconic_NEG")
                    End If
                End If

                'import png
                PNG = New Bitmap(PNG_File)
                PNG_NEG = New Bitmap(PNG_NEG_File)

                'make recipe folder
                If CB_recipe_generate.Checked Then
                    Dim recipe_folder As String = PARSE.TEXT(TB_recipe_file.Text)
                    recipe_folder = recipe_folder.Substring(0, recipe_folder.LastIndexOf("/"))
                    My.Computer.FileSystem.CreateDirectory(TB_input_folder.Text + "\output" + recipe_folder)
                End If

                '~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

                'MAIN recolor/parse everything!
                For i As Integer = 1 To colors_to_use.Count
                    ProgBar_Rand()
                    Dim c_QB As QBModel
                    Dim c_PNG As Bitmap
                    'set current color
                    currentColor = colors_to_use(i)
                    'Console.WriteLine(currentColor.ToString)

                    'set current folder and file name
                    Dim name = TB_iname.Text + "_" + currentColor.name
                    Dim folder = TB_input_folder.Text + "\output\" + PARSE.TEXT(TB_ifolder.Text) '+ name

                    'write current folder
                    If CB_o_skip_qb.Checked = False Or CB_o_skip_png.Checked = False Or CB_o_skip_json.Checked = False Then
                        My.Computer.FileSystem.CreateDirectory(folder)
                    End If

                    'reimport qb and iconic qb
                    If QB_File <> "" And CB_o_skip_qb.Checked = False Then
                        QB = QBFile.open(QB_File)
                        'Console.WriteLine("QB_File imported as QB")
                    End If
                    If QB_iconic_File <> "" And CB_o_skip_qb.Checked = False Then
                        QB_iconic = QBFile.open(QB_iconic_File)
                        'Console.WriteLine("QB_iconic_File imported as QB_iconic")
                    End If

                    'qb
                    If CB_o_skip_qb.Checked = False Then
                        c_QB = color.recolorQB(QB, QB_NEG, currentColor)
                        QBFile.save(c_QB, folder + "\" + name + ".qb")
                    End If
                    'iconic qb
                    If CB_o_skip_iconic_qb.Checked = False Then
                        c_QB = color.recolorQB(QB_iconic, QB_iconic_NEG, currentColor)
                        QBFile.save(c_QB, folder + "\" + name + "_iconic.qb")
                    End If

                    'png
                    If CB_o_skip_png.Checked = False Then
                        c_PNG = color.recolorPNG(PNG, PNG_NEG, currentColor)
                        c_PNG.Save(folder + "\" + name + ".png")
                    End If

                    'json
                    If CB_o_skip_json.Checked = False Then
                        For j As Integer = 0 To JSON_files.Count - 1

                            'get ending
                            Dim ending As String
                            ending = JSON_files(j).Substring(JSON_files(j).LastIndexOf("\") + 1)
                            If ending.ToLower = "base.json" Then
                                ending = ".json"
                            Else
                                ending = "_" + ending
                            End If

                            PARSE.JSON(JSON_files(j), folder + "\" + name + ending)
                        Next
                    End If

                    'recipe
                    If CB_recipe_generate.Checked = True Then
                        If CB_recipe_in_folder.Checked Then
                            If JSON_recipe <> "" Then
                                PARSE.JSON(JSON_recipe, TB_input_folder.Text + "\output" + PARSE.TEXT(TB_recipe_file))
                            End If
                        Else
                            'TODO construct recipe.json and save
                            Dim lines() As String
                            Dim recipe_str = "{" & vbCrLf & _
                                vbTab & """type"":""recipe""," & vbCrLf & vbCrLf & _
                                vbTab & """work_units""  : " & NUD_work.Value.ToString & "," & vbCrLf & _
                                vbTab & """recipe_name"" : """ & PARSE.TEXT(TB_recipe_name) & """," & vbCrLf & _
                                vbTab & """description"" : """ & PARSE.TEXT(TB_recipe_desc) & """," & vbCrLf & _
                                vbTab & """flavor""      : """ & PARSE.TEXT(TB_recipe_flavor) & """," & vbCrLf & _
                                vbTab & """portrait""    : """ & PARSE.TEXT(TB_recipe_port) & """," & vbCrLf & _
                                vbTab & """level_requirement"" : " & NUD_recipe_lvl.Value.ToString & "," & vbCrLf & vbCrLf & _
                                vbTab & """ingredients"": [" & vbCrLf

                            lines = TB_recipe_ingre.Lines()
                            For ii As Integer = 0 To lines.Length - 1
                                If lines(ii) <> "" Then
                                    recipe_str = recipe_str & PARSE.RECIP_INGR(lines(ii))
                                    If ii <> lines.Length - 1 Then
                                        recipe_str = recipe_str & "," & vbCrLf
                                    Else
                                        recipe_str = recipe_str & vbCrLf
                                    End If
                                End If
                            Next

                            recipe_str = recipe_str & vbTab & "]," & vbCrLf & _
                                vbTab & """produces"": [" & vbCrLf

                            lines = TB_recipe_prod.Lines()
                            For ii As Integer = 0 To lines.Length - 1
                                If lines(ii) <> "" Then
                                    Dim temp() As String = lines(ii).Split(",")
                                    For iii As Integer = 0 To Integer.Parse(temp(0)) - 1
                                        recipe_str = recipe_str & vbTab & vbTab & "{" & vbCrLf & vbTab & vbTab & vbTab & """ item"" : """ & PARSE.TEXT(temp(1)) & """" & vbNewLine & vbTab & vbTab & "}"
                                        If lines.Length = 1 Then
                                            recipe_str = recipe_str & vbCrLf
                                        Else
                                            If ii <> lines.Length - 1 Then
                                                recipe_str = recipe_str & "," & vbCrLf
                                            Else
                                                If iii <> temp.Length - 1 Then
                                                    recipe_str = recipe_str & "," & vbCrLf
                                                Else
                                                    recipe_str = recipe_str & vbCrLf
                                                End If
                                            End If
                                        End If
                                    Next
                                End If
                            Next

                            recipe_str = recipe_str & vbTab & "]" & vbCrLf & "}"

                            'save
                            'Dim fileWriter = My.Computer.FileSystem.OpenTextFileWriter(TB_input_folder.Text + "\output" + PARSE.TEXT(TB_recipe_file), False, Encoding.ASCII)
                            'fileWriter.Write(recipe_str)
                            'fileWriter.Dispose()
                            My.Computer.FileSystem.WriteAllText(TB_input_folder.Text + "\output" + PARSE.TEXT(TB_recipe_file), recipe_str, False, Encoding.ASCII)

                        End If
                    End If

                    'recipelist
                    If CB_recipe_gen_list.Checked Then
                        If Recipe_List = "" Then
                            Recipe_List = Recipe_List + vbTab + vbTab + vbTab + PARSE.TEXT(TB_recipe_gen_list.Text)
                        Else
                            Recipe_List = Recipe_List + "," & vbCrLf & vbTab + vbTab + vbTab + PARSE.TEXT(TB_recipe_gen_list.Text)
                        End If
                    End If

                    'manifest aliases
                    If CB_o_gen_alias.Checked Then
                        If Manifest_Aliases = "" Then
                            Manifest_Aliases = Manifest_Aliases + vbTab + vbTab + PARSE.TEXT(TB_option_alias.Text)
                        Else
                            Manifest_Aliases = Manifest_Aliases + "," & vbCrLf & vbTab + vbTab + PARSE.TEXT(TB_option_alias.Text)
                        End If
                    End If

                Next

            End If

            '~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            'write recipe list
            If CB_recipe_gen_list.Checked Then
                Dim fileWriter = My.Computer.FileSystem.OpenTextFileWriter(TB_input_folder.Text + "\output\recipes_list.txt", False)
                fileWriter.Write(Recipe_List)
                fileWriter.Dispose()
            End If

            'write manifest aliases
            If CB_o_gen_alias.Checked Then
                Dim fileWriter = My.Computer.FileSystem.OpenTextFileWriter(TB_input_folder.Text + "\output\manifest_aliases.txt", False)
                fileWriter.Write(Manifest_Aliases)
                fileWriter.Dispose()
            End If

            'PNG.Dispose()
            'PNG_NEG.Dispose()

            ProgBar_100()
            BTN_start.Text = "Finished! - Click here to do it again!"
            BTN_start.Enabled = True
        Else
            BTN_start.Text = "Input Folder does not exists, please try again."
        End If
    End Sub

    Private Sub CheckBox4_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox4.CheckedChanged
        If (CheckBox4.Checked) Then
            GroupBox1.Enabled = True
        Else
            GroupBox1.Enabled = False
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        WebBrowser1.Navigate("file:///" & IO.Path.GetFullPath(".\Help\Help.html"))
        TB_input_folder.Text = IO.Path.GetFullPath(".\Example")
        CheckBox9.CheckState = CheckState.Checked
        ProgBar_Rand()
    End Sub

    Private Sub CheckBox9_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox9.CheckedChanged
        If CheckBox9.Checked Then
            For index As Integer = 0 To (CheckedListBox1.Items.Count - 1)
                CheckedListBox1.SetItemChecked(index, True)
            Next
        Else
            For index As Integer = 0 To (CheckedListBox1.Items.Count - 1)
                CheckedListBox1.SetItemChecked(index, False)
            Next
        End If
    End Sub

    Private Sub CheckBox10_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox10.CheckedChanged
        If CheckBox10.Checked Then
            For index As Integer = 0 To (CheckedListBox2.Items.Count - 1)
                CheckedListBox2.SetItemChecked(index, True)
            Next
        Else
            For index As Integer = 0 To (CheckedListBox2.Items.Count - 1)
                CheckedListBox2.SetItemChecked(index, False)
            Next
        End If
    End Sub

    Private Sub CheckBox11_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox11.CheckedChanged
        If CheckBox11.Checked Then
            For index As Integer = 0 To (CheckedListBox3.Items.Count - 1)
                CheckedListBox3.SetItemChecked(index, True)
            Next
        Else
            For index As Integer = 0 To (CheckedListBox3.Items.Count - 1)
                CheckedListBox3.SetItemChecked(index, False)
            Next
        End If
    End Sub

    Private Sub CheckBox12_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox12.CheckedChanged
        If CheckBox12.Checked Then
            For index As Integer = 0 To (CheckedListBox4.Items.Count - 1)
                CheckedListBox4.SetItemChecked(index, True)
            Next
        Else
            For index As Integer = 0 To (CheckedListBox4.Items.Count - 1)
                CheckedListBox4.SetItemChecked(index, False)
            Next
        End If
    End Sub

    Private Sub CheckBox13_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox13.CheckedChanged
        If CheckBox13.Checked Then
            For index As Integer = 0 To (CheckedListBox5.Items.Count - 1)
                CheckedListBox5.SetItemChecked(index, True)
            Next
        Else
            For index As Integer = 0 To (CheckedListBox5.Items.Count - 1)
                CheckedListBox5.SetItemChecked(index, False)
            Next
        End If
    End Sub

    Private Sub CB_recipe_generate_CheckedChanged(sender As Object, e As EventArgs) Handles CB_recipe_generate.CheckedChanged
        If CB_recipe_generate.Checked Then
            TB_recipe_file.Enabled = True
            NUD_work.Enabled = True
            NUD_recipe_lvl.Enabled = True
            TB_recipe_crafter.Enabled = True
            TB_recipe_desc.Enabled = True
            TB_recipe_flavor.Enabled = True
            TB_recipe_ingre.Enabled = True
            TB_recipe_port.Enabled = True
            TB_recipe_prod.Enabled = True
            TB_recipe_name.Enabled = True
            CB_recipe_in_folder.Enabled = True
        Else

            NUD_work.Enabled = False
            NUD_recipe_lvl.Enabled = False
            CB_recipe_in_folder.Enabled = False
            TB_recipe_crafter.Enabled = False
            TB_recipe_desc.Enabled = False
            TB_recipe_flavor.Enabled = False
            TB_recipe_ingre.Enabled = False
            TB_recipe_port.Enabled = False
            TB_recipe_prod.Enabled = False
            TB_recipe_port.Enabled = False
            TB_recipe_name.Enabled = False

            If CB_recipe_gen_list.Checked Then
                TB_recipe_file.Enabled = True
            Else
                TB_recipe_file.Enabled = False
            End If
        End If
    End Sub

    Private Sub CB_recipe_in_folder_CheckedChanged(sender As Object, e As EventArgs) Handles CB_recipe_in_folder.CheckedChanged
        If CB_recipe_in_folder.Checked = False Then
            NUD_work.Enabled = True
            NUD_recipe_lvl.Enabled = True
            TB_recipe_desc.Enabled = True
            TB_recipe_flavor.Enabled = True
            TB_recipe_ingre.Enabled = True
            TB_recipe_port.Enabled = True
            TB_recipe_prod.Enabled = True
            TB_recipe_name.Enabled = True
        Else
            NUD_work.Enabled = False
            NUD_recipe_lvl.Enabled = False
            TB_recipe_desc.Enabled = False
            TB_recipe_flavor.Enabled = False
            TB_recipe_ingre.Enabled = False
            TB_recipe_port.Enabled = False
            TB_recipe_prod.Enabled = False
            TB_recipe_port.Enabled = False
            TB_recipe_name.Enabled = False
        End If
    End Sub

    Private Sub CB_recipe_gen_list_CheckedChanged(sender As Object, e As EventArgs) Handles CB_recipe_gen_list.CheckedChanged
        If CB_recipe_gen_list.Checked Then
            TB_recipe_gen_list.Enabled = True
            TB_recipe_file.Enabled = True
        Else
            TB_recipe_gen_list.Enabled = False

            If CB_recipe_generate.Checked Then
                TB_recipe_file.Enabled = True
            Else
                TB_recipe_file.Enabled = False
            End If
        End If
    End Sub

    Private Sub CB_option_alias_CheckedChanged(sender As Object, e As EventArgs) Handles CB_o_gen_alias.CheckedChanged
        If CB_o_gen_alias.Checked Then
            TB_option_alias.Enabled = True
        Else
            TB_option_alias.Enabled = False
        End If
    End Sub

    Private Sub CB_option_qb_CheckedChanged(sender As Object, e As EventArgs) Handles CB_o_skip_qb.CheckedChanged
        If CB_o_skip_qb.Checked Then
            CB_o_skip_iconic_qb.CheckState = CheckState.Checked
        Else
            CB_o_skip_iconic_qb.CheckState = CheckState.Unchecked
        End If
    End Sub

    Private Sub BTN_open_Click(sender As Object, e As EventArgs) Handles BTN_open.Click
        FolderBrowserDialog1.ShowDialog()
        TB_input_folder.Text = FolderBrowserDialog1.SelectedPath
    End Sub

    Private Sub ProgBar_Rand()
        Dim rand As New Random
        ProgressBar1.Value = (rand.Next(0, 100))
        ProgressBar2.Value = (rand.Next(0, 100))
        ProgressBar3.Value = (rand.Next(0, 100))
    End Sub
    Private Sub ProgBar_100()
        Dim rand As New Random
        ProgressBar1.Value = (100)
        ProgressBar2.Value = (100)
        ProgressBar3.Value = (100)
    End Sub

    Private Sub BTN_clear_Click(sender As Object, e As EventArgs) Handles BTN_clear.Click
        TB_hriname.Text = ""
        TB_ifolder.Text = ""
        TB_iname.Text = ""
        TB_input_folder.Text = ""
        TB_mname.Text = ""

        TB_option_alias.Text = ""
        NUD_o_R.Value = 0
        NUD_o_G.Value = 0
        NUD_o_B.Value = 0

        TB_recipe_crafter.Text = ""
        TB_recipe_crafter.Text = ""
        TB_recipe_desc.Text = ""
        TB_recipe_file.Text = ""
        TB_recipe_flavor.Text = ""
        TB_recipe_gen_list.Text = ""
        TB_recipe_ingre.Text = ""
        TB_recipe_name.Text = ""
        TB_recipe_port.Text = ""
        TB_recipe_prod.Text = ""
        NUD_work.Value = 1

        TB_CC.Text = ""
        TB_CC_name.Text = ""
        TB_CC_HRCN.Text = ""
        TB_CC_dye.Text = ""
        NUD_CC_H.Value = 0
        NUD_CC_S.Value = 0
        NUD_CC_V.Value = 0

        WebBrowser1.Navigate("file:///" & IO.Path.GetFullPath(".\Help\Help.html"))
    End Sub

    Private Sub BTN_open_output_Click(sender As Object, e As EventArgs) Handles BTN_open_output.Click
        If My.Computer.FileSystem.DirectoryExists(TB_input_folder.Text) Then
            Process.Start("explorer.exe", TB_input_folder.Text)
            BTN_open_output.Text = "Open Output Folder"
        Else
            BTN_open_output.Text = "Directory does not exist."
        End If
    End Sub

    Private Sub TB_input_folder_TextChanged(sender As Object, e As EventArgs) Handles TB_input_folder.TextChanged
        If TB_input_folder.Text = "" Then
            BTN_open_output.Enabled = False
        Else
            BTN_open_output.Enabled = True
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CB_o_CNEG.CheckedChanged
        If CB_o_CNEG.Checked Then
            NUD_o_R.Enabled = True
            NUD_o_G.Enabled = True
            NUD_o_B.Enabled = True
            Label17.Enabled = True
        Else
            NUD_o_R.Enabled = False
            NUD_o_G.Enabled = False
            NUD_o_B.Enabled = False
            Label17.Enabled = False
        End If
    End Sub

    Private Sub BTN_AddCC_Click(sender As Object, e As EventArgs) Handles BTN_AddCC.Click
        Dim NewCC As String = ""

        If TB_CC.Text <> "" Then
            NewCC = vbNewLine
        End If

        NewCC = NewCC & TB_CC_name.Text & "," & TB_CC_HRCN.Text & "," & TB_CC_dye.Text & ","

        If RB_CC_H_Set.Checked Then
            NewCC = NewCC & "s"
        End If
        If RB_CC_H_Per.Checked Then
            NewCC = NewCC & "p"
        End If
        NewCC = NewCC & NUD_CC_H.Value & ","

        If RB_CC_S_Set.Checked Then
            NewCC = NewCC & "s"
        End If
        If RB_CC_S_Per.Checked Then
            NewCC = NewCC & "p"
        End If
        NewCC = NewCC & NUD_CC_S.Value & ","

        If RB_CC_V_Set.Checked Then
            NewCC = NewCC & "s"
        End If
        If RB_CC_V_Per.Checked Then
            NewCC = NewCC & "p"
        End If
        NewCC = NewCC & NUD_CC_V.Value

        TB_CC.Text = TB_CC.Text & NewCC
    End Sub
End Class

Public Class QBFile
    Public Shared Function open(ByVal inFile As String)
        Dim QB_M As New QBModel
        'Console.WriteLine("Reading QB: " + inFile)

        Using reader As New BinaryReader(File.Open(inFile, FileMode.Open))

            QB_M.setVersion(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte())
            'Console.WriteLine("QB Version: " + QB_M.getVersionAsString())

            QB_M.setColorFormat(reader.ReadInt32)
            'Console.WriteLine("QB Color F: " + QB_M.getColorFormat.ToString)

            QB_M.setZOrient(reader.ReadInt32)
            'Console.WriteLine("QB Z Orien: " + QB_M.getZOrient.ToString)

            QB_M.setCompress(reader.ReadInt32)
            'Console.WriteLine("QB Compres: " + QB_M.getCompress.ToString)

            QB_M.setVisMask(reader.ReadInt32)
            'Console.WriteLine("QB VisMask: " + QB_M.getVisMask.ToString)

            QB_M.setMatrixCount(reader.ReadInt32 - 1)
            'Console.WriteLine("QB #Matrix: " + QB_M.getMatrixCount.ToString)

            'read matrices 
            For i As Integer = 1 To QB_M.getMatrixCount
                'Dim name_length As Byte = reader.ReadByte
                Dim j As Integer = i - 1
                QB_M.matrix(j).setName(reader.ReadString)
                'Console.WriteLine("Matrix Name: " + QB_M.matrix(j).getName)

                QB_M.matrix(j).setMatrixSize(reader.ReadInt32 - 1, reader.ReadInt32 - 1, reader.ReadInt32 - 1)
                'Console.WriteLine("Matrix Size: " + QB_M.matrix(j).getSizeX.ToString + "," + QB_M.matrix(j).getSizeY.ToString + "," + QB_M.matrix(j).getSizeZ.ToString)

                QB_M.matrix(j).setPos(reader.ReadInt32, reader.ReadInt32, reader.ReadInt32)

                If QB_M.getCompress = 0 Then    'if uncompressed
                    For z As Integer = 0 To QB_M.matrix(j).data.GetLength(2) - 1
                        For y As Integer = 0 To QB_M.matrix(j).data.GetLength(1) - 1
                            For x As Integer = 0 To QB_M.matrix(j).data.GetLength(0) - 1
                                QB_M.matrix(j).data(x, y, z) = reader.ReadInt32
                                'Console.WriteLine(x.ToString + "," + y.ToString + "," + z.ToString + ": " + QB_M.matrix(j).data(x, y, z).ToString)
                            Next
                        Next
                    Next
                Else    'if compressed
                    Console.WriteLine("ERROR: Compressed QB, can not process!")
                End If
            Next

        End Using

        Return QB_M
    End Function
    Public Shared Sub save(ByVal out_QB As QBModel, ByVal outFile As String)
        'Console.WriteLine("QB out: " + outFile)
        Using writer As BinaryWriter = New BinaryWriter(File.Open(outFile, FileMode.Create))
            writer.Write(out_QB.getVersion(0))
            writer.Write(out_QB.getVersion(1))
            writer.Write(out_QB.getVersion(2))
            writer.Write(out_QB.getVersion(3))

            writer.Write(out_QB.getColorFormat)

            writer.Write(out_QB.getZOrient)

            writer.Write(out_QB.getCompress)

            writer.Write(out_QB.getVisMask)

            writer.Write(out_QB.getMatrixCount)

            For i As Integer = 1 To out_QB.getMatrixCount
                'Dim name_length As Byte = reader.ReadByte
                Dim j As Integer = i - 1
                writer.Write(out_QB.matrix(j).getName)
                'Console.WriteLine("Matrix Name: " + out_QB.matrix(j).getName)

                writer.Write(out_QB.matrix(j).data.GetLength(0))
                writer.Write(out_QB.matrix(j).data.GetLength(1))
                writer.Write(out_QB.matrix(j).data.GetLength(2))
                'Console.WriteLine("Matrix Size: " + out_QB.matrix(j).getSizeX.ToString + "," + out_QB.matrix(j).getSizeY.ToString + "," + out_QB.matrix(j).getSizeZ.ToString)

                writer.Write(out_QB.matrix(j).getPosX)
                writer.Write(out_QB.matrix(j).getPosY)
                writer.Write(out_QB.matrix(j).getPosZ)

                If out_QB.getCompress = 0 Then    'if uncompressed
                    For z As Integer = 0 To out_QB.matrix(j).data.GetLength(2) - 1
                        For y As Integer = 0 To out_QB.matrix(j).data.GetLength(1) - 1
                            For x As Integer = 0 To out_QB.matrix(j).data.GetLength(0) - 1
                                writer.Write(out_QB.matrix(j).data(x, y, z))
                                'Console.WriteLine(x.ToString + "," + y.ToString + "," + z.ToString + ": " + out_QB.matrix(j).data(x, y, z).ToString)
                            Next
                        Next
                    Next
                Else    'if compressed
                    Console.WriteLine("ERROR: Compressed QB, can not process!")
                End If
            Next
            writer.Dispose()
        End Using
    End Sub
End Class

Public Class QBModel
    Dim Version(3) As Byte
    Dim Color_format As Integer
    Dim z_orient As Integer
    Dim Compress As Integer
    Dim Vis_mask As Integer
    Dim matrix_cnt As Integer   'is this needed?
    Public matrix(0) As QBMatrix

    Sub New()
        Version(0) = 0
        Version(1) = 0
        Version(2) = 0
        Version(3) = 0

        Color_format = 0
        z_orient = 0
        Compress = 0
        Vis_mask = 0
        matrix(0) = New QBMatrix

    End Sub
    Sub New(ByVal v1 As Byte, ByVal v2 As Byte, ByVal v3 As Byte, ByVal v4 As Byte, ByVal cf As Byte, ByVal zo As Byte, ByVal com As Byte, ByVal vm As Byte, ByVal M As QBMatrix())
        Version(0) = v1
        Version(1) = v2
        Version(2) = v3
        Version(3) = v4

        Color_format = cf
        z_orient = zo
        Compress = com
        Vis_mask = vm
        matrix = M

    End Sub
    Sub New(ByVal QB As QBModel)
        Version(0) = QB.Version(0)
        Version(1) = QB.Version(1)
        Version(2) = QB.Version(2)
        Version(3) = QB.Version(3)

        Color_format = QB.Color_format
        z_orient = QB.z_orient
        Compress = QB.Compress
        Vis_mask = QB.Vis_mask
        matrix = QB.matrix
    End Sub

    Public Sub setVersion(ByVal ver1 As Byte, ByVal ver2 As Byte, ByVal ver3 As Byte, ByVal ver4 As Byte)
        Version(0) = ver1
        Version(1) = ver2
        Version(2) = ver3
        Version(3) = ver4
    End Sub
    Public Function getVersion(ByVal i As Byte)
        Return Version(i)
    End Function
    Public Function getVersionAsString()
        Dim ver As String = Version(0).ToString + "." + Version(1).ToString + "." + Version(2).ToString + "." + Version(3).ToString
        Return ver
    End Function

    Public Sub setColorFormat(ByVal CF As Integer)
        Color_format = CF
    End Sub
    Public Function getColorFormat()
        Return Color_format
    End Function

    Public Sub setZOrient(ByVal ZO As Integer)
        z_orient = ZO
    End Sub
    Public Function getZOrient()
        Return z_orient
    End Function

    Public Sub setCompress(ByVal comp As Integer)
        Compress = comp
    End Sub
    Public Function getCompress()
        Return Compress
    End Function

    Public Sub setVisMask(ByVal VisMask As Integer)
        Vis_mask = VisMask
    End Sub
    Public Function getVisMask()
        Return Vis_mask
    End Function

    Public Sub setMatrixCount(ByVal M_CNT As Integer)
        'matrix_cnt = M_CNT
        ReDim Preserve matrix(M_CNT)
    End Sub
    Public Function getMatrixCount()
        'Return matrix_cnt
        Return matrix.GetLength(0)
    End Function

End Class

Public Class QBMatrix
    Dim name As String
    Dim posX As Integer
    Dim posY As Integer
    Dim posZ As Integer
    Public data(,,) As Integer

    Sub New()
        name = ""
        posX = 0
        posY = 0
        posZ = 0
        ReDim data(0, 0, 0)
    End Sub
    Sub New(ByVal m_name As String)
        name = m_name
        posX = 0
        posY = 0
        posZ = 0
        ReDim data(0, 0, 0)
    End Sub
    Sub New(ByVal m_name As String, ByVal pos_x As Integer, ByVal pos_y As Integer, ByVal pos_z As Integer)
        name = m_name
        posX = pos_x
        posY = pos_y
        posZ = pos_z
        ReDim data(0, 0, 0)
    End Sub
    Sub New(ByVal m_name As String, ByVal pos_x As Integer, ByVal pos_y As Integer, ByVal pos_z As Integer, ByVal size_x As Integer, ByVal size_y As Integer, ByVal size_z As Integer)
        name = m_name
        posX = pos_x
        posY = pos_y
        posZ = pos_z
        ReDim data(size_x, size_y, size_z)
    End Sub

    Public Sub setName(ByVal name_str As String)
        name = name_str
    End Sub
    Public Function getName()
        Return name
    End Function

    Public Sub setPosX(ByVal pos As String)
        posX = pos
    End Sub
    Public Function getPosX()
        Return posX
    End Function
    Public Sub setPosY(ByVal pos As String)
        posY = pos
    End Sub
    Public Function getPosY()
        Return posY
    End Function
    Public Sub setPosZ(ByVal pos As String)
        posZ = pos
    End Sub
    Public Function getPosZ()
        Return posZ
    End Function
    Public Sub setPos(ByVal x As Integer, ByVal y As Integer, ByVal z As Integer)
        posX = x
        posY = y
        posZ = z
    End Sub

    Public Sub setMatrixSize(ByVal x As Integer, ByVal y As Integer, ByVal z As Integer)
        ReDim data(x, y, z)
    End Sub

    Public Function getSizeX()
        Return data.GetLength(0)
    End Function
    Public Function getSizeY()
        Return data.GetLength(1)
    End Function
    Public Function getSizeZ()
        Return data.GetLength(2)
    End Function

End Class

Public Class color
    'convert fomr INT32 to RGBA and back
    Public Shared Function INT32toRGBA(ByVal int32 As Integer)
        Dim bytes() As Byte = BitConverter.GetBytes(int32)
        'Console.WriteLine("RGBA color(" + int32.ToString + "): " + bytes(0).ToString + "," + bytes(1).ToString + "," + bytes(2).ToString + "," + bytes(3).ToString)
        Return bytes
    End Function
    Public Shared Function RGBAtoINT32(ByVal r As Integer, ByVal g As Integer, ByVal b As Integer, ByVal a As Integer)
        'convert stuff here
        Dim bytes() As Byte = New Byte() {r, g, b, a}
        Dim int32 As Integer = BitConverter.ToInt32(bytes, 0)
        'Console.WriteLine("Int32 color(" + bytes(0).ToString + "," + bytes(1).ToString + "," + bytes(2).ToString + "," + bytes(3).ToString + "): " + int32.ToString)
        Return int32
    End Function

    'convert RGBA to HSV version 2
    Public Shared Function RGBtoHSV2(ByVal R As Integer, ByVal G As Integer, ByVal B As Integer) As Integer()
        ''# Normalize the RGB values by scaling them to be between 0 and 1
        Dim red As Decimal = R / 255D
        Dim green As Decimal = G / 255D
        Dim blue As Decimal = B / 255D

        Dim minValue As Decimal = Math.Min(red, Math.Min(green, blue))
        Dim maxValue As Decimal = Math.Max(red, Math.Max(green, blue))
        Dim delta As Decimal = maxValue - minValue

        Dim h As Decimal
        Dim s As Decimal
        Dim v As Decimal = maxValue

        ''# Calculate the hue (in degrees of a circle, between 0 and 360)
        Select Case maxValue
            Case red
                If green >= blue Then
                    If delta = 0 Then
                        h = 0
                    Else
                        h = 60 * (green - blue) / delta
                    End If
                ElseIf green < blue Then
                    h = 60 * (green - blue) / delta + 360
                End If
            Case green
                h = 60 * (blue - red) / delta + 120
            Case blue
                h = 60 * (red - green) / delta + 240
        End Select

        ''# Calculate the saturation (between 0 and 1)
        If maxValue = 0 Then
            s = 0
        Else
            s = 1D - (minValue / maxValue)
        End If

        ''# Scale the saturation and value to a percentage between 0 and 100
        s *= 255
        v *= 255

        ''# Return a color in the new color space
        Return {CInt(Math.Round(h, MidpointRounding.AwayFromZero)), _
                       CInt(Math.Round(s, MidpointRounding.AwayFromZero)), _
                       CInt(Math.Round(v, MidpointRounding.AwayFromZero))}
    End Function
    Public Shared Function HSVtoRGB2(ByVal H As Integer, ByVal S As Integer, ByVal V As Integer) As Byte()
        ''# Scale the Saturation and Value components to be between 0 and 1
        Dim hue As Decimal = H
        Dim sat As Decimal = S / 255D
        Dim val As Decimal = V / 255D

        Dim r As Decimal
        Dim g As Decimal
        Dim b As Decimal

        If sat = 0 Then
            ''# If the saturation is 0, then all colors are the same.
            ''# (This is some flavor of gray.)
            r = val
            g = val
            b = val
        Else
            ''# Calculate the appropriate sector of a 6-part color wheel
            Dim sectorPos As Decimal = hue / 60D
            Dim sectorNumber As Integer = CInt(Math.Floor(sectorPos))

            ''# Get the fractional part of the sector
            ''# (that is, how many degrees into the sector you are)
            Dim fractionalSector As Decimal = sectorPos - sectorNumber

            ''# Calculate values for the three axes of the color
            Dim p As Decimal = val * (1 - sat)
            Dim q As Decimal = val * (1 - (sat * fractionalSector))
            Dim t As Decimal = val * (1 - (sat * (1 - fractionalSector)))

            ''# Assign the fractional colors to red, green, and blue
            ''# components based on the sector the angle is in
            Select Case sectorNumber
                Case 0, 6
                    r = val
                    g = t
                    b = p
                Case 1
                    r = q
                    g = val
                    b = p
                Case 2
                    r = p
                    g = val
                    b = t
                Case 3
                    r = p
                    g = q
                    b = val
                Case 4
                    r = t
                    g = p
                    b = val
                Case 5
                    r = val
                    g = p
                    b = q
            End Select
        End If

        ''# Scale the red, green, and blue values to be between 0 and 255
        r *= 255
        g *= 255
        b *= 255

        ''# Return a color in the new color space
        Return {CInt(Math.Round(r, MidpointRounding.AwayFromZero)), _
                       CInt(Math.Round(g, MidpointRounding.AwayFromZero)), _
                       CInt(Math.Round(b, MidpointRounding.AwayFromZero))}
    End Function

    'TODO: convert from INT32 to HSV and back

    'recolor QB
    Public Shared Function recolorQB(ByVal QB As QBModel, ByVal QB_NEG As QBModel, ByVal dyeColor As dye_color)
        Dim NewQB As New QBModel(recolorQB(QB, QB_NEG, dyeColor.hue, dyeColor.sat, dyeColor.val))
        Return NewQB
    End Function
    Public Shared Function recolorQB(ByVal QB As QBModel, ByVal QB_NEG As QBModel, ByVal hue As String, ByVal sat As String, ByVal val As String) As QBModel
        Dim NewQB As New QBModel(QB)
        'recolor code

        'go through QB_NEG until a black voxel is found
        For i As Integer = 1 To QB_NEG.getMatrixCount
            'Dim name_length As Byte = reader.ReadByte
            Dim j As Integer = i - 1
            For z As Integer = 0 To QB_NEG.matrix(j).data.GetLength(2) - 1
                For y As Integer = 0 To QB_NEG.matrix(j).data.GetLength(1) - 1
                    For x As Integer = 0 To QB_NEG.matrix(j).data.GetLength(0) - 1
                        If QB_NEG.matrix(j).data(x, y, z) > 0 Then
                            Dim colorbytes() As Byte = INT32toRGBA(QB_NEG.matrix(j).data(x, y, z))
                            Dim recolor As Boolean = False

                            If Form1.CB_o_CNEG.Checked Then
                                If colorbytes(0) = Form1.NUD_o_R.Value And colorbytes(1) = Form1.NUD_o_R.Value And colorbytes(2) = Form1.NUD_o_R.Value And colorbytes(3) <> 0 Then recolor = True 'voxel is visible and equal to custom color
                            Else
                                If colorbytes(0) = 0 And colorbytes(1) = 0 And colorbytes(2) = 0 And colorbytes(3) <> 0 Then recolor = True 'voxel is visible and pure black
                            End If

                            If recolor = True Then
                                'open that voxel in NewQB
                                'convert int32color to rgba
                                Dim rgba() As Byte = INT32toRGBA(QB.matrix(j).data(x, y, z))
                                'convert rgb to hsv and recolor
                                Dim hsv() As Integer = recolorHSV(RGBtoHSV2(rgba(0), rgba(1), rgba(2)), hue, sat, val)

                                'convert new hsv to rgba (using original a)
                                Dim rgba_temp() As Byte = HSVtoRGB2(hsv(0), hsv(1), hsv(2))
                                rgba(0) = rgba_temp(0)
                                rgba(1) = rgba_temp(1)
                                rgba(2) = rgba_temp(2)

                                'convert rbga to int32color
                                'set voxel in NewQB to int32color
                                NewQB.matrix(j).data(x, y, z) = RGBAtoINT32(rgba(0), rgba(1), rgba(2), rgba(3))
                            End If
                        End If
                    Next
                Next
            Next
        Next


        Return NewQB
    End Function

    'recolor PNG
    Public Shared Function recolorPNG(ByVal PNG As Bitmap, ByVal PNG_NEG As Bitmap, ByVal dyeColor As dye_color)
        Dim NewPNG As Bitmap
        NewPNG = recolorPNG(PNG, PNG_NEG, dyeColor.hue, dyeColor.sat, dyeColor.val)
        Return NewPNG
    End Function
    Public Shared Function recolorPNG(ByVal PNG As Bitmap, ByVal PNG_NEG As Bitmap, ByVal hue As String, ByVal sat As String, ByVal val As String)
        Dim NewPNG As New Bitmap(PNG)
        'recolor code here
        For x As Integer = 0 To PNG_NEG.Width - 1
            For y As Integer = 0 To PNG_NEG.Width - 1
                Dim recolor As Boolean = False

                If Form1.CB_o_CNEG.Checked Then
                    If PNG_NEG.GetPixel(x, y).R = Form1.NUD_o_R.Value And PNG_NEG.GetPixel(x, y).G = Form1.NUD_o_G.Value And PNG_NEG.GetPixel(x, y).B = Form1.NUD_o_B.Value And PNG_NEG.GetPixel(x, y).A <> 0 Then recolor = True
                Else
                    If PNG_NEG.GetPixel(x, y).R = 0 And PNG_NEG.GetPixel(x, y).G = 0 And PNG_NEG.GetPixel(x, y).B = 0 And PNG_NEG.GetPixel(x, y).A <> 0 Then recolor = True
                End If

                If recolor = True Then
                    Dim hsv() As Integer = recolorHSV(RGBtoHSV2(PNG.GetPixel(x, y).R, PNG.GetPixel(x, y).G, PNG.GetPixel(x, y).B), hue, sat, val)

                    Dim rgb() As Byte = HSVtoRGB2(hsv(0), hsv(1), hsv(2))
                    Dim a As Byte = PNG.GetPixel(x, y).A
                    NewPNG.SetPixel(x, y, Drawing.Color.FromArgb(a, rgb(0), rgb(1), rgb(2)))

                End If
            Next
        Next

        Return NewPNG
    End Function

    Public Shared Function recolorHSV(ByVal HSV() As Integer, ByVal HC As String, ByVal SC As String, ByVal VC As String) As Integer()
        Return recolorHSV(HSV(0), HSV(1), HSV(2), HC, SC, VC)
    End Function
    Public Shared Function recolorHSV(ByVal H As Integer, ByVal S As Integer, ByVal V As Integer, ByVal HC As String, ByVal SC As String, ByVal VC As String) As Integer()
        'New hsv
        Dim hsv() As Integer = {H, S, V}

        'change Hue
        If HC.ToLower.StartsWith("s") Then
            hsv(0) = Integer.Parse(HC.Substring(1))
        ElseIf HC.ToLower.StartsWith("p") Then
            hsv(0) = hsv(0) * (Integer.Parse(HC.Substring(1)) / 100)
        Else
            hsv(0) = hsv(0) + Integer.Parse(HC)
        End If

        'change saturation
        If SC.ToLower.StartsWith("s") Then
            hsv(1) = Integer.Parse(SC.Substring(1))
        ElseIf SC.ToLower.StartsWith("p") Then
            hsv(1) = hsv(1) * (Integer.Parse(SC.Substring(1)) / 100)
        Else
            hsv(1) = hsv(1) + Integer.Parse(SC)
        End If

        'change value
        If VC.ToLower.StartsWith("s") Then
            hsv(2) = Integer.Parse(VC.Substring(1))
        ElseIf VC.ToLower.StartsWith("p") Then
            hsv(2) = hsv(2) * (Integer.Parse(VC.Substring(1)) / 100)
        Else
            hsv(2) = hsv(2) + Integer.Parse(VC)
        End If

        'make sure hsv is in the correct bounds
        If hsv(0) < 0 Then hsv(0) = hsv(0) + 360
        If hsv(0) > 360 Then hsv(0) = hsv(0) - 360

        If hsv(1) < 0 Then hsv(1) = 0
        If hsv(1) > 255 Then hsv(1) = 255

        If hsv(2) < 0 Then hsv(2) = 0
        If hsv(2) > 255 Then hsv(2) = 255

        Return hsv
    End Function
End Class

Public Class dye_color
    Public name As String
    Public hrname As String
    Public dcolor As String
    Public hue As String
    Public sat As String
    Public val As String

    Public Sub New(ByVal nNAME As String, ByVal nHRNAME As String, ByVal nDCOLOR As String, ByVal nHUE As String, ByVal nSAT As String, ByVal nVAL As String)
        name = nNAME
        hrname = nHRNAME
        dcolor = nDCOLOR
        hue = nHUE
        sat = nSAT
        val = nVAL
    End Sub

End Class

Public Class PARSE
    'parses text
    Public Shared Function TEXT(ByVal in_txt As TextBox)
        Return TEXT(in_txt.Text)
    End Function
    Public Shared Function TEXT(ByVal in_str As String)
        Dim out_str As String = ""

        If in_str.Contains("~") Then
            'parse string
            Dim in_str_split() As String = in_str.Split("~")

            For Each str_sub As String In in_str_split
                If str_sub = "iname" Or str_sub = "name" Then
                    out_str = out_str + Form1.TB_iname.Text
                ElseIf str_sub = "hriname" Or str_sub = "hrname" Then
                    out_str = out_str + Form1.TB_hriname.Text
                ElseIf str_sub = "mname" Then
                    out_str = out_str + Form1.TB_mname.Text
                ElseIf str_sub = "color" Then
                    out_str = out_str + Form1.currentColor.name
                ElseIf str_sub = "hrcolor" Then
                    out_str = out_str + Form1.currentColor.hrname
                ElseIf str_sub = "rcolor" Or str_sub = "dcolor" Then
                    out_str = out_str + Form1.currentColor.dcolor
                ElseIf str_sub = "ifolder" Or str_sub = "impath" Then
                    out_str = out_str + TEXT(Form1.TB_ifolder.Text)    'possible never ending loop if ~ifolder~ is put in TB_ifolder TODO fix in form
                ElseIf str_sub = "rfile" Then
                    out_str = out_str + TEXT(Form1.TB_recipe_file.Text)    'possible never ending loop if ~rfile~ is put in TB_recipe_file TODO fix in form
                ElseIf str_sub = "crafter" Then
                    out_str = out_str + Form1.TB_recipe_crafter.Text
                Else
                    out_str = out_str + str_sub
                End If
            Next
        Else
            'string doesn't need parsed
            out_str = in_str
        End If

        'Console.WriteLine("Parsing String: " + in_str)
        'Console.WriteLine("Parsed String: " + out_str)
        Return out_str
    End Function

    'parses and saves JSON files
    Public Shared Sub JSON(ByVal inFile As String, ByVal outFile As String)
        Dim fileReader = My.Computer.FileSystem.OpenTextFileReader(inFile)
        Dim fileWriter = My.Computer.FileSystem.OpenTextFileWriter(outFile, False, Encoding.ASCII)
        While Not fileReader.EndOfStream
            Dim blah = fileReader.ReadLine
            Dim neoBlah = TEXT(blah)
            fileWriter.WriteLine(neoBlah)

            'Console.WriteLine("JSON: " + neoBlah)
        End While

        fileReader.Dispose()
        fileWriter.Dispose()
    End Sub

    'parese custom colors
    Public Shared Function COLOR(ByVal str As String) As dye_color
        Dim split() As String = str.Split(",")

        Return New dye_color(split(0), split(1), split(2), split(3), split(4), split(5))
    End Function

    'parse recipe ingredients
    Public Shared Function RECIP_INGR(ByVal str As String) As String
        Dim newstr As String
        Dim temp() As String = str.Split(",")
        newstr = vbTab & vbTab & "{" & vbCrLf & _
            vbTab & vbTab & vbTab & """material"" : """ & PARSE.TEXT(temp(1)) & """," & vbCrLf & _
            vbTab & vbTab & vbTab & """count"" : " & temp(0) & vbCrLf & _
            vbTab & vbTab & "}"
        Return newstr
    End Function
End Class

Public Class VerticalProgressBar
    Inherits ProgressBar
    Protected Overrides ReadOnly Property CreateParams() As CreateParams
        Get
            Dim cp As CreateParams = MyBase.CreateParams
            cp.Style = cp.Style Or &H4
            Return cp
        End Get
    End Property
End Class