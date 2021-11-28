# SharpConstraintLayout 🗜️📏

[![NuGet version(SharpConstraintLayout.Core)](https://img.shields.io/nuget/v/SharpConstraintLayout.Core?label=SharpConstraintLayout.Core)](https://www.nuget.org/packages/SharpConstraintLayout.Core/)
[![NuGet version(SharpConstraintLayout.Wpf)](https://img.shields.io/nuget/v/SharpConstraintLayout.Wpf?label=SharpConstraintLayout.Wpf)](https://www.nuget.org/packages/SharpConstraintLayout.Wpf/)

This is a C# port of [ConstraintLayout](https://github.com/androidx/constraintlayout), it convert [constraintlayout.core](https://github.com/androidx/constraintlayout/tree/main/constraintlayout/core) and create ConstraintLayout for WPF. Now, you can use ConstraintLayout in C# world 🎆

## Using ConstraintLayout

### ⬇️ Installation

Search and install nuget **SharpConstraintLayout.Wpf**, it base on .NET5.

If you want custom constraintlayout, you can install SharpConstraintLayout.Core, it base on .netstandard2.0.

### ✨🤩📱 Key Features

**Notice**: now you can only create constraint by code.

Create layout and add children view.

```
var layout = new ConstraintLayout();
var firstButton = new Button(){Constent="first"};
var firstButton = new Button(){Content="second"};
layout.Children.Add(firstButton)
layout.Children.Add(secondButton)
```

You have two way to create constraint

- Use ConstraintSet

  ```
  new ConstraintSet(constraintlayout)
  .AddConnect(firstButton, ConstraintAnchor.Type.CENTER, layout, ConstraintAnchor.Type.CENTER)
  .AddConnect(secondButton,ConstraintAnchor.Type.LEFT,firstdButton,ConstraintAnchor.Type.RIGHT)
  .AddConnect(secondButton, ConstraintAnchor.Type.TOP,firstButton,ConstraintAnchor.Type.BOTTOM);
  ```

- Use new api, see [NewApiTest](https://github.com/xtuzy/SharpConstraintLayout/blob/6d2ca9be3273724e2355c6d5581b164228a5f719/SharpConstraintLayout.Example.Reload/ComplexLayoutTestWindow.xaml.cs#L129)

  ```
  firstButton.Center(Page);
  
  //LeftToX, At toView Left is negative
  secondButton.LeftToLeft(firstButton,-20).BottomToTop(firstButton,60);
  thirdButton.LeftToLeft(firstButton,20).BottomToTop(firstButton,5);
  
  forthButton.LeftToRight(firstButton,-20).BottomToTop(firstButton,60);
  fifthButton.LeftToRight(firstButton,20).BottomToTop(firstButton,5);
  
  //RightToX, At toView Right is negative
  var HorizontalCenterGuidline = new GuideLine() { Percent = 0.5f,Orientation=GuideLine.Orientations.HORIZONTAL };
  Page.Children.Add(HorizontalCenterGuidline);
  sixthButton.RightToLeft(firstButton, -20).BottomToTop(HorizontalCenterGuidline);
  seventhButton.RightToLeft(firstButton,20).TopToBottom(HorizontalCenterGuidline);
  
  eighthButton.RightToRight(firstButton,-20).BottomToTop(HorizontalCenterGuidline);
  ninthButton.RightToRight(firstButton,20).TopToBottom(HorizontalCenterGuidline);
  
  var VerticalCenterGuideline = new GuideLine() { Percent = 0.5f,Orientation=GuideLine.Orientations.VERTICAL };
  Page.Children.Add(VerticalCenterGuideline);
  //TopToX, At toView Top is negative
  tenthButton.RightToLeft(eleventhButton).TopToTop(firstButton, -20);
  eleventhButton.RightToLeft(VerticalCenterGuideline,20).TopToTop(firstButton, 20);
  
  twelfthButton.RightToLeft(thirteenthButton).TopToBottom(firstButton, -20);
  thirteenthButton.RightToLeft(VerticalCenterGuideline,20).TopToBottom(firstButton, 20);
  //BottomToX, At toView Bottom is negative
  fourteenthButton.LeftToRight(VerticalCenterGuideline, 20).BottomToTop(firstButton, -20);
  fifteenthButton.LeftToRight(fourteenthButton).BottomToTop(firstButton, 20);
  
  sixteenthButton.LeftToRight(VerticalCenterGuideline, 20).BottomToBottom(firstButton, -20);
  seventeenthButton.LeftToRight(sixteenthButton).BottomToBottom(firstButton, 20);
  ```
  
  
  
  ![newapi](https://github.com/xtuzy/SharpConstraintLayout/blob/master/Resources/NewApi.png)

🦮 **GuideLine** allow reactive layout behavior with fixed or percentage based positioning for multiple widgets.
```
var VerticalCenterGuideline = new GuideLine() { Percent = 0.5f,Orientation=GuideLine.Orientations.VERTICAL };
```

🚧 **BarrierLine** references multiple widgets to create a virtual guideline based on the most extreme widget on the specified side.

```
BarrierLine barrier = new BarrierLine() { BarrierSide = BarrierLine.Side.Bottom };
Page.Children.Add(barrier);
barrier.AddView(firstTextBox);
barrier.AddView(secondTextBox);
```



More example see [ComplexLayoutTest](https://github.com/xtuzy/SharpConstraintLayout/blob/master/SharpConstraintLayout.Example.Reload/ComplexLayoutTestWindow.xaml.cs)


## 🤝 Contributing

If you'd like to get involved and contribute please read [CONTRIBUTING](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.

## 💻 Authors

- **John Hoford** : MotionLayout ([jafu888](https://github.com/jafu888))
- **Nicolas Roard** : ConstraintLayout ([camaelon](https://github.com/camaelon))

See also the list of [contributors](https://github.com/androidx/constraintlayout/graphs/contributors) who participated in this project.

## 🔖 License

This project is licensed under the Apache 2.0 License - see the [LICENSE](LICENSE) file for details



