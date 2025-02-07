using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
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
        if (showRegisterPasswordToggle != null)
        {
            showRegisterPasswordToggle.isOn = false;
            showRegisterPasswordToggle.onValueChanged.AddListener(ToggleShowRegisterPassword);
        }

        if (showConfirmPasswordToggle != null)
        {
            showConfirmPasswordToggle.isOn = false;
            showConfirmPasswordToggle.onValueChanged.AddListener(ToggleShowConfirmPassword);
        }

        if (showPasswordToggle != null)
        {
            showPasswordToggle.isOn = false;
            showPasswordToggle.onValueChanged.AddListener(ToggleShowPassword);
        }

        rememberMeToggle.isOn = false;
        LoadRememberMeData();

        AutoLoginIfRemembered();
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

    /*--------------------| Scene Management |--------------------*/
    public void GoToRegisterScene()
    {
        SceneManager.LoadScene("RegisterScene");
    }

    public void GoToLoginScene()
    {
        SceneManager.LoadScene("RegLogScene");
    }

    public void GoToMenuScene()
    {
        SceneManager.LoadScene("Menu");
    }

    public void GoToGlobalLeaderboardScene()
    {
        SceneManager.LoadScene("GlobalLeaderboard");
    }

    public void GoToNearbyLeaderboardScene()
    {
        SceneManager.LoadScene("NearbyLeaderboard");
    }

    public void GoToShopScene()
    {
        SceneManager.LoadScene("Shop");
    }

    public void GoToInventoryScene()
    {
        SceneManager.LoadScene("Inventory");
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
        string playFabId = result.PlayFabId;

        UpdateMessage(loginMessage, $"Login Success! Welcome, {displayName} ({playFabId})");

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

        SceneManager.LoadScene("Menu");
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
        string guestName = $"Guest_{result.PlayFabId}";
        UpdateMessage(loginMessage, $"Guest Login Successful! Welcome, {guestName}");

        SceneManager.LoadScene("Menu");
    }

    void LoadRememberMeData()
    {
        if (PlayerPrefs.GetInt("RememberMe", 0) == 1)
        {
            emailUsernameField.text = PlayerPrefs.GetString("SavedEmailUsername", "");
            passwordField.text = PlayerPrefs.GetString("SavedPassword", "");
            rememberMeToggle.isOn = true;
        }
        else
        {
            rememberMeToggle.isOn = false;
        }

        passwordField.contentType = TMP_InputField.ContentType.Password;
        passwordField.ForceLabelUpdate();
    }

    void AutoLoginIfRemembered()
    {
        if (PlayerPrefs.GetInt("RememberMe", 0) == 1)
        {
            LoginUser();
        }
    }

    public void Logout()
    {
        PlayerPrefs.SetInt("RememberMe", 0);
        PlayerPrefs.DeleteKey("SavedEmailUsername");
        PlayerPrefs.DeleteKey("SavedPassword");
        SceneManager.LoadScene("RegLogScene");
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
