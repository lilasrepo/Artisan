using Dalamud.Bindings.ImGui;
using OtterGui.OtterGuiInternal.Enums;
using OtterGuiInternal.Enums;
using OtterGuiInternal.Structs;
using OtterGuiInternal.Utility;

namespace OtterGuiInternal;

public static unsafe class ImGuiInternal
{
    /// <summary> Obtain a wrapped pointer to the current window. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImGuiWindowPtr GetCurrentWindow()
    {
        return ImGuiP.GetCurrentWindow();
    }

    /// <summary> Specify the size of the next item to add. Advances the cursor for that size. </summary>
    /// <param name="boundingBox"> The bounding box of the item. </param>
    /// <param name="textBaseLineY"> Usually FramePadding.Y, or negative to automatically compute. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ItemSize(ImRect boundingBox, float textBaseLineY = -1f)
    {
        ImGuiP.ItemSize(boundingBox, textBaseLineY);
    }

    /// <summary> Specify the size of the next item to add. Advances the cursor for that size. </summary>
    /// <param name="size"> The size of the item. </param>
    /// <param name="textBaseLineY"> Usually FramePadding.Y, or negative to automatically compute. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ItemSize(Vector2 size, float textBaseLineY = -1f)
    {
        ImGuiP.ItemSize(size, textBaseLineY);
    }

    /// <summary> Add an item to the item stack for clipping and interaction. </summary>
    /// <param name="boundingBox"> The bounding box of the item, for things like e.g. hover. </param>
    /// <param name="id"> The unique ID of the item. </param>
    /// <param name="flags"> Additional flags. </param>
    /// <returns> Whether the item could be added successfully. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ItemAdd(ImRect boundingBox, ImGuiId id, ImGuiItemFlags flags = 0)
    {
        return ImGuiP.ItemAdd(boundingBox, (uint)id, null, flags);
    }

    /// <summary> Add an item to the item stack. </summary>
    /// <param name="boundingBox"> The bounding box of the item, for things like e.g. hover. </param>
    /// <param name="id"> The unique ID of the item. </param>
    /// <param name="navBoundingBox"> Another bounding box for navigation. </param>
    /// <param name="flags"> Additional flags. </param>
    /// <returns> Whether the item could be added successfully. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ItemAdd(ImRect boundingBox, ImGuiId id, ImRect navBoundingBox, ImGuiItemFlags flags = 0)
    {
        return ImGuiP.ItemAdd(boundingBox, (uint)id, &navBoundingBox, flags);
    }

    /// <summary> Control the key usage of an item. </summary>
    /// <param name="key"> The key to control. </param>
    /// <param name="flags"> The input flags. </param>
    /// <remarks> This is not yet implemented in our version of ImGui. </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ItemSetKeyOwner(ImGuiKey key, ImGuiInputFlags flags)
    {
        throw new NotImplementedException();
    }

    /// <summary> Control the mouse wheel usage of an item. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ItemSetUsingMouseWheel()
    {
        ImGuiP.SetItemUsingMouseWheel();
    }

    /// <summary> Make an area behave like a button. </summary>
    /// <param name="boundingBox"> The areas bounding box. </param>
    /// <param name="id"> The unique ID of the button. </param>
    /// <param name="hovered"> Returns whether the button is hovered. </param>
    /// <param name="held"> Returns whether the button is hovered and one of the mouse buttons denoted by the flags is held down on it. </param>
    /// <param name="flags"> Misc. flags that control the button behavior. </param>
    /// <returns> Whether the button was clicked this frame. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ButtonBehavior(ImRect boundingBox, ImGuiId id, out bool hovered, out bool held, ImGuiButtonFlags flags = 0)
    {
        hovered = false;
        held    = false;
        return ImGuiP.ButtonBehavior(boundingBox, (uint)id, ref hovered, ref held, flags);
    }

    /// <summary> Render a navigation highlight for things like drag & drop I assume. </summary>
    /// <param name="boundingBox"> The bounding box of the object. </param>
    /// <param name="id"> The unique ID of the object. </param>
    /// <param name="flags"> Misc. flags. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderNavHighlight(ImRect boundingBox, ImGuiId id, ImGuiNavHighlightFlags flags = 0)
    {
        ImGuiP.RenderNavHighlight(boundingBox, (uint)id, flags);
    }


