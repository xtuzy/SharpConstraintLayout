namespace SharpConstraintLayout.Maui.Example.Pages;

using MauiGestures;
using SharpConstraintLayout.Maui.Widget;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using static SharpConstraintLayout.Maui.Widget.FluentConstraintSet;

public partial class ShowZIndexView : ContentView
{
    public ShowZIndexView()
    {
        InitializeComponent();
        var defaultSet = new FluentConstraintSet();
        defaultSet.Clone(layout);
        defaultSet.Select(guideline).GuidelineOrientation(Orientation.X).GuidelinePercent(0.7f)
            .Select(box).CenterYTo().Visibility(Visibility.Invisible)
            .Select(box1, rect1, rect2).Height(200).CenterXTo()
           ;
        foreach (var view in layout.Children)
        {
            var child = view as View;
            if (child.ZIndex > 1)
            {
                defaultSet.Select(child).Width(200 - 10 * (4 - child.ZIndex)).BottomToTop(guideline, 10 * (4 - child.ZIndex));
            }
        }
        defaultSet.ApplyTo(layout);

        //leftSwipeGesture = new SwipeGestureRecognizer { Direction = SwipeDirection.Left };
        //leftSwipeGesture.Swiped += LeftSwipeGesture_Swiped;
        //rect2.GestureRecognizers.Add(leftSwipeGesture);

        MauiGestures.Gesture.SetPanPointCommand(layout, PanPointCommand);
    }

    public ICommand PanPointCommand => new Command<PanEventArgs>(args =>
    {
        PanGestureRecognizer_PanUpdated(args);
    });

    Point firstPoint = Point.Zero;
    DateTime firstPanTime = DateTime.MinValue;
    bool isRemoving = false;
    /// <summary>
    /// A anim like https://twitter.com/philipcdavis/status/1534192823792128000?s=20&t=ehif9KI77CwIz-yZ3obnNg
    /// </summary>
    /// <param name="e"></param>
    private void PanGestureRecognizer_PanUpdated(PanEventArgs e)
    {
        View firstView = null;
        foreach (var view in layout.Children)
        {
            if (view.ZIndex == 4)
            {
                firstView = view as View;
            }
        }
        if (firstPoint == Point.Zero)
        {
            if (e.Status == GestureStatus.Started)
            {
                //点击中时才触发
                if (new Rect(firstView.X, firstView.Y, firstView.Width, firstView.Height).Contains(e.Point))
                {
                    firstPoint = e.Point;
                    firstPanTime = DateTime.Now;
                    isRemoving = true;
                    //依据点击左右旋转
                    firstView.Rotation = 0;//如果之前变动过,先还原
                    if (e.Point.X < firstView.X + firstView.Width / 2)
                        firstView.RotateTo(3, 100);
                    else
                        firstView.RotateTo(-3, 100);
                }
            }
        }
        else
        {
            if (isRemoving)
            {
                if (e.Status == GestureStatus.Running)
                {
                    var dy = e.Point.Y - firstPoint.Y;
                    var startStateSet = new FluentConstraintSet();
                    startStateSet.Clone(layout);
                    startStateSet.Select(firstView).BottomToTop(guideline, -(int)dy).Rotation((float)firstView.Rotation);
                    startStateSet.ApplyTo(layout);
                }
                else if (e.Status == GestureStatus.Completed)
                {
                    var dy = e.Point.Y - firstPoint.Y;
                    var dtime = (DateTime.Now - firstPanTime).TotalMilliseconds;
                    var startStateSet = new FluentConstraintSet();
                    startStateSet.Clone(layout);
                    startStateSet.Select(firstView).BottomToTop(guideline, -(int)dy).Rotation((float)firstView.Rotation);

                    int offsetOfTop = (int)Math.Abs(dy / dtime * 500);
                    var topStateSet = new FluentConstraintSet();
                    topStateSet.Clone(startStateSet);
                    var startMargin = startStateSet.GetMargin(firstView.GetId(), ConstraintSet.Bottom);
                    topStateSet.Select(firstView)
                        .BottomToTop(guideline, startMargin + offsetOfTop)
                        .Rotation(firstView.Rotation > 0 ? 360 : -360);
                    //移动下面的View大小和位置
                    List<View> goUpViews = new List<View>();
                    foreach (var view in layout.Children)
                    {
                        var child = view as View;
                        if (child.ZIndex > 1)
                        {
                            if (child != firstView)
                            {
                                goUpViews.Add(child);
                                topStateSet.Select(child).Width(200 - 10 * (4 - (child.ZIndex + 1))).BottomToTop(guideline, 10 * (4 - (child.ZIndex + 1)));
                            }
                        }
                    }

                    var goUpAnim = layout.CreateAnimation(startStateSet, topStateSet, default, Easing.Linear);

                    //other view go up one ZIndex
                    View newFirstView = null;
                    var ZIndexAnim = new Animation((v) =>
                    {

                    }, 0, 1, null, () =>
                    {
                        //修改Z高度
                        foreach (var view1 in layout.Children)
                            if (view1.ZIndex > 1)
                            {
                                (view1 as View).ZIndex = view1.ZIndex + 1;
                                if (view1.ZIndex == 4)
                                    newFirstView = view1 as View;
                            }
                        firstView.ZIndex = 2;
                    });

                    var finishStateSet = new FluentConstraintSet();
                    finishStateSet.Clone(topStateSet);
                    finishStateSet.Select(firstView).Clear().CenterXTo().Width(200 - 10 * (4 - 2)).Height(200).BottomToTop(guideline, 10 * (4 - 2)).Rotation(firstView.Rotation > 0 ? 720 : -720);

                    var goDownAnim = layout.CreateAnimation(topStateSet, finishStateSet, new List<View> { firstView }, Easing.Linear);
                    var allAnim = new Animation()
                    {
                        {0,0.5, goUpAnim},
                        {0,0.5, ZIndexAnim },
                        {0.5,1, goDownAnim },
                    };
                    allAnim.Commit(layout, "cardOut", 10, 1000, Easing.Linear, (v, b) =>
                    {
                        Debug.WriteLine(firstView.TranslationX);
                        finishStateSet.Select(firstView).Rotation(0);
                        layout.FinishedAnimation();
                    });
                    firstPoint = Point.Zero;
                    firstPanTime = DateTime.MinValue;
                }
            }
        }
    }

