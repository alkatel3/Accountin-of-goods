using BLL;
using BLL.Entities;

namespace TestProject1
{
    public class GoodsProviderTest
    {
        [Test]
        public void Sell_SellTenGoodsFromModelDb_CountOfGoodsMinusTen()
        {
            var goods = new GoodsBLL()
            {
                Name = "Any",
                CategoryBLL = new CategoryBLL() { Name = "category", Id = 10 },
                Count = 100,
                Priсe = 10,
                Id = 10
            };
            GoodsProvider provider = new();

            provider.Deliver(10, goods);

            goods.Count.Should().Be(110);
        }
    }
}