    /// <summary> Render a general frame. </summary>
    /// <param name="min"> One corner of the frame.  </param>
    /// <param name="max"> The other corner of the frame.  </param>
    /// <param name="fillColor"> The color to fill the frame with. </param>
    /// <param name="border"> Whether to add borders. The border size is taken from the style. </param>
    /// <param name="rounding"> How strongly to round the frame. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderFrame(Vector2 min, Vector2 max, uint fillColor, bool border = true, float rounding = 0f)
    {
        ImGuiP.RenderFrame(min, max, fillColor, border, rounding);
    }

    /// <summary> Render a general frame. </summary>
    /// <param name="rect"> The rectangle for the frame.  </param>
    /// <param name="fillColor"> The color to fill the frame with. </param>
    /// <param name="border"> Whether to add borders. The border size is taken from the style. </param>
    /// <param name="rounding"> How strongly to round the frame. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderFrame(ImRect rect, uint fillColor, bool border = true, float rounding = 0f)
    {
        ImGuiP.RenderFrame(rect.Min, rect.Max, fillColor, border, rounding);
    }

    /// <summary> Calculate the effective size of something. </summary>
    /// <param name="min"> The minimum size parameter given by the user. If negative, content region + <paramref name="min"/> is used. </param>
    /// <param name="defaultWidth"> The actual width the object should probably have without custom layout. </param>
    /// <param name="defaultHeight">  The actual height the object should probably have without custom layout.  </param>
    /// <returns> The effective size. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 CalcItemSize(Vector2 min, float defaultWidth, float defaultHeight)
    {
        Vector2 result = default;
        ImGuiP.CalcItemSize(ref result, min, defaultWidth, defaultHeight);
        return result;
    }


