// <copyright file="DatabaseManagerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using EntityFramework.Testing;
    using Moq;
    using NUnit.Framework;
    using ParentalControl.Data.Database;

    /// <summary>
    /// DatabaseManagerTests class.
    /// </summary>
    [TestFixture]
    public class DatabaseManagerTests
    {
        private DatabaseManager databaseManager;
        private Mock<ParentalControlEntities> mockParentalControlEntities;
        private Mock<DbSet<User>> usersDbSet;
        private Mock<DbSet<ProgramLimitation>> programLimitationsDbSet;
        private Mock<DbSet<Keyword>> keywordsDbSet;
        private Mock<DbSet<WebLimitation>> webLimitationsDbSet;

        /// <summary>
        /// Gets user create test cases.
        /// </summary>
        public static IEnumerable<object[]> UserCreateTestCases
        {
            get
            {
                yield return new object[] { "username1", "password1", "?", "!" };
                yield return new object[] { "username2", "password2", "?", "!" };
            }
        }

        /// <summary>
        /// Gets user read delete test cases.
        /// </summary>
        public static IEnumerable<object[]> UserReadDeleteTestCases
        {
            get
            {
                Func<User, bool> condition = null;
                yield return new object[] { condition, 2 };
                condition = x => x.Username == "username1";
                yield return new object[] { condition, 1 };
                condition = x => x.SecurityQuestion == "?";
                yield return new object[] { condition, 2 };
            }
        }

        /// <summary>
        /// Gets user update test cases.
        /// </summary>
        public static IEnumerable<object[]> UserUpdateTestCases
        {
            get
            {
                Action<User> action = x => x.SecurityQuestion = "?!";
                Func<User, bool> condition = null;
                Func<User, bool> expectation = x => x.SecurityQuestion == "?!";
                yield return new object[] { action, condition, expectation };
                action = x => x.SecurityQuestion = "?!";
                condition = x => x.Username == "username1";
                expectation = x => x.SecurityQuestion == "?!";
                yield return new object[] { action, condition, expectation };
            }
        }

        /// <summary>
        /// Gets program limitation create test cases.
        /// </summary>
        public static IEnumerable<object[]> ProgramLimitationCreateTestCases
        {
            get
            {
                yield return new object[] { 1, "Program1", "Path1", false, 0, false, 0, 0, false, default, default };
                yield return new object[] { 2, "Program2", "Path2", true, 30, false, 0, 0, false, default, default };
                yield return new object[] { 1, "Program3", "Path3", true, 30, true, 10, 2, false, default, default };
                yield return new object[] { 2, "Program4", "Path4", false, 0, true, 10, 2, false, default, default };
                yield return new object[] { 1, "Program5", "Path5", false, 0, false, 0, 0, true, TimeSpan.Parse("12:00"), TimeSpan.Parse("13:00") };
                yield return new object[] { 2, "Program6", "Path6", true, 30, false, 0, 0, true, TimeSpan.Parse("12:00"), TimeSpan.Parse("13:00") };
                yield return new object[] { 1, "Program7", "Path7", true, 30, true, 10, 2, true, TimeSpan.Parse("12:00"), TimeSpan.Parse("13:00") };
                yield return new object[] { 2, "Program8", "Path8", false, 0, true, 10, 2, true, TimeSpan.Parse("12:00"), TimeSpan.Parse("13:00") };
            }
        }

        /// <summary>
        /// Gets program limitation read delete test cases.
        /// </summary>
        public static IEnumerable<object[]> ProgramLimitationReadDeleteTestCases
        {
            get
            {
                Func<ProgramLimitation, bool> condition = null;
                yield return new object[] { condition, 8 };
                condition = x => x.Orderly == true;
                yield return new object[] { condition, 4 };
                condition = x => x.Repeat == false;
                yield return new object[] { condition, 4 };
                condition = x => x.Pause == 0;
                yield return new object[] { condition, 4 };
            }
        }

        /// <summary>
        /// Gets program limitation update test cases.
        /// </summary>
        public static IEnumerable<object[]> ProgramLimitationUpdateTestCases
        {
            get
            {
                Action<ProgramLimitation> action = x => x.Path = "Path!";
                Func<ProgramLimitation, bool> condition = null;
                Func<ProgramLimitation, bool> expectation = x => x.Path == "Path!";
                yield return new object[] { action, condition, expectation };
                action = x => x.Path = "Path!";
                condition = x => x.FromTime != default && x.ToTime != default;
                expectation = x => x.Path == "Path!";
                yield return new object[] { action, condition, expectation };
            }
        }

        /// <summary>
        /// Gets time limitation create test cases.
        /// </summary>
        public static IEnumerable<object[]> TimeLimitationCreateTestCases
        {
            get
            {
                yield return new object[] { 1, false, 0, false, default, default };
                yield return new object[] { 2, true, 30, true, TimeSpan.Parse("12:00"), TimeSpan.Parse("13:00") };
            }
        }

        /// <summary>
        /// Gets time limitation read test cases.
        /// </summary>
        public static IEnumerable<object[]> TimeLimitationReadTestCases
        {
            get
            {
                Func<User, bool> condition = null;
                yield return new object[] { condition, 2 };
                condition = x => x.Occasional == true;
                yield return new object[] { condition, 1 };
                condition = x => x.Orderly == false;
                yield return new object[] { condition, 1 };
                condition = x => x.FromTime == default && x.ToTime == default;
                yield return new object[] { condition, 1 };
            }
        }

        /// <summary>
        /// Gets time limitation update test cases.
        /// </summary>
        public static IEnumerable<object[]> TimeLimitationUpdateTestCases
        {
            get
            {
                Action<User> action = x => x.Occasional = true;
                Func<User, bool> condition = null;
                Func<User, bool> expectation = x => x.Occasional == true;
                yield return new object[] { action, condition, expectation };
                action = x => x.Occasional = true;
                condition = x => x.FromTime != default && x.ToTime != default;
                expectation = x => x.Occasional == true;
                yield return new object[] { action, condition, expectation };
            }
        }

        /// <summary>
        /// Gets web limitation create test cases.
        /// </summary>
        public static IEnumerable<object[]> WebLimitationCreateTestCases
        {
            get
            {
                yield return new object[] { 1, 2 };
                yield return new object[] { 2, 1 };
            }
        }

        /// <summary>
        /// Gets web limitation read delete test cases.
        /// </summary>
        public static IEnumerable<object[]> WebLimitationReadDeleteTestCases
        {
            get
            {
                Func<WebLimitation, bool> condition = null;
                yield return new object[] { condition, 2 };
                condition = x => x.UserID == 1;
                yield return new object[] { condition, 1 };
                condition = x => x.KeywordID == 2;
                yield return new object[] { condition, 1 };
            }
        }

        /// <summary>
        /// Gets keyword create test cases.
        /// </summary>
        public static IEnumerable<object[]> KeywordCreateTestCases
        {
            get
            {
                yield return new object[] { "Keyword1" };
                yield return new object[] { "Keyword2" };
            }
        }

        /// <summary>
        /// Gets keyword read delete test cases.
        /// </summary>
        public static IEnumerable<object[]> KeywordReadDeleteTestCases
        {
            get
            {
                Func<Keyword, bool> condition = null;
                yield return new object[] { condition, 2 };
                condition = x => x.Name == "Keyword2";
                yield return new object[] { condition, 1 };
            }
        }

        /// <summary>
        /// Gets keyword update test cases.
        /// </summary>
        public static IEnumerable<object[]> KeywordUpdateTestCases
        {
            get
            {
                Action<Keyword> action = x => x.Name = "Keyword!";
                Func<Keyword, bool> condition = null;
                Func<Keyword, bool> expectation = x => x.Name == "Keyword!";
                yield return new object[] { action, condition, expectation };
                action = x => x.Name = "Keyword!";
                condition = x => x.Name == "Keyword2";
                expectation = x => x.Name == "Keyword!";
                yield return new object[] { action, condition, expectation };
            }
        }

        /// <summary>
        /// Initialize tests.
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.usersDbSet = CreateMockDbSet<User>();
            this.programLimitationsDbSet = CreateMockDbSet<ProgramLimitation>();
            this.keywordsDbSet = CreateMockDbSet<Keyword>();
            this.webLimitationsDbSet = CreateMockDbSet<WebLimitation>();

            this.mockParentalControlEntities = new Mock<ParentalControlEntities>();
            this.mockParentalControlEntities.SetupGet(m => m.Users).Returns(this.usersDbSet.Object);
            this.mockParentalControlEntities.SetupGet(m => m.ProgramLimitations).Returns(this.programLimitationsDbSet.Object);
            this.mockParentalControlEntities.SetupGet(m => m.Keywords).Returns(this.keywordsDbSet.Object);
            this.mockParentalControlEntities.SetupGet(m => m.WebLimitations).Returns(this.webLimitationsDbSet.Object);

            this.databaseManager = Construct<DatabaseManager>();
            this.databaseManager.GetType().GetField("entities", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this.databaseManager, this.mockParentalControlEntities.Object);
        }

        /// <summary>
        /// Clear DbSets.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            ClearMockDbSet(this.usersDbSet);
            ClearMockDbSet(this.programLimitationsDbSet);
            ClearMockDbSet(this.keywordsDbSet);
            ClearMockDbSet(this.webLimitationsDbSet);
        }

        /// <summary>
        /// Create user test. It should throw nothing.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="securityQuestion">Security question.</param>
        /// <param name="securityAnswer">Security answer.</param>
        [TestCaseSource(nameof(UserCreateTestCases))]
        public void CreateUser_Should_ThrowNothing(string username, string password, string securityQuestion, string securityAnswer)
        {
            int countBefore = this.mockParentalControlEntities.Object.Users.Count();
            this.databaseManager.Transaction(() => this.databaseManager.CreateUser(username, password, securityQuestion, securityAnswer));
            int countAfter = this.mockParentalControlEntities.Object.Users.Count();
            Assert.That(countBefore + 1 == countAfter);
        }

        /// <summary>
        /// Create program limitation test. It should throw nothing.
        /// </summary>
        /// <param name="userID">UserID.</param>
        /// <param name="name">Name.</param>
        /// <param name="path">Path.</param>
        /// <param name="occasional">Occasional.</param>
        /// <param name="minutes">Minutes.</param>
        /// <param name="repeat">Repeat.</param>
        /// <param name="pause">Puase.</param>
        /// <param name="quantity">Quantity.</param>
        /// <param name="orderly">Orderly.</param>
        /// <param name="fromTime">From time.</param>
        /// <param name="toTime">To time.</param>
        [TestCaseSource(nameof(ProgramLimitationCreateTestCases))]
        public void CreateProgramLimitation_Should_ThrowNothing(int userID, string name, string path, bool occasional, int minutes, bool repeat, int pause, int quantity, bool orderly, TimeSpan fromTime, TimeSpan toTime)
        {
            this.CreateUsersHelper();
            int countBefore = this.mockParentalControlEntities.Object.ProgramLimitations.Count();
            this.databaseManager.Transaction(() => this.databaseManager.CreateProgramLimitation(userID, name, path, occasional, minutes, repeat, pause, quantity, orderly, fromTime, toTime));
            int countAfter = this.mockParentalControlEntities.Object.ProgramLimitations.Count();
            Assert.That(countBefore + 1 == countAfter);
        }

        /// <summary>
        /// Create web limitation test. It should throw nothing.
        /// </summary>
        /// <param name="userID">UserID.</param>
        /// <param name="keywordID">KeywordID.</param>
        [TestCaseSource(nameof(WebLimitationCreateTestCases))]
        public void CreateWebLimitation_Should_ThrowNothing(int userID, int keywordID)
        {
            this.CreateUsersHelper();
            this.CreateKeywordsHelper();
            int countBefore = this.mockParentalControlEntities.Object.WebLimitations.Count();
            this.databaseManager.Transaction(() => this.databaseManager.CreateWebLimitation(userID, keywordID));
            int countAfter = this.mockParentalControlEntities.Object.WebLimitations.Count();
            Assert.That(countBefore + 1 == countAfter);
        }

        /// <summary>
        /// Create keyword test. It should throw nothing.
        /// </summary>
        /// <param name="name">Name.</param>
        [TestCaseSource(nameof(KeywordCreateTestCases))]
        public void CreateKeyword_Should_ThrowNothing(string name)
        {
            int countBefore = this.mockParentalControlEntities.Object.Keywords.Count();
            this.databaseManager.Transaction(() => this.databaseManager.CreateKeyword(name));
            int countAfter = this.mockParentalControlEntities.Object.Keywords.Count();
            Assert.That(countBefore + 1 == countAfter);
        }

        /// <summary>
        /// Read users test.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <param name="count">Count.</param>
        [TestCaseSource(nameof(UserReadDeleteTestCases))]
        public void ReadUsers_Should_ReturnCorrectCount_When_Condition(Func<User, bool> condition, int count)
        {
            this.CreateUsersHelper();
            Assert.AreEqual(count, this.databaseManager.ReadUsers(condition).Count);
        }

        /// <summary>
        /// Read program limitation test.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <param name="count">Count.</param>
        [TestCaseSource(nameof(ProgramLimitationReadDeleteTestCases))]
        public void ReadProgramLimitations_Should_ReturnCorrectCount_When_Condition(Func<ProgramLimitation, bool> condition, int count)
        {
            this.CreateProgramLimitationsHelper();
            Assert.AreEqual(count, this.databaseManager.ReadProgramLimitations(condition).Count);
        }

        /// <summary>
        /// Read time limitations test.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <param name="count">Count.</param>
        [TestCaseSource(nameof(TimeLimitationReadTestCases))]
        public void ReadTimeLimitations_Should_ReturnCorrectCount_When_Condition(Func<User, bool> condition, int count)
        {
            this.CreateTimeLimitationsHelper();
            Assert.AreEqual(count, this.databaseManager.ReadUsers(condition).Count);
        }

        /// <summary>
        /// Read web limitation test.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <param name="count">Count.</param>
        [TestCaseSource(nameof(WebLimitationReadDeleteTestCases))]
        public void ReadWebLimitations_Should_ReturnCorrectCount_When_Condition(Func<WebLimitation, bool> condition, int count)
        {
            this.CreateWebLimitationsHelper();
            Assert.AreEqual(count, this.databaseManager.ReadWebLimitations(condition).Count);
        }

        /// <summary>
        /// Read keywords test.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <param name="count">Count.</param>
        [TestCaseSource(nameof(KeywordReadDeleteTestCases))]
        public void ReadKeywords_Should_ReturnCorrectCount_When_Condition(Func<Keyword, bool> condition, int count)
        {
            this.CreateKeywordsHelper();
            Assert.AreEqual(count, this.databaseManager.ReadKeywords(condition).Count);
        }

        /// <summary>
        /// Delete users test.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <param name="count">Count.</param>
        [TestCaseSource(nameof(UserReadDeleteTestCases))]
        public void DeleteUsers_Should_ThrowNothing(Func<User, bool> condition, int count)
        {
            this.CreateUsersHelper();
            this.databaseManager.Transaction(() => this.databaseManager.DeleteUsers(condition));
            Assert.AreEqual(0, this.databaseManager.ReadUsers(condition).Count);
        }

        /// <summary>
        /// Delete program limitations test.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <param name="count">Count.</param>
        [TestCaseSource(nameof(ProgramLimitationReadDeleteTestCases))]
        public void DeleteProgramLimitations_Should_ThrowNothing(Func<ProgramLimitation, bool> condition, int count)
        {
            this.CreateProgramLimitationsHelper();
            this.databaseManager.Transaction(() => this.databaseManager.DeleteProgramLimitations(condition));
            Assert.AreEqual(0, this.databaseManager.ReadProgramLimitations(condition).Count);
        }

        /// <summary>
        /// Delete web limitations test.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <param name="count">Count.</param>
        [TestCaseSource(nameof(WebLimitationReadDeleteTestCases))]
        public void DeleteWebLimitations_Should_ThrowNothing(Func<WebLimitation, bool> condition, int count)
        {
            this.CreateWebLimitationsHelper();
            this.databaseManager.Transaction(() => this.databaseManager.DeleteWebLimitations(condition));
            Assert.AreEqual(0, this.databaseManager.ReadWebLimitations(condition).Count);
        }

        /// <summary>
        /// Delete keywords test.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <param name="count">Count.</param>
        [TestCaseSource(nameof(KeywordReadDeleteTestCases))]
        public void DeleteKeywords_Should_ThrowNothing(Func<Keyword, bool> condition, int count)
        {
            this.CreateKeywordsHelper();
            this.databaseManager.Transaction(() => this.databaseManager.DeleteKeywords(condition));
            Assert.AreEqual(0, this.databaseManager.ReadKeywords(condition).Count);
        }

        /// <summary>
        /// Update users test.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="condition">Condition.</param>
        /// <param name="expection">Expection.</param>
        [TestCaseSource(nameof(UserUpdateTestCases))]
        public void UpdateUsers_Should_Update_When_Condition(Action<User> action, Func<User, bool> condition, Func<User, bool> expection)
        {
            this.CreateUsersHelper();
            var users = this.databaseManager.ReadUsers(condition);
            this.databaseManager.Transaction(() => this.databaseManager.UpdateUsers(action, condition));
            Assert.IsTrue(users.All(expection));
        }

        /// <summary>
        /// Update users test. It should throw ArgumentNullException.
        /// </summary>
        [Test]
        public void UpdateUsers_Should_ThrowArgumentNullException_When_ActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => this.databaseManager.UpdateUsers(null));
        }

        /// <summary>
        /// Update program limitations test.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="condition">Condition.</param>
        /// <param name="expection">Expection.</param>
        [TestCaseSource(nameof(ProgramLimitationUpdateTestCases))]
        public void UpdateProgramLimitations_Should_Update_When_Condition(Action<ProgramLimitation> action, Func<ProgramLimitation, bool> condition, Func<ProgramLimitation, bool> expection)
        {
            this.CreateProgramLimitationsHelper();
            var programLimitations = this.databaseManager.ReadProgramLimitations(condition);
            this.databaseManager.Transaction(() => this.databaseManager.UpdateProgramLimitations(action, condition));
            Assert.IsTrue(programLimitations.All(expection));
        }

        /// <summary>
        /// Update program limitations test. It should throw ArgumentNullException.
        /// </summary>
        [Test]
        public void UpdateProgramLimitations_Should_ThrowArgumentNullException_When_ActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => this.databaseManager.UpdateProgramLimitations(null));
        }

        /// <summary>
        /// Update time limitations test.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="condition">Condition.</param>
        /// <param name="expection">Expection.</param>
        [TestCaseSource(nameof(TimeLimitationUpdateTestCases))]
        public void UpdateTimeLimitations_Should_Update_When_Condition(Action<User> action, Func<User, bool> condition, Func<User, bool> expection)
        {
            this.CreateTimeLimitationsHelper();
            var timeLimitations = this.databaseManager.ReadUsers(condition);
            this.databaseManager.Transaction(() => this.databaseManager.UpdateUsers(action, condition));
            Assert.IsTrue(timeLimitations.All(expection));
        }

        /// <summary>
        /// Update time limitations test. It should throw ArgumentNullException.
        /// </summary>
        [Test]
        public void UpdateTimeLimitations_Should_ThrowArgumentNullException_When_ActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => this.databaseManager.UpdateUsers(null));
        }

        /// <summary>
        /// Update keywords test.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="condition">Condition.</param>
        /// <param name="expection">Expection.</param>
        [TestCaseSource(nameof(KeywordUpdateTestCases))]
        public void UpdateKeywords_Should_Update_When_Condition(Action<Keyword> action, Func<Keyword, bool> condition, Func<Keyword, bool> expection)
        {
            this.CreateKeywordsHelper();
            var keywords = this.databaseManager.ReadKeywords(condition);
            this.databaseManager.Transaction(() => this.databaseManager.UpdateKeywords(action, condition));
            Assert.IsTrue(keywords.All(expection));
        }

        /// <summary>
        /// Update keywords test. It should throw ArgumentNullException.
        /// </summary>
        [Test]
        public void UpdateKeywords_Should_ThrowArgumentNullException_When_ActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => this.databaseManager.UpdateKeywords(null));
        }

        private static T Construct<T>(Type[] paramTypes = null, object[] paramValues = null)
        {
            if (paramTypes == null)
            {
                paramTypes = new Type[] { };
            }

            if (paramValues == null)
            {
                paramValues = new object[] { };
            }

            Type t = typeof(T);
            ConstructorInfo ci = t.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, paramTypes, null);
            return (T)ci.Invoke(paramValues);
        }

        private static Mock<DbSet<T>> CreateMockDbSet<T>()
            where T : class
        {
            var mockList = new List<T>();
            return new Mock<DbSet<T>>().SetupData(mockList);
        }

        private static void ClearMockDbSet<T>(Mock<DbSet<T>> mockObject)
            where T : class
        {
            var mockList = new List<T>();
            mockObject.SetupData(mockList);
        }

        private void CreateUsersHelper()
        {
            int count = 1;
            foreach (var userTestCase in UserCreateTestCases)
            {
                this.databaseManager.Transaction(() => this.databaseManager.CreateUser(
                    (string)userTestCase[0],
                    (string)userTestCase[1],
                    (string)userTestCase[2],
                    (string)userTestCase[3]));
                this.databaseManager.Transaction(() => this.databaseManager.UpdateUsers(x => x.ID = count++, x => x.Username == (string)userTestCase[0]));
            }
        }

        private void CreateProgramLimitationsHelper()
        {
            this.CreateUsersHelper();
            foreach (var programLimitationTestCase in ProgramLimitationCreateTestCases)
            {
                this.databaseManager.Transaction(() => this.databaseManager.CreateProgramLimitation(
                    (int)programLimitationTestCase[0],
                    (string)programLimitationTestCase[1],
                    (string)programLimitationTestCase[2],
                    (bool)programLimitationTestCase[3],
                    (int)programLimitationTestCase[4],
                    (bool)programLimitationTestCase[5],
                    (int)programLimitationTestCase[6],
                    (int)programLimitationTestCase[7],
                    (bool)programLimitationTestCase[8],
                    programLimitationTestCase[9] == null ? default : (TimeSpan)programLimitationTestCase[9],
                    programLimitationTestCase[10] == null ? default : (TimeSpan)programLimitationTestCase[10]));
            }
        }

        private void CreateTimeLimitationsHelper()
        {
            this.CreateUsersHelper();
            foreach (var timeLimitationTestCase in TimeLimitationCreateTestCases)
            {
                this.databaseManager.UpdateUsers(
                    x =>
                    {
                        x.IsTimeLimitationActive = true;
                        x.Occasional = (bool)timeLimitationTestCase[1];
                        x.Minutes = (int)timeLimitationTestCase[2];
                        x.Orderly = (bool)timeLimitationTestCase[3];
                        x.FromTime = timeLimitationTestCase[4] == null ? default : (TimeSpan)timeLimitationTestCase[4];
                        x.ToTime = timeLimitationTestCase[5] == null ? default : (TimeSpan)timeLimitationTestCase[5];
                    },
                    x => x.ID == (int)timeLimitationTestCase[0]);
            }
        }

        private void CreateWebLimitationsHelper()
        {
            this.CreateUsersHelper();
            this.CreateKeywordsHelper();
            foreach (var webLimitationTestCase in WebLimitationCreateTestCases)
            {
                this.databaseManager.Transaction(() => this.databaseManager.CreateWebLimitation(
                    (int)webLimitationTestCase[0],
                    (int)webLimitationTestCase[1]));
            }
        }

        private void CreateKeywordsHelper()
        {
            int count = 1;
            foreach (var keywordTestCase in KeywordCreateTestCases)
            {
                this.databaseManager.Transaction(() => this.databaseManager.CreateKeyword(
                    (string)keywordTestCase[0]));
                this.databaseManager.Transaction(() => this.databaseManager.UpdateKeywords(x => x.ID = count++, x => x.Name == (string)keywordTestCase[0]));
            }
        }
    }
}
