using Project;
using Library;

namespace Test
{
    [TestClass]
    public class Testing
    {
        [Test]
        public void Add_IncreasesCount()
        {
            var p = new MyProject();
            p.Add("Buy milk");

            Assert.AreEqual(1, p.Count);
        }

        [Test]
        public void Add_SavesCorrectTask()
        {
            var p = new MyProject();

            p.Add("Learn C#");

            Assert.AreEqual("Learn C#", p.Get(0));
        }

        [Test]
        public void Add_EmptyTask_ThrowsException()
        {
            var p = new MyProject();

            Assert.Throws<ArgumentException>(() =>
            {
                p.Add("");
            });
        }

        [Test]
        public void Remove_DecreasesCount()
        {
            var p = new MyProject();
            p.Add("Task 1");
            p.Add("Task 2");

            p.Remove(0);

            Assert.AreEqual(1, p.Count);
        }

        [Test]
        public void Remove_InvalidIndex_ThrowsException()
        {
            var p = new MyProject();

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                p.Remove(0);
            });
        }

        [Test]
        public void MarkCompleted_MarksTaskAsCompleted()
        {
            var p = new MyProject();
            p.Add("Finish homework");

            p.MarkCompleted(0);

            Assert.IsTrue(p.IsCompleted(0));
        }

        [Test]
        public void IsCompleted_NotCompleted_ReturnsFalse()
        {
            var p = new MyProject();
            p.Add("Read book");

            Assert.IsFalse(p.IsCompleted(0));
        }

        [Test]
        public void IsCompleted_InvalidIndex_ThrowsException()
        {
            var p = new MyProject();

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                p.IsCompleted(0);
            });
        }

        [Test(priority: 10)]
        public void MultipleOperations_WorkCorrectly()
        {
            var p = new MyProject();

            p.Add("Task A");
            p.Add("Task B");
            p.MarkCompleted(1);
            p.Remove(0);

            Assert.AreEqual(1, p.Count);
            Assert.IsTrue(p.IsCompleted(0));
        }

        [Test(enabled: false)]
        public void Disabled_Test_ShouldNotRun()
        {
            Assert.IsTrue(false);
        }
    }
}
