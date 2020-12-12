using NUnit.Framework;
using Hopper.Core.FS;
using System.Collections.Generic;

namespace Hopper.Tests
{
    public class FS_Test
    {
        private class TestFile : File
        {
            public string message;
        }

        private FS<TestFile> fs;
        private TestFile file_Hello = new TestFile { message = "Hello" };
        private TestFile file_World = new TestFile { message = "World" };

        public FS_Test()
        {
        }

        [SetUp]
        public void Setup()
        {
            fs = new FS<TestFile>();
        }

        [Test]
        public void FileIsAdded()
        {
            var file1 = fs.GetFileLazy("test", file_Hello);
            var file2 = fs.GetFile("test");
            Assert.AreEqual("Hello", file1.message);
            Assert.AreEqual("Hello", file2.message);
        }

        [Test]
        public void FileIsNotReplaced()
        {
            var file1 = fs.GetFileLazy("test", file_Hello);
            var file2 = fs.GetFileLazy("test", file_World);
            Assert.AreEqual("Hello", file1.message);
            Assert.AreEqual("Hello", file2.message);
        }

        [Test]
        public void ForDeepPaths_MultipleDirectoriesAreCreated()
        {
            var file1 = fs.GetFileLazy("1/2/test", file_Hello);

            var node1 = fs.GetNode("1");
            Assert.That(node1 is Directory);

            var node2 = fs.GetNode("1/2");
            Assert.That(node2 is Directory);

            var dir1 = (Directory)node1;
            var dir2 = (Directory)node2;
            Assert.AreSame(dir1.nodes["2"], dir2);
        }

        [Test]
        public void MulticastPath_WorksCorrectly()
        {
            var file1 = fs.GetFilesLazy("1/Hello", file_Hello);
            var file2 = fs.GetFilesLazy("2/World", file_World);

            var files = fs.GetFiles("*/*");
            Assert.AreEqual(2, files.Count);

            Assert.Throws<KeyNotFoundException>(() => fs.GetFiles("*/Hello"));

            var files2 = fs.GetFiles("1/*");
            Assert.AreEqual(1, files2.Count);
            Assert.AreEqual("Hello", files2[0].message);
        }

        [Test]
        public void GettingAllFiles_WorksCorrectly()
        {
            var file1 = fs.GetFilesLazy("Hello", file_Hello);
            var file2 = fs.GetFilesLazy("2/World", file_World);

            var files = fs.GetAllFiles();
            Assert.AreEqual(2, files.Count);
        }
    }
}