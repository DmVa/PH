﻿using PH.Config;
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
        private bool _commandInProgress;
        private DataService _dataService;
        private ConfigManager _configManger;
        
        private UserData _selectedUser;
        
        private List<UserData> _users;
        private List<IssueData> _issues;
        private List<WorklogData> _worklogs;

        Dictionary<string, List<WorklogLine>> _teamWorklog;
        Dictionary<string, List<IssueUserStoryData>> _teamIssues;
        private DateTime _worklogFromDate;
        private DateTime _worklogToDate;

        private List<BaseCommand> _commandsList;
        private BaseCommand _saveSettingsCommand;
        private BaseCommand _loadUsersCommand;
        private BaseCommand _loadSprintTasksCommand;
        private BaseCommand _loadWorklogCommand;

        public DateTime WorklogFromDate
        {
            get { return _worklogFromDate; }
            set
            {
                _worklogFromDate = value;
                RaisePropertyChanged();
            }
        }
        public DateTime WorklogToDate
        {
            get { return _worklogToDate; }
            set
            {
                _worklogToDate = value;
                RaisePropertyChanged();
            }
        }
        public DataLoadSettings Settings
        {
            get { return _configManger.Settings; }
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
                RaisePropertyChanged("UserWorklog");
                RaisePropertyChanged("UserStories");
            }
        }

        /// <summary>
        /// This is list is shows worklog for selected user.
        /// </summary>
        public List<WorklogLine> UserWorklog
        {
            get
            {
                string key = _selectedUser?.Key;
                if (string.IsNullOrEmpty(key))
                    key = _dataService.GetAllUsersKey();

                if (_teamWorklog != null)
                {
                    if (_teamWorklog.ContainsKey(key))
                    {
                        return _teamWorklog[key];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }

            }
        }

        /// <summary>
        /// This is list is shows userstories for selected user.
        /// </summary>
        public List<IssueUserStoryData> UserStories
        {
            get
            {
                string key = _selectedUser?.Key;
                if (string.IsNullOrEmpty(key))
                    key = _dataService.GetAllUsersKey();

                if (_teamIssues != null)
                {
                    if (_teamIssues.ContainsKey(key))
                    {
                        return _teamIssues[key];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public void Init(ConfigManager configManager)
        {
            _configManger = configManager;
            _configManger.Init();

            _dataService = new DataService();
            _dataService.Init(_configManger.Settings);
            WorklogFromDate = _dataService.GetDefaultDate();
            WorklogToDate = _dataService.GetDefaultDate();
            CreateCommands();
        }
        #region "Commands"

        public ICommand LoadWorklogCommand
        {
            get { return _loadWorklogCommand; }
        }

     
        public ICommand SaveSettingsCommand
        {
            get { return _saveSettingsCommand; }
        }

        public ICommand LoadUsersCommand
        {
            get { return _loadUsersCommand; }
        }

        public ICommand LoadSprintTasksCommand
        {
            get { return _loadSprintTasksCommand; }
        }
#endregion

        private void CreateCommands()
        {
            _commandsList = new List<BaseCommand>();
            _loadWorklogCommand = new BaseCommand(DoLoadworklog, CanRunCommands, SetCommandInProgress);
            _commandsList.Add(_loadWorklogCommand);
            _saveSettingsCommand = new BaseCommand(DoSaveSettings, CanRunCommands, SetCommandInProgress);
            _commandsList.Add(_saveSettingsCommand);
            _loadUsersCommand = new BaseCommand(DoLoadUsers, CanRunCommands, SetCommandInProgress);
            _commandsList.Add(_loadUsersCommand);
            _loadSprintTasksCommand = new BaseCommand(DoLoadSprintTasks, CanRunCommands, SetCommandInProgress);
            _commandsList.Add(_loadSprintTasksCommand);
        }

        private void DoLoadSprintTasks()
        {
            _issues = _dataService.GetIssues(_configManger.Settings.Sprint.SprintId);
            _teamIssues = _dataService.GetTeamIssues(_issues);
            RaisePropertyChanged("UserStories");
        }

        private void DoLoadUsers()
        {
            Users = _dataService.LoadUsers();
        }

        private void DoSaveSettings()
        {
            _configManger.SaveSettings();
            _dataService.Init(_configManger.Settings);
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

        private void DoLoadworklog()
        {
            _worklogs = _dataService.GetWorklog(_worklogFromDate, _worklogToDate);
            _teamWorklog = _dataService.GetTeamWorklog(_worklogs, _issues);
            if (SelectedUser != null)
            {
                RaisePropertyChanged("UserWorklog");
            }
        }
    }
}
