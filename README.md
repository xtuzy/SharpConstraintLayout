# SharpConstraintLayout 🗜️📏

[![NuGet version(SharpConstraintLayout.Core)](https://img.shields.io/nuget/v/SharpConstraintLayout.Core?label=SharpConstraintLayout.Core)](https://www.nuget.org/packages/SharpConstraintLayout.Core/)
[![NuGet version(SharpConstraintLayout.Maui.Native)](https://img.shields.io/nuget/v/SharpConstraintLayout.Maui.Native?label=SharpConstraintLayout.Maui.Native)](https://www.nuget.org/packages/SharpConstraintLayout.Maui.Native/)
[![NuGet version(SharpConstraintLayout.Maui)](https://img.shields.io/nuget/v/SharpConstraintLayout.Maui?label=SharpConstraintLayout.Maui)](https://www.nuget.org/packages/SharpConstraintLayout.Maui/)
This is a C# port of [ConstraintLayout](https://github.com/androidx/constraintlayout), it convert [constraintlayout.core](https://github.com/androidx/constraintlayout/tree/main/constraintlayout/core) and create ConstraintLayout for WPF. Now, you can use ConstraintLayout in C# world 🎆

## Using ConstraintLayout

### ⬇️ Installation

Search and install nuget **SharpConstraintLayout.Maui.Native**

If you want custom constraintlayout, you can install SharpConstraintLayout.Core, it base on net6.

### ✨🤩📱 Key Features

**Notice**: now you can only create constraint by code.

Create layout and add children view.

```
var layout = new ConstraintLayout();
var firstButton = new Button(){Content="first"};
layout.AddElement(firstButton)
using(var set = FluentConstraintSet())
{
  set.Clone(layout);
  set.Select(firstButton).CenterTo(layout)
  .Width(SizeBehavier.WrapContent).Height(SizeBehavier.WrapContent);
  set.ApplyTo(layout);
}
```

📐 **Ratio of Width with Height** defines one dimension of a widget as a ratio of the other one. If both `width` and `height` are set to `0dp` the system sets the largest dimensions that satisfy all constraints while maintaining the aspect ratio.

⛓️ **Chains** provide group-like behavior in a single axis (horizontally or vertically). The other axis can be constrained independently.

🦮 **Guideline** allow reactive layout behavior with fixed or percentage based positioning for multiple widgets.

🚧 **Barrier** references multiple widgets to create a virtual guideline based on the most extreme widget on the specified side.

🌊 **Flow** is a VirtualLayout that allows positioning of referenced widgets horizontally or vertically similar to a Chain. If the referenced elements do not fit within the given bounds it has the ability to wrap them and create multiple chains.

## 🤝 Contributing

If you'd like to get involved and contribute please read [CONTRIBUTING](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.

## 💻 Authors

- **John Hoford** : MotionLayout ([jafu888](https://github.com/jafu888))
- **Nicolas Roard** : ConstraintLayout ([camaelon](https://github.com/camaelon))

See also the list of [contributors](https://github.com/androidx/constraintlayout/graphs/contributors) who participated in this project.

## 🔖 License

This project is licensed under the Apache 2.0 License - see the [LICENSE](LICENSE) file for details



