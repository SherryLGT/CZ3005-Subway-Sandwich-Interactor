using SbsSW.SwiPlCs;
using SbsSW.SwiPlCs.Exceptions;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace SubwaySandwichInteractor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        static readonly string[] options = { "meal", "bread", "main", "veggie", "sauce", "side", "topup" };
        static int currentStep = 0;

        public MainWindow()
        {
            InitializeComponent();

            // Try to clean up dirty Prolog engine resources from previous runs if execution terminates unexpectedly.
            try
            {
                PlEngine.PlCleanup();
            }
            catch (PlException) { }
            catch (Exception) { }

            // Initialize Prolog engine with sandwich.pl file
            String[] param = { "-q", "-f", "sandwich.pl" };
            PlEngine.Initialize(param);

            Reset();
        }

        private void Reset()
        {
            currentStep = 0;
            using (PlQuery q = new PlQuery("reset"))
            {
                q.NextSolution();
            }
            DisplayOptions();
            DisplayOrders();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            currentStep++;
            DisplayOptions();
        }

        private void DisplayOptions()
        {
            switch (currentStep)
            {
                case 0:
                    LblStepTitle.Content = "Select Your Meal Types";
                    break;
                case 1:
                    LblStepTitle.Content = "Select Your Bread Type";
                    break;
                case 2:
                    LblStepTitle.Content = "Select Your Main";
                    break;
                case 3:
                    LblStepTitle.Content = "Select Your Veggies";
                    break;
                case 4:
                    LblStepTitle.Content = "Select Your Sauces";
                    break;
                case 5:
                    LblStepTitle.Content = "Select Your Sides";
                    break;
                case 6:
                    LblStepTitle.Content = "Select Your Top-ups";
                    break;
            }

            LstOption.Items.Clear();
            using (PlQuery q = new PlQuery("options(X," + options[currentStep] + ")"))
                foreach (PlTermV v in q.Solutions)
                    LstOption.Items.Add(Beautify(v[0].ToString()));

            var nextStep = currentStep + 1;
            var end = false;
            if (nextStep == options.Length)
            {
                end = true;
            }
            else
            {
                using (PlQuery q = new PlQuery("options(X," + options[nextStep] + ")"))
                    if (q.Solutions.Count() == 0)
                        end = true;
            }

            if (end)
            {
                BtnNext.Visibility = Visibility.Collapsed;
                BtnConfirm.Visibility = Visibility.Visible;
            }
            else
            {
                BtnNext.Visibility = Visibility.Visible;
                BtnConfirm.Visibility = Visibility.Collapsed;
            }

            if (currentStep == 0)
                BtnBack.Visibility = Visibility.Hidden;
            else
                BtnBack.Visibility = Visibility.Visible;

            TxbMessage.Text = "";
        }

        private void DisplayOrders()
        {
            LblOrders.Content = "";
            foreach (var category in options)
            {
                using (PlQuery q = new PlQuery("selected(X," + category + ")"))
                {
                    if (q.Solutions.Count() == 0)
                        continue;
                    LblOrders.Content += Beautify(category) + "\n";
                    foreach (PlTermV v in q.Solutions)
                        LblOrders.Content += " - " + Beautify(v[0].ToString()) + "\n";
                }
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            currentStep--;
            DisplayOptions();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (LstOption.SelectedItem == null)
                return;
            var selected = LstOption.SelectedItem.ToString();
            using (PlQuery q = new PlQuery("select(" + UnBeautify(selected) + ")"))
            {
                var success = q.NextSolution();
                if (success)
                {
                    TxbMessage.Text = string.Format("'{0}' added successfully.", selected);
                    TxbMessage.Foreground = Brushes.Green;
                }
                else
                {
                    TxbMessage.Text = string.Format("Error adding '{0}'. \nEither '{0}' is invalid, is already added, or the {1} category only allows one option.", selected, Beautify(options[currentStep]));
                    TxbMessage.Foreground = Brushes.Red;
                }
            }
            DisplayOrders();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Order Confirmed!");
            Reset();
        }

        private string Beautify(string str)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(str.Replace('_', ' '));
        }

        private string UnBeautify(string str)
        {
            return str.Replace(' ', '_').ToLower();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Clean up Prolog engine resources.
            PlEngine.PlCleanup();
        }
    }
}
