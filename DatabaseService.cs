using System.Collections.Generic;
using System.Linq;
using System.Text;

using SQLite;
using PaperTrails_ThomasAdams_c3429938.Models;


namespace PaperTrails_ThomasAdams_c3429938.Services
{
    internal static class DatabaseService
    {
        private static string _databaseFile;

        private static string DatabaseFile
        {
            get
            {
                // Create database directory and file path if not already set
                if (_databaseFile == null)
                {
                    string databaseDir = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "data");
                    System.IO.Directory.CreateDirectory(databaseDir);

                    _databaseFile = Path.Combine(databaseDir, "book_data1.sqlite");
                }
                return _databaseFile;
            }
        }

        private static string _statsDatabaseFile;

        private static string StatsDatabaseFile
        {
            get
            {
                // Create stats database directory and file path if not already set
                if (_statsDatabaseFile == null)
                {
                    string databaseDir = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "data");
                    System.IO.Directory.CreateDirectory(databaseDir);
                    _statsDatabaseFile = Path.Combine(databaseDir, "book_stats1.sqlite");
                }
                return _statsDatabaseFile;
            }
        }

        private static string _readingLocationsDatabaseFile;

        private static string ReadingLocationsDatabaseFile
        {
            get
            {
                // Create reading locations database directory and file path if not already set
                if (_readingLocationsDatabaseFile == null)
                {
                    string databaseDir = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "data");
                    System.IO.Directory.CreateDirectory(databaseDir);
                    _readingLocationsDatabaseFile = Path.Combine(databaseDir, "reading_locations1.sqlite");
                }
                return _readingLocationsDatabaseFile;
            }
        }

        private static SQLiteConnection _connection;

        public static SQLiteConnection Connection
        {
            get
            {
                // Create main database connection and ensure Book table exists
                if (_connection == null)
                {
                    _connection = new SQLiteConnection(DatabaseFile);
                    _connection.CreateTable<Book>();
                }
                return _connection;
            }
        }

        private static SQLiteConnection _statsConnection;

        public static SQLiteConnection StatsConnection
        {
            get
            {
                // Create stats database connection and ensure BookStats table exists
                if (_statsConnection == null)
                {
                    _statsConnection = new SQLiteConnection(StatsDatabaseFile);
                    _statsConnection.CreateTable<BookStats>();
                }
                return _statsConnection;
            }
        }

        private static SQLiteConnection _readingLocationsConnection;

        public static SQLiteConnection ReadingLocationsConnection
        {
            get
            {
                // Create reading locations database connection and ensure ReadingLocation table exists
                if (_readingLocationsConnection == null)
                {
                    _readingLocationsConnection = new SQLiteConnection(ReadingLocationsDatabaseFile);
                    _readingLocationsConnection.CreateTable<ReadingLocation>();
                }
                return _readingLocationsConnection;
            }
        }
    }
}
