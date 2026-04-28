using lab4.TestedProject;
using lab4.Tests;
using lab4.Tests.Assertions;
using System;
using System.Collections.Generic;
using System.Text;

namespace lab4.TestsProject
{
    [TestClassAttr]
    public class TaskManagerTests
    {
        private TaskManager _manager = null!;

        [Before]
        public void Setup()
        {
            _manager = new TaskManager();
        }

        [Test(priority: 4)]
        [Category("Tasks")]
        [Author("Pavel")]
        public void Add_IncreasesCount()
        {
            _manager.Add("Buy milk");
            Assert.AreEqual(1, _manager.Count);
        }

        [Test(priority: 1)]
        [Category("Tasks")]
        [Author("Pavel")]
        public void Add_SavesCorrectTask()
        {
            _manager.Add("Learn C#");
            Assert.AreEqual("Learn C#", _manager.GetAt(0));
        }

        [Test(enabled: false, priority: 0)]
        [Category("Tasks")]
        [Author("Pavel")]
        public void Disabled_Test_Example()
        {
            Assert.IsTrue(false);
        }
    }
}
