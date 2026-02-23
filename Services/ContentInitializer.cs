using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Xml.Linq;

namespace Courses.Services
{
    public class ContentInitializer
    {
        private readonly string _connectionString;
        
        private const string TestDataLecturesFolder = "TestData/Lectures";
        private const string TestDataTestsFolder = "TestData/Tests";
        
        public ContentInitializer(string conn) => _connectionString = conn;

        public void SeedInitialData()
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Courses");
            
            var lecturesPath = Path.Combine(appDataPath, "Lectures");
            var testsPath = Path.Combine(appDataPath, "Tests");

            Directory.CreateDirectory(lecturesPath);
            Directory.CreateDirectory(testsPath);

            CopyTestDataToAppData(TestDataLecturesFolder, lecturesPath);
            CopyTestDataToAppData(TestDataTestsFolder, testsPath);

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var checkCmd = new SqliteCommand("SELECT COUNT(*) FROM Users", connection);
            if ((long)checkCmd.ExecuteScalar() > 0) return;

            using var transaction = connection.BeginTransaction();
            try
            {
                string salt;
                string hash = AuthService.HashPassword("123", out salt);

                var cmd1 = new SqliteCommand(
                    "INSERT INTO Users (Username, Password, Salt, UserType) VALUES (@Username, @Password, @Salt, @UserType)",
                    connection, transaction);
                cmd1.Parameters.AddWithValue("@Username", "AdminTeacher");
                cmd1.Parameters.AddWithValue("@Password", hash);
                cmd1.Parameters.AddWithValue("@Salt", salt);
                cmd1.Parameters.AddWithValue("@UserType", (int)UserType.Teacher);
                cmd1.ExecuteNonQuery();

                var cmd2 = new SqliteCommand("INSERT INTO Courses (CourseName, TeacherId) VALUES ('Штучний інтелект', 1)", connection, transaction);
                cmd2.ExecuteNonQuery();

                var lecture1Path = Path.Combine(lecturesPath, "lecture1.xml");
                var lecture2Path = Path.Combine(lecturesPath, "lecture2.xml");
                var lecture3Path = Path.Combine(lecturesPath, "lecture3.xml");

                var cmd3 = new SqliteCommand(
                    "INSERT INTO Lectures (CourseId, Title, ContentFilePath, AvailableFrom, AvailableUntil) VALUES (1, 'Лекція 1: Вступ до штучного інтелекту', @path, '2026-02-01T00:00:00Z', '2026-03-01T23:59:59Z')",
                    connection, transaction);
                cmd3.Parameters.AddWithValue("@path", lecture1Path);
                cmd3.ExecuteNonQuery();

                var cmd4 = new SqliteCommand(
                    "INSERT INTO Lectures (CourseId, Title, ContentFilePath, AvailableFrom, AvailableUntil) VALUES (1, 'Лекція 2: Машинне навчання', @path, '2026-02-10T00:00:00Z', '2026-03-10T23:59:59Z')",
                    connection, transaction);
                cmd4.Parameters.AddWithValue("@path", lecture2Path);
                cmd4.ExecuteNonQuery();

                var cmd5 = new SqliteCommand(
                    "INSERT INTO Lectures (CourseId, Title, ContentFilePath, AvailableFrom, AvailableUntil) VALUES (1, 'Лекція 3: Нейронні мережі та глибоке навчання', @path, '2026-02-15T00:00:00Z', '2026-03-15T23:59:59Z')",
                    connection, transaction);
                cmd5.Parameters.AddWithValue("@path", lecture3Path);
                cmd5.ExecuteNonQuery();

                var test1Path = Path.Combine(testsPath, "test_lecture1.xml");
                var test2Path = Path.Combine(testsPath, "test_lecture2.xml");
                var test3Path = Path.Combine(testsPath, "test_lecture3.xml");

                var cmdTest1 = new SqliteCommand(
                    "INSERT INTO Tests (CourseId, TestName, ContentFilePath, AvailableFrom, AvailableUntil, IsFinalTest, TestMax) VALUES (1, 'Тест до лекції 1', @path, '2026-02-01T00:00:00Z', '2026-03-01T23:59:59Z', 0, 3)",
                    connection, transaction);
                cmdTest1.Parameters.AddWithValue("@path", test1Path);
                cmdTest1.ExecuteNonQuery();

                var cmdTest2 = new SqliteCommand(
                    "INSERT INTO Tests (CourseId, TestName, ContentFilePath, AvailableFrom, AvailableUntil, IsFinalTest, TestMax) VALUES (1, 'Тест до лекції 2', @path, '2026-02-10T00:00:00Z', '2026-03-10T23:59:59Z', 0, 3)",
                    connection, transaction);
                cmdTest2.Parameters.AddWithValue("@path", test2Path);
                cmdTest2.ExecuteNonQuery();

                var cmdTest3 = new SqliteCommand(
                    "INSERT INTO Tests (CourseId, TestName, ContentFilePath, AvailableFrom, AvailableUntil, IsFinalTest, TestMax) VALUES (1, 'Підсумковий тест', @path, '2026-02-15T00:00:00Z', '2026-03-15T23:59:59Z', 1, 10)",
                    connection, transaction);
                cmdTest3.Parameters.AddWithValue("@path", test3Path);
                cmdTest3.ExecuteNonQuery();

                string salt2;
                string hash2 = AuthService.HashPassword("123", out salt2);
                var cmd6 = new SqliteCommand(
                    "INSERT INTO Users (Username, Password, Salt, UserType) VALUES (@Username, @Password, @Salt, @UserType)",
                    connection, transaction);
                cmd6.Parameters.AddWithValue("@Username", "Student1");
                cmd6.Parameters.AddWithValue("@Password", hash2);
                cmd6.Parameters.AddWithValue("@Salt", salt2);
                cmd6.Parameters.AddWithValue("@UserType", (int)UserType.Student);
                cmd6.ExecuteNonQuery();

                var cmd7 = new SqliteCommand(
                    "INSERT INTO Enrollments (UserId, CourseId, EnrollmentDate) VALUES (2, 1, '2026-02-01T00:00:00Z')",
                    connection, transaction);
                cmd7.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                System.Diagnostics.Debug.WriteLine($"[ContentInitializer] Error seeding data: {ex.Message}");
            }
        }

