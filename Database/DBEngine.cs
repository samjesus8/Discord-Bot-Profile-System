using Npgsql;
using System;
using System.Threading.Tasks;

namespace DiscordBotTemplate.Database
{
    public class DBEngine
    {
        private string connectionString = "Host=bq02ribpagyqsk9xeylx-postgresql.services.clever-cloud.com;Username=u0ravvj9cjq0eqzmw7ag;Password=Cla7Zd1Etu5d5BfPPfYwqKQNrGQRlF;Database=bq02ribpagyqsk9xeylx";

        public async Task<bool> StoreUserAsync(DUser user)
        {
            var totalUsers = await GetTotalUsersAsync();
            if (totalUsers.Item1 != true)
            {
                throw new Exception();
            }
            else
            {
                totalUsers.Item2++;
            }

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "INSERT INTO data.userinfo (userno, username, serverid, avatarurl, level, xp, xplimit)" +
                                   $"VALUES ('{totalUsers.Item2}', '{user.UserName}', '{user.GuildID}', '{user.AvatarURL}', '{user.Level}', '{user.XP}', '{user.XPLimit}')";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public async Task<bool> CheckUserExistsAsync(string username, ulong serverID)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = $"SELECT EXISTS (SELECT 1 FROM data.userinfo WHERE username = '{username}' AND serverid = {serverID} LIMIT 1);";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        bool doesExist = (bool)await cmd.ExecuteScalarAsync();

                        if (doesExist == true)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public async Task<(bool, DUser)> GetUserAsync(string username, ulong serverID)
        {
            DUser result;

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT u.username, u.serverid, u.avatarurl, u.level, u.xp, u.xplimit " +
                                   "FROM data.userinfo u " +
                                   $"WHERE username = '{username}' AND serverid = {serverID}";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var reader = await cmd.ExecuteReaderAsync();
                        await reader.ReadAsync();


                        result = new DUser
                        {
                            UserName = reader.GetString(0),
                            GuildID = (ulong)reader.GetInt64(1),
                            AvatarURL = reader.GetString(2),
                            Level = reader.GetInt32(3),
                            XP = reader.GetInt32(4),
                            XPLimit = reader.GetInt32(5),
                        };
                    }
                }

                return (true, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (false, null);
            }
        }

        private async Task<(bool, long)> GetTotalUsersAsync()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT COUNT (*) FROM data.userinfo";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var userCount = await cmd.ExecuteScalarAsync();
                        return (true, Convert.ToInt64(userCount));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (false, -1);
            }
        }
    }
}
