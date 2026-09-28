using cribbage_Royale_PROYECTO.DataAccess;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.Session;
using cribbage_Royale_PROYECTO.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class BlockedPlayersPage : Page
    {
        private readonly BlockRepository _blockRepository;
        private readonly UserRepository _userRepository;
        private readonly int _currentAccountId;

        public BlockedPlayersPage()
        {
            InitializeComponent();
            _blockRepository = new BlockRepository();
            _userRepository = new UserRepository();
            _currentAccountId = _userRepository.GetUserId(CurrentSession.Username);
            LoadBlockedPlayers();
        }

        private void LoadBlockedPlayers()
        {
            messagePanel.HidePanel();
            try
            {
                List<BlockedPlayerSummary> blockedPlayers = _blockRepository.GetBlockedPlayers(_currentAccountId);
                blockedPlayersDataGrid.ItemsSource = blockedPlayers;
                if (blockedPlayers.Count == 0)
                {
                    messagePanel.ShowInformation(Lang.BlockedPlayers_NoBlocksMessage);
                }
            }
            catch (Exception dataAccessException)
            {
                MessageDialog.ShowError(Window.GetWindow(this), Lang.Dialog_DatabaseAccessErrorMessage);
            }
        }

        private void UnblockButton_Click(object sender, RoutedEventArgs e)
        {
            Button unblockButton = (Button)sender;
            BlockedPlayerSummary selectedPlayer = (BlockedPlayerSummary)unblockButton.DataContext;
            bool confirmed = MessageDialog.ShowConfirmation(Window.GetWindow(this), string.Format(Lang.Dialog_ConfirmUnblockMessageFormat, selectedPlayer.DisplayName));
            if (confirmed)
            {
                try
                {
                    _blockRepository.UnblockPlayer(_currentAccountId, selectedPlayer.AccountId);
                    MessageDialog.ShowSuccess(Window.GetWindow(this), Lang.Dialog_UnblockSuccessMessage);
                    LoadBlockedPlayers();
                }
                catch (Exception dataAccessException)
                {
                    MessageDialog.ShowError(Window.GetWindow(this), Lang.Dialog_DatabaseAccessErrorMessage);
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}