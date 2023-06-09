Imports MySql.Data.MySqlClient
Public Class Form1
    Dim conn As MySqlConnection = New MySqlConnection("Data Source=localhost;Database=db_sales2023;User=root;Password=;")
    Public sql, sql1 As String
    Public dbcomm As MySqlCommand
    Public dbread As MySqlDataReader
    Public dataAdapter As MySqlDataAdapter
    Public ds As DataSet = New DataSet

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            sql = "SELECT i.item_id,i.description, s.quantity FROM item i INNER JOIN stock s ON(i.item_id = s.item_id)"
            conn.Open()

            dbcomm = New MySqlCommand(sql, conn)
            dbread = dbcomm.ExecuteReader()
            ComboBox1.ValueMember = "Value"
            ComboBox1.DisplayMember = "Key"
            While dbread.Read()
                ComboBox1.Items.Add(dbread("item_id"))

                'ComboBox1.Items.Add(New DictionaryEntry(dbread("description"), dbread("item_id")))
            End While
            dbread.Close()
            sql = "SELECT i.item_id, i.description, i.cost_price,i.sell_price, s.quantity FROM item i INNER JOIN stock s ON(i.item_id = s.item_id)"
            dataAdapter = New MySqlDataAdapter(sql, conn)

            dataAdapter.Fill(ds, "item")
            DataGridView1.DataSource = ds
            DataGridView1.DataMember = "item"

        Catch ex As MySqlException
            MsgBox(ex.Message())
        End Try

        conn.Close()
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Try
            sql = "SELECT * FROM item i INNER JOIN stock s ON(i.item_id = s.item_id) where i.item_id = " & Val(ComboBox1.SelectedItem)

            MsgBox(sql)
            conn.Open()
            dbcomm = New MySqlCommand(sql, conn)
            dbread = dbcomm.ExecuteReader()
            While dbread.Read()
                txtDescription.Text = dbread("description")
                txtCost.Text = dbread("cost_price")
                txtSell.Text = dbread("sell_price")
                numQty.Value = dbread("quantity")
            End While


        Catch ex As MySqlException
            MsgBox(ex.Message())
        End Try
        dbread.Close()
        conn.Close()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            'sql = $"UPDATE item SET description = '{txtDescription.Text}', cost_price = {Convert.ToDouble(txtCost.Text)}, sell_price = {CDbl(txtSell.Text)}, image_path = 'default.jpg' WHERE item_id = {Val(ComboBox1.SelectedItem)}"
            sql = $"UPDATE item ,stock s  INNER JOIN item i ON(i.item_id = s.item_id) 
                SET i.description = '{txtDescription.Text}', i.cost_price = {Convert.ToDouble(txtCost.Text)}, i.sell_price = {CDbl(txtSell.Text)}, s.quantity = {Val(numQty.Value)},i.image_path = 'default.jpg' WHERE i.item_id = {Val(ComboBox1.SelectedItem)} "
            conn.Open()
            MessageBox.Show("do you want to save", "new item", MessageBoxButtons.YesNoCancel)
            If DialogResult.Yes Then
                dbcomm = New MySqlCommand(sql, conn)
                Dim i As Integer = dbcomm.ExecuteNonQuery
                If (i > 0) Then
                    MsgBox("item updated")
                End If
            End If
            'sql = $"UPDATE stock SET quantity = {Val(numQty.Value)} WHERE item_id =  {Val(ComboBox1.SelectedItem)}"
            'dbcomm = New MySqlCommand(sql, conn)
            'Dim j As Integer = dbcomm.ExecuteNonQuery
            'If (j > 0) Then
            '    MsgBox("item updated")
            'End If
        Catch ex As MySqlException
            MsgBox(ex.Message())
        End Try
        conn.Close()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Try
            conn.Open()

            'sql = $"DELETE i, s FROM item i INNER JOIN stock s ON (i.item_id = s.item_id) WHERE i.item_id = {Val(ComboBox1.SelectedItem)}"

            sql = $"DELETE FROM item WHERE item_id ={Val(ComboBox1.SelectedItem)}"
            dbcomm = New MySqlCommand(sql, conn)
            Dim i As Integer = dbcomm.ExecuteNonQuery
            If (i > 0) Then
                MsgBox("item deleted")
            End If
            Dim sql2 = $"DELETE FROM stock where item_id = {Val(ComboBox1.SelectedItem)}"
            dbcomm = New MySqlCommand(sql2, conn)
            Dim j As Integer = dbcomm.ExecuteNonQuery
            If (j > 0) Then
                MsgBox("item deleted")
            End If
        Catch ex As MySqlException
            MsgBox(ex.Message())
        End Try

        conn.Close()
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged

        Dim itemName As String = txtSearch.Text
        ds.Clear()

        Try
            sql = $"SELECT i.item_id, i.description, i.cost_price,i.sell_price, s.quantity FROM item i INNER JOIN stock s ON(i.item_id = s.item_id) WHERE i.description LIKE '%{itemName}%'"
            TextBox1.Text = sql
            conn.Open()
            dataAdapter = New MySqlDataAdapter(sql, conn)

            dataAdapter.Fill(ds, "searchItem")

            DataGridView1.DataSource = ds
            DataGridView1.DataMember = "searchItem"
        Catch ex As MySqlException
            MsgBox(ex.Message())
        End Try

        conn.Close()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim description As String = txtDescription.Text
        Dim cost As Double = Convert.ToDouble(txtCost.Text)
        Dim sell As Double = CDbl(txtSell.Text)

        Try
            sql = $"INSERT INTO item (description, cost_price, sell_price, image_path) VALUES ('{description}', {cost}, {sell}, 'default.jpg')"

            conn.Open()
            MessageBox.Show("do you want to save", "new item", MessageBoxButtons.YesNoCancel)
            If DialogResult.Yes Then
                dbcomm = New MySqlCommand(sql, conn)
                Dim i As Integer = dbcomm.ExecuteNonQuery
                If (i > 0) Then
                    MsgBox("item saved")
                End If
            End If
            sql = $"INSERT INTO stock (item_id, quantity) VALUES(last_insert_id(), {Val(numQty.Value)})"
            TextBox1.Text = sql

            dbcomm = New MySqlCommand(sql, conn)
            Dim j As Integer = dbcomm.ExecuteNonQuery
            If (j > 0) Then
                MsgBox("stock saved")
            End If

        Catch ex As MySqlException
            MsgBox(ex.Message())
        End Try
        conn.Close()
    End Sub



End Class
