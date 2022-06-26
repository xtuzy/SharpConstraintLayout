# SharpConstraintLayout 🗜️📏

This is a C# port of [ConstraintLayout](https://github.com/androidx/constraintlayout), it convert [constraintlayout.core](https://github.com/androidx/constraintlayout/tree/main/constraintlayout/core) and create ConstraintLayout for dotnet UI framework. Now, you can use ConstraintLayout in C# world 🎆

[![NuGet version(SharpConstraintLayout.Core)](https://img.shields.io/nuget/v/SharpConstraintLayout.Core?label=SharpConstraintLayout.Core)](https://www.nuget.org/packages/SharpConstraintLayout.Core/), it contain core logic of constraintlayout, you can base on it create your ConstraintLayout for other dotnet UI framework.

[![NuGet version(SharpConstraintLayout.Maui.Native)](https://img.shields.io/nuget/v/SharpConstraintLayout.Maui.Native?label=SharpConstraintLayout.Maui.Native)](https://www.nuget.org/packages/SharpConstraintLayout.Maui.Native/), it contain layout for net6-android,net6-ios,winui3, such as at android, you can use it instead [Xamarin.AndroidX.ConstraintLayout](https://www.nuget.org/packages/Xamarin.AndroidX.ConstraintLayout/)

[![NuGet version(SharpConstraintLayout.Maui)](https://img.shields.io/nuget/v/SharpConstraintLayout.Maui?label=SharpConstraintLayout.Maui)](https://www.nuget.org/packages/SharpConstraintLayout.Maui/), it contain layout for Maui, ConstraintLayout is powerful, you can use it instead other layout


## Using SharpConstraintLayout

[![Maui SharpConstraintLayout Example](https://youtube-md.vercel.app/d5nUq7CQuck/640/360)](https://www.youtube.com/watch?v=d5nUq7CQuck)

**Installation**

Search and install nuget **SharpConstraintLayout.Maui** (For Maui) or  **SharpConstraintLayout.Maui.Native** (For net6-android,net6-ios,WinUI3)

**Notice**
1. Now you create constraint only by code.
2. Here all example use Maui
3. Run example app, you can find more example

**Basis Align**
First you need create layout and add controls to layout,
```
var layout = new ConstraintLayout();
var firstButton = new Button(){ Text = "first"};
var secondButton = new Button(){ Text = "second"};
layout.AddElement(firstButton,secondButton)
```
then set first button at center of layout.
```
using(var set = FluentConstraintSet())
{
  set.Clone(layout);
  set.Select(firstButton).CenterTo()
  set.ApplyTo(layout);
}
```
or you can set second button align first button
```
using(var set = FluentConstraintSet())
{
  set.Clone(layout);
  set.Select(firstButton).CenterTo()
  .Select(secondButton).LeftToRight(firstButton).CenterYTo(firstButton)
  set.ApplyTo(layout);
}
```

## Current Features
- [x] Align Left,Right,Top,Bottom,Center
- [x] **Ratio of Width with Height**
Ratio defines one dimension of a widget as a ratio of the other one. If both `width` and `height` are set to `0dp` the system sets the largest dimensions that satisfy all constraints while maintaining the aspect ratio.

- [x] **Chains** 
Chains provide group-like behavior in a single axis (horizontally or vertically). The other axis can be constrained independently.

- [x] **Guideline** 
Guideline allow reactive layout behavior with fixed or percentage based positioning for multiple widgets.

- [x] **Barrier** 
Barrier references multiple widgets to create a virtual guideline based on the most extreme widget on the specified side.

- [x] **Flow** 
Flow is a VirtualLayout that allows positioning of referenced widgets horizontally or vertically similar to a Chain. If the referenced elements do not fit within the given bounds it has the ability to wrap them and create multiple chains.
- [x] **Group** 
Group can control visibility of multiple views.
- [ ] RTL
- [ ] Align Baseline

## 💻 Authors of ConstraintLayout

- **John Hoford** : MotionLayout ([jafu888](https://github.com/jafu888))
- **Nicolas Roard** : ConstraintLayout ([camaelon](https://github.com/camaelon))

See also the list of [contributors](https://github.com/androidx/constraintlayout/graphs/contributors) who participated in this project.

## 🔖 License

This project is licensed under the Apache 2.0 License - see the [LICENSE](LICENSE) file for details



