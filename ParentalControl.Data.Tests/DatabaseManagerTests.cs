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

    [TestFixture]
    public class DatabaseManagerTests
    {
        private DatabaseManager databaseManager;
        private Mock<ParentalControlEntities> mockParentalControlEntities;

        [SetUp]
        public void Init()
        {
            var users = CreateMockDbSet<User>();
            var programSettings = CreateMockDbSet<ProgramSetting>();
            var timeSettings = CreateMockDbSet<TimeSetting>();
            var keywords = CreateMockDbSet<Keyword>();
            var webSettings = CreateMockDbSet<WebSetting>();

            this.mockParentalControlEntities = new Mock<ParentalControlEntities>();
            this.mockParentalControlEntities.SetupGet(m => m.Users).Returns(users.Object);
            this.mockParentalControlEntities.SetupGet(m => m.ProgramSettings).Returns(programSettings.Object);
            this.mockParentalControlEntities.SetupGet(m => m.TimeSettings).Returns(timeSettings.Object);
            this.mockParentalControlEntities.SetupGet(m => m.Keywords).Returns(keywords.Object);
            this.mockParentalControlEntities.SetupGet(m => m.WebSettings).Returns(webSettings.Object);

            this.databaseManager = Construct<DatabaseManager>();
            this.databaseManager.GetType().GetField("entities", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(this.databaseManager, this.mockParentalControlEntities.Object);
        }

        [Test]
        public void Test()
        {
            int countBefore = this.mockParentalControlEntities.Object.Users.Count();
            this.databaseManager.Transaction(() => this.databaseManager.CreateUser("Test", "Test", "Test", "Test"));
            int countAfter = this.mockParentalControlEntities.Object.Users.Count();
            Assert.That(countBefore + 1 == countAfter);
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
            var mockObject = new Mock<DbSet<T>>().SetupData(mockList);
            return mockObject;
        }
    }
}
