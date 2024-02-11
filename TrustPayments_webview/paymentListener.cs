using SAPbouiCOM;
using System;

namespace TrustPayments_webview
{
    public class SapFieldUpdater
    {
        private SAPbouiCOM.Application SBO_Application;

        public SapFieldUpdater()
        {
            SboGuiApi sboGuiApi = new SboGuiApi();
            sboGuiApi.Connect("0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056");
            SBO_Application = sboGuiApi.GetApplication();
        }

        public void UpdateFieldInForm(string formType, string itemUID, string value)
        {
            try
            {
                SAPbouiCOM.Form oForm = SBO_Application.Forms.ActiveForm;

                if (oForm == null || oForm.TypeEx != formType)
                {
                    SBO_Application.MessageBox($"The active form is not the correct type ({formType}).");
                    return;
                }

                Item item = oForm.Items.Item(itemUID);
                if (item == null)
                {
                    SBO_Application.MessageBox($"Item with UID {itemUID} not found.");
                    return;
                }

                EditText editText = item.Specific as EditText;
                if (editText == null)
                {
                    SBO_Application.MessageBox($"Item with UID {itemUID} is not an EditText.");
                    return;
                }

                editText.Value = value;
            }
            catch (Exception ex)
            {
                SBO_Application.MessageBox($"Error updating field: {ex.Message}");
            }
        }
    }
}
