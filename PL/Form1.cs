using BLL;
using UoW;

namespace PL
{
    public partial class Form1 : Form
    {
        UnitOfWork UoW;
        GetCategory goods;
        public Form1()
        {
            UoW = new UnitOfWork();
            goods = new GetCategory(UoW);
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var result = goods.GetAll();
            listBox1.Items.Add(10);
        }
    }
}