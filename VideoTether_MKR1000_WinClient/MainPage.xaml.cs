using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace VideoTether_MKR1000_WinClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void NumericTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            var tb = (sender as TextBox);
            if (tb != null) {
                var nonNumericRegEx = new Regex(@"[^\d\.-]");
                tb.Text = nonNumericRegEx.Replace(tb.Text, "");
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            bool postSuccessful = false;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var values = new Dictionary<string, string> {
                        { "distance", this.Distance.Text },
                        { "distance_mm_multiplier", ((ComboBoxItem)this.Distance_mm_multiplier.SelectedItem).Tag.ToString() },
                        { "duration", this.Duration.Text },
                        { "duration_sec_multiplier", ((ComboBoxItem)this.Duration_sec_multiplier.SelectedItem).Tag.ToString() },
                    };
                    using (FormUrlEncodedContent content = new FormUrlEncodedContent(values))
                    {
                        client.Timeout = TimeSpan.FromSeconds(30);
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("multipart/form-data"));

                        // Posting data to device...

                        HttpResponseMessage result = client.PostAsync(this.Address.Text, content).Result;
                        postSuccessful = result.IsSuccessStatusCode;

                        if (!postSuccessful) {
                            ShowError(string.Format("Posting Failed ({0} {1})", (int)result.StatusCode, result.ReasonPhrase));
                        }
                    }
                }
                catch (Exception ex)
                {
                    postSuccessful = false;
                    ShowError("Error: " + ex.Message);
                }

                if (postSuccessful) {
                    ClearErrorMessage();
                }
            }
        }

        private void ClearErrorMessage() {
            ShowError("");
        }

        private void ShowError(string message) {
            this.Error.Text = message;
        }
    }
}
