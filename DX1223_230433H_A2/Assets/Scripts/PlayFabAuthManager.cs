using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayFabAuthManager : MonoBehaviour
{
    [Header("Login Panel")]
    [SerializeField] TMP_InputField emailUsernameField, passwordField;
    [SerializeField] Toggle rememberMeToggle, showPasswordToggle;
    [SerializeField] TextMeshProUGUI loginMessage;

    [Header("Register Panel")]
    [SerializeField] TMP_InputField displayNameField, usernameField, emailField, passwordRegisterField, confirmPasswordField;
    [SerializeField] Toggle showRegisterPasswordToggle, showConfirmPasswordToggle;
    [SerializeField] TextMeshProUGUI registerMessage;

    void Start()
    {
        LoadRememberMeData();

        if (showPasswordToggle != null)
            showPasswordToggle.onValueChanged.AddListener(ToggleShowPassword);

        if (showRegisterPasswordToggle != null)
            showRegisterPasswordToggle.onValueChanged.AddListener(ToggleShowRegisterPassword);

        if (showConfirmPasswordToggle != null)
            showConfirmPasswordToggle.onValueChanged.AddListener(ToggleShowConfirmPassword);
    }

    void UpdateMessage(TextMeshProUGUI messageField, string msg)
    {
        if (messageField != null)
        {
            Debug.Log(msg);
            messageField.text = msg;
        }
    }

    void OnError(PlayFabError error, TextMeshProUGUI messageField)
    {
        UpdateMessage(messageField, "Error: " + error.GenerateErrorReport());
    }

    // **Scene Management**
    public void GoToRegisterScene()
    {
        SceneManager.LoadScene("RegisterScene");
    }

    public void GoToLoginScene()
    {
        SceneManager.LoadScene("RegLogScene");
    }

    public void RegisterUser()
    {
        if (passwordRegisterField.text != confirmPasswordField.text)
        {
            UpdateMessage(registerMessage, "Passwords do not match!");
            return;
        }

        var registerRequest = new RegisterPlayFabUserRequest
        {
            Email = emailField.text,
            Password = passwordRegisterField.text,
            Username = usernameField.text
        };

        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, error => OnError(error, registerMessage));
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        UpdateMessage(registerMessage, "Registration successful!");

        var displayNameRequest = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayNameField.text
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(displayNameRequest, r => UpdateMessage(registerMessage, "Display Name Updated!"), error => OnError(error, registerMessage));
    }

    public void LoginUser()
    {
        if (emailUsernameField.text.Contains("@"))
        {
            LoginWithEmail();
        }
        else
        {
            LoginWithUsername();
        }
    }

    void LoginWithEmail()
    {
        var loginRequest = new LoginWithEmailAddressRequest
        {
            Email = emailUsernameField.text,
            Password = passwordField.text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams { GetPlayerProfile = true }
        };

        PlayFabClientAPI.LoginWithEmailAddress(loginRequest, OnLoginSuccess, error => OnError(error, loginMessage));
    }

    void LoginWithUsername()
    {
        var loginRequest = new LoginWithPlayFabRequest
        {
            Username = emailUsernameField.text,
            Password = passwordField.text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams { GetPlayerProfile = true }
        };

        PlayFabClientAPI.LoginWithPlayFab(loginRequest, OnLoginSuccess, error => OnError(error, loginMessage));
    }

    void OnLoginSuccess(LoginResult result)
    {
        string displayName = result.InfoResultPayload.PlayerProfile?.DisplayName ?? "No Display Name";
        UpdateMessage(loginMessage, "Login Success! Welcome, " + displayName);

        if (rememberMeToggle.isOn)
        {
            PlayerPrefs.SetInt("RememberMe", 1);
            PlayerPrefs.SetString("SavedEmailUsername", emailUsernameField.text);
            PlayerPrefs.SetString("SavedPassword", passwordField.text);
        }
        else
        {
            PlayerPrefs.SetInt("RememberMe", 0);
            PlayerPrefs.DeleteKey("SavedEmailUsername");
            PlayerPrefs.DeleteKey("SavedPassword");
        }
    }

    public void GuestLogin()
    {
        string customId = SystemInfo.deviceUniqueIdentifier; // Use device ID as Guest ID

        var loginRequest = new LoginWithCustomIDRequest
        {
            CustomId = customId,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(loginRequest, OnGuestLoginSuccess, error => OnError(error, loginMessage));
    }

    void OnGuestLoginSuccess(LoginResult result)
    {
        UpdateMessage(loginMessage, "Guest Login Successful! Welcome.");
    }

    void LoadRememberMeData()
    {
        if (PlayerPrefs.HasKey("RememberMe") && PlayerPrefs.GetInt("RememberMe") == 1)
        {
            rememberMeToggle.isOn = true;
            emailUsernameField.text = PlayerPrefs.GetString("SavedEmailUsername", "");
            passwordField.text = PlayerPrefs.GetString("SavedPassword", "");
        }
    }

    public void ForgotPassword()
    {
        if (emailUsernameField == null || string.IsNullOrEmpty(emailUsernameField.text))
        {
            UpdateMessage(loginMessage, "Please enter your email to reset the password.");
            return;
        }

        var request = new SendAccountRecoveryEmailRequest
        {
            Email = emailUsernameField.text,
            TitleId = PlayFabSettings.TitleId
        };

        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, error => OnError(error, loginMessage));
    }

    void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
        UpdateMessage(loginMessage, "Password reset email sent! Check your inbox.");
    }


    void ToggleShowPassword(bool isChecked)
    {
        passwordField.contentType = isChecked ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        passwordField.ForceLabelUpdate();
    }

    void ToggleShowRegisterPassword(bool isChecked)
    {
        passwordRegisterField.contentType = isChecked ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        passwordRegisterField.ForceLabelUpdate();
    }

    void ToggleShowConfirmPassword(bool isChecked)
    {
        confirmPasswordField.contentType = isChecked ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        confirmPasswordField.ForceLabelUpdate();
    }


}
