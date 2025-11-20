using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using TypingSpeedTester.Models;

namespace TypingSpeedTester.DAL
{
    public class TypingTestDAL
    {
        // Get connection string from Web.config
        private string connectionString = ConfigurationManager.ConnectionStrings["TypingTestDB"].ConnectionString;

        // CREATE: Save a new typing test result
        public bool SaveTestResult(TypingTest test)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO TypingTests 
                                   (UserName, WPM, Accuracy, ErrorCount, TimeDuration, TestText, TypedText) 
                                   VALUES 
                                   (@UserName, @WPM, @Accuracy, @ErrorCount, @TimeDuration, @TestText, @TypedText)";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@UserName", test.UserName);
                    cmd.Parameters.AddWithValue("@WPM", test.WPM);
                    cmd.Parameters.AddWithValue("@Accuracy", test.Accuracy);
                    cmd.Parameters.AddWithValue("@ErrorCount", test.ErrorCount);
                    cmd.Parameters.AddWithValue("@TimeDuration", test.TimeDuration);
                    cmd.Parameters.AddWithValue("@TestText", test.TestText);
                    cmd.Parameters.AddWithValue("@TypedText", test.TypedText);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        // READ: Get all test results
        public List<TypingTest> GetAllResults()
        {
            List<TypingTest> tests = new List<TypingTest>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM TypingTests ORDER BY TestDate DESC";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        tests.Add(new TypingTest
                        {
                            TestId = Convert.ToInt32(reader["TestId"]),
                            UserName = reader["UserName"].ToString(),
                            TestDate = Convert.ToDateTime(reader["TestDate"]),
                            WPM = Convert.ToInt32(reader["WPM"]),
                            Accuracy = Convert.ToDecimal(reader["Accuracy"]),
                            ErrorCount = Convert.ToInt32(reader["ErrorCount"]),
                            TimeDuration = Convert.ToInt32(reader["TimeDuration"]),
                            TestText = reader["TestText"].ToString(),
                            TypedText = reader["TypedText"].ToString()
                        });
                    }
                    reader.Close();
                }
            }
            catch (Exception)
            {
                // Handle error
            }

            return tests;
        }

        // READ: Get a single result by ID
        public TypingTest GetResultById(int id)
        {
            TypingTest test = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM TypingTests WHERE TestId = @TestId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@TestId", id);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        test = new TypingTest
                        {
                            TestId = Convert.ToInt32(reader["TestId"]),
                            UserName = reader["UserName"].ToString(),
                            TestDate = Convert.ToDateTime(reader["TestDate"]),
                            WPM = Convert.ToInt32(reader["WPM"]),
                            Accuracy = Convert.ToDecimal(reader["Accuracy"]),
                            ErrorCount = Convert.ToInt32(reader["ErrorCount"]),
                            TimeDuration = Convert.ToInt32(reader["TimeDuration"]),
                            TestText = reader["TestText"].ToString(),
                            TypedText = reader["TypedText"].ToString()
                        };
                    }
                    reader.Close();
                }
            }
            catch (Exception)
            {
                // Handle error
            }

            return test;
        }

        // DELETE: Remove a test result
        public bool DeleteResult(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM TypingTests WHERE TestId = @TestId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@TestId", id);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Get statistics
        public Dictionary<string, object> GetStatistics(string userName = null)
        {
            var stats = new Dictionary<string, object>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"SELECT 
                                    COUNT(*) as TotalTests,
                                    AVG(CAST(WPM as FLOAT)) as AvgWPM,
                                    MAX(WPM) as BestWPM,
                                    AVG(Accuracy) as AvgAccuracy
                                    FROM TypingTests";

                    if (!string.IsNullOrEmpty(userName))
                    {
                        query += " WHERE UserName = @UserName";
                    }

                    SqlCommand cmd = new SqlCommand(query, conn);
                    if (!string.IsNullOrEmpty(userName))
                    {
                        cmd.Parameters.AddWithValue("@UserName", userName);
                    }

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        stats["TotalTests"] = reader["TotalTests"];
                        stats["AvgWPM"] = reader["AvgWPM"] != DBNull.Value ? Math.Round(Convert.ToDouble(reader["AvgWPM"]), 2) : 0;
                        stats["BestWPM"] = reader["BestWPM"] != DBNull.Value ? reader["BestWPM"] : 0;
                        stats["AvgAccuracy"] = reader["AvgAccuracy"] != DBNull.Value ? Math.Round(Convert.ToDouble(reader["AvgAccuracy"]), 2) : 0;
                    }
                    reader.Close();
                }
            }
            catch (Exception)
            {
                // Handle error
            }

            return stats;
        }
    }
}