using Bogus;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using SharpConstraintLayout.Maui.Helper.Widget;
using SharpConstraintLayout.Maui.Widget;
using System.Diagnostics;

namespace SharpConstraintLayout.Maui.Example.Pages;

public partial class LayoutPerformanceTestPage : ContentPage
{
    public LayoutPerformanceTestPage()
    {
        InitializeComponent();
        var viewmodel = new ViewModel();
        collectionView.BindingContext = viewmodel;
        collectionView.ItemsSource = viewmodel.Models;
    }

    public class ViewModel
    {
        public static Stopwatch Stopwatch = new Stopwatch();

        public static List<long> Times = new List<long>();
        public static int LimitCount = 100;
        public static void CalculateMeanMeasureTimeAsync(long time)
        {
            if (Times.Count >= LimitCount)
            {
                long count = 0;
                long max = 0;
                long min = 100;
                foreach (long t in Times)
                {
                    if (t > max)
                        max = t;
                    if (t < min)
                        min = t;
                    count += t;
                }
                Times.Clear();
                Task.Run(() =>
                {
                    Shell.Current.CurrentPage?.Dispatcher.Dispatch(async () =>
                    {
                        await Shell.Current.CurrentPage?.DisplayAlert("Alert", $" Measure {LimitCount} Items: All-{count} Mean-{count * 1.0 / LimitCount} Max-{max} Min-{min} ms", "OK");
                    });
                });
            }
            else
            {
                Times.Add(time);
            }
        }

        public List<Model> Models { get; set; }

        private Faker<Model> faker;
        public ViewModel()
        {
            faker = new Faker<Model>();
            faker
                //.RuleFor(m => m.PersonIconUrl, f => f.Person.Avatar)
                .RuleFor(m => m.PersonName, f => f.Person.FullName)
                .RuleFor(m => m.PersonGender, f => f.Person.Gender.ToString())
                .RuleFor(m => m.PersonPhone, f => f.Person.Phone)
                .RuleFor(m => m.PersonTextBlogTitle, f => f.WaffleText(1, false))
                .RuleFor(m => m.PersonTextBlog, f => f.WaffleText(1, false))
                //.RuleFor(m => m.PersonImageBlogUrl, f => f.Image.PicsumUrl())
                .RuleFor(m => m.FirstComment, f => f.WaffleText(1, false))
                //.RuleFor(m => m.LikeIconUrl, f => f.Person.Avatar)
                //.RuleFor(m => m.CommentIconUrl, f => f.Person.Avatar)
                //.RuleFor(m => m.ShareIconUrl, f => f.Person.Avatar)
                ;
            Models = faker.Generate(1000);
        }
    }

