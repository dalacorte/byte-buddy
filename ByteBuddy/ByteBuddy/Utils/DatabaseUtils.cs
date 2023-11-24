using ByteBuddy.Entities;
using Dapper;
using System.Data.SQLite;

namespace ByteBuddy.Utils
{
    public class DatabaseUtils
    {
        private static string GetDb() => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Database\bytebuddy.sqlite");

        private static string GetConnectionString()
        {
            string db = GetDb();

            return $"Data Source={db};Version=3;";
        }

        public static void CreateDatabase()
        {
            try
            {
                string db = GetDb();

                if (!File.Exists(db))
                    SQLiteConnection.CreateFile(db);

                DebugLogger.Debug("Database created");
            }
            catch
            {
                throw;
            }
        }

        public static void CreateTable()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(GetConnectionString()))
                {
                    connection.Open();

                    string createTableSql = "CREATE TABLE IF NOT EXISTS Buddies(" +
                                            "Id VARCHAR(32), " +
                                            "Name VARCHAR(50), " +
                                            "Type VARCHAR(4), " +
                                            "Path VARCHAR, " +
                                            "ThumbnailPath VARCHAR, " +
                                            "Width INT, " +
                                            "Height INT, " +
                                            "Frames INT, " +
                                            "Bytes BLOB)";
                    DebugLogger.Debug(createTableSql);
                    connection.Execute(createTableSql);
                }

                DebugLogger.Debug("Table buddies created");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void InitialPayload()
        {
            try
            {
                string resources = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources");

                DirectoryInfo d = new DirectoryInfo(resources);

                string fileExtension = ".png";
                FileInfo[] files = d.GetFiles($"*{fileExtension}");

                using (SQLiteConnection connection = new SQLiteConnection(GetConnectionString()))
                {
                    foreach (FileInfo file in files)
                    {
                        Buddy buddy = new Buddy(id: Guid.NewGuid().ToString("N").ToUpper(),
                                                name: file.Name.Replace(fileExtension, ""),
                                                type: fileExtension,
                                                path: file.FullName,
                                                thumbnailPath: string.Empty,
                                                width: 11520,
                                                height: 192,
                                                frames: 60,
                                                bytes: File.ReadAllBytes(file.FullName));

                        AddBuddy(buddy);
                    }
                }

                DebugLogger.Debug("Initial payload loaded");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static T Get<T>(string sql)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(GetConnectionString()))
                {
                    connection.Open();

                    DebugLogger.Debug(sql);

                    return connection.Query<T>(sql).First();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<T> GetAll<T>(string sql)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(GetConnectionString()))
                {
                    connection.Open();

                    DebugLogger.Debug(sql);

                    return connection.Query<T>(sql).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void AddBuddy(Buddy buddy)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(GetConnectionString()))
                {
                    connection.Open();

                    string insertSql = @"INSERT INTO Buddies 
                                 (Id, Name, Type, Path, ThumbnailPath, Width, Height, Frames, Bytes) 
                                 VALUES 
                                 (@Id, @Name, @Type, @Path, @ThumbnailPath, @Width, @Height, @Frames, @Bytes)";

                    DebugLogger.Debug(insertSql);

                    connection.Execute(insertSql, buddy);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void UpdateBuddy(Buddy buddy)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(GetConnectionString()))
                {
                    connection.Open();

                    string updateSql = @"UPDATE Buddies 
                                 SET 
                                     Name = @Name, 
                                     Type = @Type, 
                                     Path = @Path, 
                                     ThumbnailPath = @ThumbnailPath, 
                                     Width = @Width, 
                                     Height = @Height, 
                                     Frames = @Frames, 
                                     Bytes = @Bytes 
                                 WHERE Id = @Id";

                    DebugLogger.Debug(updateSql);

                    connection.Execute(updateSql, buddy);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void DeleteBuddy(string id)
        {
            using (SQLiteConnection connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();

                string deleteSql = "DELETE FROM Buddies WHERE Id = @Id";

                DebugLogger.Debug(deleteSql);

                connection.Execute(deleteSql, new { Id = id });
            }
        }
    }
}
