using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class PfUserMgt : MonoBehaviour
{
    [SerializeField] TMP_Text msgbox;
    [SerializeField] TMP_InputField if_username, if_email, if_password;

    public void OnButtonRegUser() // For button click
    {
        var reqReq = new RegisterPlayFabUserRequest // Create request object
        {
            Email = if_email.text,
            Password = if_password.text,
            Username = if_username.text,
        };
        // Execute request by calling PlayFab API
        PlayFabClientAPI.RegisterPlayFabUser(reqReq, OnRegSucc, OnError);
    }

    public void OnButtonLogin()
    {
        var loginReq = new LoginWithEmailAddressRequest
        {
            Email = if_email.text,
            Password = if_password.text,
        };
        PlayFabClientAPI.LoginWithEmailAddress(loginReq, OnLoginSucc, OnError);
    }

    void OnLoginSucc(LoginResult r)
    {
        msgbox.text = "Login Success!" + r.PlayFabId;
        LoadLevel();
    }

    void OnRegSucc(RegisterPlayFabUserResult r)
    {
        msgbox.text = "Register Success!" + r.PlayFabId;
    }

    void OnError(PlayFabError e) // Function to handle error
    {
        msgbox.text = "Error" + e.GenerateErrorReport();
    }

    void LoadLevel()
    {
        SceneManager.LoadScene("InventoryScene");
    }

    public void OnLogOut()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        SceneManager.LoadScene("RegLoginScene");
    }

    public void OnResetPassword()
    {
        var ResetPassReq = new SendAccountRecoveryEmailRequest
        {
            Email = if_email.text,
            TitleId = PlayFabSettings.TitleId
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(ResetPassReq, onResetPassSucc, OnError);
    }

    void onResetPassSucc(SendAccountRecoveryEmailResult r)
    {
        msgbox.text = "Recovery email sent! Please check your email";
    }

    void UpdateMsg (string msg)
    {
        msgbox.text = msg;
    }

    public void ClientGetTitleData()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(),
            result =>
            {
                if (result.Data == null || result.Data.ContainsKey("MOTD")) UpdateMsg("No MOTD");
                else UpdateMsg("MOTD: " + result.Data["MOTD"]);
            },
            error =>
            {
                UpdateMsg("Got error getting titleData:");
                UpdateMsg(error.GenerateErrorReport());
            }
        );
    }
}
