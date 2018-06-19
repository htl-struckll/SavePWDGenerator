using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace SavePWDGenerator_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region var´s

        private const string BiblioPath = @"Data\biblio.bin";
        private const string PwdDictPath = @"Data\pwd.dict";

        private static List<string> _specChars;
        private static List<string> _biblioList;
        private static List<string> _pwdsList;
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            _biblioList = LoadBiblioList(BiblioPath);
            _specChars = LoadSpecChars();
            _pwdsList = LoadPwdList(PwdDictPath);
        }

        #region Data

        /// <summary>
        /// Loads the dictionary of passwords
        /// </summary>
        /// <param name="path">Path of dictionary</param>
        /// <returns>List of pwds</returns>
        private List<string> LoadPwdList(string path)
        {
            List<string> retVal = new List<string>();

            try
            {
                string[] lines;

                using (StreamReader reader = new StreamReader(path))
                {
                    lines = reader.ReadToEnd().Split('\r');
                }
                foreach (string line in lines)
                {
                    retVal.Add(line);
                }

            }
            catch (Exception ex)
            {
                Log(ex.Message, "Error");
            }

            return retVal;
        }

        /// <summary>
        /// Fills the specChar with Spechial chars
        /// </summary>
        /// <returns></returns>
        private List<string> LoadSpecChars()
        {
            List<string> retVal = new List<string>();

            retVal.Add("&");
            retVal.Add(")");
            retVal.Add("§");
            retVal.Add("%");
            retVal.Add("(");

            return retVal;
        }

        /// <summary>
        /// Loads the Dictionary
        /// </summary>
        /// <param name="path">Path of Dictionary</param>
        /// <returns>The Dictionary</returns>
        private List<string> LoadBiblioList(string path)
        {
            List<string> retVal = new List<string>();

            try
            {
                string[] lines;

                using (StreamReader reader = new StreamReader(path))
                {
                    lines = reader.ReadToEnd().Split('\r');
                }
                foreach (string line in lines)
                {
                    retVal.Add(line);
                }

            }
            catch (Exception ex)
            {
                Log(ex.Message, "Error");
            }

            return retVal;
        }

        #endregion


        private void Generate_Click(object sender, RoutedEventArgs e) => Start();
             
        /// <summary>
        /// Starts the generate prozess and Calcs Brut/Dict
        /// </summary>
        private void Start()
        {
            PwdOutput.Text = Generate();
            BrutforceOutput.Text = GetBrutforceDate(PwdOutput.Text).ToString(CultureInfo.InvariantCulture);
            PossibilitysOutput.Text = PwdPossibilitys(PwdOutput.Text).ToString(CultureInfo.InvariantCulture);
            IsDictionaryOutput.Text = IsDictionaryHackable(PwdOutput.Text).Equals(true) ? "Yes" : "No";
        }

        /// <summary>
        /// Checks if pwd is attackable by dictionary
        /// </summary>
        /// <param name="pwd">Password</param>
        /// <returns>Is attackable</returns>
        private bool IsDictionaryHackable(string pwd)
        {
            foreach (string pwdDict in _pwdsList)
            {
                if (pwd.Equals(pwdDict))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Get all Pwd possible combinations
        /// </summary>
        /// <param name="pwd">Password</param>
        /// <returns>all possibilitys</returns>
        private double PwdPossibilitys(string pwd) => Math.Pow(pwd.Length, 36);


        /// <summary>
        /// Gets the estimated time to Brutforce your password
        /// </summary>
        /// <param name="pwd">Password</param>
        /// <returns>Estimated time</returns>
        private DateTime GetBrutforceDate(string pwd)
        {
            DateTime retVal;
            switch (pwd.Length)
            {
                case 7:
                    retVal =  DateTime.Now.AddMilliseconds(29);
                    break;
                case 8:
                    retVal = DateTime.Now.AddHours(5);
                    break;
                case 9:
                    retVal = DateTime.Now.AddDays(5);
                    break;
                case 10:
                    retVal = DateTime.Now.AddMonths(4);
                    break;
                case 11:
                    retVal = DateTime.Now.AddYears(10);
                    break;
                case 12:
                    retVal = DateTime.Now.AddYears(200);
                    break;
                default:
                    retVal = DateTime.Now.AddYears(1000);
                    break;
            }

            return retVal.AddDays(new Random().Next(0, 10)).AddHours(new Random().Next(0, 24))
                .AddSeconds(new Random().Next(0, 60)).AddMinutes(new Random().Next(0, 60)).AddMonths(new Random().Next(0,10));
        }

        /// <summary>
        /// Generates pwd
        /// </summary>
        /// <returns>Generated pwd</returns>
        private string Generate()
        {
            Random rnd = new Random();

            string retPwd = (_biblioList[rnd.Next(0, _biblioList.Count)] + _biblioList[rnd.Next(0, _biblioList.Count)] + _biblioList[rnd.Next(0,_biblioList.Count)] + _biblioList[rnd.Next(0,_biblioList.Count)]).Trim();
            retPwd = retPwd.Insert(rnd.Next(1,retPwd.Length-1), _specChars[rnd.Next(0,_specChars.Count)]);

            return retPwd;
        }


        /// <summary>
        /// Simple logging Method
        /// </summary>
        /// <param name="msg">Message to display</param>
        /// <param name="caption">Caption of window</param>
        static void Log(string msg, string caption = "Info") =>
            MessageBox.Show("[" + DateTime.Now + "] " + msg, caption);
    }
}
