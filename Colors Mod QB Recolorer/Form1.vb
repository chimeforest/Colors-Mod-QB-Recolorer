Imports System.IO

Public Class Form1
    Public basicColors() As dye_color = {New dye_color("red", "Red", 0, 0, 0, False, False),
                                         New dye_color("orange", "Orange", 30, 0, 0, False, False),
                                         New dye_color("yellow", "Yellow", 60, 0, 0, False, False),
                                         New dye_color("green", "Green", 125, 0, 0, False, False),
                                         New dye_color("blue", "Blue", 210, 0, 0, False, False),
                                         New dye_color("purple", "Purple", 275, 0, 0, False, False),
                                         New dye_color("undyed", "Undyed", 40, 30, 0, True, False)}
    'more colors here

    Public currentColor As dye_color

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Button1.Text = "This may take a while! Please be patient..."
        Button1.Enabled = False
        'Dim cont As Integer = MsgBox("This progess can take a while, during which time the window may become unresponsive. Continue?", vbYesNo, "Continue?")
        'better message box needed here with option to not show it again.
        Dim num_of_files = My.Computer.FileSystem.GetFiles(TB_input_folder.Text).Count

        Dim JSON_files As New Collection
        Dim JSON_recipe As String = ""
        Dim PNG_File As String = ""
        Dim PNG_NEG_File As String = ""
        Dim QB_NEG_File As String = ""
        Dim QB_iconic_NEG_File As String
        Dim QB_File As String = ""
        Dim QB_iconic_File As String = ""

        Dim QB As New QBModel
        Dim QB_NEG As New QBModel
        Dim QB_iconic As New QBModel
        Dim QB_iconic_NEG As New QBModel
        Dim PNG As Bitmap
        Dim PNG_NEG As Bitmap

        Dim colors_to_use As New Collection

        ProgressBar1.Value = 0
        ProgressBar2.Value = 0
        ProgressBar3.Value = 0

        Console.WriteLine("Starting...")
        If TB_input_folder.Text = "" Then   'if none of the required fields are empty then get files
            Button1.Text = "Input Folder text field is empty, please try again."
        ElseIf TB_iname.Text = "" Then
            Button1.Text = "item_name text field is empty, please try again."
        ElseIf TB_mname.Text = "" Then
            Button1.Text = "mod_name text field is empty, please try again."
        ElseIf TB_hriname.Text = "" Then
            Button1.Text = "Human Readable Name text field is empty, please try again."
        ElseIf TB_ifolder.Text = "" Then
            Button1.Text = "Folder location in mod text field is empty, please try again."
        Else    'TODO need to add recipe field checks
            For Each foundFile As String In My.Computer.FileSystem.GetFiles(TB_input_folder.Text)
                Console.WriteLine(foundFile)
                If foundFile.EndsWith(".png") Then
                    If foundFile.EndsWith("NEG.png") Then
                        PNG_NEG_File = foundFile
                        Console.WriteLine("NEG.png found")
                    Else
                        PNG_File = foundFile
                        Console.WriteLine(".png found")
                    End If
                ElseIf foundFile.EndsWith(".qb") Then
                    If foundFile.EndsWith("NEG.qb") Then
                        If foundFile.EndsWith("iconic-NEG.qb") Then
                            QB_iconic_NEG_File = foundFile
                            Console.WriteLine("iconic-NEG.qb found")
                        Else
                            QB_NEG_File = foundFile
                            Console.WriteLine("NEG.qb found")
                        End If
                    Else
                        If foundFile.EndsWith("iconic.qb") Then
                            QB_iconic_File = foundFile
                            Console.WriteLine("iconic.qb found")
                        Else
                            QB_File = foundFile
                            Console.WriteLine(".qb found")
                        End If
                    End If
                ElseIf foundFile.EndsWith(".json") Then
                    If foundFile.EndsWith("recipe.json") Then
                        JSON_recipe = foundFile
                        Console.WriteLine("recipe.json found")
                    Else
                        JSON_files.Add(foundFile)
                        Console.WriteLine(".json found")
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
                    Console.WriteLine(basicColors(i).name + " is checked")
                End If
            Next
            'more colors here

            'import qb
            If QB_File <> "" And CB_o_skip_qb.Checked = False Then
                QB = QBFile.open(QB_File)
                Console.WriteLine("QB_File imported as QB")
                If QB_NEG_File <> "" And CB_o_skip_qb.Checked = False Then
                    QB_NEG = QBFile.open(QB_NEG_File)
                    Console.WriteLine("QB_NEG_File imported as QB_NEG")
                End If
            End If

            If QB_iconic_File <> "" And CB_o_skip_qb.Checked = False Then
                QB_iconic = QBFile.open(QB_iconic_File)
                Console.WriteLine("QB_iconic_File imported as QB_iconic")
                If QB_iconic_NEG_File <> "" And CB_o_skip_qb.Checked = False Then
                    QB_iconic_NEG = QBFile.open(QB_iconic_NEG_File)
                    Console.WriteLine("QB_iconic_NEG_File imported as QB_iconic_NEG")
                End If
            End If

            'import png
            PNG = New Bitmap(PNG_File)
            PNG_NEG = New Bitmap(PNG_NEG_File)

            '~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            'MAIN recolor/parse everything!
            For i As Integer = 1 To colors_to_use.Count
                Dim c_QB As QBModel
                Dim c_PNG As Bitmap
                'set current color
                currentColor = colors_to_use(i)
                Console.WriteLine(currentColor.ToString)

                'set current folder and file name
                Dim name = TB_iname.Text + "_" + currentColor.name
                Dim folder = TB_input_folder.Text + "\output\" + name

                'write current folder
                My.Computer.FileSystem.CreateDirectory(folder)

                'qb
                If CB_o_skip_qb.Checked = False Then
                    c_QB = color.recolorQB(QB, QB_NEG, currentColor)
                    QBFile.save(c_QB, folder + "\" + name + ".qb")

                    'iconic qb
                End If

                'png

            Next

            '~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            'json
            'JSON.parse(JSON_files(1), TB_input_folder.Text + "\output\test\test.json")

            'recipe
            'manifest

            'Dim width As Int32 = PNG.Width
            'Dim height As Int32 = PNG.Height
            'PNG.Save(TB_input_folder.Text + "\output\test\test.png", System.Drawing.Imaging.ImageFormat.Png)

        End If
        Button1.Text = "Finished! - Click here to do it again!"
        Button1.Enabled = True
    End Sub

    Private Sub CheckBox4_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox4.CheckedChanged
        If (CheckBox4.Checked) Then
            TextBox10.Enabled = True
        Else
            TextBox10.Enabled = False
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        WebBrowser1.Navigate("file:///" & IO.Path.GetFullPath(".\Help\Help.html"))
        CheckBox9.CheckState = CheckState.Checked
        random_PB()
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
            CB_recipe_in_folder.Enabled = True
            CB_recipe_gen_list.Enabled = True
            TB_recipe_crafter.Enabled = True
            TB_recipe_desc.Enabled = True
            TB_recipe_flavor.Enabled = True
            TB_recipe_ingre.Enabled = True
            TB_recipe_port.Enabled = True
            TB_recipe_prod.Enabled = True
            TB_recipe_name.Enabled = True
        Else
            TB_recipe_file.Enabled = False
            NUD_work.Enabled = False
            CB_recipe_in_folder.Enabled = False
            CB_recipe_gen_list.Enabled = False
            TB_recipe_crafter.Enabled = False
            TB_recipe_desc.Enabled = False
            TB_recipe_flavor.Enabled = False
            TB_recipe_ingre.Enabled = False
            TB_recipe_port.Enabled = False
            TB_recipe_prod.Enabled = False
            TB_recipe_port.Enabled = False
            TB_recipe_name.Enabled = False
        End If
    End Sub

    Private Sub CB_recipe_in_folder_CheckedChanged(sender As Object, e As EventArgs) Handles CB_recipe_in_folder.CheckedChanged
        If CB_recipe_in_folder.Checked = False Then
            TB_recipe_file.Enabled = True
            NUD_work.Enabled = True
            TB_recipe_crafter.Enabled = True
            TB_recipe_desc.Enabled = True
            TB_recipe_flavor.Enabled = True
            TB_recipe_ingre.Enabled = True
            TB_recipe_port.Enabled = True
            TB_recipe_prod.Enabled = True
            TB_recipe_name.Enabled = True
        Else
            TB_recipe_file.Enabled = False
            NUD_work.Enabled = False
            TB_recipe_crafter.Enabled = False
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
        Else
            TB_recipe_gen_list.Enabled = False
        End If
    End Sub

    Private Sub CB_option_alias_CheckedChanged(sender As Object, e As EventArgs) Handles CB_o_gen_alias.CheckedChanged
        If CB_o_gen_alias.Checked Then
            TB_option_alias.Enabled = True
        Else
            TB_option_alias.Enabled = False
        End If
    End Sub

    Private Sub BTN_open_Click(sender As Object, e As EventArgs) Handles BTN_open.Click
        FolderBrowserDialog1.ShowDialog()
        TB_input_folder.Text = FolderBrowserDialog1.SelectedPath
    End Sub

    Private Sub random_PB()
        Dim rand As New Random
        ProgressBar1.Value = (rand.Next(0, 100))
        ProgressBar2.Value = (rand.Next(0, 100))
        ProgressBar3.Value = (rand.Next(0, 100))
    End Sub
End Class

Public Class QBFile
    Public Shared Function open(ByVal inFile As String)
        Dim QB_M As New QBModel
        Console.WriteLine("Reading QB: " + inFile)

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
        Console.WriteLine("QB out: " + outFile)
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

    'get and set matrices

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

    'convert from RGBA to HSV and back
    'TODO: create RGBAtoHSV which use arrays instead of single numbers?
    Public Shared Function RGBtoHSV(ByVal r As Integer, ByVal g As Integer, ByVal b As Integer)
        Dim hsv(2) As Integer
        Dim min As Double, max As Double, delta As Double

        If r <= g And r <= b Then min = r
        If b <= g And b <= r Then min = b
        If g <= r And g <= b Then min = g

        If r >= g And r >= b Then max = r
        If b >= g And b >= r Then max = b
        If g >= r And g >= b Then max = g

        hsv(2) = max
        delta = max - min

        If max <> 0 Then                'was delta
            hsv(1) = (delta / max) * 255
            If r = max Then
                hsv(0) = (g - b) / delta
            ElseIf g = max Then
                hsv(0) = 2 + (b - r) / delta
            Else
                hsv(0) = 4 + (r - g) / delta
            End If
            hsv(0) = hsv(0) * 60
            If hsv(0) < 0 Then hsv(0) = hsv(0) + 360
            If hsv(0) > 360 Then hsv(0) = hsv(0) - 360

        Else
            hsv(1) = 0
            hsv(0) = 0
        End If

        'Console.WriteLine("HSV Color(" + r.ToString + "," + g.ToString + "," + b.ToString + "): " + hsv(0).ToString + "," + hsv(1).ToString + "," + hsv(2).ToString)
        Return hsv
    End Function
    Public Shared Function HSVtoRGB(ByVal h As Integer, ByVal s As Integer, ByVal v As Integer)
        Dim rgb(2) As Byte
        'convert stuff here
        Dim i As Long
        Dim f As Double, p As Double, q As Double, t As Double

        If s = 0 Then
            rgb(0) = v
            rgb(1) = v
            rgb(2) = v
        Else
            'i = h \ 60 replaced by floor
            h = h / 60
            i = Math.Floor(h)
            f = h - i
            p = v * (1 - (s / 255))
            q = v * (1 - (s / 255) * f)
            t = v * (1 - (s / 255) * (1 - f))

            Select Case i
                Case 0
                    rgb(0) = v
                    rgb(1) = t
                    rgb(2) = p
                Case 1
                    rgb(0) = q
                    rgb(1) = v
                    rgb(2) = p
                Case 2
                    rgb(0) = p
                    rgb(1) = v
                    rgb(2) = t
                Case 3
                    rgb(0) = p
                    rgb(1) = q
                    rgb(2) = v
                Case 4
                    rgb(0) = t
                    rgb(1) = p
                    rgb(2) = v
                Case 5
                Case Else
                    rgb(0) = v
                    rgb(1) = p
                    rgb(2) = q
            End Select
        End If

        'Console.WriteLine("RGB Color(" + h.ToString + "," + s.ToString + "," + v.ToString + "): " + rgb(0).ToString + "," + rgb(1).ToString + "," + rgb(2).ToString)
        Return rgb
    End Function

    'TODO: convert from INT32 to HSV and back

    Public Shared Function recolorQB(ByVal QB As QBModel, ByVal QB_NEG As QBModel, ByVal dyeColor As dye_color)
        Dim NewQB As New QBModel
        NewQB = recolorQB(QB, QB_NEG, dyeColor.hue, dyeColor.sat, dyeColor.val, dyeColor.setSat, dyeColor.setVal)
        Return NewQB
    End Function
    Public Shared Function recolorQB(ByVal QB As QBModel, ByVal QB_NEG As QBModel, ByVal hue As Integer, ByVal sat As Integer, ByVal val As Integer)
        Dim NewQB As New QBModel
        NewQB = recolorQB(QB, QB_NEG, hue, sat, val, False, False)
        Return NewQB
    End Function
    Public Shared Function recolorQB(ByVal QB As QBModel, ByVal QB_NEG As QBModel, ByVal hue As Integer, ByVal sat As Integer, ByVal val As Integer, ByVal setSat As Boolean, ByVal setVal As Boolean)
        Dim NewQB As New QBModel
        NewQB = QB
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
                            If colorbytes(0) = 0 And colorbytes(1) = 0 And colorbytes(2) = 0 And colorbytes(3) <> 0 Then    'voxel is visible and pure black
                                'open that voxel in NewQB
                                'convert int32color to rgba, rgba to hsv
                                Dim rgba() As Byte = INT32toRGBA(NewQB.matrix(j).data(x, y, z))
                                Dim hsv() As Integer = RGBtoHSV(rgba(0), rgba(1), rgba(2))

                                'change hue
                                hsv(0) = hue
                                'change saturation
                                If setSat Then
                                    hsv(1) = sat
                                Else
                                    hsv(1) = hsv(1) + sat
                                End If
                                'change value
                                If setVal Then
                                    hsv(2) = val
                                Else
                                    hsv(2) = hsv(2) + val
                                End If

                                'make sure hsv is in the correct bounds
                                If hsv(0) < 0 Then hsv(0) = 0
                                If hsv(0) > 360 Then hsv(0) = 360

                                If hsv(1) < 0 Then hsv(1) = 0
                                If hsv(1) > 255 Then hsv(1) = 255

                                If hsv(2) < 0 Then hsv(2) = 0
                                If hsv(2) > 255 Then hsv(2) = 255

                                'convert new hsv to rgba (using original a)
                                Dim rgba_temp() As Byte = HSVtoRGB(hsv(0), hsv(1), hsv(2))
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

    Public Shared Function recolorQB(ByVal PNG As Bitmap, ByVal PNG_NEG As Bitmap, ByVal dyeColor As dye_color)
        Dim NewPNG As Bitmap
        NewPNG = recolorPNG(PNG, PNG_NEG, dyeColor.hue, dyeColor.sat, dyeColor.val, dyeColor.setSat, dyeColor.setVal)
        Return NewPNG
    End Function
    Public Shared Function recolorPNG(ByVal PNG As Bitmap, ByVal PNG_NEG As Bitmap, ByVal hue As Integer, ByVal sat As Integer, ByVal val As Integer)
        Dim NewPNG As Bitmap
        NewPNG = recolorPNG(PNG, PNG_NEG, hue, sat, val, False, False)
        Return NewPNG
    End Function
    Public Shared Function recolorPNG(ByVal PNG As Bitmap, ByVal PNG_NEG As Bitmap, ByVal hue As Integer, ByVal sat As Integer, ByVal val As Integer, ByVal setSat As Boolean, ByVal setVal As Boolean)
        Dim NewPNG As New Bitmap(PNG)
        'recolor code here

        Return NewPNG
    End Function
End Class

Public Class JSON
    Public Shared Sub parse(ByVal inFile As String, ByVal outFile As String)
        Dim fileReader = My.Computer.FileSystem.OpenTextFileReader(inFile)
        Dim fileWriter = My.Computer.FileSystem.OpenTextFileWriter(outFile, False)
        While Not fileReader.EndOfStream
            Dim blah = fileReader.ReadLine
            Dim neoBlah = shortcuts.parse(blah)
            fileWriter.WriteLine(neoBlah)

            'Console.WriteLine("JSON: " + neoBlah)
        End While

        fileReader.Dispose()
        fileWriter.Dispose()
    End Sub
End Class

Public Class dye_color
    Public name As String
    Public hrname As String
    Public hue As Integer
    Public sat As Integer
    Public val As Integer
    Public setSat As Boolean
    Public setVal As Boolean

    Public Sub New(ByVal nNAME As String, ByVal nHRNAME As String, ByVal nHUE As Integer, ByVal nSAT As Integer, ByVal nVAL As Integer, ByVal nSETSAT As Boolean, ByVal nSETVAL As Boolean)
        name = nNAME
        hrname = nHRNAME
        hue = nHUE
        sat = nSAT
        val = nVAL
        setSat = nSETSAT
        setVal = nSETVAL
    End Sub

    Public Function ToString()
        Return (name + " " + hrname + ":" + hue.ToString + "," + sat.ToString + "," + val.ToString + ":" + setSat.ToString + "," + setVal.ToString)
    End Function
End Class

Public Class shortcuts
    Public Shared Function parse(ByVal in_str As String)
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
                ElseIf str_sub = "ifolder" Or str_sub = "impath" Then
                    out_str = out_str + parse(Form1.TB_ifolder.Text)    'possible never ending loop if ~ifolder~ is put in TB_ifolder TODO fix in form
                ElseIf str_sub = "rfile" Then
                    out_str = out_str + Form1.TB_recipe_file.Text
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
