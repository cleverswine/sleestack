namespace SleeStack.ViewModels
{
    public class SlackItem : ViewModelBase
    {
        private string _subTitle;
        private string _title;
        private string _imagePath;

        public SlackItem()
        {
            
        }

        public SlackItem(string id, string title, string subTitle, string description = null, string content = null, string imagePath = null)
        {
            UniqueId = id;
            Title = title;
            SubTitle = subTitle;
            Description = description;
            Content = content;
            ImagePath = imagePath;
        }

        public string UniqueId { get; set; }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyPropertyChanged("Title");
            }
        }

        public string SubTitle
        {
            get { return _subTitle; }
            set
            {
                _subTitle = value;
                NotifyPropertyChanged("SubTitle");
            }
        }

        public string Description { get; set; }
        public string Content { get; set; }

        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                NotifyPropertyChanged("ImagePath");
            }
        }

        public double Ts { get; set; }
    }
}