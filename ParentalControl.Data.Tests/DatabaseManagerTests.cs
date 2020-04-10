using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;

namespace ParentalControl.Data.Tests
{
    [TestFixture]
    public class DatabaseManagerTests
    {
        public Mock<DatabaseManager> DatabaseManager { get; set; }

        [SetUp]
        public void Init()
        {
            this.DatabaseManager = new Mock<DatabaseManager>();
        }

        [Test]
        public void Test()
        {

        }
    }
}
