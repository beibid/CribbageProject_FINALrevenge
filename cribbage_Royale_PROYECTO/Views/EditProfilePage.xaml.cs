using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CribbageRoyale.Contracts.DataContracts;
using Microsoft.Win32;
using cribbage_Royale_PROYECTO.Localization;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.ServiceAccess;
using cribbage_Royale_PROYECTO.Session;
using cribbage_Royale_PROYECTO.Views.Dialogs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class EditProfilePage : Page
    {
        private readonly PlayerProfileClient _profileClient;
        private readonly OwnProfile _profile;

        private byte[] _newAvatarImage;

        public EditProfilePage(OwnProfile profile)
        {
            InitializeComponent();
            _profileClient = ((App)Application.Current).PlayerProfileClient;
            _profile = profile;
            _newAvatarImage = new byte[0];
            usernameTextBlock.Text = profile.UserName;
            displayNameTextBox.Text = profile.DisplayName;
            AvatarImage.ShowIn(avatarEllipse, profile.AvatarImage);
        }

        private void ChangePictureButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = Lang.EditProfile_ImageFileFilter;

            if (dialog.ShowDialog(Window.GetWindow(this)) == true)
            {
                LoadNewAvatar(dialog.FileName);
            }
        }

        private void LoadNewAvatar(string path)
        {
            try
            {
                _newAvatarImage = AvatarImage.CreateFromFile(path);
                AvatarImage.ShowIn(avatarEllipse, _newAvatarImage);
            }
            catch (NotSupportedException)
            {
                ShowImageLoadError();
            }
            catch (FileFormatException)
            {
                ShowImageLoadError();
            }
            catch (IOException)
            {
                ShowImageLoadError();
            }
            catch (UnauthorizedAccessException)
            {
                ShowImageLoadError();
            }
        }

        private void ShowImageLoadError()
        {
            _newAvatarImage = new byte[0];
            MessageDialog.ShowWarning(Window.GetWindow(this), Lang.EditProfile_ImageLoadErrorMessage);
        }

        private async void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            string displayName = displayNameTextBox.Text.Trim();
            bool isNameChanged = displayName != _profile.DisplayName;
            bool isImageChanged = _newAvatarImage.Length > 0;

            if (string.IsNullOrEmpty(displayName))
            {
                MessageDialog.ShowWarning(Window.GetWindow(this), Lang.Common_EmptyFieldsMessage);
            }
            else if (!isNameChanged && !isImageChanged)
            {
                MessageDialog.ShowWarning(Window.GetWindow(this), Lang.EditProfile_NoChangesMessage);
            }
            else
            {
                await SaveChangesAsync(displayName, isNameChanged, isImageChanged);
            }
        }

        private async Task SaveChangesAsync(string displayName, bool isNameChanged, bool isImageChanged)
        {
            SetButtonsEnabled(false);

            ProfileResult result = new ProfileResult(ServiceFailureReason.None, null, new OwnProfile());

            if (isNameChanged)
            {
                result = await _profileClient.UpdateDisplayNameAsync(CurrentSession.SessionToken, displayName);
            }

            if (result.Succeeded && isImageChanged)
            {
                result = await _profileClient.UpdateAvatarAsync(CurrentSession.SessionToken, _newAvatarImage);
            }

            SetButtonsEnabled(true);
            ShowSaveResult(result);
        }

        private void ShowSaveResult(ProfileResult result)
        {
            if (result.Succeeded)
            {
                MessageDialog.ShowSuccess(Window.GetWindow(this), Lang.EditProfile_SaveSuccessMessage);
                NavigationService.Navigate(new ProfilePage());
            }
            else
            {
                MessageDialog.ShowWarning(Window.GetWindow(this), ProfileMessages.Resolve(result));
            }
        }

        private void SetButtonsEnabled(bool isEnabled)
        {
            changePictureButton.IsEnabled = isEnabled;
            saveChangesButton.IsEnabled = isEnabled;
            cancelButton.IsEnabled = isEnabled;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            bool discard = MessageDialog.ShowConfirmation(
                Window.GetWindow(this),
                Lang.EditProfile_DiscardConfirmationMessage);

            if (discard)
            {
                NavigationService.Navigate(new ProfilePage());
            }
        }
    }
}