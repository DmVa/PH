using PH.Data;
using PH.Helpers;
using PH.Helpers.Win;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PH.Win.ViewModels
{
    public class MainWindowViewModel: BasePropertyChanged
    {
        
        private DateTime _currentDate;
        private List<UserData> _users;
        private BaseCommand _loadCommand = null;
        private bool _commandInProgress;
        private DataService _dataService;
        private List<BaseCommand> _commandsList;
        private List<WorklogLine> _userWorklog;
        private UserData _selectedUser;
        Dictionary<string, List<WorklogLine>> _teamWorklog;
        public DateTime CurrentDate
        {
            get { return _currentDate; }
            set
            {
                _currentDate = value;
                RaisePropertyChanged();
            }
        }

        public List<UserData> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                RaisePropertyChanged();
               
            }
        }

      
        public UserData SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                RaisePropertyChanged();
                SetSelectedUserWorklog();

            }
        }

        private void SetSelectedUserWorklog()
        {
            if (_selectedUser == null)
            {
                UserWorklog = null;
            }
            else
            {

                if (_teamWorklog != null)
                {
                    if (_teamWorklog.ContainsKey(_selectedUser.Key))
                    {
                        UserWorklog = _teamWorklog[SelectedUser.Key];
                    }
                    else
                    {
                        UserWorklog = null;
                    }
                }
                else
                {
                    UserWorklog = null;
                }
            }
        }

        public List<WorklogLine> UserWorklog
        {
            get { return _userWorklog; }
            set
            {
                _userWorklog = value;
                RaisePropertyChanged();

            }
        }

        public async void Init(DataService dataService)
        {
            _dataService = dataService;
            CurrentDate = dataService.GetDefaultDate();
            CreateCommands();
            Users = await _dataService.LoadUsersAsync();
        }

        #region "Commands"
        public ICommand LoadCommand
        {
            get { return _loadCommand; }
        }

   

        #endregion

        private void CreateCommands()
        {
            _commandsList = new List<BaseCommand>();
            _loadCommand = new BaseCommand(DoLoadData, CanRunCommands, SetCommandInProgress);
            _commandsList.Add(_loadCommand);
        }

        private void SetCommandInProgress(bool value)
        {
            _commandInProgress = value;
            foreach (var cmd in _commandsList)
                cmd.RaiseCanExecuteChanged(null);
        }

        private bool CanRunCommands()
        {
            return !_commandInProgress;
        }

        private void DoLoadData()
        {
            SelectedUser = null;
            _teamWorklog = _dataService.GetTeamWorklog(_currentDate, _currentDate);
        }
    }
}
