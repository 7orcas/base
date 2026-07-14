using Npgsql;
using System.Reflection.Metadata;

/// <summary>
/// Database access
/// Created: March 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Database
{
    public class Sql
    {

        public static async Task Run(
            string sqlString,
            Action<NpgsqlDataReader> action,
            params NpgsqlParameter[] parameters)
        {
            ArgumentNullException.ThrowIfNull(action);

            await using var connection =
                new NpgsqlConnection(AppSettings.DBMainConnection);

            await connection.OpenAsync();

            await using var command =
                new NpgsqlCommand(sqlString, connection);

            if (parameters?.Length > 0)
                command.Parameters.AddRange(parameters);

            await using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                action(reader);
            }
        }

        public static async Task<int> ExecuteAsync(
            string sql,
            params NpgsqlParameter[] parameters)
        {
            try
            {
                await using var connection =
                    new NpgsqlConnection(AppSettings.DBMainConnection);

                await connection.OpenAsync();

                await using var command =
                    new NpgsqlCommand(sql, connection);

                if (parameters?.Length > 0)
                    command.Parameters.AddRange(parameters);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(
                    ex,
                    "Database ExecuteAsync failed. SQL: {Sql}",
                    sql);

                throw;
            }
        }

        public static async Task<int> ExecuteAsync(string sql)
        {
            try
            {
                await using var connection =
                new NpgsqlConnection(AppSettings.DBMainConnection);

                await connection.OpenAsync();

                await using var command =
                    new NpgsqlCommand(sql, connection);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(
                    ex,
                    "Database ExecuteAsync failed. SQL: {Sql}",
                    sql);

                throw;
            }

        }

        public static async Task<long> ExecuteAndReturnIdAsync(string sql)
        {
            sql += " RETURNING id;";

            try
            {
                await using var connection = new NpgsqlConnection(AppSettings.DBMainConnection);
                await connection.OpenAsync();

                await using var command = new NpgsqlCommand(sql, connection);

                var result = await command.ExecuteScalarAsync();

                if (result == null || result == DBNull.Value)
                    throw new InvalidOperationException("No ID was returned from the query.");

                return Convert.ToInt64(result);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Failed to execute SQL: {Sql}", sql);
                throw;
            }
        }

        //static public async Task<long> ExecuteAndReturnId(string sqlString)
        //{
        //    //sqlString += "; SELECT SCOPE_IDENTITY();"; //Sql
        //    sqlString += " RETURNING id;";  //Postgres

        //    return await Task.Run(() =>
        //    {

        //        //Thread.Sleep(400);
        //        var connectionString = AppSettings.DBMainConnection;

        //        NpgsqlConnection connection = null;
        //        try
        //        {
        //            connection = new NpgsqlConnection(connectionString);
        //            connection.Open();
        //            var command = new NpgsqlCommand(sqlString, connection);
        //            object result = command.ExecuteScalar();
        //            return Convert.ToInt64(result); // ✅ ID returned
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //            Log.Logger.Error(sqlString + " -> " + ex.Message);
        //            throw;
        //        }
        //        finally
        //        {
        //            if (connection != null) connection.Close();
        //        }
        //    });
        //}

        static public async Task<long> ExecuteAndReturnId(string sqlString, object parameters)
        {

            if (sqlString.EndsWith(";"))
                sqlString = sqlString.Trim().TrimEnd(';');

            sqlString += " RETURNING id;"; // Postgres

            var connectionString = AppSettings.DBMainConnection;

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(sqlString, connection);

            //Add parameters dynamically
            foreach (var prop in parameters.GetType().GetProperties())
            {
                var value = prop.GetValue(parameters) ?? DBNull.Value;
                command.Parameters.AddWithValue(prop.Name, value);
            }

            try
            {
                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt64(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Logger.Error(sqlString + " -> " + ex.Message);
                throw;
            }
        }

    }
}
