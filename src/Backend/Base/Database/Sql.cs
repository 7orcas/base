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



        //DELETE ME
        //static public async Task<bool> Run(string sqlString, Action<NpgsqlDataReader> action, params NpgsqlParameter[] parameters)
        //{
        //    return await Task.Run(() =>
        //    {

        //        //Thread.Sleep(400);
        //        var connectionString = AppSettings.DBMainConnection;

        //        NpgsqlConnection connection = null;
        //        NpgsqlDataReader reader = null;
        //        try
        //        {
        //            connection = new NpgsqlConnection(connectionString);
        //            connection.Open();
        //            var command = new NpgsqlCommand(sqlString, connection);

        //            if (parameters != null && parameters.Length > 0)
        //                command.Parameters.AddRange(parameters);

        //            reader = command.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                action(reader);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);

        //            var p = "";
        //            for (int i = 0; parameters != null && i < parameters.Length; i++)
        //                p += (p.Length > 0 ? "," : "") + parameters[i].NpgsqlDbType + ":" + parameters[i].Value.ToString();

        //            if (p.Length > 0) p = " p -> (" + p + ")";

        //            Log.Logger.Error(sqlString +
        //                p +
        //                " -> " + ex.Message);
        //            throw;
        //        }
        //        finally
        //        {
        //            if (reader != null) reader.Close();
        //            if (connection != null) connection.Close();
        //        }

        //        return true;
        //    });
        //}

        //static public async Task<bool> Execute(string sqlString)
        //{
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
        //            command.ExecuteNonQuery();
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

        //        return true;
        //    });
        //}

        static public async Task<long> ExecuteAndReturnId(string sqlString)
        {
            //sqlString += "; SELECT SCOPE_IDENTITY();"; //Sql
            sqlString += " RETURNING id;";  //Postgres

            return await Task.Run(() =>
            {

                //Thread.Sleep(400);
                var connectionString = AppSettings.DBMainConnection;

                NpgsqlConnection connection = null;
                try
                {
                    connection = new NpgsqlConnection(connectionString);
                    connection.Open();
                    var command = new NpgsqlCommand(sqlString, connection);
                    object result = command.ExecuteScalar();
                    return Convert.ToInt64(result); // ✅ ID returned
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Log.Logger.Error(sqlString + " -> " + ex.Message);
                    throw;
                }
                finally
                {
                    if (connection != null) connection.Close();
                }
            });
        }

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
