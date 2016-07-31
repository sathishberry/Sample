Imports System.Data
Imports System.Web.Services
Imports System.Data.SqlClient
Imports System.Configuration

Partial Class VB
    Inherits System.Web.UI.Page
    Private Shared PageSize As Integer = 5
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindDummyRow()
        End If
    End Sub

    Private Sub BindDummyRow()
        Dim dummy As New DataTable()
        dummy.Columns.Add("CustomerID")
        dummy.Columns.Add("ContactName")
        dummy.Columns.Add("City")
        dummy.Rows.Add()
        gvCustomers.DataSource = dummy
        gvCustomers.DataBind()
    End Sub


    <WebMethod()> _
    Public Shared Function GetCustomers(searchTerm As String, pageIndex As Integer) As String
        Dim query As String = "[GetCustomers_Pager]"
        Dim cmd As New SqlCommand(query)
        cmd.CommandType = CommandType.StoredProcedure
        cmd.Parameters.AddWithValue("@SearchTerm", searchTerm)
        cmd.Parameters.AddWithValue("@PageIndex", pageIndex)
        cmd.Parameters.AddWithValue("@PageSize", PageSize)
        cmd.Parameters.Add("@RecordCount", SqlDbType.Int, 4).Direction = ParameterDirection.Output
        Return GetData(cmd, pageIndex).GetXml()
    End Function

    Private Shared Function GetData(cmd As SqlCommand, pageIndex As Integer) As DataSet
        Dim strConnString As String = ConfigurationManager.ConnectionStrings("conString").ConnectionString
        Using con As New SqlConnection(strConnString)
            Using sda As New SqlDataAdapter()
                cmd.Connection = con
                sda.SelectCommand = cmd
                Using ds As New DataSet()
                    sda.Fill(ds, "Customers")
                    Dim dt As New DataTable("Pager")
                    dt.Columns.Add("PageIndex")
                    dt.Columns.Add("PageSize")
                    dt.Columns.Add("RecordCount")
                    dt.Rows.Add()
                    dt.Rows(0)("PageIndex") = pageIndex
                    dt.Rows(0)("PageSize") = PageSize
                    dt.Rows(0)("RecordCount") = cmd.Parameters("@RecordCount").Value
                    ds.Tables.Add(dt)
                    Return ds
                End Using
            End Using
        End Using
    End Function
End Class
