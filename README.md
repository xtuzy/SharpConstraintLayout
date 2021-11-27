# SharpConstraintLayout 🗜️📏

![](https://img.shields.io/nuget/v/SharpConstraintLayout.Core?label=SharpConstraintLayout.Core)![](https://img.shields.io/nuget/v/SharpConstraintLayout.Wpf?label=SharpConstraintLayout.Wpf)

This is a C# port of [ConstraintLayout](https://github.com/androidx/constraintlayout), it convert [constraintlayout.core](https://github.com/androidx/constraintlayout/tree/main/constraintlayout/core) and create ConstraintLayout for WPF. Now,you can use ConstraintLayout in C# world 🎆

## Using ConstraintLayout

**Notice**: now you can only create constraint by code,

Install nuget

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



More example see [test](https://github.com/xtuzy/SharpConstraintLayout/tree/master/SharpConstraintLayout.Example.Reload)

## 🔖 License

This project is licensed under the Apache 2.0 License - see the [LICENSE](LICENSE) file for details



