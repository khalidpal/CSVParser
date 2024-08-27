using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Testing.Platform.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net.Http;

namespace CSVParser
{
    [TestClass]
    public class ParserClassTests
    {
        //static List<DataModel> listDataModel = null;
        static List<DataModel> listDataModel = null;
        private static Mock<Microsoft.Extensions.Logging.ILogger<ParserClass>> _loggerMock;
        static string filePath = "testing.csv";
        Mock<IParserClass> parserClassMoq = null;

        [ClassInitialize]
        public static void ParserClassRequiredObjects(TestContext context)
        {
            File.WriteAllText(filePath, "organisation_id,suborg_id,organisation_name,organisation_number,parent_or_child,license\n" +
                                         "1,,OrgA,123456,Parent,\n" +
                                         "2,Sub1,OrgB,654321,Child,License123");

            listDataModel = new List<DataModel>
            {
                new DataModel { organisation_id = 1, suborg_id = "", organisation_name = "OrgA", organisation_number = "123456", parent_or_child = "Parent", license = "", errors = "" },
                new DataModel { organisation_id = 2, suborg_id = "Sub1", organisation_name = "OrgB", organisation_number = "654321", parent_or_child = "Child", license = "License123", errors = "" }
            };

            _loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<ParserClass>>();

        }

        [ClassCleanup]
        public static void ParserClassCleanStaticObjects()
        {
            File.Delete(filePath);
        }


        [TestInitialize]
        public void ParserClassMockObjectsSetup()
        {
            parserClassMoq = new Mock<IParserClass>();
            parserClassMoq.Setup(x => x.Parse(filePath)).Returns(listDataModel);
            
            parserClassMoq.Setup(x => x.Parse(filePath)).Returns((List<DataModel>)Enumerable.Empty<List<DataModel>>());
            parserClassMoq.Setup(x => x.Parse(filePath)).Returns(new List<DataModel>());

            parserClassMoq.Setup(x => x.Parse(filePath)).Throws(new Exception("Error reading CSV file"));

        }

        [TestCleanup]
        public void CleanAllMockedObjects()
        {
            listDataModel = null;
        }


        [TestMethod]
        public void ParseClass_ValidCsvFile_ReturnEmptyData()
        {
            var theResult = parserClassMoq.Object.Parse(filePath);

            //Assert.That.

            Assert.IsTrue(theResult.Count().Equals(0));

        }

        [TestMethod]
        public void ParseClass_ValidCsvFile_ReturnsCorrectData()
        {
            var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<ParserClass>>();

            // Arrange
            // sut == System Under Test
            ParserClass sut = new ParserClass(_loggerMock.Object);

            // Act
            var returnValue = sut.Parse(filePath);

            // Assert
            returnValue.Should().BeOfType<List<DataModel>>();

            Assert.AreEqual(2, returnValue.Count); // Two rows

            Assert.AreEqual(1, returnValue[0].organisation_id);
            Assert.AreEqual("", returnValue[0].suborg_id);
            Assert.AreEqual("OrgA", returnValue[0].organisation_name);
            Assert.AreEqual("123456", returnValue[0].organisation_number);
            Assert.AreEqual("Parent", returnValue[0].parent_or_child);
            Assert.AreEqual("", returnValue[0].license);

            Assert.AreEqual(2, returnValue[1].organisation_id);
            Assert.AreEqual("Sub1", returnValue[1].suborg_id);
            Assert.AreEqual("OrgB", returnValue[1].organisation_name);
            Assert.AreEqual("654321", returnValue[1].organisation_number);
            Assert.AreEqual("Child", returnValue[1].parent_or_child);
            Assert.AreEqual("License123", returnValue[1].license);

            //// Cleanup
            //File.Delete(filePath);
        }

        [TestMethod]
        public void ParseClass_FileNotFound_ThrowsFileNotFoundException()
        {
            // Arrange
            var parserClass = new ParserClass(_loggerMock.Object);
            var filePath = "nonexistent.csv";

            // Act & Assert
            Assert.ThrowsException<FileNotFoundException>(() => parserClass.Parse(filePath));
        }

    }
}
