using RevitPluginWalls.Abstract;
using RevitPluginWalls.CommandData;
using RevitPluginWalls.Views;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace RevitPluginWalls.ViewModels
{
    public class PluginViewModel : IViewModel, INotifyPropertyChanged
    {
        public string Login
        {
            get => _login;
            set
            {
                _login = value;
            }
        }
        string _login;

        public string Password
        {
            get => _password; 
            set => _password = value;
        }
        string _password;

        public string Url
        {
            get => _url; 
            set => _url = value;
        }
        string _url;

        public string ProjectId
        {
            get => _projectId;
            set => _projectId = value;
        }
        string _projectId;

        public bool IsBuildFast
        {
            get => _isBuildFast; 
            set => _isBuildFast = value;
        }
        bool _isBuildFast;

        public ICommand BuildModel { get => _buildModel; }
        ICommand _buildModel;

        public ICommand Cancel { get => _cancel; }
        ICommand _cancel;
        

        PluginSettingsView _view;
        CommandDataStorage _data = null;

        public PluginViewModel()
        {
            _login = string.Empty;
            _password = string.Empty;
            _url = string.Empty;
            _projectId = string.Empty;
            _isBuildFast = false;

            _view = new PluginSettingsView();
            _view.DataContext = this;

            _buildModel = new RelayCommand(OnBuildModel, CanBuildModel);
            _cancel = new RelayCommand(OnCancel, (obj) => true);
        }

        private void OnCancel(object obj)
        {
            if (_data == null) return;

            _data.SaveNewData = true;
            _data.IsBuild = false;
            _data.Login = _login;
            _data.Password = _password;
            _data.Url = _url;
            _data.ProcjectId = _projectId;
            _data.IsBuildFast = _isBuildFast;

            _view.Close();
        }

        private void OnBuildModel(object obj)
        {
            if (_data == null) return;

            _data.SaveNewData = true;
            _data.IsBuild = true;
            _data.Login = _login;
            _data.Password = _password;
            _data.Url = _url;
            _data.ProcjectId = _projectId;
            _data.IsBuildFast = _isBuildFast;

            _view.Close();
        }

        private bool CanBuildModel(object arg)
        {
            if (_data == null) return false;

            return !string.IsNullOrEmpty(_login) &&
                !string.IsNullOrEmpty(_password) &&
                !string.IsNullOrEmpty(_url) &&
                !string.IsNullOrEmpty(_projectId);
        }

        public void ShowDialog(CommandDataStorage data)
        {
            _login = data.Login;
            _password = data.Password;
            _url = data.Url;
            _isBuildFast = data.IsBuildFast;
            _projectId = data.ProcjectId;
            _data = data;

            OnPropertyChanged(nameof(Login));
            OnPropertyChanged(nameof(Password));
            OnPropertyChanged(nameof(Url));
            OnPropertyChanged(nameof(ProjectId));
            OnPropertyChanged(nameof(IsBuildFast));

            _view.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
