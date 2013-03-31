using Microsoft.Win32;
using PresentationToolKit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WikiaLibrary;
using WikiaLibrary.Queries;

namespace WikiaFileTransfer.ViewModels
{
    public class UploadViewModel : ViewModelBase
    {
        public ObservableCollection<UploadFileViewModel> FilesToUpload { get; private set; }

        private string editToken;

        public UploadViewModel()
        {
            FilesToUpload = new ObservableCollection<UploadFileViewModel>();

            UploadCommand = new AnonymousCommand(OnUpload);

            AddFilesCommand = new AnonymousCommand(OnAddFiles);
            RemoveFilesCommand = new AnonymousCommand(OnRemoveFiles) { IsEnabled = false };
            ClearFilesCommand = new AnonymousCommand(OnClearFiles) { IsEnabled = false };

            SelectionChangedCommand = new AnonymousCommand(OnSelectionChanged);
        }

        public ICommand UploadCommand { get; private set; }

        public ICommand AddFilesCommand { get; private set; }
        public ICommand RemoveFilesCommand { get; private set; }
        public ICommand ClearFilesCommand { get; private set; }

        public ICommand SelectionChangedCommand { get; private set; }

        private bool isUploading;
        public bool IsUploading
        {
            get { return isUploading; }
            set { SetValue(ref isUploading, value); }
        }

        private bool ignoreWarnings;
        public bool IgnoreWarnings
        {
            get { return ignoreWarnings; }
            set { SetValue(ref ignoreWarnings, value); }
        }

        private string status;
        public string Status
        {
            get { return status; }
            set { SetValue(ref status, value); }
        }

        private bool statusType = true;
        public bool StatusType
        {
            get { return statusType; }
            set { SetValue(ref statusType, value); }
        }

        private async void OnUpload()
        {
            if (IsUploading == false)
            {
                await UploadFiles();
            }
            else
            {
                IsUploading = false;
            }
        }

        private async Task UploadFiles()
        {
            if (RootViewModel.Client == null)
            {
                Status = "Impossible to upload, not connected.";
                StatusType = false;
                return;
            }

            if (editToken == null)
            {
                Status = "Retrieving edit token...";
                StatusType = true;

                for (var i = 0; i < 3 && editToken == null; i++)
                    editToken = await Utility.RequestEditToken(RootViewModel.Client);

                if (editToken == null)
                {
                    Status = "Impossible to get edit token.";
                    StatusType = false;
                    return;
                }
            }

            Status = "Uploading files...";
            StatusType = true;
            IsUploading = true;

            var localIgnoreWarnings = ignoreWarnings;

            foreach (var f in FilesToUpload)
            {
                f.Result = UploadResult.Success;
                f.ShouldUpload = true;
            }

            UploadFileViewModel file;
            while (IsUploading && (file = FilesToUpload.FirstOrDefault(f => f.ShouldUpload)) != null)
            {
                Upload upload = null;

                try
                {
                    upload = new Upload(RootViewModel.Client);
                    await Utility.UploadFileAsync(file.Filename, editToken, localIgnoreWarnings, upload);
                }
                catch (Exception ex)
                {
                    Status = ex.Message;
                    StatusType = false;
                    break;
                }

                if (upload.IsSuccess)
                    FilesToUpload.Remove(file);
                else
                {
                    file.ShouldUpload = false;
                    file.RawResult = upload.RawResult;

                    if (upload.IsWarning)
                    {
                        if (upload.IsDuplicate)
                        {
                            file.Result = UploadResult.DuplicateWarning;
                            file.DuplicateFile = upload.DuplicatingFile;
                        }
                        else
                            file.Result = UploadResult.UnknownWarning;
                    }
                    else if (upload.IsError)
                    {
                        file.Result = UploadResult.Error;
                        file.ErrorCode = upload.ErrorCode;
                        file.ErrorInfo = upload.ErrorInfo;
                    }
                }
            }

            IsUploading = false;
            ((AnonymousCommand)ClearFilesCommand).IsEnabled = FilesToUpload.Count > 0;

            Status = "Upload done.";
            StatusType = true;
        }

        private void OnAddFiles()
        {
            var dlg = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "All Files (*.*)|*.*",
                Multiselect = true,
                Title = "Select files to upload",
            };

            if (dlg.ShowDialog() != true)
                return;

            var diff = dlg.FileNames.Except(FilesToUpload.Select(f => f.Filename));

            foreach (var file in diff)
                FilesToUpload.Add(new UploadFileViewModel(file));

            if (FilesToUpload.Count > 0)
                ((AnonymousCommand)ClearFilesCommand).IsEnabled = true;
        }

        private void OnRemoveFiles()
        {
            selectionLocked = true;
            try
            {
                foreach (var file in FilesToUpload.Where(f => f.IsSelected).ToArray())
                    FilesToUpload.Remove(file);

                ((AnonymousCommand)RemoveFilesCommand).IsEnabled = false;
                ((AnonymousCommand)ClearFilesCommand).IsEnabled = FilesToUpload.Count > 0;
            }
            finally
            {
                selectionLocked = false;
            }
        }

        private void OnClearFiles()
        {
            if (FilesToUpload.Count == 0)
                return;

            FilesToUpload.Clear();

            ((AnonymousCommand)ClearFilesCommand).IsEnabled = false;
        }

        private bool selectionLocked;

        private void OnSelectionChanged(object parameter)
        {
            if (selectionLocked)
                return;

            var hasSelection = FilesToUpload.Any(f => f.IsSelected);
            ((AnonymousCommand)RemoveFilesCommand).IsEnabled = hasSelection;
        }
    }

    public class FilenameEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return Path.GetFileName(x) == Path.GetFileName(y);
        }

        public int GetHashCode(string obj)
        {
            var str = obj as string;
            if (str == null)
                return 0;

            return Path.GetFileName(str).GetHashCode();
        }
    }
}
