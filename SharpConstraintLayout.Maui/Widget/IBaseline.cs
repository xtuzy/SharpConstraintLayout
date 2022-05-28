using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui.Widget
{
    /// <summary>
    /// 自定义View在ConstraintLayout中需要Baseline对齐时,实现此接口
    /// </summary>
    public interface IBaseline
    {
        int GetBaseline();
    }
}
