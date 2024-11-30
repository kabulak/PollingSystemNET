using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

public static class DatabaseHelper
{
    // Connection string to the SQLite database
    private static readonly string ConnectionString = "Data Source=Database/PollingSystem.db;Version=3;";

    static DatabaseHelper()
    {
        var databaseFolder = "Database";

        // Check if the database folder exists; if not, create it
        if (!Directory.Exists(databaseFolder))
        {
            Directory.CreateDirectory(databaseFolder);
        }

        // Initialize the database (create tables if not already created)
        InitializeDatabase();
    }

    private static void InitializeDatabase()
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();

            // Create Polls table if it doesn't exist
            string createPollsTable = @"
                CREATE TABLE IF NOT EXISTS Polls (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Question TEXT NOT NULL
                );";

            // Create Choices table if it doesn't exist
            string createChoicesTable = @"
                CREATE TABLE IF NOT EXISTS Choices (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PollId INTEGER NOT NULL,
                    Choice TEXT NOT NULL,
                    Votes INTEGER DEFAULT 0,
                    FOREIGN KEY (PollId) REFERENCES Polls(Id)
                );";

            using (var command = new SQLiteCommand(createPollsTable, connection))
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SQLiteCommand(createChoicesTable, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    // Example: Method to create a poll
    public static int CreatePoll(string question, List<string> choices)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                var insertPoll = "INSERT INTO Polls (Question) VALUES (@Question);";
                using (var command = new SQLiteCommand(insertPoll, connection))
                {
                    command.Parameters.AddWithValue("@Question", question);
                    command.ExecuteNonQuery();
                }

                var pollId = (int)connection.LastInsertRowId;

                var insertChoice = "INSERT INTO Choices (PollId, Choice) VALUES (@PollId, @Choice);";
                foreach (var choice in choices)
                {
                    using (var command = new SQLiteCommand(insertChoice, connection))
                    {
                        command.Parameters.AddWithValue("@PollId", pollId);
                        command.Parameters.AddWithValue("@Choice", choice);
                        command.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
                return pollId;
            }
        }
    }

    // Example: Method to get all polls
    public static List<string> GetPolls()
    {
        var polls = new List<string>();
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            var query = "SELECT Question FROM Polls;";
            using (var command = new SQLiteCommand(query, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    polls.Add(reader.GetString(0));
                }
            }
        }
        return polls;
    }

    // Other database interaction methods like GetChoices, Vote, etc.
}