    public DataTemplate BuildConstraintLayout()
    {
        return new DataTemplate(() =>
        {
            var root = new ContainerLayout();
            var layout = new ConstraintLayout() { ConstrainWidth = ConstraintSet.MatchParent, ConstrainHeight = ConstraintSet.WrapContent, BackgroundColor = Color.FromRgb(66, 66, 66) };
            root.Add(layout);
            var PersonIconContainer = new Border() { StrokeShape = new RoundRectangle() { CornerRadius = new CornerRadius(20) } };
            var PersonIcon = new Image() { BackgroundColor = Colors.AliceBlue };

            PersonIconContainer.Content = PersonIcon;
            var PersonName = new Label() { TextColor = Colors.White };
            var PersonGender = new Label() { TextColor = Colors.White };
            var PersonPhone = new Label() { TextColor = Colors.White };
            var PersonTextBlogTitle = new Label() { FontSize = 20, LineBreakMode = LineBreakMode.WordWrap, MaxLines = 2, TextColor = Colors.White, BackgroundColor = Colors.SlateGray };
            var PersonTextBlog = new Label() { LineBreakMode = LineBreakMode.WordWrap, MaxLines = 3, TextColor = Colors.White, BackgroundColor = Colors.SlateGray };
            var PersonImageBlog = new Image() { BackgroundColor = Colors.AliceBlue };
            var TestButton = new Button() { Text = "Hello" };
            var FirstComment = new Label();
            var likeContainer = new Flow();
            var LikeIcon = new Image() { BackgroundColor = Colors.AliceBlue };
            var LikeCountLabel = new Label { Text = "555" };

            var commentContainer = new Flow() { BackgroundColor = Colors.Red };
            var CommentIcon = new Image() { BackgroundColor = Colors.AliceBlue };
            var CommentCountLabel = new Label { Text = "1000" };
            var shareContaner = new Flow();
            var ShareIcon = new Image() { BackgroundColor = Colors.AliceBlue };
            var ShareCountLabel = new Label { Text = "999" };
            layout.AddElement(PersonIconContainer, PersonName, PersonGender, PersonPhone, PersonTextBlogTitle, PersonTextBlog, PersonImageBlog, TestButton
                , likeContainer, LikeIcon, LikeCountLabel,
                     commentContainer, CommentIcon, CommentCountLabel,
                     shareContaner, ShareIcon, ShareCountLabel);
            likeContainer.RefElement(LikeIcon, LikeCountLabel);
            commentContainer.RefElement(CommentIcon, CommentCountLabel);
            shareContaner.RefElement(ShareIcon, ShareCountLabel);
            likeContainer.SetOrientation(Flow.Horizontal);
            likeContainer.SetWrapMode(Flow.WrapChain);
            likeContainer.SetHorizontalStyle(Flow.ChainSpreadInside);
            commentContainer.SetOrientation(Flow.Horizontal);
            commentContainer.SetWrapMode(Flow.WrapChain);
            commentContainer.SetHorizontalStyle(Flow.ChainSpreadInside);
            shareContaner.SetOrientation(Flow.Horizontal);
            shareContaner.SetWrapMode(Flow.WrapChain);
            shareContaner.SetHorizontalStyle(Flow.ChainSpreadInside);
            var guideline1_6 = new Guideline() { };
            var guideline3_6 = new Guideline();
            var guideline5_6 = new Guideline();
            layout.AddElement(guideline1_6, guideline3_6, guideline5_6);
            using (var set = new FluentConstraintSet())
            {
                set.Clone(layout);
                set.Select(PersonIconContainer).LeftToLeft().TopToTop(null, 5).Width(40).Height(40)
                    .Select(PersonName).LeftToRight(PersonIconContainer, 5).TopToTop(PersonIconContainer)
                    .Select(PersonGender).LeftToLeft(PersonName).BottomToBottom(PersonIconContainer)
                    .Select(PersonPhone).LeftToRight(PersonGender, 5).TopToTop(PersonGender)
                    .Select(PersonTextBlogTitle).LeftToLeft(PersonIconContainer).TopToBottom(PersonIconContainer).Width(SizeBehavier.MatchParent)
                    .Select(PersonImageBlog).LeftToLeft(PersonTextBlogTitle).TopToBottom(PersonTextBlogTitle).Width(100).Height(100)
                    .Select(PersonTextBlog).LeftToLeft(PersonTextBlogTitle).TopToBottom(PersonImageBlog).Width(SizeBehavier.MatchParent)
                    .Select(TestButton).RightToRight().CenterYTo(PersonImageBlog)
                    .Select(guideline1_6).GuidelineOrientation(Orientation.Y).GuidelinePercent(0.16f)
                    .Select(guideline3_6).GuidelineOrientation(Orientation.Y).GuidelinePercent(0.5f)
                    .Select(guideline5_6).GuidelineOrientation(Orientation.Y).GuidelinePercent(5.0f / 6.0f)
                    .Select(LikeIcon, CommentIcon, ShareIcon).Width(30).Height(30)
                    .Select(likeContainer).LeftToRight(guideline1_6).TopToBottom(PersonTextBlog, 5).Width(SizeBehavier.WrapContent).Height(SizeBehavier.WrapContent)
                    .Select(commentContainer).CenterXTo(guideline3_6).CenterYTo(likeContainer).Width(SizeBehavier.WrapContent).Height(SizeBehavier.WrapContent)
                    .Select(shareContaner).CenterXTo(guideline5_6).CenterYTo(likeContainer).Width(SizeBehavier.WrapContent).Height(SizeBehavier.WrapContent);
                ;
                set.ApplyTo(layout);
            }

            PersonIcon.SetBinding(Image.SourceProperty, nameof(Model.PersonIconUrl));
            PersonName.SetBinding(Label.TextProperty, nameof(Model.PersonName));
            PersonGender.SetBinding(Label.TextProperty, nameof(Model.PersonGender));
            PersonPhone.SetBinding(Label.TextProperty, nameof(Model.PersonPhone));
            PersonTextBlogTitle.SetBinding(Label.TextProperty, nameof(Model.PersonTextBlogTitle));
            PersonTextBlog.SetBinding(Label.TextProperty, nameof(Model.PersonTextBlog));
            PersonImageBlog.SetBinding(Image.SourceProperty, nameof(Model.PersonImageBlogUrl));
            return root;
        });
    }

