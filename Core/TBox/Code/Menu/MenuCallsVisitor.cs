using System;
using System.Collections.Generic;
using System.Linq;
using Mnk.Library.Common.Log;
using Mnk.Library.WpfControls.Menu;

namespace Mnk.TBox.Core.Application.Code.Menu
{
    class MenuCallsVisitor
    {
        private readonly IList<IMenuRunHandler> handlers = new List<IMenuRunHandler>();
        private static readonly ILog InfoLog = LogManager.GetInfoLogger<MenuCallsVisitor>();

        public void AddHandler(IMenuRunHandler handler)
        {
            handlers.Add(handler);
        }

        public void ApplyMenuItems(params UMenuItem[] menuItems)
        {
            ApplyCustomCallback(menuItems, Array.Empty<string>());
        }

        private void ApplyCustomCallback(IEnumerable<UMenuItem> menuItems, IList<string> path)
        {
            foreach (var item in menuItems)
            {
                if (item.Items.Any())
                {
                    ApplyCustomCallback(item.Items, path.Concat(new[] { item.Header }).ToArray());
                }
                else if (item.OnClick != null)
                {
                    var itemPath = path.Concat(new[] { item.Header }).ToArray();
                    var clone = new UMenuItem
                    {
                        Header = item.Header,
                        Icon = item.Icon,
                        OnClick = item.OnClick
                    };
                    item.OnClick = o => VisitMenuItem(o, clone, itemPath);
                }
            }
        }

        private void VisitMenuItem(object o, UMenuItem item, string[] path)
        {
            foreach (var h in handlers)
            {
                h.Handle(item, path);
            }
            var time = Environment.TickCount;
            item.OnClick(o);
            InfoLog.Write("Open time: {0}, menu element: {1}",
                Environment.TickCount - time,
                string.Join("->", path)
                );
        }

        public void RemoveMenuItem(UMenuItem item)
        {
            if (item != null) item.OnClick = null;
        }

        public void ClearHandlers()
        {
            handlers.Clear();
        }
    }
}
