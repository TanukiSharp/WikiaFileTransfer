using PresentationToolKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WikiaFileTransfer.ViewModels
{
    public enum UploadResult
    {
        Success,
        DuplicateWarning,
        UnknownWarning,
        Error,
    }

    public class UploadFileViewModel : ViewModelBase
    {
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { SetValue(ref isSelected, value); }
        }

        private bool shouldUpload;
        public bool ShouldUpload
        {
            get { return shouldUpload; }
            set { SetValue(ref shouldUpload, value); }
        }

        public string Filename { get; private set; }

        public UploadFileViewModel(string filename)
        {
            Filename = filename;

            CopyToClipboardCommand = new AnonymousCommand(OnCopyToClipboard);
        }

        public ICommand CopyToClipboardCommand { get; private set; }

        private void OnCopyToClipboard()
        {
            var sb = new StringBuilder(Filename);
            sb.AppendLine();

            if (Result == UploadResult.DuplicateWarning)
            {
                sb.AppendLine("---");
                sb.AppendLine(string.Format("Duplicate File: {0}", DuplicateFile));
            }
            else if (Result == UploadResult.Error)
            {
                sb.AppendLine("---");
                sb.AppendLine(string.Format("Error Code: {0}", ErrorCode));
                sb.AppendLine(string.Format("Error Info: {0}", ErrorInfo));
            }

            sb.AppendLine("---");
            sb.AppendLine(RawResult);

            Clipboard.SetText(sb.ToString());
        }

        private UploadResult result;
        public UploadResult Result
        {
            get { return result; }
            set { SetValue(ref result, value); }
        }

        private string errorCode;
        public string ErrorCode
        {
            get { return errorCode; }
            set { SetValue(ref errorCode, value); }
        }

        private string errorInfo;
        public string ErrorInfo
        {
            get { return errorInfo; }
            set { SetValue(ref errorInfo, value); }
        }

        private string duplicateFile;
        public string DuplicateFile
        {
            get { return duplicateFile; }
            set { SetValue(ref duplicateFile, value); }
        }

        private string rawResult;
        public string RawResult
        {
            get { return rawResult; }
            set { SetValue(ref rawResult, value); }
        }
    }
}
