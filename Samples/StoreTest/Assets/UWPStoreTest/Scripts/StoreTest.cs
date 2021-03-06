﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Microsoft.UnityPlugins;

public class StoreTest : MonoBehaviour
{
    //public Button loadLicenseXmlButton;
    public Text text;

    private void prettyPrintErrors(string apiName, CallbackResponse response)
    {
        if(response.Status == CallbackStatus.Failure)
        {
            text.text = "\n" + apiName + "API call failed.\n" + response.Exception.Message + "\n" + text.text + "\n";
            Debug.LogError(apiName + " API call failed");

            if (response.Exception != null)
            {
                Debug.LogError(apiName + " Exception: " + response.Exception.Message);
            }

            Debug.LogError(apiName + " Exiting without doing anything");
        }
    }

    private void prettyPrintSuccess(string apiName, CallbackResponse response)
    {
        if (response.Status == CallbackStatus.Failure)
        {
            text.text = "\n" + apiName + "API call succeeded.\n" + text.text + "\n";
        }
    }

    public void LoadLicenseXmlButton()
    {
        Debug.Log("LoadLicenseXmlButton clicked");

        Store.LoadLicenseXMLFile((response) =>
        {
            Debug.Log("Invoking Callback");

            if (response.Status == CallbackStatus.Failure)
            {
                prettyPrintErrors("LoadLicenseXml", response);
                return;
            }

            prettyPrintSuccess("LoadLicenseXml", response);
        });
    }

    public void IsInTrialApp()
    {
        Debug.Log("Is In Trial button clicked");

#if UNITY_EDITOR
        prettyPrintErrors("IsInTrialApp", new CallbackResponse { Status = CallbackStatus.Failure, Exception = new System.Exception("Windows Store APIs cannot be called in Unity Editor")});
        return;
#endif

        if (Store.IsInTrialMode())
        {
            text.text = "\nYes! App is in trial mode" + text.text + "\n";
        }
        else
        {
            text.text = "\nNo! App is not in trial mode" + text.text + "\n";
        }
    }

    public void LoadListingInformationForApp()
    {
        Debug.Log("LoadListingInformation button clicked");

        Store.LoadListingInformation((response) =>
        {

            if (response.Status == CallbackStatus.Failure)
            {
                prettyPrintErrors("LoadListingInformation", response);
                return;
            }

            prettyPrintSuccess("LoadListingInformation", response);
            text.text = "\nFinished Loading Listing Information" + text.text + "\n";
            Debug.Log(response.Result.Description.ToString());
            foreach (var productListingKey in response.Result.ProductListings.Keys)
            {
                Debug.Log("Key: " + productListingKey + " value: " + response.Result.ProductListings[productListingKey].Name);
            }
        });
    }

    public void GetLicenseInformationForApp()
    {
        Debug.Log("Get License Information Button clicked");

#if UNITY_EDITOR
        Debug.Log("Windows Store APIs cannot be called from the Unity Editor");
        prettyPrintErrors("GetLicenseInformationForApp", new CallbackResponse { Status = CallbackStatus.Failure, Exception = new System.Exception("Windows Store APIs cannot be called in Unity Editor") });
        return;
#endif

        text.text = "\nGot License Information" + text.text + "\n";
        var licenseInformation = Store.GetLicenseInformation();
        Debug.Log("LicenseInformation: IsActive " + licenseInformation.IsActive.ToString());
        Debug.Log("LicenseInformation: IsTrial " + licenseInformation.IsTrial.ToString());
        Debug.Log("LicenseInformation: ExpirationDate " + licenseInformation.ExpirationDate.ToString());
        foreach (var productLicense in licenseInformation.ProductLicenses.Keys)
        {
            Debug.Log("Key: " + productLicense + " value: " + licenseInformation.ProductLicenses[productLicense].ProductId);
        }
    }

    public void GetProductLicenseForApp()
    {
        Debug.Log("Get product license clicked");


#if UNITY_EDITOR
        Debug.Log("Windows Store APIs cannot be called from the Unity Editor");
        prettyPrintErrors("GetProductLicenseForApp", new CallbackResponse { Status = CallbackStatus.Failure, Exception = new System.Exception("Windows Store APIs cannot be called in Unity Editor") });
        return;
#endif

        Store.GetProductLicense("Durable1");
        text.text = "\nGot product license" + text.text + "\n";
    }