    DateTime lastSwipeTime;
    private SwipeGestureRecognizer leftSwipeGesture;

    private void LeftSwipeGesture_Swiped(object sender, SwipedEventArgs e)
    {
        if (lastSwipeTime == null)
            lastSwipeTime = DateTime.Now;
        else
        {
            if ((DateTime.Now - lastSwipeTime).TotalMilliseconds < 1500)
            {
                lastSwipeTime = DateTime.Now;
                return;
            }
        }
        View secondView = null;
        foreach (var view in layout.Children)
        {
            if (view.ZIndex == 3)
            {
                secondView = view as View;
            }
        }
        var firstView = sender as View;
        firstView.GestureRecognizers.Remove(leftSwipeGesture);
        //firstView go to left anim
        var set = new FluentConstraintSet();
        set.Clone(layout);
        set.Select(firstView).Clear().Width(200).Height(200).RightToLeft(secondView).BottomToTop(guideline, 10 * (4 - firstView.ZIndex))//.Rotation(-60)
           ;
        var firstViewGoLeftAnim = layout.CreateAnimation(set, new List<View> { firstView }, Easing.SpringIn);

        View newFirstView = null;
        //other view go up one ZIndex
        var ZIndexAnim = new Animation((v) =>
        {

        }, 0, 1, null, () =>
        {
            //修改Z高度
            foreach (var view1 in layout.Children)
                if (view1.ZIndex > 1)
                {
                    (view1 as View).ZIndex = view1.ZIndex + 1;
                    if (view1.ZIndex == 4)
                        newFirstView = view1 as View;
                }
            firstView.ZIndex = 2;
        });

        //移动下面的View大小和位置
        var otherViewGoUpFinishStateSet = new FluentConstraintSet();
        otherViewGoUpFinishStateSet.Clone(layout);
        List<View> goUpViews = new List<View>();
        foreach (var view in layout.Children)
        {
            var child = view as View;
            if (child.ZIndex > 1)
            {
                if (child != firstView)
                {
                    goUpViews.Add(child);
                    otherViewGoUpFinishStateSet.Select(child).Width(200 - 10 * (4 - (child.ZIndex + 1))).BottomToTop(guideline, 10 * (4 - (child.ZIndex + 1)));
                }
            }
        }
        var otherViewGoUpAnim = layout.CreateAnimation(otherViewGoUpFinishStateSet, goUpViews, Easing.SpringOut);
        var finishStateSet = new FluentConstraintSet();
        finishStateSet.Clone(layout);
        finishStateSet.Select(firstView).Clear().CenterXTo().Width(200 - 10 * (4 - 2)).Height(200).BottomToTop(guideline, 10 * (4 - 2)); ;
        finishStateSet.Clone(otherViewGoUpFinishStateSet, goUpViews.ToArray());

        var finishAnim = layout.CreateAnimation(finishStateSet, new List<View> { firstView }, Easing.SpringOut);
        var allAnim = new Animation()
        {
            {0,0.5,firstViewGoLeftAnim },
            {0.4,0.5,ZIndexAnim },
            {0.4,0.5,otherViewGoUpAnim },
            {0.5,1, finishAnim}
        };
        allAnim.Commit(layout, "cardOut", 16, 1000, Easing.Linear, (v, b) =>
        {
            finishStateSet.ApplyTo(layout);
            newFirstView?.GestureRecognizers.Add(leftSwipeGesture);
        });
    }
}
