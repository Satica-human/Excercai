using System.Data;
using MySql.Data.MySqlClient;

namespace QuanLySinhVien_.DataAccess;

/// <summary>Các thao tác SQL cơ bản với MySQL.</summary>
public sealed class DataHelper : IDisposable
{
    private readonly MySqlConnection _connection = MySqlConnectionFactory.CreateConnection();

    public void OpenConnection()
    {
        if (_connection.State == ConnectionState.Broken)
            _connection.Close();

        if (_connection.State == ConnectionState.Closed)
            _connection.Open();
    }

    public void CloseConnection()
    {
        if (_connection.State != ConnectionState.Closed)
            _connection.Close();
    }

    public void ExecuteNonQuery(string sqlExecuteQuery)
    {
        try
        {
            OpenConnection();
            using var command = new MySqlCommand(sqlExecuteQuery, _connection);
            command.ExecuteNonQuery();
        }
        finally
        {
            CloseConnection();
        }
    }

    /// <summary>
    /// Đọc dữ liệu. Hãy đóng/dispose reader sau khi dùng để đóng kết nối.
    /// </summary>
    public MySqlDataReader ExecuteReader(string sqlSelect)
    {
        OpenConnection();
        try
        {
            var command = new MySqlCommand(sqlSelect, _connection);
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }
        catch
        {
            CloseConnection();
            throw;
        }
    }

    // Giữ tên cũ trong trường hợp nơi gọi đang dùng cách viết ExcuteReader.
    public MySqlDataReader ExcuteReader(string sqlSelect) => ExecuteReader(sqlSelect);

    public DataTable GetDataTable(string sqlSelect)
    {
        try
        {
            OpenConnection();
            using var adapter = new MySqlDataAdapter(sqlSelect, _connection);
            var table = new DataTable();
            adapter.Fill(table);
            return table;
        }
        finally
        {
            CloseConnection();
        }
    }

    public void Dispose() => _connection.Dispose();
}