    public void GetAppReceiptForApp()
    {
        Debug.Log("Get app receipt");

        Store.GetAppReceipt((response) =>
        {
            if (response.Status == CallbackStatus.Failure)
            {
                prettyPrintErrors("GetAppReceipt", response);
                return;
            }

            prettyPrintSuccess("GetAppReceipt", response);
            text.text = "\ngot app receipt" + text.text + "\n";
            Debug.Log("App Receipt: " + response.Result);
        });
    }

    public void GetProductReceiptForApp()
    {
        Debug.Log("Get product license clicked");

        Store.GetProductReceipt("Consumable1", ((response) =>
        {
            if (response.Status == CallbackStatus.Failure)
            {
                prettyPrintErrors("GetProductReceipt", response);
                return;
            }

            prettyPrintSuccess("GetProductReceipt", response);
            text.text = "\ngot product receipt" + text.text + "\n";
            Debug.Log("Product Receipt: " + response.Result);
        }));
    }

    public void LoadUnfulfilledConsumablesForApp()
    {
        Debug.Log("LoadUnfulfilledConsumablesForApp called");
        Store.LoadUnfulfilledConsumables((response) =>
        {
            if (response.Status == CallbackStatus.Failure)
            {
                prettyPrintErrors("LoadUnfulfilledConsumables", response);
                return;
            }

            prettyPrintSuccess("LoadUnfulfilledConsumables", response);
            Debug.Log("Loaded Unfulfilled consumables");
            foreach (var u in response.Result)
            {
                Debug.Log("offerId: " + u.OfferId + "transactionId: " + u.TransactionId);
            }
            text.text = "\nLoaded Unfulfilled consumables" + text.text + "\n";
        });
    }

    public void ReportConsumablesFulfilledForApp()
    {
        System.Guid guid= new System.Guid("00000001-0000-0000-0000-000000000000");
        Store.ReportConsumableFulfillment("consumable1", guid, (response) =>
        {
            if (response.Status == CallbackStatus.Failure)
            {
                prettyPrintErrors("ReportConsumablesFulfilled", response);
                return;
            }

            prettyPrintSuccess("ReportConsumableFulfillment", response);
            Debug.Log("Reported Consumable Fulfillment Result: " + response.Result.ToString());
            text.text = "\nReported Consumable fulfillment Result: " + response.Result.ToString() + text.text + "\n";
        });
    }

    public void RequestAppPurchaseForApp()
    {
        Debug.Log("RequestingAppPurchaseForApp begun");
        Store.RequestAppPurchase(true, (response) =>
        {
            if (response.Status == CallbackStatus.Success)
            {
                prettyPrintSuccess("RequestAppPurchase", response);
                text.text = "\nPurchase Succeeded";
                if (!string.IsNullOrEmpty(response.Result))
                {
                    Debug.Log("App Purchase Receipt: " + response.Result);
                }
            }
            else
            {
                prettyPrintErrors("RequestAppPurchase", response);
                text.text = "\nPurchase failed" + text.text + "\n";
            }
        });
    }

    public void RequestProductPurchaseForApp()
    {
        Debug.Log("RequestinProductpPurchaseForApp begun");
        Store.RequestProductPurchase("Consumable1", (response) =>
        {
            if (response.Status == CallbackStatus.Failure)
            {
                prettyPrintErrors("RequestProductPurchase", response);
                return;
            }

            prettyPrintSuccess("RequestProductPurchase", response);
            text.text = "\nPurchase status: " + response.Result.Status.ToString() + text.text + "\n";
            Debug.Log("Purchase Status: " + response.Result.Status.ToString());
            Debug.Log("Purchase OfferId: " + response.Result.OfferId);
            Debug.Log("Purchase ReceiptXml: " + response.Result.ReceiptXml);

            //Store.VerifyReceipt(response.Result.ReceiptXml, (ReceiptResponse) =>
            //{
            //    Debug.Log("response: " + ReceiptResponse.Result.AppId);
            //    Debug.Log("response: " + ReceiptResponse.Result.IsValidReceipt.ToString());
            //});
        });
    }
}