        private void CopyTestDataToAppData(string sourceFolder, string destinationPath)
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var sourcePath = Path.Combine(baseDir, sourceFolder);

            if (!Directory.Exists(sourcePath))
                return;

            Directory.CreateDirectory(destinationPath);

            foreach (var file in Directory.GetFiles(sourcePath, "*.xml"))
            {
                var fileName = Path.GetFileName(file);
                var destPath = Path.Combine(destinationPath, fileName);
                File.Copy(file, destPath, true);
            }
        }

        private readonly DatabaseService _dbService;

        public ContentInitializer() => _dbService = new DatabaseService();

        public void ImportXmlTest(string xmlFilePath, int courseId)
        {
            XDocument doc = XDocument.Load(xmlFilePath);
            var testTitle = doc.Root.Element("title")?.Value ?? "Тест";

            using var conn = new SqliteConnection(_dbService.ConnectionString);
            conn.Open();

            // 1. Створюємо запис тесту
            var testCmd = new SqliteCommand(
                "INSERT INTO Tests (CourseId, TestName, ContentFilePath) VALUES (@cid, @name, @path); SELECT last_insert_rowid();", conn);
            testCmd.Parameters.AddWithValue("@cid", courseId);
            testCmd.Parameters.AddWithValue("@name", testTitle);
            testCmd.Parameters.AddWithValue("@path", xmlFilePath);
            int testId = Convert.ToInt32(testCmd.ExecuteScalar());

            // 2. Парсимо питання
            foreach (var qElem in doc.Root.Elements("question"))
            {
                var qCmd = new SqliteCommand(
                    "INSERT INTO Questions (TestId, QuestionText) VALUES (@tid, @text); SELECT last_insert_rowid();", conn);
                qCmd.Parameters.AddWithValue("@tid", testId);
                qCmd.Parameters.AddWithValue("@text", qElem.Element("text")?.Value);
                int qId = Convert.ToInt32(qCmd.ExecuteScalar());

                // 3. Парсимо відповіді
                foreach (var optElem in qElem.Element("options").Elements("option"))
                {
                    var optCmd = new SqliteCommand(
                        "INSERT INTO AnswerOptions (QuestionId, AnswerText, Points) VALUES (@qid, @text, @pts)", conn);
                    optCmd.Parameters.AddWithValue("@qid", qId);
                    optCmd.Parameters.AddWithValue("@text", optElem.Value);
                    // Якщо correct="true", даємо 1 бал
                    optCmd.Parameters.AddWithValue("@pts", optElem.Attribute("correct")?.Value == "true" ? 1 : 0);
                    optCmd.ExecuteNonQuery();
                }
            }
        }
    }
}
