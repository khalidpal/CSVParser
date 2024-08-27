using CsvHelper;
using CsvHelper.Configuration;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Globalization;
using System.IO.Abstractions.TestingHelpers;
using System.Text;

namespace CSVParser
{
    [TestClass]
    public class ParserClassTests
    {
        static private ICsvHandler _csvHandler;
        static private Mock<IFactory> _csvHelper;
        static private Mock<IWriter> _csvWriter;
        static private Mock<IReader> _csvReader;
        static private MockFileStream _streamWriter;

        static string fileContent;
        static List<DataModel> expected;

        static Mock<CsvContext> _csvContext;
        static CsvConfiguration configuration;

        private StringBuilder GetFileContent()
        {
            StringBuilder testContents = new StringBuilder();
            testContents.AppendLine("organisation_id,suborg_id,organisation_name,organisation_number,parent_or_child,license");
            testContents.AppendLine("1,,OrgA,123456,Parent,");
            testContents.AppendLine("2,Sub2,OrgB,654321,Child,License123");
            testContents.AppendLine("3,Sub3,OrgC,245311,Parent,License456");
            return testContents;
        }


        Mock<ICsvHandler> parserClassMoq = null;
        static string mockFileContents = "";
        static MemoryStream mockMemoryStream = new MemoryStream();
        static byte[] mockFileBytes;
        //static List<DataModel> listDataModel = null;
        static List<DataModel> listDataModel = null;
        private static Mock<Microsoft.Extensions.Logging.ILogger<CsvHandler>> _loggerMock;
        static string mockFilePath = "testing.csv";

        [ClassInitialize]
        public static void ParserClassRequiredObjects(TestContext context)
        {
            string mockFileContents = "organisation_id,suborg_id,organisation_name,organisation_number,parent_or_child,license,errors\n" +
                                        "1,,OrgA,123456,Parent,\n" +
                                        "2,Sub1,OrgB,654321,Child,License123";

            mockFileBytes = Encoding.UTF8.GetBytes(mockFileContents);

            listDataModel = new List<DataModel>
            {
                new DataModel { organisation_id = 1, suborg_id = "", organisation_name = "OrgA", organisation_number = "123456", parent_or_child = "Parent", license = "", errors = "suborg_id is nulllicense is null" },
                new DataModel { organisation_id = 2, suborg_id = "Sub1", organisation_name = "OrgB", organisation_number = "654321", parent_or_child = "Child", license = "License123", errors = "" }
            };

            _loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<CsvHandler>>();


            //arrange
            fileContent = mockFileContents;
            expected = listDataModel;
            configuration = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                Delimiter = ",",
                MissingFieldFound = null,
                TrimOptions = TrimOptions.Trim,
                HeaderValidated = null,
                BadDataFound = null
            };

            _csvHelper = new Mock<IFactory>(MockBehavior.Loose);
            _csvWriter = new Mock<IWriter>(MockBehavior.Loose);
            _csvReader = new Mock<IReader>(MockBehavior.Loose);
            _csvHandler = new CsvHandler(_loggerMock.Object);


        }

        [ClassCleanup]
        public static void ParserClassCleanStaticObjects()
        {
            //File.Delete(mockFilePath);
        }


        [TestInitialize]
        public void ParserClassMockObjectsSetup()
        {
            _csvReader.Setup(s => s.Configuration).Returns(configuration);
            _csvHelper.Setup(s => s.CreateReader(It.IsAny<StringReader>(), CultureInfo.CurrentCulture)).Returns(_csvReader.Object);

            _csvContext = new Mock<CsvContext>(_csvReader.Object);

            _csvReader.Setup(s => s.Context).Returns(_csvContext.Object);
            _csvReader.Setup(s => s.Context.RegisterClassMap<DataModelMap>()).Returns(It.IsAny<DataModelMap>());
            //_csvReader.Setup(s => s.GetRecords<DataModel>()).Returns(expected);


        }

        [TestCleanup]
        public void CleanAllMockedObjects()
        {
            listDataModel = null;
            _csvReader.Setup(p => p.Dispose());
        }

        [TestMethod]
        public void ParseClass_ReturnCorrectFileContents()
        {
            //act
            var actual = _csvHandler.GetListOfRecords<DataModel, DataModelMap>(fileContent).ToList();

            //assert
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
