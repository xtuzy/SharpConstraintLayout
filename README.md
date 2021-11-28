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

**Hello World**

 Create layout and add children view.

```
var layout = new ConstraintLayout();
var firstButton = new Button(){Constent="first"};
var firstButton = new Button(){Content="second"};
layout.Children.Add(firstButton)
layout.Children.Add(secondButton)
```

Create constraint.

```
new ConstraintSet(constraintlayout)
.AddConnect(firstButton, ConstraintAnchor.Type.CENTER, layout, ConstraintAnchor.Type.CENTER)
.AddConnect(secondButton,ConstraintAnchor.Type.LEFT,firstdButton,ConstraintAnchor.Type.RIGHT)
.AddConnect(secondButton, ConstraintAnchor.Type.TOP,firstButton,ConstraintAnchor.Type.BOTTOM);
```

🦮 **Guidelines** allow reactive layout behavior with fixed or percentage based positioning for multiple widgets.

More example see [ComplexLayoutTest](https://github.com/xtuzy/SharpConstraintLayout/blob/master/SharpConstraintLayout.Example.Reload/ComplexLayoutTestWindow.xaml.cs)

## 🔖 License

This project is licensed under the Apache 2.0 License - see the [LICENSE](LICENSE) file for details



