using app_poprizonok.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для Page2.xaml
    /// </summary>
    public partial class Page2 : Page
    {
        private Agent agent;
        private int curSelPr = 0;
        private int curTypAg = 0;

        public Page2(Agent ag)
        {
            InitializeComponent();
            try
            {
                Type.ItemsSource = helper.GetContext().AgentType.ToList();
                product.ItemsSource = helper.GetContext().Product.ToList();
            }
            catch { };
            if (ag != null)
            {
                agent = ag;
                Type.SelectedItem = ag.AgentType;
                this.Title.Text = ag.Title;
                this.Adress.Text = ag.Address;
                this.Inn.Text = ag.INN;
                this.Kpp.Text = ag.KPP;
                this.Director.Text = ag.DirectorName;
                this.Phone.Text = ag.Phone;
                this.Email.Text = ag.Email;
                this.Prioritet.Text = ag.Priority.ToString();
                historyGrid.ItemsSource = helper.GetContext().ProductSale.Where(ProductSale => ProductSale.AgentID == ag.ID).ToList();
            }
            else
            {
                agent = new Agent();
                btnDelAg.IsEnabled = false;
                btnWritHi.IsEnabled = false;
                btnDelHi.IsEnabled = false;
            }
            this.DataContext = agent;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (helper.GetContext() == null)
            {
                MessageBox.Show("Нет подключения к базе данных. Пожалуйста, убедитесь, что вы подключены к рабочей базе данных.");
                return;
            }

            if (string.IsNullOrWhiteSpace(this.Title.Text))
            {
                MessageBox.Show("Введите наименование агента.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (curTypAg == -1)
            {
                MessageBox.Show("Выберите тип агента.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(this.Adress.Text))
            {
                MessageBox.Show("Введите адрес агента.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Regex.IsMatch(this.Inn.Text, @"\d{10}|\d{12}"))
            {
                MessageBox.Show("Введите корректный ИНН.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Regex.IsMatch(this.Kpp.Text, @"\d{9}"))
            {
                MessageBox.Show("Введите корректный КПП.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Regex.IsMatch(this.Phone.Text, @"^\+?\d{0,2}\-?\d{3}\-?\d{3}\-?\d{4}"))
            {
                MessageBox.Show("Введите корректный номер телефона.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!string.IsNullOrWhiteSpace(this.Email.Text) && !Regex.IsMatch(this.Email.Text, @"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)"))
            {
                MessageBox.Show("Введите корректный адрес электронной почты.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(this.Prioritet.Text, out int priority))
            {
                MessageBox.Show("Введите корректное значение приоритета (целое число).", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            agent.Title = this.Title.Text;
            agent.AgentTypeID = curTypAg;
            agent.Address = this.Adress.Text;
            agent.INN = this.Inn.Text;
            agent.KPP = this.Kpp.Text;
            agent.Phone = this.Phone.Text;
            agent.DirectorName = this.Director.Text;
            agent.Phone = this.Phone.Text;
            agent.Email = this.Email.Text;
            agent.Priority = priority;

            try
            {
                if (agent.ID > 0)
                {
                    helper.GetContext().Entry(agent).State = EntityState.Modified;
                    helper.GetContext().SaveChanges();
                    MessageBox.Show("Информация об агенте успешно обновлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    helper.ent.Agent.Add(agent);
                    helper.ent.SaveChanges();
                    MessageBox.Show("Информация об агенте успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var error in ex.EntityValidationErrors)
                {
                    foreach (var errorMessage in error.ValidationErrors)
                    {
                        sb.AppendLine($"- {errorMessage.ErrorMessage}");
                    }
                }
                MessageBox.Show($"Произошла ошибка валидации сущности:\n{sb.ToString()}", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            btnDelAg.IsEnabled = true;
            btnWritHi.IsEnabled = true;
            btnDelHi.IsEnabled = true;
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (agent.ProductSale.Count > 0)
            {
                MessageBox.Show("Удаление не возможно!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            foreach (Shop shop in agent.Shop)
            {
                helper.GetContext().Shop.Remove(shop);
            }
            foreach (AgentPriorityHistory apr in agent.AgentPriorityHistory)
            {
                helper.GetContext().AgentPriorityHistory.Remove(apr);
            }
            helper.GetContext().Agent.Remove(agent);
            helper.GetContext().SaveChanges();
            MessageBox.Show("Удаление информации об агенте завершено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            this.NavigationService.GoBack();

        }

        private void historyGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            curTypAg = ((AgentType)Type.SelectedItem).ID;
        }

        private void product_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            curSelPr = ((Product)product.SelectedItem).ID;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            int cnt = 0;
            try
            {
                cnt = Convert.ToInt32(count.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверный формат количества товара. Пожалуйста, введите корректное целое число.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string dt = date.ToString();
            if (curSelPr > 0 && dt != "" && cnt > 0)
            {
                ProductSale pr = new ProductSale();
                pr.AgentID = agent.ID;
                pr.ProductID = curSelPr;
                pr.SaleDate = (DateTime)date.SelectedDate;
                pr.ProductCount = cnt;
                try
                {
                    helper.GetContext().ProductSale.Add(pr);
                    helper.GetContext().SaveChanges();
                    historyGrid.ItemsSource = helper.GetContext().ProductSale.Where(ProductSale => ProductSale.AgentID == agent.ID).ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при добавлении записи о продаже:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Невозможно добавить запись о продаже. Пожалуйста, проверьте введенные данные и повторите попытку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < historyGrid.SelectedItems.Count; i++)
            {
                ProductSale prs = historyGrid.SelectedItems[i] as ProductSale;
                if (prs != null)
                {
                    helper.GetContext().ProductSale.Remove(prs);
                }
            }
            try
            {
                helper.GetContext().SaveChanges();
                historyGrid.ItemsSource = helper.GetContext().ProductSale.Where(ProductSale => ProductSale.AgentID == agent.ID).ToList();
            }
            catch { return; };

        }


        private void mask_TextChanged(object sender, TextChangedEventArgs e)
        {
            string fnd = ((TextBox)sender).Text;
            try
            {
                product.ItemsSource = helper.GetContext().Product.Where(Product => Product.Title.Contains(fnd)).ToList();
            }
            catch { };

        }
    }
}
