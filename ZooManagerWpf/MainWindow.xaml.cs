using System;
using System.Collections.Generic;
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
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
namespace ZooManagerWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;
        public MainWindow()
        {
            InitializeComponent();
            string connectionString = ConfigurationManager.ConnectionStrings["ZooManagerWpf.Properties.Settings.tmamDBConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);
            showZoos();
            showAllAnimals();
        }

        private void showZoos()
        {
           
            try {
                string query = "select * from Zoo";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable zooTable = new DataTable();
                    sqlDataAdapter.Fill(zooTable);

                    listZoos.DisplayMemberPath = "Location";
                    listZoos.SelectedValuePath = "Id";
                    listZoos.ItemsSource = zooTable.DefaultView;
                }
            }
            catch (Exception e){
                MessageBox.Show(e.ToString());
            }
        }

        private void showAssociatedAnimals()
        {

            try
            {
                string query = "select * from Animal a inner join ZooAnimal za on a.Id=za.AnimalId " +
                 "where za.ZooId = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                using (sqlDataAdapter) {
                    sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                    DataTable animalTable=new DataTable();
                    sqlDataAdapter.Fill(animalTable);

                    listAssociatedAnmials.DisplayMemberPath = "Name";
                    listAssociatedAnmials.SelectedValuePath= "Id";
                    listAssociatedAnmials.ItemsSource = animalTable.DefaultView;
                }
            }
            catch (Exception e)
            {
              //  MessageBox.Show(e.ToString());
            }
        }
        private void showAllAnimals()
        {
            try
            {
                string query = "select * from Animal";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);
                using(sqlDataAdapter){
                    DataTable animalTable= new DataTable();
                    sqlDataAdapter.Fill(animalTable);

                    listAllAnmials.DisplayMemberPath= "Name";
                    listAllAnmials.SelectedValuePath = "Id";
                    listAllAnmials.ItemsSource = animalTable.DefaultView;

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void listZoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            showAssociatedAnimals();
        }

        private void DeleteZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Zoo where Id=@ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                sqlCommand.ExecuteScalar();


            }
            catch (Exception ex) { 
              MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                showZoos();
            }
        }
    }
}
