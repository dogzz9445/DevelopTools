using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDT.Core.WidgetSystems.Bases;
public static class WidgetLayoutConstants
{
    public const int MaxCols = 16;
    public const int VisibleRows = 8;
}

public interface IWidgetLayoutXY
{
    int X { get; }
    int Y { get; }
}

public interface IWidgetLayoutWH
{
    int W { get; }
    int H { get; }
}

public interface IWidgetLayoutItemRect : IWidgetLayoutXY, IWidgetLayoutWH { }

public interface IWidgetLayoutItem : IEntity
{
    Guid WidgetId { get; }
    IWidgetLayoutItemRect Rect { get; }
}

public interface IEntityList<T> : IList<T> where T : IEntity { }

public class WidgetLayoutItemRect : IWidgetLayoutItemRect
{
    public int X { get; set; }
    public int Y { get; set; }
    public int W { get; set; }
    public int H { get; set; }
}

public class WidgetLayout : IWidgetLayoutItem
{
    public Guid Id { get; set; }
    public Guid WidgetId { get; set; }
    public IWidgetLayoutItemRect Rect { get; set; }
}

public class Dashboard : List<WidgetLayout>, IEntityList<WidgetLayout> { }

public class WidgetLayoutItemMutableRect : WidgetLayout
{
    public new WidgetLayoutItemRect Rect { get; set; }
    public bool Initiator { get; set; }
}

public static class WidgetLayoutUtils
{
    public static bool ItemsCollide(WidgetLayout item1, WidgetLayout item2)
    {
        return (
            item1.Id != item2.Id &&
            item1.Rect.X + item1.Rect.W > item2.Rect.X && item2.Rect.X + item2.Rect.W > item1.Rect.X &&
            item1.Rect.Y + item1.Rect.H > item2.Rect.Y && item2.Rect.Y + item2.Rect.H > item1.Rect.Y
        );
    }

    public static List<T> GetCollisions<T>(List<T> items, WidgetLayout item) where T : WidgetLayout
    {
        return items.Where(i => ItemsCollide(i, item)).ToList();
    }

    public static List<T> SortItems<T>(List<T> items) where T : WidgetLayout
    {
        return items.OrderByDescending(a => a.Rect.Y).ThenByDescending(a => a.Rect.X).ToList();
    }

    public static List<WidgetLayoutItemMutableRect> FixCollisionsIter(
        List<WidgetLayoutItemMutableRect> layout,
        WidgetLayoutItemMutableRect item)
    {
        var sorted = SortItems(layout);
        var collisions = GetCollisions(sorted, item);

        foreach (var collision in collisions)
        {
            if (collision.Initiator) continue;

            collision.Rect.Y = item.Rect.Y + item.Rect.H;
            layout = FixCollisionsIter(layout, collision);
        }

        return layout;
    }

    public static List<WidgetLayout> FixCollisions(
        List<WidgetLayout> layout,
        Guid itemId)
    {
        var layoutMutableRect = layout.Select(item => new WidgetLayoutItemMutableRect
        {
            Id = item.Id,
            WidgetId = item.WidgetId,
            Rect = new WidgetLayoutItemRect { X = item.Rect.X, Y = item.Rect.Y, W = item.Rect.W, H = item.Rect.H },
            Initiator = item.Id == itemId
        }).ToList();

        var item = layoutMutableRect.FirstOrDefault(i => i.Id == itemId);
        if (item == null) return layout;

        var newLayout = FixCollisionsIter(layoutMutableRect, item)
            .Select(i => new WidgetLayout
            {
                Id = i.Id,
                WidgetId = i.WidgetId,
                Rect = i.Rect
            }).ToList();

        return newLayout;
    }

    public static WidgetLayoutItemRect FixRect(WidgetLayoutItemRect rect)
    {
        int x = Math.Max(0, Math.Min(WidgetLayoutConstants.MaxCols - rect.W, rect.X));
        int y = Math.Max(0, rect.Y);
        int w = Math.Max(0, Math.Min(WidgetLayoutConstants.MaxCols, rect.W));
        int h = Math.Max(0, rect.H);
        return new WidgetLayoutItemRect { X = x, Y = y, W = w, H = h };
    }

    public static List<WidgetLayout> UpdateLayoutItemRect(
        List<WidgetLayout> layout,
        Guid itemId,
        WidgetLayoutItemRect newRect)
    {
        var newLayout = layout.Select(item =>
        {
            if (item.Id == itemId)
                return new WidgetLayout
                {
                    Id = item.Id,
                    WidgetId = item.WidgetId,
                    Rect = newRect
                };
            else
                return item;
        }).ToList();

        return FixCollisions(newLayout, itemId);
    }

    public static List<WidgetLayout> CreateLayout()
    {
        return new List<WidgetLayout>();
    }

