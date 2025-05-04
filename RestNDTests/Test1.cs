using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestND;
using RestND.Data;
using RestND.MVVM.Model;

namespace RestNDTests
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var services = new ProductService();

            var product = new Product
            {
               Product_ID = 1,
                Product_Name = "Test Product",
                Quantity_Available = 9
            };
            var res = services.Add(product);


            Assert.IsTrue(res);

        }
    }
}
