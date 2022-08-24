using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using FestpunktDB.Business.DataServices;

namespace FestpunktDB.GUI
{
    /// <summary>
    /// Interaction logic for AuthenticationWindow.xaml
    /// </summary>
    public partial class AuthenticationWindow : Window
    {
        public AuthenticationWindow()
        {
            InitializeComponent();
            _worker = new BackgroundWorker { WorkerReportsProgress = true };
            _worker.DoWork += worker_AuthenticateUser;
            _worker.ProgressChanged += worker_ProgressChanged;
            _worker.RunWorkerCompleted += worker_DatabaseScanned;
        }

        private string _usernameInput;
        private readonly BackgroundWorker _worker;

        #region events
        private void ConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsernameInput.Text == string.Empty)
            {
                MessageBox.Show("Bitte geben Sie einen Benutzernamen ein.");
                return;
            }
            _usernameInput = UsernameInput.Text;
            MwLoadProgress.Value = 3;
            _worker.RunWorkerAsync();
        }

        private void UsernameInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (UsernameInput.Text == string.Empty)
            {
                MessageBox.Show("Bitte geben Sie einen Benutzernamen ein.");
                return;
            }
            _usernameInput = UsernameInput.Text;
            MwLoadProgress.Value = 3;
            _worker.RunWorkerAsync();
        }

        /// <summary>
        /// Scan the user database for a match.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void worker_AuthenticateUser(object sender, DoWorkEventArgs e)
        {
            using var db = new UserDatabaseContext();
            var count = db.Userverwaltung.Count();
            var progressPercentage = 10;
            ((BackgroundWorker)sender).ReportProgress(10);
            foreach (var user in db.Userverwaltung)
            {
                progressPercentage += 75 / count;
                if (user.Username.Equals(_usernameInput, StringComparison.OrdinalIgnoreCase))
                {
                    ((BackgroundWorker) sender).ReportProgress(90);
                    e.Result = user.Status;
                    break;
                }

                ((BackgroundWorker)sender).ReportProgress(progressPercentage);
            }

            ((BackgroundWorker)sender).ReportProgress(0);
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MwLoadProgress.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// Show message or open appropriate window based on authenticating result.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void worker_DatabaseScanned(object sender, RunWorkerCompletedEventArgs e)
        {
            if (string.IsNullOrEmpty((string) e.Result))
            {
                MessageBox.Show("Username existiert nicht.\nBitte versuchen Sie es erneut.");
                return;
            }

            var main = new MainWindow();
            main.Show();
            if ((string)e.Result == "Gast") main.EditTabItem.Visibility = Visibility.Collapsed;
            Close();
        }
        #endregion

    }
}