    public static (List<WidgetLayout> Layout, WidgetLayout LayoutItem) CreateLayoutItem(
        List<WidgetLayout> layout,
        Guid id,
        WidgetLayoutItemRect rect,
        Guid widgetId)
    {
        if (layout.Any(item => item.Id == id))
            return (layout, null);

        var newItem = new WidgetLayout
        {
            Id = id,
            Rect = FixRect(rect),
            WidgetId = widgetId
        };

        var newLayout = new List<WidgetLayout>(layout) { newItem };
        newLayout = FixCollisions(newLayout, id);

        return (newLayout, newItem);
    }

    public static (List<WidgetLayout> Layout, WidgetLayout LayoutItem) CreateLayoutItemAtFreeArea(
        List<WidgetLayout> layout,
        Guid id,
        WidgetLayoutItemRect size,
        Guid widgetId)
    {
        if (layout.Any(item => item.Id == id))
            return (layout, null);

        if (size.W > WidgetLayoutConstants.MaxCols)
            return (layout, null);

        var sorted = SortItems(layout);

        for (int y = 0; ; y++)
        {
            for (int x = 0; x <= WidgetLayoutConstants.MaxCols - size.W;)
            {
                var item = new WidgetLayout
                {
                    Id = id,
                    WidgetId = widgetId,
                    Rect = new WidgetLayoutItemRect { X = x, Y = y, W = size.W, H = size.H }
                };
                var collisions = GetCollisions(sorted, item);
                if (!collisions.Any())
                {
                    var newLayout = new List<WidgetLayout>(layout) { item };
                    return (newLayout, item);
                }
                else
                {
                    x = collisions[0].Rect.X + collisions[0].Rect.W;
                }
            }
        }
    }

    public static List<WidgetLayout> MoveLayoutItem(
        List<WidgetLayout> layout,
        Guid itemId,
        IWidgetLayoutXY toXY)
    {
        var item = layout.FirstOrDefault(i => i.Id == itemId);
        if (item == null) return layout;

        var newRect = FixRect(new WidgetLayoutItemRect
        {
            X = toXY.X,
            Y = toXY.Y,
            W = item.Rect.W,
            H = item.Rect.H
        });

        if (item.Rect.Y == newRect.Y && item.Rect.X == newRect.X)
            return layout;

        return UpdateLayoutItemRect(layout, itemId, newRect);
    }

    public static List<WidgetLayout> RemoveLayoutItem(
        List<WidgetLayout> layout,
        Guid itemId)
    {
        return layout.Where(item => item.Id != itemId).ToList();
    }

    public static List<WidgetLayout> ResizeLayoutItemByEdges(
        List<WidgetLayout> layout,
        Guid itemId,
        (int? Left, int? Top, int? Right, int? Bottom) delta,
        (int W, int H) minSize)
    {
        var item = layout.FirstOrDefault(i => i.Id == itemId);
        if (item == null) return layout;

        var rect = item.Rect;
        int x = rect.X, y = rect.Y, w = rect.W, h = rect.H;

        if (delta.Left.HasValue)
        {
            int deltaLeft = Math.Min(x, Math.Max(minSize.W - w, delta.Left.Value));
            x -= deltaLeft;
            w += deltaLeft;
        }
        if (delta.Top.HasValue)
        {
            int deltaTop = Math.Min(y, Math.Max(minSize.H - h, delta.Top.Value));
            y -= deltaTop;
            h += deltaTop;
        }
        if (delta.Right.HasValue)
        {
            int deltaRight = Math.Min(WidgetLayoutConstants.MaxCols - x - w, Math.Max(minSize.W - w, delta.Right.Value));
            w += deltaRight;
        }
        if (delta.Bottom.HasValue)
        {
            int deltaBottom = Math.Max(minSize.H - h, delta.Bottom.Value);
            h += deltaBottom;
        }

        var newRect = new WidgetLayoutItemRect { X = x, Y = y, W = w, H = h };

        if (rect.Y == newRect.Y && rect.X == newRect.X && rect.W == newRect.W && rect.H == newRect.H)
            return layout;

        return UpdateLayoutItemRect(layout, itemId, newRect);
    }

    private static WidgetLayout FindEntityOnList(List<WidgetLayout> layout, Guid itemId)
    {
        return layout.FirstOrDefault(item => item.Id == itemId);
    }

    private static int FindEntityIndexOnList(List<WidgetLayout> layout, Guid itemId)
    {
        return layout.FindIndex(item => item.Id == itemId);
    }

    private static List<WidgetLayout> RemoveEntityFromListAtIndex(List<WidgetLayout> layout, int index)
    {
        layout.RemoveAt(index);
        return layout;
    }

    private static List<WidgetLayout> UpdateEntityOnList(List<WidgetLayout> layout, WidgetLayout updatedItem)
    {
        return layout.Select(item => item.Id == updatedItem.Id ? updatedItem : item).ToList();
    }
}