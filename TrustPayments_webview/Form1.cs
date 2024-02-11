using System;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using trust_payment_integration; // Ensure this namespace is correct

namespace TrustPayments_webview
{
    public partial class Form1 : Form
    {
        private const string SpecificUrl = "https://payments.securetrading.net/process/payments/details"; // Replace with the specific URL you're looking for
        private SalesOrderDataExtractor extractor;
        private SapFieldUpdater field_mapper;

        private string conn_string;

        public Form1()
        {
            //conn_string = connection_string;
            //MessageBox.Show(conn_string);
            InitializeComponent();
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await webPage.EnsureCoreWebView2Async(null);
            LoadHtmlFromExtractor();
            webPage.CoreWebView2.NavigationCompleted += WebView_NavigationCompleted;
        }

        private void LoadHtmlFromExtractor()
        {
            
            extractor = new SalesOrderDataExtractor(); //pass the connection string here
            
            
            string htmlContent = extractor.ExtractDataFromCurrentSalesOrder();

            if (!string.IsNullOrEmpty(htmlContent))
            {
                webPage.NavigateToString(htmlContent);
            }
            else
            {
                MessageBox.Show("No HTML content to display.");
            }
        }

        private async void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            field_mapper = new SapFieldUpdater(); //pass the connection string here
            if (e.IsSuccess && webPage.Source.ToString() == SpecificUrl)
            {
                string script = "document.querySelector('#st-transactionreference-value').innerText;"; // Adjust as needed

                try
                {
                    var result = await webPage.CoreWebView2.ExecuteScriptAsync(script);
                    var extractedValue = System.Text.Json.JsonSerializer.Deserialize<string>(result);
                    field_mapper.UpdateFieldInForm ("139", "BOYX_11", extractedValue);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error extracting value: " + ex.Message);
                }
            }
        }

        private void webPage_Click(object sender, EventArgs e)
        {
            // Code to handle the click event of webPage
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Code to execute when the form loads
        }
    }
}
