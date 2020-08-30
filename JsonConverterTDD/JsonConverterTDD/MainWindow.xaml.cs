using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace JsonConverterTDD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UpdateTreeView(@"{'info':{'age':10,'name':'Smith'}}");
        }

        private void UpdateTreeView(string value)
        {
            try
            {
                var jTokenAdapter = new JTokenAdapter(JObject.Parse(value));
                List<JTokenAdapter> items = new List<JTokenAdapter>();
                items.Add(jTokenAdapter);
                jsonTreeView.ItemsSource = items;
            }
            catch(Newtonsoft.Json.JsonReaderException e)
            {
                return;
            }
        }

        private void targetTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var htmlText = new HtmlText(targetTextBox.Text);
            string normalText = htmlText.GetDecodedText();
            UpdateTreeView(normalText);
        }
    }

    public class TemplateSelector: DataTemplateSelector
    {
        public DataTemplate RootTemplate { get; set; }
        public DataTemplate IntermediateTemplate { get; set; }
        public DataTemplate LeafTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            JTokenAdapter adapter = item as JTokenAdapter;
            switch (adapter.GetDataType())
            {
                case JTokenAdapter.DataType.Root:
                    return RootTemplate;
                case JTokenAdapter.DataType.Intermediate:
                    return IntermediateTemplate;
                case JTokenAdapter.DataType.Leaf:
                default:
                    return LeafTemplate;
            }
        }
    }

    public class HtmlText
    {
        public HtmlText(string htmlText)
        {
            this.htmlText = htmlText;
        }

        public string GetDecodedText()
        {
            return WebUtility.HtmlDecode(htmlText);
        }

        private string htmlText;
    }

    public class JTokenAdapter: TreeViewItemBase
    {
        public JTokenAdapter(JToken jToken)
        {
            Children = GetPropertyAndValuesFrom(jToken);
        }

        public JTokenAdapter(string property, JToken value)
        {
            Property = property;
            switch (value.Type)
            {
                case JTokenType.Object:
                    Children = GetPropertyAndValuesFrom(value);
                    break;
                case JTokenType.Array:
                    Children = GetPropertyAndValuesFrom(value);
                    break;
                default:
                    Value = value;
                    break;
            }
        }

        public enum DataType
        {
            Root,
            Intermediate,
            Leaf
        };

        public DataType GetDataType()
        {
            if (Property == null)
                return DataType.Root;
            else if (Children != null)
                return DataType.Intermediate;
            else
                return DataType.Leaf;
        }

        private static ObservableCollection<JTokenAdapter> GetPropertyAndValuesFrom(JToken jToken)
        {
            var ret = new ObservableCollection<JTokenAdapter>();
            JObject jObject = jToken.ToObject<JObject>();
            foreach (var propertyAndValue in jObject)
            {
                string property = propertyAndValue.Key;
                JToken value = propertyAndValue.Value;
                ret.Add(new JTokenAdapter(property, value));
            }
            return ret;
        }

        public string Property { get; set; }

        public JToken Value { get; set; }

        public string ValueToString {
            get {
                if (Value is null)
                    return "";

                if(Value.Type == JTokenType.String)
                    return "'" + Value.ToString() + "'";

                return Value.ToString();
            }
        }

        public ObservableCollection<JTokenAdapter> Children { get; set; }
    }

    public class TreeViewItemBase : INotifyPropertyChanged
    {
        private bool isSelected;
        public bool IsSelected {
            get { return this.isSelected; }
            set {
                if (value != this.isSelected)
                {
                    this.isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }

        private bool isExpanded;
        public bool IsExpanded {
            get { return this.isExpanded; }
            set {
                if (value != this.isExpanded)
                {
                    this.isExpanded = value;
                    NotifyPropertyChanged("IsExpanded");
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