    /// <inheritdoc cref="RenderTextClipped(ImDrawListPtr,Vector2,Vector2,ReadOnlySpan{char},Vector2,Vector2,ImRect,bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderTextClipped(ImDrawListPtr drawList, Vector2 posMin, Vector2 posMax, ReadOnlySpan<char> text, Vector2 align,
        bool scanText)
    {
        RenderTextClippedExInternal(drawList, posMin, posMax, text, null, align, default, scanText);
    }

    /// <inheritdoc cref="RenderTextClipped(ImDrawListPtr,Vector2,Vector2,ReadOnlySpan{char},Vector2,Vector2,ImRect,bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderTextClipped(ImDrawListPtr drawList, Vector2 posMin, Vector2 posMax, ReadOnlySpan<char> text, Vector2 align,
        Vector2 knownTextSize, bool scanText)
    {
        RenderTextClippedExInternal(drawList, posMin, posMax, text, &knownTextSize, align, default, scanText);
    }

    /// <inheritdoc cref="RenderTextClipped(ImDrawListPtr,Vector2,Vector2,ReadOnlySpan{char},Vector2,Vector2,ImRect,bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderTextClipped(ImDrawListPtr drawList, Vector2 posMin, Vector2 posMax, ReadOnlySpan<char> text, Vector2 align,
        ImRect clipRect,
        bool scanText)
    {
        RenderTextClippedExInternal(drawList, posMin, posMax, text, null, align, clipRect, scanText);
    }

    /// <summary> Render text while clipping parts of it. </summary>
    /// <param name="drawList"> The draw list to render in. </param>
    /// <param name="posMin"> The starting position of the text. </param>
    /// <param name="posMax"> The other corner of the rectangle to render the text in, using <paramref name="align"/> to position it. </param>
    /// <param name="text"> The text to render. </param>
    /// <param name="align"> The alignment of the text inside the rectangle. </param>
    /// <param name="knownTextSize"> The render-size of the text if computed beforehand. </param>
    /// <param name="clipRect"> A separate rectangle to clip the text to, defaulting to [<paramref name="posMin"/>, <paramref name="posMax"/>]. </param>
    /// <param name="scanText"> Whether to scan the text for 0-bytes or label-denoting ## before rendering. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderTextClipped(ImDrawListPtr drawList, Vector2 posMin, Vector2 posMax, ReadOnlySpan<char> text, Vector2 align,
        Vector2 knownTextSize, ImRect clipRect, bool scanText)
    {
        RenderTextClippedExInternal(drawList, posMin, posMax, text, &knownTextSize, align, clipRect, scanText);
    }


    /// <inheritdoc cref="RenderTextClipped(ImDrawListPtr,ImRect,ReadOnlySpan{char},Vector2,Vector2,ImRect,bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderTextClipped(ImDrawListPtr drawList, ImRect box, ReadOnlySpan<char> text, Vector2 align, bool scanText)
    {
        RenderTextClippedExInternal(drawList, box.Min, box.Max, text, null, align, default, scanText);
    }

    /// <inheritdoc cref="RenderTextClipped(ImDrawListPtr,ImRect,ReadOnlySpan{char},Vector2,Vector2,ImRect,bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderTextClipped(ImDrawListPtr drawList, ImRect box, ReadOnlySpan<char> text, Vector2 align,
        Vector2 knownTextSize, bool scanText)
    {
        RenderTextClippedExInternal(drawList, box.Min, box.Max, text, &knownTextSize, align, default, scanText);
    }

    /// <inheritdoc cref="RenderTextClipped(ImDrawListPtr,ImRect,ReadOnlySpan{char},Vector2,Vector2,ImRect,bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderTextClipped(ImDrawListPtr drawList, ImRect box, ReadOnlySpan<char> text, Vector2 align, ImRect clipRect,
        bool scanText)
    {
        RenderTextClippedExInternal(drawList, box.Min, box.Max, text, null, align, clipRect, scanText);
    }

    /// <summary> Render text while clipping parts of it. </summary>
    /// <param name="drawList"> The draw list to render in. </param>
    /// <param name="box"> The rectangle to render the text in, using <paramref name="align"/> to position it. </param>
    /// <param name="text"> The text to render. </param>
    /// <param name="align"> The alignment of the text inside the rectangle. </param>
    /// <param name="knownTextSize"> The render-size of the text if computed beforehand. </param>
    /// <param name="clipRect"> A separate rectangle to clip the text to, defaulting to <paramref name="box"/>. </param>
    /// <param name="scanText"> Whether to scan the text for 0-bytes or label-denoting ## before rendering. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderTextClipped(ImDrawListPtr drawList, ImRect box, ReadOnlySpan<char> text, Vector2 align, Vector2 knownTextSize,
        ImRect clipRect, bool scanText)
    {
        RenderTextClippedExInternal(drawList, box.Min, box.Max, text, &knownTextSize, align, clipRect, scanText);
    }

    /// <summary> Rotate a vector. </summary>
    /// <param name="vec"> The vector to rotate. </param>
    /// <param name="cos"> The cosine of the rotation angle. </param>
    /// <param name="sin"> The sine of the rotation angle. </param>
    /// <returns> The rotated vector. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Rotate(Vector2 vec, float cos, float sin)
    {
        Vector2 ret = default;
        ImGuiP.ImRotate(ref ret, vec, cos, sin);
        return ret;
    }


    [SkipLocalsInit]
    private static void RenderTextClippedExInternal(ImDrawList* drawList, ImVec2 posMin, ImVec2 posMax, ReadOnlySpan<char> text,
        Vector2* textSizeIfKnown, ImVec2 align, ImRect clipRect, bool scanText)
    {
        var (visibleEnd, _, _) = scanText ? StringHelpers.SplitStringWithNull(text) : (text.Length, 0, 0);
        if (visibleEnd == 0)
            return;

        var maxSize  = visibleEnd * 4;
        var bytes    = maxSize > StringHelpers.MaxStackAlloc ? new byte[maxSize] : stackalloc byte[maxSize];
        var numBytes = Encoding.UTF8.GetBytes(text[..visibleEnd], bytes);
        ImGuiP.RenderTextClippedEx(drawList, posMin, posMax, bytes[..numBytes], *textSizeIfKnown, align, clipRect);
    }
}


