using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cards.Core.Utilities
{
    public static class AcrDialogUtilities
    {
        public static async Task<string> promptInput(string message)
        {
            PromptConfig prompt = new PromptConfig();
            prompt.SetCancelText("Cancel");
            prompt.SetOkText("Accept");
            prompt.SetMessage(message);
            prompt.SetInputMode(InputType.Default);

            PromptResult promptResult = await UserDialogs.Instance.PromptAsync(prompt);

            if (promptResult.Text == null) return ""; 

                return promptResult.Text;

        }

        public static async Task<bool> promptConfirm(string message)
        {
            var result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
            {
                Message = message,
                OkText = "Yes",
                CancelText = "No"
            });
            return result; 
        }
    }
}
