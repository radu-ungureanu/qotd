using DataSourceProvider;
using System;
using System.ComponentModel;

namespace QuoteOfTheDay
{
    public class QuoteOfTheDayVM : INotifyPropertyChanged
    {
        public EventHandler QuoteCompleted;

        DataService _service;
        Quote _quoteOfTheDay;

        public QuoteOfTheDayVM()
        {
            _service = new DataService();
            LoadQuote();
        }

        private string _author;
        public string Author
        {
            get
            {
                return _author;
            }
            set
            {
                if (_author != value)
                {
                    _author = value;
                    OnChanged("Author");
                }
            }
        }

        private string _quote;
        public string Quote
        {
            get
            {
                return _quote;
            }
            set
            {
                if (_quote != value)
                {
                    _quote = value;
                    OnChanged("Quote");
                }
            }
        }

        private async void LoadQuote()
        {
            _quoteOfTheDay = await _service.GetQuoteOfTheDayAsync();
            Author = _quoteOfTheDay.Author;
            Quote = _quoteOfTheDay.Content;
            if (QuoteCompleted != null)
            {
                QuoteCompleted(this, EventArgs.Empty);
            }
        }

        public PropertyChangedEventHandler _propertyChanged;
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { this._propertyChanged += value; }
            remove { this._propertyChanged -= value; }
        }

        private void OnChanged(string propertyName)
        {
            if (_propertyChanged != null)
            {
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
