Imports System.IO.Ports
Imports Microsoft.Office.Interop
Public Class Form1
    Dim ThreadData As Threading.Thread
    Dim ThreadExport As Threading.Thread
    Dim WithEvents ElectricMeter As New System.IO.Ports.SerialPort
    Dim MyPort As Array
    Dim DataTable As DataTable
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Form1.CheckForIllegalCrossThreadCalls = False
        MyPort = IO.Ports.SerialPort.GetPortNames()
        ComboBox1.Items.AddRange(MyPort)
    End Sub
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        ElectricMeter.PortName = ComboBox1.Text
        ElectricMeter.BaudRate = 9600
        ElectricMeter.Parity = Parity.None
        ElectricMeter.DataBits = 8
        ElectricMeter.StopBits = StopBits.One
        ElectricMeter.Open()
        Button1.Enabled = False
        Button1.Text = "Connected"
    End Sub
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        ListBox1.Items.Clear()
        TextBox10.Text = ""
        ThreadData = New System.Threading.Thread(AddressOf CVFlow)
        ThreadData.IsBackground = False
        ThreadData.Start()
        Button2.Enabled = False
        Button3.Enabled = False
        Button2.Text = "Scanning"
    End Sub
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        ListBox1.Items.Clear()
        TextBox10.Text = ""
        ThreadData = New System.Threading.Thread(AddressOf LSVFlow)
        ThreadData.IsBackground = False
        ThreadData.Start()
        Button2.Enabled = False
        Button3.Enabled = False
        Button3.Text = "Scanning"
    End Sub
    Sub CVFlow()
        ListBox1.Items.Clear()
        DataTable = New DataTable
        DataTable.Columns.Add("Voltage")
        DataTable.Columns.Add("Current")
        Dim DataCount As Integer = (Math.Abs(TextBox2.Text - TextBox1.Text) / TextBox5.Text + Math.Abs(TextBox3.Text - TextBox2.Text) / TextBox5.Text + Math.Abs(TextBox4.Text - TextBox3.Text) / TextBox5.Text + 1) * TextBox6.Text
        Dim LoopCount As Integer = TextBox6.Text
        Dim NowVoltage As Double = TextBox1.Text
        Dim StartVoltage As Double = TextBox1.Text
        Dim HighVoltage As Double = TextBox2.Text
        Dim LowVoltage As Double = TextBox3.Text
        Dim EndVoltage As Double = TextBox4.Text
        Dim VoltageInterval As Double = TextBox5.Text
        Dim Line As String = ComboBox6.Text
        Dim Direction As String = ComboBox7.Text
        Dim Data As String = "1"
        ListBox1.Items.Add("*RST")                                                                    '重置所有設定
        ElectricMeter.Write("*RST" & vbCrLf)
        ListBox1.Items.Add(":SOUR:FUNC VOLT")                                              '設定電流輸出
        'ElectricMeter.Write(":SOUR:FUNC VOLT" & vbCrLf)
        ListBox1.Items.Add(":SOUR:VOLT:MODE FIXED")                                  '固定電壓模式
        ElectricMeter.Write(":SOUR:VOLT:MODE FIXED" & vbCrLf)
        ListBox1.Items.Add(":SOUR:VOLT:RANG 20")                                        '電壓範圍(不知道幹麻的)
        ElectricMeter.Write(":SOUR:VOLT:RANG 20" & vbCrLf)
        ListBox1.Items.Add(":SENS:CURR:PROT 1")                                            '(最大電流)
        ElectricMeter.Write(":SENS:CURR:PROT 1" & vbCrLf)
        ListBox1.Items.Add(":SENS:FUNC " & Chr(34) & "CURR" & Chr(34))                                           '(不知道啥設定)
        ElectricMeter.Write(":SENS:FUNC " & Chr(34) & "CURR" & Chr(34) & vbCrLf)
        ListBox1.Items.Add(":FORM:ELEM CURR")                                             '(不知道啥設定)
        ElectricMeter.Write(":FORM:ELEM CURR" & vbCrLf)
        ListBox1.Items.Add("SENS:FUNC " & Chr(34) & "CURR" & Chr(34))     '測電流
        ElectricMeter.Write("SENS:FUNC " & Chr(34) & "CURR" & Chr(34) & vbCrLf)
        ListBox1.Items.Add("SENS:CURR:RANG:AUTO ON")                             '電流範圍自動
        ElectricMeter.Write("SENS:CURR:RANG:AUTO ON" & vbCrLf)
        Select Case Line
            Case "4"
                ListBox1.Items.Add(":SYST:RSEN ON")                                                   'On-四線   Off-兩線
                ElectricMeter.Write(":SYST:RSEN ON" & vbCrLf)
            Case "2"
                ListBox1.Items.Add(":SYST:RSEN Off")                                                   'On-四線   Off-兩線
                ElectricMeter.Write(":SYST:RSEN Off" & vbCrLf)
        End Select
        ListBox1.Items.Add(":OUTP ON")                                                            '開始
        ElectricMeter.Write(":OUTP ON" & vbCrLf)
        For i = 0 To LoopCount - 1
            Select Case Direction
                Case "正向"
                    Do Until NowVoltage >= HighVoltage
                        ListBox1.Items.Add(":SOUR:VOLT:LEV " & NowVoltage.ToString("0.0000"))
                        ElectricMeter.Write(":SOUR:VOLT:LEV " & NowVoltage.ToString("0.0000") & vbCrLf)
                        ListBox1.Items.Add("READ?")
                        ElectricMeter.Write("READ?" & vbCrLf)
                        Data = ElectricMeter.ReadLine
                        ListBox1.Items.Add("Voltage:" & NowVoltage.ToString)
                        ListBox1.Items.Add("Current:" & Data.ToString)
                        DataTable.Rows.Add(NowVoltage, Convert.ToDouble(Data))
                        NowVoltage = Math.Round(NowVoltage + VoltageInterval, 4)
                    Loop
                    Do Until NowVoltage <= LowVoltage
                        ListBox1.Items.Add(":SOUR:VOLT:LEV " & NowVoltage.ToString("0.0000"))
                        ElectricMeter.Write(":SOUR:VOLT:LEV " & NowVoltage.ToString("0.0000") & vbCrLf)
                        ListBox1.Items.Add("READ?")
                        ElectricMeter.Write("READ?" & vbCrLf)
                        Data = ElectricMeter.ReadLine
                        ListBox1.Items.Add("Voltage:" & NowVoltage.ToString)
                        ListBox1.Items.Add("Current:" & Data.ToString)
                        DataTable.Rows.Add(NowVoltage, Convert.ToDouble(Data))
                        NowVoltage = Math.Round(NowVoltage - VoltageInterval, 4)
                    Loop
                    Do Until NowVoltage >= EndVoltage + VoltageInterval
                        ListBox1.Items.Add(":SOUR:VOLT:LEV " & NowVoltage.ToString("0.0000"))
                        ElectricMeter.Write(":SOUR:VOLT:LEV " & NowVoltage.ToString("0.0000") & vbCrLf)
                        ListBox1.Items.Add("READ?")
                        ElectricMeter.Write("READ?" & vbCrLf)
                        Data = ElectricMeter.ReadLine
                        ListBox1.Items.Add("Voltage:" & NowVoltage.ToString)
                        ListBox1.Items.Add("Current:" & Data.ToString)
                        DataTable.Rows.Add(NowVoltage, Convert.ToDouble(Data))
                        NowVoltage = Math.Round(NowVoltage + VoltageInterval, 4)
                    Loop
                    NowVoltage = StartVoltage
                Case "反向"
                    Do Until NowVoltage <= LowVoltage
                        ListBox1.Items.Add(":SOUR:VOLT:LEV " & NowVoltage.ToString("0.0000"))
                        ElectricMeter.Write(":SOUR:VOLT:LEV " & NowVoltage.ToString("0.0000") & vbCrLf)
                        ListBox1.Items.Add("READ?")
                        ElectricMeter.Write("READ?" & vbCrLf)
                        Data = ElectricMeter.ReadLine
                        ListBox1.Items.Add("Voltage:" & NowVoltage.ToString)
                        ListBox1.Items.Add("Current:" & Data.ToString)
                        DataTable.Rows.Add(NowVoltage, Convert.ToDouble(Data))
                        NowVoltage = Math.Round(NowVoltage - VoltageInterval, 4)
                    Loop
                    Do Until NowVoltage >= HighVoltage
                        ListBox1.Items.Add(":SOUR:VOLT:LEV " & NowVoltage.ToString("0.0000"))
                        ElectricMeter.Write(":SOUR:VOLT:LEV " & NowVoltage.ToString("0.0000") & vbCrLf)
                        ListBox1.Items.Add("READ?")
                        ElectricMeter.Write("READ?" & vbCrLf)
                        Data = ElectricMeter.ReadLine
                        ListBox1.Items.Add("Voltage:" & NowVoltage.ToString)
                        ListBox1.Items.Add("Current:" & Data.ToString)
                        DataTable.Rows.Add(NowVoltage, Convert.ToDouble(Data))
                        NowVoltage = Math.Round(NowVoltage + VoltageInterval, 4)
                    Loop
                    Do Until NowVoltage <= EndVoltage - VoltageInterval
                        ListBox1.Items.Add(":SOUR:VOLT:LEV " & NowVoltage.ToString("0.0000"))
                        ElectricMeter.Write(":SOUR:VOLT:LEV " & NowVoltage.ToString("0.0000") & vbCrLf)
                        ListBox1.Items.Add("READ?")
                        ElectricMeter.Write("READ?" & vbCrLf)
                        Data = ElectricMeter.ReadLine
                        ListBox1.Items.Add("Voltage:" & NowVoltage.ToString)
                        ListBox1.Items.Add("Current:" & Data.ToString)
                        DataTable.Rows.Add(NowVoltage, Convert.ToDouble(Data))
                        NowVoltage = Math.Round(NowVoltage - VoltageInterval, 4)
                    Loop
                    NowVoltage = StartVoltage
            End Select
        Next
        ListBox1.Items.Add("OUPT OFF")
        ElectricMeter.Write("OUPT OFF" & vbCrLf)
        CVExport()
        Button2.Enabled = True
        Button3.Enabled = True
        Button2.Text = "Start"
    End Sub
    Sub CVExport()
        Dim App As New Excel.Application
        Dim Book As Excel.Workbook
        Dim Sheet As Excel.Worksheet
        Dim Range As Excel.Range
        App.DisplayAlerts = False
        App.Visible = False
        '---------------------寫入資料
        Book = App.Workbooks.Add
        Sheet = Book.Sheets(1)
        For i = 0 To DataTable.Rows.Count - 1
            Range = Sheet.Cells(i + 1, 1)
            Range.Value = DataTable.Rows(i).Item(0)
            Range = Sheet.Cells(i + 1, 2)
            Range.Value = DataTable.Rows(i).Item(1)
        Next
        '------------------建立圖表
        Dim Chart As Excel.Chart
        Dim MyChart As Excel.ChartObject
        MyChart = Book.Sheets(1).Chartobjects.Add(150, 0, 1200, 600) '起始X,起始Y,寬度,高度
        Chart = MyChart.Chart
        Chart.ChartType = Excel.XlChartType.xlXYScatter ' 設定為分佈圖
        Dim Y_Start As String
        Dim Y_End As String
        Y_Start = "A2"
        Y_End = "B" & DataTable.Rows.Count - 1
        Range = Book.Sheets(1).range(Y_Start, Y_End)
        Chart.SetSourceData(Source:=Range)
        '-----------------存檔，關閉
        Book.SaveAs(Application.StartupPath & "\Data\CV\" & Date.Now.ToString("yyyy_MM_dd_HH_mm_ss") & ".xls")
        Book.Close()
        App.Quit()
        System.Runtime.InteropServices.Marshal.ReleaseComObject(App)
        DataTable = Nothing
        Range = Nothing
        Sheet = Nothing
        Book = Nothing
        App = Nothing
        GC.Collect()
        MsgBox("Finished", MsgBoxStyle.OkOnly)
    End Sub
    Sub LSVFlow()
        ListBox1.Items.Clear()
        DataTable = New DataTable
        DataTable.Columns.Add("Voltage")
        DataTable.Columns.Add("Current")
        DataTable.Columns.Add("CurrentLog")
        Dim DataCount As Integer = Math.Abs(TextBox8.Text - TextBox7.Text) / TextBox9.Text
        Dim NowVoltage As Double = TextBox7.Text
        Dim StartVoltage As Double = TextBox7.Text
        Dim EndVoltage As Double = TextBox8.Text
        Dim VoltageInterval As Double = TextBox9.Text
        Dim Line As String = ComboBox8.Text
        Dim Current As String = ""
        Dim CurrentLog As String = ""
        Dim LogCurrent() As Double

        ListBox1.Items.Add("*RST")                                                                    '重置所有設定
        ElectricMeter.Write("*RST" & vbCrLf)
        ListBox1.Items.Add(":SOUR:FUNC VOLT")                                              '設定電流輸出
        ElectricMeter.Write(":SOUR:FUNC VOLT" & vbCrLf)
        ListBox1.Items.Add(":SOUR:VOLT:MODE FIXED")                                  '固定電壓模式
        ElectricMeter.Write(":SOUR:VOLT:MODE FIXED" & vbCrLf)
        ListBox1.Items.Add(":SOUR:VOLT:RANG 20")                                        '電壓範圍(不知道幹麻的)
        ElectricMeter.Write(":SOUR:VOLT:RANG 20" & vbCrLf)
        ListBox1.Items.Add(":SENS:CURR:PROT 1")                                            '(最大電流)
        ElectricMeter.Write(":SENS:CURR:PROT 1" & vbCrLf)
        ListBox1.Items.Add(":SENS:FUNC " & Chr(34) & "CURR" & Chr(34))                                           '(不知道啥設定)
        ElectricMeter.Write(":SENS:FUNC " & Chr(34) & "CURR" & Chr(34) & vbCrLf)
        ListBox1.Items.Add(":FORM:ELEM CURR")                                             '(不知道啥設定)
        ElectricMeter.Write(":FORM:ELEM CURR" & vbCrLf)
        ListBox1.Items.Add("SENS:FUNC " & Chr(34) & "CURR" & Chr(34))     '測電流
        ElectricMeter.Write("SENS:FUNC " & Chr(34) & "CURR" & Chr(34) & vbCrLf)
        ListBox1.Items.Add("SENS:CURR:RANG:AUTO ON")                             '電流範圍自動
        ElectricMeter.Write("SENS:CURR:RANG:AUTO ON" & vbCrLf)
        Select Case Line
            Case "4"
                ListBox1.Items.Add(":SYST:RSEN ON")                                                   'On-四線   Off-兩線
                ElectricMeter.Write(":SYST:RSEN ON" & vbCrLf)
            Case "2"
                ListBox1.Items.Add(":SYST:RSEN Off")                                                   'On-四線   Off-兩線
                ElectricMeter.Write(":SYST:RSEN Off" & vbCrLf)
        End Select
        ListBox1.Items.Add(":OUTP ON")                                                            '開始
        ElectricMeter.Write(":OUTP ON" & vbCrLf)


        If StartVoltage < EndVoltage Then
            Do Until NowVoltage > EndVoltage
                ListBox1.Items.Add(":SOUR:VOLT:LEV " & NowVoltage.ToString("0.0000"))
                ElectricMeter.Write(":SOUR:VOLT:LEV " & NowVoltage.ToString("0.0000") & vbCrLf)
                ListBox1.Items.Add("READ?")
                ElectricMeter.Write("READ?" & vbCrLf)
                Current = ElectricMeter.ReadLine
                CurrentLog = Format(Math.Log10(Math.Abs(Convert.ToDouble(Current))), "E")

                ListBox1.Items.Add("Voltage:" & NowVoltage.ToString)
                ListBox1.Items.Add("Current:" & Current.ToString)
                ListBox1.Items.Add("CurrentLog:" & CurrentLog.ToString)

                DataTable.Rows.Add(NowVoltage, Convert.ToDouble(Current), Convert.ToDouble(CurrentLog))
                NowVoltage = Math.Round(NowVoltage + VoltageInterval, 4)
            Loop
        ElseIf StartVoltage > EndVoltage Then
            Do Until NowVoltage < EndVoltage
                ListBox1.Items.Add(":SOUR:VOLT:LEV " & NowVoltage.ToString("0.0000"))
                ElectricMeter.Write(":SOUR:VOLT:LEV " & NowVoltage.ToString("0.0000") & vbCrLf)
                ListBox1.Items.Add("READ?")
                ElectricMeter.Write("READ?" & vbCrLf)
                Current = ElectricMeter.ReadLine
                CurrentLog = Format(Math.Log10(Math.Abs(Convert.ToDouble(Current))), "E")

                ListBox1.Items.Add("Voltage:" & NowVoltage.ToString)
                ListBox1.Items.Add("Current:" & Current.ToString)
                ListBox1.Items.Add("CurrentLog:" & CurrentLog.ToString)

                DataTable.Rows.Add(NowVoltage, Convert.ToDouble(Current), Convert.ToDouble(CurrentLog))
                NowVoltage = Math.Round(NowVoltage - VoltageInterval, 4)
            Loop
        End If
        ListBox1.Items.Add("OUPT OFF")
        ElectricMeter.Write("OUPT OFF" & vbCrLf)
        Dim num As Integer = DataTable.Rows.Count - 1
        ReDim LogCurrent(num - 1)
        Dim voltage(num - 1) As Double
        For i = 0 To num - 1
            LogCurrent(i) = DataTable.Rows(i).Item(2)
            voltage(i) = DataTable.Rows(i).Item(0)
        Next
        Dim min As Double = LogCurrent.Min
        Dim Index As Integer = Array.IndexOf(LogCurrent, min)
        Dim voltageTarget As Double = voltage(Index)
        TextBox10.Text = voltageTarget.ToString

        LSVExport()
        Button2.Enabled = True
        Button3.Enabled = True
        Button3.Text = "Start"
    End Sub
    Sub LSVCalculate()
        Dim num As Integer = DataTable.Rows.Count - 1
        Dim voltage(num) As Double
        Dim current(num) As Double
        Dim currentLog(num) As Double
        For i = 0 To num - 1
            voltage(i) = Convert.ToDouble(DataTable.Rows(i).Item(0))
            current(i) = Math.Abs(Convert.ToDouble(DataTable.Rows(i).Item(1)))
            currentLog(i) = Math.Log10(current(i))
        Next
        Dim min As Double = currentLog.Min
        Dim voltageIndex As Integer = Array.IndexOf(currentLog, min)
        Dim voltageTarget As Double = voltage(voltageIndex)
        TextBox10.Text = voltageTarget.ToString
    End Sub
    Sub LSVExport()
        Dim App As New Excel.Application
        Dim Book As Excel.Workbook
        Dim Sheet As Excel.Worksheet
        Dim Range As Excel.Range
        App.DisplayAlerts = False
        App.Visible = False
        '---------------------寫入資料
        Book = App.Workbooks.Add
        Sheet = Book.Sheets(1)
        For i = 0 To DataTable.Rows.Count - 1
            Range = Sheet.Cells(i + 1, 1)
            Range.Value = DataTable.Rows(i).Item(0)
            Range = Sheet.Cells(i + 1, 2)
            Range.Value = DataTable.Rows(i).Item(1)
            'Range = Sheet.Cells(i + 1, 3)
            'Range.Value = DataTable.Rows(i).Item(2)
        Next
        '------------------建立圖表
        Dim Chart As Excel.Chart
        Dim MyChart As Excel.ChartObject
        MyChart = Book.Sheets(1).Chartobjects.Add(200, 0, 1200, 600) '起始X,起始Y,寬度,高度
        Chart = MyChart.Chart
        Chart.ChartType = Excel.XlChartType.xlXYScatter ' 設定為分佈圖
        Dim Y_Start As String
        Dim Y_End As String
        Y_Start = "A2"
        Y_End = "B" & DataTable.Rows.Count - 1
        Range = Book.Sheets(1).range(Y_Start, Y_End)
        Chart.SetSourceData(Source:=Range)
        '-----------------存檔，關閉
        Book.SaveAs(Application.StartupPath & "\Data\LSV\" & Date.Now.ToString("yyyy_MM_dd_HH_mm_ss") & ".xls")
        Book.Close()
        App.Quit()
        System.Runtime.InteropServices.Marshal.ReleaseComObject(App)
        DataTable = Nothing
        Range = Nothing
        Sheet = Nothing
        Book = Nothing
        App = Nothing
        GC.Collect()
        MsgBox("Finished", MsgBoxStyle.OkOnly)
    End Sub
End Class
