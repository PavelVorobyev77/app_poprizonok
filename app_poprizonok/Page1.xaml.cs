using app_poprizonok.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace app_poprizonok
{
    /// <summary>
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        private int start = 0;
        private int fullCount = 0;
        private int order = 0;
        private string fnd ="";
        private int iag = 0;
        private Frame fr;

        public Page1(Frame frame)
        {
            InitializeComponent();
            fr = frame;
            List<AgentType> agents = new List<AgentType> { };
            agents = helper.GetContext().AgentType.ToList();
            agents.Add(new AgentType { Title = "Все типы" });
            Type.ItemsSource = agents.OrderBy(AgentType => AgentType.ID);

            Load();
        }

        public void Load()
        {
            try
            {
                List<Agent> agents = new List<Agent>();
                var ag = helper.GetContext().Agent.Where(Agent => Agent.Title.Contains(fnd) || Agent.Phone.Contains(fnd) || Agent.Email.Contains(fnd));
                if (iag > 0) ag = helper.GetContext().Agent.Where(Agent => (Agent.Title.Contains(fnd) || Agent.Phone.Contains(fnd) || Agent.Email.Contains(fnd)) && (Agent.AgentTypeID == iag));
                agents.Clear();
                foreach (Agent agent in ag)
                {
                    if (agent.Logo == "")
                    {
                        agent.Logo = "/images/picture.png";
                    }
                    int sum = 0;
                    double fsum = 0;
                    foreach (ProductSale ps in agent.ProductSale)
                    {
                        List<ProductMaterial> mtr = new List<ProductMaterial> { };
                        mtr = helper.GetContext().ProductMaterial.Where(ProductMaterial => ProductMaterial.ProductID == ps.ProductID).ToList();
                        foreach (ProductMaterial mt in mtr)
                        {
                            double f = decimal.ToDouble(mt.Material.Cost);
                            fsum += f * (double)mt.Count;
                        }
                        fsum = fsum * ps.ProductCount;
                        if (ps.SaleDate.AddDays(365).CompareTo(DateTime.Today) > 0)
                            sum += ps.ProductCount;
                    }
                    agent.sale = sum;
                    //agent.fsale = fsum;
                    agent.percent = 0;
                    if (fsum >= 10000 && fsum < 50000) agent.percent = 5;
                    if (fsum >= 50000 && fsum < 150000) agent.percent = 10;
                    if (fsum >= 150000 && fsum < 500000) agent.percent = 20;
                    if (fsum >= 500000) agent.percent = 25;
                    agents.Add(agent);
                }
                fullCount = ag.Count();
                if (order == 0) agentGrid.ItemsSource = ag.OrderBy(Agent => Agent.ID).Skip(start * 10).Take(10).ToList();
                if (order == 1) agentGrid.ItemsSource = ag.OrderBy(Agent => Agent.Title).Skip(start * 10).Take(10).ToList();
                if (order == 2) agentGrid.ItemsSource = ag.OrderByDescending(Agent => Agent.Title).Skip(start * 10).Take(10).ToList();
                if (order == 3) agentGrid.ItemsSource = ag.OrderBy(Agent => Agent.Priority).Skip(start * 10).Take(10).ToList();
                if (order == 4) agentGrid.ItemsSource = ag.OrderByDescending(Agent => Agent.Priority).Skip(start * 10).Take(10).ToList();
                if (order == 5 || order == 6)
                {
                    agents.Sort(Srt);
                    agentGrid.ItemsSource = agents.Skip(start * 10).Take(10).ToList();
                }
                full.Text = fullCount.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            };



            int ost = fullCount % 10;
            int pag = (fullCount - ost) / 10;
            if (ost > 0) pag++;
            pagin.Children.Clear();
            for (int i = 0; i < pag; i++)
            {
                Button myButton = new Button();
                myButton.Height = 30;
                myButton.Content = i + 1;
                myButton.Width = 20;
                myButton.HorizontalAlignment = HorizontalAlignment.Center;
                myButton.Tag = i;
                myButton.Click += new RoutedEventHandler(paginButto_Click); ;
                pagin.Children.Add(myButton);
            }
            turnButton();


        }
        private int Srt(Agent x, Agent y)
        {
            if (x.percent == y.percent) return 0;
            if (((order == 5) && (x.percent > y.percent)) || ((order == 6)) && (x.percent < y.percent)) return 1;
            return -1;
        }


        private void turnButton()
        {
            if (start == 0) { back.IsEnabled = false; }
            else { back.IsEnabled = true; };
            if ((start + 1) * 10 > fullCount) { forward.IsEnabled = false; }
            else { forward.IsEnabled = true; };
        }


        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            if (agentGrid.SelectedItems.Count > 0)
            {
                int prt = 0;
                foreach (Agent agent in agentGrid.SelectedItems)
                {
                    if (agent.Priority > prt) prt = agent.Priority;
                }
                Window1 dlg = new Window1(prt);
                helper.prioritet = prt;
                helper.flag = false;
                dlg.ShowDialog();
                if (helper.flag)
                {
                    foreach (Agent agent in agentGrid.SelectedItems)
                    {
                        agent.Priority = helper.prioritet;
                        helper.GetContext().Entry(agent).State = EntityState.Modified;
                    }
                    helper.GetContext().SaveChanges();
                    Load();
                }
            }


        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            fr.Content = new Page2(null);
        }

        private void paginButto_Click(object sender, RoutedEventArgs e)
        {
            start = Convert.ToInt32(((Button)sender).Tag.ToString());
            Load();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            start--;
            Load();

        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            start++;
            Load();

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            order = Convert.ToInt32(selectedItem.Tag.ToString());
            Load();

        }

        private void Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            iag = ((AgentType)Type.SelectedItem).ID;
            Load();

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            fnd = ((TextBox)sender).Text;
            Load();
        }

        private void agentGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (agentGrid.SelectedItems.Count > 0)
            {
                Agent agent = agentGrid.SelectedItems[0] as Agent;

                if (agent != null)
                {
                    fr.Content = new Page2(agent);
                }
            }

        }
    }

}
