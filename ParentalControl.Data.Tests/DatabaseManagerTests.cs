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
        private Mock<DbSet<ProgramSetting>> programSettingsDbSet;
        private Mock<DbSet<TimeSetting>> timeSettingsDbSet;
        private Mock<DbSet<Keyword>> keywordsDbSet;
        private Mock<DbSet<WebSetting>> webSettingsDbSet;

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
        /// Gets program setting create test cases.
        /// </summary>
        public static IEnumerable<object[]> ProgramSettingCreateTestCases
        {
            get
            {
                yield return new object[] { 1, "Program1", "Path1", false, 0, false, 0, 0, false, default, default };
                yield return new object[] { 2, "Program2", "Path2", true, 30, false, 0, 0, false, default, default };
                yield return new object[] { 3, "Program3", "Path3", true, 30, true, 10, 2, false, default, default };
                yield return new object[] { 4, "Program4", "Path4", false, 0, true, 10, 2, false, default, default };
                yield return new object[] { 5, "Program5", "Path5", false, 0, false, 0, 0, true, TimeSpan.Parse("12:00"), TimeSpan.Parse("13:00") };
                yield return new object[] { 6, "Program6", "Path6", true, 30, false, 0, 0, true, TimeSpan.Parse("12:00"), TimeSpan.Parse("13:00") };
                yield return new object[] { 7, "Program7", "Path7", true, 30, true, 10, 2, true, TimeSpan.Parse("12:00"), TimeSpan.Parse("13:00") };
                yield return new object[] { 8, "Program8", "Path8", false, 0, true, 10, 2, true, TimeSpan.Parse("12:00"), TimeSpan.Parse("13:00") };
            }
        }

        /// <summary>
        /// Gets time setting create test cases.
        /// </summary>
        public static IEnumerable<object[]> TimeSettingCreateTestCases
        {
            get
            {
                yield return new object[] { 1, false, 0, false, default, default };
                yield return new object[] { 2, true, 30, false, default, default };
                yield return new object[] { 3, false, 0, true, TimeSpan.Parse("12:00"), TimeSpan.Parse("13:00") };
                yield return new object[] { 4, true, 30, true, TimeSpan.Parse("12:00"), TimeSpan.Parse("13:00") };
            }
        }

        /// <summary>
        /// Gets web setting create test cases.
        /// </summary>
        public static IEnumerable<object[]> WebSettingCreateTestCases
        {
            get
            {
                yield return new object[] { 1, 2 };
                yield return new object[] { 2, 1 };
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
        /// Gets program setting read delete test cases.
        /// </summary>
        public static IEnumerable<object[]> ProgramSettingReadDeleteTestCases
        {
            get
            {
                Func<ProgramSetting, bool> condition = null;
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
        /// Gets time setting read delete test cases.
        /// </summary>
        public static IEnumerable<object[]> TimeSettingReadDeleteTestCases
        {
            get
            {
                Func<TimeSetting, bool> condition = null;
                yield return new object[] { condition, 4 };
                condition = x => x.Occasional == true;
                yield return new object[] { condition, 2 };
                condition = x => x.Orderly == false;
                yield return new object[] { condition, 2 };
                condition = x => x.FromTime == default && x.ToTime == default;
                yield return new object[] { condition, 2 };
            }
        }

        /// <summary>
        /// Gets web setting read delete test cases.
        /// </summary>
        public static IEnumerable<object[]> WebSettingReadDeleteTestCases
        {
            get
            {
                Func<WebSetting, bool> condition = null;
                yield return new object[] { condition, 2 };
                condition = x => x.UserID == 1;
                yield return new object[] { condition, 1 };
                condition = x => x.KeywordID == 2;
                yield return new object[] { condition, 1 };
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
        /// Gets program setting update test cases.
        /// </summary>
        public static IEnumerable<object[]> ProgramSettingUpdateTestCases
        {
            get
            {
                Action<ProgramSetting> action = x => x.Path = "Path!";
                Func<ProgramSetting, bool> condition = null;
                Func<ProgramSetting, bool> expectation = x => x.Path == "Path!";
                yield return new object[] { action, condition, expectation };
                action = x => x.Path = "Path!";
                condition = x => x.FromTime != default && x.ToTime != default;
                expectation = x => x.Path == "Path!";
                yield return new object[] { action, condition, expectation };
            }
        }

        /// <summary>
        /// Gets time setting update test cases.
        /// </summary>
        public static IEnumerable<object[]> TimeSettingUpdateTestCases
        {
            get
            {
                Action<TimeSetting> action = x => x.Occasional = true;
                Func<TimeSetting, bool> condition = null;
                Func<TimeSetting, bool> expectation = x => x.Occasional == true;
                yield return new object[] { action, condition, expectation };
                action = x => x.Occasional = true;
                condition = x => x.FromTime != default && x.ToTime != default;
                expectation = x => x.Occasional == true;
                yield return new object[] { action, condition, expectation };
            }
        }

        /// <summary>
        /// Gets web setting update test cases.
        /// </summary>
        public static IEnumerable<object[]> WebSettingUpdateTestCases
        {
            get
            {
                Action<WebSetting> action = x => x.KeywordID = 3;
                Func<WebSetting, bool> condition = null;
                Func<WebSetting, bool> expectation = x => x.KeywordID == 3;
                yield return new object[] { action, condition, expectation };
                action = x => x.KeywordID = 3;
                condition = x => x.UserID == 1;
                expectation = x => x.KeywordID == 3;
                yield return new object[] { action, condition, expectation };
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
            this.programSettingsDbSet = CreateMockDbSet<ProgramSetting>();
            this.timeSettingsDbSet = CreateMockDbSet<TimeSetting>();
            this.keywordsDbSet = CreateMockDbSet<Keyword>();
            this.webSettingsDbSet = CreateMockDbSet<WebSetting>();

            this.mockParentalControlEntities = new Mock<ParentalControlEntities>();
            this.mockParentalControlEntities.SetupGet(m => m.Users).Returns(this.usersDbSet.Object);
            this.mockParentalControlEntities.SetupGet(m => m.ProgramSettings).Returns(this.programSettingsDbSet.Object);
            this.mockParentalControlEntities.SetupGet(m => m.TimeSettings).Returns(this.timeSettingsDbSet.Object);
            this.mockParentalControlEntities.SetupGet(m => m.Keywords).Returns(this.keywordsDbSet.Object);
            this.mockParentalControlEntities.SetupGet(m => m.WebSettings).Returns(this.webSettingsDbSet.Object);

            this.databaseManager = Construct<DatabaseManager>();
            this.databaseManager.GetType().GetField("entities", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(this.databaseManager, this.mockParentalControlEntities.Object);
        }

        /// <summary>
        /// Clear DbSets.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            ClearMockDbSet(this.usersDbSet);
            ClearMockDbSet(this.programSettingsDbSet);
            ClearMockDbSet(this.timeSettingsDbSet);
            ClearMockDbSet(this.keywordsDbSet);
            ClearMockDbSet(this.webSettingsDbSet);
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
        /// Create program setting test. It should throw nothing.
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
        [TestCaseSource(nameof(ProgramSettingCreateTestCases))]
        public void CreateProgramSetting_Should_ThrowNothing(int userID, string name, string path, bool occasional, int minutes, bool repeat, int pause, int quantity, bool orderly, TimeSpan fromTime, TimeSpan toTime)
        {
            int countBefore = this.mockParentalControlEntities.Object.ProgramSettings.Count();
            this.databaseManager.Transaction(() => this.databaseManager.CreateProgramSetting(userID, name, path, occasional, minutes, repeat, pause, quantity, orderly, fromTime, toTime));
            int countAfter = this.mockParentalControlEntities.Object.ProgramSettings.Count();
            Assert.That(countBefore + 1 == countAfter);
        }

        /// <summary>
        /// Create time setting test. It should throw nothing.
        /// </summary>
        /// <param name="userID">UserID.</param>
        /// <param name="occasional">Occasional.</param>
        /// <param name="minutes">Minutes.</param>
        /// <param name="orderly">Orderly.</param>
        /// <param name="fromTime">From time.</param>
        /// <param name="toTime">To time.</param>
        [TestCaseSource(nameof(TimeSettingCreateTestCases))]
        public void CreateTimeSetting_Should_ThrowNothing(int userID, bool occasional, int minutes, bool orderly, TimeSpan fromTime, TimeSpan toTime)
        {
            int countBefore = this.mockParentalControlEntities.Object.TimeSettings.Count();
            this.databaseManager.Transaction(() => this.databaseManager.CreateTimeSetting(userID, occasional, minutes, orderly, fromTime, toTime));
            int countAfter = this.mockParentalControlEntities.Object.TimeSettings.Count();
            Assert.That(countBefore + 1 == countAfter);
        }

        /// <summary>
        /// Create web setting test. It should throw nothing.
        /// </summary>
        /// <param name="userID">UserID.</param>
        /// <param name="keywordID">KeywordID.</param>
        [TestCaseSource(nameof(WebSettingCreateTestCases))]
        public void CreateWebSetting_Should_ThrowNothing(int userID, int keywordID)
        {
            int countBefore = this.mockParentalControlEntities.Object.WebSettings.Count();
            this.databaseManager.Transaction(() => this.databaseManager.CreateWebSetting(userID, keywordID));
            int countAfter = this.mockParentalControlEntities.Object.WebSettings.Count();
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
        /// Read program settings test.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <param name="count">Count.</param>
        [TestCaseSource(nameof(ProgramSettingReadDeleteTestCases))]
        public void ReadProgramSettings_Should_ReturnCorrectCount_When_Condition(Func<ProgramSetting, bool> condition, int count)
        {
            this.CreateProgramSettingsHelper();
            Assert.AreEqual(count, this.databaseManager.ReadProgramSettings(condition).Count);
        }

        /// <summary>
        /// Read time settings test.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <param name="count">Count.</param>
        [TestCaseSource(nameof(TimeSettingReadDeleteTestCases))]
        public void ReadTimeSettings_Should_ReturnCorrectCount_When_Condition(Func<TimeSetting, bool> condition, int count)
        {
            this.CreateTimeSettingsHelper();
            Assert.AreEqual(count, this.databaseManager.ReadTimeSettings(condition).Count);
        }

        /// <summary>
        /// Read web settings test.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <param name="count">Count.</param>
        [TestCaseSource(nameof(WebSettingReadDeleteTestCases))]
        public void ReadWebSettings_Should_ReturnCorrectCount_When_Condition(Func<WebSetting, bool> condition, int count)
        {
            this.CreateWebSettingsHelper();
            Assert.AreEqual(count, this.databaseManager.ReadWebSettings(condition).Count);
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
        /// Delete program settings test.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <param name="count">Count.</param>
        [TestCaseSource(nameof(ProgramSettingReadDeleteTestCases))]
        public void DeleteProgramSettings_Should_ThrowNothing(Func<ProgramSetting, bool> condition, int count)
        {
            this.CreateProgramSettingsHelper();
            this.databaseManager.Transaction(() => this.databaseManager.DeleteProgramSettings(condition));
            Assert.AreEqual(0, this.databaseManager.ReadProgramSettings(condition).Count);
        }

        /// <summary>
        /// Delete time settings test.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <param name="count">Count.</param>
        [TestCaseSource(nameof(TimeSettingReadDeleteTestCases))]
        public void DeleteTimeSettings_Should_ThrowNothing(Func<TimeSetting, bool> condition, int count)
        {
            this.CreateTimeSettingsHelper();
            this.databaseManager.Transaction(() => this.databaseManager.DeleteTimeSettings(condition));
            Assert.AreEqual(0, this.databaseManager.ReadTimeSettings(condition).Count);
        }

        /// <summary>
        /// Delete web settings test.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <param name="count">Count.</param>
        [TestCaseSource(nameof(WebSettingReadDeleteTestCases))]
        public void DeleteWebSettings_Should_ThrowNothing(Func<WebSetting, bool> condition, int count)
        {
            this.CreateWebSettingsHelper();
            this.databaseManager.Transaction(() => this.databaseManager.DeleteWebSettings(condition));
            Assert.AreEqual(0, this.databaseManager.ReadWebSettings(condition).Count);
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
        /// Update program settings test.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="condition">Condition.</param>
        /// <param name="expection">Expection.</param>
        [TestCaseSource(nameof(ProgramSettingUpdateTestCases))]
        public void UpdateProgramSettings_Should_Update_When_Condition(Action<ProgramSetting> action, Func<ProgramSetting, bool> condition, Func<ProgramSetting, bool> expection)
        {
            this.CreateProgramSettingsHelper();
            var programSettings = this.databaseManager.ReadProgramSettings(condition);
            this.databaseManager.Transaction(() => this.databaseManager.UpdateProgramSettings(action, condition));
            Assert.IsTrue(programSettings.All(expection));
        }

        /// <summary>
        /// Update time settings test.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="condition">Condition.</param>
        /// <param name="expection">Expection.</param>
        [TestCaseSource(nameof(TimeSettingUpdateTestCases))]
        public void UpdateTimeSettings_Should_Update_When_Condition(Action<TimeSetting> action, Func<TimeSetting, bool> condition, Func<TimeSetting, bool> expection)
        {
            this.CreateTimeSettingsHelper();
            var timeSetting = this.databaseManager.ReadTimeSettings(condition);
            this.databaseManager.Transaction(() => this.databaseManager.UpdateTimeSettings(action, condition));
            Assert.IsTrue(timeSetting.All(expection));
        }

        /// <summary>
        /// Update web settings test.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="condition">Condition.</param>
        /// <param name="expection">Expection.</param>
        [TestCaseSource(nameof(WebSettingUpdateTestCases))]
        public void UpdateWebSettings_Should_Update_When_Condition(Action<WebSetting> action, Func<WebSetting, bool> condition, Func<WebSetting, bool> expection)
        {
            this.CreateWebSettingsHelper();
            var webSetting = this.databaseManager.ReadWebSettings(condition);
            this.databaseManager.Transaction(() => this.databaseManager.UpdateWebSettings(action, condition));
            Assert.IsTrue(webSetting.All(expection));
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
            var keyword = this.databaseManager.ReadKeywords(condition);
            this.databaseManager.Transaction(() => this.databaseManager.UpdateKeywords(action, condition));
            Assert.IsTrue(keyword.All(expection));
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
        /// Update program settings test. It should throw ArgumentNullException.
        /// </summary>
        [Test]
        public void UpdateProgramSettings_Should_ThrowArgumentNullException_When_ActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => this.databaseManager.UpdateProgramSettings(null));
        }

        /// <summary>
        /// Update time settings test. It should throw ArgumentNullException.
        /// </summary>
        [Test]
        public void UpdateTimeSettings_Should_ThrowArgumentNullException_When_ActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => this.databaseManager.UpdateTimeSettings(null));
        }

        /// <summary>
        /// Update web settings test. It should throw ArgumentNullException.
        /// </summary>
        [Test]
        public void UpdateWebSettings_Should_ThrowArgumentNullException_When_ActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => this.databaseManager.UpdateWebSettings(null));
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
            foreach (var userTestCase in UserCreateTestCases)
            {
                this.databaseManager.Transaction(() => this.databaseManager.CreateUser(
                    (string)userTestCase[0],
                    (string)userTestCase[1],
                    (string)userTestCase[2],
                    (string)userTestCase[3]));
            }
        }

        private void CreateProgramSettingsHelper()
        {
            foreach (var programSettingTestCase in ProgramSettingCreateTestCases)
            {
                this.databaseManager.Transaction(() => this.databaseManager.CreateProgramSetting(
                    (int)programSettingTestCase[0],
                    (string)programSettingTestCase[1],
                    (string)programSettingTestCase[2],
                    (bool)programSettingTestCase[3],
                    (int)programSettingTestCase[4],
                    (bool)programSettingTestCase[5],
                    (int)programSettingTestCase[6],
                    (int)programSettingTestCase[7],
                    (bool)programSettingTestCase[8],
                    programSettingTestCase[9] == null ? default : (TimeSpan)programSettingTestCase[9],
                    programSettingTestCase[10] == null ? default : (TimeSpan)programSettingTestCase[10]));
            }
        }

        private void CreateTimeSettingsHelper()
        {
            foreach (var timeSettingTestCase in TimeSettingCreateTestCases)
            {
                this.databaseManager.Transaction(() => this.databaseManager.CreateTimeSetting(
                    (int)timeSettingTestCase[0],
                    (bool)timeSettingTestCase[1],
                    (int)timeSettingTestCase[2],
                    (bool)timeSettingTestCase[3],
                    timeSettingTestCase[4] == null ? default : (TimeSpan)timeSettingTestCase[4],
                    timeSettingTestCase[5] == null ? default : (TimeSpan)timeSettingTestCase[5]));
            }
        }

        private void CreateWebSettingsHelper()
        {
            foreach (var webSettingTestCase in WebSettingCreateTestCases)
            {
                this.databaseManager.Transaction(() => this.databaseManager.CreateWebSetting(
                    (int)webSettingTestCase[0],
                    (int)webSettingTestCase[1]));
            }
        }

        private void CreateKeywordsHelper()
        {
            foreach (var keywordTestCase in KeywordCreateTestCases)
            {
                this.databaseManager.Transaction(() => this.databaseManager.CreateKeyword(
                    (string)keywordTestCase[0]));
            }
        }
    }
}
