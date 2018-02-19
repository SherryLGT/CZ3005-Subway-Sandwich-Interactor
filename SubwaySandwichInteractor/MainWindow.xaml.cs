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
        // Options category
        static readonly string[] options = { "meal", "bread", "main", "veggie", "sauce", "side", "topup" };
        // Keep track of order step
        static int currentStep = 0;

        public MainWindow()
        {
            InitializeComponent();

            // Try to clean up dirty Prolog engine resources from previous runs if execution terminates unexpectedly
            try
            {
                PlEngine.PlCleanup();
            }
            catch (PlException) { }
            catch (Exception) { }

            // Initialize Prolog engine with sandwich.pl file
            String[] param = { "-q", "-f", "sandwich.pl" };
            PlEngine.Initialize(param);

            // Ensure all values are at its initial state
            Reset();
        }

        // Setting everything back to initial state
        private void Reset()
        {
            currentStep = 0;
            // Using prolog logic "reset"
            using (PlQuery q = new PlQuery("reset"))
            {
                q.NextSolution();
            }
            DisplayOptions();
            DisplayOrders();
        }

        // Proceed to next step
        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            // Increment of currentStep to indicate proceed of next step
            currentStep++;
            DisplayOptions();
        }

        // Setup and display of order options
        private void DisplayOptions()
        {
            // Using currentstep value to display respective title
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

            // Clear any previous option
            LstOption.Items.Clear();
            // Get respective options from prolog logic, e.g. options(X,meal)
            using (PlQuery q = new PlQuery("options(X," + options[currentStep] + ")"))
                foreach (PlTermV v in q.Solutions)
                    // For each option, add into element for display
                    LstOption.Items.Add(Beautify(v[0].ToString()));

            // Checking for the last option
            var nextStep = currentStep + 1;
            var end = false;
            if (nextStep == options.Length)
            {
                end = true;
            }
            else
            {
                // Check if there are any more option in next step, e.g. select(X,topup)
                // If there are none, it means that the current step is the last
                using (PlQuery q = new PlQuery("options(X," + options[nextStep] + ")"))
                    if (q.Solutions.Count() == 0)
                        end = true;
            }

            // If it is the last step, confirm button is to be displayed instead of next button
            if (end)
            {
                BtnNext.Visibility = Visibility.Collapsed;
                BtnConfirm.Visibility = Visibility.Visible;
            }
            // Else, confirm button is hidden and next button is to be displayed
            else
            {
                BtnNext.Visibility = Visibility.Visible;
                BtnConfirm.Visibility = Visibility.Collapsed;
            }

            // For first step back button is hidden, and subsequent steps back button
            if (currentStep == 0)
                BtnBack.Visibility = Visibility.Hidden;
            else
                BtnBack.Visibility = Visibility.Visible;

            // Clear any previous message
            TxbMessage.Text = "";
        }

        // Setup and display of order list
        private void DisplayOrders()
        {
            // Clear any previous order
            LblOrders.Content = "";
            foreach (var category in options)
            {
                // Get respective selected options from prolog, e.g. selected(X,bread)
                using (PlQuery q = new PlQuery("selected(X," + category + ")"))
                {
                    if (q.Solutions.Count() == 0)
                        continue;
                    // Display category of selected options
                    LblOrders.Content += Beautify(category) + "\n";
                    foreach (PlTermV v in q.Solutions)
                        // For each selected options, add into element for display
                        LblOrders.Content += " - " + Beautify(v[0].ToString()) + "\n";
                }
            }
        }

        // Proceed to previous step
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            // Decrement of currentStep to indicate proceed of previous step
            currentStep--;
            DisplayOptions();
        }

        // Adding of selected options into order list
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            // No selected item
            if (LstOption.SelectedItem == null)
                return;
            var selected = LstOption.SelectedItem.ToString();
            // Set selected option from prolog logic, e.g. select(ham)
            using (PlQuery q = new PlQuery("select(" + UnBeautify(selected) + ")"))
            {
                // Indication of successful retrieval shows that adding is successful
                var success = q.NextSolution();
                if (success)
                {
                    // Display success message
                    TxbMessage.Text = string.Format("'{0}' added successfully.", selected);
                    TxbMessage.Foreground = Brushes.Green;
                }
                else
                {
                    // Display error message
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

        // Format string to title case for display
        private string Beautify(string str)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(str.Replace('_', ' '));
        }

        // Format string to default format
        private string UnBeautify(string str)
        {
            return str.Replace(' ', '_').ToLower();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Clean up Prolog engine resources
            PlEngine.PlCleanup();
        }
    }
}
