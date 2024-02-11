using SAPbouiCOM;
using System;
using System.Diagnostics;
using System.IO;

namespace trust_payment_integration
{
    public class SalesOrderDataExtractor
    {
        private SAPbouiCOM.Application SBO_Application;
        

        public SalesOrderDataExtractor()
        {
            SboGuiApi sboGuiApi = new SboGuiApi();
            sboGuiApi.Connect("0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056");
            SBO_Application = sboGuiApi.GetApplication();
        }

        private void LogMessage(string message)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        public string ExtractDataFromCurrentSalesOrder()
        {
            try
            {
                SAPbouiCOM.Form oForm = SBO_Application.Forms.ActiveForm;

                if (oForm == null)
                {
                    SBO_Application.MessageBox("No active form found.");
                    LogMessage("No active form found.");
                    return null;
                }

                if (oForm.TypeEx != "139")
                {
                    SBO_Application.MessageBox("The active form is not a Sales Order form.");
                    LogMessage("The active form is not a Sales Order form.");
                    return null;
                }

                string docEntry = ((EditText)oForm.Items.Item("8").Specific).Value;
                string cardCode = ((EditText)oForm.Items.Item("4").Specific).Value;
                string billingCustomerFullName = ((EditText)oForm.Items.Item("54").Specific).Value;
                string billingAddress = ((EditText)oForm.Items.Item("6").Specific).Value;
                string totalAmountWithCurrency = ((EditText)oForm.Items.Item("29").Specific).Value;
               

                // Assuming the name is in 'FirstName LastName' format
                string[] nameParts = billingCustomerFullName.Split(new[] { ' ' }, 2);
                string firstName = nameParts.Length > 0 ? nameParts[0] : "";
                string lastName = nameParts.Length > 1 ? nameParts[1] : "";

                // Remove currency symbol from the amount
                string totalAmount = new string(totalAmountWithCurrency.SkipWhile(c => !char.IsDigit(c)).ToArray());
                string currency = new string(totalAmountWithCurrency.TakeWhile(c => !char.IsDigit(c) && c != '.' && c != ',').ToArray()).Trim();


                // Splitting the billing address using both newline and carriage return characters as delimiters
                var addressParts = billingAddress.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

                // Now, assign the address parts to the variables

                string billingStreet = addressParts.Length > 0 ? addressParts[0].Trim() : "";
                string billingTown = addressParts.Length > 1 ? addressParts[1].Trim() : "";
                string billingCounty = addressParts.Length > 2 ? addressParts[2].Trim() : "";
                string billingPostcode = addressParts.Length > 3 ? addressParts[3].Trim() : "";
                string billingCountry = addressParts.Length > 4 ? addressParts[4].Trim() : "";

                // Create HTML template
                string htmlTemplate = $@"
 <html>
  <body>
    <form method='POST' action='https://payments.securetrading.net/process/payments/choice'>
      <input type='hidden' name='sitereference' value='test_everedge119869'>
      <input type='hidden' name='stprofile' value='default'>
      <input type='hidden' name='currencyiso3a' value='{currency}'>
      <input type='hidden' name='mainamount' value='{totalAmount}'>
      <input type='hidden' name='version' value='2'>
       <input type='hidden' name='accounttypedescription' value='MOTO'>
      <input type='hidden' name='docEntry' value='{docEntry}'>
      <input type='hidden' name='cardCode' value='{cardCode}'>
      <input type='hidden' name='billingfirstname' value='{firstName}'>
      <input type='hidden' name='billinglastname' value='{lastName}'>
      <input type='hidden' name='billingstreet' value='{billingStreet}'>
      <input type='hidden' name='billingtown' value='{billingTown}'>
      <input type='hidden' name='billingcounty' value='{billingCounty}'>
      <input type='hidden' name='billingpostcode' value='{billingPostcode}'>
      <input type='hidden' name='billingcountry' value='{billingCountry}'>
      
        
      <input type=hidden name='ruleidentifier' value='STR-8'>
      <input type=hidden name='successfulurlnotification' value='https://dec1-85-236-147-134.ngrok-free.app/SalesOrder/payment_confirmation'>

      <input type='submit' value='Pay'>

     </form>
  </body>
</html>";


                // Replace placeholders in the HTML template
                string filledHtml = string.Format(htmlTemplate, docEntry, cardCode, firstName, lastName, totalAmount, billingStreet, billingTown, billingCounty, billingPostcode, billingCountry);

                return filledHtml;


            }
            catch (Exception ex)
            {
                SBO_Application.MessageBox($"Error: {ex.Message}");
                LogMessage($"Error during data extraction: {ex.Message}");
                return null;
            }
        }

    }
}