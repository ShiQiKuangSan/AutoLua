using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.Graphics;
using Android.OS;
using AutoLua.Core.AutoAccessibility.Accessibility.Node;

namespace AutoLua.Core.AutoAccessibility.Accessibility.Filter
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class By
    {
        #region 字段

        /// <summary>
        /// 表达式执行者
        /// </summary>
        private readonly IList<IExpressionExecutor> _expressionExecutors = new List<IExpressionExecutor>();

        private readonly IUiNodeSearch _uiNodeSearch;

        #endregion 字段

        public By()
        {
            _uiNodeSearch = new UiNodeSearch();
        }

        /// <summary>
        /// 为当前选择器附加控件"text等于字符串str"的筛选条件。
        /// </summary>
        /// <param name="str">控件文本</param>
        /// <returns></returns>
        public By text(string str)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
            {
                if (string.IsNullOrWhiteSpace(node.Text))
                {
                    return false;
                }

                return node.Text == str;
            }));

            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件"text需要包含字符串str"的筛选条件。
        /// </summary>
        /// <param name="str">要包含的字符串</param>
        /// <returns></returns>
        public By textContains(string str)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
                !string.IsNullOrWhiteSpace(node.Text) && node.Text.Contains(str)));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件"text需要以prefix开头"的筛选条件。
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <returns></returns>
        public By textStartsWith(string prefix)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
                !string.IsNullOrWhiteSpace(node.Text) && node.Text.StartsWith(prefix)));

            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件"text需要以suffix结束"的筛选条件。
        /// </summary>
        /// <param name="suffix">后缀</param>
        /// <returns></returns>
        public By textEndsWith(string suffix)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
                !string.IsNullOrWhiteSpace(node.Text) && node.Text.EndsWith(suffix)));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件"text需要满足正则表达式reg"的条件。
        /// </summary>
        /// <param name="reg">要满足的正则表达式。</param>
        /// <returns></returns>
        public By textMatches(string reg)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
                !string.IsNullOrWhiteSpace(node.Text) && Regex.IsMatch(node.Text, reg)));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件"desc等于字符串str"的筛选条件。
        /// </summary>
        /// <param name="str">控件文本</param>
        /// <returns></returns>
        public By desc(string str)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
            {
                if (string.IsNullOrWhiteSpace(node.Desc))
                {
                    return false;
                }

                return node.Desc == str;
            }));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件"desc需要包含字符串str"的筛选条件。
        /// </summary>
        /// <param name="str">要包含的字符串</param>
        /// <returns></returns>
        public By descContains(string str)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
                !string.IsNullOrWhiteSpace(node.Desc) && node.Desc.Contains(str)));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件"desc需要以prefix开头"的筛选条件。
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <returns></returns>
        public By descStartsWith(string prefix)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
                !string.IsNullOrWhiteSpace(node.Desc) && node.Desc.StartsWith(prefix)));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件"desc需要以suffix结束"的筛选条件。
        /// </summary>
        /// <param name="suffix">后缀</param>
        /// <returns></returns>
        public By descEndsWith(string suffix)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
                !string.IsNullOrWhiteSpace(node.Desc) && node.Desc.EndsWith(suffix)));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件"desc需要满足正则表达式reg"的条件。
        /// </summary>
        /// <param name="reg">要满足的正则表达式。</param>
        /// <returns></returns>
        public By descMatches(string reg)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
                !string.IsNullOrWhiteSpace(node.Desc) && Regex.IsMatch(node.Desc, reg)));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加"id等于resId"的筛选条件。
        /// </summary>
        /// <param name="resId">控件的id，以"包名:id/"开头，例如"com.tencent.mm:id/send_btn"。也可以不指定包名，这时会以当前正在运行的应用的包名来补全id。例如id("send_btn"),在QQ界面想当于id("com.tencent.mobileqq:id/send_btn")。</param>
        /// <returns></returns>
        public By id(string resId)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
            {
                if (string.IsNullOrWhiteSpace(node.FullId))
                {
                    return false;
                }

                return node.FullId == resId;
            }));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件"id包含字符串str"的筛选条件。比较少用。
        /// </summary>
        /// <param name="str">id要包含的字符串</param>
        /// <returns></returns>
        public By idContains(string str)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
            {
                if (string.IsNullOrWhiteSpace(node.FullId))
                {
                    return false;
                }

                return node.FullId.Contains(str);
            }));

            return this;
        }

        /// <summary>
        /// 为当前选择器附加"id需要以prefix开头"的筛选条件。比较少用。
        /// </summary>
        /// <param name="prefix">id前缀</param>
        /// <returns></returns>
        public By idStartsWith(string prefix)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
            {
                if (string.IsNullOrWhiteSpace(node.FullId))
                {
                    return false;
                }

                return node.FullId.StartsWith(prefix);
            }));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加"id需要以suffix结束"的筛选条件。比较少用。
        /// </summary>
        /// <param name="suffix">id后缀</param>
        /// <returns></returns>
        public By idEndsWith(string suffix)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
            {
                if (string.IsNullOrWhiteSpace(node.FullId))
                {
                    return false;
                }

                return node.FullId.EndsWith(suffix);
            }));
            return this;
        }

        /// <summary>
        /// 附加id需要满足正则表达式。
        /// </summary>
        /// <remarks>
        /// 需要注意的是，如果正则表达式是字符串，则需要使用\\来表达\(也即Java正则表达式的形式)，例如textMatches("\\d+")匹配多位数字；但如果使用JavaScript语法的正则表达式则不需要，例如textMatches(/\d+/)。但如果使用字符串的正则表达式则该字符串不能以"/"同时以"/"结束，也即不能写诸如textMatches("/\\d+/")的表达式，否则会被开头的"/"和结尾的"/"会被忽略。
        /// </remarks>
        /// <param name="reg">id要满足的正则表达式</param>
        /// <returns></returns>
        public By idMatches(string reg)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
                !string.IsNullOrWhiteSpace(node.FullId) &&
                Regex.IsMatch(node.FullId, reg)));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件"className等于字符串str"的筛选条件。
        /// </summary>
        /// <param name="str">控件文本</param>
        /// <returns></returns>
        public By className(string str)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
            {
                if (string.IsNullOrWhiteSpace(node.ClassName))
                {
                    return false;
                }

                return node.ClassName == str;
            }));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件"className需要包含字符串str"的筛选条件。
        /// </summary>
        /// <param name="str">要包含的字符串</param>
        /// <returns></returns>
        public By classNameContains(string str)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
                !string.IsNullOrWhiteSpace(node.ClassName) && node.ClassName.Contains(str)));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件"className需要以prefix开头"的筛选条件。
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <returns></returns>
        public By classNameStartsWith(string prefix)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
                !string.IsNullOrWhiteSpace(node.ClassName) && node.ClassName.StartsWith(prefix)));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件"className需要以suffix结束"的筛选条件。
        /// </summary>
        /// <param name="suffix">后缀</param>
        /// <returns></returns>
        public By classNameEndsWith(string suffix)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
                !string.IsNullOrWhiteSpace(node.ClassName) && node.ClassName.EndsWith(suffix)));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件"className需要满足正则表达式reg"的条件。
        /// </summary>
        /// <param name="reg">要满足的正则表达式。</param>
        /// <returns></returns>
        public By classNameMatches(string reg)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
                !string.IsNullOrWhiteSpace(node.ClassName) && Regex.IsMatch(node.ClassName, reg)));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件"packageName等于字符串str"的筛选条件。
        /// </summary>
        /// 控件的packageName表示控件所属界面的应用包名。例如微信的包名为"com.tencent.mm", 那么微信界面的控件的packageName为"com.tencent.mm"。
        /// <remarks>
        /// </remarks>
        /// <param name="str">控件文本</param>
        /// <returns></returns>
        public By packageName(string str)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
            {
                if (string.IsNullOrWhiteSpace(node.PackageName))
                {
                    return false;
                }

                return node.PackageName == str;
            }));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件"packageName需要包含字符串str"的筛选条件。
        /// </summary>
        /// <param name="str">要包含的字符串</param>
        /// <returns></returns>
        public By packageNameContains(string str)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
                !string.IsNullOrWhiteSpace(node.PackageName) && node.PackageName.Contains(str)));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件"packageName需要以prefix开头"的筛选条件。
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <returns></returns>
        public By packageNameStartsWith(string prefix)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
                !string.IsNullOrWhiteSpace(node.PackageName) && node.PackageName.StartsWith(prefix)));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件"packageName需要以suffix结束"的筛选条件。
        /// </summary>
        /// <param name="suffix">后缀</param>
        /// <returns></returns>
        public By packageNameEndsWith(string suffix)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
                !string.IsNullOrWhiteSpace(node.PackageName) && node.PackageName.EndsWith(suffix)));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件"packageName需要满足正则表达式reg"的条件。
        /// </summary>
        /// <param name="reg">要满足的正则表达式。</param>
        /// <returns></returns>
        public By packageNameMatches(string reg)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
                !string.IsNullOrWhiteSpace(node.PackageName) && Regex.IsMatch(node.PackageName, reg)));
            return this;
        }

        /// <summary>
        /// 一个控件的bounds属性为这个控件在屏幕上显示的范围。我们可以用这个范围来定位这个控件。尽管用这个方法定位控件对于静态页面十分准确，却无法兼容不同分辨率的设备；同时对于列表页面等动态页面无法达到效果，因此使用不推荐该选择器。
        /// </summary>
        /// <param name="left">控件左边缘与屏幕左边的距离</param>
        /// <param name="top">控件上边缘与屏幕上边的距离</param>
        /// <param name="right">控件右边缘与屏幕左边的距离</param>
        /// <param name="bottom">控件下边缘与屏幕上边的距离</param>
        /// <returns></returns>
        public By bounds(int left, int top, int right, int bottom)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
            {
                var parentNode = new Rect();
                node.GetBoundsInScreen(parentNode);

                return parentNode == new Rect(left, top, right, bottom);
            }));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件"bounds需要在left, top, right, buttom构成的范围里面"的条件。
        /// </summary>
        /// <param name="left">控件左边缘与屏幕左边的距离</param>
        /// <param name="top">控件上边缘与屏幕上边的距离</param>
        /// <param name="right">控件右边缘与屏幕左边的距离</param>
        /// <param name="bottom">控件下边缘与屏幕上边的距离</param>
        /// <returns></returns>
        public By boundsInside(int left, int top, int right, int bottom)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
            {
                var parentNode = new Rect();
                node.GetBoundsInScreen(parentNode);

                return new Rect(left, top, right, bottom).Contains(parentNode);
            }));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件"bounds需要包含left, top, right, buttom构成的范围"的条件。
        /// </summary>
        /// <param name="left">控件左边缘与屏幕左边的距离</param>
        /// <param name="top">控件上边缘与屏幕上边的距离</param>
        /// <param name="right">控件右边缘与屏幕左边的距离</param>
        /// <param name="bottom">控件下边缘与屏幕上边的距离</param>
        /// <returns></returns>
        public By boundsContains(int left, int top, int right, int bottom)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
            {
                var parentNode = new Rect();
                node.GetBoundsInScreen(parentNode);

                return parentNode.Contains(new Rect(left, top, right, bottom));
            }));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件"drawingOrder等于order"的条件。
        /// </summary>
        /// <param name="order">控件在父视图中的绘制顺序</param>
        /// <returns></returns>
        public By drawingOrder(int order)
        {
            _expressionExecutors.Add(new ExpressionExecutor(node =>
                Build.VERSION.SdkInt >= BuildVersionCodes.N && node.DrawingOrder == order));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件是否可点击的条件。但并非所有clickable为false的控件都真的不能点击，这取决于控件的实现。对于自定义控件(例如显示类名为android.view.View的控件)很多的clickable属性都为false都却能点击。
        /// </summary>
        /// <param name="b">表示控件是否可点击</param>
        /// <returns></returns>
        public By clickable(bool b = true)
        {
            _expressionExecutors.Add(new ExpressionExecutor((node) => node.Clickable == b));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件是否可长按的条件。
        /// </summary>
        /// <param name="b">表示控件是否可长按</param>
        /// <returns></returns>
        public By longClickable(bool b = true)
        {
            _expressionExecutors.Add(new ExpressionExecutor((node) => node.LongClickable == b));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件是否可勾选的条件。勾选通常是对于勾选框而言的，例如图片多选时左上角通常有一个勾选框。
        /// </summary>
        /// <param name="b">表示控件是否可勾选</param>
        /// <returns></returns>
        public By checkable(bool b = true)
        {
            _expressionExecutors.Add(new ExpressionExecutor((node) => node.Checkable == b));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public By checkeds(bool b = true)
        {
            _expressionExecutors.Add(new ExpressionExecutor((node) => node.Checked == b));
            return this;
        }

        public By password(bool b = true)
        {
            _expressionExecutors.Add(new ExpressionExecutor((node) => node.Password == b));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件是否已选中的条件。被选中指的是，例如QQ聊天界面点击下方的"表情按钮"时，会出现自己收藏的表情，这时"表情按钮"便处于选中状态，其selected属性为true。
        /// </summary>
        /// <param name="b">表示控件是否被选</param>
        /// <returns></returns>
        public By selected(bool b = true)
        {
            _expressionExecutors.Add(new ExpressionExecutor((node) => node.IsSelected == b));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件是否已启用的条件。大多数控件都是启用的状态(enabled为true)，处于“禁用”状态通常是灰色并且不可点击。
        /// </summary>
        /// <param name="b">表示控件是否已启用</param>
        /// <returns></returns>
        public By enabled(bool b = true)
        {
            _expressionExecutors.Add(new ExpressionExecutor((node) => node.Enabled == b));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件是否可滑动的条件。滑动包括上下滑动和左右滑动。
        /// </summary>
        /// <param name="b">表示控件是否可滑动</param>
        /// <returns></returns>
        public By scrollable(bool b = true)
        {
            _expressionExecutors.Add(new ExpressionExecutor((node) => node.Scrollable == b));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件是否可编辑的条件。一般来说可编辑的控件为输入框(EditText)，但不是所有的输入框(EditText)都可编辑。
        /// </summary>
        /// <param name="b">表示控件是否可编辑</param>
        /// <returns></returns>
        public By editable(bool b = true)
        {
            _expressionExecutors.Add(new ExpressionExecutor((node) => node.Editable == b));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加控件是否文本或输入框控件是否是多行显示的条件。
        /// </summary>
        /// <param name="b">表示文本或输入框控件是否是多行显示的</param>
        /// <returns></returns>
        public By multiLine(bool b = true)
        {
            _expressionExecutors.Add(new ExpressionExecutor((node) => node.MultiLine == b));
            return this;
        }

        /// <summary>
        /// 为当前选择器附加自定义的过滤条件。
        /// </summary>
        /// <param name="action">过滤函数，参数为UiNode，返回值为bool</param>
        /// <returns></returns>
        public By filter(ByFilterDelegate action)
        {
            _expressionExecutors.Add(new ExpressionExecutor((node) => action(node)));
            return this;
        }

        /// <summary>
        /// 根据当前的选择器所确定的筛选条件，对屏幕上的控件进行搜索，直到屏幕上出现满足条件的一个控件为止，并返回该控件。如果找不到控件，当屏幕内容发生变化时会重新寻找，直至找到。
        /// </summary>
        /// <returns></returns>
        public UiNode findOne()
        {
            var node = Execute(null, 1).FirstOrDefault();

            if (node != null)
                return node;

            var task = new Task(async () =>
            {
                while (node == null)
                {
                    await Task.Delay(100);
                    node = Execute(null, 1).FirstOrDefault();
                }
            });

            task.Start();

            task.Wait();

            return node;
        }

        /// <summary>
        /// 根据当前的选择器所确定的筛选条件，对屏幕上的控件进行搜索，直到屏幕上出现满足条件的一个控件为止，并返回该控件；如果在timeout毫秒的时间内没有找到符合条件的控件，则终止搜索并返回null。
        /// </summary>
        /// <param name="timeout">搜索的超时时间，单位毫秒</param>
        /// <returns></returns>
        public UiNode findOne(int timeout)
        {
            UiNode node = null;

            var start = SystemClock.UptimeMillis();

            do
            {
                if (SystemClock.UptimeMillis() - start > timeout)
                {
                    break;
                }

                System.Threading.Thread.Sleep(100);

                node = Execute(null, 1).FirstOrDefault();
            } while (node == null);

            return node;
        }

        /// <summary>
        /// 根据当前的选择器所确定的筛选条件，对屏幕上的控件进行搜索，如果找到符合条件的控件则返回该控件；否则返回null。
        /// </summary>
        /// <returns></returns>
        public UiNode findOnce()
        {
            return Execute(null, 1).FirstOrDefault();
        }

        /// <summary>
        /// 根据当前的选择器所确定的筛选条件，对屏幕上的控件进行搜索，如果找到符合条件的控件则返回该控件；否则返回null。
        /// </summary>
        /// <param name="node">根据当前节点进行子节点查找</param>
        /// <returns></returns>
        public UiNode findOnce(UiNode node)
        {
            return Execute(node, 1).FirstOrDefault();
        }

        /// <summary>
        /// 根据当前的选择器所确定的筛选条件，对屏幕上的控件进行搜索，找到所有满足条件的控件集合并返回。这个搜索只进行一次，并不保证一定会找到，因而会出现返回的控件集合为空的情况。
        /// </summary>
        /// <returns></returns>
        public IList<UiNode> find()
        {
            return Execute().ToList();
        }

        /// <summary>
        /// 根据当前的选择器所确定的筛选条件，对屏幕上的控件进行搜索，找到所有满足条件的控件集合并返回。这个搜索只进行一次，并不保证一定会找到，因而会出现返回的控件集合为空的情况。
        /// </summary>
        /// <returns></returns>
        public IList<UiNode> find(UiNode node)
        {
            return Execute(node).ToList();
        }

        /// <summary>
        /// 判断屏幕上是否存在控件符合选择器所确定的条件。例如要判断某个文本出现就执行某个动作
        /// </summary>
        /// <returns></returns>
        public bool exists()
        {
            return Execute(null, 1).Any();
        }

        /// <summary>
        /// 等待屏幕上出现符合条件的控件；在满足该条件的控件出现之前，该函数会一直保持阻塞。
        /// </summary>
        /// <returns></returns>
        public bool waitFor()
        {
            var status = Execute(null, 1).Any();

            var task = new Task(async () =>
            {
                while (status == false)
                {
                    await Task.Delay(100);
                    status = Execute(null, 1).Any();
                }
            });

            task.Start();

            task.Wait();

            return status;
        }

        /// <summary>
        /// 等待屏幕上出现符合条件的控件；在满足该条件的控件出现之前，该函数会一直保持阻塞。
        /// </summary>
        /// <param name="timeout">轮询时间 单位毫秒</param>
        /// <returns></returns>
        public bool waitFor(int timeout = 1000)
        {
            var status = Execute(null, 1).Any();

            var task = new Task(async () =>
            {
                while (status == false)
                {
                    await Task.Delay(timeout);
                    status = Execute(null, 1).Any();
                }
            });

            task.Start();

            task.Wait();

            return status;
        }

        /// <summary>
        /// 返回表达式的格式
        /// </summary>
        /// <returns></returns>
        public string toString()
        {
            return _expressionExecutors.ToString();
        }

        private IEnumerable<UiNode> Execute(UiNode child = null, int max = int.MaxValue)
        {
            if (AutoAccessibilityService.Instance == null)
                return new List<UiNode>();

            if (!AutoAccessibilityService.Instance.Windows.ToList().Any())
            {
                return new List<UiNode>();
            }

            var windows = AutoAccessibilityService.Instance.Windows
                .Where(x => x != null)
                .Select(x => new UiNode(x.Root))
                .Where(x => x.VisibleToUser).ToList();

            var list = new List<UiNode>();

            if (child != null)
            {
                //查找给定节点的子节点是否有符合要求的
                foreach (var window in windows)
                {
                    var items = _uiNodeSearch.Search(child, _expressionExecutors).ToList();
                    list.AddRange(items);
                    if (list.Count >= max)
                        break;
                }
            }
            else
            {
                foreach (var node in windows)
                {
                    var items = _uiNodeSearch.Search(node, _expressionExecutors).ToList();

                    list.AddRange(items);
                    if (list.Count >= max)
                        break;
                }
            }

            return list;
        }
    }
}