    public DataTemplate BuildGrid()
    {
        return new DataTemplate(() =>
        {
            var root = new ContainerLayout();
            var layout = new Grid()
            {
                BackgroundColor = Color.FromRgb(66, 66, 66),
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition(){ Height = GridLength.Auto },
                    new RowDefinition(){ Height = GridLength.Auto },
                    new RowDefinition(){ Height = GridLength.Auto },
                    new RowDefinition(){ Height = GridLength.Auto },
                    new RowDefinition(){ Height = GridLength.Auto },
                }
            };
            root.Add(layout);
            var PersonIconContainer = new Border() { WidthRequest = 40, HeightRequest = 40, StrokeShape = new RoundRectangle() { CornerRadius = new CornerRadius(20) } };
            var PersonIcon = new Image() { BackgroundColor = Colors.AliceBlue };
            PersonIconContainer.Content = PersonIcon;
            var personInfoContainer = new HorizontalStackLayout();
            var personTextInfoContainer = new VerticalStackLayout();
            var PersonName = new Label() { TextColor = Colors.White };
            var personOtherInfoContainer = new HorizontalStackLayout();
            var PersonGender = new Label() { TextColor = Colors.White };
            var PersonPhone = new Label() { Margin = new Thickness(5, 0, 0, 0), TextColor = Colors.White };
            personOtherInfoContainer.Add(PersonGender);
            personOtherInfoContainer.Add(PersonPhone);
            personTextInfoContainer.Add(PersonName);
            personTextInfoContainer.Add(personOtherInfoContainer);
            personInfoContainer.Add(PersonIconContainer);
            personInfoContainer.Add(personTextInfoContainer);
            var PersonTextBlogTitle = new Label() { FontSize = 20, LineBreakMode = LineBreakMode.WordWrap, MaxLines = 2, TextColor = Colors.White, BackgroundColor = Colors.SlateGray };
            var PersonTextBlog = new Label() { LineBreakMode = LineBreakMode.WordWrap, MaxLines = 3, TextColor = Colors.White, BackgroundColor = Colors.SlateGray };
            var imageInfoContainer = new Grid();
            var PersonImageBlog = new Image() { WidthRequest = 100, HeightRequest = 100, BackgroundColor = Colors.AliceBlue, HorizontalOptions = LayoutOptions.Start };
            var TestButton = new Button() { Text = "Hello", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.End };
            imageInfoContainer.Add(PersonImageBlog);
            imageInfoContainer.Add(TestButton);
            layout.Add(personInfoContainer);
            layout.Add(PersonTextBlogTitle);
            layout.Add(imageInfoContainer);
            layout.Add(PersonTextBlog);

            Grid.SetRow(personInfoContainer, 0);
            Grid.SetRow(PersonTextBlogTitle, 1);
            Grid.SetRow(imageInfoContainer, 2);
            Grid.SetRow(PersonTextBlog, 3);

            var bottomIconBar = new Grid()
            {
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition(){ Width = GridLength.Star },
                    new ColumnDefinition(){ Width = GridLength.Star },
                    new ColumnDefinition(){ Width = GridLength.Star },
                }
            };

            var likeContainer = new HorizontalStackLayout() { HorizontalOptions = LayoutOptions.Center };
            var LikeIcon = new Image() { WidthRequest = 30, HeightRequest = 30, BackgroundColor = Colors.AliceBlue };
            var LikeCountLabel = new Label { Text = "555", VerticalOptions = LayoutOptions.Center };
            likeContainer.Add(LikeIcon);
            likeContainer.Add(LikeCountLabel);
            var commentContainer = new HorizontalStackLayout() { HorizontalOptions = LayoutOptions.Center, BackgroundColor = Colors.Red };
            var CommentIcon = new Image() { WidthRequest = 30, HeightRequest = 30, BackgroundColor = Colors.AliceBlue };
            var CommentCountLabel = new Label { Text = "1000", VerticalOptions = LayoutOptions.Center };
            commentContainer.Add(CommentIcon);
            commentContainer.Add(CommentCountLabel);
            var shareContaner = new HorizontalStackLayout() { HorizontalOptions = LayoutOptions.Center };
            var ShareIcon = new Image() { WidthRequest = 30, HeightRequest = 30, BackgroundColor = Colors.AliceBlue };
            var ShareCountLabel = new Label { Text = "999", VerticalOptions = LayoutOptions.Center };
            shareContaner.Add(ShareIcon);
            shareContaner.Add(ShareCountLabel);

            Grid.SetColumn(likeContainer, 0);
            Grid.SetColumn(commentContainer, 1);
            Grid.SetColumn(shareContaner, 2);
            bottomIconBar.Add(likeContainer);
            bottomIconBar.Add(commentContainer);
            bottomIconBar.Add(shareContaner);


            layout.Add(bottomIconBar);
            Grid.SetRow(bottomIconBar, 4);

            PersonIcon.SetBinding(Image.SourceProperty, nameof(Model.PersonIconUrl));
            PersonName.SetBinding(Label.TextProperty, nameof(Model.PersonName));
            PersonGender.SetBinding(Label.TextProperty, nameof(Model.PersonGender));
            PersonPhone.SetBinding(Label.TextProperty, nameof(Model.PersonPhone));
            PersonTextBlogTitle.SetBinding(Label.TextProperty, nameof(Model.PersonTextBlogTitle));
            PersonTextBlog.SetBinding(Label.TextProperty, nameof(Model.PersonTextBlog));
            PersonImageBlog.SetBinding(Image.SourceProperty, nameof(Model.PersonImageBlogUrl));
            return root;
        });
    }

    public class Model
    {
        public string PersonIconUrl { get; set; }
        public string PersonName { get; set; }
        public string PersonGender { get; set; }
        public string PersonPhone { get; set; }
        public string PersonTextBlogTitle { get; set; }
        public string PersonTextBlog { get; set; }
        public string PersonImageBlogUrl { get; set; }
        public string FirstComment { get; set; }
        public string LikeIconUrl { get; set; }
        public string CommentIconUrl { get; set; }
        public string ShareIconUrl { get; set; }
    }

    public class ContainerLayout : Layout
    {
        protected override ILayoutManager CreateLayoutManager()
        {
            return new ContainerLayoutManager(this);
        }
    }

    public class ContainerLayoutManager : LayoutManager
    {

        public ContainerLayoutManager(Microsoft.Maui.ILayout layout) : base(layout)
        {
        }

        public override Size ArrangeChildren(Rect bounds)
        {
            var layout = Layout as Layout;
            (layout.Children[0] as IView).Arrange(bounds);
            return bounds.Size;
        }

        public override Size Measure(double widthConstraint, double heightConstraint)
        {
            var layout = Layout as Layout;
            ViewModel.Stopwatch.Restart();
            var size = (layout.Children[0] as IView).Measure(widthConstraint, heightConstraint);
            ViewModel.Stopwatch.Stop();
            ViewModel.CalculateMeanMeasureTimeAsync(ViewModel.Stopwatch.ElapsedMilliseconds);
            return size;
        }
    }

    private void UseConstraintLayout_Clicked(object sender, EventArgs e)
    {
        collectionView.ItemTemplate = BuildConstraintLayout();
    }

    private void UseGrid_Clicked(object sender, EventArgs e)
    {
        collectionView.ItemTemplate = BuildGrid();
    }
}