using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using ECommons.DalamudServices;
using System.Numerics;

namespace ECommons.ImGuiMethods;

// porting-note(api13): PunishXIV's ECommons carried ImGuiEx.AddHeaderIcon, which the
// a388ee2 anchor (and current ECommons HEAD) no longer has, while Artisan's crafting and
// recipe windows still call it. Gap-filled here rather than dropping the button.
// The fork reached the window flags through its own [LibraryImport("cimgui")]
// igGetCurrentWindow shim; API13's bindings expose ImGuiPNative.GetCurrentWindow()
// natively, so that shim is not carried forward.
public static partial class ImGuiEx
{
    public record HeaderIconOptions
    {
        public Vector2 Offset { get; init; } = Vector2.Zero;
        public ImGuiMouseButton MouseButton { get; init; } = ImGuiMouseButton.Left;
        public string Tooltip { get; init; } = string.Empty;
        public uint Color { get; init; } = 0xFFFFFFFF;
        public bool ToastTooltipOnClick { get; init; } = false;
        public ImGuiMouseButton ToastTooltipOnClickButton { get; init; } = ImGuiMouseButton.Left;
    }

    private static uint headerLastWindowID = 0;
    private static ulong headerLastFrame = 0;
    private static float headerCurrentPos = 0;
    private static float headerImGuiButtonWidth = 0;

    private static unsafe ImGuiWindowFlags CurrentWindowFlags()
        => ImGuiPNative.GetCurrentWindow()->Flags;

    private static unsafe bool CurrentWindowHasClose()
        => ImGuiPNative.GetCurrentWindow()->HasCloseButton != 0;

    public static bool AddHeaderIcon(string id, FontAwesomeIcon icon, HeaderIconOptions? options = null)
    {
        if (ImGui.IsWindowCollapsed()) return false;

        var currentID = ImGui.GetID(0);
        if (currentID != headerLastWindowID || headerLastFrame != Svc.PluginInterface.UiBuilder.FrameCount)
        {
            headerLastWindowID = currentID;
            headerLastFrame = Svc.PluginInterface.UiBuilder.FrameCount;
            headerCurrentPos = 0.25f * ImGui.GetStyle().FramePadding.Length();
            if (!CurrentWindowFlags().HasFlag(ImGuiWindowFlags.NoTitleBar))
                headerCurrentPos = 1;
            headerImGuiButtonWidth = 0f;
            if (CurrentWindowHasClose())
                headerImGuiButtonWidth += 17f.Scale();
            if (!CurrentWindowFlags().HasFlag(ImGuiWindowFlags.NoCollapse))
                headerImGuiButtonWidth += 17f.Scale();
        }

        options ??= new();
        var prevCursorPos = ImGui.GetCursorPos();
        var buttonSize = new Vector2(20f.Scale());
        var buttonPos = new Vector2(
            ImGui.GetWindowWidth() - buttonSize.X - headerImGuiButtonWidth.Scale() * headerCurrentPos
                - ImGui.GetStyle().FramePadding.X.Scale(),
            ImGui.GetScrollY() + 1);
        ImGui.SetCursorPos(buttonPos);
        var drawList = ImGui.GetWindowDrawList();
        drawList.PushClipRectFullScreen();

        var pressed = false;
        ImGui.InvisibleButton(id, buttonSize);
        var itemMin = ImGui.GetItemRectMin();
        var itemMax = ImGui.GetItemRectMax();
        var halfSize = ImGui.GetItemRectSize() / 2;
        var center = itemMin + halfSize;
        if (ImGui.IsWindowHovered() && ImGui.IsMouseHoveringRect(itemMin, itemMax, false))
        {
            if (!string.IsNullOrEmpty(options.Tooltip))
                ImGui.SetTooltip(options.Tooltip);
            ImGui.GetWindowDrawList().AddCircleFilled(center, halfSize.X,
                ImGui.GetColorU32(ImGui.IsMouseDown(ImGuiMouseButton.Left) ? ImGuiCol.ButtonActive : ImGuiCol.ButtonHovered));
            if (ImGui.IsMouseReleased(options.MouseButton))
                pressed = true;
            if (options.ToastTooltipOnClick && ImGui.IsMouseReleased(options.ToastTooltipOnClickButton))
                Notify.Info(options.Tooltip);
        }

        ImGui.SetCursorPos(buttonPos);
        ImGui.PushFont(UiBuilder.IconFont);
        var iconString = icon.ToIconString();
        drawList.AddText(UiBuilder.IconFont, ImGui.GetFontSize(),
            itemMin + halfSize - ImGui.CalcTextSize(iconString) / 2 + options.Offset, options.Color, iconString);
        ImGui.PopFont();

        ImGui.PopClipRect();
        ImGui.SetCursorPos(prevCursorPos);

        return pressed;
    }
}
