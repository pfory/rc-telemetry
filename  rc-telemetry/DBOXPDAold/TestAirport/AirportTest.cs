using DBOX;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace TestAirport
{
    
    
    /// <summary>
    ///This is a test class for AirportTest and is intended
    ///to contain all AirportTest Unit Tests
    ///</summary>
  [TestClass()]
  public class AirportTest
  {


    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext
    {
      get
      {
        return testContextInstance;
      }
      set
      {
        testContextInstance = value;
      }
    }

    #region Additional test attributes
    // 
    //You can use the following additional attributes as you write your tests:
    //
    //Use ClassInitialize to run code before running the first test in the class
    //[ClassInitialize()]
    //public static void MyClassInitialize(TestContext testContext)
    //{
    //}
    //
    //Use ClassCleanup to run code after all tests in a class have run
    //[ClassCleanup()]
    //public static void MyClassCleanup()
    //{
    //}
    //
    //Use TestInitialize to run code before running each test
    //[TestInitialize()]
    //public void MyTestInitialize()
    //{
    //}
    //
    //Use TestCleanup to run code after each test has run
    //[TestCleanup()]
    //public void MyTestCleanup()
    //{
    //}
    //
    #endregion


    /// <summary>
    ///A test for set_AirportName
    ///</summary>
    [TestMethod()]
    public void set_AirportNameTest()
    {
      Airport target = new Airport(); // TODO: Initialize to an appropriate value
      target.set_AirportName();
      Assert.Inconclusive("A method that does not return a value cannot be verified.");
    }

    /// <summary>
    ///A test for set_AirportID
    ///</summary>
    [TestMethod()]
    public void set_AirportIDTest()
    {
      Airport target = new Airport(); // TODO: Initialize to an appropriate value
      target.set_AirportID(1);
      Assert.Inconclusive("A method that does not return a value cannot be verified.");
    }

    /// <summary>
    ///A test for set_AirportAltitude
    ///</summary>
    [TestMethod()]
    public void set_AirportAltitudeTest()
    {
      Airport target = new Airport(); // TODO: Initialize to an appropriate value
      target.set_AirportAltitude();
      Assert.Inconclusive("A method that does not return a value cannot be verified.");
    }

    /// <summary>
    ///A test for get_AirportName
    ///</summary>
    [TestMethod()]
    public void get_AirportNameTest()
    {
      Airport target = new Airport(); // TODO: Initialize to an appropriate value
      string expected = string.Empty; // TODO: Initialize to an appropriate value
      string actual;
      actual = target.get_AirportName();
      Assert.AreEqual(expected, actual);
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for get_AirportID
    ///</summary>
    [TestMethod()]
    public void get_AirportIDTest()
    {
      Airport target = new Airport(); // TODO: Initialize to an appropriate value
      int expected = 0; // TODO: Initialize to an appropriate value
      int actual;
      actual = target.get_AirportID();
      Assert.AreEqual(expected, actual);
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for get_AirportAltitude
    ///</summary>
    [TestMethod()]
    public void get_AirportAltitudeTest()
    {
      Airport target = new Airport(); // TODO: Initialize to an appropriate value
      uint expected = 0; // TODO: Initialize to an appropriate value
      uint actual;
      actual = target.get_AirportAltitude();
      Assert.AreEqual(expected, actual);
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for Airport Constructor
    ///</summary>
    [TestMethod()]
    public void AirportConstructorTest()
    {
      Airport target = new Airport();
      Assert.Inconclusive("TODO: Implement code to verify target");
    }
  }
}
